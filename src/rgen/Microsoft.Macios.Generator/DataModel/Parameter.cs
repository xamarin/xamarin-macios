using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Extensions;

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
	/// Returns if the parameter type is a smart enum.
	/// </summary>
	public bool IsSmartEnum { get; init; }

	/// <summary>
	/// Returns if the parameter is an array type.
	/// </summary>
	public bool IsArray { get; init; }

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

	internal Parameter (int position, string type, string name)
	{
		Position = position;
		Type = type;
		Name = name;
	}

	public static bool TryCreate (IParameterSymbol symbol, ParameterSyntax declaration, SemanticModel semanticModel,
		[NotNullWhen (true)] out Parameter? parameter)
	{
		var type = symbol.Type is IArrayTypeSymbol arrayTypeSymbol
			? arrayTypeSymbol.ElementType.ToDisplayString ()
			: symbol.Type.ToDisplayString ().Trim ('?', '[', ']');
		parameter = new (symbol.Ordinal, type, symbol.Name) {
			IsOptional = symbol.IsOptional,
			IsParams = symbol.IsParams,
			IsThis = symbol.IsThis,
			IsNullable = symbol.NullableAnnotation == NullableAnnotation.Annotated,
			IsSmartEnum = symbol.Type.IsSmartEnum (),
			IsArray = symbol.Type is IArrayTypeSymbol,
			DefaultValue = (symbol.HasExplicitDefaultValue) ? symbol.ExplicitDefaultValue?.ToString () : null,
			ReferenceKind = symbol.RefKind.ToReferenceKind (),
			Attributes = declaration.GetAttributeCodeChanges (semanticModel),
		};
		return true;
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
		if (IsSmartEnum != other.IsSmartEnum)
			return false;
		if (IsArray != other.IsArray)
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
		hashCode.Add (IsSmartEnum);
		hashCode.Add (IsArray);
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
		sb.Append ("Attributes: ");
		sb.AppendJoin (", ", Attributes);
		sb.Append ($" IsOptional: {IsOptional}, ");
		sb.Append ($"IsParams {IsParams}, ");
		sb.Append ($"IsThis: {IsThis}, ");
		sb.Append ($"IsNullable: {IsNullable}, ");
		sb.Append ($"IsSmartEnum: {IsSmartEnum}, ");
		sb.Append ($"IsArray: {IsArray}, ");
		sb.Append ($"DefaultValue: {DefaultValue}, ");
		sb.Append ($"ReferenceKind: {ReferenceKind} }}");
		return sb.ToString ();
	}
}
