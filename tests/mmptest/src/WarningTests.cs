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
			var oldXcode = Xamarin.Tests.Configuration.xcode83_root;

			if (!Directory.Exists (oldXcode))
				Assert.Ignore ("This test requires Xcode 8.3 (or updated to a newer one that still warns MM0135).");

			MMPTests.RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir);
				var output = TI.TestUnifiedExecutable (test, environment: new string[] { "MD_APPLE_SDK_ROOT", Path.GetDirectoryName (Path.GetDirectoryName (oldXcode)) });
				output.Messages.AssertWarningPattern (135, $"Did not link system framework");
			});
		}
	}
}
