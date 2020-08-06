//
// Unit tests for CaptiveNetwork
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
using System.IO;
using Foundation;
using ObjCRuntime;
using SystemConfiguration;
#if !MONOMAC
using UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.SystemConfiguration {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CaptiveNetworkTest {
		
#if !MONOMAC // Fields are not on Mac
		[Test]
		public void Fields ()
		{
			if (Runtime.Arch == Arch.SIMULATOR) {
				// Fails (NullReferenceException) on iOS6 simulator
				TestRuntime.AssertSystemVersion (PlatformName.iOS, 7, 0, throwIfOtherPlatform: false);
			}

#if __TVOS__
			Assert.IsNull (CaptiveNetwork.NetworkInfoKeyBSSID, "kCNNetworkInfoKeyBSSID");
			Assert.IsNull (CaptiveNetwork.NetworkInfoKeySSID, "kCNNetworkInfoKeySSID");
			Assert.IsNull (CaptiveNetwork.NetworkInfoKeySSIDData, "kCNNetworkInfoKeySSIDData");
#else
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
			NSDictionary dict;
#if __TVOS__
			Assert.Throws<NotSupportedException> (() => CaptiveNetwork.TryCopyCurrentNetworkInfo (null, out dict));
#else
			Assert.Throws<ArgumentNullException> (() => CaptiveNetwork.TryCopyCurrentNetworkInfo (null, out dict));
#endif
		}
		
		[Test]
		public void TryCopyCurrentNetworkInfo ()
		{
			NSDictionary dict;
			StatusCode status;

#if __TVOS__
			Assert.Throws<NotSupportedException> (() => { status = CaptiveNetwork.TryCopyCurrentNetworkInfo ("en0", out dict); });
#else
			status = CaptiveNetwork.TryCopyCurrentNetworkInfo ("en0", out dict);

			// No network, ignore test
			if (status == StatusCode.NoKey)
				return;

			Assert.AreEqual (StatusCode.OK, status, "Status");
			// To get a non-null dictionary back, we must (https://developer.apple.com/documentation/systemconfiguration/1614126-cncopycurrentnetworkinfo)
			// * Use core location, and request (and get) authorization to use location information
			// * Add the 'com.apple.developer.networking.wifi-info' entitlement
			// I tried this, and still got null back, so just assert that we get null.
			Assert.IsNull (dict, "Dictionary");
#endif
		}

		[Test]
		public void TryGetSupportedInterfaces ()
		{
			StatusCode status;
#if __TVOS__
			Assert.Throws<NotSupportedException> (() => CaptiveNetwork.TryGetSupportedInterfaces (out var ifaces));
#else
			status = CaptiveNetwork.TryGetSupportedInterfaces (out var ifaces);
			Assert.AreEqual (StatusCode.OK, status, "Status");
#endif // __TVOS__
		}
#endif

		[Test]
		public void MarkPortalOnline_Null ()
		{
#if __TVOS__
			Assert.Throws<NotSupportedException> (() => CaptiveNetwork.MarkPortalOnline (null));
#else
			Assert.Throws<ArgumentNullException> (() => CaptiveNetwork.MarkPortalOnline (null));
#endif
		}

		[Test]
		public void MarkPortalOnline ()
		{
			TestRuntime.AssertSystemVersion (PlatformName.MacOSX, 10, 8, throwIfOtherPlatform: false);


#if __TVOS__
			Assert.Throws<NotSupportedException> (() => CaptiveNetwork.MarkPortalOnline ("xamxam"));
#else
			Assert.False (CaptiveNetwork.MarkPortalOnline ("xamxam"));
#endif
		}
		
		[Test]
		public void MarkPortalOffline_Null ()
		{
#if __TVOS__
			Assert.Throws<NotSupportedException> (() => CaptiveNetwork.MarkPortalOffline (null));
#else
			Assert.Throws<ArgumentNullException> (() => CaptiveNetwork.MarkPortalOffline (null));
#endif
		}

		[Test]
		public void MarkPortalOffline ()
		{
			TestRuntime.AssertSystemVersion (PlatformName.MacOSX, 10, 8, throwIfOtherPlatform: false);

#if __TVOS__
			Assert.Throws<NotSupportedException> (() => CaptiveNetwork.MarkPortalOffline ("xamxam"));
#else
			Assert.False (CaptiveNetwork.MarkPortalOffline ("xamxam"));
#endif
		}
		
		[Test]
		public void SetSupportedSSIDs_Null ()
		{
#if __TVOS__
			Assert.Throws<NotSupportedException> (() => CaptiveNetwork.SetSupportedSSIDs (null));
#else
			Assert.Throws<ArgumentNullException> (() => CaptiveNetwork.SetSupportedSSIDs (null));
#endif
		}

		[Test]
		public void SetSupportedSSIDs ()
		{
			TestRuntime.AssertSystemVersion (PlatformName.MacOSX, 10, 8, throwIfOtherPlatform: false);

#if MONOMAC
			bool supported = true;
#else
			// that API is deprecated in iOS9 - and it might be why it returns false (or not)
			bool supported = !TestRuntime.CheckXcodeVersion (7, 0);
#endif
#if __TVOS__
			Assert.Throws<NotSupportedException> (() => CaptiveNetwork.SetSupportedSSIDs (new string [2] { "one", "two" } ), "set");
#else
			Assert.That (CaptiveNetwork.SetSupportedSSIDs (new string [2] { "one", "two" }), Is.EqualTo (supported), "set");
#endif
		}
	}
}

#endif // !__WATCHOS__
