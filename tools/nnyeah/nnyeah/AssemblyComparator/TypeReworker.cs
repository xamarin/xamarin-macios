using System;
using Mono.Cecil;
using Mono.Cecil.Rocks;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable

namespace Microsoft.MaciOS.Nnyeah.AssemblyComparator {
	public class TypeReworker {
		ModuleDefinition module;
		TypeDefinition nintTypeReference;
		TypeDefinition nuintTypeReference;
		TypeDefinition nfloatTypeReference;
		TypeReference newNfloatTypeReference;
		ModuleReference newNfloatModuleReference;
		TypeReference newNHandleTypeReference;

		public TypeReworker (ModuleDefinition module)
		{
			this.module = module;
			nintTypeReference = GetTypeByName (module, "System.nint");
			nuintTypeReference = GetTypeByName (module, "System.nuint");
			nfloatTypeReference = GetTypeByName (module, "System.nfloat");
			newNfloatModuleReference = new ModuleReference ("System.Private.CoreLib");
			newNfloatTypeReference = new TypeReference ("System.Runtime.InteropServices",
				"NFloat", null, newNfloatModuleReference, true);
			newNHandleTypeReference = new TypeReference ("ObjCRuntime", "NativeHandle",
				null, newNfloatModuleReference, true);
		}

		static TypeDefinition GetTypeByName (ModuleDefinition module, string name)
		{
			foreach (var ty in module.Types) {
				if (ty.ToString () == name)
					return ty;
			}
			throw new Exception (String.Format (Errors.E0010, name));
		}

		public PropertyDefinition ReworkProperty (PropertyDefinition prop)
		{
			var nativeTypes = new List<bool> ();
			if (TryReworkTypeReference (prop.PropertyType, nativeTypes, out var newPropType)) {
				prop.PropertyType = newPropType;
			}
			ReworkMethods (PropMethods (prop));
			return prop;
		}

		public FieldDefinition ReworkField (FieldDefinition field)
		{
			var nativeTypes = new List<bool> ();
			if (TryReworkTypeReference (field.FieldType, nativeTypes, out var newType)) {
				field.FieldType = newType;
			}
			return field;
		}

		public EventDefinition ReworkEvent (EventDefinition @event)
		{
			var nativeTypes = new List<bool> ();
			if (TryReworkTypeReference (@event.EventType, nativeTypes, out var newType)) {
				@event.EventType = newType;
			}

			if (@event.AddMethod is not null)
				@event.AddMethod = ReworkMethod (@event.AddMethod);
			if (@event.RemoveMethod is not null)
				@event.RemoveMethod = ReworkMethod (@event.RemoveMethod);
			if (@event.InvokeMethod is not null)
				@event.InvokeMethod = ReworkMethod (@event.InvokeMethod);

			return @event;
		}

		void ReworkMethods (IEnumerable<MethodDefinition> methods)
		{
			foreach (var method in methods) {
				ReworkMethod (method);
			}
		}

		public MethodDefinition ReworkMethod (MethodDefinition method)
		{
			if (ConstructorTransforms.IsNSObjectDerived (method.DeclaringType) &&
				ConstructorTransforms.IsIntPtrCtor (method)) {
				method.Parameters [0].ParameterType = newNHandleTypeReference;
			} else {
				var nativeTypes = new List<bool> ();
				foreach (var parameter in method.Parameters) {
					nativeTypes.Clear ();
					if (TryReworkTypeReference (parameter.ParameterType, nativeTypes, out var newType)) {
						parameter.ParameterType = newType;
					}
				}

				nativeTypes.Clear ();
				if (TryReworkTypeReference (method.ReturnType, nativeTypes, out var newReturnType)) {
					method.ReturnType = newReturnType;
				} else if (IsHandleMethod (method)) {
					method.ReturnType = newNHandleTypeReference;
				}
			}

			return method;
		}

		static bool IsHandleMethod (MethodDefinition method)
		{
			return ConstructorTransforms.IsNSObjectDerived (method.DeclaringType) &&
				(method.Name == "get_ClassHandle" || method.Name == "get_Handle") &&
				method.ReturnType.ToString () == "System.IntPtr";
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

		static IEnumerable<MethodDefinition> PropMethods (PropertyDefinition prop)
		{
			if (prop.GetMethod is not null)
				yield return prop.GetMethod;
			if (prop.SetMethod is not null)
				yield return prop.SetMethod;
			foreach (var method in prop.OtherMethods)
				yield return method;
		}
	}
}
