//
// NWError.cs: Bindings the Netowrk nw_error API.
//
// Authors:
//   Miguel de Icaza (miguel@microsoft.com)
//
// Copyrigh 2018 Microsoft Inc
//
using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using ObjCRuntime;
using Foundation;
using CoreFoundation;

namespace Network {
	public enum NWErrorDomain {
		Invalid = 0,
		Posix = 1,
		Dns = 2,
		Tls = 3
	}
	
	public class NWError : INativeObject, IDisposable {
		internal IntPtr handle;
		public IntPtr Handle {
			get { return handle; }
		}

		public NWError (IntPtr handle, bool owns)
		{
			this.handle = handle;
			if (owns == false)
				CFObject.CFRetain (handle);
		}

		public NWError (IntPtr handle) : this (handle, false)
		{
		}

		~NWError ()
		{
			Dispose (false);
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

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern NWErrorDomain nw_error_get_error_domain (IntPtr error);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public NWErrorDomain ErrorDomain => nw_error_get_error_domain (handle);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern int nw_error_get_error_code (IntPtr handle);
		
		[TV (12,0), Mac (10,14), iOS (12,0)]
		public int ErrorCode => nw_error_get_error_code (handle);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		static extern IntPtr nw_error_copy_cf_error (IntPtr error);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public CFException CFError {
			get {
				return CFException.FromCFError (nw_error_copy_cf_error (handle), true);
			}
		}
	}
}
