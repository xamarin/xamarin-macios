using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Mono.CompilerServices.SymbolWriter;
using Mono.Options;
using Mono.Unix;

using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Pdb;


using InstallSources;

public class ListSourceFiles {
	static bool ParseBool (string value)
	{
		if (string.IsNullOrEmpty (value))
			return false;

		switch (value.ToLowerInvariant ()) {
		case "1":
		case "true":
		case "yes":
			return true;
		case "0":
		case "false":
		case "no":
			return false;
		default:
			return bool.Parse (value);
		}
	}

	public static string FixPathEnding(string path)
	{
		if (!path.EndsWith (Path.DirectorySeparatorChar.ToString (), StringComparison.Ordinal))
			return path + Path.DirectorySeparatorChar;
		return path;
	}

	// returns a tuple with the pdb files as the first value and the mdb files as the second
	public static Tuple <HashSet<String>, HashSet<String>> GetSplittedPaths (List<string> paths)
	{
		var splittedPaths = new Tuple<HashSet<String>, HashSet<String>> (new HashSet<string> (), new HashSet<string> ());
		foreach (var p in paths) {
			if (p.EndsWith (".pdb", StringComparison.Ordinal)) {
				splittedPaths.Item1.Add (p);
				continue;
			}
			if (p.EndsWith (".mdb", StringComparison.Ordinal)) {
				splittedPaths.Item2.Add (p);
				continue;
			}
		}
		return splittedPaths;
	}

	// returns the source paths used to create an assembly that has an mdb file
	public static HashSet<String> GetFilePathsFromMdb (string mdb_file)
	{
		var srcs = new HashSet<String> ();
		MonoSymbolFile symfile;

		try {
			symfile = MonoSymbolFile.ReadSymbolFile(mdb_file);
			srcs.UnionWith (from src in symfile.Sources select src.FileName);

		} catch (IOException ioe) {
			Console.WriteLine("IO error while reading msb file '{0}': {1}", mdb_file, ioe.Message);
		}

		return srcs;
	}

	// returns the source paths used to create an assembly that has a pdb file
	public static HashSet<String> GetFilePathsFromPdb (string pdbFile)
	{
		var pdb = Path.ChangeExtension (pdbFile, ".dll");

		var result = new HashSet<String> (StringComparer.OrdinalIgnoreCase);

		if (!File.Exists (pdbFile))
			return result;

		var assemblyResolver = new DefaultAssemblyResolver ();
		var assemblyLocation = Path.GetDirectoryName (pdb);
		assemblyResolver.AddSearchDirectory (assemblyLocation);

		var readerParameters = new ReaderParameters { AssemblyResolver = assemblyResolver };
		var writerParameters = new WriterParameters ();

		var symbolReaderProvider = new PortablePdbReaderProvider ();
		readerParameters.SymbolReaderProvider = symbolReaderProvider;
		readerParameters.ReadSymbols = true;

		var assemblyDefinition = AssemblyDefinition.ReadAssembly (pdb, readerParameters);
		var mainModule = assemblyDefinition.MainModule;
		foreach (var type in mainModule.Types) {
			foreach (var method in type.Methods) {
				if (method.DebugInformation.SequencePoints.Any ()) {
					var sequence_point = method.DebugInformation.SequencePoints[0];
					var document = sequence_point.Document;
					result.Add (document.Url);
				}
			}
		}
		return result;
	}

	public static int Main (string[] arguments)
	{
		bool link = false;
		string monopath = null;
		string opentkpath = null;
		string xamarinpath = null;
		string installDir = null;
		string destinationDir = null;
		bool verbose = false;

		var os = new OptionSet () {
			{ "link:", "If source files should be linked instead of copied. Makes the install process faster, and if you edit files when stopped in a debugger, you'll edit the right file (and not a copy).", v => link = ParseBool (v) },
			{ "mono-path=", "The path of the mono checkout.", v => monopath = v },
			{ "opentk-path=", "The path of the opentk checkout.", v => opentkpath = v},
			{ "xamarin-path=", "The path of the xamarin source.", v => xamarinpath = v },
			{ "install-dir=", "The directory to install into. The files will be put into a src subdirectory of this directory.", v => installDir = v },
			{ "destination-dir=", "The path to the directory used for the -pathmap at build time.", v => destinationDir = v},
			{ "v|erbose", "Enable verbose output", v => verbose = true },
		};

		var paths = os.Parse (arguments);
		var files = GetSplittedPaths (paths);

		var srcs = new HashSet<string> (StringComparer.OrdinalIgnoreCase);

		monopath = FixPathEnding (monopath);
		xamarinpath = FixPathEnding (xamarinpath);
		opentkpath = FixPathEnding (opentkpath);

		var manglerFactory = new PathManglerFactory {
			Verbose = verbose,
			InstallDir = installDir,
			DestinationDir = destinationDir,
			MonoSourcePath = monopath,
			XamarinSourcePath = xamarinpath,
			FrameworkPath = (installDir.Contains ("Xamarin.iOS.framework")) ? "Xamarin.iOS.framework" : "Xamarin.Mac.framework",
			OpenTKSourcePath = opentkpath,
		};

 		// add the paths from the pdb files
		foreach (string pdbFile in files.Item1) {
			if (!File.Exists (pdbFile)) {
				Console.WriteLine ("File does not exist: {0}", pdbFile);
				continue;
			}

			var assemblySrcs = GetFilePathsFromPdb(pdbFile);
			if (verbose) {
				Console.WriteLine("Pdb file sources are:");
				foreach (var p in srcs)
					Console.WriteLine($"\t{p}");
			}
			srcs.UnionWith(assemblySrcs);
		}

		// add the paths from the mdb files
		foreach (string mdbFile in files.Item2) {
			if (!File.Exists (mdbFile)) {
				Console.WriteLine ("File does not exist: {0}", mdbFile);
				continue;
			}
			var assemblySrcs = GetFilePathsFromMdb (mdbFile);
			if (verbose) {
				Console.WriteLine("Mdb file sources are:");
				foreach (var p in srcs)
					Console.WriteLine($"\t{p}");
			}
			srcs.UnionWith (assemblySrcs);
		}

		if (verbose) {
			Console.WriteLine ("Build paths are:");
			Console.WriteLine ($"\tMono path:{monopath}");
			Console.WriteLine ($"\tXamarin path:{xamarinpath}");
			Console.WriteLine ($"\tOpenTk path:{opentkpath}");
			Console.WriteLine ($"\tDestination path:{destinationDir}");
		}
		var alreadyLinked = new List<string> ();
		foreach (var src in srcs) {
			var mangler = manglerFactory.GetMangler (src);
			if (mangler == null) { // we are ignoring this file
				if (verbose)
					Console.WriteLine ($"Ignoring path {src}");
				continue;
			}
			if (verbose)
				Console.WriteLine ($"Original source is {src}");
			var fixedSource = mangler.GetSourcePath (src);

			if (String.IsNullOrEmpty (fixedSource)) { 
				Console.WriteLine ($"Skip path {src}");
				continue;
			}
			var target = mangler.GetTargetPath (fixedSource);

			if (verbose) {
				Console.WriteLine ($"Fixed source {fixedSource}");
				Console.WriteLine ($"Target path {target}");
			}

			var targetDir = Path.GetDirectoryName (target);

			if (String.IsNullOrEmpty (targetDir)) {
				Console.WriteLine ($"Got empty dir for {target}");
				continue;
			}
			
			if (verbose)
				Console.WriteLine ($"Target direcotry is {targetDir}");

			if (!Directory.Exists (targetDir)) {
				try {
					if (verbose)
						Console.WriteLine ($"Creating dir {targetDir}");
					Directory.CreateDirectory(targetDir);
				} catch (PathTooLongException e) {
					Console.WriteLine("Could not create directory {0} because the path is too long: {1}", targetDir, e);
					return 1;
				}
			} else if (File.Exists (target)) {
				try {
					File.Delete(target);
				} catch (PathTooLongException e) {
					Console.WriteLine("Could not delete file {0} because the path is too long: {1}", target, e);
					return 1;
				}
			} // else 

			if (link) {
				if (verbose)
					Console.WriteLine ("ln -s {0} {1}", fixedSource, target);
				try {
					if (!alreadyLinked.Contains (fixedSource)) {
						new UnixFileInfo (fixedSource).CreateSymbolicLink (target);
						alreadyLinked.Add(fixedSource);
					} else {
						Console.WriteLine ("Src {0} was already linked.", src);
					}
				} catch (PathTooLongException e) {
					Console.WriteLine("Could not link {0} to {1} because the path is too long: {2}", fixedSource, target, e);
					return 1;
				} catch (UnixIOException e) {
					Console.WriteLine("Could not link {0} to {1}: {2}", src, target, e);
					return 1;
				} // try/catch
			} else {
				if (verbose) {
					Console.WriteLine ("cp {0} {1}", fixedSource, target);
				}
				try {
					File.Copy (fixedSource, target);
				} catch (FileNotFoundException e) { 
					Console.WriteLine ("The file {0} could not be copied to {1} because the file does not exists: {2}", fixedSource, target, e);
					Console.WriteLine ("Debugging info:");
					Console.WriteLine ("\tSource is {0}", src);
					Console.WriteLine ("\tFixed source is {0}", fixedSource);
					Console.WriteLine ("\tPath mangler type is {0}", mangler.GetType().Name);
				} catch (PathTooLongException e) {
					Console.WriteLine ("The file {0} could not be copied to {1} because the file path is too long: {2}", fixedSource, target, e);
					return 1;
				}
			} // else 
		} // foreach

		return 0;
	}
}
