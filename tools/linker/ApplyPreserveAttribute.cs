// Copyright 2011-2013 Xamarin Inc. All rights reserved.

using System;
using System.Collections.Generic;

using Mono.Cecil;
using Mono.Linker;
using Mono.Tuner;

using Xamarin.Tuner;

namespace Xamarin.Linker.Steps {

	public class ApplyPreserveAttribute : ApplyPreserveAttributeBase {

		HashSet<TypeDefinition> preserve_synonyms;

		public override bool IsActiveFor (AssemblyDefinition assembly)
		{
			return Annotations.GetAction (assembly) == AssemblyAction.Link;
		}

		public override void Initialize (LinkContext context)
		{
			base.Initialize (context);

			// we cannot override ProcessAssembly as some decisions needs to be done before applying the [Preserve]
			// synonyms

			foreach (var assembly in context.GetAssemblies ()) {
				if (!assembly.HasCustomAttributes)
					continue;

				foreach (var attribute in assembly.CustomAttributes) {
					if (!attribute.Constructor.DeclaringType.Is (Namespaces.Foundation, "PreserveAttribute"))
						continue;

					if (!attribute.HasConstructorArguments)
						continue;
					var tr = (attribute.ConstructorArguments [0].Value as TypeReference);
					if (tr == null)
						continue;

					// we do not call `this.ProcessType` since
					// (a) we're potentially processing a different assembly and `is_active` represent the current one
					// (b) it will try to fetch the [Preserve] attribute on the type (and it's not there) as `base` would
					var type = tr.Resolve ();
					Annotations.Mark (type);
					if (attribute.HasFields) {
						foreach (var named_argument in attribute.Fields) {
							if (named_argument.Name == "AllMembers" && (bool)named_argument.Argument.Value)
								Annotations.SetPreserve (type, TypePreserve.All);
						}
					}

					// if the type is a custom attribute then it means we want to preserve what's decorated
					// with this attribute (not just the attribute alone)
					if (type.Inherits ("System", "Attribute")) {
						if (preserve_synonyms == null)
							preserve_synonyms = new HashSet<TypeDefinition> ();
						preserve_synonyms.Add (type);
					}
				}
			}
		}

		protected override bool IsPreservedAttribute (ICustomAttributeProvider provider, CustomAttribute attribute, out bool removeAttribute)
		{
			removeAttribute = false;
			TypeReference type = attribute.Constructor.DeclaringType;

			if (type.Name == "PreserveAttribute") {
				// there's no need to keep the [Preserve] attribute in the assembly once it was processed
				removeAttribute = true;
				return true;
			}
			// we need to resolve (as many reference instances can exists)
			return ((preserve_synonyms != null) && preserve_synonyms.Contains (type.Resolve ()));
		}
	}
}
