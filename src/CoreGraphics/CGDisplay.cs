#nullable enable

#if MONOMAC || __MACCATALYST__

namespace CoreGraphics {
	/// <summary>Options that control display capture behavior.</summary>
	/// <remarks>These options can be used when capturing one display or all displays.</remarks>
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	public enum CGCaptureOptions : uint {
		/// <summary>Uses the default display-capture behavior.</summary>
		None = 0,
		/// <summary>Disables filling captured display areas with black; Apple has deprecated this option.</summary>
		NoFill = 1 << 0,
	}

	/// <summary>To be added.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	public static class CGDisplay {
#if !COREBUILD
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern uint CGMainDisplayID ();

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public static int MainDisplayID {
			get {
				return (int) CGMainDisplayID ();
			}
		}

		/// <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[DllImport (Constants.CoreGraphicsLibrary, EntryPoint = "CGDisplayModeGetTypeID")]
		public static extern nint GetTypeID ();

		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern CGRect CGDisplayBounds (uint display);

		/// <param name="display">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public static CGRect GetBounds (int display)
		{
			return CGDisplayBounds ((uint) display);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern nuint CGDisplayPixelsWide (uint display);

		/// <param name="display">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public static nint GetWidth (int display)
		{
			return (nint) CGDisplayPixelsWide ((uint) display);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern nuint CGDisplayPixelsHigh (uint display);

		/// <param name="display">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public static nint GetHeight (int display)
		{
			return (nint) CGDisplayPixelsHigh ((uint) display);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern int CGSetDisplayTransferByFormula (uint display, float redMin, float redMax, float redGamma, float greenMin, float greenMax, float greenGamma, float blueMin, float blueMax, float blueGamma);

		/// <param name="display">To be added.</param>
		///         <param name="redMin">To be added.</param>
		///         <param name="redMax">To be added.</param>
		///         <param name="redGamma">To be added.</param>
		///         <param name="greenMin">To be added.</param>
		///         <param name="greenMax">To be added.</param>
		///         <param name="greenGamma">To be added.</param>
		///         <param name="blueMin">To be added.</param>
		///         <param name="blueMax">To be added.</param>
		///         <param name="blueGamma">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public static int SetDisplayTransfer (int display, float redMin, float redMax, float redGamma, float greenMin, float greenMax, float greenGamma, float blueMin, float blueMax, float blueGamma)
		{
			return CGSetDisplayTransferByFormula ((uint) display, redMin, redMax, redGamma, greenMin, greenMax, greenGamma, blueMin, blueMax, blueGamma);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern uint CGDisplayGammaTableCapacity (uint display);

		/// <param name="display">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public static int GetGammaTableCapacity (int display)
		{
			return (int) CGDisplayGammaTableCapacity ((uint) display);
		}

		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[DllImport (Constants.CoreGraphicsLibrary, EntryPoint = "CGDisplayRestoreColorSyncSettings")]
		public static extern void RestoreColorSyncSettings ();

		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("macos10.9")]
		[ObsoletedOSPlatform ("maccatalyst13.1")]
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern byte CGDisplayIsCaptured (uint display);

		/// <param name="display">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("macos10.9")]
		[ObsoletedOSPlatform ("maccatalyst13.1")]
		public static bool IsCaptured (int display)
		{
			return CGDisplayIsCaptured ((uint) display) != 0;
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern int CGDisplayCapture (uint display);

		/// <param name="display">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public static int Capture (int display)
		{
			return CGDisplayCapture ((uint) display);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern int CGDisplayCaptureWithOptions (uint display, CGCaptureOptions options);

		/// <param name="display">To be added.</param>
		///         <param name="options">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public static int Capture (int display, CGCaptureOptions options)
		{
			return CGDisplayCaptureWithOptions ((uint) display, options);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern int CGDisplayRelease (uint display);

		/// <param name="display">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public static int Release (int display)
		{
			return CGDisplayRelease ((uint) display);
		}

		/// <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[DllImport (Constants.CoreGraphicsLibrary, EntryPoint = "CGCaptureAllDisplays")]
		public static extern int CaptureAllDisplays ();

		[DllImport (Constants.CoreGraphicsLibrary, EntryPoint = "CGCaptureAllDisplaysWithOptions")]
		static extern int CaptureAllDisplays (CGCaptureOptions options);


		/// <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[DllImport (Constants.CoreGraphicsLibrary, EntryPoint = "CGReleaseAllDisplays")]
		public static extern int ReleaseAllDisplays ();

		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern int CGDisplayHideCursor (uint display);

		/// <param name="display">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public static int HideCursor (int display)
		{
			return CGDisplayHideCursor ((uint) display);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern int CGDisplayShowCursor (uint display);

		/// <param name="display">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public static int ShowCursor (int display)
		{
			return CGDisplayShowCursor ((uint) display);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern int CGDisplayMoveCursorToPoint (uint display, CGPoint point);

		/// <param name="display">To be added.</param>
		///         <param name="point">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public static int MoveCursor (int display, CGPoint point)
		{
			return CGDisplayMoveCursorToPoint ((uint) display, point);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern uint CGDisplayIDToOpenGLDisplayMask (uint display);

		/// <param name="display">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public static int GetOpenGLDisplayMask (int display)
		{
			return (int) CGDisplayIDToOpenGLDisplayMask ((uint) display);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern uint CGOpenGLDisplayMaskToDisplayID (uint mask);

		/// <param name="displayMask">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public static int GetDisplayID (int displayMask)
		{
			return (int) CGOpenGLDisplayMaskToDisplayID ((uint) displayMask);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern uint CGShieldingWindowID (uint display);

		/// <param name="display">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public static int GetShieldingWindowID (int display)
		{
			return (int) CGShieldingWindowID ((uint) display);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern int CGShieldingWindowLevel ();

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public static int ShieldingWindowLevel {
			get { return CGShieldingWindowLevel (); }
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static IntPtr CGDisplayCopyDisplayMode (uint display);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGDisplayModeRelease (IntPtr mode);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static double CGDisplayModeGetRefreshRate (IntPtr mode);

		/// <summary>Get the refresh rate for the specified display.</summary>
		/// <param name="display">The identifier for the display.</param>
		/// <returns>The display rate, in hertz, of the specified display, or <see langword="null" /> in case of failure.</returns>
		public static double? GetRefreshRate (int display)
		{
			var mode = CGDisplayCopyDisplayMode ((uint) display);
			if (mode == IntPtr.Zero)
				return null;
			var refreshRate = CGDisplayModeGetRefreshRate (mode);
			CGDisplayModeRelease (mode);
			return refreshRate;
		}
#endif
	}
}
#endif
