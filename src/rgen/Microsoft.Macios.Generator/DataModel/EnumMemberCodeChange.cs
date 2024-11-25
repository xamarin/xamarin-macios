using System;
using System.Collections.Immutable;
using System.Linq;

namespace Microsoft.Macios.Generator.DataModel;

/// <summary>
/// Structure that represents a change that was made by the user on enum members that has to be
/// reflected in the generated code.
/// </summary>
readonly struct EnumMemberCodeChange : IEquatable<EnumMemberCodeChange> {

	/// <summary>
	/// Get the name of the member.
	/// </summary>
	public string Name { get; }

	/// <summary>
	/// Get the attributes added to the member.
	/// </summary>
	public ImmutableArray<AttributeCodeChange> Attributes { get; }

	/// <summary>
	/// Create a new change that happened on a member.
	/// </summary>
	/// <param name="name">The name of the changed member.</param>
	/// <param name="attributes">The list of attribute changes in the member.</param>
	public EnumMemberCodeChange (string name, ImmutableArray<AttributeCodeChange> attributes)
	{
		Name = name;
		Attributes = attributes;
	}

	/// <summary>
	/// Create a new change that happened on a member.
	/// </summary>
	/// <param name="name">The name of the changed member.</param>
	public EnumMemberCodeChange (string name) : this (name, []) { }

	/// <inheritdoc />
	public bool Equals (EnumMemberCodeChange other)
	{
		if (Name != other.Name)
			return false;
		var attrComparer = new AttributesComparer ();
		return attrComparer.Equals (Attributes, other.Attributes);
	}

	/// <inheritdoc />
	public override bool Equals (object? obj)
	{
		return obj is EnumMemberCodeChange other && Equals (other);
	}

	/// <inheritdoc />
	public override int GetHashCode ()
	{
		return HashCode.Combine (Name, Attributes);
	}

	public static bool operator == (EnumMemberCodeChange x, EnumMemberCodeChange y)
	{
		return x.Equals (y);
	}

	public static bool operator != (EnumMemberCodeChange x, EnumMemberCodeChange y)
	{
		return !(x == y);
	}
}
