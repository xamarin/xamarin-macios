using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Microsoft.Macios.Generator.Extensions;

static class StringExtensions {

	public static bool IsValidIdentifier ([NotNullWhen (true)] this string? self)
	{
		if (self is null)
			return false;
		var kind = SyntaxFacts.GetKeywordKind (self);
		return !SyntaxFacts.IsKeywordKind (kind) && SyntaxFacts.IsValidIdentifier (self);
	}
}
