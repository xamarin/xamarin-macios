// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator.DataModel;
using Xunit;
using static Microsoft.Macios.Generator.Emitters.BindingSyntaxFactory;
using static Microsoft.Macios.Generator.Tests.TestDataFactory;

namespace Microsoft.Macios.Generator.Tests.Emitters;

public class BindingSyntaxFactoryObjCRuntimeTests {

	class TestDataCastToNativeTests : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{

			// not enum parameter
			var boolParam = new Parameter (
				position: 0,
				type: ReturnTypeForBool (),
				name: "myParam");
			yield return [boolParam, null!];

			// not smart enum parameter
			var enumParam = new Parameter (
				position: 0,
				type: ReturnTypeForEnum ("MyEnum", isNativeEnum: false),
				name: "myParam");

			yield return [enumParam, null!];

			// int64
			var byteEnum = new Parameter (
				position: 0,
				type: ReturnTypeForEnum ("MyEnum", isNativeEnum: true, underlyingType: SpecialType.System_Int64),
				name: "myParam");
			yield return [byteEnum, "(IntPtr) (long) myParam"];

			// uint64
			var int64Enum = new Parameter (
				position: 0,
				type: ReturnTypeForEnum ("MyEnum", isNativeEnum: true, underlyingType: SpecialType.System_UInt64),
				name: "myParam");
			yield return [int64Enum, "(UIntPtr) (ulong) myParam"];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[ClassData (typeof (TestDataCastToNativeTests))]
	void CastToNativeTests (Parameter parameter, string? expectedCast)
	{
		var expression = CastToNative (parameter);
		if (expectedCast is null) {
			Assert.Null (expression);
		} else {
			Assert.NotNull (expression);
			Assert.Equal (expectedCast, expression?.ToString ());
		}
	}
	
	class TestDataCastToPrimitive: IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			// not enum parameter
			var boolParam = new Parameter (
				position: 0, 
				type: ReturnTypeForBool (),
				name: "myParam");	
			yield return [boolParam, null!];
			
			var enumParam = new Parameter (
				position: 0, 
				type: ReturnTypeForEnum ("MyEnum", isNativeEnum: false),
				name: "myParam");	
			
			yield return [enumParam, "(int) myParam"];
			
			var byteParam = new Parameter (
				position: 0, 
				type: ReturnTypeForEnum ("MyEnum", isNativeEnum: false, underlyingType: SpecialType.System_Byte),
				name: "myParam");	
			
			yield return [byteParam, "(byte) myParam"];
			
			
			var longParam = new Parameter (
				position: 0, 
				type: ReturnTypeForEnum ("MyEnum", isNativeEnum: false, underlyingType: SpecialType.System_Int64),
				name: "myParam");	
			
			yield return [longParam, "(long) myParam"];
		}
		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}
	
	[Theory]
	[ClassData(typeof(TestDataCastToPrimitive))]
	void CastToPrimitiveTests (Parameter parameter, string? expectedCast)
	{
		var expression = CastToPrimitive (parameter);
		if (expectedCast is null) {
			Assert.Null (expression);	
		} else {
			Assert.NotNull (expression);
			Assert.Equal (expectedCast, expression?.ToString ());
		}
	}

	[Fact]
	void CastToByteTests ()
	{
		var boolParameter = new Parameter (0, ReturnTypeForBool (), "myParameter");
		var conditionalExpr = CastToByte (boolParameter);
		Assert.NotNull (conditionalExpr);
		Assert.Equal ("myParameter ? (byte) 1 : (byte) 0", conditionalExpr.ToString ());

		var intParameter = new Parameter (1, ReturnTypeForInt (), "myParameter");
		conditionalExpr = CastToByte (intParameter);
		Assert.Null (conditionalExpr);
	}
}
