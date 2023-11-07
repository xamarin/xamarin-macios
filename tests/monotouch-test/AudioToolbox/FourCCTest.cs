//
// FourCC Tests
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2012-2014 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using Foundation;
using AudioToolbox;
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.AudioToolbox {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class FourCCTests {

		public static string FourCC (int value)
		{
			return new string (new char [] {
				(char) (byte) (value >> 24),
				(char) (byte) (value >> 16),
				(char) (byte) (value >> 8),
				(char) (byte) value });
		}

		public static int FourCC (string s)
		{
			return (((int) s [0]) << 24 |
					((int) s [1]) << 16 |
					((int) s [2]) << 8 |
					((int) s [3]));
		}

		[Test]
		public void AudioFormatTypeTest ()
		{
			Assert.That ((int) AudioFormatType.MPEG4AAC_ELD_V2, Is.EqualTo (FourCC ("aacg")), "MPEG4AAC_ELD_V2");
		}

		[Test]
		public void AudioFilePropertyTest ()
		{
			Assert.That ((int) AudioFileProperty.AudioTrackCount, Is.EqualTo (FourCC ("atct")), "AudioTrackCount");
			Assert.That ((int) AudioFileProperty.UseAudioTrack, Is.EqualTo (FourCC ("uatk")), "UseAudioTrack");
		}

		[Test]
		public void AudioFileStreamPropertyTest ()
		{
			Assert.That ((int) AudioFileStreamProperty.InfoDictionary, Is.EqualTo (FourCC ("info")), "InfoDictionary");
		}

		[Test]
		public void AudioQueuePropertyTest ()
		{
			Assert.That ((int) AudioQueueProperty.EnableTimePitch, Is.EqualTo (FourCC ("q_tp")), "EnableTimePitch");
			Assert.That ((int) AudioQueueProperty.TimePitchAlgorithm, Is.EqualTo (FourCC ("qtpa")), "TimePitchAlgorithm");
			Assert.That ((int) AudioQueueProperty.TimePitchBypass, Is.EqualTo (FourCC ("qtpb")), "TimePitchBypass");
		}

		[Test]
		public void AudioQueueTimePitchAlgorithmtest ()
		{
			Assert.That ((int) AudioQueueTimePitchAlgorithm.Spectral, Is.EqualTo (FourCC ("spec")), "Spectral");
			Assert.That ((int) AudioQueueTimePitchAlgorithm.TimeDomain, Is.EqualTo (FourCC ("tido")), "TimeDomain");
#if !MONOMAC // LowQualityZeroLatency is iOS only
			Assert.That ((int) AudioQueueTimePitchAlgorithm.LowQualityZeroLatency, Is.EqualTo (FourCC ("lqzl")), "LowQualityZeroLatency");
#endif
			Assert.That ((int) AudioQueueTimePitchAlgorithm.Varispeed, Is.EqualTo (FourCC ("vspd")), "Varispeed");
		}

#if !MONOMAC // MusicSequenceFileTypeID and MusicSequenceType are iOS only
		[Test]
		public void MusicSequenceFileTypeIDTest ()
		{
			Assert.That ((int) MusicSequenceFileTypeID.Midi, Is.EqualTo (FourCC ("midi")), "Midi");
			Assert.That ((int) MusicSequenceFileTypeID.iMelody, Is.EqualTo (FourCC ("imel")), "iMelody");
		}

		[Test]
		public void MusicSequenceTypeTest ()
		{
			Assert.That ((int) MusicSequenceType.Beats, Is.EqualTo (FourCC ("beat")), "Beats");
			Assert.That ((int) MusicSequenceType.Seconds, Is.EqualTo (FourCC ("secs")), "Seconds");
			Assert.That ((int) MusicSequenceType.Samples, Is.EqualTo (FourCC ("samp")), "Samples");
		}
#endif
	}
}

#endif // !__WATCHOS__
