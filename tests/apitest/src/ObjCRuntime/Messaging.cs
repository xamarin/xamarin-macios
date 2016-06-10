using System;
using System.Runtime.InteropServices;

public class Messaging
{
	const string LIBOBJC_DYLIB = "/usr/lib/libobjc.dylib";


	[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
	public extern static IntPtr IntPtr_objc_msgSend (IntPtr receiver, IntPtr selector);

	[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
	public extern static bool bool_objc_msgSend_IntPtr (IntPtr receiver, IntPtr selector, IntPtr p1);

	[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
	public extern static void void_objc_msgSend (IntPtr receiver, IntPtr selector);
}

