using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

using Xamarin.Bundler;
using Xamarin.Utils;

using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Linker;
using Mono.Tuner;

using Registrar;
using System.Globalization;

#nullable enable

namespace Xamarin.Linker {
	// This type will generate the lookup code to:
	// * Convert between types and their type IDs.
	// * Map between a protocol interface and its wrapper type.
	// * Find the UnmanagedCallersOnly method for a given method ID.
	// This must be done after the linker has trimmed away any unused code,
	// because otherwise the lookup code would reference (and thus keep)
	// every exported type and method.
	public class ManagedRegistrarLookupTablesStep : ConfigurationAwareStep {
		class TypeData {
			public TypeReference Reference;
			public TypeDefinition Definition;

			public TypeData (TypeReference reference, TypeDefinition definition)
			{
				Reference = reference;
				Definition = definition;
			}
		}

		protected override string Name { get; } = "ManagedRegistrarLookupTables";
		protected override int ErrorCode { get; } = 2440;

		AppBundleRewriter abr { get { return Configuration.AppBundleRewriter; } }

		protected override void TryProcessAssembly (AssemblyDefinition assembly)
		{
			base.TryProcessAssembly (assembly);

			if (App.Registrar != RegistrarMode.ManagedStatic)
				return;

			var annotation = DerivedLinkContext.Annotations.GetCustomAnnotation ("ManagedRegistrarStep", assembly);
			var info = annotation as AssemblyTrampolineInfo;
			if (info is null)
				return;

			abr.SetCurrentAssembly (assembly);

			CreateRegistrarType (info);

			abr.ClearCurrentAssembly ();
		}

		void CreateRegistrarType (AssemblyTrampolineInfo info)
		{
			var registrarType = new TypeDefinition ("ObjCRuntime", "__Registrar__", TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit);
			registrarType.BaseType = abr.System_Object;
			registrarType.Interfaces.Add (new InterfaceImplementation (abr.ObjCRuntime_IManagedRegistrar));
			abr.CurrentAssembly.MainModule.Types.Add (registrarType);

			info.RegistrarType = registrarType;

			// Remove any methods that were linked away
			for (var i = info.Count - 1; i >= 0; i--) {
				var tinfo = info [i];
				if (IsTrimmed (tinfo.Target))
					info.RemoveAt (i);
			}

			info.SetIds ();
			var sorted = info.OrderBy (v => v.Id).ToList ();

			//
			// The callback methods themselves are all public, and thus accessible from anywhere inside
			// the assembly even if the containing type is not public, as long as the containing type is not nested.
			// However, if the containing type is nested inside another type, it gets complicated.
			//
			// We have two options:
			// 
			// 1. Just change the visibility on the nested type to make it visible inside the assembly.
			// 2. Add a method in the containing type (which has access to any directly nested private types) that can look up any unmanaged trampolines.
			//    If the containing type is also a private nested type, when we'd have to add another method in its containing type, and so on.
			//
			// The second option is more complicated to implement than the first, so we're doing the first option. If someone
			// runs into any problems (there might be with reflection: looking up a type using the wrong visibility will fail to find that type).
			// That said, there may be all sorts of problems with reflection (we're adding methods to types,
			// any logic that depends on a type having a certain number of methods will fail for instance).
			//
			foreach (var md in sorted) {
				var declType = md.Trampoline.DeclaringType;
				while (declType.IsNested) {
					if (declType.IsNestedPrivate) {
						declType.IsNestedAssembly = true;
					} else if (declType.IsNestedFamilyAndAssembly || declType.IsNestedFamily) {
						declType.IsNestedFamilyOrAssembly = true;
					}
					declType = declType.DeclaringType;
				}
			}

			// Add default ctor that just calls the base ctor
			var defaultCtor = registrarType.AddDefaultConstructor (abr);

			// Create an instance of the registrar type in the module constructor,
			// and call ObjCRuntime.RegistrarHelper.Register with the instance.
			AddLoadTypeToModuleConstructor (registrarType);

			// Compute the list of types that we need to register
			var types = GetTypesToRegister (registrarType, info);

			GenerateLookupUnmanagedFunction (registrarType, sorted);
			GenerateLookupType (info, registrarType, types);
			GenerateLookupTypeId (info, registrarType, types);
			GenerateRegisterWrapperTypes (registrarType);
			GenerateConstructNSObject (registrarType);
			GenerateConstructINativeObject (registrarType);

			// Make sure the linker doesn't sweep away anything we just generated.
			Annotations.Mark (registrarType);
			foreach (var method in registrarType.Methods)
				Annotations.Mark (method);
			foreach (var iface in registrarType.Interfaces) {
				Annotations.Mark (iface);
				Annotations.Mark (iface.InterfaceType);
				Annotations.Mark (iface.InterfaceType.Resolve ());
			}
		}

		void AddLoadTypeToModuleConstructor (TypeDefinition registrarType)
		{
			// Add a module constructor to initialize the callbacks
			var moduleType = registrarType.Module.Types.SingleOrDefault (v => v.Name == "<Module>");
			if (moduleType is null)
				throw ErrorHelper.CreateError (99, $"No <Module> type found in {registrarType.Module.Name}");
			var moduleConstructor = moduleType.GetTypeConstructor ();
			MethodBody body;
			ILProcessor il;
			if (moduleConstructor is null) {
				moduleConstructor = moduleType.AddMethod (".cctor", MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.RTSpecialName | MethodAttributes.SpecialName | MethodAttributes.Static, abr.System_Void);
				body = moduleConstructor.CreateBody (out il);
				il.Emit (OpCodes.Ret);
			} else {
				body = moduleConstructor.Body;
				il = body.GetILProcessor ();
			}
			var lastInstruction = body.Instructions.Last ();

			il.InsertBefore (lastInstruction, il.Create (OpCodes.Newobj, registrarType.GetDefaultInstanceConstructor ()));
			il.InsertBefore (lastInstruction, il.Create (OpCodes.Call, abr.RegistrarHelper_Register));
		}

		List<TypeData> GetTypesToRegister (TypeDefinition registrarType, AssemblyTrampolineInfo info)
		{
			// Compute the list of types that we need to register
			var types = new List<TypeData> ();

			// We want all the types that have been registered
			types.AddRange (StaticRegistrar.Types.Select (v => {
				var tr = v.Value.Type;
				var td = tr.Resolve ();
				return new TypeData (tr, td);
			}));

			// We also want all the types the registrar skipped (otherwise we won't be able to generate the corresponding table of skipped types).
			foreach (var st in StaticRegistrar.SkippedTypes) {
				if (!types.Any (v => v.Reference == st.Skipped))
					types.Add (new (st.Skipped, st.Skipped.Resolve ()));
				if (!types.Any (v => v.Reference == st.Actual.Type))
					types.Add (new (st.Actual.Type, st.Actual.Type.Resolve ()));
			}

			// Skip any types that are not defined in the current assembly
			types.RemoveAll (v => v.Definition.Module.Assembly != abr.CurrentAssembly);

			// Skip any types that have been linked away
			types.RemoveAll (v => IsTrimmed (v.Definition));

			// We also want all the protocol wrapper types
			foreach (var type in registrarType.Module.Types) {
				if (IsTrimmed (type))
					continue;
				var wrapperType = StaticRegistrar.GetProtocolAttributeWrapperType (type);
				if (wrapperType is null)
					continue;
				types.Add (new (wrapperType, wrapperType.Resolve ()));
			}

			// Now create a mapping from type to index
			for (var i = 0; i < types.Count; i++)
				info.RegisterType (types [i].Definition, (uint) i);

			return types;
		}

		IEnumerable<TypeDefinition> GetRelevantTypes (Func<TypeDefinition, bool> isRelevant)
			=> StaticRegistrar.GetAllTypes (abr.CurrentAssembly)
				.Cast<TypeDefinition> ()
				.Where (type => !IsTrimmed (type))
				.Where (type => type.Module.Assembly == abr.CurrentAssembly)
				.Where (isRelevant);

		bool IsTrimmed (MemberReference type)
		{
			return StaticRegistrar.IsTrimmed (type, Annotations);
		}

		void GenerateLookupTypeId (AssemblyTrampolineInfo infos, TypeDefinition registrarType, List<TypeData> types)
		{
			var lookupTypeMethod = registrarType.AddMethod ("LookupTypeId", MethodAttributes.Private | MethodAttributes.Final | MethodAttributes.Virtual | MethodAttributes.NewSlot | MethodAttributes.HideBySig, abr.System_UInt32);
			var handleParameter = lookupTypeMethod.AddParameter ("handle", abr.System_RuntimeTypeHandle);
			lookupTypeMethod.Overrides.Add (abr.IManagedRegistrar_LookupTypeId);
			var body = lookupTypeMethod.CreateBody (out var il);

			// The current implementation does something like this:
			//
			// if (RuntimeTypeHandle.Equals (handle, <ldtoken TypeA>)
			//     return 0;
			// if (RuntimeTypeHandle.Equals (handle, <ldtoken TypeB>)
			//     return 1;
			//
			// This can potentially be improved to do a dictionary lookup. The downside would be higher memory usage
			// (a simple implementation that's just a series of if conditions doesn't consume any dirty memory).
			// One idea could be to use a dictionary lookup if we have more than X types, and then fall back to the 
			// linear search otherwise.

			for (var i = 0; i < types.Count; i++) {
				il.Emit (OpCodes.Ldarga_S, handleParameter);
				il.Emit (OpCodes.Ldtoken, types [i].Reference);
				il.Emit (OpCodes.Call, abr.RuntimeTypeHandle_Equals);
				var falseTarget = il.Create (OpCodes.Nop);
				il.Emit (OpCodes.Brfalse_S, falseTarget);
				il.Emit (OpCodes.Ldc_I4, i);
				il.Emit (OpCodes.Ret);
				il.Append (falseTarget);
			}

			// No match, return -1
			il.Emit (OpCodes.Ldc_I4_M1);
			il.Emit (OpCodes.Ret);
		}

		void GenerateLookupType (AssemblyTrampolineInfo infos, TypeDefinition registrarType, List<TypeData> types)
		{
			var lookupTypeMethod = registrarType.AddMethod ("LookupType", MethodAttributes.Private | MethodAttributes.Final | MethodAttributes.Virtual | MethodAttributes.NewSlot | MethodAttributes.HideBySig, abr.System_RuntimeTypeHandle);
			lookupTypeMethod.AddParameter ("id", abr.System_UInt32);
			lookupTypeMethod.Overrides.Add (abr.IManagedRegistrar_LookupType);
			var body = lookupTypeMethod.CreateBody (out var il);

			// We know all the IDs are contiguous, so we can just do a switch statement.
			// 
			// The current implementation does something like this:
			//     switch (id) {
			//     case 0: return <ldtoken TYPE1>;
			//     case 1: return <ldtoken TYPE2>;
			//     default: return default (RuntimeTypeHandle);
			//     }

			var targets = new Instruction [types.Count];

			for (var i = 0; i < targets.Length; i++) {
				targets [i] = Instruction.Create (OpCodes.Ldtoken, types [i].Reference);
				var td = types [i].Definition;
				if (IsTrimmed (td))
					throw ErrorHelper.CreateError (99, $"Trying to add the type {td.FullName} to the registrar's lookup tables, but it's been trimmed away.");
			}

			il.Emit (OpCodes.Ldarg_1);
			il.Emit (OpCodes.Switch, targets);
			for (var i = 0; i < targets.Length; i++) {
				il.Append (targets [i]);
				il.Emit (OpCodes.Ret);
			}

			// return default (RuntimeTypeHandle)
			var temporary = body.AddVariable (abr.System_RuntimeTypeHandle);
			il.Emit (OpCodes.Ldloca, temporary);
			il.Emit (OpCodes.Initobj, abr.System_RuntimeTypeHandle);
			il.Emit (OpCodes.Ldloc, temporary);
			il.Emit (OpCodes.Ret);
		}

		void GenerateConstructNSObject (TypeDefinition registrarType)
		{
			var createInstanceMethod = registrarType.AddMethod ("ConstructNSObject", MethodAttributes.Private | MethodAttributes.Final | MethodAttributes.Virtual | MethodAttributes.NewSlot | MethodAttributes.HideBySig, abr.ObjCRuntime_INativeObject);
			var typeHandleParameter = createInstanceMethod.AddParameter ("typeHandle", abr.System_RuntimeTypeHandle);
			var nativeHandleParameter = createInstanceMethod.AddParameter ("nativeHandle", abr.ObjCRuntime_NativeHandle);
			createInstanceMethod.Overrides.Add (abr.IManagedRegistrar_ConstructNSObject);
			var body = createInstanceMethod.CreateBody (out var il);

			// We generate something like this:
			// if (RuntimeTypeHandle.Equals (typeHandle, typeof (TypeA).TypeHandle))
			//     return new TypeA (nativeHandle);
			// if (RuntimeTypeHandle.Equals (typeHandle, typeof (TypeB).TypeHandle))
			//     return new TypeB (nativeHandle);
			// return null;

			var types = GetRelevantTypes (type => type.IsNSObject (DerivedLinkContext) && !type.IsAbstract && !type.IsInterface);

			foreach (var type in types) {
				var ctorRef = FindNSObjectConstructor (type);
				if (ctorRef is null) {
					Driver.Log (9, $"Cannot include {type.FullName} in ConstructNSObject because it doesn't have a suitable constructor");
					continue;
				}

				var ctor = abr.CurrentAssembly.MainModule.ImportReference (ctorRef);
				if (IsTrimmed (ctor))
					Annotations.Mark (ctor.Resolve ());

				// We can only add a type to the table if it's not an open type.
				if (!ManagedRegistrarStep.IsOpenType (type)) {
					EnsureVisible (createInstanceMethod, ctor);

					il.Emit (OpCodes.Ldarga_S, typeHandleParameter);
					il.Emit (OpCodes.Ldtoken, type);
					il.Emit (OpCodes.Call, abr.RuntimeTypeHandle_Equals);
					var falseTarget = il.Create (OpCodes.Nop);
					il.Emit (OpCodes.Brfalse_S, falseTarget);

					il.Emit (OpCodes.Ldarg, nativeHandleParameter);
					if (ctor.Parameters [0].ParameterType.Is ("System", "IntPtr"))
						il.Emit (OpCodes.Call, abr.NativeObject_op_Implicit_IntPtr);
					il.Emit (OpCodes.Newobj, ctor);
					il.Emit (OpCodes.Ret);

					il.Append (falseTarget);
				}

				// In addition to the big lookup method, implement the static factory method on the type:
				ImplementConstructNSObjectFactoryMethod (type, ctor);
			}

			// return default (NSObject);
			il.Emit (OpCodes.Ldnull);
			il.Emit (OpCodes.Ret);
		}

		void GenerateConstructINativeObject (TypeDefinition registrarType)
		{
			var createInstanceMethod = registrarType.AddMethod ("ConstructINativeObject", MethodAttributes.Private | MethodAttributes.Final | MethodAttributes.Virtual | MethodAttributes.NewSlot | MethodAttributes.HideBySig, abr.ObjCRuntime_INativeObject);
			var typeHandleParameter = createInstanceMethod.AddParameter ("typeHandle", abr.System_RuntimeTypeHandle);
			var nativeHandleParameter = createInstanceMethod.AddParameter ("nativeHandle", abr.ObjCRuntime_NativeHandle);
			var ownsParameter = createInstanceMethod.AddParameter ("owns", abr.System_Boolean);
			createInstanceMethod.Overrides.Add (abr.IManagedRegistrar_ConstructINativeObject);
			var body = createInstanceMethod.CreateBody (out var il);

			// We generate something like this:
			// if (RuntimeTypeHandle.Equals (typeHandle, typeof (TypeA).TypeHandle))
			//     return new TypeA (nativeHandle, owns);
			// if (RuntimeTypeHandle.Equals (typeHandle, typeof (TypeB).TypeHandle))
			//     return new TypeB (nativeHandle, owns);
			// return null;

			var types = GetRelevantTypes (type => type.IsNativeObject () && !type.IsAbstract && !type.IsInterface);

			foreach (var type in types) {
				var ctorRef = FindINativeObjectConstructor (type);

				if (ctorRef is not null) {
					var ctor = abr.CurrentAssembly.MainModule.ImportReference (ctorRef);

					// we need to preserve the constructor because it might not be used anywhere else
					if (IsTrimmed (ctor))
						Annotations.Mark (ctor.Resolve ());

					if (!ManagedRegistrarStep.IsOpenType (type)) {
						EnsureVisible (createInstanceMethod, ctor);

						il.Emit (OpCodes.Ldarga_S, typeHandleParameter);
						il.Emit (OpCodes.Ldtoken, type);
						il.Emit (OpCodes.Call, abr.RuntimeTypeHandle_Equals);
						var falseTarget = il.Create (OpCodes.Nop);
						il.Emit (OpCodes.Brfalse_S, falseTarget);

						il.Emit (OpCodes.Ldarg, nativeHandleParameter);
						if (ctor.Parameters [0].ParameterType.Is ("System", "IntPtr"))
							il.Emit (OpCodes.Call, abr.NativeObject_op_Implicit_IntPtr);
						il.Emit (OpCodes.Ldarg, ownsParameter);
						il.Emit (OpCodes.Newobj, ctor);
						il.Emit (OpCodes.Ret);

						il.Append (falseTarget);
					}
				}

				// In addition to the big lookup method, implement the static factory method on the type:
				ImplementConstructINativeObjectFactoryMethod (type, ctorRef);
			}

			// return default (NSObject)
			il.Emit (OpCodes.Ldnull);
			il.Emit (OpCodes.Ret);
		}

		void ImplementConstructNSObjectFactoryMethod (TypeDefinition type, MethodReference ctor)
		{
			// skip creating the factory for NSObject itself
			if (type.Is ("Foundation", "NSObject"))
				return;

			var createInstanceMethod = type.AddMethod ("_Xamarin_ConstructNSObject", MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.NewSlot | MethodAttributes.HideBySig, abr.Foundation_NSObject);
			var nativeHandleParameter = createInstanceMethod.AddParameter ("nativeHandle", abr.ObjCRuntime_NativeHandle);
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

			Annotations.Mark (createInstanceMethod);
		}

		void ImplementConstructINativeObjectFactoryMethod (TypeDefinition type, MethodReference? ctor)
		{
			// skip creating the factory for NSObject itself
			if (type.Is ("Foundation", "NSObject"))
				return;

			// If the type is a subclass of NSObject, we prefer the NSObject "IntPtr" constructor
			var nsobjectConstructor = type.IsNSObject (DerivedLinkContext) ? FindNSObjectConstructor (type) : null;
			if (nsobjectConstructor is null && ctor is null)
				return;

			var createInstanceMethod = type.AddMethod ("_Xamarin_ConstructINativeObject", MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.NewSlot | MethodAttributes.HideBySig, abr.ObjCRuntime_INativeObject);
			var nativeHandleParameter = createInstanceMethod.AddParameter ("nativeHandle", abr.ObjCRuntime_NativeHandle);
			var ownsParameter = createInstanceMethod.AddParameter ("owns", abr.System_Boolean);
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

			Annotations.Mark (createInstanceMethod);
		}

		static MethodReference? FindNSObjectConstructor (TypeDefinition type)
		{
			return FindConstructorWithOneParameter ("ObjCRuntime", "NativeHandle")
				?? FindConstructorWithOneParameter ("System", "IntPtr");

			MethodReference? FindConstructorWithOneParameter (string ns, string cls)
				=> type.Methods.SingleOrDefault (method =>
					method.IsConstructor
						&& !method.IsStatic
						&& method.HasParameters
						&& method.Parameters.Count == 1
						&& method.Parameters [0].ParameterType.Is (ns, cls));
		}


		static MethodReference? FindINativeObjectConstructor (TypeDefinition type)
		{
			return FindConstructorWithTwoParameters ("ObjCRuntime", "NativeHandle", "System", "Boolean")
				?? FindConstructorWithTwoParameters ("System", "IntPtr", "System", "Boolean");

			MethodReference? FindConstructorWithTwoParameters (string ns1, string cls1, string ns2, string cls2)
				=> type.Methods.SingleOrDefault (method =>
					method.IsConstructor
						&& !method.IsStatic
						&& method.HasParameters
						&& method.Parameters.Count == 2
						&& method.Parameters [0].ParameterType.Is (ns1, cls1)
						&& method.Parameters [1].ParameterType.Is (ns2, cls2));
		}

		void GenerateRegisterWrapperTypes (TypeDefinition type)
		{
			var method = type.AddMethod ("RegisterWrapperTypes", MethodAttributes.Private | MethodAttributes.Final | MethodAttributes.Virtual | MethodAttributes.NewSlot | MethodAttributes.HideBySig, abr.System_Void);
			method.AddParameter ("type", abr.System_Collections_Generic_Dictionary2.CreateGenericInstanceType (abr.System_RuntimeTypeHandle, abr.System_RuntimeTypeHandle));
			method.Overrides.Add (abr.IManagedRegistrar_RegisterWrapperTypes);
			method.CreateBody (out var il);

			// Find all the protocol interfaces that are defined in the current assembly, and their corresponding wrapper type,
			// and add the pair to the dictionary.
			var addMethodReference = abr.System_Collections_Generic_Dictionary2.CreateMethodReferenceOnGenericType (abr.Dictionary2_Add, abr.System_RuntimeTypeHandle, abr.System_RuntimeTypeHandle);
			var currentTypes = StaticRegistrar.Types.Where (v => v.Value.Type.Resolve ().Module.Assembly == abr.CurrentAssembly);
			foreach (var ct in currentTypes) {
				if (!ct.Value.IsProtocol)
					continue;
				if (ct.Value.ProtocolWrapperType is null)
					continue;

				// If both the protocol interface type and the wrapper type are trimmed away, skip them.
				var keyMarked = !IsTrimmed (ct.Key.Resolve ());
				var wrapperTypeMarked = !IsTrimmed (ct.Value.ProtocolWrapperType.Resolve ());
				if (!keyMarked && !wrapperTypeMarked)
					continue;
				// If only one of them is trimmed, throw an error
				if (keyMarked ^ wrapperTypeMarked)
					throw ErrorHelper.CreateError (99, $"Mismatched trimming results between the protocol interface {ct.Key.FullName} (trimmed: {!keyMarked}) and its wrapper type {ct.Value.ProtocolWrapperType.FullName} (trimmed: {!wrapperTypeMarked})");

				il.Emit (OpCodes.Ldarg_1);
				il.Emit (OpCodes.Ldtoken, type.Module.ImportReference (ct.Key));
				il.Emit (OpCodes.Ldtoken, type.Module.ImportReference (ct.Value.ProtocolWrapperType));
				il.Emit (OpCodes.Call, addMethodReference);
			}

			il.Emit (OpCodes.Ret);
		}

		void GenerateLookupUnmanagedFunction (TypeDefinition registrar_type, IList<TrampolineInfo> trampolineInfos)
		{
			MethodDefinition? lookupMethods = null;
			if (App.IsAOTCompiled (abr.CurrentAssembly.Name.Name)) {
				// Don't generate lookup code, because native code will call the EntryPoint for the UnmanagedCallerOnly methods directly.
				Driver.Log (9, $"Not generating method lookup code for {abr.CurrentAssembly.Name.Name}, because it's AOT compiled");
			} else if (trampolineInfos.Count > 0) {
				// All the methods in a given assembly will have consecutive IDs (but might not start at 0).
				if (trampolineInfos.First ().Id + trampolineInfos.Count - 1 != trampolineInfos.Last ().Id)
					throw ErrorHelper.CreateError (99, $"Invalid ID range: {trampolineInfos.First ().Id} + {trampolineInfos.Count - 1} != {trampolineInfos.Last ().Id}");

				const int methodsPerLevel = 10;
				var levels = (int) Math.Ceiling (Math.Log (trampolineInfos.Count, methodsPerLevel));
				levels = levels == 0 ? 1 : levels;
				GenerateLookupMethods (registrar_type, trampolineInfos, methodsPerLevel, 1, levels, 0, trampolineInfos.Count - 1, out lookupMethods);
			}

			var method = registrar_type.AddMethod ("LookupUnmanagedFunction", MethodAttributes.Private | MethodAttributes.Final | MethodAttributes.Virtual | MethodAttributes.NewSlot | MethodAttributes.HideBySig, abr.System_IntPtr);
			method.AddParameter ("symbol", abr.System_String);
			method.AddParameter ("id", abr.System_Int32);
			method.Overrides.Add (abr.IManagedRegistrar_LookupUnmanagedFunction);
			method.CreateBody (out var il);
			if (lookupMethods is null) {
				il.Emit (OpCodes.Ldc_I4_M1);
				il.Emit (OpCodes.Conv_I);
			} else {
				il.Emit (OpCodes.Ldarg_1);
				il.Emit (OpCodes.Ldarg_2);
				il.Emit (OpCodes.Call, lookupMethods);
			}
			il.Emit (OpCodes.Ret);
		}

		// If WrappedLook is true we'll wrap the ldftn instruction in a separate method, which can be useful for debugging,
		// because if there's a problem with the IL in the lookup method, it can be hard to figure out which ldftn
		// instruction is causing the problem (because the JIT will just fail when compiling the method, not necessarily
		// saying which instruction is broken).
		static bool? wrappedLookup;
		static bool WrappedLookup {
			get {
				if (!wrappedLookup.HasValue)
					wrappedLookup = !string.IsNullOrEmpty (Environment.GetEnvironmentVariable ("XAMARIN_MSR_WRAPPED_LOOKUP"));
				return wrappedLookup.Value;
			}
		}
		MethodDefinition GenerateLookupMethods (TypeDefinition type, IList<TrampolineInfo> trampolineInfos, int methodsPerLevel, int level, int levels, int startIndex, int endIndex, out MethodDefinition method)
		{
			if (startIndex > endIndex)
				throw ErrorHelper.CreateError (99, $"startIndex ({startIndex}) can't be higher than endIndex ({endIndex})");

			var startId = trampolineInfos [startIndex].Id;
			var name = level == 1 ? "LookupUnmanagedFunctionImpl" : $"LookupUnmanagedFunction_{level}_{levels}__{startIndex}_{endIndex}__";
			method = type.AddMethod (name, MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.Static, abr.System_IntPtr);
			method.AddParameter ("symbol", abr.System_String);
			method.AddParameter ("id", abr.System_Int32);
			method.CreateBody (out var il);

			if (level == levels) {
				// This is the leaf method where we do the actual lookup.
				//
				// The code is something like this:
				// switch (id - <startId>) {
				// case 0: return <ldftn for first method>;
				// case 1: return <ldftn for second method>;
				// case <methodsPerLevel - 1>: return <ldftn for last method>;
				// default: return -1;
				// }
				var targetCount = endIndex - startIndex + 1;
				var targets = new Instruction [targetCount];
				for (var i = 0; i < targets.Length; i++) {
					var ti = trampolineInfos [startIndex + i];
					var md = ti.Trampoline;
					var mr = abr.CurrentAssembly.MainModule.ImportReference (md);
					if (WrappedLookup) {
						var wrappedLookup = type.AddMethod (name + ti.Id, MethodAttributes.Private | MethodAttributes.Static | MethodAttributes.HideBySig, abr.System_IntPtr);
						wrappedLookup.CreateBody (out var wrappedIl);
						wrappedIl.Emit (OpCodes.Ldftn, mr);
						wrappedIl.Emit (OpCodes.Ret);

						targets [i] = Instruction.Create (OpCodes.Call, wrappedLookup);
					} else {
						targets [i] = Instruction.Create (OpCodes.Ldftn, mr);
					}
				}

				il.Emit (OpCodes.Ldarg_1);
				if (startId != 0) {
					il.Emit (OpCodes.Ldc_I4, startId);
					il.Emit (OpCodes.Sub_Ovf_Un);
				}
				il.Emit (OpCodes.Switch, targets);
				for (var k = 0; k < targetCount; k++) {
					il.Append (targets [k]);
					il.Emit (OpCodes.Ret);
				}
			} else {
				// This is an intermediate method to not have too many ldftn instructions in a single method (it takes a long time to JIT).
				var chunkSize = (int) Math.Pow (methodsPerLevel, levels - level);

				// Some validation
				if (level == 1) {
					if (chunkSize * methodsPerLevel < trampolineInfos.Count)
						throw ErrorHelper.CreateError (99, $"chunkSize ({chunkSize}) * methodsPerLevel ({methodsPerLevel}) < trampolineInfos.Count {trampolineInfos.Count}");
				}

				// The code is something like this:
				// switch ((id - <startId>) / <chunkSize>) {
				// case 0: return Lookup_1_2__0_10__ (symbol, id);
				// case 1: return Lookup_1_2__11_20__ (symbol, id);
				// case ...
				// default: return -1;
				// }

				var count = endIndex - startIndex + 1;
				var chunks = (int) Math.Ceiling (count / (double) chunkSize);
				var targets = new Instruction [chunks];

				var lookupMethods = new MethodDefinition [targets.Length];
				for (var i = 0; i < targets.Length; i++) {
					var subStartIndex = startIndex + (chunkSize) * i;
					var subEndIndex = subStartIndex + (chunkSize) - 1;
					if (subEndIndex > endIndex)
						subEndIndex = endIndex;
					var md = GenerateLookupMethods (type, trampolineInfos, methodsPerLevel, level + 1, levels, subStartIndex, subEndIndex, out _);
					lookupMethods [i] = md;
					targets [i] = Instruction.Create (OpCodes.Ldarg_0);
				}

				il.Emit (OpCodes.Ldarg_1);
				if (startId != 0) {
					il.Emit (OpCodes.Ldc_I4, startId);
					il.Emit (OpCodes.Sub_Ovf_Un);
				}
				il.Emit (OpCodes.Ldc_I4, chunkSize);
				il.Emit (OpCodes.Div);
				il.Emit (OpCodes.Switch, targets);
				for (var k = 0; k < targets.Length; k++) {
					il.Append (targets [k]); // OpCodes.Ldarg_0
					il.Emit (OpCodes.Ldarg_1);
					il.Emit (OpCodes.Call, lookupMethods [k]);
					il.Emit (OpCodes.Ret);
				}
			}

			// no hit? this shouldn't happen
			il.Emit (OpCodes.Ldc_I4_M1);
			il.Emit (OpCodes.Conv_I);
			il.Emit (OpCodes.Ret);

			return method;
		}

		static string GetMethodSignature (MethodDefinition method)
		{
			return $"{method?.ReturnType?.FullName ?? "(null)"} {method?.DeclaringType?.FullName ?? "(null)"}::{method?.Name ?? "(null)"} ({string.Join (", ", method?.Parameters?.Select (v => v?.ParameterType?.FullName + " " + v?.Name) ?? Array.Empty<string> ())})";
		}

		static void EnsureVisible (MethodDefinition caller, MethodReference methodRef)
		{
			var method = methodRef.Resolve ();
			var type = method.DeclaringType.Resolve ();
			if (type.IsNested) {
				if (!method.IsPublic) {
					method.IsFamilyOrAssembly = true;
				}

				EnsureVisible (caller, type);
			} else if (!method.IsPublic) {
				method.IsFamilyOrAssembly = true;
			}
		}

		static void EnsureVisible (MethodDefinition caller, TypeReference typeRef)
		{
			var type = typeRef.Resolve ();
			if (type.IsNested) {
				if (!type.IsNestedPublic) {
					type.IsNestedAssembly = true;
				}

				EnsureVisible (caller, type.DeclaringType);
			} else if (!type.IsPublic) {
				type.IsNotPublic = true;
			}
		}

		StaticRegistrar StaticRegistrar {
			get { return DerivedLinkContext.StaticRegistrar; }
		}
	}
}

