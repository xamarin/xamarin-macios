// Copyright 2012-2013 Xamarin Inc. All rights reserved.

using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Linker;
using Mono.Tuner;

namespace Xamarin.Linker {
	
	public class CoreOptimizeGeneratedCode : BaseSubStep {
		
		protected bool HasGeneratedCode { get; private set; }

		public override SubStepTargets Targets {
			get { return SubStepTargets.Assembly | SubStepTargets.Type; }
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

		public override void ProcessType (TypeDefinition type)
		{
			// if 'type' inherits from NSObject inside an assembly that has [GeneratedCode]
			// or for static types used for optional members (using extensions methods), they can be optimized too
			bool extensions = type.IsSealed && type.IsAbstract && type.Name.EndsWith ("_Extensions", StringComparison.Ordinal);
			if (!HasGeneratedCode && (type.IsNSObject () || !extensions))
				return;
			
			if (type.HasMethods)
				ProcessMethods (type.Methods, extensions);
		}

		// [GeneratedCode] is not enough - e.g. it's used for anonymous delegates even if the 
		// code itself is not tool/compiler generated
		static bool IsExport (ICustomAttributeProvider provider)
		{
			return provider.HasCustomAttribute (Namespaces.Foundation, "ExportAttribute");
		}

		void ProcessMethods (IEnumerable<MethodDefinition> c, bool extensions)
		{
			foreach (MethodDefinition m in c) {
				// special processing on generated methods from NSObject-inherited types
				// it would be too risky to apply on user-generated code
				if (m.HasBody && m.IsGeneratedCode () && (extensions || IsExport (m)))
					ProcessMethod (m);
			}
		}

		// less risky to nop-ify if branches are pointing to this instruction
		static protected void Nop (Instruction ins)
		{
			ins.OpCode = OpCodes.Nop;
			ins.Operand = null;
		}
	}
}