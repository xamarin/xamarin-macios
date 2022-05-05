using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Microsoft.MaciOS.Nnyeah {
	public class FieldTransformations {
		static Dictionary<string, Transformation>? allTransforms;

		public Dictionary<string, Transformation> GetTransforms (ModuleDefinition module)
		{
			if (allTransforms is not null) {
				return allTransforms;
			}

			allTransforms = new Dictionary<string, Transformation> ();

			// nint
			var sizeReference = new MethodReference ("get_Size", module.TypeSystem.Int32, module.TypeSystem.IntPtr);
			allTransforms.Add ("System.Int32 System.nint::Size", new Transformation (
				"System.Int32 System.nint::Size",
				Instruction.Create (OpCodes.Call, sizeReference)
				));

			var maxReference = new MethodReference ("get_MaxValue", module.TypeSystem.IntPtr, module.TypeSystem.IntPtr);
			allTransforms.Add ("System.Int32 System.nint::MaxValue", new Transformation (
				"System.Int32 System.nint::MaxValue",
				Instruction.Create (OpCodes.Call, maxReference)
				));

			var minReference = new MethodReference ("get_MinValue", module.TypeSystem.IntPtr, module.TypeSystem.IntPtr);
			allTransforms.Add ("System.Int32 System.nint::MinValue", new Transformation (
				"System.Int32 System.nint::MinValue",
				Instruction.Create (OpCodes.Call, minReference)
				));

			// nuint
			sizeReference = new MethodReference ("get_Size", module.TypeSystem.Int32, module.TypeSystem.UIntPtr);
			allTransforms.Add ("System.Int32 System.nuint::Size", new Transformation (
				"System.Int32 System.nuint::Size",
				Instruction.Create (OpCodes.Call, sizeReference)
				));

			maxReference = new MethodReference ("get_MaxValue", module.TypeSystem.UIntPtr, module.TypeSystem.UIntPtr);
			allTransforms.Add ("System.Int32 System.nuint::MaxValue", new Transformation (
				"System.Int32 System.nuint::MaxValue",
				Instruction.Create (OpCodes.Call, maxReference)
				));

			minReference = new MethodReference ("get_MinValue", module.TypeSystem.UIntPtr, module.TypeSystem.UIntPtr);
			allTransforms.Add ("System.Int32 System.nuint::MinValue", new Transformation (
				"System.Int32 System.nuint::MinValue",
				Instruction.Create (OpCodes.Call, minReference)
				));

			var newNfloatModuleReference = new ModuleReference ("System.Private.CoreLib");
			var newNfloatTypeReference = new TypeReference ("System.Runtime.InteropServices",
				"NFloat", null, newNfloatModuleReference, true);

			sizeReference = new MethodReference ("get_Size", module.TypeSystem.Int32, newNfloatTypeReference);
			allTransforms.Add ("System.Int32 System.nfloat::Size", new Transformation (
				"System.Int32 System.nfloat::Size",
				Instruction.Create (OpCodes.Call, sizeReference)
				));

			maxReference = new MethodReference ("get_MaxValue", newNfloatTypeReference, newNfloatTypeReference);
			allTransforms.Add ("System.nfloat System.nfloat::MaxValue", new Transformation (
				"System.nfloat System.nfloat::MaxValue",
				Instruction.Create (OpCodes.Call, maxReference)
				));

			minReference = new MethodReference ("get_MinValue", newNfloatTypeReference, newNfloatTypeReference);
			allTransforms.Add ("System.nfloat System.nfloat::MinValue", new Transformation (
				"System.nfloat System.nfloat::MinValue",
				Instruction.Create (OpCodes.Call, minReference)
				));

			var epsilonReference = new MethodReference ("get_Epsilon", newNfloatTypeReference, newNfloatTypeReference);
			allTransforms.Add ("System.nfloat System.nfloat::Epsilon", new Transformation (
				"System.nfloat System.nfloat::Epsilon",
				Instruction.Create (OpCodes.Call, epsilonReference)
				));

			var nanReference = new MethodReference ("get_NaN", newNfloatTypeReference, newNfloatTypeReference);
			allTransforms.Add ("System.nfloat System.nfloat::NaN", new Transformation (
				"System.nfloat System.nfloat::NaN",
				Instruction.Create (OpCodes.Call, nanReference)
				));

			var infinityReference = new MethodReference ("get_NegativeInfinity", newNfloatTypeReference, newNfloatTypeReference);
			allTransforms.Add ("System.nfloat System.nfloat::NegativeInfinity", new Transformation (
				"System.nfloat System.nfloat::NegativeInfinity",
				Instruction.Create (OpCodes.Call, infinityReference)
				));

			infinityReference = new MethodReference ("get_PositiveInfinity", newNfloatTypeReference, newNfloatTypeReference);
			allTransforms.Add ("System.nfloat System.nfloat::PositiveInfinity", new Transformation (
				"System.nfloat System.nfloat::PositiveInfinity",
				Instruction.Create (OpCodes.Call, infinityReference)
				));

			var valReference = new MethodReference ("get_Value", module.TypeSystem.Double, newNfloatTypeReference);
			allTransforms.Add ("System.Double System.nfloat::v", new Transformation (
				"System.Double System.nfloat::v",
				Instruction.Create (OpCodes.Call, valReference)
				));

			return allTransforms;
		}
	}
}
