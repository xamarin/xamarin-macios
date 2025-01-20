// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
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
}
