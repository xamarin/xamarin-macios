using System;
using System.Threading.Tasks;
using System.IO;
using NUnit.Framework;
using Mono.Cecil;
using Microsoft.MaciOS.Nnyeah;
using Xamarin;


namespace Microsoft.MaciOS.Nnyeah.Tests {
	[TestFixture]
	public class DependencyRemovedTests {
		[TestCase ("nint")]
		[TestCase ("nuint")]
// nfloat has an issue on write.
//		[TestCase ("nfloat")]
		public async Task BasicDependencyRemoved (string type)
		{
			var dir = Cache.CreateTemporaryDirectory ($"DependencyRemoved_{type}");
			var code = @$"
using System;
public class Foo {{
	public {type} Ident ({type} e) => e;
}}
";
			var output = await TestRunning.BuildLibrary (code, "NoName", dir);
			var expectedOutputFile = Path.Combine (dir, "NoName.dll");
			var targetRewrite = Path.Combine (dir, "NoNameRemoved.dll");

			Assert.DoesNotThrow (() => {
				Program.ProcessAssembly (Compiler.XamarinPlatformLibraryPath (PlatformName.macOS),
					Compiler.MicrosoftPlatformLibraryPath (PlatformName.macOS), expectedOutputFile,
					targetRewrite, verbose: false, forceOverwrite: true, suppressWarnings: true);
			}, $"Failed to process assembly for type {type}");

			Assert.IsTrue (File.Exists (targetRewrite), $"target file not created for type {type}");
			var module = ModuleDefinition.ReadModule (targetRewrite);

			var platform = module.XamarinPlatformName ();
			Assert.AreEqual (Microsoft.MaciOS.Nnyeah.PlatformName.None, platform);
		}
	}
}
