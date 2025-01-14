// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Microsoft.Macios.Generator.Attributes;

namespace Microsoft.Macios.Generator.DataModel;

/// <summary>
/// Struct that unfies the data found in a FieldAttribute and extra information calculated such as
/// the library name and path needed for a field.
/// </summary>
readonly struct FieldInfo<T> : IEquatable<FieldInfo<T>> where T : Enum {


	/// <summary>
	/// Name of the library that contains the smart enum definition.
	/// </summary>
	public string LibraryName { get; }

	/// <summary>
	/// Path of the library that contains the smart enum definition.
	/// </summary>
	public string? LibraryPath { get; }

	/// <summary>
	/// The data of the field attribute used to mark the value as a binding.
	/// </summary>
	public FieldData<T> FieldData { get; }

	public FieldInfo (FieldData<T> fieldData, string libraryName, string? libraryPath = null)
	{
		LibraryName = libraryName;
		LibraryPath = libraryPath;
		FieldData = fieldData;
	}

	public void Deconstruct (out FieldData<T> fieldData, out string libraryName, out string? libraryPath)
	{
		fieldData = FieldData;
		libraryName = LibraryName;
		libraryPath = LibraryPath;
	}

	/// <inheritdoc />
	public bool Equals (FieldInfo<T> other)
	{
		if (FieldData != other.FieldData)
			return false;
		if (LibraryName != other.LibraryName)
			return false;
		return LibraryPath == other.LibraryPath;
	}

	/// <inheritdoc />
	public override bool Equals (object? obj)
	{
		return obj is FieldInfo<T> other && Equals (other);
	}

	/// <inheritdoc />
	public override int GetHashCode ()
	{
		return HashCode.Combine (FieldData, LibraryName, LibraryPath);
	}

	public static bool operator == (FieldInfo<T> x, FieldInfo<T> y)
	{
		return x.Equals (y);
	}

	public static bool operator != (FieldInfo<T> x, FieldInfo<T> y)
	{
		return !(x == y);
	}

	/// <inheritdoc />
	public override string ToString ()
	{
		return $"FieldData = {FieldData}, LibraryName = {LibraryName}, LibraryPath = {LibraryPath ?? "null"}";
	}

}
