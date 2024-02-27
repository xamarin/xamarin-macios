//
// Unit tests for MTAudioProcessingTap
//
// Authors:
//	Marek Safar (marek.safar@gmail.com)
//
// Copyright 2012 Xamarin Inc, All rights reserved.
//

#if !__WATCHOS__

using System;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;
using MediaToolbox;
using AudioToolbox;
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.MediaToolbox {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AudioProcessingTapTest {
		[DllImport (Constants.CoreFoundationLibrary)]
		extern static nint CFGetRetainCount (IntPtr handle);

		[Test]
		public unsafe void Initialization ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 9, throwIfOtherPlatform: false);

			var cb = new MTAudioProcessingTapCallbacks (
				delegate (MTAudioProcessingTap tap, nint numberFrames, MTAudioProcessingTapFlags flags, AudioBuffers bufferList, out nint numberFramesOut, out MTAudioProcessingTapFlags flagsOut)
				{
					numberFramesOut = 2;
					flagsOut = MTAudioProcessingTapFlags.StartOfStream;
				});

			cb.Initialize = delegate (MTAudioProcessingTap tap, out void* tapStorage)
			{
				tapStorage = (void*) 44;
			};

			IntPtr handle;
			using (var res = new MTAudioProcessingTap (cb, MTAudioProcessingTapCreationFlags.PreEffects)) {
				handle = res.Handle;
				Assert.AreEqual (44, (int) res.GetStorage ());
				Assert.That (CFGetRetainCount (handle), Is.EqualTo ((nint) 1), "RC");
			}
		}
	}
}

#endif // !__WATCHOS__
