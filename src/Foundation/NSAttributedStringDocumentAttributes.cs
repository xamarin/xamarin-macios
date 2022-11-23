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

		public NSDocumentType DocumentType {
			get {
				var s = WeakDocumentType;

				if (s == NSAttributedStringDocumentType.NSPlainTextDocumentType)
					return NSDocumentType.PlainText;
				if (s == NSAttributedStringDocumentType.NSRtfdTextDocumentType)
					return NSDocumentType.RTFD;
				if (s == NSAttributedStringDocumentType.NSRtfTextDocumentType)
					return NSDocumentType.RTF;
				if (s == NSAttributedStringDocumentType.NSHtmlTextDocumentType)
					return NSDocumentType.HTML;

#if __MACOS__
				if (s == NSAttributedStringDocumentType.NSMacSimpleTextDocumentType)
					return NSDocumentType.MacSimpleText;
				if (s == NSAttributedStringDocumentType.NSDocFormatTextDocumentType)
					return NSDocumentType.DocFormat;
				if (s == NSAttributedStringDocumentType.NSWordMLTextDocumentType)
					return NSDocumentType.WordML;
				if (s == NSAttributedStringDocumentType.NSWebArchiveTextDocumentType)
					return NSDocumentType.WebArchive;
				if (s == NSAttributedStringDocumentType.NSOfficeOpenXMLTextDocumentType)
					return NSDocumentType.OfficeOpenXml;
				if (s == NSAttributedStringDocumentType.NSOpenDocumentTextDocumentType)
					return NSDocumentType.OpenDocument;
#endif // __MACOS__

				return NSDocumentType.Unknown;
			}

			set {
				NSString? documentType = null;
				switch (value) {
				case NSDocumentType.PlainText:
					documentType = NSAttributedStringDocumentType.NSPlainTextDocumentType;
					break;
				case NSDocumentType.RTFD:
					documentType = NSAttributedStringDocumentType.NSRtfdTextDocumentType;
					break;
				case NSDocumentType.RTF:
					documentType = NSAttributedStringDocumentType.NSRtfTextDocumentType;
					break;
				case NSDocumentType.HTML:
					documentType = NSAttributedStringDocumentType.NSHtmlTextDocumentType;
					break;
#if __MACOS__
				case NSDocumentType.MacSimpleText:
					documentType = NSAttributedStringDocumentType.NSMacSimpleTextDocumentType;
					break;
				case NSDocumentType.DocFormat:
					documentType = NSAttributedStringDocumentType.NSDocFormatTextDocumentType;
					break;
				case NSDocumentType.WordML:
					documentType = NSAttributedStringDocumentType.NSWordMLTextDocumentType;
					break;
				case NSDocumentType.WebArchive:
					documentType = NSAttributedStringDocumentType.NSWebArchiveTextDocumentType;
					break;
				case NSDocumentType.OfficeOpenXml:
					documentType = NSAttributedStringDocumentType.NSOfficeOpenXMLTextDocumentType;
					break;
				case NSDocumentType.OpenDocument:
					documentType = NSAttributedStringDocumentType.NSOpenDocumentTextDocumentType;
					break;
#endif // __MACOS__
				}

				if (documentType is not null)
					WeakDocumentType= documentType;
			}
		}

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
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[Mac (10, 15)]
		[iOS (13, 0)]
#endif
		public NSUrl? ReadAccessUrl {
			get {
				return GetNativeValue<NSUrl> (NSAttributedStringDocumentReadingOptionKeys.ReadAccessUrlKey);
			}
			set {
				SetNativeValue (NSAttributedStringDocumentReadingOptionKeys.ReadAccessUrlKey, value);
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

#endif // !COREBUILD
	}
}
