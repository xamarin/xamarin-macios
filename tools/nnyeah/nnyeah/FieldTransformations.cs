using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

#nullable enable

namespace nnyeah {
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

			return allTransforms;
		}
	}
}
