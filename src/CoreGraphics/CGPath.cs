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

#nullable enable

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using CoreFoundation;
using ObjCRuntime;
using Foundation;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace CoreGraphics {

	// untyped enum -> CGPath.h
	public enum CGPathElementType {
		MoveToPoint,
		AddLineToPoint,
		AddQuadCurveToPoint,
		AddCurveToPoint,
		CloseSubpath
	}


#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
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

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CGPath : NativeObject {
#if !COREBUILD
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGMutablePathRef */ IntPtr CGPathCreateMutable ();

		public CGPath ()
			: base (CGPathCreateMutable (), true)
		{
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static /* CGMutablePathRef */ IntPtr CGPathCreateMutableCopyByTransformingPath (/* CGPathRef */ IntPtr path, /* const CGAffineTransform* */ CGAffineTransform* transform);

		public unsafe CGPath (CGPath reference, CGAffineTransform transform)
			: base (CGPathCreateMutableCopyByTransformingPath (Runtime.ThrowOnNull (reference, nameof (reference)).Handle, &transform), true)
		{
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGMutablePathRef */ IntPtr CGPathCreateMutableCopy (/* CGPathRef */ IntPtr path);

		public CGPath (CGPath basePath)
			: base (CGPathCreateMutableCopy (Runtime.ThrowOnNull (basePath, nameof (basePath)).Handle), true)
		{
		}

#if !NET
		public CGPath (NativeHandle handle)
			: base (handle, false)
		{
		}
#endif

		[Preserve (Conditional = true)]
		internal CGPath (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPathRelease (/* CGPathRef */ IntPtr path);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGPathRef */ IntPtr CGPathRetain (/* CGPathRef */ IntPtr path);

		protected internal override void Retain ()
		{
			CGPathRetain (GetCheckedHandle ());
		}

		protected internal override void Release ()
		{
			CGPathRelease (GetCheckedHandle ());
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static byte CGPathEqualToPath (/* CGPathRef */ IntPtr path1, /* CGPathRef */ IntPtr path2);

		public static bool operator == (CGPath? path1, CGPath? path2)
		{
			if (path1 is null)
				return path2 is null;
			return path1.Equals (path2);
		}

		public static bool operator != (CGPath? path1, CGPath? path2)
		{
			if (path1 is null)
				return path2 is not null;
			return !path1.Equals (path2);
		}

		public override int GetHashCode ()
		{
			// looks weird but it's valid
			// using the Handle property would not be since there's a special function for equality
			// see Remarks in https://docs.microsoft.com/en-us/dotnet/api/system.object.gethashcode?view=net-6.0
			return 0;
		}

		public override bool Equals (object? o)
		{
			var other = o as CGPath;
			if (other is null)
				return false;

			return CGPathEqualToPath (this.Handle, other.Handle) != 0;
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static void CGPathMoveToPoint (/* CGMutablePathRef */ IntPtr path, CGAffineTransform* m, /* CGFloat */ nfloat x, /* CGFloat */ nfloat y);

		public unsafe void MoveToPoint (nfloat x, nfloat y)
		{
			CGPathMoveToPoint (Handle, null, x, y);
		}

		public unsafe void MoveToPoint (CGPoint point)
		{
			CGPathMoveToPoint (Handle, null, point.X, point.Y);
		}

		public unsafe void MoveToPoint (CGAffineTransform transform, nfloat x, nfloat y)
		{
			CGPathMoveToPoint (Handle, &transform, x, y);
		}

		public unsafe void MoveToPoint (CGAffineTransform transform, CGPoint point)
		{
			CGPathMoveToPoint (Handle, &transform, point.X, point.Y);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static void CGPathAddLineToPoint (/* CGMutablePathRef */ IntPtr path, CGAffineTransform* m, /* CGFloat */ nfloat x, /* CGFloat */ nfloat y);

		public unsafe void AddLineToPoint (nfloat x, nfloat y)
		{
			CGPathAddLineToPoint (Handle, null, x, y);
		}

		public unsafe void AddLineToPoint (CGPoint point)
		{
			CGPathAddLineToPoint (Handle, null, point.X, point.Y);
		}

		public unsafe void AddLineToPoint (CGAffineTransform transform, nfloat x, nfloat y)
		{
			CGPathAddLineToPoint (Handle, &transform, x, y);
		}

		public unsafe void AddLineToPoint (CGAffineTransform transform, CGPoint point)
		{
			CGPathAddLineToPoint (Handle, &transform, point.X, point.Y);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static void CGPathAddQuadCurveToPoint (/* CGMutablePathRef */ IntPtr path, CGAffineTransform* m, /* CGFloat */ nfloat cpx, /* CGFloat */ nfloat cpy, /* CGFloat */ nfloat x, /* CGFloat */ nfloat y);

		public unsafe void AddQuadCurveToPoint (nfloat cpx, nfloat cpy, nfloat x, nfloat y)
		{
			CGPathAddQuadCurveToPoint (Handle, null, cpx, cpy, x, y);
		}

		public unsafe void AddQuadCurveToPoint (CGAffineTransform transform, nfloat cpx, nfloat cpy, nfloat x, nfloat y)
		{
			CGPathAddQuadCurveToPoint (Handle, &transform, cpx, cpy, x, y);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static void CGPathAddCurveToPoint (/* CGMutablePathRef */ IntPtr path, CGAffineTransform* m, /* CGFloat */ nfloat cp1x, /* CGFloat */ nfloat cp1y, /* CGFloat */ nfloat cp2x, /* CGFloat */ nfloat cp2y, /* CGFloat */ nfloat x, /* CGFloat */ nfloat y);

		public unsafe void AddCurveToPoint (CGAffineTransform transform, nfloat cp1x, nfloat cp1y, nfloat cp2x, nfloat cp2y, nfloat x, nfloat y)
		{
			CGPathAddCurveToPoint (Handle, &transform, cp1x, cp1y, cp2x, cp2y, x, y);
		}

		public unsafe void AddCurveToPoint (CGAffineTransform transform, CGPoint cp1, CGPoint cp2, CGPoint point)
		{
			CGPathAddCurveToPoint (Handle, &transform, cp1.X, cp1.Y, cp2.X, cp2.Y, point.X, point.Y);
		}

		public unsafe void AddCurveToPoint (nfloat cp1x, nfloat cp1y, nfloat cp2x, nfloat cp2y, nfloat x, nfloat y)
		{
			CGPathAddCurveToPoint (Handle, null, cp1x, cp1y, cp2x, cp2y, x, y);
		}

		public unsafe void AddCurveToPoint (CGPoint cp1, CGPoint cp2, CGPoint point)
		{
			CGPathAddCurveToPoint (Handle, null, cp1.X, cp1.Y, cp2.X, cp2.Y, point.X, point.Y);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPathCloseSubpath (/* CGMutablePathRef */ IntPtr path);

		public void CloseSubpath ()
		{
			CGPathCloseSubpath (Handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static void CGPathAddRect (/* CGMutablePathRef */ IntPtr path, CGAffineTransform* m, CGRect rect);

		public unsafe void AddRect (CGAffineTransform transform, CGRect rect)
		{
			CGPathAddRect (Handle, &transform, rect);
		}

		public unsafe void AddRect (CGRect rect)
		{
			CGPathAddRect (Handle, null, rect);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static void CGPathAddRects (/* CGMutablePathRef */ IntPtr path, CGAffineTransform* m, CGRect [] rects, /* size_t */ nint count);

		public unsafe void AddRects (CGAffineTransform m, CGRect [] rects)
		{
			if (rects is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (rects));
			CGPathAddRects (Handle, &m, rects, rects.Length);
		}

		public unsafe void AddRects (CGAffineTransform m, CGRect [] rects, int count)
		{
			if (rects is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (rects));
			if (count > rects.Length)
				throw new ArgumentException (nameof (count));
			CGPathAddRects (Handle, &m, rects, count);
		}

		public unsafe void AddRects (CGRect [] rects)
		{
			if (rects is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (rects));
			CGPathAddRects (Handle, null, rects, rects.Length);
		}

		public unsafe void AddRects (CGRect [] rects, int count)
		{
			if (rects is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (rects));
			if (count > rects.Length)
				throw new ArgumentException (nameof (count));
			CGPathAddRects (Handle, null, rects, count);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static void CGPathAddLines (/* CGMutablePathRef */ IntPtr path, CGAffineTransform* m, CGPoint [] points, /* size_t */ nint count);

		public unsafe void AddLines (CGAffineTransform m, CGPoint [] points)
		{
			if (points is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (points));
			CGPathAddLines (Handle, &m, points, points.Length);
		}

		public unsafe void AddLines (CGAffineTransform m, CGPoint [] points, int count)
		{
			if (points is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (points));
			if (count > points.Length)
				throw new ArgumentException (nameof (count));
			CGPathAddLines (Handle, &m, points, count);
		}

		public unsafe void AddLines (CGPoint [] points)
		{
			if (points is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (points));
			CGPathAddLines (Handle, null, points, points.Length);
		}

		public unsafe void AddLines (CGPoint [] points, int count)
		{
			if (points is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (points));
			if (count > points.Length)
				throw new ArgumentException (nameof (count));
			CGPathAddLines (Handle, null, points, count);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static void CGPathAddEllipseInRect (/* CGMutablePathRef */ IntPtr path, CGAffineTransform* m, CGRect rect);

		public unsafe void AddEllipseInRect (CGAffineTransform m, CGRect rect)
		{
			CGPathAddEllipseInRect (Handle, &m, rect);
		}

		public unsafe void AddEllipseInRect (CGRect rect)
		{
			CGPathAddEllipseInRect (Handle, null, rect);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static void CGPathAddArc (/* CGMutablePathRef */ IntPtr path, CGAffineTransform* m, /* CGFloat */ nfloat x, /* CGFloat */ nfloat y, /* CGFloat */ nfloat radius, /* CGFloat */ nfloat startAngle, /* CGFloat */ nfloat endAngle, byte clockwise);

		public unsafe void AddArc (CGAffineTransform m, nfloat x, nfloat y, nfloat radius, nfloat startAngle, nfloat endAngle, bool clockwise)
		{
			CGPathAddArc (Handle, &m, x, y, radius, startAngle, endAngle, clockwise.AsByte ());
		}

		public unsafe void AddArc (nfloat x, nfloat y, nfloat radius, nfloat startAngle, nfloat endAngle, bool clockwise)
		{
			CGPathAddArc (Handle, null, x, y, radius, startAngle, endAngle, clockwise.AsByte ());
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static void CGPathAddArcToPoint (/* CGMutablePathRef */ IntPtr path, CGAffineTransform* m, /* CGFloat */ nfloat x1, /* CGFloat */ nfloat y1, /* CGFloat */ nfloat x2, /* CGFloat */ nfloat y2, /* CGFloat */ nfloat radius);

		public unsafe void AddArcToPoint (CGAffineTransform m, nfloat x1, nfloat y1, nfloat x2, nfloat y2, nfloat radius)
		{
			CGPathAddArcToPoint (Handle, &m, x1, y1, x2, y2, radius);
		}

		public unsafe void AddArcToPoint (nfloat x1, nfloat y1, nfloat x2, nfloat y2, nfloat radius)
		{
			CGPathAddArcToPoint (Handle, null, x1, y1, x2, y2, radius);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static void CGPathAddRelativeArc (/* CGMutablePathRef */ IntPtr path, CGAffineTransform* m, /* CGFloat */ nfloat x, /* CGFloat */ nfloat y, /* CGFloat */ nfloat radius, /* CGFloat */ nfloat startAngle, /* CGFloat */ nfloat delta);

		public unsafe void AddRelativeArc (CGAffineTransform m, nfloat x, nfloat y, nfloat radius, nfloat startAngle, nfloat delta)
		{
			CGPathAddRelativeArc (Handle, &m, x, y, radius, startAngle, delta);
		}

		public unsafe void AddRelativeArc (nfloat x, nfloat y, nfloat radius, nfloat startAngle, nfloat delta)
		{
			CGPathAddRelativeArc (Handle, null, x, y, radius, startAngle, delta);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static void CGPathAddPath (/* CGMutablePathRef */ IntPtr path1, CGAffineTransform* m, /* CGMutablePathRef */ IntPtr path2);

		public unsafe void AddPath (CGAffineTransform t, CGPath path2)
		{
			if (path2 is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (path2));
			CGPathAddPath (Handle, &t, path2.Handle);
		}

		public unsafe void AddPath (CGPath path2)
		{
			if (path2 is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (path2));
			CGPathAddPath (Handle, null, path2.Handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static byte CGPathIsEmpty (/* CGPathRef */ IntPtr path);

		public bool IsEmpty {
			get {
				return CGPathIsEmpty (Handle) != 0;
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static byte CGPathIsRect (/* CGPathRef */ IntPtr path, CGRect* rect);

		public bool IsRect (out CGRect rect)
		{
			unsafe {
				rect = default;
				return CGPathIsRect (Handle, (CGRect*) Unsafe.AsPointer<CGRect> (ref rect)) != 0;
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGPoint CGPathGetCurrentPoint (/* CGPathRef */ IntPtr path);

		public CGPoint CurrentPoint {
			get {
				return CGPathGetCurrentPoint (Handle);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGRect CGPathGetBoundingBox (/* CGPathRef */IntPtr path);

		public CGRect BoundingBox {
			get {
				return CGPathGetBoundingBox (Handle);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGRect CGPathGetPathBoundingBox (/* CGPathRef */ IntPtr path);

		public CGRect PathBoundingBox {
			get {
				return CGPathGetPathBoundingBox (Handle);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static byte CGPathContainsPoint (IntPtr path, CGAffineTransform* m, CGPoint point, byte eoFill);

		public unsafe bool ContainsPoint (CGAffineTransform m, CGPoint point, bool eoFill)
		{
			return CGPathContainsPoint (Handle, &m, point, eoFill.AsByte ()) != 0;
		}

		public unsafe bool ContainsPoint (CGPoint point, bool eoFill)
		{
			return CGPathContainsPoint (Handle, null, point, eoFill.AsByte ()) != 0;
		}

		public delegate void ApplierFunction (CGPathElement element);

		delegate void CGPathApplierFunction (/* void* */ IntPtr info, /* const CGPathElement* */ IntPtr element);
#if NET
		[UnmanagedCallersOnly]
#else
#if !MONOMAC
		[MonoPInvokeCallback (typeof (CGPathApplierFunction))]
#endif
#endif
		static void ApplierCallback (IntPtr info, IntPtr element_ptr)
		{
			GCHandle gch = GCHandle.FromIntPtr (info);
			// note: CGPathElementType is an untyped enum, always 32bits
			CGPathElement element = new CGPathElement (Marshal.ReadInt32 (element_ptr, 0));
			var func = gch.Target as ApplierFunction;
			if (func is null)
				return;

			IntPtr ptr = Marshal.ReadIntPtr (element_ptr, IntPtr.Size);
			int ptsize = Marshal.SizeOf<CGPoint> ();

			switch (element.Type) {
			case CGPathElementType.CloseSubpath:
				break;

			case CGPathElementType.MoveToPoint:
			case CGPathElementType.AddLineToPoint:
				element.Point1 = Marshal.PtrToStructure<CGPoint> (ptr)!;
				break;

			case CGPathElementType.AddQuadCurveToPoint:
				element.Point1 = Marshal.PtrToStructure<CGPoint> (ptr)!;
				element.Point2 = Marshal.PtrToStructure<CGPoint> (((IntPtr) (((long) ptr) + ptsize)))!;
				break;

			case CGPathElementType.AddCurveToPoint:
				element.Point1 = Marshal.PtrToStructure<CGPoint> (ptr)!;
				element.Point2 = Marshal.PtrToStructure<CGPoint> (((IntPtr) (((long) ptr) + ptsize)))!;
				element.Point3 = Marshal.PtrToStructure<CGPoint> (((IntPtr) (((long) ptr) + (2 * ptsize))))!;
				break;
			}

			func (element);
		}


		[DllImport (Constants.CoreGraphicsLibrary)]
#if NET
		extern unsafe static void CGPathApply (/* CGPathRef */ IntPtr path, /* void* */ IntPtr info, delegate* unmanaged<IntPtr, IntPtr, void> function);
#else
		extern static void CGPathApply (/* CGPathRef */ IntPtr path, /* void* */ IntPtr info, CGPathApplierFunction function);
#endif

		public void Apply (ApplierFunction func)
		{
			GCHandle gch = GCHandle.Alloc (func);
#if NET
			unsafe {
				CGPathApply (Handle, GCHandle.ToIntPtr (gch), &ApplierCallback);
			}
#else
			CGPathApply (Handle, GCHandle.ToIntPtr (gch), ApplierCallback);
#endif
			gch.Free ();
		}

#if NET
		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("maccatalyst16.0")]
		[SupportedOSPlatform ("macos13.0")]
		[SupportedOSPlatform ("tvos16.0")]
#else
		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0), Watch (9, 0)]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern IntPtr CGPathCreateCopyByNormalizing (IntPtr path, byte evenOddFillRule);

		public CGPath? CreateByNormalizing (bool evenOddFillRule)
		{
			return Runtime.GetINativeObject<CGPath> (CGPathCreateCopyByNormalizing (Handle, evenOddFillRule.AsByte ()), owns: true);
		}

#if NET
		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("maccatalyst16.0")]
		[SupportedOSPlatform ("macos13.0")]
		[SupportedOSPlatform ("tvos16.0")]
#else
		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0), Watch (9, 0)]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern IntPtr CGPathCreateCopyByUnioningPath (IntPtr path, IntPtr maskPath, byte evenOddFillRule);

		public CGPath? CreateByUnioningPath (CGPath? maskPath, bool evenOddFillRule)
		{
			return Runtime.GetINativeObject<CGPath> (CGPathCreateCopyByUnioningPath (Handle, maskPath.GetHandle (), evenOddFillRule.AsByte ()), owns: true);
		}

#if NET
		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("maccatalyst16.0")]
		[SupportedOSPlatform ("macos13.0")]
		[SupportedOSPlatform ("tvos16.0")]
#else
		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0), Watch (9, 0)]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern IntPtr CGPathCreateCopyByIntersectingPath (IntPtr path, IntPtr maskPath, byte evenOddFillRule);

		public CGPath? CreateByIntersectingPath (CGPath? maskPath, bool evenOddFillRule)
		{
			return Runtime.GetINativeObject<CGPath> (CGPathCreateCopyByIntersectingPath (Handle, maskPath.GetHandle (), evenOddFillRule.AsByte ()), owns: true);
		}

#if NET
		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("maccatalyst16.0")]
		[SupportedOSPlatform ("macos13.0")]
		[SupportedOSPlatform ("tvos16.0")]
#else
		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0), Watch (9, 0)]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern IntPtr CGPathCreateCopyBySubtractingPath (IntPtr path, IntPtr maskPath, byte evenOddFillRule);

		public CGPath? CreateBySubtractingPath (CGPath? maskPath, bool evenOddFillRule)
		{
			return Runtime.GetINativeObject<CGPath> (CGPathCreateCopyBySubtractingPath (Handle, maskPath.GetHandle (), evenOddFillRule.AsByte ()), owns: true);
		}

#if NET
		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("maccatalyst16.0")]
		[SupportedOSPlatform ("macos13.0")]
		[SupportedOSPlatform ("tvos16.0")]
#else
		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0), Watch (9, 0)]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern IntPtr CGPathCreateCopyBySymmetricDifferenceOfPath (IntPtr path, IntPtr maskPath, byte evenOddFillRule);

		public CGPath? CreateBySymmetricDifferenceOfPath (CGPath? maskPath, bool evenOddFillRule)
		{
			return Runtime.GetINativeObject<CGPath> (CGPathCreateCopyBySymmetricDifferenceOfPath (Handle, maskPath.GetHandle (), evenOddFillRule.AsByte ()), owns: true);
		}

#if NET
		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("maccatalyst16.0")]
		[SupportedOSPlatform ("macos13.0")]
		[SupportedOSPlatform ("tvos16.0")]
#else
		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0), Watch (9, 0)]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern IntPtr CGPathCreateCopyOfLineBySubtractingPath (IntPtr path, IntPtr maskPath, byte evenOddFillRule);

		public CGPath? CreateLineBySubtractingPath (CGPath? maskPath, bool evenOddFillRule)
		{
			return Runtime.GetINativeObject<CGPath> (CGPathCreateCopyOfLineBySubtractingPath (Handle, maskPath.GetHandle (), evenOddFillRule.AsByte ()), owns: true);
		}

#if NET
		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("maccatalyst16.0")]
		[SupportedOSPlatform ("macos13.0")]
		[SupportedOSPlatform ("tvos16.0")]
#else
		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0), Watch (9, 0)]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern IntPtr CGPathCreateCopyOfLineByIntersectingPath (IntPtr path, IntPtr maskPath, byte evenOddFillRule);

		public CGPath? CreateLineByIntersectingPath (CGPath? maskPath, bool evenOddFillRule)
		{
			return Runtime.GetINativeObject<CGPath> (CGPathCreateCopyOfLineByIntersectingPath (Handle, maskPath.GetHandle (), evenOddFillRule.AsByte ()), owns: true);
		}

#if NET
		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("maccatalyst16.0")]
		[SupportedOSPlatform ("macos13.0")]
		[SupportedOSPlatform ("tvos16.0")]
#else
		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0), Watch (9, 0)]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern unsafe /* CFArrayRef __nullable */ IntPtr CGPathCreateSeparateComponents (IntPtr path, byte evenOddFillRule);

		public CGPath [] GetSeparateComponents (bool evenOddFillRule)
		{
			var cfArrayRef = CGPathCreateSeparateComponents (Handle, evenOddFillRule.AsByte ());
			if (cfArrayRef == IntPtr.Zero)
				return Array.Empty<CGPath> ();
			return NSArray.ArrayFromHandle<CGPath> (cfArrayRef);
		}

#if NET
		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("maccatalyst16.0")]
		[SupportedOSPlatform ("macos13.0")]
		[SupportedOSPlatform ("tvos16.0")]
#else
		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0), Watch (9, 0)]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern IntPtr CGPathCreateCopyByFlattening (IntPtr path, nfloat flatteningThreshold);

		public CGPath? CreateByFlattening (nfloat flatteningThreshold)
		{
			return Runtime.GetINativeObject<CGPath> (CGPathCreateCopyByFlattening (Handle, flatteningThreshold), owns: true);
		}

#if NET
		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("maccatalyst16.0")]
		[SupportedOSPlatform ("macos13.0")]
		[SupportedOSPlatform ("tvos16.0")]
#else
		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0), Watch (9, 0)]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern byte CGPathIntersectsPath (IntPtr path1, IntPtr path2, byte evenOddFillRule);

		public bool DoesIntersect (CGPath? maskPath, bool evenOddFillRule)
		{
			return CGPathIntersectsPath (Handle, maskPath.GetHandle (), evenOddFillRule.AsByte ()) != 0;
		}

		static CGPath MakeMutable (IntPtr source, bool owns)
		{
			var mutable = CGPathCreateMutableCopy (source);
			if (owns)
				CGPathRelease (source);
			return new CGPath (mutable, owns: true);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern unsafe static IntPtr CGPathCreateCopyByDashingPath (
			/* CGPathRef */ IntPtr path,
			/* const CGAffineTransform * */ CGAffineTransform* transform,
			/* CGFloat */ nfloat phase,
			/* CGFloat */ nfloat* lengths,
			/* size_t */ nint count);

		public CGPath CopyByDashingPath (CGAffineTransform transform, nfloat [] lengths)
		{
			return CopyByDashingPath (transform, lengths, 0);
		}

		public unsafe CGPath CopyByDashingPath (CGAffineTransform transform, nfloat [] lengths, nfloat phase)
		{
			fixed (nfloat* lengthsPtr = lengths) {
				return MakeMutable (CGPathCreateCopyByDashingPath (Handle, &transform, phase, lengthsPtr, lengths is null ? 0 : lengths.Length), true);
			}
		}

		public CGPath CopyByDashingPath (nfloat [] lengths)
		{
			return CopyByDashingPath (lengths, 0);
		}

		public unsafe CGPath CopyByDashingPath (nfloat [] lengths, nfloat phase)
		{
			fixed (nfloat* lengthsPtr = lengths) {
				var path = CGPathCreateCopyByDashingPath (Handle, null, phase, lengthsPtr, lengths is null ? 0 : lengths.Length);
				return MakeMutable (path, true);
			}
		}

		public unsafe CGPath Copy ()
		{
			return MakeMutable (Handle, false);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static IntPtr CGPathCreateCopyByStrokingPath (/* CGPathRef */ IntPtr path, CGAffineTransform* transform, nfloat lineWidth, CGLineCap lineCap, CGLineJoin lineJoin, /* CGFloat */ nfloat miterLimit);

		public unsafe CGPath CopyByStrokingPath (CGAffineTransform transform, nfloat lineWidth, CGLineCap lineCap, CGLineJoin lineJoin, nfloat miterLimit)
		{
			return MakeMutable (CGPathCreateCopyByStrokingPath (Handle, &transform, lineWidth, lineCap, lineJoin, miterLimit), true);
		}

		public unsafe CGPath CopyByStrokingPath (nfloat lineWidth, CGLineCap lineCap, CGLineJoin lineJoin, nfloat miterLimit)
		{
			return MakeMutable (CGPathCreateCopyByStrokingPath (Handle, null, lineWidth, lineCap, lineJoin, miterLimit), true);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static IntPtr CGPathCreateCopyByTransformingPath (/* CGPathRef */ IntPtr path, CGAffineTransform* transform);

		public CGPath CopyByTransformingPath (CGAffineTransform transform)
		{
			unsafe {
				return MakeMutable (CGPathCreateCopyByTransformingPath (Handle, &transform), true);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static IntPtr CGPathCreateWithEllipseInRect (CGRect boundingRect, CGAffineTransform* transform);

		static public unsafe CGPath EllipseFromRect (CGRect boundingRect, CGAffineTransform transform)
		{
			return MakeMutable (CGPathCreateWithEllipseInRect (boundingRect, &transform), true);
		}

		static public unsafe CGPath EllipseFromRect (CGRect boundingRect)
		{
			return MakeMutable (CGPathCreateWithEllipseInRect (boundingRect, null), true);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static IntPtr CGPathCreateWithRect (CGRect boundingRect, CGAffineTransform* transform);

		static public unsafe CGPath FromRect (CGRect rectangle, CGAffineTransform transform)
		{
			return MakeMutable (CGPathCreateWithRect (rectangle, &transform), true);
		}

		static public unsafe CGPath FromRect (CGRect rectangle)
		{
			return MakeMutable (CGPathCreateWithRect (rectangle, null), true);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static /* CGPathRef */ IntPtr CGPathCreateWithRoundedRect (CGRect rect, /* CGFloat */ nfloat cornerWidth, /* CGFloat */ nfloat cornerHeight, CGAffineTransform* transform);

		static unsafe CGPath _FromRoundedRect (CGRect rectangle, nfloat cornerWidth, nfloat cornerHeight, CGAffineTransform* transform)
		{
			if ((cornerWidth < 0) || (2 * cornerWidth > rectangle.Width))
				throw new ArgumentException (nameof (cornerWidth));
			if ((cornerHeight < 0) || (2 * cornerHeight > rectangle.Height))
				throw new ArgumentException (nameof (cornerHeight));
			return MakeMutable (CGPathCreateWithRoundedRect (rectangle, cornerWidth, cornerHeight, transform), true);
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		static unsafe public CGPath FromRoundedRect (CGRect rectangle, nfloat cornerWidth, nfloat cornerHeight)
		{
			return _FromRoundedRect (rectangle, cornerWidth, cornerHeight, null);
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		static public unsafe CGPath FromRoundedRect (CGRect rectangle, nfloat cornerWidth, nfloat cornerHeight, CGAffineTransform transform)
		{
			return _FromRoundedRect (rectangle, cornerWidth, cornerHeight, &transform);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static void CGPathAddRoundedRect (/* CGMutablePathRef */ IntPtr path, CGAffineTransform* transform, CGRect rect, /* CGFloat */ nfloat cornerWidth, /* CGFloat */ nfloat cornerHeight);

#if NET
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public unsafe void AddRoundedRect (CGAffineTransform transform, CGRect rect, nfloat cornerWidth, nfloat cornerHeight)
		{
			CGPathAddRoundedRect (Handle, &transform, rect, cornerWidth, cornerHeight);
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public unsafe void AddRoundedRect (CGRect rect, nfloat cornerWidth, nfloat cornerHeight)
		{
			CGPathAddRoundedRect (Handle, null, rect, cornerWidth, cornerHeight);
		}
#endif // !COREBUILD
	}
}
