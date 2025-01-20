// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

namespace Microsoft.Macios.Transformer.Attributes;

public struct FieldData : IEquatable<FieldData> {

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
		data = default;

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

	public bool Equals (FieldData other)
	{
		if (SymbolName != other.SymbolName)
			return false;
		return LibraryName == other.LibraryName;
	}

	/// <inheritdoc />
	public override bool Equals (object? obj)
	{
		return obj is FieldData other && Equals (other);
	}

	/// <inheritdoc />
	public override int GetHashCode ()
	{
		return HashCode.Combine (SymbolName, LibraryName);
	}

	public static bool operator == (FieldData x, FieldData y)
	{
		return x.Equals (y);
	}

	public static bool operator != (FieldData x, FieldData y)
	{
		return !(x == y);
	}

	public override string ToString ()
	{
		return $"{{ SymbolName: '{SymbolName}' LibraryName: '{LibraryName ?? "null"}' }}";
	}
}
