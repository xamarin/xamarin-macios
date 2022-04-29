using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;

namespace Microsoft.MaciOS.Nnyeah {
	public class Transformation {
		static List<Instruction> emptyInstructions = new List<Instruction> ();
		public Transformation (string operand, TransformationAction action, List<Instruction> instructions)
		{
			Operand = operand;
			Action = action;
			Instructions = new List<Instruction> (instructions.Count);
			Instructions.AddRange (instructions);
		}

		public Transformation (string operand)
			: this (operand, TransformationAction.Remove, emptyInstructions)
		{
		}

		public Transformation (string operand, Instruction instr)
			: this (operand, TransformationAction.Replace, new List<Instruction> () { instr })
		{
		}

		public Transformation (string operand, string warningMessage)
		{
			Operand = operand;
			Action = TransformationAction.Warn;
			Instructions = new List<Instruction> ();
			Message = warningMessage;
		}

		public string Operand { get; private set; }
		public TransformationAction Action { get; private set; }
		public List<Instruction> Instructions { get; private set; }
		public string? Message { get; private set; }

		public bool TryPerformTransform (Instruction old, MethodBody body)
		{
			var il = body.GetILProcessor ();
			switch (Action) {
			case TransformationAction.Remove:
				il.Remove (old);
				break;
			case TransformationAction.Insert:
				foreach (var instr in Instructions) {
					il.InsertBefore (old, AddVariableIfNeeded (body, instr));
				}
				break;
			case TransformationAction.Append:
				foreach (var instr in Enumerable.Reverse (Instructions)) {
					il.InsertAfter (old, AddVariableIfNeeded (body, instr));
				}
				break;
			case TransformationAction.Replace:
				foreach (var instr in Instructions) {
					il.InsertBefore (old, AddVariableIfNeeded (body, instr));
				}
				il.Remove (old);
				break;
			case TransformationAction.Warn:
				return false;
			}
			return true;
		}

		static Instruction AddVariableIfNeeded (MethodBody body, Instruction instr)
		{
			if (instr.Operand is VariableDefinition variable && !body.Variables.Contains (variable))
				body.Variables.Add (variable);

			return instr;
		}
	}
}
