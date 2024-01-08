//
// MusicPlayer unit Tests
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2023 Microsoft Corp. All rights reserved.
//

#if !__WATCHOS__

using System;

using AudioToolbox;
using Foundation;
using ObjCRuntime;

using NUnit.Framework;

namespace MonoTouchFixtures.AudioToolbox {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MusicPlayerTest {

		[Test]
		public void Defaults ()
		{
			using (var player = new MusicPlayer ()) {
				Assert.IsFalse (player.IsPlaying, "IsPlaying");
				Assert.AreEqual (0, player.Time, "Time");
				Assert.AreEqual (1, player.PlayRateScalar, "PlayRateScalar");
				Assert.AreEqual (MusicPlayerStatus.InvalidPlayerState, player.GetHostTimeForBeats (0, out var hosttime), "GetHostTimeForBeats");
				Assert.AreEqual (0, hosttime, "GetHostTimeForBeats - rv");
				Assert.AreEqual (MusicPlayerStatus.InvalidPlayerState, player.GetBeatsForHostTime (0, out var beats), "GetBeatsForHostTime");
				Assert.AreEqual (0, beats, "GetBeatsForHostTime - rv");
				Assert.IsNull (player.MusicSequence, "MusicSequence");
			}
		}

		[Test]
		public void MusicSequenceTest ()
		{
			using (var player = new MusicPlayer ()) {
				using (var ms = new MusicSequence ()) {
					Assert.IsNull (player.MusicSequence, "MusicSequence A");
					player.MusicSequence = null;
					Assert.IsNull (player.MusicSequence, "MusicSequence B");
					player.MusicSequence = ms;
					Assert.AreSame (ms, player.MusicSequence, "MusicSequence C");
					player.MusicSequence = null;
					Assert.IsNull (player.MusicSequence, "MusicSequence D");
				}
			}
		}

		[Test]
		public void PlayRateScalarTest ()
		{
			using (var player = new MusicPlayer ()) {
				Assert.AreEqual (1, player.PlayRateScalar, "PlayRateScalar A");
				player.PlayRateScalar = 2;
				Assert.AreEqual (2, player.PlayRateScalar, "PlayRateScalar B");
			}
		}

		[Test]
		public void TimeTest ()
		{
			using (var player = new MusicPlayer ()) {
				Assert.AreEqual (0, player.Time, "Time A");
				player.Time = 1;
				Assert.AreEqual (0, player.Time, "Time B");
				Assert.AreEqual (MusicPlayerStatus.Success, player.GetTime (out var time), "GetTime A");
				Assert.AreEqual (0, time, "GetTime B");
				Assert.AreEqual (MusicPlayerStatus.Success, player.SetTime (1), "SetTime A");
				Assert.AreEqual (MusicPlayerStatus.Success, player.GetTime (out time), "GetTime C");
				Assert.AreEqual (0, time, "GetTime D");
			}
		}

		[Test]
		public void CreateTest ()
		{
			using var player = MusicPlayer.Create (out var status);
			Assert.NotNull (player, "Got a player");
			Assert.AreEqual (MusicPlayerStatus.Success, status, "Status");
		}

		[Test]
		public void StartStopPreroll ()
		{
			using var player = MusicPlayer.Create (out var status);
			Assert.NotNull (player, "Got a player");
			Assert.AreEqual (MusicPlayerStatus.Success, status, "Status");
			Assert.AreEqual (MusicPlayerStatus.NoSequence, player.Preroll (), "Preroll");
			Assert.AreEqual (MusicPlayerStatus.NoSequence, player.Start (), "Start");
			Assert.AreEqual (MusicPlayerStatus.NoSequence, player.Stop (), "Stop");
		}
	}
}

#endif // !__WATCHOS__
