using System;
using System.IO;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.MaciOS.Nnyeah.AssemblyComparator;

namespace Microsoft.MaciOS.Nnyeah {

	public class Reworker {
		const string NetCoreAppDependency = "NETCoreApp,Version=v6.0";
		// Module does not copy it's input stream, so we'll keep it referenced here
		// to prevent 'Cannot access a closed file' crashes with Cecil
		FileStream Stream;
		ModuleContainer Modules;
		ModuleDefinition ModuleToEdit => Modules.ModuleToEdit;

		TypeDefinition EmbeddedAttributeTypeDef;
		TypeDefinition NativeIntegerAttributeTypeDef;
		TypeReference NativeIntegerAttributeTypeRef;
		MethodReference NativeIntegerCtorNoArgsRef;
		MethodReference NativeIntegerCtorOneArgRef;
		TypeReference CompilerGeneratedAttributeTypeRef;
		MethodReference CompilerGeneratedCtorRef;
		TypeReference EmbeddedAttributeTypeRef;
		MethodReference EmbeddedAttributeCtorRef;
		TypeReference NintTypeReference;
		TypeReference NuintTypeReference;
		TypeReference NfloatTypeReference;
		TypeReference NewNfloatTypeReference;
		TypeDefinition NewNativeHandleTypeDefinition;
		TypeReference AttributeTypeReference;
		TypeReference AttributeTargetsTypeReference;
		TypeReference AttributeUsageTypeReference;
		AssemblyNameReference InteropServicesAssembly;
		MethodReference NativeHandleGetHandleReference;

		TypeAndMemberMap ModuleMap;

		Dictionary<string, Transformation> MethodSubs;
		Dictionary<string, Transformation> FieldSubs;
		ConstructorTransforms ConstructorTransforms;

		public event EventHandler<WarningEventArgs>? WarningIssued;
		public event EventHandler<TransformEventArgs>? Transformed;

		public static bool NeedsReworking (ModuleDefinition module)
		{
			// simple predicate for seeing if there any references
			// to types we need to care about.
			// Foundation.NSObject is for handling of types that
			// descend from NObject and need to have the constructor changed 
			return module.GetTypeReferences ().Any (
				tr =>
					tr.FullName == "System.nint" ||
					tr.FullName == "System.nuint" ||
					tr.FullName == "System.nfloat" ||
					tr.FullName == "Foundation.NSObject" ||
					tr.FullName == "ObjCRuntime.DisposableObject"
				);
		}

		public static Reworker? CreateReworker (FileStream stream, ModuleContainer modules, TypeAndMemberMap moduleMap)
		{
			if (!NeedsReworking (modules.ModuleToEdit)) {
				return null;
			}
			return new Reworker (stream, modules, moduleMap);
		}

		Reworker (FileStream stream, ModuleContainer modules, TypeAndMemberMap moduleMap)
		{
			Stream = stream;
			Modules = modules;
			ModuleMap = moduleMap;

			AttributeTypeReference = ImportNamedTypeReferenceWithFallback ("System.Attribute", typeof (Attribute));
			AttributeTargetsTypeReference = ImportNamedTypeReferenceWithFallback ("System.AttributeTargets", typeof (AttributeTargets));
			AttributeUsageTypeReference = ImportNamedTypeReferenceWithFallback ("System.AttributeUsageAttribute", typeof (AttributeUsageAttribute));
			CompilerGeneratedAttributeTypeRef = ImportNamedTypeReferenceWithFallback ("System.Runtime.CompilerServices.CompilerGeneratedAttribute", typeof (System.Runtime.CompilerServices.CompilerGeneratedAttribute));
			CompilerGeneratedCtorRef = new MethodReference (".ctor", ModuleToEdit.TypeSystem.Void, CompilerGeneratedAttributeTypeRef);
			CompilerGeneratedCtorRef.HasThis = true;

			EmbeddedAttributeTypeDef = ModuleToEdit.Types.FirstOrDefault (td => td.FullName == "Microsoft.CodeAnalysis.EmbeddedAttribute")
				?? MakeEmbeddedAttribute (ModuleToEdit);

			EmbeddedAttributeTypeRef = ModuleToEdit.ImportReference (new TypeReference (EmbeddedAttributeTypeDef.Namespace,
				EmbeddedAttributeTypeDef.Name, EmbeddedAttributeTypeDef.Module, EmbeddedAttributeTypeDef.Scope));

			EmbeddedAttributeCtorRef = CtorWithNArgs (EmbeddedAttributeTypeDef, 0);

			NativeIntegerAttributeTypeDef = ModuleToEdit.Types.FirstOrDefault (td => td.FullName == "System.Runtime.CompilerServices.NativeIntegerAttribute")
				?? MakeNativeIntegerAttribute (ModuleToEdit);

			NativeIntegerAttributeTypeRef = new TypeReference (NativeIntegerAttributeTypeDef.Namespace,
				NativeIntegerAttributeTypeDef.Name, NativeIntegerAttributeTypeDef.Module, NativeIntegerAttributeTypeDef.Scope);

			NativeIntegerCtorNoArgsRef = NativeIntegerAttributeTypeDef.Methods.First (m => m.Name == ".ctor" && m.Parameters.Count == 0);
			NativeIntegerCtorOneArgRef = NativeIntegerAttributeTypeDef.Methods.First (m => m.Name == ".ctor" && m.Parameters.Count == 1);

			if (modules.MicrosoftModule.AssemblyReferences.FirstOrDefault (an => an.Name == "System.Runtime.InteropServices") is AssemblyNameReference validReference) {
				InteropServicesAssembly = validReference;
			} else {
				throw new NotSupportedException ($"Assembly {modules.MicrosoftModule.Name} does not have reference to System.Runtime.InteropServices. This is not possible.");
			}

			// if these type references aren't found in the module
			// then we don't ever need to worry about reworking them
			// and EmptyTypeReference is a fine substitute.
			if (!ModuleToEdit.TryGetTypeReference ("System.nint", out NintTypeReference)) {
				NintTypeReference = EmptyTypeReference;
			}
			if (!ModuleToEdit.TryGetTypeReference ("System.nuint", out NuintTypeReference)) {
				NuintTypeReference = EmptyTypeReference;
			}
			if (!ModuleToEdit.TryGetTypeReference ("System.nfloat", out NfloatTypeReference)) {
				NfloatTypeReference = EmptyTypeReference;
			}
			NewNfloatTypeReference = ModuleToEdit.ImportReference (new TypeReference ("System.Runtime.InteropServices", "NFloat", null, InteropServicesAssembly, true));
			NewNativeHandleTypeDefinition = modules.MicrosoftModule.Types.First (t => t.FullName == "ObjCRuntime.NativeHandle");

			var nativeHandleOpImplicit = NewNativeHandleTypeDefinition.Resolve ().GetMethods ().First (m => m.FullName == "ObjCRuntime.NativeHandle ObjCRuntime.NativeHandle::op_Implicit(System.IntPtr)");
			ReplacePlatformAssemblyReference ();
			ConstructorTransforms = new ConstructorTransforms (ModuleToEdit.ImportReference (NewNativeHandleTypeDefinition), ModuleToEdit.TypeSystem.Boolean, ModuleToEdit.ImportReference (nativeHandleOpImplicit), WarningIssued, Transformed);
			NativeHandleGetHandleReference = NewNativeHandleTypeDefinition.Methods.First (m => m.Name == "get_Handle");

			MethodSubs = LoadMethodSubs ();
			FieldSubs = LoadFieldSubs ();
		}

		TypeReference ImportNamedTypeReferenceWithFallback (string name, Type fallback)
		{
			if (Modules.MicrosoftModule.TryGetTypeReference (name, out var resultType)) {
				return ModuleToEdit.ImportReference (resultType);
			} else {
				return ModuleToEdit.ImportReference (fallback);
			}
		}

		static TypeReference FetchFromSystemRuntime (ModuleDefinition module, string nameSpace, string typeName)
		{
			var type = module.GetTypeReferences ().FirstOrDefault (tr => tr.Namespace == nameSpace && tr.Name == typeName);
			if (type is not null)
				return type;

			type = new TypeReference (nameSpace, typeName, module, new AssemblyNameReference ("System.Runtime", new Version ("6.0.0.0")));
			return module.ImportReference (type);
		}

		TypeDefinition MakeEmbeddedAttribute (ModuleDefinition module)
		{
			// make type definition
			var typeDef = new TypeDefinition ("Microsoft.CodeAnalysis", "EmbeddedAttribute",
				TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit | TypeAttributes.NotPublic | TypeAttributes.Sealed,
				module.TypeSystem.Object);
			module.Types.Add (typeDef);

			// add inheritance
			typeDef.BaseType = AttributeTypeReference;

			// add [CompilerGenerated]
			var attr_CompilerGenerated_1 = new CustomAttribute (module.ImportReference (CompilerGeneratedCtorRef));
			typeDef.CustomAttributes.Add (attr_CompilerGenerated_1);

			// add default constructor
			var ctor = new MethodDefinition (".ctor", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.RTSpecialName | MethodAttributes.SpecialName,
				module.TypeSystem.Void);
			typeDef.Methods.Add (ctor);
			var il_ctor_4 = ctor.Body.GetILProcessor ();
			il_ctor_4.Emit (OpCodes.Ldarg_0);
			il_ctor_4.Emit (OpCodes.Call, module.ImportReference (DefaultCtorFor (typeDef.BaseType)));
			il_ctor_4.Emit (OpCodes.Ret);

			// add [EmbeddedAttribute] - requires both the constructor above and the type ref
			var embeddedAttr = new CustomAttribute (ctor);
			typeDef.CustomAttributes.Add (embeddedAttr);

			return typeDef;
		}

		TypeDefinition MakeNativeIntegerAttribute (ModuleDefinition module)
		{
			var typeDef = new TypeDefinition ("System.Runtime.CompilerServices", "NativeIntegerAttribute", TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit | TypeAttributes.NotPublic | TypeAttributes.Sealed, module.TypeSystem.Object);
			module.Types.Add (typeDef);
			typeDef.BaseType = AttributeTypeReference; //module.ImportReference (typeof (Attribute));

			// add [CompilerGenerated]
			var attr_CompilerGenerated = new CustomAttribute (module.ImportReference (CompilerGeneratedCtorRef));
			typeDef.CustomAttributes.Add (attr_CompilerGenerated);

			// add [Embedded]
			var attr_Embedded = new CustomAttribute (EmbeddedAttributeCtorRef);
			typeDef.CustomAttributes.Add (attr_Embedded);

			// add [AttributeUsage(...)]
			module.ImportReference (AttributeUsageTypeReference);
			var attrUsageCtorReference = new MethodReference (".ctor", module.TypeSystem.Void, AttributeUsageTypeReference);
			attrUsageCtorReference.HasThis = true;
			attrUsageCtorReference.Parameters.Add (new ParameterDefinition (AttributeTargetsTypeReference));
			module.ImportReference (attrUsageCtorReference);
			var attr_AttributeUsage = new CustomAttribute (attrUsageCtorReference);
			attr_AttributeUsage.ConstructorArguments.Add (new CustomAttributeArgument (AttributeTargetsTypeReference, AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Parameter | AttributeTargets.ReturnValue | AttributeTargets.GenericParameter));
			attr_AttributeUsage.Properties.Add (new CustomAttributeNamedArgument ("AllowMultiple", new CustomAttributeArgument (module.TypeSystem.Boolean, false)));
			attr_AttributeUsage.Properties.Add (new CustomAttributeNamedArgument ("Inherited", new CustomAttributeArgument (module.TypeSystem.Boolean, false)));
			typeDef.CustomAttributes.Add (attr_AttributeUsage);

			var transformFlags = new FieldDefinition ("TransformFlags", FieldAttributes.Public, module.TypeSystem.Boolean.MakeArrayType ());
			typeDef.Fields.Add (transformFlags);

			// default ctor
			var defaultCtor = new MethodDefinition (".ctor", MethodAttributes.Public | MethodAttributes.RTSpecialName | MethodAttributes.SpecialName | MethodAttributes.HideBySig, module.TypeSystem.Void);
			typeDef.Methods.Add (defaultCtor);
			defaultCtor.Body.InitLocals = true;
			var il_defaultCtor = defaultCtor.Body.GetILProcessor ();
			il_defaultCtor.Emit (OpCodes.Ldarg_0);
			il_defaultCtor.Emit (OpCodes.Call, module.ImportReference (DefaultCtorFor (typeDef.BaseType)));
			il_defaultCtor.Emit (OpCodes.Nop);
			il_defaultCtor.Emit (OpCodes.Ldarg_0);
			il_defaultCtor.Emit (OpCodes.Ldc_I4_1);
			il_defaultCtor.Emit (OpCodes.Newarr, module.TypeSystem.Boolean);
			il_defaultCtor.Emit (OpCodes.Dup);
			il_defaultCtor.Emit (OpCodes.Ldc_I4_0);
			il_defaultCtor.Emit (OpCodes.Ldc_I4_1);
			il_defaultCtor.Emit (OpCodes.Stelem_I1);
			il_defaultCtor.Emit (OpCodes.Stfld, transformFlags);
			il_defaultCtor.Emit (OpCodes.Ret);

			// ctor (byte[] P_0)
			var arrayCtor = new MethodDefinition (".ctor", MethodAttributes.Public | MethodAttributes.RTSpecialName | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
				module.TypeSystem.Void);
			typeDef.Methods.Add (arrayCtor);
			arrayCtor.Body.InitLocals = true;
			var il_arrayCtor = arrayCtor.Body.GetILProcessor ();
			il_arrayCtor.Emit (OpCodes.Ldarg_0);
			il_arrayCtor.Emit (OpCodes.Call, module.ImportReference (DefaultCtorFor (typeDef.BaseType)));

			//Parameters of 'public NativeIntegerAttribute(bool[] P_0)'
			var p_P_0_14 = new ParameterDefinition ("P_0", ParameterAttributes.None, module.TypeSystem.Boolean.MakeArrayType ());
			arrayCtor.Parameters.Add (p_P_0_14);

			//TransformFlags = P_0;
			var Ldarg_0_15 = il_arrayCtor.Create (OpCodes.Ldarg_0);
			il_arrayCtor.Append (Ldarg_0_15);
			il_arrayCtor.Emit (OpCodes.Ldarg_1);
			il_arrayCtor.Emit (OpCodes.Stfld, transformFlags);
			il_arrayCtor.Emit (OpCodes.Ret);

			return typeDef;
		}

		CustomAttribute NativeIntAttribute (List<bool> nativeTypes)
		{
			var methodReference = nativeTypes.Count == 1 ?
				NativeIntegerCtorNoArgsRef : NativeIntegerCtorOneArgRef;

			var nativeIntAttr = new CustomAttribute (methodReference);
			if (nativeTypes.Count > 1) {
				var boolArrayParameter = new ParameterDefinition (Modules.TypeSystem.Boolean.MakeArrayType ());
				nativeIntAttr.Constructor.Parameters.Add (boolArrayParameter);
				var boolArray = nativeTypes.Select (b => new CustomAttributeArgument (Modules.TypeSystem.Boolean, b)).ToArray ();
				nativeIntAttr.ConstructorArguments.Add (new CustomAttributeArgument (Modules.TypeSystem.Boolean.MakeArrayType (), boolArray));
			}
			return nativeIntAttr;
		}

		Dictionary<string, Transformation> LoadMethodSubs ()
		{
			var methodSubSource = new MethodTransformations ();
			var subs = methodSubSource.GetTransforms (Modules, NativeIntAttribute, NewNfloatTypeReference);

			return subs;
		}

		Dictionary<string, Transformation> LoadFieldSubs ()
		{
			var fieldSubSource = new FieldTransformations ();
			var subs = fieldSubSource.GetTransforms (ModuleToEdit);

			return subs;
		}

		public void Rework (Stream stm)
		{
			foreach (var type in ModuleToEdit.Types) {
				ReworkType (type);
			}
			ChangeTargetFramework ();
			ModuleToEdit.Write (stm);
			stm.Flush ();
		}

		void ChangeTargetFramework ()
		{
			if (TryGetTargetFrameworkAttribute (out var attribute)) {
				if (attribute.ConstructorArguments.Count == 1) { // should always be true
					attribute.ConstructorArguments [0] = new CustomAttributeArgument (ModuleToEdit.TypeSystem.String, NetCoreAppDependency);
				}
			}
		}

		bool TryGetTargetFrameworkAttribute ([NotNullWhen (returnValue: true)] out CustomAttribute? result)
		{
			foreach (var attribute in ModuleToEdit.Assembly.CustomAttributes) {
				if (attribute.AttributeType.FullName == "System.Runtime.Versioning.TargetFrameworkAttribute") {
					result = attribute;
					return true;
				}
			}
			result = null;
			return false;
		}

		void ReplacePlatformAssemblyReference ()
		{
			for (int i = ModuleToEdit.AssemblyReferences.Count - 1; i >= 0; i--) {
				if (IsXamarinReference (ModuleToEdit.AssemblyReferences [i])) {
					ModuleToEdit.AssemblyReferences [i] = AssemblyNameReference.Parse (Modules.MicrosoftModule.Assembly.ToString ());
				}
			}
		}

		static HashSet<string> XamarinAssemblyNames = new HashSet<string> () {
			"Xamarin.Mac",
			"Xamarin.iOS",
			"Xamarin.TVOS",
			"Xamarin.WatchOS",
		};

		static bool IsXamarinReference (AssemblyNameReference reference)
		{
			return XamarinAssemblyNames.Contains (reference.Name);
		}

		void ReworkType (TypeDefinition definition)
		{
			// This must occur before general processing as
			// the list of transformed constructors is used later for rewriting
			ConstructorTransforms.ReworkAsNeeded (definition);

			foreach (var field in definition.Fields) {
				ReworkField (field);
			}
			foreach (var property in definition.Properties) {
				ReworkProperty (property);
			}
			ReworkMethods (definition.Methods);
			foreach (var @event in definition.Events) {
				ReworkEvent (@event);
			}
			foreach (var innerType in definition.NestedTypes) {
				ReworkType (innerType);
			}
		}

		void ReworkMethods (IEnumerable<MethodDefinition> methods)
		{
			foreach (var method in methods) {
				ReworkMethod (method);
			}
		}

		void ReworkMethod (MethodDefinition method)
		{
			var nativeTypes = new List<bool> ();
			foreach (var parameter in method.Parameters) {
				nativeTypes.Clear ();
				if (TryReworkTypeReference (parameter.ParameterType, nativeTypes, out var newType)) {
					parameter.ParameterType = newType;
				}
				if (nativeTypes.Contains (true)) {
					parameter.CustomAttributes.Add (NativeIntAttribute (nativeTypes));
				}
			}

			nativeTypes.Clear ();
			if (TryReworkTypeReference (method.ReturnType, nativeTypes, out var newReturnType)) {
				method.ReturnType = newReturnType;
			}
			if (nativeTypes.Contains (true)) {
				method.MethodReturnType.CustomAttributes.Add (NativeIntAttribute (nativeTypes));
			}

			if (method.Body is not null) {
				foreach (var variable in method.Body.Variables) {
					nativeTypes.Clear ();
					if (TryReworkTypeReference (variable.VariableType, nativeTypes, out var newVarType)) {
						variable.VariableType = newVarType;
					}
				}
				ReworkCodeBlock (method.Body);
			}
		}

		void ReworkProperty (PropertyDefinition prop)
		{
			var nativeTypes = new List<bool> ();
			if (TryReworkTypeReference (prop.PropertyType, nativeTypes, out var newPropType)) {
				prop.PropertyType = newPropType;
			}
			if (nativeTypes.Contains (true)) {
				prop.CustomAttributes.Add (NativeIntAttribute (nativeTypes));
			}
			ReworkMethods (PropMethods (prop));
		}

		void ReworkField (FieldDefinition field)
		{
			var nativeTypes = new List<bool> ();
			if (TryReworkTypeReference (field.FieldType, nativeTypes, out var newType)) {
				field.FieldType = newType;
			}
			if (nativeTypes.Contains (true)) {
				field.CustomAttributes.Add (NativeIntAttribute (nativeTypes));
			}
		}

		void ReworkEvent (EventDefinition @event)
		{
			var nativeTypes = new List<bool> ();
			if (TryReworkTypeReference (@event.EventType, nativeTypes, out var newType)) {
				@event.EventType = newType;
			}
			if (nativeTypes.Contains (true)) {
				@event.CustomAttributes.Add (NativeIntAttribute (nativeTypes));
			}
			ReworkMethods (EventMethods (@event));
		}

		bool TryReworkTypeReference (TypeReference type, List<bool> nativeTypes, [NotNullWhen (returnValue: true)] out TypeReference result)
		{
			// what happens here? if the type gets changed, the flag will get set to true
			// and it will return the new type. The list, nativeTypes, will get set to the
			// a boolean for each subtype within the type if it in turn has been changed.
			// Why a list? Because the type reference could be something like:
			// Foo<nint, Bar<Baz<nfloat>, int>
			// For any of nint, nuint, this will set the particular bool to true, false otherwise.
			// This list will get passed to NativeIntegerAttribute, which is the special sauce
			// that lets the runtime tell the difference between IntPtr and nint.
			var typeAsString = type.ToString ();

			if (ModuleMap.TypeIsNotPresent (typeAsString)) {
				throw new TypeNotFoundException (typeAsString);
			}

			if (type == Modules.TypeSystem.IntPtr || type == Modules.TypeSystem.UIntPtr) {
				nativeTypes.Add (false);
				result = type;
				return false;
			} else if (type == NintTypeReference || type == NuintTypeReference) {
				nativeTypes.Add (true);
				result = type == NintTypeReference ? Modules.TypeSystem.IntPtr : Modules.TypeSystem.UIntPtr;
				return true;
			} else if (type == NfloatTypeReference) {
				// changing the type to NFloat doesn't require changing the flags.
				nativeTypes.Add (false);
				result = NewNfloatTypeReference;
				return true;
			} else if (ModuleMap.TryGetMappedType (typeAsString, out var mappedType)) {
				result = ModuleToEdit.ImportReference (mappedType);
				return true;
			} else if (type.IsGenericInstance) {
				return TryReworkGenericType ((GenericInstanceType) type, nativeTypes, out result);
			} else if (type.IsArray) {
				return TryReworkArray ((ArrayType) type, nativeTypes, out result);
			}
			result = type;
			return false;
		}

		bool TryReworkGenericType (GenericInstanceType type, List<bool> nativeTypes, [NotNullWhen (returnValue: true)] out TypeReference result)
		{
			var localChanged = false;
			for (var i = 0; i < type.GenericArguments.Count; i++) {
				var genType = type.GenericArguments [i];
				if (TryReworkTypeReference (genType, nativeTypes, out var newType)) {
					localChanged = true;
					type.GenericArguments [i] = newType;
				}
			}
			result = type;
			return localChanged;
		}

		bool TryReworkArray (ArrayType type, List<bool> nativeTypes, [NotNullWhen (returnValue: true)] out TypeReference result)
		{
			if (TryReworkTypeReference (type.ElementType, nativeTypes, out var elementType)) {
				result = elementType.MakeArrayType (type.Rank);
				return true;
			}
			result = type;
			return false;
		}

		void ReworkCodeBlock (MethodBody body)
		{
			var changes = new List<Tuple<Instruction, Transformation>> ();
			foreach (var instruction in body.Instructions) {
				if (instruction.Operand?.ToString () is string operandText) {
					if (ModuleMap.TypeIsNotPresent (operandText)) {
						throw new TypeNotFoundException (operandText);
					}
					if (ModuleMap.MemberIsNotPresent (operandText)) {
						throw new MemberNotFoundException (operandText);
					}
					if (ModuleMap.TryGetMappedType (operandText, out var type)) {
						var newInstruction = ChangeTypeInstruction (instruction, type);
						changes.Add (new Tuple<Instruction, Transformation> (instruction, new Transformation (operandText, newInstruction)));
						continue;
					} else if (ModuleMap.TryGetMappedMember (operandText, out var member)) {
						var newInstruction = ChangeMemberInstruction (instruction, member);
						changes.Add (new Tuple<Instruction, Transformation> (instruction, new Transformation (operandText, newInstruction)));
						continue;
					}
				}
				if (TryGetMethodTransform (body, instruction, out var transform)) {
					changes.Add (new Tuple<Instruction, Transformation> (instruction, transform));
					continue;
				}
				if (TryGetFieldTransform (instruction, out transform)) {
					changes.Add (new Tuple<Instruction, Transformation> (instruction, transform));
					continue;
				}
				if (ConstructorTransforms.TryGetConstructorCallTransformation (instruction, out transform)) {
					changes.Add (new Tuple<Instruction, Transformation> (instruction, transform));
					continue;
				}
			}

			foreach (var (instr, trans) in changes) {
				if (!trans.TryPerformTransform (instr, body)) {
					WarningIssued?.Invoke (this, new WarningEventArgs (body.Method.DeclaringType.FullName, body.Method.Name, trans.Operand, trans.Message!));
				} else {
					var added = (uint) trans.Instructions.Count;
					var removed = trans.Action == TransformationAction.Remove || trans.Action == TransformationAction.Replace ? (uint) 1 : 0;
					Transformed?.Invoke (this, new TransformEventArgs (body.Method.DeclaringType.FullName, body.Method.Name, trans.Operand, added, removed));
				}
			}

			// Stunt optimization - there will only have been
			// a need to patch class handle references if and only
			// if there had been a prior change to this method.
			// This is because the call to get_ClassHandle gets
			// redirected by TryGetMappedMember
			if (changes.Count > 0)
				PatchHandleReferences (body);
		}

		Instruction ChangeTypeInstruction (Instruction instruction, TypeReference typeReference)
		{
			if (instruction.Operand is TypeReference) {
				return Instruction.Create (instruction.OpCode, ModuleToEdit.ImportReference (typeReference));
			}
			// should never happen
			throw new ArgumentException (nameof (instruction));
		}

		Instruction ChangeMemberInstruction (Instruction instruction, IMemberDefinition member)
		{
			switch (member) {
			case MethodDefinition method:
				if (instruction.Operand is MethodReference) {
					return Instruction.Create (instruction.OpCode, ModuleToEdit.ImportReference (method));
				}
				// should never happen
				throw new ArgumentException (nameof (instruction));
			case FieldDefinition field:
				if (instruction.Operand is FieldReference) {
					return Instruction.Create (instruction.OpCode, ModuleToEdit.ImportReference (field));
				}
				// should never happen
				throw new ArgumentException (nameof (instruction));
			case EventDefinition @event:
			case PropertyDefinition @property:
				// AFAICT no instruction will ever have a property or event
				// as its operand.
				throw new ArgumentException (nameof (member));
			default:
				throw new ArgumentException ($"Unknown member of type {member.GetType ().Name}", nameof (member));

			}
		}

		static bool IsCallInstruction (Instruction instr)
		{
			return instr.OpCode == OpCodes.Call ||
				instr.OpCode == OpCodes.Calli ||
				instr.OpCode == OpCodes.Callvirt;
		}

		bool TryGetMethodTransform (MethodBody body, Instruction instr, [NotNullWhen (returnValue: true)] out Transformation? result)
		{
			if (!IsCallInstruction (instr)) {
				result = null;
				return false;
			}

			if (instr.Operand is MethodReference method && MethodSubs.TryGetValue (method.ToString (), out result)) {
				return true;
			}
			result = null;
			return false;
		}

		void PatchHandleReferences (MethodBody body)
		{
			ILProcessor processor = body.GetILProcessor ();
			for (int i = body.Instructions.Count - 1; i >= 0; i--) {
				var instr = body.Instructions [i];
				if (!IsCallInstruction (instr))
					continue;
				var operandStr = instr.Operand.ToString ()!;
				if (IsHandleReference (operandStr)) {
					var reference = ModuleToEdit.ImportReference (NativeHandleGetHandleReference);
					processor.InsertAfter (instr, Instruction.Create (OpCodes.Call, reference));
					Transformed?.Invoke (this, new TransformEventArgs (body.Method.DeclaringType.FullName,
						body.Method.Name, operandStr, 1, 0));
				}
			}
		}

		bool IsHandleReference (string operandStr)
		{
			return operandStr == "ObjCRuntime.NativeHandle ObjCRuntime.DisposableObject::get_Handle()" ||
				operandStr == "ObjCRuntime.NativeHandle Foundation.NSObject::get_ClassHandle()" ||
				operandStr == "ObjCRuntime.NativeHandle Foundation.NSObject::get_Handle()";
		}

		bool TryGetFieldTransform (Instruction instr, [NotNullWhen (returnValue: true)] out Transformation? result)
		{
			// if we get a Stsfld instruction, someone has written some truly
			// awful code
			if (instr.OpCode != OpCodes.Ldsfld && instr.OpCode != OpCodes.Stsfld) {
				result = null;
				return false;
			}
			if (instr.Operand is FieldReference field && FieldSubs.TryGetValue (field.ToString (), out result)) {
				return true;
			}
			result = null;
			return false;
		}

		internal static IEnumerable<MethodDefinition> PropMethods (PropertyDefinition prop)
		{
			if (prop.GetMethod is not null)
				yield return prop.GetMethod;
			if (prop.SetMethod is not null)
				yield return prop.SetMethod;
			foreach (var method in prop.OtherMethods)
				yield return method;
		}

		internal static IEnumerable<MethodDefinition> EventMethods (EventDefinition @event)
		{
			if (@event.AddMethod is not null)
				yield return @event.AddMethod;
			if (@event.RemoveMethod is not null)
				yield return @event.RemoveMethod;
			if (@event.InvokeMethod is not null)
				yield return @event.InvokeMethod;
			foreach (var method in @event.OtherMethods)
				yield return method;
		}

		static MethodReference? DefaultCtorFor (TypeReference type)
		{
			var resolved = type.Resolve ();
			if (resolved is null)
				return null;

			var ctor = resolved.Methods.SingleOrDefault (m => m.IsConstructor && m.Parameters.Count == 0 && !m.IsStatic);
			if (ctor is null)
				return DefaultCtorFor (resolved.BaseType);

			return new MethodReference (".ctor", type.Module.TypeSystem.Void, type) { HasThis = true };
		}

		static MethodReference CtorWithNArgs (TypeDefinition type, int args) =>
			type.Methods.First (m => m.Name == ".ctor" && m.Parameters.Count == args);

		static TypeReference EmptyTypeReference = new TypeReference ("none", "still_none", null, null);
	}
}
