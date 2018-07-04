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
#if XAMCORE_2_0
using Foundation;
using ObjCRuntime;
using MediaToolbox;
using AudioToolbox;
#else
using MonoTouch;
using MonoTouch.Foundation;
using MonoTouch.MediaToolbox;
using MonoTouch.AudioToolbox;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

#if XAMCORE_2_0
using RectangleF=CoreGraphics.CGRect;
using SizeF=CoreGraphics.CGSize;
using PointF=CoreGraphics.CGPoint;
#else
using nfloat=global::System.Single;
using nint=global::System.Int32;
using nuint=global::System.UInt32;
#endif

namespace MonoTouchFixtures.MediaToolbox
{
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AudioProcessingTapTest
	{
		[DllImport (Constants.CoreFoundationLibrary)]
		extern static nint CFGetRetainCount (IntPtr handle);

		[Test]
		public unsafe void Initialization ()
		{
			TestRuntime.AssertSystemVersion (PlatformName.MacOSX, 10, 9, throwIfOtherPlatform: false);

			var cb = new MTAudioProcessingTapCallbacks (
#if XAMCORE_2_0
				delegate(MTAudioProcessingTap tap, nint numberFrames, MTAudioProcessingTapFlags flags, AudioBuffers bufferList, out nint numberFramesOut, out MTAudioProcessingTapFlags flagsOut) {
#else
				delegate(MTAudioProcessingTap tap, long numberFrames, MTAudioProcessingTapFlags flags, AudioBuffers bufferList, out long numberFramesOut, out MTAudioProcessingTapFlags flagsOut) {
#endif
					numberFramesOut = 2;
					flagsOut = MTAudioProcessingTapFlags.StartOfStream;
			});
			
			cb.Initialize = delegate(MTAudioProcessingTap tap, out void* tapStorage) {
				tapStorage = (void*)44;
			};

			IntPtr handle;
			using (var res = new MTAudioProcessingTap (cb, MTAudioProcessingTapCreationFlags.PreEffects))
			{
				handle = res.Handle;
				Assert.AreEqual (44, (int)res.GetStorage ());
				Assert.That (CFGetRetainCount (handle), Is.EqualTo ((nint) 1), "RC");
			}
		}
	}
}

#endif // !__WATCHOS__
