//
// Unit tests for AVAudioPlayer
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
using System.IO;
using Foundation;
using AVFoundation;
using NUnit.Framework;

namespace MonoTouchFixtures.AVFoundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AudioPlayerTest {

		[Test]
		public void FromUrl ()
		{
			string file = Path.Combine (NSBundle.MainBundle.ResourcePath, "Hand.wav");
			Assert.True (File.Exists (file), file);
			NSError error;
			using (var url = new NSUrl (file, false))
			using (var ap = AVAudioPlayer.FromUrl (url, out error)) {
				Assert.NotNull (ap, "AVAudioPlayer");
				Assert.Null (error, "NSError");
			}
		}
	}
}

#endif // !__WATCHOS__
