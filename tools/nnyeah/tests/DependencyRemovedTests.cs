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
		[Test]
		[Ignore ("doesn't remove dependency")]
		public async Task BasicDependencyRemoved ()
		{
			var dir = Cache.CreateTemporaryDirectory ("DependencyRemoved");
			var code = @"
using System;
public class Foo {
	public nint Ident (nint e) => e;
}
";
			var output = await TestRunning.BuildLibrary (code, "NoName", dir);
			var expectedOutputFile = Path.Combine (dir, "NoName.dll");
			var targetRewrite = Path.Combine (dir, "NoNameRemoved.dll");

			Assert.DoesNotThrow (() => {
				Program.ProcessAssembly (Compiler.XamarinPlatformLibraryPath (PlatformName.macOS),
					Compiler.MicrosoftPlatformLibraryPath (PlatformName.macOS), expectedOutputFile,
					targetRewrite, verbose: false, forceOverwrite: true, suppressWarnings: true);
			}, "Failed to process assembly");

			Assert.IsTrue (File.Exists (targetRewrite), "target file not created");
			var module = ModuleDefinition.ReadModule (targetRewrite);

			var platform = module.XamarinPlatformName ();
			Assert.AreEqual (Microsoft.MaciOS.Nnyeah.PlatformName.None, platform);
		}
	}
}
