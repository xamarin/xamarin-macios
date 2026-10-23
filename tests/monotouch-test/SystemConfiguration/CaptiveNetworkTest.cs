//
// Unit tests for CaptiveNetwork
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

using System.IO;
using SystemConfiguration;
#if !MONOMAC
using UIKit;
#endif
using Xamarin.Utils;

namespace MonoTouchFixtures.SystemConfiguration {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CaptiveNetworkTest {

#if !MONOMAC // Fields are not on Mac
		[Test]
		public void Fields ()
		{
			if (TestRuntime.IsSimulatorOrDesktop) {
				// Fails (NullReferenceException) on iOS6 simulator
				TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 7, 0, throwIfOtherPlatform: false);
			}

#if !__TVOS__
			Assert.That (CaptiveNetwork.NetworkInfoKeyBSSID.ToString (), Is.EqualTo ("BSSID"), "kCNNetworkInfoKeyBSSID");
			Assert.That (CaptiveNetwork.NetworkInfoKeySSID.ToString (), Is.EqualTo ("SSID"), "kCNNetworkInfoKeySSID");
			Assert.That (CaptiveNetwork.NetworkInfoKeySSIDData.ToString (), Is.EqualTo ("SSIDDATA"), "kCNNetworkInfoKeySSIDData");
#endif
		}
#endif

#if !MONOMAC // TryCopyCurrentNetworkInfo and fields checked are not on Mac
		[Test]
		public void TryCopyCurrentNetworkInfo_Null ()
		{
#if !__TVOS__
			Assert.Throws<ArgumentNullException> (() => CaptiveNetwork.TryCopyCurrentNetworkInfo (null, out var dict));
#endif
		}

		[Test]
		public void TryCopyCurrentNetworkInfo ()
		{
#if !__TVOS__
			var status = CaptiveNetwork.TryCopyCurrentNetworkInfo ("en0", out var dict);

			// No network, ignore test
			if (status == StatusCode.NoKey)
				return;

			Assert.That (status, Is.EqualTo (StatusCode.OK), "Status");
			// It's quite complex to figure out whether we should get a dictionary back or not.
			// References:
			// * https://github.com/dotnet/macios/commit/24331f35dd67d19f3ed9aca7b8b21827ce0823c0
			// * https://github.com/dotnet/macios/issues/11504
			// * https://github.com/dotnet/macios/issues/12278
			// * https://developer.apple.com/documentation/systemconfiguration/1614126-cncopycurrentnetworkinfo
			// So don't assert anything about the dictionary.
#endif
		}

		[Test]
		public void TryGetSupportedInterfaces ()
		{
#if !__TVOS__
			var status = CaptiveNetwork.TryGetSupportedInterfaces (out var ifaces);
			Assert.That (status, Is.EqualTo (StatusCode.OK), "Status");
#endif // __TVOS__
		}
#endif

		[Test]
		public void MarkPortalOnline_Null ()
		{
#if !__TVOS__
			Assert.Throws<ArgumentNullException> (() => CaptiveNetwork.MarkPortalOnline (null));
#endif
		}

		[Test]
		public void MarkPortalOnline ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 8, throwIfOtherPlatform: false);


#if !__TVOS__
			Assert.That (CaptiveNetwork.MarkPortalOnline ("xamxam"), Is.False);
#endif
		}

		[Test]
		public void MarkPortalOffline_Null ()
		{
#if !__TVOS__
			Assert.Throws<ArgumentNullException> (() => CaptiveNetwork.MarkPortalOffline (null));
#endif
		}

		[Test]
		public void MarkPortalOffline ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 8, throwIfOtherPlatform: false);

#if !__TVOS__
			Assert.That (CaptiveNetwork.MarkPortalOffline ("xamxam"), Is.False);
#endif
		}

		[Test]
		public void SetSupportedSSIDs_Null ()
		{
#if !__TVOS__
			Assert.Throws<ArgumentNullException> (() => CaptiveNetwork.SetSupportedSSIDs (null));
#endif
		}

		[Test]
#if APP_EXTENSION
		[Ignore ("App extensions don't have CaptiveNetwork entitlements.")]
#endif
		public void SetSupportedSSIDs ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 8, throwIfOtherPlatform: false);

#if MONOMAC || __MACCATALYST__
			bool supported = true;
#else
			// that API is deprecated in iOS9 - and it might be why it returns false (or not)
			bool supported = !TestRuntime.CheckXcodeVersion (7, 0);
#endif
#if !__TVOS__
			Assert.That (CaptiveNetwork.SetSupportedSSIDs (new string [2] { "one", "two" }), Is.EqualTo (supported), "set");
#endif
		}
	}
}
