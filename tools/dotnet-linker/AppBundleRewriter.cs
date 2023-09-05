using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
		Dictionary<string, (FieldDefinition, FieldReference)> field_map = new ();

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

		public MethodReference GetMethodReference (AssemblyDefinition assembly, TypeReference tr, string name, bool isStatic, params TypeReference [] parameterTypes)
		{
			return GetMethodReference (assembly, tr, name, name, isStatic, parameterTypes);
		}

		public MethodReference GetMethodReference (AssemblyDefinition assembly, TypeReference tr, string name, string key, bool isStatic, params TypeReference [] parameterTypes)
		{
			return GetMethodReference (assembly, tr, name, key, isStatic, 0, parameterTypes);
		}

		public MethodReference GetMethodReference (AssemblyDefinition assembly, TypeReference tr, string name, string key, bool isStatic, int genericParameterCount, params TypeReference [] parameterTypes)
		{
			return GetMethodReference (assembly, tr, name, key, (v) => {
				if (v.IsStatic != isStatic)
					return false;
				if (v.HasParameters != (parameterTypes.Length != 0))
					return false;
				if (v.Parameters.Count != parameterTypes.Length)
					return false;

				if (v.HasGenericParameters != (genericParameterCount != 0))
					return false;
				if (v.GenericParameters.Count != genericParameterCount)
					return false;

				for (var p = 0; p < parameterTypes.Length; p++) {
					var p1 = v.Parameters [p].ParameterType;
					var p2 = parameterTypes [p];
					if ((object) p1 == (object) p2)
						continue;
					if (p1.Name != p2.Name)
						return false;
					if (p1.Namespace != p2.Namespace)
						return false;
				}

				return true;
			});
		}

		static string GetMethodSignature (MethodDefinition method)
		{
			return $"{method?.ReturnType?.FullName ?? "(null)"} {method?.DeclaringType?.FullName ?? "(null)"}::{method?.Name ?? "(null)"} ({string.Join (", ", method?.Parameters?.Select (v => v?.ParameterType?.FullName + " " + v?.Name) ?? Array.Empty<string> ())})";
		}

		public FieldReference GetFieldReference (AssemblyDefinition assembly, TypeReference tr, string name, string key, out FieldDefinition field)
		{
			if (!field_map.TryGetValue (key, out var tuple)) {
				var td = tr.Resolve ();
				var fd = td.Fields.SingleOrDefault (v => v.Name == name);
				if (fd is null)
					throw new InvalidOperationException ($"Unable to find the field '{tr.FullName}::{name}' (for key '{key}') in {assembly.Name.Name}. Fields in type:\n\t{string.Join ("\n\t", td.Fields.Select (f => f.Name).OrderBy (v => v))}");

				tuple.Item1 = fd;
				tuple.Item2 = CurrentAssembly.MainModule.ImportReference (fd);
				field_map.Add (key, tuple);
			}

			field = tuple.Item1;
			return tuple.Item2;
		}

		/* Types */

		public TypeReference System_Boolean {
			get {
				return CurrentAssembly.MainModule.ImportReference (CorlibAssembly.MainModule.TypeSystem.Boolean);
			}
		}

		public TypeReference System_Byte {
			get {
				return CurrentAssembly.MainModule.ImportReference (CorlibAssembly.MainModule.TypeSystem.Byte);
			}
		}

		public TypeReference System_Delegate {
			get {
				return GetTypeReference (CorlibAssembly, "System.Delegate", out var _);
			}
		}

		public TypeReference System_Exception {
			get {
				return GetTypeReference (CorlibAssembly, "System.Exception", out var _);
			}
		}
		public TypeReference System_Int32 {
			get {
				return CurrentAssembly.MainModule.ImportReference (CorlibAssembly.MainModule.TypeSystem.Int32);
			}
		}

		public TypeReference System_IntPtr {
			get {
				return CurrentAssembly.MainModule.ImportReference (CorlibAssembly.MainModule.TypeSystem.IntPtr);
			}
		}

		public FieldReference System_IntPtr_Zero {
			get {
				return GetFieldReference (CorlibAssembly, System_IntPtr, "Zero", "System.IntPtr::Zero", out var _);
			}
		}

		public TypeReference System_Nullable_1 {
			get {
				return GetTypeReference (CorlibAssembly, "System.Nullable`1", out var _);
			}
		}

		public TypeReference System_Object {
			get {
				return CurrentAssembly.MainModule.ImportReference (CorlibAssembly.MainModule.TypeSystem.Object);
			}
		}

		public TypeReference System_RuntimeMethodHandle {
			get {
				return GetTypeReference (CorlibAssembly, "System.RuntimeMethodHandle", out var _);
			}
		}

		public TypeReference System_RuntimeTypeHandle {
			get {
				return GetTypeReference (CorlibAssembly, "System.RuntimeTypeHandle", out var _);
			}
		}

		public TypeReference System_String {
			get {
				return CurrentAssembly.MainModule.ImportReference (CorlibAssembly.MainModule.TypeSystem.String);
			}
		}

		public TypeReference System_Type {
			get {
				return GetTypeReference (CorlibAssembly, "System.Type", out var _);
			}
		}

		public TypeReference System_UInt16 {
			get {
				return CurrentAssembly.MainModule.ImportReference (CorlibAssembly.MainModule.TypeSystem.UInt16);
			}
		}

		public TypeReference System_UInt32 {
			get {
				return CurrentAssembly.MainModule.ImportReference (CorlibAssembly.MainModule.TypeSystem.UInt32);
			}
		}

		public TypeReference System_Void {
			get {
				return CurrentAssembly.MainModule.ImportReference (CorlibAssembly.MainModule.TypeSystem.Void);
			}
		}

		public TypeReference System_ValueType {
			get {
				return GetTypeReference (CorlibAssembly, "System.ValueType", out var _);
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

		public TypeReference System_Diagnostics_CodeAnalysis_DynamicallyAccessedMemberTypes {
			get {
				return GetTypeReference (CorlibAssembly, "System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes", out var _);
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

		public TypeReference CoreFoundation_CFArray {
			get {
				return GetTypeReference (PlatformAssembly, "CoreFoundation.CFArray", out var _);
			}
		}

		public TypeReference CoreFoundation_CFString {
			get {
				return GetTypeReference (PlatformAssembly, "CoreFoundation.CFString", out var _);
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

		public FieldReference Foundation_NSObject_HandleField {
			get {
				return GetFieldReference (PlatformAssembly, Foundation_NSObject, "handle", "Foundation.NSObject::handle", out var _);
			}
		}

#if NET
		public MethodReference Foundation_NSObject_FlagsSetterMethod {
			get {
				return GetMethodReference (PlatformAssembly, Foundation_NSObject, "set_flags", "Foundation.NSObject::set_flags", predicate: null, out var _);
			}
		}
#else
		public FieldReference Foundation_NSObject_FlagsField {
			get {
				return GetFieldReference (PlatformAssembly, Foundation_NSObject, "flags", "Foundation.NSObject::flags", out var _);
			}
		}
#endif

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

		public TypeReference Foundation_INSObjectFactory {
			get {
				return GetTypeReference (PlatformAssembly, "Foundation.INSObjectFactory", out var _);
			}
		}

		public TypeReference ObjCRuntime_INativeObject {
			get {
				return GetTypeReference (PlatformAssembly, "ObjCRuntime.INativeObject", out var _);
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
				return GetMethodReference (CorlibAssembly, System_Nullable_1, "get_HasValue", isStatic: false);
			}
		}

		public MethodReference Nullable_Value {
			get {
				return GetMethodReference (CorlibAssembly, System_Nullable_1, "get_Value", isStatic: false);
			}
		}

		public MethodReference Type_GetTypeFromHandle {
			get {
				return GetMethodReference (CorlibAssembly, System_Type, "GetTypeFromHandle", isStatic: true, System_RuntimeTypeHandle);
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
				return GetMethodReference (CorlibAssembly,
						System_Diagnostics_CodeAnalysis_DynamicDependencyAttribute,
						".ctor",
						".ctor(String,Type)",
						isStatic: false,
						System_String,
						System_Type);
			}
		}

		public MethodReference DynamicDependencyAttribute_ctor__DynamicallyAccessedMemberTypes_Type {
			get {
				return GetMethodReference (CorlibAssembly,
						System_Diagnostics_CodeAnalysis_DynamicDependencyAttribute,
						".ctor",
						".ctor(DynamicallyAccessedMemberTypes,Type)",
						isStatic: false,
						System_Diagnostics_CodeAnalysis_DynamicallyAccessedMemberTypes,
						System_Type);
			}
		}

		public MethodReference DynamicDependencyAttribute_ctor__String_String_String {
			get {
				return GetMethodReference (CorlibAssembly,
						System_Diagnostics_CodeAnalysis_DynamicDependencyAttribute,
						".ctor",
						".ctor(String,String,String)",
						isStatic: false,
						System_String,
						System_String,
						System_String);
			}
		}

		public MethodReference RuntimeTypeHandle_Equals {
			get {
				if (configuration.Application.XamarinRuntime == XamarinRuntime.MonoVM) {
					return RegistrarHelper_RuntimeTypeHandleEquals;
				}
				return GetMethodReference (CorlibAssembly, System_RuntimeTypeHandle, "Equals", isStatic: false, System_RuntimeTypeHandle);
			}
		}

		public MethodReference MethodBase_Invoke {
			get {
				return GetMethodReference (CorlibAssembly, System_Reflection_MethodBase, "Invoke", (v) =>
						!v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 2
						&& v.Parameters [0].ParameterType.Is ("System", "Object")
						&& v.Parameters [1].ParameterType is ArrayType at && at.ElementType.Is ("System", "Object")
						&& !v.HasGenericParameters);
			}
		}

		public MethodReference MethodBase_GetMethodFromHandle__RuntimeMethodHandle {
			get {
				return GetMethodReference (CorlibAssembly,
						System_Reflection_MethodBase, "GetMethodFromHandle",
						nameof (MethodBase_GetMethodFromHandle__RuntimeMethodHandle),
						isStatic: true,
						System_RuntimeMethodHandle);
			}
		}

		public MethodReference BindAs_ConvertNSArrayToManagedArray {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_BindAs, "ConvertNSArrayToManagedArray", (v) =>
						v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 2
						&& v.Parameters [0].ParameterType.Is ("System", "IntPtr")
						&& v.Parameters [1].ParameterType is FunctionPointerType fpt
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
						&& v.Parameters [1].ParameterType is FunctionPointerType fpt
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
						&& v.Parameters [1].ParameterType is FunctionPointerType fpt
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
						&& v.Parameters [1].ParameterType is FunctionPointerType fpt
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
						&& v.Parameters [0].ParameterType.Is ("System", "IntPtr")
						&& v.Parameters [1].ParameterType is FunctionPointerType fpt
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
						&& v.Parameters [0].ParameterType.Is ("System", "IntPtr")
						&& v.Parameters [1].ParameterType is FunctionPointerType fpt1
						&& v.Parameters [2].ParameterType is FunctionPointerType fpt2
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
				return GetMethodReference (PlatformAssembly,
						ObjCRuntime_RegistrarHelper, "Register",
						isStatic: true,
						ObjCRuntime_IManagedRegistrar);
			}
		}

		public MethodReference RegistrarHelper_RuntimeTypeHandleEquals {
			get {
				return GetMethodReference (PlatformAssembly,
						ObjCRuntime_RegistrarHelper,
						"RuntimeTypeHandleEquals",
						(v) => v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 2
						&& v.Parameters [0].ParameterType is ByReferenceType brt1 && brt1.ElementType.Is ("System", "RuntimeTypeHandle")
						&& v.Parameters [1].ParameterType.Is ("System", "RuntimeTypeHandle")
						&& !v.HasGenericParameters);
			}
		}

		public MethodReference IManagedRegistrar_LookupUnmanagedFunction {
			get {
				return GetMethodReference (PlatformAssembly,
						ObjCRuntime_IManagedRegistrar, "LookupUnmanagedFunction",
						isStatic: false,
						System_String,
						System_Int32);
			}
		}

		public MethodReference IManagedRegistrar_LookupType {
			get {
				return GetMethodReference (PlatformAssembly,
						ObjCRuntime_IManagedRegistrar, "LookupType",
						isStatic: false,
						System_UInt32);
			}
		}

		public MethodReference IManagedRegistrar_LookupTypeId {
			get {
				return GetMethodReference (PlatformAssembly,
						ObjCRuntime_IManagedRegistrar, "LookupTypeId",
						isStatic: false,
						System_RuntimeTypeHandle);
			}
		}

		public MethodReference IManagedRegistrar_ConstructNSObject {
			get {
				return GetMethodReference (PlatformAssembly,
						ObjCRuntime_IManagedRegistrar, "ConstructNSObject",
						isStatic: false,
						System_RuntimeTypeHandle,
						ObjCRuntime_NativeHandle);
			}
		}

		public MethodReference INSObjectFactory__Xamarin_ConstructNSObject {
			get {
				return GetMethodReference (PlatformAssembly,
						Foundation_INSObjectFactory, "_Xamarin_ConstructNSObject",
						nameof (INSObjectFactory__Xamarin_ConstructNSObject),
						isStatic: true,
						ObjCRuntime_NativeHandle);
			}
		}

		public MethodReference IManagedRegistrar_ConstructINativeObject {
			get {
				return GetMethodReference (PlatformAssembly,
						ObjCRuntime_IManagedRegistrar, "ConstructINativeObject",
						nameof (IManagedRegistrar_ConstructINativeObject),
						isStatic: false,
						System_RuntimeTypeHandle,
						ObjCRuntime_NativeHandle,
						System_Boolean);
			}
		}

		public MethodReference INativeObject__Xamarin_ConstructINativeObject {
			get {
				return GetMethodReference (PlatformAssembly,
						ObjCRuntime_INativeObject, "_Xamarin_ConstructINativeObject",
						nameof (INativeObject__Xamarin_ConstructINativeObject),
						isStatic: true,
						ObjCRuntime_NativeHandle,
						System_Boolean);
			}
		}

		public MethodReference IManagedRegistrar_RegisterWrapperTypes {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_IManagedRegistrar, "RegisterWrapperTypes", (v) =>
						v.HasParameters
						&& v.Parameters.Count == 1
						&& v.Parameters [0].ParameterType is GenericInstanceType git && git.ElementType.Is ("System.Collections.Generic", "Dictionary`2")
						&& !v.HasGenericParameters);
			}
		}

		public MethodReference Runtime_AllocGCHandle {
			get {
				return GetMethodReference (PlatformAssembly,
						ObjCRuntime_Runtime, "AllocGCHandle",
						nameof (Runtime_AllocGCHandle),
						isStatic: true,
						System_Object);
			}
		}

		public MethodReference Runtime_HasNSObject {
			get {
				return GetMethodReference (PlatformAssembly,
						ObjCRuntime_Runtime, "HasNSObject",
						nameof (Runtime_HasNSObject),
						isStatic: true,
						System_IntPtr);
			}
		}

		public MethodReference Runtime_TryGetNSObject {
			get {
				return GetMethodReference (PlatformAssembly,
						ObjCRuntime_Runtime, "TryGetNSObject",
						nameof (Runtime_TryGetNSObject),
						isStatic: true,
						System_IntPtr,
						System_Boolean);
			}
		}
		public MethodReference Runtime_GetNSObject__System_IntPtr {
			get {
				return GetMethodReference (PlatformAssembly,
						ObjCRuntime_Runtime, "GetNSObject",
						nameof (Runtime_GetNSObject__System_IntPtr),
						isStatic: true,
						System_IntPtr);
			}
		}

		public MethodReference Runtime_GetNSObject_T___System_IntPtr_System_IntPtr_System_RuntimeMethodHandle_bool {
			get {
				return GetMethodReference (PlatformAssembly,
						ObjCRuntime_Runtime, "GetNSObject",
						nameof (Runtime_GetNSObject_T___System_IntPtr_System_IntPtr_System_RuntimeMethodHandle_bool),
						isStatic: true,
						genericParameterCount: 1,
						System_IntPtr,
						System_IntPtr,
						System_RuntimeMethodHandle,
						System_Boolean);
			}
		}

		public MethodReference Runtime_GetNSObject_T___System_IntPtr {
			get {
				return GetMethodReference (PlatformAssembly,
						ObjCRuntime_Runtime, "GetNSObject",
						nameof (Runtime_GetNSObject_T___System_IntPtr),
						isStatic: true,
						genericParameterCount: 1,
						System_IntPtr);
			}
		}

		public MethodReference Runtime_CreateRuntimeException {
			get {
				return GetMethodReference (PlatformAssembly,
						ObjCRuntime_Runtime, "CreateRuntimeException",
						nameof (Runtime_CreateRuntimeException),
						isStatic: true,
						System_Int32,
						System_String);
			}
		}

		public MethodReference BlockLiteral_CreateBlockForDelegate {
			get {
				return GetMethodReference (PlatformAssembly,
						ObjCRuntime_BlockLiteral,
						"CreateBlockForDelegate",
						isStatic: true,
						System_Delegate,
						System_Delegate,
						System_String);
			}
		}

		public MethodReference RegistrarHelper_GetBlockForDelegate {
			get {
				return GetMethodReference (PlatformAssembly,
						ObjCRuntime_RegistrarHelper, "GetBlockForDelegate",
						isStatic: true,
						System_Object,
						System_RuntimeMethodHandle);
			}
		}

		public MethodReference RegistrarHelper_GetBlockPointer {
			get {
				return GetMethodReference (PlatformAssembly,
						ObjCRuntime_RegistrarHelper, "GetBlockPointer",
						isStatic: true,
						ObjCRuntime_BlockLiteral);
			}
		}

		public MethodReference BlockLiteral_Copy {
			get {
				return GetMethodReference (PlatformAssembly,
						ObjCRuntime_BlockLiteral, "Copy",
						isStatic: true,
						System_IntPtr);
			}
		}

		public MethodReference Runtime_ReleaseBlockWhenDelegateIsCollected {
			get {
				return GetMethodReference (PlatformAssembly,
						ObjCRuntime_Runtime, "ReleaseBlockWhenDelegateIsCollected",
						nameof (Runtime_ReleaseBlockWhenDelegateIsCollected),
						isStatic: true,
						System_IntPtr,
						System_Delegate);
			}
		}

		public MethodReference Runtime_GetBlockWrapperCreator {
			get {
				return GetMethodReference (PlatformAssembly,
						ObjCRuntime_Runtime, "GetBlockWrapperCreator",
						nameof (Runtime_GetBlockWrapperCreator),
						isStatic: true,
						System_Reflection_MethodInfo,
						System_Int32);
			}
		}

		public MethodReference Runtime_CreateBlockProxy {
			get {
				return GetMethodReference (PlatformAssembly,
						ObjCRuntime_Runtime, "CreateBlockProxy",
						nameof (Runtime_CreateBlockProxy),
						isStatic: true,
						System_Reflection_MethodInfo,
						System_IntPtr);
			}
		}

		public MethodReference Runtime_TraceCaller {
			get {
				return GetMethodReference (PlatformAssembly,
						ObjCRuntime_Runtime, "TraceCaller",
						isStatic: true,
						System_String);
			}
		}

		public MethodReference Runtime_FindClosedMethod {
			get {
				return GetMethodReference (PlatformAssembly,
						ObjCRuntime_Runtime, "FindClosedMethod",
						isStatic: true,
						System_Object,
						System_RuntimeTypeHandle,
						System_RuntimeMethodHandle);
			}
		}

		public MethodReference Runtime_FindClosedParameterType {
			get {
				return GetMethodReference (PlatformAssembly,
						ObjCRuntime_Runtime, "FindClosedParameterType",
						isStatic: true,
						System_Object,
						System_RuntimeTypeHandle,
						System_RuntimeMethodHandle,
						System_Int32);
			}
		}
		public MethodReference CFString_FromHandle {
			get {
				return GetMethodReference (PlatformAssembly,
						CoreFoundation_CFString, "FromHandle",
						nameof (CFString_FromHandle),
						isStatic: true,
						ObjCRuntime_NativeHandle);
			}
		}

		public MethodReference CFString_CreateNative {
			get {
				return GetMethodReference (PlatformAssembly,
						CoreFoundation_CFString, "CreateNative",
						nameof (CFString_CreateNative),
						isStatic: true,
						System_String);
			}
		}

		public MethodReference CFArray_StringArrayFromHandle {
			get {
				return GetMethodReference (PlatformAssembly,
						CoreFoundation_CFArray, "StringArrayFromHandle",
						nameof (CFArray_StringArrayFromHandle),
						isStatic: true,
						ObjCRuntime_NativeHandle);
			}
		}

		public MethodReference RegistrarHelper_CreateCFArray {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_RegistrarHelper, "CreateCFArray", nameof (RegistrarHelper_CreateCFArray), (v) =>
						v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 1
						&& v.Parameters [0].ParameterType is ArrayType at && at.GetElementType ().Is ("System", "String")
						&& !v.HasGenericParameters);
			}
		}

		public MethodReference NSArray_ArrayFromHandle {
			get {
				return GetMethodReference (PlatformAssembly,
						Foundation_NSArray, "ArrayFromHandle",
						nameof (NSArray_ArrayFromHandle),
						isStatic: true,
						ObjCRuntime_NativeHandle,
						System_Type);
			}
		}

		public MethodReference NSArray_ArrayFromHandle_1 {
			get {
				return GetMethodReference (PlatformAssembly,
						Foundation_NSArray, "ArrayFromHandle",
						nameof (NSArray_ArrayFromHandle_1),
						isStatic: true,
						genericParameterCount: 1,
						ObjCRuntime_NativeHandle);
			}
		}

		public MethodReference RegistrarHelper_ManagedArrayToNSArray {
			get {
				return GetMethodReference (PlatformAssembly,
						ObjCRuntime_RegistrarHelper, "ManagedArrayToNSArray",
						nameof (RegistrarHelper_ManagedArrayToNSArray),
						isStatic: true,
						System_Object,
						System_Boolean);
			}
		}

		public MethodReference NativeObjectExtensions_GetHandle {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_NativeObjectExtensions, "GetHandle");
			}
		}

		public MethodReference NativeObject_op_Implicit_IntPtr {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_NativeHandle, "op_Implicit", nameof (NativeObject_op_Implicit_IntPtr), (v) =>
						v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 1
						&& v.Parameters [0].ParameterType.Is ("ObjCRuntime", "NativeHandle")
						&& v.ReturnType.Is ("System", "IntPtr")
						&& !v.HasGenericParameters);
			}
		}

		public MethodReference NativeObject_op_Implicit_NativeHandle {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_NativeHandle, "op_Implicit", nameof (NativeObject_op_Implicit_NativeHandle), (v) =>
						v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 1
						&& v.Parameters [0].ParameterType.Is ("System", "IntPtr")
						&& v.ReturnType.Is ("ObjCRuntime", "NativeHandle")
						&& !v.HasGenericParameters);
			}
		}

		public MethodReference Runtime_CopyAndAutorelease {
			get {
				return GetMethodReference (PlatformAssembly,
						ObjCRuntime_Runtime, "CopyAndAutorelease",
						isStatic: true,
						System_IntPtr);
			}
		}

		public MethodReference Runtime_RetainNSObject {
			get {
				return GetMethodReference (PlatformAssembly,
						ObjCRuntime_Runtime, "RetainNSObject",
						isStatic: true,
						Foundation_NSObject);
			}
		}

		public MethodReference Runtime_RetainNativeObject {
			get {
				return GetMethodReference (PlatformAssembly,
						ObjCRuntime_Runtime, "RetainNativeObject",
						isStatic: true,
						ObjCRuntime_INativeObject);
			}
		}

		public MethodReference Runtime_RetainAndAutoreleaseNSObject {
			get {
				return GetMethodReference (PlatformAssembly,
						ObjCRuntime_Runtime, "RetainAndAutoreleaseNSObject",
						isStatic: true,
						Foundation_NSObject);
			}
		}

		public MethodReference Runtime_RetainAndAutoreleaseNativeObject {
			get {
				return GetMethodReference (PlatformAssembly,
						ObjCRuntime_Runtime, "RetainAndAutoreleaseNativeObject",
						isStatic: true,
						ObjCRuntime_INativeObject);
			}
		}

		public MethodReference Runtime_TryReleaseINativeObject {
			get {
				return GetMethodReference (PlatformAssembly,
						ObjCRuntime_Runtime, "TryReleaseINativeObject",
						isStatic: true,
						ObjCRuntime_INativeObject);
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
						&& v.Parameters [0].ParameterType.IsPointer && v.Parameters [0].ParameterType.GetElementType ().Is ("System", "Void")
						&& v.HasGenericParameters);
			}
		}

#if NET
		public bool TryGet_NSObject_RegisterToggleRef(out MethodDefinition? md) {
			// the NSObject.RegisterToggleRef method isn't present on all platforms (for example on Mac)
			try {
				_ = GetMethodReference (PlatformAssembly, Foundation_NSObject, "RegisterToggleRef", "Foundation.NSObject::RegisterToggleRef", predicate: null, out md);
				return true;
			} catch (InvalidOperationException) {
				md = null;
				return false;
			}
		}
#endif

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
			if (action == AssemblyAction.Copy) {
				// Preserve TypeForwardedTo which would the linker sweep otherwise
				// Note that the linker will sweep type forwarders even if the assembly isn't trimmed:
				// https://github.com/dotnet/runtime/blob/9dd59af3aee2f403e63887afef50d98022a2e575/src/tools/illink/src/linker/Linker.Steps/SweepStep.cs#L191-L200
				if (assembly.MainModule.HasExportedTypes) {
					foreach (var type in assembly.MainModule.ExportedTypes) {
						annotations.Mark (type);
					}
				}
				annotations.SetAction (assembly, AssemblyAction.Save);
			}
		}

		public void ClearCurrentAssembly ()
		{
			current_assembly = null;
			type_map.Clear ();
			method_map.Clear ();
			field_map.Clear ();
		}

		public CustomAttribute CreateDynamicDependencyAttribute (string memberSignature, TypeDefinition type)
		{
			if (type.HasGenericParameters) {
				var typeName = Xamarin.Utils.DocumentationComments.GetSignature (type);
				var assemblyName = type.Module.Assembly.Name.Name;
				return CreateDynamicDependencyAttribute (memberSignature, typeName, assemblyName);
			}

			var attribute = new CustomAttribute (DynamicDependencyAttribute_ctor__String_Type);
			attribute.ConstructorArguments.Add (new CustomAttributeArgument (System_String, memberSignature));
			attribute.ConstructorArguments.Add (new CustomAttributeArgument (System_Type, type));
			return attribute;
		}

		public CustomAttribute CreateDynamicDependencyAttribute (string memberSignature, string typeName, string assemblyName)
		{
			var attribute = new CustomAttribute (DynamicDependencyAttribute_ctor__String_String_String);
			attribute.ConstructorArguments.Add (new CustomAttributeArgument (System_String, memberSignature));
			attribute.ConstructorArguments.Add (new CustomAttributeArgument (System_String, typeName));
			attribute.ConstructorArguments.Add (new CustomAttributeArgument (System_String, assemblyName));
			return attribute;
		}

		public CustomAttribute CreateDynamicDependencyAttribute (DynamicallyAccessedMemberTypes memberTypes, TypeDefinition type)
		{
			var attribute = new CustomAttribute (DynamicDependencyAttribute_ctor__DynamicallyAccessedMemberTypes_Type);
			// typed as 'int' because that's how the linker expects it: https://github.com/dotnet/runtime/blob/3c5ad6c677b4a3d12bc6a776d654558cca2c36a9/src/tools/illink/src/linker/Linker/DynamicDependency.cs#L97
			attribute.ConstructorArguments.Add (new CustomAttributeArgument (System_Diagnostics_CodeAnalysis_DynamicallyAccessedMemberTypes, (int) memberTypes));
			attribute.ConstructorArguments.Add (new CustomAttributeArgument (System_Type, type));
			return attribute;
		}
	}
}
