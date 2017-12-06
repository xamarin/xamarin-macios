//
// Copyright 2010, Novell, Inc.
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

#if MONOMAC

using System;
using System.Runtime.InteropServices;

using XamCore.Foundation;
using XamCore.CoreGraphics;

namespace XamCore.ObjCRuntime {
#if !XAMCORE_2_0 || COREBUILD
	public
#endif
	static partial class Messaging {
		internal const string LIBOBJC_DYLIB = "/usr/lib/libobjc.dylib";

#if !XAMCORE_2_0
		// Some explaination here. These are objc_msgSend's we use internally for binding
		// but were exposed to the user in compat. We have to keep these unbroken for bindings
		// projects and customer code, hense this file even if we stopped using them.

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static uint uint_objc_msgSend (IntPtr receiver, IntPtr selector);
		
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static int int_objc_msgSend (IntPtr receiver, IntPtr selector);

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

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static void void_objc_msgSendSuper (IntPtr [] super, IntPtr selector);
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
		public extern static IntPtr IntPtr_objc_msgSend_IntPtr (IntPtr receiver, IntPtr selector, IntPtr arg1);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static IntPtr intptr_objc_msgSend (IntPtr receiver, IntPtr selector);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static IntPtr intptr_objc_msgSend_intptr (IntPtr receiver, IntPtr selector, IntPtr arg1);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]		
		public extern static IntPtr intptr_objc_msgsend_intptr_int (IntPtr receiver, IntPtr selector, IntPtr arg1, int arg2);
		
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
		public extern static CGSize cgsize_objc_msgSend_cgpoint_intptr (IntPtr receiver, IntPtr selector, CGPoint arg1, IntPtr arg2);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static CGSize cgsize_objc_msgSend (IntPtr receiver, IntPtr selector);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static Boolean Boolean_objc_msgSend_IntPtr_Double_IntPtr (IntPtr receiver, IntPtr selector, IntPtr arg1, Double arg2, IntPtr arg3);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static Boolean Boolean_objc_msgSendSuper_IntPtr_Double_IntPtr (IntPtr receiver, IntPtr selector, IntPtr arg1, Double arg2, IntPtr arg3);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static bool bool_objc_msgSend_bool_IntPtr (System.IntPtr receiver, System.IntPtr selector, bool arg1, System.IntPtr arg2);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static bool bool_objc_msgSend_bool_IntPtr_IntPtr_IntPtr (System.IntPtr receiver, System.IntPtr selector, bool arg1, System.IntPtr arg2, System.IntPtr arg3, System.IntPtr arg4);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static bool bool_objc_msgSend_CMTimeRange_IntPtr_CMTime_IntPtr (System.IntPtr receiver, System.IntPtr selector, MonoMac.CoreMedia.CMTimeRange arg1, System.IntPtr arg2, MonoMac.CoreMedia.CMTime arg3, System.IntPtr arg4);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static bool bool_objc_msgSend_IntPtr_bool_IntPtr_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, bool arg2, System.IntPtr arg3, System.IntPtr arg4);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static bool bool_objc_msgSend_IntPtr_int_UInt32_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, int arg2, uint arg3, System.IntPtr arg4);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static bool bool_objc_msgSend_IntPtr_int_UInt32_IntPtr_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, int arg2, uint arg3, System.IntPtr arg4, System.IntPtr arg5);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static bool bool_objc_msgSend_IntPtr_IntPtr_CMTime_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, System.IntPtr arg2, MonoMac.CoreMedia.CMTime arg3, System.IntPtr arg4);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static bool bool_objc_msgSend_IntPtr_IntPtr_IntPtr_int_IntPtr_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, System.IntPtr arg2, System.IntPtr arg3, int arg4, System.IntPtr arg5, System.IntPtr arg6);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static bool bool_objc_msgSend_IntPtr_IntPtr_IntPtr_IntPtr_IntPtr_IntPtr_IntPtr_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, System.IntPtr arg2, System.IntPtr arg3, System.IntPtr arg4, System.IntPtr arg5, System.IntPtr arg6, System.IntPtr arg7, System.IntPtr arg8);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static bool bool_objc_msgSend_IntPtr_out_Boolean_out_Boolean_out_Boolean_IntPtr_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, out bool arg2, out bool arg3, out bool arg4, System.IntPtr arg5, System.IntPtr arg6);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static bool bool_objc_msgSend_IntPtr_SecIdentity_IntPtr_IntPtr_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, MonoMac.Security.SecIdentity arg2, System.IntPtr arg3, System.IntPtr arg4, System.IntPtr arg5);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static bool bool_objc_msgSend_UInt32_IntPtr (System.IntPtr receiver, System.IntPtr selector, uint arg1, System.IntPtr arg2);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static bool bool_objc_msgSendSuper_bool_IntPtr (System.IntPtr receiver, System.IntPtr selector, bool arg1, System.IntPtr arg2);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static bool bool_objc_msgSendSuper_bool_IntPtr_IntPtr_IntPtr (System.IntPtr receiver, System.IntPtr selector, bool arg1, System.IntPtr arg2, System.IntPtr arg3, System.IntPtr arg4);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static bool bool_objc_msgSendSuper_CMTimeRange_IntPtr_CMTime_IntPtr (System.IntPtr receiver, System.IntPtr selector, MonoMac.CoreMedia.CMTimeRange arg1, System.IntPtr arg2, MonoMac.CoreMedia.CMTime arg3, System.IntPtr arg4);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static bool bool_objc_msgSendSuper_IntPtr_bool_IntPtr_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, bool arg2, System.IntPtr arg3, System.IntPtr arg4);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static bool bool_objc_msgSendSuper_IntPtr_int_UInt32_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, int arg2, uint arg3, System.IntPtr arg4);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static bool bool_objc_msgSendSuper_IntPtr_int_UInt32_IntPtr_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, int arg2, uint arg3, System.IntPtr arg4, System.IntPtr arg5);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static bool bool_objc_msgSendSuper_IntPtr_IntPtr_CMTime_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, System.IntPtr arg2, MonoMac.CoreMedia.CMTime arg3, System.IntPtr arg4);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static bool bool_objc_msgSendSuper_IntPtr_IntPtr_IntPtr_int_IntPtr_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, System.IntPtr arg2, System.IntPtr arg3, int arg4, System.IntPtr arg5, System.IntPtr arg6);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static bool bool_objc_msgSendSuper_IntPtr_IntPtr_IntPtr_IntPtr_IntPtr_IntPtr_IntPtr_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, System.IntPtr arg2, System.IntPtr arg3, System.IntPtr arg4, System.IntPtr arg5, System.IntPtr arg6, System.IntPtr arg7, System.IntPtr arg8);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static bool bool_objc_msgSendSuper_IntPtr_out_Boolean_out_Boolean_out_Boolean_IntPtr_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, out bool arg2, out bool arg3, out bool arg4, System.IntPtr arg5, System.IntPtr arg6);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static bool bool_objc_msgSendSuper_IntPtr_SecIdentity_IntPtr_IntPtr_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, MonoMac.Security.SecIdentity arg2, System.IntPtr arg3, System.IntPtr arg4, System.IntPtr arg5);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static bool bool_objc_msgSendSuper_UInt32_IntPtr (System.IntPtr receiver, System.IntPtr selector, uint arg1, System.IntPtr arg2);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static int int_objc_msgSend_IntPtr_IntPtr_int_int_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, System.IntPtr arg2, int arg3, int arg4, System.IntPtr arg5);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static int int_objc_msgSend_IntPtr_IntPtr_int_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, System.IntPtr arg2, int arg3, System.IntPtr arg4);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static int int_objc_msgSendSuper_IntPtr_IntPtr_int_int_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, System.IntPtr arg2, int arg3, int arg4, System.IntPtr arg5);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static int int_objc_msgSendSuper_IntPtr_IntPtr_int_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, System.IntPtr arg2, int arg3, System.IntPtr arg4);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static System.IntPtr IntPtr_objc_msgSend_CMTime_out_CMTime_IntPtr (System.IntPtr receiver, System.IntPtr selector, MonoMac.CoreMedia.CMTime arg1, out MonoMac.CoreMedia.CMTime arg2, System.IntPtr arg3);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static System.IntPtr IntPtr_objc_msgSend_int_int_IntPtr_bool_IntPtr (System.IntPtr receiver, System.IntPtr selector, int arg1, int arg2, System.IntPtr arg3, bool arg4, System.IntPtr arg5);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static System.IntPtr IntPtr_objc_msgSend_int_IntPtr_IntPtr_IntPtr (System.IntPtr receiver, System.IntPtr selector, int arg1, System.IntPtr arg2, System.IntPtr arg3, System.IntPtr arg4);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static System.IntPtr IntPtr_objc_msgSend_int_IntPtr_ref_NSRange_ref_NSRange_IntPtr (System.IntPtr receiver, System.IntPtr selector, int arg1, System.IntPtr arg2, ref MonoMac.Foundation.NSRange arg3, ref MonoMac.Foundation.NSRange arg4, System.IntPtr arg5);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static System.IntPtr IntPtr_objc_msgSend_IntPtr_bool_IntPtr_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, bool arg2, System.IntPtr arg3, System.IntPtr arg4);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static System.IntPtr IntPtr_objc_msgSend_IntPtr_int_int_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, int arg2, int arg3, System.IntPtr arg4);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static System.IntPtr IntPtr_objc_msgSend_IntPtr_int_IntPtr_out_Boolean_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, int arg2, System.IntPtr arg3, out bool arg4, System.IntPtr arg5);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static System.IntPtr IntPtr_objc_msgSend_IntPtr_int_ref_NSPropertyListFormat_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, int arg2, ref MonoMac.Foundation.NSPropertyListFormat arg3, System.IntPtr arg4);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static System.IntPtr IntPtr_objc_msgSend_IntPtr_IntPtr_int_IntPtr_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, System.IntPtr arg2, int arg3, System.IntPtr arg4, System.IntPtr arg5);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static System.IntPtr IntPtr_objc_msgSend_IntPtr_NSRange_UInt64_IntPtr_int_IntPtr_out_Int32 (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, MonoMac.Foundation.NSRange arg2, ulong arg3, System.IntPtr arg4, int arg5, System.IntPtr arg6, out int arg7);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static System.IntPtr IntPtr_objc_msgSend_IntPtr_QTTimeRange_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, MonoMac.QTKit.QTTimeRange arg2, System.IntPtr arg3);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static System.IntPtr IntPtr_objc_msgSend_NSRange_IntPtr_int_IntPtr (System.IntPtr receiver, System.IntPtr selector, MonoMac.Foundation.NSRange arg1, System.IntPtr arg2, int arg3, System.IntPtr arg4);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static System.IntPtr IntPtr_objc_msgSend_QTTime_IntPtr_IntPtr (System.IntPtr receiver, System.IntPtr selector, MonoMac.QTKit.QTTime arg1, System.IntPtr arg2, System.IntPtr arg3);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static System.IntPtr IntPtr_objc_msgSend_QTTimeRange_IntPtr (System.IntPtr receiver, System.IntPtr selector, MonoMac.QTKit.QTTimeRange arg1, System.IntPtr arg2);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static System.IntPtr IntPtr_objc_msgSendSuper_CMTime_out_CMTime_IntPtr (System.IntPtr receiver, System.IntPtr selector, MonoMac.CoreMedia.CMTime arg1, out MonoMac.CoreMedia.CMTime arg2, System.IntPtr arg3);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static System.IntPtr IntPtr_objc_msgSendSuper_int_int_IntPtr_bool_IntPtr (System.IntPtr receiver, System.IntPtr selector, int arg1, int arg2, System.IntPtr arg3, bool arg4, System.IntPtr arg5);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static System.IntPtr IntPtr_objc_msgSendSuper_int_IntPtr_IntPtr_IntPtr (System.IntPtr receiver, System.IntPtr selector, int arg1, System.IntPtr arg2, System.IntPtr arg3, System.IntPtr arg4);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static System.IntPtr IntPtr_objc_msgSendSuper_int_IntPtr_ref_NSRange_ref_NSRange_IntPtr (System.IntPtr receiver, System.IntPtr selector, int arg1, System.IntPtr arg2, ref MonoMac.Foundation.NSRange arg3, ref MonoMac.Foundation.NSRange arg4, System.IntPtr arg5);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static System.IntPtr IntPtr_objc_msgSendSuper_IntPtr_bool_IntPtr_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, bool arg2, System.IntPtr arg3, System.IntPtr arg4);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static System.IntPtr IntPtr_objc_msgSendSuper_IntPtr_int_int_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, int arg2, int arg3, System.IntPtr arg4);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static System.IntPtr IntPtr_objc_msgSendSuper_IntPtr_int_IntPtr_out_Boolean_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, int arg2, System.IntPtr arg3, out bool arg4, System.IntPtr arg5);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static System.IntPtr IntPtr_objc_msgSendSuper_IntPtr_int_ref_NSPropertyListFormat_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, int arg2, ref MonoMac.Foundation.NSPropertyListFormat arg3, System.IntPtr arg4);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static System.IntPtr IntPtr_objc_msgSendSuper_IntPtr_IntPtr_int_IntPtr_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, System.IntPtr arg2, int arg3, System.IntPtr arg4, System.IntPtr arg5);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static System.IntPtr IntPtr_objc_msgSendSuper_IntPtr_NSRange_UInt64_IntPtr_int_IntPtr_out_Int32 (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, MonoMac.Foundation.NSRange arg2, ulong arg3, System.IntPtr arg4, int arg5, System.IntPtr arg6, out int arg7);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static System.IntPtr IntPtr_objc_msgSendSuper_IntPtr_QTTimeRange_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, MonoMac.QTKit.QTTimeRange arg2, System.IntPtr arg3);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static System.IntPtr IntPtr_objc_msgSendSuper_NSRange_IntPtr_int_IntPtr (System.IntPtr receiver, System.IntPtr selector, MonoMac.Foundation.NSRange arg1, System.IntPtr arg2, int arg3, System.IntPtr arg4);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static System.IntPtr IntPtr_objc_msgSendSuper_QTTime_IntPtr_IntPtr (System.IntPtr receiver, System.IntPtr selector, MonoMac.QTKit.QTTime arg1, System.IntPtr arg2, System.IntPtr arg3);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static System.IntPtr IntPtr_objc_msgSendSuper_QTTimeRange_IntPtr (System.IntPtr receiver, System.IntPtr selector, MonoMac.QTKit.QTTimeRange arg1, System.IntPtr arg2);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static float monomac_float_objc_msgSend (System.IntPtr receiver, System.IntPtr selector);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static float monomac_float_objc_msgSendSuper (System.IntPtr receiver, System.IntPtr selector);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static System.IntPtr monomac_IntPtr_objc_msgSend_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static System.IntPtr monomac_IntPtr_objc_msgSendSuper_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static void void_objc_msgSend_IntPtr_int_IntPtr_int_IntPtr_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, int arg2, System.IntPtr arg3, int arg4, System.IntPtr arg5, System.IntPtr arg6);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static void void_objc_msgSend_IntPtr_int_IntPtr_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, int arg2, System.IntPtr arg3, System.IntPtr arg4);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static void void_objc_msgSend_IntPtr_out_Single_int (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, out float arg2, int arg3);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static void void_objc_msgSendSuper_IntPtr_int_IntPtr_int_IntPtr_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, int arg2, System.IntPtr arg3, int arg4, System.IntPtr arg5, System.IntPtr arg6);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static void void_objc_msgSendSuper_IntPtr_int_IntPtr_IntPtr (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, int arg2, System.IntPtr arg3, System.IntPtr arg4);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static void void_objc_msgSendSuper_IntPtr_out_Single_int (System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1, out float arg2, int arg3);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static IntPtr IntPtr_objc_msgSend_IntPtr_IntPtr_IntPtr_NSRange_int (IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2, IntPtr arg3, MonoMac.Foundation.NSRange arg4, int arg5);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static IntPtr IntPtr_objc_msgSend_IntPtr_IntPtr_NSRange_int (IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2, MonoMac.Foundation.NSRange arg3, int arg4);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static IntPtr IntPtr_objc_msgSendSuper_IntPtr_IntPtr_IntPtr_NSRange_int (IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2, IntPtr arg3, MonoMac.Foundation.NSRange arg4, int arg5);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static IntPtr IntPtr_objc_msgSendSuper_IntPtr_IntPtr_NSRange_int (IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2, MonoMac.Foundation.NSRange arg3, int arg4);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper_stret")]
		public extern static void AVAudio3DVectorOrientation_objc_msgSendSuper_stret (out global::MonoMac.AVFoundation.AVAudio3DVectorOrientation retval, IntPtr receiver, IntPtr selector);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void AVAudio3DVectorOrientation_objc_msgSend_stret (out global::MonoMac.AVFoundation.AVAudio3DVectorOrientation retval, IntPtr receiver, IntPtr selector);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper_stret")]
		public extern static void AVAudioConverterPrimeInfo_objc_msgSendSuper_stret (out global::MonoMac.AVFoundation.AVAudioConverterPrimeInfo retval, IntPtr receiver, IntPtr selector);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void AVAudioConverterPrimeInfo_objc_msgSend_stret (out global::MonoMac.AVFoundation.AVAudioConverterPrimeInfo retval, IntPtr receiver, IntPtr selector);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper_stret")]
		public extern static void AVPixelAspectRatio_objc_msgSendSuper_stret (out global::MonoMac.AVFoundation.AVPixelAspectRatio retval, IntPtr receiver, IntPtr selector);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void AVPixelAspectRatio_objc_msgSend_stret (out global::MonoMac.AVFoundation.AVPixelAspectRatio retval, IntPtr receiver, IntPtr selector);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper_stret")]
		public extern static void NSRange_objc_msgSendSuper_stret (out NSRange retval, IntPtr receiver, IntPtr selector);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper_stret")]
		public extern static void NSRange_objc_msgSendSuper_stret_IntPtr (out NSRange retval, IntPtr receiver, IntPtr selector, IntPtr arg1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper_stret")]
		public extern static void NSRange_objc_msgSendSuper_stret_IntPtr_NSRange_NSRange (out NSRange retval, IntPtr receiver, IntPtr selector, IntPtr arg1, NSRange arg2, NSRange arg3);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper_stret")]
		public extern static void NSRange_objc_msgSendSuper_stret_IntPtr_UInt32 (out NSRange retval, IntPtr receiver, IntPtr selector, IntPtr arg1, global::System.UInt32 arg2);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper_stret")]
		public extern static void NSRange_objc_msgSendSuper_stret_IntPtr_UInt64_NSRange (out NSRange retval, IntPtr receiver, IntPtr selector, IntPtr arg1, global::System.UInt64 arg2, NSRange arg3);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper_stret")]
		public extern static void NSRange_objc_msgSendSuper_stret_IntPtr_int (out NSRange retval, IntPtr receiver, IntPtr selector, IntPtr arg1, int arg2);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper_stret")]
		public extern static void NSRange_objc_msgSendSuper_stret_IntPtr_int_IntPtr_bool_int_IntPtr (out NSRange retval, IntPtr receiver, IntPtr selector, IntPtr arg1, int arg2, IntPtr arg3, bool arg4, int arg5, IntPtr arg6);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper_stret")]
		public extern static void NSRange_objc_msgSendSuper_stret_IntPtr_int_IntPtr_bool_int_out_Int32 (out NSRange retval, IntPtr receiver, IntPtr selector, IntPtr arg1, int arg2, IntPtr arg3, bool arg4, int arg5, out int arg6);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper_stret")]
		public extern static void NSRange_objc_msgSendSuper_stret_IntPtr_int_NSRange (out NSRange retval, IntPtr receiver, IntPtr selector, IntPtr arg1, int arg2, NSRange arg3);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper_stret")]
		public extern static void NSRange_objc_msgSendSuper_stret_NSRange (out NSRange retval, IntPtr receiver, IntPtr selector, NSRange arg1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper_stret")]
		public extern static void NSRange_objc_msgSendSuper_stret_NSRange_IntPtr_UInt32 (out NSRange retval, IntPtr receiver, IntPtr selector, NSRange arg1, IntPtr arg2, global::System.UInt32 arg3);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper_stret")]
		public extern static void NSRange_objc_msgSendSuper_stret_NSRange_int (out NSRange retval, IntPtr receiver, IntPtr selector, NSRange arg1, int arg2);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper_stret")]
		public extern static void NSRange_objc_msgSendSuper_stret_NSRange_out_NSRange (out NSRange retval, IntPtr receiver, IntPtr selector, NSRange arg1, out NSRange arg2);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper_stret")]
		public extern static void NSRange_objc_msgSendSuper_stret_PointF (out NSRange retval, IntPtr receiver, IntPtr selector, global::System.Drawing.PointF arg1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper_stret")]
		public extern static void NSRange_objc_msgSendSuper_stret_RectangleF (out NSRange retval, IntPtr receiver, IntPtr selector, global::System.Drawing.RectangleF arg1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper_stret")]
		public extern static void NSRange_objc_msgSendSuper_stret_RectangleF_IntPtr (out NSRange retval, IntPtr receiver, IntPtr selector, global::System.Drawing.RectangleF arg1, IntPtr arg2);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper_stret")]
		public extern static void NSRange_objc_msgSendSuper_stret_UInt32 (out NSRange retval, IntPtr receiver, IntPtr selector, global::System.UInt32 arg1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper_stret")]
		public extern static void NSRange_objc_msgSendSuper_stret_int (out NSRange retval, IntPtr receiver, IntPtr selector, int arg1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper_stret")]
		public extern static void NSRange_objc_msgSendSuper_stret_int_int_IntPtr (out NSRange retval, IntPtr receiver, IntPtr selector, int arg1, int arg2, IntPtr arg3);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void NSRange_objc_msgSend_stret (out NSRange retval, IntPtr receiver, IntPtr selector);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void NSRange_objc_msgSend_stret_IntPtr (out NSRange retval, IntPtr receiver, IntPtr selector, IntPtr arg1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void NSRange_objc_msgSend_stret_IntPtr_NSRange_NSRange (out NSRange retval, IntPtr receiver, IntPtr selector, IntPtr arg1, NSRange arg2, NSRange arg3);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void NSRange_objc_msgSend_stret_IntPtr_UInt32 (out NSRange retval, IntPtr receiver, IntPtr selector, IntPtr arg1, global::System.UInt32 arg2);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void NSRange_objc_msgSend_stret_IntPtr_UInt64_NSRange (out NSRange retval, IntPtr receiver, IntPtr selector, IntPtr arg1, global::System.UInt64 arg2, NSRange arg3);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void NSRange_objc_msgSend_stret_IntPtr_int (out NSRange retval, IntPtr receiver, IntPtr selector, IntPtr arg1, int arg2);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void NSRange_objc_msgSend_stret_IntPtr_int_IntPtr_bool_int_IntPtr (out NSRange retval, IntPtr receiver, IntPtr selector, IntPtr arg1, int arg2, IntPtr arg3, bool arg4, int arg5, IntPtr arg6);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void NSRange_objc_msgSend_stret_IntPtr_int_IntPtr_bool_int_out_Int32 (out NSRange retval, IntPtr receiver, IntPtr selector, IntPtr arg1, int arg2, IntPtr arg3, bool arg4, int arg5, out int arg6);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void NSRange_objc_msgSend_stret_IntPtr_int_NSRange (out NSRange retval, IntPtr receiver, IntPtr selector, IntPtr arg1, int arg2, NSRange arg3);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void NSRange_objc_msgSend_stret_NSRange (out NSRange retval, IntPtr receiver, IntPtr selector, NSRange arg1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void NSRange_objc_msgSend_stret_NSRange_IntPtr_UInt32 (out NSRange retval, IntPtr receiver, IntPtr selector, NSRange arg1, IntPtr arg2, global::System.UInt32 arg3);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void NSRange_objc_msgSend_stret_NSRange_int (out NSRange retval, IntPtr receiver, IntPtr selector, NSRange arg1, int arg2);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void NSRange_objc_msgSend_stret_NSRange_out_NSRange (out NSRange retval, IntPtr receiver, IntPtr selector, NSRange arg1, out NSRange arg2);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void NSRange_objc_msgSend_stret_PointF (out NSRange retval, IntPtr receiver, IntPtr selector, global::System.Drawing.PointF arg1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void NSRange_objc_msgSend_stret_RectangleF (out NSRange retval, IntPtr receiver, IntPtr selector, global::System.Drawing.RectangleF arg1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void NSRange_objc_msgSend_stret_RectangleF_IntPtr (out NSRange retval, IntPtr receiver, IntPtr selector, global::System.Drawing.RectangleF arg1, IntPtr arg2);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void NSRange_objc_msgSend_stret_UInt32 (out NSRange retval, IntPtr receiver, IntPtr selector, global::System.UInt32 arg1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void NSRange_objc_msgSend_stret_int (out NSRange retval, IntPtr receiver, IntPtr selector, int arg1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void NSRange_objc_msgSend_stret_int_int_IntPtr (out NSRange retval, IntPtr receiver, IntPtr selector, int arg1, int arg2, IntPtr arg3);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper_stret")]
		public extern static void PointF_objc_msgSendSuper_stret (out global::System.Drawing.PointF retval, IntPtr receiver, IntPtr selector);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper_stret")]
		public extern static void PointF_objc_msgSendSuper_stret_IntPtr (out global::System.Drawing.PointF retval, IntPtr receiver, IntPtr selector, IntPtr arg1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper_stret")]
		public extern static void PointF_objc_msgSendSuper_stret_IntPtr_float (out global::System.Drawing.PointF retval, IntPtr receiver, IntPtr selector, IntPtr arg1, float arg2);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper_stret")]
		public extern static void PointF_objc_msgSendSuper_stret_PointF (out global::System.Drawing.PointF retval, IntPtr receiver, IntPtr selector, global::System.Drawing.PointF arg1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper_stret")]
		public extern static void PointF_objc_msgSendSuper_stret_PointF_IntPtr (out global::System.Drawing.PointF retval, IntPtr receiver, IntPtr selector, global::System.Drawing.PointF arg1, IntPtr arg2);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper_stret")]
		public extern static void PointF_objc_msgSendSuper_stret_PointF_PointF (out global::System.Drawing.PointF retval, IntPtr receiver, IntPtr selector, global::System.Drawing.PointF arg1, global::System.Drawing.PointF arg2);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper_stret")]
		public extern static void PointF_objc_msgSendSuper_stret_RectangleF (out global::System.Drawing.PointF retval, IntPtr receiver, IntPtr selector, global::System.Drawing.RectangleF arg1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper_stret")]
		public extern static void PointF_objc_msgSendSuper_stret_int (out global::System.Drawing.PointF retval, IntPtr receiver, IntPtr selector, int arg1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void PointF_objc_msgSend_stret (out global::System.Drawing.PointF retval, IntPtr receiver, IntPtr selector);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void PointF_objc_msgSend_stret_IntPtr (out global::System.Drawing.PointF retval, IntPtr receiver, IntPtr selector, IntPtr arg1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void PointF_objc_msgSend_stret_IntPtr_float (out global::System.Drawing.PointF retval, IntPtr receiver, IntPtr selector, IntPtr arg1, float arg2);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void PointF_objc_msgSend_stret_PointF (out global::System.Drawing.PointF retval, IntPtr receiver, IntPtr selector, global::System.Drawing.PointF arg1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void PointF_objc_msgSend_stret_PointF_IntPtr (out global::System.Drawing.PointF retval, IntPtr receiver, IntPtr selector, global::System.Drawing.PointF arg1, IntPtr arg2);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void PointF_objc_msgSend_stret_PointF_PointF (out global::System.Drawing.PointF retval, IntPtr receiver, IntPtr selector, global::System.Drawing.PointF arg1, global::System.Drawing.PointF arg2);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void PointF_objc_msgSend_stret_RectangleF (out global::System.Drawing.PointF retval, IntPtr receiver, IntPtr selector, global::System.Drawing.RectangleF arg1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void PointF_objc_msgSend_stret_int (out global::System.Drawing.PointF retval, IntPtr receiver, IntPtr selector, int arg1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper_stret")]
		public extern static void SizeF_objc_msgSendSuper_stret (out global::System.Drawing.SizeF retval, IntPtr receiver, IntPtr selector);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper_stret")]
		public extern static void SizeF_objc_msgSendSuper_stret_IntPtr (out global::System.Drawing.SizeF retval, IntPtr receiver, IntPtr selector, IntPtr arg1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper_stret")]
		public extern static void SizeF_objc_msgSendSuper_stret_IntPtr_IntPtr (out global::System.Drawing.SizeF retval, IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper_stret")]
		public extern static void SizeF_objc_msgSendSuper_stret_IntPtr_IntPtr_IntPtr (out global::System.Drawing.SizeF retval, IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2, IntPtr arg3);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper_stret")]
		public extern static void SizeF_objc_msgSendSuper_stret_IntPtr_IntPtr_int (out global::System.Drawing.SizeF retval, IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2, int arg3);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper_stret")]
		public extern static void SizeF_objc_msgSendSuper_stret_IntPtr_NSRange_IntPtr_UInt32 (out global::System.Drawing.SizeF retval, IntPtr receiver, IntPtr selector, IntPtr arg1, NSRange arg2, global::System.IntPtr arg3, global::System.UInt32 arg4);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper_stret")]
		public extern static void SizeF_objc_msgSendSuper_stret_IntPtr_SizeF (out global::System.Drawing.SizeF retval, IntPtr receiver, IntPtr selector, IntPtr arg1, global::System.Drawing.SizeF arg2);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper_stret")]
		public extern static void SizeF_objc_msgSendSuper_stret_IntPtr_SizeF_SizeF (out global::System.Drawing.SizeF retval, IntPtr receiver, IntPtr selector, IntPtr arg1, global::System.Drawing.SizeF arg2, global::System.Drawing.SizeF arg3);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper_stret")]
		public extern static void SizeF_objc_msgSendSuper_stret_RectangleF (out global::System.Drawing.SizeF retval, IntPtr receiver, IntPtr selector, global::System.Drawing.RectangleF arg1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper_stret")]
		public extern static void SizeF_objc_msgSendSuper_stret_SizeF (out global::System.Drawing.SizeF retval, IntPtr receiver, IntPtr selector, global::System.Drawing.SizeF arg1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper_stret")]
		public extern static void SizeF_objc_msgSendSuper_stret_SizeF_IntPtr (out global::System.Drawing.SizeF retval, IntPtr receiver, IntPtr selector, global::System.Drawing.SizeF arg1, IntPtr arg2);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper_stret")]
		public extern static void SizeF_objc_msgSendSuper_stret_SizeF_IntPtr_IntPtr_int_int_int (out global::System.Drawing.SizeF retval, IntPtr receiver, IntPtr selector, global::System.Drawing.SizeF arg1, IntPtr arg2, IntPtr arg3, int arg4, int arg5, int arg6);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper_stret")]
		public extern static void SizeF_objc_msgSendSuper_stret_SizeF_bool_bool_int (out global::System.Drawing.SizeF retval, IntPtr receiver, IntPtr selector, global::System.Drawing.SizeF arg1, bool arg2, bool arg3, int arg4);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper_stret")]
		public extern static void SizeF_objc_msgSendSuper_stret_UInt32 (out global::System.Drawing.SizeF retval, IntPtr receiver, IntPtr selector, global::System.UInt32 arg1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper_stret")]
		public extern static void SizeF_objc_msgSendSuper_stret_bool (out global::System.Drawing.SizeF retval, IntPtr receiver, IntPtr selector, bool arg1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper_stret")]
		public extern static void SizeF_objc_msgSendSuper_stret_int (out global::System.Drawing.SizeF retval, IntPtr receiver, IntPtr selector, int arg1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void SizeF_objc_msgSend_stret (out global::System.Drawing.SizeF retval, IntPtr receiver, IntPtr selector);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void SizeF_objc_msgSend_stret_IntPtr (out global::System.Drawing.SizeF retval, IntPtr receiver, IntPtr selector, IntPtr arg1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void SizeF_objc_msgSend_stret_IntPtr_IntPtr (out global::System.Drawing.SizeF retval, IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void SizeF_objc_msgSend_stret_IntPtr_IntPtr_IntPtr (out global::System.Drawing.SizeF retval, IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2, IntPtr arg3);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void SizeF_objc_msgSend_stret_IntPtr_IntPtr_int (out global::System.Drawing.SizeF retval, IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2, int arg3);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void SizeF_objc_msgSend_stret_IntPtr_NSRange_IntPtr_UInt32 (out global::System.Drawing.SizeF retval, IntPtr receiver, IntPtr selector, IntPtr arg1, NSRange arg2, global::System.IntPtr arg3, global::System.UInt32 arg4);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void SizeF_objc_msgSend_stret_IntPtr_SizeF (out global::System.Drawing.SizeF retval, IntPtr receiver, IntPtr selector, IntPtr arg1, global::System.Drawing.SizeF arg2);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void SizeF_objc_msgSend_stret_IntPtr_SizeF_SizeF (out global::System.Drawing.SizeF retval, IntPtr receiver, IntPtr selector, IntPtr arg1, global::System.Drawing.SizeF arg2, global::System.Drawing.SizeF arg3);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void SizeF_objc_msgSend_stret_RectangleF (out global::System.Drawing.SizeF retval, IntPtr receiver, IntPtr selector, global::System.Drawing.RectangleF arg1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void SizeF_objc_msgSend_stret_SizeF (out global::System.Drawing.SizeF retval, IntPtr receiver, IntPtr selector, global::System.Drawing.SizeF arg1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void SizeF_objc_msgSend_stret_SizeF_IntPtr (out global::System.Drawing.SizeF retval, IntPtr receiver, IntPtr selector, global::System.Drawing.SizeF arg1, IntPtr arg2);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void SizeF_objc_msgSend_stret_SizeF_IntPtr_IntPtr_int_int_int (out global::System.Drawing.SizeF retval, IntPtr receiver, IntPtr selector, global::System.Drawing.SizeF arg1, IntPtr arg2, IntPtr arg3, int arg4, int arg5, int arg6);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void SizeF_objc_msgSend_stret_SizeF_bool_bool_int (out global::System.Drawing.SizeF retval, IntPtr receiver, IntPtr selector, global::System.Drawing.SizeF arg1, bool arg2, bool arg3, int arg4);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void SizeF_objc_msgSend_stret_UInt32 (out global::System.Drawing.SizeF retval, IntPtr receiver, IntPtr selector, global::System.UInt32 arg1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void SizeF_objc_msgSend_stret_bool (out global::System.Drawing.SizeF retval, IntPtr receiver, IntPtr selector, bool arg1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend_stret")]
		public extern static void SizeF_objc_msgSend_stret_int (out global::System.Drawing.SizeF retval, IntPtr receiver, IntPtr selector, int arg1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static uint UInt32_objc_msgSendSuper_IntPtr_NSRange (IntPtr receiver, IntPtr selector, IntPtr arg1, MonoMac.Foundation.NSRange arg2);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static uint UInt32_objc_msgSend_IntPtr_NSRange (IntPtr receiver, IntPtr selector, IntPtr arg1, MonoMac.Foundation.NSRange arg2);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static int int_objc_msgSendSuper_NSRange_IntPtr_IntPtr_IntPtr_IntPtr (IntPtr receiver, IntPtr selector, MonoMac.Foundation.NSRange arg1, IntPtr arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static int int_objc_msgSendSuper_NSRange_IntPtr_IntPtr_IntPtr_IntPtr_IntPtr (IntPtr receiver, IntPtr selector, MonoMac.Foundation.NSRange arg1, IntPtr arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5, IntPtr arg6);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static int int_objc_msgSend_NSRange_IntPtr_IntPtr_IntPtr_IntPtr (IntPtr receiver, IntPtr selector, MonoMac.Foundation.NSRange arg1, IntPtr arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static int int_objc_msgSend_NSRange_IntPtr_IntPtr_IntPtr_IntPtr_IntPtr (IntPtr receiver, IntPtr selector, MonoMac.Foundation.NSRange arg1, IntPtr arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5, IntPtr arg6);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static int int_objc_msgSend_NSRange_IntPtr_IntPtr_IntPtr_IntPtr_IntPtr (IntPtr receiver, IntPtr selector, MonoMac.Foundation.NSRange arg1, IntPtr arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5, IntPtr arg6);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public extern static IntPtr IntPtr_objc_msgSend_IntPtr_nint (IntPtr receiver, IntPtr selector, global::System.IntPtr arg1, nint arg2);
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSendSuper")]
		public extern static IntPtr IntPtr_objc_msgSendSuper_IntPtr_nint (IntPtr receiver, IntPtr selector, global::System.IntPtr arg1, nint arg2);
#endif
	}
}

#endif // MONOMAC
