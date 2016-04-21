// Copyright 2011-2013 Xamarin Inc. All rights reserved.

using System;

using Mono.Cecil;
using Mono.Linker;
using Mono.Tuner;
using Xamarin.Linker;

namespace MonoMac.Tuner {

	public class ApplyPreserveAttribute : ApplyPreserveAttributeBase {

		bool system_service_model;

		// System.ServiceModel.dll is an SDK assembly but it does contain types with [DataMember] attributes
		public override bool IsActiveFor (AssemblyDefinition assembly)
		{
			system_service_model = false;
			if (Profile.IsSdkAssembly (assembly)) {
				system_service_model = (assembly.Name.Name != "System.ServiceModel");
			}
			return Annotations.GetAction (assembly) == AssemblyAction.Link;
		}

		protected override bool IsPreservedAttribute (ICustomAttributeProvider provider, CustomAttribute attribute, out bool removeAttribute)
		{
			removeAttribute = false;
			TypeReference type = attribute.Constructor.DeclaringType;

			switch (type.Namespace) {
			case "System.Runtime.Serialization":
				// do not process this on System.ServiceModel.dll
				if (system_service_model)
					return false;

				bool srs = false;
				// http://bugzilla.xamarin.com/show_bug.cgi?id=1415
				// http://msdn.microsoft.com/en-us/library/system.runtime.serialization.datamemberattribute.aspx
				if (provider is PropertyDefinition || provider is FieldDefinition || provider is EventDefinition)
					srs = (type.Name == "DataMemberAttribute");
				else if (provider is TypeDefinition)
					srs = (type.Name == "DataContractAttribute");

				if (srs) {
					MarkDefautConstructor (provider);
					return true;
				}
				break;
			case "System.Xml.Serialization":
				// http://msdn.microsoft.com/en-us/library/83y7df3e.aspx
				string name = type.Name;
				if ((name.StartsWith ("Xml", StringComparison.Ordinal) && name.EndsWith ("Attribute", StringComparison.Ordinal))) {
					// but we do not have to keep things that XML serialization will ignore anyway!
					if (name != "XmlIgnoreAttribute") {
						// the default constructor of the type *being used* is needed
						MarkDefautConstructor (provider);
						return true;
					}
				}
				break;
			default:
				if (type.Is (Namespaces.Foundation, "PreserveAttribute")) {
					// there's no need to keep the [Preserve] attribute in the assembly once it was processed
					removeAttribute = true;
					return true;
				}
				break;
			}
			// keep them (provider and attribute)
			return false;
		}

		// xml serialization requires the default .ctor to be present
		void MarkDefautConstructor (ICustomAttributeProvider provider)
		{
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
