//
// NWListener.cs: Bindings the Netowrk nw_listener_t API
//
// Authors:
//   Miguel de Icaza (miguel@microsoft.com)
//
// Copyrigh 2018 Microsoft Inc
//

#nullable enable

using System;
using System.Runtime.InteropServices;
using ObjCRuntime;
using Foundation;
using CoreFoundation;

using nw_connection_group_t = System.IntPtr;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace Network {

#if NET
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
#else
	[Watch (6, 0)]
#endif
	public class NWListener : NativeObject {
		bool connectionHandlerWasSet = false;
		object connectionHandlerLock = new object ();
		[Preserve (Conditional = true)]
#if NET
		internal NWListener (NativeHandle handle, bool owns) : base (handle, owns)
#else
		public NWListener (NativeHandle handle, bool owns) : base (handle, owns)
#endif
		{
		}

		[DllImport (Constants.NetworkLibrary)]
		extern static IntPtr nw_listener_create_with_port (IntPtr port, IntPtr nwparameters);

		public static NWListener? Create (string port, NWParameters parameters)
		{
			IntPtr handle;

			if (parameters is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (parameters));
			if (port is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (port));

			using var portPtr = new TransientString (port);
			handle = nw_listener_create_with_port (portPtr, parameters.Handle);
			if (handle == IntPtr.Zero)
				return null;
			return new NWListener (handle, owns: true);
		}

		[DllImport (Constants.NetworkLibrary)]
		extern static IntPtr nw_listener_create (IntPtr nwparameters);

		public static NWListener? Create (NWParameters parameters)
		{
			IntPtr handle;

			if (parameters is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (parameters));

			handle = nw_listener_create (parameters.Handle);
			if (handle == IntPtr.Zero)
				return null;
			return new NWListener (handle, owns: true);
		}

		[DllImport (Constants.NetworkLibrary)]
		extern static IntPtr nw_listener_create_with_connection (IntPtr nwconnection, IntPtr nwparameters);

		public static NWListener? Create (NWConnection connection, NWParameters parameters)
		{
			if (parameters is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (parameters));
			if (connection is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (connection));

			var handle = nw_listener_create_with_connection (connection.Handle, parameters.Handle);
			if (handle == IntPtr.Zero)
				return null;
			return new NWListener (handle, owns: true);
		}

		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_listener_set_queue (IntPtr listener, IntPtr queue);

		public void SetQueue (DispatchQueue queue)
		{
			if (queue is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (queue));

			nw_listener_set_queue (GetCheckedHandle (), queue.Handle);
		}

		[DllImport (Constants.NetworkLibrary)]
		extern static ushort nw_listener_get_port (IntPtr listener);

		public ushort Port => nw_listener_get_port (GetCheckedHandle ());

		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_listener_start (IntPtr handle);

		public void Start ()
		{
			lock (connectionHandlerLock) {
				// we will get a sigabort if the handler is not set, lets be nicer.
				if (!connectionHandlerWasSet)
					throw new InvalidOperationException ("A connection handler should be set before starting a NWListener.");
				nw_listener_start (GetCheckedHandle ());
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_listener_cancel (IntPtr handle);

		public void Cancel () => nw_listener_cancel (GetCheckedHandle ());

#if !NET
		delegate void nw_listener_state_changed_handler_t (IntPtr block, NWListenerState state, IntPtr nwerror);
		static nw_listener_state_changed_handler_t static_ListenerStateChanged = TrampolineListenerStateChanged;

		[MonoPInvokeCallback (typeof (nw_listener_state_changed_handler_t))]
#else
		[UnmanagedCallersOnly]
#endif
		static void TrampolineListenerStateChanged (IntPtr block, NWListenerState state, IntPtr nwerror)
		{
			var del = BlockLiteral.GetTarget<Action<NWListenerState, NWError?>> (block);
			if (del is not null) {
				NWError? err = nwerror == IntPtr.Zero ? null : new NWError (nwerror, owns: false);
				del (state, err);
				err?.Dispose ();
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern unsafe void nw_listener_set_state_changed_handler (IntPtr handle, BlockLiteral* callback);

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void SetStateChangedHandler (Action<NWListenerState, NWError?> callback)
		{
			unsafe {
				if (callback is null) {
					nw_listener_set_state_changed_handler (GetCheckedHandle (), null);
					return;
				}

#if NET
				delegate* unmanaged<IntPtr, NWListenerState, IntPtr, void> trampoline = &TrampolineListenerStateChanged;
				using var block = new BlockLiteral (trampoline, callback, typeof (NWListener), nameof (TrampolineListenerStateChanged));
#else
				var block = new BlockLiteral ();
				block.SetupBlockUnsafe (static_ListenerStateChanged, callback);
#endif
				nw_listener_set_state_changed_handler (GetCheckedHandle (), &block);
			}
		}

#if !NET
		delegate void nw_listener_new_connection_handler_t (IntPtr block, IntPtr connection);
		static nw_listener_new_connection_handler_t static_NewConnection = TrampolineNewConnection;

		[MonoPInvokeCallback (typeof (nw_listener_new_connection_handler_t))]
#else
		[UnmanagedCallersOnly]
#endif
		static void TrampolineNewConnection (IntPtr block, IntPtr connection)
		{
			var del = BlockLiteral.GetTarget<Action<NWConnection>> (block);
			if (del is not null) {
				var nwconnection = new NWConnection (connection, owns: false);
				del (nwconnection);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern unsafe void nw_listener_set_new_connection_handler (IntPtr handle, BlockLiteral* callback);

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void SetNewConnectionHandler (Action<NWConnection> callback)
		{
			lock (connectionHandlerLock) {
				unsafe {
					if (callback is null) {
						nw_listener_set_new_connection_handler (GetCheckedHandle (), null);
						return;
					}

#if NET
					delegate* unmanaged<IntPtr, IntPtr, void> trampoline = &TrampolineNewConnection;
					using var block = new BlockLiteral (trampoline, callback, typeof (NWListener), nameof (TrampolineNewConnection));
#else
					using var block = new BlockLiteral ();
					block.SetupBlockUnsafe (static_NewConnection, callback);
#endif
					nw_listener_set_new_connection_handler (GetCheckedHandle (), &block);
					connectionHandlerWasSet = true;
				}
			}
		}

#if !NET
		delegate void nw_listener_advertised_endpoint_changed_handler_t (IntPtr block, IntPtr endpoint, byte added);
		static nw_listener_advertised_endpoint_changed_handler_t static_AdvertisedEndpointChangedHandler = TrampolineAdvertisedEndpointChangedHandler;
#endif

		public delegate void AdvertisedEndpointChanged (NWEndpoint endpoint, bool added);

#if !NET
		[MonoPInvokeCallback (typeof (nw_listener_advertised_endpoint_changed_handler_t))]
#else
		[UnmanagedCallersOnly]
#endif
		static void TrampolineAdvertisedEndpointChangedHandler (IntPtr block, IntPtr endpoint, byte added)
		{
			var del = BlockLiteral.GetTarget<AdvertisedEndpointChanged> (block);
			if (del is not null) {
				using var nwendpoint = new NWEndpoint (endpoint, owns: false);
				del (nwendpoint, added != 0 ? true : false);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		static extern unsafe void nw_listener_set_advertised_endpoint_changed_handler (IntPtr handle, BlockLiteral* callback);

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void SetAdvertisedEndpointChangedHandler (AdvertisedEndpointChanged callback)
		{
			unsafe {
				if (callback is null) {
					nw_listener_set_advertised_endpoint_changed_handler (GetCheckedHandle (), null);
					return;
				}

#if NET
				delegate* unmanaged<IntPtr, IntPtr, byte, void> trampoline = &TrampolineAdvertisedEndpointChangedHandler;
				using var block = new BlockLiteral (trampoline, callback, typeof (NWListener), nameof (TrampolineAdvertisedEndpointChangedHandler));
#else
				using var block = new BlockLiteral ();
				block.SetupBlockUnsafe (static_AdvertisedEndpointChangedHandler, callback);
#endif
				nw_listener_set_advertised_endpoint_changed_handler (GetCheckedHandle (), &block);
			}
		}

		[DllImport (Constants.NetworkLibrary)]
		extern static void nw_listener_set_advertise_descriptor (IntPtr handle, IntPtr advertiseDescriptor);

		public void SetAdvertiseDescriptor (NWAdvertiseDescriptor descriptor)
		{
			nw_listener_set_advertise_descriptor (GetCheckedHandle (), descriptor.GetHandle ());
		}

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[TV (13, 0)]
		[iOS (13, 0)]
#endif
		[DllImport (Constants.NetworkLibrary)]
		static extern uint nw_listener_get_new_connection_limit (IntPtr listener);

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[TV (13, 0)]
		[iOS (13, 0)]
#endif
		[DllImport (Constants.NetworkLibrary)]
		static extern void nw_listener_set_new_connection_limit (IntPtr listener, uint new_connection_limit);

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[TV (13, 0)]
		[iOS (13, 0)]
#endif
		public uint ConnectionLimit {
			get => nw_listener_get_new_connection_limit (GetCheckedHandle ());
			set => nw_listener_set_new_connection_limit (GetCheckedHandle (), value);
		}

#if NET
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos12.0")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst15.0")]
#else
		[Watch (8, 0)]
		[TV (15, 0)]
		[Mac (12, 0)]
		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
#endif
		[DllImport (Constants.NetworkLibrary)]
		unsafe static extern void nw_listener_set_new_connection_group_handler (IntPtr listener, /* [NullAllowed] */ BlockLiteral* handler);

#if !NET
		delegate void nw_listener_new_connection_group_handler_t (IntPtr block, nw_connection_group_t group);
		static nw_listener_new_connection_group_handler_t static_NewConnectionGroup = TrampolineNewConnectionGroup;

		[MonoPInvokeCallback (typeof (nw_listener_new_connection_group_handler_t))]
#else
		[UnmanagedCallersOnly]
#endif
		static void TrampolineNewConnectionGroup (IntPtr block, nw_connection_group_t connectionGroup)
		{
			var del = BlockLiteral.GetTarget<Action<NWConnectionGroup>> (block);
			if (del is null)
				return;
			using var nwConnectionGroup = new NWConnectionGroup (connectionGroup, owns: false);
			del (nwConnectionGroup);
		}

#if NET
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos12.0")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst15.0")]
#else
		[Watch (8, 0)]
		[TV (15, 0)]
		[Mac (12, 0)]
		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
#endif
		[BindingImpl (BindingImplOptions.Optimizable)]
		public void SetNewConnectionGroupHandler (Action<NWConnectionGroup> handler)
		{
			unsafe {
#if NET
				delegate* unmanaged<IntPtr, nw_connection_group_t, void> trampoline = &TrampolineNewConnectionGroup;
				using var block = new BlockLiteral (trampoline, handler, typeof (NWListener), nameof (TrampolineNewConnectionGroup));
#else
				using var block = new BlockLiteral ();
				block.SetupBlockUnsafe (static_NewConnectionGroup, handler);
#endif
				nw_listener_set_new_connection_group_handler (GetCheckedHandle (), &block);
			}
		}
	}
}
