using System;
using System.Collections.Generic;
using System.IO;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Xamarin.Bundler;

namespace MonoTouch.Tuner {

	static class Stripper {

		public static void Process (string assembly_file, string output_file)
		{
			try {
				// FIXME: disabled until fixed or replaced
				// File.Copy (assembly_file, Path.ChangeExtension (assembly_file, "unstripped" + Path.GetExtension (assembly_file)));
				// AssemblyStripper.Process (assembly_file);

				// mono-cil-strip is part of the mono SDK and embeds the old cecil source code so it can strip easily
				// 6.3.x requires mono 3.0.x to be installed so the path will be valid
				// using /usr/bin/mono-cil-strip is not always safe, e.g. bug #12459

				Driver.Log (1, "Stripping assembly {0} to {1}", assembly_file, output_file);

				string tool = "/Library/Frameworks/Mono.framework/Versions/Current/lib/mono/4.5/mono-cil-strip.exe";
				string cmd = tool + " " + Driver.Quote (assembly_file) + " " + Driver.Quote (output_file);
				if (Driver.RunCommand ("/Library/Frameworks/Mono.framework/Versions/Current/bin/mono", cmd) != 0)
					throw new MonoTouchException (6002, true, "Could not strip assembly `{0}`.", assembly_file);
			} catch (NotSupportedException e) {
				throw new MonoTouchException (6001, true, e.Message);
			} catch (UnauthorizedAccessException e) {
				// access denied, e.g. non-writable (by SCM) assembly - see assistly #10923 
				throw new MonoTouchException (6003, true, e.Message);
			} catch (Exception e) {
				throw new MonoTouchException (6002, true, e, "Could not strip assembly `{0}`.", assembly_file);
			}
		}
	}
}