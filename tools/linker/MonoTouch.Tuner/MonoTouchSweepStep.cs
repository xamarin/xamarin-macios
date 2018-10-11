// Copyright 2012-2013 Xamarin Inc. All rights reserved.

using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Tuner;
using Xamarin.Linker;

namespace MonoTouch.Tuner {
	
	public class MonoTouchSweepStep : CoreSweepStep {

		public MonoTouchSweepStep (LinkerOptions options)
			: base (options.LinkSymbols)
		{
		}

		protected override void SweepAssemblyDefinition (AssemblyDefinition assembly)
		{
			base.SweepAssemblyDefinition (assembly);

			if (assembly.HasCustomAttributes)
				SweepAttributes (assembly.CustomAttributes);
		}

		void SweepAttributes (IList<CustomAttribute> attributes)
		{
			for (int i = 0; i < attributes.Count; i++) {
				var ca = attributes [i];

				// we do not have to keep IVT to assemblies that are not part of the application
				if (!ca.AttributeType.Is ("System.Runtime.CompilerServices", "InternalsVisibleToAttribute"))
					continue;

				// validating the public key and the public key token would be time consuming
				// worse case (no match) is that we keep the attribute while it's not needed
				var fqn = (ca.ConstructorArguments [0].Value as string);
				int comma = fqn.IndexOf (',');
				if (comma != -1)
					fqn = fqn.Substring (0, comma);

				bool need_ivt = false;
				foreach (var assembly in Context.GetAssemblies ()) {
					if (assembly.Name.Name == fqn) {
						need_ivt = true;
						break;
					}
				}
				if (!need_ivt)
					attributes.RemoveAt (i--);
			}
		}
	}
}