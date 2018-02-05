//
// Class.cs
//
// Copyright 2009 Novell, Inc
// Copyright 2011 - 2015 Xamarin Inc. All rights reserved.
//

// #define LOG_TYPELOAD

using System;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Foundation;
#if !COREBUILD
using Registrar;
#endif

namespace ObjCRuntime {
	public partial class Class : INativeObject
	{
#if !COREBUILD
		public static bool ThrowOnInitFailure = true;

		internal IntPtr handle;

		internal unsafe static void Initialize (Runtime.InitializationOptions* options)
		{
			var map = options->RegistrationMap;

			if (map == null)
				return;

			for (int i = 0; i < map->assembly_count; i++) {
				var ptr = Marshal.ReadIntPtr (map->assembly, i * IntPtr.Size);
				Runtime.Registrar.SetAssemblyRegistered (Marshal.PtrToStringAuto (ptr));
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

		internal unsafe static Type FindType (IntPtr @class, out bool is_custom_type)
		{
			var map = Runtime.options->RegistrationMap;
			Runtime.MTClassMap? entry = null;

			is_custom_type = false;

			if (map == null) {
#if LOG_TYPELOAD
				Console.WriteLine ($"FindType (0x{@class:X} = {Marshal.PtrToStringAuto (class_getName (@class))}) => found no map.");
#endif
				return null;
			}

			// Find the ObjC class pointer in our map
			// Potential improvement: order the type handles after loading them, which means we could do a binary search here.
			// A binary search will likely be faster than a dictionary for any real-world scenario (and if slower, not much slower),
			// but it would need a lot less memory (very little when sorting, could probably use stack memory, and then nothing at all afterwards).
			for (int i = 0; i < map->map_count; i++) {
				if (map->map [i].handle != @class)
					continue;

				entry = map->map [i];
				is_custom_type = i >= (map->map_count - map->custom_type_count);
				break;
			}

			if (!entry.HasValue) {
#if LOG_TYPELOAD
				Console.WriteLine ($"FindType (0x{@class:X} = {Marshal.PtrToStringAuto (class_getName (@class))}) => found no type.");
#endif
				return null;
			}

			// Resolve the map entry we found to a managed type
			var member = ResolveTokenReference (entry.Value.type_reference, 0x02000000);
			var type = member as Type;

			if (type == null && member != null)
				throw ErrorHelper.CreateError (8022, $"Expected the token reference 0x{entry.Value.type_reference:X} to be a type, but it's a {member.GetType ().Name}. Please file a bug report at http://bugzilla.xamarin.com.");

#if LOG_TYPELOAD
			Console.WriteLine ($"FindType (0x{@class:X} = {Marshal.PtrToStringAuto (class_getName (@class))}) => {type.FullName}; is custom: {is_custom_type} (token reference: 0x{entry.Value.type_reference:X}).");
#endif

			return type;
		}

		internal unsafe static MemberInfo ResolveFullTokenReference (uint token_reference)
		{
			// sizeof (MTFullTokenReference) = IntPtr.Size + 4 + 4
			var entry = Runtime.options->RegistrationMap->full_token_references + (IntPtr.Size + 8) * (int) (token_reference >> 1);
			var assembly_name = Marshal.PtrToStringAuto (Marshal.ReadIntPtr (entry));
			var module_token = (uint) Marshal.ReadInt32 (entry + IntPtr.Size);
			var token = (uint) Marshal.ReadInt32 (entry + IntPtr.Size + 4);

#if LOG_TYPELOAD
			Console.WriteLine ($"ResolveFullTokenReference (0x{token_reference:X}) assembly name: {assembly_name} module token: 0x{module_token:X} token: 0x{token:X}.");
#endif

			var assembly = ResolveAssembly (assembly_name);
			var module = ResolveModule (assembly, module_token);
			return ResolveToken (module, token);
		}

		internal unsafe static MemberInfo ResolveTokenReference (uint token_reference, uint implicit_token_type)
		{
			var map = Runtime.options->RegistrationMap;

			if ((token_reference & 0x1) == 0x1)
				return ResolveFullTokenReference (token_reference);

			var assembly_index = (token_reference >> 1) & 0x7F;
			uint token = (token_reference >> 8) + implicit_token_type;

#if LOG_TYPELOAD
			Console.WriteLine ($"ResolveTokenReference (0x{token_reference:X}) assembly index: {assembly_index} token: 0x{token:X}.");
#endif

			var assembly_name = Marshal.PtrToStringAuto (Marshal.ReadIntPtr (map->assembly, (int) assembly_index * IntPtr.Size));
			var assembly = ResolveAssembly (assembly_name);
			var module = ResolveModule (assembly, 0x1);

			return ResolveToken (module, token | implicit_token_type);
		}

		static MemberInfo ResolveToken (Module module, uint token)
		{
			// Finally resolve the token.
			var token_type = token & 0xFF000000;
			switch (token & 0xFF000000) {
			case 0x02000000: // TypeDef
				var type = module.ResolveType ((int) token);
#if LOG_TYPELOAD
				Console.WriteLine ($"ResolveToken (0x{token:X}) => Type: {type.FullName}");
#endif
				return type;
			case 0x06000000: // Method
				var method = module.ResolveMethod ((int) token);
#if LOG_TYPELOAD
				Console.WriteLine ($"ResolveToken (0x{token:X}) => Method: {method.DeclaringType.FullName}.{method.Name}");
#endif
				return method;
			default:
				throw ErrorHelper.CreateError (8021, $"Unknown implicit token type: 0x{token_type}.");
			}
		}

		static Module ResolveModule (Assembly assembly, uint token)
		{
			foreach (var mod in assembly.GetModules ()) {
				if (mod.MetadataToken != token)
					continue;

#if LOG_TYPELOAD
				Console.WriteLine ($"ResolveModule (\"{assembly.FullName}\", 0x{token:X}): {mod.Name}.");
#endif
				return mod;
			}

			throw ErrorHelper.CreateError (8020, $"Could not find the module with MetadataToken 0x{token:X} in the assembly {assembly}.");
		}

		static Assembly ResolveAssembly (string assembly_name)
		{
			// Find the assembly. We've already loaded all the assemblies that contain registered types, so just look at those assemblies.
			foreach (var asm in AppDomain.CurrentDomain.GetAssemblies ()) {
				if (assembly_name != asm.GetName ().Name)
					continue;

#if LOG_TYPELOAD
				Console.WriteLine ($"ResolveAssembly (\"{assembly_name}\"): {asm.FullName}.");
#endif
				return asm;
			}

			throw ErrorHelper.CreateError (8019, $"Could not find the assembly {assembly_name} in the loaded assemblies.");
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
