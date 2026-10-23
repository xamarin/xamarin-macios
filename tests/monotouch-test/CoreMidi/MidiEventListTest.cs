//
// Unit tests for MidiEventList
//
// Copyright 2025 Microsoft Corp. All rights reserved.
//

#if HAS_COREMIDI

using System;
using System.Collections.Generic;
using System.Linq;

using CoreMidi;
using Foundation;

using NUnit.Framework;

namespace MonoTouchFixtures.CoreMidi {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MidiEventListTest {
		[Test]
		public void CtorTest ()
		{
			Assert.Multiple (() => {
				using var obj = new MidiEventList (MidiProtocolId.Protocol_1_0);
				Assert.That (obj.Protocol, Is.EqualTo (MidiProtocolId.Protocol_1_0), "Protocol");
				Assert.That (obj.PacketCount, Is.EqualTo (0), "PacketCount");
				var packets = obj.ToArray ();
				Assert.That (packets.Length, Is.EqualTo (0), "ToArray ().Length");
			});
		}

		[Test]
		public void CtorTest_Size ()
		{
			Exception ex;

			Assert.Multiple (() => {
				ex = Assert.Throws<ArgumentOutOfRangeException> (() => new MidiEventList (MidiProtocolId.Protocol_1_0, int.MinValue), "AOORE int.MinValue");
				Assert.That (ex.Message, Does.Contain ("size must be at least 276."), "AOORE msg int.MinValue");
				ex = Assert.Throws<ArgumentOutOfRangeException> (() => new MidiEventList (MidiProtocolId.Protocol_1_0, -1), "AOORE -1");
				Assert.That (ex.Message, Does.Contain ("size must be at least 276."), "AOORE msg -1");
				ex = Assert.Throws<ArgumentOutOfRangeException> (() => new MidiEventList (MidiProtocolId.Protocol_1_0, 0), "AOORE 0");
				Assert.That (ex.Message, Does.Contain ("size must be at least 276."), "AOORE msg 0");
				ex = Assert.Throws<ArgumentOutOfRangeException> (() => new MidiEventList (MidiProtocolId.Protocol_1_0, 275), "AOORE 275");
				Assert.That (ex.Message, Does.Contain ("size must be at least 276."), "AOORE msg 275");

				using var obj = new MidiEventList (MidiProtocolId.Protocol_1_0, 276);
				Assert.That (obj.Protocol, Is.EqualTo (MidiProtocolId.Protocol_1_0), "Protocol");
				Assert.That (obj.PacketCount, Is.EqualTo (0), "PacketCount");
				var packets = obj.ToArray ();
				Assert.That (packets.Length, Is.EqualTo (0), "ToArray ().Length");
			});
		}

		[Test]
		public void AddTest ()
		{
			Assert.Multiple (() => {
				using var obj = new MidiEventList (MidiProtocolId.Protocol_2_0);
				Assert.That (obj.Protocol, Is.EqualTo (MidiProtocolId.Protocol_2_0), "Protocol");
				Assert.That (obj.PacketCount, Is.EqualTo (0), "PacketCount");

				var rv = obj.Add (123, new uint [] { 1, 2, 3 });
				Assert.That (rv, Is.EqualTo (true), "Add B");
				Assert.That (obj.Protocol, Is.EqualTo (MidiProtocolId.Protocol_2_0), "Protocol B");
				Assert.That (obj.PacketCount, Is.EqualTo (1), "PacketCount B");

				var packets = obj.ToArray ();
				Assert.That (packets.Length, Is.EqualTo (1), "ToArray ().Length");
				Assert.That (packets [0].Timestamp, Is.EqualTo (123), "Item[0].Timestamp");
				Assert.That (packets [0].WordCount, Is.EqualTo (3), "Item[0].WordCount");
				Assert.That (packets [0].Words, Is.EqualTo (new uint [] { 1, 2, 3 }), "Item[0].Words");
			});
		}

		[Test]
		public void AddTest_ManyWords ()
		{
			Assert.Multiple (() => {
				using var obj = new MidiEventList (MidiProtocolId.Protocol_2_0);
				Assert.That (obj.Protocol, Is.EqualTo (MidiProtocolId.Protocol_2_0), "Protocol");
				Assert.That (obj.PacketCount, Is.EqualTo (0), "PacketCount");

				var manyWords = Enumerable.Range (1, 65).Select (v => (uint) v).ToArray ();
				var rv = obj.Add (123, manyWords);
				Assert.That (rv, Is.EqualTo (false), "Add B");
			});
		}

		[Test]
		public void AddTest_NotEnoughSpace ()
		{
			Assert.Multiple (() => {
				using var obj = new MidiEventList (MidiProtocolId.Protocol_2_0);
				Assert.That (obj.Protocol, Is.EqualTo (MidiProtocolId.Protocol_2_0), "Protocol");
				Assert.That (obj.PacketCount, Is.EqualTo (0), "PacketCount");

				var fitsTwice = Enumerable.Range (1, 24).Select (v => (uint) v).ToArray ();
				var rv = obj.Add (123, fitsTwice);
				Assert.That (rv, Is.EqualTo (true), "Add B");
				rv = obj.Add (456, fitsTwice);
				Assert.That (rv, Is.EqualTo (true), "Add C");
				rv = obj.Add (789, fitsTwice);
				Assert.That (rv, Is.EqualTo (false), "Add C");
			});
		}

		[Test]
		public void EnumeratorTest ()
		{
			Assert.Multiple (() => {
				using var obj = new MidiEventList (MidiProtocolId.Protocol_2_0);
				var rv = obj.Add (789, new uint [] { 4, 5, 6 });
				Assert.That (rv, Is.EqualTo (true), "Add B");
				Assert.That (obj.Protocol, Is.EqualTo (MidiProtocolId.Protocol_2_0), "Protocol B");
				Assert.That (obj.PacketCount, Is.EqualTo (1), "PacketCount B");

				var packets = obj.ToArray ();
				Assert.That (packets.Length, Is.EqualTo (1), "ToArray ().Length");
				Assert.That (packets [0].Timestamp, Is.EqualTo (789), "Item[0].Timestamp");
				Assert.That (packets [0].WordCount, Is.EqualTo (3), "Item[0].WordCount");
				Assert.That (packets [0].Words, Is.EqualTo (new uint [] { 4, 5, 6 }), "Item[0].Words");
			});
		}

		[Test]
		public void IteratorTest ()
		{
			Assert.Multiple (() => {
				using var obj = new MidiEventList (MidiProtocolId.Protocol_2_0);
				var rv = obj.Add (456, new uint [] { 1, 2, 3, 4, 5, 6 });
				Assert.That (rv, Is.EqualTo (true), "Add B");
				Assert.That (obj.Protocol, Is.EqualTo (MidiProtocolId.Protocol_2_0), "Protocol B");
				Assert.That (obj.PacketCount, Is.EqualTo (1), "PacketCount B");

				var packets = obj.ToArray ();
				Assert.That (packets.Length, Is.EqualTo (1), "ToArray ().Length");
				Assert.That (packets [0].Timestamp, Is.EqualTo (456), "Item[0].Timestamp");
				Assert.That (packets [0].WordCount, Is.EqualTo (6), "Item[0].WordCount");
				Assert.That (packets [0].Words, Is.EqualTo (new uint [] { 1, 2, 3, 4, 5, 6 }), "Item[0].Words");

				var packetList = new List<MidiEventPacket> ();
				obj.Iterate ((in MidiEventPacket packet) => {
					packetList.Add (packet);
				});
				Assert.That (packetList.Count, Is.EqualTo (1), "packetList.Length");
				Assert.That (packetList [0].Timestamp, Is.EqualTo (456), "packetList[0].Timestamp");
				Assert.That (packetList [0].WordCount, Is.EqualTo (6), "packetList[0].WordCount");
				Assert.That (packetList [0].Words, Is.EqualTo (new uint [] { 1, 2, 3, 4, 5, 6 }), "packetList[0].Words");
			});
		}

		// Build a MIDI 1.0 channel voice Note On UMP (a single 32-bit word).
		static uint Midi1NoteOn (byte group, byte channel, byte note, byte velocity)
			=> ((uint) MidiMessageType.ChannelVoice1 << 28) | ((uint) group << 24) | (0x9u << 20) | ((uint) channel << 16) | ((uint) note << 8) | velocity;

		[Test]
		public void IterateTest_Null ()
		{
			using var obj = new MidiEventList (MidiProtocolId.Protocol_1_0);
			Assert.Throws<ArgumentNullException> (() => obj.Iterate (null), "Iterate (null)");
		}

		[Test]
		public void ForEachEventTest_Null ()
		{
			using var obj = new MidiEventList (MidiProtocolId.Protocol_1_0);
			Assert.Throws<ArgumentNullException> (() => obj.ForEachEvent (null), "ForEachEvent (null)");
		}

		[Test]
		public void ForEachEventTest_Empty ()
		{
			using var obj = new MidiEventList (MidiProtocolId.Protocol_1_0);
			var count = 0;
			obj.ForEachEvent ((ulong timeStamp, MidiUniversalMessage message) => count++);
			Assert.That (count, Is.EqualTo (0), "count");
		}

		[Test]
		public void ForEachEventTest_Midi1NoteOn ()
		{
			using var obj = new MidiEventList (MidiProtocolId.Protocol_1_0);
			Assert.That (obj.Add (1234, new uint [] { Midi1NoteOn (0, 3, 60, 100) }), Is.True, "Add");

			var messages = new List<(ulong TimeStamp, MidiUniversalMessage Message)> ();
			obj.ForEachEvent ((ulong timeStamp, MidiUniversalMessage message) => messages.Add ((timeStamp, message)));

			Assert.That (messages.Count, Is.EqualTo (1), "Count");
			Assert.Multiple (() => {
				Assert.That (messages [0].TimeStamp, Is.EqualTo ((ulong) 1234), "TimeStamp");
				var message = messages [0].Message;
				Assert.That (message.Type, Is.EqualTo (MidiMessageType.ChannelVoice1), "Type");
				Assert.That (message.ChannelVoice1.Status, Is.EqualTo (MidiCVStatus.NoteOn), "Status");
				Assert.That (message.ChannelVoice1.Channel, Is.EqualTo (3), "Channel");
				Assert.That (message.ChannelVoice1.Note.Number, Is.EqualTo (60), "Note.Number");
				Assert.That (message.ChannelVoice1.Note.Velocity, Is.EqualTo (100), "Note.Velocity");
			});
		}

		[Test]
		public void ForEachEventTest_Midi2NoteOn ()
		{
			using var obj = new MidiEventList (MidiProtocolId.Protocol_2_0);
			// A MIDI 2.0 channel voice Note On UMP (two 32-bit words).
			var word0 = ((uint) MidiMessageType.ChannelVoice2 << 28) | (0x9u << 20) | (2u << 16) | (60u << 8) | (uint) MidiNoteAttribute.None;
			var word1 = (0xCAFEu << 16) | 0xBEEFu;
			Assert.That (obj.Add (4321, new uint [] { word0, word1 }), Is.True, "Add");

			var messages = new List<(ulong TimeStamp, MidiUniversalMessage Message)> ();
			obj.ForEachEvent ((ulong timeStamp, MidiUniversalMessage message) => messages.Add ((timeStamp, message)));

			Assert.That (messages.Count, Is.EqualTo (1), "Count");
			Assert.Multiple (() => {
				Assert.That (messages [0].TimeStamp, Is.EqualTo ((ulong) 4321), "TimeStamp");
				var message = messages [0].Message;
				Assert.That (message.Type, Is.EqualTo (MidiMessageType.ChannelVoice2), "Type");
				Assert.That (message.ChannelVoice2.Status, Is.EqualTo (MidiCVStatus.NoteOn), "Status");
				Assert.That (message.ChannelVoice2.Channel, Is.EqualTo (2), "Channel");
				Assert.That (message.ChannelVoice2.Note.Number, Is.EqualTo (60), "Note.Number");
				Assert.That (message.ChannelVoice2.Note.AttributeType, Is.EqualTo (MidiNoteAttribute.None), "Note.AttributeType");
				Assert.That (message.ChannelVoice2.Note.Velocity, Is.EqualTo (0xCAFE), "Note.Velocity");
				Assert.That (message.ChannelVoice2.Note.Attribute, Is.EqualTo (0xBEEF), "Note.Attribute");
			});
		}

		[Test]
		public void ForEachEventTest_DovregubbensHall ()
		{
			// The opening theme from "Dovregubbens Hall" in B minor.
			var melody = new byte [] {
				35, 37, 38, 40, 42, 38, 42,
				41, 37, 41, 40, 36, 40,
			};

			using var obj = new MidiEventList (MidiProtocolId.Protocol_1_0, 4096);
			for (var i = 0; i < melody.Length; i++)
				Assert.That (obj.Add ((ulong) (i + 1), new uint [] { Midi1NoteOn (0, 0, melody [i], 96) }), Is.True, $"Add #{i}");

			var notes = new List<byte> ();
			var timeStamps = new List<ulong> ();
			obj.ForEachEvent ((ulong timeStamp, MidiUniversalMessage message) => {
				Assert.That (message.Type, Is.EqualTo (MidiMessageType.ChannelVoice1), "Type");
				Assert.That (message.ChannelVoice1.Status, Is.EqualTo (MidiCVStatus.NoteOn), "Status");
				notes.Add (message.ChannelVoice1.Note.Number);
				timeStamps.Add (timeStamp);
			});

			Assert.Multiple (() => {
				Assert.That (notes, Is.EqualTo (new List<byte> (melody)), "notes");
				Assert.That (timeStamps, Is.EqualTo (Enumerable.Range (1, melody.Length).Select (v => (ulong) v).ToList ()), "timeStamps");
			});
		}
	}
}

#endif
