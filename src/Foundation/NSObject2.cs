// Copyright 2011 - 2014 Xamarin Inc
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using System;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
#if !NO_SYSTEM_DRAWING
using System.Drawing;
#endif
using System.Runtime.Versioning;
using System.Diagnostics;

#if NET
using System.Runtime.InteropServices.ObjectiveC;
#endif

using ObjCRuntime;
#if !COREBUILD
using Xamarin.Bundler;
#if MONOTOUCH
using UIKit;
#if !WATCH
using CoreAnimation;
#endif
#endif
using CoreGraphics;
#endif

#if !NET
using NativeHandle = System.IntPtr;
#endif

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Foundation {

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public enum NSObjectFlag {
		Empty,
	}
#else
	public class NSObjectFlag {
		public static readonly NSObjectFlag Empty;

		NSObjectFlag () { }
	}
#endif

#if NET
	// This interface will be made public when the managed static registrar is used.
	internal interface INSObjectFactory {
		// The method will be implemented via custom linker step if the managed static registrar is used
		// for NSObject subclasses which have an (NativeHandle) or (IntPtr) constructor.
		[MethodImpl(MethodImplOptions.NoInlining)]
		virtual static NSObject _Xamarin_ConstructNSObject (NativeHandle handle) => null;
	}
#endif

#if NET && !COREBUILD
	[ObjectiveCTrackedType]
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public partial class NSObject : INativeObject
#if !COREBUILD
		, IEquatable<NSObject>
		, IDisposable
#endif
#if NET
		, INSObjectFactory
#endif
	{
#if !COREBUILD
		const string selConformsToProtocol = "conformsToProtocol:";
		const string selEncodeWithCoder = "encodeWithCoder:";

#if MONOMAC
		static IntPtr selConformsToProtocolHandle = Selector.GetHandle (selConformsToProtocol);
		static IntPtr selEncodeWithCoderHandle = Selector.GetHandle (selEncodeWithCoder);
#endif

		// replace older Mono[Touch|Mac]Assembly field (ease code sharing across platforms)
		public static readonly Assembly PlatformAssembly = typeof (NSObject).Assembly;

		NativeHandle handle;
		IntPtr super; /* objc_super* */

#if !NET
		Flags flags;
#else
		// See  "Toggle-ref support for CoreCLR" in coreclr-bridge.m for more information.
		Flags actual_flags;
		internal unsafe Runtime.TrackedObjectInfo* tracked_object_info;

		unsafe Flags flags {
			get {
				// Get back the InFinalizerQueue flag, it's the only flag we'll set in the tracked object info structure from native code.
				// The InFinalizerQueue will never be cleared once set, so there's no need to unset it here if it's not set in the tracked_object_info structure.
				if (tracked_object_info is not null && ((tracked_object_info->Flags) & Flags.InFinalizerQueue) == Flags.InFinalizerQueue)
					actual_flags |= Flags.InFinalizerQueue;

				return actual_flags;
			}
			set {
				actual_flags = value;
				// Update the flags value that we can access them from the toggle ref callback as well.
				if (tracked_object_info is not null)
					tracked_object_info->Flags = value;
			}
		}
#endif // NET

		// This enum has a native counterpart in runtime.h
		[Flags]
		internal enum Flags : byte {
			Disposed = 1,
			NativeRef = 2,
			IsDirectBinding = 4,
			RegisteredToggleRef = 8,
			InFinalizerQueue = 16,
			HasManagedRef = 32,
			// 64, // Used by SoM
			IsCustomType = 128,
		}

		// Must be kept in sync with the same enum in trampolines.h
		enum XamarinGCHandleFlags : uint {
			None = 0,
			WeakGCHandle = 1,
			HasManagedRef = 2,
			InitialSet = 4,
		}

		[StructLayout (LayoutKind.Sequential)]
		struct objc_super {
			public IntPtr Handle;
			public IntPtr ClassHandle;
		}

		bool disposed {
			get { return ((flags & Flags.Disposed) == Flags.Disposed); }
			set { flags = value ? (flags | Flags.Disposed) : (flags & ~Flags.Disposed); }
		}

		bool HasManagedRef {
			get { return (flags & Flags.HasManagedRef) == Flags.HasManagedRef; }
			set { flags = value ? (flags | Flags.HasManagedRef) : (flags & ~Flags.HasManagedRef); }
		}

		internal bool IsRegisteredToggleRef {
			get { return ((flags & Flags.RegisteredToggleRef) == Flags.RegisteredToggleRef); }
			set { flags = value ? (flags | Flags.RegisteredToggleRef) : (flags & ~Flags.RegisteredToggleRef); }
		}

		[DebuggerBrowsable (DebuggerBrowsableState.Never)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		protected internal bool IsDirectBinding {
			get { return ((flags & Flags.IsDirectBinding) == Flags.IsDirectBinding); }
			set { flags = value ? (flags | Flags.IsDirectBinding) : (flags & ~Flags.IsDirectBinding); }
		}

		internal bool InFinalizerQueue {
			get { return ((flags & Flags.InFinalizerQueue) == Flags.InFinalizerQueue); }
		}

		bool IsCustomType {
			get {
				var value = (flags & Flags.IsCustomType) == Flags.IsCustomType;
				if (!value) {
					value = Class.IsCustomType (GetType ());
					if (value)
						flags |= Flags.IsCustomType;
				}
				return value;
			}
		}

		[Export ("init")]
		public NSObject ()
		{
			bool alloced = AllocIfNeeded ();
			InitializeObject (alloced);
		}

		// This is just here as a constructor chain that can will
		// only do Init at the most derived class.
		public NSObject (NSObjectFlag x)
		{
			bool alloced = AllocIfNeeded ();
			InitializeObject (alloced);
		}

		[EditorBrowsable (EditorBrowsableState.Never)]
#if NET
		protected internal NSObject (NativeHandle handle)
#else
		public NSObject (NativeHandle handle)
#endif
			: this (handle, false)
		{
		}

		[EditorBrowsable (EditorBrowsableState.Never)]
#if NET
		protected NSObject (NativeHandle handle, bool alloced)
#else
		public NSObject (NativeHandle handle, bool alloced)
#endif
		{
			this.handle = handle;
			InitializeObject (alloced);
		}

		~NSObject ()
		{
			Dispose (false);
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		internal static IntPtr CreateNSObject (IntPtr type_gchandle, IntPtr handle, Flags flags)
		{
#if NET
			if (Runtime.IsManagedStaticRegistrar) {
				throw new System.Diagnostics.UnreachableException ();
			}
#endif

			// This function is called from native code before any constructors have executed.
			var type = (Type) Runtime.GetGCHandleTarget (type_gchandle);
			try {
				var obj = (NSObject) RuntimeHelpers.GetUninitializedObject (type);
				obj.handle = handle;
				obj.flags = flags;
				return Runtime.AllocGCHandle (obj);
			} catch (Exception e) {
				throw ErrorHelper.CreateError (8041, e, Errors.MX8041 /* Unable to create an instance of the type {0} */, type.FullName);
			}
		}

		NativeHandle GetSuper ()
		{
			if (super == NativeHandle.Zero) {
				IntPtr ptr;

				unsafe {
					ptr = Marshal.AllocHGlobal (sizeof (objc_super));
					*(objc_super*) ptr = default (objc_super); // zero fill
				}

				var previousValue = Interlocked.CompareExchange (ref super, ptr, IntPtr.Zero);
				if (previousValue != IntPtr.Zero) {
					// somebody beat us to the assignment.
					Marshal.FreeHGlobal (ptr);
					ptr = IntPtr.Zero;
				}
			}

			unsafe {
				objc_super* sup = (objc_super*) super;
				if (sup->ClassHandle == NativeHandle.Zero)
					sup->ClassHandle = ClassHandle;
				sup->Handle = handle;
			}

			return super;
		}

		internal static NativeHandle Initialize ()
		{
			return class_ptr;
		}

#if NET
		internal Flags FlagsInternal {
			get { return flags; }
			set { flags = value; }
		}
#endif

#if !NET || !__MACOS__
		[MethodImplAttribute (MethodImplOptions.InternalCall)]
		extern static void RegisterToggleRef (NSObject obj, IntPtr handle, bool isCustomType);
#endif // !NET || !__MACOS__

		[DllImport ("__Internal")]
		static extern void xamarin_release_managed_ref (IntPtr handle, [MarshalAs (UnmanagedType.I1)] bool user_type);

		static void RegisterToggleReference (NSObject obj, IntPtr handle, bool isCustomType)
		{
#if NET && __MACOS__
			Runtime.RegisterToggleReferenceCoreCLR (obj, handle, isCustomType);
#elif NET
			if (Runtime.IsCoreCLR) {
				Runtime.RegisterToggleReferenceCoreCLR (obj, handle, isCustomType);
			} else {
				RegisterToggleRef (obj, handle, isCustomType);
			}
#else
			RegisterToggleRef (obj, handle, isCustomType);
#endif
		}

#if !XAMCORE_3_0
		[EditorBrowsable (EditorBrowsableState.Never)]
		public static bool IsNewRefcountEnabled ()
		{
			return true;
		}
#endif

		/*
		Register the current object with the toggleref machinery if the following conditions are met:
		-The new refcounting is enabled; and
		-The class is not a custom type - it must wrap a framework class.
		*/
		[EditorBrowsable (EditorBrowsableState.Never)]
		protected void MarkDirty ()
		{
			MarkDirty (false);
		}

		internal void MarkDirty (bool allowCustomTypes)
		{
			if (IsRegisteredToggleRef)
				return;

			if (!allowCustomTypes && IsCustomType)
				return;

			IsRegisteredToggleRef = true;
			RegisterToggleReference (this, Handle, allowCustomTypes);
		}

		private void InitializeObject (bool alloced)
		{
			if (alloced && handle == NativeHandle.Zero && Class.ThrowOnInitFailure) {
				if (ClassHandle == NativeHandle.Zero)
					throw new Exception ($"Could not create an native instance of the type '{GetType ().FullName}': the native class hasn't been loaded.\n{Constants.SetThrowOnInitFailureToFalse}.");
				throw new Exception ($"Could not create an native instance of the type '{new Class (ClassHandle).Name}'.\n{Constants.SetThrowOnInitFailureToFalse}.");
			}

			// The authorative value for the IsDirectBinding value is the register attribute:
			//
			//     [Register ("MyClass", true)] // the second parameter specifies the IsDirectBinding value
			//     class MyClass : NSObject {}
			//
			// Unfortunately looking up this attribute every time a class is instantiated is
			// slow (since fetching attributes is slow), so we guess here: if the actual type
			// of the object is in the platform assembly, then we assume IsDirectBinding=true:
			//
			// IsDirectBinding = (this.GetType ().Assembly == PlatformAssembly);
			//
			// and any subclasses in the platform assembly which is not a direct binding have
			// to set the correct value in their constructors.
			IsDirectBinding = (this.GetType ().Assembly == PlatformAssembly);
			Runtime.RegisterNSObject (this, handle);

			bool native_ref = (flags & Flags.NativeRef) == Flags.NativeRef;
			CreateManagedRef (!alloced || native_ref);
		}

		[DllImport ("__Internal")]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool xamarin_set_gchandle_with_flags_safe (IntPtr handle, IntPtr gchandle, XamarinGCHandleFlags flags);

		void CreateManagedRef (bool retain)
		{
			HasManagedRef = true;
			bool isUserType = Runtime.IsUserType (handle);
			if (isUserType) {
				var flags = XamarinGCHandleFlags.HasManagedRef | XamarinGCHandleFlags.InitialSet | XamarinGCHandleFlags.WeakGCHandle;
				var gchandle = GCHandle.Alloc (this, GCHandleType.WeakTrackResurrection);
				var h = GCHandle.ToIntPtr (gchandle);
				if (!xamarin_set_gchandle_with_flags_safe (handle, h, flags)) {
					// A GCHandle already existed: this shouldn't happen, but let's handle it anyway.
					Runtime.NSLog ($"Tried to create a managed reference from an object that already has a managed reference (type: {GetType ()})");
					gchandle.Free ();
				}
			}

			if (retain)
				DangerousRetain ();
		}

		void ReleaseManagedRef ()
		{
			var handle = this.Handle; // Get a copy of the handle, because it will be cleared out when calling Runtime.NativeObjectHasDied, and we still need the handle later.
			var user_type = Runtime.IsUserType (handle);
			HasManagedRef = false;
			if (!user_type) {
				/* If we're a wrapper type, we need to unregister here, since we won't enter the release trampoline */
				Runtime.NativeObjectHasDied (handle, this);
			}
			xamarin_release_managed_ref (handle, user_type);
			FreeData ();
		}

		static bool IsProtocol (Type type, IntPtr protocol)
		{
			while (type != typeof (NSObject) && type is not null) {
				var attrs = type.GetCustomAttributes (typeof (ProtocolAttribute), false);
				var protocolAttribute = (ProtocolAttribute) (attrs.Length > 0 ? attrs [0] : null);
				if (protocolAttribute is not null && !protocolAttribute.IsInformal) {
					string name;

					if (!string.IsNullOrEmpty (protocolAttribute.Name)) {
						name = protocolAttribute.Name;
					} else {
						attrs = type.GetCustomAttributes (typeof (RegisterAttribute), false);
						var registerAttribute = (RegisterAttribute) (attrs.Length > 0 ? attrs [0] : null);
						if (registerAttribute is not null && !string.IsNullOrEmpty (registerAttribute.Name)) {
							name = registerAttribute.Name;
						} else {
							name = type.Name;
						}
					}

					var proto = Runtime.GetProtocol (name);
					if (proto != IntPtr.Zero && proto == protocol)
						return true;
				}
				type = type.BaseType;
			}

			return false;
		}

		[Preserve]
		bool InvokeConformsToProtocol (NativeHandle protocol)
		{
			return ConformsToProtocol (protocol);
		}

		[Export ("conformsToProtocol:")]
		[Preserve ()]
		[BindingImpl (BindingImplOptions.Optimizable)]
		public virtual bool ConformsToProtocol (NativeHandle protocol)
		{
			bool does;
			bool is_wrapper = IsDirectBinding;
			bool is_third_party;

			if (is_wrapper) {
				is_third_party = this.GetType ().Assembly != NSObject.PlatformAssembly;
				if (is_third_party) {
					// Third-party bindings might lie about IsDirectBinding (see bug #14772),
					// so don't trust any 'true' values unless we're in monotouch.dll.
					var attribs = this.GetType ().GetCustomAttributes (typeof (RegisterAttribute), false);
					if (attribs is not null && attribs.Length == 1)
						is_wrapper = ((RegisterAttribute) attribs [0]).IsWrapper;
				}
			}

#if MONOMAC
			if (is_wrapper) {
				does = Messaging.bool_objc_msgSend_IntPtr (this.Handle, selConformsToProtocolHandle, protocol) != 0;
			} else {
				does = Messaging.bool_objc_msgSendSuper_IntPtr (this.SuperHandle, selConformsToProtocolHandle, protocol) != 0;
			}
#else
			if (is_wrapper) {
				does = Messaging.bool_objc_msgSend_IntPtr (this.Handle, Selector.GetHandle (selConformsToProtocol), protocol) != 0;
			} else {
				does = Messaging.bool_objc_msgSendSuper_IntPtr (this.SuperHandle, Selector.GetHandle (selConformsToProtocol), protocol) != 0;
			}
#endif

			if (does)
				return true;

			if (!Runtime.DynamicRegistrationSupported)
				return false;

			// the linker/trimmer will remove the following code if the dynamic registrar is removed from the app
			var classHandle = ClassHandle;
			lock (Runtime.protocol_cache) {
#if NET
				ref var map = ref CollectionsMarshal.GetValueRefOrAddDefault (Runtime.protocol_cache, classHandle, out var exists);
				if (!exists)
					map = new ();
				ref var result = ref CollectionsMarshal.GetValueRefOrAddDefault (map, protocol, out exists);
				if (!exists)
					result = DynamicConformsToProtocol (protocol);
#else
				bool new_map = false;
				if (!Runtime.protocol_cache.TryGetValue (classHandle, out var map)) {
					map = new ();
					new_map = true;
					Runtime.protocol_cache.Add (classHandle, map);
				}
				if (new_map || !map.TryGetValue (protocol, out var result)) {
					result = DynamicConformsToProtocol (protocol);
					map.Add (protocol, result);
				}
#endif
				return result;
			}
		}

		bool DynamicConformsToProtocol (NativeHandle protocol)
		{
#if NET
			if (Runtime.IsNativeAOT)
				throw Runtime.CreateNativeAOTNotSupportedException ();
#endif

			object [] adoptedProtocols = GetType ().GetCustomAttributes (typeof (AdoptsAttribute), true);
			foreach (AdoptsAttribute adopts in adoptedProtocols) {
				if (adopts.ProtocolHandle == protocol)
					return true;
			}

			// Check if this class or any of the interfaces
			// it implements are protocols.

			if (IsProtocol (GetType (), protocol))
				return true;

			var ifaces = GetType ().GetInterfaces ();
			foreach (var iface in ifaces) {
				if (IsProtocol (iface, protocol))
					return true;
			}

			return false;
		}

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		public void DangerousRelease ()
		{
			DangerousRelease (handle);
		}

		internal static void DangerousRelease (NativeHandle handle)
		{
			if (handle == IntPtr.Zero)
				return;
#if MONOMAC
			Messaging.void_objc_msgSend (handle, Selector.ReleaseHandle);
#else
			Messaging.void_objc_msgSend (handle, Selector.GetHandle (Selector.Release));
#endif
		}

		internal static void DangerousRetain (NativeHandle handle)
		{
			if (handle == IntPtr.Zero)
				return;
#if MONOMAC
			Messaging.void_objc_msgSend (handle, Selector.RetainHandle);
#else
			Messaging.void_objc_msgSend (handle, Selector.GetHandle (Selector.Retain));
#endif
		}

		internal static void DangerousAutorelease (NativeHandle handle)
		{
#if MONOMAC
			Messaging.void_objc_msgSend (handle, Selector.AutoreleaseHandle);
#else
			Messaging.void_objc_msgSend (handle, Selector.GetHandle (Selector.Autorelease));
#endif
		}

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		public NSObject DangerousRetain ()
		{
#if MONOMAC
			Messaging.void_objc_msgSend (handle, Selector.RetainHandle);
#else
			Messaging.void_objc_msgSend (handle, Selector.GetHandle (Selector.Retain));
#endif
			return this;
		}

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		public NSObject DangerousAutorelease ()
		{
#if MONOMAC
			Messaging.void_objc_msgSend (handle, Selector.AutoreleaseHandle);
#else
			Messaging.void_objc_msgSend (handle, Selector.GetHandle (Selector.Autorelease));
#endif
			return this;
		}

		[EditorBrowsable (EditorBrowsableState.Never)]
		public NativeHandle SuperHandle {
			get {
				if (handle == IntPtr.Zero)
					ObjCRuntime.ThrowHelper.ThrowObjectDisposedException (this);

				return GetSuper ();
			}
		}

		[EditorBrowsable (EditorBrowsableState.Never)]
		public NativeHandle Handle {
			get { return handle; }
			set {
				if (handle == value)
					return;

				if (handle != IntPtr.Zero)
					Runtime.UnregisterNSObject (handle);

				handle = value;

#if NET
				unsafe {
					if (tracked_object_info is not null)
						tracked_object_info->Handle = value;
				}
#endif

				if (handle != IntPtr.Zero)
					Runtime.RegisterNSObject (this, handle);
			}
		}

		[EditorBrowsable (EditorBrowsableState.Never)]
		protected void InitializeHandle (NativeHandle handle)
		{
			InitializeHandle (handle, "init*");
		}

		[EditorBrowsable (EditorBrowsableState.Never)]
		protected void InitializeHandle (NativeHandle handle, string initSelector)
		{
			if (this.handle == NativeHandle.Zero && Class.ThrowOnInitFailure) {
				if (ClassHandle == NativeHandle.Zero)
					throw new Exception ($"Could not create an native instance of the type '{GetType ().FullName}': the native class hasn't been loaded.\n{Constants.SetThrowOnInitFailureToFalse}.");
				throw new Exception ($"Could not create an native instance of the type '{new Class (ClassHandle).Name}'.\n{Constants.SetThrowOnInitFailureToFalse}.");
			}

			if (handle == NativeHandle.Zero && Class.ThrowOnInitFailure) {
				Handle = NativeHandle.Zero; // We'll crash if we don't do this.
				throw new Exception ($"Could not initialize an instance of the type '{GetType ().FullName}': the native '{initSelector}' method returned nil.\n{Constants.SetThrowOnInitFailureToFalse}.");
			}

			this.Handle = handle;
		}

		private bool AllocIfNeeded ()
		{
			if (handle == NativeHandle.Zero) {
#if MONOMAC
				handle = Messaging.IntPtr_objc_msgSend (Class.GetHandle (this.GetType ()), Selector.AllocHandle);
#else
				handle = Messaging.IntPtr_objc_msgSend (Class.GetHandle (this.GetType ()), Selector.GetHandle (Selector.Alloc));
#endif
				return true;
			}
			return false;
		}

#if !XAMCORE_3_0
		private IntPtr GetObjCIvar (string name)
		{
			IntPtr native;

			object_getInstanceVariable (handle, name, out native);

			return native;
		}

		[Obsolete ("Do not use; this API does not properly retain/release existing/new values, so leaks and/or crashes may occur.")]
		public NSObject GetNativeField (string name)
		{
			IntPtr field = GetObjCIvar (name);

			if (field == IntPtr.Zero)
				return null;
			return Runtime.GetNSObject (field);
		}

		private void SetObjCIvar (string name, IntPtr value)
		{
			object_setInstanceVariable (handle, name, value);
		}

		[Obsolete ("Do not use; this API does not properly retain/release existing/new values, so leaks and/or crashes may occur.")]
		public void SetNativeField (string name, NSObject value)
		{
			if (value is null)
				SetObjCIvar (name, IntPtr.Zero);
			else
				SetObjCIvar (name, value.Handle);
		}

		[DllImport (Messaging.LIBOBJC_DYLIB)]
		extern static void object_getInstanceVariable (IntPtr obj, string name, out IntPtr val);

		[DllImport (Messaging.LIBOBJC_DYLIB)]
		extern static void object_setInstanceVariable (IntPtr obj, string name, IntPtr val);
#endif // !XAMCORE_3_0

		private void InvokeOnMainThread (Selector sel, NSObject obj, bool wait)
		{
#if NET
			Messaging.void_objc_msgSend_NativeHandle_NativeHandle_bool (this.Handle, Selector.GetHandle (Selector.PerformSelectorOnMainThreadWithObjectWaitUntilDone), sel.Handle, obj.GetHandle (), wait ? (byte) 1 : (byte) 0);
#else
#if MONOMAC
			Messaging.void_objc_msgSend_IntPtr_IntPtr_bool (this.Handle, Selector.PerformSelectorOnMainThreadWithObjectWaitUntilDoneHandle, sel.Handle, obj.GetHandle (), wait ? (byte) 1 : (byte) 0);
#else
			Messaging.void_objc_msgSend_IntPtr_IntPtr_bool (this.Handle, Selector.GetHandle (Selector.PerformSelectorOnMainThreadWithObjectWaitUntilDone), sel.Handle, obj.GetHandle (), wait ? (byte) 1 : (byte) 0);
#endif
#endif
		}

		public void BeginInvokeOnMainThread (Selector sel, NSObject obj)
		{
			InvokeOnMainThread (sel, obj, false);
		}

		public void InvokeOnMainThread (Selector sel, NSObject obj)
		{
			InvokeOnMainThread (sel, obj, true);
		}

		public void BeginInvokeOnMainThread (Action action)
		{
			var d = new NSAsyncActionDispatcher (action);
#if NET
			Messaging.void_objc_msgSend_NativeHandle_NativeHandle_bool (d.Handle, Selector.GetHandle (Selector.PerformSelectorOnMainThreadWithObjectWaitUntilDone),
		                                                        NSDispatcher.Selector.Handle, d.Handle, 0);
#else
#if MONOMAC
			Messaging.void_objc_msgSend_IntPtr_IntPtr_bool (d.Handle, Selector.PerformSelectorOnMainThreadWithObjectWaitUntilDoneHandle, 
		                                                        NSDispatcher.Selector.Handle, d.Handle, 0);
#else
			Messaging.void_objc_msgSend_IntPtr_IntPtr_bool (d.Handle, Selector.GetHandle (Selector.PerformSelectorOnMainThreadWithObjectWaitUntilDone),
															Selector.GetHandle (NSDispatcher.SelectorName), d.Handle, 0);
#endif
#endif
		}

		internal void BeginInvokeOnMainThread (System.Threading.SendOrPostCallback cb, object state)
		{
			var d = new NSAsyncSynchronizationContextDispatcher (cb, state);
#if NET
			Messaging.void_objc_msgSend_NativeHandle_NativeHandle_bool (d.Handle, Selector.GetHandle (Selector.PerformSelectorOnMainThreadWithObjectWaitUntilDone),
			                                                Selector.GetHandle (NSDispatcher.SelectorName), d.Handle, 0);
#else
#if MONOMAC
			Messaging.void_objc_msgSend_IntPtr_IntPtr_bool (d.Handle, Selector.PerformSelectorOnMainThreadWithObjectWaitUntilDoneHandle,
		                                                        NSDispatcher.Selector.Handle, d.Handle, 0);
#else
			Messaging.void_objc_msgSend_IntPtr_IntPtr_bool (d.Handle, Selector.GetHandle (Selector.PerformSelectorOnMainThreadWithObjectWaitUntilDone),
															Selector.GetHandle (NSDispatcher.SelectorName), d.Handle, 0);
#endif
#endif
		}

		public void InvokeOnMainThread (Action action)
		{
			using (var d = new NSActionDispatcher (action)) {
#if NET
				Messaging.void_objc_msgSend_NativeHandle_NativeHandle_bool (d.Handle, Selector.GetHandle (Selector.PerformSelectorOnMainThreadWithObjectWaitUntilDone), 
				                                                Selector.GetHandle (NSDispatcher.SelectorName), d.Handle, 1);
#else
#if MONOMAC
				Messaging.void_objc_msgSend_IntPtr_IntPtr_bool (d.Handle, Selector.PerformSelectorOnMainThreadWithObjectWaitUntilDoneHandle, 
		                                                                NSDispatcher.Selector.Handle, d.Handle, 1);
#else
				Messaging.void_objc_msgSend_IntPtr_IntPtr_bool (d.Handle, Selector.GetHandle (Selector.PerformSelectorOnMainThreadWithObjectWaitUntilDone),
																Selector.GetHandle (NSDispatcher.SelectorName), d.Handle, 1);
#endif
#endif
			}
		}

		internal void InvokeOnMainThread (System.Threading.SendOrPostCallback cb, object state)
		{
			using (var d = new NSSynchronizationContextDispatcher (cb, state)) {
#if NET
				Messaging.void_objc_msgSend_NativeHandle_NativeHandle_bool (d.Handle, Selector.GetHandle (Selector.PerformSelectorOnMainThreadWithObjectWaitUntilDone),
				                                                Selector.GetHandle (NSDispatcher.SelectorName), d.Handle, 1);
#else
#if MONOMAC
				Messaging.void_objc_msgSend_IntPtr_IntPtr_bool (d.Handle, Selector.PerformSelectorOnMainThreadWithObjectWaitUntilDoneHandle,
			                                                        NSDispatcher.Selector.Handle, d.Handle, 1);
#else
				Messaging.void_objc_msgSend_IntPtr_IntPtr_bool (d.Handle, Selector.GetHandle (Selector.PerformSelectorOnMainThreadWithObjectWaitUntilDone),
																Selector.GetHandle (NSDispatcher.SelectorName), d.Handle, 1);
#endif
#endif
			}
		}

		public static NSObject FromObject (object obj)
		{
			if (obj is null)
				return NSNull.Null;
			var t = obj.GetType ();
			if (t == typeof (NSObject) || t.IsSubclassOf (typeof (NSObject)))
				return (NSObject) obj;

			switch (Type.GetTypeCode (t)) {
			case TypeCode.Boolean:
				return new NSNumber ((bool) obj);
			case TypeCode.Char:
				return new NSNumber ((ushort) (char) obj);
			case TypeCode.SByte:
				return new NSNumber ((sbyte) obj);
			case TypeCode.Byte:
				return new NSNumber ((byte) obj);
			case TypeCode.Int16:
				return new NSNumber ((short) obj);
			case TypeCode.UInt16:
				return new NSNumber ((ushort) obj);
			case TypeCode.Int32:
				return new NSNumber ((int) obj);
			case TypeCode.UInt32:
				return new NSNumber ((uint) obj);
			case TypeCode.Int64:
				return new NSNumber ((long) obj);
			case TypeCode.UInt64:
				return new NSNumber ((ulong) obj);
			case TypeCode.Single:
				return new NSNumber ((float) obj);
			case TypeCode.Double:
				return new NSNumber ((double) obj);
			case TypeCode.String:
				return new NSString ((string) obj);
			default:
				if (t == typeof (NativeHandle))
					return NSValue.ValueFromPointer ((NativeHandle) obj);
#if !NO_SYSTEM_DRAWING
				if (t == typeof (SizeF))
					return NSValue.FromSizeF ((SizeF) obj);
				else if (t == typeof (RectangleF))
					return NSValue.FromRectangleF ((RectangleF) obj);
				else if (t == typeof (PointF))
					return NSValue.FromPointF ((PointF) obj);
#endif
				if (t == typeof (nint))
					return NSNumber.FromNInt ((nint) obj);
				else if (t == typeof (nuint))
					return NSNumber.FromNUInt ((nuint) obj);
				else if (t == typeof (nfloat))
					return NSNumber.FromNFloat ((nfloat) obj);
				else if (t == typeof (CGSize))
					return NSValue.FromCGSize ((CGSize) obj);
				else if (t == typeof (CGRect))
					return NSValue.FromCGRect ((CGRect) obj);
				else if (t == typeof (CGPoint))
					return NSValue.FromCGPoint ((CGPoint) obj);

#if !MONOMAC
				if (t == typeof (CGAffineTransform))
					return NSValue.FromCGAffineTransform ((CGAffineTransform) obj);
				else if (t == typeof (UIEdgeInsets))
					return NSValue.FromUIEdgeInsets ((UIEdgeInsets) obj);
#if !WATCH
				else if (t == typeof (CATransform3D))
					return NSValue.FromCATransform3D ((CATransform3D) obj);
#endif
#endif
				// last chance for types like CGPath, CGColor... that are not NSObject but are CFObject
				// see https://bugzilla.xamarin.com/show_bug.cgi?id=8458
				INativeObject native = (obj as INativeObject);
				if (native is not null)
					return Runtime.GetNSObject (native.Handle);
				return null;
			}
		}

		public void SetValueForKeyPath (NativeHandle handle, NSString keyPath)
		{
			if (keyPath is null)
				throw new ArgumentNullException ("keyPath");
#if NET
			if (IsDirectBinding) {
				ObjCRuntime.Messaging.void_objc_msgSend_NativeHandle_NativeHandle (this.Handle, Selector.GetHandle ("setValue:forKeyPath:"), handle, keyPath.Handle);
			} else {
				ObjCRuntime.Messaging.void_objc_msgSendSuper_NativeHandle_NativeHandle (this.SuperHandle, Selector.GetHandle ("setValue:forKeyPath:"), handle, keyPath.Handle);
			}
#else
#if MONOMAC
			if (IsDirectBinding) {
				ObjCRuntime.Messaging.void_objc_msgSend_IntPtr_IntPtr (this.Handle, selSetValue_ForKeyPath_XHandle, handle, keyPath.Handle);
			} else {
				ObjCRuntime.Messaging.void_objc_msgSendSuper_IntPtr_IntPtr (this.SuperHandle, selSetValue_ForKeyPath_XHandle, handle, keyPath.Handle);
			}
#else
			if (IsDirectBinding) {
				ObjCRuntime.Messaging.void_objc_msgSend_IntPtr_IntPtr (this.Handle, Selector.GetHandle ("setValue:forKeyPath:"), handle, keyPath.Handle);
			} else {
				ObjCRuntime.Messaging.void_objc_msgSendSuper_IntPtr_IntPtr (this.SuperHandle, Selector.GetHandle ("setValue:forKeyPath:"), handle, keyPath.Handle);
			}
#endif
#endif
		}

		// if IsDirectBinding is false then we _likely_ have managed state and it's up to the subclass to provide
		// a correct implementation of GetHashCode / Equals. We default to Object.GetHashCode (like classic)

		public override int GetHashCode ()
		{
			if (!IsDirectBinding)
				return base.GetHashCode ();
			// Hash is nuint so 64 bits, and Int32.GetHashCode == same Int32
			return GetNativeHash ().GetHashCode ();
		}

		public override bool Equals (object obj)
		{
			var o = obj as NSObject;
			if (o is null)
				return false;

			bool isDirectBinding = IsDirectBinding;
			// is only one is a direct binding then both cannot be equals
			if (isDirectBinding != o.IsDirectBinding)
				return false;

			// we can only ask `isEqual:` to test equality if both objects are direct bindings
			return isDirectBinding ? IsEqual (o) : ReferenceEquals (this, o);
		}

		// IEquatable<T>
		public bool Equals (NSObject obj) => Equals ((object) obj);

		public override string ToString ()
		{
			if (disposed)
				return base.ToString ();
			return Description ?? base.ToString ();
		}

		public virtual void Invoke (Action action, double delay)
		{
			var d = new NSAsyncActionDispatcher (action);
			d.PerformSelector (NSDispatcher.Selector, null, delay);
		}

		public virtual void Invoke (Action action, TimeSpan delay)
		{
			var d = new NSAsyncActionDispatcher (action);
			d.PerformSelector (NSDispatcher.Selector, null, delay.TotalSeconds);
		}

		internal void ClearHandle ()
		{
			handle = NativeHandle.Zero;
		}

		protected virtual void Dispose (bool disposing)
		{
			if (disposed)
				return;
			disposed = true;

			if (handle != NativeHandle.Zero) {
				if (disposing) {
					ReleaseManagedRef ();
				} else {
					NSObject_Disposer.Add (this);
				}
			} else {
				FreeData ();
			}
		}

		unsafe void FreeData ()
		{
			if (super != NativeHandle.Zero) {
				Marshal.FreeHGlobal (super);
				super = NativeHandle.Zero;
			}
		}

		[Register ("__NSObject_Disposer")]
		[Preserve (AllMembers = true)]
		internal class NSObject_Disposer : NSObject {
			static readonly List<NSObject> drainList1 = new List<NSObject> ();
			static readonly List<NSObject> drainList2 = new List<NSObject> ();
			static List<NSObject> handles = drainList1;

			static readonly IntPtr class_ptr = Class.GetHandle ("__NSObject_Disposer");
#if MONOMAC
			static readonly IntPtr drainHandle = Selector.GetHandle ("drain:");
#endif

			static readonly object lock_obj = new object ();

			private NSObject_Disposer ()
			{
				// Disable default ctor, there should be no instances of this class.
			}

			static internal void Add (NSObject handle)
			{
				bool call_drain;
				lock (lock_obj) {
					handles.Add (handle);
					call_drain = handles.Count == 1;
				}
				if (!call_drain)
					return;
				ScheduleDrain ();
			}

			static void ScheduleDrain ()
			{
#if NET
				Messaging.void_objc_msgSend_NativeHandle_NativeHandle_bool (class_ptr, Selector.GetHandle (Selector.PerformSelectorOnMainThreadWithObjectWaitUntilDone), Selector.GetHandle ("drain:"), NativeHandle.Zero, 0);
#else
#if MONOMAC
				Messaging.void_objc_msgSend_IntPtr_IntPtr_bool (class_ptr, Selector.PerformSelectorOnMainThreadWithObjectWaitUntilDoneHandle, drainHandle, IntPtr.Zero, 0);
#else
				Messaging.void_objc_msgSend_IntPtr_IntPtr_bool (class_ptr, Selector.GetHandle (Selector.PerformSelectorOnMainThreadWithObjectWaitUntilDone), Selector.GetHandle ("drain:"), IntPtr.Zero, 0);
#endif
#endif
			}

			static bool draining;

			[Export ("drain:")]
			static void Drain (NSObject ctx)
			{
				List<NSObject> drainList;

				lock (lock_obj) {
					// This function isn't re-entrant safe, so protect against it. The only possibility I can
					// see where this function would be re-entrant, is if in the call to ReleaseManagedRef below,
					// the native dealloc method for a type ended up executing the run loop, and that runloop
					// processed a drain request, ending up in this method (again).
					if (draining) {
						ScheduleDrain ();
						return;
					}
					draining = true;

					drainList = handles;
					if (handles == drainList1)
						handles = drainList2;
					else
						handles = drainList1;
				}

				foreach (NSObject x in drainList)
					x.ReleaseManagedRef ();
				drainList.Clear ();

				lock (lock_obj) {
					draining = false;
				}
			}
		}

		[Register ("__XamarinObjectObserver")]
		class Observer : NSObject {
			WeakReference obj;
			Action<NSObservedChange> cback;
			NSString key;

			public Observer (NSObject obj, NSString key, Action<NSObservedChange> observer)
			{
				if (observer is null)
					throw new ArgumentNullException (nameof (observer));

				this.obj = new WeakReference (obj);
				this.key = key;
				this.cback = observer;
				IsDirectBinding = false;
			}

			[Preserve (Conditional = true)]
			public override void ObserveValue (NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
			{
				if (keyPath == key && context == Handle)
					cback (new NSObservedChange (change));
				else
					base.ObserveValue (keyPath, ofObject, change, context);
			}

			protected override void Dispose (bool disposing)
			{
				if (disposing) {
					NSObject target;
					if (obj is not null) {
						target = (NSObject) obj.Target;
						if (target is not null)
							target.RemoveObserver (this, key, Handle);
					}
					obj = null;
					cback = null;
				} else {
					Runtime.NSLog ("Warning: observer object was not disposed manually with Dispose()");
				}
				base.Dispose (disposing);
			}
		}

		public IDisposable AddObserver (string key, NSKeyValueObservingOptions options, Action<NSObservedChange> observer)
		{
			return AddObserver (new NSString (key), options, observer);
		}

		public IDisposable AddObserver (NSString key, NSKeyValueObservingOptions options, Action<NSObservedChange> observer)
		{
			var o = new Observer (this, key, observer);
			AddObserver (o, key, options, o.Handle);
			return o;
		}

		[EditorBrowsable (EditorBrowsableState.Never)]
		public static NSObject Alloc (Class kls)
		{
			var h = Messaging.IntPtr_objc_msgSend (kls.Handle, Selector.GetHandle (Selector.Alloc));
			return new NSObject (h, true);
		}

		[EditorBrowsable (EditorBrowsableState.Never)]
		public void Init ()
		{
			if (handle == IntPtr.Zero)
				throw new Exception ("you have not allocated the native object");

			handle = Messaging.IntPtr_objc_msgSend (handle, Selector.GetHandle ("init"));
		}

		public static void InvokeInBackground (Action action)
		{
			// using the parameterized Thread.Start to avoid capturing
			// the 'action' parameter (it'll needlessly create an extra
			// object).
			new System.Threading.Thread ((v) => {
				((Action) v) ();
			}) {
				IsBackground = true,
			}.Start (action);
		}
#endif // !COREBUILD
	}

#if !COREBUILD
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class NSObservedChange {
		NSDictionary dict;
		public NSObservedChange (NSDictionary source)
		{
			dict = source;
		}

		public NSKeyValueChange Change {
			get {
				var n = (NSNumber) dict [NSObject.ChangeKindKey];
				return (NSKeyValueChange) n.Int32Value;
			}
		}

		public NSObject NewValue {
			get {
				return dict [NSObject.ChangeNewKey];
			}
		}

		public NSObject OldValue {
			get {
				return dict [NSObject.ChangeOldKey];
			}
		}

		public NSIndexSet Indexes {
			get {
				return (NSIndexSet) dict [NSObject.ChangeIndexesKey];
			}
		}

		public bool IsPrior {
			get {
				var n = dict [NSObject.ChangeNotificationIsPriorKey] as NSNumber;
				if (n is null)
					return false;
				return n.BoolValue;
			}
		}
	}
#endif
}
