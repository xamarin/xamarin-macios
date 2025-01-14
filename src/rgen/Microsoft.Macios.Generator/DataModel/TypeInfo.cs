// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator.Extensions;

namespace Microsoft.Macios.Generator.DataModel;

/// <summary>
/// Readonly structure that represents a change in a method return type.
/// </summary>
readonly struct TypeInfo : IEquatable<TypeInfo> {

	/// <summary>
	/// Type of the parameter.
	/// </summary>
	public string Name { get; }

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

	readonly bool isNSObject = false;

	public bool IsNSObject {
		get => isNSObject;
		init => isNSObject = value;
	}

	readonly bool isINativeObject = false;

	public bool IsINativeObject {
		get => isINativeObject;
		init => isINativeObject = value;
	}

	readonly ImmutableArray<string> parents = [];
	public ImmutableArray<string> Parents {
		get => parents;
		init => parents = value;
	}

	readonly ImmutableArray<string> interfaces = [];

	public ImmutableArray<string> Interfaces {
		get => interfaces;
		init => interfaces = value;
	}

	internal TypeInfo (string name)
	{
		Name = name;
		IsVoid = name == "void";
	}

	internal TypeInfo (string name,
		bool isNullable = false,
		bool isBlittable = false,
		bool isSmartEnum = false,
		bool isArray = false,
		bool isReferenceType = false) : this (name)
	{
		IsNullable = isNullable;
		IsBlittable = isBlittable;
		IsSmartEnum = isSmartEnum;
		IsArray = isArray;
		IsReferenceType = isReferenceType;
	}

	internal TypeInfo (ITypeSymbol symbol) :
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
		symbol.GetInheritance (
			isNSObject: out isNSObject,
			isNativeObject: out isINativeObject,
			parents: out parents,
			interfaces: out interfaces);
	}

	/// <inheritdoc/>
	public bool Equals (TypeInfo other)
	{
		if (Name != other.Name)
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

		// compare base classes and interfaces, order does not matter at all
		var listComparer = new CollectionComparer<string> ();
		if (!listComparer.Equals (parents, other.Parents))
			return false;
		if (!listComparer.Equals (interfaces, other.Interfaces))
			return false;

		return true;
	}

	/// <inheritdoc/>
	public override bool Equals (object? obj)
	{
		return obj is TypeInfo other && Equals (other);
	}

	/// <inheritdoc/>
	public override int GetHashCode ()
	{
		return HashCode.Combine (Name, IsNullable, IsBlittable, IsSmartEnum, IsArray, IsReferenceType, IsVoid);
	}

	public static bool operator == (TypeInfo left, TypeInfo right)
	{
		return left.Equals (right);
	}

	public static bool operator != (TypeInfo left, TypeInfo right)
	{
		return !left.Equals (right);
	}

	/// <inheritdoc/>
	public override string ToString ()
	{
		var sb = new StringBuilder ("{");
		sb.Append ($"Type: {Name}, ");
		sb.Append ($"IsNullable: {IsNullable}, ");
		sb.Append ($"IsBlittable: {IsBlittable}, ");
		sb.Append ($"IsSmartEnum: {IsSmartEnum}, ");
		sb.Append ($"IsArray: {IsArray}, ");
		sb.Append ($"IsReferenceType: {IsReferenceType}, ");
		sb.Append ($"IsVoid : {IsVoid}, ");
		sb.Append ($"IsNSObject : {IsNSObject}, ");
		sb.Append ($"IsNativeObject: {IsINativeObject}, ");
		sb.Append ("Parents: [");
		sb.AppendJoin (", ", parents);
		sb.Append ("], Interfaces: [");
		sb.AppendJoin (", ", interfaces);
		sb.Append ("]}");
		return sb.ToString ();
	}
}
