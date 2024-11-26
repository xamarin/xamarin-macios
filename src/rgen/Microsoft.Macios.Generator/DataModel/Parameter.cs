using System;
using System.Collections.Immutable;
using System.Text;

namespace Microsoft.Macios.Generator.DataModel;

/// <summary>
/// Readonly structure that represents a change in a parameter.
/// </summary>
readonly struct Parameter : IEquatable<Parameter> {
	/// <summary>
	/// Parameter position in the method.
	/// </summary>
	public int Position { get; }

	/// <summary>
	/// Type of the parameter.
	/// </summary>
	public string Type { get; }

	/// <summary>
	/// Parameter name
	/// </summary>
	public string Name { get; }

	/// <summary>
	/// True if the parameter is optional
	/// </summary>
	public bool IsOptional { get; init; }

	/// <summary>
	/// True if a parameter is using the 'params' modifier.
	/// </summary>
	public bool IsParams { get; init; }

	/// <summary>
	/// True if the parameter represents the 'this' pointer.
	/// </summary>
	public bool IsThis { get; init; }

	/// <summary>
	/// True if the parameter is nullable.:w
	/// </summary>
	public bool IsNullable { get; init; }

	/// <summary>
	/// Optional default value.
	/// </summary>
	public string? DefaultValue { get; init; }

	/// <summary>
	/// The reference type used.
	/// </summary>
	public ReferenceKind ReferenceKind { get; init; }

	/// <summary>
	/// List of attributes attached to the parameter.
	/// </summary>
	public ImmutableArray<AttributeCodeChange> Attributes { get; init; } = [];

	public Parameter (int position, string type, string name)
	{
		Position = position;
		Type = type;
		Name = name;
	}

	/// <inheritdoc/>
	public bool Equals (Parameter other)
	{
		if (Position != other.Position)
			return false;
		if (Type != other.Type)
			return false;
		if (Name != other.Name)
			return false;
		if (IsOptional != other.IsOptional)
			return false;
		if (IsParams != other.IsParams)
			return false;
		if (IsThis != other.IsThis)
			return false;
		if (IsNullable != other.IsNullable)
			return false;
		if (DefaultValue != other.DefaultValue)
			return false;
		if (ReferenceKind != other.ReferenceKind)
			return false;

		var attributeComparer = new AttributesEqualityComparer ();
		return attributeComparer.Equals (Attributes, other.Attributes);
	}

	/// <inheritdoc/>
	public override bool Equals (object? obj)
	{
		return obj is Parameter other && Equals (other);
	}

	public override int GetHashCode ()
	{
		var hashCode = new HashCode ();
		hashCode.Add (Position);
		hashCode.Add (Type);
		hashCode.Add (Name);
		hashCode.Add (IsOptional);
		hashCode.Add (IsParams);
		hashCode.Add (IsThis);
		hashCode.Add (IsNullable);
		hashCode.Add (DefaultValue);
		hashCode.Add ((int) ReferenceKind);
		return hashCode.ToHashCode ();
	}

	public static bool operator == (Parameter left, Parameter right)
	{
		return left.Equals (right);
	}

	public static bool operator != (Parameter left, Parameter right)
	{
		return !left.Equals (right);
	}

	/// <inheritdoc/>
	public override string ToString ()
	{
		var sb = new StringBuilder ("{");
		sb.Append ($"Position: {Position}, ");
		sb.Append ($"Type: {Type}, ");
		sb.Append ($"Name: {Name}, ");
		sb.Append ($"IsOptional: {IsOptional}, ");
		sb.Append ($"IsParams {IsParams}, ");
		sb.Append ($"IsThis: {IsThis}, ");
		sb.Append ($"IsNullable: {IsNullable}, ");
		sb.Append ($"DefaultValue: {DefaultValue}, ");
		sb.Append ($"ReferenceKind: {ReferenceKind} }}");
		return sb.ToString ();
	}
}
