using System;
using Mono.Cecil;
using System.Collections.Generic;
using System.Linq;

namespace Mono.Linker {
	public static class TypeReferenceExtensions {
		public static TypeReference GetInflatedBaseType (this TypeReference type)
		{
			if (type is null)
				return null;

			if (type.IsGenericParameter || type.IsByReference || type.IsPointer)
				return null;

			var sentinelType = type as SentinelType;
			if (sentinelType is not null)
				return sentinelType.ElementType.GetInflatedBaseType ();

			var pinnedType = type as PinnedType;
			if (pinnedType is not null)
				return pinnedType.ElementType.GetInflatedBaseType ();

			var requiredModifierType = type as RequiredModifierType;
			if (requiredModifierType is not null)
				return requiredModifierType.ElementType.GetInflatedBaseType ();

			var genericInstance = type as GenericInstanceType;
			if (genericInstance is not null) {
				var baseType = type.Resolve ()?.BaseType;
				var baseTypeGenericInstance = baseType as GenericInstanceType;

				if (baseTypeGenericInstance is not null)
					return InflateGenericType (genericInstance, baseType);

				return baseType;
			}

			return type.Resolve ()?.BaseType;
		}

		public static IEnumerable<TypeReference> GetInflatedInterfaces (this TypeReference typeRef)
		{
			var typeDef = typeRef.Resolve ();

			if (typeDef?.HasInterfaces != true)
				yield break;

			var genericInstance = typeRef as GenericInstanceType;
			if (genericInstance is not null) {
				foreach (var interfaceImpl in typeDef.Interfaces)
					yield return InflateGenericType (genericInstance, interfaceImpl.InterfaceType);
			} else {
				foreach (var interfaceImpl in typeDef.Interfaces)
					yield return interfaceImpl.InterfaceType;
			}
		}

		public static TypeReference InflateGenericType (GenericInstanceType genericInstanceProvider, TypeReference typeToInflate)
		{
			var arrayType = typeToInflate as ArrayType;
			if (arrayType is not null) {
				var inflatedElementType = InflateGenericType (genericInstanceProvider, arrayType.ElementType);

				if (inflatedElementType != arrayType.ElementType)
					return new ArrayType (inflatedElementType, arrayType.Rank);

				return arrayType;
			}

			var genericInst = typeToInflate as GenericInstanceType;
			if (genericInst is not null)
				return MakeGenericType (genericInstanceProvider, genericInst);

			var genericParameter = typeToInflate as GenericParameter;
			if (genericParameter is not null) {
				if (genericParameter.Owner is MethodReference)
					return genericParameter;

				var elementType = genericInstanceProvider.ElementType.Resolve ();
				var parameter = elementType.GenericParameters.Single (p => p == genericParameter);
				return genericInstanceProvider.GenericArguments [parameter.Position];
			}

			var functionPointerType = typeToInflate as FunctionPointerType;
			if (functionPointerType is not null) {
				var result = new FunctionPointerType ();
				result.ReturnType = InflateGenericType (genericInstanceProvider, functionPointerType.ReturnType);

				for (int i = 0; i < functionPointerType.Parameters.Count; i++) {
					var inflatedParameterType = InflateGenericType (genericInstanceProvider, functionPointerType.Parameters [i].ParameterType);
					result.Parameters.Add (new ParameterDefinition (inflatedParameterType));
				}

				return result;
			}

			var modifierType = typeToInflate as IModifierType;
			if (modifierType is not null) {
				var modifier = InflateGenericType (genericInstanceProvider, modifierType.ModifierType);
				var elementType = InflateGenericType (genericInstanceProvider, modifierType.ElementType);

				if (modifierType is OptionalModifierType) {
					return new OptionalModifierType (modifier, elementType);
				}

				return new RequiredModifierType (modifier, elementType);
			}

			var pinnedType = typeToInflate as PinnedType;
			if (pinnedType is not null) {
				var elementType = InflateGenericType (genericInstanceProvider, pinnedType.ElementType);

				if (elementType != pinnedType.ElementType)
					return new PinnedType (elementType);

				return pinnedType;
			}

			var pointerType = typeToInflate as PointerType;
			if (pointerType is not null) {
				var elementType = InflateGenericType (genericInstanceProvider, pointerType.ElementType);

				if (elementType != pointerType.ElementType)
					return new PointerType (elementType);

				return pointerType;
			}

			var byReferenceType = typeToInflate as ByReferenceType;
			if (byReferenceType is not null) {
				var elementType = InflateGenericType (genericInstanceProvider, byReferenceType.ElementType);

				if (elementType != byReferenceType.ElementType)
					return new ByReferenceType (elementType);

				return byReferenceType;
			}

			var sentinelType = typeToInflate as SentinelType;
			if (sentinelType is not null) {
				var elementType = InflateGenericType (genericInstanceProvider, sentinelType.ElementType);

				if (elementType != sentinelType.ElementType)
					return new SentinelType (elementType);

				return sentinelType;
			}

			return typeToInflate;
		}

		private static GenericInstanceType MakeGenericType (GenericInstanceType genericInstanceProvider, GenericInstanceType type)
		{
			var result = new GenericInstanceType (type.ElementType);

			for (var i = 0; i < type.GenericArguments.Count; ++i) {
				result.GenericArguments.Add (InflateGenericType (genericInstanceProvider, type.GenericArguments [i]));
			}

			return result;
		}

		public static IEnumerable<MethodReference> GetMethods (this TypeReference type)
		{
			var typeDef = type.Resolve ();

			if (typeDef?.HasMethods != true)
				yield break;

			var genericInstanceType = type as GenericInstanceType;
			if (genericInstanceType is not null) {
				foreach (var methodDef in typeDef.Methods)
					yield return MakeMethodReferenceForGenericInstanceType (genericInstanceType, methodDef);
			} else {
				foreach (var method in typeDef.Methods)
					yield return method;
			}
		}

		private static MethodReference MakeMethodReferenceForGenericInstanceType (GenericInstanceType genericInstanceType, MethodDefinition methodDef)
		{
			var method = new MethodReference (methodDef.Name, methodDef.ReturnType, genericInstanceType) {
				HasThis = methodDef.HasThis,
				ExplicitThis = methodDef.ExplicitThis,
				CallingConvention = methodDef.CallingConvention
			};

			foreach (var parameter in methodDef.Parameters)
				method.Parameters.Add (new ParameterDefinition (parameter.Name, parameter.Attributes, parameter.ParameterType));

			foreach (var gp in methodDef.GenericParameters)
				method.GenericParameters.Add (new GenericParameter (gp.Name, method));

			return method;
		}

		public static string ToCecilName (this string fullTypeName)
		{
			return fullTypeName.Replace ('+', '/');
		}

		public static bool HasDefaultConstructor (this TypeReference type)
		{
			foreach (var m in type.GetMethods ()) {
				if (m.HasParameters)
					continue;

				var definition = m.Resolve ();
				if (definition?.IsDefaultConstructor () == true)
					return true;
			}

			return false;
		}

		public static MethodReference GetDefaultInstanceConstructor (this TypeReference type)
		{
			foreach (var m in type.GetMethods ()) {
				if (m.HasParameters)
					continue;

				var definition = m.Resolve ();
				if (!definition.IsDefaultConstructor ())
					continue;

				return m;
			}

			throw new NotImplementedException ();
		}

		public static bool IsTypeOf (this TypeReference type, string ns, string name)
		{
			return type.Name == name
				&& type.Namespace == ns;
		}
	}
}
