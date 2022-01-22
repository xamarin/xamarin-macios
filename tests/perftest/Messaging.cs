using System;
using System.Runtime.InteropServices;

using CoreGraphics;
using Foundation;
using ObjCRuntime;

class Messaging {
	internal const string LIBOBJC_DYLIB = "/usr/lib/libobjc.dylib";

	[DllImport (LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
	public extern static void void_objc_msgSend_IntPtr_IntPtr (IntPtr receiver, IntPtr selector, IntPtr p1, IntPtr p2);

	[DllImport (LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
	public extern static IntPtr IntPtr_objc_msgSend (IntPtr receiver, IntPtr selector);

	[DllImport (LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
	public extern static int int_objc_msgSend_int (IntPtr receiver, IntPtr selector, int p0);

	[DllImport (LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
	public extern static IntPtr IntPtr_objc_msgSend_IntPtr (IntPtr receiver, IntPtr selector, IntPtr p1);

	[DllImport (LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
	public extern static IntPtr IntPtr_objc_msgSend_int (IntPtr r, IntPtr s, int p0);

	[DllImport (LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
	public extern static IntPtr IntPtr_objc_msgSend_long (IntPtr r, IntPtr s, long p0);

	[DllImport (LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
	public extern static void void_objc_msgSend (IntPtr r, IntPtr s);

}
