//
// Unit tests for AudioStreamBasicDescription
//
// Authors:
//	Marek Safar (marek.safar@gmail.com)
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
using System.IO;
using Foundation;
using AudioToolbox;
using CoreFoundation;
using NUnit.Framework;
using System.Threading;

namespace MonoTouchFixtures.AudioToolbox {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AudioStreamBasicDescriptionTest
	{
		[Test]
		public void CreateLinearPCM ()
		{
			var pcm = AudioStreamBasicDescription.CreateLinearPCM ();
			Assert.IsNotNull (pcm.FormatName);
			Assert.IsFalse (pcm.IsVariableBitrate);
		}

		[Test]
		public void VBR ()
		{
			var mp3 = new AudioStreamBasicDescription (AudioFormatType.MPEGLayer3);
			Assert.IsTrue (mp3.IsVariableBitrate);
		}

		[Test]
		public void GetFormatInfo ()
		{
			var asbd = new AudioStreamBasicDescription (AudioFormatType.MPEG4AAC);
			Assert.AreEqual (AudioFormatError.None, AudioStreamBasicDescription.GetFormatInfo (ref asbd));

			Assert.IsNotNull (AudioStreamBasicDescription.GetAvailableEncodeChannelLayoutTags (asbd));
			Assert.IsNotNull (AudioStreamBasicDescription.GetAvailableEncodeNumberChannels (asbd));
			Assert.IsNotNull (asbd.GetOutputFormatList ());
		}
	}
}

#endif // !__WATCHOS__
