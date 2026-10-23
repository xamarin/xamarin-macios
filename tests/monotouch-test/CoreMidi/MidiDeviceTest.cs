// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#nullable enable

#if !__TVOS__
using System;
using System.Diagnostics;

using CoreMidi;
using Foundation;

using NUnit.Framework;

namespace MonoTouchFixtures.CoreMidi {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MidiDeviceTest {
		[Test]
		public void ExternalDevice ()
		{
			using var device = Midi.CreateExternalDevice ("MonoTouchTestMidiTestDevice", "MonoTouchTestMidiTestManufacturer", "MonoTouchTestMidiTestModel", out var status);
			if (status == MidiError.NotPermitted)
				Assert.Inconclusive ("MIDI permission not granted in this environment.");
			Assert.That (device, Is.Not.Null, "Device");
			Assert.That (status, Is.EqualTo (MidiError.Ok), "Status");
			if (device is not null) {
				var rv = MidiSetup.AddExternalDevice (device);
				Assert.That (rv, Is.EqualTo (MidiError.Ok), "Add Status");
				rv = MidiSetup.RemoveExternalDevice (device);
				Assert.That (rv, Is.EqualTo (MidiError.Ok), "Remove Status");
			}
		}
	}
}
#endif
