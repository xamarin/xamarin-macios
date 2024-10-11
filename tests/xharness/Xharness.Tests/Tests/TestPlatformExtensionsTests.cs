using System.Collections;
using Microsoft.DotNet.XHarness.iOS.Shared;
using NUnit.Framework;

namespace Xharness.Tests.Tests {

	[TestFixture]
	public class TestPlatformExtensionsTests {

		public class TestCasesData {
			public static IEnumerable GetSimulatorTestCases {
				get {
					foreach (var platform in new [] { TestPlatform.iOS }) {
						yield return new TestCaseData (platform).Returns ("iOS " + Xamarin.SdkVersions.MiniOSSimulator);
					}
					yield return new TestCaseData (TestPlatform.tvOS).Returns ("tvOS " + Xamarin.SdkVersions.MinTVOSSimulator);
				}
			}

			public static IEnumerable IsMacTestCases {
				get {

					foreach (var platform in new [] { TestPlatform.None,
													  TestPlatform.All,
													  TestPlatform.iOS,
													  TestPlatform.tvOS }) {
						yield return new TestCaseData (platform).Returns (false);
					}

					foreach (var platform in new [] { TestPlatform.Mac }) {
						yield return new TestCaseData (platform).Returns (true);
					}
				}
			}

			public static IEnumerable CanSymlinkTestCases {
				get {
					foreach (var platform in new [] { TestPlatform.iOS }) {
						yield return new TestCaseData (platform).Returns (true);
					}

					foreach (var platform in new [] {TestPlatform.None,

													  TestPlatform.tvOS,
													  TestPlatform.Mac }) {
						yield return new TestCaseData (platform).Returns (false);
					}
				}
			}
		}

		[Test, TestCaseSource (typeof (TestCasesData), "GetSimulatorTestCases")]
		public string GetSimulatorMinVersionTest (TestPlatform platform)
			=> platform.GetSimulatorMinVersion ();

		[Test, TestCaseSource (typeof (TestCasesData), "IsMacTestCases")]
		public bool IsMacTest (TestPlatform platform) => platform.IsMac ();

		[Test, TestCaseSource (typeof (TestCasesData), "CanSymlinkTestCases")]
		public bool CanSymlinkTest (TestPlatform platform) => platform.CanSymlink ();

	}

}
