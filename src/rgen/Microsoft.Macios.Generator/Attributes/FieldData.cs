using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator.Extensions;

namespace Microsoft.Macios.Generator.Attributes;

record FieldData<T> where T : Enum {
	public string SymbolName { get; }
	public string? LibraryName { get; private set; }

	public T? Flags { get; private set; } = default;

	FieldData (string symbolName, string? libraryName = null)
	{
		SymbolName = symbolName;
		LibraryName = libraryName;
		Flags = default;
	}

	FieldData (string symbolName, T? flags)
	{
		SymbolName = symbolName;
		LibraryName = default;
		Flags = flags;
	}

	public static bool TryParse (SyntaxNode attributeSyntax, AttributeData attributeData,
		[NotNullWhen (true)] out FieldData<T>? data)
	{
		data = default;

		var count = attributeData.ConstructorArguments.Length;
		string? symbolName = null;
		switch (count) {
		case 1:
			if (attributeData.ConstructorArguments [0].TryGetIdentifier (out symbolName)) {
				data = new (symbolName);
			} else {
				// wrong content data from the user. The symbol provided cannot represent an identifier
				return false;
			}
			break;
		case 2:
			switch (attributeData.ConstructorArguments [1].Value) {
			// there are two possible cases here:
			// 1. The second argument is a string
			// 2. The second argument is an enum
			case T enumValue:
				if (attributeData.ConstructorArguments [0].TryGetIdentifier (out symbolName)) {
					data = new (symbolName, enumValue);
				} else {
					// wrong content data from the user. The symbol provided cannot represent an identifier
					return false;
				}
				break;
			case string libraryName: {
				if (attributeData.ConstructorArguments [0].TryGetIdentifier (out symbolName)) {
					data = new (symbolName, libraryName);
				} else {
					// wrong content data from the user. The symbol provided cannot represent an identifier
					return false;
				}

				break;
			}
			default:
				// unexpected value :/
				return false;
			}
			break;
		default:
			// 0 should not be an option..
			return false;
		}

		if (attributeData.NamedArguments.Length == 0)
			return true;

		// LibraryName can be a param value
		foreach (var (name, value) in attributeData.NamedArguments) {
			switch (name) {
			case "LibraryName":
				data.LibraryName = (string?) value.Value!;
				break;
			case "Flags":
				data.Flags = (T) value.Value!;
				break;
			default:
				data = null;
				return false;
			}
		}
		return true;
	}
}
