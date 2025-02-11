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
readonly partial struct Method : IEquatable<Method> {

	/// <summary>
	/// Type name that owns the method.
	/// </summary>
	public string Type { get; }

	/// <summary>
	/// Method name.
	/// </summary>
	public string Name { get; }

	/// <summary>
	/// Method return type.
	/// </summary>
	public TypeInfo ReturnType { get; }

	/// <summary>
	/// The platform availability of the method.
	/// </summary>
	public SymbolAvailability SymbolAvailability { get; }

	/// <summary>
	/// Get the attributes added to the constructor.
	/// </summary>
	public ImmutableArray<AttributeCodeChange> Attributes { get; } = [];

	/// <summary>
	/// Modifiers list.
	/// </summary>
	public ImmutableArray<SyntaxToken> Modifiers { get; init; } = [];

	/// <summary>
	/// Parameters list.
	/// </summary>
	public ImmutableArray<Parameter> Parameters { get; } = [];

	/// <inheritdoc/>
	public bool Equals (Method other)
	{
		if (Type != other.Type)
			return false;
		if (Name != other.Name)
			return false;
		if (ReturnType != other.ReturnType)
			return false;
		if (SymbolAvailability != other.SymbolAvailability)
			return false;
		if (ExportMethodData != other.ExportMethodData)
			return false;
		if (BindAs != other.BindAs)
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
		return obj is Method other && Equals (other);
	}

	/// <inheritdoc/>
	public override int GetHashCode ()
	{
		var hashCode = new HashCode ();
		hashCode.Add (Type);
		hashCode.Add (Name);
		hashCode.Add (ReturnType);
		hashCode.Add (BindAs);
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

	public static bool operator == (Method left, Method right)
	{
		return left.Equals (right);
	}

	public static bool operator != (Method left, Method right)
	{
		return !left.Equals (right);
	}

	/// <inheritdoc/>
	public override string ToString ()
	{
		var sb = new StringBuilder ($"{{ Method: Type: {Type}, ");
		sb.Append ($"Name: {Name}, ");
		sb.Append ($"ReturnType: {ReturnType}, ");
		sb.Append ($"SymbolAvailability: {SymbolAvailability}, ");
		sb.Append ($"ExportMethodData: {ExportMethodData}, ");
		sb.Append ($"BindAs: {BindAs}, ");
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
