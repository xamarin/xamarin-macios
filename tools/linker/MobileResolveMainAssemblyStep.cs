// Copyright 2015 Xamarin Inc. All rights reserved.

using System;

using Mono.Cecil;
using Mono.Linker.Steps;
using Xamarin.Tuner;

namespace Xamarin.Linker.Steps {

	public class MobileResolveMainAssemblyStep : ResolveFromAssemblyStep {

		AssemblyDefinition assembly;
		bool embeddinator;

		public MobileResolveMainAssemblyStep (AssemblyDefinition ad, bool embeddinator) : base (ad)
		{
			assembly = ad;
			this.embeddinator = embeddinator;
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
			Context.SafeReadSymbols (assembly);

			Context.Tracer.Push (assembly);

			var is_product_assembly = Mono.Tuner.Profile.IsProductAssembly (assembly);
			foreach (var t in assembly.MainModule.Types) {
				if (embeddinator) {
					if (t.IsPublic && !is_product_assembly) {
						// Mark all public types when in embeddinator mode.
						// There may be no types with the Register attribute,
						// which means that without this the assembly might be completely ignored even if it's not linked.
						Annotations.Mark (t);
					}
					continue;
				}

				if (!t.HasCustomAttribute (Namespaces.Foundation, "RegisterAttribute"))
					continue;
				Annotations.Mark (t);
				// the existing logic (both the general one and applying to NSObject) 
				// should bring everything else that's required
			}

			Context.Tracer.Pop ();
		}
	}
}
