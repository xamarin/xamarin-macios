#if IOS

using System;

using UIKit;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using System.Runtime.Versioning;

namespace UIKit {
	public unsafe static partial class UIStringDrawing  {
#if !NET
		[Deprecated (PlatformName.iOS, 7, 0, message : "Use NSString.DrawString(CGPoint, UIStringAttributes) instead.")]
#else
		[UnsupportedOSPlatform ("ios7.0")]
		[Obsolete ("Starting with ios7.0 use NSString.DrawString(CGPoint, UIStringAttributes) instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		public static CGSize DrawString (this string This, CGPoint point, UIFont font)
		{
			using (var self = ((NSString) This))
				return self.DrawString (point, font);
		}

#if !NET
		[Deprecated (PlatformName.iOS, 7, 0, message : "Use NSString.DrawString(CGRect, UIStringAttributes) instead.")]
#else
		[UnsupportedOSPlatform ("ios7.0")]
		[Obsolete ("Starting with ios7.0 use NSString.DrawString(CGRect, UIStringAttributes) instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		public static CGSize DrawString (this string This, CGPoint point, global::System.nfloat width, UIFont font, UILineBreakMode breakMode)
		{
			using (var self = ((NSString) This))
				return self.DrawString (point, width, font, breakMode);
		}

#if !NET
		[Deprecated (PlatformName.iOS, 7, 0, message : "Use NSString.DrawString(CGRect, UIStringAttributes) instead.")]
#else
		[UnsupportedOSPlatform ("ios7.0")]
		[Obsolete ("Starting with ios7.0 use NSString.DrawString(CGRect, UIStringAttributes) instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		public static CGSize DrawString (this string This, CGPoint point, global::System.nfloat width, UIFont font, global::System.nfloat fontSize, UILineBreakMode breakMode, UIBaselineAdjustment adjustment)
		{
			using (var self = ((NSString) This))
				return self.DrawString (point, width, font, fontSize, breakMode, adjustment);
		}

#if !NET
		[Deprecated (PlatformName.iOS, 7, 0, message : "Use NSString.DrawString(CGRect, UIStringAttributes) instead.")]
#else
		[UnsupportedOSPlatform ("ios7.0")]
		[Obsolete ("Starting with ios7.0 use NSString.DrawString(CGRect, UIStringAttributes) instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		public static CGSize DrawString (this string This, CGPoint point, global::System.nfloat width, UIFont font, global::System.nfloat minFontSize, ref global::System.nfloat actualFontSize, UILineBreakMode breakMode, UIBaselineAdjustment adjustment)
		{
			using (var self = ((NSString) This))
				return self.DrawString (point, width, font, minFontSize, ref actualFontSize, breakMode, adjustment);
		}

#if !NET
		[Deprecated (PlatformName.iOS, 7, 0, message : "Use NSString.DrawString(CGRect, UIStringAttributes) instead.")]
#else
		[UnsupportedOSPlatform ("ios7.0")]
		[Obsolete ("Starting with ios7.0 use NSString.DrawString(CGRect, UIStringAttributes) instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		public static CGSize DrawString (this string This, CGRect rect, UIFont font)
		{
			using (var self = ((NSString) This))
				return self.DrawString (rect, font);
		}

#if !NET
		[Deprecated (PlatformName.iOS, 7, 0, message : "Use NSString.DrawString(CGRect, UIStringAttributes) instead.")]
#else
		[UnsupportedOSPlatform ("ios7.0")]
		[Obsolete ("Starting with ios7.0 use NSString.DrawString(CGRect, UIStringAttributes) instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		public static CGSize DrawString (this string This, CGRect rect, UIFont font, UILineBreakMode mode)
		{
			using (var self = ((NSString) This))
				return self.DrawString (rect, font, mode);
		}

#if !NET
		[Deprecated (PlatformName.iOS, 7, 0, message : "Use NSString.DrawString(CGRect, UIStringAttributes) instead.")]
#else
		[UnsupportedOSPlatform ("ios7.0")]
		[Obsolete ("Starting with ios7.0 use NSString.DrawString(CGRect, UIStringAttributes) instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		public static CGSize DrawString (this string This, CGRect rect, UIFont font, UILineBreakMode mode, UITextAlignment alignment)
		{
			using (var self = ((NSString) This))
				return self.DrawString (rect, font, mode, alignment);
		}

#if !NET
		[Deprecated (PlatformName.iOS, 7, 0, message : "Use NSString.GetSizeUsingAttributes(UIStringAttributes) instead.")]
#else
		[UnsupportedOSPlatform ("ios7.0")]
		[Obsolete ("Starting with ios7.0 use NSString.GetSizeUsingAttributes(UIStringAttributes) instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		public static CGSize StringSize (this string This, UIFont font)
		{
			using (var self = ((NSString) This))
				return self.StringSize (font);
		}

#if !NET
		[Deprecated (PlatformName.iOS, 7, 0, message : "Use NSString.GetBoundingRect (CGSize, NSStringDrawingOptions, UIStringAttributes, NSStringDrawingContext) instead.")]
#else
		[UnsupportedOSPlatform ("ios7.0")]
		[Obsolete ("Starting with ios7.0 use NSString.GetBoundingRect (CGSize, NSStringDrawingOptions, UIStringAttributes, NSStringDrawingContext) instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		public static CGSize StringSize (this string This, UIFont font, global::System.nfloat forWidth, UILineBreakMode breakMode)
		{
			using (var self = ((NSString) This))
				return self.StringSize (font, forWidth, breakMode);
		}

#if !NET
		[Deprecated (PlatformName.iOS, 7, 0, message : "Use NSString.GetBoundingRect (CGSize, NSStringDrawingOptions, UIStringAttributes, NSStringDrawingContext) instead.")]
#else
		[UnsupportedOSPlatform ("ios7.0")]
		[Obsolete ("Starting with ios7.0 use NSString.GetBoundingRect (CGSize, NSStringDrawingOptions, UIStringAttributes, NSStringDrawingContext) instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		public static CGSize StringSize (this string This, UIFont font, CGSize constrainedToSize)
		{
			using (var self = ((NSString) This))
				return self.StringSize (font, constrainedToSize);
		}

#if !NET
		[Deprecated (PlatformName.iOS, 7, 0, message : "Use NSString.GetBoundingRect (CGSize, NSStringDrawingOptions, UIStringAttributes, NSStringDrawingContext) instead.")]
#else
		[UnsupportedOSPlatform ("ios7.0")]
		[Obsolete ("Starting with ios7.0 use NSString.GetBoundingRect (CGSize, NSStringDrawingOptions, UIStringAttributes, NSStringDrawingContext) instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		public static CGSize StringSize (this string This, UIFont font, CGSize constrainedToSize, UILineBreakMode lineBreakMode)
		{
			using (var self = ((NSString) This))
				return self.StringSize (font, constrainedToSize, lineBreakMode);
		}

#if !NET
		[Deprecated (PlatformName.iOS, 7, 0)]
#else
		[UnsupportedOSPlatform ("ios7.0")]
#endif
		public static CGSize StringSize (this string This, UIFont font, global::System.nfloat minFontSize, ref global::System.nfloat actualFontSize, global::System.nfloat forWidth, UILineBreakMode lineBreakMode)
		{
			using (var self = ((NSString) This))
				return self.StringSize (font, minFontSize, ref actualFontSize, forWidth, lineBreakMode);
		}
	}
}

#endif // IOS
