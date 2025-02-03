// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator.Attributes;
using ObjCBindings;

namespace Microsoft.Macios.Generator.Extensions;

static partial class FieldSymbolExtensions {

	/// <summary>
	/// Retrieve the FieldData from the field symbol.
	/// </summary>
	/// <param name="fieldSymbol">The field whose attribute data we want to parse.</param>
	/// <returns>True if the FieldData attribute was presend and could be parsed.</returns>
	public static FieldData<EnumValue>? GetFieldData (this IFieldSymbol fieldSymbol)
		=> GetFieldData<FieldData<EnumValue>> (fieldSymbol, AttributesNames.EnumFieldAttribute, FieldData<EnumValue>.TryParse);
}
