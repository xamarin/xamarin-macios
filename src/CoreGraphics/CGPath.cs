// 
// CGPath.cs: Implements the managed CGPath
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
	public enum CGPathElementType {
		MoveToPoint,
		AddLineToPoint,
		AddQuadCurveToPoint,
		AddCurveToPoint,
		CloseSubpath
	}

	// CGPath.h
	public struct CGPathElement {
		public CGPathElementType Type;

		public CGPathElement (int t)
		{
			Type = (CGPathElementType) t;
			Point1 = Point2 = Point3 = CGPoint.Empty;
		}
		
		// Set for MoveToPoint, AddLineToPoint, AddQuadCurveToPoint, AddCurveToPoint
		public CGPoint Point1;

		// Set for AddQuadCurveToPoint, AddCurveToPoint
		public CGPoint Point2;

		// Set for AddCurveToPoint
		public CGPoint Point3;
	}
	
	public class CGPath : INativeObject, IDisposable {
		internal IntPtr handle;

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGMutablePathRef */ IntPtr CGPathCreateMutable ();

		public CGPath ()
		{
			handle = CGPathCreateMutable ();
		}

		public CGPath (CGPath reference, CGAffineTransform transform)
		{
			if (reference == null)
				throw new ArgumentNullException ("reference");
			handle = CGPathCreateMutableCopyByTransformingPath (reference.Handle, ref transform);
		}
	
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGMutablePathRef */ IntPtr CGPathCreateMutableCopy (/* CGPathRef */ IntPtr path);

		public CGPath (CGPath basePath)
		{
			if (basePath == null)
				throw new ArgumentNullException ("basePath");
			handle = CGPathCreateMutableCopy (basePath.handle);
		}

		//
		// For use by marshallrs
		//
		public CGPath (IntPtr handle)
		{
			CGPathRetain (handle);
			this.handle = handle;
		}

		// Indicates that we own it `owns'
		[Preserve (Conditional=true)]
		internal CGPath (IntPtr handle, bool owns)
		{
			if (!owns)
				CGPathRetain (handle);
			
			this.handle = handle;
		}
		
		~CGPath ()
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
		}
	
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPathRelease (/* CGPathRef */ IntPtr path);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGPathRef */ IntPtr CGPathRetain (/* CGPathRef */ IntPtr path);
		
		protected virtual void Dispose (bool disposing)
		{
			if (handle != IntPtr.Zero){
				CGPathRelease (handle);
				handle = IntPtr.Zero;
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static bool CGPathEqualToPath (/* CGPathRef */ IntPtr path1, /* CGPathRef */ IntPtr path2);

		public static bool operator == (CGPath path1, CGPath path2)
		{
			return Object.Equals (path1, path2);
		}

		public static bool operator != (CGPath path1, CGPath path2)
		{
			return !Object.Equals (path1, path2);
		}

		public override int GetHashCode ()
		{
			return handle.GetHashCode ();
		}

		public override bool Equals (object o)
		{
			CGPath other = o as CGPath;
			if (other == null)
				return false;

			return CGPathEqualToPath (this.handle, other.handle);
		}
       
		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static void CGPathMoveToPoint (/* CGMutablePathRef */ IntPtr path, CGAffineTransform *m, /* CGFloat */ nfloat x, /* CGFloat */ nfloat y);

		public unsafe void MoveToPoint (nfloat x, nfloat y)
		{
			CGPathMoveToPoint (handle, null, x, y);
		}

		public unsafe void MoveToPoint (CGPoint point)
		{
			CGPathMoveToPoint (handle, null, point.X, point.Y);
		}
		
		public unsafe void MoveToPoint (CGAffineTransform transform, nfloat x, nfloat y)
		{
			CGPathMoveToPoint (handle, &transform, x, y);
		}

		public unsafe void MoveToPoint (CGAffineTransform transform, CGPoint point)
		{
			CGPathMoveToPoint (handle, &transform, point.X, point.Y);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static void CGPathAddLineToPoint (/* CGMutablePathRef */ IntPtr path, CGAffineTransform *m, /* CGFloat */ nfloat x, /* CGFloat */ nfloat y);

#if !XAMCORE_2_0
		[Advice ("Use 'AddLineToPoint' instead.")] // Bad name
		public void CGPathAddLineToPoint (nfloat x, nfloat y)
		{
			AddLineToPoint (x, y);
		}
#endif

		public unsafe void AddLineToPoint (nfloat x, nfloat y)
		{
			CGPathAddLineToPoint (handle, null, x, y);
		}

		public unsafe void AddLineToPoint (CGPoint point)
		{
			CGPathAddLineToPoint (handle, null, point.X, point.Y);
		}
		
#if !XAMCORE_2_0
		[Advice ("Use 'AddLineToPoint' instead.")] // Bad name
		public void CGPathAddLineToPoint (CGAffineTransform transform, nfloat x, nfloat y)
		{
			AddLineToPoint (transform, x, y);
		}
#endif

		public unsafe void AddLineToPoint (CGAffineTransform transform, nfloat x, nfloat y)
		{
			CGPathAddLineToPoint (handle, &transform, x, y);
		}

		public unsafe void AddLineToPoint (CGAffineTransform transform, CGPoint point)
		{
			CGPathAddLineToPoint (handle, &transform, point.X, point.Y);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static void CGPathAddQuadCurveToPoint (/* CGMutablePathRef */ IntPtr path, CGAffineTransform *m, /* CGFloat */ nfloat cpx, /* CGFloat */ nfloat cpy, /* CGFloat */ nfloat x, /* CGFloat */ nfloat y);

		public unsafe void AddQuadCurveToPoint (nfloat cpx, nfloat cpy, nfloat x, nfloat y)
		{
			CGPathAddQuadCurveToPoint (handle, null, cpx, cpy, x, y);
		}

		public unsafe void AddQuadCurveToPoint (CGAffineTransform transform, nfloat cpx, nfloat cpy, nfloat x, nfloat y)
		{
			CGPathAddQuadCurveToPoint (handle, &transform, cpx, cpy, x, y);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static void CGPathAddCurveToPoint (/* CGMutablePathRef */ IntPtr path, CGAffineTransform *m, /* CGFloat */ nfloat cp1x, /* CGFloat */ nfloat cp1y, /* CGFloat */ nfloat cp2x, /* CGFloat */ nfloat cp2y, /* CGFloat */ nfloat x, /* CGFloat */ nfloat y);

		public unsafe void AddCurveToPoint (CGAffineTransform transform, nfloat cp1x, nfloat cp1y, nfloat cp2x, nfloat cp2y, nfloat x, nfloat y)
		{
			CGPathAddCurveToPoint (handle, &transform, cp1x, cp1y, cp2x, cp2y, x, y);
		}

		public unsafe void AddCurveToPoint (CGAffineTransform transform, CGPoint cp1, CGPoint cp2, CGPoint point)
		{
			CGPathAddCurveToPoint (handle, &transform, cp1.X, cp1.Y, cp2.X, cp2.Y, point.X, point.Y);
		}

		public unsafe void AddCurveToPoint (nfloat cp1x, nfloat cp1y, nfloat cp2x, nfloat cp2y, nfloat x, nfloat y)
		{
			CGPathAddCurveToPoint (handle, null, cp1x, cp1y, cp2x, cp2y, x, y);
		}
			
		public unsafe void AddCurveToPoint (CGPoint cp1, CGPoint cp2, CGPoint point)
		{
			CGPathAddCurveToPoint (handle, null, cp1.X, cp1.Y, cp2.X, cp2.Y, point.X, point.Y);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPathCloseSubpath (/* CGMutablePathRef */ IntPtr path);

		public void CloseSubpath ()
		{
			CGPathCloseSubpath (handle);
		}
			
		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static void CGPathAddRect (/* CGMutablePathRef */ IntPtr path, CGAffineTransform *m, CGRect rect);

		public unsafe void AddRect (CGAffineTransform transform, CGRect rect)
		{
			CGPathAddRect (handle, &transform, rect);
		}

		public unsafe void AddRect (CGRect rect)
		{
			CGPathAddRect (handle, null, rect);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static void CGPathAddRects (/* CGMutablePathRef */ IntPtr path, CGAffineTransform *m, CGRect [] rects, /* size_t */ nint count);

		public unsafe void AddRects (CGAffineTransform m, CGRect [] rects)
		{
			if (rects == null)
				throw new ArgumentNullException ("rects");
			CGPathAddRects (handle, &m, rects, rects.Length);
		}

		public unsafe void AddRects (CGAffineTransform m, CGRect [] rects, int count)
		{
			if (rects == null)
				throw new ArgumentNullException ("rects");
			if (count > rects.Length)
				throw new ArgumentException ("count");
			CGPathAddRects (handle, &m, rects, count);
		}
		
		public unsafe void AddRects (CGRect [] rects)
		{
			if (rects == null)
				throw new ArgumentNullException ("rects");
			CGPathAddRects (handle, null, rects, rects.Length);
		}

		public unsafe void AddRects (CGRect [] rects, int count)
		{
			if (rects == null)
				throw new ArgumentNullException ("rects");
			if (count > rects.Length)
				throw new ArgumentException ("count");
			CGPathAddRects (handle, null, rects, count);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static void CGPathAddLines (/* CGMutablePathRef */ IntPtr path, CGAffineTransform *m, CGPoint [] points, /* size_t */ nint count);

		public unsafe void AddLines (CGAffineTransform m, CGPoint [] points)
		{
			if (points == null)
				throw new ArgumentNullException ("points");
			CGPathAddLines (handle, &m, points, points.Length);
		}

#if !XAMCORE_2_0
		[Advice ("Misnamed method, it's 'AddLines'.")]
		public unsafe void AddRects (CGAffineTransform m, CGPoint [] points, int count)
#else
		public unsafe void AddLines (CGAffineTransform m, CGPoint [] points, int count)
#endif
		{
			if (points == null)
				throw new ArgumentNullException ("points");
			if (count > points.Length)
				throw new ArgumentException ("count");
			CGPathAddLines (handle, &m, points, count);
		}

		public unsafe void AddLines (CGPoint [] points)
		{
			if (points == null)
				throw new ArgumentNullException ("points");
			CGPathAddLines (handle, null, points, points.Length);
		}

#if !XAMCORE_2_0
		[Advice ("Misnamed method, it's 'AddLines'.")]
		public unsafe void AddRects (CGPoint [] points, int count)
#else
		public unsafe void AddLines (CGPoint [] points, int count)
#endif
		{
			if (points == null)
				throw new ArgumentNullException ("points");
			if (count > points.Length)
				throw new ArgumentException ("count");
			CGPathAddLines (handle, null, points, count);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static void CGPathAddEllipseInRect (/* CGMutablePathRef */ IntPtr path, CGAffineTransform *m, CGRect rect);

		public unsafe void AddEllipseInRect (CGAffineTransform m, CGRect rect)
		{
			CGPathAddEllipseInRect (handle, &m, rect);
		}
#if !XAMCORE_2_0
		[Obsolete ("Use 'AddEllipseInRect' instead.")]
		public unsafe void AddElipseInRect (CGAffineTransform m, CGRect rect)
		{
			CGPathAddEllipseInRect (handle, &m, rect);
		}
#endif
		
		public unsafe void AddEllipseInRect (CGRect rect)
		{
			CGPathAddEllipseInRect (handle, null, rect);
		}
#if !XAMCORE_2_0
		[Obsolete ("Use 'AddEllipseInRect' instead.")]
		public unsafe void AddElipseInRect (CGRect rect)
		{
			CGPathAddEllipseInRect (handle, null, rect);
		}
#endif

		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static void CGPathAddArc (/* CGMutablePathRef */ IntPtr path, CGAffineTransform *m, /* CGFloat */ nfloat x, /* CGFloat */ nfloat y, /* CGFloat */ nfloat radius, /* CGFloat */ nfloat startAngle, /* CGFloat */ nfloat endAngle, bool clockwise);

		public unsafe void AddArc (CGAffineTransform m, nfloat x, nfloat y, nfloat radius, nfloat startAngle, nfloat endAngle, bool clockwise)
		{
			CGPathAddArc (handle, &m, x, y, radius, startAngle, endAngle, clockwise);
		}
		
		public unsafe void AddArc (nfloat x, nfloat y, nfloat radius, nfloat startAngle, nfloat endAngle, bool clockwise)
		{
			CGPathAddArc (handle, null, x, y, radius, startAngle, endAngle, clockwise);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static void CGPathAddArcToPoint (/* CGMutablePathRef */ IntPtr path, CGAffineTransform *m, /* CGFloat */ nfloat x1, /* CGFloat */ nfloat y1, /* CGFloat */ nfloat x2, /* CGFloat */ nfloat y2, /* CGFloat */ nfloat radius);

		public unsafe void AddArcToPoint (CGAffineTransform m, nfloat x1, nfloat y1, nfloat x2, nfloat y2, nfloat radius)
		{
			CGPathAddArcToPoint (handle, &m, x1, y1, x2, y2, radius);
		}
		
		public unsafe void AddArcToPoint (nfloat x1, nfloat y1, nfloat x2, nfloat y2, nfloat radius)
		{
			CGPathAddArcToPoint (handle, null, x1, y1, x2, y2, radius);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static void CGPathAddRelativeArc (/* CGMutablePathRef */ IntPtr path, CGAffineTransform *m, /* CGFloat */ nfloat x, /* CGFloat */ nfloat y, /* CGFloat */ nfloat radius, /* CGFloat */ nfloat startAngle, /* CGFloat */ nfloat delta);

		public unsafe void AddRelativeArc (CGAffineTransform m, nfloat x, nfloat y, nfloat radius, nfloat startAngle, nfloat delta)
		{
			CGPathAddRelativeArc (handle, &m, x, y, radius, startAngle, delta);
		}

		public unsafe void AddRelativeArc (nfloat x, nfloat y, nfloat radius, nfloat startAngle, nfloat delta)
		{
			CGPathAddRelativeArc (handle, null, x, y, radius, startAngle, delta);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static void CGPathAddPath (/* CGMutablePathRef */ IntPtr path1, CGAffineTransform *m, /* CGMutablePathRef */ IntPtr path2);

		public unsafe void AddPath (CGAffineTransform t, CGPath path2)
		{
			if (path2 == null)
				throw new ArgumentNullException ("path2");
			CGPathAddPath (handle, &t, path2.handle);
		}
		
		public unsafe void AddPath (CGPath path2)
		{
			if (path2 == null)
				throw new ArgumentNullException ("path2");
			CGPathAddPath (handle, null, path2.handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static bool CGPathIsEmpty (/* CGPathRef */ IntPtr path);

		public bool IsEmpty {
			get {
				return CGPathIsEmpty (handle);
			}
		}
			
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static bool CGPathIsRect (/* CGPathRef */ IntPtr path, out CGRect rect);

		public bool IsRect (out CGRect rect)
		{
			return CGPathIsRect (handle, out rect);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGPoint CGPathGetCurrentPoint(/* CGPathRef */ IntPtr path);

		public CGPoint CurrentPoint {
			get {
				return CGPathGetCurrentPoint (handle);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGRect CGPathGetBoundingBox (/* CGPathRef */IntPtr path);

		public CGRect BoundingBox {
			get {
				return CGPathGetBoundingBox (handle);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGRect CGPathGetPathBoundingBox (/* CGPathRef */ IntPtr path);

		public CGRect PathBoundingBox {
			get {
				return CGPathGetPathBoundingBox (handle);
			}
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static bool CGPathContainsPoint(IntPtr path, CGAffineTransform *m, CGPoint point, bool eoFill);

		public unsafe bool ContainsPoint (CGAffineTransform m, CGPoint point, bool eoFill)
		{
			return CGPathContainsPoint (handle, &m, point, eoFill);
		}
		
		public unsafe bool ContainsPoint (CGPoint point, bool eoFill)
		{
			return CGPathContainsPoint (handle, null, point, eoFill);
		}

		public delegate void ApplierFunction (CGPathElement element);

		delegate void CGPathApplierFunction (/* void* */ IntPtr info, /* const CGPathElement* */ IntPtr element);
		
#if !MONOMAC
		[MonoPInvokeCallback (typeof (CGPathApplierFunction))]
#endif
		static void ApplierCallback (IntPtr info, IntPtr element_ptr)
		{
			GCHandle gch = GCHandle.FromIntPtr (info);
			// note: CGPathElementType is an untyped enum, always 32bits
			CGPathElement element = new CGPathElement (Marshal.ReadInt32(element_ptr, 0));
			ApplierFunction func = (ApplierFunction) gch.Target;

			IntPtr ptr = Marshal.ReadIntPtr (element_ptr, IntPtr.Size);
			int ptsize = Marshal.SizeOf (typeof (CGPoint));

			switch (element.Type){
			case CGPathElementType.CloseSubpath:
				break;

			case CGPathElementType.MoveToPoint:
			case CGPathElementType.AddLineToPoint:
				element.Point1 = (CGPoint) Marshal.PtrToStructure (ptr, typeof (CGPoint));
				break;

			case CGPathElementType.AddQuadCurveToPoint:
				element.Point1 = (CGPoint) Marshal.PtrToStructure (ptr, typeof (CGPoint));
				element.Point2 = (CGPoint) Marshal.PtrToStructure (((IntPtr) (((long)ptr) + ptsize)), typeof (CGPoint));
				break;

			case CGPathElementType.AddCurveToPoint:
				element.Point1 = (CGPoint) Marshal.PtrToStructure (ptr, typeof (CGPoint));
				element.Point2 = (CGPoint) Marshal.PtrToStructure (((IntPtr) (((long)ptr) + ptsize)), typeof (CGPoint));
				element.Point3 = (CGPoint) Marshal.PtrToStructure (((IntPtr) (((long)ptr) + (2*ptsize))), typeof (CGPoint));
				break;
			}

			func (element);
		}


		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPathApply (/* CGPathRef */ IntPtr path, /* void* */ IntPtr info, CGPathApplierFunction function);

		public void Apply (ApplierFunction func)
		{
			GCHandle gch = GCHandle.Alloc (func);
			CGPathApply (handle, GCHandle.ToIntPtr (gch), ApplierCallback);
			gch.Free ();
		}

		static CGPath MakeMutable (IntPtr source)
		{
			var mutable = CGPathCreateMutableCopy (source);
			return new CGPath (mutable, owns: true);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern unsafe static IntPtr CGPathCreateCopyByDashingPath (
			/* CGPathRef */ IntPtr path, 
			/* const CGAffineTransform * */ CGAffineTransform *transform, 
			/* CGFloat */ nfloat phase,
			/* CGFloat */ nfloat [] lengths,
			/* size_t */ nint count);

		public CGPath CopyByDashingPath (CGAffineTransform transform, nfloat [] lengths)
		{
			return CopyByDashingPath (transform, lengths, 0);
		}

		public unsafe CGPath CopyByDashingPath (CGAffineTransform transform, nfloat [] lengths, nfloat phase)
		{
			return MakeMutable (CGPathCreateCopyByDashingPath (handle, &transform, phase, lengths, lengths == null ? 0 : lengths.Length));
		}

		public CGPath CopyByDashingPath (nfloat [] lengths)
		{
			return CopyByDashingPath (lengths, 0);
		}

		public unsafe CGPath CopyByDashingPath (nfloat [] lengths, nfloat phase)
		{
			var path = CGPathCreateCopyByDashingPath (handle, null, phase, lengths, lengths == null ? 0 : lengths.Length);
			return MakeMutable (path);
		}

		public unsafe CGPath Copy ()
		{
			return MakeMutable (handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static IntPtr CGPathCreateCopyByStrokingPath (/* CGPathRef */ IntPtr path, CGAffineTransform *transform, nfloat lineWidth, CGLineCap lineCap, CGLineJoin lineJoin, /* CGFloat */ nfloat miterLimit);

		public unsafe CGPath CopyByStrokingPath (CGAffineTransform transform, nfloat lineWidth, CGLineCap lineCap, CGLineJoin lineJoin, nfloat miterLimit)
		{
			return MakeMutable (CGPathCreateCopyByStrokingPath (handle, &transform, lineWidth, lineCap, lineJoin, miterLimit));
		}

		public unsafe CGPath CopyByStrokingPath (nfloat lineWidth, CGLineCap lineCap, CGLineJoin lineJoin, nfloat miterLimit)
		{
			return MakeMutable (CGPathCreateCopyByStrokingPath (handle, null, lineWidth, lineCap, lineJoin, miterLimit));
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static IntPtr CGPathCreateCopyByTransformingPath (/* CGPathRef */ IntPtr path, ref CGAffineTransform transform);

		public CGPath CopyByTransformingPath (CGAffineTransform transform)
		{
			return MakeMutable (CGPathCreateCopyByTransformingPath (handle, ref transform));
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGMutablePathRef */ IntPtr CGPathCreateMutableCopyByTransformingPath (/* CGPathRef */ IntPtr path, /* const CGAffineTransform* */ ref CGAffineTransform transform);

		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static IntPtr CGPathCreateWithEllipseInRect (CGRect boundingRect, CGAffineTransform *transform);

		static public unsafe CGPath EllipseFromRect (CGRect boundingRect, CGAffineTransform transform)
		{
			return MakeMutable (CGPathCreateWithEllipseInRect (boundingRect, &transform));
		}

		static public unsafe CGPath EllipseFromRect (CGRect boundingRect)
		{
			return MakeMutable (CGPathCreateWithEllipseInRect (boundingRect, null));
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static IntPtr CGPathCreateWithRect (CGRect boundingRect, CGAffineTransform *transform);

		static public unsafe CGPath FromRect (CGRect rectangle, CGAffineTransform transform)
		{
			return MakeMutable (CGPathCreateWithRect (rectangle, &transform));
		}

		static public unsafe CGPath FromRect (CGRect rectangle)
		{
			return MakeMutable (CGPathCreateWithRect (rectangle, null));
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static /* CGPathRef */ IntPtr CGPathCreateWithRoundedRect (CGRect rect, /* CGFloat */ nfloat cornerWidth, /* CGFloat */ nfloat cornerHeight, CGAffineTransform *transform);

		static unsafe CGPath _FromRoundedRect (CGRect rectangle, nfloat cornerWidth, nfloat cornerHeight, CGAffineTransform *transform)
		{
			if ((cornerWidth < 0) || (2 * cornerWidth > rectangle.Width))
				throw new ArgumentException ("cornerWidth");
			if ((cornerHeight < 0) || (2 * cornerHeight > rectangle.Height))
				throw new ArgumentException ("cornerHeight");
			return MakeMutable (CGPathCreateWithRoundedRect (rectangle, cornerWidth, cornerHeight, transform));
		}

		[Mac(10,9)][iOS (7,0)]
		static unsafe public CGPath FromRoundedRect (CGRect rectangle, nfloat cornerWidth, nfloat cornerHeight)
		{
			return _FromRoundedRect (rectangle, cornerWidth, cornerHeight, null);
		}

		[Mac(10,9)][iOS (7,0)]
		static public unsafe CGPath FromRoundedRect (CGRect rectangle, nfloat cornerWidth, nfloat cornerHeight, CGAffineTransform transform)
		{
			return _FromRoundedRect (rectangle, cornerWidth, cornerHeight, &transform);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static void CGPathAddRoundedRect (/* CGMutablePathRef */ IntPtr path, CGAffineTransform *transform, CGRect rect, /* CGFloat */ nfloat cornerWidth, /* CGFloat */ nfloat cornerHeight);

		[Mac(10,9)][iOS (7,0)]
		public unsafe void AddRoundedRect (CGAffineTransform transform, CGRect rect, nfloat cornerWidth, nfloat cornerHeight)
		{
			CGPathAddRoundedRect (handle, &transform, rect, cornerWidth, cornerHeight);
		}

		[Mac(10,9)][iOS (7,0)]
		public unsafe void AddRoundedRect (CGRect rect, nfloat cornerWidth, nfloat cornerHeight)
		{
			CGPathAddRoundedRect (handle, null, rect, cornerWidth, cornerHeight);
		}
	}
}
