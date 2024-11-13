using System.IO;
using NUnit.Framework;
using Xharness.TestImporter;
using Xharness.TestImporter.Xamarin;

namespace Xharness.Tests.TestImporter.Xamarin.Tests {

	// very simple class, but we want to make sure that iOS, tvOS get the iosDownload and the mac
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
		public void GetAssembliesRootLocationTest (Platform platform, string expected)
			=> Assert.AreEqual (expected, assemblyLocator.GetAssembliesRootLocation (platform));
	}
}
