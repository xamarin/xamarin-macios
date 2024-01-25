using System;
using System.IO;
using System.Threading.Tasks;

using NUnit.Framework;

using Xamarin;

namespace Microsoft.MaciOS.Nnyeah.Tests.SmokeTests {
	[TestFixture]
	public class CompileALibrary {
		[Test]
		public async Task BasicLibrary ()
		{
			await TestRunning.BuildTemporaryLibrary (@"
using System;
public class Foo {
	public nint Ident (nint e) => e;
}");
		}

		[Test]
		public void BasicExecutable ()
		{
			var dir = Cache.CreateTemporaryDirectory ("BasicExecutable");
		}

		[Test]
		public async Task LibraryWithXamarinReference ()
		{
			var dir = Cache.CreateTemporaryDirectory ("LibraryWithXamarinReference");
			var code = @"
using System;
using Foundation;

public class Foo {
	public bool IsStaleHandle (NSObject o) => o.Handle != IntPtr.Zero;
}
";
			await TestRunning.BuildLibrary (code, "NoName", dir);
			var libraryFile = Path.Combine (dir, "NoName.dll");
			Assert.IsTrue (File.Exists (libraryFile));

			var convertedFile = Path.Combine (dir, "NoName-Converted.dll");
			AssemblyConverter.Convert (Compiler.XamarinPlatformLibraryPath (PlatformName.macOS), Compiler.MicrosoftPlatformLibraryPath (PlatformName.macOS), libraryFile, convertedFile, true, true, false);
		}

		[Test]
		public void HasXamarinMacOSFile ()
		{
			var xamarinDll = Compiler.XamarinPlatformLibraryPath (PlatformName.macOS);
			Assert.IsTrue (File.Exists (xamarinDll), "Xamarin file doesn't exist");
		}

		[Test]
		public void HasMicrosoftMacOSFile ()
		{
			var microsoftDll = Compiler.MicrosoftPlatformLibraryPath (PlatformName.macOS);
			Assert.IsTrue (File.Exists (microsoftDll));
		}
	}
}
