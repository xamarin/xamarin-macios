// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

namespace Microsoft.Macios.Transformer.Attributes;

readonly struct BackingFieldTypeData : IEquatable<BackingFieldTypeData> {

	public string TypeName { get; }

	public BackingFieldTypeData (string typeName)
	{
		TypeName = typeName;
	}

	public static bool TryParse (AttributeData attributeData,
		[NotNullWhen (true)] out BackingFieldTypeData? data)
	{
		data = null;
		var count = attributeData.ConstructorArguments.Length;
		string backingField;
		// custom marshal directive values

		switch (count) {
		case 1:
			backingField = ((INamedTypeSymbol) attributeData.ConstructorArguments [0].Value!).ToDisplayString ();
			break;
		default:
			// 0 should not be an option..
			return false;
		}

		if (attributeData.NamedArguments.Length == 0) {
			data = new (backingField);
			return true;
		}

		foreach (var (argumentName, value) in attributeData.NamedArguments) {
			switch (argumentName) {
			case "BackingFieldType":
				backingField = ((INamedTypeSymbol) value.Value!).ToDisplayString ();
				break;
			default:
				data = null;
				return false;
			}
		}

		data = new (backingField);
		return true;
	}

	public bool Equals (BackingFieldTypeData other)
	{
		return TypeName == other.TypeName;
	}

	/// <inheritdoc />
	public override bool Equals (object? obj)
	{
		return obj is BackingFieldTypeData other && Equals (other);
	}

	/// <inheritdoc />
	public override int GetHashCode ()
		=> TypeName.GetHashCode ();


	public static bool operator == (BackingFieldTypeData x, BackingFieldTypeData y)
	{
		return x.Equals (y);
	}

	public static bool operator != (BackingFieldTypeData x, BackingFieldTypeData y)
	{
		return !(x == y);
	}

	/// <inheritdoc />
	public override string ToString ()
	{
		return $"{{ BackingFieldType: '{TypeName}' }}";
	}
}
