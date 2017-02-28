using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Mono.CompilerServices.SymbolWriter;
using Mono.Options;
using Mono.Unix;

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
		if (!path.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
			return path + Path.DirectorySeparatorChar;
		return path;
	}

	public static int Main (string[] arguments)
	{
		bool link = false;
		string monopath = null;
		string opentkpath = null;
		string xamarinpath = null;
		string installDir = null;
		bool verbose = false;

		var os = new OptionSet () {
			{ "link:", "If source files should be linked instead of copied. Makes the install process faster, and if you edit files when stopped in a debugger, you'll edit the right file (and not a copy).", v => link = ParseBool (v) },
			{ "mono-path=", "The path of the mono checkout.", v => monopath = v },
			{ "opentk-path=", "The path of the opentk checkout.", v => opentkpath = v},
			{ "xamarin-path=", "The path of the xamarin source.", v => xamarinpath = v },
			{ "install-dir=", "The directory to install into. The files will be put into a src subdirectory of this directory.", v => installDir = v },
			{ "v|erbose", "Enable verbose output", v => verbose = true },
		};

		var mdb_files = os.Parse (arguments); 

		var srcs = new HashSet<string> (StringComparer.OrdinalIgnoreCase);

		monopath = FixPathEnding (monopath);
		xamarinpath = FixPathEnding (xamarinpath);
		opentkpath = FixPathEnding (opentkpath);

		var manglerFactory = new PathManglerFactory {
			InstallDir = installDir,
			MonoSourcePath = monopath,
			XamarinSourcePath = xamarinpath,
			FrameworkPath = (installDir.Contains ("Xamarin.iOS.framework")) ? "Xamarin.iOS.framework" : "Xamarin.Mac.framework",
			OpenTKSourcePath = opentkpath,
		};

		foreach (string mdb_file in mdb_files) {
			if (!File.Exists (mdb_file)) {
				Console.WriteLine ("File does not exist: {0}", mdb_file);
				continue;
			}

			MonoSymbolFile symfile;

			try {
				symfile = MonoSymbolFile.ReadSymbolFile (mdb_file);
			} catch (IOException ioe) {
				Console.WriteLine ("IO error while reading msb file '{0}': {1}", mdb_file, ioe.Message);
				continue;
			}

			srcs.UnionWith (from src in symfile.Sources select src.FileName);
		}

		var alreadyLinked = new List<string> ();
		foreach (var src in srcs) {
			var mangler = manglerFactory.GetMangler (src);
			var fixedSource = mangler.GetSourcePath (src);

			if (String.IsNullOrEmpty (fixedSource)) { 
				Console.WriteLine ($"Skip path {src}");
				continue;
			}

			var target = mangler.GetTargetPath (fixedSource);

			var targetDir = Path.GetDirectoryName (target);

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
					Console.WriteLine ($"Xamarin path is {xamarinpath}");
					Console.WriteLine ("cp {0} {1}", fixedSource, target);
				}
				try {
					File.Copy (fixedSource, target);
				} catch (PathTooLongException e) {
					Console.WriteLine ("The file {0} could not be copied to {1} because the file path is too long: {2}", fixedSource, target, e);
					return 1;
				}
			} // else 
		} // foreach

		return 0;
	}
}
