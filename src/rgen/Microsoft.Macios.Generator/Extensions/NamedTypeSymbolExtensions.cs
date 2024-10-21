using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator.Attributes;

namespace Microsoft.Macios.Generator.Extensions;

static class NamedTypeSymbolExtensions {
	public static bool TryGetEnumFields (this INamedTypeSymbol enumSymbol,
		[NotNullWhen (true)]
		out ImmutableArray<(IFieldSymbol Symbol, FieldData FieldData)>? fields,
		[NotNullWhen (false)] out ImmutableArray<Diagnostic>? diagnostics)
	{
		fields = null;
		diagnostics = null;

		// because we are dealing with an enum, we need to get all the fields from the symbol but we need to
		// keep the order in which they are defined in the source code.

		var fieldBucket =
			ImmutableArray.CreateBuilder<(IFieldSymbol Symbol, FieldData FieldData)> ();

		var members = enumSymbol.GetMembers ().OfType<IFieldSymbol> ().ToArray ();
		foreach (var fieldSymbol in members) {
			var attributes = fieldSymbol.GetAttributeData ();
			if (attributes.Count == 0)
				continue;

			// Get all the FieldAttribute, parse it and add the data to the result
			if (attributes.TryGetValue (AttributesNames.FieldAttribute, out var fieldAttrData)) {
				var fieldSyntax = fieldAttrData.ApplicationSyntaxReference?.GetSyntax ();
				if (fieldSyntax is null)
					continue;

				if (FieldData.TryParse (fieldSyntax, fieldAttrData, out var fieldData)) {
					fieldBucket.Add ((Symbol: fieldSymbol, FieldData: fieldData));
				} else {
					// TODO: diagnostics
				}
			}
		}

		fields = fieldBucket.ToImmutable ();
		return true;
	}
}
