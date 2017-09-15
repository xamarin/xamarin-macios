// Unit test for AVAudioRecorder
// Authors: 
// 		Paola Villarreal (paola.villarreal@xamarin.com)
// Copyright 2014 Xamarin Inc. All rights reserved.

#if !__TVOS__ && !__WATCHOS__

using System;
#if XAMCORE_2_0
using Foundation;
using AudioToolbox;
using AVFoundation;
#else
using MonoTouch.AVFoundation;
using MonoTouch.Foundation;
using MonoTouch.AudioToolbox;
#endif
using NUnit.Framework;
namespace MonoTouchFixtures.AVFoundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AudioRecorderTest {
		NSObject[] Values = new NSObject[]
		{
			NSNumber.FromFloat ((float)44100), //Sample Rate
			NSNumber.FromInt32 ((int)AudioFormatType.AppleLossless), //AVFormat
			NSNumber.FromInt32 (2),
			NSNumber.FromInt32 ((int)AVAudioQuality.Min)
		};

		NSObject[] Keys = new NSObject[]
		{
			AVAudioSettings.AVSampleRateKey,
			AVAudioSettings.AVFormatIDKey,
			AVAudioSettings.AVNumberOfChannelsKey,
			AVAudioSettings.AVEncoderAudioQualityKey
		};
		[Test]
		public void Create ()
		{
			TestRuntime.RequestMicrophonePermission ();

			var url =  NSUrl.FromFilename ("/dev/null");
			NSError error;
			var audioSettings = new AudioSettings (NSDictionary.FromObjectsAndKeys (Values, Keys));

			using (var recorder = AVAudioRecorder.Create (url, audioSettings, out error)) {
				Assert.NotNull (recorder);
				Assert.Null (error);
			}
		}
		[Test]
		public void CreateWithError ()
		{
			TestRuntime.RequestMicrophonePermission ();

			var url =  NSUrl.FromFilename ("/dev/fake.wav");
			NSError error;
			var audioSettings = new AudioSettings (NSDictionary.FromObjectsAndKeys (Values, Keys));
			using (var recorder = AVAudioRecorder.Create (url, audioSettings, out error)) {
				Assert.Null (recorder);
				Assert.NotNull (error);
			}
		}

	}
}

#endif // !__TVOS__ && !__WATCHOS__
