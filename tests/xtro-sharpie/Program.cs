using System;
using System.Collections.Generic;

namespace Extrospection {

	// this is used to be executed from a custom 64bits mono
	class MainClass	{
		
		static int Main (string [] args)
		{
			if (args.Length < 2) {
				Console.Error.WriteLine ("Usage: mono64 xtro-sharpie.exe pch-file dll-file [dll2-file]");
				return 1;
			}

			try {
				var assemblies = new List<string> ();
				for (int i = 1; i < args.Length; i++)
					assemblies.Add (args [i]);
				new Runner ().Execute (args [0], assemblies);
				return 0;
			}
			catch (Exception e) {
				Console.WriteLine (e);
				return 1;
			}
		}
	}
}