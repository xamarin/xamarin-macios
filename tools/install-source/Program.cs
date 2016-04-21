using System;
using System.Collections.Generic;
using System.IO;

using Mono.CompilerServices.SymbolWriter;
using Mono.Options;
using Mono.Unix;

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
	public static void Main (string[] arguments)
	{
		bool link = false;
		string monopath = null;
		string installDir = null;
		bool verbose = false;

		var os = new OptionSet () {
			{ "link:", "If source files should be linked instead of copied. Makes the install process faster, and if you edit files when stopped in a debugger, you'll edit the right file (and not a copy).", v => link = ParseBool (v) },
			{ "mono-path=", "The path of the mono checkout.", v => monopath = v },
			{ "install-dir=", "The directory to install into. The files will be put into a src subdirectory of this directory.", v => installDir = v },
			{ "v|erbose", "Enable verbose output", v => verbose = true },
		};

		var mdb_files = os.Parse (arguments); 

		var srcs = new HashSet<string> (StringComparer.OrdinalIgnoreCase);

		if (!monopath.EndsWith (Path.DirectorySeparatorChar.ToString ()))
			monopath += Path.DirectorySeparatorChar;

		foreach (string mdb_file in mdb_files) {
			if (!File.Exists (mdb_file)) {
				Console.WriteLine ("File does not exist: {0}", mdb_file);
				continue;
			}

			if (mdb_file.EndsWith ("monotouch.dll.mdb")) {
				// don't include monotouch.dll source
				continue;
			} else if (mdb_file.EndsWith ("Xamarin.iOS.dll.mdb")) {
				// same for Xamarin.iOS.dll
				continue;
			} else if (mdb_file.EndsWith ("XamMac.dll.mdb")) {
				continue;
			} else if (mdb_file.EndsWith ("Xamarin.Mac.dll.mdb")) {
				continue;
			}

			MonoSymbolFile symfile;

			try {
				symfile = MonoSymbolFile.ReadSymbolFile (mdb_file);
			} catch (IOException ioe) {
				Console.WriteLine ("IO error while reading msb file '{0}': {1}", mdb_file, ioe.Message);
				continue;
			}

			foreach (var source_file in symfile.Sources) {
				var src = source_file.FileName;

				if (!src.StartsWith (monopath)) {
					if (verbose)
						Console.WriteLine ("{0}: not a mono source file", src);
					continue;
				}

				srcs.Add (src);
			}
		}

		foreach (var src in srcs) {
			var relativePath = src.Substring (monopath.Length);
			var target = Path.Combine (installDir, "src", "mono", relativePath);
			var targetDir = Path.GetDirectoryName (target);

			if (!Directory.Exists (targetDir)) {
				Directory.CreateDirectory (targetDir);
			} else if (File.Exists (target)) {
				File.Delete (target);
			}

			if (link) {
				if (verbose)
					Console.WriteLine ("ln -s {0} {1}", src, target);
				new UnixFileInfo (src).CreateSymbolicLink (target);
			} else {
				if (verbose)
					Console.WriteLine ("cp {0} {1}", src, target);
				File.Copy (src, target);
			}
		}
	}
}