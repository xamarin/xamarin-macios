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

using System;
using System.Runtime.InteropServices;

using ObjCRuntime;
using Foundation;

namespace CoreGraphics {

	// untyped enum -> CGPath.h
	public enum CGLineJoin {
		Miter,
		Round,
		Bevel
	}
	
	// untyped enum -> CGPath.h
	public enum CGLineCap {
		Butt,
		Round,
		Square
	}
	
	// untyped enum -> CGContext.h
	public enum CGPathDrawingMode {
		Fill,
		EOFill,
		Stroke,
		FillStroke,
		EOFillStroke
	}
	
	// untyped enum -> CGContext.h
	public enum CGTextDrawingMode : uint {
		Fill,
		Stroke,
		FillStroke,
		Invisible,
		FillClip,
		StrokeClip,
		FillStrokeClip,
		Clip
	}

	// untyped enum -> CGContext.h
	[Deprecated (PlatformName.iOS, 7, 0)]
	[Deprecated (PlatformName.MacOSX, 10, 9)]
	public enum CGTextEncoding {
		FontSpecific,
		MacRoman
	}

	// untyped enum -> CGContext.h
	public enum CGInterpolationQuality {
		Default,
		None,	
		Low,	
		High,
		Medium		       /* Yes, in this order, since Medium was added in 4 */
	}
	
	// untyped enum -> CGContext.h
	public enum CGBlendMode {
		Normal,
		Multiply,
		Screen,
		Overlay,
		Darken,
		Lighten,
		ColorDodge,
		ColorBurn,
		SoftLight,
		HardLight,
		Difference,
		Exclusion,
		Hue,
		Saturation,
		Color,
		Luminosity,
		
		Clear,
		Copy,	
		SourceIn,	
		SourceOut,	
		SourceAtop,		
		DestinationOver,	
		DestinationIn,	
		DestinationOut,	
		DestinationAtop,	
		XOR,		
		PlusDarker,		
		PlusLighter		
	}

	public class CGContext : INativeObject
#if !COREBUILD
		, IDisposable
#endif
	{
#if !COREBUILD
		IntPtr handle;

		public CGContext (IntPtr handle)
			: this (handle, false)
		{
		}

		internal CGContext ()
		{
		}
		
		[Preserve (Conditional=true)]
		internal CGContext (IntPtr handle, bool owns)
		{
			Handle = handle;
			if (!owns)
				CGContextRetain (handle);
		}

		~CGContext ()
		{
			Dispose (false);
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		public IntPtr Handle {
			get { return handle; }
			internal set {
				if (value == IntPtr.Zero)
					throw new Exception ("Invalid parameters to context creation");
				handle = value;
			}
		}
	
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextRelease (/* CGContextRef */ IntPtr c);
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGContextRef */ IntPtr CGContextRetain (/* CGContextRef */ IntPtr c);
		
		protected virtual void Dispose (bool disposing)
		{
			if (handle != IntPtr.Zero){
				CGContextRelease (handle);
				handle = IntPtr.Zero;
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSaveGState (/* CGContextRef */ IntPtr c);
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextRestoreGState (/* CGContextRef */ IntPtr c);
		
		public void SaveState ()
		{
			CGContextSaveGState (handle);
		}

		public void RestoreState ()
		{
			CGContextRestoreGState (handle);
		}

		//
		// Transformation matrix
		//

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextScaleCTM (/* CGContextRef */ IntPtr c, /* CGFloat */ nfloat sx, /* CGFloat */ nfloat sy);

		public void ScaleCTM (nfloat sx, nfloat sy)
		{
			CGContextScaleCTM (handle, sx, sy);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextTranslateCTM (/* CGContextRef */ IntPtr c, /* CGFloat */ nfloat tx, /* CGFloat */ nfloat ty);

		public void TranslateCTM (nfloat tx, nfloat ty)
		{
			CGContextTranslateCTM (handle, tx, ty);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextRotateCTM (/* CGContextRef */ IntPtr c, /* CGFloat */ nfloat angle);

		public void RotateCTM (nfloat angle)
		{
			CGContextRotateCTM (handle, angle);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextConcatCTM (/* CGContextRef */ IntPtr c, CGAffineTransform transform);

		public void ConcatCTM (CGAffineTransform transform)
		{
			CGContextConcatCTM (handle, transform);
		}

		// Settings
		[DllImport (Constants.CoreGraphicsLibrary)]		
		extern static void CGContextSetLineWidth (/* CGContextRef */ IntPtr c, /* CGFloat */ nfloat width);

		public void SetLineWidth (nfloat w)
		{
			CGContextSetLineWidth (handle, w);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetLineCap (/* CGContextRef */ IntPtr c, CGLineCap cap);

		public void SetLineCap (CGLineCap cap)
		{
			CGContextSetLineCap (handle, cap);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetLineJoin (/* CGContextRef */  IntPtr c, CGLineJoin join);

		public void SetLineJoin (CGLineJoin join)
		{
			CGContextSetLineJoin (handle, join);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetMiterLimit (/* CGContextRef */ IntPtr c, /* CGFloat */ nfloat limit);

		public void SetMiterLimit (nfloat limit)
		{
			CGContextSetMiterLimit (handle, limit);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetLineDash (/* CGContextRef */ IntPtr c, /* CGFloat */ nfloat phase, /* CGFloat[] */ nfloat [] lengths, /* size_t */ nint count);

		public void SetLineDash (nfloat phase, nfloat [] lengths)
		{
			int n = lengths == null ? 0 : lengths.Length;
			CGContextSetLineDash (handle, phase, lengths, n);
		}

		public void SetLineDash (nfloat phase, nfloat [] lengths, int n)
		{
			if (lengths == null)
				n = 0;
			else if (n < 0 || n > lengths.Length)
				throw new ArgumentException ("n");
			CGContextSetLineDash (handle, phase, lengths, n);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetFlatness (/* CGContextRef */ IntPtr c, /* CGFloat */ nfloat flatness);

		public void SetFlatness (nfloat flatness)
		{
			CGContextSetFlatness (handle, flatness);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetAlpha (/* CGContextRef */ IntPtr c, /* CGFloat */ nfloat alpha);

		public void SetAlpha (nfloat alpha)
		{
			CGContextSetAlpha (handle, alpha);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetBlendMode (/* CGContextRef */ IntPtr c, CGBlendMode mode);

		public void SetBlendMode (CGBlendMode mode)
		{
			CGContextSetBlendMode (handle, mode);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGAffineTransform CGContextGetCTM (/* CGContextRef */ IntPtr c);

		public CGAffineTransform GetCTM ()
		{
			return CGContextGetCTM (handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextBeginPath (/* CGContextRef */ IntPtr c);

		public void BeginPath ()
		{
			CGContextBeginPath (handle);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextMoveToPoint (/* CGContextRef */ IntPtr c, /* CGFloat */ nfloat x, /* CGFloat */ nfloat y);

		public void MoveTo (nfloat x, nfloat y)
		{
			CGContextMoveToPoint (handle, x, y);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextAddLineToPoint (/* CGContextRef */ IntPtr c, /* CGFloat */ nfloat x, /* CGFloat */ nfloat y);

		public void AddLineToPoint (nfloat x, nfloat y)
		{
			CGContextAddLineToPoint (handle, x, y);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextAddCurveToPoint (/* CGContextRef */ IntPtr c, /* CGFloat */ nfloat cp1x, /* CGFloat */ nfloat cp1y, /* CGFloat */ nfloat cp2x, /* CGFloat */ nfloat cp2y, /* CGFloat */ nfloat x, /* CGFloat */ nfloat y);

		public void AddCurveToPoint (nfloat cp1x, nfloat cp1y, nfloat cp2x, nfloat cp2y, nfloat x, nfloat y)
		{
			CGContextAddCurveToPoint (handle, cp1x, cp1y, cp2x, cp2y, x, y);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextAddQuadCurveToPoint (/* CGContextRef */ IntPtr c, /* CGFloat */ nfloat cpx, /* CGFloat */ nfloat cpy, /* CGFloat */ nfloat x, /* CGFloat */ nfloat y);

		public void AddQuadCurveToPoint (nfloat cpx, nfloat cpy, nfloat x, nfloat y)
		{
			CGContextAddQuadCurveToPoint (handle, cpx, cpy, x, y);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextClosePath (/* CGContextRef */ IntPtr c);

		public void ClosePath ()
		{
			CGContextClosePath (handle);
		}
			
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextAddRect (/* CGContextRef */ IntPtr c, CGRect rect);

		public void AddRect (CGRect rect)
		{
			CGContextAddRect (handle, rect);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextAddRects (/* CGContextRef */ IntPtr c, CGRect [] rects, /* size_t */ nint count);

		public void AddRects (CGRect [] rects)
		{
			if (rects == null)
				throw new ArgumentNullException ("rects");
			CGContextAddRects (handle, rects, rects.Length);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextAddLines (/* CGContextRef */ IntPtr c, CGPoint [] points, /* size_t */ nint count);
		public void AddLines (CGPoint [] points)
		{
			if (points == null)
				throw new ArgumentNullException ("points");
			CGContextAddLines (handle, points, points.Length);
		}
			
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextAddEllipseInRect (/* CGContextRef */ IntPtr c, CGRect rect);

		public void AddEllipseInRect (CGRect rect)
		{
			CGContextAddEllipseInRect (handle, rect);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextAddArc (/* CGContextRef */ IntPtr c, /* CGFloat */ nfloat x, /* CGFloat */ nfloat y, /* CGFloat */ nfloat radius, /* CGFloat */ nfloat startAngle, /* CGFloat */ nfloat endAngle, /* int */ int clockwise);

		public void AddArc (nfloat x, nfloat y, nfloat radius, nfloat startAngle, nfloat endAngle, bool clockwise)
		{
			CGContextAddArc (handle, x, y, radius, startAngle, endAngle, clockwise ? 1 : 0);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextAddArcToPoint (/* CGContextRef */ IntPtr c, /* CGFloat */ nfloat x1, /* CGFloat */ nfloat y1, /* CGFloat */ nfloat x2, /* CGFloat */ nfloat y2, /* CGFloat */ nfloat radius);

		public void AddArcToPoint (nfloat x1, nfloat y1, nfloat x2, nfloat y2, nfloat radius)
		{
			CGContextAddArcToPoint (handle, x1, y1, x2, y2, radius);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextAddPath (/* CGContextRef */ IntPtr c, /* CGPathRef */ IntPtr path);

		public void AddPath (CGPath path)
		{
			if (path == null)
				throw new ArgumentNullException ("path");
			CGContextAddPath (handle, path.handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextReplacePathWithStrokedPath (/* CGContextRef */ IntPtr c);

		public void ReplacePathWithStrokedPath ()
		{
			CGContextReplacePathWithStrokedPath (handle);
		}

		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static bool CGContextIsPathEmpty (/* CGContextRef */ IntPtr context);

		public bool IsPathEmpty ()
		{
			return CGContextIsPathEmpty (handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGPoint CGContextGetPathCurrentPoint (/* CGContextRef */ IntPtr context);

		public CGPoint GetPathCurrentPoint ()
		{
			return CGContextGetPathCurrentPoint (handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGRect CGContextGetPathBoundingBox (/* CGContextRef */ IntPtr context);

		public CGRect GetPathBoundingBox ()
		{
			return CGContextGetPathBoundingBox (handle);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static bool CGContextPathContainsPoint (/* CGContextRef */ IntPtr context, CGPoint point, CGPathDrawingMode mode);

		public bool PathContainsPoint (CGPoint point, CGPathDrawingMode mode)
		{
			return CGContextPathContainsPoint (handle, point, mode);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextDrawPath (/* CGContextRef */ IntPtr context, CGPathDrawingMode mode);

		public void DrawPath (CGPathDrawingMode mode)
		{
			CGContextDrawPath (handle, mode);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextFillPath (/* CGContextRef */ IntPtr c);

		public void FillPath ()
		{
			CGContextFillPath (handle);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextEOFillPath (/* CGContextRef */ IntPtr c);

		public void EOFillPath ()
		{
			CGContextEOFillPath (handle);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextStrokePath (/* CGContextRef */ IntPtr c);

		public void StrokePath ()
		{
			CGContextStrokePath (handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextFillRect (/* CGContextRef */ IntPtr c, CGRect rect);

		public void FillRect (CGRect rect)
		{
			CGContextFillRect (handle, rect);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextFillRects (/* CGContextRef */ IntPtr c, CGRect [] rects, /* size_t */ nint count);

		public void ContextFillRects (CGRect [] rects)
		{
			if (rects == null)
				throw new ArgumentNullException ("rects");
			CGContextFillRects (handle, rects, rects.Length);
		}
			
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextStrokeRect (/* CGContextRef */ IntPtr c, CGRect rect);

		public void StrokeRect (CGRect rect)
		{
			CGContextStrokeRect (handle, rect);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextStrokeRectWithWidth (/* CGContextRef */ IntPtr c, CGRect rect, /* GCFloat */ nfloat width);

		public void StrokeRectWithWidth (CGRect rect, nfloat width)
		{
			CGContextStrokeRectWithWidth (handle, rect, width);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextClearRect (/* CGContextRef */ IntPtr c, CGRect rect);

		public void ClearRect (CGRect rect)
		{
			CGContextClearRect (handle, rect);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextFillEllipseInRect (/* CGContextRef */ IntPtr context, CGRect rect);

		public void FillEllipseInRect (CGRect rect)
		{
			CGContextFillEllipseInRect (handle, rect);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextStrokeEllipseInRect (/* CGContextRef */ IntPtr context, CGRect rect);

		public void StrokeEllipseInRect (CGRect rect)
		{
			CGContextStrokeEllipseInRect (handle, rect);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextStrokeLineSegments (/* CGContextRef __nullable */ IntPtr c, 
			/* const CGPoint* __nullable */ CGPoint [] points,
			/* size_t */ nint count);

		public void StrokeLineSegments (CGPoint [] points)
		{
			CGContextStrokeLineSegments (handle, points, points == null ? 0 : points.Length);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextClip (/* CGContextRef */ IntPtr c);

		public void Clip ()
		{
			CGContextClip (handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextEOClip (/* CGContextRef */ IntPtr c);

		public void EOClip ()
		{
			CGContextEOClip (handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		[iOS (11,0), Mac(10,13), TV(11,0), Watch(4,0)]
		extern static void CGContextResetClip (/* CGContextRef */ IntPtr c);

		[iOS (11,0), Mac(10,13), TV(11,0), Watch(4,0)]
		public void ResetClip ()
		{
			CGContextResetClip (handle);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextClipToMask (/* CGContextRef */ IntPtr c, CGRect rect, 
			/* CGImageRef __nullable */ IntPtr mask);

		public void ClipToMask (CGRect rect, CGImage mask)
		{
			CGContextClipToMask (handle, rect, mask == null ? IntPtr.Zero : mask.handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGRect CGContextGetClipBoundingBox (/* CGContextRef */ IntPtr c);

		public CGRect GetClipBoundingBox ()
		{
			return CGContextGetClipBoundingBox (handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextClipToRect (/* CGContextRef */ IntPtr c, CGRect rect);

		public void ClipToRect (CGRect rect)
		{
			CGContextClipToRect (handle, rect);
		}
		       
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextClipToRects (/* CGContextRef */ IntPtr c, CGRect [] rects, /* size_t */ nint count);

		public void ClipToRects (CGRect [] rects)
		{
			if (rects == null)
				throw new ArgumentNullException ("rects");
			CGContextClipToRects (handle, rects, rects.Length);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetFillColorWithColor (/* CGContextRef */ IntPtr c,
			/* CGColorRef __nullable */ IntPtr color);

		public void SetFillColor (CGColor color)
		{
			CGContextSetFillColorWithColor (handle, color == null ? IntPtr.Zero : color.handle);
		}

#if !XAMCORE_2_0
		[Advice ("Use SetFillColor() instead.")]
		public void SetFillColorWithColor (CGColor color)
		{
			SetFillColor (color);
		}
#endif
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetStrokeColorWithColor (/* CGContextRef */ IntPtr c,
			/* CGColorRef __nullable */ IntPtr color);

		public void SetStrokeColor (CGColor color)
		{
			CGContextSetStrokeColorWithColor (handle, color == null ? IntPtr.Zero : color.handle);
		}
		
#if !XAMCORE_2_0
		[Advice ("Use SetStrokeColor() instead.")]
		public void SetStrokeColorWithColor (CGColor color)
		{
			SetStrokeColor (color);
		}
#endif

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetFillColorSpace (/* CGContextRef */ IntPtr context,
			/* CGColorSpaceRef __nullable */ IntPtr space);

		public void SetFillColorSpace (CGColorSpace space)
		{
			CGContextSetFillColorSpace (handle, space == null ? IntPtr.Zero : space.handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetStrokeColorSpace (/* CGContextRef */ IntPtr context,
			/* CGColorSpaceRef __nullable */ IntPtr space);

		public void SetStrokeColorSpace (CGColorSpace space)
		{
			CGContextSetStrokeColorSpace (handle, space == null ? IntPtr.Zero : space.handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetFillColor (/* CGContextRef */ IntPtr context,
			/* const CGFloat * __nullable */ nfloat [] components);

		public void SetFillColor (nfloat [] components)
		{
			CGContextSetFillColor (handle, components);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetStrokeColor (/* CGContextRef */ IntPtr context,
			/* const CGFloat * __nullable */ nfloat [] components);

		public void SetStrokeColor (nfloat [] components)
		{
			CGContextSetStrokeColor (handle, components);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetFillPattern (/* CGContextRef */ IntPtr context,
			/* CGPatternRef __nullable */ IntPtr pattern, /* const CGFloat * __nullable */ nfloat [] components);

		public void SetFillPattern (CGPattern pattern, nfloat [] components)
		{
			CGContextSetFillPattern (handle, pattern == null ? IntPtr.Zero : pattern.handle, components);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetStrokePattern (/* CGContextRef */ IntPtr context,
			/* CGPatternRef __nullable */ IntPtr pattern, /* const CGFloat * __nullable */ nfloat [] components);

		public void SetStrokePattern (CGPattern pattern, nfloat [] components)
		{
			CGContextSetStrokePattern (handle, pattern == null ? IntPtr.Zero : pattern.handle, components);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetPatternPhase (/* CGContextRef */ IntPtr context, CGSize phase);

		public void SetPatternPhase (CGSize phase)
		{
			CGContextSetPatternPhase (handle, phase);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetGrayFillColor (/* CGContextRef */ IntPtr context, /* CGFloat */ nfloat gray, /* CGFloat */ nfloat alpha);

		public void SetFillColor (nfloat gray, nfloat alpha)
		{
			CGContextSetGrayFillColor (handle, gray, alpha);
		}

#if !XAMCORE_2_0
		[Advice ("Use SetFillColor() instead.")]
		public void SetGrayFillColor (nfloat gray, nfloat alpha)
		{
			CGContextSetGrayFillColor (handle, gray, alpha);
		}
#endif
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetGrayStrokeColor (/* CGContextRef */ IntPtr context, /* CGFloat */ nfloat gray, /* CGFloat */ nfloat alpha);

		public void SetStrokeColor (nfloat gray, nfloat alpha)
		{
			CGContextSetGrayStrokeColor (handle, gray, alpha);
		}
		
#if !XAMCORE_2_0
		[Advice ("Use SetStrokeColor() instead.")]
		public void SetGrayStrokeColor (nfloat gray, nfloat alpha)
		{
			CGContextSetGrayStrokeColor (handle, gray, alpha);
		}
#endif
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetRGBFillColor (/* CGContextRef */ IntPtr context, /* CGFloat */ nfloat red, /* CGFloat */ nfloat green, /* CGFloat */ nfloat blue, /* CGFloat */ nfloat alpha);

		public void SetFillColor (nfloat red, nfloat green, nfloat blue, nfloat alpha)
		{
			CGContextSetRGBFillColor (handle, red, green, blue, alpha);
		}
		
#if !XAMCORE_2_0
		[Advice ("Use SetFillColor() instead.")]
		public void SetRGBFillColor (nfloat red, nfloat green, nfloat blue, nfloat alpha)
		{
			CGContextSetRGBFillColor (handle, red, green, blue, alpha);
		}
#endif

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetRGBStrokeColor (/* CGContextRef */ IntPtr context, /* CGFloat */ nfloat red, /* CGFloat */ nfloat green, /* CGFloat */ nfloat blue, /* CGFloat */ nfloat alpha);

		public void SetStrokeColor (nfloat red, nfloat green, nfloat blue, nfloat alpha)
		{
			CGContextSetRGBStrokeColor (handle, red, green, blue, alpha);
		}
		
#if !XAMCORE_2_0
		[Advice ("Use SetStrokeColor() instead.")]
		public void SetRGBStrokeColor (nfloat red, nfloat green, nfloat blue, nfloat alpha)
		{
			CGContextSetRGBStrokeColor (handle, red, green, blue, alpha);
		}
#endif

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetCMYKFillColor (/* CGContextRef */ IntPtr context, /* CGFloat */ nfloat cyan, /* CGFloat */ nfloat magenta, /* CGFloat */ nfloat yellow, /* CGFloat */ nfloat black, /* CGFloat */ nfloat alpha);

		public void SetFillColor (nfloat cyan, nfloat magenta, nfloat yellow, nfloat black, nfloat alpha)
		{
			CGContextSetCMYKFillColor (handle, cyan, magenta, yellow, black, alpha);
		}
		
#if !XAMCORE_2_0
		[Advice ("Use SetFillColor() instead.")]
		public void SetCMYKFillColor (nfloat cyan, nfloat magenta, nfloat yellow, nfloat black, nfloat alpha)
		{
			CGContextSetCMYKFillColor (handle, cyan, magenta, yellow, black, alpha);
		}
#endif

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetCMYKStrokeColor (/* CGContextRef */ IntPtr context, /* CGFloat */ nfloat cyan, /* CGFloat */ nfloat magenta, /* CGFloat */ nfloat yellow, /* CGFloat */ nfloat black, /* CGFloat */ nfloat alpha);

		public void SetStrokeColor (nfloat cyan, nfloat magenta, nfloat yellow, nfloat black, nfloat alpha)
		{
			CGContextSetCMYKStrokeColor (handle, cyan, magenta, yellow, black, alpha);
		}
		
#if !XAMCORE_2_0
		[Advice ("Use SetStrokeColor() instead.")]
		public void SetCMYKStrokeColor (nfloat cyan, nfloat magenta, nfloat yellow, nfloat black, nfloat alpha)
		{
			CGContextSetCMYKStrokeColor (handle, cyan, magenta, yellow, black, alpha);
		}
#endif

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetRenderingIntent (/* CGContextRef */ IntPtr context, CGColorRenderingIntent intent);

		public void SetRenderingIntent (CGColorRenderingIntent intent)
		{
			CGContextSetRenderingIntent (handle, intent);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextDrawImage (/* CGContextRef */ IntPtr c, CGRect rect,
			/* CGImageRef __nullable */ IntPtr image);

		public void DrawImage (CGRect rect, CGImage image)
		{
			CGContextDrawImage (handle, rect, image == null ? IntPtr.Zero : image.Handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextDrawTiledImage (/* CGContextRef */ IntPtr c, CGRect rect,
			/* CGImageRef __nullable */ IntPtr image);

		public void DrawTiledImage (CGRect rect, CGImage image)
		{
			CGContextDrawTiledImage (handle, rect, image == null ? IntPtr.Zero : image.Handle);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGInterpolationQuality CGContextGetInterpolationQuality (/* CGContextRef */ IntPtr context);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetInterpolationQuality (/* CGContextRef */ IntPtr context, CGInterpolationQuality quality);
		
		public CGInterpolationQuality  InterpolationQuality {
			get {
				return CGContextGetInterpolationQuality (handle);
			}

			set {
				CGContextSetInterpolationQuality (handle, value);
			}
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetShadowWithColor (/* CGContextRef __nullable */ IntPtr context, CGSize offset,
			/* CGFloat */ nfloat blur, /* CGColorRef __nullable */ IntPtr color);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetShadow (IntPtr context, CGSize offset, nfloat blur);

#if XAMCORE_2_0
		public void SetShadow (CGSize offset, nfloat blur, CGColor color = null)
		{
			if (color == null)
				CGContextSetShadow (handle, offset, blur);
			else
				CGContextSetShadowWithColor (handle, offset, blur, color.handle);
		}
#else
		public void SetShadowWithColor (CGSize offset, nfloat blur, CGColor color)
		{
			CGContextSetShadowWithColor (handle, offset, blur, color == null ? IntPtr.Zero : color.handle);
		}
		
		public void SetShadow (CGSize offset, nfloat blur)
		{
			CGContextSetShadow (handle, offset, blur);
		}
#endif

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextDrawLinearGradient (/* CGContextRef __nullable */ IntPtr context,
			/* CGGradientRef __nullable */ IntPtr gradient, CGPoint startPoint, CGPoint endPoint,
			CGGradientDrawingOptions options);

		public void DrawLinearGradient (CGGradient gradient, CGPoint startPoint, CGPoint endPoint, CGGradientDrawingOptions options)
		{
			CGContextDrawLinearGradient (handle, gradient == null ? IntPtr.Zero : gradient.handle, startPoint, endPoint, options);
		}
			
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextDrawRadialGradient (/* CGContextRef __nullable */ IntPtr context,
			/* CGGradientRef __nullable */ IntPtr gradient, 
			CGPoint startCenter, /* CGFloat */ nfloat startRadius,
			CGPoint endCenter, /* CGFloat */ nfloat endRadius, CGGradientDrawingOptions options);

		public void DrawRadialGradient (CGGradient gradient, CGPoint startCenter, nfloat startRadius, CGPoint endCenter, nfloat endRadius, CGGradientDrawingOptions options)
		{
			CGContextDrawRadialGradient (handle, gradient == null ? IntPtr.Zero : gradient.handle, startCenter, startRadius, endCenter, endRadius, options);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextDrawShading (/* CGContextRef */ IntPtr context,
			/* CGShadingRef __nullable */ IntPtr shading);

		public void DrawShading (CGShading shading)
		{
			CGContextDrawShading (handle, shading == null ? IntPtr.Zero : shading.handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetCharacterSpacing (/* CGContextRef */ IntPtr context, /* GCFloat */ nfloat spacing);

		public void SetCharacterSpacing (nfloat spacing)
		{
			CGContextSetCharacterSpacing (handle, spacing);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetTextPosition (/* CGContextRef */ IntPtr c, /* GCFloat */ nfloat x, /* GCFloat */ nfloat y);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGPoint CGContextGetTextPosition (/* CGContextRef */ IntPtr context);

		public CGPoint TextPosition {
			get {
				return CGContextGetTextPosition (handle);
			}
			set {
				CGContextSetTextPosition (handle, value.X, value.Y);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetTextMatrix (/* CGContextRef */ IntPtr c, CGAffineTransform t);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGAffineTransform CGContextGetTextMatrix (/* CGContextRef */ IntPtr c);

		public CGAffineTransform TextMatrix {
			get {
				return CGContextGetTextMatrix (handle);
			}
			set {
				CGContextSetTextMatrix (handle, value);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetTextDrawingMode (/* CGContextRef */ IntPtr c, CGTextDrawingMode mode);

		public void SetTextDrawingMode (CGTextDrawingMode mode)
		{
			CGContextSetTextDrawingMode (handle, mode);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetFont (/* CGContextRef */ IntPtr c, /* CGFontRef __nullable */ IntPtr font);

		public void SetFont (CGFont font)
		{
			CGContextSetFont (handle, font == null ? IntPtr.Zero : font.handle);
		}
			
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetFontSize (/* CGContextRef */ IntPtr c, /* CGFloat */ nfloat size);

		public void SetFontSize (nfloat size)
		{
			CGContextSetFontSize (handle, size);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSelectFont (/* CGContextRef */ IntPtr c,
			/* const char* __nullable */ string name, /* CGFloat */ nfloat size, CGTextEncoding textEncoding);

		[Deprecated (PlatformName.iOS, 7, 0, message : "Use the 'CoreText' API instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 9, message : "Use the 'CoreText' API instead.")]
		public void SelectFont (string name, nfloat size, CGTextEncoding textEncoding)
		{
			CGContextSelectFont (handle, name, size, textEncoding);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextShowGlyphsAtPositions (/* CGContextRef __nullable */ IntPtr context,
			/* const CGGlyph * __nullable */ ushort [] glyphs,
			/* const CGPoint * __nullable */ CGPoint [] positions, /* size_t */ nint count);

#if XAMCORE_2_0
		public void ShowGlyphsAtPositions (ushort [] glyphs, CGPoint [] positions, int count = -1)
		{
			if (glyphs == null)
				count = 0;
			else if (count < 0)
				count = glyphs.Length;
			CGContextShowGlyphsAtPositions (handle, glyphs, positions, count);
		}
#else
		public void ShowGlyphsAtPositions (ushort [] glyphs, CGPoint [] positions, int size_t_count)
		{
			if (size_t_count < 0 || size_t_count > glyphs.Length)
				throw new ArgumentException ("size_t_count");
			CGContextShowGlyphsAtPositions (handle, glyphs, positions, size_t_count);
		}
#endif

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextShowText (/* CGContextRef */ IntPtr c, /* const char* __nullable */ string s, /* size_t */ nint length);

		[Deprecated (PlatformName.iOS, 7, 0, message : "Use the 'CoreText' API instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 9, message : "Use the 'CoreText' API instead.")]
		public void ShowText (string str, int count)
		{
			if (str == null)
				count = 0;
			else if (count > str.Length)
				throw new ArgumentException ("count");
			CGContextShowText (handle, str, count);
		}

		[Deprecated (PlatformName.iOS, 7, 0, message : "Use the 'CoreText' API instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 9, message : "Use the 'CoreText' API instead.")]
		public void ShowText (string str)
		{
			CGContextShowText (handle, str, str == null ? 0 : str.Length);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextShowText (/* CGContextRef */ IntPtr c, /* const char* __nullable */ byte[] bytes, /* size_t */ nint length);

		[Deprecated (PlatformName.iOS, 7, 0, message : "Use the 'CoreText' API instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 9, message : "Use the 'CoreText' API instead.")]
		public void ShowText (byte[] bytes, int count)
		{
			if (bytes == null)
				count = 0;
			else if (count > bytes.Length)
				throw new ArgumentException ("count");
			CGContextShowText (handle, bytes, count);
		}
		
		[Deprecated (PlatformName.iOS, 7, 0, message : "Use the 'CoreText' API instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 9, message : "Use the 'CoreText' API instead.")]
		public void ShowText (byte[] bytes)
		{
			CGContextShowText (handle, bytes, bytes == null ? 0 : bytes.Length);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextShowTextAtPoint (/* CGContextRef __nullable */ IntPtr c, /* CGFloat */ nfloat x, 
			/* CGFloat */ nfloat y, /* const char* __nullable */ string str, /* size_t */ nint length);

		[Deprecated (PlatformName.iOS, 7, 0, message : "Use the 'CoreText' API instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 9, message : "Use the 'CoreText' API instead.")]
		public void ShowTextAtPoint (nfloat x, nfloat y, string str, int length)
		{
			CGContextShowTextAtPoint (handle, x, y, str, length);
		}

		[Deprecated (PlatformName.iOS, 7, 0, message : "Use the 'CoreText' API instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 9, message : "Use the 'CoreText' API instead.")]
		public void ShowTextAtPoint (nfloat x, nfloat y, string str)
		{
			CGContextShowTextAtPoint (handle, x, y, str, str == null ? 0 : str.Length);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextShowTextAtPoint (/* CGContextRef */ IntPtr c, /* CGFloat */ nfloat x, /* CGFloat */ nfloat y, /* const char* */ byte[] bytes, /* size_t */ nint length);

		public void ShowTextAtPoint (nfloat x, nfloat y, byte[] bytes, int length)
		{
			CGContextShowTextAtPoint (handle, x, y, bytes, length);
		}
		
		public void ShowTextAtPoint (nfloat x, nfloat y, byte[] bytes)
		{
			CGContextShowTextAtPoint (handle, x, y, bytes, bytes == null ? 0 : bytes.Length);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextShowGlyphs (/* CGContextRef __nullable */ IntPtr c,
			/* const CGGlyph * __nullable */ ushort [] glyphs, /* size_t */ nint count);

		[Deprecated (PlatformName.iOS, 7, 0, message : "Use the 'CoreText' API instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 9, message : "Use the 'CoreText' API instead.")]
		public void ShowGlyphs (ushort [] glyphs)
		{
			CGContextShowGlyphs (handle, glyphs, glyphs == null ? 0 : glyphs.Length);
		}

		[Deprecated (PlatformName.iOS, 7, 0, message : "Use the 'CoreText' API instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 9, message : "Use the 'CoreText' API instead.")]
		public void ShowGlyphs (ushort [] glyphs, int count)
		{
			if (glyphs == null)
				count = 0;
			else if (count > glyphs.Length)
				throw new ArgumentException ("count");
			CGContextShowGlyphs (handle, glyphs, count);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextShowGlyphsAtPoint (/* CGContextRef */ IntPtr context, /* CGFloat */ nfloat x,
			/* CGFloat */ nfloat y, /* const CGGlyph * __nullable */ ushort [] glyphs, /* size_t */ nint count);

		[Deprecated (PlatformName.iOS, 7, 0, message : "Use the 'CoreText' API instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 9, message : "Use the 'CoreText' API instead.")]
		public void ShowGlyphsAtPoint (nfloat x, nfloat y, ushort [] glyphs, int count)
		{
			if (glyphs == null)
				count = 0;
			else if (count > glyphs.Length)
				throw new ArgumentException ("count");
			CGContextShowGlyphsAtPoint (handle, x, y, glyphs, count);
		}

		[Deprecated (PlatformName.iOS, 7, 0, message : "Use the 'CoreText' API instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 9, message : "Use the 'CoreText' API instead.")]
		public void ShowGlyphsAtPoint (nfloat x, nfloat y, ushort [] glyphs)
		{
			CGContextShowGlyphsAtPoint (handle, x, y, glyphs, glyphs == null ? 0 : glyphs.Length);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextShowGlyphsWithAdvances (/* CGContextRef __nullable */ IntPtr c,
			/* const CGGlyph * __nullable */ ushort [] glyphs,
			/* const CGSize * __nullable */ CGSize [] advances, /* size_t */ nint count);

		[Deprecated (PlatformName.iOS, 7, 0, message : "Use the 'CoreText' API instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 9, message : "Use the 'CoreText' API instead.")]
		public void ShowGlyphsWithAdvances (ushort [] glyphs, CGSize [] advances, int count)
		{
			if (glyphs == null)
				count = 0;
			if (count > glyphs.Length || count > advances.Length)
				throw new ArgumentException ("count");
			CGContextShowGlyphsWithAdvances (handle, glyphs, advances, count);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextDrawPDFPage (/* CGContextRef __nullable */ IntPtr c,
			/* CGPDFPageRef __nullable */ IntPtr page);

		public void DrawPDFPage (CGPDFPage page)
		{
			CGContextDrawPDFPage (handle, page == null ? IntPtr.Zero : page.handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static void CGContextBeginPage (/* CGContextRef __nullable */ IntPtr c,
			/* const CGRect * __nullable */ CGRect *mediaBox);

		public unsafe void BeginPage (CGRect? rect)
		{
			if (rect.HasValue){
				CGRect v = rect.Value;
				CGContextBeginPage (handle, &v);
			} else {
				CGContextBeginPage (handle, null);
			}
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextEndPage (/* CGContextRef __nullable */ IntPtr c);

		public void EndPage ()
		{
			CGContextEndPage (handle);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextFlush (/* CGContextRef __nullable */ IntPtr c);

		public void Flush ()
		{
			CGContextFlush (handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSynchronize (/* CGContextRef __nullable */ IntPtr c);

		public void Synchronize ()
		{
			CGContextSynchronize (handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetShouldAntialias (/* CGContextRef */ IntPtr context, bool shouldAntialias);

		public void SetShouldAntialias (bool shouldAntialias)
		{
			CGContextSetShouldAntialias (handle, shouldAntialias);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetAllowsAntialiasing (/* CGContextRef */ IntPtr context, bool allowsAntialiasing);
		public void SetAllowsAntialiasing (bool allowsAntialiasing)
		{
			CGContextSetAllowsAntialiasing (handle, allowsAntialiasing);
		}
			
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetShouldSmoothFonts (/* CGContextRef */ IntPtr context, bool shouldSmoothFonts);

		public void SetShouldSmoothFonts (bool shouldSmoothFonts)
		{
			CGContextSetShouldSmoothFonts (handle, shouldSmoothFonts);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGAffineTransform CGContextGetUserSpaceToDeviceSpaceTransform (/* CGContextRef */ IntPtr context);

		public CGAffineTransform GetUserSpaceToDeviceSpaceTransform ()
		{
			return CGContextGetUserSpaceToDeviceSpaceTransform (handle);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGPoint CGContextConvertPointToDeviceSpace (/* CGContextRef */ IntPtr context, CGPoint point);

		public CGPoint PointToDeviceSpace (CGPoint point)
		{
			return CGContextConvertPointToDeviceSpace (handle, point);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGPoint CGContextConvertPointToUserSpace (/* CGContextRef */ IntPtr context, CGPoint point);

		public CGPoint ConvertPointToUserSpace (CGPoint point)
		{
			return CGContextConvertPointToUserSpace (handle, point);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGSize CGContextConvertSizeToDeviceSpace (/* CGContextRef */ IntPtr context, CGSize size);

		public CGSize ConvertSizeToDeviceSpace (CGSize size)
		{
			return CGContextConvertSizeToDeviceSpace (handle, size);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGSize CGContextConvertSizeToUserSpace (/* CGContextRef */ IntPtr context, CGSize size);

		public CGSize ConvertSizeToUserSpace (CGSize size)
		{
			return CGContextConvertSizeToUserSpace (handle, size);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGRect CGContextConvertRectToDeviceSpace (/* CGContextRef */ IntPtr context, CGRect rect);

		public CGRect ConvertRectToDeviceSpace (CGRect rect)
		{
			return CGContextConvertRectToDeviceSpace (handle, rect);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGRect CGContextConvertRectToUserSpace (/* CGContextRef */ IntPtr context, CGRect rect);

		public CGRect ConvertRectToUserSpace (CGRect rect)
		{
			return CGContextConvertRectToUserSpace (handle, rect);
		}

		// CGLayer.h
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextDrawLayerInRect (/* CGContextRef */ IntPtr context, CGRect rect, /* CGLayerRef */ IntPtr layer);

		public void DrawLayer (CGLayer layer, CGRect rect)
		{
			if (layer == null)
				throw new ArgumentNullException ("layer");
			CGContextDrawLayerInRect (handle, rect, layer.Handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextDrawLayerAtPoint (/* CGContextRef */ IntPtr context, CGPoint rect, /* CGLayerRef */ IntPtr layer);

		public void DrawLayer (CGLayer layer, CGPoint point)
		{
			if (layer == null)
				throw new ArgumentNullException ("layer");
			CGContextDrawLayerAtPoint (handle, point, layer.Handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGPathRef */ IntPtr CGContextCopyPath (/* CGContextRef */ IntPtr context);

		public CGPath CopyPath ()
		{
			var r = CGContextCopyPath (handle);
			return new CGPath (r, true);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetAllowsFontSmoothing (/* CGContextRef */ IntPtr context, bool shouldSubpixelPositionFonts);

		public void SetAllowsFontSmoothing (bool allows)
		{
			CGContextSetAllowsFontSmoothing (handle, allows);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetAllowsFontSubpixelPositioning (/* CGContextRef */ IntPtr context, bool allowsFontSubpixelPositioning);

		public void SetAllowsSubpixelPositioning (bool allows)
		{
			CGContextSetAllowsFontSubpixelPositioning (handle, allows);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetAllowsFontSubpixelQuantization (/* CGContextRef */ IntPtr context, bool shouldSubpixelQuantizeFonts);

		public void SetAllowsFontSubpixelQuantization (bool allows)
		{
			CGContextSetAllowsFontSubpixelQuantization (handle, allows);
		}
			
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetShouldSubpixelPositionFonts (/* CGContextRef */ IntPtr context, bool shouldSubpixelPositionFonts);

		public void SetShouldSubpixelPositionFonts (bool shouldSubpixelPositionFonts)
		{
			CGContextSetShouldSubpixelPositionFonts (handle, shouldSubpixelPositionFonts);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextSetShouldSubpixelQuantizeFonts (/* CGContextRef */ IntPtr context, bool shouldSubpixelQuantizeFonts);

		public void ShouldSubpixelQuantizeFonts (bool shouldSubpixelQuantizeFonts)
		{
			CGContextSetShouldSubpixelQuantizeFonts (handle, shouldSubpixelQuantizeFonts);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextBeginTransparencyLayer (/* CGContextRef */ IntPtr context, /* CFDictionaryRef __nullable */ IntPtr auxiliaryInfo);

#if !XAMCORE_2_0
		public void BeginTransparencyLayer ()
		{
			CGContextBeginTransparencyLayer (handle, IntPtr.Zero);
		}
#endif
		
		public void BeginTransparencyLayer (NSDictionary auxiliaryInfo = null)
		{
			CGContextBeginTransparencyLayer (handle, auxiliaryInfo == null ? IntPtr.Zero : auxiliaryInfo.Handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextBeginTransparencyLayerWithRect (/* CGContextRef */ IntPtr context, CGRect rect, /* CFDictionaryRef __nullable */ IntPtr auxiliaryInfo);

		public void BeginTransparencyLayer (CGRect rectangle, NSDictionary auxiliaryInfo = null)
		{
			CGContextBeginTransparencyLayerWithRect (handle, rectangle, auxiliaryInfo == null ? IntPtr.Zero : auxiliaryInfo.Handle);
		}

#if !XAMCORE_2_0
		public void BeginTransparencyLayer (CGRect rectangle)
		{
			CGContextBeginTransparencyLayerWithRect (handle, rectangle, IntPtr.Zero);
		}
#endif

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextEndTransparencyLayer (/* CGContextRef */ IntPtr context);

		public void EndTransparencyLayer ()
		{
			CGContextEndTransparencyLayer (handle);
		}

		public CGBitmapContext AsBitmapContext ()
		{
			return new CGBitmapContext (Handle, false);
		}
#endif // !COREBUILD
	}

	[iOS(11,0), Mac(10,13)]
	public enum CGPDFAccessPermissions : uint {
		AllowsLowQualityPrinting    = (1 << 0),
		AllowsHighQualityPrinting   = (1 << 1),
		AllowsDocumentChanges       = (1 << 2),
		AllowsDocumentAssembly      = (1 << 3),
		AllowsContentCopying        = (1 << 4),
		AllowsContentAccessibility  = (1 << 5),
		AllowsCommenting            = (1 << 6),
		AllowsFormFieldEntry        = (1 << 7),
	}
}
