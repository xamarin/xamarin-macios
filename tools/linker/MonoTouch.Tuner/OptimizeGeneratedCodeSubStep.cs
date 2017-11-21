// Copyright 2012-2014, 2016 Xamarin Inc. All rights reserved.
//#define TRACE
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

		public bool InlineSetupBlock {
			get {
				// Enabled by default always.
				return LinkContext.App.Optimizations.InlineSetupBlock != false;
			}
		}

		MethodDefinition setupblock_def;
		MethodReference GetBlockSetupImpl (MethodDefinition caller)
		{
			if (setupblock_def == null) {
				var type = LinkContext.GetAssembly (Driver.GetProductAssembly (LinkContext.Target.App)).MainModule.GetType (Namespaces.ObjCRuntime, "BlockLiteral");
				foreach (var method in type.Methods) {
					if (method.Name != "SetupBlockImpl")
						continue;
					setupblock_def = method;
					setupblock_def.IsPublic = true;
					break;
				}
				if (setupblock_def == null)
					throw new NotImplementedException ();
			}
			return caller.Module.ImportReference (setupblock_def);
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
			if (LinkContext.App.Optimizations.InlineIntPtrSize.HasValue) {
				ApplyIntPtrSizeOptimization = LinkContext.App.Optimizations.InlineIntPtrSize.Value;
			} else {
				ApplyIntPtrSizeOptimization = ((Profile.Current as BaseProfile).ProductAssembly == assembly.Name.Name) || !IsDualBuild;
			}

			base.Process (assembly);
		}

		protected override void Process (TypeDefinition type)
		{
			if (!HasOptimizableCode)
				return;

			isdirectbinding_constant = type.IsNSObject (LinkContext) ? type.GetIsDirectBindingConstant (LinkContext) : null;
			base.Process (type);
		}

		protected override void Process (MethodDefinition method)
		{
			if (!method.HasBody)
				return;

			if (method.IsOptimizableCode (LinkContext)) {
				// We optimize methods that have the [LinkerOptimize] attribute,
			} else if (method.IsGeneratedCode (LinkContext) && (IsExtensionType || IsExport (method))) {
				// We optimize methods that have the [GeneratedCodeAttribute] and is either an extension type or an exported method
			} else {
				// but it would be too risky to apply on user-generated code
				return;
			}
			
			if (!LinkContext.DynamicRegistrationSupported && method.Name == "get_DynamicRegistrationSupported" && method.DeclaringType.Is ("ObjCRuntime", "Runtime")) {
				// Rewrite to return 'false'
				var instr = method.Body.Instructions;
				instr.Clear ();
				instr.Add (Instruction.Create (OpCodes.Ldc_I4_0));
				instr.Add (Instruction.Create (OpCodes.Ret));
				return; // nothing else to do here.
			}

			var instructions = method.Body.Instructions;
			for (int i = 0; i < instructions.Count; i++) {
				var ins = instructions [i];
				switch (ins.OpCode.Code) {
				case Code.Call:
					i += ProcessCalls (method, ins);
					break;
				case Code.Ldsfld:
					ProcessLoadStaticField (method, ins);
					break;
				}
			}

			EliminateDeadCode (method);
		}

		TypeReference InflateType (GenericInstanceType git, TypeReference type)
		{
			var gt = type as GenericParameter;
			if (gt != null)
				return git.GenericArguments [gt.Position];
			if (type is TypeSpecification)
				throw new NotImplementedException ();
			return type;
		}

		MethodReference InflateMethod (TypeReference inflatedDeclaringType, MethodDefinition method)
		{
			var git = inflatedDeclaringType as GenericInstanceType;
			if (git == null)
				return method;
			var mr = new MethodReference (method.Name, InflateType (git, method.ReturnType), git);
			if (method.HasParameters) {
				for (int i = 0; i < method.Parameters.Count; i++) {
					var p = new ParameterDefinition (method.Parameters [i].Name, method.Parameters [i].Attributes, InflateType (git, method.Parameters [i].ParameterType));
					mr.Parameters.Add (p);
				}
			}
			return mr;
		}

		bool IsSubclassed (TypeDefinition type, TypeDefinition byType)
		{
			if (byType.Is (type.Namespace, type.Name))
				return true;
			if (byType.HasNestedTypes) {
				foreach (var ns in byType.NestedTypes) {
					if (IsSubclassed (type, ns))
						return true;
				}
			}
			return false;
		}

		bool IsSubclassed (TypeDefinition type)
		{
			foreach (var a in context.GetAssemblies ()) {
				foreach (var s in a.MainModule.Types) {
					if (IsSubclassed (type, s))
						return true;
				}
			}
			return false;
		}

		// returns the number of instructions added (if positive) or removed (if negative)
		int ProcessCalls (MethodDefinition caller, Instruction ins)
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
			case "SetupBlock":
			case "SetupBlockUnsafe":
				// This will optimize calls to SetupBlock and SetupBlockUnsafe by calculating the signature for the block
				// (which both SetupBlock and SetupBlockUnsafe do), and then rewrite the code to call SetupBlockImpl instead
				// (which takes the block signature as an argument instead of calculating it). This is required when
				// removing the dynamic registrar, because calculating the block signature is done in the dynamic registrar.
				if (!InlineSetupBlock)
					return 0;
				if (!mr.DeclaringType.Is (Namespaces.ObjCRuntime, "BlockLiteral"))
					return 0;

				var prev = ins.Previous;
				if (prev.OpCode.StackBehaviourPop != StackBehaviour.Pop0) {
					Driver.Log (1, "Failed to optimize {0}': for instruction '{1}' expected previous instruction '{2}' to be Pop0", caller, prev.Next, prev);
					return 0;
				} else if (prev.OpCode.StackBehaviourPush != StackBehaviour.Push1) {
					Driver.Log (1, "Failed to optimize {0}': for instruction '{1}' expected previous instruction '{2}' to be Push1", caller, prev.Next, prev);
					return 0;
				}
				prev = prev.Previous;
				TypeReference delegateType = null;

				if (prev.OpCode.StackBehaviourPop != StackBehaviour.Pop0) {
					Driver.Log (1, "Failed to optimize {0}': for instruction '{1}' expected previous instruction '{2}' to be Pop0", caller, prev.Next, prev);
					return 0;
				} else if (prev.OpCode.StackBehaviourPush != StackBehaviour.Push1) {
					Driver.Log (1, "Failed to optimize {0}': for instruction '{1}' expected previous instruction '{2}' to be Push1", caller, prev.Next, prev);
					return 0;
				} else if (prev.OpCode.Code == Code.Ldsfld) {
					delegateType = ((FieldReference) prev.Operand).Resolve ().FieldType;
				} else {
					Driver.Log (1, "Failed to optimize {0}': for instruction '{1}' expected previous instruction '{2}' to be Ldsfld", caller, prev.Next, prev);
					return 0;
				}

				// Calculate the block signature
				TypeReference userDelegateType = null;
				var delegateTypeDefinition = delegateType.Resolve ();
				foreach (var attrib in delegateTypeDefinition.CustomAttributes) {
					var attribType = attrib.Constructor.DeclaringType;
					if (!attribType.Is (Namespaces.ObjCRuntime, "UserDelegateTypeAttribute"))
						continue;
					userDelegateType = attrib.ConstructorArguments [0].Value as TypeReference;
					break;
				}
				bool blockSignature = false;
				string signature = null;
				MethodReference userMethod = null;
				if (userDelegateType != null) {
					var userDelegateTypeDefinition = userDelegateType.Resolve ();
					MethodDefinition userMethodDefinition = null;
					foreach (var method in userDelegateTypeDefinition.Methods) {
						if (method.Name != "Invoke")
							continue;
						userMethodDefinition = method;
						break;
					}
					if (userMethodDefinition == null)
						throw new NotImplementedException ();
					blockSignature = true;
					userMethod = InflateMethod (userDelegateType, userMethodDefinition);
				} else if (delegateType.Is ("System", "Action`1") && (delegateType is GenericInstanceType git) && git.GenericArguments [0].Is ("System", "IntPtr")) {
					signature = "v@?";
				} else {
					Driver.Log (0, "Failed to optimize {0}: for instruction '{1}' could not find the UserDelegateTypeAttribute on {2}", caller, ins, delegateType.FullName);
					return 0;
				}
				if (signature == null) {
					try {
						var parameters = new TypeReference [userMethod.Parameters.Count];
						for (int p = 0; p < parameters.Length; p++)
							parameters [p] = userMethod.Parameters [p].ParameterType;
						signature = LinkContext.Target.StaticRegistrar.ComputeSignature (userMethod.DeclaringType, false, userMethod.ReturnType, parameters, isBlockSignature: blockSignature);
					} catch (Exception e) {
						Driver.Log (1, "Failed to optimize {0}: for instruction '{1}': {2}", caller, ins, e.Message);
						signature = "BROKEN SIGNATURE"; // FIXME: fix the broken binding
					}
				}

				var instructions = caller.Body.Instructions;
				var index = instructions.IndexOf (ins);
				instructions.Insert (index, Instruction.Create (OpCodes.Ldstr, signature));
				instructions.Insert (index, Instruction.Create (mr.Name == "SetupBlockUnsafe" ? OpCodes.Ldc_I4_0 : OpCodes.Ldc_I4_1));
				ins.Operand = GetBlockSetupImpl (caller);

				Driver.Log (1, "Optimized {0} ('{1}') with delegate type {2} and signature {3}", caller, ins, delegateType.FullName, signature);

				return 2;
			case "get_DynamicRegistrationSupported":
				if (LinkContext.DynamicRegistrationSupported)
					return 0;
				
				if (!mr.DeclaringType.Is (Namespaces.ObjCRuntime, "Runtime"))
					return 0;
				
				ProcessIsDynamicSupported (caller, ins, false);
				break;
			}

			return 0;
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

		void ProcessIsDynamicSupported (MethodDefinition caller, Instruction ins, bool value)
		{
			const string operation = "inline Runtime.IsDynamicSupported";

			/* FIXME: check for user-settable optimization condition */

			// Verify we're checking the right Runtime.IsDynamicSupported call
			var mr = ins.Operand as MethodReference;
			if (!mr.DeclaringType.Is (Namespaces.ObjCRuntime, "Runtime"))
				return;
			
			if (!ValidateInstruction (caller, ins, operation, Code.Call))
				return;

			// We're fine, inline the Runtime.Arch condition
			// The enum values are IsDynamicSupported condition
			ins.OpCode = LinkContext.DynamicRegistrationSupported ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0;
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