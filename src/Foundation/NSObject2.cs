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
using System.Diagnostics.CodeAnalysis;
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

using System.Runtime.InteropServices.ObjectiveC;

using ObjCRuntime;
#if !COREBUILD
using Xamarin.Bundler;
#if MONOTOUCH
using UIKit;
using CoreAnimation;
#endif
using CoreGraphics;
#endif

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Foundation {

	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public enum NSObjectFlag {
		Empty,
	}

	// This interface will be made public when the managed static registrar is used.
	internal interface INSObjectFactory {
		// The method will be implemented via custom linker step if the managed static registrar is used
		// for NSObject subclasses which have an (NativeHandle) or (IntPtr) constructor.
		[MethodImpl (MethodImplOptions.NoInlining)]
		virtual static NSObject _Xamarin_ConstructNSObject (NativeHandle handle) => null;
	}

#if !COREBUILD
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
		, INSObjectFactory {
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
		protected internal NSObject (NativeHandle handle)
			: this (handle, false)
		{
		}

		[EditorBrowsable (EditorBrowsableState.Never)]
		protected NSObject (NativeHandle handle, bool alloced)
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

		// This method should never be called when using the managed static registrar, so assert that never happens by throwing an exception in that case.
		// This method doesn't necessarily work with NativeAOT, but this is covered by the exception, because the managed static registrar is required for NativeAOT.
		//
		// IL2072: 'type' argument does not satisfy 'DynamicallyAccessedMemberTypes.PublicConstructors', 'DynamicallyAccessedMemberTypes.NonPublicConstructors' in call to 'System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(Type)'. The return value of method 'ObjCRuntime.Runtime.GetGCHandleTarget(IntPtr)' does not have matching annotations. The source value must declare at least the same requirements as those declared on the target location it is assigned to.
		[UnconditionalSuppressMessage ("", "IL2072", Justification = "The APIs this method tries to access are marked by other means, so this is linker-safe.")]
		internal static IntPtr CreateNSObject (IntPtr type_gchandle, IntPtr handle, Flags flags)
		{
			// Note that the code in this method doesn't necessarily work with NativeAOT, so assert that never happens by throwing an exception if using the managed static registrar (which is required for NativeAOT)
			if (Runtime.IsManagedStaticRegistrar) {
				throw new System.Diagnostics.UnreachableException ();
			}

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

		internal Flags FlagsInternal {
			get { return flags; }
			set { flags = value; }
		}

#if !__MACOS__
		[MethodImplAttribute (MethodImplOptions.InternalCall)]
		extern static void RegisterToggleRef (NSObject obj, IntPtr handle, bool isCustomType);
#endif // !__MACOS__

		[DllImport ("__Internal")]
		static extern void xamarin_release_managed_ref (IntPtr handle, byte user_type);

		static void RegisterToggleReference (NSObject obj, IntPtr handle, bool isCustomType)
		{
#if __MACOS__
			Runtime.RegisterToggleReferenceCoreCLR (obj, handle, isCustomType);
#else
			if (Runtime.IsCoreCLR) {
				Runtime.RegisterToggleReferenceCoreCLR (obj, handle, isCustomType);
			} else {
				RegisterToggleRef (obj, handle, isCustomType);
			}
#endif
		}

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
		static extern byte xamarin_set_gchandle_with_flags_safe (IntPtr handle, IntPtr gchandle, XamarinGCHandleFlags flags);

		void CreateManagedRef (bool retain)
		{
			HasManagedRef = true;
			if (!Runtime.TryGetIsUserType (handle, out var isUserType, out var error_message))
				throw new InvalidOperationException ($"Unable to create a managed reference for the pointer {handle} whose managed type is {GetType ().FullName} because it wasn't possible to get the class of the pointer: {error_message}");

			if (isUserType) {
				var flags = XamarinGCHandleFlags.HasManagedRef | XamarinGCHandleFlags.InitialSet | XamarinGCHandleFlags.WeakGCHandle;
				var gchandle = GCHandle.Alloc (this, GCHandleType.WeakTrackResurrection);
				var h = GCHandle.ToIntPtr (gchandle);
				if (xamarin_set_gchandle_with_flags_safe (handle, h, flags) == 0) {
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
			if (!Runtime.TryGetIsUserType (handle, out var user_type, out var error_message))
				throw new InvalidOperationException ($"Unable to release the managed reference for the pointer {handle} whose managed type is {GetType ().FullName} because it wasn't possible to get the class of the pointer: {error_message}");
			HasManagedRef = false;
			if (!user_type) {
				/* If we're a wrapper type, we need to unregister here, since we won't enter the release trampoline */
				Runtime.NativeObjectHasDied (handle, this);
			}
			xamarin_release_managed_ref (handle, user_type.AsByte ());
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
				ref var map = ref CollectionsMarshal.GetValueRefOrAddDefault (Runtime.protocol_cache, classHandle, out var exists);
				if (!exists)
					map = new ();
				ref var result = ref CollectionsMarshal.GetValueRefOrAddDefault (map, protocol, out exists);
				if (!exists)
					result = DynamicConformsToProtocol (protocol);
				return result;
			}
		}

		// Note that this method does not work with NativeAOT, so throw an exception in that case.
		// IL2075: 'this' argument does not satisfy 'DynamicallyAccessedMemberTypes.Interfaces' in call to 'System.Type.GetInterfaces()'. The return value of method 'System.Object.GetType()' does not have matching annotations. The source value must declare at least the same requirements as those declared on the target location it is assigned to.
		[UnconditionalSuppressMessage ("", "IL2075", Justification = "The APIs this method tries to access are marked by other means, so this is linker-safe.")]
		bool DynamicConformsToProtocol (NativeHandle protocol)
		{
			// Note that this method does not work with NativeAOT, so throw an exception in that case.
			if (Runtime.IsNativeAOT)
				throw Runtime.CreateNativeAOTNotSupportedException ();

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

		/// <summary>Calls the 'release' selector on this object.</summary>
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		public void DangerousRelease ()
		{
			DangerousRelease (handle);
		}

		/// <summary>Calls the 'release' selector on an Objective-C object.</summary>
		/// <param name="handle">The Objective-C object to release.</param>
		/// <remarks>It's safe to call this function with <see cref="NativeHandle.Zero" />.</remarks>
		[EditorBrowsable (EditorBrowsableState.Never)]
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

		/// <summary>Calls the 'retain' selector on an Objective-C object.</summary>
		/// <param name="handle">The Objective-C object to retain.</param>
		/// <remarks>It's safe to call this function with <see cref="NativeHandle.Zero" />.</remarks>
		[EditorBrowsable (EditorBrowsableState.Never)]
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

		/// <summary>Calls the 'autorelease' selector on an Objective-C object.</summary>
		/// <param name="handle">The Objective-C object to autorelease.</param>
		/// <remarks>It's safe to call this function with <see cref="NativeHandle.Zero" />.</remarks>
		internal static void DangerousAutorelease (NativeHandle handle)
		{
#if MONOMAC
			Messaging.void_objc_msgSend (handle, Selector.AutoreleaseHandle);
#else
			Messaging.void_objc_msgSend (handle, Selector.GetHandle (Selector.Autorelease));
#endif
		}

		/// <summary>Calls the 'retain' selector on this object.</summary>
		/// <returns>This object.</returns>
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		public NSObject DangerousRetain ()
		{
			DangerousRetain (handle);
			return this;
		}

		/// <summary>Calls the 'autorelease' selector on this object.</summary>
		/// <returns>This object.</returns>
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		public NSObject DangerousAutorelease ()
		{
			DangerousAutorelease (handle);
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

				unsafe {
					if (tracked_object_info is not null)
						tracked_object_info->Handle = value;
				}

				if (handle != IntPtr.Zero)
					Runtime.RegisterNSObject (this, handle);
			}
		}

		[EditorBrowsable (EditorBrowsableState.Never)]
		protected void InitializeHandle (NativeHandle handle)
		{
			InitializeHandle (handle, "init*", Class.ThrowOnInitFailure);
		}

		[EditorBrowsable (EditorBrowsableState.Never)]
		protected void InitializeHandle (NativeHandle handle, string initSelector)
		{
			InitializeHandle (handle, initSelector, Class.ThrowOnInitFailure);
		}

		[EditorBrowsable (EditorBrowsableState.Never)]
		internal void InitializeHandle (NativeHandle handle, string initSelector, bool throwOnInitFailure)
		{
			if (this.handle == NativeHandle.Zero && throwOnInitFailure) {
				if (ClassHandle == NativeHandle.Zero)
					throw new Exception ($"Could not create an native instance of the type '{GetType ().FullName}': the native class hasn't been loaded.\n{Constants.SetThrowOnInitFailureToFalse}.");
				throw new Exception ($"Could not create an native instance of the type '{new Class (ClassHandle).Name}'.\n{Constants.SetThrowOnInitFailureToFalse}.");
			}

			if (handle == NativeHandle.Zero && throwOnInitFailure) {
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

		private void InvokeOnMainThread (Selector sel, NSObject obj, bool wait)
		{
			Messaging.void_objc_msgSend_NativeHandle_NativeHandle_bool (this.Handle, Selector.GetHandle (Selector.PerformSelectorOnMainThreadWithObjectWaitUntilDone), sel.Handle, obj.GetHandle (), wait ? (byte) 1 : (byte) 0);
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
			Messaging.void_objc_msgSend_NativeHandle_NativeHandle_bool (d.Handle, Selector.GetHandle (Selector.PerformSelectorOnMainThreadWithObjectWaitUntilDone),
																NSDispatcher.Selector.Handle, d.Handle, 0);
		}

		internal void BeginInvokeOnMainThread (System.Threading.SendOrPostCallback cb, object state)
		{
			var d = new NSAsyncSynchronizationContextDispatcher (cb, state);
			Messaging.void_objc_msgSend_NativeHandle_NativeHandle_bool (d.Handle, Selector.GetHandle (Selector.PerformSelectorOnMainThreadWithObjectWaitUntilDone),
															Selector.GetHandle (NSDispatcher.SelectorName), d.Handle, 0);
		}

		public void InvokeOnMainThread (Action action)
		{
			using (var d = new NSActionDispatcher (action)) {
				Messaging.void_objc_msgSend_NativeHandle_NativeHandle_bool (d.Handle, Selector.GetHandle (Selector.PerformSelectorOnMainThreadWithObjectWaitUntilDone),
																Selector.GetHandle (NSDispatcher.SelectorName), d.Handle, 1);
			}
		}

		internal void InvokeOnMainThread (System.Threading.SendOrPostCallback cb, object state)
		{
			using (var d = new NSSynchronizationContextDispatcher (cb, state)) {
				Messaging.void_objc_msgSend_NativeHandle_NativeHandle_bool (d.Handle, Selector.GetHandle (Selector.PerformSelectorOnMainThreadWithObjectWaitUntilDone),
																Selector.GetHandle (NSDispatcher.SelectorName), d.Handle, 1);
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
				else if (t == typeof (CATransform3D))
					return NSValue.FromCATransform3D ((CATransform3D) obj);
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
			if (IsDirectBinding) {
				ObjCRuntime.Messaging.void_objc_msgSend_NativeHandle_NativeHandle (this.Handle, Selector.GetHandle ("setValue:forKeyPath:"), handle, keyPath.Handle);
			} else {
				ObjCRuntime.Messaging.void_objc_msgSendSuper_NativeHandle_NativeHandle (this.SuperHandle, Selector.GetHandle ("setValue:forKeyPath:"), handle, keyPath.Handle);
			}
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

		// This option is changed by setting the DisposeTaggedPointers MSBuild property in the project file.
		static bool? dispose_tagged_pointers;
		static bool DisposeTaggedPointers {
			get {
				if (!dispose_tagged_pointers.HasValue) {
					if (AppContext.TryGetSwitch ("Foundation.NSObject.DisposeTaggedPointers", out var dtp)) {
						dispose_tagged_pointers = dtp;
					} else {
						// The default logic here must match how we set the default value for the DisposeTaggedPointers MSBuild property.
#if NET10_0_OR_GREATER
						dispose_tagged_pointers = false;
#else
						dispose_tagged_pointers = true;
#endif
					}
				}
				return dispose_tagged_pointers.Value;
			}
		}

		protected virtual void Dispose (bool disposing)
		{
			if (disposed)
				return;
			disposed = true;

			var isTaggedPointerThatShouldNotBeDisposed = false;
			if (!DisposeTaggedPointers) {
				/* Tagged pointer is limited to 64bit, which is all we support anyway.
				 *
				 * The bit that identifies if a pointer is a tagged pointer is:
				 *
				 * Arm64 (everywhere): most significant bit
				 * Simulators (both on arm64 and x64 desktops): most significant bit
				 * Desktop/x64 (macOS + Mac Catalyst): least significant bit
				 *
				 * Ref: https://github.com/apple-oss-distributions/objc4/blob/89543e2c0f67d38ca5211cea33f42c51500287d5/runtime/objc-internal.h#L603-L672
				 */
#if __MACOS__ || __MACCATALYST__
				ulong _OBJC_TAG_MASK;
				if (Runtime.IsARM64CallingConvention) {
					_OBJC_TAG_MASK = 1UL << 63;
				} else {
					_OBJC_TAG_MASK = 1UL;
				}
#else
				const ulong _OBJC_TAG_MASK = 1UL << 63;
#endif

				unchecked {
					var ulongHandle = (ulong) (IntPtr) handle;
					var isTaggedPointer = (ulongHandle & _OBJC_TAG_MASK) == _OBJC_TAG_MASK;
					isTaggedPointerThatShouldNotBeDisposed = isTaggedPointer;
				}
			}

			if (isTaggedPointerThatShouldNotBeDisposed) {
				FreeData (); // still need to do this though.
			} else if (handle != NativeHandle.Zero) {
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
				Messaging.void_objc_msgSend_NativeHandle_NativeHandle_bool (class_ptr, Selector.GetHandle (Selector.PerformSelectorOnMainThreadWithObjectWaitUntilDone), Selector.GetHandle ("drain:"), NativeHandle.Zero, 0);
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
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
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
