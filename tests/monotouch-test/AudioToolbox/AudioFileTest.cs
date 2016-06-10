// Copyright 2011 Xamarin Inc. All rights reserved

#if !__WATCHOS__

using System;
using System.Drawing;
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

namespace MonoTouchFixtures.AudioToolbox {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AudioFileTest {
		
		[Test]
		public void ReadTest ()
		{
			var path = Path.GetFullPath (Path.Combine ("AudioToolbox", "1.caf"));
			var af = AudioFile.Open (CFUrl.FromFile (path), AudioFilePermission.Read, AudioFileType.CAF);
		
			int len;
			long current = 0;
			long size = 1024;
			byte [] buffer = new byte [size];
			while ((len = af.Read (current, buffer, 0, buffer.Length, false)) != -1) {
				current += len;
			}
		
			var full_len = new FileInfo (path).Length;
			int header = 4096;
			Assert.That (header + current == full_len, "#1");
		}
	}
}

#endif // !__WATCHOS__
