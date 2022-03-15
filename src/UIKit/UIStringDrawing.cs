#if IOS

using System;

using UIKit;
using CoreGraphics;
using Foundation;
using ObjCRuntime;

namespace UIKit {
	public unsafe static partial class UIStringDrawing  {
#if NET
		[UnsupportedOSPlatform ("ios7.0")]
#if IOS
		[Obsolete ("Starting with ios7.0 use NSString.DrawString(CGPoint, UIStringAttributes) instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("tvos")]
#else
		[Deprecated (PlatformName.iOS, 7, 0, message : "Use NSString.DrawString(CGPoint, UIStringAttributes) instead.")]
#endif
		public static CGSize DrawString (this string This, CGPoint point, UIFont font)
		{
			using (var self = ((NSString) This))
				return self.DrawString (point, font);
		}

#if NET
		[UnsupportedOSPlatform ("ios7.0")]
#if IOS
		[Obsolete ("Starting with ios7.0 use NSString.DrawString(CGRect, UIStringAttributes) instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("tvos")]
#else
		[Deprecated (PlatformName.iOS, 7, 0, message : "Use NSString.DrawString(CGRect, UIStringAttributes) instead.")]
#endif
		public static CGSize DrawString (this string This, CGPoint point, nfloat width, UIFont font, UILineBreakMode breakMode)
		{
			using (var self = ((NSString) This))
				return self.DrawString (point, width, font, breakMode);
		}

#if NET
		[UnsupportedOSPlatform ("ios7.0")]
#if IOS
		[Obsolete ("Starting with ios7.0 use NSString.DrawString(CGRect, UIStringAttributes) instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("tvos")]
#else
		[Deprecated (PlatformName.iOS, 7, 0, message : "Use NSString.DrawString(CGRect, UIStringAttributes) instead.")]
#endif
		public static CGSize DrawString (this string This, CGPoint point, nfloat width, UIFont font, nfloat fontSize, UILineBreakMode breakMode, UIBaselineAdjustment adjustment)
		{
			using (var self = ((NSString) This))
				return self.DrawString (point, width, font, fontSize, breakMode, adjustment);
		}

#if NET
		[UnsupportedOSPlatform ("ios7.0")]
#if IOS
		[Obsolete ("Starting with ios7.0 use NSString.DrawString(CGRect, UIStringAttributes) instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("tvos")]
#else
		[Deprecated (PlatformName.iOS, 7, 0, message : "Use NSString.DrawString(CGRect, UIStringAttributes) instead.")]
#endif
		public static CGSize DrawString (this string This, CGPoint point, nfloat width, UIFont font, nfloat minFontSize, ref nfloat actualFontSize, UILineBreakMode breakMode, UIBaselineAdjustment adjustment)
		{
			using (var self = ((NSString) This))
				return self.DrawString (point, width, font, minFontSize, ref actualFontSize, breakMode, adjustment);
		}

#if NET
		[UnsupportedOSPlatform ("ios7.0")]
#if IOS
		[Obsolete ("Starting with ios7.0 use NSString.DrawString(CGRect, UIStringAttributes) instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("tvos")]
#else
		[Deprecated (PlatformName.iOS, 7, 0, message : "Use NSString.DrawString(CGRect, UIStringAttributes) instead.")]
#endif
		public static CGSize DrawString (this string This, CGRect rect, UIFont font)
		{
			using (var self = ((NSString) This))
				return self.DrawString (rect, font);
		}

#if NET
		[UnsupportedOSPlatform ("ios7.0")]
#if IOS
		[Obsolete ("Starting with ios7.0 use NSString.DrawString(CGRect, UIStringAttributes) instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("tvos")]
#else
		[Deprecated (PlatformName.iOS, 7, 0, message : "Use NSString.DrawString(CGRect, UIStringAttributes) instead.")]
#endif
		public static CGSize DrawString (this string This, CGRect rect, UIFont font, UILineBreakMode mode)
		{
			using (var self = ((NSString) This))
				return self.DrawString (rect, font, mode);
		}

#if NET
		[UnsupportedOSPlatform ("ios7.0")]
#if IOS
		[Obsolete ("Starting with ios7.0 use NSString.DrawString(CGRect, UIStringAttributes) instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("tvos")]
#else
		[Deprecated (PlatformName.iOS, 7, 0, message : "Use NSString.DrawString(CGRect, UIStringAttributes) instead.")]
#endif
		public static CGSize DrawString (this string This, CGRect rect, UIFont font, UILineBreakMode mode, UITextAlignment alignment)
		{
			using (var self = ((NSString) This))
				return self.DrawString (rect, font, mode, alignment);
		}

#if NET
		[UnsupportedOSPlatform ("ios7.0")]
#if IOS
		[Obsolete ("Starting with ios7.0 use NSString.GetSizeUsingAttributes(UIStringAttributes) instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("tvos")]
#else
		[Deprecated (PlatformName.iOS, 7, 0, message : "Use NSString.GetSizeUsingAttributes(UIStringAttributes) instead.")]
#endif
		public static CGSize StringSize (this string This, UIFont font)
		{
			using (var self = ((NSString) This))
				return self.StringSize (font);
		}

#if NET
		[UnsupportedOSPlatform ("ios7.0")]
#if IOS
		[Obsolete ("Starting with ios7.0 use NSString.GetBoundingRect (CGSize, NSStringDrawingOptions, UIStringAttributes, NSStringDrawingContext) instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("tvos")]
#else
		[Deprecated (PlatformName.iOS, 7, 0, message : "Use NSString.GetBoundingRect (CGSize, NSStringDrawingOptions, UIStringAttributes, NSStringDrawingContext) instead.")]
#endif
		public static CGSize StringSize (this string This, UIFont font, nfloat forWidth, UILineBreakMode breakMode)
		{
			using (var self = ((NSString) This))
				return self.StringSize (font, forWidth, breakMode);
		}

#if NET
		[UnsupportedOSPlatform ("ios7.0")]
#if IOS
		[Obsolete ("Starting with ios7.0 use NSString.GetBoundingRect (CGSize, NSStringDrawingOptions, UIStringAttributes, NSStringDrawingContext) instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("tvos")]
#else
		[Deprecated (PlatformName.iOS, 7, 0, message : "Use NSString.GetBoundingRect (CGSize, NSStringDrawingOptions, UIStringAttributes, NSStringDrawingContext) instead.")]
#endif
		public static CGSize StringSize (this string This, UIFont font, CGSize constrainedToSize)
		{
			using (var self = ((NSString) This))
				return self.StringSize (font, constrainedToSize);
		}

#if NET
		[UnsupportedOSPlatform ("ios7.0")]
#if IOS
		[Obsolete ("Starting with ios7.0 use NSString.GetBoundingRect (CGSize, NSStringDrawingOptions, UIStringAttributes, NSStringDrawingContext) instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("tvos")]
#else
		[Deprecated (PlatformName.iOS, 7, 0, message : "Use NSString.GetBoundingRect (CGSize, NSStringDrawingOptions, UIStringAttributes, NSStringDrawingContext) instead.")]
#endif
		public static CGSize StringSize (this string This, UIFont font, CGSize constrainedToSize, UILineBreakMode lineBreakMode)
		{
			using (var self = ((NSString) This))
				return self.StringSize (font, constrainedToSize, lineBreakMode);
		}

#if NET
		[UnsupportedOSPlatform ("ios7.0")]
#if IOS
		[Obsolete ("Starting with ios7.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("tvos")]
#else
		[Deprecated (PlatformName.iOS, 7, 0)]
#endif
		public static CGSize StringSize (this string This, UIFont font, nfloat minFontSize, ref nfloat actualFontSize, nfloat forWidth, UILineBreakMode lineBreakMode)
		{
			using (var self = ((NSString) This))
				return self.StringSize (font, minFontSize, ref actualFontSize, forWidth, lineBreakMode);
		}
	}
}

#endif // IOS
