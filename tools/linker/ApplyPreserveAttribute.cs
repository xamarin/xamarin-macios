// Copyright 2011-2013 Xamarin Inc. All rights reserved.

using System;
using System.Collections.Generic;

using Mono.Cecil;
using Mono.Linker;
using Mono.Tuner;

using Xamarin.Tuner;

namespace Xamarin.Linker.Steps {

	public class ApplyPreserveAttribute : ApplyPreserveAttributeBase {

		bool is_sdk;
		HashSet<TypeDefinition> preserve_synonyms;

		DerivedLinkContext LinkContext {
			get { return (DerivedLinkContext) base.context; }
		}

		// System.ServiceModel.dll is an SDK assembly but it does contain types with [DataMember] attributes
		// just like System.Xml.dll contais [Xml*] attributes - we do not want to keep them unless the application
		// shows the feature is being used
		public override bool IsActiveFor (AssemblyDefinition assembly)
		{
			is_sdk = Profile.IsSdkAssembly (assembly);
			return Annotations.GetAction (assembly) == AssemblyAction.Link;
		}

		public override void Initialize (LinkContext context)
		{
			base.Initialize (context);

			// we cannot override ProcessAssembly as some decisions needs to be done before applyting the [Preserve]
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

			switch (type.Namespace) {
			case "System.Runtime.Serialization":
				bool srs = false;
				// http://bugzilla.xamarin.com/show_bug.cgi?id=1415
				// http://msdn.microsoft.com/en-us/library/system.runtime.serialization.datamemberattribute.aspx
				if (provider is PropertyDefinition || provider is FieldDefinition || provider is EventDefinition)
					srs = (type.Name == "DataMemberAttribute");
				else if (provider is TypeDefinition)
					srs = (type.Name == "DataContractAttribute");

				if (srs) {
					MarkDefautConstructor (provider, is_sdk ? LinkContext.DataContract : null);
					return !is_sdk;
				}
				break;
			case "System.Xml.Serialization":
				// http://msdn.microsoft.com/en-us/library/83y7df3e.aspx
				string name = type.Name;
				if ((name.StartsWith ("Xml", StringComparison.Ordinal) && name.EndsWith ("Attribute", StringComparison.Ordinal))) {
					// but we do not have to keep things that XML serialization will ignore anyway!
					if (name != "XmlIgnoreAttribute") {
						// the default constructor of the type *being used* is needed
						MarkDefautConstructor (provider, is_sdk ? LinkContext.XmlSerialization : null);
						return !is_sdk;
					}
				}
				break;
			default:
				if (type.Name == "PreserveAttribute") {
					// there's no need to keep the [Preserve] attribute in the assembly once it was processed
					removeAttribute = true;
					return true;
				}
				// we need to resolve (as many reference instances can exists)
				return ((preserve_synonyms != null) && preserve_synonyms.Contains (type.Resolve ()));
			}
			// keep them (provider and attribute)
			return false;
		}

		// xml serialization requires the default .ctor to be present
		void MarkDefautConstructor (ICustomAttributeProvider provider, IList<ICustomAttributeProvider> list)
		{
			if (list != null) {
				list.Add (provider);
				return;
			}

			TypeDefinition td = (provider as TypeDefinition);
			if (td == null) {
				PropertyDefinition pd = (provider as PropertyDefinition);
				if (pd != null) {
					MarkDefautConstructor (pd.DeclaringType);					
					MarkGenericType (pd.PropertyType as GenericInstanceType);
					td = pd.PropertyType.Resolve ();
				} else {
					FieldDefinition fd = (provider as FieldDefinition);
					if (fd != null) {
						MarkDefautConstructor (fd.DeclaringType);
						MarkGenericType (fd.FieldType as GenericInstanceType);
						td = (fd.FieldType as TypeReference).Resolve ();
					}
				}
			}

			// e.g. <T> property (see bug #5543) or field (see linkall unit tests)
			if (td != null)
				MarkDefautConstructor (td);
		}

		void MarkGenericType (GenericInstanceType git)
		{
			if (git == null || !git.HasGenericArguments)
				return;

			foreach (TypeReference tr in git.GenericArguments)
				MarkDefautConstructor (tr.Resolve ());
		}

		void MarkDefautConstructor (TypeDefinition type)
		{
			if ((type == null) || !type.HasMethods)
				return;

			foreach (MethodDefinition ctor in type.Methods) {
				if (!ctor.IsConstructor || ctor.IsStatic || ctor.HasParameters)
					continue;

				Annotations.AddPreservedMethod (type, ctor);
			}
		}
	}
}
