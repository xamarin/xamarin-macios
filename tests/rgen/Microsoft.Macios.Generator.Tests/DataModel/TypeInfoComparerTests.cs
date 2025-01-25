// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using Microsoft.Macios.Generator.DataModel;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.DataModel;

public class TypeInfoComparerTests {

	TypeInfoComparer compare = new ();

	[Fact]
	public void CompareDiffReturnType ()
	{
		var x = new TypeInfo ("string");
		var y = new TypeInfo ("int");
		Assert.Equal (String.Compare (x.FullyQualifiedName, y.FullyQualifiedName, StringComparison.Ordinal), compare.Compare (x, y));
	}

	[Fact]
	public void CompareDiffNullable ()
	{
		var x = new TypeInfo (
			name: "void",
			isNullable: true,
			isBlittable: false,
			isSmartEnum: false,
			isArray: false,
			isReferenceType: false
		);

		var y = new TypeInfo (
			name: "void",
			isNullable: false,
			isBlittable: false,
			isSmartEnum: false,
			isArray: false,
			isReferenceType: false
		);
		Assert.Equal (x.IsNullable.CompareTo (y.IsNullable), compare.Compare (x, y));
	}

	[Fact]
	public void CompareDiffBlittable ()
	{
		var x = new TypeInfo (
			name: "void",
			isNullable: false,
			isBlittable: true,
			isSmartEnum: false,
			isArray: false,
			isReferenceType: false
		);

		var y = new TypeInfo (
			name: "void",
			isNullable: false,
			isBlittable: false,
			isSmartEnum: false,
			isArray: false,
			isReferenceType: false
		);
		Assert.Equal (x.IsBlittable.CompareTo (y.IsBlittable), compare.Compare (x, y));
	}

	[Fact]
	public void CompareDiffSmartEnum ()
	{
		var x = new TypeInfo (
			name: "void",
			isNullable: false,
			isBlittable: false,
			isSmartEnum: true,
			isArray: false,
			isReferenceType: false
		);

		var y = new TypeInfo (
			name: "void",
			isNullable: false,
			isBlittable: false,
			isSmartEnum: false,
			isArray: false,
			isReferenceType: false
		);
		Assert.Equal (x.IsSmartEnum.CompareTo (y.IsSmartEnum), compare.Compare (x, y));
	}

	[Fact]
	public void CompareDiffIsArray ()
	{
		var x = new TypeInfo (
			name: "void",
			isNullable: false,
			isBlittable: false,
			isSmartEnum: false,
			isArray: true,
			isReferenceType: false
		);

		var y = new TypeInfo (
			name: "void",
			isNullable: false,
			isBlittable: false,
			isSmartEnum: false,
			isArray: false,
			isReferenceType: false
		);
		Assert.Equal (x.IsArray.CompareTo (y.IsArray), compare.Compare (x, y));
	}

	[Fact]
	public void CompareDiffIsReference ()
	{
		var x = new TypeInfo (
			name: "void",
			isNullable: false,
			isBlittable: false,
			isSmartEnum: false,
			isArray: false,
			isReferenceType: true
		);

		var y = new TypeInfo (
			name: "void",
			isNullable: false,
			isBlittable: false,
			isSmartEnum: false,
			isArray: false,
			isReferenceType: false
		);
		Assert.Equal (x.IsReferenceType.CompareTo (y.IsReferenceType), compare.Compare (x, y));
	}
}
