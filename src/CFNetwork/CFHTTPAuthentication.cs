//
// MonoMac.CoreServices.CFHTTPAuthentication
//
// Authors:
//      Martin Baulig (martin.baulig@gmail.com)
//
// Copyright 2012-2014 Xamarin Inc. (http://www.xamarin.com)
//

using System;
using System.Net;
using System.Runtime.InteropServices;
using Foundation;
using CoreFoundation;
using ObjCRuntime;

// CFHTTPAuthentication is in CFNetwork.framework, no idea why it ended up in CoreServices when it was bound.
#if XAMCORE_4_0
namespace CFNetwork {
#else
namespace CoreServices {
#endif

	public class CFHTTPAuthentication : CFType, INativeObject, IDisposable {
		internal IntPtr handle;

		internal CFHTTPAuthentication (IntPtr handle)
			: this (handle, false)
		{
		}

		internal CFHTTPAuthentication (IntPtr handle, bool owns)
		{
			if (!owns)
				CFObject.CFRetain (handle);
			this.handle = handle;
		}

		[DllImport (Constants.CFNetworkLibrary, EntryPoint="CFHTTPAuthenticationGetTypeID")]
		public extern static /* CFTypeID */ nint GetTypeID ();

		~CFHTTPAuthentication ()
		{
			Dispose (false);
		}
		
		protected void CheckHandle ()
		{
			if (handle == IntPtr.Zero)
				throw new ObjectDisposedException (GetType ().Name);
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		public IntPtr Handle {
			get {
				CheckHandle ();
				return handle;
			}
		}
		
		protected virtual void Dispose (bool disposing)
		{
			if (handle != IntPtr.Zero) {
				CFObject.CFRelease (handle);
				handle = IntPtr.Zero;
			}
		}

		[DllImport (Constants.CFNetworkLibrary)]
		extern static /* CFHTTPAuthenticationRef */ IntPtr CFHTTPAuthenticationCreateFromResponse (/* CFAllocatorRef */ IntPtr alloc, /* CFHTTPMessageRef */ IntPtr response);

		public static CFHTTPAuthentication CreateFromResponse (CFHTTPMessage response)
		{
			if (response == null)
				throw new ArgumentNullException ("response");

			if (response.IsRequest)
				throw new InvalidOperationException ();

			var handle = CFHTTPAuthenticationCreateFromResponse (IntPtr.Zero, response.Handle);
			if (handle == IntPtr.Zero)
				return null;

			return new CFHTTPAuthentication (handle);
		}

		[DllImport (Constants.CFNetworkLibrary)]
		extern static /* Boolean */ bool CFHTTPAuthenticationIsValid (/* CFHTTPAuthenticationRef */ IntPtr auth, /* CFStreamError* */ IntPtr error);

		public bool IsValid {
			get { return CFHTTPAuthenticationIsValid (Handle, IntPtr.Zero); }
		}

		[DllImport (Constants.CFNetworkLibrary)]
		extern static /* Boolean */ bool CFHTTPAuthenticationAppliesToRequest (/* CFHTTPAuthenticationRef */ IntPtr auth, /* CFHTTPMessageRef */ IntPtr request);

		public bool AppliesToRequest (CFHTTPMessage request)
		{
			if (request == null)
				throw new ArgumentNullException ("request");

			if (!request.IsRequest)
				throw new InvalidOperationException ();

			return CFHTTPAuthenticationAppliesToRequest (Handle, request.Handle);
		}

		[DllImport (Constants.CFNetworkLibrary)]
		extern static /* Boolean */ bool CFHTTPAuthenticationRequiresAccountDomain (/* CFHTTPAuthenticationRef */ IntPtr auth);

		public bool RequiresAccountDomain {
			get { return CFHTTPAuthenticationRequiresAccountDomain (Handle); }
		}

		[DllImport (Constants.CFNetworkLibrary)]
		extern static /* Boolean */ bool CFHTTPAuthenticationRequiresOrderedRequests (/* CFHTTPAuthenticationRef */ IntPtr auth);

		public bool RequiresOrderedRequests {
			get { return CFHTTPAuthenticationRequiresOrderedRequests (Handle); }
		}

		[DllImport (Constants.CFNetworkLibrary)]
		extern static /* Boolean */ bool CFHTTPAuthenticationRequiresUserNameAndPassword (/* CFHTTPAuthenticationRef */ IntPtr auth);

		public bool RequiresUserNameAndPassword {
			get { return CFHTTPAuthenticationRequiresUserNameAndPassword (Handle); }
		}

		[DllImport (Constants.CFNetworkLibrary)]
		extern static /* CFString */ IntPtr CFHTTPAuthenticationCopyMethod (/* CFHTTPAuthenticationRef */ IntPtr auth);

		public string GetMethod ()
		{
			var ptr = CFHTTPAuthenticationCopyMethod (Handle);
			if (ptr == IntPtr.Zero)
				return null;
			using (var method = new CFString (ptr))
				return method;
		}
	}
}
