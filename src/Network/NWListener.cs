//
// NWListener.cs: Bindings the Netowrk nw_listener_t API
//
// Authors:
//   Miguel de Icaza (miguel@microsoft.com)
//
// Copyrigh 2018 Microsoft Inc
//
using System;
using System.Runtime.InteropServices;
using ObjCRuntime;
using Foundation;
using CoreFoundation;

namespace Network {
	public enum NWListenerState {
		Invalid = 0,
		Waiting = 1,
		Ready = 2,
		Failed = 3,
		Cancelled = 4,
			
	}
	
	public class NWListener : INativeObject, IDisposable {
		IntPtr handle;
		public IntPtr Handle {
			get { return handle; }
		}

		public NWListener (IntPtr handle, bool owns)
		{
			this.handle = handle;
			if (owns == false)
				CFObject.CFRetain (handle);
		}

		public NWListener (IntPtr handle) : this (handle, false)
		{
		}

		~NWListener ()
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
		extern static IntPtr nw_listener_create_with_port(string port, IntPtr nwparameters);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public static NWListener Create (string port, NWParameters parameters)
		{
			IntPtr handle;

			if (parameters == null)
				throw new ArgumentNullException (nameof (parameters));
			if (port == null)
				throw new ArgumentNullException (nameof (parameters));
			
			handle = nw_listener_create_with_port (port, parameters.handle);
			if (handle == IntPtr.Zero)
				return null;
			return new NWListener (handle);
		}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		extern static IntPtr nw_listener_create (IntPtr nwparameters);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public static NWListener Create (NWParameters parameters)
		{
			IntPtr handle;

			if (parameters == null)
				throw new ArgumentNullException (nameof (parameters));
			
			handle = nw_listener_create (parameters.handle);
			if (handle == IntPtr.Zero)
				return null;
			return new NWListener (handle);
		}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		extern static IntPtr nw_listener_create_with_connection (IntPtr nwconnection, IntPtr nwparameters);


		[TV (12,0), Mac (10,14), iOS (12,0)]
		public static NWListener Create (NWConnection connection, NWParameters parameters)
		{
			if (parameters == null)
				throw new ArgumentNullException (nameof (parameters));
			if (connection == null)
				throw new ArgumentNullException (nameof (connection));
			
			var handle = nw_listener_create_with_connection (connection.handle, parameters.handle);
			if (handle == IntPtr.Zero)
				return null;
			return new NWListener (handle);
		}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_listener_set_queue(IntPtr listener, IntPtr queue);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void SetQueue (DispatchQueue queue)
		{
			if (queue == null)
			 	throw new ArgumentNullException (nameof(queue));

			nw_listener_set_queue (handle, queue.handle);
		}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		extern static ushort nw_listener_get_port (IntPtr listener);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public ushort Port => nw_listener_get_port (handle);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		extern static void 	nw_listener_start (IntPtr handle);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void Start () => nw_listener_start (handle);
		

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.NetworkLibrary)]
		extern static void 	nw_listener_cancel (IntPtr handle);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void Cancel () => nw_listener_cancel (handle);
		
	}
}
