// Copyright 2012-2013,2016 Xamarin Inc. All rights reserved.

using System;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Tuner;
using Xamarin.Linker;

namespace MonoMac.Tuner {
	
	public class OptimizeGeneratedCodeSubStep : CoreOptimizeGeneratedCode {
		
		public OptimizeGeneratedCodeSubStep (bool ensureUiThread)
		{
			EnsureUIThread = ensureUiThread;
		}
		
		public bool EnsureUIThread { get; set; }
		
		public override bool IsActiveFor (AssemblyDefinition assembly)
		{
			// do not remove the code if we want the checks
			if (EnsureUIThread)
				return false;

			return base.IsActiveFor (assembly);
		}
		
		protected override void Process (MethodDefinition method)
		{
			// special processing on generated methods from NSObject-inherited types
			// it would be too risky to apply on user-generated code
			if (!method.HasBody || !method.IsGeneratedCode (LinkContext) || (!IsExtensionType && !IsExport (method)))
				return;
			
			var instructions = method.Body.Instructions;
			for (int i = 0; i < instructions.Count; i++) {
				switch (instructions [i].OpCode.Code) {
				case Code.Call:
					ProcessCalls (method, i);
					break;
				}
			}
		}
		
		void ProcessCalls (MethodDefinition caller, int i)
		{
			var instructions = caller.Body.Instructions;
			Instruction ins = instructions [i];
			MethodReference md = ins.Operand as MethodReference;
			// if it could not be resolved to a definition then it won't be NSObject
			if (md == null)
				return;
			
			switch (md.Name) {
			case "EnsureUIThread":
				if (EnsureUIThread || !md.DeclaringType.Is (Namespaces.AppKit, "NSApplication"))
					return;
#if DEBUG
				Console.WriteLine ("\t{0} EnsureUIThread {1}", caller, EnsureUIThread);
#endif						
				Nop (ins);								// call void MonoMac.AppKit.NSApplication::EnsureUIThread()
				break;
			}
		}
	}
}