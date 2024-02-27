//
// Unit tests for HMCharacteristicTest
//
// Authors:
//	TJ Lambert <TJ.Lambert@microsoft.com>
//
//
// Copyright 2022 Microsoft. All rights reserved.
//

#if HAS_HOMEKIT

using System;
using NUnit.Framework;

using Foundation;
using HomeKit;
using ObjCRuntime;
using Xamarin.Utils;

namespace MonoTouchFixtures.HomeKit {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class HMCharacteristicTest {
		[Test]
		public void WriteValueNullTest ()
		{
#if __MACCATALYST__
			TestRuntime.AssertSystemVersion (ApplePlatform.MacCatalyst, 14, 0);
#endif
			var characteristic = new HMCharacteristic ();
			Assert.Throws<ArgumentNullException> (delegate { characteristic.WriteValue (null, null); }, $"WriteValue should accept a null argument for 'value'.");
		}
	}
}

#endif // HAS_HOMEKIT
