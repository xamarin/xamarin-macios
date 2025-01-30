// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

namespace Microsoft.Macios.Transformer.Attributes;

readonly record struct NativeData {

	public string? NativeName { get; }

	public NativeData () { }

	public NativeData (string? nativeName)
	{
		NativeName = nativeName;
	}


	public static bool TryParse (AttributeData attributeData,
		[NotNullWhen (true)] out NativeData? data)
	{
		data = null;
		var count = attributeData.ConstructorArguments.Length;
		string? nativeName;

		switch (count) {
		case 0:
			nativeName = null;
			break;
		case 1:
			nativeName = (string) attributeData.ConstructorArguments [0].Value!;
			break;
		default:
			// 0 should not be an option..
			return false;
		}

		if (attributeData.NamedArguments.Length == 0) {
			data = new (nativeName);
			return true;
		}

		foreach (var (argumentName, value) in attributeData.NamedArguments) {
			switch (argumentName) {
			case "NativeName":
				nativeName = (string) value.Value!;
				break;
			default:
				data = null;
				return false;
			}
		}

		data = new (nativeName);
		return true;
	}
}
