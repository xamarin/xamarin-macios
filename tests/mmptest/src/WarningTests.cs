using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace Xamarin.MMP.Tests {
	[TestFixture]
	public class WarningTests {
		[Test]
		public void MM0135 ()
		{
			var oldXcode = Xamarin.Tests.Configuration.xcode94_root;

			if (!Directory.Exists (oldXcode))
				Assert.Ignore ("This test requires Xcode 9.4 (or updated to a newer one that still warns MM0135).");

			if (PlatformHelpers.CheckSystemVersion (10, 15))
				Assert.Ignore ("This test requires Xcode 9.4, which doesn't work on macOS 10.15+");

			MMPTests.RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir);
				var output = TI.TestUnifiedExecutable (test, environment: new Dictionary<string, string> { { "MD_APPLE_SDK_ROOT", Path.GetDirectoryName (Path.GetDirectoryName (oldXcode)) } });
				output.Messages.AssertWarningPattern (135, $"Did not link system framework");
			});
		}
	}
}
