using System.IO;

using Mono.Cecil;
using Mono.Linker;
using Mono.Linker.Steps;
using Xamarin.Bundler;
using Xamarin.Tuner;
using Xamarin.Utils;

#nullable enable

namespace Xamarin.Linker.Steps {

	public class ApplyPreserveAttributeStep : AssemblyModifierStep, IApplyPreserveAttribute {
		ApplyPreserveAttributeImpl impl;
		readonly XmlDescriptor xmlDescriptor = new ();
		protected override string Name { get => "Apply Preserve Attribute"; }
		protected override int ErrorCode { get => 2450; }

		bool? create_xml_description_file;
		public bool CreateXmlDescriptionFile {
			get {
				if (create_xml_description_file.HasValue)
					return create_xml_description_file.Value;
				return Configuration.Application.XamarinRuntime == XamarinRuntime.NativeAOT;
			}
			set {
				create_xml_description_file = value;
			}
		}

		public bool UseXmlDescriptionFile { get; set; }
		public string XmlDescriptionPath { get; set; } = string.Empty;

		public ApplyPreserveAttributeStep ()
		{
			impl = new ApplyPreserveAttributeImpl (this);
		}

		protected override void TryProcess ()
		{
			DerivedLinkContext.DidRunApplyPreserveAttributeStep = true;
			base.TryProcess ();
		}

		protected override bool IsActiveFor (AssemblyDefinition assembly)
		{
			// We only care about assemblies that are being linked.
			if (Annotations.GetAction (assembly) != AssemblyAction.Link)
				return false;

			return true;
		}

		protected override bool ModifyAssembly (AssemblyDefinition assembly)
		{
			return impl.Process (assembly);
		}

		protected override void TryEndProcess ()
		{
			if (!UseXmlDescriptionFile)
				return;

			WriteXmlDescription ();
		}

		bool IApplyPreserveAttribute.PreserveUnconditional (IMetadataTokenProvider provider)
		{
			if (UseXmlDescriptionFile) {
				AddUnconditionalXmlDescription (provider);
				return false;
			}

			// We want to add a dynamic dependency attribute to preserve methods and fields
			// but not to preserve types while we're marking the chain of declaring types.
			if (provider is not TypeDefinition) {
				return AddDynamicDependencyAttribute (provider);
			}
			return false;
		}

		bool IApplyPreserveAttribute.PreserveType (TypeDefinition type, bool allMembers)
		{
			if (UseXmlDescriptionFile) {
				if (allMembers)
					xmlDescriptor.PreserveTypeWithAllMembers (type);
				else if (type.IsEnum)
					xmlDescriptor.PreserveTypeFields (type);
				else
					xmlDescriptor.PreserveType (type);
				return false;
			}

			return AddDynamicDependencyAttribute (type, allMembers);
		}

		MethodDefinition GetOrCreateModuleConstructor (ModuleDefinition @module, out bool modified)
		{
			var moduleType = @module.GetModuleType ();
			return abr.GetOrCreateStaticConstructor (moduleType, out modified);
		}

		bool IApplyPreserveAttribute.PreserveConditional (TypeDefinition onType, MethodDefinition forMethod)
		{
			if (UseXmlDescriptionFile) {
				xmlDescriptor.PreserveMethod (forMethod, required: false);
				return false;
			}

			return AddConditionalDynamicDependencyAttribute (onType, forMethod);
		}

		bool AddDynamicDependencyAttribute (TypeDefinition type, bool allMembers)
		{
			var moduleConstructor = GetOrCreateModuleConstructor (abr.CurrentAssembly.MainModule, out var modified);

			// only preserve fields for enums
			if (type.IsEnum) {
				var enumAttrib = abr.CreateDynamicDependencyAttribute (DynamicallyAccessedMemberTypes.PublicFields, type);
				modified |= abr.AddAttributeOnlyOnce (moduleConstructor, enumAttrib);
				return modified;
			}

			if (allMembers) {
				modified |= abr.AddPreserveAllMembersDynamicDependencyAttributes (moduleConstructor, type);
				return modified;
			}

			var members = DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors;
			var attrib = abr.CreateDynamicDependencyAttribute (members, type);
			modified |= abr.AddAttributeOnlyOnce (moduleConstructor, attrib);
			return modified;
		}

		bool AddConditionalDynamicDependencyAttribute (TypeDefinition onType, MethodDefinition forMethod)
		{
			return abr.AddDynamicDependencyAttributeToStaticConstructor (onType, forMethod);
		}

		bool AddDynamicDependencyAttribute (IMetadataTokenProvider provider)
		{
			var member = provider as IMemberDefinition;
			if (member is null)
				throw ErrorHelper.CreateError (99, $"Unable to add dynamic dependency attribute to {provider.GetType ().FullName}");

			var moduleConstructor = GetOrCreateModuleConstructor (member.GetModule (), out var modified);
			var signature = DocumentationComments.GetSignature (member);
			var attrib = abr.CreateDynamicDependencyAttribute (signature, member.DeclaringType);
			modified |= abr.AddAttributeOnlyOnce (moduleConstructor, attrib);
			return modified;
		}

		string GetXmlDescriptionFilePath ()
		{
			if (!string.IsNullOrEmpty (XmlDescriptionPath))
				return XmlDescriptionPath;

			return Path.Combine (Configuration.CacheDirectory, "apply-preserve-attribute.xml");
		}

		void AddUnconditionalXmlDescription (IMetadataTokenProvider provider)
		{
			switch (provider) {
			case MethodDefinition method:
				xmlDescriptor.PreserveMethod (method);
				break;
			case FieldDefinition field:
				xmlDescriptor.PreserveField (field);
				break;
			}
		}

		void WriteXmlDescription ()
		{
			var xmlPath = GetXmlDescriptionFilePath ();
			xmlDescriptor.Save (xmlPath);

			if (CreateXmlDescriptionFile) {
				var items = new List<MSBuildItem> ();
				var item = new MSBuildItem (xmlPath);
				items.Add (item);
				Configuration.WriteOutputForMSBuild ("TrimmerRootDescriptor", items);
			}

		}
	}
}
