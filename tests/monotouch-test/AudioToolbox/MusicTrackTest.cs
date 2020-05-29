//
// MusicTrack unit Tests
//
// Authors:
//	Manuel de la Pena <mandel@microsoft.com>
//
// Copyright 2019 Microsoft Corporation All rights reserved.
//

#if !__WATCHOS__ && !MONOMAC && !__TVOS__

using System;
using AudioToolbox;
using Foundation;
using ObjCRuntime;
using CoreMidi;
using NUnit.Framework;

namespace MonoTouchFixtures.AudioToolbox {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MusicTrackTest {

		MusicSequence sequence;
		MusicTrack track;

		[SetUp]
		public void SetUp ()
		{
			sequence = new MusicSequence ();
			track = MusicTrack.FromSequence (sequence);
		}

		[TearDown]
		public void TearDown ()
		{
			track?.Dispose ();
			sequence?.Dispose ();
		}

		[Test]
		public void Defaults ()
		{
			Assert.That (track.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
			Assert.That (track.Sequence, Is.Not.Null, "Sequence");
		}

		[Test]
		public void MidiEndPointProperty ()
		{
			// get one of the endpoints, and set it and get it
			for (int i = 0; i < Midi.SourceCount; i++) {
				using (var endpoint = MidiEndpoint.GetSource (i)) {
					if (endpoint.Handle == 0)
						continue;
					track.SetDestMidiEndpoint (endpoint);
					MidiEndpoint outEnpoint;
					var status = track.GetDestMidiEndpoint (out outEnpoint);
					Assert.AreEqual (endpoint.Handle, outEnpoint.Handle, "Track endpoint.");
				}
			}
		}
	}
}

#endif // !__WATCHOS__
