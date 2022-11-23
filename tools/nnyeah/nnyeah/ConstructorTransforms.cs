using System;
using Mono.Cecil;
using Mono.Cecil.Rocks;
using System.Linq;

using Microsoft.MaciOS.Nnyeah.AssemblyComparator;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.MaciOS.Nnyeah {
	// Converting constructors from Legacy to NET6 have some special concerns:
	//     - The arguments to NSObject's base class (IntPtr) and (IntPtr, bool) must be moved to NativeHandle, but _only_ if we're derived from NSObject
	//         - There are some absurd cases where people store off that IntPtr that we can't convert safely, so bail on non-trivial ctors
	//     - Invocations to said constructors need to be converted, but those should hopefully rare....
	public class ConstructorTransforms {
		// ObjCRuntime.NativeHandle
		TypeReference NewNativeHandleTypeDefinition;

		// System.Bool
		TypeReference BoolTypeDefinition;

		// NativeHandle::op_Implicit(IntPtr)
		MethodReference NativeHandleOpImplicit;

		EventHandler<WarningEventArgs>? WarningIssued;
		EventHandler<TransformEventArgs>? Transformed;

		Dictionary<string, MethodDefinition> TransformedConstructors = new Dictionary<string, MethodDefinition> ();

		public ConstructorTransforms (TypeReference newNativeHandleTypeDefinition, TypeReference boolTypeReference,
			MethodReference nativeHandleOpImplicit, EventHandler<WarningEventArgs>? warningIssued, EventHandler<TransformEventArgs>? transformed)
		{
			NewNativeHandleTypeDefinition = newNativeHandleTypeDefinition;
			WarningIssued = warningIssued;
			Transformed = transformed;

			// Get the definition of System.Bool from the ctor we already have
			BoolTypeDefinition = boolTypeReference; // intPtrCtorWithBool.Parameters[1].ParameterType;

			NativeHandleOpImplicit = nativeHandleOpImplicit;
		}

		public static bool IsIntPtrCtor (MethodDefinition d)
		{
			if (d.Name != ".ctor")
				return false;
			bool isSingle = d.Parameters.Count == 1 &&
				d.Parameters.First ().ParameterType.FullName == "System.IntPtr";
			bool isDouble = d.Parameters.Count == 2 &&
				d.Parameters.First ().ParameterType.FullName == "System.IntPtr" &&
				d.Parameters.Last ().ParameterType.FullName == "System.Boolean";
			return isSingle || isDouble;
		}

		public static bool IsNSObjectDerived (TypeReference? typeReference, ConstructorTransforms? self = null)
		{
			if (typeReference is null)
				return false;
			var initialTypeReference = typeReference;
			TypeReference? firstIssuedTypeReference = null;
			var reworkNeededOnTheWayUp = false;
			while (true) {
				if (typeReference is null) {
					return false;
				}
				if (typeReference.FullName == "Foundation.NSObject" || typeReference.FullName == "ObjCRuntime.DisposableObject") {
					if (reworkNeededOnTheWayUp && firstIssuedTypeReference is not null) {
						if (self is not null) {
							self.WarningIssued?.Invoke (self, new WarningEventArgs (initialTypeReference.DeclaringType.FullName,
								initialTypeReference.Name, "IntPtr", String.Format (Errors.N0009, initialTypeReference.FullName,
								firstIssuedTypeReference.FullName)));
						}
					}
					return true;
				}
				typeReference = typeReference.Resolve ().BaseType;
				if (!reworkNeededOnTheWayUp && typeReference is not null) {
					var typeDefinition = typeReference.Resolve ();
					if (typeDefinition is not null) {
						if (typeDefinition.Methods.Any (IsIntPtrCtor)) {
							if (typeDefinition.Module != initialTypeReference.Module) {
								reworkNeededOnTheWayUp = true;
								firstIssuedTypeReference = typeDefinition;
							}
						}
					}
				}
			}
		}

		public void ReworkAsNeeded (TypeDefinition definition)
		{
			// Rework code blocks _before_ fixing the constructors, so we can tell which are
			// invocations to the "old" IntPtr variant
			ReworkCodeBlockAsNeeded (definition.Properties.SelectMany (p => Reworker.PropMethods (p)));
			ReworkCodeBlockAsNeeded (definition.Methods);
			ReworkCodeBlockAsNeeded (definition.Events.SelectMany (e => Reworker.EventMethods (e)));

			if (IsNSObjectDerived (definition.BaseType, this)) {
				if (definition.GetConstructors ().FirstOrDefault (IsIntPtrCtor) is MethodDefinition ctor) {
					ctor.Parameters [0].ParameterType = NewNativeHandleTypeDefinition;
					Transformed?.Invoke (this, new TransformEventArgs (ctor.DeclaringType.FullName,
						ctor.Name, "IntPtr", 0, 0));
					TransformedConstructors.Add (ctor.ToString (), ctor);
				}
			}
		}

		// TODO - There is non-trivial overlap between this and the TryGetConstructorCallTransformation
		// codepath below. While it works, unification would be nice.
		void ReworkCodeBlockAsNeeded (IEnumerable<MethodDefinition> methods)
		{
			foreach (var method in methods) {
				ReworkCodeBlockAsNeeded (method);
			}
		}

		// (Instruction, Number of Ctor Arguments)
		List<(Instruction, int)> FindConstructorInstruction (IList<Instruction> instructions)
		{
			var index = new List<(Instruction, int)> ();
			for (int i = 0; i < instructions.Count; i++) {
				Instruction instruction = instructions [i];
				if (instruction.OpCode.Code == Code.Newobj && instruction.Operand is MethodDefinition invokedMethod) {
					if (invokedMethod.IsConstructor && IsNSObjectDerived (invokedMethod.DeclaringType, this)) {
						switch (invokedMethod.Parameters.Count) {
						case 1: {
							if (invokedMethod.Parameters [0].ParameterType.ToString () == "System.IntPtr") {
								index.Add ((instruction, 1));
							}
							break;
						}
						case 2: {
							if (invokedMethod.Parameters [0].ParameterType.ToString () == "System.IntPtr" &&
								invokedMethod.Parameters [1].ParameterType.ToString () == "System.Boolean") {
								index.Add ((instruction, 2));
							}
							break;
						}
						}
					}
				}
			}
			// Order last instruction first so processing one doesn't offset later elements
			index.Reverse ();
			return index;
		}

		void ReworkCodeBlockAsNeeded (MethodDefinition method)
		{
			if (method.Body is null)
				return;
			var processor = method.Body.GetILProcessor ();
			foreach ((Instruction instruction, int argCount) in FindConstructorInstruction (method.Body.Instructions)) {
				switch (argCount) {
				case 1:
					processor.InsertBefore (instruction, Instruction.Create (OpCodes.Call, NativeHandleOpImplicit));
					break;
				case 2: {
					var variable = new VariableDefinition (BoolTypeDefinition);
					method.Body.Variables.Add (variable);
					processor.InsertBefore (instruction, Instruction.Create (OpCodes.Stloc, variable));
					processor.InsertBefore (instruction, Instruction.Create (OpCodes.Call, NativeHandleOpImplicit));
					processor.InsertBefore (instruction, Instruction.Create (OpCodes.Ldloc, variable));
					break;
				}
				}
			}
		}

		public bool TryGetConstructorCallTransformation (Instruction instruction, [NotNullWhen (returnValue: true)] out Transformation? result)
		{
			if (instruction.OpCode == OpCodes.Newobj &&
				instruction.Operand is MethodReference invokedMethod &&
				TransformedConstructors.TryGetValue (invokedMethod.ToString (), out var originalCtor)) {
				switch (originalCtor.Parameters.Count) {
				case 1:
					result = new Transformation (instruction.ToString (), TransformationAction.Insert, new List<Instruction> () {
						Instruction.Create (OpCodes.Call, NativeHandleOpImplicit)
					});
					break;
				case 2:
					result = new Transformation (instruction.ToString (), TransformationAction.Insert, new List<Instruction> () {
						Instruction.Create (OpCodes.Stloc, new VariableDefinition (BoolTypeDefinition)),
						Instruction.Create (OpCodes.Call, NativeHandleOpImplicit),
						Instruction.Create (OpCodes.Ldloc, new VariableDefinition (BoolTypeDefinition)),
					});
					break;
				default:
					result = null;
					return false;
				}
				return true;
			}
			result = null;
			return false;
		}
	}
}
