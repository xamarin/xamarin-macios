//
// Unit tests for Midi2DeviceRevisionLevel
//

#if !__TVOS__ && !__WATCHOS__
using System;
using Foundation;
using CoreMidi;
using NUnit.Framework;

namespace MonoTouchFixtures.CoreMidi {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class Midi2DeviceRevisionLevelTest {
		[Test]
		public void RevisionLevel ()
		{
			var value = default (Midi2DeviceRevisionLevel);
			CollectionAssert.AreEqual (new byte [] { 0, 0, 0, 0 }, value.RevisionLevel, "A");

			value.RevisionLevel = new byte [] { 1, 2, 3, 4 };
			CollectionAssert.AreEqual (new byte [] { 1, 2, 3, 4 }, value.RevisionLevel, "B");

			Assert.Throws<ArgumentNullException> (() => value.RevisionLevel = null, "C");
			Assert.Throws<ArgumentOutOfRangeException> (() => value.RevisionLevel = new byte [2], "D");
		}
	}
}
#endif
