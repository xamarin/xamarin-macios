//
// EAAccessoryManager Unit Tests
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc.
//

#if !__TVOS__ && !__WATCHOS__ && !MONOMAC

using System;
#if XAMCORE_2_0
using Foundation;
using ExternalAccessory;
#else
using MonoTouch.ExternalAccessory;
using MonoTouch.Foundation;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.ExternalAccessory {

	[TestFixture]
	// we want the test to be availble if we use the linker
	[Preserve (AllMembers = true)]
	[TestFixture]
	public class AccessoryManagerTest {

		[Test]
		public void Shared ()
		{
			// reported to throw an InvalidCastException on http://stackoverflow.com/q/18884195/220643
			var am = EAAccessoryManager.SharedAccessoryManager;
			// IsEmpty most of the time... unless you docked something, like a keyboard
			Assert.IsNotNull (am.ConnectedAccessories, "ConnectedAccessories");
		}

		[Test]
		public void ShowBluetoothAccessoryPicker ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (6,0))
				Assert.Inconclusive ("Requires iOS6+");

			EAAccessoryManager.SharedAccessoryManager.ShowBluetoothAccessoryPicker (null, null);
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
