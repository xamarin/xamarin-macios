//
// Unit tests for ASIdentifierManager
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2012,2015 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__ && !MONOMAC

using System;
#if XAMCORE_2_0
using Foundation;
using UIKit;
using AdSupport;
#else
using MonoTouch.Foundation;
using MonoTouch.AdSupport;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.AdSupport {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class IdentifierManagerTest {
		
		[Test]
		public void SharedManager ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (6,0))
				Assert.Inconclusive ("AdSupport is new in 6.0");

			// IsAdvertisingTrackingEnabled - device specific config
			Assert.NotNull (ASIdentifierManager.SharedManager.AdvertisingIdentifier, "AdvertisingIdentifier");
		}
	}
}

#endif // !__WATCHOS__
