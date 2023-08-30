// 
// CGContext.cs: Implements the managed CGContext
//
// Authors: Mono Team
//     
// Copyright 2009 Novell, Inc
// Copyright 2011-2014 Xamarin Inc
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

#nullable enable

using System;
using System.Runtime.InteropServices;

using CoreFoundation;
using ObjCRuntime;
using Foundation;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace CoreGraphics {

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CGContext : NativeObject {
#if !COREBUILD
#if !NET
		public CGContext (NativeHandle handle)
			: base (handle, false)
		{
		}
#endif

		[Preserve (Conditional = true)]
		internal CGContext (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextRelease (/* CGContextRef */ IntPtr c);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGContextRef */ IntPtr CGContextRetain (/* CGContextRef */ IntPtr c);

		protected internal override void Retain ()
		{
			CGContextRetain (GetCheckedHandle ());
		}

		protected internal override void Release ()
		{
			CGContextRelease (GetCheckedHandle ());
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSaveGState (/* CGContextRef */ IntPtr c);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextRestoreGState (/* CGContextRef */ IntPtr c);

		public void SaveState ()
		{
			CGContextSaveGState (Handle);
		}

		public void RestoreState ()
		{
			CGContextRestoreGState (Handle);
		}

		//
		// Transformation matrix
		//

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextScaleCTM (/* CGContextRef */ IntPtr c, /* CGFloat */ nfloat sx, /* CGFloat */ nfloat sy);

		public void ScaleCTM (nfloat sx, nfloat sy)
		{
			CGContextScaleCTM (Handle, sx, sy);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextTranslateCTM (/* CGContextRef */ IntPtr c, /* CGFloat */ nfloat tx, /* CGFloat */ nfloat ty);

		public void TranslateCTM (nfloat tx, nfloat ty)
		{
			CGContextTranslateCTM (Handle, tx, ty);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextRotateCTM (/* CGContextRef */ IntPtr c, /* CGFloat */ nfloat angle);

		public void RotateCTM (nfloat angle)
		{
			CGContextRotateCTM (Handle, angle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextConcatCTM (/* CGContextRef */ IntPtr c, CGAffineTransform transform);

		public void ConcatCTM (CGAffineTransform transform)
		{
			CGContextConcatCTM (Handle, transform);
		}

		// Settings
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetLineWidth (/* CGContextRef */ IntPtr c, /* CGFloat */ nfloat width);

		public void SetLineWidth (nfloat w)
		{
			CGContextSetLineWidth (Handle, w);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetLineCap (/* CGContextRef */ IntPtr c, CGLineCap cap);

		public void SetLineCap (CGLineCap cap)
		{
			CGContextSetLineCap (Handle, cap);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetLineJoin (/* CGContextRef */  IntPtr c, CGLineJoin join);

		public void SetLineJoin (CGLineJoin join)
		{
			CGContextSetLineJoin (Handle, join);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetMiterLimit (/* CGContextRef */ IntPtr c, /* CGFloat */ nfloat limit);

		public void SetMiterLimit (nfloat limit)
		{
			CGContextSetMiterLimit (Handle, limit);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static unsafe void CGContextSetLineDash (/* CGContextRef */ IntPtr c, /* CGFloat */ nfloat phase, /* CGFloat[] */ nfloat* lengths, /* size_t */ nint count);

		public void SetLineDash (nfloat phase, nfloat []? lengths)
		{
			int n = lengths is null ? 0 : lengths.Length;
			unsafe {
				fixed (nfloat* lengthsPtr = lengths) {
					CGContextSetLineDash (Handle, phase, lengthsPtr, n);
				}
			}
		}

		public void SetLineDash (nfloat phase, nfloat []? lengths, int n)
		{
			if (lengths is null)
				n = 0;
			else if (n < 0 || n > lengths.Length)
				throw new ArgumentException (nameof (n));
			unsafe {
				fixed (nfloat* lengthsPtr = lengths) {
					CGContextSetLineDash (Handle, phase, lengthsPtr, n);
				}
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetFlatness (/* CGContextRef */ IntPtr c, /* CGFloat */ nfloat flatness);

		public void SetFlatness (nfloat flatness)
		{
			CGContextSetFlatness (Handle, flatness);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetAlpha (/* CGContextRef */ IntPtr c, /* CGFloat */ nfloat alpha);

		public void SetAlpha (nfloat alpha)
		{
			CGContextSetAlpha (Handle, alpha);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetBlendMode (/* CGContextRef */ IntPtr c, CGBlendMode mode);

		public void SetBlendMode (CGBlendMode mode)
		{
			CGContextSetBlendMode (Handle, mode);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGAffineTransform CGContextGetCTM (/* CGContextRef */ IntPtr c);

		public CGAffineTransform GetCTM ()
		{
			return CGContextGetCTM (Handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextBeginPath (/* CGContextRef */ IntPtr c);

		public void BeginPath ()
		{
			CGContextBeginPath (Handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextMoveToPoint (/* CGContextRef */ IntPtr c, /* CGFloat */ nfloat x, /* CGFloat */ nfloat y);

		public void MoveTo (nfloat x, nfloat y)
		{
			CGContextMoveToPoint (Handle, x, y);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextAddLineToPoint (/* CGContextRef */ IntPtr c, /* CGFloat */ nfloat x, /* CGFloat */ nfloat y);

		public void AddLineToPoint (nfloat x, nfloat y)
		{
			CGContextAddLineToPoint (Handle, x, y);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextAddCurveToPoint (/* CGContextRef */ IntPtr c, /* CGFloat */ nfloat cp1x, /* CGFloat */ nfloat cp1y, /* CGFloat */ nfloat cp2x, /* CGFloat */ nfloat cp2y, /* CGFloat */ nfloat x, /* CGFloat */ nfloat y);

		public void AddCurveToPoint (nfloat cp1x, nfloat cp1y, nfloat cp2x, nfloat cp2y, nfloat x, nfloat y)
		{
			CGContextAddCurveToPoint (Handle, cp1x, cp1y, cp2x, cp2y, x, y);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextAddQuadCurveToPoint (/* CGContextRef */ IntPtr c, /* CGFloat */ nfloat cpx, /* CGFloat */ nfloat cpy, /* CGFloat */ nfloat x, /* CGFloat */ nfloat y);

		public void AddQuadCurveToPoint (nfloat cpx, nfloat cpy, nfloat x, nfloat y)
		{
			CGContextAddQuadCurveToPoint (Handle, cpx, cpy, x, y);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextClosePath (/* CGContextRef */ IntPtr c);

		public void ClosePath ()
		{
			CGContextClosePath (Handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextAddRect (/* CGContextRef */ IntPtr c, CGRect rect);

		public void AddRect (CGRect rect)
		{
			CGContextAddRect (Handle, rect);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextAddRects (/* CGContextRef */ IntPtr c, CGRect [] rects, /* size_t */ nint count);

		public void AddRects (CGRect [] rects)
		{
			if (rects is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (rects));
			CGContextAddRects (Handle, rects, rects.Length);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextAddLines (/* CGContextRef */ IntPtr c, CGPoint [] points, /* size_t */ nint count);
		public void AddLines (CGPoint [] points)
		{
			if (points is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (points));
			CGContextAddLines (Handle, points, points.Length);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextAddEllipseInRect (/* CGContextRef */ IntPtr c, CGRect rect);

		public void AddEllipseInRect (CGRect rect)
		{
			CGContextAddEllipseInRect (Handle, rect);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextAddArc (/* CGContextRef */ IntPtr c, /* CGFloat */ nfloat x, /* CGFloat */ nfloat y, /* CGFloat */ nfloat radius, /* CGFloat */ nfloat startAngle, /* CGFloat */ nfloat endAngle, /* int */ int clockwise);

		public void AddArc (nfloat x, nfloat y, nfloat radius, nfloat startAngle, nfloat endAngle, bool clockwise)
		{
			CGContextAddArc (Handle, x, y, radius, startAngle, endAngle, clockwise ? 1 : 0);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextAddArcToPoint (/* CGContextRef */ IntPtr c, /* CGFloat */ nfloat x1, /* CGFloat */ nfloat y1, /* CGFloat */ nfloat x2, /* CGFloat */ nfloat y2, /* CGFloat */ nfloat radius);

		public void AddArcToPoint (nfloat x1, nfloat y1, nfloat x2, nfloat y2, nfloat radius)
		{
			CGContextAddArcToPoint (Handle, x1, y1, x2, y2, radius);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextAddPath (/* CGContextRef */ IntPtr c, /* CGPathRef */ IntPtr path);

		public void AddPath (CGPath path)
		{
			if (path is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (path));
			CGContextAddPath (Handle, path.Handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextReplacePathWithStrokedPath (/* CGContextRef */ IntPtr c);

		public void ReplacePathWithStrokedPath ()
		{
			CGContextReplacePathWithStrokedPath (Handle);
		}


		[DllImport (Constants.CoreGraphicsLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool CGContextIsPathEmpty (/* CGContextRef */ IntPtr context);

		public bool IsPathEmpty ()
		{
			return CGContextIsPathEmpty (Handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGPoint CGContextGetPathCurrentPoint (/* CGContextRef */ IntPtr context);

		public CGPoint GetPathCurrentPoint ()
		{
			return CGContextGetPathCurrentPoint (Handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGRect CGContextGetPathBoundingBox (/* CGContextRef */ IntPtr context);

		public CGRect GetPathBoundingBox ()
		{
			return CGContextGetPathBoundingBox (Handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool CGContextPathContainsPoint (/* CGContextRef */ IntPtr context, CGPoint point, CGPathDrawingMode mode);

		public bool PathContainsPoint (CGPoint point, CGPathDrawingMode mode)
		{
			return CGContextPathContainsPoint (Handle, point, mode);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextDrawPath (/* CGContextRef */ IntPtr context, CGPathDrawingMode mode);

		public void DrawPath (CGPathDrawingMode mode)
		{
			CGContextDrawPath (Handle, mode);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextFillPath (/* CGContextRef */ IntPtr c);

		public void FillPath ()
		{
			CGContextFillPath (Handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextEOFillPath (/* CGContextRef */ IntPtr c);

		public void EOFillPath ()
		{
			CGContextEOFillPath (Handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextStrokePath (/* CGContextRef */ IntPtr c);

		public void StrokePath ()
		{
			CGContextStrokePath (Handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextFillRect (/* CGContextRef */ IntPtr c, CGRect rect);

		public void FillRect (CGRect rect)
		{
			CGContextFillRect (Handle, rect);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextFillRects (/* CGContextRef */ IntPtr c, CGRect [] rects, /* size_t */ nint count);

		public void ContextFillRects (CGRect [] rects)
		{
			if (rects is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (rects));
			CGContextFillRects (Handle, rects, rects.Length);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextStrokeRect (/* CGContextRef */ IntPtr c, CGRect rect);

		public void StrokeRect (CGRect rect)
		{
			CGContextStrokeRect (Handle, rect);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextStrokeRectWithWidth (/* CGContextRef */ IntPtr c, CGRect rect, /* GCFloat */ nfloat width);

		public void StrokeRectWithWidth (CGRect rect, nfloat width)
		{
			CGContextStrokeRectWithWidth (Handle, rect, width);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextClearRect (/* CGContextRef */ IntPtr c, CGRect rect);

		public void ClearRect (CGRect rect)
		{
			CGContextClearRect (Handle, rect);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextFillEllipseInRect (/* CGContextRef */ IntPtr context, CGRect rect);

		public void FillEllipseInRect (CGRect rect)
		{
			CGContextFillEllipseInRect (Handle, rect);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextStrokeEllipseInRect (/* CGContextRef */ IntPtr context, CGRect rect);

		public void StrokeEllipseInRect (CGRect rect)
		{
			CGContextStrokeEllipseInRect (Handle, rect);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextStrokeLineSegments (/* CGContextRef __nullable */ IntPtr c,
			/* const CGPoint* __nullable */ CGPoint []? points,
			/* size_t */ nint count);

		public void StrokeLineSegments (CGPoint []? points)
		{
			CGContextStrokeLineSegments (Handle, points, points is null ? 0 : points.Length);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextClip (/* CGContextRef */ IntPtr c);

		public void Clip ()
		{
			CGContextClip (Handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextEOClip (/* CGContextRef */ IntPtr c);

		public void EOClip ()
		{
			CGContextEOClip (Handle);
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextResetClip (/* CGContextRef */ IntPtr c);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		public void ResetClip ()
		{
			CGContextResetClip (Handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextClipToMask (/* CGContextRef */ IntPtr c, CGRect rect,
			/* CGImageRef __nullable */ IntPtr mask);

		public void ClipToMask (CGRect rect, CGImage? mask)
		{
			CGContextClipToMask (Handle, rect, mask.GetHandle ());
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGRect CGContextGetClipBoundingBox (/* CGContextRef */ IntPtr c);

		public CGRect GetClipBoundingBox ()
		{
			return CGContextGetClipBoundingBox (Handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextClipToRect (/* CGContextRef */ IntPtr c, CGRect rect);

		public void ClipToRect (CGRect rect)
		{
			CGContextClipToRect (Handle, rect);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextClipToRects (/* CGContextRef */ IntPtr c, CGRect [] rects, /* size_t */ nint count);

		public void ClipToRects (CGRect [] rects)
		{
			if (rects is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (rects));
			CGContextClipToRects (Handle, rects, rects.Length);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetFillColorWithColor (/* CGContextRef */ IntPtr c,
			/* CGColorRef __nullable */ IntPtr color);

		public void SetFillColor (CGColor? color)
		{
			CGContextSetFillColorWithColor (Handle, color.GetHandle ());
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetStrokeColorWithColor (/* CGContextRef */ IntPtr c,
			/* CGColorRef __nullable */ IntPtr color);

		public void SetStrokeColor (CGColor? color)
		{
			CGContextSetStrokeColorWithColor (Handle, color.GetHandle ());
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetFillColorSpace (/* CGContextRef */ IntPtr context,
			/* CGColorSpaceRef __nullable */ IntPtr space);

		public void SetFillColorSpace (CGColorSpace? space)
		{
			CGContextSetFillColorSpace (Handle, space.GetHandle ());
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetStrokeColorSpace (/* CGContextRef */ IntPtr context,
			/* CGColorSpaceRef __nullable */ IntPtr space);

		public void SetStrokeColorSpace (CGColorSpace? space)
		{
			CGContextSetStrokeColorSpace (Handle, space.GetHandle ());
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static unsafe void CGContextSetFillColor (/* CGContextRef */ IntPtr context,
			/* const CGFloat * __nullable */ nfloat* components);

		public void SetFillColor (nfloat []? components)
		{
			unsafe {
				fixed (nfloat* componentsPtr = components) {
					CGContextSetFillColor (Handle, componentsPtr);
				}
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static unsafe void CGContextSetStrokeColor (/* CGContextRef */ IntPtr context,
			/* const CGFloat * __nullable */ nfloat* components);

		public void SetStrokeColor (nfloat []? components)
		{
			unsafe {
				fixed (nfloat* componentsPtr = components) {
					CGContextSetStrokeColor (Handle, componentsPtr);
				}
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static unsafe void CGContextSetFillPattern (/* CGContextRef */ IntPtr context,
			/* CGPatternRef __nullable */ IntPtr pattern, /* const CGFloat * __nullable */ nfloat* components);

		public void SetFillPattern (CGPattern pattern, nfloat []? components)
		{
			unsafe {
				fixed (nfloat* componentsPtr = components) {
					CGContextSetFillPattern (Handle, pattern.GetHandle (), componentsPtr);
				}
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static unsafe void CGContextSetStrokePattern (/* CGContextRef */ IntPtr context,
			/* CGPatternRef __nullable */ IntPtr pattern, /* const CGFloat * __nullable */ nfloat* components);

		public void SetStrokePattern (CGPattern? pattern, nfloat []? components)
		{
			unsafe {
				fixed (nfloat* componentsPtr = components) {
					CGContextSetStrokePattern (Handle, pattern.GetHandle (), componentsPtr);
				}
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetPatternPhase (/* CGContextRef */ IntPtr context, CGSize phase);

		public void SetPatternPhase (CGSize phase)
		{
			CGContextSetPatternPhase (Handle, phase);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetGrayFillColor (/* CGContextRef */ IntPtr context, /* CGFloat */ nfloat gray, /* CGFloat */ nfloat alpha);

		public void SetFillColor (nfloat gray, nfloat alpha)
		{
			CGContextSetGrayFillColor (Handle, gray, alpha);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetGrayStrokeColor (/* CGContextRef */ IntPtr context, /* CGFloat */ nfloat gray, /* CGFloat */ nfloat alpha);

		public void SetStrokeColor (nfloat gray, nfloat alpha)
		{
			CGContextSetGrayStrokeColor (Handle, gray, alpha);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetRGBFillColor (/* CGContextRef */ IntPtr context, /* CGFloat */ nfloat red, /* CGFloat */ nfloat green, /* CGFloat */ nfloat blue, /* CGFloat */ nfloat alpha);

		public void SetFillColor (nfloat red, nfloat green, nfloat blue, nfloat alpha)
		{
			CGContextSetRGBFillColor (Handle, red, green, blue, alpha);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetRGBStrokeColor (/* CGContextRef */ IntPtr context, /* CGFloat */ nfloat red, /* CGFloat */ nfloat green, /* CGFloat */ nfloat blue, /* CGFloat */ nfloat alpha);

		public void SetStrokeColor (nfloat red, nfloat green, nfloat blue, nfloat alpha)
		{
			CGContextSetRGBStrokeColor (Handle, red, green, blue, alpha);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetCMYKFillColor (/* CGContextRef */ IntPtr context, /* CGFloat */ nfloat cyan, /* CGFloat */ nfloat magenta, /* CGFloat */ nfloat yellow, /* CGFloat */ nfloat black, /* CGFloat */ nfloat alpha);

		public void SetFillColor (nfloat cyan, nfloat magenta, nfloat yellow, nfloat black, nfloat alpha)
		{
			CGContextSetCMYKFillColor (Handle, cyan, magenta, yellow, black, alpha);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetCMYKStrokeColor (/* CGContextRef */ IntPtr context, /* CGFloat */ nfloat cyan, /* CGFloat */ nfloat magenta, /* CGFloat */ nfloat yellow, /* CGFloat */ nfloat black, /* CGFloat */ nfloat alpha);

		public void SetStrokeColor (nfloat cyan, nfloat magenta, nfloat yellow, nfloat black, nfloat alpha)
		{
			CGContextSetCMYKStrokeColor (Handle, cyan, magenta, yellow, black, alpha);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetRenderingIntent (/* CGContextRef */ IntPtr context, CGColorRenderingIntent intent);

		public void SetRenderingIntent (CGColorRenderingIntent intent)
		{
			CGContextSetRenderingIntent (Handle, intent);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextDrawImage (/* CGContextRef */ IntPtr c, CGRect rect,
			/* CGImageRef __nullable */ IntPtr image);

		public void DrawImage (CGRect rect, CGImage? image)
		{
			CGContextDrawImage (Handle, rect, image.GetHandle ());
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextDrawTiledImage (/* CGContextRef */ IntPtr c, CGRect rect,
			/* CGImageRef __nullable */ IntPtr image);

		public void DrawTiledImage (CGRect rect, CGImage? image)
		{
			CGContextDrawTiledImage (Handle, rect, image.GetHandle ());
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGInterpolationQuality CGContextGetInterpolationQuality (/* CGContextRef */ IntPtr context);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetInterpolationQuality (/* CGContextRef */ IntPtr context, CGInterpolationQuality quality);

		public CGInterpolationQuality InterpolationQuality {
			get {
				return CGContextGetInterpolationQuality (Handle);
			}

			set {
				CGContextSetInterpolationQuality (Handle, value);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetShadowWithColor (/* CGContextRef __nullable */ IntPtr context, CGSize offset,
			/* CGFloat */ nfloat blur, /* CGColorRef __nullable */ IntPtr color);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetShadow (IntPtr context, CGSize offset, nfloat blur);

		public void SetShadow (CGSize offset, nfloat blur, CGColor? color = null)
		{
			if (color is null)
				CGContextSetShadow (Handle, offset, blur);
			else
				CGContextSetShadowWithColor (Handle, offset, blur, color.Handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextDrawLinearGradient (/* CGContextRef __nullable */ IntPtr context,
			/* CGGradientRef __nullable */ IntPtr gradient, CGPoint startPoint, CGPoint endPoint,
			CGGradientDrawingOptions options);

		public void DrawLinearGradient (CGGradient? gradient, CGPoint startPoint, CGPoint endPoint, CGGradientDrawingOptions options)
		{
			CGContextDrawLinearGradient (Handle, gradient.GetHandle (), startPoint, endPoint, options);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextDrawRadialGradient (/* CGContextRef __nullable */ IntPtr context,
			/* CGGradientRef __nullable */ IntPtr gradient,
			CGPoint startCenter, /* CGFloat */ nfloat startRadius,
			CGPoint endCenter, /* CGFloat */ nfloat endRadius, CGGradientDrawingOptions options);

		public void DrawRadialGradient (CGGradient? gradient, CGPoint startCenter, nfloat startRadius, CGPoint endCenter, nfloat endRadius, CGGradientDrawingOptions options)
		{
			CGContextDrawRadialGradient (Handle, gradient.GetHandle (), startCenter, startRadius, endCenter, endRadius, options);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextDrawShading (/* CGContextRef */ IntPtr context,
			/* CGShadingRef __nullable */ IntPtr shading);

		public void DrawShading (CGShading? shading)
		{
			CGContextDrawShading (Handle, shading.GetHandle ());
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetCharacterSpacing (/* CGContextRef */ IntPtr context, /* GCFloat */ nfloat spacing);

		public void SetCharacterSpacing (nfloat spacing)
		{
			CGContextSetCharacterSpacing (Handle, spacing);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetTextPosition (/* CGContextRef */ IntPtr c, /* GCFloat */ nfloat x, /* GCFloat */ nfloat y);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGPoint CGContextGetTextPosition (/* CGContextRef */ IntPtr context);

		public CGPoint TextPosition {
			get {
				return CGContextGetTextPosition (Handle);
			}
			set {
				CGContextSetTextPosition (Handle, value.X, value.Y);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetTextMatrix (/* CGContextRef */ IntPtr c, CGAffineTransform t);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGAffineTransform CGContextGetTextMatrix (/* CGContextRef */ IntPtr c);

		public CGAffineTransform TextMatrix {
			get {
				return CGContextGetTextMatrix (Handle);
			}
			set {
				CGContextSetTextMatrix (Handle, value);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetTextDrawingMode (/* CGContextRef */ IntPtr c, CGTextDrawingMode mode);

		public void SetTextDrawingMode (CGTextDrawingMode mode)
		{
			CGContextSetTextDrawingMode (Handle, mode);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetFont (/* CGContextRef */ IntPtr c, /* CGFontRef __nullable */ IntPtr font);

		public void SetFont (CGFont? font)
		{
			CGContextSetFont (Handle, font.GetHandle ());
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetFontSize (/* CGContextRef */ IntPtr c, /* CGFloat */ nfloat size);

		public void SetFontSize (nfloat size)
		{
			CGContextSetFontSize (Handle, size);
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos10.9")]
		[ObsoletedOSPlatform ("ios7.0")]
#else
		[Deprecated (PlatformName.iOS, 7, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 9)]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSelectFont (/* CGContextRef */ IntPtr c,
			/* const char* __nullable */ IntPtr name, /* CGFloat */ nfloat size, CGTextEncoding textEncoding);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos10.9", "Use the 'CoreText' API instead.")]
		[ObsoletedOSPlatform ("ios7.0", "Use the 'CoreText' API instead.")]
#else
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use the 'CoreText' API instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 9, message: "Use the 'CoreText' API instead.")]
#endif
		public void SelectFont (string? name, nfloat size, CGTextEncoding textEncoding)
		{
			using var namePtr = new TransientString (name);
			CGContextSelectFont (Handle, namePtr, size, textEncoding);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextShowGlyphsAtPositions (/* CGContextRef __nullable */ IntPtr context,
			/* const CGGlyph * __nullable */ ushort []? glyphs,
			/* const CGPoint * __nullable */ CGPoint []? positions, /* size_t */ nint count);

		public void ShowGlyphsAtPositions (ushort []? glyphs, CGPoint []? positions, int count = -1)
		{
			if (glyphs is null)
				count = 0;
			else if (count < 0)
				count = glyphs.Length;
			CGContextShowGlyphsAtPositions (Handle, glyphs, positions, count);
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos10.9")]
		[ObsoletedOSPlatform ("ios7.0")]
#else
		[Deprecated (PlatformName.iOS, 7, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 9)]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextShowText (/* CGContextRef */ IntPtr c, /* const char* __nullable */ IntPtr s, /* size_t */ nint length);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos10.9", "Use the 'CoreText' API instead.")]
		[ObsoletedOSPlatform ("ios7.0", "Use the 'CoreText' API instead.")]
#else
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use the 'CoreText' API instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 9, message: "Use the 'CoreText' API instead.")]
#endif
		public void ShowText (string? str, int count)
		{
			if (str is null)
				count = 0;
			else if (count > str.Length)
				throw new ArgumentException (nameof (count));
			using var strPtr = new TransientString (str);
			CGContextShowText (Handle, strPtr, count);
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos10.9", "Use the 'CoreText' API instead.")]
		[ObsoletedOSPlatform ("ios7.0", "Use the 'CoreText' API instead.")]
#else
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use the 'CoreText' API instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 9, message: "Use the 'CoreText' API instead.")]
#endif
		public void ShowText (string? str)
		{
			using var strPtr = new TransientString (str);
			CGContextShowText (Handle, strPtr, str is null ? 0 : str.Length);
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos10.9")]
		[ObsoletedOSPlatform ("ios7.0")]
#else
		[Deprecated (PlatformName.iOS, 7, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 9)]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextShowText (/* CGContextRef */ IntPtr c, /* const char* __nullable */ byte []? bytes, /* size_t */ nint length);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos10.9", "Use the 'CoreText' API instead.")]
		[ObsoletedOSPlatform ("ios7.0", "Use the 'CoreText' API instead.")]
#else
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use the 'CoreText' API instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 9, message: "Use the 'CoreText' API instead.")]
#endif
		public void ShowText (byte []? bytes, int count)
		{
			if (bytes is null)
				count = 0;
			else if (count > bytes.Length)
				throw new ArgumentException (nameof (count));
			CGContextShowText (Handle, bytes, count);
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos10.9", "Use the 'CoreText' API instead.")]
		[ObsoletedOSPlatform ("ios7.0", "Use the 'CoreText' API instead.")]
#else
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use the 'CoreText' API instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 9, message: "Use the 'CoreText' API instead.")]
#endif
		public void ShowText (byte []? bytes)
		{
			CGContextShowText (Handle, bytes, bytes is null ? 0 : bytes.Length);
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos10.9")]
		[ObsoletedOSPlatform ("ios7.0")]
#else
		[Deprecated (PlatformName.iOS, 7, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 9)]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextShowTextAtPoint (/* CGContextRef __nullable */ IntPtr c, /* CGFloat */ nfloat x,
			/* CGFloat */ nfloat y, /* const char* __nullable */ IntPtr str, /* size_t */ nint length);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos10.9", "Use the 'CoreText' API instead.")]
		[ObsoletedOSPlatform ("ios7.0", "Use the 'CoreText' API instead.")]
#else
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use the 'CoreText' API instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 9, message: "Use the 'CoreText' API instead.")]
#endif
		public void ShowTextAtPoint (nfloat x, nfloat y, string? str, int length)
		{
			using var strPtr = new TransientString (str);
			CGContextShowTextAtPoint (Handle, x, y, strPtr, length);
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos10.9", "Use the 'CoreText' API instead.")]
		[ObsoletedOSPlatform ("ios7.0", "Use the 'CoreText' API instead.")]
#else
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use the 'CoreText' API instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 9, message: "Use the 'CoreText' API instead.")]
#endif
		public void ShowTextAtPoint (nfloat x, nfloat y, string? str)
		{
			using var strPtr = new TransientString (str);
			CGContextShowTextAtPoint (Handle, x, y, strPtr, str is null ? 0 : str.Length);
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos10.9")]
		[ObsoletedOSPlatform ("ios7.0")]
#else
		[Deprecated (PlatformName.iOS, 7, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 9)]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextShowTextAtPoint (/* CGContextRef */ IntPtr c, /* CGFloat */ nfloat x, /* CGFloat */ nfloat y, /* const char* */ byte []? bytes, /* size_t */ nint length);

		public void ShowTextAtPoint (nfloat x, nfloat y, byte []? bytes, int length)
		{
			CGContextShowTextAtPoint (Handle, x, y, bytes, length);
		}

		public void ShowTextAtPoint (nfloat x, nfloat y, byte []? bytes)
		{
			CGContextShowTextAtPoint (Handle, x, y, bytes, bytes is null ? 0 : bytes.Length);
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos10.9")]
		[ObsoletedOSPlatform ("ios7.0")]
#else
		[Deprecated (PlatformName.iOS, 7, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 9)]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextShowGlyphs (/* CGContextRef __nullable */ IntPtr c,
			/* const CGGlyph * __nullable */ ushort []? glyphs, /* size_t */ nint count);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos10.9", "Use the 'CoreText' API instead.")]
		[ObsoletedOSPlatform ("ios7.0", "Use the 'CoreText' API instead.")]
#else
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use the 'CoreText' API instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 9, message: "Use the 'CoreText' API instead.")]
#endif
		public void ShowGlyphs (ushort []? glyphs)
		{
			CGContextShowGlyphs (Handle, glyphs, glyphs is null ? 0 : glyphs.Length);
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos10.9", "Use the 'CoreText' API instead.")]
		[ObsoletedOSPlatform ("ios7.0", "Use the 'CoreText' API instead.")]
#else
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use the 'CoreText' API instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 9, message: "Use the 'CoreText' API instead.")]
#endif
		public void ShowGlyphs (ushort []? glyphs, int count)
		{
			if (glyphs is null)
				count = 0;
			else if (count > glyphs.Length)
				throw new ArgumentException (nameof (count));
			CGContextShowGlyphs (Handle, glyphs, count);
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos10.9")]
		[ObsoletedOSPlatform ("ios7.0")]
#else
		[Deprecated (PlatformName.iOS, 7, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 9)]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextShowGlyphsAtPoint (/* CGContextRef */ IntPtr context, /* CGFloat */ nfloat x,
			/* CGFloat */ nfloat y, /* const CGGlyph * __nullable */ ushort []? glyphs, /* size_t */ nint count);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos10.9", "Use the 'CoreText' API instead.")]
		[ObsoletedOSPlatform ("ios7.0", "Use the 'CoreText' API instead.")]
#else
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use the 'CoreText' API instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 9, message: "Use the 'CoreText' API instead.")]
#endif
		public void ShowGlyphsAtPoint (nfloat x, nfloat y, ushort []? glyphs, int count)
		{
			if (glyphs is null)
				count = 0;
			else if (count > glyphs.Length)
				throw new ArgumentException (nameof (count));
			CGContextShowGlyphsAtPoint (Handle, x, y, glyphs, count);
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos10.9", "Use the 'CoreText' API instead.")]
		[ObsoletedOSPlatform ("ios7.0", "Use the 'CoreText' API instead.")]
#else
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use the 'CoreText' API instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 9, message: "Use the 'CoreText' API instead.")]
#endif
		public void ShowGlyphsAtPoint (nfloat x, nfloat y, ushort []? glyphs)
		{
			CGContextShowGlyphsAtPoint (Handle, x, y, glyphs, glyphs is null ? 0 : glyphs.Length);
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos10.9")]
		[ObsoletedOSPlatform ("ios7.0")]
#else
		[Deprecated (PlatformName.iOS, 7, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 9)]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextShowGlyphsWithAdvances (/* CGContextRef __nullable */ IntPtr c,
			/* const CGGlyph * __nullable */ ushort []? glyphs,
			/* const CGSize * __nullable */ CGSize []? advances, /* size_t */ nint count);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos10.9", "Use the 'CoreText' API instead.")]
		[ObsoletedOSPlatform ("ios7.0", "Use the 'CoreText' API instead.")]
#else
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use the 'CoreText' API instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 9, message: "Use the 'CoreText' API instead.")]
#endif
		public void ShowGlyphsWithAdvances (ushort []? glyphs, CGSize []? advances, int count)
		{
			if (glyphs is null)
				count = 0;
			if (count > (glyphs?.Length ?? 0) || count > (advances?.Length ?? 0))
				throw new ArgumentException (nameof (count));
			CGContextShowGlyphsWithAdvances (Handle, glyphs, advances, count);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextDrawPDFPage (/* CGContextRef __nullable */ IntPtr c,
			/* CGPDFPageRef __nullable */ IntPtr page);

		public void DrawPDFPage (CGPDFPage? page)
		{
			CGContextDrawPDFPage (Handle, page.GetHandle ());
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static void CGContextBeginPage (/* CGContextRef __nullable */ IntPtr c,
			/* const CGRect * __nullable */ CGRect* mediaBox);

		public unsafe void BeginPage (CGRect? rect)
		{
			if (rect.HasValue) {
				CGRect v = rect.Value;
				CGContextBeginPage (Handle, &v);
			} else {
				CGContextBeginPage (Handle, null);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextEndPage (/* CGContextRef __nullable */ IntPtr c);

		public void EndPage ()
		{
			CGContextEndPage (Handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextFlush (/* CGContextRef __nullable */ IntPtr c);

		public void Flush ()
		{
			CGContextFlush (Handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSynchronize (/* CGContextRef __nullable */ IntPtr c);

		public void Synchronize ()
		{
			CGContextSynchronize (Handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetShouldAntialias (/* CGContextRef */ IntPtr context, [MarshalAs (UnmanagedType.I1)] bool shouldAntialias);

		public void SetShouldAntialias (bool shouldAntialias)
		{
			CGContextSetShouldAntialias (Handle, shouldAntialias);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetAllowsAntialiasing (/* CGContextRef */ IntPtr context, [MarshalAs (UnmanagedType.I1)] bool allowsAntialiasing);
		public void SetAllowsAntialiasing (bool allowsAntialiasing)
		{
			CGContextSetAllowsAntialiasing (Handle, allowsAntialiasing);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetShouldSmoothFonts (/* CGContextRef */ IntPtr context, [MarshalAs (UnmanagedType.I1)] bool shouldSmoothFonts);

		public void SetShouldSmoothFonts (bool shouldSmoothFonts)
		{
			CGContextSetShouldSmoothFonts (Handle, shouldSmoothFonts);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGAffineTransform CGContextGetUserSpaceToDeviceSpaceTransform (/* CGContextRef */ IntPtr context);

		public CGAffineTransform GetUserSpaceToDeviceSpaceTransform ()
		{
			return CGContextGetUserSpaceToDeviceSpaceTransform (Handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGPoint CGContextConvertPointToDeviceSpace (/* CGContextRef */ IntPtr context, CGPoint point);

		public CGPoint PointToDeviceSpace (CGPoint point)
		{
			return CGContextConvertPointToDeviceSpace (Handle, point);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGPoint CGContextConvertPointToUserSpace (/* CGContextRef */ IntPtr context, CGPoint point);

		public CGPoint ConvertPointToUserSpace (CGPoint point)
		{
			return CGContextConvertPointToUserSpace (Handle, point);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGSize CGContextConvertSizeToDeviceSpace (/* CGContextRef */ IntPtr context, CGSize size);

		public CGSize ConvertSizeToDeviceSpace (CGSize size)
		{
			return CGContextConvertSizeToDeviceSpace (Handle, size);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGSize CGContextConvertSizeToUserSpace (/* CGContextRef */ IntPtr context, CGSize size);

		public CGSize ConvertSizeToUserSpace (CGSize size)
		{
			return CGContextConvertSizeToUserSpace (Handle, size);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGRect CGContextConvertRectToDeviceSpace (/* CGContextRef */ IntPtr context, CGRect rect);

		public CGRect ConvertRectToDeviceSpace (CGRect rect)
		{
			return CGContextConvertRectToDeviceSpace (Handle, rect);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGRect CGContextConvertRectToUserSpace (/* CGContextRef */ IntPtr context, CGRect rect);

		public CGRect ConvertRectToUserSpace (CGRect rect)
		{
			return CGContextConvertRectToUserSpace (Handle, rect);
		}

		// CGLayer.h
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextDrawLayerInRect (/* CGContextRef */ IntPtr context, CGRect rect, /* CGLayerRef */ IntPtr layer);

		public void DrawLayer (CGLayer layer, CGRect rect)
		{
			if (layer is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (layer));
			CGContextDrawLayerInRect (Handle, rect, layer.Handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextDrawLayerAtPoint (/* CGContextRef */ IntPtr context, CGPoint rect, /* CGLayerRef */ IntPtr layer);

		public void DrawLayer (CGLayer layer, CGPoint point)
		{
			if (layer is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (layer));
			CGContextDrawLayerAtPoint (Handle, point, layer.Handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGPathRef */ IntPtr CGContextCopyPath (/* CGContextRef */ IntPtr context);

		public CGPath CopyPath ()
		{
			var r = CGContextCopyPath (Handle);
			return new CGPath (r, true);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetAllowsFontSmoothing (/* CGContextRef */ IntPtr context, [MarshalAs (UnmanagedType.I1)] bool shouldSubpixelPositionFonts);

		public void SetAllowsFontSmoothing (bool allows)
		{
			CGContextSetAllowsFontSmoothing (Handle, allows);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetAllowsFontSubpixelPositioning (/* CGContextRef */ IntPtr context, [MarshalAs (UnmanagedType.I1)] bool allowsFontSubpixelPositioning);

		public void SetAllowsSubpixelPositioning (bool allows)
		{
			CGContextSetAllowsFontSubpixelPositioning (Handle, allows);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetAllowsFontSubpixelQuantization (/* CGContextRef */ IntPtr context, [MarshalAs (UnmanagedType.I1)] bool shouldSubpixelQuantizeFonts);

		public void SetAllowsFontSubpixelQuantization (bool allows)
		{
			CGContextSetAllowsFontSubpixelQuantization (Handle, allows);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetShouldSubpixelPositionFonts (/* CGContextRef */ IntPtr context, [MarshalAs (UnmanagedType.I1)] bool shouldSubpixelPositionFonts);

		public void SetShouldSubpixelPositionFonts (bool shouldSubpixelPositionFonts)
		{
			CGContextSetShouldSubpixelPositionFonts (Handle, shouldSubpixelPositionFonts);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetShouldSubpixelQuantizeFonts (/* CGContextRef */ IntPtr context, [MarshalAs (UnmanagedType.I1)] bool shouldSubpixelQuantizeFonts);

		public void ShouldSubpixelQuantizeFonts (bool shouldSubpixelQuantizeFonts)
		{
			CGContextSetShouldSubpixelQuantizeFonts (Handle, shouldSubpixelQuantizeFonts);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextBeginTransparencyLayer (/* CGContextRef */ IntPtr context, /* CFDictionaryRef __nullable */ IntPtr auxiliaryInfo);

		public void BeginTransparencyLayer (NSDictionary? auxiliaryInfo = null)
		{
			CGContextBeginTransparencyLayer (Handle, auxiliaryInfo.GetHandle ());
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextBeginTransparencyLayerWithRect (/* CGContextRef */ IntPtr context, CGRect rect, /* CFDictionaryRef __nullable */ IntPtr auxiliaryInfo);

		public void BeginTransparencyLayer (CGRect rectangle, NSDictionary? auxiliaryInfo = null)
		{
			CGContextBeginTransparencyLayerWithRect (Handle, rectangle, auxiliaryInfo.GetHandle ());
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextEndTransparencyLayer (/* CGContextRef */ IntPtr context);

		public void EndTransparencyLayer ()
		{
			CGContextEndTransparencyLayer (Handle);
		}

		public CGBitmapContext AsBitmapContext ()
		{
			return new CGBitmapContext (Handle, false);
		}

#if NET
		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("maccatalyst17.0")]
		[SupportedOSPlatform ("macos14.0")]
		[SupportedOSPlatform ("tvos17.0")]
#else
		[Mac (14, 0), iOS (17, 0), TV (17, 0), MacCatalyst (17, 0), Watch (10, 0)]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern void CGContextDrawConicGradient (/* CGContext */ IntPtr context, /*[NullAllowed] CGGradient*/ IntPtr gradient, CGPoint center, nfloat angle);

		public void DrawConicGradient (CGGradient? gradient, CGPoint point, nfloat angle) =>
			CGContextDrawConicGradient (Handle, gradient.GetHandle (), point, angle);

#endif // !COREBUILD
	}
}
