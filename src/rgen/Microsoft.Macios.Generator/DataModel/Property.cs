// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator.Availability;

namespace Microsoft.Macios.Generator.DataModel;

/// <summary>
/// Readonly struct that represent the changes that a user has made in a property.
/// </summary>
[StructLayout(LayoutKind.Auto)]
readonly partial struct Property : IEquatable<Property> {
	/// <summary>
	/// Name of the property.
	/// </summary>
	public string Name { get; } = string.Empty;

	public string BackingField { get; private init; }

	/// <summary>
	/// Representation of the property type.
	/// </summary>
	public TypeInfo ReturnType { get; } = default;

	/// <summary>
	/// Returns if the property type is bittable.
	/// </summary>
	public bool IsBlittable => ReturnType.IsBlittable;

	/// <summary>
	/// Returns if the property type is a smart enum.
	/// </summary>
	public bool IsSmartEnum => ReturnType.IsSmartEnum;

	/// <summary>
	/// Returns if the property type is a reference type.
	/// </summary>
	public bool IsReferenceType => ReturnType.IsReferenceType;

	/// <summary>
	/// The platform availability of the property.
	/// </summary>
	public SymbolAvailability SymbolAvailability { get; }
	
	/// <summary>
	/// Get the attributes added to the member.
	/// </summary>
	public ImmutableArray<AttributeCodeChange> Attributes { get; } = [];
	
	/// <summary>
	/// Get the modifiers of the property.
	/// </summary>
	public ImmutableArray<SyntaxToken> Modifiers { get; } = [];

	/// <summary>
	/// Get the list of accessor changes of the property.
	/// </summary>
	public ImmutableArray<Accessor> Accessors { get; } = [];

	public Accessor? GetAccessor (AccessorKind accessorKind)
	{
		// careful, do not use FirstOrDefault from LINQ because we are using structs!
		foreach (var accessor in Accessors) {
			if (accessor.Kind == accessorKind)
				return accessor;
		}
		return null;
	}

	internal Property (string name, TypeInfo returnType,
		SymbolAvailability symbolAvailability,
		ImmutableArray<AttributeCodeChange> attributes,
		ImmutableArray<SyntaxToken> modifiers, ImmutableArray<Accessor> accessors)
	{
		Name = name;
		BackingField = $"_{Name}";
		ReturnType = returnType;
		SymbolAvailability = symbolAvailability;
		Attributes = attributes;
		Modifiers = modifiers;
		Accessors = accessors;
	}

	/// <inheritdoc />
	public bool Equals (Property other)
	{
		// this could be a large && but ifs are more readable
		if (Name != other.Name)
			return false;
		if (ReturnType != other.ReturnType)
			return false;
		if (IsBlittable != other.IsBlittable)
			return false;
		if (IsSmartEnum != other.IsSmartEnum)
			return false;
		if (IsReferenceType != other.IsReferenceType)
			return false;
		if (SymbolAvailability != other.SymbolAvailability)
			return false;
		if (ExportFieldData != other.ExportFieldData)
			return false;
		if (ExportPropertyData != other.ExportPropertyData)
			return false;

		var attrsComparer = new AttributesEqualityComparer ();
		if (!attrsComparer.Equals (Attributes, other.Attributes))
			return false;

		var modifiersComparer = new ModifiersEqualityComparer ();
		if (!modifiersComparer.Equals (Modifiers, other.Modifiers))
			return false;

		var accessorComparer = new AccessorsEqualityComparer ();
		return accessorComparer.Equals (Accessors, other.Accessors);
	}

	/// <inheritdoc />
	public override bool Equals (object? obj)
	{
		return obj is Property other && Equals (other);
	}

	/// <inheritdoc />
	public override int GetHashCode ()
	{
		return HashCode.Combine (Name, ReturnType, IsSmartEnum, Attributes, Modifiers, Accessors);
	}

	public static bool operator == (Property left, Property right)
	{
		return left.Equals (right);
	}

	public static bool operator != (Property left, Property right)
	{
		return !left.Equals (right);
	}
	
	/// <inheritdoc />
	public override string ToString ()
	{
		var sb = new StringBuilder (
			$"Name: '{Name}', Type: {ReturnType}, Supported Platforms: {SymbolAvailability}, ExportFieldData: '{ExportFieldData?.ToString () ?? "null"}', ExportPropertyData: '{ExportPropertyData?.ToString () ?? "null"}' Attributes: [");
		sb.AppendJoin (",", Attributes);
		sb.Append ("], Modifiers: [");
		sb.AppendJoin (",", Modifiers.Select (x => x.Text));
		sb.Append ("], Accessors: [");
		sb.AppendJoin (",", Accessors);
		sb.Append (']');
		return sb.ToString ();
	}
}
