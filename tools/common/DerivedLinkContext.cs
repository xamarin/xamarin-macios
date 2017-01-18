using System.Collections.Generic;

using Mono.Cecil;
using Mono.Linker;

namespace Xamarin.Tuner
{
	public class DerivedLinkContext : LinkContext
	{
		Dictionary<string, MemberReference> required_symbols;
		List<MethodDefinition> marshal_exception_pinvokes;

		public Dictionary<string, MemberReference> RequiredSymbols {
			get {
				if (required_symbols == null)
					required_symbols = new Dictionary<string, MemberReference> ();
				return required_symbols;
			}
		}

		public List<MethodDefinition> MarshalExceptionPInvokes {
			get {
				if (marshal_exception_pinvokes == null)
					marshal_exception_pinvokes = new List<MethodDefinition> ();
				return marshal_exception_pinvokes;
			}
		}

		public DerivedLinkContext (Pipeline pipeline, AssemblyResolver resolver)
			: base (pipeline, resolver)
		{
		}
	}
}
