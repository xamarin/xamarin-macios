using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator.Extensions;

namespace Microsoft.Macios.Generator.Attributes;

readonly struct FieldData<T> : IEquatable<FieldData<T>> where T : Enum {

	public enum ParsingError {
		None = 0,
		NotIdentifier,
		UnknownConstructor,
		UnknownNamedArgument,
	}
	public string SymbolName { get; }
	public string? LibraryName { get; }

	public T? Flags { get; } = default;

	internal FieldData (string symbolName, string? libraryName, T? flags)
	{
		SymbolName = symbolName;
		LibraryName = libraryName;
		Flags = flags;
	}

	public static bool TryParse (AttributeData attributeData,
		[NotNullWhen (true)] out FieldData<T>? data)
		=> TryParse (attributeData, out data, out _);

	public static bool TryParse (AttributeData attributeData,
		[NotNullWhen (true)] out FieldData<T>? data, out AttributeParsingError<ParsingError> error)
	{
		data = default;
		error = new (ParsingError.None, null);

		var count = attributeData.ConstructorArguments.Length;
		string? symbolName;
		string? libraryName = null;
		T? flags = default;
		switch (count) {
		case 1:
			if (!attributeData.ConstructorArguments [0].TryGetIdentifier (out symbolName)) {
				error = new (ParsingError.NotIdentifier, attributeData.ConstructorArguments [0].Value);
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
				error = new (ParsingError.UnknownConstructor, attributeData.ConstructorArguments.Length);
				return false;
			}
			break;
		default:
			// 0 should not be an option.
			return false;
		}

		if (attributeData.NamedArguments.Length == 0) {
			data = new (symbolName, libraryName, flags);
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
				error = new (ParsingError.UnknownNamedArgument, name);
				return false;
			}
		}
		data = new (symbolName, libraryName, flags);
		return true;
	}

	/// <inheritdoc />
	public bool Equals (FieldData<T> other)
	{
		if (SymbolName != other.SymbolName)
			return false;
		if (LibraryName != other.LibraryName)
			return false;
		if (Flags is not null && other.Flags is not null) {
			return Flags.Equals (other.Flags);
		}
		return false;
	}

	/// <inheritdoc />
	public override bool Equals (object? obj)
	{
		return obj is FieldData<T> other && Equals (other);
	}

	/// <inheritdoc />
	public override int GetHashCode ()
	{
		return HashCode.Combine (SymbolName, LibraryName, Flags);
	}

	public static bool operator == (FieldData<T> x, FieldData<T> y)
	{
		return x.Equals (y);
	}

	public static bool operator != (FieldData<T> x, FieldData<T> y)
	{
		return !(x == y);
	}

	/// <inheritdoc />
	public override string ToString ()
	{
		return $"{{ SymbolName: '{SymbolName}' LibraryName: '{LibraryName ?? "null"}', Flags: '{Flags}' }}";
	}
}
