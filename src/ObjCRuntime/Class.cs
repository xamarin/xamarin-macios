//
// Class.cs
//
// Copyright 2009 Novell, Inc
// Copyright 2011 - 2015 Xamarin Inc. All rights reserved.
//

using System;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using XamCore.Foundation;
#if !COREBUILD
using XamCore.Registrar;
#endif

namespace XamCore.ObjCRuntime {
	public partial class Class : INativeObject
	{
#if !COREBUILD
		public static bool ThrowOnInitFailure = true;

		internal IntPtr handle;

		internal unsafe static void Initialize (ref Runtime.InitializationOptions options)
		{
			if (options.RegistrationData != null) {
				var map = options.RegistrationData->map;
				var lazy_map = Runtime.Registrar.GetRegistrationMap (options.RegistrationData->total_count);
				while (map != null) {
					RegisterMap (map, lazy_map);
					map = map->next;
				}
			}
		}

		static unsafe void RegisterMap (Runtime.MTRegistrationMap *registration_map, Dictionary<IntPtr, LazyMapEntry> lazy_map)
		{
			var map = registration_map->map;
			var size = registration_map->map_count;
			var first_custom_type = size - registration_map->custom_type_count;

#if LOG_MAP
			Runtime.NSLog ("RegisterMap () {0} assemblies with {1} types", registration_map->assembly_count, registration_map->map_count);
#endif

			for (int i = 0; i < registration_map->assembly_count; i++) {
				var assembly = Marshal.PtrToStringAuto (Marshal.ReadIntPtr (registration_map->assembly, i * IntPtr.Size));
#if LOG_MAP
				Runtime.NSLog ("    {0}", assembly);
#endif
				Runtime.Registrar.SetAssemblyRegistered (assembly);
			}

			for (int i = 0; i < size; i++) {
				if (map [i].handle == IntPtr.Zero)
					continue; 

				sbyte* ptr = map [i].typename;
				int num = 0;
				while (0 != *ptr++)
					num++;

				var entry = new LazyMapEntry ();
				entry.Typename = new String (map [i].typename, 0, num, System.Text.Encoding.UTF8);
				entry.IsCustomType = i >= first_custom_type;
				lazy_map [map [i].handle] = entry;

#if LOG_MAP
				Runtime.NSLog ("    {0} => 0x{1} IsCustomType: {2}", entry.Typename, map [i].handle.ToString ("x"), entry.IsCustomType);
#endif
			}
		}

		public Class (string name)
		{
			this.handle = objc_getClass (name);

			if (this.handle == IntPtr.Zero)
				throw new ArgumentException (String.Format ("'{0}' is an unknown class", name));
		}

		public Class (Type type)
		{
			this.handle = Class.Register (type);
		}

		public Class (IntPtr handle)
		{
			this.handle = handle;
		}

		internal static Class Construct (IntPtr handle) 
		{
			return new Class (handle);
		}

		public IntPtr Handle {
			get { return this.handle; }
		}

		public IntPtr SuperClass {
			get { return class_getSuperclass (handle); }
		}

		public unsafe string Name {
			get {
				IntPtr ptr = class_getName (this.handle);
				return Marshal.PtrToStringAuto (ptr);
			}
		}

		public static IntPtr GetHandle (string name)
		{
			return objc_getClass (name);
		}

		// This method is treated as an intrinsic operation by
		// the aot compiler, generating a static reference to the
		// class (it will be faster than GetHandle, but it will
		// not compile unless the class in question actually exists
		// as an ObjectiveC class in the binary).
		public static IntPtr GetHandleIntrinsic (string name) {
			return objc_getClass (name);
		}

		public static IntPtr GetHandle (Type type) {
			return Register (type);
		}

		internal static IntPtr GetClassForObject (IntPtr obj)
		{
			return Messaging.IntPtr_objc_msgSend (obj, Selector.GetHandle (Selector.Class));
		}

		// note: PreserveCode.cs keep this around only for debug builds (see: monotouch-glue.m)
		internal static string LookupFullName (IntPtr klass)
		{
			Type type = Lookup (klass);
			return type == null ? null : type.FullName;
		}

		public static Type Lookup (Class @class)
		{
			return Lookup (@class.Handle, true);
		}

		internal static Type Lookup (IntPtr klass)
		{
			return Lookup (klass, true);
		}

		internal static Type Lookup (IntPtr klass, bool throw_on_error)
		{
			return Runtime.Registrar.Lookup (klass, throw_on_error);
		}

		internal static IntPtr Register (Type type)
		{
			return Runtime.Registrar.Register (type);
		}

		internal static void Register (Type type, ref List<Exception> exceptions)
		{
			Runtime.Registrar.Register (type, ref exceptions);
		}

		internal static Dictionary <IntPtr, MethodDescription> GetMethods (Type t)
		{
			return Runtime.Registrar.GetMethods (t);
		}

		/*
		Type must have been previously registered.
		*/
#if !XAMCORE_2_0 && !MONOTOUCH // Accidently exposed this to public, can't break API
		public
#else
		internal
#endif
		static bool IsCustomType (Type type)
		{
			return Runtime.Registrar.IsCustomType (type);
		}

		[DllImport ("/usr/lib/libobjc.dylib")]
		internal static extern IntPtr objc_allocateClassPair (IntPtr superclass, string name, IntPtr extraBytes);

		[DllImport ("/usr/lib/libobjc.dylib")]
		internal static extern IntPtr objc_getClass (string name);

		[DllImport ("/usr/lib/libobjc.dylib")]
		internal static extern void objc_registerClassPair (IntPtr cls);

		[DllImport ("/usr/lib/libobjc.dylib")]
		internal static extern bool class_addIvar (IntPtr cls, string name, IntPtr size, byte alignment, string types);

		[DllImport ("/usr/lib/libobjc.dylib")]
		internal static extern bool class_addMethod (IntPtr cls, IntPtr name, IntPtr imp, string types);

		[DllImport ("/usr/lib/libobjc.dylib")]
		internal extern static bool class_addMethod (IntPtr cls, IntPtr name, Delegate imp, string types);

		[DllImport ("/usr/lib/libobjc.dylib")]
		internal extern static bool class_addProtocol (IntPtr cls, IntPtr protocol);

		[DllImport ("/usr/lib/libobjc.dylib")]
		internal static extern IntPtr class_getName (IntPtr cls);

		[DllImport ("/usr/lib/libobjc.dylib")]
		internal static extern IntPtr class_getSuperclass (IntPtr cls);

		[DllImport ("/usr/lib/libobjc.dylib")]
		internal static extern IntPtr object_getClass (IntPtr obj);

		[DllImport ("/usr/lib/libobjc.dylib")]
		internal extern static IntPtr class_getMethodImplementation (IntPtr cls, IntPtr sel);

		[DllImport ("/usr/lib/libobjc.dylib")]
		internal extern static IntPtr class_getInstanceVariable (IntPtr cls, string name);

#if MONOMAC && !XAMCORE_2_0
		[DllImport ("/usr/lib/libc.dylib", SetLastError=true)]
		internal extern static int mprotect (IntPtr addr, nint len, int prot);

		[DllImport ("/usr/lib/libc.dylib", SetLastError=true)]
		static extern IntPtr mmap (IntPtr start, ulong length, int prot, int flags, int fd, long offset);
#endif
		
		[DllImport ("/usr/lib/libobjc.dylib", CharSet=CharSet.Ansi)]
		internal extern static bool class_addProperty (IntPtr cls, string name, objc_attribute_prop [] attributes, int count);

		[StructLayout (LayoutKind.Sequential, CharSet=CharSet.Ansi)]
		internal struct objc_attribute_prop {
			[MarshalAs (UnmanagedType.LPStr)] internal string name;
			[MarshalAs (UnmanagedType.LPStr)] internal string value;
		}
#endif // !COREBUILD
	}
}
