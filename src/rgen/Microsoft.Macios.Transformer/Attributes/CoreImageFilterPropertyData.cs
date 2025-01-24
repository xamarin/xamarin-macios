// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

namespace Microsoft.Macios.Transformer.Attributes;

readonly struct CoreImageFilterPropertyData : IEquatable<CoreImageFilterPropertyData> {
	
	public string Name { get; }

	public CoreImageFilterPropertyData (string name)
	{
		Name = name;
	}

	public static bool TryParse (AttributeData attributeData,
		[NotNullWhen (true)] out CoreImageFilterPropertyData? data)
	{
		data = null;
		var count = attributeData.ConstructorArguments.Length;
		string name;
		
		switch (count) {
		case 1:
			name = (string) attributeData.ConstructorArguments [0].Value!;
			break;
		default:
			// 0 should not be an option..
			return false;
		}

		if (attributeData.NamedArguments.Length == 0) {
			data = new (name);
			return true;
		}

		foreach (var (argumentName, value) in attributeData.NamedArguments) {
			switch (argumentName) {
			case "Name":
				name = (string) value.Value!;
				break;
			default:
				data = null;
				return false;
			}
		}

		data = new(name);
		return true;
	}

	public bool Equals (CoreImageFilterPropertyData other)
		=> Name == other.Name;

	/// <inheritdoc />
	public override bool Equals (object? obj)
	{
		return obj is CoreImageFilterPropertyData other && Equals (other);
	}

	/// <inheritdoc />
	public override int GetHashCode ()
		=> HashCode.Combine (Name);


	public static bool operator == (CoreImageFilterPropertyData x, CoreImageFilterPropertyData y)
	{
		return x.Equals (y);
	}

	public static bool operator != (CoreImageFilterPropertyData x, CoreImageFilterPropertyData y)
	{
		return !(x == y);
	}

	/// <inheritdoc />
	public override string ToString ()
	{
		return $"{{ Name: '{Name}' }}";
	}
}
