//
// Unit tests for AudioFormat
//
// Authors:
//	Marek Safar (marek.safar@gmail.com)
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
using System.IO;
#if XAMCORE_2_0
using Foundation;
using MediaPlayer;
using AudioToolbox;
using CoreFoundation;
#else
using MonoTouch.Foundation;
using MonoTouch.MediaPlayer;
using MonoTouch.AudioToolbox;
using MonoTouch.CoreFoundation;
#endif
using NUnit.Framework;
using System.Threading;

namespace MonoTouchFixtures.AudioToolbox {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AudioFormatTest
	{
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
