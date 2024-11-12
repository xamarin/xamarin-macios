using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator.Extensions;

namespace Microsoft.Macios.Generator.Attributes;

readonly struct FieldData<T> where T : Enum {
	public string SymbolName { get; }
	public string? LibraryName { get; }

	public T? Flags { get; } = default;

	FieldData (string symbolName, string? libraryName, T? flags)
	{
		SymbolName = symbolName;
		LibraryName = libraryName;
		Flags = flags;
	}

	public static bool TryParse (AttributeData attributeData,
		[NotNullWhen (true)] out FieldData<T>? data)
	{
		data = default;

		var count = attributeData.ConstructorArguments.Length;
		string? symbolName;
		string? libraryName = null;
		T? flags = default;
		switch (count) {
		case 1:
			if (!attributeData.ConstructorArguments [0].TryGetIdentifier (out symbolName)) {
				return false;
			}
			break;
		case 2:
			if (!attributeData.ConstructorArguments [0].TryGetIdentifier (out symbolName)) {
				return false;
			}
			switch (attributeData.ConstructorArguments [1].Value) {
			// there are two possible cases here:
			// 1. The second argument is a string
			// 2. The second argument is an enum
			case T enumValue:
				flags = enumValue;
				break;
			case string lib:
				libraryName = lib;
				break;
			default:
				// unexpected value :/
				return false;
			}
			break;
		default:
			// 0 should not be an option..
			return false;
		}

		if (attributeData.NamedArguments.Length == 0) {
			data = new(symbolName, libraryName, flags);	
			return true;
		}

		// LibraryName can be a param value
		foreach (var (name, value) in attributeData.NamedArguments) {
			switch (name) {
			case "LibraryName":
				libraryName = (string?) value.Value!;
				break;
			case "Flags":
				flags = (T) value.Value!;
				break;
			default:
				data = null;
				return false;
			}
		}
		data = new(symbolName, libraryName, flags);	
		return true;
	}
}
