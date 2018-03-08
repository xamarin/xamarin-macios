//
// Unit tests for MidiThruConnection
//
// Authors:
//	Alex Soto <alex.soto@xamarin.com>
//	
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

#if !__TVOS__ && !__WATCHOS__
using System;

#if XAMCORE_2_0
using Foundation;
using CoreMidi;
#else
using MonoTouch.Foundation;
using MonoTouch.CoreMidi;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.CoreMidi {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MidiThruConnectionTests {

		[Test]
		public void ConnectionCreateTest ()
		{
			var cnnParams = new MidiThruConnectionParams {
				Controls = new MidiControlTransform[] {
					new MidiControlTransform (),
					new MidiControlTransform (),
					new MidiControlTransform ()
				},
				Maps = new MidiValueMap[] {
					new MidiValueMap (),
					new MidiValueMap ()
				},
				KeyPressure = new MidiTransform (MidiTransformType.Scale, 2),
				FilterOutAllControls = true,
				FilterOutBeatClock = true,
				FilterOutMtc = true,
				FilterOutSysEx = true,
				FilterOutTuneRequest = true,
				LowNote = 1,
				HighNote = 8
			};

			MidiError err;
			using (var connection = MidiThruConnection.Create ("com.xamarin.midi", cnnParams, out err)) {
				Assert.IsTrue (err == MidiError.Ok, "midi connection error");
				Assert.IsNotNull (connection, "midi connection should not be null");
			}
		}

		[Test]
		public void GetSetParamsTest ()
		{
			var cnnParams = new MidiThruConnectionParams {
				Controls = new MidiControlTransform[] {
					new MidiControlTransform (),
					new MidiControlTransform (),
					new MidiControlTransform ()
				},
				Maps = new MidiValueMap[] {
					new MidiValueMap (),
					new MidiValueMap ()
				},
				KeyPressure = new MidiTransform (MidiTransformType.Scale, 2),
				FilterOutAllControls = true,
				FilterOutBeatClock = true,
				FilterOutMtc = true,
				FilterOutSysEx = true,
				FilterOutTuneRequest = true,
				LowNote = 1,
				HighNote = 8
			};

			MidiError err;
			using (var connection = MidiThruConnection.Create ("com.xamarin.midi", cnnParams, out err)) {
				Assert.IsTrue (err == MidiError.Ok, "midi connection error");
				Assert.IsNotNull (connection, "midi connection should not be null");

				var gotParams = connection.GetParams (out err);
				Assert.IsTrue (err == MidiError.Ok, "midi connection error");
				// Test dynamic part of the struct
				Assert.IsTrue (gotParams.Controls.Length == cnnParams.Controls.Length, "midi params objects should be the same amount");
				Assert.IsTrue (gotParams.Maps.Length == cnnParams.Maps.Length, "midi params objects should be the same amount");

				var newParams = new MidiThruConnectionParams {
					FilterOutAllControls = false,
					FilterOutBeatClock = true,
					FilterOutMtc = true,
					FilterOutSysEx = true,
					FilterOutTuneRequest = true,
					HighNote = 5
				};

				err = connection.SetParams (newParams);
				Assert.IsTrue (err == MidiError.Ok, "midi connection error");

				gotParams = connection.GetParams (out err);
				Assert.IsTrue (err == MidiError.Ok, "midi connection error");
				Assert.IsTrue (gotParams.FilterOutBeatClock, "FilterOutBeatClock should be true");
				Assert.IsFalse (gotParams.FilterOutAllControls, "FilterOutAllControls should be false");
				Assert.IsTrue (gotParams.HighNote == 5, "HighNote should be 5");
			}
		}

		[Test]
		public void FindTest()
		{
			var cnnParams1 = new MidiThruConnectionParams {
				FilterOutAllControls = true,
				FilterOutSysEx = true,
				FilterOutTuneRequest = true,
				LowNote = 1,
				HighNote = 8
			};

			var cnnParams2 = new MidiThruConnectionParams {
				FilterOutAllControls = true,
				FilterOutSysEx = false,
				FilterOutTuneRequest = false,
				LowNote = 1,
				HighNote = 4
			};

			var ownerId = $"com.xamarin.midi.{DateTime.UtcNow.Ticks}";
			using (var connection1 = MidiThruConnection.Create (ownerId, cnnParams1))
			using (var connection2 = MidiThruConnection.Create (ownerId, cnnParams2)) {
				var connections = MidiThruConnection.Find (ownerId, out var err);
				Assert.IsTrue (err == MidiError.Ok, "midi connection error");
				Assert.NotNull (connections, "connections should not be null");
				Assert.That (connections.Length, Is.EqualTo (2), "2 midi connections expected");
			}
		}
	}
}
#endif // !__TVOS__ && !__WATCHOS__
