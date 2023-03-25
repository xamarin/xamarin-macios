#nullable enable

namespace Xamarin.Tests {
	[Category ("Windows")]
	public class WindowsTest : TestBaseClass {
		[Test]
		public void OnlyOnWindows ()
		{
			Assert.True (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform (System.Runtime.InteropServices.OSPlatform.Windows), "On Windows");
		}

		[Test]
		[TestCase (ApplePlatform.iOS, "ios-arm64")]
		public void ConfigurationTestWhileDebugging (ApplePlatform platform, string runtimeIdentifiers)
		{
			Configuration.IgnoreIfIgnoredPlatform (platform);
		}
	}
}
