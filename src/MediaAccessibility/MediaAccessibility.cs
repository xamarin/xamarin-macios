//
// MediaAccessibility.cs: binding for iOS (7+) MediaAccessibility framework
//
// Authors:
//   Aaron Bockover (abock@xamarin.com)
//
// Copyright 2013, 2015 Xamarin Inc.

#nullable enable

using System;
using System.Runtime.InteropServices;

using ObjCRuntime;
using CoreFoundation;
using CoreGraphics;
using CoreText;
using Foundation;

namespace MediaAccessibility {

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("tvos")]
#endif
	public static partial class MACaptionAppearance {

#if !NET
		// FIXME: make this a real notification
		public static readonly NSString? SettingsChangedNotification;

		[Advice ("Use 'MediaCharacteristic.DescribesMusicAndSoundForAccessibility' getter.")]
		public static readonly NSString? MediaCharacteristicDescribesMusicAndSoundForAccessibility;

		[Advice ("Use 'MediaCharacteristic.TranscribesSpokenDialogForAccessibility' getter.")]
		public static readonly NSString? MediaCharacteristicTranscribesSpokenDialogForAccessibility;

		static MACaptionAppearance ()
		{
			var handle = Libraries.MediaAccessibility.Handle;
			SettingsChangedNotification = Dlfcn.GetStringConstant (handle, "kMACaptionAppearanceSettingsChangedNotification");

			MediaCharacteristicDescribesMusicAndSoundForAccessibility = Dlfcn.GetStringConstant (handle,
				"MAMediaCharacteristicDescribesMusicAndSoundForAccessibility");
			MediaCharacteristicTranscribesSpokenDialogForAccessibility = Dlfcn.GetStringConstant (handle,
				"MAMediaCharacteristicTranscribesSpokenDialogForAccessibility");
		}
#endif

		[DllImport (Constants.MediaAccessibilityLibrary)]
		static extern byte MACaptionAppearanceAddSelectedLanguage (nint domain,
			/* CFStringRef __nonnull */ IntPtr language);

		public static bool AddSelectedLanguage (MACaptionAppearanceDomain domain, string language)
		{
			// this will throw an ANE if language is null
			using (var lang = new CFString (language)) {
				return MACaptionAppearanceAddSelectedLanguage ((int) domain, lang.Handle) != 0;
			}
		}

		[DllImport (Constants.MediaAccessibilityLibrary)]
		static extern /* CFArrayRef __nonnull */ IntPtr MACaptionAppearanceCopySelectedLanguages (nint domain);

		public static string? [] GetSelectedLanguages (MACaptionAppearanceDomain domain)
		{
			using (var langs = new CFArray (MACaptionAppearanceCopySelectedLanguages ((int) domain), owns: true)) {
				var languages = new string? [langs.Count];
				for (int i = 0; i < langs.Count; i++) {
					languages [i] = CFString.FromHandle (langs.GetValue (i));
				}
				return languages;
			}
		}

		[DllImport (Constants.MediaAccessibilityLibrary)]
		static extern nint MACaptionAppearanceGetDisplayType (nint domain);

		public static MACaptionAppearanceDisplayType GetDisplayType (MACaptionAppearanceDomain domain)
		{
			return (MACaptionAppearanceDisplayType) (int) MACaptionAppearanceGetDisplayType ((int) domain);
		}

		[DllImport (Constants.MediaAccessibilityLibrary)]
		static extern void MACaptionAppearanceSetDisplayType (nint domain, nint displayType);

		public static void SetDisplayType (MACaptionAppearanceDomain domain, MACaptionAppearanceDisplayType displayType)
		{
			MACaptionAppearanceSetDisplayType ((int) domain, (int) displayType);
		}

		[DllImport (Constants.MediaAccessibilityLibrary)]
		static extern /* CFArrayRef __nonnull */ IntPtr MACaptionAppearanceCopyPreferredCaptioningMediaCharacteristics (nint domain);

		public static NSString [] GetPreferredCaptioningMediaCharacteristics (MACaptionAppearanceDomain domain)
		{
			using (var chars = new CFArray (MACaptionAppearanceCopyPreferredCaptioningMediaCharacteristics ((int) domain), owns: true)) {
				NSString [] characteristics = new NSString [chars.Count];
				for (int i = 0; i < chars.Count; i++) {
					characteristics [i] = new NSString (chars.GetValue (i));
				}
				return characteristics;
			}
		}

		[DllImport (Constants.MediaAccessibilityLibrary)]
		unsafe static extern /* CGColorRef __nonnull */ IntPtr MACaptionAppearanceCopyForegroundColor (nint domain,
			/* MACaptionAppearanceBehavior * __nullable */ nint* behavior);

		public static CGColor GetForegroundColor (MACaptionAppearanceDomain domain, ref MACaptionAppearanceBehavior behavior)
		{
			nint b = (int) behavior;
			IntPtr handle;
			unsafe {
				handle = MACaptionAppearanceCopyForegroundColor ((int) domain, &b);
			}
			var rv = new CGColor (handle, owns: true);
			behavior = (MACaptionAppearanceBehavior) (int) b;
			return rv;
		}

		[DllImport (Constants.MediaAccessibilityLibrary)]
		unsafe static extern /* CGColorRef __nonnull */ IntPtr MACaptionAppearanceCopyBackgroundColor (nint domain,
			/* MACaptionAppearanceBehavior * __nullable */ nint* behavior);

		public static CGColor GetBackgroundColor (MACaptionAppearanceDomain domain, ref MACaptionAppearanceBehavior behavior)
		{
			nint b = (int) behavior;
			IntPtr handle;
			unsafe {
				handle = MACaptionAppearanceCopyBackgroundColor ((int) domain, &b);
			}
			var rv = new CGColor (handle, owns: true);
			behavior = (MACaptionAppearanceBehavior) (int) b;
			return rv;
		}

		[DllImport (Constants.MediaAccessibilityLibrary)]
		unsafe static extern /* CGColorRef __nonnull */ IntPtr MACaptionAppearanceCopyWindowColor (nint domain,
			/* MACaptionAppearanceBehavior * __nullable */ nint* behavior);

		public static CGColor GetWindowColor (MACaptionAppearanceDomain domain, ref MACaptionAppearanceBehavior behavior)
		{
			nint b = (int) behavior;
			IntPtr handle;
			unsafe {
				handle = MACaptionAppearanceCopyWindowColor ((int) domain, &b);
			}
			var rv = new CGColor (handle, owns: true);
			behavior = (MACaptionAppearanceBehavior) (int) b;
			return rv;
		}

		[DllImport (Constants.MediaAccessibilityLibrary)]
		unsafe static extern nfloat MACaptionAppearanceGetForegroundOpacity (nint domain, nint* behavior);

		public static nfloat GetForegroundOpacity (MACaptionAppearanceDomain domain, ref MACaptionAppearanceBehavior behavior)
		{
			nint b = (int) behavior;
			nfloat rv;
			unsafe {
				rv = MACaptionAppearanceGetForegroundOpacity ((int) domain, &b);
			}
			behavior = (MACaptionAppearanceBehavior) (int) b;
			return rv;
		}

		[DllImport (Constants.MediaAccessibilityLibrary)]
		unsafe static extern nfloat MACaptionAppearanceGetBackgroundOpacity (nint domain,
			/* MACaptionAppearanceBehavior * __nullable */ nint* behavior);

		public static nfloat GetBackgroundOpacity (MACaptionAppearanceDomain domain, ref MACaptionAppearanceBehavior behavior)
		{
			nint b = (int) behavior;
			nfloat rv;
			unsafe {
				rv = MACaptionAppearanceGetBackgroundOpacity ((int) domain, &b);
			}
			behavior = (MACaptionAppearanceBehavior) (int) b;
			return rv;
		}

		[DllImport (Constants.MediaAccessibilityLibrary)]
		unsafe static extern nfloat MACaptionAppearanceGetWindowOpacity (nint domain,
			/* MACaptionAppearanceBehavior * __nullable */ nint* behavior);

		public static nfloat GetWindowOpacity (MACaptionAppearanceDomain domain, ref MACaptionAppearanceBehavior behavior)
		{
			nint b = (int) behavior;
			nfloat rv;
			unsafe {
				rv = MACaptionAppearanceGetWindowOpacity ((int) domain, &b);
			}
			behavior = (MACaptionAppearanceBehavior) (int) b;
			return rv;
		}

		[DllImport (Constants.MediaAccessibilityLibrary)]
		unsafe static extern nfloat MACaptionAppearanceGetWindowRoundedCornerRadius (nint domain,
			/* MACaptionAppearanceBehavior * __nullable */ nint* behavior);

		public static nfloat GetWindowRoundedCornerRadius (MACaptionAppearanceDomain domain, ref MACaptionAppearanceBehavior behavior)
		{
			nint b = (int) behavior;
			nfloat rv;
			unsafe {
				rv = MACaptionAppearanceGetWindowRoundedCornerRadius ((int) domain, &b);
			}
			behavior = (MACaptionAppearanceBehavior) (int) b;
			return rv;
		}

		[DllImport (Constants.MediaAccessibilityLibrary)]
		unsafe static extern /* CTFontDescriptorRef __nonnull */ IntPtr MACaptionAppearanceCopyFontDescriptorForStyle (nint domain,
			/* MACaptionAppearanceBehavior * __nullable */ nint* behavior, nint fontStyle);

		public static CTFontDescriptor GetFontDescriptor (MACaptionAppearanceDomain domain, ref MACaptionAppearanceBehavior behavior, MACaptionAppearanceFontStyle fontStyle)
		{
			nint b = (int) behavior;
			IntPtr handle;
			unsafe {
				handle = MACaptionAppearanceCopyFontDescriptorForStyle ((int) domain, &b, (int) fontStyle);
			}
			var rv = new CTFontDescriptor (handle, owns: true);
			behavior = (MACaptionAppearanceBehavior) (int) b;
			return rv;
		}

		[DllImport (Constants.MediaAccessibilityLibrary)]
		unsafe static extern nfloat MACaptionAppearanceGetRelativeCharacterSize (nint domain,
			/* MACaptionAppearanceBehavior * __nullable */ nint* behavior);

		public static nfloat GetRelativeCharacterSize (MACaptionAppearanceDomain domain, ref MACaptionAppearanceBehavior behavior)
		{
			nint b = (int) behavior;
			nfloat rv;
			unsafe {
				rv = MACaptionAppearanceGetRelativeCharacterSize ((int) domain, &b);
			}
			behavior = (MACaptionAppearanceBehavior) (int) b;
			return rv;
		}

		[DllImport (Constants.MediaAccessibilityLibrary)]
		unsafe static extern nint MACaptionAppearanceGetTextEdgeStyle (nint domain,
			/* MACaptionAppearanceBehavior * __nullable */ nint* behavior);

		public static MACaptionAppearanceTextEdgeStyle GetTextEdgeStyle (MACaptionAppearanceDomain domain, ref MACaptionAppearanceBehavior behavior)
		{
			nint b = (int) behavior;
			nint rv;
			unsafe {
				rv = MACaptionAppearanceGetTextEdgeStyle ((int) domain, &b);
			}
			behavior = (MACaptionAppearanceBehavior) (int) b;
			return (MACaptionAppearanceTextEdgeStyle) (int) rv;
		}

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[TV (13, 0)]
		[iOS (13, 0)]
#endif
		[DllImport (Constants.MediaAccessibilityLibrary)]
		static extern void MACaptionAppearanceDidDisplayCaptions (IntPtr /* CFArratRef */ strings);

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[TV (13, 0)]
		[iOS (13, 0)]
#endif
		public static void DidDisplayCaptions (string [] strings)
		{
			if ((strings is null) || (strings.Length == 0))
				MACaptionAppearanceDidDisplayCaptions (IntPtr.Zero);
			else {
				using (var array = NSArray.FromStrings (strings))
					MACaptionAppearanceDidDisplayCaptions (array.Handle);
			}
		}

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[TV (13, 0)]
		[iOS (13, 0)]
#endif
		public static void DidDisplayCaptions (NSAttributedString [] strings)
		{
			// CFAttributedString is “toll-free bridged” with its Foundation counterpart, NSAttributedString.
			// https://developer.apple.com/documentation/corefoundation/cfattributedstring?language=objc
			if ((strings is null) || (strings.Length == 0))
				MACaptionAppearanceDidDisplayCaptions (IntPtr.Zero);
			else {
				using (var array = NSArray.FromNSObjects (strings))
					MACaptionAppearanceDidDisplayCaptions (array.Handle);
			}
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	static partial class MAAudibleMedia {
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		[DllImport (Constants.MediaAccessibilityLibrary)]
		static extern unsafe IntPtr /* CFArrayRef __nonnull */ MAAudibleMediaCopyPreferredCharacteristics ();

		// according to webkit source code (the only use I could find) this is an array of CFString
		// https://github.com/WebKit/webkit/blob/master/Source/WebCore/page/CaptionUserPreferencesMediaAF.cpp
		static public string? []? GetPreferredCharacteristics ()
		{
			var handle = MAAudibleMediaCopyPreferredCharacteristics ();
			if (handle == IntPtr.Zero)
				return null;
			var result = CFArray.StringArrayFromHandle (handle);
			CFObject.CFRelease (handle); // *Copy* API
			return result;
		}
	}

#if NET
	[SupportedOSPlatform ("ios16.4")]
	[SupportedOSPlatform ("maccatalyst16.4")]
	[SupportedOSPlatform ("macos13.3")]
	[SupportedOSPlatform ("tvos16.4")]
#endif
	public static partial class MAVideoAccommodations {
#if NET
		[SupportedOSPlatform ("ios16.4")]
		[SupportedOSPlatform ("maccatalyst16.4")]
		[SupportedOSPlatform ("macos13.3")]
		[SupportedOSPlatform ("tvos16.4")]
#else
		[Mac (13, 3), TV (16, 4), iOS (16, 4)]
#endif
		[DllImport (Constants.MediaAccessibilityLibrary)]
		static extern byte MADimFlashingLightsEnabled ();

#if NET
		[SupportedOSPlatform ("ios16.4")]
		[SupportedOSPlatform ("maccatalyst16.4")]
		[SupportedOSPlatform ("macos13.3")]
		[SupportedOSPlatform ("tvos16.4")]
#else
		[Mac (13, 3), TV (16, 4), iOS (16, 4)]
#endif
		public static bool IsDimFlashingLightsEnabled () => MADimFlashingLightsEnabled () != 0;
	}
}
