using System.Collections;

using Microsoft.DotNet.XHarness.iOS.Shared;

using NUnit.Framework;

namespace Xharness.Tests.Tests {

	[TestFixture]
	public class TestPlatformExtensionsTests {

		public class TestCasesData {
			public static IEnumerable GetSimulatorTestCases {
				get {
					foreach (var platform in new [] { TestPlatform.iOS,
														TestPlatform.iOS_Unified,
													  TestPlatform.iOS_TodayExtension64,
													  TestPlatform.iOS_Unified32,
													  TestPlatform.iOS_Unified64,}) {
						yield return new TestCaseData (platform).Returns ("iOS " + Xamarin.SdkVersions.MiniOSSimulator);
					}
					yield return new TestCaseData (TestPlatform.tvOS).Returns ("tvOS " + Xamarin.SdkVersions.MinTVOSSimulator);
					foreach (var platform in new [] { TestPlatform.watchOS,
													  TestPlatform.watchOS_32,
													  TestPlatform.watchOS_64_32}) {
						yield return new TestCaseData (platform).Returns ("watchOS " + Xamarin.SdkVersions.MinWatchOSSimulator);
					}
				}
			}

			public static IEnumerable IsMacTestCases {
				get {

					foreach (var platform in new [] { TestPlatform.None,
													  TestPlatform.All,
													  TestPlatform.iOS,
													  TestPlatform.iOS_Unified,
													  TestPlatform.iOS_Unified32,
													  TestPlatform.iOS_Unified64,
													  TestPlatform.iOS_TodayExtension64,
													  TestPlatform.tvOS,
													  TestPlatform.watchOS,
													  TestPlatform.watchOS_32,
													  TestPlatform.watchOS_64_32 }) {
						yield return new TestCaseData (platform).Returns (false);
					}

					foreach (var platform in new [] { TestPlatform.Mac,
													  TestPlatform.Mac_Modern,
													  TestPlatform.Mac_Full,
													  TestPlatform.Mac_System }) {
						yield return new TestCaseData (platform).Returns (true);
					}
				}
			}

			public static IEnumerable CanSymlinkTestCases {
				get {
					foreach (var platform in new [] { TestPlatform.iOS,
													  TestPlatform.iOS_TodayExtension64,
														TestPlatform.iOS_Unified,
													  TestPlatform.iOS_Unified32,
													  TestPlatform.iOS_Unified64 }) {
						yield return new TestCaseData (platform).Returns (true);
					}

					foreach (var platform in new [] {TestPlatform.None,

													  TestPlatform.tvOS,
													  TestPlatform.watchOS,
													  TestPlatform.watchOS_32,
													  TestPlatform.watchOS_64_32,
													  TestPlatform.Mac,
													  TestPlatform.Mac_Modern,
													  TestPlatform.Mac_Full,
													  TestPlatform.Mac_System}) {
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
