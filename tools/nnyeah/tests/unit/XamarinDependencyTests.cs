using System.Threading.Tasks;
using System.IO;
using NUnit.Framework;
using Mono.Cecil;
using Microsoft.MaciOS.Nnyeah;
using Xamarin;

namespace Microsoft.MaciOS.Nnyeah.Tests {
	[TestFixture]
	public class XamarinDependencyTests {
		[Test]
		public async Task HasReferenceTest ()
		{
			var dir = Cache.CreateTemporaryDirectory ("HasReferenceTest");
			var code = @"
using System;
public class Foo {
	public nint Ident (nint e) => e;
}
";
			await TestRunning.BuildLibrary (code, "NoName", dir);
			var expectedOutputFile = Path.Combine (dir, "NoName.dll");

			var module = ModuleDefinition.ReadModule (expectedOutputFile);
			var platform = module.XamarinPlatformName ();
			Assert.AreEqual (Microsoft.MaciOS.Nnyeah.PlatformName.macOS, platform);
		}

		[Test]
		public async Task DoesntHaveReferenceTest ()
		{
			var dir = Cache.CreateTemporaryDirectory ("DoesntHaveReferenceTest");
			var code = @"
using System;
public class Foo {
	public char Ident (char e) => e;
}
";
			await TestRunning.BuildLibrary (code, "NoName", dir);
			var expectedOutputFile = Path.Combine (dir, "NoName.dll");

			var module = ModuleDefinition.ReadModule (expectedOutputFile);
			var platform = module.XamarinPlatformName ();
			Assert.AreEqual (Microsoft.MaciOS.Nnyeah.PlatformName.None, platform);
		}
	}
}
