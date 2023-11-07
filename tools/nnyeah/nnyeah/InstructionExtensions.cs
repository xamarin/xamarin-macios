using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Microsoft.MaciOS.Nnyeah {
	public static class InstructionExtensions {
		// The ctor for Instruction is internal and there is no clone/copy method,
		// therefore we have to drill through the types.
		public static Instruction Copy (this Instruction i)
		{
			if (i.Operand is null)
				return Instruction.Create (i.OpCode);
			switch (i.Operand) {
			case TypeReference tr:
				return Instruction.Create (i.OpCode, tr);
			case CallSite cs:
				return Instruction.Create (i.OpCode, cs);
			case MethodReference mr:
				return Instruction.Create (i.OpCode, mr);
			case FieldReference fr:
				return Instruction.Create (i.OpCode, fr);
			case string str:
				return Instruction.Create (i.OpCode, str);
			case sbyte sb:
				return Instruction.Create (i.OpCode, sb);
			case byte b:
				return Instruction.Create (i.OpCode, b);
			case int it:
				return Instruction.Create (i.OpCode, it);
			case long lg:
				return Instruction.Create (i.OpCode, lg);
			case float ft:
				return Instruction.Create (i.OpCode, ft);
			case double db:
				return Instruction.Create (i.OpCode, db);
			case Instruction instr:
				return Instruction.Create (i.OpCode, instr);
			case Instruction [] instrArr:
				return Instruction.Create (i.OpCode, instrArr);
			case VariableDefinition vd:
				return Instruction.Create (i.OpCode, vd);
			case ParameterDefinition pd:
				return Instruction.Create (i.OpCode, pd);
			default: // can't happen (in theory)
				throw new InvalidOperationException ();
			}
		}
	}
}

