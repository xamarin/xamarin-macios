using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

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
		switch (count) {
		case 1:
			data = new ((string) attributeData.ConstructorArguments [0].Value!);
			break;
		case 2:
			switch (attributeData.ConstructorArguments [1].Value) {
			// there are two possible cases here:
			// 1. The second argument is a string
			// 2. The second argument is an enum
			case T enumValue:
				data = new ((string) attributeData.ConstructorArguments [0].Value!, enumValue);
				break;
			case string libraryName:
				data = new ((string) attributeData.ConstructorArguments [0].Value!, libraryName);
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
