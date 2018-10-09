//
// MediaAccessibility.cs: binding for iOS (7+) MediaAccessibility framework
//
// Authors:
//   Aaron Bockover (abock@xamarin.com)
//
// Copyright 2013, 2015 Xamarin Inc.

using System;
using System.Runtime.InteropServices;

using ObjCRuntime;
using CoreFoundation;
using CoreGraphics;
using CoreText;
using Foundation;

namespace MediaAccessibility {

	[Native]
	[iOS (7,0)][Mac (10,9)]
	public enum MACaptionAppearanceDomain : long {
		Default = 0,
		User = 1
	}

	[Native]
	[iOS (7,0)][Mac (10,9)]
	public enum MACaptionAppearanceDisplayType : long {
		ForcedOnly = 0,
		Automatic = 1,
		AlwaysOn = 2
	}

	[Native]
	[iOS (7,0)][Mac (10,9)]
	public enum MACaptionAppearanceBehavior : long {
		UseValue = 0,
		UseContentIfAvailable = 1
	}

	[Native]
	[iOS (7,0)][Mac (10,9)]
	public enum MACaptionAppearanceFontStyle : long {
		Default = 0,
		MonospacedWithSerif = 1,
		ProportionalWithSerif = 2,
		MonospacedWithoutSerif = 3,
		ProportionalWithoutSerif = 4,
		Casual = 5,
		Cursive = 6,
		SmallCapital = 7
	}

	[Native]
	[iOS (7,0)][Mac (10,9)]
	public enum MACaptionAppearanceTextEdgeStyle : long {
		Undefined = 0,
		None = 1,
		Raised = 2,
		Depressed = 3,
		Uniform = 4,
		DropShadow = 5
	}

	[iOS (7,0)][Mac (10,9)]
	public static partial class MACaptionAppearance {

#if !XAMCORE_4_0
		// FIXME: make this a real notification
		public static readonly NSString SettingsChangedNotification;

		[Advice ("Use 'MediaCharacteristic.DescribesMusicAndSoundForAccessibility' getter.")]
		public static readonly NSString MediaCharacteristicDescribesMusicAndSoundForAccessibility;

		[Advice ("Use 'MediaCharacteristic.TranscribesSpokenDialogForAccessibility' getter.")]
		public static readonly NSString MediaCharacteristicTranscribesSpokenDialogForAccessibility;

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
		static extern bool MACaptionAppearanceAddSelectedLanguage (nint domain,
			/* CFStringRef __nonnull */ IntPtr language);

		public static bool AddSelectedLanguage (MACaptionAppearanceDomain domain, string language)
		{
			// this will throw an ANE if language is null
			using (var lang = new CFString (language)) {
				return MACaptionAppearanceAddSelectedLanguage ((int) domain, lang.Handle);
			}
		}

		[DllImport (Constants.MediaAccessibilityLibrary)]
		static extern /* CFArrayRef __nonnull */ IntPtr MACaptionAppearanceCopySelectedLanguages (nint domain);

		public static string [] GetSelectedLanguages (MACaptionAppearanceDomain domain)
		{
			using (var langs = new CFArray (MACaptionAppearanceCopySelectedLanguages ((int) domain), owns: true)) {
				var languages = new string [langs.Count];
				for (int i = 0; i < langs.Count; i++) {
					languages[i] = CFString.FetchString (langs.GetValue (i));
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
					characteristics[i] = new NSString (chars.GetValue (i));
				}
				return characteristics;
			}
		}

		[DllImport (Constants.MediaAccessibilityLibrary)]
		static extern /* CGColorRef __nonnull */ IntPtr MACaptionAppearanceCopyForegroundColor (nint domain, 
			/* MACaptionAppearanceBehavior * __nullable */ ref nint behavior);
		
		public static CGColor GetForegroundColor (MACaptionAppearanceDomain domain, ref MACaptionAppearanceBehavior behavior)
		{
			nint b = (int) behavior;
			var rv = new CGColor (MACaptionAppearanceCopyForegroundColor ((int) domain, ref b), owns: true);
			behavior = (MACaptionAppearanceBehavior) (int) b;
			return rv;
		}

		[DllImport (Constants.MediaAccessibilityLibrary)]
		static extern /* CGColorRef __nonnull */ IntPtr MACaptionAppearanceCopyBackgroundColor (nint domain, 
			/* MACaptionAppearanceBehavior * __nullable */ ref nint behavior);

		public static CGColor GetBackgroundColor (MACaptionAppearanceDomain domain, ref MACaptionAppearanceBehavior behavior)
		{
			nint b = (int) behavior;
			var rv = new CGColor (MACaptionAppearanceCopyBackgroundColor ((int) domain, ref b), owns: true);
			behavior = (MACaptionAppearanceBehavior) (int) b;
			return rv;
		}

		[DllImport (Constants.MediaAccessibilityLibrary)]
		static extern /* CGColorRef __nonnull */ IntPtr MACaptionAppearanceCopyWindowColor (nint domain, 
			/* MACaptionAppearanceBehavior * __nullable */ ref nint behavior);

		public static CGColor GetWindowColor (MACaptionAppearanceDomain domain, ref MACaptionAppearanceBehavior behavior)
		{
			nint b = (int) behavior;
			var rv = new CGColor (MACaptionAppearanceCopyWindowColor ((int) domain, ref b), owns: true);
			behavior = (MACaptionAppearanceBehavior) (int) b;
			return rv;
		}

		[DllImport (Constants.MediaAccessibilityLibrary)]
		static extern nfloat MACaptionAppearanceGetForegroundOpacity (nint domain, ref nint behavior);

		public static nfloat GetForegroundOpacity (MACaptionAppearanceDomain domain, ref MACaptionAppearanceBehavior behavior)
		{
			nint b = (int) behavior;
			var rv = MACaptionAppearanceGetForegroundOpacity ((int) domain, ref b);
			behavior = (MACaptionAppearanceBehavior) (int) b;
			return rv;
		}

		[DllImport (Constants.MediaAccessibilityLibrary)]
		static extern nfloat MACaptionAppearanceGetBackgroundOpacity (nint domain,
			/* MACaptionAppearanceBehavior * __nullable */ ref nint behavior);

		public static nfloat GetBackgroundOpacity (MACaptionAppearanceDomain domain, ref MACaptionAppearanceBehavior behavior)
		{
			nint b = (int) behavior;
			var rv = MACaptionAppearanceGetBackgroundOpacity ((int) domain, ref b);
			behavior = (MACaptionAppearanceBehavior) (int) b;
			return rv;
		}

		[DllImport (Constants.MediaAccessibilityLibrary)]
		static extern nfloat MACaptionAppearanceGetWindowOpacity (nint domain,
			/* MACaptionAppearanceBehavior * __nullable */ ref nint behavior);

		public static nfloat GetWindowOpacity (MACaptionAppearanceDomain domain, ref MACaptionAppearanceBehavior behavior)
		{
			nint b = (int) behavior;
			var rv = MACaptionAppearanceGetWindowOpacity ((int) domain, ref b);
			behavior = (MACaptionAppearanceBehavior) (int) b;
			return rv;
		}

		[DllImport (Constants.MediaAccessibilityLibrary)]
		static extern nfloat MACaptionAppearanceGetWindowRoundedCornerRadius (nint domain,
			/* MACaptionAppearanceBehavior * __nullable */ ref nint behavior);

		public static nfloat GetWindowRoundedCornerRadius (MACaptionAppearanceDomain domain, ref MACaptionAppearanceBehavior behavior)
		{
			nint b = (int) behavior;
			var rv = MACaptionAppearanceGetWindowRoundedCornerRadius ((int) domain, ref b);
			behavior = (MACaptionAppearanceBehavior) (int) b;
			return rv;
		}

		[DllImport (Constants.MediaAccessibilityLibrary)]
		static extern /* CTFontDescriptorRef __nonnull */ IntPtr MACaptionAppearanceCopyFontDescriptorForStyle (nint domain, 
			/* MACaptionAppearanceBehavior * __nullable */ ref nint behavior, nint fontStyle);

		public static CTFontDescriptor GetFontDescriptor (MACaptionAppearanceDomain domain, ref MACaptionAppearanceBehavior behavior, MACaptionAppearanceFontStyle fontStyle)
		{
			nint b = (int) behavior;
			var rv = new CTFontDescriptor (MACaptionAppearanceCopyFontDescriptorForStyle ((int) domain, ref b, (int) fontStyle), owns: true);
			behavior = (MACaptionAppearanceBehavior) (int) b;
			return rv;
		}

		[DllImport (Constants.MediaAccessibilityLibrary)]
		static extern nfloat MACaptionAppearanceGetRelativeCharacterSize (nint domain,
			/* MACaptionAppearanceBehavior * __nullable */ ref nint behavior);

		public static nfloat GetRelativeCharacterSize (MACaptionAppearanceDomain domain, ref MACaptionAppearanceBehavior behavior)
		{
			nint b = (int) behavior;
			var rv = MACaptionAppearanceGetRelativeCharacterSize ((int) domain, ref b);
			behavior = (MACaptionAppearanceBehavior) (int) b;
			return rv;
		}

		[DllImport (Constants.MediaAccessibilityLibrary)]
		static extern nint MACaptionAppearanceGetTextEdgeStyle (nint domain,
			/* MACaptionAppearanceBehavior * __nullable */ ref nint behavior);

		public static MACaptionAppearanceTextEdgeStyle GetTextEdgeStyle (MACaptionAppearanceDomain domain, ref MACaptionAppearanceBehavior behavior)
		{
			nint b = (int) behavior;
			var rv = MACaptionAppearanceGetTextEdgeStyle ((int) domain, ref b);
			behavior = (MACaptionAppearanceBehavior) (int) b;
			return (MACaptionAppearanceTextEdgeStyle) (int) rv;
		}
	}

	static partial class MAAudibleMedia {
		[iOS (8,0)][Mac (10,10)]
		[DllImport (Constants.MediaAccessibilityLibrary)]
		static extern unsafe IntPtr /* CFArrayRef __nonnull */ MAAudibleMediaCopyPreferredCharacteristics ();

		// according to webkit source code (the only use I could find) this is an array of CFString
		// https://github.com/WebKit/webkit/blob/master/Source/WebCore/page/CaptionUserPreferencesMediaAF.cpp
		static public string[] GetPreferredCharacteristics ()
		{
			var handle = MAAudibleMediaCopyPreferredCharacteristics ();
			if (handle == IntPtr.Zero)
				return null;
			var result = NSArray.StringArrayFromHandle (handle);
			CFObject.CFRelease (handle); // *Copy* API
			return result;
		}
	}
}
