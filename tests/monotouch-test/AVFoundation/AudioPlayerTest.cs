//
// Unit tests for AVAudioPlayer
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

using System.IO;
using Foundation;
using AVFoundation;
using NUnit.Framework;
using System;

#nullable enable

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
		public void FromUrlWithHint ()
		{
			var file = Path.Combine (NSBundle.MainBundle.ResourcePath, "Hand.wav");
			Assert.True (File.Exists (file), file);
			using var url = new NSUrl (file, false);
			{
				using var ap = AVAudioPlayer.FromUrl (url, AVFileTypes.Wave, out var error);
				Assert.NotNull (ap, "AVAudioPlayer");
				Assert.Null (error, "NSError");
			}
			{
				using var ap = AVAudioPlayer.FromUrl (url, AVFileTypes.Wave.GetConstant (), out var error);
				Assert.NotNull (ap, "AVAudioPlayer 2");
				Assert.Null (error, "NSError 2");
			}
		}

		[Test]
		public void FromInvalidUrlWithHint ()
		{
			using var url = new NSUrl ("sdf", false);
			{
				using var ap = AVAudioPlayer.FromUrl (url, AVFileTypes.Wave, out var error);
				Assert.Null (ap, "AVAudioPlayer");
				Assert.NotNull (error, "NSError");
			}
			{
				using var ap = AVAudioPlayer.FromUrl (url, AVFileTypes.Wave.GetConstant (), out var error);
				Assert.Null (ap, "AVAudioPlayer 2");
				Assert.NotNull (error, "NSError 2");
			}
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
		public void FromDataWithHint ()
		{
			using var data = NSData.FromFile (NSBundle.MainBundle.PathForResource ("Hand", "wav"));
			{
				using var player = AVAudioPlayer.FromData (data, AVFileTypes.Wave, out var error);
				Assert.NotNull (player, "AVAudioPlayer");
				Assert.Null (error, "NSError");
			}
			{
				using var player = AVAudioPlayer.FromData (data, AVFileTypes.Wave.GetConstant (), out var error);
				Assert.NotNull (player, "AVAudioPlayer 2");
				Assert.Null (error, "NSError 2");
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
