using System;
using System.IO;
using System.Reflection;

using Foundation;
using ObjCRuntime;

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

			var assembly = Assembly.LoadFrom (bindingsAssembly);

#if __MACOS__
			AppKit.NSApplication.Init ();
#endif

			var type = assembly.GetType ("Bindings.Test.UltimateMachine");
			var instance = Activator.CreateInstance (type);
			var getter = type.GetMethod ("get_SharedInstance");
			var method = type.GetMethod ("GetAnswer");
			var sharedInstance = getter.Invoke (instance, new object [0]);
			var answer = method.Invoke (sharedInstance, new object [0]);

			if ((int) answer != 42) {
				Console.WriteLine ($"Expected BindingsTest.Test.UltimateMachine.SharedInstance.GetAnswer to return 42, but it returned {answer}");
				return 1;
			}
			Console.WriteLine ($"Got the answer: {answer}");

			Console.WriteLine (Environment.GetEnvironmentVariable ("MAGIC_WORD"));

			return 0;
		}
	}
}
