// This is copied from https://github.com/mono/linker/blob/fa9ccbdaf6907c69ef1bb117906f8f012218d57f/src/tuner/Mono.Tuner/ApplyPreserveAttributeBase.cs
// and modified to work without a Profile class.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

using Mono.Linker;
using Mono.Linker.Steps;

using Mono.Cecil;
using Mono.Cecil.Cil;

using Xamarin.Bundler;
using Xamarin.Linker;
using Xamarin.Utils;

#nullable enable

namespace Mono.Tuner {

	public abstract class ApplyPreserveAttributeBase : ConfigurationAwareSubStep {

		AppBundleRewriter? abr;

		protected override string Name { get => "Apply Preserve Attribute"; }

		protected override int ErrorCode { get => 2450; }

		// set 'removeAttribute' to true if you want the preserved attribute to be removed from the final assembly
		protected abstract bool IsPreservedAttribute (ICustomAttributeProvider provider, CustomAttribute attribute, out bool removeAttribute);

		public override SubStepTargets Targets {
			get {
				return SubStepTargets.Type
					| SubStepTargets.Field
					| SubStepTargets.Method
					| SubStepTargets.Property
					| SubStepTargets.Event
					| SubStepTargets.Assembly;
			}
		}

		public override void Initialize (LinkContext context)
		{
			base.Initialize (context);

			if (Configuration.Application.XamarinRuntime == XamarinRuntime.NativeAOT)
				abr = Configuration.AppBundleRewriter;
		}

		public override bool IsActiveFor (AssemblyDefinition assembly)
		{
			return Annotations.GetAction (assembly) == AssemblyAction.Link;
		}

		protected override void Process (TypeDefinition type)
		{
			TryApplyPreserveAttribute (type);
		}

		protected override void Process (FieldDefinition field)
		{
			foreach (var attribute in GetPreserveAttributes (field))
				Mark (field, attribute);
		}

		protected override void Process (MethodDefinition method)
		{
			MarkMethodIfPreserved (method);
		}

		protected override void Process (PropertyDefinition property)
		{
			foreach (var attribute in GetPreserveAttributes (property)) {
				MarkMethod (property.GetMethod, attribute);
				MarkMethod (property.SetMethod, attribute);
			}
		}

		protected override void Process (EventDefinition @event)
		{
			foreach (var attribute in GetPreserveAttributes (@event)) {
				MarkMethod (@event.AddMethod, attribute);
				MarkMethod (@event.InvokeMethod, attribute);
				MarkMethod (@event.RemoveMethod, attribute);
			}
		}

		void MarkMethodIfPreserved (MethodDefinition method)
		{
			foreach (var attribute in GetPreserveAttributes (method))
				MarkMethod (method, attribute);
		}

		void MarkMethod (MethodDefinition? method, CustomAttribute? preserve_attribute)
		{
			if (method is null)
				return;

			Mark (method, preserve_attribute);
			Annotations.SetAction (method, MethodAction.Parse);
		}

		void Mark (IMetadataTokenProvider provider, CustomAttribute? preserve_attribute)
		{
			if (IsConditionalAttribute (preserve_attribute)) {
				PreserveConditional (provider);
				return;
			}

			PreserveUnconditional (provider);
		}

		void PreserveConditional (IMetadataTokenProvider provider)
		{
			var method = provider as MethodDefinition;
			if (method is null) {
				// workaround to support (uncommon but valid) conditional fields form [Preserve]
				PreserveUnconditional (provider);
				return;
			}

			Annotations.AddPreservedMethod (method.DeclaringType, method);
			AddConditionalDynamicDependencyAttribute (method.DeclaringType, method);
		}

		static bool IsConditionalAttribute (CustomAttribute? attribute)
		{
			if (attribute is null)
				return false;

			foreach (var named_argument in attribute.Fields)
				if (named_argument.Name == "Conditional")
					return (bool) named_argument.Argument.Value;

			return false;
		}

		void PreserveUnconditional (IMetadataTokenProvider provider)
		{
			Annotations.Mark (provider);
			AddDynamicDependencyAttribute (provider);

			var member = provider as IMemberDefinition;
			if (member is null || member.DeclaringType is null)
				return;

			Mark (member.DeclaringType, null);
		}

		void TryApplyPreserveAttribute (TypeDefinition type)
		{
			foreach (var attribute in GetPreserveAttributes (type)) {
				PreserveType (type, attribute);
			}
		}

		List<CustomAttribute> GetPreserveAttributes (ICustomAttributeProvider provider)
		{
			List<CustomAttribute> attrs = new List<CustomAttribute> ();

			if (!provider.HasCustomAttributes)
				return attrs;

			var attributes = provider.CustomAttributes;

			for (int i = attributes.Count - 1; i >= 0; i--) {
				var attribute = attributes [i];

				bool remote_attribute;
				if (!IsPreservedAttribute (provider, attribute, out remote_attribute))
					continue;

				attrs.Add (attribute);
				if (remote_attribute)
					attributes.RemoveAt (i);
			}

			return attrs;
		}

		protected void PreserveType (TypeDefinition type, CustomAttribute preserveAttribute)
		{
			var allMembers = false;
			if (preserveAttribute.HasFields) {
				foreach (var named_argument in preserveAttribute.Fields)
					if (named_argument.Name == "AllMembers" && (bool) named_argument.Argument.Value)
						allMembers = true;
			}

			PreserveType (type, allMembers);
		}

		protected void PreserveType (TypeDefinition type, bool allMembers)
		{
			Annotations.Mark (type);
			if (allMembers)
				Annotations.SetPreserve (type, TypePreserve.All);
			AddDynamicDependencyAttribute (type, allMembers);
		}

		MethodDefinition GetOrCreateModuleConstructor (ModuleDefinition @module)
		{
			var moduleType = @module.GetModuleType ();
			var moduleConstructor = moduleType.GetTypeConstructor ();
			if (moduleConstructor is null) {
				moduleConstructor = moduleType.AddMethod (".cctor", MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.RTSpecialName | MethodAttributes.SpecialName | MethodAttributes.Static, abr!.System_Void);
				moduleConstructor.CreateBody (out var il);
				il.Emit (OpCodes.Ret);
			}
			return moduleConstructor;
		}

		void AddDynamicDependencyAttribute (TypeDefinition type, bool allMembers)
		{
			if (abr is null)
				return;

			abr.ClearCurrentAssembly ();
			abr.SetCurrentAssembly (type.Module.Assembly);

			var moduleConstructor = GetOrCreateModuleConstructor (type.GetModule ());
			var attrib = abr.CreateDynamicDependencyAttribute (allMembers ? DynamicallyAccessedMemberTypes.All : DynamicallyAccessedMemberTypes.None, type);
			moduleConstructor.CustomAttributes.Add (attrib);

			abr.ClearCurrentAssembly ();
		}

		void AddConditionalDynamicDependencyAttribute (TypeDefinition onType, MethodDefinition forMethod)
		{
			if (abr is null)
				return;

			// I haven't found a way to express a conditional Preserve attribute using DynamicDependencyAttribute :/
			ErrorHelper.Warning (2112, Errors.MX2112 /* Unable to apply the conditional [Preserve] attribute on the member {0} */, forMethod.FullName);
		}

		void AddDynamicDependencyAttribute (IMetadataTokenProvider provider)
		{
			if (abr is null)
				return;

			var member = provider as IMemberDefinition;
			if (member is null)
				throw ErrorHelper.CreateError (99, $"Unable to add dynamic dependency attribute to {provider.GetType ().FullName}");

			var module = member.GetModule ();
			abr.ClearCurrentAssembly ();
			abr.SetCurrentAssembly (module.Assembly);

			var moduleConstructor = GetOrCreateModuleConstructor (module);
			var signature = DocumentationComments.GetSignature (member);
			var attrib = abr.CreateDynamicDependencyAttribute (signature, member.DeclaringType);
			moduleConstructor.CustomAttributes.Add (attrib);

			abr.ClearCurrentAssembly ();
		}
	}
}
