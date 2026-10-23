//
// Unit tests for MidiEndpointTest
//
// Authors:
//	Alex Soto <alexsoto@microsoft.com>
//	
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

#if !__TVOS__
using System;
using System.Collections.Generic;

using Foundation;
using CoreMidi;

using NUnit.Framework;

namespace MonoTouchFixtures.CoreMidi {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MidiEndpointTest {
		[Test]
		public void CorrectDisposeTest ()
		{
			// Test for bug 43582 - https://bugzilla.xamarin.com/show_bug.cgi?id=43582
			// This will throw if bug dispose code isn't fixed
			// System.InvalidOperationException: Handle is not initialized
			Assert.DoesNotThrow (() => {
				for (int i = 0; i < Midi.SourceCount; i++) {
					using (var endpoint = MidiEndpoint.GetSource (i)) {
						if (endpoint.Handle == 0)
							continue;
					}
				}
			});
		}

		[Test]
		public void RefConsTest ()
		{
			var anyChecks = false;

			for (var i = 0; i < Midi.DeviceCount; i++) {
				using var device = Midi.GetDevice (i);
				Assert.IsNotNull (device, "Device");
				for (var e = 0; e < device.EntityCount; e++) {
					using var entity = device.GetEntity (e);
					var endpoints = new List<MidiEndpoint> ();
					for (var d = 0; d < entity.Destinations; d++)
						endpoints.Add (entity.GetDestination (d));
					for (var d = 0; d < entity.Sources; d++)
						endpoints.Add (entity.GetSource (d));

					foreach (var ep in endpoints) {
						Assert.NotNull (ep, "EndPoint");

						// GetRefCons/SetRefCons return GeneralParamError (-50) on hosts that don't
						// support refcons for endpoints, but can succeed (Ok) on hosts that do.
						// Accept both, and only verify the refcon round-trip when the calls succeed.
						const MidiError ParamErr = (MidiError) (-50);

						var getA = ep.GetRefCons (out var ref1, out var ref2);
						Assert.That (getA, Is.EqualTo (MidiError.Ok).Or.EqualTo (ParamErr), "GetRefCons A");
						if (getA == ParamErr) {
							Assert.That (ref1, Is.EqualTo (IntPtr.Zero), "GetRefCons A 1");
							Assert.That (ref2, Is.EqualTo (IntPtr.Zero), "GetRefCons A 2");
						}

						ref1 = unchecked((IntPtr) 0xfee1600d);
						ref2 = 0x42f00f00;
						var setB = ep.SetRefCons (ref1, ref2);
						Assert.That (setB, Is.EqualTo (MidiError.Ok).Or.EqualTo (ParamErr), "SetRefCons B");

						var getC = ep.GetRefCons (out var ref1C, out var ref2C);
						Assert.That (getC, Is.EqualTo (MidiError.Ok).Or.EqualTo (ParamErr), "GetRefCons C");
						if (setB == MidiError.Ok && getC == MidiError.Ok) {
							Assert.That (ref1C, Is.EqualTo (ref1), "GetRefCons C 1");
							Assert.That (ref2C, Is.EqualTo (ref2), "GetRefCons C 2");
						} else if (getC == ParamErr) {
							Assert.That (ref1C, Is.EqualTo (IntPtr.Zero), "GetRefCons C 1");
							Assert.That (ref2C, Is.EqualTo (IntPtr.Zero), "GetRefCons C 2");
						}

						var setD = ep.SetRefCons (IntPtr.Zero, IntPtr.Zero);
						Assert.That (setD, Is.EqualTo (MidiError.Ok).Or.EqualTo (ParamErr), "SetRefCons D");

						var getE = ep.GetRefCons (out var ref1E, out var ref2E);
						Assert.That (getE, Is.EqualTo (MidiError.Ok).Or.EqualTo (ParamErr), "GetRefCons E");
						if (setD == MidiError.Ok && getE == MidiError.Ok) {
							Assert.That (ref1E, Is.EqualTo (IntPtr.Zero), "GetRefCons E 1");
							Assert.That (ref2E, Is.EqualTo (IntPtr.Zero), "GetRefCons E 2");
						} else if (getE == ParamErr) {
							Assert.That (ref1E, Is.EqualTo (IntPtr.Zero), "GetRefCons E 1");
							Assert.That (ref2E, Is.EqualTo (IntPtr.Zero), "GetRefCons E 2");
						}

						anyChecks = true;
					}
				}
			}

			if (!anyChecks)
				Assert.Inconclusive ("No applicable MidiEntity found.");
		}
	}
}
#endif
