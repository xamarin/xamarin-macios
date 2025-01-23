// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.Extensions;

namespace Microsoft.Macios.Generator.DataModel;

/// <summary>
/// Readonly structure that represents a change in a method return type.
/// </summary>
readonly partial struct TypeInfo : IEquatable<TypeInfo> {

	public static TypeInfo Void = new ("void", SpecialType.System_Void) { Parents = ["System.ValueType", "object"], };

	readonly string fullyQualifiedName = string.Empty;
	/// <summary>
	/// The fully qualified name of the type.
	/// </summary>
	public string FullyQualifiedName {
		get => fullyQualifiedName;
		init {
			fullyQualifiedName = value;
			var index = fullyQualifiedName.LastIndexOf ('.');
			Name = index != -1
				? fullyQualifiedName.Substring (index + 1)
				: fullyQualifiedName;
		}
	}

	public string Name { get; private init; } = string.Empty;

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
	/// Returns if the type is a struct.
	/// </summary>
	public bool IsStruct { get; }

	/// <summary>
	/// Returns if the return type is void.
	/// </summary>
	public bool IsVoid => SpecialType == SpecialType.System_Void;

	/// <summary>
	/// True if the type is for an interface.
	/// </summary>
	public bool IsInterface { get; init; }

	/// <summary>
	/// True if the type represents an integer that was built using one of the keywords, like byte, int, nint etc.
	///
	/// This can be used to decide if we should use the name of the metadata name to cast the value.
	/// </summary>
	public bool IsNativeIntegerType { get; init; }

	/// <summary>
	/// True if an enumerator was marked with the NativeAttribute.
	/// </summary>
	public bool IsNativeEnum { get; init; }

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
		FullyQualifiedName = name;
		SpecialType = specialType;
	}

	internal TypeInfo (string name,
		SpecialType specialType = SpecialType.None,
		bool isNullable = false,
		bool isBlittable = false,
		bool isSmartEnum = false,
		bool isArray = false,
		bool isReferenceType = false,
		bool isStruct = false) : this (name, specialType)
	{
		IsNullable = isNullable;
		IsBlittable = isBlittable;
		IsSmartEnum = isSmartEnum;
		IsArray = isArray;
		IsReferenceType = isReferenceType;
		IsStruct = isStruct;
	}

	/// <inheritdoc/>
	public bool Equals (TypeInfo other)
	{
		if (FullyQualifiedName != other.FullyQualifiedName)
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
		if (IsStruct != other.IsStruct)
			return false;
		if (IsVoid != other.IsVoid)
			return false;
		if (EnumUnderlyingType != other.EnumUnderlyingType)
			return false;
		if (IsInterface != other.IsInterface)
			return false;
		if (IsNativeIntegerType != other.IsNativeIntegerType)
			return false;
		if (IsNativeEnum != other.IsNativeEnum)
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
		return HashCode.Combine (FullyQualifiedName, IsNullable, IsBlittable, IsSmartEnum, IsArray, IsReferenceType, IsVoid);
	}

	public static bool operator == (TypeInfo left, TypeInfo right)
	{
		return left.Equals (right);
	}

	public static bool operator != (TypeInfo left, TypeInfo right)
	{
		return !left.Equals (right);
	}

	const string NativeHandle = "NativeHandle";
	const string IntPtr = "IntPtr";
	const string UIntPtr = "UIntPtr";

	public string? ToMarshallType (ReferenceKind referenceKind)
	{
#pragma warning disable format
		var type = this switch {
			// special cases based on name
			{ Name: "nfloat" or "NFloat" } => "nfloat", 
			{ Name: "nint" or "nuint" } => MetadataName,
			// special string case
			{ SpecialType: SpecialType.System_String } => NativeHandle, // use a NSString when we get a string

			// NSObject should use the native handle
			{ IsNSObject: true } => NativeHandle, 
			{ IsINativeObject: true } => NativeHandle,

			// structs will use their name
			{ IsStruct: true, SpecialType: SpecialType.System_Double } => "Double", 
			{ IsStruct: true } => Name,

			// enums:
			// IsSmartEnum: We are using a nsstring, so it should be a native handle.
			// IsNativeEnum: Depends on the enum backing field kind.
			// GeneralEnum: Depends on the EnumUnderlyingType

			{ IsSmartEnum: true } => NativeHandle, 
			{ IsNativeEnum: true, EnumUnderlyingType: SpecialType.System_Int64 } => IntPtr, 
			{ IsNativeEnum: true, EnumUnderlyingType: SpecialType.System_UInt64 } => UIntPtr, 
			{ IsEnum: true, EnumUnderlyingType: not null } => EnumUnderlyingType.GetKeyword (),

			// special type that is a keyword (none would be a ref type)
			{ SpecialType: SpecialType.System_Void } => SpecialType.GetKeyword (),

			// This should not happen in bindings because all of the types should either be native objects
			// nsobjects, or structs 
			{ IsReferenceType: false } => Name,

			_ => null,
		};
#pragma warning restore format
		return type;
	}

	/// <inheritdoc/>
	public override string ToString ()
	{
		var sb = new StringBuilder ("{");
		sb.Append ($"Name: '{FullyQualifiedName}', ");
		sb.Append ($"MetadataName: '{MetadataName}', ");
		sb.Append ($"SpecialType: '{SpecialType}', ");
		sb.Append ($"IsNullable: {IsNullable}, ");
		sb.Append ($"IsBlittable: {IsBlittable}, ");
		sb.Append ($"IsSmartEnum: {IsSmartEnum}, ");
		sb.Append ($"IsArray: {IsArray}, ");
		sb.Append ($"IsReferenceType: {IsReferenceType}, ");
		sb.Append ($"IsStruct: {IsStruct}, ");
		sb.Append ($"IsVoid : {IsVoid}, ");
		sb.Append ($"IsNSObject : {IsNSObject}, ");
		sb.Append ($"IsNativeObject: {IsINativeObject}, ");
		sb.Append ($"IsInterface: {IsInterface}, ");
		sb.Append ($"IsNativeIntegerType: {IsNativeIntegerType}, ");
		sb.Append ($"IsNativeEnum: {IsNativeEnum}, ");
		sb.Append ($"EnumUnderlyingType: '{EnumUnderlyingType?.ToString () ?? "null"}', ");
		sb.Append ("Parents: [");
		sb.AppendJoin (", ", parents);
		sb.Append ("], Interfaces: [");
		sb.AppendJoin (", ", interfaces);
		sb.Append ("]}");
		return sb.ToString ();
	}
}
