// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

namespace Microsoft.Macios.Transformer.Attributes;

readonly record struct SnippetData {

	public string Code { get; }

	public bool Optimizable { get; }

	public SnippetData (string code)
	{
		Code = code;
	}

	public SnippetData (string code, bool optimizable)
	{
		Code = code;
		Optimizable = optimizable;
	}

	public static bool TryParse (AttributeData attributeData,
		[NotNullWhen (true)] out SnippetData? data)
	{
		data = null;
		var count = attributeData.ConstructorArguments.Length;
		string code;
		var optimizable = false;

		// custom marshal directive values

		switch (count) {
		case 1:
			code = (string) attributeData.ConstructorArguments [0].Value!;
			break;
		default:
			// 0 should not be an option..
			return false;
		}

		if (attributeData.NamedArguments.Length == 0) {
			data = new (code);
			return true;
		}

		foreach (var (argumentName, value) in attributeData.NamedArguments) {
			switch (argumentName) {
			case "Code":
				code = (string?) value.Value!;
				break;
			case "Optimizable":
				optimizable = (bool) value.Value!;
				break;
			default:
				data = null;
				return false;
			}
		}

		data = new (code, optimizable);
		return true;
	}
}
