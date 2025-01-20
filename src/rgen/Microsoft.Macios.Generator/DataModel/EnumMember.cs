// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.Availability;
using ObjCBindings;

namespace Microsoft.Macios.Generator.DataModel;

/// <summary>
/// Structure that represents a change that was made by the user on enum members that has to be
/// reflected in the generated code.
/// </summary>
[StructLayout(LayoutKind.Auto)]
readonly partial struct EnumMember : IEquatable<EnumMember> {
	/// <summary>
	/// Get the name of the member.
	/// </summary>
	public string Name { get; }

	/// <summary>
	/// The platform availability of the enum value.
	/// </summary>
	public SymbolAvailability SymbolAvailability { get; }

	/// <summary>
	/// Get the attributes added to the member.
	/// </summary>
	public ImmutableArray<AttributeCodeChange> Attributes { get; }
	
	/// <summary>
	/// Create a new change that happened on a member.
	/// </summary>
	/// <param name="name">The name of the changed member.</param>
	/// <param name="libraryName">The library name of the smart enum.</param>
	/// <param name="libraryPath">The library path to the library, null if it is a known frameworl.</param>
	public EnumMember (string name, string libraryName, string? libraryPath)
		: this (
			name: name,
			libraryName: libraryName,
			libraryPath: libraryPath,
			fieldData: null,
			symbolAvailability: new SymbolAvailability (),
			attributes: ImmutableArray<AttributeCodeChange>.Empty)
	{
	}

	/// <inheritdoc />
	public bool Equals (EnumMember other)
	{
		if (Name != other.Name)
			return false;
		if (SymbolAvailability != other.SymbolAvailability)
			return false;
		if (FieldInfo != other.FieldInfo)
			return false;

		var attrComparer = new AttributesEqualityComparer ();
		return attrComparer.Equals (Attributes, other.Attributes);
	}

	/// <inheritdoc />
	public override bool Equals (object? obj)
	{
		return obj is EnumMember other && Equals (other);
	}

	/// <inheritdoc />
	public override int GetHashCode ()
	{
		return HashCode.Combine (Name, SymbolAvailability, Attributes);
	}

	public static bool operator == (EnumMember x, EnumMember y)
	{
		return x.Equals (y);
	}

	public static bool operator != (EnumMember x, EnumMember y)
	{
		return !(x == y);
	}

	/// <inheritdoc />
	public override string ToString ()
	{
		var sb = new StringBuilder (
			$"{{ Name: '{Name}' SymbolAvailability: {SymbolAvailability} FieldInfo: {FieldInfo} Attributes: [");
		sb.AppendJoin (", ", Attributes);
		sb.Append ("] }");
		return sb.ToString ();
	}
}
