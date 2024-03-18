using System;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
#if !__TVOS__
using MapKit;
#endif
#if !__WATCHOS__
using CoreAnimation;
#endif
using CoreLocation;
#if !__WATCHOS__
using CoreMedia;
#endif
using NUnit.Framework;

#if NET
using System.Numerics;
#else
using OpenTK;
#endif

namespace MonoTouchFixtures.ObjCRuntime {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class TrampolineTest {
		public static readonly nfloat pi = 3.14159f;
#if MONOMAC || __MACCATALYST__
		public static bool IsX64 { get { return IntPtr.Size == 8 && !IsArm64CallingConvention; } }
		public static bool IsX86 { get { return IntPtr.Size == 4; } }
#else
		public static bool IsX64 { get { return IntPtr.Size == 8 && Runtime.Arch == Arch.SIMULATOR && !IsArm64CallingConvention; } }
		public static bool IsX86 { get { return IntPtr.Size == 4 && Runtime.Arch == Arch.SIMULATOR; } }
#endif
		public static bool IsArm64 { get { return IntPtr.Size == 8 && IsArm64CallingConvention; } }
		public static bool IsArm32 {
			get {
#if __WATCHOS__ || __MACOS__ || __MACCATALYST__
				return false;
#else
				return IntPtr.Size == 4 && Runtime.Arch == Arch.DEVICE;
#endif
			}
		}

		public static bool IsArm64CallingConvention {
			get {
				return Runtime.IsARM64CallingConvention;
			}
		}

		public static bool IsArmv7k {
			get {
#if __WATCHOS__
				return Runtime.Arch == Arch.DEVICE && !IsArm64CallingConvention;
#else
				return false;
#endif
			}
		}

#if !__WATCHOS__
		[Test]
		public void StretTrampolineTest ()
		{
			CMTimeRange tr;
			using (var obj = new StretTrampolines ()) {
				if (IsArm64CallingConvention) {
					tr = CMTimeRange_objc_msgSend (obj.Handle, Selector.GetHandle ("myTimeRange"));
				} else {
					CMTimeRange_objc_msgSend (out tr, obj.Handle, Selector.GetHandle ("myTimeRange"));
				}
			}
			Assert.AreEqual (12, tr.Duration.Value);
			Assert.AreEqual (1, tr.Duration.TimeScale);
			Assert.AreEqual (1, tr.Start.Value);
			Assert.AreEqual (1, tr.Start.TimeScale);
		}
#endif // !__WATCHOS__

		[Test]
		public void DoubleReturnTest ()
		{
			DoubleStretTrampolines obj = new DoubleStretTrampolines ();
			IntPtr class_ptr = Class.GetHandle ("DoubleStretTrampolines");

			Assert.That (0 == Messaging.Double_objc_msgSend (obj.Handle, new Selector ("Test_Zero").Handle), "#1");
			Assert.That (0 == Messaging.Double_objc_msgSend (class_ptr, new Selector ("Test_StaticZero").Handle), "#2");
			Assert.That (0 == Messaging.Double_objc_msgSend (obj.Handle, new Selector ("Test_ZeroProperty").Handle), "#3");
			Assert.That (0 == Messaging.Double_objc_msgSend (class_ptr, new Selector ("Test_StaticZeroProperty").Handle), "#4");
		}

		[Test]
		public void FloatReturnTest ()
		{
			FloatStretTrampolines obj = new FloatStretTrampolines ();
			IntPtr class_ptr = Class.GetHandle ("FloatStretTrampolines");

			Assert.That (0 == Messaging.float_objc_msgSend (obj.Handle, new Selector ("Test_Zero").Handle), "#1");
			Assert.That (0 == Messaging.float_objc_msgSend (class_ptr, new Selector ("Test_StaticZero").Handle), "#2");
			Assert.That (0 == Messaging.float_objc_msgSend (obj.Handle, new Selector ("Test_ZeroProperty").Handle), "#3");
			Assert.That (0 == Messaging.float_objc_msgSend (class_ptr, new Selector ("Test_StaticZeroProperty").Handle), "#4");
		}

		[Test]
		public void LoooongTest ()
		{
			LongTrampolines obj = new LongTrampolines ();
			long a = 3;
			long b = 5;
			long c = 15;
			long d = 29;
			long e;

			void_objc_msgSend_long_long_ref_long_ref_long (obj.Handle, new Selector ("ManyLongs:b:c:d:").Handle, a, b, out c, out d);
			Assert.That (c == a + b, "#a1");
			Assert.That (d == b - a, "#a2");

			c = 15;
			d = 29;

			e = long_objc_msgSend_long_long_out_long_out_long (obj.Handle, new Selector ("VeryManyLongs:b:c:d:").Handle, a, b, out c, out d);
			Assert.That (a + b + c + d == e, "#b1");
			Assert.That (c == a + b, "#b2");
			Assert.That (d == a - b, "#b3");
		}

		const string LIBOBJC_DYLIB = "/usr/lib/libobjc.dylib";

#if !__WATCHOS__ && !NET
		[DllImport (LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
		extern static OpenTK.Matrix4 Matrix4_objc_msgSend (IntPtr receiver, IntPtr selector);

		[DllImport (LIBOBJC_DYLIB, EntryPoint = "objc_msgSend_stret")]
		extern static void Matrix4_objc_msgSend_stret (out OpenTK.Matrix4 retval, IntPtr receiver, IntPtr selector);
#endif // !__WATCHOS__ && !NET

		[DllImport (LIBOBJC_DYLIB, EntryPoint = "objc_msgSend_stret")]
		extern static void double_objc_msgSend_stret_out_double (out double retval, IntPtr receiver, IntPtr selector, out double arg1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint = "objc_msgSend_stret")]
		extern static void double_objc_msgSend_stret_out_float (out double retval, IntPtr receiver, IntPtr selector, out float arg1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint = "objc_msgSend_stret")]
		extern static void float_objc_msgSend_stret_out_double (out float retval, IntPtr receiver, IntPtr selector, out double arg1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint = "objc_msgSend_stret")]
		extern static void float_objc_msgSend_stret_out_float (out float retval, IntPtr receiver, IntPtr selector, out float arg1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint = "objc_msgSend_stret")]
		extern static void CGPoint_objc_msgSend_stret_out_double (out CGPoint retval, IntPtr receiver, IntPtr selector, out double arg1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint = "objc_msgSend_stret")]
		extern static void CGPoint_objc_msgSend_stret_out_float (out CGPoint retval, IntPtr receiver, IntPtr selector, out float arg1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint = "objc_msgSend_stret")]
		extern static void CGRect_objc_msgSend_stret_IntPtr_IntPtr_CGRect (out CGRect retval, IntPtr receiver, IntPtr selector, IntPtr a, IntPtr b, CGRect c);

		[DllImport (LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
		extern static CGRect CGRect_objc_msgSend_IntPtr_IntPtr_CGRect (IntPtr receiver, IntPtr selector, IntPtr a, IntPtr b, CGRect c);

		[DllImport (LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
		extern static void void_objc_msgSend_out_NSError (IntPtr receiver, IntPtr selector, out IntPtr error, int arg1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
		extern static void void_objc_msgSend_long_long_ref_long_ref_long (IntPtr receiver, IntPtr selector, long arg1, long arg2, out long arg3, out long arg4);

		[DllImport (LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
		extern static long long_objc_msgSend_long_long_out_long_out_long (IntPtr receiver, IntPtr selector, long arg1, long arg2, out long arg3, out long arg4);

		[DllImport (LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
		extern static bool bool_objc_msgSend_out_CGPoint (IntPtr receiver, IntPtr selector, out CGPoint point);


#if !__WATCHOS__
		[DllImport (LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
		extern static CMTimeRange CMTimeRange_objc_msgSend (IntPtr receiver, IntPtr selector);

		[DllImport (LIBOBJC_DYLIB, EntryPoint = "objc_msgSend_stret")]
		extern static void CMTimeRange_objc_msgSend (out CMTimeRange retval, IntPtr receiver, IntPtr selector);
#endif // !__WATCHOS__

		void AreAlmostEqual (CGRect left, CGRect right, string message)
		{
			var delta = 0.000001f;
			Assert.AreEqual (left.X, right.X, delta, message);
			Assert.AreEqual (left.Y, right.Y, delta, message);
			Assert.AreEqual (left.Width, right.Width, delta, message);
			Assert.AreEqual (left.Height, right.Height, delta, message);
		}

		[Test]
		public void FloatingPointStretTrampolineTest ()
		{
			CGRect rect, rect2, rect3, rect4;
#if !__TVOS__
			MKCoordinateRegion mkregion;
			MKMapRect mkmaprect;
#endif
			FloatingPointStretTrampolines obj = new FloatingPointStretTrampolines ();
			IntPtr class_ptr = Class.GetHandle ("FloatingPointStretTrampolines");
			NSString tmp_obj = obj.StringObj;
#if !__WATCHOS__ && !NET
			Matrix3 matrix3;
			Matrix4 matrix4;
			CATransform3D catransform3d;
#endif // !__WATCHOS__ && !NET
			int i;

			rect2 = new CGRect (1.2f, 2.3f, 3.4f, 4.5f);
			if (IsArm64CallingConvention || IsArmv7k) {
				rect = CGRect_objc_msgSend_IntPtr_IntPtr_CGRect (obj.Handle, new Selector ("testCGRect_string_string_CGRect:b:c:").Handle, new NSString ("a").Handle, new NSString ("b").Handle, rect2);
			} else {
				CGRect_objc_msgSend_stret_IntPtr_IntPtr_CGRect (out rect, obj.Handle, new Selector ("testCGRect_string_string_CGRect:b:c:").Handle, new NSString ("a").Handle, new NSString ("b").Handle, rect2);
			}
			Assert.That (rect == new CGRect (rect2.X * pi, rect2.Y * pi, rect2.Width * pi, rect2.Height * pi), "#testCGRect_string_string_CGRect:b:c:");

			if (IsArm64CallingConvention || IsArmv7k) {
				rect = Messaging.CGRect_objc_msgSend (obj.Handle, new Selector ("testCGRect").Handle);
			} else {
				Messaging.CGRect_objc_msgSend_stret (out rect, obj.Handle, new Selector ("testCGRect").Handle);
			}
			Assert.That (rect == new CGRect (1.2f, 2.3f, 3.4f, 4.5f), "#testCGRect");

			if (IsArm64CallingConvention || IsArmv7k) {
				rect = Messaging.CGRect_objc_msgSend (class_ptr, new Selector ("staticTestCGRect").Handle);
			} else {
				Messaging.CGRect_objc_msgSend_stret (out rect, class_ptr, new Selector ("staticTestCGRect").Handle);
			}
			Assert.That (rect == new CGRect (1.2f, 2.3f, 3.4f, 4.5f), "#static testCGRect");

			i = 152;
			if (IsArm64CallingConvention || IsArmv7k) {
				rect = Messaging.CGRect_objc_msgSend_int (obj.Handle, new Selector ("testCGRect_int:").Handle, 152);
			} else {
				Messaging.CGRect_objc_msgSend_stret_int (out rect, obj.Handle, new Selector ("testCGRect_int:").Handle, 152);
			}
			Assert.That (rect == new CGRect (i + .1f, i + .2f, i + .3f, i + .4f), "#ret RectF-int", "#testCGRect_int:");

			if (IsArm64CallingConvention || IsArmv7k) {
				rect = Messaging.CGRect_objc_msgSend_IntPtr (obj.Handle, new Selector ("testCGRect_IntPtr:").Handle, tmp_obj.Handle);
			} else {
				Messaging.CGRect_objc_msgSend_stret_IntPtr (out rect, obj.Handle, new Selector ("testCGRect_IntPtr:").Handle, tmp_obj.Handle);
			}
			AreAlmostEqual (rect, new CGRect (pi + 0.4f, pi + 0.3f, pi + 0.2f, pi + 0.1f), "#ret RectF-IntPtr");

#if !__TVOS__
			mkregion = new MKCoordinateRegion (new CLLocationCoordinate2D (123.456, 345.678), new MKCoordinateSpan (987.654, 654.321));
			if (IsArm64CallingConvention || IsArmv7k) {
				rect = Messaging.CGRect_objc_msgSend_MKCoordinateRegion_IntPtr (obj.Handle, new Selector ("testCGRect_MCCoordinateRegion_IntPtr:str:").Handle, mkregion, tmp_obj.Handle);
			} else {
				Messaging.CGRect_objc_msgSend_stret_MKCoordinateRegion_IntPtr (out rect, obj.Handle, new Selector ("testCGRect_MCCoordinateRegion_IntPtr:str:").Handle, mkregion, tmp_obj.Handle);
			}
			Assert.That (rect == new CGRect (123.456f + pi, 345.678f - pi, 987.654f * pi, 654.321f / pi), "#testCGRect_MCCoordinateRegion_IntPtr:str:");

			mkmaprect = new MKMapRect (111.1, 222.2, 333.3, 444.4);
			if (IsArm64CallingConvention || IsArmv7k) {
				rect = Messaging.CGRect_objc_msgSend_MKMapRect (obj.Handle, new Selector ("testCGRect_MKMapRect:").Handle, mkmaprect);
			} else {
				Messaging.CGRect_objc_msgSend_stret_MKMapRect (out rect, obj.Handle, new Selector ("testCGRect_MKMapRect:").Handle, mkmaprect);
			}
			Assert.That (rect == new CGRect (111.1f, 222.2f, 333.3f, 444.4f), "#testCGRect_MKMapRect:");
#endif // !__TVOS__

			rect2 = new CGRect (9.9f, 8.8f, 7.7f, 6.6f);
			if (IsArm64CallingConvention || IsArmv7k) {
				rect = Messaging.CGRect_objc_msgSend_CGRect (obj.Handle, new Selector ("testCGRect_CGRect:").Handle, rect2);
			} else {
				Messaging.CGRect_objc_msgSend_stret_CGRect (out rect, obj.Handle, new Selector ("testCGRect_CGRect:").Handle, rect2);
			}
			Assert.That (rect == rect2, "#testCGRect_CGRect:");

			rect2 = new CGRect (5.44f, 4.33f, 3.22f, 2.11f);
			i = 8;
			if (IsArm64CallingConvention || IsArmv7k) {
				rect = Messaging.CGRect_objc_msgSend_CGRect_int (obj.Handle, new Selector ("testCGRect_CGRect_int:i:").Handle, rect2, 8);
			} else {
				Messaging.CGRect_objc_msgSend_stret_CGRect_int (out rect, obj.Handle, new Selector ("testCGRect_CGRect_int:i:").Handle, rect2, 8);
			}
			AreAlmostEqual (rect, new CGRect (5.44f * i, 4.33f + i, 3.22f - i, 2.11f / i), "testCGRect_CGRect_int:i:");

			rect2 = new CGRect (5.4f, 4.3f, 3.2f, 2.1f);
			if (IsArm64CallingConvention || IsArmv7k) {
				rect = Messaging.CGRect_objc_msgSend_CGRect_IntPtr (obj.Handle, new Selector ("testCGRect_CGRect_IntPtr:str:").Handle, rect2, tmp_obj.Handle);
			} else {
				Messaging.CGRect_objc_msgSend_stret_CGRect_IntPtr (out rect, obj.Handle, new Selector ("testCGRect_CGRect_IntPtr:str:").Handle, rect2, tmp_obj.Handle);
			}
			Assert.That (rect == new CGRect (5.4f * pi, 4.3f + pi, 3.2f - pi, 2.1f / pi));

			rect2 = new CGRect (1, 2, 3, 4);
			rect3 = new CGRect (9, 8, 7, 6);
			if (IsArm64CallingConvention || IsArmv7k) {
				rect = Messaging.CGRect_objc_msgSend_CGRect_CGRect_float (obj.Handle, new Selector ("testCGRect_CGRect_CGRect_float:b:c:").Handle, rect2, rect3, (float) pi);
			} else {
				Messaging.CGRect_objc_msgSend_stret_CGRect_CGRect_float (out rect, obj.Handle, new Selector ("testCGRect_CGRect_CGRect_float:b:c:").Handle, rect2, rect3, (float) pi);
			}
			Assert.That (rect == new CGRect (1 * 9 * pi, 2 * 8 * pi, 3 * 7 * pi, 4 * 6 * pi), "#testCGRect_CGRect_CGRect_float:b:c:");

			rect2 = new CGRect (1, 2, 3, 4);
			rect3 = new CGRect (9, 8, 7, 6);
			rect4 = new CGRect (10, 20, 30, 40);
			if (IsArm64CallingConvention || IsArmv7k) {
				rect = Messaging.CGRect_objc_msgSend_CGRect_CGRect_CGRect (obj.Handle, new Selector ("testCGRect_CGRect_CGRect_CGRect:b:c:").Handle, rect2, rect3, rect4);
			} else {
				Messaging.CGRect_objc_msgSend_stret_CGRect_CGRect_CGRect (out rect, obj.Handle, new Selector ("testCGRect_CGRect_CGRect_CGRect:b:c:").Handle, rect2, rect3, rect4);
			}
			Assert.That (rect == new CGRect (20, 30, 40, 50), "#testCGRect_CGRect_CGRect_CGRect:b:c:");

#if !__WATCHOS__ && !NET
			if (IsArm64CallingConvention) {
				matrix3 = Messaging.Matrix3_objc_msgSend (obj.Handle, new Selector ("testMatrix3").Handle);
			} else {
				Messaging.Matrix3_objc_msgSend_stret (out matrix3, obj.Handle, new Selector ("testMatrix3").Handle);
			}
			Assert.That (matrix3.Equals (new Matrix3 (1, 2, 3, 4, 5, 6, 7, 8, 9)), "#testMatrix3");
			if (IsArm64CallingConvention) {
				matrix4 = Matrix4_objc_msgSend (obj.Handle, new Selector ("testMatrix4").Handle);
			} else {
				Matrix4_objc_msgSend_stret (out matrix4, obj.Handle, new Selector ("testMatrix4").Handle);
			}
			Assert.That (matrix4.Equals (new Matrix4 (9, 8, 7, 6, 5, 4, 3, 2, 1, 9, 8, 7, 6, 5, 4, 3)), "#testMatrix4");

			if (IsArm64CallingConvention) {
				catransform3d = Messaging.CATransform3D_objc_msgSend (obj.Handle, new Selector ("testCATransform3D").Handle);
			} else {
				Messaging.CATransform3D_objc_msgSend_stret (out catransform3d, obj.Handle, new Selector ("testCATransform3D").Handle);
			}
			CATransform3D res = new CATransform3D ();
			res.M11 = 11.1f;
			res.M22 = 22.2f;
			res.M33 = 33.3f;
			res.M44 = 44.4f;
			Assert.That (catransform3d.Equals (res), "#testCATransform3D");
#endif // !__WATCHOS__

			CGPoint point;
			CGSize size;

			if (IsArm32) {
				Messaging.CGPoint_objc_msgSend_stret (out point, obj.Handle, new Selector ("testCGPoint").Handle);
			} else {
				point = Messaging.CGPoint_objc_msgSend (obj.Handle, new Selector ("testCGPoint").Handle);
			}
			Assert.That (point == new CGPoint (pi * 2, pi * 20), "#testCGPoint");

			if (IsArm32) {
				Messaging.CGSize_objc_msgSend_stret (out size, obj.Handle, new Selector ("testCGSize").Handle);
			} else {
				size = Messaging.CGSize_objc_msgSend (obj.Handle, new Selector ("testCGSize").Handle);
			}
			Assert.That (size == new CGSize (pi * 3, pi * 30), "#testCGSize");
		}

		[Test]
		public void OutValueTypeTest ()
		{
			var obj = new OutParams ();
			CGPoint point = new CGPoint (3, 14);
			bool res;

			res = bool_objc_msgSend_out_CGPoint (obj.Handle, new Selector ("Test_CGPoint:").Handle, out point);
			Assert.That (res, "#res");
			Assert.That (point.X, Is.EqualTo ((nfloat) 3.1415f), "#x");
			Assert.That (point.Y, Is.EqualTo ((nfloat) 0), "#y");
		}

		[Test]
		public void OutParamTest ()
		{
			var obj = new OutParams ();
			NSError error = new NSError (new NSString ("doomed"), 314);
			IntPtr errorptr = error.Handle;

			void_objc_msgSend_out_NSError (obj.Handle, new Selector ("Test_NSError:arg1:").Handle, out errorptr, 0);
			error = errorptr == IntPtr.Zero ? null : Runtime.GetNSObject<NSError> (errorptr);
			Assert.That (error, Is.Null, "#1");

			void_objc_msgSend_out_NSError (obj.Handle, new Selector ("Test_NSError:arg1:").Handle, out errorptr, 1);
			error = Runtime.GetNSObject<NSError> (errorptr);
			Assert.That (error.Domain, Is.EqualTo ("domain"), "#2 - domain");
			Assert.That (error.Code, Is.EqualTo ((nint) 1), "#3 - code");
		}

		[Test]
		public void ArrayTest ()
		{
			var obj = new ArrayTrampolines ();
			string [] arr = new string [] { "abc" };
			int c;

			c = Messaging.int_objc_msgSend_IntPtr (obj.Handle, new Selector ("Test_StringArray:").Handle, NSArray.FromStrings (arr).Handle);

			Assert.That (c == 1, "#a1");
			Assert.That (arr [0] == "abc"); // array elements aren't copied back out (maybe they should be?)

			arr = NSArray.StringArrayFromHandle (Messaging.IntPtr_objc_msgSend (obj.Handle, new Selector ("Test_StringArrayReturn").Handle));

			Assert.That (arr.Length == 1, "#b1");
			Assert.That (arr [0] == "def", "#b2");

			arr = NSArray.StringArrayFromHandle (Messaging.IntPtr_objc_msgSend (obj.Handle, new Selector ("Test_StringArrayNullReturn").Handle));
			Assert.IsNull (arr, "#c1");

			c = Messaging.int_objc_msgSend_IntPtr (obj.Handle, new Selector ("Test_StringArray:").Handle, IntPtr.Zero);
			Assert.AreEqual (-1, c, "#d1");
		}

		[Test]
		public void IntPtrTest ()
		{
			var obj = new IntPtrTrampolines ();
			bool res;

			res = Messaging.bool_objc_msgSend_IntPtr (obj.Handle, new Selector ("IsZero:").Handle, IntPtr.Zero);
			Assert.That (res, "#1");
			res = Messaging.bool_objc_msgSend_IntPtr (obj.Handle, new Selector ("IsZero:").Handle, new IntPtr (1));
			Assert.That (!res, "#2");
		}

		[Test]
		public void X64ArgumentOverflow ()
		{
			using (var obj = new MiscTrampolines ()) {
				Messaging.void_objc_msgSend_IntPtr_IntPtr_IntPtr_NSRange_IntPtr (obj.Handle, Selector.GetHandle ("x64argumentoverflow:::::"), new IntPtr (1), new IntPtr (2), new IntPtr (3), new NSRange (4, 5), new IntPtr (6));
			}
		}
	}

	[Preserve (AllMembers = true)]
	[Register ("MiscTrampolines")]
	public class MiscTrampolines : NSObject {
		[Export ("x64argumentoverflow:::::")]
		void X64ArgumentOverflow (nint a, nint b, nint c, NSRange overflow, nint d)
		{
			Assert.AreEqual ((nint) 1, a, "1");
			Assert.AreEqual ((nint) 2, b, "2");
			Assert.AreEqual ((nint) 3, c, "3");
			Assert.AreEqual ((nint) 4, overflow.Location, "length");
			Assert.AreEqual ((nint) 5, overflow.Length, "location");
			Assert.AreEqual ((nint) 6, d, "4");
		}
	}

	[Register ("IntPtrTrampolines")]
	[Preserve (AllMembers = true)]
	public class IntPtrTrampolines : NSObject {
		[Export ("IsZero:")]
		bool IsZero (IntPtr foo)
		{
			return foo == IntPtr.Zero;
		}
	}

	[Register ("ArrayTrampolines")]
	[Preserve (AllMembers = true)]
	public class ArrayTrampolines : NSObject {
		[Export ("Test_StringArray:")]
		int Test_StringArray (string [] arr)
		{
			if (arr is null)
				return -1;
			if (arr.Length == 0)
				return 0;
			arr [0] = "def";
			return arr.Length;
		}

		[Export ("Test_StringArrayReturn")]
		string [] Test_StringArrayReturn ()
		{
			return new string [] { "def" };
		}

		[Export ("Test_StringArrayNullReturn")]
		string [] Test_StringArrayNullReturn ()
		{
			return null;
		}
	}

	[Register ("OutParams")]
	[Preserve (AllMembers = true)]
	public class OutParams : NSObject {
		[Export ("Test_NSError:arg1:")]
		void Test_NSError (out NSError error, int arg1)
		{
			switch (arg1) {
			case 0:
				error = null;
				break;
			case 1:
				error = new NSError (new NSString ("domain"), 1);
				break;
			default:
				throw new ArgumentOutOfRangeException ("arg1");
			}
		}
		[Export ("Test_CGPoint:")]
		bool Test_CGPoint (out CGPoint point)
		{
			point = new CGPoint (3.1415f, 0);
			return true;
		}
	}

	[Preserve (AllMembers = true)]
	[Register ("LongTrampolines")]
	public class LongTrampolines : NSObject {
		[Export ("ManyLongs:b:c:d:")]
		void ManyLongs (long a, long b, ref long c, ref long d)
		{
			//Console.WriteLine ("a: {0} b: {1} c: {2} d: {3}", a, b, c, d);
			c = a + b;
			d = b - a;
		}

		[Export ("VeryManyLongs:b:c:d:")]
		long VeryManyLongs (long a, long b, out long c, out long d)
		{
			c = a + b;
			d = a - b;
			//Console.WriteLine ("a: {0} b: {1} c: {2} d: {3}", a, b, c, d);
			return a + b + c + d;
		}
	}

	[Register ("StretTrampolines")]
	[Preserve (AllMembers = true)]
	public class StretTrampolines : NSObject {
#if !__WATCHOS__
		[Export ("myTimeRange")]
		CMTimeRange TimeRange {
			get {
				var rv = new CMTimeRange () { Duration = new CMTime (12, 1), Start = new CMTime (1, 1) };
				return rv;
			}
		}
#endif // !__WATCHOS__
	}


	[Register ("DoubleStretTrampolines")]
	[Preserve (AllMembers = true)]
	public class DoubleStretTrampolines : NSObject {
		[Export ("Test_Zero")]
		double Test_Zero ()
		{
			return 0;
		}

		[Export ("Test_StaticZero")]
		static double Test_StaticZero ()
		{
			return 0;
		}

		double Test_ZeroProperty {
			[Export ("Test_ZeroProperty")]
			get { return 0; }
		}

		static double Test_StaticZeroProperty {
			[Export ("Test_StaticZeroProperty")]
			get { return 0; }
		}

		[Export ("Test_Double_out_Double:")]
		double Test_Double_out_Double (out double foo)
		{
			foo = 3.16f;
			return 3.16;
		}

		[Export ("Test_StaticDouble_out_Float:")]
		static double Test_StaticDouble_out_Float (out float foo)
		{
			foo = 3.17f;
			return 3.17;
		}
	}

	[Register ("FloatStretTrampolines")]
	[Preserve (AllMembers = true)]
	public class FloatStretTrampolines : NSObject {
		[Export ("Test_Zero")]
		float Test_Zero ()
		{
			return 0;
		}

		[Export ("Test_StaticZero")]
		static float Test_StaticZero ()
		{
			return 0;
		}

		float Test_ZeroProperty {
			[Export ("Test_ZeroProperty")]
			get { return 0; }
		}

		static float Test_StaticZeroProperty {
			[Export ("Test_StaticZeroProperty")]
			get { return 0; }
		}

		[Export ("Test_Float_out_Double:")]
		float Test_Float_out_Double (out double foo)
		{
			foo = 3.18f;
			return 3.18f;
		}

		[Export ("Test_StaticFloat_out_Float:")]
		static float Test_StaticFloat_out_Float (out float foo)
		{
			foo = 3.19f;
			return 3.19f;
		}
	}


	[Register ("FloatingPointStretTrampolines")]
	[Preserve (AllMembers = true)]
	public class FloatingPointStretTrampolines : NSObject {
		const float pi = 3.14159f;
		public NSString StringObj = new NSString ("3.14159");

		static float ParseString (string str)
		{
			return float.Parse (str, CultureInfo.InvariantCulture.NumberFormat);
		}

		[Export ("testCGRect")]
		public CGRect Test_CGRect ()
		{
			return new CGRect (1.2f, 2.3f, 3.4f, 4.5f);
		}

		[Export ("staticTestCGRect")]
		static public CGRect StaticTest_CGRect ()
		{
			return new CGRect (1.2f, 2.3f, 3.4f, 4.5f);
		}

		[Export ("testCGRect_int:")]
		public CGRect Test_CGRect_int (int a)
		{
			return new CGRect (a + 0.1f, a + 0.2f, a + 0.3f, a + 0.4f);
		}

		[Export ("testCGRect_IntPtr:")]
		public CGRect Test_CGRect_IntPtr (NSString str)
		{
			float pi = ParseString (str.ToString ());
			if ((object) StringObj != (object) str)
				return CGRect.Empty;
			return new CGRect (pi + 0.4f, pi + 0.3f, pi + 0.2f, pi + 0.1f);
		}

#if !__TVOS__
		[Export ("testCGRect_MCCoordinateRegion_IntPtr:str:")]
		public CGRect Test_CGRect_MKCoordinateRegion_IntPtr (MKCoordinateRegion a, NSString str)
		{
			float pi = ParseString (str.ToString ());
			if ((object) StringObj != (object) str)
				return CGRect.Empty;
			return new CGRect ((double) (float) a.Center.Latitude + pi, (double) (float) a.Center.Longitude - pi, (double) (float) a.Span.LatitudeDelta * pi, (double) (float) a.Span.LongitudeDelta / pi);
		}

		[Export ("testCGRect_MKMapRect:")]
		public CGRect Test_CGRect_MKMapRect (MKMapRect a)
		{
			return new CGRect ((float) a.Origin.X, (float) a.Origin.Y, (float) a.Width, (float) a.Height);
		}
#endif // !__TVOS__

		[Export ("testCGRect_CGRect:")]
		public CGRect Test_CGRect_CGRect (CGRect a)
		{
			return new CGRect (a.X, a.Y, a.Width, a.Height);
		}

		[Export ("testCGRect_CGRect_int:i:")]
		public CGRect Test_CGRect_CGRect_int (CGRect a, int i)
		{
			return new CGRect (a.X * i, a.Y + i, a.Width - i, a.Height / i);
		}

		[Export ("testCGRect_CGRect_IntPtr:str:")]
		public CGRect Test_CGRect_CGRect_IntPtr (CGRect a, NSString str)
		{
			float pi = ParseString (str.ToString ());
			if ((object) StringObj != (object) str)
				return CGRect.Empty;
			return new CGRect (a.X * pi, a.Y + pi, a.Width - pi, a.Height / pi);
		}

		[Export ("testCGRect_CGRect_CGRect_float:b:c:")]
		public CGRect Test_CGRect_CGRect_CGRect_float (CGRect a, CGRect b, float c)
		{
			return new CGRect (a.X * b.X * c, a.Y * b.Y * c, a.Width * b.Width * c, a.Height * b.Height * c);
		}


		[Export ("testCGRect_CGRect_CGRect_CGRect:b:c:")]
		public CGRect Test_CGRect_CGRect_CGRect_CGRect (CGRect a, CGRect b, CGRect c)
		{
			return new CGRect (a.X + b.X + c.X, a.Y + b.Y + c.Y, a.Width + b.Width + c.Width, a.Height + b.Height + c.Height);
		}

		[Export ("testCGRect_string_string_CGRect:b:c:")]
		public CGRect Test_CGRect_string_string_CGRect (NSString a, NSString b, CGRect c)
		{
			Assert.That (Is.Equals (a.ToString (), "a"), "#a");
			Assert.That (Is.Equals (b.ToString (), "b"), "#b");
			return new CGRect (c.X * pi, c.Y * pi, c.Width * pi, c.Height * pi);
		}

#if !__WATCHOS__
#if !NET
		[Export ("testMatrix3")]
		public Matrix3 Test_Matrix3 ()
		{
			return new Matrix3 (1, 2, 3, 4, 5, 6, 7, 8, 9);
		}

		[Export ("testMatrix4")]
		public Matrix4 Test_Matrix4 ()
		{
			return new Matrix4 (9, 8, 7, 6, 5, 4, 3, 2, 1, 9, 8, 7, 6, 5, 4, 3);
		}
#endif // !NET

		[Export ("testCATransform3D")]
		public CATransform3D Test_CATransform3D ()
		{
			CATransform3D res = new CATransform3D ();
			res.M11 = 11.1f;
			res.M22 = 22.2f;
			res.M33 = 33.3f;
			res.M44 = 44.4f;
			return res;
		}
#endif // !__WATCHOS__

		[Export ("testCGPoint")]
		public CGPoint Test_CGPoint ()
		{
			return new CGPoint (pi * 2, pi * 20);
		}

		[Export ("testCGSize")]
		public CGSize Test_CGSize ()
		{
			return new CGSize (pi * 3, pi * 30);
		}

		[Export ("Test_CGPoint_out_Double:")]
		CGPoint Test_CGPoint_out_Double (out double foo)
		{
			foo = 3.18f;
			return new CGPoint (1, 2);
		}

		[Export ("Test_StaticCGPoint_out_Float:")]
		static CGPoint Test_StaticCGPoint_out_Float (out float foo)
		{
			foo = 3.20f;
			return new CGPoint (10, 20);
		}
	}
}
