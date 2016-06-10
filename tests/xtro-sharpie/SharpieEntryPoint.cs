using System;
using System.Collections.Generic;
using System.IO;

using Sharpie;
using Sharpie.Tooling;

namespace Extrospection {

	[Tool ("extro", "Run extrospection tests", 500)]
	public class SharpieEntryPoint : CommonTool {

		protected override void Run ()
		{
			// ensure Mono.Cecil.dll will be in the directory where this is built
			typeof (Mono.Cecil.AssemblyDefinition).ToString ();

			base.Run ();

			var pch = PositionalArguments.DequeueOrDefault ();
			if (pch == null || !File.Exists (pch))
				throw new ExitException ("Precompiled header file (pch) must be specified");

			var assemblies = new List<string> ();
			while (PositionalArguments.Count > 0) {
				var dll = PositionalArguments.DequeueOrDefault ();
				if (dll == null || !File.Exists (dll))
					throw new ExitException ("Assembly file (dll) must be specified");

				assemblies.Add (dll);
			}
			new Runner ().Execute (pch, assemblies);
		}
	}
}