using System;
using NUnit.Framework;
using Xharness.BCLTestImporter;
using Xharness.BCLTestImporter.Xamarin;

namespace Xharness.Tests.BCLTestImporter.Xamarin.Tests {

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
	}
}
