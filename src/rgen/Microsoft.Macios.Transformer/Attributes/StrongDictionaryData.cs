// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

namespace Microsoft.Macios.Transformer.Attributes;

readonly record struct StrongDictionaryData {

	public string TypeWithKeys { get; }
	public string? Suffix { get; }

	public StrongDictionaryData (string typeWithKeys)
	{
		TypeWithKeys = typeWithKeys;
	}

	public StrongDictionaryData (string typeWithKeys, string? suffix)
	{
		TypeWithKeys = typeWithKeys;
		Suffix = suffix;
	}

	public static bool TryParse (AttributeData attributeData,
		[NotNullWhen (true)] out StrongDictionaryData? data)
	{
		data = null;
		var count = attributeData.ConstructorArguments.Length;
		string typeWithKeys;
		string? suffix = null;

		// custom marshal directive values

		switch (count) {
		case 1:
			typeWithKeys = (string) attributeData.ConstructorArguments [0].Value!;
			break;
		default:
			// 0 should not be an option..
			return false;
		}

		if (attributeData.NamedArguments.Length == 0) {
			data = new (typeWithKeys);
			return true;
		}

		foreach (var (argumentName, value) in attributeData.NamedArguments) {
			switch (argumentName) {
			case "TypeWithKeys":
				typeWithKeys = (string?) value.Value!;
				break;
			case "Suffix":
				suffix = (string?) value.Value!;
				break;
			}
		}

		data = new (typeWithKeys, suffix);
		return true;
	}
}
