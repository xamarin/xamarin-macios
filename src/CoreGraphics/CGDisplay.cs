#nullable enable

#if MONOMAC || __MACCATALYST__
using System;
using System.Runtime.InteropServices;
using ObjCRuntime;
using Foundation;

namespace CoreGraphics
{

#if NET
	[SupportedOSPlatform ("maccatalyst15.0")]
	[SupportedOSPlatform ("macos")]
#else
	[MacCatalyst (15,0)]
#endif
	public enum CGCaptureOptions : uint
	{
		None = 0,
		NoFill = 1 << 0
	}

#if NET
	[SupportedOSPlatform ("maccatalyst15.0")]
	[SupportedOSPlatform ("macos")]
#else
	[MacCatalyst (15,0)]
#endif
	public static class CGDisplay
	{
#if !COREBUILD
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern uint CGMainDisplayID ();

		public static int MainDisplayID {
			get {
				return (int)CGMainDisplayID ();
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary, EntryPoint="CGDisplayModeGetTypeID")]
		public static extern nint GetTypeID ();

		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern CGRect CGDisplayBounds (uint display);

		public static CGRect GetBounds (int display)
		{
			return CGDisplayBounds ((uint)display);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern nuint CGDisplayPixelsWide (uint display);

		public static nint GetWidth (int display)
		{
			return (nint)CGDisplayPixelsWide ((uint)display);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern nuint CGDisplayPixelsHigh (uint display);

		public static nint GetHeight (int display)
		{
			return (nint)CGDisplayPixelsHigh ((uint)display);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern int CGSetDisplayTransferByFormula (uint display, float redMin, float redMax, float redGamma, float greenMin, float greenMax, float greenGamma, float blueMin, float blueMax, float blueGamma);

		public static int SetDisplayTransfer (int display, float redMin, float redMax, float redGamma, float greenMin, float greenMax, float greenGamma, float blueMin, float blueMax, float blueGamma)
		{
			return CGSetDisplayTransferByFormula ((uint)display, redMin, redMax, redGamma, greenMin, greenMax, greenGamma, blueMin, blueMax, blueGamma);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern uint CGDisplayGammaTableCapacity (uint display);

		public static int GetGammaTableCapacity (int display)
		{
			return (int)CGDisplayGammaTableCapacity ((uint)display);
		}

		[DllImport (Constants.CoreGraphicsLibrary, EntryPoint = "CGDisplayRestoreColorSyncSettings")]
		public static extern void RestoreColorSyncSettings ();

#if NET
		[SupportedOSPlatform ("maccatalyst15.0")]
		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("macos10.9")]
#else
		[Deprecated (PlatformName.MacOSX, 10, 9)]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool CGDisplayIsCaptured (uint display);

		public static bool IsCaptured (int display)
		{
			return CGDisplayIsCaptured ((uint)display);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern int CGDisplayCapture (uint display);

		public static int Capture (int display)
		{
			return CGDisplayCapture ((uint)display);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern int CGDisplayCaptureWithOptions (uint display, CGCaptureOptions options);

		public static int Capture (int display, CGCaptureOptions options)
		{
			return CGDisplayCaptureWithOptions ((uint)display, options);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern int CGDisplayRelease (uint display);

		public static int Release (int display)
		{
			return CGDisplayRelease ((uint)display);
		}

		[DllImport (Constants.CoreGraphicsLibrary, EntryPoint = "CGCaptureAllDisplays")]
		public static extern int CaptureAllDisplays ();

		[DllImport (Constants.CoreGraphicsLibrary, EntryPoint="CGCaptureAllDisplaysWithOptions")]
		static extern int CaptureAllDisplays (CGCaptureOptions options);


		[DllImport (Constants.CoreGraphicsLibrary, EntryPoint = "CGReleaseAllDisplays")]
		public static extern int ReleaseAllDisplays ();

		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern int CGDisplayHideCursor (uint display);

		public static int HideCursor (int display)
		{
			return CGDisplayHideCursor ((uint)display);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern int CGDisplayShowCursor (uint display);

		public static int ShowCursor (int display)
		{
			return CGDisplayShowCursor ((uint)display);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern int CGDisplayMoveCursorToPoint (uint display, CGPoint point);

		public static int MoveCursor (int display, CGPoint point)
		{
			return CGDisplayMoveCursorToPoint ((uint)display, point);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern uint CGDisplayIDToOpenGLDisplayMask (uint display);

		public static int GetOpenGLDisplayMask (int display)
		{
			return (int)CGDisplayIDToOpenGLDisplayMask ((uint)display);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern uint CGOpenGLDisplayMaskToDisplayID (uint mask);

		public static int GetDisplayID (int displayMask)
		{
			return (int)CGOpenGLDisplayMaskToDisplayID ((uint)displayMask);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern uint CGShieldingWindowID (uint display);

		public static int GetShieldingWindowID (int display)
		{
			return (int)CGShieldingWindowID ((uint)display);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern int CGShieldingWindowLevel ();

		public static int ShieldingWindowLevel {
			get { return CGShieldingWindowLevel (); }
		}
#endif
	}
}
#endif
