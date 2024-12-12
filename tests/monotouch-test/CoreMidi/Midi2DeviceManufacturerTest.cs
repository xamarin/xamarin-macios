//
// Unit tests for Midi2DeviceManufacturer
//

#if !__TVOS__ && !__WATCHOS__
using System;
using Foundation;
using CoreMidi;
using NUnit.Framework;

namespace MonoTouchFixtures.CoreMidi {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class Midi2DeviceManufacturerTest {
		[Test]
		public void SysExIdByte ()
		{
			var value = default (Midi2DeviceManufacturer);
			CollectionAssert.AreEqual (new byte [] { 0, 0, 0 }, value.SysExIdByte, "A");

			value.SysExIdByte = new byte [] { 1, 2, 3 };
			CollectionAssert.AreEqual (new byte [] { 1, 2, 3 }, value.SysExIdByte, "B");

			Assert.Throws<ArgumentNullException> (() => value.SysExIdByte = null, "C");
			Assert.Throws<ArgumentOutOfRangeException> (() => value.SysExIdByte = new byte [2], "D");
		}
	}
}
#endif
