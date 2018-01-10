#if IOS

using System;

using UIKit;
using CoreGraphics;
using Foundation;
using ObjCRuntime;

namespace UIKit {
	public unsafe static partial class UIStringDrawing  {
		[iOS (2, 0)]
		[Deprecated (PlatformName.iOS, 7, 0, message : "Use NSString.DrawString(CGPoint, UIStringAttributes) instead.")]
		public static CGSize DrawString (this string This, CGPoint point, UIFont font)
		{
			using (var self = ((NSString) This))
				return self.DrawString (point, font);
		}

		[iOS (2, 0)]
		[Deprecated (PlatformName.iOS, 7, 0, message : "Use NSString.DrawString(CGRect, UIStringAttributes) instead.")]
		public static CGSize DrawString (this string This, CGPoint point, global::System.nfloat width, UIFont font, UILineBreakMode breakMode)
		{
			using (var self = ((NSString) This))
				return self.DrawString (point, width, font, breakMode);
		}

		[iOS (2, 0)]
		[Deprecated (PlatformName.iOS, 7, 0, message : "Use NSString.DrawString(CGRect, UIStringAttributes) instead.")]
		public static CGSize DrawString (this string This, CGPoint point, global::System.nfloat width, UIFont font, global::System.nfloat fontSize, UILineBreakMode breakMode, UIBaselineAdjustment adjustment)
		{
			using (var self = ((NSString) This))
				return self.DrawString (point, width, font, fontSize, breakMode, adjustment);
		}

		[iOS (2, 0)]
		[Deprecated (PlatformName.iOS, 7, 0, message : "Use NSString.DrawString(CGRect, UIStringAttributes) instead.")]
		public static CGSize DrawString (this string This, CGPoint point, global::System.nfloat width, UIFont font, global::System.nfloat minFontSize, ref global::System.nfloat actualFontSize, UILineBreakMode breakMode, UIBaselineAdjustment adjustment)
		{
			using (var self = ((NSString) This))
				return self.DrawString (point, width, font, minFontSize, ref actualFontSize, breakMode, adjustment);
		}

		[iOS (2, 0)]
		[Deprecated (PlatformName.iOS, 7, 0, message : "Use NSString.DrawString(CGRect, UIStringAttributes) instead.")]
		public static CGSize DrawString (this string This, CGRect rect, UIFont font)
		{
			using (var self = ((NSString) This))
				return self.DrawString (rect, font);
		}

		[iOS (2, 0)]
		[Deprecated (PlatformName.iOS, 7, 0, message : "Use NSString.DrawString(CGRect, UIStringAttributes) instead.")]
		public static CGSize DrawString (this string This, CGRect rect, UIFont font, UILineBreakMode mode)
		{
			using (var self = ((NSString) This))
				return self.DrawString (rect, font, mode);
		}

		[iOS (2, 0)]
		[Deprecated (PlatformName.iOS, 7, 0, message : "Use NSString.DrawString(CGRect, UIStringAttributes) instead.")]
		public static CGSize DrawString (this string This, CGRect rect, UIFont font, UILineBreakMode mode, UITextAlignment alignment)
		{
			using (var self = ((NSString) This))
				return self.DrawString (rect, font, mode, alignment);
		}

		[iOS (2, 0)]
		[Deprecated (PlatformName.iOS, 7, 0, message : "Use NSString.GetSizeUsingAttributes(UIStringAttributes) instead.")]
		public static CGSize StringSize (this string This, UIFont font)
		{
			using (var self = ((NSString) This))
				return self.StringSize (font);
		}

		[iOS (2, 0)]
		[Deprecated (PlatformName.iOS, 7, 0, message : "Use NSString.GetBoundingRect (CGSize, NSStringDrawingOptions, UIStringAttributes, NSStringDrawingContext) instead.")]
		public static CGSize StringSize (this string This, UIFont font, global::System.nfloat forWidth, UILineBreakMode breakMode)
		{
			using (var self = ((NSString) This))
				return self.StringSize (font, forWidth, breakMode);
		}

		[iOS (2, 0)]
		[Deprecated (PlatformName.iOS, 7, 0, message : "Use NSString.GetBoundingRect (CGSize, NSStringDrawingOptions, UIStringAttributes, NSStringDrawingContext) instead.")]
		public static CGSize StringSize (this string This, UIFont font, CGSize constrainedToSize)
		{
			using (var self = ((NSString) This))
				return self.StringSize (font, constrainedToSize);
		}

		[iOS (2, 0)]
		[Deprecated (PlatformName.iOS, 7, 0, message : "Use NSString.GetBoundingRect (CGSize, NSStringDrawingOptions, UIStringAttributes, NSStringDrawingContext) instead.")]
		public static CGSize StringSize (this string This, UIFont font, CGSize constrainedToSize, UILineBreakMode lineBreakMode)
		{
			using (var self = ((NSString) This))
				return self.StringSize (font, constrainedToSize, lineBreakMode);
		}

		[iOS (2, 0)]
		[Deprecated (PlatformName.iOS, 7, 0)]
		public static CGSize StringSize (this string This, UIFont font, global::System.nfloat minFontSize, ref global::System.nfloat actualFontSize, global::System.nfloat forWidth, UILineBreakMode lineBreakMode)
		{
			using (var self = ((NSString) This))
				return self.StringSize (font, minFontSize, ref actualFontSize, forWidth, lineBreakMode);
		}
	}
}

#endif // IOS
