using System;
using System.IO;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.MaciOS.Nnyeah {
	public class Reworker {
		Stream stm;
		ModuleDefinition module = EmptyModule;
		TypeDefinition embeddedAttributeTypeDef = EmptyTypeDefinition;
		TypeDefinition nativeIntegerAttributeTypeDef = EmptyTypeDefinition;
		TypeReference nativeIntegerAttributeTypeRef = EmptyTypeReference;
		TypeReference compilerGeneratedAttributeTypeRef = EmptyTypeReference;
		TypeReference embeddedAttributeTypeRef = EmptyTypeReference;
		TypeReference nintTypeReference = EmptyTypeReference;
		TypeReference nuintTypeReference = EmptyTypeReference;
		TypeReference nfloatTypeReference = EmptyTypeReference;
		TypeReference newNfloatTypeReference = EmptyTypeReference;
		ModuleReference newNfloatModuleReference = EmptyModuleReference;

		Dictionary<string, Transformation> methodSubs = new Dictionary<string, Transformation> ();
		Dictionary<string, Transformation> fieldSubs = new Dictionary<string, Transformation> ();

		public event EventHandler<WarningEventArgs>? WarningIssued;
		public event EventHandler<TransformEventArgs>? Transformed;

		public Reworker (Stream stm)
		{
			this.stm = stm;
		}

		public void Load ()
		{
			if (module != EmptyModule)
				return;
			module = ModuleDefinition.ReadModule (stm);
		}

		public bool NeedsReworking ()
		{
			CheckModule ();

			// simple predicate for seeing if there any references
			// to types we need to care about.
			// IntPtr is for future handling of types that
			// descend from NObject and need to have the constructor
			// changed to NHandle
			return module.GetTypeReferences ().Any (
				tr =>
					tr.FullName == "System.nint" ||
					tr.FullName == "System.nuint" ||
					tr.FullName == "System.nfloat" ||
					tr.FullName == "System.IntPtr"
				);
		}

		void CheckModule ()
		{
			if (module == EmptyModule)
				throw new Exception (Errors.E0005);
		}

		public void Rework (Stream stm)
		{
			CheckModule ();

			// get the types that we need to refer to later
			FetchCompilerGeneratedAttributeTypeRef ();
			AddEmbeddedAttributeIfNeeded ();
			AddNativeIntegerAttributeIfNeeded ();
			module.TryGetTypeReference ("System.nint", out nintTypeReference);
			module.TryGetTypeReference ("System.nuint", out nuintTypeReference);
			module.TryGetTypeReference ("System.nfloat", out nfloatTypeReference);
			newNfloatModuleReference = new ModuleReference ("System.Private.CoreLib");
			newNfloatTypeReference = new TypeReference ("System.Runtime.InteropServices",
				"NFloat", null, newNfloatModuleReference, true);

			// load the substitutions
			methodSubs = LoadMethodSubs ();
			fieldSubs = LoadFieldSubs ();

			foreach (var type in module.Types) {
				ReworkType (type);
			}

			module.Write (stm);
			stm.Flush ();
		}

		void ReworkType (TypeDefinition definition)
		{
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

		CustomAttribute NativeIntAttribute (List<bool> nativeTypes)
		{
			var nativeIntAttr = new CustomAttribute (new MethodReference (".ctor", module.TypeSystem.Void, nativeIntegerAttributeTypeRef));
			if (nativeTypes.Count > 1) {
				var boolArrayParameter = new ParameterDefinition (module.TypeSystem.Boolean.MakeArrayType ());
				nativeIntAttr.Constructor.Parameters.Add (boolArrayParameter);
				var boolArray = nativeTypes.Select (b =>
					new CustomAttributeArgument (module.TypeSystem.Boolean, b)).ToArray ();

				nativeIntAttr.ConstructorArguments.Add (new CustomAttributeArgument (module.TypeSystem.Boolean.MakeArrayType (),
					boolArray));
			}
			return nativeIntAttr;
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
			if (type == module.TypeSystem.IntPtr || type == module.TypeSystem.UIntPtr) {
				nativeTypes.Add (false);
				result = type;
				return false;
			} else if (type == nintTypeReference || type == nuintTypeReference) {
				nativeTypes.Add (true);
				result = type == nintTypeReference ? module.TypeSystem.IntPtr : module.TypeSystem.UIntPtr;
				return true;
			} else if (type == nfloatTypeReference) {
				// changing the type to NFloat doesn't require changing the flags.
				nativeTypes.Add (false);
				result = newNfloatTypeReference;
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
				if (TryGetMethodTransform (instruction, out var transform)) {
					changes.Add (new Tuple<Instruction, Transformation> (instruction, transform));
					continue;
				}
				if (TryGetFieldTransform (instruction, out transform)) {
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
		}

		bool TryGetMethodTransform (Instruction instr, [NotNullWhen (returnValue: true)] out Transformation? result)
		{
			if (instr.OpCode != OpCodes.Call && instr.OpCode != OpCodes.Calli
				&& instr.OpCode != OpCodes.Callvirt) {
				result = null;
				return false;
			}
			if (instr.Operand is MethodReference method && methodSubs.TryGetValue (method.ToString (), out result)) {
				return true;
			}
			result = null;
			return false;
		}

		bool TryGetFieldTransform (Instruction instr, [NotNullWhen (returnValue: true)] out Transformation? result)
		{
			// if we get a Stsfld instruction, someone has written some truly
			// awful code
			if (instr.OpCode != OpCodes.Ldsfld && instr.OpCode != OpCodes.Stsfld) {
				result = null;
				return false;
			}
			if (instr.Operand is FieldReference field && fieldSubs.TryGetValue (field.ToString (), out result)) {
				return true;
			}
			result = null;
			return false;
		}

		void FetchCompilerGeneratedAttributeTypeRef ()
		{
			compilerGeneratedAttributeTypeRef = FetchFromSystemRuntime ("System.Runtime.CompilerServices", "CompilerGeneratedAttribute");
		}

		TypeReference FetchFromSystemRuntime (string nameSpace, string typeName)
		{
			var type = module.GetTypeReferences ().FirstOrDefault (tr => tr.Namespace == nameSpace && tr.Name == typeName);
			if (type is not null)
				return type;
			type = new TypeReference (nameSpace, typeName, module, new AssemblyNameReference ("System.Runtime", new Version ("6.0.0.0")));
			var finalReference = module.ImportReference (type);
			return finalReference;
		}

		void AddEmbeddedAttributeIfNeeded ()
		{
			embeddedAttributeTypeDef = module.Types.FirstOrDefault (td => td.FullName == "Microsoft.CodeAnalysis.EmbeddedAttribute")
				?? MakeEmbeddedAttribute ();
			if (embeddedAttributeTypeRef is null)
				embeddedAttributeTypeRef = module.ImportReference (new TypeReference (embeddedAttributeTypeDef.Namespace,
					embeddedAttributeTypeDef.Name, embeddedAttributeTypeDef.Module, embeddedAttributeTypeDef.Scope));
		}

		TypeDefinition MakeEmbeddedAttribute ()
		{
			// make type definition
			var typeDef = new TypeDefinition ("Microsoft.CodeAnalysis", "EmbeddedAttribute",
				TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit | TypeAttributes.NotPublic | TypeAttributes.Sealed,
				module.TypeSystem.Object);
			module.Types.Add (typeDef);
			// make reference
			embeddedAttributeTypeRef = module.ImportReference (new TypeReference (typeDef.Namespace, typeDef.Name, typeDef.Module, typeDef.Scope));

			// add inheritance
			typeDef.BaseType = module.ImportReference (typeof (Attribute));

			// add [CompilerGenerated]
			var attr_CompilerGenerated_1 = new CustomAttribute (module.ImportReference (typeof (System.Runtime.CompilerServices.CompilerGeneratedAttribute).GetConstructor (new Type [0] { })));
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
			var embeddedAttr = new CustomAttribute (new MethodReference (".ctor", module.TypeSystem.Void, embeddedAttributeTypeRef));
			typeDef.CustomAttributes.Add (embeddedAttr);

			return typeDef;
		}

		void AddNativeIntegerAttributeIfNeeded ()
		{
			nativeIntegerAttributeTypeDef = module.Types.FirstOrDefault (td => td.FullName == "System.Runtime.CompilerServices.NativeIntegerAttribute")
				?? MakeNativeIntegerAttribute ();

			if (nativeIntegerAttributeTypeRef is null) {
				nativeIntegerAttributeTypeRef = new TypeReference (nativeIntegerAttributeTypeDef.Namespace,
					nativeIntegerAttributeTypeDef.Name, nativeIntegerAttributeTypeDef.Module,
					nativeIntegerAttributeTypeDef.Scope);
			}
		}

		TypeDefinition MakeNativeIntegerAttribute ()
		{
			var typeDef = new TypeDefinition ("System.Runtime.CompilerServices", "NativeIntegerAttribute", TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit | TypeAttributes.NotPublic | TypeAttributes.Sealed, module.TypeSystem.Object);
			module.Types.Add (typeDef);
			typeDef.BaseType = module.ImportReference (typeof (Attribute));

			// add [CompilerGenerated]
			var attr_CompilerGenerated = new CustomAttribute (module.ImportReference (typeof (System.Runtime.CompilerServices.CompilerGeneratedAttribute).GetConstructor (new Type [0] { })));
			typeDef.CustomAttributes.Add (attr_CompilerGenerated);

			// add [Embedded]
			var attr_Embedded = new CustomAttribute (new MethodReference (".ctor", module.TypeSystem.Void, embeddedAttributeTypeRef));
			typeDef.CustomAttributes.Add (attr_Embedded);

			// add [AttributeUsage(...)]
			var attr_AttributeUsage = new CustomAttribute (module.ImportReference (typeof (System.AttributeUsageAttribute).GetConstructor (new Type [1] { typeof (AttributeTargets) })));
			attr_AttributeUsage.ConstructorArguments.Add (new CustomAttributeArgument (module.ImportReference (typeof (AttributeTargets)), AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Parameter | AttributeTargets.ReturnValue | AttributeTargets.GenericParameter));
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

		static IEnumerable<MethodDefinition> PropMethods (PropertyDefinition prop)
		{
			if (prop.GetMethod is not null)
				yield return prop.GetMethod;
			if (prop.SetMethod is not null)
				yield return prop.SetMethod;
			foreach (var method in prop.OtherMethods)
				yield return method;
		}

		static IEnumerable<MethodDefinition> EventMethods (EventDefinition @event)
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

		Dictionary<string, Transformation> LoadMethodSubs ()
		{
			var methodSubSource = new MethodTransformations ();
			var subs = methodSubSource.GetTransforms (module, NativeIntAttribute);

			return subs;
		}

		Dictionary<string, Transformation> LoadFieldSubs ()
		{
			var fieldSubSource = new FieldTransformations ();
			var subs = fieldSubSource.GetTransforms (module);

			return subs;
		}

		static TypeDefinition EmptyTypeDefinition = new TypeDefinition ("none", "still_none", TypeAttributes.NotPublic);
		static TypeReference EmptyTypeReference = new TypeReference ("none", "still_none", null, null);
		static ModuleDefinition EmptyModule = ModuleDefinition.CreateModule ("ThisIsNotARealModule", ModuleKind.Dll);
		static ModuleReference EmptyModuleReference = new ModuleReference ("ThisIsNotARealModuleReference");
	}
}
