//
// NSAttributedStringDocumentAttributes.cs
//
// Authors:
//   Rolf Bjarne Kvinge (rolf@xamarin.com)
//
// Copyright 2022 Microsoft Corp

#nullable enable

using System;

#if HAS_APPKIT
using AppKit;
#endif
using CoreGraphics;
using Foundation;
#if HAS_UIKIT
using UIKit;
#endif
#if !COREBUILD && HAS_WEBKIT
using WebKit;
#endif
using ObjCRuntime;

#if !COREBUILD
#if __MACOS__
using XColor = AppKit.NSColor;
#else
using XColor = UIKit.UIColor;
#endif
#endif

namespace Foundation {
	public partial class NSAttributedStringDocumentAttributes : DictionaryContainer {
#if !COREBUILD
		public NSAttributedStringDocumentAttributes () { }
		public NSAttributedStringDocumentAttributes (NSDictionary? dictionary) : base (dictionary) { }

		public XColor? BackgroundColor {
			get {
				return GetNativeValue<XColor> (NSAttributedStringDocumentAttributeKey.NSBackgroundColorDocumentAttribute);
			}
			set {
				SetNativeValue (NSAttributedStringDocumentAttributeKey.NSBackgroundColorDocumentAttribute, value);
			}
		}

		public float? DefaultTabInterval {
			get {
				return GetFloatValue (NSAttributedStringDocumentAttributeKey.NSDefaultTabIntervalDocumentAttribute);
			}
			set {
				if (value < 0 || value > 1.0f)
					throw new ArgumentOutOfRangeException (nameof (value), value, "Value must be between 0 and 1");

				SetNumberValue (NSAttributedStringDocumentAttributeKey.NSDefaultTabIntervalDocumentAttribute, value);
			}
		}

		public float? HyphenationFactor {
			get {
				return GetFloatValue (NSAttributedStringDocumentAttributeKey.NSHyphenationFactorDocumentAttribute);
			}
			set {
				if (value < 0 || value > 1.0f)
					throw new ArgumentOutOfRangeException (nameof (value), value, "Value must be between 0 and 1");

				SetNumberValue (NSAttributedStringDocumentAttributeKey.NSHyphenationFactorDocumentAttribute, value);
			}
		}

		public NSStringEncoding? StringEncoding {
			get {
				return (NSStringEncoding?) (long?) GetNIntValue (NSAttributedStringDocumentAttributeKey.NSCharacterEncodingDocumentAttribute);
			}
			set {
				SetNumberValue (NSAttributedStringDocumentAttributeKey.NSCharacterEncodingDocumentAttribute, (nint?) (long?) value);
			}
		}

		public NSString? WeakDocumentType {
			get {
				return GetNSStringValue (NSAttributedStringDocumentAttributeKey.NSDocumentTypeDocumentAttribute);
			}
			set {
				SetStringValue (NSAttributedStringDocumentAttributeKey.NSDocumentTypeDocumentAttribute, value);
			}
		}

#if XAMCORE_5_0
		public NSAttributedStringDocumentType DocumentType {
			get {

				return NSAttributedStringDocumentTypeExtensions.GetValue (WeakDocumentType);
			}
			set {
				WeakDocumentType = value.GetConstant ();
			}
		}
#else
		public NSDocumentType DocumentType {
			get {

				return (NSDocumentType) NSAttributedStringDocumentTypeExtensions.GetValue (WeakDocumentType);
			}
			set {
				WeakDocumentType = ((NSAttributedStringDocumentType) value).GetConstant ();
			}
		}
#endif // !XAMCORE_5_0

		public NSDictionary? WeakDefaultAttributes {
			get {
				return GetNativeValue<NSDictionary> (NSAttributedStringDocumentAttributeKey.NSDefaultAttributesDocumentAttribute);
			}
			set {
				SetNativeValue (NSAttributedStringDocumentAttributeKey.NSDefaultAttributesDocumentAttribute, value);
			}
		}

		public CGSize? PaperSize {
			get {
				return GetCGSizeValue (NSAttributedStringDocumentAttributeKey.NSPaperSizeDocumentAttribute);
			}
			set {
				SetCGSizeValue (NSAttributedStringDocumentAttributeKey.NSPaperSizeDocumentAttribute, value);
			}
		}

#if !__MACOS__
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("macos")]
#endif // NET
		public UIEdgeInsets? PaperMargin {
			get {
				if (!Dictionary.TryGetValue (NSAttributedStringDocumentAttributeKey.NSPaperMarginDocumentAttribute, out var value))
					return null;

				if (value is NSValue size)
					return size.UIEdgeInsetsValue;

				return null;
			}
			set {
				SetNativeValue (NSAttributedStringDocumentAttributeKey.NSPaperMarginDocumentAttribute, value is null ? null : NSValue.FromUIEdgeInsets (value.Value));
			}
		}
#endif // !__MACOS__

		public CGSize? ViewSize {
			get {
				return GetCGSizeValue (NSAttributedStringDocumentAttributeKey.NSViewSizeDocumentAttribute);
			}
			set {
				SetCGSizeValue (NSAttributedStringDocumentAttributeKey.NSViewSizeDocumentAttribute, value);
			}
		}

		public float? ViewZoom {
			get {
				return GetFloatValue (NSAttributedStringDocumentAttributeKey.NSViewZoomDocumentAttribute);
			}
			set {
				SetNumberValue (NSAttributedStringDocumentAttributeKey.NSViewZoomDocumentAttribute, value);
			}
		}

		public NSDocumentViewMode? ViewMode {
			get {
				return (NSDocumentViewMode?) GetInt32Value (NSAttributedStringDocumentAttributeKey.NSViewModeDocumentAttribute);
			}
			set {
				SetNumberValue (NSAttributedStringDocumentAttributeKey.NSViewModeDocumentAttribute, value is null ? null : (int) value.Value);
			}
		}

#if XAMCORE_5_0 || __MACOS__
		public bool? ReadOnly {
			get {
				var value = GetInt32Value (NSAttributedStringDocumentAttributeKey.NSReadOnlyDocumentAttribute);
				if (value is null)
					return null;
				return value.Value == 1;
			}
			set {
				SetNumberValue (NSAttributedStringDocumentAttributeKey.NSReadOnlyDocumentAttribute, value is null ? null : (value.Value ? 1 : 0));
			}
		}
#else
		public bool ReadOnly {
			get {
				var value = GetInt32Value (NSAttributedStringDocumentAttributeKey.NSReadOnlyDocumentAttribute);
				if (value is null || value.Value != 1)
					return false;
				return true;
			}
			set {
				SetNumberValue (NSAttributedStringDocumentAttributeKey.NSReadOnlyDocumentAttribute, value ? 1 : 0);
			}
		}
#endif // XAMCORE_5_0 || __MACOS__

#if !TVOS && !WATCH
		// documentation is unclear if an NSString or an NSUrl should be used...
		// but providing an `NSString` throws a `NSInvalidArgumentException Reason: (null) is not a file URL`
#if NET
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[iOS (13, 0)]
#endif
		public NSUrl? ReadAccessUrl {
			get {
				return GetNativeValue<NSUrl> (NSAttributedStringDocumentReadingOptionKey.NSReadAccessUrlDocumentOption);
			}
			set {
				SetNativeValue (NSAttributedStringDocumentReadingOptionKey.NSReadAccessUrlDocumentOption, value);
			}
		}
#endif // !TVOS && !WATCH

#if __MACOS__
#if NET
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
#endif // NET
		public WebPreferences? WebPreferences {
			get {
				return GetNativeValue<WebPreferences> (NSAttributedStringDocumentReadingOptionKey.NSWebPreferencesDocumentOption);
			}
			set {
				SetNativeValue (NSAttributedStringDocumentReadingOptionKey.NSWebPreferencesDocumentOption, value);
			}
		}
#endif // !__MACOS__

#if __MACOS__
#if NET
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
#endif // NET
		public NSObject? WebResourceLoadDelegate {
			get {
				return GetNativeValue<NSObject> (NSAttributedStringDocumentReadingOptionKey.NSWebResourceLoadDelegateDocumentOption);
			}
			set {
				SetNativeValue (NSAttributedStringDocumentReadingOptionKey.NSWebResourceLoadDelegateDocumentOption, value);
			}
		}
#endif // !__MACOS__

#if __MACOS__
#if NET
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
#endif // NET
		public NSUrl? BaseUrl {
			get {
				return GetNativeValue<NSUrl> (NSAttributedStringDocumentReadingOptionKey.NSBaseURLDocumentOption);
			}
			set {
				SetNativeValue (NSAttributedStringDocumentReadingOptionKey.NSBaseURLDocumentOption, value);
			}
		}
#endif // !__MACOS__

#if __MACOS__
#if NET
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
#endif // NET
		public string? TextEncodingName {
			get {
				return GetStringValue (NSAttributedStringDocumentReadingOptionKey.NSTextEncodingNameDocumentOption);
			}
			set {
				SetStringValue (NSAttributedStringDocumentReadingOptionKey.NSTextEncodingNameDocumentOption, value);
			}
		}
#endif // !__MACOS__

#if __MACOS__
#if NET
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
#endif // NET
		public float? TextSizeMultiplier {
			get {
				return GetFloatValue (NSAttributedStringDocumentReadingOptionKey.NSTextSizeMultiplierDocumentOption);
			}
			set {
				SetNumberValue (NSAttributedStringDocumentReadingOptionKey.NSTextSizeMultiplierDocumentOption, value);
			}
		}
#endif // !__MACOS__

#if __MACOS__
#if NET
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
#endif // NET
		public float? Timeout {
			get {
				return GetFloatValue (NSAttributedStringDocumentReadingOptionKey.NSTimeoutDocumentOption);
			}
			set {
				SetNumberValue (NSAttributedStringDocumentReadingOptionKey.NSTimeoutDocumentOption, value);
			}
		}
#endif // !__MACOS__

#endif // !COREBUILD
	}
}
