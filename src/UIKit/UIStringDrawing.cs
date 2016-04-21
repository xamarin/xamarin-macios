#if IOS

using System;

using XamCore.UIKit;
using XamCore.CoreGraphics;
using XamCore.Foundation;
using XamCore.ObjCRuntime;

namespace XamCore.UIKit {
	public unsafe static partial class UIStringDrawing  {
		[Availability (Introduced = Platform.iOS_2_0, Deprecated = Platform.iOS_7_0, Message = "Use NSString.DrawString(CGPoint, UIStringAttributes) instead.")]
		public static CGSize DrawString (this string This, CGPoint point, UIFont font)
		{
			using (var self = ((NSString) This))
				return self.DrawString (point, font);
		}

		[Availability (Introduced = Platform.iOS_2_0, Deprecated = Platform.iOS_7_0, Message = "Use NSString.DrawString(CGRect, UIStringAttributes) instead.")]
		public static CGSize DrawString (this string This, CGPoint point, global::System.nfloat width, UIFont font, UILineBreakMode breakMode)
		{
			using (var self = ((NSString) This))
				return self.DrawString (point, width, font, breakMode);
		}

		[Availability (Introduced = Platform.iOS_2_0, Deprecated = Platform.iOS_7_0, Message = "Use NSString.DrawString(CGRect, UIStringAttributes) instead.")]
		public static CGSize DrawString (this string This, CGPoint point, global::System.nfloat width, UIFont font, global::System.nfloat fontSize, UILineBreakMode breakMode, UIBaselineAdjustment adjustment)
		{
			using (var self = ((NSString) This))
				return self.DrawString (point, width, font, fontSize, breakMode, adjustment);
		}

		[Availability (Introduced = Platform.iOS_2_0, Deprecated = Platform.iOS_7_0, Message = "Use NSString.DrawString(CGRect, UIStringAttributes) instead.")]
		public static CGSize DrawString (this string This, CGPoint point, global::System.nfloat width, UIFont font, global::System.nfloat minFontSize, ref global::System.nfloat actualFontSize, UILineBreakMode breakMode, UIBaselineAdjustment adjustment)
		{
			using (var self = ((NSString) This))
				return self.DrawString (point, width, font, minFontSize, ref actualFontSize, breakMode, adjustment);
		}

		[Availability (Introduced = Platform.iOS_2_0, Deprecated = Platform.iOS_7_0, Message = "Use NSString.DrawString(CGRect, UIStringAttributes) instead.")]
		public static CGSize DrawString (this string This, CGRect rect, UIFont font)
		{
			using (var self = ((NSString) This))
				return self.DrawString (rect, font);
		}

		[Availability (Introduced = Platform.iOS_2_0, Deprecated = Platform.iOS_7_0, Message = "Use NSString.DrawString(CGRect, UIStringAttributes) instead.")]
		public static CGSize DrawString (this string This, CGRect rect, UIFont font, UILineBreakMode mode)
		{
			using (var self = ((NSString) This))
				return self.DrawString (rect, font, mode);
		}

		[Availability (Introduced = Platform.iOS_2_0, Deprecated = Platform.iOS_7_0, Message = "Use NSString.DrawString(CGRect, UIStringAttributes) instead.")]
		public static CGSize DrawString (this string This, CGRect rect, UIFont font, UILineBreakMode mode, UITextAlignment alignment)
		{
			using (var self = ((NSString) This))
				return self.DrawString (rect, font, mode, alignment);
		}

		[Availability (Introduced = Platform.iOS_2_0, Deprecated = Platform.iOS_7_0, Message = "Use NSString.GetSizeUsingAttributes(UIStringAttributes) instead.")]
		public static CGSize StringSize (this string This, UIFont font)
		{
			using (var self = ((NSString) This))
				return self.StringSize (font);
		}

		[Availability (Introduced = Platform.iOS_2_0, Deprecated = Platform.iOS_7_0, Message = "Use NSString.GetBoundingRect (CGSize, NSStringDrawingOptions, UIStringAttributes, NSStringDrawingContext) instead.")]
		public static CGSize StringSize (this string This, UIFont font, global::System.nfloat forWidth, UILineBreakMode breakMode)
		{
			using (var self = ((NSString) This))
				return self.StringSize (font, forWidth, breakMode);
		}

		[Availability (Introduced = Platform.iOS_2_0, Deprecated = Platform.iOS_7_0, Message = "Use NSString.GetBoundingRect (CGSize, NSStringDrawingOptions, UIStringAttributes, NSStringDrawingContext) instead.")]
		public static CGSize StringSize (this string This, UIFont font, CGSize constrainedToSize)
		{
			using (var self = ((NSString) This))
				return self.StringSize (font, constrainedToSize);
		}

		[Availability (Introduced = Platform.iOS_2_0, Deprecated = Platform.iOS_7_0, Message = "Use NSString.GetBoundingRect (CGSize, NSStringDrawingOptions, UIStringAttributes, NSStringDrawingContext) instead.")]
		public static CGSize StringSize (this string This, UIFont font, CGSize constrainedToSize, UILineBreakMode lineBreakMode)
		{
			using (var self = ((NSString) This))
				return self.StringSize (font, constrainedToSize, lineBreakMode);
		}

		[Availability (Introduced = Platform.iOS_2_0, Deprecated = Platform.iOS_7_0)]
		public static CGSize StringSize (this string This, UIFont font, global::System.nfloat minFontSize, ref global::System.nfloat actualFontSize, global::System.nfloat forWidth, UILineBreakMode lineBreakMode)
		{
			using (var self = ((NSString) This))
				return self.StringSize (font, minFontSize, ref actualFontSize, forWidth, lineBreakMode);
		}
	}
}

#endif // IOS
