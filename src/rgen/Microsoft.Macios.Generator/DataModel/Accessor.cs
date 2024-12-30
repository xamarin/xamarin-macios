using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator.Availability;

namespace Microsoft.Macios.Generator.DataModel;

readonly struct Accessor : IEquatable<Accessor> {
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

	/// <summary>
	/// Create a new code change in a property accessor.
	/// </summary>
	/// <param name="accessorKind">The kind of accessor.</param>
	/// <param name="symbolAvailability">The os availability of the symbol.</param>
	/// <param name="attributes">The list of attributes attached to the accessor.</param>
	/// <param name="modifiers">The list of visibility modifiers of the accessor.</param>
	public Accessor (AccessorKind accessorKind, SymbolAvailability symbolAvailability, ImmutableArray<AttributeCodeChange> attributes,
		ImmutableArray<SyntaxToken> modifiers)
	{
		Kind = accessorKind;
		SymbolAvailability = symbolAvailability;
		Attributes = attributes;
		Modifiers = modifiers;
	}

	/// <inheritdoc />
	public bool Equals (Accessor other)
	{
		if (Kind != other.Kind)
			return false;
		if (SymbolAvailability != other.SymbolAvailability)
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
		return HashCode.Combine ((int) Kind, Attributes, Modifiers);
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
		var sb = new StringBuilder ($"{{ Kind: {Kind}, Supported Platforms: {SymbolAvailability} Modifiers: [");
		sb.AppendJoin (",", Modifiers.Select (x => x.Text));
		sb.Append ("], Attributes: [");
		sb.AppendJoin (", ", Attributes);
		sb.Append ("] }");
		return sb.ToString ();
	}
}
