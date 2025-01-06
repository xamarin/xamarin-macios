using System;
using Microsoft.Macios.Generator.DataModel;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.DataModel;

public class MethodReturnTypeComparerTests {

	MethodReturnTypeComparer compare = new ();

	[Fact]
	public void CompareDiffReturnType ()
	{
		var x = new MethodReturnType ("string");
		var y = new MethodReturnType ("int");
		Assert.Equal (String.Compare (x.Type, y.Type, StringComparison.Ordinal), compare.Compare (x, y));
	}

	[Fact]
	public void CompareDiffNullable ()
	{
		var x = new MethodReturnType (
			type: "void", 
			isNullable: true, 
			isBlittable: false, 
			isSmartEnum: false, 
			isArray: false, 
			isReferenceType: false
		);
		
		var y = new MethodReturnType (
			type: "void", 
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
		var x = new MethodReturnType (
			type: "void", 
			isNullable: false, 
			isBlittable: true, 
			isSmartEnum: false, 
			isArray: false, 
			isReferenceType: false
		);
		
		var y = new MethodReturnType (
			type: "void", 
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
		var x = new MethodReturnType (
			type: "void", 
			isNullable: false, 
			isBlittable: false, 
			isSmartEnum: true, 
			isArray: false, 
			isReferenceType: false
		);
		
		var y = new MethodReturnType (
			type: "void", 
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
		var x = new MethodReturnType (
			type: "void", 
			isNullable: false, 
			isBlittable: false, 
			isSmartEnum: false, 
			isArray: true, 
			isReferenceType: false
		);
		
		var y = new MethodReturnType (
			type: "void", 
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
		var x = new MethodReturnType (
			type: "void", 
			isNullable: false, 
			isBlittable: false, 
			isSmartEnum: false, 
			isArray: false, 
			isReferenceType: true 
		);
		
		var y = new MethodReturnType (
			type: "void", 
			isNullable: false, 
			isBlittable: false, 
			isSmartEnum: false, 
			isArray: false, 
			isReferenceType: false
		);
		Assert.Equal (x.IsReferenceType.CompareTo (y.IsReferenceType), compare.Compare (x, y));
	}
}
