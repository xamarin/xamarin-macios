// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.CodeAnalysis;
using Microsoft.Macios.Transformer;
using Microsoft.Macios.Transformer.Attributes;

namespace Microsoft.Macios.Generator.Extensions;

static partial class FieldSymbolExtensions {

	/// <summary>
	/// Retrieve the FieldData from the field symbol, usually used for enum values.
	/// </summary>
	/// <param name="fieldSymbol">The symbol under query.</param>
	/// <returns>The data of the FieldAttribute that was used on the symbol or null if it is not present.</returns>
	public static FieldData? GetFieldData (this IFieldSymbol fieldSymbol)
		=> GetFieldData<FieldData> (fieldSymbol, AttributesNames.FieldAttribute, FieldData.TryParse);
}
