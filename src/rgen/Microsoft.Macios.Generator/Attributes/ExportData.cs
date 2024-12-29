using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using ObjCRuntime;

namespace Microsoft.Macios.Generator.Attributes;

/// <summary>
/// Represents the data found in an ExportAttribute&lt;T&gt; 
/// </summary>
/// <typeparam name="T">The configuration flags used on the exported element.</typeparam>
readonly struct ExportData<T> : IEquatable<ExportData<T>> where T : Enum {

	/// <summary>
	/// The exported native selector.
	/// </summary>
	public string? Selector { get; }

	/// <summary>
	/// The configuration flags used on the exported member.
	/// </summary>
	public T? Flags { get; }

	/// <summary>
	/// Argument semantics to use with the selector.
	/// </summary>
	public ArgumentSemantic ArgumentSemantic { get; } = ArgumentSemantic.None;

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

	/// <summary>
	/// Try to parse the attribute data to retrieve the information of an ExportAttribute&lt;T&gt;.
	/// </summary>
	/// <param name="attributeData">The attribute data to be parsed.</param>
	/// <param name="data">The parsed data. Null if we could not parse the attribute data.</param>
	/// <returns>True if the data was parsed.</returns>
	public static bool TryParse (AttributeData attributeData,
		[NotNullWhen (true)] out ExportData<T>? data)
	{
		data = null;
		var count = attributeData.ConstructorArguments.Length;
		string? selector = null;
		ArgumentSemantic argumentSemantic = ArgumentSemantic.None;
		T? flags = default;
		switch (count) {
		case 1:
			selector = (string?) attributeData.ConstructorArguments [0].Value!;
			break;
		case 2:
			// there are two possible cases in this situation.
			// 1. The second argument is an ArgumentSemantic
			// 2. The second argument is a T
			if (attributeData.ConstructorArguments [1].Value is ArgumentSemantic) {
				selector = (string?) attributeData.ConstructorArguments [0].Value!;
				argumentSemantic = (ArgumentSemantic) attributeData.ConstructorArguments [1].Value!;
			} else {
				selector = (string?) attributeData.ConstructorArguments [0].Value!;
				argumentSemantic = ArgumentSemantic.None;
				flags = (T) attributeData.ConstructorArguments [1].Value!;
			}
			break;
		case 3:
			selector = (string?) attributeData.ConstructorArguments [0].Value!;
			argumentSemantic = (ArgumentSemantic) attributeData.ConstructorArguments [1].Value!;
			flags = (T) attributeData.ConstructorArguments [2].Value!;
			break;
		default:
			// 0 should not be an option..
			return false;
		}

		if (attributeData.NamedArguments.Length == 0) {
			data = flags is not null ?
				new (selector, argumentSemantic, flags) : new (selector, argumentSemantic);
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
			case "Flags":
				flags = (T) value.Value!;
				break;
			default:
				data = null;
				return false;
			}
		}

		data = flags is not null ?
			new (selector, argumentSemantic, flags) : new (selector, argumentSemantic);
		return true;
	}
	
	/// <inheritdoc />
	public bool Equals (ExportData<T> other)
	{
		if (Selector != other.Selector)
			return false;
		if (ArgumentSemantic != other.ArgumentSemantic)
			return false;
		if (Flags is not null && other.Flags is not null) {
			return Flags.Equals (other.Flags);
		}
		return false;
	}

	/// <inheritdoc />
	public override bool Equals (object? obj)
	{
		return obj is ExportData<T> other && Equals (other);
	}

	/// <inheritdoc />
	public override int GetHashCode ()
	{
		return HashCode.Combine (Selector, Flags);
	}

	public static bool operator == (ExportData<T> x, ExportData<T> y)
	{
		return x.Equals (y);
	}

	public static bool operator != (ExportData<T> x, ExportData<T> y)
	{
		return !(x == y);
	}

	/// <inheritdoc />
	public override string ToString ()
	{
		return $"{{ Type: '{typeof(T).FullName}', Selector: '{Selector ?? "null"}', ArgumentSemantic: '{ArgumentSemantic}', Flags: '{Flags}' }}";
	}
}
