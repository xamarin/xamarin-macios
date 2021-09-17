using System;
using System.IO;

using Mono.Cecil;

namespace fixcatalystsystemnethttp {
	class MainClass {
		public static int Main (string [] args)
		{
			var input = args [0];
			var output = args [1];
			return Fix (input, output);
		}

		static int Fix (string input, string output)
		{
			var rp = new ReaderParameters { ReadSymbols = true };
			var resolver = new DefaultAssemblyResolver ();
			resolver.AddSearchDirectory (Path.GetDirectoryName (input));
			rp.AssemblyResolver = resolver;

			var ad = AssemblyDefinition.ReadAssembly (input, rp);
			foreach (var ca in ad.CustomAttributes) {
				if (ca.AttributeType.Name != "InternalsVisibleToAttribute")
					continue;

				var arg = (string) ca.ConstructorArguments [0].Value;
				if (!arg.StartsWith ("Xamarin.iOS", StringComparison.Ordinal))
					continue;
				arg = "Xamarin.MacCatalyst" + arg.Substring ("Xamarin.iOS".Length);
				ca.ConstructorArguments [0] = new CustomAttributeArgument (ca.ConstructorArguments [0].Type, arg);
				Console.WriteLine ("Changed InternalsVisibleToAttribute to point to Xamarin.MacCatalyst instead of Xamarin.iOS.");
			}
			foreach (var ar in ad.MainModule.AssemblyReferences) {
				if (ar.Name == "Xamarin.iOS") {
					ar.Name = "Xamarin.MacCatalyst";
					Console.WriteLine ("Changed assembly reference to point to Xamarin.MacCatalyst instead of Xamarin.iOS.");
				}
			}

			ad.Write (output, new WriterParameters { WriteSymbols = true });
			Console.WriteLine ($"Fixed assembly written to {output}");
			return 0;
		}
	}
}
