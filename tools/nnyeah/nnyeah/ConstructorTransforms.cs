using System;
using Mono.Cecil;
using Mono.Cecil.Rocks;
using System.Linq;

using Microsoft.MaciOS.Nnyeah.AssemblyComparator;
using System.Collections.Generic;
using Mono.Cecil.Cil;

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

		// Foundation.NSObject::.ctor(IntPtr)
		MethodDefinition IntPtrCtor;

		// Foundation.NSObject::.ctor(IntPtr, bool)
		MethodDefinition IntPtrCtorWithBool;

		// NativeHandle::op_Implicit(IntPtr)
		MethodDefinition NativeHandleOpImplicit;

		EventHandler<WarningEventArgs>? WarningIssued;
		EventHandler<TransformEventArgs>? Transformed;

		public ConstructorTransforms (TypeReference newNativeHandleTypeDefinition, MethodDefinition intPtrCtor, MethodDefinition intPtrCtorWithBool,
			EventHandler<WarningEventArgs>? warningIssued, EventHandler<TransformEventArgs>? transformed)
		{
			NewNativeHandleTypeDefinition = newNativeHandleTypeDefinition;
			IntPtrCtor = intPtrCtor;
			IntPtrCtorWithBool = intPtrCtorWithBool;
			WarningIssued = warningIssued;
			Transformed = transformed;

			// Get the definition of System.Bool from the ctor we already have
			BoolTypeDefinition = intPtrCtorWithBool.Parameters[1].ParameterType;

			NativeHandleOpImplicit = NewNativeHandleTypeDefinition.Resolve ().GetMethods ().First (m => m.FullName == "ObjCRuntime.NativeHandle ObjCRuntime.NativeHandle::op_Implicit(System.IntPtr)");
		}

		public void AddTransforms (TypeAndMemberMap moduleMap)
		{
			// Remove "NSObject (IntPtr)" from missing list and add "NSObject (NativeHandle)"
			const string IntPtrCtorSignature = "System.Void Foundation.NSObject::.ctor(System.IntPtr)";
			moduleMap.MethodsNotPresent.Remove (IntPtrCtorSignature);
			IntPtrCtor.Parameters [0].ParameterType = NewNativeHandleTypeDefinition;
			moduleMap.MethodMap.Add (IntPtrCtorSignature, IntPtrCtor);

			// Remove "NSObject (IntPtr, bool)" from missing list and add "NSObject (NativeHandle,System.Boolean)"
			const string IntPtrBoolCtorSignature = "System.Void Foundation.NSObject::.ctor(System.IntPtr,System.Boolean)";
			moduleMap.MethodsNotPresent.Remove (IntPtrBoolCtorSignature);
			IntPtrCtorWithBool.Parameters [0].ParameterType = NewNativeHandleTypeDefinition;
			moduleMap.MethodMap.Add (IntPtrBoolCtorSignature, IntPtrCtorWithBool);
		}

		static bool IsIntPtrCtor (MethodDefinition d)
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

		bool IsNSObjectDerived (TypeReference? typeReference)
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
				if (typeReference.FullName == "Foundation.NSObject") {
					if (reworkNeededOnTheWayUp && firstIssuedTypeReference is not null) {
						WarningIssued?.Invoke (this, new WarningEventArgs (initialTypeReference.DeclaringType.FullName,
							initialTypeReference.Name, "IntPtr", String.Format (Errors.N0009, initialTypeReference.FullName,
							firstIssuedTypeReference.FullName)));
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

			if (IsNSObjectDerived (definition.BaseType)) {
				if (definition.GetConstructors ().FirstOrDefault (IsIntPtrCtor) is MethodDefinition ctor) {
					// How many instructions it takes to invoke a 1 or 2 param base ctor
					// We can not safely process things that might store off the IntPtr
					// or other such insanity, so just fail fast
					if (ctor.Body.Instructions.Count > 7) {
						throw new ConversionException (Errors.E0016, definition);
					}
					ctor.Parameters [0].ParameterType = NewNativeHandleTypeDefinition;
					Transformed?.Invoke (this, new TransformEventArgs (ctor.DeclaringType.FullName,
						ctor.Name, "IntPtr", 0, 0));
				}
			}

		}

		void ReworkCodeBlockAsNeeded (IEnumerable<MethodDefinition> methods)
		{
			foreach (var method in methods) {
				ReworkCodeBlockAsNeeded (method);
			}
		}

		// (Instruction Index, Number of Ctor Arguments)
		List<(int, int)> FindConstructorInstructionIndex (IList<Instruction> instructions)
		{
			var index = new List<(int, int)> ();
			for (int i = 0 ; i < instructions.Count ; i++) {
				Instruction instruction = instructions[i];
				if (instruction.OpCode.Code == Code.Newobj && instruction.Operand is MethodDefinition invokedMethod) {
					if (invokedMethod.IsConstructor && IsNSObjectDerived (invokedMethod.DeclaringType)) {
						switch (invokedMethod.Parameters.Count) {
						case 1: {
							if (invokedMethod.Parameters[0].ParameterType.ToString () == "System.IntPtr") {
								index.Add ((i, 1));
							}
							break;
						}
						case 2: {
							if (invokedMethod.Parameters[0].ParameterType.ToString () == "System.IntPtr" &&
								invokedMethod.Parameters[1].ParameterType.ToString () == "System.Boolean") {
								index.Add ((i, 2));
							}
							break;
						}
						default:
							throw new ConversionException (Errors.E0017, invokedMethod, invokedMethod.DeclaringType);
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
			foreach ((int index, int argCount) in FindConstructorInstructionIndex (method.Body.Instructions)) {
				switch (argCount) {
					case 1:
						method.Body.Instructions.Insert (index, Instruction.Create (OpCodes.Call, NativeHandleOpImplicit));
						break;
					case 2: {
						var variable = new VariableDefinition (BoolTypeDefinition);
						method.Body.Variables.Add (variable);
						method.Body.Instructions.Insert (index, Instruction.Create (OpCodes.Ldloc, variable));
						method.Body.Instructions.Insert (index, Instruction.Create (OpCodes.Call, NativeHandleOpImplicit));
						method.Body.Instructions.Insert (index, Instruction.Create (OpCodes.Stloc, variable));
						break;
					}
					default:
						throw new ConversionException (Errors.E0017, method, method.DeclaringType);
				}
			}
		}
	}
}
