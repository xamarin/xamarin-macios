// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using ObjCRuntime;

namespace Microsoft.Macios.Transformer.Attributes;

public struct ExportData : IEquatable<ExportData> {

	/// <summary>
	/// The exported native selector.
	/// </summary>
	public string? Selector { get; }

	/// <summary>
	/// Argument semantics to use with the selector.
	/// </summary>
	public ArgumentSemantic ArgumentSemantic { get; } = ArgumentSemantic.None;

	public ExportData (string? selector)
	{
		Selector = selector;
	}

	public ExportData (string? selector, ArgumentSemantic argumentSemantic)
	{
		Selector = selector;
		ArgumentSemantic = argumentSemantic;
	}

	/// <summary>
	/// Try to parse the attribute data to retrieve the information of an ExportAttribute&lt;T&gt;.
	/// </summary>
	/// <param name="attributeData">The attribute data to be parsed.</param>
	/// <param name="data">The parsed data. Null if we could not parse the attribute data.</param>
	/// <returns>True if the data was parsed.</returns>
	public static bool TryParse (AttributeData attributeData,
		[NotNullWhen (true)] out ExportData? data)
	{
		data = null;
		var count = attributeData.ConstructorArguments.Length;
		string? selector;
		ArgumentSemantic argumentSemantic = ArgumentSemantic.None;

		// custom marshal directive values

		switch (count) {
		case 1:
			selector = (string?) attributeData.ConstructorArguments [0].Value!;
			break;
		case 2:
			// there are two possible cases in this situation.
			// 1. The second argument is an ArgumentSemantic
			// 2. The second argument is a T
			selector = (string?) attributeData.ConstructorArguments [0].Value!;
			argumentSemantic = (ArgumentSemantic) attributeData.ConstructorArguments [1].Value!;
			break;
		default:
			// 0 should not be an option..
			return false;
		}

		if (attributeData.NamedArguments.Length == 0) {
			data = new (selector, argumentSemantic);
			return true;
		}

		foreach (var (name, value) in attributeData.NamedArguments) {
			switch (name) {
			case "Selector":
				selector = (string?) value.Value!;
				break;
			case "ArgumentSemantic":
				argumentSemantic = (ArgumentSemantic) value.Value!;
				break;
			default:
				data = null;
				return false;
			}
		}

		data = new (selector, argumentSemantic);
		return true;
	}

	public bool Equals (ExportData other)
	{
		if (Selector != other.Selector)
			return false;
		return ArgumentSemantic == other.ArgumentSemantic;
	}

	/// <inheritdoc />
	public override bool Equals (object? obj)
	{
		return obj is ExportData other && Equals (other);
	}

	/// <inheritdoc />
	public override int GetHashCode ()
	{
		return HashCode.Combine (Selector, ArgumentSemantic);
	}

	public static bool operator == (ExportData x, ExportData y)
	{
		return x.Equals (y);
	}

	public static bool operator != (ExportData x, ExportData y)
	{
		return !(x == y);
	}

	/// <inheritdoc />
	public override string ToString ()
	{
		return $"{{ Selector: '{Selector ?? "null"}', ArgumentSemantic: {ArgumentSemantic} }}";
	}
}
