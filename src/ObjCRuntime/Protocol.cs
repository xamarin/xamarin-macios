//
// Protocol.cs
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

using System;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using XamCore.Foundation;

namespace XamCore.ObjCRuntime {
	public partial class Protocol : INativeObject {
#if !COREBUILD
		IntPtr handle;

		public Protocol (string name)
		{
			this.handle = objc_getProtocol (name);

			if (this.handle == IntPtr.Zero)
				throw new ArgumentException (String.Format ("'{0}' is an unknown protocol", name));
		}

		public Protocol (Type type)
		{
			if (type.IsInterface) {
				foreach (var pa in type.GetCustomAttributes<ProtocolAttribute> (false)) {
					handle = objc_getProtocol (pa.Name);
					if (handle != IntPtr.Zero)
						return;
				}
			}
			if (handle == IntPtr.Zero)
				throw new ArgumentException (string.Format ("'{0}' is an unknown protocol", type.FullName));
		}

		public Protocol (IntPtr handle)
		{
			this.handle = handle;
		}

		[Preserve (Conditional = true)]
		internal Protocol (IntPtr handle, bool owns)
		{
			// protocols can't be freed, so we ignore the 'owns' parameter.
			this.handle = handle;
		}

		public IntPtr Handle {
			get { return this.handle; }
		}

		public string Name {
			get {
				IntPtr ptr = protocol_getName (this.handle);
				return Marshal.PtrToStringAuto (ptr);
			}
		}

		public static IntPtr GetHandle (string name)
		{
			return objc_getProtocol (name);
		}

		[DllImport ("/usr/lib/libobjc.dylib")]
		internal extern static IntPtr objc_getProtocol (string name);

		[DllImport ("/usr/lib/libobjc.dylib")]
		internal extern static IntPtr objc_getProtocol (IntPtr ptr);

		[DllImport ("/usr/lib/libobjc.dylib")]
		internal extern static bool protocol_conformsToProtocol (IntPtr proto, IntPtr other);

		[DllImport ("/usr/lib/libobjc.dylib")]
		internal extern static IntPtr objc_allocateProtocol (string name);

		[DllImport ("/usr/lib/libobjc.dylib")]
		internal extern static void objc_registerProtocol (IntPtr protocol);

		[DllImport ("/usr/lib/libobjc.dylib")]
		internal extern static void protocol_addProperty (IntPtr protocol, string name, Class.objc_attribute_prop [] attributes, int count, bool isRequired, bool isInstance);

		[DllImport ("/usr/lib/libobjc.dylib")]
		internal extern static void protocol_addMethodDescription (IntPtr protocol, IntPtr nameSelector, string signature, bool isRequired, bool isInstance);

		[DllImport ("/usr/lib/libobjc.dylib")]
		internal extern static void protocol_addProtocol (IntPtr protocol, IntPtr addition);

		[DllImport ("/usr/lib/libobjc.dylib")]
		internal extern static IntPtr protocol_getName (IntPtr protocol);
#endif
	}
}
