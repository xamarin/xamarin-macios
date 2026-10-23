//
// Unit tests for MidiEventPacket
//
// Copyright 2025 Microsoft Corp. All rights reserved.
//

#if HAS_COREMIDI

using System;
using System.Linq;

using CoreMidi;
using Foundation;

using NUnit.Framework;

namespace MonoTouchFixtures.CoreMidi {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MidiEventPacketTest {
		[Test]
		public void Default ()
		{
			Assert.Multiple (() => {
				Exception ex;
				uint v;

				var value = new MidiEventPacket ();
				Assert.That (value.Timestamp, Is.EqualTo (0), "Timestamp");
				Assert.That (value.WordCount, Is.EqualTo (0), "WordCount");
				Assert.That (value.Words.Length, Is.EqualTo (0), "Words.Length");

				ex = Assert.Throws<ArgumentOutOfRangeException> (() => v = value [-1], $"Index #-1");
				Assert.That (ex.Message, Does.Contain ("index must be positive."), $"Index #-1 message");

				for (var i = 0; i < 64; i++) {
					ex = Assert.Throws<ArgumentOutOfRangeException> (() => v = value [i], $"Index #{i}");
					Assert.That (ex.Message, Does.Contain ("index must be less than WordCount."), $"Index #{i} message");
				}

				ex = Assert.Throws<ArgumentOutOfRangeException> (() => v = value [64], $"Index #64");
				Assert.That (ex.Message, Does.Contain ("index must be less than 64."), $"Index #64 message");
			});
		}

		[Test]
		public void Roundtrips ()
		{
			Assert.Multiple (() => {
				Exception ex;

				var value = new MidiEventPacket ();

				// Timestamp
				value.Timestamp = 2;
				Assert.That (value.Timestamp, Is.EqualTo (2), "Timestamp");

				value.Timestamp = ulong.MinValue;
				Assert.That (value.Timestamp, Is.EqualTo (ulong.MinValue), "Timestamp #2");

				value.Timestamp = ulong.MaxValue;
				Assert.That (value.Timestamp, Is.EqualTo (ulong.MaxValue), "Timestamp #3");

				// WordCount

				value.WordCount = 3;
				Assert.That (value.WordCount, Is.EqualTo (3), "WordCount");
				Assert.That (value.Words.Length, Is.EqualTo (3), "WordCount");

				value.WordCount = uint.MinValue;
				Assert.That (value.WordCount, Is.EqualTo (uint.MinValue), "WordCount #2");
				Assert.That (value.Words.Length, Is.EqualTo (uint.MinValue), "WordCount #2");

				ex = Assert.Throws<ArgumentOutOfRangeException> (() => value.WordCount = uint.MaxValue, "WordCount #3");
				Assert.That (ex.Message, Does.Contain ("WordCount can't be higher than 64."), $"WordCount #3 message");

				ex = Assert.Throws<ArgumentOutOfRangeException> (() => value.WordCount = 65, "WordCount #4");
				Assert.That (ex.Message, Does.Contain ("WordCount can't be higher than 64."), $"WordCount #4 message");

				value.WordCount = 64;
				Assert.That (value.WordCount, Is.EqualTo (64), "WordCount #5");
				Assert.That (value.Words.Length, Is.EqualTo (64), "WordCount #5");
				for (var i = 0; i < value.WordCount; i++)
					Assert.That (value.Words [i], Is.EqualTo (0), $"WordCount #5 - {i}");
				for (var i = 0; i < value.WordCount; i++)
					Assert.That (value [i], Is.EqualTo (0), $"WordCount #5 - idx {i}");

				// Words

				Assert.Throws<ArgumentNullException> (() => value.Words = null, "Words Null");

				value.Words = new uint [0];
				Assert.That (value.WordCount, Is.EqualTo (0), "Words #1");
				Assert.That (value.Words.Length, Is.EqualTo (0), "Words #1 - Length");

				value.Words = new uint [] { 2 };
				Assert.That (value.WordCount, Is.EqualTo (1), "Words #2");
				Assert.That (value.Words.Length, Is.EqualTo (1), "Words #2 - Length");
				Assert.That (value.Words [0], Is.EqualTo (2), "Words #2 - element");
				Assert.That (value [0], Is.EqualTo (2), "Words #2 - idx");

				ex = Assert.Throws<ArgumentOutOfRangeException> (() => value.Words = new uint [65], "Words #3");
				Assert.That (ex.Message, Does.Contain ("WordCount can't be higher than 64."), $"Words #3 message");

				var array = Enumerable.Range (1, 64).Select (v => (uint) (v * 2)).ToArray ();
				value.Words = Enumerable.Range (1, 64).Select (v => (uint) (v * 2)).ToArray ();
				Assert.That (value.WordCount, Is.EqualTo (64), "Words #5");
				Assert.That (value.Words.Length, Is.EqualTo (64), "Words #5 - Length");
				for (var i = 0; i < 64; i++) {
					Assert.That (value.Words [i], Is.EqualTo ((i + 1) * 2), $"Words #5 - element {i}");
					Assert.That (value [i], Is.EqualTo ((i + 1) * 2), $"Words #5 - indexer {i}");
					Assert.That (array [i], Is.EqualTo ((i + 1) * 2), $"Words #5 - array {i}");
				}

				// indexer
				value.Words = new uint [64];
				Assert.That (value.WordCount, Is.EqualTo (64), "indexer #1");
				Assert.That (value.Words.Length, Is.EqualTo (64), "indexer #1 - Length");
				for (var i = 0; i < 64; i++) {
					Assert.That (value [i], Is.EqualTo (0), $"indexer #1 - element {i} - 1");
					var v = (uint) ((i + 3) * 3);
					value [i] = v;
					Assert.That (value [i], Is.EqualTo (v), $"indexer #1 - element {i} - 2");
				}
			});
		}
	}
}

#endif // HAS_COREMIDI
