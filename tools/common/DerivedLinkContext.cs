using System;
using System.Collections.Generic;

using Mono.Cecil;
using Mono.Linker;
using Mono.Collections.Generic;

using Registrar;
using Mono.Tuner;
using Xamarin.Bundler;

#if NET
using LinkContext = Xamarin.Bundler.DotNetLinkContext;
#endif

namespace Xamarin.Tuner {
	public class DerivedLinkContext : LinkContext {
		internal StaticRegistrar StaticRegistrar => Target.StaticRegistrar;
		internal Target Target;
		Symbols required_symbols;

		// Any errors or warnings during the link process that won't prevent linking from continuing can be stored here.
		// This is typically used to show as many problems as possible per build (so that the user doesn't have to fix one thing, rebuild, fix another, rebuild, fix another, etc).
		// Obviously if the problem is such that cascading errors may end up reported, this should not be used.
		// The errors/warnings will be shown when the linker is done.
		public List<Exception> Exceptions = new List<Exception> ();

		// SDK candidates - they will be preserved only if the application (not the SDK) uses it
		List<ICustomAttributeProvider> srs_data_contract = new List<ICustomAttributeProvider> ();
		List<ICustomAttributeProvider> xml_serialization = new List<ICustomAttributeProvider> ();

		HashSet<TypeDefinition> cached_isnsobject;
		// Tristate:
		//   null = don't know, must check at runtime (can't inline)
		//   true/false = corresponding constant value
		Dictionary<TypeDefinition, bool?> isdirectbinding_value;

		// Store interfaces the linker has linked away so that the static registrar can access them.
		public Dictionary<TypeDefinition, List<TypeDefinition>> ProtocolImplementations { get; private set; } = new Dictionary<TypeDefinition, List<TypeDefinition>> ();
		// Store types the linker has linked away so that the static registrar can access them.
		Dictionary<string, LinkedAwayTypeReference> LinkedAwayTypes = new Dictionary<string, LinkedAwayTypeReference> ();
		// The linked away TypeDefinitions lacks some information (it can't even find itself in the LinkedAwayTypes dictionary)
		// so we need a second dictionary
		Dictionary<TypeDefinition, LinkedAwayTypeReference> LinkedAwayTypeMap = new Dictionary<TypeDefinition, LinkedAwayTypeReference> ();

#if NET
		public DerivedLinkContext (Xamarin.Linker.LinkerConfiguration configuration, Target target)
			: base (configuration)
		{
			this.Target = target;
		}
#endif

		public Application App {
			get {
				return Target.App;
			}
		}

		AssemblyDefinition corlib;
		public AssemblyDefinition Corlib {
			get {
				if (corlib is null) {
					var name = Driver.CorlibName;
					corlib = this.GetAssembly (name);
					if (corlib is null)
						throw ErrorHelper.CreateError (2111, Errors.MX2111 /* Can not find the corlib assembly '{0}' in the list of loaded assemblies. */, name);
				}
				return corlib;
			}
		}
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
				if (required_symbols is null)
					required_symbols = new Symbols ();
				return required_symbols;
			}
		}

		public bool RequireMonoNative {
			get; set;
		}

#if !NET
		public DerivedLinkContext (Pipeline pipeline, AssemblyResolver resolver)
			: base (pipeline, resolver)
		{
			UserAction = AssemblyAction.Link;
		}
#endif

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

		public void StoreProtocolMethods (TypeDefinition type)
		{
			var attribs = Annotations.GetCustomAnnotations ("ProtocolMethods");
			object value;
			if (!attribs.TryGetValue (type, out value))
				attribs [type] = type.Methods.ToArray (); // Make a copy of the collection, since the linker may remove methods from it.
		}

		public IList<MethodDefinition> GetProtocolMethods (TypeDefinition type)
		{
			var attribs = Annotations.GetCustomAnnotations ("ProtocolMethods");
			object value;
			if (attribs.TryGetValue (type, out value))
				return (MethodDefinition []) value;
			return null;
		}

		public void AddLinkedAwayType (TypeDefinition td)
		{
			var latr = new LinkedAwayTypeReference (td);
			LinkedAwayTypes.Add (td.Module.Assembly.Name.Name + ": " + td.FullName, latr);
			LinkedAwayTypeMap.Add (td, latr);
		}

		// Returns a TypeDefinition for a type that was linked away.
		// The Module property is null for the returned TypeDefinition (unfortunately TypeDefinition is
		// sealed, so I can't provide a custom TypeDefinition subclass), so we need to return
		// the module as an out parameter instead.
		public TypeDefinition GetLinkedAwayType (TypeReference tr, out ModuleDefinition module)
		{
			module = null;

			if (tr is TypeDefinition td) {
				// We might get called again for a TypeDefinition we already returned,
				// in which case tr.Scope will be null, so we can't use the code below.
				// Instead look in our second dictionary, of TypeDefinition -> LinkedAwayTypeReference
				// to find the module.
				if (LinkedAwayTypeMap.TryGetValue (td, out var latd))
					module = latd.Module;
				return td;
			}

			string name;
			switch (tr.Scope.MetadataScopeType) {
			case MetadataScopeType.ModuleDefinition:
				var md = (ModuleDefinition) tr.Scope;
				name = md.Assembly.Name.Name;
				break;
			default:
				name = tr.Scope.Name;
				break;
			}

			if (LinkedAwayTypes.TryGetValue (name + ": " + tr.FullName, out var latr)) {
				module = latr.Module;
				return latr.Resolve ();
			}

			return null;
		}

		class AttributeStorage : ICustomAttribute {
			public CustomAttribute Attribute;
			public TypeReference AttributeType { get; set; }

			public bool HasFields => Attribute.HasFields;
			public bool HasProperties => Attribute.HasProperties;
			public bool HasConstructorArguments => Attribute.HasConstructorArguments;

			public Collection<CustomAttributeNamedArgument> Fields => Attribute.Fields;
			public Collection<CustomAttributeNamedArgument> Properties => Attribute.Properties;
			public Collection<CustomAttributeArgument> ConstructorArguments => Attribute.ConstructorArguments;
		}

		class LinkedAwayTypeReference : TypeReference {
			// When a type is linked away, its Module and Scope properties
			// return null.
			// This class keeps those values around.
			TypeDefinition type;
			ModuleDefinition module;
			IMetadataScope scope;

			public LinkedAwayTypeReference (TypeDefinition type)
				: base (type.Namespace, type.Name)
			{
				this.type = type;
				this.module = type.Module;
				this.scope = type.Scope;
			}

			public override TypeDefinition Resolve ()
			{
				return type;
			}

			public override ModuleDefinition Module {
				get {
					return module;
				}
			}

			public override IMetadataScope Scope {
				get { return scope; }
				set { scope = value; }
			}
		}
	}
}
