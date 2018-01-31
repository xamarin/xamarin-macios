//
// This file describes the API that the generator will produce
//
// Authors:
//   Miguel de Icaza
//
// Copyrigh 2012-2014, Xamarin Inc.
//

#if !WATCH

using XamCore.Foundation;
using XamCore.ObjCRuntime;
using XamCore.UIKit;
using XamCore.CoreGraphics;

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace XamCore.UIKit {

	// helper enum - not part of Apple API
	public enum UIAccessibilityPostNotification
	{
		Announcement,
		LayoutChanged,
		PageScrolled,
		ScreenChanged,
	}

	// NSInteger -> UIAccessibilityZoom.h
	[Native]
	public enum UIAccessibilityZoomType : nint {
		InsertionPoint,
	}

	public static partial class UIAccessibility {
		// UIAccessibility.h
		[DllImport (Constants.UIKitLibrary)]
		extern static /* BOOL */ bool UIAccessibilityIsVoiceOverRunning ();

		static public bool IsVoiceOverRunning {
			get {
				return UIAccessibilityIsVoiceOverRunning ();
			}
		}
		
		// UIAccessibility.h
		[DllImport (Constants.UIKitLibrary)]
		extern static /* BOOL */ bool UIAccessibilityIsMonoAudioEnabled ();

		static public bool IsMonoAudioEnabled {
			get {
				return UIAccessibilityIsMonoAudioEnabled ();
			}
		}

		
		// UIAccessibility.h
		[iOS (9,0)]
		[DllImport (Constants.UIKitLibrary)]
		extern static /* NSObject */ IntPtr UIAccessibilityFocusedElement (IntPtr assistiveTechnologyIdentifier);

		[iOS (9,0)]
		public static NSObject FocusedElement (string assistiveTechnologyIdentifier)
		{
			using (var s = new NSString (assistiveTechnologyIdentifier))
				return Runtime.GetNSObject (UIAccessibilityFocusedElement (s.Handle));
		}

		// UIAccessibility.h
		[iOS (9,0)]
		[DllImport (Constants.UIKitLibrary)]
		extern static /* BOOL */ bool UIAccessibilityIsShakeToUndoEnabled ();

		[iOS (9,0)]
		public static bool IsShakeToUndoEnabled {
			get {
				return UIAccessibilityIsShakeToUndoEnabled ();
			}
		}
		
		// UIAccessibility.h
		[DllImport (Constants.UIKitLibrary)]
		extern static /* BOOL */ bool UIAccessibilityIsClosedCaptioningEnabled ();

		static public bool IsClosedCaptioningEnabled {
			get {
				return UIAccessibilityIsClosedCaptioningEnabled ();
			}
		}
		
		// UIAccessibility.h
		[iOS (6,0)]
		[DllImport (Constants.UIKitLibrary)]
		extern static /* BOOL */ bool UIAccessibilityIsInvertColorsEnabled ();

		[iOS (6,0)]
		static public bool IsInvertColorsEnabled {
			get {
				return UIAccessibilityIsInvertColorsEnabled ();
			}
		}
		
		// UIAccessibility.h
		[iOS (6,0)]
		[DllImport (Constants.UIKitLibrary)]
		extern static /* BOOL */ bool UIAccessibilityIsGuidedAccessEnabled ();

		[iOS (6,0)]
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
			UIAccessibilityPostNotification (notification, argument == null ? IntPtr.Zero : argument.Handle);
		}

		static int NotificationEnumToInt (UIAccessibilityPostNotification notification)
		{
			switch (notification)
			{
			case XamCore.UIKit.UIAccessibilityPostNotification.Announcement:
				return UIView.AnnouncementNotification;
			case XamCore.UIKit.UIAccessibilityPostNotification.LayoutChanged:
				return UIView.LayoutChangedNotification;
			case XamCore.UIKit.UIAccessibilityPostNotification.PageScrolled:
				return UIView.PageScrolledNotification;
			case XamCore.UIKit.UIAccessibilityPostNotification.ScreenChanged:
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
			UIAccessibilityZoomFocusChanged ((IntPtr) type, frame, view != null ? view.Handle : IntPtr.Zero);
		}

		// UIAccessibilityZoom.h
		[DllImport (Constants.UIKitLibrary, EntryPoint = "UIAccessibilityRegisterGestureConflictWithZoom")]
		extern public static void RegisterGestureConflictWithZoom ();

		// UIAccessibility.h
		[iOS (7,0)]
		[DllImport (Constants.UIKitLibrary)]
		extern static /* UIBezierPath* */ IntPtr UIAccessibilityConvertPathToScreenCoordinates (/* UIBezierPath* */ IntPtr path, /* UIView* */ IntPtr view);

		[iOS (7,0)]
		public static UIBezierPath ConvertPathToScreenCoordinates (UIBezierPath path, UIView view)
		{
			if (path == null)
				throw new ArgumentNullException ("path");
			if (view == null)
				throw new ArgumentNullException ("view");

			return new UIBezierPath (UIAccessibilityConvertPathToScreenCoordinates (path.Handle, view.Handle));
		}

		// UIAccessibility.h
		[iOS (7,0)]
		[DllImport (Constants.UIKitLibrary)]
		extern static CGRect UIAccessibilityConvertFrameToScreenCoordinates (CGRect rect, /* UIView* */ IntPtr view);

		[iOS (7,0)]
		public static CGRect ConvertFrameToScreenCoordinates (CGRect rect, UIView view)
		{
			if (view == null)
				throw new ArgumentNullException ("view");

			return UIAccessibilityConvertFrameToScreenCoordinates (rect, view.Handle);
		}

		// UIAccessibility.h
		[iOS (7,0)]
		[DllImport (Constants.UIKitLibrary)]
		extern unsafe static void UIAccessibilityRequestGuidedAccessSession (/* BOOL */ bool enable, /* void(^completionHandler)(BOOL didSucceed) */ void * completionHandler);

		[iOS (7,0)]
		[BindingImpl (BindingImplOptions.Optimizable)]
		public static void RequestGuidedAccessSession (bool enable, Action<bool> completionHandler)
		{
			unsafe {
				BlockLiteral *block_ptr_handler;
				BlockLiteral block_handler;
				block_handler = new BlockLiteral ();
				block_ptr_handler = &block_handler;
				block_handler.SetupBlock (callback, completionHandler);

				UIAccessibilityRequestGuidedAccessSession (enable, (void*) block_ptr_handler);
				block_ptr_handler->CleanupBlock ();
			}
		}

		[iOS (7,0)]
		public static Task<bool> RequestGuidedAccessSessionAsync (bool enable)
		{
			var tcs = new TaskCompletionSource<bool> ();
			RequestGuidedAccessSession (enable, (result) => {
				tcs.SetResult (result);
			});
			return tcs.Task;
		}
		
		internal delegate void InnerRequestGuidedAccessSession (IntPtr block, bool enable);
		static readonly InnerRequestGuidedAccessSession callback = TrampolineRequestGuidedAccessSession;

		[MonoPInvokeCallback (typeof (InnerRequestGuidedAccessSession))]
		static unsafe void TrampolineRequestGuidedAccessSession (IntPtr block, bool enable)
		{
			var descriptor = (BlockLiteral *) block;
			var del = (Action<bool>) (descriptor->Target);
			if (del != null)
				del (enable);
		}

		[iOS (8,0)]
		[DllImport (Constants.UIKitLibrary)]
		static extern bool UIAccessibilityDarkerSystemColorsEnabled ();

		[iOS (8,0)]
		public static bool DarkerSystemColosEnabled {
			get {
				return UIAccessibilityDarkerSystemColorsEnabled ();
			}
		}

		[iOS (8,0)]
		[DllImport (Constants.UIKitLibrary)]
		static extern bool UIAccessibilityIsBoldTextEnabled ();

		[iOS (8,0)]
		public static bool IsBoldTextEnabled {
			get {
				return UIAccessibilityIsBoldTextEnabled ();	
			}
		}

		[iOS (8,0)]
		[DllImport (Constants.UIKitLibrary)]
		static extern bool UIAccessibilityIsGrayscaleEnabled ();

		[iOS (8,0)]
		static public bool IsGrayscaleEnabled {
			get {
				return UIAccessibilityIsGrayscaleEnabled ();
			}
		}

		[iOS (8,0)]
		[DllImport (Constants.UIKitLibrary)]
		static extern bool UIAccessibilityIsReduceMotionEnabled ();

		[iOS (8,0)]
		static public bool IsReduceMotionEnabled {
			get {
				return UIAccessibilityIsReduceMotionEnabled ();
			}
		}

		[iOS (8,0)]
		[DllImport (Constants.UIKitLibrary)]
		static extern bool UIAccessibilityIsReduceTransparencyEnabled ();

		[iOS (8,0)]
		static public bool IsReduceTransparencyEnabled {
			get {
				return UIAccessibilityIsReduceTransparencyEnabled ();
			}
		}

		[iOS (8,0)]
		[DllImport (Constants.UIKitLibrary)]
		static extern bool UIAccessibilityIsSwitchControlRunning ();

		[iOS (8,0)]
		static public bool IsSwitchControlRunning {
			get {
				return UIAccessibilityIsSwitchControlRunning ();
			}
		}

		[iOS (8,0)]
		[DllImport (Constants.UIKitLibrary)]
		static extern bool UIAccessibilityIsSpeakSelectionEnabled ();
		[iOS (8,0)]
		static public bool IsSpeakSelectionEnabled {
			get {
				return UIAccessibilityIsSpeakSelectionEnabled ();
			}
		}

		[iOS (8,0)]
		[DllImport (Constants.UIKitLibrary)]
		static extern bool UIAccessibilityIsSpeakScreenEnabled ();
		[iOS (8,0)]
		static public bool IsSpeakScreenEnabled {
			get {
				return UIAccessibilityIsSpeakScreenEnabled ();
			}
		}

		[iOS (10,0), TV (10,0)]
		[DllImport (Constants.UIKitLibrary)]
		static extern bool UIAccessibilityIsAssistiveTouchRunning ();
		[iOS (10,0), TV (10,0)]
		public static bool IsAssistiveTouchRunning {
			get {
				return UIAccessibilityIsAssistiveTouchRunning ();
			}
		}

#if !TVOS
		[iOS (10,0)]
		[DllImport (Constants.UIKitLibrary)]
		static extern nuint UIAccessibilityHearingDevicePairedEar ();

		[iOS (10,0)]
		public static UIAccessibilityHearingDeviceEar HearingDevicePairedEar {
			get {
				return (UIAccessibilityHearingDeviceEar)(ulong) UIAccessibilityHearingDevicePairedEar ();
			}
		}
#endif
	}


}

#endif // !WATCH
