// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

namespace Microsoft.Macios.Transformer.Attributes;

readonly record struct BindData {

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
}
