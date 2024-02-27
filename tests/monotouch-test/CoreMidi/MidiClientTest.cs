//
// Unit tests for MidiClient
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2023 Microsoft Corp. All rights reserved.
//

#if !__TVOS__ && !__WATCHOS__
using System;
using System.Diagnostics;

using CoreMidi;
using Foundation;

using NUnit.Framework;

namespace MonoTouchFixtures.CoreMidi {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MidiClientTest {
		[Test]
		public void SendTest ()
		{
			if (Midi.DestinationCount <= 0)
				Assert.Inconclusive ("No Midi Destinations");

			using var ep = MidiEndpoint.GetDestination (0);
			Assert.NotNull (ep, "EndPoint");

			var mevent = new byte [] { 0x90, 0x40, 0x70 };
			using var client = new MidiClient ($"outputclient-{Process.GetCurrentProcess ().Id}");
			using var port = client.CreateOutputPort ($"outputport-{Process.GetCurrentProcess ().Id}");
			unsafe {
				fixed (byte* meventPtr = mevent) {
					using var packet1 = new MidiPacket (0, (ushort) mevent.Length, (IntPtr) meventPtr);
					using var packet2 = new MidiPacket (0, mevent);
					using var packet3 = new MidiPacket (0, mevent, 0, mevent.Length);
					var packets = new MidiPacket [] { packet1, packet2, packet3 };
					port.Send (ep, packets);
				}
			}
		}
	}
}
#endif
