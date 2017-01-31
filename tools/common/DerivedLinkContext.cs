using System.Collections.Generic;

using Mono.Cecil;
using Mono.Linker;

using XamCore.Registrar;

namespace Xamarin.Tuner
{
	public class DerivedLinkContext : LinkContext
	{
		internal StaticRegistrar StaticRegistrar;
		Dictionary<string, List<MemberReference>> required_symbols;
		Dictionary<string, TypeDefinition> objectivec_classes;

		// SDK candidates - they will be preserved only if the application (not the SDK) uses it
		List<ICustomAttributeProvider> srs_data_contract = new List<ICustomAttributeProvider> ();
		List<ICustomAttributeProvider> xml_serialization = new List<ICustomAttributeProvider> ();

		HashSet<TypeDefinition> cached_isnsobject;
		HashSet<TypeDefinition> needs_isdirectbinding_check;
		HashSet<MethodDefinition> generated_code;

		public HashSet<TypeDefinition> CachedIsNSObject {
			get { return cached_isnsobject; }
			set { cached_isnsobject = value; }
		}

		public HashSet<TypeDefinition> NeedsIsDirectBindingCheck {
			get { return needs_isdirectbinding_check; }
			set { needs_isdirectbinding_check = value; }
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

		public List<MemberReference> GetRequiredSymbolList (string symbol)
		{
			List<MemberReference> rv;
			if (!RequiredSymbols.TryGetValue (symbol, out rv))
				required_symbols [symbol] = rv = new List<MemberReference> ();
			return rv;
		}

		public Dictionary<string, List<MemberReference>> RequiredSymbols {
			get {
				if (required_symbols == null)
					required_symbols = new Dictionary<string, List<MemberReference>> ();
				return required_symbols;
			}
		}

		public Dictionary<string, TypeDefinition> ObjectiveCClasses {
			get {
				if (objectivec_classes == null)
					objectivec_classes = new Dictionary<string, TypeDefinition> ();
				return objectivec_classes;
			}
		}
		
		public DerivedLinkContext (Pipeline pipeline, AssemblyResolver resolver)
			: base (pipeline, resolver)
		{
		}
	}
}
