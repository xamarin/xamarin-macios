using System;

using Mono.Cecil;
namespace Microsoft.MaciOS.Nnyeah {
	public static class MethodReferenceExtensions {
		public static MethodReference WithParameters (this MethodReference reference, params TypeReference [] types)
		{
			foreach (var type in types) {
				reference.Parameters.Add (new ParameterDefinition (type));
			}
			return reference;
		}
		public static MethodReference WithParameter (this MethodReference reference, TypeReference type)
		{
			reference.Parameters.Add (new ParameterDefinition (type));
			return reference;
		}
	}
}

