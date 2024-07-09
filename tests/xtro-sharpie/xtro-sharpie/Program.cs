using System;
using System.Collections.Generic;

using Mono.Options;

namespace Extrospection {

	// this is used to be executed from a custom 64bits mono
	class MainClass {

		static int Main (string [] arguments)
		{
			var outputDirectory = string.Empty;
			var searchDirectories = new List<string> ();
			var options = new OptionSet {
				{ "output-directory=", (v) => outputDirectory = v },
				{ "lib=", (v) => searchDirectories.Add (v) },
			};
			var args = options.Parse (arguments);

			if (args.Count < 2) {
				Console.Error.WriteLine ("Usage: xtro-sharpie.exe [--output-directory=<output directory>] [--lib=<assembly search directory>] pch-file dll-file [dll2-file]");
				return 1;
			}

			try {
				var assemblies = new List<string> ();
				for (int i = 1; i < args.Count; i++)
					assemblies.Add (args [i]);
				new Runner ().Execute (args [0], assemblies, outputDirectory, searchDirectories);
				return 0;
			} catch (Exception e) {
				Console.WriteLine (e);
				return 1;
			}
		}
	}
}
