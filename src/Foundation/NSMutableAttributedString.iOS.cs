// 
// NSMutableAttributedString.cs: Helpers and overloads for NSMutableAttributedString members on UIKit
//
//     
// Copyright 2013 Xamarin Inc
//
//

#if !WATCH // NSMutableAttributedString needs some work before it can be included in WatchOS
#if !MONOMAC

using UIKit;
using CoreText;

namespace Foundation {

	public partial class NSMutableAttributedString {
		public void SetAttributes (UIStringAttributes attrs, NSRange range)
		{
			SetAttributes (attrs == null ? null : attrs.Dictionary, range);
		}

		public void AddAttributes (UIStringAttributes attrs, NSRange range)
		{
			AddAttributes (attrs == null ? null : attrs.Dictionary, range);
		}

	}
}

#endif // !MONOMAC
#endif // !WATCH
