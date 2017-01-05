//
// Unit tests for GCExtendedGamepadSnapshot
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
#if XAMCORE_2_0
using Foundation;
#if !MONOMAC
using UIKit;
#endif
using GameController;
#else
using MonoTouch.Foundation;
using MonoTouch.GameController;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.GameController {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ExtendedGamepadSnapshotTest {

		[Test]
		public void Nullability ()
		{
#if MONOMAC
			if (!TestRuntime.CheckMacSystemVersion (10, 9))
				Assert.Inconclusive ("GameController is macOS 10.9+");
#else
			if (!UIDevice.CurrentDevice.CheckSystemVersion (7, 0))
				Assert.Inconclusive ("GameController is iOS7+");
#endif
			GCExtendedGamepadSnapShotDataV100 data;
			Assert.False (GCExtendedGamepadSnapshot.TryGetSnapShotData (null, out data), "TryGetSnapshotData");
			Assert.True (data.Version == 0, "Version");
			Assert.True (data.Size == 0, "Size");

			data = new GCExtendedGamepadSnapShotDataV100 ();
			Assert.True (data.Version == 0, "Version-2");
			Assert.True (data.Size == 0, "Size-2");

			using (var nsd = data.ToNSData ()) {
				Assert.True (GCExtendedGamepadSnapshot.TryGetSnapShotData (nsd, out data), "TryGetSnapshotData-2");
				Assert.True (data.Version == 0x100, "Version-3");
				Assert.True (data.Size == nsd.Length, "Size-3");
			}
		}
	}
}

#endif // !__WATCHOS__
