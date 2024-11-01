using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

namespace Microsoft.Macios.Generator.Extensions;

static class TypedConstantExtensions {

	public static bool TryGetIdentifier (this TypedConstant self, [NotNullWhen (true)] out string? value)
	{
		value = self.Value as string;
		return value.IsValidIdentifier ();
	}
}
