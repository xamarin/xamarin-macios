using System.Collections.Generic;

using Mono.Cecil;
using Mono.Linker;

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
		HashSet<MethodDefinition> generated_code;

		public HashSet<TypeDefinition> CachedIsNSObject {
			get { return cached_isnsobject; }
			set { cached_isnsobject = value; }
		}

		public Dictionary<TypeDefinition, bool?> IsDirectBindingValue {
			get { return isdirectbinding_value; }
			set { isdirectbinding_value = value; }
		}

		public HashSet<MethodDefinition> GeneratedCode {
			get { return generated_code; }
			set { generated_code = value; }
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
	}
}
