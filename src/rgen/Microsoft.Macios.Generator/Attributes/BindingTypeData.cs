using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

namespace Microsoft.Macios.Generator.Attributes;

record BindingTypeData {

	public string? Name { get; }

	public BindingTypeData () { }

	public BindingTypeData (string name)
	{
		Name = name;
	}

	public static bool TryParse (SyntaxNode attributeSyntax, AttributeData attributeData,
		[NotNullWhen (true)] out BindingTypeData? data)
	{
		data = default;
		var count = attributeData.ConstructorArguments.Length;
		if (count != 0) {
			// we do not have a constructor for the BindingTypeAttribute that takes any arguments
			return false;
		}

		// parse the named arguments
		if (attributeData.NamedArguments.Length == 0) {
			data = new();
		}

		foreach ((var name, TypedConstant value) in attributeData.NamedArguments) {
			if (name != "Name")
				continue;
			data = new ((string) value.Value!);
			break;
		}

		return data is not null;
	}
}
