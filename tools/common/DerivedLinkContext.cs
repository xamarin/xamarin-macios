using System.Collections.Generic;

using Mono.Cecil;
using Mono.Linker;
using Mono.Collections.Generic;

using XamCore.Registrar;
using Xamarin.Bundler;

namespace Xamarin.Tuner
{
	public class DerivedLinkContext : LinkContext
	{
		internal StaticRegistrar StaticRegistrar;
		internal Target Target;
		Symbols required_symbols;

		// SDK candidates - they will be preserved only if the application (not the SDK) uses it
		List<ICustomAttributeProvider> srs_data_contract = new List<ICustomAttributeProvider> ();
		List<ICustomAttributeProvider> xml_serialization = new List<ICustomAttributeProvider> ();

		HashSet<TypeDefinition> cached_isnsobject;
		// Tristate:
		//   null = don't know, must check at runtime (can't inline)
		//   true/false = corresponding constant value
		Dictionary<TypeDefinition, bool?> isdirectbinding_value;

		public HashSet<TypeDefinition> CachedIsNSObject {
			get { return cached_isnsobject; }
			set { cached_isnsobject = value; }
		}

		public Dictionary<TypeDefinition, bool?> IsDirectBindingValue {
			get { return isdirectbinding_value; }
			set { isdirectbinding_value = value; }
		}

		public IList<ICustomAttributeProvider> DataContract {
			get {
				return srs_data_contract;
			}
		}

		public IList<ICustomAttributeProvider> XmlSerialization {
			get {
				return xml_serialization;
			}
		}

		public Symbols RequiredSymbols {
			get {
				if (required_symbols == null)
					required_symbols = new Symbols ();
				return required_symbols;
			}
		}

		public DerivedLinkContext (Pipeline pipeline, AssemblyResolver resolver)
			: base (pipeline, resolver)
		{
			UserAction = AssemblyAction.Link;
		}

		public Dictionary<IMetadataTokenProvider, object> GetAllCustomAttributes (string storage_name)
		{
			return Annotations?.GetCustomAnnotations (storage_name);
		}

		public List<ICustomAttribute> GetCustomAttributes (ICustomAttributeProvider provider, string storage_name)
		{
			var annotations = Annotations?.GetCustomAnnotations (storage_name);
			object storage = null;
			if (annotations?.TryGetValue (provider, out storage) != true)
				return null;
			return (List<ICustomAttribute>) storage;
		}

		// Stores custom attributes in the link context, so that the attribute can be retrieved and
		// inspected even if it's linked away.
		public void StoreCustomAttribute (ICustomAttributeProvider provider, CustomAttribute attribute, string storage_name)
		{
			var dict = Annotations.GetCustomAnnotations (storage_name);
			List<ICustomAttribute> attribs;
			object attribObjects;
			if (!dict.TryGetValue (provider, out attribObjects)) {
				attribs = new List<ICustomAttribute> ();
				dict [provider] = attribs;
			} else {
				attribs = (List<ICustomAttribute>) attribObjects;
			}
			// Make sure the attribute is resolved, since after removing the attribute
			// it won't be able to do it. The 'CustomAttribute.Resolve' method is private, but fetching
			// any property will cause it to be called.
			// We also need to store the constructor's DeclaringType separately, because it may
			// be nulled out from the constructor by the linker if the attribute type itself is linked away.
			var dummy = attribute.HasConstructorArguments;
			attribs.Add (new AttributeStorage { Attribute = attribute, AttributeType = attribute.Constructor.DeclaringType });
		}

		public List<ICustomAttribute> GetCustomAttributes (ICustomAttributeProvider provider, string @namespace, string name)
		{
			// The equivalent StoreCustomAttribute method below ignores the namespace (it's not needed so far since all attribute names we care about are unique),
			// so we need to retrieve the attributes the same way (using the name only).
			return GetCustomAttributes (provider, name);
		}

		public void StoreCustomAttribute (ICustomAttributeProvider provider, CustomAttribute attribute)
		{
			StoreCustomAttribute (provider, attribute, attribute.AttributeType.Name);
		}

		class AttributeStorage : ICustomAttribute
		{
			public CustomAttribute Attribute;
			public TypeReference AttributeType { get; set; }

			public bool HasFields => Attribute.HasFields;
			public bool HasProperties => Attribute.HasProperties;
			public bool HasConstructorArguments => Attribute.HasConstructorArguments;

			public Collection<CustomAttributeNamedArgument> Fields => Attribute.Fields;
			public Collection<CustomAttributeNamedArgument> Properties => Attribute.Properties;
			public Collection<CustomAttributeArgument> ConstructorArguments => Attribute.ConstructorArguments;
		}
	}
}
