// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator.Extensions;

namespace Microsoft.Macios.Generator.DataModel;

/// <summary>
/// Readonly structure that describes a parameter in a delegate. This class contains less information
/// than Parameter since some of the extra fields make no sense in delegates.
/// </summary>
readonly struct DelegateParameter : IEquatable<DelegateParameter> {

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
	/// True if the parameter type is blittable.
	/// </summary>
	public bool IsBlittable { get; }

	/// <summary>
	/// Returns if the parameter type is a smart enum.
	/// </summary>
	public bool IsSmartEnum { get; init; }

	/// <summary>
	/// Returns if the parameter is an array type.
	/// </summary>
	public bool IsArray { get; init; }

	/// <summary>
	/// The reference type used.
	/// </summary>
	public ReferenceKind ReferenceKind { get; init; }

	public DelegateParameter (int position, string type, string name, bool isBlittable)
	{
		Position = position;
		Type = type;
		Name = name;
		IsBlittable = isBlittable;
	}

	public static bool TryCreate (IParameterSymbol symbol,
		[NotNullWhen (true)] out DelegateParameter? parameter)
	{
		var type = symbol.Type is IArrayTypeSymbol arrayTypeSymbol
			? arrayTypeSymbol.ElementType.ToDisplayString ()
			: symbol.Type.ToDisplayString ().Trim ('?', '[', ']');

		parameter = new (symbol.Ordinal, type, symbol.Name, symbol.Type.IsBlittable ()) {
			IsOptional = symbol.IsOptional,
			IsParams = symbol.IsParams,
			IsThis = symbol.IsThis,
			IsNullable = symbol.NullableAnnotation == NullableAnnotation.Annotated,
			IsSmartEnum = symbol.Type.IsSmartEnum (),
			IsArray = symbol.Type is IArrayTypeSymbol,
			ReferenceKind = symbol.RefKind.ToReferenceKind (),
		};
		return true;
	}

	/// <inheritdoc/>
	public bool Equals (DelegateParameter other)
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
		if (IsBlittable != other.IsBlittable)
			return false;
		if (IsSmartEnum != other.IsSmartEnum)
			return false;
		if (IsArray != other.IsArray)
			return false;
		return ReferenceKind == other.ReferenceKind;
	}

	/// <inheritdoc/>
	public override bool Equals (object? obj)
	{
		return obj is DelegateParameter other && Equals (other);
	}

	/// <inheritdoc/>
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
		hashCode.Add ((int) ReferenceKind);
		return hashCode.ToHashCode ();
	}

	public static bool operator == (DelegateParameter left, DelegateParameter right)
	{
		return left.Equals (right);
	}

	public static bool operator != (DelegateParameter left, DelegateParameter right)
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
		sb.Append ($"IsParams: {IsParams}, ");
		sb.Append ($"IsThis: {IsThis}, ");
		sb.Append ($"IsNullable: {IsNullable}, ");
		sb.Append ($"IsBlittable: {IsBlittable}, ");
		sb.Append ($"IsSmartEnum: {IsSmartEnum}, ");
		sb.Append ($"IsArray: {IsArray}, ");
		sb.Append ($"ReferenceKind: {ReferenceKind} ");
		return sb.ToString ();
	}
}
