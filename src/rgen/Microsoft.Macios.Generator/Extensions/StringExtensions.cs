using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Microsoft.Macios.Generator.Extensions;

public static class StringExtensions {

	public static bool IsValidIdentifier ([NotNullWhen(true)]this string? self)
		=> self is not null && SyntaxFacts.IsValidIdentifier (self);
}
