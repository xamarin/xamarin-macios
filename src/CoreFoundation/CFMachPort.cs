/*
 * MachPort.cs: Bindings to the CFMachPort API
 * 
 * Authors:
 *    Miguel de Icaza
 * 
 * Copyright 2014 Xamarin Inc
 * All Rights Reserved
 */

#nullable enable

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;

namespace CoreFoundation {

#if false
	[StructLayout(LayoutKind.Sequential)]
	internal public struct CFMachPortContext {
		public delegate IntPtr CBRetain (IntPtr info);
		public delegate void CBRelease (IntPtr info);
		public delegate IntPtr CBCopyDescription (IntPtr _EventInfo);

		public IntPtr Version;
		public IntPtr Info;
		public CBRetain Retain;
		public CBRelease Release;
		public CBCopyDescription CopyDescription;
	}

	delegate void CFMachPortCallBack (IntPtr cfMachPort, IntPtr msg, IntPtr size, IntPtr info);
#endif

	public class CFMachPort : NativeObject {
		delegate void CFMachPortCallBack (IntPtr cfmachport, IntPtr msg, nint len, IntPtr context);

		[Preserve (Conditional = true)]
		internal CFMachPort (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static IntPtr CFMachPortGetPort (IntPtr handle);

		public IntPtr MachPort {
			get {
				return CFMachPortGetPort (Handle);
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static void CFMachPortInvalidate (IntPtr handle);

		public void Invalidate ()
		{
			CFMachPortInvalidate (Handle);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static byte CFMachPortIsValid (IntPtr handle);
		public bool IsValid {
			get {
				return CFMachPortIsValid (Handle) != 0;
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static IntPtr CFMachPortCreateRunLoopSource (IntPtr allocator, IntPtr port, IntPtr order);

		public CFRunLoopSource CreateRunLoopSource ()
		{
			// order is currently ignored, we must pass 0
			var source = CFMachPortCreateRunLoopSource (IntPtr.Zero, Handle, IntPtr.Zero);
			return new CFRunLoopSource (source, true);
		}

	}
}
