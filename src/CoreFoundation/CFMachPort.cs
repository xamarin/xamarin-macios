/*
 * MachPort.cs: Bindings to the CFMachPort API
 * 
 * Authors:
 *    Miguel de Icaza
 * 
 * Copyright 2014 Xamarin Inc
 * All Rights Reserved
 */
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;

namespace CoreFoundation
{

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
	
	public class CFMachPort : INativeObject, IDisposable 
	{
		delegate void CFMachPortCallBack (IntPtr cfmachport, IntPtr msg, nint len, IntPtr context);
			
		internal IntPtr handle;

		public CFMachPort (IntPtr handle) : this (handle, false)
		{
		}

		public CFMachPort (IntPtr handle, bool ownsHandle)
		{
			if (!ownsHandle)
				CFObject.CFRetain (handle);
			this.handle = handle;
		}

		~CFMachPort ()
		{
			Dispose (false);
		}

		public IntPtr Handle {
			get {
				return handle;
			}
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		public virtual void Dispose (bool disposing)
		{
			if (handle != IntPtr.Zero) {
				CFObject.CFRelease (handle);
				handle = IntPtr.Zero;
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static IntPtr CFMachPortGetPort (IntPtr handle);

		public IntPtr MachPort {
			get {
				return CFMachPortGetPort (handle);
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static void CFMachPortInvalidate (IntPtr handle);

		public void Invalidate ()
		{
			CFMachPortInvalidate (handle);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static bool CFMachPortIsValid (IntPtr handle);
		public bool IsValid { 
			get {
				return CFMachPortIsValid (handle);
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static IntPtr CFMachPortCreateRunLoopSource (IntPtr allocator, IntPtr port, IntPtr order);

		public CFRunLoopSource CreateRunLoopSource ()
		{
			// order is currently ignored, we must pass 0
			var source = CFMachPortCreateRunLoopSource (IntPtr.Zero, handle, IntPtr.Zero);
			return new CFRunLoopSource (source, true);
		}

	}
}

