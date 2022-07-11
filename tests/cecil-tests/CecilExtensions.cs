using Mono.Cecil;

namespace Xamarin.Utils {
	public static partial class CecilExtensions {

		// note: direct check, no inheritance
		public static bool Is (this TypeReference type, string @namespace, string name)
		{
			return (type is not null) && (type.Name == name) && (type.Namespace == @namespace);
		}
	}
}
