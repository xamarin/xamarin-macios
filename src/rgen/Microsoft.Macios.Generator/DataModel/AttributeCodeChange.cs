using System;
using System.Collections.Immutable;

namespace Microsoft.Macios.Generator.DataModel;

/// <summary>
/// Structure that represents a change that was made by the user on a members attribute list that has to be
/// reflected in the generated code.
/// </summary>
readonly struct AttributeCodeChange : IEquatable<AttributeCodeChange> {

	/// <summary>
	/// Get the name of the attribute that was added.
	/// </summary>
	public string Name { get; }

	/// <summary>
	/// Get the arguments used to create the attribute.
	/// </summary>
	public ImmutableArray<string> Arguments { get; }

	/// <summary>
	/// Create a new change that happened on an attribute.
	/// </summary>
	/// <param name="name">The name of the attribute that was added.</param>
	/// <param name="arguments">The arguments used to create the attribute.</param>
	public AttributeCodeChange (string name, ImmutableArray<string> arguments)
	{
		Name = name;
		Arguments = arguments;
	}

	/// <summary>
	/// Create a new change that happened on an attribute with no parameters.
	/// </summary>
	/// <param name="name">The name of the attribute that was added.</param>
	public AttributeCodeChange (string name) : this (name, []) { }

	/// <inheritdoc />
	public bool Equals (AttributeCodeChange other)
	{
		if (Name != other.Name || Arguments.Length != other.Arguments.Length)
			return false;
		// arguments CANNOT be sorted, since the order of the arguments is important
		for (var index = 0; index < Arguments.Length; index++) {
			if (Arguments [index] != other.Arguments [index])
				return false;
		}

		return true;
	}

	/// <inheritdoc />
	public override bool Equals (object? obj)
	{
		return obj is AttributeCodeChange other && Equals (other);
	}

	/// <inheritdoc />
	public override int GetHashCode ()
	{
		return HashCode.Combine (Name, Arguments);
	}

	/// <summary>
	/// Compare two <see cref="AttributeCodeChange"/> instances for equality.
	/// </summary>
	/// <param name="x">Code change to compare.</param>
	/// <param name="y">Code change to compare.</param>
	/// <returns>True if the code changes are equal.</returns>
	public static bool operator == (AttributeCodeChange x, AttributeCodeChange y)
	{
		return x.Equals ((object) y);
	}

	/// <summary>
	/// Compare two <see cref="AttributeCodeChange"/> instances for inequality.
	/// </summary>
	/// <param name="x">Code change to compare.</param>
	/// <param name="y">Code change to compare.</param>
	/// <returns>True if the objects are not equal.</returns>
	public static bool operator != (AttributeCodeChange x, AttributeCodeChange y)
	{
		return !(x == y);
	}
}
