//
// Unit tests for AudioQueue
//
// Authors:
//	Marek Safar (marek.safar@gmail.com)
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
#if XAMCORE_2_0
using Foundation;
using AudioToolbox;
#else
using MonoTouch.Foundation;
using MonoTouch.MediaPlayer;
using MonoTouch.AudioToolbox;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.AudioToolbox {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AudioQueueTest
	{
#if !MONOMAC // HardwareCodecPolicy and SetChannelAssignments are iOS only
		[Test]
		public void Properties ()
		{
			TestRuntime.RequestMicrophonePermission ();

			var b = new InputAudioQueue (AudioStreamBasicDescription.CreateLinearPCM ());
			b.HardwareCodecPolicy = AudioQueueHardwareCodecPolicy.PreferHardware;

			Assert.That (b.HardwareCodecPolicy, Is.EqualTo (AudioQueueHardwareCodecPolicy.PreferHardware), "#1");
		}

		[Test]
		public void ChannelAssignments ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (6,0))
				Assert.Inconclusive ("Requires iOS 6");

			var aq = new OutputAudioQueue (AudioStreamBasicDescription.CreateLinearPCM ());

			Assert.AreEqual (AudioQueueStatus.Ok, aq.SetChannelAssignments (
					new AudioQueueChannelAssignment ("11", 0),
					new AudioQueueChannelAssignment ("22", 1)
				));
		}
#endif
		
		[Test][Ignore ("Fails on some machines with undefined error code 5")]
		public void ProcessingTap ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (6,0))
				Assert.Inconclusive ("AudioQueueProcessingTapNew requires iOS 6");

			var aq = new InputAudioQueue (AudioStreamBasicDescription.CreateLinearPCM ());
			AudioQueueStatus ret;
			bool called = false;

			using (var tap = aq.CreateProcessingTap (
				delegate(AudioQueueProcessingTap audioQueueTap, uint numberOfFrames, ref AudioTimeStamp timeStamp, ref AudioQueueProcessingTapFlags flags, AudioBuffers data) {
					called = true;
					return 33;
				}, AudioQueueProcessingTapFlags.PreEffects, out ret)) {
				Assert.AreEqual (AudioQueueStatus.Ok, ret, "#1");

				unsafe {
					AudioQueueBuffer* buffer;
					Assert.AreEqual (AudioQueueStatus.Ok, aq.AllocateBuffer (5000, out buffer), "#2");
					Assert.AreEqual (AudioQueueStatus.Ok, aq.EnqueueBuffer (buffer), "#3");
					//Assert.AreEqual (AudioQueueStatus.Ok, aq.Start (), "#4");
				}
			}

			//Assert.That (called, Is.True, "#10");
		}

		[Test]
		public void InvalidAudioBasicDescription ()
		{
			TestRuntime.RequestMicrophonePermission ();
			Assert.Throws<AudioQueueException> (() => new InputAudioQueue (new AudioStreamBasicDescription ()), "A");
		}
	}
}

#endif // !__WATCHOS__
