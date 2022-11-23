// 
// UIStringAttributes.cs: Implements strongly typed access to UIKit specific part of UIStringAttributeKey
//
// Authors: Marek Safar (marek.safar@gmail.com)
//     
// Copyright 2012-2013, Xamarin Inc.
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

#nullable enable

using System;

using ObjCRuntime;
using Foundation;
using CoreGraphics;

#if !MONOMAC
using UIKit;
#endif

namespace Foundation {

	public partial class NSAttributedStringDocumentAttributes : DictionaryContainer {
#if !MONOMAC && !COREBUILD
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
				var s = GetNSStringValue (NSAttributedStringDocumentAttributeKey.NSDocumentTypeDocumentAttribute);
				if (s == NSAttributedStringDocumentType.NSPlainTextDocumentType)
					return NSDocumentType.PlainText;
				if (s == NSAttributedStringDocumentType.NSRtfdTextDocumentType)
					return NSDocumentType.RTFD;
				if (s == NSAttributedStringDocumentType.NSRtfTextDocumentType)
					return NSDocumentType.RTF;
				if (s == NSAttributedStringDocumentType.NSHtmlTextDocumentType)
					return NSDocumentType.HTML;
				return NSDocumentType.Unknown;
			}

			set {
				switch (value) {
				case NSDocumentType.PlainText:
					SetStringValue (NSAttributedStringDocumentAttributeKey.NSDocumentTypeDocumentAttribute, NSAttributedStringDocumentType.NSPlainTextDocumentType);
					break;
				case NSDocumentType.RTFD:
					SetStringValue (NSAttributedStringDocumentAttributeKey.NSDocumentTypeDocumentAttribute, NSAttributedStringDocumentType.NSRtfdTextDocumentType);
					break;
				case NSDocumentType.RTF:
					SetStringValue (NSAttributedStringDocumentAttributeKey.NSDocumentTypeDocumentAttribute, NSAttributedStringDocumentType.NSRtfTextDocumentType);
					break;
				case NSDocumentType.HTML:
					SetStringValue (NSAttributedStringDocumentAttributeKey.NSDocumentTypeDocumentAttribute, NSAttributedStringDocumentType.NSHtmlTextDocumentType);
					break;
				}
			}
		}

		public CGSize? PaperSize {
			get {
				NSObject value;
				Dictionary.TryGetValue (NSAttributedStringDocumentAttributeKey.NSPaperSizeDocumentAttribute, out value);
				var size = value as NSValue;
				if (size != null)
					return size.CGSizeValue;
				return null;
			}
			set {
				if (value is null)
					RemoveValue (NSAttributedStringDocumentAttributeKey.NSPaperSizeDocumentAttribute);
				else
					Dictionary [NSAttributedStringDocumentAttributeKey.NSPaperSizeDocumentAttribute] = NSValue.FromCGSize (value.Value);
			}
		}

		public UIEdgeInsets? PaperMargin {
			get {
				NSObject value;
				Dictionary.TryGetValue (NSAttributedStringDocumentAttributeKey.NSPaperMarginDocumentAttribute, out value);
				var size = value as NSValue;
				if (size != null)
					return size.UIEdgeInsetsValue;
				return null;
			}
			set {
				if (value is null)
					RemoveValue (NSAttributedStringDocumentAttributeKey.NSPaperMarginDocumentAttribute);
				else
					Dictionary [NSAttributedStringDocumentAttributeKey.NSPaperMarginDocumentAttribute] = NSValue.FromUIEdgeInsets (value.Value);
			}
		}

		public CGSize? ViewSize {
			get {
				NSObject value;
				Dictionary.TryGetValue (NSAttributedStringDocumentAttributeKey.NSViewSizeDocumentAttribute, out value);
				var size = value as NSValue;
				if (size != null)
					return size.CGSizeValue;
				return null;
			}
			set {
				if (value is null)
					RemoveValue (NSAttributedStringDocumentAttributeKey.NSViewSizeDocumentAttribute);
				else
					Dictionary [NSAttributedStringDocumentAttributeKey.NSViewSizeDocumentAttribute] = NSValue.FromCGSize (value.Value);
			}
		}

		public float? ViewZoom {
			get {
				return GetFloatValue (NSAttributedStringDocumentAttributeKey.NSViewZoomDocumentAttribute);
			}
			set {
				if (value is null)
					RemoveValue (NSAttributedStringDocumentAttributeKey.NSViewZoomDocumentAttribute);
				else
					SetNumberValue (NSAttributedStringDocumentAttributeKey.NSViewZoomDocumentAttribute, value);
			}
		}

		public NSDocumentViewMode? ViewMode {
			get {
				var value = GetInt32Value (NSAttributedStringDocumentAttributeKey.NSViewModeDocumentAttribute);
				if (value is null)
					return null;
				else
					return (NSDocumentViewMode) value.Value;
			}
			set {
				if (value is null)
					RemoveValue (NSAttributedStringDocumentAttributeKey.NSViewModeDocumentAttribute);
				else
					SetNumberValue (NSAttributedStringDocumentAttributeKey.NSViewModeDocumentAttribute, (int) value.Value);
			}
		}

		public bool ReadOnly {
			get {
				var value = GetInt32Value (NSAttributedStringDocumentAttributeKey.NSReadOnlyDocumentAttribute);
				if (value is null || value.Value <= 0)
					return false;
				return true;
			}
			set {
				SetNumberValue (NSAttributedStringDocumentAttributeKey.NSReadOnlyDocumentAttribute, value ? 1 : 0);
			}
		}

		public UIColor? BackgroundColor {
			get {
				NSObject? value;
				Dictionary.TryGetValue (NSAttributedStringDocumentAttributeKey.NSBackgroundColorDocumentAttribute, out value);
				return value as UIColor;
			}
			set {
				if (value is null)
					RemoveValue (NSAttributedStringDocumentAttributeKey.NSBackgroundColorDocumentAttribute);
				else
					Dictionary [NSAttributedStringDocumentAttributeKey.NSBackgroundColorDocumentAttribute] = value;
			}
		}

		public float? HyphenationFactor {
			get {
				return GetFloatValue (NSAttributedStringDocumentAttributeKey.NSHyphenationFactorDocumentAttribute);
			}
			set {
				if (value is null)
					RemoveValue (NSAttributedStringDocumentAttributeKey.NSHyphenationFactorDocumentAttribute);
				else {
					if (value < 0 || value > 1.0f)
						throw new ArgumentException ("value must be between 0 and 1");
					SetNumberValue (NSAttributedStringDocumentAttributeKey.NSHyphenationFactorDocumentAttribute, value);
				}
			}
		}

		public float? DefaultTabInterval {
			get {
				return GetFloatValue (NSAttributedStringDocumentAttributeKey.NSDefaultTabIntervalDocumentAttribute);
			}
			set {
				if (value is null)
					RemoveValue (NSAttributedStringDocumentAttributeKey.NSDefaultTabIntervalDocumentAttribute);
				else {
					if (value < 0 || value > 1.0f)
						throw new ArgumentException ("value must be between 0 and 1");
					SetNumberValue (NSAttributedStringDocumentAttributeKey.NSDefaultTabIntervalDocumentAttribute, value);
				}
			}
		}

		public NSDictionary? WeakDefaultAttributes {
			get {
				NSObject? value;
				Dictionary.TryGetValue (NSAttributedStringDocumentAttributeKey.NSDefaultAttributesDocumentAttribute, out value);
				return value as NSDictionary;
			}
			set {
				if (value is null)
					RemoveValue (NSAttributedStringDocumentAttributeKey.NSDefaultAttributesDocumentAttribute);
				else
					Dictionary [NSAttributedStringDocumentAttributeKey.NSDefaultAttributesDocumentAttribute] = value;
			}
		}
#endif
#if !COREBUILD && !TVOS && !WATCH
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
				Dictionary.TryGetValue (NSAttributedStringDocumentReadingOptionKeys.ReadAccessUrlKey, out var value);
				return value as NSUrl;
			}
			set {
				if (value is null)
					RemoveValue (NSAttributedStringDocumentReadingOptionKeys.ReadAccessUrlKey);
				else
					Dictionary [NSAttributedStringDocumentReadingOptionKeys.ReadAccessUrlKey] = value;
			}
		}
#endif
	}
}
