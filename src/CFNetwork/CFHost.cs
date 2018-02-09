//
// MonoMac.CoreServices.CFHost
//
// Authors:
//      Martin Baulig (martin.baulig@xamarin.com)
//
// Copyright 2012-2015 Xamarin Inc. (http://www.xamarin.com)
//

using System;
using System.Net;
using System.Runtime.InteropServices;
using CoreFoundation;
using Foundation;
using ObjCRuntime;

// CFHost is in CFNetwork.framework, no idea why it ended up in CoreServices when it was bound.
#if XAMCORE_4_0
namespace CFNetwork {
#else
namespace CoreServices {
#endif

	// used by CFStream.cs (only?)
	class CFHost : INativeObject, IDisposable {
		internal IntPtr handle;

		CFHost (IntPtr handle)
		{
			this.handle = handle;
		}

		~CFHost ()
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
		
		protected virtual void Dispose (bool disposing)
		{
			if (handle != IntPtr.Zero){
				CFObject.CFRelease (handle);
				handle = IntPtr.Zero;
			}
		}

		[DllImport (Constants.CFNetworkLibrary)]
		extern static /* CFHostRef __nonnull */ IntPtr CFHostCreateWithAddress (
			/* CFAllocatorRef __nullable */ IntPtr allocator, /* CFDataRef __nonnull */ IntPtr addr);

		public static CFHost Create (IPEndPoint endpoint)
		{
			// CFSocketAddress will throw the ANE
			using (var data = new CFSocketAddress (endpoint))
				return new CFHost (CFHostCreateWithAddress (IntPtr.Zero, data.Handle));
		}

		[DllImport (Constants.CFNetworkLibrary)]
		extern static /* CFHostRef __nonnull */ IntPtr CFHostCreateWithName (
			/* CFAllocatorRef __nullable */ IntPtr allocator, /* CFStringRef __nonnull */ IntPtr hostname);

		public static CFHost Create (string name)
		{
			// CFString will throw the ANE
			using (var ptr = new CFString (name))
				return new CFHost (CFHostCreateWithName (IntPtr.Zero, ptr.Handle));
		}
	}
}
