// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

namespace Microsoft.Macios.Transformer.Attributes;

readonly record struct BindAsData {
	
	public string Type { get; init; }
	public string? OriginalType { get; init; }

	public BindAsData (string type)
	{
		Type = type;
	}

	public BindAsData (string type, string? originalType)
	{
		Type = type;
		OriginalType = originalType;
		
	}

	public static bool TryParse (AttributeData attributeData,
		[NotNullWhen (true)] out BindAsData? data)
	{
		data = null;
		var count = attributeData.ConstructorArguments.Length;
		string type;
		string? originalType = null;
		
		switch (count) {
		case 1:
			type = ((ITypeSymbol) attributeData.ConstructorArguments [0].Value!).ToDisplayString ();
			break;
		default:
			// 0 should not be an option..
			return false;
		}
		
		if (attributeData.NamedArguments.Length == 0) {
			data = new (type);
			return true;
		}
		
		foreach (var (argumentName, value) in attributeData.NamedArguments) {
			switch (argumentName) {
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

}
