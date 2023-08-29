// Copyright 2011-2013 Xamarin Inc. All rights reserved.

using System;
using System.Collections.Generic;

using Mono.Cecil;
using Mono.Linker;
using Mono.Tuner;

using Xamarin.Tuner;

namespace Xamarin.Linker.Steps {

	public class ApplyPreserveAttribute : ApplyPreserveAttributeBase {

#if !NET
		HashSet<TypeDefinition> preserve_synonyms;
#endif

		// We need to run the ApplyPreserveAttribute step even if we're only linking sdk assemblies, because even
		// though we know that sdk assemblies will never have Preserve attributes, user assemblies may have
		// [assembly: LinkSafe] attributes, which means we treat them as sdk assemblies and those may have
		// Preserve attributes.
		public override bool IsActiveFor (AssemblyDefinition assembly)
		{
			return Annotations.GetAction (assembly) == AssemblyAction.Link;
		}

#if NET
		protected override void Process (AssemblyDefinition assembly)
		{
			base.Process (assembly);
			ProcessAssemblyAttributes (assembly);
		}
#else
		public override void Initialize (LinkContext context)
		{
			base.Initialize (context);

			// we cannot override ProcessAssembly as some decisions needs to be done before applying the [Preserve]
			// synonyms
			foreach (var assembly in context.GetAssemblies ())
				ProcessAssemblyAttributes (assembly);
		}
#endif

		void ProcessAssemblyAttributes (AssemblyDefinition assembly)
		{
			if (!assembly.HasCustomAttributes)
				return;

			foreach (var attribute in assembly.CustomAttributes) {
				if (!attribute.Constructor.DeclaringType.Is (Namespaces.Foundation, "PreserveAttribute"))
					continue;

				if (!attribute.HasConstructorArguments)
					continue;
				var tr = (attribute.ConstructorArguments [0].Value as TypeReference);
				if (tr is null)
					continue;

				// we do not call `this.ProcessType` since
				// (a) we're potentially processing a different assembly and `is_active` represent the current one
				// (b) it will try to fetch the [Preserve] attribute on the type (and it's not there) as `base` would
				var type = tr.Resolve ();

#if NET
				PreserveType (type, attribute);
#else
				Annotations.Mark (type);
				if (attribute.HasFields) {
					foreach (var named_argument in attribute.Fields) {
						if (named_argument.Name == "AllMembers" && (bool) named_argument.Argument.Value)
							Annotations.SetPreserve (type, TypePreserve.All);
					}
				}
#endif

				// In .NET6, ApplyPreserveAttribute no longer runs on all assemblies.
				// [assembly: Preserve (typeof (SomeAttribute))] no longer gives SomeAttribute "Preserve" semantics.
#if !NET
				// if the type is a custom attribute then it means we want to preserve what's decorated
				// with this attribute (not just the attribute alone)
				if (type.Inherits ("System", "Attribute")) {
					if (preserve_synonyms is null)
						preserve_synonyms = new HashSet<TypeDefinition> ();
					preserve_synonyms.Add (type);
				}
#endif
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
#if NET
			return false;
#else
			// we need to resolve (as many reference instances can exists)
			return ((preserve_synonyms is not null) && preserve_synonyms.Contains (type.Resolve ()));
#endif
		}
	}
}
