using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.Macios.Generator.Availability;

namespace Microsoft.Macios.Generator.DataModel;

/// <summary>
/// Structure that represents a change that was made by the user on enum members that has to be
/// reflected in the generated code.
/// </summary>
readonly struct EnumMember : IEquatable<EnumMember> {
	/// <summary>
	/// Get the name of the member.
	/// </summary>
	public string Name { get; }

	/// <summary>
	/// The platform availability of the eum value.
	/// </summary>
	public SymbolAvailability SymbolAvailability { get; }

	/// <summary>
	/// Get the attributes added to the member.
	/// </summary>
	public ImmutableArray<AttributeCodeChange> Attributes { get; }

	/// <summary>
	/// Create a new change that happened on a member.
	/// </summary>
	/// <param name="name">The name of the changed member.</param>
	/// <param name="symbolAvailability">The symbol availability of the member.</param>
	/// <param name="attributes">The list of attribute changes in the member.</param>
	public EnumMember (string name, SymbolAvailability symbolAvailability,
		ImmutableArray<AttributeCodeChange> attributes)
	{
		Name = name;
		SymbolAvailability = symbolAvailability;
		Attributes = attributes;
	}

	/// <summary>
	/// Create a new change that happened on a member.
	/// </summary>
	/// <param name="name">The name of the changed member.</param>
	public EnumMember (string name) : this (name, new SymbolAvailability (), ImmutableArray<AttributeCodeChange>.Empty)
	{
	}

	/// <inheritdoc />
	public bool Equals (EnumMember other)
	{
		if (Name != other.Name)
			return false;
		if (SymbolAvailability != other.SymbolAvailability)
			return false;
		var attrComparer = new AttributesEqualityComparer ();
		return attrComparer.Equals (Attributes, other.Attributes);
	}

	/// <inheritdoc />
	public override bool Equals (object? obj)
	{
		return obj is EnumMember other && Equals (other);
	}

	/// <inheritdoc />
	public override int GetHashCode ()
	{
		return HashCode.Combine (Name, SymbolAvailability, Attributes);
	}

	public static bool operator == (EnumMember x, EnumMember y)
	{
		return x.Equals (y);
	}

	public static bool operator != (EnumMember x, EnumMember y)
	{
		return !(x == y);
	}
}
