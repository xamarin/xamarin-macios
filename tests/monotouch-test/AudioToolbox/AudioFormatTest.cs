//
// Unit tests for AudioFormat
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
	public class AudioFormatTest {
		[Test]
		public void GetFirstPlayableFormat ()
		{
			var asbd = new AudioStreamBasicDescription (AudioFormatType.MPEG4AAC);
			AudioStreamBasicDescription.GetFormatInfo (ref asbd);

			var ofl = asbd.GetOutputFormatList ();

			Assert.IsNotNull (AudioFormat.GetFirstPlayableFormat (ofl));
		}
	}
}

#endif // !__WATCHOS__
