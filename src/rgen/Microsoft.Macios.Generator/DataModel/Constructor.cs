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

[StructLayout(LayoutKind.Auto)]
readonly partial struct Constructor : IEquatable<Constructor> {
	/// <summary>
	/// Type name that owns the constructor.
	/// </summary>
	public string Type { get; }

	/// <summary>
	/// The platform availability of the constructor.
	/// </summary>
	public SymbolAvailability SymbolAvailability { get; }

	/// <summary>
	/// Get the attributes added to the constructor.
	/// </summary>
	public ImmutableArray<AttributeCodeChange> Attributes { get; } = [];

	/// <summary>
	/// Modifiers list.
	/// </summary>
	public ImmutableArray<SyntaxToken> Modifiers { get; } = [];

	/// <summary>
	/// Parameters list.
	/// </summary>
	public ImmutableArray<Parameter> Parameters { get; } = [];

	public Constructor (string type,
		SymbolAvailability symbolAvailability,
		ImmutableArray<AttributeCodeChange> attributes,
		ImmutableArray<SyntaxToken> modifiers,
		ImmutableArray<Parameter> parameters)
	{
		Type = type;
		SymbolAvailability = symbolAvailability;
		Attributes = attributes;
		Modifiers = modifiers;
		Parameters = parameters;
	}

	/// <inheritdoc/>
	public bool Equals (Constructor other)
	{
		if (Type != other.Type)
			return false;
		if (SymbolAvailability != other.SymbolAvailability)
			return false;

		var attrsComparer = new AttributesEqualityComparer ();
		if (!attrsComparer.Equals (Attributes, other.Attributes))
			return false;
		var modifiersComparer = new ModifiersEqualityComparer ();
		if (!modifiersComparer.Equals (Modifiers, other.Modifiers))
			return false;

		var paramComparer = new MethodParameterEqualityComparer ();
		return paramComparer.Equals (Parameters, other.Parameters);
	}

	/// <inheritdoc/>
	public override bool Equals (object? obj)
	{
		return obj is Constructor other && Equals (other);
	}

	/// <inheritdoc/>
	public override int GetHashCode ()
	{
		var hashCode = new HashCode ();
		hashCode.Add (Type);
		hashCode.Add (SymbolAvailability);
		foreach (var modifier in Modifiers) {
			hashCode.Add (modifier);
		}

		foreach (var attr in Attributes) {
			hashCode.Add (attr);
		}

		foreach (var parameter in Parameters) {
			hashCode.Add (parameter);
		}

		return hashCode.ToHashCode ();
	}

	public static bool operator == (Constructor left, Constructor right)
	{
		return left.Equals (right);
	}

	public static bool operator != (Constructor left, Constructor right)
	{
		return !left.Equals (right);
	}

	/// <inheritdoc/>
	public override string ToString ()
	{
		var sb = new StringBuilder ($"{{ Ctr: Type: {Type}, ");
		sb.Append ("Attributes: [");
		sb.AppendJoin (", ", Attributes);
		sb.Append ("], Modifiers: [");
		sb.AppendJoin (", ", Modifiers.Select (x => x.Text));
		sb.Append ("], Parameters: [");
		sb.AppendJoin (", ", Parameters);
		sb.Append ("] }}");
		return sb.ToString ();
	}
}
