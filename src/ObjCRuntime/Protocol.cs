//
// Protocol.cs
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

#nullable enable

using System;
using System.Runtime.InteropServices;

using Foundation;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace ObjCRuntime {
	public partial class Protocol : INativeObject {
#if !COREBUILD
		NativeHandle handle;

		public Protocol (string name)
		{
			this.handle = objc_getProtocol (name);

			if (this.handle == IntPtr.Zero)
				throw new ArgumentException (String.Format ("'{0}' is an unknown protocol", name));
		}

		public Protocol (Type type)
		{
			this.handle = Runtime.GetProtocolForType (type);
		}

		public Protocol (NativeHandle handle)
		{
			this.handle = handle;
		}

		[Preserve (Conditional = true)]
		internal Protocol (NativeHandle handle, bool owns)
		{
			// protocols can't be freed, so we ignore the 'owns' parameter.
			this.handle = handle;
		}

		/// <summary>Handle (pointer) to the unmanaged object representation.</summary>
		///         <value>A pointer</value>
		///         <remarks>
		///           <para>This IntPtr is a handle to the underlying unmanaged representation for this object.</para>
		///         </remarks>
		public NativeHandle Handle {
			get { return this.handle; }
		}

		/// <summary>Name of the protocol.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		public string? Name {
			get {
				IntPtr ptr = protocol_getName (Handle);
				return Marshal.PtrToStringAuto (ptr);
			}
		}

		public static IntPtr GetHandle (string name)
		{
			return objc_getProtocol (name);
		}

		[DllImport (Messaging.LIBOBJC_DYLIB)]
		extern static IntPtr objc_getProtocol (IntPtr name);

		internal static IntPtr objc_getProtocol (string? name)
		{
			var namePtr = new TransientString (name);
			return objc_getProtocol (namePtr);
		}

		[DllImport (Messaging.LIBOBJC_DYLIB)]
		extern static IntPtr objc_allocateProtocol (IntPtr name);

		internal static IntPtr objc_allocateProtocol (string name)
		{
			using var namePtr = new TransientString (name);
			return objc_allocateProtocol (namePtr);
		}

		[DllImport (Messaging.LIBOBJC_DYLIB)]
		internal extern static void objc_registerProtocol (IntPtr protocol);

		[DllImport (Messaging.LIBOBJC_DYLIB)]
		extern static unsafe void protocol_addProperty (IntPtr protocol, IntPtr name, IntPtr* attributes, int count, byte isRequired, byte isInstance);

		internal static void protocol_addProperty (IntPtr protocol, string name, Class.objc_attribute_prop [] attributes, int count, bool isRequired, bool isInstance)
		{
			using var namePtr = new TransientString (name);
			var propArr = Class.PropertyStringsToPtrs (attributes);
			unsafe {
				fixed (IntPtr* propArrPtr = propArr) {
					protocol_addProperty (protocol, namePtr, propArrPtr, count, isRequired.AsByte (), isInstance.AsByte ());
				}
			}
			Class.FreeStringPtrs (propArr);
		}

		[DllImport (Messaging.LIBOBJC_DYLIB)]
		extern static void protocol_addMethodDescription (IntPtr protocol, IntPtr nameSelector, IntPtr signature, byte isRequired, byte isInstance);

		internal static void protocol_addMethodDescription (IntPtr protocol, IntPtr nameSelector, string signature, bool isRequired, bool isInstance)
		{
			using var signaturePtr = new TransientString (signature);
			protocol_addMethodDescription (protocol, nameSelector, signaturePtr, isRequired.AsByte (), isInstance.AsByte ());
		}

		[DllImport (Messaging.LIBOBJC_DYLIB)]
		internal extern static void protocol_addProtocol (IntPtr protocol, IntPtr addition);

		[DllImport (Messaging.LIBOBJC_DYLIB)]
		internal extern static IntPtr protocol_getName (IntPtr protocol);
#endif
	}
}
