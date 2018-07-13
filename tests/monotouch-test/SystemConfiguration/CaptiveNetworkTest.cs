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
#if XAMCORE_2_0
using Foundation;
using ObjCRuntime;
using SystemConfiguration;
#if !MONOMAC
using UIKit;
#endif
#else
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.SystemConfiguration;
using MonoTouch.UIKit;
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
#if __TVOS__
		[ExpectedException (typeof (NotSupportedException))]
#else
		[ExpectedException (typeof (ArgumentNullException))]
#endif
		public void TryCopyCurrentNetworkInfo_Null ()
		{
			NSDictionary dict;
			CaptiveNetwork.TryCopyCurrentNetworkInfo (null, out dict);
		}
		
		[Test]
#if __TVOS__
		[ExpectedException (typeof (NotSupportedException))]
#endif
		public void TryCopyCurrentNetworkInfo ()
		{
			if (Runtime.Arch == Arch.SIMULATOR) {
#if __IOS__
				if (TestRuntime.CheckSystemVersion (PlatformName.iOS, 6, 0))
					Assert.Inconclusive ("This test throws EntryPointNotFoundException on the iOS 6 simulator with Lion");
#endif
			}

			NSDictionary dict;
			var status = CaptiveNetwork.TryCopyCurrentNetworkInfo ("en0", out dict);

			// No network, ignore test
			if (status == StatusCode.NoKey)
				return;

			Assert.AreEqual (StatusCode.OK, status, "Status");

#if __TVOS__
			Assert.IsNull (dict, "Dictionary"); // null?
			return;
#endif

			if ((dict == null) && (Runtime.Arch == Arch.DEVICE) && TestRuntime.CheckSystemVersion (PlatformName.iOS, 9, 0))
				Assert.Ignore ("null on iOS9 devices - CaptiveNetwork is being deprecated ?!?");

			if (dict.Count == 3) {
				Assert.NotNull (dict [CaptiveNetwork.NetworkInfoKeyBSSID], "NetworkInfoKeyBSSID");
				Assert.NotNull (dict [CaptiveNetwork.NetworkInfoKeySSID], "NetworkInfoKeySSID");
				Assert.NotNull (dict [CaptiveNetwork.NetworkInfoKeySSIDData], "NetworkInfoKeySSIDData");
			} else {
				Assert.Fail ("Unexpected dictionary result with {0} items", dict.Count);
			}
		}
#endif

		[Test]
#if __TVOS__
		[ExpectedException (typeof (NotSupportedException))]
#else
		[ExpectedException (typeof (ArgumentNullException))]
#endif
		public void MarkPortalOnline_Null ()
		{
			CaptiveNetwork.MarkPortalOnline (null);
		}

		[Test]
#if __TVOS__
		[ExpectedException (typeof (NotSupportedException))]
#endif
		public void MarkPortalOnline ()
		{
			TestRuntime.AssertSystemVersion (PlatformName.MacOSX, 10, 8, throwIfOtherPlatform: false);

			Assert.False (CaptiveNetwork.MarkPortalOnline ("xamxam"));
		}
		
		[Test]
#if __TVOS__
		[ExpectedException (typeof (NotSupportedException))]
#else
		[ExpectedException (typeof (ArgumentNullException))]
#endif
		public void MarkPortalOffline_Null ()
		{
			CaptiveNetwork.MarkPortalOffline (null);
		}

		[Test]
#if __TVOS__
		[ExpectedException (typeof (NotSupportedException))]
#endif
		public void MarkPortalOffline ()
		{
			TestRuntime.AssertSystemVersion (PlatformName.MacOSX, 10, 8, throwIfOtherPlatform: false);

			Assert.False (CaptiveNetwork.MarkPortalOffline ("xamxam"));
		}
		
		[Test]
#if __TVOS__
		[ExpectedException (typeof (NotSupportedException))]
#else
		[ExpectedException (typeof (ArgumentNullException))]
#endif
		public void SetSupportedSSIDs_Null ()
		{
			CaptiveNetwork.SetSupportedSSIDs (null);
		}

		[Test]
#if __TVOS__
		[ExpectedException (typeof (NotSupportedException))]
#endif
		public void SetSupportedSSIDs ()
		{
			TestRuntime.AssertSystemVersion (PlatformName.MacOSX, 10, 8, throwIfOtherPlatform: false);

#if MONOMAC
			bool supported = true;
#else
			// that API is deprecated in iOS9 - and it might be why it returns false (or not)
			bool supported = !TestRuntime.CheckXcodeVersion (7, 0);
#endif
			Assert.That (CaptiveNetwork.SetSupportedSSIDs (new string [2] { "one", "two" } ), Is.EqualTo (supported), "set");
		}
	}
}

#endif // !__WATCHOS__
