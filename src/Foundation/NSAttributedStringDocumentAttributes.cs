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
		public NSString? WeakDocumentType {
			get {
				return GetNSStringValue (NSAttributedStringDocumentAttributeKey.DocumentTypeDocumentAttribute);
			}
			set {
				SetStringValue (NSAttributedStringDocumentAttributeKey.DocumentTypeDocumentAttribute, value);
			}
		}

#if !XAMCORE_5_0
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
				return GetNativeValue<NSDictionary> (NSAttributedStringDocumentAttributeKey.DefaultAttributesDocumentAttribute);
			}
			set {
				SetNativeValue (NSAttributedStringDocumentAttributeKey.DefaultAttributesDocumentAttribute, value);
			}
		}

#if XAMCORE_5_0 || __MACOS__
		public bool? ReadOnly {
			get {
				var value = GetInt32Value (NSAttributedStringDocumentAttributeKey.ReadOnlyDocumentAttribute);
				if (value is null)
					return null;
				return value.Value == 1;
			}
			set {
				SetNumberValue (NSAttributedStringDocumentAttributeKey.ReadOnlyDocumentAttribute, value is null ? null : (value.Value ? 1 : 0));
			}
		}
#else
		public bool ReadOnly {
			get {
				var value = GetInt32Value (NSAttributedStringDocumentAttributeKey.ReadOnlyDocumentAttribute);
				if (value is null || value.Value != 1)
					return false;
				return true;
			}
			set {
				SetNumberValue (NSAttributedStringDocumentAttributeKey.ReadOnlyDocumentAttribute, value ? 1 : 0);
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
				return GetNativeValue<NSUrl> (NSAttributedStringDocumentReadingOptionKey.ReadAccessUrlDocumentOption);
			}
			set {
				SetNativeValue (NSAttributedStringDocumentReadingOptionKey.ReadAccessUrlDocumentOption, value);
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
				return GetNativeValue<WebPreferences> (NSAttributedStringDocumentReadingOptionKey.WebPreferencesDocumentOption);
			}
			set {
				SetNativeValue (NSAttributedStringDocumentReadingOptionKey.WebPreferencesDocumentOption, value);
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
				return GetNativeValue<NSObject> (NSAttributedStringDocumentReadingOptionKey.WebResourceLoadDelegateDocumentOption);
			}
			set {
				SetNativeValue (NSAttributedStringDocumentReadingOptionKey.WebResourceLoadDelegateDocumentOption, value);
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
				return GetNativeValue<NSUrl> (NSAttributedStringDocumentReadingOptionKey.BaseUrlDocumentOption);
			}
			set {
				SetNativeValue (NSAttributedStringDocumentReadingOptionKey.BaseUrlDocumentOption, value);
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
				return GetFloatValue (NSAttributedStringDocumentReadingOptionKey.TextSizeMultiplierDocumentOption);
			}
			set {
				SetNumberValue (NSAttributedStringDocumentReadingOptionKey.TextSizeMultiplierDocumentOption, value);
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
				return GetFloatValue (NSAttributedStringDocumentReadingOptionKey.TimeoutDocumentOption);
			}
			set {
				SetNumberValue (NSAttributedStringDocumentReadingOptionKey.TimeoutDocumentOption, value);
			}
		}
#endif // !__MACOS__

#endif // !COREBUILD
	}
}
