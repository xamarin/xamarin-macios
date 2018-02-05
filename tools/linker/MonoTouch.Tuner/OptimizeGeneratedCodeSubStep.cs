// Copyright 2012-2014, 2016 Xamarin Inc. All rights reserved.
using System;
using Mono.Tuner;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Xamarin.Linker;
using Xamarin.Bundler;

namespace MonoTouch.Tuner {
	
	public class OptimizeGeneratedCodeSubStep : CoreOptimizeGeneratedCode {
		
		public OptimizeGeneratedCodeSubStep (LinkerOptions options)
			: base (options)
		{
		}

		public bool Device {
			get { return Options.Device; }
		}

		// https://app.asana.com/0/77259014252/77812690163
		protected override void ProcessLoadStaticField (MethodDefinition caller, Instruction ins)
		{
			FieldReference fr = ins.Operand as FieldReference;
			switch (fr?.Name) {
			case "Arch":
				ProcessRuntimeArch (caller, ins);
				break;
			}

			base.ProcessLoadStaticField (caller, ins);
		}

		void ProcessRuntimeArch (MethodDefinition caller, Instruction ins)
		{
			const string operation = "inline Runtime.Arch";

			if (Optimizations.InlineRuntimeArch != true)
				return;

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
	}
}