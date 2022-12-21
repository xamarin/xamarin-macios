//
// Unit tests for AudioUnit
//
// Authors:
//	Rolf Bjarne Kvinge (rolf@xamarin.com)
//
// Copyright 2022 Microsoft Corp. All rights reserved.
//

#if __MACOS__ && NET

using System;
using System.Threading;

using Foundation;
using AudioToolbox;
using AudioUnit;

using NUnit.Framework;

namespace MonoTouchFixtures.AudioToolbox {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AudioUnitTest {
		ManualResetEvent inputCallbackEvent = new ManualResetEvent (false);

		// This test currently only works on macOS, probably due to missing microphone entitlements/permissions for mobile platforms.
		[Test]
		public void Callbacks ()
		{
			var audioComponent = AudioComponent.FindComponent (AudioTypeOutput.VoiceProcessingIO);
			using var audioUnit = new global::AudioUnit.AudioUnit (audioComponent);

			var rv = audioUnit.SetInputCallback (InputCallback, AudioUnitScopeType.Input, 1);
			if (rv == AudioUnitStatus.CannotDoInCurrentContext)
				Assert.Ignore ("Can't set input callback"); // No microphone? In a VM? This seems to happen often on bots.
			Assert.AreEqual (AudioUnitStatus.OK, rv, "SetInputCallback");
			Assert.AreEqual (AudioUnitStatus.OK, audioUnit.Initialize (), "Initialize");
			try {
				Assert.AreEqual (AudioUnitStatus.OK, audioUnit.Start (), "Start");
				Assert.IsTrue (inputCallbackEvent.WaitOne (TimeSpan.FromSeconds (1)), "No input callback for 1 second");
			} finally {
				Assert.AreEqual (AudioUnitStatus.OK, audioUnit.Stop (), "Stop");
			}
		}

		AudioUnitStatus InputCallback (AudioUnitRenderActionFlags actionFlags, AudioTimeStamp timeStamp, uint busNumber, uint numberFrames, global::AudioUnit.AudioUnit audioUnit)
		{
			inputCallbackEvent.Set ();
			return AudioUnitStatus.NoError;
		}
	}
}

#endif // __MACOS__ && NET
