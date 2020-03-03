using System;
using System.IO;
using NUnit.Framework;

namespace Xamarin.MMP.Tests
{
	[TestFixture]
	public class WarningTests
	{
		[Test]
		public void MM0135 ()
		{
			var oldXcode = Xamarin.Tests.Configuration.xcode94_root;

			if (!Directory.Exists (oldXcode))
				Assert.Ignore ("This test requires Xcode 9.4 (or updated to a newer one that still warns MM0135).");
			else if (Environment.OSVersion.Version.Major >= 19 /* macOS 10.15+ */)
				Assert.Ignore ("Xcode 9.4 does not work on Catalina or later."); // This can check can be removed after switching to a newer Xcode than 9.4.

			if (PlatformHelpers.CheckSystemVersion (10, 15))
				Assert.Ignore ("This test requires Xcode 9.4, which doesn't work on macOS 10.15+");

			MMPTests.RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir);
				var output = TI.TestUnifiedExecutable (test, environment: new string[] { "MD_APPLE_SDK_ROOT", Path.GetDirectoryName (Path.GetDirectoryName (oldXcode)) });
				output.Messages.AssertWarningPattern (135, $"Did not link system framework");
			});
		}
	}
}
