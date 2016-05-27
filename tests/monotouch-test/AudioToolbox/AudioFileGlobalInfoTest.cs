//
// Unit tests for AudioFileGlobalInfo
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
	public class AudioFileGlobalInfoTest
	{
		[Test]
		public void Properties ()
		{
			var a1 = AudioFileGlobalInfo.ReadableTypes;
			var a2 = AudioFileGlobalInfo.GetFileTypeName (AudioFileType.MP2);
			var a3 = AudioFileGlobalInfo.GetAvailableFormats (AudioFileType.MPEG4);
			var a4 = AudioFileGlobalInfo.GetAvailableStreamDescriptions (AudioFileType.MPEG4, AudioFormatType.MPEG4AAC);
			var a5 = AudioFileGlobalInfo.AllExtensions;
			var a6 = AudioFileGlobalInfo.GetExtensions (AudioFileType.MPEG4);
			var a7 = AudioFileGlobalInfo.GetMIMETypes (AudioFileType.MPEG4);
			var a8 = AudioFileGlobalInfo.GetUTIs (AudioFileType.MPEG4);
		}
	}
}

#endif // !__WATCHOS__
