#nullable enable

global using nfloat = System.Runtime.InteropServices.NFloat;
global using System.Runtime.InteropServices;

using System;

using CoreGraphics;
using Foundation;
using ObjCRuntime;
#if __IOS__ || __TVOS__ || __MACCATALYST__
using UIKit;
#endif

namespace ObjCRuntime {
	internal static class NFloatHelpers {
		public static nfloat[]? ConvertArray (float[]? input)
		{
			if (input is null)
				return null;

			var rv = new nfloat [input.Length];
			for (var i = 0; i < rv.Length; i++)
				rv [i] = new NFloat (input [i]);
			return rv;
		}

		public static nfloat[]? ConvertArray (double[]? input)
		{
			if (input is null)
				return null;

			var rv = new nfloat [input.Length];
			for (var i = 0; i < rv.Length; i++)
				rv [i] = new NFloat (input [i]);
			return rv;
		}

		public static bool IsNaN              (nfloat f) { return double.IsNaN (f.Value); }
		public static bool IsInfinity         (nfloat f) { return double.IsInfinity (f.Value); }
		public static bool IsPositiveInfinity (nfloat f) { return double.IsPositiveInfinity (f.Value); }
		public static bool IsNegativeInfinity (nfloat f) { return double.IsNegativeInfinity (f.Value); }
		public static NFloat MaxValue {
			get {
				if (IntPtr.Size == 8) {
					return new NFloat (double.MaxValue);
				} else {
					return new NFloat (float.MaxValue);
				}
			}
		}
	}
}

#if __MACOS__ || __MACCATALYST__
namespace AppKit {
#if !__MACCATALYST__
	internal static class NSStackViewNFloatExtensions {
		public static void SetCustomSpacing (this NSStackView self, float arg0, NSView arg1)
		{
			self.SetCustomSpacing (new NFloat (arg0), arg1);
		}

		public static void SetCustomSpacing (this NSStackView self, double arg0, NSView arg1)
		{
			self.SetCustomSpacing (new NFloat (arg0), arg1);
		}
	}

	internal static class NSStatusBarNFloatExtensions {
		public static NSStatusItem CreateStatusItem (this NSStatusBar self, float arg0)
		{
			return self.CreateStatusItem (new NFloat (arg0));
		}

		public static NSStatusItem CreateStatusItem (this NSStatusBar self, double arg0)
		{
			return self.CreateStatusItem (new NFloat (arg0));
		}
	}
#endif // !__MACCATALYST__
}
#endif // __MACOS__ || __MACCATALYST__

namespace CoreGraphics {
	internal static class CGRectNFloatExtensions {
		public static CGRect Inset (this CGRect self, float x, float y)
		{
			return self.Inset (new NFloat (x), new NFloat (y));
		}
	}

	internal static class CGPathNFloatExtensions {
		// AddArc
		public static void AddArc (this CGPath self, CGAffineTransform m, float x, float y, float radius, float startAngle, float endAngle, bool clockwise)
		{
			self.AddArc (m, new NFloat (x), new NFloat (y), new NFloat (radius), new NFloat (startAngle), new NFloat (endAngle), clockwise);
		}

		public static void AddArc (this CGPath self, float x, float y, float radius, float startAngle, float endAngle, bool clockwise)
		{
			self.AddArc (new NFloat (x), new NFloat (y), new NFloat (radius), new NFloat (startAngle), new NFloat (endAngle), clockwise);
		}

		// MoveToPoint
		public static void MoveToPoint (this CGPath self, float x, float y)
		{
			self.MoveToPoint (new NFloat (x), new NFloat (y));
		}

		public static void MoveToPoint (this CGPath self, CGAffineTransform transform, float x, float y)
		{
			self.MoveToPoint (transform, new NFloat (x), new NFloat (y));
		}

		// AddRelativeArc
		public static void AddRelativeArc (this CGPath self, CGAffineTransform m, float x, float y, float radius, float startAngle, float delta)
		{
			self.AddRelativeArc (m, new NFloat (x), new NFloat (y), new NFloat (radius), new NFloat (startAngle), new NFloat (delta));
		}

		public static void AddRelativeArc (this CGPath self, float x, float y, float radius, float startAngle, float delta)
		{
			self.AddRelativeArc (new NFloat (x), new NFloat (y), new NFloat (radius), new NFloat (startAngle), new NFloat (delta));
		}

		// AddArcToPoint
		public static void AddArcToPoint (this CGPath self, CGAffineTransform m, float x1, float y1, float x2, float y2, float radius)
		{
			self.AddArcToPoint (m, new NFloat (x1), new NFloat (y1), new NFloat (x2), new NFloat (y2), new NFloat (radius));
		}

		public static void AddArcToPoint (this CGPath self, float x1, float y1, float x2, float y2, float radius)
		{
			self.AddArcToPoint (new NFloat (x1), new NFloat (y1), new NFloat (x2), new NFloat (y2), new NFloat (radius));
		}

		// AddQuadCurveToPoint
		public static void AddQuadCurveToPoint (this CGPath self, float cpx, float cpy, float x, float y)
		{
			self.AddQuadCurveToPoint (new NFloat (cpx), new NFloat (cpy), new NFloat (x), new NFloat (y));
		}

		public static void AddQuadCurveToPoint (this CGPath self, CGAffineTransform transform, float cpx, float cpy, float x, float y)
		{
			self.AddQuadCurveToPoint (transform, new NFloat (cpx), new NFloat (cpy), new NFloat (x), new NFloat (y));
		}

		// AddCurveToPoint
		public static void AddCurveToPoint (this CGPath self, CGAffineTransform transform, float cp1x, float cp1y, float cp2x, float cp2y, float x, float y)
		{
			self.AddCurveToPoint (transform, new NFloat (cp1x), new NFloat (cp1y), new NFloat (cp2x), new NFloat (cp2y), new NFloat (x), new NFloat (y));
		}

		public static void AddCurveToPoint (this CGPath self, float cp1x, float cp1y, float cp2x, float cp2y, float x, float y)
		{
			self.AddCurveToPoint (new NFloat (cp1x), new NFloat (cp1y), new NFloat (cp2x), new NFloat (cp2y), new NFloat (x), new NFloat (y));
		}

		// AddLineToPoint
		public static void AddLineToPoint (this CGPath self, float x, float y)
		{
			self.AddLineToPoint (new NFloat (x), new NFloat (y));
		}

		public static void AddLineToPoint (this CGPath self, CGAffineTransform transform, float x, float y)
		{
			self.AddLineToPoint (transform, new NFloat (x), new NFloat (y));
		}

		// CopyByDashingPath
		public static CGPath CopyByDashingPath (this CGPath self, CGAffineTransform transform, float []? lengths)
		{
			return self.CopyByDashingPath (transform, NFloatHelpers.ConvertArray (lengths)!);
		}

		public static CGPath CopyByDashingPath (this CGPath self, CGAffineTransform transform, float []? lengths, float phase)
		{
			return self.CopyByDashingPath (transform, NFloatHelpers.ConvertArray (lengths)!, new NFloat (phase));
		}

		public static CGPath CopyByDashingPath (this CGPath self, float []? lengths)
		{
			return self.CopyByDashingPath (NFloatHelpers.ConvertArray (lengths)!);
		}

		public static CGPath CopyByDashingPath (this CGPath self, float []? lengths, float phase)
		{
			return self.CopyByDashingPath (NFloatHelpers.ConvertArray (lengths)!, new NFloat (phase));
		}
	}

	internal static class CGContextNFloatExtensions {
		public static void SetLineDash (this CGContext self, float phase, float []? lengths, int n)
		{
			self.SetLineDash (new NFloat (phase), NFloatHelpers.ConvertArray (lengths), n);
		}

		public static void SetLineDash (this CGContext self, float phase, float []? lengths)
		{
			self.SetLineDash (new NFloat (phase), NFloatHelpers.ConvertArray (lengths));
		}

		public static void SetLineDash (this CGContext self, double phase, double []? lengths, int n)
		{
			self.SetLineDash (new NFloat (phase), NFloatHelpers.ConvertArray (lengths), n);
		}

		public static void SetLineDash (this CGContext self, double phase, double []? lengths)
		{
			self.SetLineDash (new NFloat (phase), NFloatHelpers.ConvertArray (lengths));
		}

		public static void SetFillPattern (this CGContext self, CGPattern pattern, float []? components)
		{
			self.SetFillPattern (pattern, NFloatHelpers.ConvertArray (components));
		}

		public static void MoveTo (this CGContext self, float x, float y)
		{
			self.MoveTo (new NFloat (x), new NFloat (y));
		}

		public static void SetLineWidth (this CGContext self, float w)
		{
			self.SetLineWidth (new NFloat (w));
		}

		public static void RotateCTM (this CGContext self, float value)
		{
			self.RotateCTM (new NFloat (value));
		}

		public static void RotateCTM (this CGContext self, double value)
		{
			self.RotateCTM (new NFloat (value));
		}

		public static void SelectFont (this CGContext self, string fontName, float weight, CGTextEncoding encoding)
		{
			self.SelectFont (fontName, new NFloat (weight), encoding);
		}

		public static void SetFillColor (this CGContext self, float red, float green, float blue, float alpha)
		{
			self.SetFillColor (new NFloat (red), new NFloat (green), new NFloat (blue), new NFloat (alpha));
		}

		public static void TranslateCTM (this CGContext self, float tx, float ty)
		{
			self.TranslateCTM (new NFloat (tx), new NFloat (ty));
		}

		public static void ShowTextAtPoint (this CGContext self, float x, float y, string text)
		{
			self.ShowTextAtPoint (new NFloat (x), new NFloat (y), text);
		}

		public static void AddArc (this CGContext self, float x, float y, float radius, float startAngle, float endAngle, bool clockwise)
		{
			self.AddArc (new NFloat (x), new NFloat (y), new NFloat (radius), new NFloat (startAngle), new NFloat (endAngle), clockwise);
		}

		public static void AddLineToPoint (this CGContext self, float x, float y)
		{
			self.AddLineToPoint (new NFloat (x), new NFloat (y));
		}

		public static void ScaleCTM (this CGContext self, float sx, float sy)
		{
			self.ScaleCTM (new NFloat (sx), new NFloat (sy));
		}

		public static void SetStrokeColor (this CGContext self, float red, float green, float blue, float alpha)
		{
			self.SetStrokeColor (new NFloat (red), new NFloat (green), new NFloat (blue), new NFloat (alpha));
		}

		public static void SetStrokeColor (this CGContext self, float cyan, float magenta, float yellow, float black, float alpha)
		{
			self.SetStrokeColor (new NFloat (cyan), new NFloat (magenta), new NFloat (yellow), new NFloat (black), new NFloat (alpha));
		}

		public static void AddQuadCurveToPoint (this CGContext self, float cpx, float cpy, float x, float y)
		{
			self.AddQuadCurveToPoint (new NFloat (cpx), new NFloat (cpy), new NFloat (x), new NFloat (y));
		}
	}
}

namespace Foundation {
	internal static class NSString_NFloatExtensions {
#if !__TVOS__ && !__MACOS__
		public static CGSize DrawString (this NSString self, CGPoint point, float width, UIFont font, float minFontSize, ref nfloat actualFontSize, UILineBreakMode breakMode, UIBaselineAdjustment adjustment)
		{
			return self.DrawString (point, new NFloat (width), font, new NFloat (minFontSize), ref actualFontSize, breakMode, adjustment);
		}

		public static CGSize StringSize (this NSString self, UIFont font, float minFontSize, ref nfloat actualFontSize, float forWidth, UILineBreakMode lineBreakMode)
		{
			return self.StringSize (font, new NFloat (minFontSize), ref actualFontSize, new NFloat (forWidth), lineBreakMode);
		}

		public static CGSize StringSize (this NSString self, UIFont font, float forWidth, UILineBreakMode breakMode)
		{
			return self.StringSize (font, new NFloat (forWidth), breakMode);
		}

		public static CGSize DrawString (this NSString self, CGPoint point, float width, UIFont font, UILineBreakMode breakMode)
		{
			return self.DrawString (point, new NFloat (width), font, breakMode);
		}

		public static CGSize DrawString (this string self, CGPoint point, float width, UIFont font, UILineBreakMode breakMode)
		{
			return self.DrawString (point, new NFloat (width), font, breakMode);
		}
#endif
	}
}
