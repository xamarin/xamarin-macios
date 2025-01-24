// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

namespace Microsoft.Macios.Transformer.Attributes;

readonly struct BindData : IEquatable<BindData> {

	public string Selector { get; }
	public bool Virtual { get; init; }

	public BindData (string selector, bool isVirtual = false)
	{
		Selector = selector;
		Virtual = isVirtual;
	}

	public static bool TryParse (AttributeData attributeData,
		[NotNullWhen (true)] out BindData? data)
	{
		data = null;
		var count = attributeData.ConstructorArguments.Length;
		string selector;
		bool @virtual = false;
		// custom marshal directive values

		switch (count) {
		case 1:
			selector = (string) attributeData.ConstructorArguments [0].Value!;
			break;
		default:
			// 0 should not be an option..
			return false;
		}

		if (attributeData.NamedArguments.Length == 0) {
			data = new (selector);
			return true;
		}

		foreach (var (argumentName, value) in attributeData.NamedArguments) {
			switch (argumentName) {
			case "Selector":
				selector = (string) value.Value!;
				break;
			case "Virtual":
				@virtual = (bool) value.Value!;
				break;
			default:
				data = null;
				return false;
			}
		}

		data = new (selector) { Virtual = @virtual, };
		return true;
	}

	public bool Equals (BindData other)
	{
		if (Selector != other.Selector)
			return false;
		return Virtual == other.Virtual;
	}

	/// <inheritdoc />
	public override bool Equals (object? obj)
	{
		return obj is BindData other && Equals (other);
	}

	/// <inheritdoc />
	public override int GetHashCode ()
		=> HashCode.Combine (Selector, Virtual);


	public static bool operator == (BindData x, BindData y)
	{
		return x.Equals (y);
	}

	public static bool operator != (BindData x, BindData y)
	{
		return !(x == y);
	}

	/// <inheritdoc />
	public override string ToString ()
	{
		return $"{{ BindSelector: '{Selector}', Virtual: {Virtual} }}";
	}
}
