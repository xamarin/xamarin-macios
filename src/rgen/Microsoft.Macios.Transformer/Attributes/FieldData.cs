// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

namespace Microsoft.Macios.Transformer.Attributes;

readonly record struct FieldData {

	public string SymbolName { get; }
	public string? LibraryName { get; }

	internal FieldData (string symbolName, string? libraryName)
	{
		SymbolName = symbolName;
		LibraryName = libraryName;
	}

	internal FieldData (string symbolName) : this (symbolName, null) { }

	public static bool TryParse (AttributeData attributeData,
		[NotNullWhen (true)] out FieldData? data)
	{
		data = null;

		var count = attributeData.ConstructorArguments.Length;
		string? symbolName;
		string? libraryName = null;
		switch (count) {
		case 1:
			symbolName = (string?) attributeData.ConstructorArguments [0].Value!;
			break;
		case 2:
			symbolName = (string?) attributeData.ConstructorArguments [0].Value!;
			libraryName = (string?) attributeData.ConstructorArguments [1].Value!;
			break;
		default:
			// 0 should not be an option.
			return false;
		}

		if (attributeData.NamedArguments.Length == 0) {
			data = new (symbolName, libraryName);
			return true;
		}

		// LibraryName can be a param value
		foreach (var (name, value) in attributeData.NamedArguments) {
			switch (name) {
			case "LibraryName":
				libraryName = (string?) value.Value!;
				break;
			default:
				data = null;
				return false;
			}
		}
		data = new (symbolName, libraryName);
		return true;
	}
}
