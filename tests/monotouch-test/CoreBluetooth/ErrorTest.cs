//
// Unit tests for CB[ATT]Error enums
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
using CoreBluetooth;
#else
using MonoTouch.Foundation;
using MonoTouch.CoreBluetooth;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.CoreBluetooth {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ErrorTest {

		[Test]
		public void ErrorDomain ()
		{
			Assert.That (CBError.None.GetDomain ().ToString (), Is.EqualTo ("CBErrorDomain"), "Domain");
		}

		[Test]
		public void AttErrorDomain ()
		{
			Assert.That (CBATTError.Success.GetDomain ().ToString (), Is.EqualTo ("CBATTErrorDomain"), "Domain");
		}
	}
}

#endif // !__WATCHOS__
