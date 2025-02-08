// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator.Availability;
using Microsoft.Macios.Transformer.Attributes;

namespace Microsoft.Macios.Generator.DataModel;

readonly partial struct Accessor {

	readonly ExportData? overrideExportData;

	/// <summary>
	/// The data of the field attribute used to mark the value as a property binding. 
	/// </summary>
	public ExportData? ExportPropertyData {
		get => overrideExportData ?? ExportAttribute;
		init => overrideExportData = value;
	}

	/// <summary>
	/// State if we should marshal native exceptions when generating the accessor.
	/// </summary>
	public bool MarshalNativeExceptions => HasMarshalNativeExceptionsFlag;

	public Accessor (AccessorKind accessorKind,
		SymbolAvailability symbolAvailability,
		Dictionary<string, List<AttributeData>> attributes)
	{
		Kind = accessorKind;
		SymbolAvailability = symbolAvailability;
		AttributesDictionary = attributes;

		// we trust the modifiers of the property itself
		Modifiers = [];
	}

	/// <inheritdoc />
	public override string ToString ()
	{
		var sb = new StringBuilder ($"{{ Kind: {Kind}, ");
		sb.Append ($"Supported Platforms: {SymbolAvailability}, ");
		sb.Append ($"ExportData: {ExportPropertyData?.ToString () ?? "null"} Modifiers: [");
		sb.AppendJoin (",", Modifiers.Select (x => x.Text));
		sb.Append ("] }");
		return sb.ToString ();
	}
}
