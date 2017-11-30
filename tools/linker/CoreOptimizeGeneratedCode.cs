// Copyright 2012-2013, 2016 Xamarin Inc. All rights reserved.

using System;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Linker;
using Mono.Tuner;
using Xamarin.Bundler;

namespace Xamarin.Linker {

	public abstract class CoreOptimizeGeneratedCode : ExceptionalSubStep {

		protected override string Name { get; } = "Binding Optimizer";
		protected override int ErrorCode { get; } = 2020;

		protected bool HasGeneratedCode { get; private set; }
		protected bool IsExtensionType { get; private set; }
		protected bool ProcessMethods { get; private set; }

		public override SubStepTargets Targets {
			get { return SubStepTargets.Assembly | SubStepTargets.Type | SubStepTargets.Method; }
		}
		
		public override bool IsActiveFor (AssemblyDefinition assembly)
		{
			// we're sure "pure" SDK assemblies don't use XamMac.dll (i.e. they are the Product assemblies)
			if (Profile.IsSdkAssembly (assembly)) {
#if DEBUG
				Console.WriteLine ("Assembly {0} : skipped (SDK)", assembly);
#endif
				return false;
			}
			
			// process only assemblies where the linker is enabled (e.g. --linksdk, --linkskip) 
			AssemblyAction action = Annotations.GetAction (assembly);
			if (action != AssemblyAction.Link) {
#if DEBUG
				Console.WriteLine ("Assembly {0} : skipped ({1})", assembly, action);
#endif
				return false;
			}
			
			// if the assembly does not refer to [CompilerGeneratedAttribute] then there's not much we can do
			HasGeneratedCode = false;
			foreach (TypeReference tr in assembly.MainModule.GetTypeReferences ()) {
				if (tr.Is ("System.Runtime.CompilerServices", "CompilerGeneratedAttribute")) {
#if DEBUG
					Console.WriteLine ("Assembly {0} : processing", assembly);
#endif
					HasGeneratedCode = true;
					break;
				}
			}
#if DEBUG
			if (!HasGeneratedCode)
				Console.WriteLine ("Assembly {0} : no [CompilerGeneratedAttribute] present (applying basic optimizations)", assembly);
#endif
			// we always apply the step
			return true;
		}

		protected override void Process (TypeDefinition type)
		{
			// if 'type' inherits from NSObject inside an assembly that has [GeneratedCode]
			// or for static types used for optional members (using extensions methods), they can be optimized too
			IsExtensionType = type.IsSealed && type.IsAbstract && type.Name.EndsWith ("_Extensions", StringComparison.Ordinal);
			ProcessMethods = HasGeneratedCode || (!type.IsNSObject (LinkContext) && !IsExtensionType);
		}

		// [GeneratedCode] is not enough - e.g. it's used for anonymous delegates even if the 
		// code itself is not tool/compiler generated
		static protected bool IsExport (ICustomAttributeProvider provider)
		{
			return provider.HasCustomAttribute (Namespaces.Foundation, "ExportAttribute");
		}

		// less risky to nop-ify if branches are pointing to this instruction
		static protected void Nop (Instruction ins)
		{
			ins.OpCode = OpCodes.Nop;
			ins.Operand = null;
		}

		protected static bool ValidateInstruction (MethodDefinition caller, Instruction ins, string operation, Code expected)
		{
			if (ins.OpCode.Code != expected) {
				Driver.Log (1, "Could not {0} in {1} at offset {2}, expected {3} got {4}", operation, caller, ins.Offset, expected, ins);
				return false;
			}

			return true;
		}

		protected static bool ValidateInstruction (MethodDefinition caller, Instruction ins, string operation, params Code [] expected)
		{
			foreach (var code in expected) {
				if (ins.OpCode.Code == code)
					return true;
			}

			Driver.Log (1, "Could not {0} in {1} at offset {2}, expected any of [{3}] got {4}", operation, caller, ins.Offset, string.Join (", ", expected), ins);
			return false;
		}

		// Calculate the code blocks for both the true and false portion of a branch instruction.
		// Example:
		//   IL_0  brfalse IL_4
		//   IL_1  <true code block>
		//   IL_2  <true code block>
		//   IL_3  br IL_6
		//   IL_4  <false code block>
		//   IL_5  <false code block>
		//   IL_6  ret
		// equivalent to the following
		//   if (condition) {
		//     <true code block>
		//   } else {
		//     <false code block>
		//   }
		// return the ranges
		//    insTrueFirst: IL_1
		//    insTrueLast: IL_2
		//    insFalseFirst: IL_4
		//    insFalseLast: IL_5
		// 
		// The ins*Last instructions will be null if this is a condition without an 'else' block.
		protected static bool GetBranchRange (MethodDefinition caller, Instruction insBranch, string operation, out Instruction insTrueFirst, out Instruction insTrueLast, out Instruction insFalseFirst, out Instruction insFalseLast)
		{
			insTrueFirst = null;
			insTrueLast = null;
			insFalseFirst = null;
			insFalseLast = null;

			if (!ValidateInstruction (caller, insBranch, operation, Code.Brfalse, Code.Brfalse_S, Code.Brtrue, Code.Brtrue_S, Code.Bne_Un, Code.Bne_Un_S))
				return false;

			var branchTarget = (Instruction) insBranch.Operand;
			Instruction endTarget;

			switch (branchTarget.Previous.OpCode.Code) {
			case Code.Br:
			case Code.Br_S:
				endTarget = (Instruction) branchTarget.Previous.Operand;
				endTarget = endTarget.Previous;
				break;
			case Code.Ret:
			case Code.Throw:
			case Code.Rethrow:
				endTarget = branchTarget;
				while (endTarget.OpCode.FlowControl != FlowControl.Return && endTarget.OpCode.FlowControl != FlowControl.Throw)
					endTarget = endTarget.Next;
				break;
			case Code.Leave:
				if (caller.Body.ExceptionHandlers.Count != 1) {
					Driver.Log (1, "Could not {0} in {1} at offset {2} because there are not exactly 1 exception handlers (found {3})", operation, caller, insBranch.Offset, caller.Body.ExceptionHandlers.Count);
					return false;
				}
				endTarget = caller.Body.ExceptionHandlers [0].TryEnd.Previous;
				break;
			case Code.Call:
			case Code.Stsfld:
				// condition without 'else' clause
				// there are a lot more instructions that can go into this case statement, but keep a whitelist for now.
				endTarget = null;
				break;
			default:
				Driver.Log (1, "Could not {0} in {1} at offset {2} because the instruction before the branch target was unexpected ({3})", operation, caller, insBranch.Offset, branchTarget.Previous);
				return false;
			}
			switch (insBranch.OpCode.Code) {
			case Code.Brfalse:
			case Code.Brfalse_S:
			case Code.Bne_Un:
			case Code.Bne_Un_S:
				insTrueFirst = insBranch.Next;
				insTrueLast = branchTarget.Previous;
				insFalseFirst = branchTarget;
				insFalseLast = endTarget;
				break;
			case Code.Brtrue:
			case Code.Brtrue_S:
				insFalseFirst = insBranch.Next;
				insFalseLast = branchTarget.Previous;
				insTrueFirst = branchTarget;
				insTrueLast = endTarget;
				break;
			default:
				Driver.Log (1, "Could not {0} in {1} at offset {2} because the branch instruction was unexpected ({3})", operation, caller, insBranch.Offset, insBranch);
				return false;
			}

			return true;
		}

		// This method will clear out (Nop) the non-taken conditional code block, depending on the constant value 'constantValue'.
		// insBranch must be a conditional branch instruction (brtrue/brfalse/bne).
		// The caller must clear out the instructions that calculates the value loaded on the stack for the branch instruction.
		protected static bool InlineBranchCondition (MethodDefinition caller, string operation, Instruction insBranch, bool constantValue)
		{
			Instruction insTrueFirst;
			Instruction insTrueLast;
			Instruction insFalseFirst;
			Instruction insFalseLast;
			if (!GetBranchRange (caller, insBranch, operation, out insTrueFirst, out insTrueLast, out insFalseFirst, out insFalseLast))
				return false;

			Instruction first = !constantValue ? insTrueFirst : insFalseFirst;
			Instruction last = !constantValue ? insTrueLast : insFalseLast;

			if (last == null) {
				// This is a condition without an 'else' block, and the 'else' block is the one we'd remove,
				// which means we just have to clear the branch instruction itself
				Nop (insBranch);
				return true;
			}

			// Check if there are other branch instructions into the instructions that will be removed
			var instructions = caller.Body.Instructions;
			for (int i = 0; i < instructions.Count; i++) {
				var ins = instructions [i];
				if (ins == insBranch)
					continue;
				if (ins.Offset >= first.Offset && ins.Offset <= last.Offset)
					continue;

				switch (ins.OpCode.FlowControl) {
				case FlowControl.Branch:
				case FlowControl.Cond_Branch:
					var target = (Instruction) ins.Operand;
					if (target.Offset >= first.Offset && target.Offset <= last.Offset) {
						Driver.Log (1, "Could not {0} in {1} because there's a branch instruction elsewhere in the method that branches into the section of code to be removed.\n" + 
						            "\tFirst instruction to be removed: {2}\n" + 
						            "\tLast instruction to be removed: {3}\n" +
						            "\tOffending branch instruction: {4}\n" +
						            "\tBranching to: {5}", 
						            operation, caller, first, last, ins, target);
						return false;
					}
					break;
				default:
					continue;
				}
			}

			// We have the information we need, now we can start clearing instructions
			Nop (insBranch);
			Instruction current = first;
			do {
				Nop (current);
				if (current == last)
					break;
				current = current.Next;
			} while (true);

			return true;
		}
	}
}