// Copyright 2016 Xamarin Inc. All rights reserved.

using System;
using System.Collections.Generic;

using Mono.Cecil;
using Mono.Linker;
using Mono.Tuner;

namespace Xamarin.Linker.Steps {

	// at this stage we know everything that will be part of the .app and, due to JIT restrictions,
	// we know the .app won't be extendable at runtime (downloading or generating extra code
	// this opens up some possibilities to seal types, methods and devirtualize methods
	public class SealerSubStep : ExceptionalSubStep {

#if DEBUG
		int seal;
		int final;
		int devirtualize;
#endif
		protected override string Name { get; } = "Sealer";
		protected override int ErrorCode { get; } = 2060;

		public override SubStepTargets Targets {
			get {
				return SubStepTargets.Type;
			}
		}

		public override bool IsActiveFor (AssemblyDefinition assembly)
		{
			return Annotations.GetAction (assembly) == AssemblyAction.Link;
		}

		bool IsSubclassed (TypeDefinition type)
		{
			foreach (var a in context.GetAssemblies ()) {
				foreach (var s in a.MainModule.Types) {
					if (s.BaseType.Is (type.Namespace, type.Name) && Annotations.IsMarked (s))
						return true;
					if (s.HasNestedTypes) {
						foreach (var ns in s.NestedTypes) {
							if (ns.BaseType.Is (type.Namespace, type.Name) && Annotations.IsMarked (ns))
								return true;
						}
					}
				}
			}
			return false;
		}

		protected override void Process (TypeDefinition type)
		{
			// interface members are virtual (and we cannot change this)
			// we cannot seal interfaces either
			if (type.IsInterface)
				return;

			// only optimize code that was marked earlier (the rest will be swept away)
			if (!Annotations.IsMarked (type))
				return;

			// if we do not include any subclass for this type
			if (!type.IsAbstract && !type.IsSealed && !IsSubclassed (type)) {
				type.IsSealed = true;
#if DEBUG
				Console.WriteLine ("Seal {0} ({1})", type, ++seal);
#endif
			}

			if (!type.HasMethods)
				return;

			// process virtual methods to see if we can "seal" or devirtualize them
			foreach (var method in type.Methods) {
				if (method.IsFinal || !method.IsVirtual || method.IsAbstract || method.IsRuntime)
					continue;
				if (!Annotations.IsMarked (method))
					continue;

				var overrides = Annotations.GetOverrides (method);
				// we cannot de-virtualize nor seal methods if something overrides them
				if (overrides is not null) {
					// sanity (disable IsSealed == true above)
					//if (type.IsSealed)
					//	Console.WriteLine ();
					if (AreMarked (overrides))
						continue;
				}

				// we can seal the method (final in IL / !virtual in C#)
				method.IsFinal = true;
#if DEBUG
				Console.WriteLine ("Final {0} ({1})", method, ++final);
#endif
				// subclasses might need this method to satisfy an interface requirement
				// and requires dispatch/virtual support
				if (!type.IsSealed)
					continue;

				var bases = Annotations.GetBaseMethods (method);
				// look if this method is an override to existing _marked_ methods
				if (!AreMarked (bases)) {
					method.IsVirtual = false;
					method.IsFinal = false; // since it's not virtual anymore
#if DEBUG
					Console.WriteLine ("Devirtualize {0} ({1})", method, ++devirtualize);
#endif
				}
			}
		}

		bool AreMarked (List<OverrideInformation> list)
		{
			if (list is null)
				return false;
			foreach (var m in list) {
				if (Annotations.IsMarked (m.Override))
					return true;
			}
			return false;
		}

		bool AreMarked (List<MethodDefinition> list)
		{
			if (list is null)
				return false;
			foreach (var m in list) {
				if (Annotations.IsMarked (m))
					return true;
			}
			return false;
		}
	}
}
