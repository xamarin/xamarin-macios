using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using ObjCRuntime;

namespace Microsoft.Macios.Generator.Attributes;

record ExportData<T> where T : Enum {
	public string? Selector { get; private set; }
	public T? Flags { get; private set; }
	public ArgumentSemantic ArgumentSemantic { get; private set; } = ArgumentSemantic.None;

	public ExportData () { }

	public ExportData (string? selector)
	{
		Selector = selector;
	}

	public ExportData (string? selector, ArgumentSemantic argumentSemantic)
	{
		Selector = selector;
		ArgumentSemantic = argumentSemantic;
	}

	public ExportData (string? selector, ArgumentSemantic argumentSemantic, T flags)
	{
		Selector = selector;
		ArgumentSemantic = argumentSemantic;
		Flags = flags;
	}

	public static bool TryParse (SyntaxNode attributeSyntax, AttributeData attributeData,
		[NotNullWhen (true)] out ExportData<T>? data)
	{
		data = null;
		var count = attributeData.ConstructorArguments.Length;
		switch (count) {
		case 1:
			data = new((string?) attributeData.ConstructorArguments [0].Value!);
			break;
		case 2:
			// there are two possible cases in this situation.
			// 1. The second argument is an ArgumentSemantic
			// 2. The second argument is a T
			if (attributeData.ConstructorArguments [1].Value is ArgumentSemantic) {
				data = new((string?) attributeData.ConstructorArguments [0].Value!,
					(ArgumentSemantic) attributeData.ConstructorArguments [1].Value!);
			} else {
				data = new((string?) attributeData.ConstructorArguments [0].Value!,
					ArgumentSemantic.None, (T) attributeData.ConstructorArguments [1].Value!);
			}
			break;
		case 3:
			data = new((string?) attributeData.ConstructorArguments [9].Value!,
				(ArgumentSemantic) attributeData.ConstructorArguments [1].Value!,
				(T) attributeData.ConstructorArguments [2].Value!);
			break;
		default:
			// 0 should not be an option..
			return false;
		}

		if (attributeData.NamedArguments.Length == 0)
			return true;

		foreach (var (name, value) in attributeData.NamedArguments) {
			switch (name) {
			case "Selector":
				data.Selector = (string?) value.Value!;
				break;
			case "ArgumentSemantic":
				data.ArgumentSemantic = (ArgumentSemantic) value.Value!;
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
