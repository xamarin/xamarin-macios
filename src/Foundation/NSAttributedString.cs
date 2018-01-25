// 
// NSAttributedString.cs: Implements the managed NSAttributedString
//
// Authors: Marek Safar (marek.safar@gmail.com)
//     
// Copyright 2012, Xamarin Inc.
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
using CoreText;
using ObjCRuntime;
#if !MONOMAC
using UIKit;
#endif

namespace Foundation {
	public partial class NSAttributedString {

		public string Value {
			get {
				return NSString.FromHandle (LowLevelValue);
			}
		}

		public NSDictionary GetAttributes (nint location, out NSRange effectiveRange)
		{
			return Runtime.GetNSObject<NSDictionary> (LowLevelGetAttributes (location, out effectiveRange));
		}
		
		public NSAttributedString (string str, CTStringAttributes attributes)
			: this (str, attributes != null ? attributes.Dictionary : null)
		{
		}

		public CTStringAttributes GetCoreTextAttributes (nint location, out NSRange effectiveRange)
		{
			var attr = GetAttributes (location, out effectiveRange);
			return attr == null ? null : new CTStringAttributes (attr);
		}

		public CTStringAttributes GetCoreTextAttributes (nint location, out NSRange longestEffectiveRange, NSRange rangeLimit)
		{
			var attr = GetAttributes (location, out longestEffectiveRange, rangeLimit);
			return attr == null ? null : new CTStringAttributes (attr);			
		}

		public NSAttributedString Substring (nint start, nint len)
		{
			return Substring (new NSRange (start, len));
		}

#if !MONOMAC
		public NSAttributedString (string str, UIStringAttributes attributes)
			: this (str, attributes != null ? attributes.Dictionary : null)
		{
		}

		public UIStringAttributes GetUIKitAttributes (nint location, out NSRange effectiveRange)
		{
			var attr = GetAttributes (location, out effectiveRange);
			return attr == null ? null : new UIStringAttributes (attr);
		}

		public UIStringAttributes GetUIKitAttributes (nint location, out NSRange longestEffectiveRange, NSRange rangeLimit)
		{
			var attr = GetAttributes (location, out longestEffectiveRange, rangeLimit);
			return attr == null ? null : new UIStringAttributes (attr);			
		}

		static internal NSDictionary ToDictionary (
						  UIFont font,
						  UIColor foregroundColor,
						  UIColor backgroundColor,
						  UIColor strokeColor,
						  NSParagraphStyle paragraphStyle,
						  NSLigatureType ligature,
						  float kerning,
						  NSUnderlineStyle underlineStyle,
#if !WATCH
						  NSShadow shadow,
#endif
						  float strokeWidth,
						  NSUnderlineStyle strikethroughStyle)
		{
			var attr = new UIStringAttributes ();
			if (font != null){
				attr.Font = font;
			}
			if (foregroundColor != null){
				attr.ForegroundColor = foregroundColor;
			}
			if (backgroundColor != null){
				attr.BackgroundColor = backgroundColor;
			}
			if (strokeColor != null){
				attr.StrokeColor = strokeColor;
			}
			if (paragraphStyle != null){
				attr.ParagraphStyle = paragraphStyle;
			}
			if (ligature != NSLigatureType.Default){
				attr.Ligature = ligature;
			}
			if (kerning != 0){
				attr.KerningAdjustment = kerning;
			}
			if (underlineStyle != NSUnderlineStyle.None){
				attr.UnderlineStyle = underlineStyle;
			}
#if !WATCH
			if (shadow != null){
				attr.Shadow = shadow;
			}
#endif
			if (strokeWidth != 0){
				attr.StrokeWidth = strokeWidth;
			}
			if (strikethroughStyle != NSUnderlineStyle.None){
				attr.StrikethroughStyle = strikethroughStyle;
			}
			var dict = attr.Dictionary;
			return dict.Count == 0 ? null : dict;
		}				

		public NSAttributedString (string str,
					   UIFont font = null,
					   UIColor foregroundColor = null,
					   UIColor backgroundColor = null,
					   UIColor strokeColor = null,
					   NSParagraphStyle paragraphStyle = null,
					   NSLigatureType ligatures = NSLigatureType.Default,
					   float kerning = 0,
					   NSUnderlineStyle underlineStyle = NSUnderlineStyle.None,
#if !WATCH
					   NSShadow shadow = null,
#endif
					   float strokeWidth = 0,
					   NSUnderlineStyle strikethroughStyle = NSUnderlineStyle.None)
		: this (str, ToDictionary (font, foregroundColor, backgroundColor, strokeColor, paragraphStyle, ligatures, kerning, underlineStyle,
#if !WATCH
			shadow,
#endif
			strokeWidth, strikethroughStyle))
		{
		}

#if !XAMCORE_3_0
		// This is a [Category] -> C# extension method (see uikit.cs) but it targets on static selector
		// the resulting syntax does not look good in user code so we provide a better looking API
		// https://bugzilla.xamarin.com/show_bug.cgi?id=15268
		// https://trello.com/c/iQpXOxCd/227-category-and-static-methods-selectors
		// note: we cannot reuse the same method name - as it would break compilation of existing apps
		public static NSAttributedString CreateFrom (NSTextAttachment attachment)
		{
			return (null as NSAttributedString).FromTextAttachment (attachment);
		}
#endif
#endif						      
	}
}
