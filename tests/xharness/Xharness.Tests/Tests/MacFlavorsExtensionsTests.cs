using Microsoft.DotNet.XHarness.iOS.Shared;
using NUnit.Framework;

namespace Xharness.Tests.Tests {

	[TestFixture]
	public class MacFlavorsExtensionsTests {

		[TestCase (MacFlavors.Console, TestPlatform.Mac)]
		[TestCase (MacFlavors.Full, TestPlatform.Mac_Full)]
		[TestCase (MacFlavors.Modern, TestPlatform.Mac_Modern)]
		[TestCase (MacFlavors.System, TestPlatform.Mac_System)]
		public void ToTestPlatformTest (MacFlavors flavor, TestPlatform expected)
			=> Assert.AreEqual (flavor.ToTestPlatform (), expected);
	}
}
