// Copyright 2013 Xamarin Inc. All rights reserved.

using System;
using Mono.Linker;
using Mono.Tuner;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Xamarin.Linker;

namespace MonoTouch.Tuner {
	
	public class RemoveExtraRegistrar : BaseSubStep {

		bool keep_old;

		public RemoveExtraRegistrar (bool oldRegistrar)
		{
			keep_old = oldRegistrar;
		}

		public override SubStepTargets Targets {
			get { return SubStepTargets.Assembly | SubStepTargets.Type; }
		}
		
		public override bool IsActiveFor (AssemblyDefinition assembly)
		{
			if (assembly.Name.Name != (Profile.Current as BaseProfile).ProductAssembly)
				return false;

			// process only assemblies where the linker is enabled (e.g. --linksdk, --linkskip) 
			AssemblyAction action = Annotations.GetAction (assembly);
			if (action != AssemblyAction.Link) {
#if DEBUG
				Console.WriteLine ("Assembly {0} : skipped ({1})", assembly, action);
#endif
				return false;
			}

			return true;
		}

		public override void ProcessType (TypeDefinition type)
		{
			if (!type.Is (Namespaces.ObjCRuntime, "Runtime"))
				return;

			foreach (MethodDefinition m in type.Methods) {
				if (!m.IsStatic || m.Name != "CreateRegistrar" || !m.HasParameters || m.Parameters.Count != 1)
					continue;
				OptimizeRegistrar (m);
			}
		}

		MethodReference GetRegistrar (ModuleDefinition module, string typeName)
		{
			foreach (TypeDefinition type in module.Types) {
				if (type.Namespace != Namespaces.Registrar)
					continue;
				if (type.Name != typeName)
					continue;
				foreach (MethodDefinition ctor in type.Methods) {
					if (!ctor.IsConstructor || ctor.IsStatic || ctor.HasParameters)
						continue;
					return ctor;
				}
			}
			return null;
		}

		void OptimizeRegistrar (MethodDefinition m)
		{
			string name = null;
			m.Body.Instructions.Clear ();
			// newobj instance void MonoTouch.Registrar.[Old][NewRef]DynamicRegistrar::.ctor()
			if (keep_old) {
				name = "OldDynamicRegistrar";
			} else {
				name = "DynamicRegistrar";
			}
			MethodReference ctor = GetRegistrar (m.Module, name);

			ILProcessor il = m.Body.GetILProcessor ();
			il.Emit (OpCodes.Newobj, ctor);

			// stsfld class MonoTouch.Registrar.IDynamicRegistrar MonoTouch.ObjCRuntime.Runtime::Registrar
			foreach (FieldReference field in m.DeclaringType.Fields) {
				if (field.Name != "Registrar")
					continue;
				il.Emit (OpCodes.Stsfld, field);
				il.Emit (OpCodes.Ret);
				return;
			}
		}
	}
}