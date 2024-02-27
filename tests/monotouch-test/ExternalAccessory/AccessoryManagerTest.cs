//
// EAAccessoryManager Unit Tests
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc.
//

#if !__WATCHOS__

using System;
using Foundation;
using ExternalAccessory;
using ObjCRuntime;
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.ExternalAccessory {

	[TestFixture]
	// we want the test to be availble if we use the linker
	[Preserve (AllMembers = true)]
	[TestFixture]
	public class AccessoryManagerTest {

		[Test]
		public void Shared ()
		{
#if TVOS
			TestRuntime.AssertXcodeVersion (8, 0);
#endif
#if MONOMAC
			TestRuntime.AssertXcodeVersion (9, 0);
#endif
			// reported to throw an InvalidCastException on http://stackoverflow.com/q/18884195/220643
			var am = EAAccessoryManager.SharedAccessoryManager;
			// IsEmpty most of the time... unless you docked something, like a keyboard
			Assert.IsNotNull (am.ConnectedAccessories, "ConnectedAccessories");
		}

#if !MONOMAC && !__MACCATALYST__
		[Test]
		public void ShowBluetoothAccessoryPicker ()
		{
			// The API here was introduced to Mac Catalyst later than for the other frameworks, so we have this additional check
			TestRuntime.AssertSystemVersion (ApplePlatform.MacCatalyst, 14, 0, throwIfOtherPlatform: false);
			EAAccessoryManager.SharedAccessoryManager.ShowBluetoothAccessoryPicker (null, null);
		}
#endif
	}
}

#endif // !__WATCHOS__
