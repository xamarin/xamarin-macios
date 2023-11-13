//
// Copyright 2010, Novell, Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

#if !__MACCATALYST__

using System;
using System.Runtime.InteropServices;

using ObjCRuntime;
using Foundation;
using CoreGraphics;

#if !NET
using NativeHandle = System.IntPtr;
#endif

#nullable enable

namespace AppKit {
#if NET
	[SupportedOSPlatform ("macos")]
#endif
	public static class NSGraphics {
		public static readonly float White = 1;
		public static readonly float Black = 0;
		public static readonly float LightGray = (float) 2 / 3.0f;
		public static readonly float DarkGray = (float) 1 / 3.0f;

		[DllImport (Constants.AppKitLibrary)]
		extern static NSWindowDepth NSBestDepth (IntPtr colorspaceHandle, nint bitsPerSample, nint bitsPerPixel, [MarshalAs (UnmanagedType.I1)] bool planar, [MarshalAs (UnmanagedType.I1)] ref bool exactMatch);

		public static NSWindowDepth BestDepth (NSString colorspace, nint bitsPerSample, nint bitsPerPixel, [MarshalAs (UnmanagedType.I1)] bool planar, [MarshalAs (UnmanagedType.I1)] ref bool exactMatch)
		{
			if (colorspace is null)
				throw new ArgumentNullException ("colorspace");

			return NSBestDepth (colorspace.Handle, bitsPerSample, bitsPerPixel, planar, ref exactMatch);
		}

		[DllImport (Constants.AppKitLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool NSPlanarFromDepth (NSWindowDepth depth);

		public static bool PlanarFromDepth (NSWindowDepth depth)
		{
			return NSPlanarFromDepth (depth);
		}

		[DllImport (Constants.AppKitLibrary)]
		extern static IntPtr NSColorSpaceFromDepth (NSWindowDepth depth);

		public static NSString ColorSpaceFromDepth (NSWindowDepth depth)
		{
			return new NSString (NSColorSpaceFromDepth (depth));
		}

		[DllImport (Constants.AppKitLibrary, EntryPoint = "NSBitsPerSampleFromDepth")]
		public extern static nint BitsPerSampleFromDepth (NSWindowDepth depth);

		[DllImport (Constants.AppKitLibrary, EntryPoint = "NSBitsPerPixelFromDepth")]
		public extern static nint BitsPerPixelFromDepth (NSWindowDepth depth);

		[DllImport (Constants.AppKitLibrary)]
		extern static nint NSNumberOfColorComponents (IntPtr str);

		public static nint NumberOfColorComponents (NSString colorspaceName)
		{
			if (colorspaceName is null)
				throw new ArgumentNullException ("colorspaceName");
			return NSNumberOfColorComponents (colorspaceName.Handle);
		}

		[DllImport (Constants.AppKitLibrary)]
		extern static IntPtr NSAvailableWindowDepths ();

		public static NSWindowDepth [] AvailableWindowDepths {
			get {
				IntPtr depPtr = NSAvailableWindowDepths ();
				int count;

				for (count = 0; Marshal.ReadInt32 (depPtr, count) != 0; count++)
					;

				var ret = new NSWindowDepth [count];
				for (int i = 0; i < count; count++) {
					// NSWindowDepth: int
					ret [i] = (NSWindowDepth) Marshal.ReadInt32 (depPtr, i);
				}

				return ret;

			}
		}

		[DllImport (Constants.AppKitLibrary, EntryPoint = "NSRectFill")]
		public extern static void RectFill (CGRect rect);

		[DllImport (Constants.AppKitLibrary)]
		extern static void NSRectFillUsingOperation (CGRect rect, nuint op);
		public static void RectFill (CGRect rect, NSCompositingOperation op)
		{
			NSRectFillUsingOperation (rect, (nuint) (ulong) op);
		}

		[DllImport (Constants.AppKitLibrary, EntryPoint = "NSRectFillList")]
		unsafe extern static void RectFillList (CGRect* rects, nint count);

		public static void RectFill (CGRect [] rects)
		{
			if (rects is null)
				throw new ArgumentNullException ("rects");
			unsafe {
				fixed (CGRect* ptr = rects)
					RectFillList (ptr, rects.Length);
			}
		}

		[DllImport (Constants.AppKitLibrary, EntryPoint = "NSRectClip")]
		public extern static void RectClip (CGRect rect);

		[DllImport (Constants.AppKitLibrary, EntryPoint = "NSFrameRect")]
		public extern static void FrameRect (CGRect rect);

		[DllImport (Constants.AppKitLibrary, EntryPoint = "NSFrameRectWithWidth")]
		public extern static void FrameRect (CGRect rect, nfloat frameWidth);

		// Bad naming, added the overload above
		[DllImport (Constants.AppKitLibrary, EntryPoint = "NSFrameRectWithWidth")]
		public extern static void FrameRectWithWidth (CGRect rect, nfloat frameWidth);

		[DllImport (Constants.AppKitLibrary)]
		extern static void NSFrameRectWithWidthUsingOperation (CGRect rect, nfloat frameWidth, nuint operation);
		public static void FrameRect (CGRect rect, nfloat frameWidth, NSCompositingOperation operation)
		{
			NSFrameRectWithWidthUsingOperation (rect, frameWidth, (nuint) (ulong) operation);
		}

		[DllImport (Constants.AppKitLibrary, EntryPoint = "NSShowAnimationEffect")]
		extern static void NSShowAnimationEffect (nuint animationEffect, CGPoint centerLocation, CGSize size, NativeHandle animationDelegate, NativeHandle didEndSelector, IntPtr contextInfo);

		public static void ShowAnimationEffect (NSAnimationEffect animationEffect, CGPoint centerLocation, CGSize size, NSObject animationDelegate, Selector didEndSelector, IntPtr contextInfo)
		{
			NSShowAnimationEffect ((nuint) (ulong) animationEffect, centerLocation, size, animationDelegate.GetHandle (), didEndSelector.Handle, contextInfo);
		}

		public static void ShowAnimationEffect (NSAnimationEffect animationEffect, CGPoint centerLocation, CGSize size, Action endedCallback)
		{
			var d = new NSAsyncActionDispatcher (endedCallback);
			ShowAnimationEffect (animationEffect, centerLocation, size, d, NSActionDispatcher.Selector, IntPtr.Zero);
			GC.KeepAlive (d);
		}

		public static void SetFocusRingStyle (NSFocusRingPlacement placement)
		{
			SetFocusRingStyle ((nuint) (ulong) placement);
		}

		[DllImport (Constants.AppKitLibrary, EntryPoint = "NSSetFocusRingStyle")]
		extern static void SetFocusRingStyle (nuint placement);

		[DllImport (Constants.AppKitLibrary, EntryPoint = "NSDrawWhiteBezel")]
		public extern static void DrawWhiteBezel (CGRect aRect, CGRect clipRect);

		[DllImport (Constants.AppKitLibrary, EntryPoint = "NSDrawLightBezel")]
		public extern static void DrawLightBezel (CGRect aRect, CGRect clipRect);

		[DllImport (Constants.AppKitLibrary, EntryPoint = "NSDrawGrayBezel")]
		public extern static void DrawGrayBezel (CGRect aRect, CGRect clipRect);

		[DllImport (Constants.AppKitLibrary, EntryPoint = "NSDrawDarkBezel")]
		public extern static void DrawDarkBezel (CGRect aRect, CGRect clipRect);

		[DllImport (Constants.AppKitLibrary, EntryPoint = "NSDrawGroove")]
		public extern static void DrawGroove (CGRect aRect, CGRect clipRect);

		[DllImport (Constants.AppKitLibrary, EntryPoint = "NSDrawTiledRects")]
		unsafe extern static CGRect DrawTiledRects (CGRect aRect, CGRect clipRect, NSRectEdge* sides, nfloat* grays, nint count);

		public static CGRect DrawTiledRects (CGRect aRect, CGRect clipRect, NSRectEdge [] sides, nfloat [] grays)
		{
			if (sides is null)
				throw new ArgumentNullException ("sides");
			if (grays is null)
				throw new ArgumentNullException ("grays");
			if (sides.Length != grays.Length)
				throw new ArgumentOutOfRangeException ("grays", "Both array parameters must have the same length");
			unsafe {
				fixed (NSRectEdge* ptr = sides)
				fixed (nfloat* ptr2 = grays)
					return DrawTiledRects (aRect, clipRect, ptr, ptr2, sides.Length);
			}
		}

		[DllImport (Constants.AppKitLibrary, EntryPoint = "NSDrawWindowBackground")]
		public extern static void DrawWindowBackground (CGRect aRect);

#if NET
		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("macos10.11", "Not usually necessary, 'NSAnimationContext.RunAnimation' can be used instead and not suffer from performance issues.")]
#else
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Not usually necessary, 'NSAnimationContext.RunAnimation' can be used instead and not suffer from performance issues.")]
#endif
		[DllImport (Constants.AppKitLibrary, EntryPoint = "NSDisableScreenUpdates")]
		public extern static void DisableScreenUpdates ();

#if NET
		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("macos10.11", "Not usually necessary, 'NSAnimationContext.RunAnimation' can be used instead and not suffer from performance issues.")]
#else
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Not usually necessary, 'NSAnimationContext.RunAnimation' can be used instead and not suffer from performance issues.")]
#endif
		[DllImport (Constants.AppKitLibrary, EntryPoint = "NSEnableScreenUpdates")]
		public extern static void EnableScreenUpdates ();

	}
}
#endif // !__MACCATALYST__
