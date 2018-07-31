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

	[TV (12,0), Mac (10,14), iOS (12,0)]
	public class NWListener : NativeObject {
		public NWListener (IntPtr handle, bool owns) : base (handle, owns)
		{
		}

		[DllImport (Constants.NetworkLibrary)]
		extern static IntPtr nw_listener_create_with_port (string port, IntPtr nwparameters);

		public static NWListener Create (string port, NWParameters parameters)
		{
			IntPtr handle;

			if (parameters == null)
				throw new ArgumentNullException (nameof (parameters));
			if (port == null)
				throw new ArgumentNullException (nameof (port));

			handle = nw_listener_create_with_port (port, parameters.handle);
			if (handle == IntPtr.Zero)
				return null;
			return new NWListener (handle, owns: true);
		}

		[DllImport (Constants.NetworkLibrary)]
		extern static IntPtr nw_listener_create (IntPtr nwparameters);

		public static NWListener Create (NWParameters parameters)
		{
			IntPtr handle;

			if (parameters == null)
				throw new ArgumentNullException (nameof (parameters));

			handle = nw_listener_create (parameters.handle);
			if (handle == IntPtr.Zero)
				return null;
			return new NWListener (handle, owns: true);
		}

		[DllImport (Constants.NetworkLibrary)]
		extern static IntPtr nw_listener_create_with_connection (IntPtr nwconnection, IntPtr nwparameters);

		public static NWListener Create (NWConnection connection, NWParameters parameters)
		{
			if (parameters == null)
				throw new ArgumentNullException (nameof (parameters));
			if (connection == null)
				throw new ArgumentNullException (nameof (connection));

			var handle = nw_listener_create_with_connection (connection.handle, parameters.handle);
			if (handle == IntPtr.Zero)
				return null;
			return new NWListener (handle, owns: true);
		}

		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_listener_set_queue (IntPtr listener, IntPtr queue);

		public void SetQueue (DispatchQueue queue)
		{
			if (queue == null)
				throw new ArgumentNullException (nameof (queue));

			nw_listener_set_queue (GetHandle(), queue.handle);
		}

		[DllImport (Constants.NetworkLibrary)]
		extern static ushort nw_listener_get_port (IntPtr listener);

		public ushort Port => nw_listener_get_port (GetHandle());

		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_listener_start (IntPtr handle);

		public void Start () => nw_listener_start (GetHandle());

		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_listener_cancel (IntPtr handle);

		public void Cancel () => nw_listener_cancel (GetHandle());

		delegate void nw_listener_state_changed_handler_t (IntPtr block, NWListenerState state, IntPtr nwerror);
		static nw_listener_state_changed_handler_t static_ListenerStateChanged = TrampolineListenerStateChanged;

		[MonoPInvokeCallback (typeof (nw_listener_state_changed_handler_t))]
		static unsafe void TrampolineListenerStateChanged (IntPtr block, NWListenerState state, IntPtr nwerror)
		{
			var descriptor = (BlockLiteral *) block;
			var del = (Action<NWListenerState,NWError>) (descriptor->Target);
			if (del != null){
				NWError err = nwerror == IntPtr.Zero ? null : new NWError (nwerror, owns: false);
				del (state, err);
				err?.Dispose ();
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern unsafe void nw_listener_set_state_changed_handler (IntPtr handle, void *callback);

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void SetStateChangedHandler (Action<NWListenerState,NWError> callback)
		{
			unsafe {
				if (callback == null){
					nw_listener_set_state_changed_handler (GetHandle(), null);
					return;
				}

				BlockLiteral *block_ptr_handler;
				BlockLiteral block_handler;
				block_handler = new BlockLiteral ();
				block_ptr_handler = &block_handler;
				block_handler.SetupBlockUnsafe (static_ListenerStateChanged, callback);

				nw_listener_set_state_changed_handler (GetHandle(), (void*) block_ptr_handler);
				block_ptr_handler->CleanupBlock ();
			}
		}

		delegate void nw_listener_new_connection_handler_t (IntPtr block, IntPtr connection);
		static nw_listener_new_connection_handler_t static_NewConnection = TrampolineNewConnection;

		[MonoPInvokeCallback (typeof (nw_listener_new_connection_handler_t))]
		static unsafe void TrampolineNewConnection (IntPtr block, IntPtr connection)
		{
			var descriptor = (BlockLiteral *) block;
			var del = (Action<NWConnection>) (descriptor->Target);
			if (del != null){
				var nwconnection = new NWConnection (connection, owns: false);
			        del (nwconnection);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern unsafe void nw_listener_set_new_connection_handler (IntPtr handle, void *callback);

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void SetNewConnectionHandler (Action<NWConnection> callback)
		{
			unsafe {
				if (callback == null){
					nw_listener_set_new_connection_handler (GetHandle(), null);
					return;
				}

				BlockLiteral *block_ptr_handler;
				BlockLiteral block_handler;
				block_handler = new BlockLiteral ();
				block_ptr_handler = &block_handler;
				block_handler.SetupBlockUnsafe (static_NewConnection, callback);

				nw_listener_set_new_connection_handler (GetHandle(), (void*) block_ptr_handler);
				block_ptr_handler->CleanupBlock ();
			}
		}

		delegate void nw_listener_advertised_endpoint_changed_handler_t (IntPtr block, IntPtr endpoint, byte added);
		static nw_listener_advertised_endpoint_changed_handler_t static_AdvertisedEndpointChangedHandler = TrampolineAdvertisedEndpointChangedHandler;

		public delegate void AdvertisedEndpointChanged (NWEndpoint endpoint, bool added);

		[MonoPInvokeCallback (typeof (nw_listener_advertised_endpoint_changed_handler_t))]
		static unsafe void TrampolineAdvertisedEndpointChangedHandler (IntPtr block, IntPtr endpoint, byte added)
		{
			var descriptor = (BlockLiteral *) block;
			var del = (AdvertisedEndpointChanged) (descriptor->Target);
			if (del != null) {
				var nwendpoint = new NWEndpoint (endpoint, owns: false);
				del (nwendpoint, added != 0 ? true : false);
				nwendpoint.Dispose ();
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern unsafe void nw_listener_set_advertised_endpoint_changed_handler (IntPtr handle, void *callback);

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void SetAdvertisedEndpointChangedHandler (AdvertisedEndpointChanged callback)
		{
			unsafe {
				if (callback == null){
					nw_listener_set_advertised_endpoint_changed_handler (GetHandle(), null);
					return;
				}

				BlockLiteral *block_ptr_handler;
				BlockLiteral block_handler;
				block_handler = new BlockLiteral ();
				block_ptr_handler = &block_handler;
				block_handler.SetupBlockUnsafe (static_AdvertisedEndpointChangedHandler, callback);

				nw_listener_set_advertised_endpoint_changed_handler (GetHandle(), (void*) block_ptr_handler);
				block_ptr_handler->CleanupBlock ();
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_listener_set_advertise_descriptor (IntPtr handle, IntPtr advertiseDescriptor);

		public void SetAdvertiseDescriptor (NWAdvertiseDescriptor descriptor)
		{
			nw_listener_set_advertise_descriptor (GetHandle(), descriptor == null ? IntPtr.Zero : descriptor.handle);
		}
	}
}
