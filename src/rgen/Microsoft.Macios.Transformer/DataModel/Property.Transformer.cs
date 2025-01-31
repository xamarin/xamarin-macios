// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.Macios.Transformer.Attributes;

namespace Microsoft.Macios.Generator.DataModel;

readonly partial struct Property {

	/// <summary>
	/// The data of the field attribute used to mark the value as a field binding. 
	/// </summary>
	public FieldData? ExportFieldData { get; init; }

	/// <summary>
	/// True if the property represents a Objc field.
	/// </summary>
	[MemberNotNullWhen (true, nameof (ExportFieldData))]
	public bool IsField => ExportFieldData is not null;

	public bool IsNotification => throw new NotImplementedException ();

	/// <summary>
	/// The data of the field attribute used to mark the value as a property binding. 
	/// </summary>
	public ExportData? ExportPropertyData { get; init; }

	/// <summary>
	/// True if the property represents a Objc property.
	/// </summary>
	[MemberNotNullWhen (true, nameof (ExportPropertyData))]
	public bool IsProperty => ExportPropertyData is not null;

	/// <summary>
	/// True if the method was exported with the MarshalNativeExceptions flag allowing it to support native exceptions.
	/// </summary>
	public bool MarshalNativeExceptions => throw new NotImplementedException ();
	
	/// <summary>
	/// Returns the bind from data if present in the binding.
	/// </summary>
	public BindAsData? BindAs => BindAsAttribute;

	/// <inheritdoc />
	public bool Equals (Property other) => Comparer.Equals (this, other);

	/// <inheritdoc />
	public override string ToString ()
	{
		var sb = new StringBuilder ($"Name: '{Name}', Type: {ReturnType}, ");
		sb.Append ($"Supported Platforms: {SymbolAvailability}, ");
		sb.Append ($"ExportFieldData: '{ExportFieldData?.ToString () ?? "null"}', ");
		sb.Append ($"ExportPropertyData: '{ExportPropertyData?.ToString () ?? "null"}' Attributes: [");
		sb.AppendJoin (",", Attributes);
		sb.Append ("], Modifiers: [");
		sb.AppendJoin (",", Modifiers.Select (x => x.Text));
		sb.Append ("], Accessors: [");
		sb.AppendJoin (",", Accessors);
		sb.Append (']');
		return sb.ToString ();
	}
}
