// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator.Extensions;

namespace Microsoft.Macios.Generator.DataModel;

/// <summary>
/// Readonly structure that represents a change in a method return type.
/// </summary>
readonly struct ReturnType : IEquatable<ReturnType> {

	/// <summary>
	/// Type of the parameter.
	/// </summary>
	public string Type { get; }

	/// <summary>
	/// True if the parameter is nullable.
	/// </summary>
	public bool IsNullable { get; }

	/// <summary>
	/// True if the parameter type is blittable.
	/// </summary>
	public bool IsBlittable { get; }

	/// <summary>
	/// Returns if the return type is a smart enum.
	/// </summary>
	public bool IsSmartEnum { get; }

	/// <summary>
	/// Returns if the return type is an array type.
	/// </summary>
	public bool IsArray { get; }

	/// <summary>
	/// Returns if the return type is a reference type.
	/// </summary>
	public bool IsReferenceType { get; }

	/// <summary>
	/// Returns if the return type is void.
	/// </summary>
	public bool IsVoid { get; }

	internal ReturnType (string type)
	{
		Type = type;
		IsVoid = type == "void";
	}

	internal ReturnType (string type,
		bool isNullable = false,
		bool isBlittable = false,
		bool isSmartEnum = false,
		bool isArray = false,
		bool isReferenceType = false) : this (type)
	{
		IsNullable = isNullable;
		IsBlittable = isBlittable;
		IsSmartEnum = isSmartEnum;
		IsArray = isArray;
		IsReferenceType = isReferenceType;
	}

	internal ReturnType (ITypeSymbol symbol) :
		this (
			symbol is IArrayTypeSymbol arrayTypeSymbol
				? arrayTypeSymbol.ElementType.ToDisplayString ()
				: symbol.ToDisplayString ().Trim ('?', '[', ']'))
	{
		IsNullable = symbol.NullableAnnotation == NullableAnnotation.Annotated;
		IsBlittable = symbol.IsBlittable ();
		IsSmartEnum = symbol.IsSmartEnum ();
		IsArray = symbol is IArrayTypeSymbol;
		IsReferenceType = symbol.IsReferenceType;
	}

	/// <inheritdoc/>
	public bool Equals (ReturnType other)
	{
		if (Type != other.Type)
			return false;
		if (IsNullable != other.IsNullable)
			return false;
		if (IsBlittable != other.IsBlittable)
			return false;
		if (IsSmartEnum != other.IsSmartEnum)
			return false;
		if (IsArray != other.IsArray)
			return false;
		if (IsReferenceType != other.IsReferenceType)
			return false;
		if (IsVoid != other.IsVoid)
			return false;

		return true;
	}

	/// <inheritdoc/>
	public override bool Equals (object? obj)
	{
		return obj is ReturnType other && Equals (other);
	}

	/// <inheritdoc/>
	public override int GetHashCode ()
	{
		return HashCode.Combine (Type, IsNullable, IsBlittable, IsSmartEnum, IsArray, IsReferenceType, IsVoid);
	}

	public static bool operator == (ReturnType left, ReturnType right)
	{
		return left.Equals (right);
	}

	public static bool operator != (ReturnType left, ReturnType right)
	{
		return !left.Equals (right);
	}

	/// <inheritdoc/>
	public override string ToString ()
	{
		var sb = new StringBuilder ("{");
		sb.Append ($"Type: {Type}, ");
		sb.Append ($"IsNullable: {IsNullable}, ");
		sb.Append ($"IsBlittable: {IsBlittable}, ");
		sb.Append ($"IsSmartEnum: {IsSmartEnum}, ");
		sb.Append ($"IsArray: {IsArray}, ");
		sb.Append ($"IsReferenceType: {IsReferenceType}, ");
		sb.Append ($"IsVoid : {IsVoid} }}");
		return sb.ToString ();
	}
}
