using System;
using System.Runtime.InteropServices;
using System.Drawing;

#if XAMCORE_2_0
#if !__WATCHOS__
using CoreAnimation;
#endif
#if !__TVOS__
using MapKit;
#endif
using Foundation;
#else
using MonoTouch.CoreAnimation;
using MonoTouch.MapKit;
using MonoTouch.Foundation;
#endif
using OpenTK;

#if XAMCORE_2_0
using RectangleF=CoreGraphics.CGRect;
using SizeF=CoreGraphics.CGSize;
using PointF=CoreGraphics.CGPoint;
#else
using nfloat=global::System.Single;
using nint=global::System.Int32;
using nuint=global::System.UInt32;
#endif

#if XAMCORE_2_0
namespace ObjCRuntime
#else
namespace MonoTouch.ObjCRuntime
#endif
{
	public static class Messaging
	{
		internal const string LIBOBJC_DYLIB = "/usr/lib/libobjc.dylib";

		public struct objc_super {
			public IntPtr Handle;
			public IntPtr SuperHandle;
		}

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static void void_objc_msgSend_IntPtr_PointF_ref_PointF (IntPtr receiver, IntPtr selector, IntPtr scrollView, PointF velocity, ref PointF targetContentOffset);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static void void_objc_msgSendSuper (ref objc_super receiver, IntPtr selector);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static void void_objc_msgSend_IntPtr (IntPtr receiver, IntPtr selector, IntPtr value);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static void void_objc_msgSend_ref_IntPtr (IntPtr receiver, IntPtr selector, ref IntPtr value);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static void void_objc_msgSend_out_IntPtr (IntPtr receiver, IntPtr selector, out IntPtr value);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static void void_objc_msgSend_IntPtr_IntPtr (IntPtr receiver, IntPtr selector, IntPtr p1, IntPtr p2);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static nint nint_objc_msgSend_IntPtr_nint (IntPtr receiver, IntPtr selector, IntPtr p1, nint p2);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static void void_objc_msgSend_IntPtr_ref_BlockLiteral (IntPtr receiver, IntPtr selector, IntPtr p1, ref BlockLiteral p2);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static void void_objc_msgSend_IntPtr_IntPtr_IntPtr_IntPtr (IntPtr receiver, IntPtr selector, IntPtr p1, IntPtr p2, IntPtr p3, IntPtr p4);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static void void_objc_msgSend_IntPtr_IntPtr_IntPtr_NSRange_IntPtr (IntPtr receiver, IntPtr selector, IntPtr p1, IntPtr p2, IntPtr p3, NSRange p4, IntPtr p5);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static void void_objc_msgSend_int (IntPtr receiver, IntPtr selector, int value);

		[DllImport (LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
		public extern static void void_objc_msgSend_int_int_int_int_int_int_IntPtr (IntPtr receiver, IntPtr selector, int p1, int p2, int p3, int p4, int p5, int p6, IntPtr p7);

		[DllImport (LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
		public extern static void void_objc_msgSend_IntPtr_IntPtr_IntPtr_long_int_IntPtr (IntPtr receiver, IntPtr selector, IntPtr p1, IntPtr p2, IntPtr p3, long p4, int p5, IntPtr p7);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static void void_objc_msgSend_long (IntPtr receiver, IntPtr selector, long value);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static void void_objc_msgSend_int_int_long (IntPtr receiver, IntPtr selector, int p1, int p2, long p3);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static void void_objc_msgSend_long_int_long (IntPtr receiver, IntPtr selector, long p1, int p2, long p3);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static IntPtr IntPtr_objc_msgSend (IntPtr receiver, IntPtr selector);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static IntPtr IntPtr_objc_msgSend_int (IntPtr receiver, IntPtr selector, int p1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static IntPtr IntPtr_objc_msgSend_long (IntPtr receiver, IntPtr selector, long p1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static int int_objc_msgSend_int (IntPtr receiver, IntPtr selector, int p1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static IntPtr IntPtr_objc_msgSend_ref_IntPtr (IntPtr receiver, IntPtr selector, ref IntPtr p1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static IntPtr IntPtr_objc_msgSend_IntPtr (IntPtr receiver, IntPtr selector, IntPtr p1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static IntPtr IntPtr_objc_msgSend_IntPtr_IntPtr (IntPtr receiver, IntPtr selector, IntPtr p1, IntPtr p2);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static IntPtr IntPtr_objc_msgSend_double_double (IntPtr receiver, IntPtr selector, double a, double b);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static IntPtr IntPtr_objc_msgSend_bool (IntPtr receiver, IntPtr selector, bool p1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static double Double_objc_msgSend (IntPtr receiver, IntPtr selector);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static float float_objc_msgSend (IntPtr receiver, IntPtr selector);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static bool bool_objc_msgSend (IntPtr receiver, IntPtr selector);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static int int_objc_msgSend (IntPtr receiver, IntPtr selector);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static long long_objc_msgSend (IntPtr receiver, IntPtr selector);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static void void_objc_msgSend (IntPtr receiver, IntPtr selector);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static int int_objc_msgSend_IntPtr (IntPtr receiver, IntPtr selector, IntPtr p1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static bool bool_objc_msgSend_IntPtr (IntPtr receiver, IntPtr selector, IntPtr p1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static bool bool_objc_msgSend_IntPtr_int (IntPtr receiver, IntPtr selector, IntPtr p1, int p2);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static IntPtr IntPtr_objc_msgSend_SizeF (IntPtr receiver, IntPtr selector, SizeF p1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static PointF PointF_objc_msgSend (IntPtr receiver, IntPtr selector);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static SizeF SizeF_objc_msgSend (IntPtr receiver, IntPtr selector);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static void void_objc_msgSend_RectangleF (IntPtr receiver, IntPtr selector, RectangleF p1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static RectangleF RectangleF_objc_msgSend (IntPtr receiver, IntPtr selector);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static RectangleF RectangleF_objc_msgSend_int (IntPtr receiver, IntPtr selector, int p1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static RectangleF RectangleF_objc_msgSend_IntPtr (IntPtr receiver, IntPtr selector, IntPtr p1);

#if !__TVOS__
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static RectangleF RectangleF_objc_msgSend_MKCoordinateRegion_IntPtr (IntPtr receiver, IntPtr selector, MKCoordinateRegion p1, IntPtr p2);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static RectangleF RectangleF_objc_msgSend_MKMapRect (IntPtr receiver, IntPtr selector, MKMapRect p1);
#endif // !__TVOS__

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static RectangleF RectangleF_objc_msgSend_RectangleF (IntPtr receiver, IntPtr selector, RectangleF p1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static RectangleF RectangleF_objc_msgSend_RectangleF_int (IntPtr receiver, IntPtr selector, RectangleF p1, int p2);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static RectangleF RectangleF_objc_msgSend_RectangleF_IntPtr (IntPtr receiver, IntPtr selector, RectangleF p1, IntPtr p2);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static RectangleF RectangleF_objc_msgSend_RectangleF_RectangleF_float (IntPtr receiver, IntPtr selector, RectangleF p1, RectangleF p2, float p3);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static RectangleF RectangleF_objc_msgSend_RectangleF_RectangleF_RectangleF (IntPtr receiver, IntPtr selector, RectangleF p1, RectangleF p2, RectangleF p3);

#if !__WATCHOS__
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static Matrix3 Matrix3_objc_msgSend (IntPtr receiver, IntPtr selector);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static CATransform3D CATransform3D_objc_msgSend (IntPtr receiver, IntPtr selector);
#endif // !__WATCHOS__

#if !__TVOS__
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void RectangleF_objc_msgSend_stret_MKMapRect (out RectangleF buf, IntPtr receiver, IntPtr selector, MKMapRect p1);
#endif // !__TVOS__

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void RectangleF_objc_msgSend_stret (out RectangleF buf, IntPtr receiver, IntPtr selector);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void PointF_objc_msgSend_stret (out PointF buf, IntPtr receiver, IntPtr selector);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void SizeF_objc_msgSend_stret (out SizeF buf, IntPtr receiver, IntPtr selector);

#if !__WATCHOS__
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void Matrix3_objc_msgSend_stret (out Matrix3 buf, IntPtr receiver, IntPtr selector);
#endif // !__WATCHOS__

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void RectangleF_objc_msgSend_stret_int (out RectangleF buf, IntPtr receiver, IntPtr selector, int p1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void RectangleF_objc_msgSend_stret_IntPtr (out RectangleF buf, IntPtr receiver, IntPtr selector, IntPtr p1);

#if !__TVOS__
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void RectangleF_objc_msgSend_stret_MKCoordinateRegion_IntPtr (out RectangleF buf, IntPtr receiver, IntPtr selector, MKCoordinateRegion p1, IntPtr p2);
#endif // !__TVOS__

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void RectangleF_objc_msgSend_stret_RectangleF (out RectangleF buf, IntPtr receiver, IntPtr selector, RectangleF p1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void RectangleF_objc_msgSend_stret_RectangleF_int (out RectangleF buf, IntPtr receiver, IntPtr selector, RectangleF p1, int p2);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void RectangleF_objc_msgSend_stret_RectangleF_IntPtr (out RectangleF buf, IntPtr receiver, IntPtr selector, RectangleF p1, IntPtr p2);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void RectangleF_objc_msgSend_stret_RectangleF_RectangleF_float (out RectangleF buf, IntPtr receiver, IntPtr selector, RectangleF p1, RectangleF p2, float p3);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void RectangleF_objc_msgSend_stret_RectangleF_RectangleF_RectangleF (out RectangleF buf, IntPtr receiver, IntPtr selector, RectangleF p1, RectangleF p2, RectangleF p3);

#if !__WATCHOS__
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void CATransform3D_objc_msgSend_stret (out CATransform3D buf, IntPtr receiver, IntPtr selector);
#endif // !__WATCHOS__
	}
}

