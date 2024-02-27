// Copyright 2011-2013 Xamarin Inc. All rights reserved.

using System;
using System.Collections.Generic;

using Mono.Cecil;
using Mono.Linker;
using Mono.Tuner;

using Xamarin.Tuner;

namespace Xamarin.Linker.Steps {

	public class MobileApplyPreserveAttribute : ApplyPreserveAttribute {

		bool is_sdk;

		DerivedLinkContext LinkContext {
			get { return (DerivedLinkContext) base.context; }
		}

		// System.ServiceModel.dll is an SDK assembly but it does contain types with [DataMember] attributes
		// just like System.Xml.dll contais [Xml*] attributes - we do not want to keep them unless the application
		// shows the feature is being used
		public override bool IsActiveFor (AssemblyDefinition assembly)
		{
			is_sdk = Profile.IsSdkAssembly (assembly);
			return base.IsActiveFor (assembly);
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
					MarkDefaultConstructor (provider, is_sdk ? LinkContext.DataContract : null);
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
						MarkDefaultConstructor (provider, is_sdk ? LinkContext.XmlSerialization : null);
						return !is_sdk;
					}
				}
				break;
			default:
				return base.IsPreservedAttribute (provider, attribute, out removeAttribute);
			}
			// keep them (provider and attribute)
			return false;
		}

		// xml serialization requires the default .ctor to be present
		void MarkDefaultConstructor (ICustomAttributeProvider provider, IList<ICustomAttributeProvider> list)
		{
			if (list is not null) {
				list.Add (provider);
				return;
			}

			TypeDefinition td = (provider as TypeDefinition);
			if (td is null) {
				PropertyDefinition pd = (provider as PropertyDefinition);
				if (pd is not null) {
					MarkDefaultConstructor (pd.DeclaringType);
					MarkGenericType (pd.PropertyType as GenericInstanceType);
					td = pd.PropertyType.Resolve ();
				} else {
					FieldDefinition fd = (provider as FieldDefinition);
					if (fd is not null) {
						MarkDefaultConstructor (fd.DeclaringType);
						MarkGenericType (fd.FieldType as GenericInstanceType);
						td = (fd.FieldType as TypeReference).Resolve ();
					}
				}
			}

			// e.g. <T> property (see bug #5543) or field (see linkall unit tests)
			if (td is not null)
				MarkDefaultConstructor (td);
		}

		void MarkGenericType (GenericInstanceType git)
		{
			if (git is null || !git.HasGenericArguments)
				return;

			foreach (TypeReference tr in git.GenericArguments)
				MarkDefaultConstructor (tr.Resolve ());
		}

		void MarkDefaultConstructor (TypeDefinition type)
		{
			if ((type is null) || !type.HasMethods)
				return;

			foreach (MethodDefinition ctor in type.Methods) {
				if (!ctor.IsConstructor || ctor.IsStatic || ctor.HasParameters)
					continue;

				Annotations.AddPreservedMethod (type, ctor);
			}
		}
	}
}
