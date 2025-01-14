// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
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
	/// The metadata name of the type. This is normally the same as name except
	/// when the SpecialType is not None.
	/// </summary>
	public string? MetadataName { get; init; }

	/// <summary>
	/// If the type is an enum, it returns the special type of the underlying type.
	/// </summary>
	public SpecialType? EnumUnderlyingType { get; init; }

	/// <summary>
	/// If the type is an enum type.
	/// </summary>
	[MemberNotNullWhen (true, nameof (EnumUnderlyingType))]
	public bool IsEnum => EnumUnderlyingType is not null;

	/// <summary>
	/// The special type enum of the type info. This is used to differentiate nint from IntPtr and other.
	/// </summary>
	public SpecialType SpecialType { get; } = SpecialType.None;

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
	public bool IsVoid => SpecialType == SpecialType.System_Void;

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

	internal TypeInfo (string name, SpecialType specialType)
	{
		Name = name;
		SpecialType = specialType;
	}

	internal TypeInfo (string name,
		SpecialType specialType = SpecialType.None,
		bool isNullable = false,
		bool isBlittable = false,
		bool isSmartEnum = false,
		bool isArray = false,
		bool isReferenceType = false) : this (name, specialType)
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
				: symbol.ToDisplayString ().Trim ('?', '[', ']'),
			symbol.SpecialType)
	{
		IsNullable = symbol.NullableAnnotation == NullableAnnotation.Annotated;
		IsBlittable = symbol.IsBlittable ();
		IsSmartEnum = symbol.IsSmartEnum ();
		IsArray = symbol is IArrayTypeSymbol;
		IsReferenceType = symbol.IsReferenceType;

		// data that we can get from the symbol without being INamedType
		symbol.GetInheritance (
			isNSObject: out isNSObject,
			isNativeObject: out isINativeObject,
			parents: out parents,
			interfaces: out interfaces);

		// try to get the named type symbol to have more educated decisions
		var namedTypeSymbol = symbol as INamedTypeSymbol;

		// store the enum special type, useful when generate code that needs to cast
		EnumUnderlyingType = namedTypeSymbol?.EnumUnderlyingType?.SpecialType;

		if (!IsReferenceType && IsNullable && namedTypeSymbol is not null) {
			// get the type argument for nullable, which we know is the data that was boxed and use it to 
			// overwrite the SpecialType 
			var typeArgument = namedTypeSymbol.TypeArguments [0];
			SpecialType = typeArgument.SpecialType;
			MetadataName = SpecialType is SpecialType.None or SpecialType.System_Void
				? null : typeArgument.MetadataName;
		} else {
			MetadataName = SpecialType is SpecialType.None or SpecialType.System_Void
				? null : symbol.MetadataName;
		}

	}

	/// <inheritdoc/>
	public bool Equals (TypeInfo other)
	{
		if (Name != other.Name)
			return false;
		if (SpecialType != other.SpecialType)
			return false;
		if (MetadataName != other.MetadataName)
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
		if (EnumUnderlyingType != other.EnumUnderlyingType)
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
		sb.Append ($"Name: '{Name}', ");
		sb.Append ($"MetadataName: '{MetadataName}', ");
		sb.Append ($"SpecialType: '{SpecialType}', ");
		sb.Append ($"IsNullable: {IsNullable}, ");
		sb.Append ($"IsBlittable: {IsBlittable}, ");
		sb.Append ($"IsSmartEnum: {IsSmartEnum}, ");
		sb.Append ($"IsArray: {IsArray}, ");
		sb.Append ($"IsReferenceType: {IsReferenceType}, ");
		sb.Append ($"IsVoid : {IsVoid}, ");
		sb.Append ($"IsNSObject : {IsNSObject}, ");
		sb.Append ($"IsNativeObject: {IsINativeObject}, ");
		sb.Append ($"EnumUnderlyingType: '{EnumUnderlyingType?.ToString () ?? "null"}', ");
		sb.Append ("Parents: [");
		sb.AppendJoin (", ", parents);
		sb.Append ("], Interfaces: [");
		sb.AppendJoin (", ", interfaces);
		sb.Append ("]}");
		return sb.ToString ();
	}
}
