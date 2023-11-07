//
// Unit tests for AudioChannelLayout
//
// Authors:
//	Marek Safar (marek.safar@gmail.com)
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using Foundation;
using AudioToolbox;
using NUnit.Framework;

namespace MonoTouchFixtures.AudioToolbox {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AudioChannelLayoutTest {
		[Test]
		public void Validate ()
		{
			var acl = AudioChannelLayout.FromAudioChannelLayoutTag (AudioChannelLayoutTag.AudioUnit_6_1);
			Assert.AreEqual (AudioFormatError.None, AudioChannelLayout.Validate (acl));
		}

		[Test]
		public void FromAudioChannelBitmap ()
		{
			var bitmap = AudioChannelLayoutTag.AudioUnit_7_1_Front.ToAudioChannel ();
			Assert.IsNotNull (AudioChannelLayout.FromAudioChannelBitmap (bitmap.Value));
		}

		[Test]
		public void FromAudioChannelLayoutTag ()
		{
			Assert.IsNotNull (AudioChannelLayout.FromAudioChannelLayoutTag (AudioChannelLayoutTag.AudioUnit_6_1));
		}

		[Test]
		public void GetTagForChannelLayout ()
		{
			var acl = AudioChannelLayout.FromAudioChannelLayoutTag (AudioChannelLayoutTag.AudioUnit_6_1);
			Assert.That (AudioChannelLayout.GetTagForChannelLayout (acl), Is.EqualTo (AudioChannelLayoutTag.AudioUnit_6_1));
		}

		[Test]
		public void GetNumberOfChannels ()
		{
			var acl = AudioChannelLayout.FromAudioChannelLayoutTag (AudioChannelLayoutTag.MPEG_7_1_B);
			Assert.That (AudioChannelLayout.GetNumberOfChannels (acl), Is.EqualTo (8));
		}

		[Test]
		public void GetChannelMap ()
		{
			var acl1 = AudioChannelLayout.FromAudioChannelLayoutTag (AudioChannelLayoutTag.AudioUnit_6_1);
			var acl2 = AudioChannelLayout.FromAudioChannelLayoutTag (AudioChannelLayoutTag.MPEG_7_1_B);

			Assert.IsNotNull (AudioChannelLayout.GetChannelMap (acl1, acl2));
		}

		[Test]
		public void GetTagsForNumberOfChannels ()
		{
			Assert.IsNotNull (AudioChannelLayout.GetTagsForNumberOfChannels (4));
		}

		[Test]
		public void GetMatrixMixMap ()
		{
			var acl1 = AudioChannelLayout.FromAudioChannelLayoutTag (AudioChannelLayoutTag.AudioUnit_6_1);
			var acl2 = AudioChannelLayout.FromAudioChannelLayoutTag (AudioChannelLayoutTag.MPEG_7_1_B);

			Assert.IsNotNull (AudioChannelLayout.GetMatrixMixMap (acl1, acl2));
		}
	}
}

#endif // !__WATCHOS__
