// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

namespace Microsoft.Macios.Generator.Attributes;

readonly struct BindFromData : IEquatable<BindFromData> {

	public string Type { get; }
	public string? OriginalType { get; }

	public BindFromData (string type)
	{
		Type = type;
	}

	public BindFromData (string type, string? originalType)
	{
		Type = type;
		OriginalType = originalType;
	}


	public static bool TryParse (AttributeData attributeData,
		[NotNullWhen (true)] out BindFromData? data)
	{
		data = null;
		var count = attributeData.ConstructorArguments.Length;
		string? type;
		string? originalType = null;

		switch (count) {
		case 1:
			type = ((INamedTypeSymbol) attributeData.ConstructorArguments [0].Value!).ToDisplayString ();
			break;
		default:
			// no other constructors are available
			return false;
		}

		if (attributeData.NamedArguments.Length == 0) {
			data = new (type);
			return true;
		}

		foreach (var (name, value) in attributeData.NamedArguments) {
			switch (name) {
			case "Type":
				type = ((INamedTypeSymbol) value.Value!).ToDisplayString ();
				break;
			case "OriginalType":
				originalType = ((INamedTypeSymbol) value.Value!).ToDisplayString ();
				break;
			default:
				data = null;
				return false;
			}
		}
		data = new (type, originalType);
		return true;
	}

	/// <inheritdoc />
	public bool Equals (BindFromData other)
	{
		if (Type != other.Type)
			return false;
		return OriginalType == other.OriginalType;
	}

	/// <inheritdoc />
	public override bool Equals (object? obj)
	{
		return obj is BindFromData other && Equals (other);
	}

	/// <inheritdoc />
	public override int GetHashCode ()
	{
		return HashCode.Combine (Type, OriginalType);
	}

	public static bool operator == (BindFromData x, BindFromData y)
	{
		return x.Equals (y);
	}

	public static bool operator != (BindFromData x, BindFromData y)
	{
		return !(x == y);
	}

	public override string ToString ()
	{
		return $"{{ Type: '{Type}', OriginalType: '{OriginalType ?? "null"}' }}";
	}
}
