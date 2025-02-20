// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Macios.Transformer.Attributes;

namespace Microsoft.Macios.Generator.DataModel;

readonly record struct FieldInfo {

	/// <summary>
	/// Name of the library that contains the smart enum definition.
	/// </summary>
	public string LibraryName { get; }

	/// <summary>
	/// Path of the library that contains the smart enum definition.
	/// </summary>
	public string? LibraryPath { get; }

	/// <summary>
	/// The data of the field attribute used to mark the value as a binding.
	/// </summary>
	public FieldData? FieldData { get; }

	public FieldInfo (FieldData? fieldData, string libraryName, string? libraryPath = null)
	{
		FieldData = fieldData;
		LibraryName = libraryName;
		LibraryPath = libraryPath;
	}
}
