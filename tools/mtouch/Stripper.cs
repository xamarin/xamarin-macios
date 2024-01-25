using System;
using System.Collections.Generic;
using System.IO;

using Mono.Cecil;
using Mono.Cecil.Cil;

using Xamarin.Bundler;
using Xamarin.Utils;

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
				if (Driver.RunCommand ("/Library/Frameworks/Mono.framework/Versions/Current/bin/mono", tool, assembly_file, output_file) != 0)
					throw new ProductException (6002, true, "Could not strip assembly `{0}`.", assembly_file);
			} catch (NotSupportedException e) {
				throw new ProductException (6001, true, e.Message);
			} catch (UnauthorizedAccessException e) {
				// access denied, e.g. non-writable (by SCM) assembly - see assistly #10923 
				throw new ProductException (6003, true, e.Message);
			} catch (Exception e) {
				throw new ProductException (6002, true, e, "Could not strip assembly `{0}`.", assembly_file);
			}
		}
	}
}
