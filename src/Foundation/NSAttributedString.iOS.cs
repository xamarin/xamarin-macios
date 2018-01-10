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

#if !MONOMAC

using System;

using ObjCRuntime;
using Foundation;
using UIKit;
using CoreGraphics;

namespace Foundation {

#if !COREBUILD
	public partial class NSAttributedString {
		static NSDictionary ignore;

		public NSAttributedString (NSUrl url, NSAttributedStringDocumentAttributes documentAttributes, ref NSError error)
		: this (url, documentAttributes, out ignore, ref error) {}

		public NSAttributedString (NSData data, NSAttributedStringDocumentAttributes documentAttributes, ref NSError error)
		: this (data, documentAttributes, out ignore, ref error) {}

		public NSAttributedString (NSUrl url, ref NSError error)
		: this (url, (NSDictionary) null, out ignore, ref error) {}

		public NSAttributedString (NSData data, ref NSError error)
		: this (data, (NSDictionary) null, out ignore, ref error) {}

#if IOS // not TVOS or WATCH
		// use the best selector based on the OS version
		public NSAttributedString (NSUrl url, NSDictionary options, out NSDictionary resultDocumentAttributes, ref NSError error)
		{
			if (UIDevice.CurrentDevice.CheckSystemVersion (9,0))
				Handle = InitWithURL (url, options, out resultDocumentAttributes, ref error);
			else
				Handle = InitWithFileURL (url, options, out resultDocumentAttributes, ref error);

			if (Handle == IntPtr.Zero)
				throw new ArgumentException ();
		}
#endif

	}
#endif
	
	public partial class NSAttributedStringDocumentAttributes : DictionaryContainer {
#if !COREBUILD
		public NSAttributedStringDocumentAttributes () : base (new NSMutableDictionary ()) {}
		public NSAttributedStringDocumentAttributes (NSDictionary dictionary) : base (dictionary) {}

		public NSStringEncoding? StringEncoding {
			get {
				var value = GetInt32Value (UIStringAttributeKey.NSCharacterEncodingDocumentAttribute);
				if (value == null)
					return null;
				else
					return (NSStringEncoding) value.Value;
			}
			set {
				SetNumberValue (UIStringAttributeKey.NSCharacterEncodingDocumentAttribute, (int?) value);
			}
		}
		
		public NSString WeakDocumentType {
			get {
				return GetNSStringValue (UIStringAttributeKey.NSDocumentTypeDocumentAttribute);
			}
			set {
				SetStringValue (UIStringAttributeKey.NSDocumentTypeDocumentAttribute, value);
			}
		}
		
		public NSDocumentType DocumentType {
			get {
				var s = GetNSStringValue (UIStringAttributeKey.NSDocumentTypeDocumentAttribute);
				if (s == UIStringAttributeKey.NSPlainTextDocumentType)
					return NSDocumentType.PlainText;
				if (s == UIStringAttributeKey.NSRTFDTextDocumentType)
					return NSDocumentType.RTFD;
				if (s == UIStringAttributeKey.NSRTFTextDocumentType)
					return NSDocumentType.RTF;
				if (s == UIStringAttributeKey.NSHTMLTextDocumentType)
					return NSDocumentType.HTML;
				return NSDocumentType.Unknown;
			}

			set {
				switch (value){
				case NSDocumentType.PlainText:
					SetStringValue (UIStringAttributeKey.NSDocumentTypeDocumentAttribute, UIStringAttributeKey.NSPlainTextDocumentType);
					break;
				case NSDocumentType.RTFD:
					SetStringValue (UIStringAttributeKey.NSDocumentTypeDocumentAttribute, UIStringAttributeKey.NSRTFDTextDocumentType);
					break;
				case NSDocumentType.RTF:
					SetStringValue (UIStringAttributeKey.NSDocumentTypeDocumentAttribute, UIStringAttributeKey.NSRTFTextDocumentType);
					break;
				case NSDocumentType.HTML:
					SetStringValue (UIStringAttributeKey.NSDocumentTypeDocumentAttribute, UIStringAttributeKey.NSHTMLTextDocumentType);
					break;
				}
			}
		}

		public CGSize? PaperSize {
			get {
				NSObject value;
				Dictionary.TryGetValue (UIStringAttributeKey.NSPaperSizeDocumentAttribute, out value);
				var size = value as NSValue;
				if (size != null)
					return size.CGSizeValue;
				return null;
			}
			set {
				if (value == null)
					RemoveValue (UIStringAttributeKey.NSPaperSizeDocumentAttribute);
				else
					Dictionary [UIStringAttributeKey.NSPaperSizeDocumentAttribute] = NSValue.FromCGSize (value.Value);
			}
		}

		public UIEdgeInsets? PaperMargin {
			get {
				NSObject value;
				Dictionary.TryGetValue (UIStringAttributeKey.NSPaperMarginDocumentAttribute, out value);
				var size = value as NSValue;
				if (size != null)
					return size.UIEdgeInsetsValue;
				return null;
			}
			set {
				if (value == null)
					RemoveValue (UIStringAttributeKey.NSPaperMarginDocumentAttribute);
				else
					Dictionary [UIStringAttributeKey.NSPaperMarginDocumentAttribute] = NSValue.FromUIEdgeInsets (value.Value);
			}
		}
		
		public CGSize? ViewSize {
			get {
				NSObject value;
				Dictionary.TryGetValue (UIStringAttributeKey.NSViewSizeDocumentAttribute, out value);
				var size = value as NSValue;
				if (size != null)
					return size.CGSizeValue;
				return null;
			}
			set {
				if (value == null)
					RemoveValue (UIStringAttributeKey.NSViewSizeDocumentAttribute);
				else
					Dictionary [UIStringAttributeKey.NSViewSizeDocumentAttribute] = NSValue.FromCGSize (value.Value);
			}
		}

		public float? ViewZoom {
			get {
				return GetFloatValue (UIStringAttributeKey.NSViewZoomDocumentAttribute);
			}
			set {
				if (value == null)
					RemoveValue (UIStringAttributeKey.NSViewZoomDocumentAttribute);
				else
					SetNumberValue (UIStringAttributeKey.NSViewZoomDocumentAttribute, value);
			}
		}

		public NSDocumentViewMode? ViewMode {
			get {
				var value = GetInt32Value (UIStringAttributeKey.NSViewModeDocumentAttribute);
				if (value == null)
					return null;
				else
					return (NSDocumentViewMode) value.Value;
			}
			set {
				if (value == null)
					RemoveValue (UIStringAttributeKey.NSViewModeDocumentAttribute);
				else
					SetNumberValue (UIStringAttributeKey.NSViewModeDocumentAttribute, (int) value.Value);
			}
		}

		public bool ReadOnly {
			get {
				var value = GetInt32Value (UIStringAttributeKey.NSReadOnlyDocumentAttribute);
				if (value == null || value.Value <= 0)
					return false;
				return true;
			}
			set {
				SetNumberValue (UIStringAttributeKey.NSReadOnlyDocumentAttribute, value ? 1 : 0);
			}
		}

		public UIColor BackgroundColor {
			get {
				NSObject value;
				Dictionary.TryGetValue (UIStringAttributeKey.NSBackgroundColorDocumentAttribute, out value);
				return value as UIColor;
			}
			set {
				if (value == null)
					RemoveValue (UIStringAttributeKey.NSBackgroundColorDocumentAttribute);
				else
					Dictionary [UIStringAttributeKey.NSBackgroundColorDocumentAttribute] = value;
			}
		}

		public float? HyphenationFactor {
			get {
				return GetFloatValue (UIStringAttributeKey.NSReadOnlyDocumentAttribute);
			}
			set {
				if (value == null)
					RemoveValue (UIStringAttributeKey.NSReadOnlyDocumentAttribute);
				else {
					if (value < 0 || value > 1.0f)
						throw new ArgumentException ("value must be between 0 and 1");
					SetNumberValue (UIStringAttributeKey.NSReadOnlyDocumentAttribute, value);
				}
			}
		}

		public float? DefaultTabInterval {
			get {
				return GetFloatValue (UIStringAttributeKey.NSDefaultTabIntervalDocumentAttribute);
			}
			set {
				if (value == null)
					RemoveValue (UIStringAttributeKey.NSDefaultTabIntervalDocumentAttribute);
				else {
					if (value < 0 || value > 1.0f)
						throw new ArgumentException ("value must be between 0 and 1");
					SetNumberValue (UIStringAttributeKey.NSDefaultTabIntervalDocumentAttribute, value);
				}
			}
		}

		public NSDictionary WeakDefaultAttributes {
			get {
				NSObject value;
				Dictionary.TryGetValue (UIStringAttributeKey.NSDefaultAttributesDocumentAttribute, out value);
				return value as NSDictionary;
			}
			set {
				if (value == null)
					RemoveValue (UIStringAttributeKey.NSDefaultAttributesDocumentAttribute);
				else
					Dictionary [UIStringAttributeKey.NSDefaultAttributesDocumentAttribute] = value;
			}
		}
#endif
	}
}

#endif // !MONOMAC
