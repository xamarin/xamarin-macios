// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
#pragma warning disable APL0003
using System.Collections.Immutable;
using Microsoft.Macios.Generator.Availability;
using Microsoft.Macios.Generator.DataModel;
using ObjCBindings;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.DataModel;

public class EnumMembersEqualityComparerTests {

	readonly EnumMembersEqualityComparer comparer;

	public EnumMembersEqualityComparerTests ()
	{
		comparer = new EnumMembersEqualityComparer ();
	}

	[Fact]
	public void NotEqualsDiffLength ()
	{
		ImmutableArray<EnumMember> x = [new ("name", string.Empty, string.Empty, new (), new (), []), new ("name1", string.Empty, string.Empty)];
		ImmutableArray<EnumMember> y = [new ("name", string.Empty, string.Empty, new (), new (), [])];
		Assert.False (comparer.Equals (x, y));
	}

	[Fact]
	public void NotEqualsDiffAttributes ()
	{
		ImmutableArray<EnumMember> x = [new ("name", string.Empty, string.Empty, new (), new (), []), new ("name1", string.Empty, string.Empty)];
		ImmutableArray<EnumMember> y = [new ("name", string.Empty, string.Empty, new (), new (), [
			new ("AttrName")
		]),
			new ("name1", string.Empty, string.Empty)];
		Assert.False (comparer.Equals (x, y));
	}

	[Fact]
	public void EqualsSameOrder ()
	{
		ImmutableArray<EnumMember> x = [new ("name", string.Empty, string.Empty, new (), new SymbolAvailability (), []), new ("name1", string.Empty, string.Empty)];
		ImmutableArray<EnumMember> y = [new ("name", string.Empty, string.Empty, new (), new SymbolAvailability (), []), new ("name1", string.Empty, string.Empty)];
		Assert.True (comparer.Equals (x, y));
	}

	[Fact]
	public void EqualsDiffOrder ()
	{
		ImmutableArray<EnumMember> x = [new ("name1", string.Empty,  string.Empty), new ("name", string.Empty, string.Empty)];
		ImmutableArray<EnumMember> y = [new ("name", string.Empty, string.Empty), new ("name1", string.Empty, string.Empty)];
		Assert.True (comparer.Equals (x, y));
	}

	[Fact]
	public void NotEqualsDiffFieldData ()
	{
		ImmutableArray<EnumMember> x = [new ("name", "", "", new ("x", "xLib", EnumValue.Default), new SymbolAvailability (), []), new ("name1", "", "")];
		ImmutableArray<EnumMember> y = [new ("name", "", "", new ("y", "xLib", EnumValue.Default), new SymbolAvailability (), []), new ("name1", "", "")];
		Assert.False (comparer.Equals (x, y));
	}
}
