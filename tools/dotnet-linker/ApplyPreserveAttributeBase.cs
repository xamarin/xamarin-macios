// This is copied from https://github.com/mono/linker/blob/fa9ccbdaf6907c69ef1bb117906f8f012218d57f/src/tuner/Mono.Tuner/ApplyPreserveAttributeBase.cs
// and modified to work without a Profile class.

using System.Linq;

using Mono.Linker;
using Mono.Linker.Steps;

using Mono.Cecil;
using Mono.Tuner;

#nullable enable

namespace Xamarin.Linker.Steps {
	public interface IApplyPreserveAttribute {
		bool PreserveType (TypeDefinition type, bool allMembers);
		bool PreserveUnconditional (IMetadataTokenProvider provider);
		bool PreserveConditional (TypeDefinition onType, MethodDefinition forMethod);
	}

	public class ApplyPreserveAttributeImpl {
		IApplyPreserveAttribute applyPreserveAttribute;

		public ApplyPreserveAttributeImpl (IApplyPreserveAttribute applyPreserveAttribute)
		{
			this.applyPreserveAttribute = applyPreserveAttribute;
		}

		bool IsPreservedAttribute (ICustomAttributeProvider provider, CustomAttribute attribute)
		{
			TypeReference type = attribute.Constructor.DeclaringType;
			return type.Name == "PreserveAttribute";
		}

		public bool Process (AssemblyDefinition assembly)
		{
			var modified = false;
			modified |= BrowseTypes (assembly.MainModule.Types);
			modified |= ProcessAssemblyAttributes (assembly);
			return modified;
		}

		bool ProcessAssemblyAttributes (AssemblyDefinition assembly)
		{
			if (!assembly.HasCustomAttributes)
				return false;

			var modified = false;
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

				modified |= PreserveType (type, attribute);
			}
			return modified;
		}

		bool BrowseTypes (IEnumerable<TypeDefinition> types)
		{
			var modified = false;
			foreach (var type in (new List<TypeDefinition> (types))) {
				modified |= ProcessType (type);

				if (type.HasFields) {
					foreach (var field in type.Fields.ToArray ())
						modified |= ProcessField (field);
				}

				if (type.HasMethods) {
					foreach (var method in type.Methods.ToArray ())
						modified |= ProcessMethod (method);
				}

				if (type.HasProperties) {
					foreach (var property in type.Properties.ToArray ())
						modified |= ProcessProperty (property);
				}

				if (type.HasEvents) {
					foreach (var @event in type.Events.ToArray ())
						modified |= ProcessEvent (@event);
				}

				if (type.HasNestedTypes) {
					modified |= BrowseTypes (type.NestedTypes);
				}
			}
			return modified;
		}

		bool ProcessType (TypeDefinition type)
		{
			return TryApplyPreserveAttribute (type);
		}

		bool ProcessField (FieldDefinition field)
		{
			var modified = false;
			foreach (var attribute in GetPreserveAttributes (field))
				modified |= Mark (field, attribute);
			return modified;
		}

		bool ProcessMethod (MethodDefinition method)
		{
			return MarkMethodIfPreserved (method);
		}

		bool ProcessProperty (PropertyDefinition property)
		{
			var modified = false;
			foreach (var attribute in GetPreserveAttributes (property)) {
				modified |= MarkMethod (property.GetMethod, attribute);
				modified |= MarkMethod (property.SetMethod, attribute);
			}
			return modified;
		}

		bool ProcessEvent (EventDefinition @event)
		{
			var modified = false;
			foreach (var attribute in GetPreserveAttributes (@event)) {
				modified |= MarkMethod (@event.AddMethod, attribute);
				modified |= MarkMethod (@event.InvokeMethod, attribute);
				modified |= MarkMethod (@event.RemoveMethod, attribute);
			}
			return modified;
		}

		bool MarkMethodIfPreserved (MethodDefinition method)
		{
			var modified = false;
			foreach (var attribute in GetPreserveAttributes (method))
				modified |= MarkMethod (method, attribute);
			return modified;
		}

		bool MarkMethod (MethodDefinition? method, CustomAttribute? preserve_attribute)
		{
			if (method is null)
				return false;

			return Mark (method, preserve_attribute);
		}

		bool Mark (IMetadataTokenProvider provider, CustomAttribute? preserve_attribute)
		{
			if (IsConditionalAttribute (preserve_attribute))
				return PreserveConditional (provider);

			return PreserveUnconditional (provider);
		}

		bool PreserveConditional (IMetadataTokenProvider provider)
		{
			var method = provider as MethodDefinition;
			if (method is null) {
				// workaround to support (uncommon but valid) conditional fields form [Preserve]
				return PreserveUnconditional (provider);
			}

			return applyPreserveAttribute.PreserveConditional (method.DeclaringType, method);
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

		bool PreserveUnconditional (IMetadataTokenProvider provider)
		{
			var modified = false;

			modified |= applyPreserveAttribute.PreserveUnconditional (provider);

			var member = provider as IMemberDefinition;
			if (member is null || member.DeclaringType is null)
				return modified;

			modified |= Mark (member.DeclaringType, null);

			return modified;
		}

		bool TryApplyPreserveAttribute (TypeDefinition type)
		{
			var modified = false;
			foreach (var attribute in GetPreserveAttributes (type)) {
				modified |= PreserveType (type, attribute);
			}
			return modified;
		}

		List<CustomAttribute> GetPreserveAttributes (ICustomAttributeProvider provider)
		{
			if (!provider.HasCustomAttributes)
				return new List<CustomAttribute> ();

			return provider.CustomAttributes.Where (a => IsPreservedAttribute (provider, a)).ToList ();
		}

		protected bool PreserveType (TypeDefinition type, CustomAttribute preserveAttribute)
		{
			var allMembers = false;
			if (preserveAttribute.HasFields) {
				foreach (var named_argument in preserveAttribute.Fields)
					if (named_argument.Name == "AllMembers" && (bool) named_argument.Argument.Value)
						allMembers = true;
			}

			return PreserveType (type, allMembers);
		}

		bool PreserveType (TypeDefinition type, bool allMembers)
		{
			return applyPreserveAttribute.PreserveType (type, allMembers);
		}
	}
}
