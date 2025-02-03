// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator.Availability;
using Microsoft.Macios.Transformer.Attributes;

namespace Microsoft.Macios.Generator.DataModel;

readonly partial struct EnumMember {

	/// <summary>
	/// The data of the field attribute used to mark the value as a binding.
	/// </summary>
	public FieldInfo? FieldInfo { get; init; }

	public EnumMember (string name,
		string libraryName,
		string? libraryPath,
		SymbolAvailability symbolAvailability,
		Dictionary<string, List<AttributeData>> attributes)
	{
		Name = name;
		SymbolAvailability = symbolAvailability;
		AttributesDictionary = attributes;
		FieldInfo = FieldAttribute is null ? null : new FieldInfo (FieldAttribute.Value, libraryName, libraryPath);
	}

	public EnumMember (string name,
		string libraryName,
		string? libraryPath,
		FieldData? fieldData,
		SymbolAvailability symbolAvailability)
	{
		Name = name;
		SymbolAvailability = symbolAvailability;
		FieldInfo = new (fieldData, libraryName, libraryPath);
	}

}
