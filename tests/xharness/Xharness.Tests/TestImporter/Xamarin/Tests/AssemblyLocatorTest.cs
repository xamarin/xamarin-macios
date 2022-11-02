using System.IO;
using NUnit.Framework;
using Xharness.TestImporter;
using Xharness.TestImporter.Xamarin;

namespace Xharness.Tests.TestImporter.Xamarin.Tests {

	// very simple class, but we want to make sure that iOS, tvOS, watchOS get the iosDownload and the mac
	// versions get the mac download
	[TestFixture]
	public class AssemblyLocatorTest {

		const string iOSPath = "/path/to/ios/download";
		const string macOSPath = "/path/to/mac/download";
		AssemblyLocator assemblyLocator = new AssemblyLocator {
			iOSMonoSDKPath = iOSPath,
			MacMonoSDKPath = macOSPath,
		};

		[TestCase (Platform.iOS, iOSPath)]
		[TestCase (Platform.TvOS, iOSPath)]
		[TestCase (Platform.WatchOS, iOSPath)]
		[TestCase (Platform.MacOSFull, macOSPath)]
		[TestCase (Platform.MacOSModern, macOSPath)]
		public void GetAssembliesRootLocationTest (Platform platform, string expected)
			=> Assert.AreEqual (expected, assemblyLocator.GetAssembliesRootLocation (platform));

		[TestCase (Platform.iOS, iOSPath, "ios-bcl", "monotouch", "tests")]
		[TestCase (Platform.TvOS, iOSPath, "ios-bcl", "monotouch_tv", "tests")]
		[TestCase (Platform.WatchOS, iOSPath, "ios-bcl", "monotouch_watch", "tests")]
		[TestCase (Platform.MacOSFull, macOSPath, "mac-bcl", "xammac_net_4_5", "tests")]
		public void GetAssembliesLocation (Platform platform, params string [] expected)
			=> Assert.AreEqual (Path.Combine (expected), assemblyLocator.GetAssembliesLocation (platform));

	}
}
