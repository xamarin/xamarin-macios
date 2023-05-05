using System;
using System.Collections.Generic;
using System.Linq;

using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Linker;
using Mono.Tuner;

using Xamarin.Bundler;
using Xamarin.Linker;

#nullable enable

namespace Xamarin.Linker {

	// This is a helper class when rewriting and adding code to assemblies with Cecil.
	// It knows how to find types and methods in other assemblies, and will create a 
	// (type/method) reference to them in the assembly currently being processed when
	// these types/methods are fetched.
	class AppBundleRewriter {
		LinkerConfiguration configuration;

		AssemblyDefinition? current_assembly;
		AssemblyDefinition? corlib_assembly;
		AssemblyDefinition? platform_assembly;

		public AssemblyDefinition CurrentAssembly {
			get {
				if (current_assembly is null)
					throw ErrorHelper.CreateError (99, "No current assembly!");
				return current_assembly;
			}
		}

		public AssemblyDefinition CorlibAssembly {
			get {
				if (corlib_assembly is null)
					throw ErrorHelper.CreateError (99, "No corlib assembly!");
				return corlib_assembly;
			}
		}

		public AssemblyDefinition PlatformAssembly {
			get {
				if (platform_assembly is null)
					throw ErrorHelper.CreateError (99, "No platform assembly!");
				return platform_assembly;
			}
		}

		Dictionary<AssemblyDefinition, Dictionary<string, (TypeDefinition, TypeReference)>> type_map = new ();
		Dictionary<string, (MethodDefinition, MethodReference)> method_map = new ();

		public AppBundleRewriter (LinkerConfiguration configuration)
		{
			this.configuration = configuration;

			// Find corlib and the platform assemblies
			foreach (var asm in configuration.Assemblies) {
				if (asm.Name.Name == Driver.CorlibName) {
					corlib_assembly = asm;
				} else if (asm.Name.Name == configuration.PlatformAssembly) {
					platform_assembly = asm;
				}
			}
		}

		TypeReference GetTypeReference (AssemblyDefinition assembly, string fullname, out TypeDefinition type)
		{
			if (!type_map.TryGetValue (assembly, out var map))
				type_map.Add (assembly, map = new Dictionary<string, (TypeDefinition, TypeReference)> ());

			if (!map.TryGetValue (fullname, out var tuple)) {
				var td = assembly.MainModule.Types.SingleOrDefault (v => v.FullName == fullname);
				if (td is null)
					throw ErrorHelper.CreateError (99, $"Unable to find the type '{fullname}' in {assembly.Name.Name}");
				if (!td.IsPublic) {
					td.IsPublic = true;
					SaveAssembly (td.Module.Assembly);
				}
				var tr = CurrentAssembly.MainModule.ImportReference (td);
				map [fullname] = tuple = new (td, tr);
			}

			type = tuple.Item1;
			return tuple.Item2;
		}

		public MethodReference GetMethodReference (AssemblyDefinition assembly, string fullname, string name, Func<MethodDefinition, bool>? predicate)
		{
			GetTypeReference (assembly, fullname, out var td);
			return GetMethodReference (assembly, td, name, fullname + "::" + name, predicate);
		}

		public MethodReference GetMethodReference (AssemblyDefinition assembly, TypeReference tr, string name)
		{
			return GetMethodReference (assembly, tr, name, tr.FullName + "::" + name, null);
		}

		public MethodReference GetMethodReference (AssemblyDefinition assembly, TypeReference tr, string name, Func<MethodDefinition, bool>? predicate)
		{
			return GetMethodReference (assembly, tr, name, tr.FullName + "::" + name, predicate);
		}

		public MethodReference GetMethodReference (AssemblyDefinition assembly, TypeReference tr, string name, string key, Func<MethodDefinition, bool>? predicate)
		{
			return GetMethodReference (assembly, tr, name, key, predicate, out var _);
		}

		public MethodReference GetMethodReference (AssemblyDefinition assembly, TypeReference tr, string name, string key, Func<MethodDefinition, bool>? predicate, out MethodDefinition method)
		{
			if (!method_map.TryGetValue (key, out var tuple)) {
				var td = tr.Resolve ();
				var md = td.Methods.SingleOrDefault (v => v.Name == name && (predicate is null || predicate (v)));
				if (md is null)
					throw new InvalidOperationException ($"Unable to find the method '{tr.FullName}::{name}' (for key '{key}') in {assembly.Name.Name}. Methods in type:\n\t{string.Join ("\n\t", td.Methods.Select (GetMethodSignature).OrderBy (v => v))}");

				tuple.Item1 = md;
				tuple.Item2 = CurrentAssembly.MainModule.ImportReference (md);
				method_map.Add (key, tuple);

				// Make the method public so that we can call it.
				if (!md.IsPublic) {
					md.IsPublic = true;
					SaveAssembly (md.Module.Assembly);
				}
			}

			method = tuple.Item1;
			return tuple.Item2;
		}

		static string GetMethodSignature (MethodDefinition method)
		{
			return $"{method?.ReturnType?.FullName ?? "(null)"} {method?.DeclaringType?.FullName ?? "(null)"}::{method?.Name ?? "(null)"} ({string.Join (", ", method?.Parameters?.Select (v => v?.ParameterType?.FullName + " " + v?.Name) ?? Array.Empty<string> ())})";
		}

		/* Types */

		public TypeReference System_Byte {
			get {
				return GetTypeReference (CorlibAssembly, "System.Byte", out var _);
			}
		}

		public TypeReference System_Exception {
			get {
				return GetTypeReference (CorlibAssembly, "System.Exception", out var _);
			}
		}
		public TypeReference System_Int32 {
			get {
				return GetTypeReference (CorlibAssembly, "System.Int32", out var _);
			}
		}

		public TypeReference System_IntPtr {
			get {
				return GetTypeReference (CorlibAssembly, "System.IntPtr", out var _);
			}
		}

		public TypeReference System_Nullable_1 {
			get {
				return GetTypeReference (CorlibAssembly, "System.Nullable`1", out var _);
			}
		}

		public TypeReference System_Object {
			get {
				return GetTypeReference (CorlibAssembly, "System.Object", out var _);
			}
		}

		public TypeReference System_RuntimeTypeHandle {
			get {
				return GetTypeReference (CorlibAssembly, "System.RuntimeTypeHandle", out var _);
			}
		}

		public TypeReference System_String {
			get {
				return GetTypeReference (CorlibAssembly, "System.String", out var _);
			}
		}

		public TypeReference System_Type {
			get {
				return GetTypeReference (CorlibAssembly, "System.Type", out var _);
			}
		}

		public TypeReference System_UInt16 {
			get {
				return GetTypeReference (CorlibAssembly, "System.UInt16", out var _);
			}
		}

		public TypeReference System_UInt32 {
			get {
				return GetTypeReference (CorlibAssembly, "System.UInt32", out var _);
			}
		}

		public TypeReference System_Void {
			get {
				return GetTypeReference (CorlibAssembly, "System.Void", out var _);
			}
		}

		public TypeReference System_Collections_Generic_Dictionary2 {
			get {
				return GetTypeReference (CorlibAssembly, "System.Collections.Generic.Dictionary`2", out var _);
			}
		}

		public TypeReference System_Diagnostics_CodeAnalysis_DynamicDependencyAttribute {
			get {
				return GetTypeReference (CorlibAssembly, "System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute", out var _);
			}
		}

		public TypeReference System_Reflection_MethodBase {
			get {
				return GetTypeReference (CorlibAssembly, "System.Reflection.MethodBase", out var _);
			}
		}

		public TypeReference System_Reflection_MethodInfo {
			get {
				return GetTypeReference (CorlibAssembly, "System.Reflection.MethodInfo", out var _);
			}
		}

		public TypeReference Foundation_NSArray {
			get {
				return GetTypeReference (PlatformAssembly, "Foundation.NSArray", out var _);
			}
		}

		public TypeReference Foundation_NSString {
			get {
				return GetTypeReference (PlatformAssembly, "Foundation.NSString", out var _);
			}
		}

		public TypeReference Foundation_NSObject {
			get {
				return GetTypeReference (PlatformAssembly, "Foundation.NSObject", out var _);
			}
		}

		public TypeReference ObjCRuntime_BindAs {
			get {
				return GetTypeReference (PlatformAssembly, "ObjCRuntime.BindAs", out var _);
			}
		}

		public TypeReference ObjCRuntime_BlockLiteral {
			get {
				return GetTypeReference (PlatformAssembly, "ObjCRuntime.BlockLiteral", out var _);
			}
		}

		public TypeReference ObjCRuntime_IManagedRegistrar {
			get {
				return GetTypeReference (PlatformAssembly, "ObjCRuntime.IManagedRegistrar", out var _);
			}
		}

		public TypeReference ObjCRuntime_NativeHandle {
			get {
				return GetTypeReference (PlatformAssembly, "ObjCRuntime.NativeHandle", out var _);
			}
		}

		public TypeReference ObjCRuntime_NativeObjectExtensions {
			get {
				return GetTypeReference (PlatformAssembly, "ObjCRuntime.NativeObjectExtensions", out var _);
			}
		}

		public TypeReference ObjCRuntime_RegistrarHelper {
			get {
				return GetTypeReference (PlatformAssembly, "ObjCRuntime.RegistrarHelper", out var _);
			}
		}

		public TypeReference ObjCRuntime_Runtime {
			get {
				return GetTypeReference (PlatformAssembly, "ObjCRuntime.Runtime", out var _);
			}
		}

		public TypeReference ObjCRuntime_RuntimeException {
			get {
				return GetTypeReference (PlatformAssembly, "ObjCRuntime.RuntimeException", out var _);
			}
		}

		/* Methods */

		public MethodReference System_Object__ctor {
			get {
				return GetMethodReference (CorlibAssembly, System_Object, ".ctor", (v) => v.IsDefaultConstructor ());
			}
		}

		public MethodReference Nullable_HasValue {
			get {
				return GetMethodReference (CorlibAssembly, System_Nullable_1, "get_HasValue", (v) =>
						!v.IsStatic
						&& !v.HasParameters
						&& !v.HasGenericParameters);
			}
		}

		public MethodReference Nullable_Value {
			get {
				return GetMethodReference (CorlibAssembly, System_Nullable_1, "get_Value", (v) =>
						!v.IsStatic
						&& !v.HasParameters
						&& !v.HasGenericParameters);
			}
		}

		public MethodReference Type_GetTypeFromHandle {
			get {
				return GetMethodReference (CorlibAssembly, System_Type, "GetTypeFromHandle", (v) =>
						v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 1
						&& v.Parameters [0].ParameterType.Is ("System", "RuntimeTypeHandle")
						&& !v.HasGenericParameters);
			}
		}

		public MethodReference Dictionary2_Add {
			get {
				return GetMethodReference (CorlibAssembly, System_Collections_Generic_Dictionary2, "Add", (v) =>
						!v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 2
						&& !v.HasGenericParameters);
			}
		}

		public MethodReference DynamicDependencyAttribute_ctor__String_Type {
			get {
				return GetMethodReference (CorlibAssembly, System_Diagnostics_CodeAnalysis_DynamicDependencyAttribute, ".ctor", (v) =>
						!v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 2
						&& v.Parameters [0].ParameterType.Is ("System", "String")
						&& v.Parameters [1].ParameterType.Is ("System", "Type")
						&& !v.HasGenericParameters);
			}
		}

		public MethodReference RuntimeTypeHandle_Equals {
			get {
				return GetMethodReference (CorlibAssembly, System_RuntimeTypeHandle, "Equals", (v) =>
						!v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 1
						&& v.Parameters [0].ParameterType.Is ("System", "RuntimeTypeHandle")
						&& !v.HasGenericParameters);
			}
		}
		public MethodReference MethodBase_Invoke {
			get {
				return GetMethodReference (CorlibAssembly, System_Reflection_MethodBase, "Invoke", (v) =>
						!v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 2
						&& v.Parameters [0].ParameterType.Is ("System", "Object")
						&& v.Parameters [1].ParameterType is ArrayType at
						&& at.ElementType.Is ("System", "Object")
						&& !v.HasGenericParameters);
			}
		}

		public MethodReference MethodBase_GetMethodFromHandle__RuntimeMethodHandle {
			get {
				return GetMethodReference (CorlibAssembly, System_Reflection_MethodBase, "GetMethodFromHandle", nameof (MethodBase_GetMethodFromHandle__RuntimeMethodHandle), (v) =>
						v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 1
						&& v.Parameters [0].ParameterType.Is ("System", "RuntimeMethodHandle")
						&& !v.HasGenericParameters);
			}
		}

		public MethodReference NSObject_AllocateNSObject {
			get {
				return GetMethodReference (PlatformAssembly, Foundation_NSObject, "AllocateNSObject", nameof (NSObject_AllocateNSObject), (v) =>
						v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 2
						&& v.Parameters [0].ParameterType.Is ("ObjCRuntime", "NativeHandle")
						&& v.Parameters [1].ParameterType.Is ("", "Flags") && v.Parameters [1].ParameterType.DeclaringType.Is ("Foundation", "NSObject")
						&& v.HasGenericParameters
						&& v.GenericParameters.Count == 1);
			}
		}

		public MethodReference BindAs_ConvertNSArrayToManagedArray {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_BindAs, "ConvertNSArrayToManagedArray", (v) =>
						v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 2
						&& v.Parameters [0].ParameterType.Is ("System", "IntPtr")
						// && v.Parameters [1].ParameterType.Is ("System", "IntPtr")
						&& v.HasGenericParameters
						&& v.GenericParameters.Count == 1);
			}
		}

		public MethodReference BindAs_ConvertNSArrayToManagedArray2 {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_BindAs, "ConvertNSArrayToManagedArray2", (v) =>
						v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 3
						&& v.Parameters [0].ParameterType.Is ("System", "IntPtr")
						// && v.Parameters [1].ParameterType.Is ("System", "IntPtr")
						&& v.HasGenericParameters
						&& v.GenericParameters.Count == 2);
			}
		}

		public MethodReference BindAs_ConvertManagedArrayToNSArray {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_BindAs, "ConvertManagedArrayToNSArray", (v) =>
						v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 2
						&& v.Parameters [0].ParameterType is ArrayType at
						// && v.Parameters [1].ParameterType.Is ("System", "IntPtr")
						&& v.HasGenericParameters
						&& v.GenericParameters.Count == 1);
			}
		}

		public MethodReference BindAs_ConvertManagedArrayToNSArray2 {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_BindAs, "ConvertManagedArrayToNSArray2", (v) =>
						v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 3
						&& v.Parameters [0].ParameterType is ArrayType at
						// && v.Parameters [1].ParameterType.Is ("System", "IntPtr")
						&& v.HasGenericParameters
						&& v.GenericParameters.Count == 2);
			}
		}

		public MethodReference BindAs_CreateNullable {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_BindAs, "CreateNullable", (v) =>
						v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 2
						// && v.Parameters [0].ParameterType.Is ("System", "IntPtr")
						// && v.Parameters [1].ParameterType.Is ("System", "IntPtr")
						&& v.HasGenericParameters
						&& v.GenericParameters.Count == 1);
			}
		}

		public MethodReference BindAs_CreateNullable2 {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_BindAs, "CreateNullable2", (v) =>
						v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 3
						// && v.Parameters [0].ParameterType.Is ("System", "IntPtr")
						// && v.Parameters [1].ParameterType.Is ("System", "IntPtr")
						&& v.HasGenericParameters
						&& v.GenericParameters.Count == 2);
			}
		}

		public MethodReference RegistrarHelper_NSArray_string_native_to_managed {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_RegistrarHelper, "NSArray_string_native_to_managed", (v) =>
						v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 3
						&& v.Parameters [0].ParameterType is PointerType pt && pt.ElementType.Is ("System", "IntPtr")
						&& v.Parameters [1].ParameterType is ByReferenceType brt1 && brt1.ElementType is ArrayType at1 && at1.ElementType.Is ("System", "String")
						&& v.Parameters [2].ParameterType is ByReferenceType brt2 && brt2.ElementType is ArrayType at2 && at2.ElementType.Is ("System", "String")
						&& !v.HasGenericParameters);
			}
		}

		public MethodReference RegistrarHelper_NSArray_string_managed_to_native {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_RegistrarHelper, "NSArray_string_managed_to_native", (v) =>
						v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 4
						&& v.Parameters [0].ParameterType is PointerType pt && pt.ElementType.Is ("System", "IntPtr")
						&& v.Parameters [1].ParameterType is ArrayType at1 && at1.ElementType.Is ("System", "String")
						&& v.Parameters [2].ParameterType is ArrayType at2 && at2.ElementType.Is ("System", "String")
						&& v.Parameters [3].ParameterType.Is ("System", "Boolean")
						&& !v.HasGenericParameters);
			}
		}

		public MethodReference RegistrarHelper_NSArray_native_to_managed {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_RegistrarHelper, "NSArray_native_to_managed", (v) =>
						v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 3
						&& v.Parameters [0].ParameterType is PointerType pt && pt.ElementType.Is ("System", "IntPtr")
						&& v.Parameters [1].ParameterType is ByReferenceType brt1 && brt1.ElementType is ArrayType at1 && at1.ElementType.Is ("", "T")
						&& v.Parameters [2].ParameterType is ByReferenceType brt2 && brt2.ElementType is ArrayType at2 && at2.ElementType.Is ("", "T")
						&& v.HasGenericParameters
						&& v.GenericParameters.Count == 1);
			}
		}

		public MethodReference RegistrarHelper_NSArray_managed_to_native {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_RegistrarHelper, "NSArray_managed_to_native", (v) =>
						v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 4
						&& v.Parameters [0].ParameterType is PointerType pt && pt.ElementType.Is ("System", "IntPtr")
						&& v.Parameters [1].ParameterType is ArrayType at1 && at1.ElementType.Is ("", "T")
						&& v.Parameters [2].ParameterType is ArrayType at2 && at2.ElementType.Is ("", "T")
						&& v.Parameters [3].ParameterType.Is ("System", "Boolean")
						&& v.HasGenericParameters
						&& v.GenericParameters.Count == 1);
			}
		}

		public MethodReference RegistrarHelper_NSObject_native_to_managed {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_RegistrarHelper, "NSObject_native_to_managed", (v) =>
						v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 3
						&& v.Parameters [0].ParameterType is PointerType pt && pt.ElementType.Is ("System", "IntPtr")
						&& v.Parameters [1].ParameterType is ByReferenceType brt1 && brt1.ElementType.Is ("", "T")
						&& v.Parameters [2].ParameterType is ByReferenceType brt2 && brt2.ElementType.Is ("", "T")
						&& v.HasGenericParameters
						&& v.GenericParameters.Count == 1);
			}
		}

		public MethodReference RegistrarHelper_NSObject_managed_to_native {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_RegistrarHelper, "NSObject_managed_to_native", (v) =>
						v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 4
						&& v.Parameters [0].ParameterType is PointerType pt && pt.ElementType.Is ("System", "IntPtr")
						&& v.Parameters [1].ParameterType.Is ("Foundation", "NSObject")
						&& v.Parameters [2].ParameterType.Is ("Foundation", "NSObject")
						&& v.Parameters [3].ParameterType.Is ("System", "Boolean")
						&& !v.HasGenericParameters);
			}
		}

		public MethodReference RegistrarHelper_string_native_to_managed {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_RegistrarHelper, "string_native_to_managed", (v) =>
						v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 3
						&& v.Parameters [0].ParameterType is PointerType pt && pt.ElementType.Is ("ObjCRuntime", "NativeHandle")
						&& v.Parameters [1].ParameterType is ByReferenceType brt1 && brt1.ElementType.Is ("System", "String")
						&& v.Parameters [2].ParameterType is ByReferenceType brt2 && brt2.ElementType.Is ("System", "String")
						&& !v.HasGenericParameters);
			}
		}

		public MethodReference RegistrarHelper_string_managed_to_native {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_RegistrarHelper, "string_managed_to_native", (v) =>
						v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 4
						&& v.Parameters [0].ParameterType is PointerType pt && pt.ElementType.Is ("ObjCRuntime", "NativeHandle")
						&& v.Parameters [1].ParameterType.Is ("System", "String")
						&& v.Parameters [2].ParameterType.Is ("System", "String")
						&& v.Parameters [3].ParameterType.Is ("System", "Boolean")
						&& !v.HasGenericParameters);
			}
		}

		public MethodReference RegistrarHelper_INativeObject_native_to_managed {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_RegistrarHelper, "INativeObject_native_to_managed", (v) =>
						v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 4
						&& v.Parameters [0].ParameterType is PointerType pt && pt.ElementType.Is ("System", "IntPtr")
						&& v.Parameters [1].ParameterType is ByReferenceType brt1 && brt1.ElementType.Is ("", "T")
						&& v.Parameters [2].ParameterType is ByReferenceType brt2 && brt2.ElementType.Is ("", "T")
						&& v.Parameters [3].ParameterType.Is ("System", "RuntimeTypeHandle")
						&& v.HasGenericParameters
						&& v.GenericParameters.Count == 1);
			}
		}

		public MethodReference RegistrarHelper_INativeObject_managed_to_native {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_RegistrarHelper, "INativeObject_managed_to_native", (v) =>
						v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 4
						&& v.Parameters [0].ParameterType is PointerType pt && pt.ElementType.Is ("System", "IntPtr")
						&& v.Parameters [1].ParameterType.Is ("ObjCRuntime", "INativeObject")
						&& v.Parameters [2].ParameterType.Is ("ObjCRuntime", "INativeObject")
						&& v.Parameters [3].ParameterType.Is ("System", "Boolean")
						&& !v.HasGenericParameters);
			}
		}

		public MethodReference RegistrarHelper_Register {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_RegistrarHelper, "Register", (v) =>
						v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 1
						&& v.Parameters [0].ParameterType.Is ("ObjCRuntime", "IManagedRegistrar")
						&& !v.HasGenericParameters);
			}
		}

		public MethodReference IManagedRegistrar_LookupUnmanagedFunction {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_IManagedRegistrar, "LookupUnmanagedFunction", (v) =>
						v.HasParameters
						&& v.Parameters.Count == 2
						&& v.Parameters [0].ParameterType.Is ("System", "String")
						&& v.Parameters [1].ParameterType.Is ("System", "Int32")
						&& !v.HasGenericParameters);
			}
		}

		public MethodReference IManagedRegistrar_LookupType {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_IManagedRegistrar, "LookupType", (v) =>
						v.HasParameters
						&& v.Parameters.Count == 1
						&& v.Parameters [0].ParameterType.Is ("System", "UInt32")
						&& !v.HasGenericParameters);
			}
		}

		public MethodReference IManagedRegistrar_LookupTypeId {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_IManagedRegistrar, "LookupTypeId", (v) =>
						v.HasParameters
						&& v.Parameters.Count == 1
						&& v.Parameters [0].ParameterType.Is ("System", "RuntimeTypeHandle")
						&& !v.HasGenericParameters);
			}
		}

		public MethodReference IManagedRegistrar_RegisterWrapperTypes {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_IManagedRegistrar, "RegisterWrapperTypes", (v) =>
						v.HasParameters
						&& v.Parameters.Count == 1
						// && v.Parameters [0].ParameterType is GenericInstanceType git && git.GetElementType ().Is ("System.Collections.Generic", "Dictionary`2")
						&& !v.HasGenericParameters);
			}
		}

		public MethodReference Runtime_AllocGCHandle {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_Runtime, "AllocGCHandle", nameof (Runtime_AllocGCHandle), (v) =>
						v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 1
						&& v.Parameters [0].ParameterType.Is ("System", "Object")
						&& !v.HasGenericParameters);
			}
		}

		public MethodReference Runtime_HasNSObject {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_Runtime, "HasNSObject", nameof (Runtime_HasNSObject), (v) =>
						v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 1
						&& v.Parameters [0].ParameterType.Is ("ObjCRuntime", "NativeHandle")
						&& !v.HasGenericParameters);
			}
		}

		public MethodReference Runtime_GetNSObject__System_IntPtr {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_Runtime, "GetNSObject", nameof (Runtime_GetNSObject__System_IntPtr), (v) =>
						v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 1
						&& v.Parameters [0].ParameterType.Is ("System", "IntPtr")
						&& !v.HasGenericParameters);
			}
		}

		public MethodReference Runtime_GetNSObject_T___System_IntPtr_System_IntPtr_System_RuntimeMethodHandle_bool {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_Runtime, "GetNSObject", nameof (Runtime_GetNSObject_T___System_IntPtr_System_IntPtr_System_RuntimeMethodHandle_bool), (v) =>
						v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 4
						&& v.Parameters [0].ParameterType.Is ("System", "IntPtr")
						&& v.Parameters [1].ParameterType.Is ("System", "IntPtr")
						&& v.Parameters [2].ParameterType.Is ("System", "RuntimeMethodHandle")
						&& v.Parameters [3].ParameterType.Is ("System", "Boolean")
						&& v.HasGenericParameters
						&& v.GenericParameters.Count == 1);
			}
		}

		public MethodReference Runtime_GetNSObject_T___System_IntPtr {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_Runtime, "GetNSObject", nameof (Runtime_GetNSObject_T___System_IntPtr), (v) =>
						v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 1
						&& v.Parameters [0].ParameterType.Is ("System", "IntPtr")
						&& v.HasGenericParameters
						&& v.GenericParameters.Count == 1);
			}
		}

		public MethodReference Runtime_GetINativeObject__IntPtr_Boolean_Type_Type {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_Runtime, "GetINativeObject", nameof (Runtime_GetINativeObject__IntPtr_Boolean_Type_Type), (v) =>
						v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 4
						&& v.Parameters [0].ParameterType.Is ("System", "IntPtr")
						&& v.Parameters [1].ParameterType.Is ("System", "Boolean")
						&& v.Parameters [2].ParameterType.Is ("System", "Type")
						&& v.Parameters [3].ParameterType.Is ("System", "Type")
						&& !v.HasGenericParameters);
			}
		}

		public MethodReference ProductException_ctor_Int32_bool_string {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_RuntimeException, ".ctor", nameof (ProductException_ctor_Int32_bool_string), (v) =>
						v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 3
						&& v.Parameters [0].ParameterType.Is ("System", "Int32")
						&& v.Parameters [0].ParameterType.Is ("System", "Boolean")
						&& v.Parameters [0].ParameterType.Is ("System", "String")
						&& !v.HasGenericParameters);
			}
		}

		public MethodReference BlockLiteral_CreateBlockForDelegate {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_BlockLiteral, "CreateBlockForDelegate", (v) =>
						v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 3
						&& v.Parameters [0].ParameterType.Is ("System", "Delegate")
						&& v.Parameters [1].ParameterType.Is ("System", "Delegate")
						&& v.Parameters [2].ParameterType.Is ("System", "String")
						&& !v.HasGenericParameters);
			}
		}

		public MethodReference RegistrarHelper_GetBlockForDelegate {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_RegistrarHelper, "GetBlockForDelegate", (v) =>
						v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 2
						&& v.Parameters [0].ParameterType.Is ("System", "Object")
						&& v.Parameters [1].ParameterType.Is ("System", "RuntimeMethodHandle")
						&& !v.HasGenericParameters);
			}
		}

		public MethodReference RegistrarHelper_GetBlockPointer {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_RegistrarHelper, "GetBlockPointer", (v) =>
						v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 1
						&& v.Parameters [0].ParameterType.Is ("ObjCRuntime", "BlockLiteral")
						&& !v.HasGenericParameters);
			}
		}

		public MethodReference BlockLiteral_Copy {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_BlockLiteral, "Copy", (v) =>
						v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 1
						&& v.Parameters [0].ParameterType.Is ("System", "IntPtr")
						&& !v.HasGenericParameters);
			}
		}

		public MethodReference Runtime_ReleaseBlockWhenDelegateIsCollected {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_Runtime, "ReleaseBlockWhenDelegateIsCollected", "ReleaseBlockWhenDelegateIsCollected", (v) =>
						v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 2
						&& v.Parameters [0].ParameterType.Is ("System", "IntPtr")
						&& v.Parameters [1].ParameterType.Is ("System", "Delegate")
						&& !v.HasGenericParameters);
			}
		}

		public MethodReference Runtime_GetBlockWrapperCreator {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_Runtime, "GetBlockWrapperCreator", nameof (Runtime_GetBlockWrapperCreator), (v) =>
						v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 2
						&& v.Parameters [0].ParameterType.Is ("System.Reflection", "MethodInfo")
						&& v.Parameters [1].ParameterType.Is ("System", "Int32")
						&& !v.HasGenericParameters);
			}
		}

		public MethodReference Runtime_CreateBlockProxy {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_Runtime, "CreateBlockProxy", nameof (Runtime_CreateBlockProxy), (v) =>
						v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 2
						&& v.Parameters [0].ParameterType.Is ("System.Reflection", "MethodInfo")
						&& v.Parameters [1].ParameterType.Is ("System", "IntPtr")
						&& !v.HasGenericParameters);
			}
		}

		public MethodReference Runtime_TraceCaller {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_Runtime, "TraceCaller", (v) =>
						v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 1
						&& v.Parameters [0].ParameterType.Is ("System", "String")
						&& !v.HasGenericParameters);
			}
		}

		public MethodReference Runtime_FindClosedMethod {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_Runtime, "FindClosedMethod", (v) =>
						v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 3
						&& v.Parameters [0].ParameterType.Is ("System", "Object")
						&& v.Parameters [1].ParameterType.Is ("System", "RuntimeTypeHandle")
						&& v.Parameters [2].ParameterType.Is ("System", "RuntimeMethodHandle")
						&& !v.HasGenericParameters);
			}
		}

		public MethodReference Runtime_FindClosedParameterType {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_Runtime, "FindClosedParameterType", (v) =>
						v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 4
						&& v.Parameters [0].ParameterType.Is ("System", "Object")
						&& v.Parameters [1].ParameterType.Is ("System", "RuntimeTypeHandle")
						&& v.Parameters [2].ParameterType.Is ("System", "RuntimeMethodHandle")
						&& v.Parameters [3].ParameterType.Is ("System", "Int32")
						&& !v.HasGenericParameters);
			}
		}
		public MethodReference CFString_FromHandle {
			get {
				return GetMethodReference (PlatformAssembly, "CoreFoundation.CFString", "FromHandle", (v) =>
						v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 1
						&& v.Parameters [0].ParameterType.Is ("ObjCRuntime", "NativeHandle")
						&& !v.HasGenericParameters);
			}
		}

		public MethodReference CFString_CreateNative {
			get {
				return GetMethodReference (PlatformAssembly, "CoreFoundation.CFString", "CreateNative", (v) =>
						v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 1
						&& v.Parameters [0].ParameterType.Is ("System", "String")
						&& !v.HasGenericParameters);
			}
		}

		public MethodReference CFArray_StringArrayFromHandle {
			get {
				return GetMethodReference (PlatformAssembly, "CoreFoundation.CFArray", "StringArrayFromHandle", (v) =>
						v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 1
						&& v.Parameters [0].ParameterType.Is ("ObjCRuntime", "NativeHandle")
						&& !v.HasGenericParameters);
			}
		}

		public MethodReference RegistrarHelper_CreateCFArray {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_RegistrarHelper, "CreateCFArray", nameof (RegistrarHelper_CreateCFArray), (v) =>
						v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 1
						&& v.Parameters [0].ParameterType is ArrayType at
						&& at.GetElementType ().Is ("System", "String")
						&& !v.HasGenericParameters);
			}
		}

		public MethodReference NSArray_ArrayFromHandle {
			get {
				return GetMethodReference (PlatformAssembly, Foundation_NSArray, "ArrayFromHandle", (v) =>
						v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 2
						&& v.Parameters [0].ParameterType.Is ("ObjCRuntime", "NativeHandle")
						&& v.Parameters [1].ParameterType.Is ("System", "Type")
						&& !v.HasGenericParameters);
			}
		}

		public MethodReference NSArray_ArrayFromHandle_1 {
			get {
				return GetMethodReference (PlatformAssembly, Foundation_NSArray, "ArrayFromHandle", "ArrayFromHandle`1", (v) =>
						v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 1
						&& v.Parameters [0].ParameterType.Is ("ObjCRuntime", "NativeHandle")
						&& v.HasGenericParameters
						&& v.GenericParameters.Count == 1);
			}
		}

		public MethodReference RegistrarHelper_ManagedArrayToNSArray {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_RegistrarHelper, "ManagedArrayToNSArray", (v) =>
						v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 2
						&& v.Parameters [0].ParameterType.Is ("System", "Object")
						&& v.Parameters [1].ParameterType.Is ("System", "Boolean")
						&& !v.HasGenericParameters);
			}
		}

		public MethodReference NativeObjectExtensions_GetHandle {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_NativeObjectExtensions, "GetHandle");
			}
		}

		public MethodReference NativeObject_op_Implicit_IntPtr {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_NativeHandle, "op_Implicit", (v) =>
						v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 1
						&& v.Parameters [0].ParameterType.Is ("ObjCRuntime", "NativeHandle")
						&& v.ReturnType.Is ("System", "IntPtr")
						&& !v.HasGenericParameters);
			}
		}

		public MethodReference Runtime_CopyAndAutorelease {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_Runtime, "CopyAndAutorelease", (v) =>
						v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 1
						&& v.Parameters [0].ParameterType.Is ("System", "IntPtr")
						&& !v.HasGenericParameters);
			}
		}

		public MethodReference Runtime_RetainNSObject {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_Runtime, "RetainNSObject", (v) =>
						v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 1
						&& v.Parameters [0].ParameterType.Is ("Foundation", "NSObject")
						&& !v.HasGenericParameters);
			}
		}

		public MethodReference Runtime_RetainNativeObject {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_Runtime, "RetainNativeObject", (v) =>
						v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 1
						&& v.Parameters [0].ParameterType.Is ("ObjCRuntime", "INativeObject")
						&& !v.HasGenericParameters);
			}
		}

		public MethodReference Runtime_RetainAndAutoreleaseNSObject {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_Runtime, "RetainAndAutoreleaseNSObject", (v) =>
						v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 1
						&& v.Parameters [0].ParameterType.Is ("Foundation", "NSObject")
						&& !v.HasGenericParameters);
			}
		}

		public MethodReference Runtime_RetainAndAutoreleaseNativeObject {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_Runtime, "RetainAndAutoreleaseNativeObject", (v) =>
						v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 1
						&& v.Parameters [0].ParameterType.Is ("ObjCRuntime", "INativeObject")
						&& !v.HasGenericParameters);
			}
		}

		public MethodReference UnmanagedCallersOnlyAttribute_Constructor {
			get {
				return GetMethodReference (CorlibAssembly, "System.Runtime.InteropServices.UnmanagedCallersOnlyAttribute", ".ctor", (v) => v.IsDefaultConstructor ());
			}
		}

		public MethodReference Unsafe_AsRef {
			get {
				return GetMethodReference (CorlibAssembly, "System.Runtime.CompilerServices.Unsafe", "AsRef", (v) =>
						v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 1
						&& v.Parameters [0].ParameterType.IsPointer
						&& v.Parameters [0].ParameterType.GetElementType ().Is ("System", "Void")
						&& v.HasGenericParameters);
			}
		}

		public void SetCurrentAssembly (AssemblyDefinition value)
		{
			current_assembly = value;
		}

		public void SaveCurrentAssembly ()
		{
			SaveAssembly (CurrentAssembly);
		}

		void SaveAssembly (AssemblyDefinition assembly)
		{
			if (assembly != CurrentAssembly && assembly != PlatformAssembly)
				throw new InvalidOperationException ($"Can't save assembly {assembly.Name} because it's not the current assembly ({CurrentAssembly.Name}) or the platform assembly ({PlatformAssembly.Name}).");
			var annotations = configuration.Context.Annotations;
			var action = annotations.GetAction (assembly);
			if (action == AssemblyAction.Copy)
				annotations.SetAction (assembly, AssemblyAction.Save);
		}

		public void ClearCurrentAssembly ()
		{
			current_assembly = null;
			type_map.Clear ();
			method_map.Clear ();
		}
	}
}
