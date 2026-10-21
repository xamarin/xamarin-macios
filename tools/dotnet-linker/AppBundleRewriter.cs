using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Linker;
using Mono.Tuner;

using Xamarin.Bundler;
using Xamarin.Linker;
using Xamarin.Utils;

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

		// Invoked whenever an assembly is modified (i.e. whenever SaveAssembly is called).
		public Action<AssemblyDefinition>? AssemblySaved { get; set; }

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

		AssemblyDefinition? system_console_assembly;
		public AssemblyDefinition SystemConsoleAssembly {
			get {
				if (system_console_assembly is null) {
					system_console_assembly = configuration.Assemblies.SingleOrDefault (v => v.Name.Name == "System.Console")!;
					if (system_console_assembly is null) {
						system_console_assembly = CorlibAssembly.MainModule.AssemblyResolver.Resolve (new AssemblyNameReference ("System.Console", CorlibAssembly.MainModule.Assembly.Name.Version));
						if (system_console_assembly is null)
							throw ErrorHelper.CreateError (99, "Unable to find System.Console assembly");
					}
				}
				return system_console_assembly;
			}
		}

		Dictionary<AssemblyDefinition, Dictionary<string, (TypeDefinition, TypeReference)>> type_map = new ();
		Dictionary<string, (MethodDefinition, MethodReference)> method_map = new ();
		Dictionary<string, (FieldDefinition, FieldReference)> field_map = new ();
		Dictionary<string, TypeDefinition> created_types = new ();

		public AppBundleRewriter (LinkerConfiguration configuration)
		{
			this.configuration = configuration;

			// Find corlib and the platform assemblies
			foreach (var asm in configuration.Assemblies) {
				if (asm.Name.Name == Driver.CorlibName) {
					if (corlib_assembly is not null)
						throw new InvalidOperationException ($"Already have a corlib assembly named {corlib_assembly.Name}");
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
				if (!md.IsPublic && md.DeclaringType.Module.Assembly.FullName != CorlibAssembly.FullName) {
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

		public TypeReference System_Attribute {
			get {
				return GetTypeReference (CorlibAssembly, "System.Attribute", out var _);
			}
		}

		public TypeReference System_Boolean {
			get {
				return CurrentAssembly.MainModule.ImportReference (CorlibAssembly.MainModule.TypeSystem.Boolean);
			}
		}

		public TypeReference System_Reflection_AssemblyMetadataAttribute {
			get {
				return GetTypeReference (CorlibAssembly, "System.Reflection.AssemblyMetadataAttribute", out var _);
			}
		}

		public MethodReference AssemblyMetadataAttribute_Constructor_String_String {
			get {
				return GetMethodReference (CorlibAssembly, System_Reflection_AssemblyMetadataAttribute, ".ctor", (v) =>
						v.IsConstructor
						&& v.HasParameters
						&& v.Parameters.Count == 2
						&& v.Parameters [0].ParameterType.Is ("System", "String")
						&& v.Parameters [1].ParameterType.Is ("System", "String"));
			}
		}

		public TypeReference System_Byte {
			get {
				return CurrentAssembly.MainModule.ImportReference (CorlibAssembly.MainModule.TypeSystem.Byte);
			}
		}

		public TypeReference System_Console {
			get {
				return GetTypeReference (SystemConsoleAssembly, "System.Console", out var _);
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

		public TypeReference System_GC {
			get {
				return GetTypeReference (CorlibAssembly, "System.GC", out var _);
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

		public MethodReference Foundation_NSObject_GetOrCreateNSObjectForConstructor {
			get {
				return GetMethodReference (PlatformAssembly, Foundation_NSObject, "GetOrCreateNSObjectForConstructor", "Foundation.NSObject::GetOrCreateNSObjectForConstructor", true, 1, System_IntPtr, System_UInt32);
			}
		}

		public TypeReference Foundation_ProtocolAttribute {
			get {
				return GetTypeReference (PlatformAssembly, "Foundation.ProtocolAttribute", out var _);
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

		public TypeReference ObjCRuntime_Class {
			get {
				return GetTypeReference (PlatformAssembly, "ObjCRuntime.Class", out var _);
			}
		}

		public TypeReference ObjCRuntime_Dlfcn {
			get {
				return GetTypeReference (PlatformAssembly, "ObjCRuntime.Dlfcn", out var _);
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

		public TypeReference ObjCRuntime_INativeObjectProxyAttribute {
			get {
				return GetTypeReference (PlatformAssembly, "ObjCRuntime.INativeObjectProxyAttribute", out var _);
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

		public TypeReference ObjCRuntime_NSObjectProxyAttribute {
			get {
				return GetTypeReference (PlatformAssembly, "ObjCRuntime.NSObjectProxyAttribute", out var _);
			}
		}

		public TypeReference ObjCRuntime_ObjectiveCFrameworkAttribute {
			get {
				return GetTypeReference (PlatformAssembly, "ObjCRuntime.ObjectiveCFrameworkAttribute", out var _);
			}
		}

		public TypeReference ObjCRuntime_ProtocolProxyAttribute {
			get {
				return GetTypeReference (PlatformAssembly, "ObjCRuntime.ProtocolProxyAttribute", out var _);
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

		public TypeReference ObjCRuntime_SkippedObjectiveCTypeUniverse {
			get {
				return GetTypeReference (PlatformAssembly, "ObjCRuntime.SkippedObjectiveCTypeUniverse", out var _);
			}
		}

		/* Methods */

		public MethodReference System_Attribute__ctor {
			get {
				return GetMethodReference (CorlibAssembly, System_Attribute, ".ctor", (v) => v.IsDefaultConstructor ());
			}
		}

		public MethodReference System_Console__WriteLine_String_Object {
			get {
				return GetMethodReference (SystemConsoleAssembly, System_Console, "WriteLine", (v) =>
					v.IsStatic
					&& v.HasParameters
					&& v.Parameters.Count == 2
					&& v.Parameters [0].ParameterType.Is ("System", "String")
					&& v.Parameters [1].ParameterType.Is ("System", "Object")
					&& !v.HasGenericParameters);
			}
		}

		public MethodReference System_GC__KeepAlive {
			get {
				return GetMethodReference (CorlibAssembly, System_GC, "KeepAlive", (v) =>
					v.IsStatic
					&& v.HasParameters
					&& v.Parameters.Count == 1
					&& v.Parameters [0].ParameterType.Is ("System", "Object")
					&& !v.HasGenericParameters);
			}
		}

		public MethodReference System_Object__ctor {
			get {
				return GetMethodReference (CorlibAssembly, System_Object, ".ctor", (v) => v.IsDefaultConstructor ());
			}
		}

		public MethodReference System_String__op_Equality_String_String {
			get {
				return GetMethodReference (CorlibAssembly, System_String, "op_Equality", (v) =>
					v.IsStatic
					&& v.HasParameters
					&& v.Parameters.Count == 2
					&& v.Parameters [0].ParameterType.Is ("System", "String")
					&& v.Parameters [1].ParameterType.Is ("System", "String")
					&& v.ReturnType.Is ("System", "Boolean")
					&& !v.HasGenericParameters);
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

		public MethodReference Nullable_ctor {
			get {
				return GetMethodReference (CorlibAssembly, System_Nullable_1, ".ctor", isStatic: false, System_Nullable_1.GenericParameters [0]);
			}
		}

		public MethodReference Type_GetTypeFromHandle {
			get {
				return GetMethodReference (CorlibAssembly, System_Type, "GetTypeFromHandle", isStatic: true, System_RuntimeTypeHandle);
			}
		}

		public TypeReference System_Runtime_CompilerServices_RuntimeHelpers {
			get {
				return GetTypeReference (CorlibAssembly, "System.Runtime.CompilerServices.RuntimeHelpers", out var _);
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

		public MethodReference DynamicDependencyAttribute_ctor__String {
			get {
				return GetMethodReference (CorlibAssembly,
						System_Diagnostics_CodeAnalysis_DynamicDependencyAttribute,
						".ctor",
						".ctor(String)",
						isStatic: false,
						System_String);
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

		public MethodReference Class_GetHandle__System_String {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_Class, "GetHandle", (v) =>
						v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 1
						&& v.Parameters [0].ParameterType.Is ("System", "String")
						&& !v.HasGenericParameters);
			}
		}

		public MethodReference ObjectiveCFrameworkAttribute_ctor_String {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_ObjectiveCFrameworkAttribute, ".ctor", (v) =>
						v.IsConstructor
						&& v.HasParameters
						&& v.Parameters.Count == 1
						&& v.Parameters [0].ParameterType.Is ("System", "String")
						&& !v.HasGenericParameters);
			}
		}

		public MethodReference ObjCRuntime_INativeObjectProxyAttribute__ctor {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_INativeObjectProxyAttribute, ".ctor", (v) => v.IsDefaultConstructor ());
			}
		}

		public MethodReference ObjCRuntime_NSObjectProxy__ctor {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_NSObjectProxyAttribute, ".ctor", (v) => v.IsDefaultConstructor ());
			}
		}

		public MethodReference ObjCRuntime_ProtocolProxy__ctor {
			get {
				return GetMethodReference (PlatformAssembly, ObjCRuntime_ProtocolProxyAttribute, ".ctor", (v) => v.IsDefaultConstructor ());
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

		public MethodReference Runtime_InvokeGenericRegistrarTrampoline {
			get {
				return GetMethodReference (PlatformAssembly,
						ObjCRuntime_Runtime, "InvokeGenericRegistrarTrampoline",
						nameof (Runtime_InvokeGenericRegistrarTrampoline),
						isStatic: true,
						System_Object,
						System_RuntimeTypeHandle,
						System_RuntimeMethodHandle,
						new ArrayType (System_Object),
						new ByReferenceType (System_IntPtr));
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

		public MethodReference TypeMapAttribute_1_Constructor_String_Type {
			get {
				GetTypeReference (CorlibAssembly, "System.Runtime.InteropServices.TypeMapAttribute`1", out var td);
				return GetMethodReference (CorlibAssembly, td, ".ctor",
						"System.Runtime.InteropServices.TypeMapAttribute`1::.ctor(string,Type)",
						(v) =>
						!v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 2
						&& v.Parameters [0].ParameterType.Is ("System", "String")
						&& v.Parameters [1].ParameterType.Is ("System", "Type"));
			}
		}

		public MethodReference TypeMapAttribute_1_Constructor_String_Type_Type {
			get {
				GetTypeReference (CorlibAssembly, "System.Runtime.InteropServices.TypeMapAttribute`1", out var td);
				return GetMethodReference (CorlibAssembly, td, ".ctor",
						"System.Runtime.InteropServices.TypeMapAttribute`1::.ctor(string,Type,Type)",
						(v) =>
						!v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 3
						&& v.Parameters [0].ParameterType.Is ("System", "String")
						&& v.Parameters [1].ParameterType.Is ("System", "Type")
						&& v.Parameters [2].ParameterType.Is ("System", "Type"));
			}
		}

		public MethodReference TypeMapAssemblyTargetAttribute_1_Constructor_String_Type_Type {
			get {
				return GetMethodReference (CorlibAssembly, "System.Runtime.InteropServices.TypeMapAssemblyTargetAttribute`1", ".ctor", (v) =>
						!v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 1
						&& v.Parameters [0].ParameterType.Is ("System", "String"));
			}
		}
		public MethodReference TypeMapAssociationAttribute_1_Constructor_Type_Type {
			get {
				return GetMethodReference (CorlibAssembly, "System.Runtime.InteropServices.TypeMapAssociationAttribute`1", ".ctor", (v) =>
						!v.IsStatic
						&& v.HasParameters
						&& v.Parameters.Count == 2
						&& v.Parameters [0].ParameterType.Is ("System", "Type")
						&& v.Parameters [1].ParameterType.Is ("System", "Type"));
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

		public bool TryGet_NSObject_RegisterToggleRef ([NotNullWhen (true)] out MethodDefinition? md)
		{
			// the NSObject.RegisterToggleRef method isn't present on all platforms (for example on Mac)
			try {
				_ = GetMethodReference (PlatformAssembly, Foundation_NSObject, "RegisterToggleRef", "Foundation.NSObject::RegisterToggleRef", predicate: null, out md);
				return true;
			} catch (InvalidOperationException) {
				md = null;
				return false;
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
			if (configuration.HotReloadModifiedAssemblySafetyNet && configuration.HotReloadCompatibleBuild && action == AssemblyAction.Copy && assembly != PlatformAssembly) {
				configuration.Logger.LogError (ErrorHelper.CreateError (99, $"The assembly '{assembly.Name.Name}' is reloadable, but was modified during a Hot Reload compatible build."));
				return;
			}
			AssemblySaved?.Invoke (assembly);
			if (action == AssemblyAction.Copy) {
#if !ASSEMBLY_PREPARER
				// Preserve TypeForwardedTo which would the linker sweep otherwise
				// Note that the linker will sweep type forwarders even if the assembly isn't trimmed:
				// https://github.com/dotnet/runtime/blob/9dd59af3aee2f403e63887afef50d98022a2e575/src/tools/illink/src/linker/Linker.Steps/SweepStep.cs#L191-L200
				if (assembly.MainModule.HasExportedTypes) {
					foreach (var type in assembly.MainModule.ExportedTypes) {
						annotations.Mark (type);
					}
				}
#endif // !ASSEMBLY_PREPARER
				annotations.SetAction (assembly, AssemblyAction.Save);
			}
		}

		public void ClearCurrentAssembly ()
		{
			current_assembly = null;
			type_map.Clear ();
			method_map.Clear ();
			field_map.Clear ();
			created_types.Clear ();
		}

		public CustomAttribute CreateAttribute (MethodReference constructor)
		{
#if !ASSEMBLY_PREPARER
			// For some reason the trimmer doesn't mark attribute constructors
			// This is probably only needed when running as a custom linker step.
			configuration.Context.Annotations.Mark (constructor.Resolve ());
#endif // !ASSEMBLY_PREPARER
			return new CustomAttribute (constructor);
		}

		// We only need to add dependency attributes if the target dependency is in a trimmed assembly,
		// otherwise the target dependency won't be trimmed away.
		bool IsAssemblyTrimmed (IMemberDefinition member)
		{
			var assembly = member is TypeDefinition td ? td.Module.Assembly : member.DeclaringType.Module.Assembly;
			var action = configuration.Context.Annotations.GetAction (assembly);
			return action == AssemblyAction.Link;
		}

		public bool AddDynamicDependencyAttribute (MethodDefinition addToMethod, MethodDefinition dependsOn)
		{
			if (!IsAssemblyTrimmed (dependsOn))
				return false;

			var signature = DocumentationComments.GetSignature (dependsOn);
			if (addToMethod.DeclaringType == dependsOn.DeclaringType) {
				var attribute = CreateDynamicDependencyAttribute (signature);
				return AddAttributeOnlyOnce (addToMethod, attribute);
			} else if (addToMethod.DeclaringType.Module == dependsOn.DeclaringType.Module) {
				var attribute = CreateDynamicDependencyAttribute (signature, dependsOn.DeclaringType);
				return AddAttributeOnlyOnce (addToMethod, attribute);
			} else {
				var attribute = CreateDynamicDependencyAttribute (signature, dependsOn.DeclaringType, dependsOn.DeclaringType.Module.Assembly);
				return AddAttributeOnlyOnce (addToMethod, attribute);
			}
		}

		public CustomAttribute CreateDynamicDependencyAttribute (string memberSignature, TypeDefinition type)
		{
			if (type.HasGenericParameters)
				return CreateDynamicDependencyAttribute (memberSignature, type, type.Module.Assembly);

			var attribute = CreateAttribute (DynamicDependencyAttribute_ctor__String_Type);
			attribute.ConstructorArguments.Add (new CustomAttributeArgument (System_String, memberSignature));
			// Import the type into the current assembly, otherwise Cecil will serialize the Type argument
			// without an assembly-qualified name when 'type' is a TypeDefinition from another assembly (because
			// a TypeDefinition's Scope is its own module), and the trimmer won't be able to resolve it (IL2036).
			attribute.ConstructorArguments.Add (new CustomAttributeArgument (System_Type, CurrentAssembly.MainModule.ImportReference (type)));
			return attribute;
		}

		public CustomAttribute CreateDynamicDependencyAttribute (string memberSignature)
		{
			var attribute = CreateAttribute (DynamicDependencyAttribute_ctor__String);
			attribute.ConstructorArguments.Add (new CustomAttributeArgument (System_String, memberSignature));
			return attribute;
		}

		public CustomAttribute CreateDynamicDependencyAttribute (string memberSignature, TypeDefinition type, AssemblyDefinition assembly)
		{
			return CreateDynamicDependencyAttribute (memberSignature, DocumentationComments.GetSignature (type), assembly.Name.Name);
		}

		public CustomAttribute CreateDynamicDependencyAttribute (string memberSignature, string typeName, AssemblyDefinition assembly)
		{
			return CreateDynamicDependencyAttribute (memberSignature, typeName, assembly.Name.Name);
		}

		public CustomAttribute CreateDynamicDependencyAttribute (string memberSignature, string typeName, string assemblyName)
		{
			var attribute = CreateAttribute (DynamicDependencyAttribute_ctor__String_String_String);
			attribute.ConstructorArguments.Add (new CustomAttributeArgument (System_String, memberSignature));
			attribute.ConstructorArguments.Add (new CustomAttributeArgument (System_String, typeName));
			attribute.ConstructorArguments.Add (new CustomAttributeArgument (System_String, assemblyName));
			return attribute;
		}

		public CustomAttribute CreateDynamicDependencyAttribute (DynamicallyAccessedMemberTypes memberTypes, TypeDefinition type)
		{
			var attribute = CreateAttribute (DynamicDependencyAttribute_ctor__DynamicallyAccessedMemberTypes_Type);
			// typed as 'int' because that's how the linker expects it: https://github.com/dotnet/runtime/blob/3c5ad6c677b4a3d12bc6a776d654558cca2c36a9/src/tools/illink/src/linker/Linker/DynamicDependency.cs#L97
			attribute.ConstructorArguments.Add (new CustomAttributeArgument (System_Diagnostics_CodeAnalysis_DynamicallyAccessedMemberTypes, (int) memberTypes));
			// Import the type into the current assembly, otherwise Cecil will serialize the Type argument
			// without an assembly-qualified name when 'type' is a TypeDefinition from another assembly (because
			// a TypeDefinition's Scope is its own module), and the trimmer won't be able to resolve it (IL2036).
			attribute.ConstructorArguments.Add (new CustomAttributeArgument (System_Type, CurrentAssembly.MainModule.ImportReference (type)));
			return attribute;
		}

		/// <summary>
		/// Preserve all the members declared on a type (and the type itself), which is what
		/// <c>[Preserve (AllMembers = true)]</c> means.
		/// </summary>
		/// <remarks>
		///   <para>
		///     A DynamicDependency attribute with DynamicallyAccessedMemberTypes can't be used here: the trimmer
		///     resolves those member types over the entire type hierarchy, so it would also preserve every member
		///     of every base type (for an NSObject subclass that means all of NSObject, which is quite big).
		///     Instead add one DynamicDependency attribute per member declared on the type itself, and a single
		///     DynamicDependency attribute for the interfaces the type implements.
		///   </para>
		///   <para>
		///     Methods are preserved by name (without the parameter list), which preserves every overload -
		///     which is what we want here anyway (and it avoids a trimmer crash, see
		///     <see cref="DocumentationComments.GetNameSignature (MethodDefinition)" />).
		///   </para>
		/// </remarks>
		/// <param name="addToMethod">The method on which to add the dynamic dependency attributes.</param>
		/// <param name="type">The type whose members should be preserved.</param>
		public bool AddPreserveAllMembersDynamicDependencyAttributes (MethodDefinition addToMethod, TypeDefinition type)
		{
			var signatures = new HashSet<string> (StringComparer.Ordinal);

			foreach (var field in type.Fields) {
				if (!field.HasCustomAttribute ("System.Runtime.CompilerServices", "CompilerGeneratedAttribute"))
					signatures.Add (DocumentationComments.GetSignature (field));
			}
			foreach (var method in type.Methods) {
				if (!method.HasCustomAttribute ("System.Runtime.CompilerServices", "CompilerGeneratedAttribute"))
					signatures.Add (DocumentationComments.GetNameSignature (method));
			}
			foreach (var property in type.Properties)
				signatures.Add (DocumentationComments.GetSignature (property));
			foreach (var @event in type.Events)
				signatures.Add (DocumentationComments.GetSignature (@event));

			if (signatures.Count == 0) {
				// The type has no declared members, so add a placeholder member and preserve that,
				// which will keep the type itself (same pattern as AddDynamicDependencyAttributeToStaticConstructor).
				var placeholderName = "__linker_preserve__";
				FieldDefinition? placeholderMember = null;
				if (type.HasFields)
					placeholderMember = type.Fields.FirstOrDefault (f => f.Name == placeholderName && f.IsStatic);
				if (placeholderMember is null) {
					placeholderMember = new FieldDefinition (placeholderName, FieldAttributes.Private | FieldAttributes.Static, System_Int32);
					type.Fields.Add (placeholderMember);
				}
				signatures.Add (DocumentationComments.GetSignature (placeholderMember));
			}

			var modified = false;
			foreach (var signature in signatures.OrderBy (v => v, StringComparer.Ordinal))
				modified |= AddAttributeOnlyOnce (addToMethod, CreateDynamicDependencyAttribute (signature, type));

			// The interfaces a type implements must be preserved as well: the static registrar needs the
			// protocol interfaces (and the custom attributes on them) to find the block proxy types it
			// references from the generated native code.
			// Only do this if there are any interfaces to preserve, otherwise the trimmer complains that
			// no members were resolved for the attribute (IL2037).
			if (HasAnyInterfaces (type))
				modified |= AddAttributeOnlyOnce (addToMethod, CreateDynamicDependencyAttribute (DynamicallyAccessedMemberTypes.Interfaces, type));

			return modified;
		}

		/// <summary>
		/// Returns true if the type, or any of its base types, implements any interfaces.
		/// </summary>
		static bool HasAnyInterfaces (TypeDefinition? type)
		{
			while (type is not null) {
				if (type.HasInterfaces)
					return true;
				type = type.BaseType?.Resolve ();
			}
			return false;
		}

		/// <summary>
		/// Preserve a method conditionally on another type
		/// </summary>
		/// <param name="onType">The type on which to add the dynamic dependency attribute.</param>
		/// <param name="forMethod">The method that is the target of the dynamic dependency.</param>
		public bool AddDynamicDependencyAttributeToStaticConstructor (TypeDefinition onType, MethodDefinition forMethod)
		{
			CustomAttribute attrib;
			var signature = DocumentationComments.GetSignature (forMethod);

			if (onType == forMethod.DeclaringType) {
				attrib = CreateDynamicDependencyAttribute (signature);
			} else if (onType.Module == forMethod.DeclaringType.Module) {
				attrib = CreateDynamicDependencyAttribute (signature, forMethod.DeclaringType);
			} else {
				attrib = CreateDynamicDependencyAttribute (signature, forMethod.DeclaringType, forMethod.Module.Assembly);
			}

			return AddAttributeToStaticConstructor (onType, attrib);
		}

		/// <summary>
		/// Preserve a field conditionally on another type
		/// </summary>
		/// <param name="onType">The type on which to add the dynamic dependency attribute.</param>
		/// <param name="forField">The field that is the target of the dynamic dependency.</param>
		/// <returns>Whether an attribute was added or not.</returns>
		public bool AddDynamicDependencyAttributeToStaticConstructor (TypeDefinition onType, FieldDefinition forField)
		{
			var attrib = CreateDynamicDependencyAttribute (DocumentationComments.GetSignature (forField), forField.DeclaringType, forField.Module.Assembly);
			return AddAttributeToStaticConstructor (onType, attrib);
		}

		/// <summary>
		/// Preserve a type conditionally on another type (if that other type is marked)
		/// </summary>
		/// <remarks>
		///   <para>
		///     Unfortunately a DynamicDependency attribute can't point to a type, only a member within a type.
		///     So we add a placeholder member within the target type, and point the DynamicDependency attribute to that member.
		///   </para>
		///   <para>The caller is responsible for making sure the current assenbly is saved if there were any changes.</para>
		/// </remarks>
		/// <param name="onType">The type on which to add the dynamic dependency attribute.</param>
		/// <param name="forType">The type that is the target of the dynamic dependency.</param>
		/// <returns>Whether an attribute was added or not.</returns>
		public bool AddDynamicDependencyAttributeToStaticConstructor (TypeDefinition onType, TypeDefinition forType)
		{
			if (!IsAssemblyTrimmed (forType))
				return false;

			var placeholderName = "__linker_preserve__";
			FieldDefinition? placeholderMember = null;
			if (forType.HasFields)
				placeholderMember = forType.Fields.FirstOrDefault (f => f.Name == placeholderName && f.IsStatic);
			if (placeholderMember is null) {
				placeholderMember = new FieldDefinition (placeholderName, FieldAttributes.Private | FieldAttributes.Static, System_Int32);
				forType.Fields.Add (placeholderMember);
			}
			return AddDynamicDependencyAttributeToStaticConstructor (onType, placeholderMember);
		}

		public bool AddAttributeToStaticConstructor (TypeDefinition onType, CustomAttribute attribute)
		{
			var cctor = GetOrCreateStaticConstructor (onType, out var modified);
			modified |= AddAttributeOnlyOnce (cctor, attribute);
			return modified;
		}

		public MethodDefinition GetOrCreateStaticConstructor (TypeDefinition type, out bool modified)
		{
			modified = false;

			var staticCtor = type.GetTypeConstructor ();
			if (staticCtor is null) {
				staticCtor = type.AddMethod (".cctor", MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.RTSpecialName | MethodAttributes.SpecialName | MethodAttributes.Static, System_Void);
				staticCtor.CreateBody (out var il);
				il.Emit (OpCodes.Ret);
				modified = true;
			}

			// Remove the BeforeFieldInit attribute from the type, otherwise the linker may trim away the static constructor, and taking our attributes with it.
			if (type.Attributes.HasFlag (TypeAttributes.BeforeFieldInit)) {
				type.Attributes &= ~TypeAttributes.BeforeFieldInit;
				modified = true;
			}

			if (!staticCtor.Body.Instructions.Any (v => v.OpCode != OpCodes.Ret && v.OpCode != OpCodes.Nop)) {
				// FIXME: improve workaround.
				var body = staticCtor.Body;
				body.Instructions.Insert (0, Instruction.Create (OpCodes.Call, this.System_GC__KeepAlive));
				body.Instructions.Insert (0, Instruction.Create (OpCodes.Ldnull));
			}

			return staticCtor;
		}

		/// <summary>
		/// Add the given attribute to the provider, but only if an attribute with the same constructor and the same arguments isn't already present.
		/// This is needed because we may add the same dynamic dependency attribute multiple times (for example if multiple methods call the same method that needs to be preserved),
		/// and we don't want to end up with multiple copies of the same attribute, which would cause warnings in the linker.
		/// </summary>
		/// <param name="provider">The provider to which the attribute should be added.</param>
		/// <param name="attribute">The attribute to add.</param>
		/// <returns>Whether the attribute was added or not.</returns>
		public bool AddAttributeOnlyOnce (ICustomAttributeProvider provider, CustomAttribute attribute)
		{
			if (provider.HasCustomAttributes) {
				foreach (var ca in provider.CustomAttributes) {
					if (ca.Constructor == attribute.Constructor) {
						// ok so far
					} else if (ca.Constructor.DeclaringType.FullName != attribute.Constructor.DeclaringType.FullName) {
						continue;
					} else if (ca.Constructor.FullName != attribute.Constructor.FullName) {
						continue;
					}

					if (ca.ConstructorArguments.Count != attribute.ConstructorArguments.Count)
						continue;

					if (ca.Properties.Count != attribute.Properties.Count)
						continue;

					var allMatch = true;
					for (int i = 0; i < ca.ConstructorArguments.Count; i++) {
						var caArg = ca.ConstructorArguments [i];
						var attrArg = attribute.ConstructorArguments [i];
						if (!object.Equals (caArg.Value, attrArg.Value)) {
							allMatch = false;
							break;
						}
					}
					if (!allMatch)
						continue;

					for (int i = 0; i < ca.Properties.Count; i++) {
						var caProp = ca.Properties [i];
						var attrProp = attribute.Properties [i];

						if (caProp.Name != attrProp.Name) {
							allMatch = false;
							break;
						}

						if (!object.Equals (caProp.Argument.Value, attrProp.Argument.Value)) {
							allMatch = false;
							break;
						}
					}
					if (!allMatch)
						continue;

					// attribute already present
					return false;
				}
			}
			provider.CustomAttributes.Add (attribute);
			if (DebugAttributes)
				Console.WriteLine ($"Added {attribute.RenderAttribute ()} to {provider}");
			return true;
		}

		static bool? debug_attributes;
		static bool DebugAttributes {
			get {
				if (!debug_attributes.HasValue)
					debug_attributes = !string.IsNullOrEmpty (Environment.GetEnvironmentVariable ("PRINT_ATTRIBUTES"));
				return debug_attributes.Value;
			}
		}

		public TypeDefinition GetOrCreateType (ModuleDefinition module, string @namespace, string @typename, out bool created)
		{
			created = false;

			var fullName = @namespace + "." + typename;
			if (!created_types.TryGetValue (fullName, out var cachedTypeDefinition)) {
				cachedTypeDefinition = module.Types.FirstOrDefault (t => t.Namespace == @namespace && t.Name == typename);
				if (cachedTypeDefinition is null) {
					cachedTypeDefinition = new TypeDefinition (@namespace, typename, TypeAttributes.Public | TypeAttributes.Sealed, module.TypeSystem.Object);
					module.Types.Add (cachedTypeDefinition);
					created = true;
				}
				created_types [fullName] = cachedTypeDefinition;
			}

			return cachedTypeDefinition;
		}

		public MethodDefinition CreateInternalPInvoke (ModuleDefinition module, string @namespace, string @typename, string methodName, out bool created)
		{
			var cachedTypeDefinition = GetOrCreateType (module, @namespace, @typename, out _);
			var nativeMethod = methodName;
			var rv = cachedTypeDefinition.Methods.FirstOrDefault (m => m.Name == methodName);
			if (rv is not null) {
				created = false;
				return rv; // already exists, no need to create it again
			}

			// [DllImport ("__Internal")]
			// static extern IntPtr {methodName} ();

			rv = new MethodDefinition (methodName, MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.PInvokeImpl, System_IntPtr);
			rv.IsPreserveSig = true;

			var mod = module.ModuleReferences.FirstOrDefault (mr => mr.Name == "__Internal");
			if (mod is null) {
				mod = new ModuleReference ("__Internal");
				module.ModuleReferences.Add (mod);
			}
			rv.PInvokeInfo = new PInvokeInfo (PInvokeAttributes.CharSetNotSpec | PInvokeAttributes.CallConvCdecl, nativeMethod, mod);

			cachedTypeDefinition.Methods.Add (rv);

			created = true;

			return rv;
		}

		internal static MethodDefinition? FindNSObjectConstructor (TypeDefinition type)
		{
			return FindConstructorWithOneParameter ("ObjCRuntime", "NativeHandle")
				?? FindConstructorWithOneParameter ("System", "IntPtr");

			MethodDefinition? FindConstructorWithOneParameter (string ns, string cls)
				=> type.Methods.SingleOrDefault (method =>
					method.IsConstructor
						&& !method.IsStatic
						&& method.HasParameters
						&& method.Parameters.Count == 1
						&& method.Parameters [0].ParameterType.Is (ns, cls));
		}

		internal static MethodDefinition? FindINativeObjectConstructor (TypeDefinition type)
		{
			return FindConstructorWithTwoParameters ("ObjCRuntime", "NativeHandle", "System", "Boolean")
				?? FindConstructorWithTwoParameters ("System", "IntPtr", "System", "Boolean");

			MethodDefinition? FindConstructorWithTwoParameters (string ns1, string cls1, string ns2, string cls2)
				=> type.Methods.SingleOrDefault (method =>
					method.IsConstructor
						&& !method.IsStatic
						&& method.HasParameters
						&& method.Parameters.Count == 2
						&& method.Parameters [0].ParameterType.Is (ns1, cls1)
						&& method.Parameters [1].ParameterType.Is (ns2, cls2));
		}

		internal bool ImplementConstructNSObjectFactoryMethod (Tuner.DerivedLinkContext context, TypeDefinition type, MethodReference ctor)
		{
			var abr = this;

			// skip creating the factory for NSObject itself
			if (type.Is ("Foundation", "NSObject"))
				return false;

			// Make sure the type implements INSObjectFactory, otherwise we can't override the _Xamarin_ConstructNSObject method from it.
			AddTypeInterfaceImplementation (abr, context, type, abr.Foundation_INSObjectFactory);

			var createInstanceMethod = type.AddMethod ("_Xamarin_ConstructNSObject", MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.NewSlot | MethodAttributes.HideBySig, abr.Foundation_NSObject);
			var nativeHandleParameter = createInstanceMethod.AddParameter (abr.ObjCRuntime_NativeHandle); // nativeHandle
			abr.Foundation_INSObjectFactory.Resolve ().IsPublic = true;
			createInstanceMethod.Overrides.Add (abr.INSObjectFactory__Xamarin_ConstructNSObject);
			var body = createInstanceMethod.CreateBody (out var il);

			if (type.HasGenericParameters) {
				ctor = type.CreateMethodReferenceOnGenericType (ctor, type.GenericParameters.ToArray ());
			}

			// return new TypeA (nativeHandle); // for NativeHandle ctor
			// return new TypeA ((IntPtr) nativeHandle); // for IntPtr ctor
			il.Emit (OpCodes.Ldarg, nativeHandleParameter);
			if (ctor.Parameters [0].ParameterType.Is ("System", "IntPtr"))
				il.Emit (OpCodes.Call, abr.NativeObject_op_Implicit_IntPtr);
			il.Emit (OpCodes.Newobj, ctor);
			il.Emit (OpCodes.Ret);

			body.FinalizeGeneratedBody ();

			// make sure the trimmer doesn't trim it away if the type is kept
			if (context.App.Registrar == RegistrarMode.TrimmableStatic) {
				// TODO: need to investigate why this is needed (https://github.com/dotnet/macios/issues/25232)
				abr.AddDynamicDependencyAttributeToStaticConstructor (type, createInstanceMethod);
			} else {
				context.Annotations.Mark (createInstanceMethod);
			}

			return true;
		}

		internal bool ImplementConstructINativeObjectFactoryMethod (Tuner.DerivedLinkContext context, TypeDefinition type, MethodReference? ctor)
		{
			var abr = this;

			// skip creating the factory for NSObject itself
			if (type.Is ("Foundation", "NSObject"))
				return false;

			// If the type is a subclass of NSObject, we prefer the NSObject "IntPtr" constructor
			MethodReference? nsobjectConstructor = type.IsNSObject (context) ? AppBundleRewriter.FindNSObjectConstructor (type) : null;
			if (nsobjectConstructor is null && ctor is null)
				return false;

			// Make sure the type implements INativeObject, otherwise we can't override the _Xamarin_ConstructINativeObject method from it.
			AddTypeInterfaceImplementation (abr, context, type, abr.ObjCRuntime_INativeObject);

			var createInstanceMethod = type.AddMethod ("_Xamarin_ConstructINativeObject", MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.NewSlot | MethodAttributes.HideBySig, abr.ObjCRuntime_INativeObject);
			var nativeHandleParameter = createInstanceMethod.AddParameter (abr.ObjCRuntime_NativeHandle); // nativeHandle
			var ownsParameter = createInstanceMethod.AddParameter (abr.System_Boolean); // owns
			abr.INativeObject__Xamarin_ConstructINativeObject.Resolve ().IsPublic = true;
			createInstanceMethod.Overrides.Add (abr.INativeObject__Xamarin_ConstructINativeObject);
			var body = createInstanceMethod.CreateBody (out var il);

			if (nsobjectConstructor is not null) {
				// var instance = new TypeA (nativeHandle);
				// // alternatively with a cast: new TypeA ((IntPtr) nativeHandle);
				// if (instance is not null && owns)
				//     Runtime.TryReleaseINativeObject (instance);
				// return instance;

				if (type.HasGenericParameters) {
					nsobjectConstructor = type.CreateMethodReferenceOnGenericType (nsobjectConstructor, type.GenericParameters.ToArray ());
				}

				il.Emit (OpCodes.Ldarg, nativeHandleParameter);
				if (nsobjectConstructor.Parameters [0].ParameterType.Is ("System", "IntPtr"))
					il.Emit (OpCodes.Call, abr.NativeObject_op_Implicit_IntPtr);
				il.Emit (OpCodes.Newobj, nsobjectConstructor);

				var falseTarget = il.Create (OpCodes.Nop);
				il.Emit (OpCodes.Dup);
				il.Emit (OpCodes.Ldnull);
				il.Emit (OpCodes.Cgt_Un);
				il.Emit (OpCodes.Ldarg, ownsParameter);
				il.Emit (OpCodes.And);
				il.Emit (OpCodes.Brfalse_S, falseTarget);

				il.Emit (OpCodes.Dup);
				il.Emit (OpCodes.Call, abr.Runtime_TryReleaseINativeObject);

				il.Append (falseTarget);

				il.Emit (OpCodes.Ret);
			} else if (ctor is not null) {
				// return new TypeA (nativeHandle, owns); // for NativeHandle ctor
				// return new TypeA ((IntPtr) nativeHandle, owns); // IntPtr ctor

				if (type.HasGenericParameters) {
					ctor = type.CreateMethodReferenceOnGenericType (ctor, type.GenericParameters.ToArray ());
				}

				il.Emit (OpCodes.Ldarg, nativeHandleParameter);
				if (ctor.Parameters [0].ParameterType.Is ("System", "IntPtr"))
					il.Emit (OpCodes.Call, abr.NativeObject_op_Implicit_IntPtr);
				il.Emit (OpCodes.Ldarg, ownsParameter);
				il.Emit (OpCodes.Newobj, ctor);
				il.Emit (OpCodes.Ret);
			} else {
				throw new UnreachableException ();
			}

			body.FinalizeGeneratedBody ();

			// make sure the trimmer doesn't trim it away if the type is kept
			if (context.App.Registrar == RegistrarMode.TrimmableStatic) {
				// TODO: need to investigate why this is needed (https://github.com/dotnet/macios/issues/25232)
				abr.AddDynamicDependencyAttributeToStaticConstructor (type, createInstanceMethod);
			} else {
				context.Annotations.Mark (createInstanceMethod);
			}

			return true;
		}

		static void AddTypeInterfaceImplementation (AppBundleRewriter abr, Tuner.DerivedLinkContext context, TypeDefinition type, TypeReference iface)
		{
			if (type.HasInterfaces && type.Interfaces.Any (v => v.InterfaceType == iface))
				return;

			var ifaceImplementation = new InterfaceImplementation (iface);
			type.Interfaces.Add (ifaceImplementation);

			// The trimmer discovers this relationship from the generated method override.
			// Other registrar modes use linker annotations instead.
			if (context.App.Registrar != RegistrarMode.TrimmableStatic) {
				context.Annotations.Mark (ifaceImplementation);
				context.Annotations.Mark (ifaceImplementation.InterfaceType);
				context.Annotations.Mark (ifaceImplementation.InterfaceType.Resolve ());
			}
		}
	}
}
