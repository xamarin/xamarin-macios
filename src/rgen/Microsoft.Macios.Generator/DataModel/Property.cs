// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator.Availability;

namespace Microsoft.Macios.Generator.DataModel;

/// <summary>
/// Readonly struct that represent the changes that a user has made in a property.
/// </summary>
[StructLayout (LayoutKind.Auto)]
readonly partial struct Property : IEquatable<Property> {
	/// <summary>
	/// Name of the property.
	/// </summary>
	public string Name { get; } = string.Empty;

	public string BackingField { get; private init; }

	readonly TypeInfo returnType;

	/// <summary>
	/// Representation of the property type.
	/// </summary>
	public TypeInfo ReturnType {
		get => returnType;
		private init {
			returnType = value;
			ValueParameter = new Parameter (0, returnType, "value");
		}
	}

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
	public ImmutableArray<SyntaxToken> Modifiers { get; init; } = [];

	/// <summary>
	/// Get the list of accessor changes of the property.
	/// </summary>
	public ImmutableArray<Accessor> Accessors { get; } = [];

	public Parameter ValueParameter { get; private init; }

	public Accessor? GetAccessor (AccessorKind accessorKind)
	{
		// careful, do not use FirstOrDefault from LINQ because we are using structs!
		foreach (var accessor in Accessors) {
			if (accessor.Kind == accessorKind)
				return accessor;
		}
		return null;
	}

	bool CoreEquals (Property other)
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
		if (BindAs != other.BindAs)
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

}
