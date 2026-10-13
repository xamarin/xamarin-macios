// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Mono.Linker;
using Mono.Linker.Steps;
using Mono.Tuner;
using MonoTouch.Tuner;

using Xamarin.Bundler;

#nullable enable

namespace Xamarin.Linker.Steps;

// See docs/code/native-symbols.md for an overview of native symbol handling.
public class InlineDlfcnMethodsStep : AssemblyModifierStep {

	protected override string Name { get; } = "Inline Dlfcn Methods";
	protected override int ErrorCode { get; } = 2250;

	bool strictMode;

	public const string PInvokePrefix = "xamarin_Dlfcn_";
	public const string PInvokeSuffix = "_Native";

	protected override void TryProcess ()
	{
		strictMode = Configuration.InlineDlfcnMethods == InlineDlfcnMethodsMode.Strict;
		base.TryProcess ();
	}

	// When false, we don't rewrite the Dlfcn call sites (to keep reloadable assemblies byte-for-byte
	// unmodified for Hot Reload); instead we only collect the referenced native symbols (see ProcessMethod).
	bool inlining_enabled = true;

	protected override bool ModifyAssembly (AssemblyDefinition assembly)
	{
		// Dlfcn calls can only appear in assemblies that reference (or, for the platform assembly, define)
		// ObjCRuntime.Dlfcn, which is only the platform assembly and binding libraries. Skip everything else
		// (e.g. the BCL and most user assemblies) without iterating all their types, methods and instructions.
		if (!ReferencesDlfcn (assembly))
			return false;

		// When building for Hot Reload compatibility, we must not modify reloadable (user) assemblies,
		// i.e. assemblies that aren't being trimmed (AssemblyAction != Link), because inlining rewrites
		// call sites and adds helper methods/fields, which would break Hot Reload. In that case we don't
		// inline, but we still walk the assembly to collect the referenced native symbols, so the native
		// linker keeps them alive (via RequiredSymbols -> GenerateReferencesStep) just like the inlined
		// P/Invokes would have. Release builds don't set this property, so they keep inlining everywhere
		// (even non-trimmed assemblies) for the optimization. NativeAOT is not compatible with Hot Reload,
		// so we don't have to worry about the post-NativeAOT native symbol collection here.
		inlining_enabled = !(Configuration.HotReloadCompatibleBuild && Annotations.GetAction (assembly) != AssemblyAction.Link);

#if ASSEMBLY_PREPARER
		// If we're not inlining anything, this task's only other purpose is to collect some information about native symbols.
		// However, our product assembly doesn't have any native symbols we care about, so we don't need to process
		// the product assembly at all in this case (which speeds up this task significantly for debug builds).
		if (!inlining_enabled && Configuration.Profile.IsProductAssembly (assembly))
			return false;
#endif

		var modified = base.ModifyAssembly (assembly);
		inlining_enabled = true;
		return modified;
	}

	// When inlining is disabled (Hot Reload compatible build + reloadable assembly), we don't rewrite the
	// Dlfcn call site, but we still register the referenced native symbol so the native linker keeps it
	// alive (GenerateReferencesStep turns RequiredSymbols into native references, just like the surviving
	// inlined P/Invokes would). Returns true if the symbol was collected and the caller must not inline,
	// false if inlining should proceed as usual.
	bool CollectSymbolWithoutInlining (string symbolName)
	{
		if (inlining_enabled)
			return false;
		DerivedLinkContext.RequiredSymbols.AddField (symbolName);
		return true;
	}

	bool ReferencesDlfcn (AssemblyDefinition assembly)
	{
		// Dlfcn lives in the product assembly, so an assembly that doesn't even reference the product assembly
		// (the BCL, most third-party code) can't possibly call it. This only looks at the assembly references,
		// so it's the cheapest check - do it first.
		if (!Configuration.Profile.IsOrReferencesProductAssembly (assembly))
			return false;

		// The product assembly defines (and calls) Dlfcn itself.
		if (Configuration.Profile.IsProductAssembly (assembly))
			return true;

		// Otherwise (binding libraries, user code that references the product assembly) it must reference the
		// Dlfcn type specifically - this scans the type reference table, so it's the most expensive check.
		return assembly.MainModule.HasTypeReference ("ObjCRuntime.Dlfcn");
	}

	string? current_framework;
	protected override bool ProcessType (TypeDefinition type)
	{
		var modified = false;
		if (type.HasMethods) {
			if (Frameworks.TryGetFramework (App, type, out Framework? framework) && framework.IsFrameworkUnavailable (App)) {
				App.Log (3, $"Type {type.FullName} appears to be part of the '{framework.Name}' framework, which is not available in the current SDK. Skipping inlining Dlfcn calls for this type.");
				return modified;
			}

			// If the type isn't available in the simulator, and we're building for the simulator, then don't
			// inline any of its methods. Checking this once per type (instead of once per method) avoids
			// re-scanning the declaring type's availability attributes for every method.
			if (DerivedLinkContext.App.IsSimulatorBuild && DerivedLinkContext.HasAvailabilityAttributesShowingUnavailableInSimulator (type)) {
				App.Log (3, $"Type {type.FullName} is not available in the simulator. Skipping inlining Dlfcn calls for this type.");
				return modified;
			}

			current_framework = framework?.Namespace;

			foreach (var method in type.Methods)
				modified |= ProcessMethod (method);

			current_framework = null;
		}
		return modified;
	}

	TypeDefinition GetDlfcnType (MethodDefinition callingMethod)
	{
		// Check if there's a [Field] attribute with a second string argument (the library/namespace).
		// The [Field] attribute can be on the method itself, or on the property the method is a getter/setter for.
		var libraryName = callingMethod.HasCustomAttributes ? GetFieldAttributeLibraryName (callingMethod.CustomAttributes) : null;
		if (libraryName is null && callingMethod.DeclaringType.HasProperties) {
			foreach (var property in callingMethod.DeclaringType.Properties) {
				if (property.GetMethod == callingMethod || property.SetMethod == callingMethod) {
					libraryName = GetFieldAttributeLibraryName (property.CustomAttributes);
					break;
				}
			}
		}

		if (libraryName is not null)
			return GetDlfcnType (callingMethod.Module, callingMethod.DeclaringType.Namespace, libraryName);

		return GetDlfcnType (callingMethod.Module, callingMethod.DeclaringType.Namespace);
	}

	static string? GetFieldAttributeLibraryName (IList<CustomAttribute>? attributes)
	{
		if (attributes is null || attributes.Count == 0)
			return null;

		foreach (var attrib in attributes) {
			if (attrib.AttributeType.Name != "FieldAttribute")
				continue;
			if (attrib.ConstructorArguments.Count == 2 &&
				attrib.ConstructorArguments [1].Type.Name == "String" &&
				attrib.ConstructorArguments [1].Value is string libraryName &&
				!string.IsNullOrEmpty (libraryName)) {
				return libraryName;
			}
		}

		return null;
	}

	// It's important to use a type in the same namespace as the calling code, so that
	// we correctly compute which frameworks to link with.
	TypeDefinition GetDlfcnType (ModuleDefinition module, string @namespace, string? fieldLibraryName = null)
	{
		var frameworkOverride = !string.IsNullOrEmpty (fieldLibraryName) ? fieldLibraryName : current_framework;
		var ns = frameworkOverride ?? @namespace;
		var rv = abr.GetOrCreateType (module, ns, "Dlfcn", out var created);
		if (created) {
			if (!string.IsNullOrEmpty (frameworkOverride)) {
				var attrib = abr.CreateAttribute (abr.ObjectiveCFrameworkAttribute_ctor_String);
				attrib.ConstructorArguments.Add (new CustomAttributeArgument (abr.System_String, frameworkOverride));
				rv.CustomAttributes.Add (attrib);
			}
		}
		return rv;
	}

	void AddField (string assemblyName, string symbolName)
	{
		if (!Configuration.InlinedDlfcnFields.TryGetValue (assemblyName, out var set)) {
			set = new HashSet<string> ();
			Configuration.InlinedDlfcnFields [assemblyName] = set;
		}
		set.Add (symbolName);
	}

	MethodDefinition GetOrCreatePInvokeMethod (MethodDefinition callingMethod, string symbolName)
	{
		// [DllImport ("__Internal")]
		// static extern IntPtr xamarin_Dlfcn_{symbolName}_Native ();

		var methodName = $"{PInvokePrefix}{symbolName}{PInvokeSuffix}";
		var rv = abr.CreateInternalPInvoke (callingMethod.Module, callingMethod.DeclaringType.Namespace, "Dlfcn", methodName, out var created);
		if (created)
			AddField (callingMethod.Module.Assembly.Name.Name, symbolName);
		return rv;
	}

	MethodDefinition GetOrCreateGetSymbolMethod (MethodDefinition callingMethod, string symbolName)
	{
		var dlfcn = GetDlfcnType (callingMethod);
		var methodName = $"Get__{symbolName}";
		var symbolMethod = dlfcn.Methods.FirstOrDefault (m => m.Name == methodName);
		if (symbolMethod is not null)
			return symbolMethod; // already exists, no need to create it again

		// static bool Get__{symbolName}_Initialized;
		// static IntPtr Get__{symbolName}_Cached;
		// static IntPtr Get__{symbolName} ()
		// {
		//     if (!Get__{symbolName}_Initialized) {
		//         Get__{symbolName}_Cached = xamarin_Dlfcn_{symbolName}_Native ();
		//         Get__{symbolName}_Initialized = true;
		//     }
		//     return Get__{symbolName}_Cached;
		// }

		var initializedField = new FieldDefinition ($"Get__{symbolName}_Initialized", FieldAttributes.Private | FieldAttributes.Static, callingMethod.Module.TypeSystem.Boolean);
		dlfcn.Fields.Add (initializedField);

		var cachedField = new FieldDefinition ($"Get__{symbolName}_Cached", FieldAttributes.Private | FieldAttributes.Static, abr.System_IntPtr);
		dlfcn.Fields.Add (cachedField);

		var intptr = abr.System_IntPtr;
		symbolMethod = new MethodDefinition (methodName, MethodAttributes.Public | MethodAttributes.Static, intptr);
		dlfcn.Methods.Add (symbolMethod);

		var body = symbolMethod.Body;
		var il = body.GetILProcessor ();

		var loadCachedFieldInstruction = il.Create (OpCodes.Ldsfld, cachedField);

		// if (!Get__{symbolName}_Initialized) {
		// The initialized field must use volatile semantics to prevent ARM64 store
		// reordering: without it, another thread could see Initialized=true before
		// the Cached value is written, and return a stale IntPtr.Zero.
		// Note that we don't care if we call the P/Invoke more than once, that's
		// a safe operation, we just don't want to call it every time.
		il.Append (il.Create (OpCodes.Volatile));
		il.Append (il.Create (OpCodes.Ldsfld, initializedField));
		il.Append (il.Create (OpCodes.Brtrue, loadCachedFieldInstruction));

		//     Get__{symbolName}_Cached = xamarin_Dlfcn_{symbolName}_Native ();
		il.Append (il.Create (OpCodes.Call, GetOrCreatePInvokeMethod (callingMethod, symbolName)));
		il.Append (il.Create (OpCodes.Stsfld, cachedField));

		//     Get__{symbolName}_Initialized = true;
		il.Append (il.Create (OpCodes.Ldc_I4_1));
		il.Append (il.Create (OpCodes.Volatile));
		il.Append (il.Create (OpCodes.Stsfld, initializedField));

		// return Get__{symbolName}_Cached;
		il.Append (loadCachedFieldInstruction);
		il.Append (il.Create (OpCodes.Ret));

		return symbolMethod;
	}

	MethodDefinition GetOrCreateGetNativeFieldMethod (MethodDefinition callingMethod, TypeReference fieldType, string symbolName)
	{
		var dlfcn = GetDlfcnType (callingMethod);
		var methodName = $"Get__{symbolName}_{fieldType.Name}";
		var rv = dlfcn.Methods.FirstOrDefault (m => m.Name == methodName);
		if (rv is not null)
			return rv; // already exists, no need to create it again

		// static FieldType Get__{symbolName}_{fieldType} ()
		// {
		//     var ptr = Get__{symbolName} ();
		//     if (ptr == IntPtr.Zero)
		//	       return default;
		//
		//     /* if value type */
		//     return *(FieldType*)ptr;
		//
		//     /* if not value type */
		//     return Runtime.GetNSObject<FieldType> (*ptr);
		// }

		var importedFieldType = callingMethod.Module.ImportReference (fieldType);
		rv = new MethodDefinition (methodName, MethodAttributes.Public | MethodAttributes.Static, importedFieldType);
		dlfcn.Methods.Add (rv);

		var body = rv.Body;
		var il = body.GetILProcessor ();

		var ptrVariable = new VariableDefinition (abr.System_IntPtr);
		body.Variables.Add (ptrVariable);

		var loadPointerInstructionStart = il.Create (OpCodes.Ldloc, ptrVariable);

		// var ptr = Get__{symbolName} ();
		il.Append (il.Create (OpCodes.Call, GetOrCreateGetSymbolMethod (callingMethod, symbolName)));
		il.Append (il.Create (OpCodes.Stloc, ptrVariable));

		// if (ptr == IntPtr.Zero)
		il.Append (il.Create (OpCodes.Ldloc, ptrVariable));
		il.Append (il.Create (OpCodes.Ldsfld, abr.System_IntPtr_Zero));
		il.Append (il.Create (OpCodes.Bne_Un, loadPointerInstructionStart));

		//   return default;
		var fullFieldTypeName = fieldType.FullName;
		switch (fullFieldTypeName) {
		case "System.Byte":
		case "System.SByte":
		case "System.Int16":
		case "System.UInt16":
		case "System.Int32":
		case "System.UInt32":
			il.Append (il.Create (OpCodes.Ldc_I4_0));
			break;
		case "System.Int64":
			il.Append (il.Create (OpCodes.Ldc_I4_0));
			il.Append (il.Create (OpCodes.Conv_I8));
			break;
		case "System.UInt64":
			il.Append (il.Create (OpCodes.Ldc_I4_0));
			il.Append (il.Create (OpCodes.Conv_U8));
			break;
		case "System.Single":
			il.Append (il.Create (OpCodes.Ldc_R4, 0f));
			break;
		case "System.Double":
			il.Append (il.Create (OpCodes.Ldc_R8, 0.0));
			break;
		case "System.IntPtr":
		case "System.UIntPtr":
			il.Append (il.Create (OpCodes.Ldc_I4_0));
			il.Append (il.Create (OpCodes.Conv_I));
			break;
		default:
			if (fieldType.IsValueType) {
				if (fieldType.IsPrimitive) {
					Report (ErrorHelper.CreateWarning (Configuration.Application, 2254 /* Unsupported primitive field type '{0}' for symbol '{1}' in method '{2}'. Sub-optimal but functional code will be generated. Please file an issue at https://github.com/dotnet/macios/issues/new. */, callingMethod, Errors.MX2254, fieldType.FullName, symbolName, FormatMethod (callingMethod)));
				}
				var defaultTemporary = new VariableDefinition (importedFieldType);
				body.Variables.Add (defaultTemporary);
				il.Append (il.Create (OpCodes.Ldloca, defaultTemporary));
				il.Append (il.Create (OpCodes.Initobj, importedFieldType));
				il.Append (il.Create (OpCodes.Ldloc, defaultTemporary));
			} else {
				il.Append (il.Create (OpCodes.Ldnull));
			}
			break;
		}
		il.Append (il.Create (OpCodes.Ret));

		// /* if value type */
		// return *(FieldType*)ptr;
		// /* if not value type */
		// return Runtime.GetNSObject<FieldType> (*(IntPtr*)ptr);
		il.Append (loadPointerInstructionStart); // il.Create (OpCodes.Ldloc, ptrVariable);
		if (fieldType.IsValueType) {
			switch (fieldType.FullName) {
			case "System.Byte":
				il.Append (il.Create (OpCodes.Ldind_U1));
				break;
			case "System.SByte":
				il.Append (il.Create (OpCodes.Ldind_I1));
				break;
			case "System.Int16":
				il.Append (il.Create (OpCodes.Ldind_I2));
				break;
			case "System.UInt16":
				il.Append (il.Create (OpCodes.Ldind_U2));
				break;
			case "System.Int32":
				il.Append (il.Create (OpCodes.Ldind_I4));
				break;
			case "System.UInt32":
				il.Append (il.Create (OpCodes.Ldind_U4));
				break;
			case "System.Int64":
			case "System.UInt64":
				il.Append (il.Create (OpCodes.Ldind_I8));
				break;
			case "System.Single":
				il.Append (il.Create (OpCodes.Ldind_R4));
				break;
			case "System.Double":
				il.Append (il.Create (OpCodes.Ldind_R8));
				break;
			case "System.IntPtr":
			case "System.UIntPtr":
				il.Append (il.Create (OpCodes.Ldind_I));
				break;
			default:
				if (fieldType.IsPrimitive) {
					Report (ErrorHelper.CreateWarning (Configuration.Application, 2254 /* Unsupported primitive field type '{0}' for symbol '{1}' in method '{2}'. Sub-optimal but functional code will be generated. Please file an issue at https://github.com/dotnet/macios/issues/new. */, callingMethod, Errors.MX2254, fieldType.FullName, symbolName, FormatMethod (callingMethod)));
				}
				il.Append (il.Create (OpCodes.Ldobj, importedFieldType));
				break;
			}
		} else if (IsNSObjectSubclass (fieldType)) {
			il.Append (il.Create (OpCodes.Ldind_I));
			var getnsobject = abr.Runtime_GetNSObject_T___System_IntPtr.CreateGenericInstanceMethod (importedFieldType);
			il.Append (il.Create (OpCodes.Call, getnsobject));
		} else {
			Report (ErrorHelper.CreateError (Configuration.Application, 2256 /* The field type '{0}' for symbol '{1}' in method '{2}' is not an NSObject subclass. Please file an issue at https://github.com/dotnet/macios/issues/new */, callingMethod, Errors.MX2256, fieldType.FullName, symbolName, FormatMethod (callingMethod)));
		}
		il.Append (il.Create (OpCodes.Ret));

		// See the comment in CecilExtensions.FinalizeGeneratedBody for why this is needed.
		body.FinalizeGeneratedBody ();

		return rv;
	}

	MethodDefinition GetOrCreateSetNativeFieldMethod (MethodDefinition callingMethod, TypeReference fieldType, string symbolName)
	{
		var dlfcn = GetDlfcnType (callingMethod);
		var methodName = $"Set__{symbolName}_{fieldType.Name}";
		var rv = dlfcn.Methods.FirstOrDefault (m => m.Name == methodName);
		if (rv is not null)
			return rv; // already exists, no need to create it again

		// static void Set__{symbolName}_{fieldType} ({FieldType} value)
		// {
		//     var ptr = Get__{symbolName} ();
		//     if (ptr == IntPtr.Zero)
		//	       return;
		//
		//     /* if value type */
		//     *(FieldType*)ptr = value;
		//
		//     /* if not value type */
		//     *(IntPtr*)ptr = (IntPtr) Runtime.RetainNSObject (value)
		// }
		//
		// Notes:
		// * Just like the Dlfcn method(s), this generated code does not release an existing value of a field.

		var importedFieldType = callingMethod.Module.ImportReference (fieldType);
		rv = new MethodDefinition (methodName, MethodAttributes.Public | MethodAttributes.Static, abr.System_Void);
		rv.Parameters.Add (new ParameterDefinition ("value", ParameterAttributes.None, importedFieldType));
		dlfcn.Methods.Add (rv);

		var body = rv.Body;
		var il = body.GetILProcessor ();

		var ptrVariable = new VariableDefinition (abr.System_IntPtr);
		body.Variables.Add (ptrVariable);

		var loadPointerInstructionStart = il.Create (OpCodes.Ldloc, ptrVariable);

		// var ptr = Get__{symbolName} ();
		il.Append (il.Create (OpCodes.Call, GetOrCreateGetSymbolMethod (callingMethod, symbolName)));
		il.Append (il.Create (OpCodes.Stloc, ptrVariable));

		// if (ptr == IntPtr.Zero)
		il.Append (il.Create (OpCodes.Ldloc, ptrVariable));
		il.Append (il.Create (OpCodes.Ldsfld, abr.System_IntPtr_Zero));
		il.Append (il.Create (OpCodes.Bne_Un, loadPointerInstructionStart));
		//   return;
		il.Append (il.Create (OpCodes.Ret));

		// /* if value type */
		// *(FieldType*)ptr = value;
		// /* if not value type */
		// *(IntPtr*)ptr = (IntPtr) Runtime.RetainNSObject (value)
		il.Append (loadPointerInstructionStart); // il.Create (OpCodes.Ldloc, ptrVariable);
		il.Append (il.Create (OpCodes.Ldarg_0));
		if (fieldType.IsValueType) {
			switch (fieldType.FullName) {
			case "System.Byte":
			case "System.SByte":
				il.Append (il.Create (OpCodes.Stind_I1));
				break;
			case "System.Int16":
			case "System.UInt16":
				il.Append (il.Create (OpCodes.Stind_I2));
				break;
			case "System.Int32":
			case "System.UInt32":
				il.Append (il.Create (OpCodes.Stind_I4));
				break;
			case "System.Int64":
			case "System.UInt64":
				il.Append (il.Create (OpCodes.Stind_I8));
				break;
			case "System.Single":
				il.Append (il.Create (OpCodes.Stind_R4));
				break;
			case "System.Double":
				il.Append (il.Create (OpCodes.Stind_R8));
				break;
			case "System.IntPtr":
			case "System.UIntPtr":
				il.Append (il.Create (OpCodes.Stind_I));
				break;
			default:
				if (fieldType.IsPrimitive) {
					Report (ErrorHelper.CreateWarning (Configuration.Application, 2254 /* Unsupported primitive field type '{0}' for symbol '{1}' in method '{2}'. Sub-optimal but functional code will be generated. Please file an issue at https://github.com/dotnet/macios/issues/new. */, callingMethod, Errors.MX2254, fieldType.FullName, symbolName, FormatMethod (callingMethod)));
				}
				il.Append (il.Create (OpCodes.Stobj, importedFieldType));
				break;
			}
		} else if (IsNSObjectSubclass (fieldType)) {
			il.Append (il.Create (OpCodes.Call, abr.Runtime_RetainNSObject));
			il.Append (il.Create (OpCodes.Call, abr.NativeObject_op_Implicit_IntPtr));
			il.Append (il.Create (OpCodes.Stind_I));
		} else {
			Report (ErrorHelper.CreateError (Configuration.Application, 2256 /* The field type '{0}' for symbol '{1}' in method '{2}' is not an NSObject subclass. Please file an issue at https://github.com/dotnet/macios/issues/new */, callingMethod, Errors.MX2256, fieldType.FullName, symbolName, FormatMethod (callingMethod)));
		}
		il.Append (il.Create (OpCodes.Ret));

		// See the comment in CecilExtensions.FinalizeGeneratedBody for why this is needed.
		body.FinalizeGeneratedBody ();

		return rv;
	}

	MethodDefinition GetOrCreateSetNativeStringMethod (MethodDefinition callingMethod, string symbolName)
	{
		var dlfcn = GetDlfcnType (callingMethod);
		var methodName = $"Set__{symbolName}_String";
		var rv = dlfcn.Methods.FirstOrDefault (m => m.Name == methodName);
		if (rv is not null)
			return rv; // already exists, no need to create it again

		// static FieldType Set__{symbolName}_String (string? value)
		// {
		//     var ptr = Get__{symbolName} ();
		//     if (ptr == IntPtr.Zero)
		//	       return;
		//
		//     *(IntPtr*)ptr = (IntPtr) CFString.CreateNative (value);
		// }
		//
		// Notes:
		// * Just like the Dlfcn method(s), this generated code does not release an existing value of a field.

		rv = new MethodDefinition (methodName, MethodAttributes.Public | MethodAttributes.Static, abr.System_Void);
		rv.Parameters.Add (new ParameterDefinition ("value", ParameterAttributes.None, abr.System_String));
		dlfcn.Methods.Add (rv);

		var body = rv.Body;
		var il = body.GetILProcessor ();

		var ptrVariable = new VariableDefinition (abr.System_IntPtr);
		body.Variables.Add (ptrVariable);

		var loadPointerInstructionStart = il.Create (OpCodes.Ldloc, ptrVariable);

		//     var ptr = Get__{symbolName} ();
		il.Append (il.Create (OpCodes.Call, GetOrCreateGetSymbolMethod (callingMethod, symbolName)));
		il.Append (il.Create (OpCodes.Stloc, ptrVariable));

		// if (ptr == IntPtr.Zero)
		il.Append (il.Create (OpCodes.Ldloc, ptrVariable));
		il.Append (il.Create (OpCodes.Ldsfld, abr.System_IntPtr_Zero));
		il.Append (il.Create (OpCodes.Bne_Un, loadPointerInstructionStart));
		//   return;
		il.Append (il.Create (OpCodes.Ret));

		// *(IntPtr*)ptr = (IntPtr) CFString.CreateNative (value);
		il.Append (loadPointerInstructionStart); // il.Create (OpCodes.Ldloc, ptrVariable);
		il.Append (il.Create (OpCodes.Ldarg_0));
		il.Append (il.Create (OpCodes.Call, abr.CFString_CreateNative));
		il.Append (il.Create (OpCodes.Call, abr.NativeObject_op_Implicit_IntPtr));
		il.Append (il.Create (OpCodes.Stind_I));
		il.Append (il.Create (OpCodes.Ret));

		// See the comment in CecilExtensions.FinalizeGeneratedBody for why this is needed.
		body.FinalizeGeneratedBody ();

		return rv;
	}

	bool InlineSymbol (string symbolName)
	{
		// In compatibility mode, only inline symbols from [Field] attributes.
		if (!strictMode && !Configuration.FieldSymbols.Contains (symbolName))
			return false;

		// These symbols already come from [Objective-]C code, so they should already be valid identifiers,
		// which means we don't have to validate them.

		var requiredSymbol = DerivedLinkContext.RequiredSymbols.Find (symbolName);
		if (requiredSymbol?.Mode == SymbolMode.Ignore)
			return false; // don't inline if the symbol is to be ignored

		return true;
	}

	protected override bool ProcessMethod (MethodDefinition method)
	{
		var modified = false;

		if (!method.HasBody)
			return modified;

		if (method.DeclaringType.Name == "Dlfcn" && method.DeclaringType.Namespace == "ObjCRuntime")
			return modified; // don't process the Dlfcn methods themselves

		if (DerivedLinkContext.App.IsSimulatorBuild) {
			// if the method isn't available in the simulator, and we're building for the simulator, then don't inline.
			// (the declaring type's availability is checked once per type in ProcessType.)
			if (DerivedLinkContext.HasAvailabilityAttributesShowingUnavailableInSimulator (method, method)) {
				App.Log (3, $"Method {method.FullName} is not available in the simulator. Skipping inlining Dlfcn calls for this method.");
				return modified;
			}
		}

		foreach (var instr in method.Body.Instructions) {
			if (instr.Operand is not MethodReference mr)
				continue;
			if (mr.DeclaringType.Name != "Dlfcn" || mr.DeclaringType.Namespace != "ObjCRuntime")
				continue;

			// Handle Dlfcn functions of the form (libraryHandle, symbolName)
			if (mr.Parameters.Count == 2 && mr.Parameters [0].ParameterType.FullName == "System.IntPtr" && mr.Parameters [1].ParameterType.FullName == "System.String") {
				if (instr.Previous.OpCode != OpCodes.Ldstr) {
					App.Log (3, $"Unknown or unsupported Dlfcn pattern: '{FormatMethod (mr)}' in method '{FormatMethod (method)}'. The call will not be inlined.");
					continue;
				}

				// In compatibility mode, only inline symbols from [Field] attributes.
				var ldstr = instr.Previous;
				if (ldstr.Operand is not string symbolName) {
					App.Log (3, $"Unknown or unsupported Dlfcn pattern: '{FormatMethod (mr)}' in method '{FormatMethod (method)}'. The call will not be inlined.");
					continue;
				}
				if (!InlineSymbol (symbolName))
					continue;

				if (CollectSymbolWithoutInlining (symbolName))
					continue;

				switch (mr.Name) {
				// primitive types
				case "GetDouble":
				case "GetFloat":
				case "GetNFloat":
				case "GetIntPtr":
				case "GetUIntPtr":
				case "GetNInt":
				case "GetNUInt":
				case "GetInt16":
				case "GetUInt16":
				case "GetInt32":
				case "GetUInt32":
				case "GetInt64":
				case "GetUInt64":
				// non-primitive value types
				case "GetCGSize":
				case "GetCGRect":
				// classes
				case "GetNSNumber":
				case "GetStringConstant":
					ldstr.OpCode = OpCodes.Pop; // just pop the library handle/name, we don't need it
					ldstr.Operand = null;

					instr.OpCode = OpCodes.Call;
					instr.Operand = GetOrCreateGetNativeFieldMethod (method, mr.ReturnType, symbolName);

					modified = true;
					continue;
				case "GetStruct":
					if (mr is not GenericInstanceMethod gim || gim.GenericArguments.Count != 1) {
						App.Log (3, $"Unknown or unsupported Dlfcn pattern: '{FormatMethod (mr)}' in method '{FormatMethod (method)}'. The call will not be inlined.");
						continue;
					}
					var returnType = gim.GenericArguments [0];
					if (returnType.IsGenericInstance) {
						App.Log (3, $"Unknown or unsupported Dlfcn pattern: '{FormatMethod (mr)}' in method '{FormatMethod (method)}'. The call will not be inlined.");
						continue;
					}

					ldstr.OpCode = OpCodes.Pop; // just pop the library handle/name, we don't need it
					ldstr.Operand = null;

					instr.OpCode = OpCodes.Call;
					instr.Operand = GetOrCreateGetNativeFieldMethod (method, returnType, symbolName);

					modified = true;
					continue;
				case "GetIndirect":
				case "dlsym":
					ldstr.OpCode = OpCodes.Pop; // just pop the library handle/name, we don't need it
					ldstr.Operand = null;

					instr.OpCode = OpCodes.Call;
					instr.Operand = GetOrCreateGetSymbolMethod (method, symbolName);

					modified = true;
					continue;
				}
			}

			// Handle Dlfcn functions of the form (RTLD, symbolName)
			if (mr.Parameters.Count == 2 && mr.Parameters [0].ParameterType.FullName == "ObjCRuntime.Dlfcn/RTLD" && mr.Parameters [1].ParameterType.FullName == "System.String") {
				if (instr.Previous.OpCode != OpCodes.Ldstr) {
					App.Log (3, $"Unknown or unsupported Dlfcn pattern: '{FormatMethod (mr)}' in method '{FormatMethod (method)}'. The call will not be inlined.");
					continue;
				}
				var ldstr = instr.Previous;
				if (ldstr.Operand is not string symbolName) {
					App.Log (3, $"Unknown or unsupported Dlfcn pattern: '{FormatMethod (mr)}' in method '{FormatMethod (method)}'. The call will not be inlined.");
					continue;
				}

				if (!InlineSymbol (symbolName))
					continue;

				if (CollectSymbolWithoutInlining (symbolName))
					continue;


				switch (mr.Name) {
				case "dlsym":
					ldstr.OpCode = OpCodes.Pop; // just pop the library handle/name, we don't need it
					ldstr.Operand = null;

					instr.OpCode = OpCodes.Call;
					instr.Operand = GetOrCreateGetSymbolMethod (method, symbolName);

					modified = true;
					continue;
				}
			}

			// Handle Dlfcn functions of the form (libraryName, symbolName)
			if (mr.Parameters.Count == 2 && mr.Parameters [0].ParameterType.FullName == "System.String" && mr.Parameters [1].ParameterType.FullName == "System.String") {
				if (instr.Previous.OpCode != OpCodes.Ldstr) {
					App.Log (3, $"Unknown or unsupported Dlfcn pattern: '{FormatMethod (mr)}' in method '{FormatMethod (method)}'. The call will not be inlined.");
					continue;
				}
				var ldstr = instr.Previous;
				if (ldstr.Operand is not string symbolName) {
					App.Log (3, $"Unknown or unsupported Dlfcn pattern: '{FormatMethod (mr)}' in method '{FormatMethod (method)}'. The call will not be inlined.");
					continue;
				}

				if (!InlineSymbol (symbolName))
					continue;

				if (CollectSymbolWithoutInlining (symbolName))
					continue;

				switch (mr.Name) {
				// primitive types
				case "SlowGetDouble":
				case "SlowGetIntPtr":
				case "SlowGetInt32":
				case "SlowGetInt64":
				// classes
				case "SlowGetStringConstant":
					ldstr.OpCode = OpCodes.Pop; // just pop the library handle/name, we don't need it
					ldstr.Operand = null;

					instr.OpCode = OpCodes.Call;
					instr.Operand = GetOrCreateGetNativeFieldMethod (method, mr.ReturnType, symbolName);

					modified = true;
					continue;
				}
			}

			// Handle Dlfcn functions of the form void (libraryHandle|libraryName, symbolName, value)
			if (mr.Parameters.Count == 3 &&
				(mr.Parameters [0].ParameterType.FullName == "System.String" || mr.Parameters [0].ParameterType.FullName == "System.IntPtr") &&
				mr.Parameters [1].ParameterType.FullName == "System.String") {

				var ins = instr;
				Instruction? ldstr = null;

				// skip any call instructions that take a single argument and return a value, as those are likely to be calls to op_Implicit or op_Explicit functions.
				while (ins.Previous.OpCode == OpCodes.Call && ins.Previous.Operand is MethodReference prevMr && !prevMr.ReturnType.Is ("System", "Void") && prevMr.HasParameters && prevMr.Parameters.Count == 1) {
					ins = ins.Previous;
				}

				switch (ins.Previous.OpCode.StackBehaviourPop) {
				case StackBehaviour.Pop0:
					switch (ins.Previous.OpCode.StackBehaviourPush) {
					case StackBehaviour.Push1:
					case StackBehaviour.Pushi:
					case StackBehaviour.Pushi8:
					case StackBehaviour.Pushr4:
					case StackBehaviour.Pushr8:
					case StackBehaviour.Pushref:
						ldstr = ins.Previous.Previous;
						break;
					}
					break;
				case StackBehaviour.Pop1:
				case StackBehaviour.Popref:
					switch (ins.Previous.OpCode.StackBehaviourPush) {
					case StackBehaviour.Push1:
					case StackBehaviour.Pushi:
					case StackBehaviour.Pushi8:
					case StackBehaviour.Pushr4:
					case StackBehaviour.Pushr8:
					case StackBehaviour.Pushref:
						ldstr = ins.Previous.Previous.Previous;
						break;
					}
					break;
				}
				if (ldstr is null) {
					App.Log (3, $"Unknown or unsupported Dlfcn pattern: '{FormatMethod (mr)}' in method '{FormatMethod (method)}'. Unknown instruction sequence: {instr.Previous} ({instr.Previous.OpCode.StackBehaviourPop}/{instr.Previous.OpCode.StackBehaviourPush}). The call will not be inlined.");
					continue;
				}

				if (ldstr.OpCode != OpCodes.Ldstr) {
					App.Log (3, $"Unknown or unsupported Dlfcn pattern: '{FormatMethod (mr)}' in method '{FormatMethod (method)}'. Expected 'ldstr' opcode, got '{ldstr}'. The call will not be inlined.");
					continue;
				}
				if (ldstr.Operand is not string symbolName) {
					App.Log (3, $"Unknown or unsupported Dlfcn pattern: '{FormatMethod (mr)}' in method '{FormatMethod (method)}'. The call will not be inlined.");
					continue;
				}

				if (!InlineSymbol (symbolName))
					continue;

				if (CollectSymbolWithoutInlining (symbolName))
					continue;

				switch (mr.Name) {
				// primitive types
				case "SetSByte":
				case "SetByte":
				case "SetInt16":
				case "SetUInt16":
				case "SetInt32":
				case "SetUInt32":
				case "SetInt64":
				case "SetUInt64":
				case "SetArray":
				case "SetObject":
				case "SetNInt":
				case "SetNUInt":
				case "SetNFloat":
				case "SetUIntPtr":
				case "SetIntPtr":
				case "SetCGSize":
				case "SetDouble":
				case "SetFloat":
					ldstr.OpCode = OpCodes.Pop; // just pop the library handle/name, we don't need it
					ldstr.Operand = null;

					instr.OpCode = OpCodes.Call;
					instr.Operand = GetOrCreateSetNativeFieldMethod (method, mr.Parameters [2].ParameterType, symbolName);

					modified = true;
					continue;
				// classes
				case "SetString":
					if (mr.Parameters [2].ParameterType.FullName == "System.String") {
						ldstr.OpCode = OpCodes.Pop; // just pop the library handle/name, we don't need it
						ldstr.Operand = null;

						instr.OpCode = OpCodes.Call;
						instr.Operand = GetOrCreateSetNativeStringMethod (method, symbolName);
						modified = true;
						continue;
					} else if (mr.Parameters [2].ParameterType.FullName == "Foundation.NSString") {
						ldstr.OpCode = OpCodes.Pop; // just pop the library handle/name, we don't need it
						ldstr.Operand = null;

						instr.OpCode = OpCodes.Call;
						instr.Operand = GetOrCreateSetNativeFieldMethod (method, mr.Parameters [2].ParameterType, symbolName);
						modified = true;
						continue;
					}

					App.Log (3, $"Unknown or unsupported Dlfcn pattern: '{FormatMethod (mr)}' in method '{FormatMethod (method)}'. The call will not be inlined.");
					continue;
				case "CachePointer":
					if (!(mr.Parameters [2].ParameterType is PointerType pt && pt.ElementType.FullName == "System.IntPtr")) {
						App.Log (3, $"Unknown or unsupported Dlfcn pattern: '{FormatMethod (mr)}' in method '{FormatMethod (method)}'. The call will not be inlined.");
						continue;
					}

					//
					// we're going to replace the entire method body with something like:
					//
					// var ptr = Get__{symbolName} ();
					// if (ptr == IntPtr.Zero)
					//    return IntPtr.Zero;
					// return *(IntPtr *) ptr;
					//

					if (!IsGeneratedCachePointerMethod (method, out var cachePointerSymbolName, out var failureMessage)) {
						Report (ErrorHelper.CreateWarning (Configuration.Application, 2257 /* Unknown IL sequence for method with call to Dlfcn.CachePointer: '{0}' in method '{1}'. The call will not be inlined. Please file an issue at https://github.com/dotnet/macios/issues/new. */, method, Errors.MX2257, failureMessage, FormatMethod (method)));
						continue;
					}

					if (cachePointerSymbolName != symbolName) {
						Report (ErrorHelper.CreateWarning (Configuration.Application, 2257 /* Unknown IL sequence for method with call to Dlfcn.CachePointer: '{0}' in method '{1}'. The call will not be inlined. Please file an issue at https://github.com/dotnet/macios/issues/new. */, method, Errors.MX2257, $"Could not determine symbol name", FormatMethod (method)));
						continue;
					}

					ldstr.OpCode = OpCodes.Pop; // just pop the library handle/name, we don't need it
					ldstr.Operand = null;

					method.Body.Instructions.Clear ();
					var il = method.Body.GetILProcessor ();
					var ptrVariable = new VariableDefinition (abr.System_IntPtr);
					method.Body.Variables.Add (ptrVariable);
					var loadPointerInstructionStart = il.Create (OpCodes.Ldloc, ptrVariable);
					// var ptr = Get__{symbolName} ()
					il.Append (il.Create (OpCodes.Call, GetOrCreateGetSymbolMethod (method, symbolName)));
					il.Append (il.Create (OpCodes.Stloc, ptrVariable));
					// if (ptr == IntPtr.Zero)
					il.Append (il.Create (OpCodes.Ldloc, ptrVariable));
					il.Append (il.Create (OpCodes.Brtrue_S, loadPointerInstructionStart));
					//   return IntPtr.Zero;
					il.Append (il.Create (OpCodes.Ldc_I4_0));
					il.Append (il.Create (OpCodes.Conv_I));
					il.Append (il.Create (OpCodes.Ret));
					// return *(IntPtr *) ptr;
					il.Append (loadPointerInstructionStart); // il.Create (OpCodes.Ldloc, ptrVariable)
					il.Append (il.Create (OpCodes.Ldind_I));
					il.Append (il.Create (OpCodes.Ret));

					// See the comment in CecilExtensions.FinalizeGeneratedBody for why this is needed.
					method.Body.FinalizeGeneratedBody ();

					modified = true;
					return modified; // we replace the whole method body, so no need to continue processing the method
				}
			}

			switch (mr.Name) {
			case "_dlopen": // nothing to inline here
			case "dlopen": // nothing to inline here
			case "dlerror": // nothing to inline here
				continue;
			case "dlclose":
				// It might be possible to just remove these calls, because
				// (PENDING CONFIRMATION) I believe dlclose is a no-op on at least some Apple platforms.
				continue;
			default:
				App.Log (3, $"Unknown or unsupported Dlfcn pattern: '{FormatMethod (mr)}' in method '{FormatMethod (method)}'. The call will not be inlined.");
				continue;
			}
		}

		return modified;
	}

	static bool IsGeneratedCachePointerMethod (MethodDefinition method, [NotNullWhen (true)] out string? fieldName, [NotNullWhen (false)] out string? failureMessage)
	{
		fieldName = null;
		failureMessage = null;

		// The following code:
		//
		// 	get {
		// 		fixed (IntPtr *storage = &values [8])
		// 			return Dlfcn.CachePointer (Libraries.XYZ.Handle, "...", storage);
		// 	}
		//
		// has the following IL sequence:
		//
		// 	IL_0000: ldsfld System.IntPtr[] <type>::values
		// 	IL_0005: ldc.i4.0
		// 	IL_0006: ldelema System.IntPtr
		// 	IL_000b: stloc.1
		// 	IL_000c: ldloc.1
		// 	IL_000d: conv.u
		// 	IL_000e: stloc.0
		// 	IL_000f: ldsfld System.IntPtr ObjCRuntime.Libraries/<type>::Handle
		// 	IL_0014: ldstr "FIELDNAME"
		// 	IL_0019: ldloc.0
		// 	IL_001a: call System.IntPtr ObjCRuntime.Dlfcn::CachePointer(System.IntPtr,System.String,System.IntPtr*)
		//    IL_0020: stloc.2
		//    IL_0021: br.s IL_0023
		//    IL_0023: ldloc.2
		// 	IL_001f: ret
		//
		// (the indented code can happen for debug builds)
		//
		if (!method.HasBody) {
			failureMessage = "Method has no body";
			return false;
		}
		var body = method.Body;
		if (body.Instructions.Count == 0) {
			failureMessage = "Method has no instructions";
			return false;
		}

		var instr = body.Instructions.First ();
		var isLast = false;

		bool AssertOpCode ([NotNullWhen (false)] out string? failureMessage, params OpCode [] expected)
		{
			failureMessage = null;

			while (instr.OpCode == OpCodes.Nop && instr.Next is not null)
				instr = instr.Next;

			if (!expected.Any (v => v == instr.OpCode)) {
				failureMessage = $"Expected any of '{string.Join (", ", expected.Select (v => v.ToString ()))}' as instruction at offset IL{instr.Offset:X4}, got: {instr}";
				return false;
			}

			if (instr.Next is null) {
				if (isLast)
					return true;
				failureMessage = $"Expected more instructions after {instr}.";
				return false;
			}

			if (isLast) {
				failureMessage = $"Got more instructions than expected after {instr}.";
				return false;
			}

			instr = instr.Next;

			return true;
		}

		if (!AssertOpCode (out failureMessage, OpCodes.Ldsfld))
			return false;

		var ldcOpcodes = new OpCode [] { OpCodes.Ldc_I4_0, OpCodes.Ldc_I4_1, OpCodes.Ldc_I4_2, OpCodes.Ldc_I4_3, OpCodes.Ldc_I4_4, OpCodes.Ldc_I4_5, OpCodes.Ldc_I4_6, OpCodes.Ldc_I4_7, OpCodes.Ldc_I4_8, OpCodes.Ldc_I4, OpCodes.Ldc_I4_S };
		if (!AssertOpCode (out failureMessage, ldcOpcodes))
			return false;

		if (!AssertOpCode (out failureMessage, OpCodes.Ldelema))
			return false;

		var stlocOpcodes = new OpCode [] { OpCodes.Stloc_0, OpCodes.Stloc_1, OpCodes.Stloc_2, OpCodes.Stloc_3, OpCodes.Stloc, OpCodes.Stloc_S };
		if (!AssertOpCode (out failureMessage, stlocOpcodes))
			return false;

		var ldlocOpcodes = new OpCode [] { OpCodes.Ldloc_0, OpCodes.Ldloc_1, OpCodes.Ldloc_2, OpCodes.Ldloc_3, OpCodes.Ldloc, OpCodes.Ldloc_S };
		if (!AssertOpCode (out failureMessage, ldlocOpcodes))
			return false;

		if (!AssertOpCode (out failureMessage, OpCodes.Conv_U))
			return false;

		if (!AssertOpCode (out failureMessage, stlocOpcodes))
			return false;

		if (!AssertOpCode (out failureMessage, OpCodes.Ldsfld))
			return false;

		if (!AssertOpCode (out failureMessage, OpCodes.Ldstr))
			return false;
		fieldName = (string) instr.Previous.Operand;

		if (!AssertOpCode (out failureMessage, ldlocOpcodes))
			return false;

		if (!AssertOpCode (out failureMessage, OpCodes.Call))
			return false;

		if (stlocOpcodes.Any (v => v == instr.OpCode)) {
			if (!AssertOpCode (out failureMessage, stlocOpcodes))
				return false;

			var branchOpcodes = new OpCode [] { OpCodes.Br, OpCodes.Br_S };
			if (!AssertOpCode (out failureMessage, branchOpcodes))
				return false;

			if (!AssertOpCode (out failureMessage, ldlocOpcodes))
				return false;
		}

		isLast = true;
		return AssertOpCode (out failureMessage, OpCodes.Ret);
	}

	static string FormatMethod (MethodReference method)
	{
		var rv = method.FullName;
		var idx = rv.IndexOf (' ');
		if (idx > 0)
			rv = rv.Substring (idx + 1);
		return rv;
	}

	static bool IsNSObjectSubclass (TypeReference type)
	{
		var resolved = type.Resolve ();
		while (resolved is not null) {
			if (resolved.FullName == "Foundation.NSObject")
				return true;
			if (resolved.BaseType is null)
				break;
			resolved = resolved.BaseType.Resolve ();
		}
		return false;
	}
}
