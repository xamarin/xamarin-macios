using System;
using System.IO;
using NUnit.Framework;

namespace Microsoft.MaciOS.Nnyeah.Tests.SmokeTests {
	[TestFixture]
	public class CompileALibrary {
		[Test]
		public void BasicLibrary ()
		{
			using var provider = new DisposableTempDirectory ();
			var code = @"
using System;
public class Foo {
	public nint Ident (nint e) => e;
}
";
			var output = TestRunning.BuildLibrary (code, "NoName", provider.DirectoryPath);
			var expectedOutputFile = Path.Combine (provider.DirectoryPath, "NoName.dll");
			Assert.IsTrue (File.Exists (expectedOutputFile));
		}

		[Test]
		public void BasicExecutable ()
		{
			using var provider = new DisposableTempDirectory ();
		}
	}
}
