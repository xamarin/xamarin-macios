// Copyright 2012-2013, 2016 Xamarin Inc. All rights reserved.

using System;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Linker;
using Mono.Tuner;
using Xamarin.Bundler;

using MonoTouch.Tuner;
#if MONOMAC
using MonoMac.Tuner;
#endif

namespace Xamarin.Linker {

	public abstract class CoreOptimizeGeneratedCode : ExceptionalSubStep {
		// If the type currently being processed is a direct binding or not.
		// A null value means it's not a constant value, and can't be inlined.
		bool? isdirectbinding_constant;

		protected override string Name { get; } = "Binding Optimizer";
		protected override int ErrorCode { get; } = 2020;

		protected bool HasOptimizableCode { get; private set; }
		protected bool IsExtensionType { get; private set; }

		protected LinkerOptions Options { get; set; }

		public bool IsDualBuild {
			get { return Options.Application.IsDualBuild; }
		}

		public int Arch {
			get { return Options.Target.Is64Build ? 8 : 4; }
		}

		protected Optimizations Optimizations {
			get {
				return Options.Application.Optimizations;
			}
		}

		// This is per assembly, so we set it in 'void Process (AssemblyDefinition)'
		bool InlineIntPtrSize { get; set; }

		public CoreOptimizeGeneratedCode (LinkerOptions options)
		{
			Options = options;
		}

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
			HasOptimizableCode = false;
			foreach (TypeReference tr in assembly.MainModule.GetTypeReferences ()) {
				if (tr.Is ("System.Runtime.CompilerServices", "CompilerGeneratedAttribute")) {
#if DEBUG
					Console.WriteLine ("Assembly {0} : processing", assembly);
#endif
					HasOptimizableCode = true;
					break;
				} else if (tr.Is (Namespaces.ObjCRuntime, "BindingImplAttribute")) {
					HasOptimizableCode = true;
					break;
				}
			}
#if DEBUG
			if (!HasOptimizableCode)
				Console.WriteLine ("Assembly {0} : no [CompilerGeneratedAttribute] nor [BindingImplAttribute] present (applying basic optimizations)", assembly);
#endif
			// we always apply the step
			return true;
		}

		protected override void Process (TypeDefinition type)
		{
			if (!HasOptimizableCode)
				return;

			isdirectbinding_constant = type.IsNSObject (LinkContext) ? type.GetIsDirectBindingConstant (LinkContext) : null;

			// if 'type' inherits from NSObject inside an assembly that has [GeneratedCode]
			// or for static types used for optional members (using extensions methods), they can be optimized too
			IsExtensionType = type.IsSealed && type.IsAbstract && type.Name.EndsWith ("_Extensions", StringComparison.Ordinal);
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

		static int? GetConstantValue (Instruction ins)
		{
			if (ins == null)
				return null;
			
			switch (ins.OpCode.Code) {
			case Code.Ldc_I4_0:
				return 0;
			case Code.Ldc_I4_1:
				return 1;
			case Code.Ldc_I4_2:
				return 2;
			case Code.Ldc_I4_3:
				return 3;
			case Code.Ldc_I4_4:
				return 4;
			case Code.Ldc_I4_5:
				return 5;
			case Code.Ldc_I4_6:
				return 6;
			case Code.Ldc_I4_7:
				return 7;
			case Code.Ldc_I4_8:
				return 8;
			case Code.Ldc_I4:
			case Code.Ldc_I4_S:
				if (ins.Operand is int)
					return (int) ins.Operand;
				if (ins.Operand is sbyte)
					return (sbyte) ins.Operand;
				return null;
#if DEBUG
			case Code.Isinst: // We might be able to calculate a constant value here in the future
			case Code.Ldloc:
			case Code.Ldloca:
			case Code.Ldloc_0:
			case Code.Ldloc_1:
			case Code.Ldloc_2:
			case Code.Ldloc_3:
			case Code.Ldloc_S:
			case Code.Ldloca_S:
			case Code.Ldarg:
			case Code.Ldarg_0:
			case Code.Ldarg_1:
			case Code.Ldarg_2:
			case Code.Ldarg_3:
			case Code.Ldarg_S:
			case Code.Ldarga:
			case Code.Call:
			case Code.Calli:
			case Code.Callvirt:
			case Code.Box:
			case Code.Ldsfld:
				return null; // just to not hit the CWL below
#endif
			default:
#if DEBUG
				Driver.Log (9, "Unknown conditional instruction: {0}", ins);
#endif
				return null;
			}
		}

		static bool MarkInstructions (MethodDefinition method, Mono.Collections.Generic.Collection<Instruction> instructions, bool [] reachable, int start, int end)
		{
			if (reachable [start])
				return true; // We've already marked this section of code

			for (int i = start; i < end; i++) {
				if (instructions [i].OpCode.Code != Code.Nop) {
					// Not marking nop instructios makes it possible to remove catch clauses if 
					// the protected region is only nops (or empty).
					reachable [i] = true;
				}

				var ins = instructions [i];
				switch (ins.OpCode.FlowControl) {
				case FlowControl.Branch:
					// Unconditional branch, we continue marking from the instruction that we branch to.
					var br_target = (Instruction) ins.Operand;
					return MarkInstructions (method, instructions, reachable, instructions.IndexOf (br_target), end);
				case FlowControl.Cond_Branch:
					// Conditional instruction, we need to check if we can calculate a constant value for the condition
					var cond_target = (Instruction) ins.Operand;
					bool? branch = null; // did we get a constant value for the condition, and if so, did we branch or not?
					var cond_instruction_count = 0; // The number of instructions that compose the condition

					switch (ins.OpCode.Code) {
					case Code.Brtrue:
					case Code.Brtrue_S: {
							var v = GetConstantValue (ins?.Previous);
							if (v.HasValue)
								branch = v.Value != 0;
							cond_instruction_count = 2;
							break;
						}
					case Code.Brfalse:
					case Code.Brfalse_S: {
							var v = GetConstantValue (ins?.Previous);
							if (v.HasValue)
								branch = v.Value == 0;
							cond_instruction_count = 2;
							break;
						}
					case Code.Beq:
					case Code.Beq_S: {
							var x1 = GetConstantValue (ins?.Previous?.Previous);
							var x2 = GetConstantValue (ins?.Previous);
							if (x1.HasValue && x2.HasValue)
								branch = x1.Value == x2.Value;
							cond_instruction_count = 3;
							break;
						}
					case Code.Bne_Un:
					case Code.Bne_Un_S: {
							var x1 = GetConstantValue (ins?.Previous?.Previous);
							var x2 = GetConstantValue (ins?.Previous);
							if (x1.HasValue && x2.HasValue)
								branch = x1.Value != x2.Value;
							cond_instruction_count = 3;
							break;
						}
					case Code.Ble:
					case Code.Ble_S:
					case Code.Ble_Un:
					case Code.Ble_Un_S: {
							var x1 = GetConstantValue (ins?.Previous?.Previous);
							var x2 = GetConstantValue (ins?.Previous);
							if (x1.HasValue && x2.HasValue)
								branch = x1.Value <= x2.Value;
							cond_instruction_count = 3;
							break;
						}
					case Code.Blt:
					case Code.Blt_S:
					case Code.Blt_Un:
					case Code.Blt_Un_S: {
							var x1 = GetConstantValue (ins?.Previous?.Previous);
							var x2 = GetConstantValue (ins?.Previous);
							if (x1.HasValue && x2.HasValue)
								branch = x1.Value < x2.Value;
							cond_instruction_count = 3;
							break;
						}
					case Code.Bge:
					case Code.Bge_S:
					case Code.Bge_Un:
					case Code.Bge_Un_S: {
							var x1 = GetConstantValue (ins?.Previous?.Previous);
							var x2 = GetConstantValue (ins?.Previous);
							if (x1.HasValue && x2.HasValue)
								branch = x1.Value >= x2.Value;
							cond_instruction_count = 3;
							break;
						}
					case Code.Bgt:
					case Code.Bgt_S:
					case Code.Bgt_Un:
					case Code.Bgt_Un_S: {
							var x1 = GetConstantValue (ins?.Previous?.Previous);
							var x2 = GetConstantValue (ins?.Previous);
							if (x1.HasValue && x2.HasValue)
								branch = x1.Value > x2.Value;
							cond_instruction_count = 3;
							break;
						}
					default:
						Driver.Log ($"Can't optimize {0} because of unknown branch instruction: {1}", method, ins);
						break;
					}

					if (branch.HasValue) {
						// Make sure nothing else in the method branches into the middle of our supposedly constant condition,
						// bypassing our constant calculation. Note that it's not a bad to branch to the _first_ instruction in
						// the sequence (thus the +2 here), just into the middle of it.
						if (AnyBranchTo (instructions, instructions [i - cond_instruction_count + 2], ins))
							branch = null;
					}

					if (!branch.HasValue) {
						// not constant, continue marking both this code sequence and the branched sequence
						if (!MarkInstructions (method, instructions, reachable, instructions.IndexOf (cond_target), end))
							return false;
					} else {
						// we can remove the branch (and the code that loads the condition), so we mark those instructions as dead.
						for (int a = 0; a < cond_instruction_count; a++)
							reachable [i - a] = false;

						// Now continue marking according to whether we branched or not
						if (branch.Value) {
							// branch always taken
							return MarkInstructions (method, instructions, reachable, instructions.IndexOf (cond_target), end);
						} else {
							// branch never taken
							// continue looping
						}
					}
					break;
				case FlowControl.Call:
				case FlowControl.Next:
					// Nothing special, continue marking
					break;
				case FlowControl.Return:
				case FlowControl.Throw:
					// Control flow returns here, so stop marking
					return true;
				case FlowControl.Break:
				case FlowControl.Meta:
				case FlowControl.Phi:
				default:
					Driver.Log (4, "Can't optimize {0} because of unknown flow control for: {1}", method, ins);
					return false;
				}
			}

			return true;
		}

		// Check if there are any branches in the instructions that branch to anywhere between 'first' and 'last' instructions (both inclusive).
		static bool AnyBranchTo (Mono.Collections.Generic.Collection<Instruction> instructions, Instruction first, Instruction last)
		{
			if (first.Offset > last.Offset) {
				Driver.Log ($"Broken assumption: {first} is after {last}");
				return true; // This is the safe thing to do, since it will prevent inlining
			}

			for (int i = 0; i < instructions.Count; i++) {
				var ins = instructions [i];
				switch (ins.OpCode.FlowControl) {
				case FlowControl.Branch:
				case FlowControl.Cond_Branch:
					var target = ins.Operand as Instruction;
					if (target != null && target.Offset >= first.Offset && target.Offset <= last.Offset)
						return true;
					break;
				}
			}

			return false;
		}

		protected void EliminateDeadCode (MethodDefinition caller)
		{
			if (Optimizations.DeadCodeElimination != true)
				return;

			var instructions = caller.Body.Instructions;
			var reachable = new bool [instructions.Count];

			// We walk the instructions in the method, starting with the first instruction,
			// marking all reachable instructions. Any non-reachable instructions at the end
			// can be removed.

			if (!MarkInstructions (caller, instructions, reachable, 0, instructions.Count))
				return;

			// Handle exception handlers specially, they do not follow normal code flow.
			if (caller.Body.HasExceptionHandlers) {
				foreach (var eh in caller.Body.ExceptionHandlers) {
					switch (eh.HandlerType) {
					case ExceptionHandlerType.Catch:
						// We don't need catch handlers if the protected region does not have any reachable instructions.
						var startI = instructions.IndexOf (eh.TryStart);
						var endI = instructions.IndexOf (eh.TryEnd);
						var anyReachable = false;
						for (int i = startI; i < endI; i++) {
							if (reachable [i]) {
								anyReachable = true;
								break;
							}
						}
						if (anyReachable) {
							if (!MarkInstructions (caller, instructions, reachable, instructions.IndexOf (eh.HandlerStart), instructions.IndexOf (eh.HandlerEnd)))
								return;
						}
						break;
					case ExceptionHandlerType.Finally:
						// finally clauses are always executed, even if the protected region is empty
						if (!MarkInstructions (caller, instructions, reachable, instructions.IndexOf (eh.HandlerStart), instructions.IndexOf (eh.HandlerEnd)))
							return;
						break;
					case ExceptionHandlerType.Fault:
					case ExceptionHandlerType.Filter:
						// FIXME: and until fixed, exit gracefully without doing anything
						Driver.Log (4, "Unhandled exception handler: {0}, skipping dead code elimination for {1}", eh.HandlerType, caller);
						return;
					}
				}
			}

			if (Array.IndexOf (reachable, false) == -1)
				return; // entire method is reachable

			// Kill branch instructions when there are only dead instructions between the branch instruction and the target of the branch
			for (int i = 0; i < instructions.Count; i++) {
				if (!reachable [i])
					continue;

				var ins = instructions [i];
				if (ins.OpCode.Code != Code.Br && ins.OpCode.Code != Code.Br_S)
					continue;
				var target = ins.Operand as Instruction;
				if (target == null)
					continue;
				if (target.Offset < ins.Offset)
					continue; // backwards branch, keep those

				var start = i + 1;
				var end = instructions.IndexOf (target);
				var any_reachable = false;
				for (int k = start; k < end; k++) {
					if (reachable [k]) {
						any_reachable = true;
						break;
					}
				}
				if (any_reachable)
					continue;

				// The branch instruction just branches over unreachable instructions, so it can be considered unreachable too.
				reachable [i] = false;
			}

			// Check if there are unreachable instructions at the end.
			var last_reachable = Array.LastIndexOf (reachable, true);
			if (last_reachable < reachable.Length - 1) {
				// There are unreachable instructions at the end.
				// We must verify that there are no branches into these instructions.
				// In theory there shouldn't be any (if there are branches into these instructions,
				// they're reachable), but let's still verify just in case.
				var last_reachable_offset = instructions [last_reachable].Offset;
				for (int i = 0; i < last_reachable; i++) {
					if (!reachable [i])
						continue; // Unreachable instructions don't branch anywhere, because they'll be removed.
					var ins = instructions [i];
					switch (ins.OpCode.FlowControl) {
					case FlowControl.Break:
					case FlowControl.Cond_Branch:
						var target = (Instruction) ins.Operand;
						if (target.Offset > last_reachable_offset) {
							Driver.Log (4, "Can't optimize {0} because of branching beyond last instruction alive: {1}", caller, ins);
							return;
						}
						break;
					}
				}
			}
#if TRACE
			Console.WriteLine ($"{caller.FullName}:");
			for (int i = 0; i < reachable.Length; i++) {
				Console.WriteLine ($"{(reachable [i] ? "   " : "-  ")} {instructions [i]}");
				if (!reachable [i])
					Nop (instructions [i]);
			}
			Console.WriteLine ();
#endif

			// Exterminate, exterminate, exterminate
			for (int i = 0; i < reachable.Length; i++) {
				if (!reachable [i])
					Nop (instructions [i]);
			}

			// Remove unreachable instructions (nops) at the end, because the last instruction can only be ret/throw/backwards branch.
			for (int i = last_reachable + 1; i < reachable.Length; i++)
				instructions.RemoveAt (last_reachable + 1);
		}

		protected override void Process (AssemblyDefinition assembly)
		{
			// The "get_Size" is a performance (over size) optimization.
			// It always makes sense for platform assemblies because:
			// * Xamarin.TVOS.dll only ship the 64 bits code paths (all 32 bits code is extra weight better removed)
			// * Xamarin.WatchOS.dll only ship the 32 bits code paths (all 64 bits code is extra weight better removed)
			// * Xamarin.iOS.dll  ship different 32/64 bits versions of the assembly anyway (nint... support)
			//   Each is better to be optimized (it will be smaller anyway)
			//
			// However for fat (32/64) apps (i.e. iOS only right now) the optimization can duplicate the assembly
			// (metadata) for 3rd parties binding projects, increasing app size for very minimal performance gains.
			// For non-fat apps (the AppStore allows 64bits only iOS apps) then it's better to be applied
			//
			// TODO: we could make this an option "optimize for size vs optimize for speed" in the future
			if (Optimizations.InlineIntPtrSize.HasValue) {
				InlineIntPtrSize = Optimizations.InlineIntPtrSize.Value;
			} else if (!IsDualBuild) {
				InlineIntPtrSize = true;
			} else {
				InlineIntPtrSize = (Profile.Current as BaseProfile).ProductAssembly == assembly.Name.Name;
			}
			Driver.Log (4, "Optimization 'inline-intptr-size' enabled for assembly '{0}'.", assembly.Name);

			base.Process (assembly);
		}

		protected override void Process (MethodDefinition method)
		{
			if (!method.HasBody)
				return;

			if (method.IsGeneratedCode (LinkContext) && (IsExtensionType || IsExport (method))) {
				// We optimize methods that have the [GeneratedCodeAttribute] and is either an extension type or an exported method
			} else if (method.IsBindingImplOptimizableCode (LinkContext)) {
				// We optimize all methods that have the [BindingImpl (BindingImplAttributes.Optimizable)] attribute.
			} else {
				// but it would be too risky to apply on user-generated code
				return;
			}

			var instructions = method.Body.Instructions;
			for (int i = 0; i < instructions.Count; i++) {
				var ins = instructions [i];
				switch (ins.OpCode.Code) {
				case Code.Call:
					ProcessCalls (method, ins);
					break;
				case Code.Ldsfld:
					ProcessLoadStaticField (method, ins);
					break;
				}
			}

			EliminateDeadCode (method);
		}

		protected virtual void ProcessCalls (MethodDefinition caller, Instruction ins)
		{
			var mr = ins.Operand as MethodReference;
			switch (mr?.Name) {
			case "EnsureUIThread":
				ProcessEnsureUIThread (caller, ins);
				break;
			case "get_Size":
				ProcessIntPtrSize (caller, ins);
				break;
			case "get_IsDirectBinding":
				ProcessIsDirectBinding (caller, ins);
				break;
			}
		}

		protected virtual void ProcessLoadStaticField (MethodDefinition caller, Instruction ins)
		{
		}

		void ProcessEnsureUIThread (MethodDefinition caller, Instruction ins)
		{
#if MONOTOUCH
			const string operation = "remove calls to UIApplication::EnsureUIThread";
#else
			const string operation = "remove calls to NSApplication::EnsureUIThread";
#endif

			if (Optimizations.RemoveUIThreadChecks != true)
				return;

			// Verify we're checking the right get_EnsureUIThread call
			var mr = ins.Operand as MethodReference;
#if MONOTOUCH
			if (!mr.DeclaringType.Is (Namespaces.UIKit, "UIApplication"))
				return;
#else
			if (!mr.DeclaringType.Is (Namespaces.AppKit, "NSApplication"))
				return;
#endif

			// Verify a few assumptions before doing anything
			if (!ValidateInstruction (caller, ins, operation, Code.Call))
				return;

			// This is simple: just remove the call
			Nop (ins); // call void UIKit.UIApplication::EnsureUIThread()
		}

		void ProcessIntPtrSize (MethodDefinition caller, Instruction ins)
		{
			if (!InlineIntPtrSize)
				return;

			// This will inline IntPtr.Size to load the corresponding constant value instead

			// Verify we're checking the right get_Size call
			var mr = ins.Operand as MethodReference;
			if (!mr.DeclaringType.Is ("System", "IntPtr"))
				return;

			// We're fine, inline the get_Size call
			ins.OpCode = Arch == 8 ? OpCodes.Ldc_I4_8 : OpCodes.Ldc_I4_4;
			ins.Operand = null;
		}

		void ProcessIsDirectBinding (MethodDefinition caller, Instruction ins)
		{
			const string operation = "inline IsDirectBinding";

			if (Optimizations.InlineIsDirectBinding != true)
				return;

			// If we don't know the constant isdirectbinding value, then we can't inline anything
			if (!isdirectbinding_constant.HasValue)
				return;

			// Verify we're checking the right get_IsDirectBinding call
			var mr = ins.Operand as MethodReference;
			if (!mr.DeclaringType.Is (Namespaces.Foundation, "NSObject"))
				return;

			// Verify a few assumptions before doing anything
			if (!ValidateInstruction (caller, ins.Previous, operation, Code.Ldarg_0))
				return;

			if (!ValidateInstruction (caller, ins, operation, Code.Call))
				return;

			// Clearing the branch succeeded, so clear the condition too
			// ldarg.0
			Nop (ins.Previous);
			// call System.Boolean Foundation.NSObject::get_IsDirectBinding()
			ins.OpCode = isdirectbinding_constant.Value ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0;
			ins.Operand = null;
		}
	}
}