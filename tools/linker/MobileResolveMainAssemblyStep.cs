// Copyright 2015 Xamarin Inc. All rights reserved.

using System;

using Mono.Cecil;
using Mono.Linker.Steps;

namespace Xamarin.Linker.Steps {
	
	public class MobileResolveMainAssemblyStep : ResolveFromAssemblyStep {

		AssemblyDefinition assembly;

		public MobileResolveMainAssemblyStep (AssemblyDefinition ad) : base (ad)
		{
			assembly = ad;
		}

		protected override void Process ()
		{
			// when the main assembly is not a .dll (most common case) then we process as usual
			// as the .exe will tell the linker what's the root from which marking should be done
			if (assembly.MainModule.Kind != ModuleKind.Dll) {
				base.Process ();
				return;
			}
			// we have a watch extension which main project is a .dll and we have to tell the
			// linker where to start (by marking the first, entry method)
			Context.Resolver.CacheAssembly (assembly);

			Annotations.Push (assembly);

			foreach (var t in assembly.MainModule.Types) {
				if (!t.HasCustomAttribute (Namespaces.Foundation, "RegisterAttribute"))
					continue;
				Annotations.Mark (t);
				// the existing logic (both the general one and applying to NSObject) 
				// should bring everything else that's required
			}

			Annotations.Pop ();
		}
	}
}

