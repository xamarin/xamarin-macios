// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
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
	
	/// <summary>
	/// Get the native prefix to be used in the custom marshal directive.
	///
	/// Should only be present with the CustomeMarshalDirective flag.
	/// </summary >
	public string? NativePrefix { get; init; }
	
	/// <summary>
	/// Get the native sufix to be used in the custom marshal directive.
	///
	/// Should only be present with the CustomeMarshalDirective flag.
	/// </summary >
	public string? NativeSuffix { get; init; }
	
	/// <summary>
	/// Get the library to be used in the custom marshal directive.
	///
	/// Should only be present with the CustomeMarshalDirective flag.
	/// </summary >
	public string? Library { get; init; }

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
		
		// custom marshal directive values
		string? nativePrefix = null;
		string? nativeSuffix = null;
		string? library = null;
		
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
			case "NativePrefix":
				nativePrefix = (string?) value.Value!;
				break;
			case "NativeSuffix":
				nativeSuffix = (string?) value.Value!;
				break;
			case "Library":
				library = (string?) value.Value!;
				break;
			default:
				data = null;
				return false;
			}
		}

		if (flags is not null) {
			data = new (selector, argumentSemantic, flags) {
				NativePrefix = nativePrefix,
				NativeSuffix = nativeSuffix,
				Library = library
			};
			return true;
		}

		data = new(selector, argumentSemantic) {
			NativePrefix = nativePrefix, 
			NativeSuffix = nativeSuffix, 
			Library = library
		};
		return true;
	}

	/// <inheritdoc />
	public bool Equals (ExportData<T> other)
	{
		if (Selector != other.Selector)
			return false;
		if (ArgumentSemantic != other.ArgumentSemantic)
			return false;
		if (NativePrefix != other.NativePrefix)
			return false;
		if (NativeSuffix != other.NativeSuffix)
			return false;
		if (Library != other.Library)
			return false;
		return (Flags, other.Flags) switch {
			(null, null) => true,
			(null, _) => false,
			(_, null) => false,
			(_, _) => Flags!.Equals (other.Flags)
		};
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
		var sb = new StringBuilder ("{ Type: '");
		sb.Append (typeof (T).FullName);
		sb.Append ("', Selector: '");
		sb.Append (Selector ?? "null");
		sb.Append ("', ArgumentSemantic: '");
		sb.Append (ArgumentSemantic);
		sb.Append ("', Flags: '");
		sb.Append (Flags);
		sb.Append ("', NativePrefix: '");
		sb.Append (NativePrefix ?? "null");
		sb.Append ("', NativeSuffix: '");
		sb.Append (NativeSuffix ?? "null");
		sb.Append ("', Library: '");
		sb.Append (Library ?? "null");
		sb.Append ("' }");
		return sb.ToString ();
	}
}
