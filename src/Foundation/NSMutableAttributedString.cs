// 
// NSMutableAttributedString.cs: Helpers and overloads for NSMutableAttributedString members.
//
// Authors: Mono Team
//     
// Copyright 2010 Novell, Inc
// Copyright 2011, 2012 Xamarin Inc
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
//

#if !WATCH // This file needs some work before it can get included in WatchOS

using System;
#if !MONOMAC
using UIKit;
#endif

using CoreText;

namespace Foundation {

	public partial class NSMutableAttributedString {

		public NSMutableAttributedString (string str, CTStringAttributes attributes)
			: this (str, attributes is null ? null : attributes.Dictionary)
		{
		}

		public void SetAttributes (NSDictionary attributes, NSRange range)
		{
			if (attributes is null)
				throw new ArgumentNullException ("attributes");

			LowLevelSetAttributes (attributes.Handle, range);
		}

		public void SetAttributes (CTStringAttributes attrs, NSRange range)
		{
			SetAttributes (attrs is null ? null : attrs.Dictionary, range);
		}

		public void AddAttributes (CTStringAttributes attrs, NSRange range)
		{
			AddAttributes (attrs is null ? null : attrs.Dictionary, range);
		}

		public void Append (NSAttributedString first, params object [] rest)
		{
			Append (first);
			foreach (var obj in rest) {
				if (obj is NSAttributedString)
					Append ((NSAttributedString) obj);
				else if (obj is string)
					Append (new NSAttributedString ((string) obj));
				else
					Append (new NSAttributedString (obj.ToString ()));

			}
		}
#if !MONOMAC
		public NSMutableAttributedString (string str, UIStringAttributes attributes)
		: this (str, attributes is not null ? attributes.Dictionary : null)
		{
		}

		public NSMutableAttributedString (string str,
						  UIFont font = null,
						  UIColor foregroundColor = null,
						  UIColor backgroundColor = null,
						  UIColor strokeColor = null,
						  NSParagraphStyle paragraphStyle = null,
						  NSLigatureType ligatures = NSLigatureType.Default,
						  float kerning = 0,
						  NSUnderlineStyle underlineStyle = NSUnderlineStyle.None,
						  NSShadow shadow = null,
						  float strokeWidth = 0,
						  NSUnderlineStyle strikethroughStyle = NSUnderlineStyle.None)
		: this (str, ToDictionary (font, foregroundColor, backgroundColor, strokeColor, paragraphStyle, ligatures, kerning, underlineStyle, shadow, strokeWidth, strikethroughStyle))
		{
		}
#endif
	}
}

#endif // !WATCH
