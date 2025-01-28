// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator;

namespace Microsoft.Macios.Transformer.Attributes;

readonly record struct BaseTypeData {

	public string BaseType { get; } // is a type in the attribute, but we do not care for the transformation
	public string? Name { get; init; } = null;
	public string [] Events { get; init; } = []; // it is a collection of types, but we do not care for the transformation
	public string [] Delegates { get; init; } = [];
	public bool Singleton { get; init; }

	public string? KeepRefUntil { get; init; } = null;

	public bool IsStubClass { get; init; }

	public BaseTypeData (string baseType)
	{
		BaseType = baseType;
	}

	public static bool TryParse (AttributeData attributeData,
		[NotNullWhen (true)] out BaseTypeData? data)
	{
		data = null;
		var count = attributeData.ConstructorArguments.Length;
		string baseType;
		string? name = null;
		string [] events = [];
		string [] delegates = [];
		var singleton = false;
		string? keepRefUntil = null;
		var isStubClass = false;

		// custom marshal directive values

		switch (count) {
		case 1:
			baseType = ((INamedTypeSymbol) attributeData.ConstructorArguments [0].Value!).ToDisplayString ();
			break;
		default:
			// 0 should not be an option..
			return false;
		}

		if (attributeData.NamedArguments.Length == 0) {
			data = new (baseType);
			return true;
		}

		foreach (var (argumentName, value) in attributeData.NamedArguments) {
			switch (argumentName) {
			case "Name":
				name = (string?) value.Value!;
				break;
			case "Events":
				events = value.Values.Select (
					v => ((INamedTypeSymbol) v.Value!).ToDisplayString ())
					.ToArray ();
				break;
			case "Delegates":
				delegates = value.Values.Select (v => (string) v.Value!).ToArray ();
				break;
			case "Singleton":
				singleton = (bool) value.Value!;
				break;
			case "KeepRefUntil":
				keepRefUntil = (string?) value.Value!;
				break;
			case "IsStubClass":
				isStubClass = (bool) value.Value!;
				break;
			default:
				data = null;
				return false;
			}
		}

		data = new (baseType) {
			Name = name,
			Events = events,
			Delegates = delegates,
			Singleton = singleton,
			KeepRefUntil = keepRefUntil,
			IsStubClass = isStubClass,
		};
		return true;
	}

	public bool Equals (BaseTypeData other)
	{
		var stringCollectionComparer = new CollectionComparer<string?> (StringComparer.Ordinal);

		if (BaseType != other.BaseType)
			return false;
		if (Name != other.Name)
			return false;
		if (!stringCollectionComparer.Equals (Events, other.Events))
			return false;
		if (!stringCollectionComparer.Equals (Delegates, other.Delegates))
			return false;
		if (Singleton != other.Singleton)
			return false;
		if (KeepRefUntil != other.KeepRefUntil)
			return false;
		return IsStubClass == other.IsStubClass;
	}

	/// <inheritdoc />
	public override int GetHashCode ()
	{
		var hash = new HashCode ();
		hash.Add (BaseType);
		hash.Add (Name);
		foreach (var e in Events) {
			hash.Add (e);
		}
		foreach (var d in Delegates) {
			hash.Add (d);
		}
		hash.Add (Singleton);
		hash.Add (KeepRefUntil);
		return hash.ToHashCode ();
	}

	public override string ToString ()
	{
		var sb = new StringBuilder ($"BaseTypeData {{ BaseType: {BaseType}, Name: {Name ?? "null"}, ");
		sb.Append ("Events: [");
		sb.AppendJoin (", ", Events);
		sb.Append ("], Delegates: [");
		sb.AppendJoin (", ", Delegates);
		sb.Append ($"], Singleton: {Singleton}, KeepRefUntil: {KeepRefUntil ?? "null"}, IsStubClass: {IsStubClass} }}");
		return sb.ToString ();
	}
}
