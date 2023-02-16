#nullable enable
using NUnit.Framework;
using ObjCRuntime;
using Xamarin.Utils;

namespace GeneratorTests {

	[TestFixture]
	public class PlatformNameExtensions {

		[TestCase (PlatformName.iOS, "UIApplication")]
		[TestCase (PlatformName.WatchOS, "UIApplication")]
		[TestCase (PlatformName.TvOS, "UIApplication")]
		[TestCase (PlatformName.MacCatalyst, "UIApplication")]
		[TestCase (PlatformName.MacOSX, "NSApplication")]
		public void GetApplicationClassNameTest (PlatformName platformName, string expected)
			=> Assert.AreEqual (expected, platformName.GetApplicationClassName ());

		[TestCase (PlatformName.MacOSX, 2)]
		[TestCase (PlatformName.iOS, 2)]
		[TestCase (PlatformName.TvOS, 3)]
		[TestCase (PlatformName.WatchOS, 3)]
		public void GetXamcoreVersionTest (PlatformName platformName, int expected)
#if NET
			=> Assert.AreEqual (4, platformName.GetXamcoreVersion ());
#else
			=> Assert.AreEqual (expected, platformName.GetXamcoreVersion ());
#endif

		[TestCase (PlatformName.iOS, "CoreImage")]
		[TestCase (PlatformName.WatchOS, "CoreImage")]
		[TestCase (PlatformName.TvOS, "CoreImage")]
		[TestCase (PlatformName.MacCatalyst, "CoreImage")]
		[TestCase (PlatformName.MacOSX, "Quartz")]
		public void GetCoreImageMapTest (PlatformName platformName, string expected)
			=> Assert.AreEqual (expected, platformName.GetCoreImageMap ());

		[TestCase (PlatformName.iOS, "MobileCoreServices")]
		[TestCase (PlatformName.WatchOS, "MobileCoreServices")]
		[TestCase (PlatformName.TvOS, "MobileCoreServices")]
		[TestCase (PlatformName.MacCatalyst, "MobileCoreServices")]
		[TestCase (PlatformName.MacOSX, "CoreServices")]
		public void GetCoreServicesMap (PlatformName platformName, string expected)
			=> Assert.AreEqual (expected, platformName.GetCoreServicesMap ());

		[TestCase (PlatformName.iOS, "PDFKit")]
		[TestCase (PlatformName.MacCatalyst, "PDFKit")]
		[TestCase (PlatformName.MacOSX, "Quartz")]
		public void GetPDFKitMapTest (PlatformName platformName, string expected)
			=> Assert.AreEqual (expected, platformName.GetPDFKitMap ());

		[TestCase (PlatformName.iOS, ApplePlatform.iOS)]
		[TestCase (PlatformName.TvOS, ApplePlatform.TVOS)]
		[TestCase (PlatformName.MacCatalyst, ApplePlatform.MacCatalyst)]
		[TestCase (PlatformName.MacOSX, ApplePlatform.MacOSX)]
		[TestCase (PlatformName.WatchOS, ApplePlatform.WatchOS)]
		[TestCase (PlatformName.None, ApplePlatform.None)]
		public void AsApplePlatformTest (PlatformName platformName, ApplePlatform expected)
			=> Assert.AreEqual (expected, platformName.AsApplePlatform ());
	}
}
