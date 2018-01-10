// 
// UIStringAttributes.cs: Implements strongly typed access to UIKit specific part of UIStringAttributeKey
//
// Authors:
//   Marek Safar (marek.safar@gmail.com)
//   Miguel de Icaza (miguel@xamarin.com)
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

using System;

using ObjCRuntime;
using Foundation;
using CoreFoundation;
using CoreGraphics;
#if !WATCH
using CoreText;
#endif

namespace UIKit {

	[iOS (6,0)]
	public class UIStringAttributes : DictionaryContainer
	{
#if !COREBUILD
		public UIStringAttributes ()
			: base (new NSMutableDictionary ())
		{
		}

		public UIStringAttributes (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		public UIColor ForegroundColor {
			get {
				return Dictionary [UIStringAttributeKey.ForegroundColor] as UIColor;
			}
			set {
				SetNativeValue (UIStringAttributeKey.ForegroundColor, value);
			}
		}

		public UIColor BackgroundColor {
			get {
				return Dictionary [UIStringAttributeKey.BackgroundColor] as UIColor;
			}
			set {
				SetNativeValue (UIStringAttributeKey.BackgroundColor, value);
			}			
		}

		public UIFont Font {
			get {
				return Dictionary [UIStringAttributeKey.Font] as UIFont;
			}
			set {
				SetNativeValue (UIStringAttributeKey.Font, value);
			}
		}

		public float? KerningAdjustment {
			get {
				return GetFloatValue (UIStringAttributeKey.KerningAdjustment);
			}
			set {
				SetNumberValue (UIStringAttributeKey.KerningAdjustment, value);
			}
		}

		public NSLigatureType? Ligature {
			get {
				var value = GetInt32Value (UIStringAttributeKey.Ligature);
				return value == null ? null : (NSLigatureType?) value.Value;
			}
			set {
				SetNumberValue (UIStringAttributeKey.Ligature, (int?) value);
			}
		}

		public NSParagraphStyle ParagraphStyle {
			get {
				return Dictionary [UIStringAttributeKey.ParagraphStyle] as NSParagraphStyle;
			}
			set {
				SetNativeValue (UIStringAttributeKey.ParagraphStyle, value);
			}
		}

		public NSUnderlineStyle? StrikethroughStyle {
			get {
				var value = GetInt32Value (UIStringAttributeKey.StrikethroughStyle);
				return value == null ? null : (NSUnderlineStyle?) value.Value;
			}
			set {
				SetNumberValue (UIStringAttributeKey.StrikethroughStyle, (int?) value);
			}
		}

		public UIColor StrokeColor {
			get {
				return Dictionary [UIStringAttributeKey.StrokeColor] as UIColor;
			}
			set {
				SetNativeValue (UIStringAttributeKey.StrokeColor, value);
			}			
		}

		public float? StrokeWidth {
			get {
				return GetFloatValue (UIStringAttributeKey.StrokeWidth);
			}
			set {
				SetNumberValue (UIStringAttributeKey.StrokeWidth, value);
			}
		}

#if !WATCH
		public NSShadow Shadow {
			get {
				return Dictionary [UIStringAttributeKey.Shadow] as NSShadow;
			}
			set {
				SetNativeValue (UIStringAttributeKey.Shadow, value);
			}			
		}
#endif // !WATCH

		public NSUnderlineStyle? UnderlineStyle {
			get {
				var value = GetInt32Value (UIStringAttributeKey.UnderlineStyle);
				return value == null ? null : (NSUnderlineStyle?) value.Value;
			}
			set {
				SetNumberValue (UIStringAttributeKey.UnderlineStyle, (int?) value);
			}
		}

		[iOS (7,0)]
		public NSString WeakTextEffect {
			get {
				return Dictionary [UIStringAttributeKey.TextEffect] as NSString;
			}
			set {
				SetStringValue (UIStringAttributeKey.TextEffect, value);
			}
		}

		[iOS (7,0)]
		public NSTextEffect TextEffect {
			get {
				var s = WeakTextEffect;
				if (s == null)
					return NSTextEffect.None;
				
				if (s == UIStringAttributeKey.NSTextEffectLetterpressStyle)
					return NSTextEffect.LetterPressStyle;
				return NSTextEffect.UnknownUseWeakEffect;
			}
			set {
				if (value == NSTextEffect.LetterPressStyle)
					SetStringValue (UIStringAttributeKey.TextEffect, UIStringAttributeKey.NSTextEffectLetterpressStyle);
				else
					SetStringValue (UIStringAttributeKey.TextEffect, (NSString) null);
			}
		}

#if !WATCH
		[iOS (7,0)]
		public NSTextAttachment TextAttachment {
			get {
				return Dictionary [UIStringAttributeKey.Attachment] as NSTextAttachment;
			}
			set {
				SetNativeValue (UIStringAttributeKey.Attachment, value);
			}
		}
#endif // !WATCH

		[iOS (7,0)]
		public NSUrl Link {
			get {
				return Dictionary [UIStringAttributeKey.Link] as NSUrl;
			}
			set {
				SetNativeValue (UIStringAttributeKey.Link, value);
			}
		}

		[iOS (7,0)]
		public float? BaselineOffset {
			get {
				return GetFloatValue (UIStringAttributeKey.BaselineOffset);
			}
			set {
				SetNumberValue (UIStringAttributeKey.BaselineOffset, value);
			}
		}

		[iOS (7,0)]
		public UIColor StrikethroughColor {
			get {
				return Dictionary [UIStringAttributeKey.StrikethroughColor] as UIColor;
			}
			set {
				SetNativeValue (UIStringAttributeKey.StrikethroughColor, value);
			}
		}

		[iOS (7,0)]
		public UIColor UnderlineColor {
			get {
				return Dictionary [UIStringAttributeKey.UnderlineColor] as UIColor;
			}
			set {
				SetNativeValue (UIStringAttributeKey.UnderlineColor, value);
			}
		}
		

		[iOS (7,0)]
		public float? Obliqueness {
			get {
				return GetFloatValue (UIStringAttributeKey.Obliqueness);
			}
			set {
				SetNumberValue (UIStringAttributeKey.Obliqueness, value);
			}
		}

		[iOS (7,0)]
		public float? Expansion {
			get {
				return GetFloatValue (UIStringAttributeKey.Expansion);
			}
			set {
				SetNumberValue (UIStringAttributeKey.Expansion, value);
			}
		}

		[iOS (7,0)]
		public NSNumber [] WritingDirectionInt {
			get {
				return GetArray<NSNumber> (UIStringAttributeKey.WritingDirection);
			}
			set {
				SetArrayValue (UIStringAttributeKey.WritingDirection, value);
			}
		}
#endif
	}
}

