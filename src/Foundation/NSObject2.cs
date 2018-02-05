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
#if !NO_SYSTEM_DRAWING
using System.Drawing;
#endif

using ObjCRuntime;
#if !COREBUILD
#if MONOTOUCH
using UIKit;
#if !WATCH
using CoreAnimation;
#endif
#endif
using CoreGraphics;
#endif

namespace Foundation {
	public class NSObjectFlag {
		public static readonly NSObjectFlag Empty;
		
		NSObjectFlag () {}
	}

	[StructLayout (LayoutKind.Sequential)]
	public partial class NSObject 
#if !COREBUILD && XAMCORE_2_0
		: IEquatable<NSObject> 
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

		// The order of 'handle' and 'class_handle' is important: do not re-order unless SuperHandle is modified accordingly.
		IntPtr handle;
		IntPtr class_handle;
		Flags flags;

		// This enum has a native counterpart in runtime.h
		[Flags]
		enum Flags : byte {
			Disposed = 1,
			NativeRef = 2,
			IsDirectBinding = 4,
			RegisteredToggleRef = 8,
			InFinalizerQueue = 16,
			HasManagedRef = 32,
		}

		bool disposed { 
			get { return ((flags & Flags.Disposed) == Flags.Disposed); } 
			set { flags = value ? (flags | Flags.Disposed) : (flags & ~Flags.Disposed);	}
		}

		internal bool IsRegisteredToggleRef { 
			get { return ((flags & Flags.RegisteredToggleRef) == Flags.RegisteredToggleRef); } 
			set { flags = value ? (flags | Flags.RegisteredToggleRef) : (flags & ~Flags.RegisteredToggleRef);	}
		}
#if XAMCORE_2_0
		protected internal bool IsDirectBinding {
			get { return ((flags & Flags.IsDirectBinding) == Flags.IsDirectBinding); }
			set { flags = value ? (flags | Flags.IsDirectBinding) : (flags & ~Flags.IsDirectBinding); }
		}
#else
		protected internal bool IsDirectBinding;
#endif

		internal bool InFinalizerQueue {
			get { return ((flags & Flags.InFinalizerQueue) == Flags.InFinalizerQueue); }
		}

		[Export ("init")]
		public NSObject () {
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
		
		public NSObject (IntPtr handle) : this (handle, false) {
		}
		
		public NSObject (IntPtr handle, bool alloced) {
			this.handle = handle;
			InitializeObject (alloced);
		}
		
		~NSObject () {
			Dispose (false);
		}
		
		public void Dispose () {
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		internal static IntPtr Initialize ()
		{
			return class_ptr;
		}

		[MethodImplAttribute (MethodImplOptions.InternalCall)]
		extern static void RegisterToggleRef (NSObject obj, IntPtr handle, bool isCustomType);

		[MethodImplAttribute (MethodImplOptions.InternalCall)]
		static extern void xamarin_release_managed_ref (IntPtr handle, NSObject managed_obj);

		[MethodImplAttribute (MethodImplOptions.InternalCall)]
		static extern void xamarin_create_managed_ref (IntPtr handle, NSObject obj, bool retain);

#if !XAMCORE_3_0
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
		protected void MarkDirty () {
			MarkDirty (false);
		}
			
		internal void MarkDirty (bool allowCustomTypes)
		{
			if (IsRegisteredToggleRef)
				return;

			if (!allowCustomTypes && Class.IsCustomType (GetType ()))
				return;
			
			IsRegisteredToggleRef = true;
			RegisterToggleRef (this, Handle, allowCustomTypes);
		}

		private void InitializeObject (bool alloced) {
			if (alloced && handle == IntPtr.Zero && Class.ThrowOnInitFailure) {
				if (ClassHandle == IntPtr.Zero)
					throw new Exception (string.Format ("Could not create an native instance of the type '{0}': the native class hasn't been loaded.\n" +
						"It is possible to ignore this condition by setting " + (Runtime.IsUnifiedBuild ? "" : (Runtime.CompatNamespace + ".")) + "ObjCRuntime.Class.ThrowOnInitFailure to false.",
						GetType ().FullName));
				throw new Exception (string.Format ("Failed to create a instance of the native type '{0}'.\n" +
					"It is possible to ignore this condition by setting " + (Runtime.IsUnifiedBuild ? "" : (Runtime.CompatNamespace + ".")) + "ObjCRuntime.Class.ThrowOnInitFailure to false.",
					new Class (ClassHandle).Name));
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

			// If the NativeRef bit is set, it means that this call was surfaced by
			// monotouch_ctor_trampoline, which means we do not want to invoke Retain directly,
			// it will be done by monotouch_ctor_trampoline on return.
			bool native_ref = (flags & Flags.NativeRef) == Flags.NativeRef;
			if (!native_ref)
				CreateManagedRef (!alloced);
		}

		void CreateManagedRef (bool retain)
		{
			xamarin_create_managed_ref (handle, this, retain);
		}

		void ReleaseManagedRef ()
		{
			xamarin_release_managed_ref (handle, this);
		}

#if !XAMCORE_2_0
		[Export ("encodeWithCoder:")]
		public virtual void EncodeTo (NSCoder coder)
		{
			if (coder == null)
				throw new ArgumentNullException ("coder");

			if (!(this is INSCoding))
				throw new InvalidOperationException ("Type does not conform to NSCoding");

#if MONOMAC
			if (IsDirectBinding) {
				Messaging.void_objc_msgSend_intptr (this.Handle, selEncodeWithCoderHandle, coder.Handle);
			} else {
				Messaging.void_objc_msgSendSuper_intptr (this.SuperHandle, selEncodeWithCoderHandle, coder.Handle);
			}
#else
			if (IsDirectBinding) {
				Messaging.void_objc_msgSend_intptr (this.Handle, Selector.GetHandle (selEncodeWithCoder), coder.Handle);
			} else {
				Messaging.void_objc_msgSendSuper_intptr (this.SuperHandle, Selector.GetHandle (selEncodeWithCoder), coder.Handle);
			}
#endif
		}
#endif
		static bool IsProtocol (Type type, IntPtr protocol)
		{
			while (type != typeof (NSObject) && type != null) {
				var attrs = type.GetCustomAttributes (typeof(ProtocolAttribute), false);
				var protocolAttribute = (ProtocolAttribute) (attrs.Length > 0 ? attrs [0] : null);
				if (protocolAttribute != null) {
					string name;

					if (!string.IsNullOrEmpty (protocolAttribute.Name)) {
						name = protocolAttribute.Name;
					} else {
						attrs = type.GetCustomAttributes (typeof(RegisterAttribute), false);
						var registerAttribute = (RegisterAttribute) (attrs.Length > 0 ? attrs [0] : null);
						if (registerAttribute != null && !string.IsNullOrEmpty (registerAttribute.Name)) {
							name = registerAttribute.Name;
						} else {
							name = type.Name;
						}
					}

					return (Runtime.GetProtocol (name) == protocol);
				}
				type = type.BaseType;
			}

			return false;
		}

		[Preserve]
		bool InvokeConformsToProtocol (IntPtr protocol)
		{
			return ConformsToProtocol (protocol);
		}

		[Export ("conformsToProtocol:")]
		[Preserve ()]
		public virtual bool ConformsToProtocol (IntPtr protocol)
		{
			bool does;
			bool is_wrapper = IsDirectBinding;
			bool is_third_party;

			if (is_wrapper) {
				is_third_party = this.GetType ().Assembly != NSObject.PlatformAssembly;
				if (is_third_party) {
					// Third-party bindings might lie about IsDirectBinding (see bug #14772),
					// so don't trust any 'true' values unless we're in monotouch.dll.
					var attribs = this.GetType ().GetCustomAttributes (typeof(RegisterAttribute), false);
					if (attribs != null && attribs.Length == 1)
						is_wrapper = ((RegisterAttribute) attribs [0]).IsWrapper;
				}
			}

#if MONOMAC
			if (is_wrapper) {
				does = Messaging.bool_objc_msgSend_IntPtr (this.Handle, selConformsToProtocolHandle, protocol);
			} else {
				does = Messaging.bool_objc_msgSendSuper_IntPtr (this.SuperHandle, selConformsToProtocolHandle, protocol);
			}
#else
			if (is_wrapper) {
				does = Messaging.bool_objc_msgSend_IntPtr (this.Handle, Selector.GetHandle (selConformsToProtocol), protocol);
			} else {
				does = Messaging.bool_objc_msgSendSuper_IntPtr (this.SuperHandle, Selector.GetHandle (selConformsToProtocol), protocol);
			}
#endif

			if (does)
				return true;
			
			object [] adoptedProtocols = GetType ().GetCustomAttributes (typeof (AdoptsAttribute), true);
			foreach (AdoptsAttribute adopts in adoptedProtocols){
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
			
#if !XAMCORE_2_0
		[Obsolete ("Low-level API warning: Use at your own risk: this calls the Release method on the underlying object;  Use DangerousRelease to avoid this warning.")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		public void Release ()
		{
			DangerousRelease ();
		}
#endif

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		public void DangerousRelease ()
		{
			DangerousRelease (handle);
		}

		internal static void DangerousRelease (IntPtr handle)
		{
			if (handle == IntPtr.Zero)
				return;
#if MONOMAC
			Messaging.void_objc_msgSend (handle, Selector.ReleaseHandle);
#else
			Messaging.void_objc_msgSend (handle, Selector.GetHandle (Selector.Release));
#endif
		}

		internal static void DangerousRetain (IntPtr handle)
		{
			if (handle == IntPtr.Zero)
				return;
#if MONOMAC
			Messaging.void_objc_msgSend (handle, Selector.RetainHandle);
#else
			Messaging.void_objc_msgSend (handle, Selector.GetHandle (Selector.Retain));
#endif
		}
			
		internal static void DangerousAutorelease (IntPtr handle)
		{
#if MONOMAC
			Messaging.void_objc_msgSend (handle, Selector.AutoreleaseHandle);
#else
			Messaging.void_objc_msgSend (handle, Selector.GetHandle (Selector.Autorelease));
#endif
		}

#if !XAMCORE_2_0
		[Obsolete ("Low-level API warning: Use at your own risk: this calls the Retain method on the underlying object; Use DangerousRetain to avoid this warning.")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		public NSObject Retain ()
		{
			return DangerousRetain ();
		}
#endif

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

#if !XAMCORE_2_0
		[Obsolete ("Low-level API warning: Use at your own risk: this calls the Retain method on the underlying object; Use DangerousAutorelease to avoid this warning.")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		public NSObject Autorelease ()
		{
			return DangerousAutorelease ();
		}
#endif

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

		public IntPtr SuperHandle {
			get {
				if (handle == IntPtr.Zero)
					throw new ObjectDisposedException (GetType ().Name);

				if (class_handle == IntPtr.Zero)
					class_handle = ClassHandle;

				unsafe {
					fixed (IntPtr *ptr = &handle)
						return (IntPtr) (ptr);
				}
			}
		}
		
		public IntPtr Handle {
			get { return handle; }
			set {
				if (handle == value)
					return;
				
				if (handle != IntPtr.Zero)
					Runtime.UnregisterNSObject (handle);
				
				handle = value;

				if (handle != IntPtr.Zero)
					Runtime.RegisterNSObject (this, handle);
			}
		}

		[EditorBrowsable (EditorBrowsableState.Never)]
		protected void InitializeHandle (IntPtr handle)
		{
			InitializeHandle (handle, "init*");
		}

		[EditorBrowsable (EditorBrowsableState.Never)]
		protected void InitializeHandle (IntPtr handle, string initSelector)
		{
			if (this.handle == IntPtr.Zero && Class.ThrowOnInitFailure) {
				if (ClassHandle == IntPtr.Zero)
					throw new Exception (string.Format ("Could not create an native instance of the type '{0}': the native class hasn't been loaded.\n" +
						"It is possible to ignore this condition by setting ObjCRuntime.Class.ThrowOnInitFailure to false.",
						GetType ().FullName));
				throw new Exception (string.Format ("Failed to create a instance of the native type '{0}'.\n" +
					"It is possible to ignore this condition by setting ObjCRuntime.Class.ThrowOnInitFailure to false.",
					new Class (ClassHandle).Name));
			}

			if (handle == IntPtr.Zero && Class.ThrowOnInitFailure) {
				Handle = IntPtr.Zero; // We'll crash if we don't do this.
				throw new Exception (string.Format ("Could not initialize an instance of the type '{0}': the native '{1}' method returned nil.\n" +
				"It is possible to ignore this condition by setting ObjCRuntime.Class.ThrowOnInitFailure to false.",
					GetType ().FullName, initSelector));
			}

			this.Handle = handle;
		}
		
		private bool AllocIfNeeded () {
			if (handle == IntPtr.Zero) {
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
		private IntPtr GetObjCIvar (string name) {
			IntPtr native;
			
			object_getInstanceVariable (handle, name, out native);
			
			return native;
		}
		
		[Obsolete ("Do not use; this API does not properly retain/release existing/new values, so leaks and/or crashes may occur.")]
		public NSObject GetNativeField (string name) {
			IntPtr field = GetObjCIvar (name);
			
			if (field == IntPtr.Zero)
				return null;
			return Runtime.GetNSObject (field);
		}
		
		private void SetObjCIvar (string name, IntPtr value) {
			object_setInstanceVariable (handle, name, value);
		}
		
		[Obsolete ("Do not use; this API does not properly retain/release existing/new values, so leaks and/or crashes may occur.")]
		public void SetNativeField (string name, NSObject value) {
			if (value == null)
				SetObjCIvar (name, IntPtr.Zero);
			else
				SetObjCIvar (name, value.Handle);
		}
		
		[DllImport ("/usr/lib/libobjc.dylib")]
		extern static void object_getInstanceVariable (IntPtr obj, string name, out IntPtr val);

		[DllImport ("/usr/lib/libobjc.dylib")]
		extern static void object_setInstanceVariable (IntPtr obj, string name, IntPtr val);
#endif // !XAMCORE_3_0

		private void InvokeOnMainThread (Selector sel, NSObject obj, bool wait)
		{
#if MONOMAC
			Messaging.void_objc_msgSend_IntPtr_IntPtr_bool (this.Handle, Selector.PerformSelectorOnMainThreadWithObjectWaitUntilDoneHandle, sel.Handle, obj == null ? IntPtr.Zero : obj.Handle, wait);
#else
			Messaging.void_objc_msgSend_IntPtr_IntPtr_bool (this.Handle, Selector.GetHandle (Selector.PerformSelectorOnMainThreadWithObjectWaitUntilDone), sel.Handle, obj == null ? IntPtr.Zero : obj.Handle, wait);
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
#if MONOMAC
			Messaging.void_objc_msgSend_IntPtr_IntPtr_bool (d.Handle, Selector.PerformSelectorOnMainThreadWithObjectWaitUntilDoneHandle, 
			                                                NSActionDispatcher.Selector.Handle, d.Handle, false);
#else
			Messaging.void_objc_msgSend_IntPtr_IntPtr_bool (d.Handle, Selector.GetHandle (Selector.PerformSelectorOnMainThreadWithObjectWaitUntilDone), 
			                                                Selector.GetHandle (NSActionDispatcher.SelectorName), d.Handle, false);
#endif
		}
		
		public void InvokeOnMainThread (Action action)
		{
			using (var d = new NSActionDispatcher (action)) {
#if MONOMAC
				Messaging.void_objc_msgSend_IntPtr_IntPtr_bool (d.Handle, Selector.PerformSelectorOnMainThreadWithObjectWaitUntilDoneHandle, 
				                                                NSActionDispatcher.Selector.Handle, d.Handle, true);
#else
				Messaging.void_objc_msgSend_IntPtr_IntPtr_bool (d.Handle, Selector.GetHandle (Selector.PerformSelectorOnMainThreadWithObjectWaitUntilDone), 
				                                                Selector.GetHandle (NSActionDispatcher.SelectorName), d.Handle, true);
#endif
			}
		}		

		public static NSObject FromObject (object obj)
		{
			if (obj == null)
				return NSNull.Null;
			var t = obj.GetType ();
			if (t == typeof (NSObject) || t.IsSubclassOf (typeof (NSObject)))
				return (NSObject) obj;
			
			switch (Type.GetTypeCode (t)){
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
				if (t == typeof (IntPtr))
					return NSValue.ValueFromPointer ((IntPtr) obj);
#if !NO_SYSTEM_DRAWING
				if (t == typeof (SizeF))
					return NSValue.FromSizeF ((SizeF) obj);
				else if (t == typeof (RectangleF))
					return NSValue.FromRectangleF ((RectangleF) obj);
				else if (t == typeof (PointF))
					return NSValue.FromPointF ((PointF) obj);
#endif
#if XAMCORE_2_0
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
#endif

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
				if (native != null)
					return Runtime.GetNSObject (native.Handle);
				return null;
			}
		}

		public void SetValueForKeyPath (IntPtr handle, NSString keyPath)
		{
			if (keyPath == null)
				throw new ArgumentNullException ("keyPath");
#if MONOMAC
			if (IsDirectBinding) {
				ObjCRuntime.Messaging.void_objc_msgSend_IntPtr_IntPtr (this.Handle, selSetValue_ForKeyPath_Handle, handle, keyPath.Handle);
			} else {
				ObjCRuntime.Messaging.void_objc_msgSendSuper_IntPtr_IntPtr (this.SuperHandle, selSetValue_ForKeyPath_Handle, handle, keyPath.Handle);
			}
#else
			if (IsDirectBinding) {
				ObjCRuntime.Messaging.void_objc_msgSend_IntPtr_IntPtr (this.Handle, Selector.GetHandle ("setValue:forKeyPath:"), handle, keyPath.Handle);
			} else {
				ObjCRuntime.Messaging.void_objc_msgSendSuper_IntPtr_IntPtr (this.SuperHandle, Selector.GetHandle ("setValue:forKeyPath:"), handle, keyPath.Handle);
			}
#endif
		}

#if XAMCORE_2_0
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
			if (obj == null)
				return false;
			var o = obj as NSObject;
			if (o == null)
				return false;
			// is only one is a direct binding then both cannot be equals
			if (IsDirectBinding != o.IsDirectBinding)
				return false;
			// we can only ask `isEqual:` to test equality if both objects are direct bindings
			return IsDirectBinding ? IsEqual (o) : Object.ReferenceEquals (this, obj);
		}

		// IEquatable<T>
		public bool Equals (NSObject obj)
		{
			if (obj == null)
				return false;
			// we'll ask the overridden Equals (if available) if one of the instances is not a direct binding
			return (IsDirectBinding && obj.IsDirectBinding) ? IsEqual (obj) : Equals ((object) obj);
		}
#endif

		public override string ToString ()
		{
			return Description ?? base.ToString ();
		}

		public virtual void Invoke (Action action, double delay)
		{
			var d = new NSAsyncActionDispatcher (action);
			d.PerformSelector (NSActionDispatcher.Selector, null, delay);
		}

		public virtual void Invoke (Action action, TimeSpan delay)
		{
			var d = new NSAsyncActionDispatcher (action);
			d.PerformSelector (NSActionDispatcher.Selector, null, delay.TotalSeconds);
		}

		internal void ClearHandle ()
		{
			handle = IntPtr.Zero;
		}

		protected virtual void Dispose (bool disposing) {
			if (disposed)
				return;
			disposed = true;
			
			if (handle != IntPtr.Zero) {
				if (disposing) {
					ReleaseManagedRef ();
				} else {
					NSObject_Disposer.Add (this);
				}
			}
		}

		[Register ("__NSObject_Disposer")]
		[Preserve (AllMembers=true)]
		internal class NSObject_Disposer : NSObject {
			static readonly List <NSObject> drainList1 = new List<NSObject> ();
			static readonly List <NSObject> drainList2 = new List<NSObject> ();
			static List <NSObject> handles = drainList1;

			static readonly IntPtr class_ptr = Class.GetHandle ("__NSObject_Disposer");
#if MONOMAC
			static readonly IntPtr drainHandle = Selector.GetHandle ("drain:");
#endif
			
			static readonly object lock_obj = new object ();
			
			private NSObject_Disposer ()
			{
				// Disable default ctor, there should be no instances of this class.
			}
			
			static internal void Add (NSObject handle) {
				bool call_drain;
				lock (lock_obj) {
					handles.Add (handle);
					call_drain = handles.Count == 1;
				}
				if (!call_drain)
					return;
#if MONOMAC
				Messaging.void_objc_msgSend_IntPtr_IntPtr_bool (class_ptr, Selector.PerformSelectorOnMainThreadWithObjectWaitUntilDoneHandle, drainHandle, IntPtr.Zero, false);
#else
				Messaging.void_objc_msgSend_IntPtr_IntPtr_bool (class_ptr, Selector.GetHandle (Selector.PerformSelectorOnMainThreadWithObjectWaitUntilDone), Selector.GetHandle ("drain:"), IntPtr.Zero, false);
#endif
			}
			
			[Export ("drain:")]
			static  void Drain (NSObject ctx) {
				List<NSObject> drainList;
				
				lock (lock_obj) {
					drainList = handles;
					if (handles == drainList1)
						handles = drainList2;
					else
						handles = drainList1;
				}
				
				foreach (NSObject x in drainList)
					x.ReleaseManagedRef ();
				drainList.Clear();
			}
		}
			
		[Register ("__XamarinObjectObserver")]
		class Observer : NSObject {
			WeakReference obj;
			Action<NSObservedChange> cback;
			NSString key;
			
			public Observer (NSObject obj, NSString key, Action<NSObservedChange> observer)
			{
				if (observer == null)
					throw new ArgumentNullException (nameof(observer));

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
					if (obj != null) {
						target = (NSObject) obj.Target;
						if (target != null)
							target.RemoveObserver (this, key, Handle);
					}
					obj = null;
					cback = null;
				} else {
					Console.Error.WriteLine ("Warning: observer object was not disposed manually with Dispose()");
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
#endif // !COREBUILD
	}

#if !COREBUILD
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
				if (n == null)
					return false;
				return n.BoolValue;
			}  
		}
	}
#endif
}
