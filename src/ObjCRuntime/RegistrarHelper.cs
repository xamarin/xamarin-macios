//
// RegistrarHelper.cs: Helper code for the managed static registra.
//
// Authors:
//   Rolf Bjarne Kvinge
//
// Copyright 2023 Microsoft Corp


// #define TRACE

#if NET

#nullable enable

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

using CoreFoundation;
using CoreGraphics;
using Foundation;

using Registrar;

using Xamarin.Bundler;

namespace ObjCRuntime {
	// This class contains helper methods for the managed static registrar.
	// The managed static registrar will make it public when needed.
	static class RegistrarHelper {
		class MapInfo {
			public IManagedRegistrar Registrar;
			public bool RegisteredWrapperTypes;

			public MapInfo (IManagedRegistrar registrar)
			{
				Registrar = registrar;
			}
		}

		// Ignore CS8618 for these two variables:
		//     Non-nullable variable must contain a non-null value when exiting constructor.
		// Because we won't use a static constructor to initialize them, instead we're using a module initializer,
		// it's safe to ignore this warning.
#pragma warning disable 8618
		static Dictionary<string, MapInfo> assembly_map;
		static Dictionary<RuntimeTypeHandle, RuntimeTypeHandle> wrapper_types;
		static StringEqualityComparer StringEqualityComparer;
		static RuntimeTypeHandleEqualityComparer RuntimeTypeHandleEqualityComparer;
#pragma warning restore 8618

		[ModuleInitializer]
		internal static void Initialize ()
		{
			StringEqualityComparer = new StringEqualityComparer ();
			RuntimeTypeHandleEqualityComparer = new RuntimeTypeHandleEqualityComparer ();
			assembly_map = new Dictionary<string, MapInfo> (StringEqualityComparer);
			wrapper_types = new Dictionary<RuntimeTypeHandle, RuntimeTypeHandle> (RuntimeTypeHandleEqualityComparer);
		}

		static NativeHandle CreateCFArray (params string[]? values)
		{
			if (values is null)
				return NativeHandle.Zero;
			return CFArray.Create (values);
		}

		unsafe static IntPtr GetBlockPointer (BlockLiteral block)
		{
			var rv = BlockLiteral._Block_copy (&block);
			block.Dispose ();
			return rv;
		}

		static IntPtr GetBlockForDelegate (object @delegate, RuntimeMethodHandle method_handle)
		{
			var method = (MethodInfo) MethodBase.GetMethodFromHandle (method_handle)!;
			return BlockLiteral.GetBlockForDelegate (method, @delegate, Runtime.INVALID_TOKEN_REF, null);
		}


		static MapInfo GetMapEntry (Assembly assembly)
		{
			return GetMapEntry (assembly.GetName ().Name!);
		}

		static MapInfo GetMapEntry (IntPtr assembly)
		{
			return GetMapEntry (Marshal.PtrToStringAuto (assembly)!);
		}

		static MapInfo GetMapEntry (string assemblyName)
		{
			if (TryGetMapEntry (assemblyName, out var rv))
				return rv;
			throw ErrorHelper.CreateError (8055, Errors.MX8055 /* Could not find the type 'ObjCRuntime.__Registrar__' in the assembly '{0}' */, assemblyName);
		}

		static bool TryGetMapEntry (string assemblyName, [NotNullWhen (true)] out MapInfo? entry)
		{
			entry = null;

			lock (assembly_map) {
				return assembly_map.TryGetValue (assemblyName, out entry);
			}
		}

		static void Register (IManagedRegistrar registrar)
		{
			var assembly = registrar.GetType ().Assembly;
			var assemblyName = assembly.GetName ().Name!;

#if TRACE
			Runtime.NSLog ($"RegistrarHelper.Register ('{assemblyName}', '{registrar.GetType ().AssemblyQualifiedName}')");
#endif

			lock (assembly_map) {
				assembly_map.Add (assemblyName, new MapInfo (registrar));
			}
		}

		internal static Type? FindProtocolWrapperType (Type type)
		{
			var typeHandle = type.TypeHandle;

			lock (assembly_map) {
				// First check if the type is already in our dictionary.
				if (wrapper_types.TryGetValue (typeHandle, out var wrapperType))
					return Type.GetTypeFromHandle (wrapperType);

				// Not in our dictionary, get the map entry to see if we've already
				// called RegisterWrapperTypes for this assembly,
				var entry = GetMapEntry (type.Assembly);
				if (!entry.RegisteredWrapperTypes) {
					entry.Registrar.RegisterWrapperTypes (wrapper_types);
					entry.RegisteredWrapperTypes = true;
				}

				// Return whatever's in the dictionary now.
				if (wrapper_types.TryGetValue (typeHandle, out wrapperType))
					return Type.GetTypeFromHandle (wrapperType);
			}

			return null;
		}

#if TRACE
		[ThreadStatic]
		static Stopwatch? lookupWatch;
#endif

		internal static IntPtr LookupUnmanagedFunction (IntPtr assembly, string? symbol, int id)
		{
			IntPtr rv;

#if TRACE
			if (lookupWatch is null)
				lookupWatch = new Stopwatch ();

			lookupWatch.Start ();
			Console.WriteLine ("LookupUnmanagedFunction (0x{0} = {1}, {2}, {3})", assembly.ToString ("x"), Marshal.PtrToStringAuto (assembly), symbol, id);
#endif

			if (id == -1) {
				rv = IntPtr.Zero;
			} else {
				rv = LookupUnmanagedFunctionInAssembly (assembly, symbol, id);
			}

#if TRACE
			lookupWatch.Stop ();

			Console.WriteLine ("LookupUnmanagedFunction (0x{0} = {1}, {2}, {3}) => 0x{4} ElapsedMilliseconds: {5}", assembly.ToString ("x"), Marshal.PtrToStringAuto (assembly), symbol, id, rv.ToString ("x"), lookupWatch.ElapsedMilliseconds);
#endif

			if (rv != IntPtr.Zero)
				return rv;

			throw ErrorHelper.CreateError (8001, "Unable to find the managed function with id {0} ({1})", id, symbol);
		}

		static IntPtr LookupUnmanagedFunctionInAssembly (IntPtr assembly_name, string? symbol, int id)
		{
			var entry = GetMapEntry (assembly_name);
			return entry.Registrar.LookupUnmanagedFunction (symbol, id);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		static bool RuntimeTypeHandleEquals (ref RuntimeTypeHandle typeHandle, RuntimeTypeHandle otherTypeHandle)
		{
			return typeHandle.Equals (otherTypeHandle);
		}

		internal static Type LookupRegisteredType (Assembly assembly, uint id)
		{
			var entry = GetMapEntry (assembly);
			var handle = entry.Registrar.LookupType (id);
			return Type.GetTypeFromHandle (handle)!;
		}

		internal static uint LookupRegisteredTypeId (Type type)
		{
			if (!TryGetMapEntry (type.Assembly.GetName ().Name!, out var entry))
				return Runtime.INVALID_TOKEN_REF;
			return entry.Registrar.LookupTypeId (type.TypeHandle);
		}

		internal static T? ConstructNSObject<T> (Type type, NativeHandle nativeHandle)
			where T : class, INativeObject
		{
			if (!TryGetMapEntry (type.Assembly.GetName ().Name!, out var entry))
				return null;
			return (T?) entry.Registrar.ConstructNSObject (type.TypeHandle, nativeHandle);
		}

		internal static T? ConstructINativeObject<T> (Type type, NativeHandle nativeHandle, bool owns)
			where T : class, INativeObject
		{
			if (!TryGetMapEntry (type.Assembly.GetName ().Name!, out var entry))
				return null;
			return (T?) entry.Registrar.ConstructINativeObject (type.TypeHandle, nativeHandle, owns);
		}

		// helper functions for converting between native and managed objects
		static NativeHandle ManagedArrayToNSArray (object array, bool retain)
		{
			if (array is null)
				return NativeHandle.Zero;

			NSObject rv;
			if (array is NSObject[] nsobjs) {
				rv = NSArray.FromNSObjects (nsobjs);
			} else if (array is INativeObject[] inativeobjs) {
				rv = NSArray.FromNSObjects (inativeobjs);
			} else {
				throw new InvalidOperationException ($"Can't convert {array.GetType ()} to an NSArray."); // FIXME: better error
			}

			if (retain)
				return Runtime.RetainNSObject (rv);
			return Runtime.RetainAndAutoreleaseNSObject (rv);
		}

		unsafe static void NSArray_string_native_to_managed (IntPtr* ptr, ref string[]? value, ref string[]? copy)
		{
			if (ptr is not null) {
				value = NSArray.StringArrayFromHandle (*ptr);
			} else {
				value = null;
			}
			copy = value;
		}

		unsafe static void NSArray_string_managed_to_native (IntPtr* ptr, string[] value, string[] copy, bool isOut)
		{
			if (ptr is null)
				return;

			// Note that we won't notice if individual array elements change, only if the array itself changes
			if (!isOut && (object) value == (object) copy) {
#if TRACE
				Runtime.NSLog ($"NSArray_string_managed_to_native (0x{(*ptr).ToString ("x")}, equal)");
#endif
				return;
			}
			if (value is null) {
#if TRACE
				Runtime.NSLog ($"NSArray_string_managed_to_native (0x{(*ptr).ToString ("x")}, null, !null)");
#endif
				*ptr = IntPtr.Zero;
				return;
			}
			IntPtr rv = Runtime.RetainAndAutoreleaseNSObject (NSArray.FromStrings (value));
#if TRACE
			Runtime.NSLog ($"NSArray_string_managed_to_native (0x{(*ptr).ToString ("x")}, value != copy: {value?.Length} != {copy?.Length}): 0x{rv.ToString ("x")} => {value?.GetType ()}");
#endif
			*ptr = rv;
		}

		unsafe static void NSArray_native_to_managed<T> (IntPtr* ptr, ref T[]? value, ref T[]? copy) where T: class, INativeObject
		{
			if (ptr is not null) {
				value = NSArray.ArrayFromHandle<T> (*ptr);
			} else {
				value = null;
			}
			copy = value;
		}

		unsafe static void NSArray_managed_to_native<T> (IntPtr* ptr, T[] value, T[] copy, bool isOut) where T: class, INativeObject
		{
			if (ptr is null) {
#if TRACE
				Runtime.NSLog ($"NSArray_managed_to_native (NULL, ?, ?)");
#endif
				return;
			}
			// Note that we won't notice if individual array elements change, only if the array itself changes
			if (!isOut && (object) value == (object) copy) {
#if TRACE
				Runtime.NSLog ($"NSArray_managed_to_native (0x{(*ptr).ToString ("x")}, equal)");
#endif
				return;
			}
			if (value is null) {
#if TRACE
				Runtime.NSLog ($"NSArray_managed_to_native (0x{(*ptr).ToString ("x")}, null, !null)");
#endif
				*ptr = IntPtr.Zero;
				return;
			}
			IntPtr rv = Runtime.RetainAndAutoreleaseNSObject (NSArray.FromNSObjects<T> (value));
#if TRACE
			Runtime.NSLog ($"NSArray_managed_to_native (0x{(*ptr).ToString ("x")}, value != copy: {value?.Length} != {copy?.Length}): 0x{rv.ToString ("x")} => {value?.GetType ()}");
#endif
			*ptr = rv;
		}

		unsafe static void NSObject_native_to_managed<T> (IntPtr* ptr, ref T? value, ref T? copy) where T: NSObject
		{
			if (ptr is not null) {
				value = Runtime.GetNSObject<T> (*ptr, owns: false);
			} else {
				value = null;
			}
			copy = value;
		}

		unsafe static void NSObject_managed_to_native (IntPtr* ptr, NSObject value, NSObject copy, bool isOut)
		{
			if (ptr is null) {
#if TRACE
				Runtime.NSLog ($"NSObject_managed_to_native (NULL, ?, ?)");
#endif
				return;
			}
			if (!isOut && (object) value == (object) copy) {
#if TRACE
				Runtime.NSLog ($"NSObject_managed_to_native (0x{(*ptr).ToString ("x")}, equal)");
#endif
				return;
			}
			IntPtr rv = Runtime.RetainAndAutoreleaseNSObject (value);
#if TRACE
			Runtime.NSLog ($"NSObject_managed_to_native (0x{(*ptr).ToString ("x")}, ? != ?): 0x{rv.ToString ("x")} => {value?.GetType ()}");
#endif
			*ptr = rv;
		}

		unsafe static void string_native_to_managed (NativeHandle *ptr, ref string? value, ref string? copy)
		{
			if (ptr is not null) {
				value = CFString.FromHandle (*ptr);
			} else {
				value = null;
			}
			copy = value;
		}

		unsafe static void string_managed_to_native (NativeHandle *ptr, string value, string copy, bool isOut)
		{
			if (ptr is null) {
#if TRACE
				Runtime.NSLog ($"string_managed_to_native (NULL, ?, ?)");
#endif
				return;
			}
			if (!isOut && (object) value == (object) copy) {
#if TRACE
				Runtime.NSLog ($"string_managed_to_native (0x{(*ptr).ToString ()}, equal)");
#endif
				return;
			}
			var rv = CFString.CreateNative (value);
#if TRACE
			Runtime.NSLog ($"string_managed_to_native (0x{(*ptr).ToString ()}, ? != ?): 0x{rv.ToString ()} => {value}");
#endif
			*ptr = rv;
		}

		unsafe static void INativeObject_native_to_managed<T> (IntPtr* ptr, ref T? value, ref T? copy, RuntimeTypeHandle implementationType) where T: class, INativeObject
		{
			if (ptr is not null) {
				value = Runtime.GetINativeObject<T> (*ptr, implementation: Type.GetTypeFromHandle (implementationType), forced_type: false, owns: false);
			} else {
				value = null;
			}
			copy = value;
		}

		unsafe static void INativeObject_managed_to_native (IntPtr *ptr, INativeObject value, INativeObject copy, bool isOut)
		{
			if (ptr is null) {
#if TRACE
				Runtime.NSLog ($"INativeObject_managed_to_native (NULL, ?, ?)");
#endif
				return;
			}
			if (!isOut && (object) value == (object) copy) {
#if TRACE
				Runtime.NSLog ($"INativeObject_managed_to_native (0x{(*ptr).ToString ("x")}, equal)");
#endif
				return;
			}
			IntPtr rv = value.GetHandle ();
#if TRACE
			Runtime.NSLog ($"INativeObject_managed_to_native (0x{(*ptr).ToString ("x")}, ? != ?): 0x{rv.ToString ("x")} => {value?.GetType ()}");
#endif
			*ptr = rv;
		}
	}
}

#endif // NET
