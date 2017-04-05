// Copyright 2012-2013, 2016 Xamarin Inc. All rights reserved.

using System;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Linker;
using Mono.Tuner;

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
	}
}