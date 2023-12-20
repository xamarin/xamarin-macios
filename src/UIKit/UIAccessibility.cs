//
// This file describes the API that the generator will produce
//
// Authors:
//   Miguel de Icaza
//
// Copyrigh 2012-2014, Xamarin Inc.
//

#if !WATCH

using Foundation;
using ObjCRuntime;
using UIKit;
using CoreGraphics;

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace UIKit {

	// helper enum - not part of Apple API
	public enum UIAccessibilityPostNotification {
		Announcement,
		LayoutChanged,
		PageScrolled,
		ScreenChanged,
	}

	// NSInteger -> UIAccessibilityZoom.h
	[Native]
	public enum UIAccessibilityZoomType : long {
		InsertionPoint,
	}

	public static partial class UIAccessibility {
		// UIAccessibility.h
		[DllImport (Constants.UIKitLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static /* BOOL */ bool UIAccessibilityIsVoiceOverRunning ();

		static public bool IsVoiceOverRunning {
			get {
				return UIAccessibilityIsVoiceOverRunning ();
			}
		}

		// UIAccessibility.h
		[DllImport (Constants.UIKitLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static /* BOOL */ bool UIAccessibilityIsMonoAudioEnabled ();

		static public bool IsMonoAudioEnabled {
			get {
				return UIAccessibilityIsMonoAudioEnabled ();
			}
		}


		// UIAccessibility.h
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		[DllImport (Constants.UIKitLibrary)]
		extern static /* NSObject */ IntPtr UIAccessibilityFocusedElement (IntPtr assistiveTechnologyIdentifier);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public static NSObject FocusedElement (string assistiveTechnologyIdentifier)
		{
			using (var s = new NSString (assistiveTechnologyIdentifier))
				return Runtime.GetNSObject (UIAccessibilityFocusedElement (s.Handle));
		}

		// UIAccessibility.h
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		[DllImport (Constants.UIKitLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static /* BOOL */ bool UIAccessibilityIsShakeToUndoEnabled ();

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public static bool IsShakeToUndoEnabled {
			get {
				return UIAccessibilityIsShakeToUndoEnabled ();
			}
		}

		// UIAccessibility.h
		[DllImport (Constants.UIKitLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static /* BOOL */ bool UIAccessibilityIsClosedCaptioningEnabled ();

		static public bool IsClosedCaptioningEnabled {
			get {
				return UIAccessibilityIsClosedCaptioningEnabled ();
			}
		}

		// UIAccessibility.h
		[DllImport (Constants.UIKitLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static /* BOOL */ bool UIAccessibilityIsInvertColorsEnabled ();

		static public bool IsInvertColorsEnabled {
			get {
				return UIAccessibilityIsInvertColorsEnabled ();
			}
		}

		// UIAccessibility.h
		[DllImport (Constants.UIKitLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static /* BOOL */ bool UIAccessibilityIsGuidedAccessEnabled ();

		static public bool IsGuidedAccessEnabled {
			get {
				return UIAccessibilityIsGuidedAccessEnabled ();
			}
		}

		// UIAccessibility.h
		[DllImport (Constants.UIKitLibrary)]
		extern static void UIAccessibilityPostNotification (/* UIAccessibilityNotifications */ int notification, /* id */ IntPtr argument);
		// typedef uint32_t UIAccessibilityNotifications

		public static void PostNotification (UIAccessibilityPostNotification notification, NSObject argument)
		{
			PostNotification (NotificationEnumToInt (notification), argument);
		}

		public static void PostNotification (int notification, NSObject argument)
		{
			UIAccessibilityPostNotification (notification, argument is null ? IntPtr.Zero : argument.Handle);
		}

		static int NotificationEnumToInt (UIAccessibilityPostNotification notification)
		{
			switch (notification) {
			case UIKit.UIAccessibilityPostNotification.Announcement:
				return UIView.AnnouncementNotification;
			case UIKit.UIAccessibilityPostNotification.LayoutChanged:
				return UIView.LayoutChangedNotification;
			case UIKit.UIAccessibilityPostNotification.PageScrolled:
				return UIView.PageScrolledNotification;
			case UIKit.UIAccessibilityPostNotification.ScreenChanged:
				return UIView.ScreenChangedNotification;
			default:
				throw new ArgumentOutOfRangeException (string.Format ("Unknown UIAccessibilityPostNotification: {0}", notification.ToString ()));
			}
		}

		// UIAccessibilityZoom.h
		[DllImport (Constants.UIKitLibrary)]
		extern static void UIAccessibilityZoomFocusChanged (/* UIAccessibilityZoomType */ IntPtr type, CGRect frame, IntPtr view);

		public static void ZoomFocusChanged (UIAccessibilityZoomType type, CGRect frame, UIView view)
		{
			UIAccessibilityZoomFocusChanged ((IntPtr) type, frame, view is not null ? view.Handle : IntPtr.Zero);
		}

		// UIAccessibilityZoom.h
		[DllImport (Constants.UIKitLibrary, EntryPoint = "UIAccessibilityRegisterGestureConflictWithZoom")]
		extern public static void RegisterGestureConflictWithZoom ();

		// UIAccessibility.h
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		[DllImport (Constants.UIKitLibrary)]
		extern static /* UIBezierPath* */ IntPtr UIAccessibilityConvertPathToScreenCoordinates (/* UIBezierPath* */ IntPtr path, /* UIView* */ IntPtr view);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public static UIBezierPath ConvertPathToScreenCoordinates (UIBezierPath path, UIView view)
		{
			if (path is null)
				throw new ArgumentNullException ("path");
			if (view is null)
				throw new ArgumentNullException ("view");

			return new UIBezierPath (UIAccessibilityConvertPathToScreenCoordinates (path.Handle, view.Handle));
		}

		// UIAccessibility.h
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		[DllImport (Constants.UIKitLibrary)]
		extern static CGRect UIAccessibilityConvertFrameToScreenCoordinates (CGRect rect, /* UIView* */ IntPtr view);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public static CGRect ConvertFrameToScreenCoordinates (CGRect rect, UIView view)
		{
			if (view is null)
				throw new ArgumentNullException ("view");

			return UIAccessibilityConvertFrameToScreenCoordinates (rect, view.Handle);
		}

		// UIAccessibility.h
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		[DllImport (Constants.UIKitLibrary)]
		extern unsafe static void UIAccessibilityRequestGuidedAccessSession (/* BOOL */ [MarshalAs (UnmanagedType.I1)] bool enable, /* void(^completionHandler)(BOOL didSucceed) */ BlockLiteral* completionHandler);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		[BindingImpl (BindingImplOptions.Optimizable)]
		public static void RequestGuidedAccessSession (bool enable, Action<bool> completionHandler)
		{
			unsafe {
#if NET
				delegate* unmanaged<IntPtr, byte, void> trampoline = &TrampolineRequestGuidedAccessSession;
				using var block = new BlockLiteral (trampoline, completionHandler, typeof (UIAccessibility), nameof (TrampolineRequestGuidedAccessSession));
#else
				using var block = new BlockLiteral ();
				block.SetupBlock (callback, completionHandler);
#endif
				UIAccessibilityRequestGuidedAccessSession (enable, &block);
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public static Task<bool> RequestGuidedAccessSessionAsync (bool enable)
		{
			var tcs = new TaskCompletionSource<bool> ();
			RequestGuidedAccessSession (enable, (result) => {
				tcs.SetResult (result);
			});
			return tcs.Task;
		}

#if !NET
		internal delegate void InnerRequestGuidedAccessSession (IntPtr block, byte enable);
		static readonly InnerRequestGuidedAccessSession callback = TrampolineRequestGuidedAccessSession;

		[MonoPInvokeCallback (typeof (InnerRequestGuidedAccessSession))]
#else
		[UnmanagedCallersOnly]
#endif
		static unsafe void TrampolineRequestGuidedAccessSession (IntPtr block, byte enable)
		{
			var descriptor = (BlockLiteral*) block;
			var del = (Action<bool>) (descriptor->Target);
			if (del is not null)
				del (enable != 0);
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		[DllImport (Constants.UIKitLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool UIAccessibilityDarkerSystemColorsEnabled ();

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public static bool DarkerSystemColorsEnabled {
			get {
				return UIAccessibilityDarkerSystemColorsEnabled ();
			}
		}

#if !NET
		[Obsolete ("Use 'DarkerSystemColorsEnabled' instead.")]
		public static bool DarkerSystemColosEnabled {
			get {
				return UIAccessibilityDarkerSystemColorsEnabled ();
			}
		}
#endif

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		[DllImport (Constants.UIKitLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool UIAccessibilityIsBoldTextEnabled ();

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public static bool IsBoldTextEnabled {
			get {
				return UIAccessibilityIsBoldTextEnabled ();
			}
		}

#if NET
		[SupportedOSPlatform ("tvos14.0")]
		[SupportedOSPlatform ("ios14.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[TV (14, 0)]
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
#endif
		[DllImport (Constants.UIKitLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool UIAccessibilityButtonShapesEnabled ();

#if NET
		[SupportedOSPlatform ("tvos14.0")]
		[SupportedOSPlatform ("ios14.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[TV (14, 0)]
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
#endif
		public static bool ButtonShapesEnabled => UIAccessibilityButtonShapesEnabled ();

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		[DllImport (Constants.UIKitLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool UIAccessibilityIsGrayscaleEnabled ();

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		static public bool IsGrayscaleEnabled {
			get {
				return UIAccessibilityIsGrayscaleEnabled ();
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		[DllImport (Constants.UIKitLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool UIAccessibilityIsReduceMotionEnabled ();

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		static public bool IsReduceMotionEnabled {
			get {
				return UIAccessibilityIsReduceMotionEnabled ();
			}
		}

#if NET
		[SupportedOSPlatform ("tvos14.0")]
		[SupportedOSPlatform ("ios14.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[TV (14, 0)]
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
#endif
		[DllImport (Constants.UIKitLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool UIAccessibilityPrefersCrossFadeTransitions ();

#if NET
		[SupportedOSPlatform ("tvos14.0")]
		[SupportedOSPlatform ("ios14.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[TV (14, 0)]
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
#endif
		public static bool PrefersCrossFadeTransitions => UIAccessibilityPrefersCrossFadeTransitions ();

#if NET
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[iOS (13, 0)]
		[TV (13, 0)]
#endif
		[DllImport (Constants.UIKitLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool UIAccessibilityIsVideoAutoplayEnabled ();

#if NET
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[iOS (13, 0)]
		[TV (13, 0)]
#endif
		static public bool IsVideoAutoplayEnabled => UIAccessibilityIsVideoAutoplayEnabled ();

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		[DllImport (Constants.UIKitLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool UIAccessibilityIsReduceTransparencyEnabled ();

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		static public bool IsReduceTransparencyEnabled {
			get {
				return UIAccessibilityIsReduceTransparencyEnabled ();
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		[DllImport (Constants.UIKitLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool UIAccessibilityIsSwitchControlRunning ();

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		static public bool IsSwitchControlRunning {
			get {
				return UIAccessibilityIsSwitchControlRunning ();
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		[DllImport (Constants.UIKitLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool UIAccessibilityIsSpeakSelectionEnabled ();

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		static public bool IsSpeakSelectionEnabled {
			get {
				return UIAccessibilityIsSpeakSelectionEnabled ();
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		[DllImport (Constants.UIKitLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool UIAccessibilityIsSpeakScreenEnabled ();

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		static public bool IsSpeakScreenEnabled {
			get {
				return UIAccessibilityIsSpeakScreenEnabled ();
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		[DllImport (Constants.UIKitLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool UIAccessibilityIsAssistiveTouchRunning ();

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		public static bool IsAssistiveTouchRunning {
			get {
				return UIAccessibilityIsAssistiveTouchRunning ();
			}
		}

#if NET
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[iOS (13, 0)]
		[TV (13, 0)]
#endif
		[DllImport (Constants.UIKitLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool UIAccessibilityShouldDifferentiateWithoutColor ();

#if NET
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[iOS (13, 0)]
		[TV (13, 0)]
#endif
		public static bool ShouldDifferentiateWithoutColor => UIAccessibilityShouldDifferentiateWithoutColor ();

#if NET
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[iOS (13, 0)]
		[TV (13, 0)]
#endif
		[DllImport (Constants.UIKitLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool UIAccessibilityIsOnOffSwitchLabelsEnabled ();

#if NET
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[iOS (13, 0)]
		[TV (13, 0)]
#endif
		public static bool IsOnOffSwitchLabelsEnabled => UIAccessibilityIsOnOffSwitchLabelsEnabled ();

#if !TVOS
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		[DllImport (Constants.UIKitLibrary)]
		static extern nuint UIAccessibilityHearingDevicePairedEar ();

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public static UIAccessibilityHearingDeviceEar HearingDevicePairedEar {
			get {
				return (UIAccessibilityHearingDeviceEar) (ulong) UIAccessibilityHearingDevicePairedEar ();
			}
		}
#endif
	}


}

#endif // !WATCH
