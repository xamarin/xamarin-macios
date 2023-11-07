//
// Unit tests for AVAudioPlayer
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System.IO;
using Foundation;
using AVFoundation;
using NUnit.Framework;
using System;

namespace MonoTouchFixtures.AVFoundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AudioPlayerTest {

		[Test]
		public void FromUrl ()
		{
			string file = Path.Combine (NSBundle.MainBundle.ResourcePath, "Hand.wav");
			Assert.True (File.Exists (file), file);
			using (NSUrl url = new (file, false))
			using (AVAudioPlayer ap = AVAudioPlayer.FromUrl (url, out NSError error)) {
				Assert.NotNull (ap, "AVAudioPlayer");
				Assert.Null (error, "NSError");
			}
		}

		[Test]
		public void FromUrlWithInvalidUrl ()
		{
			Assert.DoesNotThrow (() => {
				using (AVAudioPlayer player = AVAudioPlayer.FromUrl (NSUrl.FromString ("sdf"), out NSError error)) {
					Assert.Null (player, "AVAudioPlayer");
					Assert.NotNull (error, "NSError");
				}
			});
		}

		[Test]
		public void FromData ()
		{
			using (NSData data = NSData.FromFile (NSBundle.MainBundle.PathForResource ("Hand", "wav")))
			using (AVAudioPlayer player = AVAudioPlayer.FromData (data, out NSError error)) {
				Assert.NotNull (player, "AVAudioPlayer");
				Assert.Null (error, "NSError");
			}
		}

		[Test]
		public void FromDataWithNullData ()
		{
			Assert.Throws<ArgumentNullException> (() => {
				using (var player = AVAudioPlayer.FromData (null, out NSError error)) {
					Assert.Null (player, "AVAudioPlayer");
					Assert.NotNull (error, "NSError");
				}
			});
		}
	}
}

#endif // !__WATCHOS__
