//
// Class.cs
//
// Copyright 2009 Novell, Inc
// Copyright 2011 - 2015 Xamarin Inc. All rights reserved.
//

// #define LOG_TYPELOAD

#nullable enable

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Foundation;
#if !COREBUILD
using Registrar;
#endif

#if !COREBUILD
using Xamarin.Bundler;
#endif

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace ObjCRuntime {
	public partial class Class : INativeObject
#if !COREBUILD
	, IEquatable<Class>
#endif
	{
#if !COREBUILD
		NativeHandle handle;

		public static bool ThrowOnInitFailure = true;

		// We use the last significant bit of the IntPtr to store if this is a custom class or not.
#pragma warning disable CS8618 // "Non-nullable field must contain a non-null value when exiting constructor." - we ensure these fields are non-null in other ways
		static Dictionary<Type, IntPtr> type_to_class; // accessed from multiple threads, locking required.
		static ConditionalWeakTable<Assembly, string> assembly_to_name; // accessed from multiple threads, but ConditionalWeakTables are thread-safe, so no locking required.
		static Dictionary<ulong, MemberInfo?> token_to_member; // accessed from multiple threads, locking required.
		static Type? [] class_to_type;
#pragma warning restore CS8618

		[BindingImpl (BindingImplOptions.Optimizable)]
		internal unsafe static void Initialize (Runtime.InitializationOptions* options)
		{
			type_to_class = new Dictionary<Type, IntPtr> (Runtime.TypeEqualityComparer);

			var map = options->RegistrationMap;
			if (map is null)
				return;

			assembly_to_name = new ConditionalWeakTable<Assembly, string> ();
			token_to_member = new Dictionary<ulong, MemberInfo?> (Runtime.UInt64EqualityComparer);
			class_to_type = new Type? [map->map_count];

			if (!Runtime.DynamicRegistrationSupported)
				return; // Only the dynamic registrar needs the list of registered assemblies.


			for (int i = 0; i < map->assembly_count; i++) {
				var assembly = map->assemblies [i];
				Runtime.Registrar.SetAssemblyRegistered (Marshal.PtrToStringAuto (assembly.name));
			}
		}

		public Class (string name)
		{
			this.handle = objc_getClass (name);

			if (handle == NativeHandle.Zero)
				ObjCRuntime.ThrowHelper.ThrowArgumentException (nameof (name), $"Unknown class {name}");
		}

		public Class (Type type)
		{
			this.handle = GetHandle (type);
		}

		public Class (NativeHandle handle)
		{
			this.handle = handle;
		}

		[Preserve (Conditional = true)]
#if NET
		internal Class (NativeHandle handle, bool owns)
#else
		public Class (NativeHandle handle, bool owns)
#endif
		{
			// Class(es) can't be freed, so we ignore the 'owns' parameter.
			this.handle = handle;
		}

		public NativeHandle Handle {
			get { return this.handle; }
		}

		public NativeHandle SuperClass {
			get { return class_getSuperclass (Handle); }
		}

		public string? Name {
			get {
				var ptr = class_getName (Handle);
				return Marshal.PtrToStringAuto (ptr);
			}
		}

		public static NativeHandle GetHandle (string name)
		{
			return objc_getClass (name);
		}

		public override bool Equals (object? right)
		{
			return Equals (right as Class);
		}

		public bool Equals (Class? right)
		{
			if (right is null)
				return false;

			return Handle == right.Handle;
		}

		public override int GetHashCode ()
		{
			return Handle.GetHashCode ();
		}

		// This method is treated as an intrinsic operation by
		// the aot compiler, generating a static reference to the
		// class (it will be faster than GetHandle, but it will
		// not compile unless the class in question actually exists
		// as an ObjectiveC class in the binary).
		public static NativeHandle GetHandleIntrinsic (string name)
		{
			return objc_getClass (name);
		}

		public static NativeHandle GetHandle (Type type)
		{
			return GetClassHandle (type, true, out _);
		}

		[BindingImpl (BindingImplOptions.Optimizable)] // To inline the Runtime.DynamicRegistrationSupported code if possible.
		static IntPtr GetClassHandle (Type type, bool throw_if_failure, out bool is_custom_type)
		{
			IntPtr @class = IntPtr.Zero;

			if (type.IsByRef || type.IsPointer || type.IsArray) {
				is_custom_type = false;
				return IntPtr.Zero;
			}

			// We cache results in a dictionary (type_to_class) - we put failures (when @class = IntPtr.Zero) in the dictionary as well.
			// We do as little as possible with the lock held (only fetch/add to the dictionary, nothing else)

			bool found;
			lock (type_to_class)
				found = type_to_class.TryGetValue (type, out @class);

			if (!found) {
				@class = FindClass (type, out is_custom_type);
				lock (type_to_class)
					type_to_class [type] = @class + (is_custom_type ? 1 : 0);
			} else {
				is_custom_type = (@class.ToInt64 () & 1) == 1;
				if (is_custom_type)
					@class -= 1;
			}

			if (@class == IntPtr.Zero) {
				if (!Runtime.DynamicRegistrationSupported) {
					if (throw_if_failure)
						throw ErrorHelper.CreateError (8026, $"Can't register the class {type.FullName} when the dynamic registrar has been linked away.");
					return IntPtr.Zero;
				}
				@class = Register (type);
				is_custom_type = Runtime.Registrar.IsCustomType (type);
				lock (type_to_class)
					type_to_class [type] = @class + (is_custom_type ? 1 : 0);
			}

			return @class;
		}

		internal static IntPtr GetClassForObject (IntPtr obj)
		{
			return Messaging.IntPtr_objc_msgSend (obj, Selector.GetHandle (Selector.Class));
		}

		public static Type? Lookup (Class? @class)
		{
			if (@class is null)
				return null;

			return Lookup (@class.Handle, true)!;
		}

		internal static Type Lookup (IntPtr klass)
		{
			return Lookup (klass, true)!;
		}

		[BindingImpl (BindingImplOptions.Optimizable)] // To inline the Runtime.DynamicRegistrationSupported code if possible.
		internal static Type? Lookup (IntPtr klass, bool throw_on_error)
		{
			bool is_custom_type;
			var find_class = klass;
			do {
				var tp = FindType (find_class, out is_custom_type);
				if (tp is not null)
					return tp;
				if (Runtime.DynamicRegistrationSupported)
					break; // We can't continue looking up the hierarchy if we have the dynamic registrar, because we might be supposed to register this class.
				find_class = class_getSuperclass (find_class);
			} while (find_class != IntPtr.Zero);

			// The linker will remove this condition (and the subsequent method call) if possible
			if (Runtime.DynamicRegistrationSupported)
				return Runtime.Registrar.Lookup (klass, throw_on_error);

			if (throw_on_error)
				throw ErrorHelper.CreateError (8026, $"Can't lookup the Objective-C class 0x{klass.ToString ("x")} ({Marshal.PtrToStringAuto (class_getName (klass))}) when the dynamic registrar has been linked away.");

			return null;
		}

		internal static IntPtr Register (Type type)
		{
			return Runtime.Registrar.Register (type);
		}

		// Assembly.GetName ().Name is horrendously slow, so cache the results.
		static string GetAssemblyName (Assembly assembly)
		{
			if (assembly_to_name.TryGetValue (assembly, out var assemblyName))
				return assemblyName;

			assemblyName = assembly.GetName ().Name!;
			assembly_to_name.AddOrUpdate (assembly, assemblyName);
			return assemblyName;
		}

		// Find the given managed type in the tables generated by the static registrar.
		unsafe static IntPtr FindClass (Type type, out bool is_custom_type)
		{
			var map = Runtime.options->RegistrationMap;

			is_custom_type = false;

			if (map is null) {
				// Using only the dynamic registrar
				return IntPtr.Zero;
			}

			if (type.IsGenericType)
				type = type.GetGenericTypeDefinition ();

			// Look for the type in the type map.
			var asm_name = GetAssemblyName (type.Assembly);
			int mod_token;
			int type_token;

			if (Runtime.IsManagedStaticRegistrar) {
#if NET
				mod_token = unchecked((int) Runtime.INVALID_TOKEN_REF);
				type_token = unchecked((int) RegistrarHelper.LookupRegisteredTypeId (type));

#if LOG_TYPELOAD
				Runtime.NSLog ($"FindClass ({type.FullName}, {is_custom_type}): type token: 0x{type_token.ToString ("x")}");
#endif

				if (type_token == -1)
					return IntPtr.Zero;
#else
				throw ErrorHelper.CreateError (99, Xamarin.Bundler.Errors.MX0099 /* Internal error */, "The managed static registrar is only available for .NET");
#endif // NET
			} else {
				mod_token = type.Module.MetadataToken;
				type_token = type.MetadataToken & ~0x02000000 /* TokenType.TypeDef */;
			}

			for (int i = 0; i < map->map_count; i++) {
				var class_map = map->map [i];
				var token_reference = class_map.type_reference;
				if (!CompareTokenReference (asm_name, mod_token, type_token, token_reference))
					continue;

				var rv = class_map.handle;
				is_custom_type = (class_map.flags & Runtime.MTTypeFlags.CustomType) == Runtime.MTTypeFlags.CustomType;
#if LOG_TYPELOAD
				Runtime.NSLog ($"FindClass ({type.FullName}, {is_custom_type}): 0x{rv.ToString ("x")} = {Marshal.PtrToStringAuto (class_getName (rv))}.");
#endif
				return rv;
			}

			// The type we're looking for might be a type the registrar skipped, in which case we must
			// find it in the table of skipped types
			for (int i = 0; i < map->skipped_map_count; i++) {
				var skipped_map = map->skipped_map [i];
				var token_reference = skipped_map.skipped_reference;
				if (!CompareTokenReference (asm_name, mod_token, type_token, token_reference))
					continue;

				// This is a skipped type, we now got the actual type reference of the type we're looking for,
				// so go look for it in the type map.
				var actual_reference = skipped_map.actual_reference;
				for (int k = 0; k < map->map_count; k++) {
					var class_map = map->map [k];
					if (class_map.type_reference == actual_reference)
						return class_map.handle;
				}
			}

			return IntPtr.Zero;
		}

		unsafe static bool CompareTokenReference (string asm_name, int mod_token, int type_token, uint token_reference)
		{
			var map = Runtime.options->RegistrationMap;
			IntPtr assembly_name;

			if ((token_reference & 0x1) == 0x1) {
				// full token reference
				var idx = (int) (token_reference >> 1);
				var entry = map->full_token_references [idx];
				// first compare what's most likely to fail (the type's metadata token)
				var token = entry.token;
				type_token |= 0x02000000 /* TypeDef - the token type is explicit in the full token reference, but not present in the type_token argument, so we have to add it before comparing */;
				if (type_token != token)
					return false;

				// then the module token
				var module_token = entry.module_token;
				if (unchecked((uint) mod_token) != module_token)
					return false;

				// leave the assembly name for the end, since it's the most expensive comparison (string comparison)
				assembly_name = map->assemblies [entry.assembly_index].name;
			} else {
				// packed token reference
				if (token_reference >> 8 != type_token)
					return false;

				var assembly_index = (token_reference >> 1) & 0x7F;
				assembly_name = map->assemblies [(int) assembly_index].name;
			}

			return Runtime.StringEquals (assembly_name, asm_name);
		}

		internal static unsafe int FindMapIndex (Runtime.MTClassMap* array, int lo, int hi, IntPtr @class)
		{
			if (hi >= lo) {
				int mid = lo + (hi - lo) / 2;
				IntPtr handle = array [mid].handle;

				if (handle == @class)
					return mid;

				if (handle.ToInt64 () > @class.ToInt64 ())
					return FindMapIndex (array, lo, mid - 1, @class);

				return FindMapIndex (array, mid + 1, hi, @class);
			}

			return -1;
		}

		internal unsafe static Type? FindType (NativeHandle @class, out bool is_custom_type)
		{
			var map = Runtime.options->RegistrationMap;

#if LOG_TYPELOAD
				Runtime.NSLog ($"FindType (0x{@class:X} = {Marshal.PtrToStringAuto (class_getName (@class))})");
#endif

			is_custom_type = false;

			if (map is null) {
#if LOG_TYPELOAD
				Runtime.NSLog ($"FindType (0x{@class:X} = {Marshal.PtrToStringAuto (class_getName (@class))}) => found no map.");
#endif
				return null;
			}

			// Find the ObjC class pointer in our map
			var mapIndex = FindMapIndex (map->map, 0, map->map_count - 1, @class);
			if (mapIndex == -1) {
#if LOG_TYPELOAD
				Runtime.NSLog ($"FindType (0x{@class:X} = {Marshal.PtrToStringAuto (class_getName (@class))}) => found no type.");
#endif
				return null;
			}
#if LOG_TYPELOAD
			Runtime.NSLog ($"FindType (0x{@class:X} = {Marshal.PtrToStringAuto (class_getName (@class))}) => found index {mapIndex}.");
#endif

			is_custom_type = (map->map [mapIndex].flags & Runtime.MTTypeFlags.CustomType) == Runtime.MTTypeFlags.CustomType;

			var type = class_to_type [mapIndex];
			if (type is not null) {
#if LOG_TYPELOAD
				Runtime.NSLog ($"FindType (0x{@class:X} = {Marshal.PtrToStringAuto (class_getName (@class))}) => found type {type.FullName} for map index {mapIndex}.");
#endif
				return type;
			}

			// Resolve the map entry we found to a managed type
			var type_reference = map->map [mapIndex].type_reference;
			type = ResolveTypeTokenReference (type_reference);

#if LOG_TYPELOAD
			Runtime.NSLog ($"FindType (0x{@class:X} = {Marshal.PtrToStringAuto (class_getName (@class))}) => {type?.FullName}; is custom: {is_custom_type} (token reference: 0x{type_reference:X}).");
#endif

			class_to_type [mapIndex] = type;

			return type;
		}

		internal unsafe static MemberInfo? ResolveFullTokenReference (uint token_reference)
		{
			// sizeof (MTFullTokenReference) = IntPtr.Size + 4 + 4
			var idx = (int) (token_reference >> 1);
			var entry = Runtime.options->RegistrationMap->full_token_references [idx];
			var assembly_name = Runtime.options->RegistrationMap->assemblies [entry.assembly_index].name;
			var module_token = entry.module_token;
			var token = entry.token;

#if LOG_TYPELOAD
			Runtime.NSLog ($"ResolveFullTokenReference (0x{token_reference:X}) assembly name: {assembly_name} module token: 0x{module_token:X} token: 0x{token:X}.");
#endif

			var assembly = ResolveAssembly (assembly_name);
			var module = ResolveModule (assembly, module_token);
			return ResolveToken (assembly, module, token);
		}

		internal static Type? ResolveTypeTokenReference (uint token_reference)
		{
			var member = ResolveTokenReference (token_reference, 0x02000000 /* TypeDef */);
			if (member is null)
				return null;
			if (member is Type type)
				return type;

			throw ErrorHelper.CreateError (8022, $"Expected the token reference 0x{token_reference:X} to be a type, but it's a {member.GetType ().Name}. {Constants.PleaseFileBugReport}");
		}

		internal static MethodBase? ResolveMethodTokenReference (uint token_reference)
		{
			var member = ResolveTokenReference (token_reference, 0x06000000 /* Method */);
			if (member is null)
				return null;
			if (member is MethodBase method)
				return method;

			throw ErrorHelper.CreateError (8022, $"Expected the token reference 0x{token_reference:X} to be a method, but it's a {member.GetType ().Name}. {Constants.PleaseFileBugReport}");
		}

		unsafe static MemberInfo? ResolveTokenReference (uint token_reference, uint implicit_token_type)
		{
			var map = Runtime.options->RegistrationMap;

			// Stuff 2 (32-bits) uints in a 64-bit ulong, and use that as the key in the dictionary where we cache the lookup.
			var key = (((ulong) token_reference) << 32) + implicit_token_type;
			lock (token_to_member) {
				if (token_to_member.TryGetValue (key, out var member))
					return member;
			}

			if ((token_reference & 0x1) == 0x1) {
				var member = ResolveFullTokenReference (token_reference);
				lock (token_to_member)
					token_to_member [key] = member;
				return member;
			}

			var assembly_index = (token_reference >> 1) & 0x7F;
			uint token = (token_reference >> 8) + implicit_token_type;

#if LOG_TYPELOAD
			Runtime.NSLog ($"ResolveTokenReference (0x{token_reference:X}) assembly index: {assembly_index} token: 0x{token:X}.");
#endif

			var assembly_name = map->assemblies [(int) assembly_index].name;
			var assembly = ResolveAssembly (assembly_name);
			var module = ResolveModule (assembly, 0x1);

			var rv = ResolveToken (assembly, module, token | implicit_token_type);
			lock (token_to_member)
				token_to_member [key] = rv;
			return rv;
		}

		static MemberInfo? ResolveToken (Assembly assembly, Module? module, uint token)
		{
			// Finally resolve the token.
			var token_type = token & 0xFF000000;
			switch (token & 0xFF000000) {
			case 0x02000000: // TypeDef
				Type type;
#if NET
				if (Runtime.IsManagedStaticRegistrar) {
					type = RegistrarHelper.LookupRegisteredType (assembly, token & 0x00FFFFFF);
#if LOG_TYPELOAD
					Runtime.NSLog ($"ResolveToken (0x{token:X}) => Type: {type.FullName}");
#endif
					return type;
				}
#endif // NET
				if (module is null) {
					throw ErrorHelper.CreateError (8053, Errors.MX8053 /* Could not resolve the module in the assembly {0}. */, assembly.FullName);
				} else {
					type = module.ResolveType ((int) token);
				}
#if LOG_TYPELOAD
				Runtime.NSLog ($"ResolveToken (0x{token:X}) => Type: {type.FullName}");
#endif
				return type;
			case 0x06000000: // Method
				if (Runtime.IsManagedStaticRegistrar)
					throw ErrorHelper.CreateError (8054, Errors.MX8054 /* Can't resolve metadata tokens for methods when using the managed static registrar (token: 0x{0}). */, token.ToString ("x"));

				if (module is null)
					throw ErrorHelper.CreateError (8053, Errors.MX8053 /* Could not resolve the module in the assembly {0}. */, assembly.FullName);

				var method = module.ResolveMethod ((int) token);
#if LOG_TYPELOAD
				Runtime.NSLog ($"ResolveToken (0x{token:X}) => Method: {method?.DeclaringType?.FullName}.{method?.Name}");
#endif
				return method;
			default:
				throw ErrorHelper.CreateError (8021, $"Unknown implicit token type: 0x{token_type:X}.");
			}
		}

		static Module? ResolveModule (Assembly assembly, uint token)
		{
			if (token == Runtime.INVALID_TOKEN_REF)
				return null;

			foreach (var mod in assembly.GetModules ()) {
				if (mod.MetadataToken != token)
					continue;

#if LOG_TYPELOAD
				Runtime.NSLog ($"ResolveModule (\"{assembly.FullName}\", 0x{token:X}): {mod.Name}.");
#endif
				return mod;
			}

			throw ErrorHelper.CreateError (8020, $"Could not find the module with MetadataToken 0x{token:X} in the assembly {assembly}.");
		}

		// Restrict this code to desktop for now, which is where most of the problems with outdated generated static registrar code occur.
#if __MACOS__ || __MACCATALYST__
		static bool? verify_static_registrar_code;
		static object? verification_lock;
		static Dictionary<IntPtr, object?>? verified_assemblies; // Use Dictionary instead of HashSet to avoid pulling in System.Core.dll.
		unsafe static void VerifyStaticRegistrarCode (IntPtr assembly_name, Assembly assembly)
		{
			if (verify_static_registrar_code is null) {
				verify_static_registrar_code = !string.IsNullOrEmpty (Environment.GetEnvironmentVariable ("XAMARIN_VALIDATE_STATIC_REGISTRAR_CODE"));
				verification_lock = new object ();
			}
			if (verify_static_registrar_code != true)
				return;

			lock (verification_lock!) {
				if (verified_assemblies is null) {
					verified_assemblies = new Dictionary<IntPtr, object?> (Runtime.IntPtrEqualityComparer);
				} else if (verified_assemblies.ContainsKey (assembly_name)) {
					return;
				}
				verified_assemblies [assembly_name] = null;
			}

			var map = Runtime.options->RegistrationMap;
			if (map is null)
				return;

			for (var i = 0; i < map->assembly_count; i++) {
				var entry = map->assemblies [i];
				var name = Marshal.PtrToStringAuto (entry.name)!;
				if (!Runtime.StringEquals (assembly_name, name))
					continue;
				try {
					var mvid = Marshal.PtrToStringAuto (entry.mvid)!;
					var runtime_mvid = assembly.ManifestModule.ModuleVersionId;
					var registered_mvid = Guid.Parse (mvid);
					if (registered_mvid == runtime_mvid)
						continue;
					throw ErrorHelper.CreateError (8044, Errors.MX8044 /* The assembly {0} has been modified since the app was built, invalidating the generated static registrar code. The MVID for the loaded assembly is {1}, while the MVID for the assembly the generated static registrar code corresponds to is {2}. */, name, runtime_mvid, registered_mvid);
				} catch (Exception e) {
					throw ErrorHelper.CreateError (8043, e, Errors.MX8043 /* An exception occurred while validating the static registrar code for {0}: {1} */, name, e.Message);
				}
			}
		}
#endif // __MACOS__ || __MACCATALYST__

		static Assembly ResolveAssembly (IntPtr assembly_name)
		{
			if (TryResolveAssembly (assembly_name, out var asm)) {
#if __MACOS__ || __MACCATALYST__
				VerifyStaticRegistrarCode (assembly_name, asm);
#endif
				return asm;
			}

			throw ErrorHelper.CreateError (8019, $"Could not find the assembly {Marshal.PtrToStringAuto (assembly_name)} in the loaded assemblies.");
		}

		static bool TryResolveAssembly (IntPtr assembly_name, [NotNullWhen (true)] out Assembly? assembly)
		{
			// Find the assembly. We've already loaded all the assemblies that contain registered types, so just look at those assemblies.
			foreach (var asm in AppDomain.CurrentDomain.GetAssemblies ()) {
				if (!Runtime.StringEquals (assembly_name, GetAssemblyName (asm)))
					continue;

#if LOG_TYPELOAD
				Runtime.NSLog ($"TryResolveAssembly (0x{assembly_name:X}): {asm.FullName}.");
#endif
				assembly = asm;
				return true;
			}

			assembly = null;
			return false;
		}

		internal unsafe static uint GetTokenReference (Type type, bool throw_exception = true)
		{
			if (type.IsGenericType)
				type = type.GetGenericTypeDefinition ();

			var asm_name = GetAssemblyName (type.Module.Assembly);

			// First check if there's a full token reference to this type
			uint token;
			if (Runtime.IsManagedStaticRegistrar) {
#if NET
				var id = RegistrarHelper.LookupRegisteredTypeId (type);
				token = GetFullTokenReference (asm_name, unchecked((int) Runtime.INVALID_TOKEN_REF), 0x2000000 /* TokenType.TypeDef */ | unchecked((int) id));
#if LOG_TYPELOAD
				Runtime.NSLog ($"GetTokenReference ({type}, {throw_exception}) id: {id} token: 0x{token.ToString ("x")}");
#endif
#else
				throw ErrorHelper.CreateError (99, Xamarin.Bundler.Errors.MX0099 /* Internal error */, "The managed static registrar is only available for .NET");
#endif // NET
			} else {
				token = GetFullTokenReference (asm_name, type.Module.MetadataToken, type.MetadataToken);
			}
			if (token != uint.MaxValue)
				return token;

			// If type.Module.MetadataToken != 1, then the token must be a full token, which is not the case because we've already checked, so throw an exception.
			if (type.Module.MetadataToken != 1) {
				if (!throw_exception)
					return Runtime.INVALID_TOKEN_REF;
				throw ErrorHelper.CreateError (8025, $"Failed to compute the token reference for the type '{type.AssemblyQualifiedName}' because its module's metadata token is {type.Module.MetadataToken} when expected 1.");
			}

			var map = Runtime.options->RegistrationMap;

			// Find the assembly index in our list of registered assemblies.
			int assembly_index = -1;
			for (int i = 0; i < map->assembly_count; i++) {
				var name_ptr = map->assemblies [(int) i].name;
				if (Runtime.StringEquals (name_ptr, asm_name)) {
					assembly_index = i;
					break;
				}
			}
			// If the assembly isn't registered, then the token must be a full token (which it isn't, because we've already checked).
			if (assembly_index == -1) {
				if (!throw_exception)
					return Runtime.INVALID_TOKEN_REF;
				throw ErrorHelper.CreateError (8025, $"Failed to compute the token reference for the type '{type.AssemblyQualifiedName}' because the assembly couldn't be found in the list of registered assemblies.");
			}

			if (assembly_index > 127) {
				if (!throw_exception)
					return Runtime.INVALID_TOKEN_REF;
				throw ErrorHelper.CreateError (8025, $"Failed to compute the token reference for the type '{type.AssemblyQualifiedName}' because the assembly index {assembly_index} is not valid (must be <= 127).");
			}

			return (uint) ((type.MetadataToken << 8) + (assembly_index << 1));

		}

		// Look for the specified metadata token in the table of full token references.
		static unsafe uint GetFullTokenReference (string assembly_name, int module_token, int metadata_token)
		{
			var map = Runtime.options->RegistrationMap;
			for (int i = 0; i < map->full_token_reference_count; i++) {
				var ftr = map->full_token_references [i];
				var token = ftr.token;
				if (token != metadata_token)
					continue;
				var mod_token = ftr.module_token;
				if (unchecked((int) mod_token) != module_token)
					continue;
				var assembly_index = ftr.assembly_index;
				var assembly = map->assemblies [assembly_index];
				if (!Runtime.StringEquals (assembly.name, assembly_name))
					continue;

				return ((uint) i << 1) + 1;
			}

			return uint.MaxValue;
		}

		/*
		Type must have been previously registered.
		*/
		[BindingImpl (BindingImplOptions.Optimizable)] // To inline the Runtime.DynamicRegistrationSupported code if possible.
		internal static bool IsCustomType (Type type)
		{
			bool is_custom_type;
			var @class = GetClassHandle (type, false, out is_custom_type);
			if (@class != IntPtr.Zero)
				return is_custom_type;

			if (Runtime.DynamicRegistrationSupported)
				return Runtime.Registrar.IsCustomType (type);

			throw ErrorHelper.CreateError (8026, $"Can't determine if {type.FullName} is a custom type when the dynamic registrar has been linked away.");
		}

		[DllImport (Messaging.LIBOBJC_DYLIB)]
		static extern IntPtr objc_allocateClassPair (IntPtr superclass, IntPtr name, IntPtr extraBytes);

		internal static IntPtr objc_allocateClassPair (IntPtr superclass, string name, IntPtr extraBytes)
		{
			using var namePtr = new TransientString (name);
			return objc_allocateClassPair (superclass, namePtr, extraBytes);
		}

		[DllImport (Messaging.LIBOBJC_DYLIB)]
		static extern IntPtr objc_getClass (IntPtr name);

		internal static IntPtr objc_getClass (string name)
		{
			using var namePtr = new TransientString (name);
			return objc_getClass (namePtr);
		}

		[DllImport (Messaging.LIBOBJC_DYLIB)]
		internal static extern void objc_registerClassPair (IntPtr cls);

		[DllImport (Messaging.LIBOBJC_DYLIB)]
		[return: MarshalAs (UnmanagedType.U1)]
		static extern bool class_addIvar (IntPtr cls, IntPtr name, IntPtr size, byte alignment, IntPtr types);

		internal static bool class_addIvar (IntPtr cls, string name, IntPtr size, byte alignment, string types)
		{
			using var namePtr = new TransientString (name);
			using var typesPtr = new TransientString (types);
			return class_addIvar (cls, namePtr, size, alignment, typesPtr);
		}

		[DllImport (Messaging.LIBOBJC_DYLIB)]
		[return: MarshalAs (UnmanagedType.U1)]
		static extern bool class_addMethod (IntPtr cls, IntPtr name, IntPtr imp, IntPtr types);

		internal static bool class_addMethod (IntPtr cls, IntPtr name, IntPtr imp, string types)
		{
			using var typesPtr = new TransientString (types);
			return class_addMethod (cls, name, imp, typesPtr);
		}

		[DllImport (Messaging.LIBOBJC_DYLIB)]
		[return: MarshalAs (UnmanagedType.U1)]
		internal extern static bool class_addProtocol (IntPtr cls, IntPtr protocol);

		[DllImport (Messaging.LIBOBJC_DYLIB)]
		internal static extern IntPtr class_getName (IntPtr cls);

		[DllImport (Messaging.LIBOBJC_DYLIB)]
		internal static extern IntPtr class_getSuperclass (IntPtr cls);

		[DllImport (Messaging.LIBOBJC_DYLIB)]
		internal static extern IntPtr object_getClass (IntPtr obj);

		[DllImport (Messaging.LIBOBJC_DYLIB)]
		internal extern static IntPtr class_getMethodImplementation (IntPtr cls, IntPtr sel);

		[DllImport (Messaging.LIBOBJC_DYLIB)]
		internal extern static IntPtr class_getInstanceVariable (IntPtr cls, IntPtr name);

		internal static IntPtr class_getInstanceVariable (IntPtr cls, string name)
		{
			using var namePtr = new TransientString (name);
			return class_getInstanceVariable (cls, namePtr);
		}

		[DllImport (Messaging.LIBOBJC_DYLIB)]
		internal extern static IntPtr class_getInstanceMethod (IntPtr cls, IntPtr sel);

		[DllImport (Messaging.LIBOBJC_DYLIB, CharSet = CharSet.Ansi)]
		[return: MarshalAs (UnmanagedType.U1)]
		extern unsafe static bool class_addProperty (IntPtr cls, IntPtr name, IntPtr* attributes, int count);

		internal static bool class_addProperty (IntPtr cls, string name, objc_attribute_prop [] attributes, int count)
		{
			using var namePtr = new TransientString (name, TransientString.Encoding.Ansi);
			var ptrs = PropertyStringsToPtrs (attributes);
			bool retval = false;
			unsafe {
				fixed (IntPtr* ptrsPtr = ptrs) {
					retval = class_addProperty (cls, namePtr, ptrsPtr, count);
				}
			}
			FreeStringPtrs (ptrs);
			return retval;
		}

		internal static IntPtr [] PropertyStringsToPtrs (objc_attribute_prop [] props)
		{
			var ptrs = new IntPtr [props.Length * 2];
			var index = 0;
			foreach (var prop in props) {
				ptrs [index++] = Marshal.StringToHGlobalAnsi (prop.name);
				ptrs [index++] = Marshal.StringToHGlobalAnsi (prop.value);
			}
			return ptrs;
		}

		internal static void FreeStringPtrs (IntPtr [] ptrs)
		{
			foreach (var ptr in ptrs) {
				Marshal.FreeHGlobal (ptr);
			}
		}

		[StructLayout (LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		internal struct objc_attribute_prop {
			[MarshalAs (UnmanagedType.LPStr)] internal string name;
			[MarshalAs (UnmanagedType.LPStr)] internal string value;
		}
#endif // !COREBUILD
	}
}
