using System;
using System.IO;

using Foundation;

namespace MyStaticRegistrarReferencesApp
{
	static class MainClass
	{
		static int Main (string[] args)
		{
			var assemblyDir = Path.GetDirectoryName (typeof (NSObject).Assembly.Location);
			var bindingsAssembly = Path.Combine (assemblyDir, "bindings-test.dll");
			if (!File.Exists (bindingsAssembly)) {
				Console.WriteLine ($"The bindings library {bindingsAssembly} does not exist.");
				return 1;
			}
			Console.WriteLine ($"Found the bindings library {bindingsAssembly}");

			Console.WriteLine (Environment.GetEnvironmentVariable ("MAGIC_WORD"));

			return 0;
		}
	}
}
