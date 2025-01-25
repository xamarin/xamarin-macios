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

[StructLayout (LayoutKind.Auto)]
readonly partial struct Accessor : IEquatable<Accessor> {
	/// <summary>
	/// The kind of accessor.
	/// </summary>
	public AccessorKind Kind { get; }

	/// <summary>
	/// The platform availability of the enum value.
	/// </summary>
	public SymbolAvailability SymbolAvailability { get; }

	/// <summary>
	/// List of attribute code changes of the accessor.
	/// </summary>
	public ImmutableArray<AttributeCodeChange> Attributes { get; }

	/// <summary>
	/// List of modifiers of the accessor.
	/// </summary>
	public ImmutableArray<SyntaxToken> Modifiers { get; }

	/// <inheritdoc />
	public bool Equals (Accessor other)
	{
		if (Kind != other.Kind)
			return false;
		if (SymbolAvailability != other.SymbolAvailability)
			return false;
		if (ExportPropertyData != other.ExportPropertyData)
			return false;

		var attrsComparer = new AttributesEqualityComparer ();
		if (!attrsComparer.Equals (Attributes, other.Attributes))
			return false;
		var modifiersComparer = new ModifiersEqualityComparer ();
		return modifiersComparer.Equals (Modifiers, other.Modifiers);
	}

	/// <inheritdoc />
	public override bool Equals (object? obj)
	{
		return obj is Accessor other && Equals (other);
	}

	/// <inheritdoc />
	public override int GetHashCode ()
	{
		return HashCode.Combine ((int) Kind, SymbolAvailability, ExportPropertyData, Attributes, Modifiers);
	}

	public static bool operator == (Accessor left, Accessor right)
	{
		return left.Equals (right);
	}

	public static bool operator != (Accessor left, Accessor right)
	{
		return !left.Equals (right);
	}

	/// <inheritdoc />
	public override string ToString ()
	{
		var sb = new StringBuilder ($"{{ Kind: {Kind}, Supported Platforms: {SymbolAvailability}, ExportData: {ExportPropertyData?.ToString () ?? "null"} Modifiers: [");
		sb.AppendJoin (",", Modifiers.Select (x => x.Text));
		sb.Append ("], Attributes: [");
		sb.AppendJoin (", ", Attributes);
		sb.Append ("] }");
		return sb.ToString ();
	}
}
