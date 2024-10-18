using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

namespace Microsoft.Macios.Generator.Attributes;

record FieldData {
	public string SymbolName { get; }
	public string? LibraryName { get; private set; }

	FieldData (string symbolName, string? libraryName = null)
	{
		SymbolName = symbolName;
		LibraryName = libraryName;
	}

	public static bool TryParse (SyntaxNode attributeSyntax, AttributeData attributeData,
		[NotNullWhen (true)] out FieldData? data)
	{
		data = default;

		var count = attributeData.ConstructorArguments.Length;
		switch (count) {
		case 1:
			data = new ((string) attributeData.ConstructorArguments [0].Value!);
			break;
		case 2:
			data = new ((string) attributeData.ConstructorArguments [0].Value!,
				(string) attributeData.ConstructorArguments [1].Value!);
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
				data.LibraryName = (string) value.Value!;
				break;
			default:
				data = null;
				return false;
			}
		}
		return true;
	}
}
