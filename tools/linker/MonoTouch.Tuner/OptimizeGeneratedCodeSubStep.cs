// Copyright 2012-2014, 2016 Xamarin Inc. All rights reserved.
using System;
using Mono.Tuner;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Xamarin.Linker;
using Xamarin.Bundler;

namespace MonoTouch.Tuner {
	
	public class OptimizeGeneratedCodeSubStep : CoreOptimizeGeneratedCode {
		// If the type currently being processed is a direct binding or not.
		// A null value means it's not a constant value, and can't be inlined.
		bool? isdirectbinding_constant;
		
		public OptimizeGeneratedCodeSubStep (LinkerOptions options)
		{
			Options = options;
#if DEBUG
			Console.WriteLine ("OptimizeGeneratedCodeSubStep Arch {0} Device: {1}, EnsureUiThread: {2}, FAT 32+64 {3}", Arch, Device, EnsureUIThread, IsDualBuild);
#endif
		}
		
		public int Arch {
			get { return Options.Arch; }
		}

		public bool Device {
			get { return Options.Device; }
		}
		
		public bool EnsureUIThread {
			get { return Options.EnsureUIThread; }
		}

		public bool IsDualBuild {
			get { return Options.IsDualBuild; }
		}

		LinkerOptions Options { get; set; }

		bool ApplyIntPtrSizeOptimization { get; set; }

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
			ApplyIntPtrSizeOptimization = ((Profile.Current as BaseProfile).ProductAssembly == assembly.Name.Name) || !IsDualBuild;
			base.Process (assembly);
		}

		protected override void Process (TypeDefinition type)
		{
			if (!HasGeneratedCode)
				return;

			isdirectbinding_constant = type.IsNSObject (LinkContext) ? type.GetIsDirectBindingConstant (LinkContext) : null;
			base.Process (type);
		}

		protected override void Process (MethodDefinition method)
		{
			if (!method.HasBody)
				return;

			if (method.IsGeneratedCode (LinkContext) && (IsExtensionType || IsExport (method))) {
				// We optimize methods that have the [GeneratedCodeAttribute] and is either an extension type or an exported method
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

		void ProcessCalls (MethodDefinition caller, Instruction ins)
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
				
		// https://app.asana.com/0/77259014252/77812690163
		void ProcessLoadStaticField (MethodDefinition caller, Instruction ins)
		{
			FieldReference fr = ins.Operand as FieldReference;
			switch (fr?.Name) {
			case "Arch":
				ProcessRuntimeArch (caller, ins);
				break;
			}
		}

		void ProcessRuntimeArch (MethodDefinition caller, Instruction ins)
		{
			const string operation = "inline Runtime.Arch";

			// Verify we're checking the right Arch field
			var fr = ins.Operand as FieldReference;
			if (!fr.DeclaringType.Is (Namespaces.ObjCRuntime, "Runtime"))
				return;
			
			// Verify a few assumptions before doing anything
			if (!ValidateInstruction (caller, ins, operation, Code.Ldsfld))
				return;

			// We're fine, inline the Runtime.Arch condition
			// The enum values are Runtime.DEVICE = 0 and Runtime.SIMULATOR = 1,
			ins.OpCode = Device ? OpCodes.Ldc_I4_0 : OpCodes.Ldc_I4_1;
			ins.Operand = null;
		}

		void ProcessEnsureUIThread (MethodDefinition caller, Instruction ins)
		{
			const string operation = "remove calls to UIApplication::EnsureUIThread";

			if (EnsureUIThread)
				return;

			// Verify we're checking the right get_EnsureUIThread call
			var mr = ins.Operand as MethodReference;
			if (!mr.DeclaringType.Is (Namespaces.UIKit, "UIApplication"))
				return;

			// Verify a few assumptions before doing anything
			if (!ValidateInstruction (caller, ins, operation, Code.Call))
				return;

			// This is simple: just remove the call
			Nop (ins); // call void UIKit.UIApplication::EnsureUIThread()
		}

		void ProcessIntPtrSize (MethodDefinition caller, Instruction ins)
		{
			const string operation = "inline IntPtr.Size";

			if (!ApplyIntPtrSizeOptimization)
				return;

			// This will optimize the following code sequence
			// if (IntPtr.Size == 8) { ... } else { ... }

			// Verify we're checking the right get_Size call
			var mr = ins.Operand as MethodReference;
			if (!mr.DeclaringType.Is ("System", "IntPtr"))
				return;

			// Verify a few assumptions before doing anything
			if (!ValidateInstruction (caller, ins.Next, operation, Code.Ldc_I4_8))
				return;

			var branchInstruction = ins.Next.Next;
			if (!ValidateInstruction (caller, branchInstruction, operation, Code.Bne_Un, Code.Bne_Un_S))
				return;

			// We're fine, inline the get_Size condition
			ins.OpCode = Arch == 8 ? OpCodes.Ldc_I4_8 : OpCodes.Ldc_I4_4;
			ins.Operand = null;
		}

		void ProcessIsDirectBinding (MethodDefinition caller, Instruction ins)
		{
			const string operation = "inline IsDirectBinding";

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