//
// Unit tests for MidiCIProfileId
//

#if !__TVOS__ && !__WATCHOS__
using System;
using Foundation;
using CoreMidi;
using NUnit.Framework;

namespace MonoTouchFixtures.CoreMidi {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MidiCIProfileIdTest {
		[Test]
		public void Standard ()
		{
			var value = default (MidiCIProfileId);
			Assert.AreEqual (0, value.Standard.ProfileIdByte1, "ProfileIdByte1 A");
			Assert.AreEqual (0, value.Standard.ProfileBank, "ProfileBank A");
			Assert.AreEqual (0, value.Standard.ProfileNumber, "ProfileNumber A");
			Assert.AreEqual (0, value.Standard.ProfileVersion, "ProfileVersion A");
			Assert.AreEqual (0, value.Standard.ProfileLevel, "ProfileLevel A");

			value.Standard = new MidiCIProfileIdStandard () {
				ProfileIdByte1 = 1,
				ProfileBank = 2,
				ProfileNumber = 3,
				ProfileVersion = 4,
				ProfileLevel = 5,
			};

			Assert.AreEqual (1, value.Standard.ProfileIdByte1, "ProfileIdByte1 B");
			Assert.AreEqual (2, value.Standard.ProfileBank, "ProfileBank B");
			Assert.AreEqual (3, value.Standard.ProfileNumber, "ProfileNumber B");
			Assert.AreEqual (4, value.Standard.ProfileVersion, "ProfileVersion B");
			Assert.AreEqual (5, value.Standard.ProfileLevel, "ProfileLevel B");
		}

		[Test]
		public void ManufacturerSpecific ()
		{
			var value = default (MidiCIProfileId);
			Assert.AreEqual (0, value.ManufacturerSpecific.SysExId1, "SysExId1 A");
			Assert.AreEqual (0, value.ManufacturerSpecific.SysExId2, "SysExId2 A");
			Assert.AreEqual (0, value.ManufacturerSpecific.SysExId3, "SysExId3 A");
			Assert.AreEqual (0, value.ManufacturerSpecific.Info1, "Info1 A");
			Assert.AreEqual (0, value.ManufacturerSpecific.Info2, "Info2 A");

			value.ManufacturerSpecific = new MidiCIProfileIdManufacturerSpecific () {
				SysExId1 = 1,
				SysExId2 = 2,
				SysExId3 = 3,
				Info1 = 4,
				Info2 = 5,
			};

			Assert.AreEqual (1, value.ManufacturerSpecific.SysExId1, "SysExId1 B");
			Assert.AreEqual (2, value.ManufacturerSpecific.SysExId2, "SysExId2 B");
			Assert.AreEqual (3, value.ManufacturerSpecific.SysExId3, "SysExId3 B");
			Assert.AreEqual (4, value.ManufacturerSpecific.Info1, "Info1 B");
			Assert.AreEqual (5, value.ManufacturerSpecific.Info2, "Info2 B");
		}
	}
}
#endif
