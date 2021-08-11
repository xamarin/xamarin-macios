//
// Unit tests for PeripheralScanningOptions
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
using Foundation;
using CoreBluetooth;

using NUnit.Framework;

namespace MonoTouchFixtures.CoreBluetooth {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class PeripheralScanningOptionsTest {

		[Test]
		public void Defaults ()
		{
			var options = new PeripheralScanningOptions ();
			Assert.That (options.Dictionary.Count, Is.EqualTo ((nuint) 0), "Count");
			Assert.False (options.AllowDuplicatesKey, "AllowDuplicatesKey");
		}

		[Test]
		public void AllowDuplicatesKey_True ()
		{
			var options = new PeripheralScanningOptions () {
				AllowDuplicatesKey = true
			};
			Assert.That (options.Dictionary.Count, Is.EqualTo ((nuint) 1), "Count");
			Assert.True (options.AllowDuplicatesKey, "AllowDuplicatesKey");
		}

		[Test]
		public void AllowDuplicatesKey_False ()
		{
			var options = new PeripheralScanningOptions () {
				AllowDuplicatesKey = false
			};
			Assert.That (options.Dictionary.Count, Is.EqualTo ((nuint) 1), "Count");
			Assert.False (options.AllowDuplicatesKey, "AllowDuplicatesKey");
		}
	}
}

#endif // !__WATCHOS__
