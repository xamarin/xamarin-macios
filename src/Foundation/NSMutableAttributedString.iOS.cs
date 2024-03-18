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

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Foundation {

	public partial class NSMutableAttributedString {
		public void SetAttributes (UIStringAttributes attrs, NSRange range)
		{
			SetAttributes (attrs is null ? null : attrs.Dictionary, range);
		}

		public void AddAttributes (UIStringAttributes attrs, NSRange range)
		{
			AddAttributes (attrs is null ? null : attrs.Dictionary, range);
		}

	}
}

#endif // !MONOMAC
#endif // !WATCH
