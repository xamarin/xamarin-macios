#if IOS || TVOS

using System;
#if !XAMCORE_2_0
using System.Drawing;
#endif
using System.Runtime.InteropServices;

using Foundation;
using CoreGraphics;

namespace ObjCRuntime {
#if !XAMCORE_2_0 || COREBUILD
	public
#endif
	static partial class Messaging {
		internal const string LIBOBJC_DYLIB = "/usr/lib/libobjc.dylib";

#if !XAMCORE_2_0
		/* void returns */
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static void void_objc_msgSend (IntPtr receiver, IntPtr selector);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static void void_objc_msgSend_intptr (IntPtr receiver, IntPtr selector, IntPtr arg1);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static void void_objc_msgSend_bool (IntPtr receiver, IntPtr selector, bool arg1);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static void void_objc_msgSend_rbool (IntPtr receiver, IntPtr selector, ref bool arg1);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static void void_objc_msgSend_rint (IntPtr receiver, IntPtr selector, ref int arg1);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static void void_objc_msgSend_rfloat (IntPtr receiver, IntPtr selector, ref float arg1);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static void void_objc_msgSend_rdouble (IntPtr receiver, IntPtr selector, ref double arg1);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static void void_objc_msgSend_rintptr (IntPtr receiver, IntPtr selector, ref IntPtr arg1);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static void void_objc_msgSend_cgsize (IntPtr receiver, IntPtr selector, CGSize arg1);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static void void_objc_msgSend_cgpoint (IntPtr receiver, IntPtr selector, CGPoint arg1);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static void void_objc_msgSend_cgrect (IntPtr receiver, IntPtr selector, CGRect arg1);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static void void_objc_msgSend_nsrange (IntPtr receiver, IntPtr selector, NSRange arg1);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static void void_objc_msgSend_intptr_int (IntPtr receiver, IntPtr selector, IntPtr arg1, int arg2);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static void void_objc_msgSend_cgpoint_intptr (IntPtr receiver, IntPtr selector, CGPoint arg1, IntPtr arg2);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static void void_objc_msgSend_intptr_intptr_bool (IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2, bool arg3);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static void void_objc_msgSend_intptr_intptr_float (IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2, float arg3);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static void void_objc_msgSend_intptr_intptr_double (IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2, double arg3);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static void void_objc_msgSend_intptr_intptr (IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static void void_objc_msgSendSuper (IntPtr receiver, IntPtr selector);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static void void_objc_msgSendSuper_intptr (IntPtr receiver, IntPtr selector, IntPtr arg1);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static void void_objc_msgSendSuper_cgsize (IntPtr receiver, IntPtr selector, CGSize arg1);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static void void_objc_msgSendSuper_cgrect (IntPtr receiver, IntPtr selector, CGRect arg1);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static void void_objc_msgSendSuper_intptr_intptr_bool (IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2, bool arg3);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static void void_objc_msgSendSuper_intptr_intptr_float (IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2, float arg3);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static void void_objc_msgSendSuper_intptr_intptr_double (IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2, double arg3);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void void_objc_msgSend_stret_rcgsize (ref CGSize stret, IntPtr receiver, IntPtr selector);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void void_objc_msgSend_stret_rcgrect (ref CGRect stret, IntPtr receiver, IntPtr selector);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void void_objc_msgSend_stret_rnsrange (ref NSRange stret, IntPtr receiver, IntPtr selector);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void void_objc_msgSend_stret_rcgsize_cgpoint_intptr (ref CGSize stret, IntPtr receiver, IntPtr selector, CGPoint arg1, IntPtr arg2);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper_stret")]
		public extern static void void_objc_msgSendSuper_stret_rcgrect (ref CGRect stret, IntPtr receiver, IntPtr selector);

		/* intptr returns */
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static IntPtr intptr_objc_msgSend (IntPtr receiver, IntPtr selector);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static IntPtr intptr_objc_msgSend_intptr (IntPtr receiver, IntPtr selector, IntPtr arg1);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static IntPtr intptr_objc_msgSend_float (IntPtr receiver, IntPtr selector, float arg1);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static IntPtr intptr_objc_msgSend_cgrect (IntPtr receiver, IntPtr selector, CGRect arg1);
		
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static IntPtr intptr_objc_msgSendSuper (IntPtr receiver, IntPtr selector);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static IntPtr intptr_objc_msgSendSuper_cgrect (IntPtr receiver, IntPtr selector, CGRect arg1);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static IntPtr intptr_objc_msgSendSuper_intptr (IntPtr receiver, IntPtr selector, IntPtr arg1);

		/* bool returns */
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static bool bool_objc_msgSend (IntPtr receiver, IntPtr selector);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static bool bool_objc_msgSend_intptr (IntPtr receiver, IntPtr selector, IntPtr arg1);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static bool bool_objc_msgSendSuper_intptr (IntPtr receiver, IntPtr selector, IntPtr arg1);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static bool bool_objc_msgSend_int (IntPtr receiver, IntPtr selector, int arg1);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static bool bool_objc_msgSendSuper_int (IntPtr receiver, IntPtr selector, int arg1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static CGSize cgsize_objc_msgSend_cgpoint_intptr (IntPtr receiver, IntPtr selector, CGPoint arg1, IntPtr arg2);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static CGSize cgsize_objc_msgSend (IntPtr receiver, IntPtr selector);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static IntPtr intptr_objc_msgsend_intptr_int (IntPtr receiver, IntPtr selector, IntPtr arg1, int arg2);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static Boolean Boolean_objc_msgSend_IntPtr_Double_IntPtr (IntPtr receiver, IntPtr selector, IntPtr arg1, Double arg2, IntPtr arg3);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static Boolean Boolean_objc_msgSendSuper_IntPtr_Double_IntPtr (IntPtr receiver, IntPtr selector, IntPtr arg1, Double arg2, IntPtr arg3);
		
		// the new generator syntax* for System.Boolean is to use 'bool' not 'Boolean' (as needed for the simulator) but keep the old one for binary compatibility
		// * https://github.com/mono/maccore/commit/2008d9fd84b28e744321d71104680487155b9fa5
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static bool bool_objc_msgSend_IntPtr_Double_IntPtr (IntPtr receiver, IntPtr selector, IntPtr arg1, Double arg2, IntPtr arg3);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void RectangleF_objc_msgSend_stret_IntPtr_IntPtr (out CGRect retval, IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper_stret")]
		public extern static void RectangleF_objc_msgSendSuper_stret_IntPtr_IntPtr (out CGRect retval, IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static void void_objc_msgSend_UInt32_IntPtr (IntPtr receiver, IntPtr selector, uint arg1, IntPtr arg2);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static void void_objc_msgSendSuper_UInt32_IntPtr (IntPtr receiver, IntPtr selector, uint arg1, IntPtr arg2);

		// Compatibility with older monotouch.dll
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static bool bool_objc_msgSend_CMTime_CGAffineTransform_CGAffineTransform_CMTimeRange (IntPtr receiver, IntPtr selector, CoreMedia.CMTime arg1, CoreGraphics.CGAffineTransform arg2, CoreGraphics.CGAffineTransform arg3, CoreMedia.CMTimeRange arg4);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static bool bool_objc_msgSendSuper_CMTime_CGAffineTransform_CGAffineTransform_CMTimeRange (IntPtr receiver, IntPtr selector, CoreMedia.CMTime arg1, CoreGraphics.CGAffineTransform arg2, CoreGraphics.CGAffineTransform arg3, CoreMedia.CMTimeRange arg4);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static bool bool_objc_msgSend_CMTime_CMTime_CMTime (IntPtr receiver, IntPtr selector, CoreMedia.CMTime arg1, CoreMedia.CMTime arg2, CoreMedia.CMTime arg3);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static bool bool_objc_msgSendSuper_CMTime_CMTime_CMTime (IntPtr receiver, IntPtr selector, CoreMedia.CMTime arg1, CoreMedia.CMTime arg2, CoreMedia.CMTime arg3);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static bool bool_objc_msgSend_CMTime_float_float_CMTimeRange (IntPtr receiver, IntPtr selector, CoreMedia.CMTime arg1, float arg2, float arg3, CoreMedia.CMTimeRange arg4);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static bool bool_objc_msgSendSuper_CMTime_float_float_CMTimeRange (IntPtr receiver, IntPtr selector, CoreMedia.CMTime arg1, float arg2, float arg3, CoreMedia.CMTimeRange arg4);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static IntPtr IntPtr_objc_msgSend_int_IntPtr_IntPtr_IntPtr_IntPtr (IntPtr receiver, IntPtr selector, int arg1, IntPtr arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static IntPtr IntPtr_objc_msgSendSuper_int_IntPtr_IntPtr_IntPtr_IntPtr (IntPtr receiver, IntPtr selector, int arg1, IntPtr arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static void void_objc_msgSend_Double_float (IntPtr receiver, IntPtr selector, double arg1, float arg2);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static void void_objc_msgSendSuper_Double_float (IntPtr receiver, IntPtr selector, double arg1, float arg2);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static void void_objc_msgSend_UInt32_UInt32 (IntPtr receiver, IntPtr selector, uint arg1, uint arg2);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static void void_objc_msgSendSuper_UInt32_UInt32 (IntPtr receiver, IntPtr selector, uint arg1, uint arg2);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static bool bool_objc_msgSend_bool_int_IntPtr (System.IntPtr receiver, System.IntPtr selector, bool arg1, int arg2, System.IntPtr arg3);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static bool bool_objc_msgSend_bool_IntPtr (System.IntPtr receiver, System.IntPtr selector, bool arg1, System.IntPtr arg2);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static bool bool_objc_msgSend_bool_IntPtr_IntPtr_IntPtr (System.IntPtr receiver, System.IntPtr selector, bool arg1, System.IntPtr arg2, System.IntPtr arg3, System.IntPtr arg4);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static bool bool_objc_msgSend_CMTimeRange_IntPtr_CMTime_IntPtr (System.IntPtr receiver, System.IntPtr selector, MonoTouch.CoreMedia.CMTimeRange arg1, System.IntPtr arg2, MonoTouch.CoreMedia.CMTime arg3, System.IntPtr arg4);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static bool bool_objc_msgSend_float_IntPtr (System.IntPtr receiver, System.IntPtr selector, float arg1, System.IntPtr arg2);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static bool bool_objc_msgSend_IntPtr_bool_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, bool arg2, System.IntPtr arg3);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static bool bool_objc_msgSend_IntPtr_bool_IntPtr_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, bool arg2, System.IntPtr arg3, System.IntPtr arg4);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static bool bool_objc_msgSend_IntPtr_int_bool_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, int arg2, bool arg3, System.IntPtr arg4);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static bool bool_objc_msgSend_IntPtr_int_IntPtr_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, int arg2, System.IntPtr arg3, System.IntPtr arg4);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static bool bool_objc_msgSend_IntPtr_IntPtr_CMTime_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, System.IntPtr arg2, MonoTouch.CoreMedia.CMTime arg3, System.IntPtr arg4);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static bool bool_objc_msgSend_IntPtr_IntPtr_int_IntPtr_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, System.IntPtr arg2, int arg3, System.IntPtr arg4, System.IntPtr arg5);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static bool bool_objc_msgSend_IntPtr_IntPtr_IntPtr_int_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, System.IntPtr arg2, System.IntPtr arg3, int arg4, System.IntPtr arg5);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static bool bool_objc_msgSend_IntPtr_IntPtr_IntPtr_int_IntPtr_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, System.IntPtr arg2, System.IntPtr arg3, int arg4, System.IntPtr arg5, System.IntPtr arg6);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static bool bool_objc_msgSend_IntPtr_IntPtr_IntPtr_IntPtr_IntPtr_IntPtr_IntPtr_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, System.IntPtr arg2, System.IntPtr arg3, System.IntPtr arg4, System.IntPtr arg5, System.IntPtr arg6, System.IntPtr arg7, System.IntPtr arg8);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static bool bool_objc_msgSendSuper_bool_int_IntPtr (System.IntPtr receiver, System.IntPtr selector, bool arg1, int arg2, System.IntPtr arg3);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static bool bool_objc_msgSendSuper_bool_IntPtr (System.IntPtr receiver, System.IntPtr selector, bool arg1, System.IntPtr arg2);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static bool bool_objc_msgSendSuper_bool_IntPtr_IntPtr_IntPtr (System.IntPtr receiver, System.IntPtr selector, bool arg1, System.IntPtr arg2, System.IntPtr arg3, System.IntPtr arg4);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static bool bool_objc_msgSendSuper_CMTimeRange_IntPtr_CMTime_IntPtr (System.IntPtr receiver, System.IntPtr selector, MonoTouch.CoreMedia.CMTimeRange arg1, System.IntPtr arg2, MonoTouch.CoreMedia.CMTime arg3, System.IntPtr arg4);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static bool bool_objc_msgSendSuper_float_IntPtr (System.IntPtr receiver, System.IntPtr selector, float arg1, System.IntPtr arg2);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static bool bool_objc_msgSendSuper_IntPtr_bool_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, bool arg2, System.IntPtr arg3);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static bool bool_objc_msgSendSuper_IntPtr_bool_IntPtr_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, bool arg2, System.IntPtr arg3, System.IntPtr arg4);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static bool bool_objc_msgSendSuper_IntPtr_int_bool_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, int arg2, bool arg3, System.IntPtr arg4);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static bool bool_objc_msgSendSuper_IntPtr_int_IntPtr_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, int arg2, System.IntPtr arg3, System.IntPtr arg4);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static bool bool_objc_msgSendSuper_IntPtr_IntPtr_CMTime_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, System.IntPtr arg2, MonoTouch.CoreMedia.CMTime arg3, System.IntPtr arg4);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static bool bool_objc_msgSendSuper_IntPtr_IntPtr_int_IntPtr_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, System.IntPtr arg2, int arg3, System.IntPtr arg4, System.IntPtr arg5);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static bool bool_objc_msgSendSuper_IntPtr_IntPtr_IntPtr_int_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, System.IntPtr arg2, System.IntPtr arg3, int arg4, System.IntPtr arg5);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static bool bool_objc_msgSendSuper_IntPtr_IntPtr_IntPtr_int_IntPtr_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, System.IntPtr arg2, System.IntPtr arg3, int arg4, System.IntPtr arg5, System.IntPtr arg6);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static bool bool_objc_msgSendSuper_IntPtr_IntPtr_IntPtr_IntPtr_IntPtr_IntPtr_IntPtr_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, System.IntPtr arg2, System.IntPtr arg3, System.IntPtr arg4, System.IntPtr arg5, System.IntPtr arg6, System.IntPtr arg7, System.IntPtr arg8);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static int int_objc_msgSend_IntPtr_IntPtr_int_int_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, System.IntPtr arg2, int arg3, int arg4, System.IntPtr arg5);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static int int_objc_msgSend_IntPtr_IntPtr_int_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, System.IntPtr arg2, int arg3, System.IntPtr arg4);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static int int_objc_msgSendSuper_IntPtr_IntPtr_int_int_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, System.IntPtr arg2, int arg3, int arg4, System.IntPtr arg5);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static int int_objc_msgSendSuper_IntPtr_IntPtr_int_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, System.IntPtr arg2, int arg3, System.IntPtr arg4);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static System.IntPtr IntPtr_objc_msgSend_CMTime_out_CMTime_IntPtr (System.IntPtr receiver, System.IntPtr selector, MonoTouch.CoreMedia.CMTime arg1, out MonoTouch.CoreMedia.CMTime arg2, System.IntPtr arg3);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static System.IntPtr IntPtr_objc_msgSend_int_int_IntPtr_bool_IntPtr (System.IntPtr receiver, System.IntPtr selector, int arg1, int arg2, System.IntPtr arg3, bool arg4, System.IntPtr arg5);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static System.IntPtr IntPtr_objc_msgSend_int_IntPtr_IntPtr_IntPtr (System.IntPtr receiver, System.IntPtr selector, int arg1, System.IntPtr arg2, System.IntPtr arg3, System.IntPtr arg4);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static System.IntPtr IntPtr_objc_msgSend_int_IntPtr_ref_NSRange_ref_NSRange_IntPtr (System.IntPtr receiver, System.IntPtr selector, int arg1, System.IntPtr arg2, ref MonoTouch.Foundation.NSRange arg3, ref MonoTouch.Foundation.NSRange arg4, System.IntPtr arg5);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static System.IntPtr IntPtr_objc_msgSend_IntPtr_int_IntPtr_out_Boolean_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, int arg2, System.IntPtr arg3, out bool arg4, System.IntPtr arg5);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static System.IntPtr IntPtr_objc_msgSend_IntPtr_int_ref_NSPropertyListFormat_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, int arg2, ref MonoTouch.Foundation.NSPropertyListFormat arg3, System.IntPtr arg4);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static System.IntPtr IntPtr_objc_msgSend_IntPtr_IntPtr_IntPtr_IntPtr_IntPtr_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, System.IntPtr arg2, System.IntPtr arg3, System.IntPtr arg4, System.IntPtr arg5, System.IntPtr arg6);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static System.IntPtr IntPtr_objc_msgSend_IntPtr_RectangleF_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, System.Drawing.RectangleF arg2, System.IntPtr arg3);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static System.IntPtr IntPtr_objc_msgSend_NSRange_IntPtr_int_IntPtr (System.IntPtr receiver, System.IntPtr selector, MonoTouch.Foundation.NSRange arg1, System.IntPtr arg2, int arg3, System.IntPtr arg4);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static System.IntPtr IntPtr_objc_msgSendSuper_CMTime_out_CMTime_IntPtr (System.IntPtr receiver, System.IntPtr selector, MonoTouch.CoreMedia.CMTime arg1, out MonoTouch.CoreMedia.CMTime arg2, System.IntPtr arg3);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static System.IntPtr IntPtr_objc_msgSendSuper_int_int_IntPtr_bool_IntPtr (System.IntPtr receiver, System.IntPtr selector, int arg1, int arg2, System.IntPtr arg3, bool arg4, System.IntPtr arg5);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static System.IntPtr IntPtr_objc_msgSendSuper_int_IntPtr_IntPtr_IntPtr (System.IntPtr receiver, System.IntPtr selector, int arg1, System.IntPtr arg2, System.IntPtr arg3, System.IntPtr arg4);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static System.IntPtr IntPtr_objc_msgSendSuper_int_IntPtr_ref_NSRange_ref_NSRange_IntPtr (System.IntPtr receiver, System.IntPtr selector, int arg1, System.IntPtr arg2, ref MonoTouch.Foundation.NSRange arg3, ref MonoTouch.Foundation.NSRange arg4, System.IntPtr arg5);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static System.IntPtr IntPtr_objc_msgSendSuper_IntPtr_int_IntPtr_out_Boolean_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, int arg2, System.IntPtr arg3, out bool arg4, System.IntPtr arg5);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static System.IntPtr IntPtr_objc_msgSendSuper_IntPtr_int_ref_NSPropertyListFormat_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, int arg2, ref MonoTouch.Foundation.NSPropertyListFormat arg3, System.IntPtr arg4);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static System.IntPtr IntPtr_objc_msgSendSuper_IntPtr_IntPtr_IntPtr_IntPtr_IntPtr_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, System.IntPtr arg2, System.IntPtr arg3, System.IntPtr arg4, System.IntPtr arg5, System.IntPtr arg6);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static System.IntPtr IntPtr_objc_msgSendSuper_IntPtr_RectangleF_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, System.Drawing.RectangleF arg2, System.IntPtr arg3);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static System.IntPtr IntPtr_objc_msgSendSuper_NSRange_IntPtr_int_IntPtr (System.IntPtr receiver, System.IntPtr selector, MonoTouch.Foundation.NSRange arg1, System.IntPtr arg2, int arg3, System.IntPtr arg4);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static void RectangleF_objc_msgSend_stret_IntPtr_IntPtr_IntPtr (out System.Drawing.RectangleF retval, System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, System.IntPtr arg2, System.IntPtr arg3);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static void RectangleF_objc_msgSendSuper_stret_IntPtr_IntPtr_IntPtr (out System.Drawing.RectangleF retval, System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, System.IntPtr arg2, System.IntPtr arg3);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static uint UInt32_objc_msgSend_IntPtr_Int64_UInt32_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, long arg2, uint arg3, System.IntPtr arg4);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static uint UInt32_objc_msgSend_IntPtr_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, System.IntPtr arg2);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static uint UInt32_objc_msgSendSuper_IntPtr_Int64_UInt32_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, long arg2, uint arg3, System.IntPtr arg4);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static uint UInt32_objc_msgSendSuper_IntPtr_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, System.IntPtr arg2);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static void void_objc_msgSend_IntPtr_int_IntPtr_int_IntPtr_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, int arg2, System.IntPtr arg3, int arg4, System.IntPtr arg5, System.IntPtr arg6);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static void void_objc_msgSend_IntPtr_int_IntPtr_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, int arg2, System.IntPtr arg3, System.IntPtr arg4);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static void void_objc_msgSend_IntPtr_ref_RectangleF_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, ref System.Drawing.RectangleF arg2, System.IntPtr arg3);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static void void_objc_msgSendSuper_IntPtr_int_IntPtr_int_IntPtr_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, int arg2, System.IntPtr arg3, int arg4, System.IntPtr arg5, System.IntPtr arg6);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static void void_objc_msgSendSuper_IntPtr_int_IntPtr_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, int arg2, System.IntPtr arg3, System.IntPtr arg4);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static void void_objc_msgSendSuper_IntPtr_ref_RectangleF_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, ref System.Drawing.RectangleF arg2, System.IntPtr arg3);

		[DllImport ("__Internal", EntryPoint="xamarin_IntPtr_objc_msgSend_IntPtr")]
		public extern static IntPtr monotouch_IntPtr_objc_msgSend_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1);
		[DllImport ("__Internal", EntryPoint="xamarin_IntPtr_objc_msgSendSuper_IntPtr")]
		public extern static IntPtr monotouch_IntPtr_objc_msgSendSuper_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static bool bool_objc_msgSend_IntPtr_IntPtr_UInt32_UInt32_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, System.IntPtr arg2, uint arg3, uint arg4, System.IntPtr arg5);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static bool bool_objc_msgSendSuper_IntPtr_IntPtr_UInt32_UInt32_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, System.IntPtr arg2, uint arg3, uint arg4, System.IntPtr arg5);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void Vector3_objc_msgSend_stret_Vector3 (out OpenTK.Vector3 retval, IntPtr receiver, IntPtr selector, OpenTK.Vector3 arg1);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void Vector3_objc_msgSendSuper_stret_Vector3 (out OpenTK.Vector3 retval, IntPtr receiver, IntPtr selector, OpenTK.Vector3 arg1);

		[DllImport ("__Internal", EntryPoint="xamarin_vector_float3__Vector3_objc_msgSend_stret_Vector3")]
		public extern static void xamarin_vector_float3__Vector3_objc_msgSend_stret_Vector3 (out global::OpenTK.Vector3 retval, IntPtr receiver, IntPtr selector, global::OpenTK.Vector3 arg1);
		[DllImport ("__Internal", EntryPoint="xamarin_vector_float3__Vector3_objc_msgSendSuper_stret_Vector3")]
		public extern static void xamarin_vector_float3__Vector3_objc_msgSendSuper_stret_Vector3 (out global::OpenTK.Vector3 retval, IntPtr receiver, IntPtr selector, global::OpenTK.Vector3 arg1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static void void_objc_msgSendSuper_IntPtr_RectangleF_IntPtr (IntPtr receiver, IntPtr selector, IntPtr arg1, System.Drawing.RectangleF arg2, IntPtr arg3);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static void void_objc_msgSend_IntPtr_RectangleF_IntPtr (IntPtr receiver, IntPtr selector, IntPtr arg1, System.Drawing.RectangleF arg2, IntPtr arg3);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static IntPtr IntPtr_objc_msgSend_IntPtr_nint (IntPtr receiver, IntPtr selector, global::System.IntPtr arg1, nint arg2);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static IntPtr IntPtr_objc_msgSendSuper_IntPtr_nint (IntPtr receiver, IntPtr selector, global::System.IntPtr arg1, nint arg2);
#endif
	}
}

#endif // IOS || TVOS
