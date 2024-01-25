using System;
using System.Collections.Generic;
using System.Linq;

using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

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
			var instructionsCopy = CopyInstructions ();
			var il = body.GetILProcessor ();
			body.SimplifyMacros ();
			switch (Action) {
			case TransformationAction.Remove:
				var @new = Instruction.Create (OpCodes.Nop);
				il.InsertAfter (old, @new);
				PatchReferences (body, old, @new);
				il.Remove (old);
				break;
			case TransformationAction.Insert:
				foreach (var instr in instructionsCopy) {
					il.InsertBefore (old, AddVariableIfNeeded (body, instr));
				}
				break;
			case TransformationAction.Append:
				foreach (var instr in Enumerable.Reverse (instructionsCopy)) {
					il.InsertAfter (old, AddVariableIfNeeded (body, instr));
				}
				break;
			case TransformationAction.Replace:
				foreach (var instr in Enumerable.Reverse (instructionsCopy)) {
					il.InsertAfter (old, AddVariableIfNeeded (body, instr));
				}
				var newInstr = instructionsCopy [0];
				PatchReferences (body, old, @newInstr);
				il.Remove (old);
				break;
			case TransformationAction.Warn:
				body.OptimizeMacros ();
				return false;
			}
			body.OptimizeMacros ();
			return true;
		}

		List<Instruction> CopyInstructions ()
		{
			var instrs = new List<Instruction> (Instructions.Count);
			instrs.AddRange (Instructions.Select (i => i.Copy ()));
			return instrs;
		}

		static void PatchReferences (MethodBody body, Instruction old, Instruction @new)
		{
			foreach (var instruction in body.Instructions) {
				if (instruction.Operand is Instruction target && target == old) {
					instruction.Operand = @new;
				} else if (instruction.Operand is Instruction [] targets) {
					for (int i = 0; i < targets.Length; i++) {
						if (targets [i] == old) {
							targets [i] = @new;
						}
					}
				}
			}
		}

		static Instruction AddVariableIfNeeded (MethodBody body, Instruction instr)
		{
			if (instr.Operand is VariableDefinition variable && !body.Variables.Contains (variable))
				body.Variables.Add (variable);

			return instr;
		}
	}
}
