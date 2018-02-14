#if IOS

using System;

using UIKit;
using CoreGraphics;
using Foundation;
using ObjCRuntime;

namespace UIKit {
	public unsafe static partial class UIStringDrawing  {
		[Deprecated (PlatformName.iOS, 7, 0, message : "Use NSString.DrawString(CGPoint, UIStringAttributes) instead.")]
		public static CGSize DrawString (this string This, CGPoint point, UIFont font)
		{
			using (var self = ((NSString) This))
				return self.DrawString (point, font);
		}

		[Deprecated (PlatformName.iOS, 7, 0, message : "Use NSString.DrawString(CGRect, UIStringAttributes) instead.")]
		public static CGSize DrawString (this string This, CGPoint point, global::System.nfloat width, UIFont font, UILineBreakMode breakMode)
		{
			using (var self = ((NSString) This))
				return self.DrawString (point, width, font, breakMode);
		}

		[Deprecated (PlatformName.iOS, 7, 0, message : "Use NSString.DrawString(CGRect, UIStringAttributes) instead.")]
		public static CGSize DrawString (this string This, CGPoint point, global::System.nfloat width, UIFont font, global::System.nfloat fontSize, UILineBreakMode breakMode, UIBaselineAdjustment adjustment)
		{
			using (var self = ((NSString) This))
				return self.DrawString (point, width, font, fontSize, breakMode, adjustment);
		}

		[Deprecated (PlatformName.iOS, 7, 0, message : "Use NSString.DrawString(CGRect, UIStringAttributes) instead.")]
		public static CGSize DrawString (this string This, CGPoint point, global::System.nfloat width, UIFont font, global::System.nfloat minFontSize, ref global::System.nfloat actualFontSize, UILineBreakMode breakMode, UIBaselineAdjustment adjustment)
		{
			using (var self = ((NSString) This))
				return self.DrawString (point, width, font, minFontSize, ref actualFontSize, breakMode, adjustment);
		}

		[Deprecated (PlatformName.iOS, 7, 0, message : "Use NSString.DrawString(CGRect, UIStringAttributes) instead.")]
		public static CGSize DrawString (this string This, CGRect rect, UIFont font)
		{
			using (var self = ((NSString) This))
				return self.DrawString (rect, font);
		}

		[Deprecated (PlatformName.iOS, 7, 0, message : "Use NSString.DrawString(CGRect, UIStringAttributes) instead.")]
		public static CGSize DrawString (this string This, CGRect rect, UIFont font, UILineBreakMode mode)
		{
			using (var self = ((NSString) This))
				return self.DrawString (rect, font, mode);
		}

		[Deprecated (PlatformName.iOS, 7, 0, message : "Use NSString.DrawString(CGRect, UIStringAttributes) instead.")]
		public static CGSize DrawString (this string This, CGRect rect, UIFont font, UILineBreakMode mode, UITextAlignment alignment)
		{
			using (var self = ((NSString) This))
				return self.DrawString (rect, font, mode, alignment);
		}

		[Deprecated (PlatformName.iOS, 7, 0, message : "Use NSString.GetSizeUsingAttributes(UIStringAttributes) instead.")]
		public static CGSize StringSize (this string This, UIFont font)
		{
			using (var self = ((NSString) This))
				return self.StringSize (font);
		}

		[Deprecated (PlatformName.iOS, 7, 0, message : "Use NSString.GetBoundingRect (CGSize, NSStringDrawingOptions, UIStringAttributes, NSStringDrawingContext) instead.")]
		public static CGSize StringSize (this string This, UIFont font, global::System.nfloat forWidth, UILineBreakMode breakMode)
		{
			using (var self = ((NSString) This))
				return self.StringSize (font, forWidth, breakMode);
		}

		[Deprecated (PlatformName.iOS, 7, 0, message : "Use NSString.GetBoundingRect (CGSize, NSStringDrawingOptions, UIStringAttributes, NSStringDrawingContext) instead.")]
		public static CGSize StringSize (this string This, UIFont font, CGSize constrainedToSize)
		{
			using (var self = ((NSString) This))
				return self.StringSize (font, constrainedToSize);
		}

		[Deprecated (PlatformName.iOS, 7, 0, message : "Use NSString.GetBoundingRect (CGSize, NSStringDrawingOptions, UIStringAttributes, NSStringDrawingContext) instead.")]
		public static CGSize StringSize (this string This, UIFont font, CGSize constrainedToSize, UILineBreakMode lineBreakMode)
		{
			using (var self = ((NSString) This))
				return self.StringSize (font, constrainedToSize, lineBreakMode);
		}

		[Deprecated (PlatformName.iOS, 7, 0)]
		public static CGSize StringSize (this string This, UIFont font, global::System.nfloat minFontSize, ref global::System.nfloat actualFontSize, global::System.nfloat forWidth, UILineBreakMode lineBreakMode)
		{
			using (var self = ((NSString) This))
				return self.StringSize (font, minFontSize, ref actualFontSize, forWidth, lineBreakMode);
		}
	}
}

#endif // IOS
