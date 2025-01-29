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

	class TestDataCastToPrimitive : IEnumerable<object []> {
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
	[ClassData (typeof (TestDataCastToPrimitive))]
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

	class TestDataGetNSArrayAuxVariableTest : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			// not array 

			yield return [
				new Parameter (0, ReturnTypeForInt (isNullable: false), "myParam"),
				null!,
				false];

			// not nullable string[]
			yield return [
				new Parameter (0, ReturnTypeForArray ("string", isNullable: false), "myParam"),
				"var nsa_myParam = NSArray.FromStrings (myParam);",
				false];

			yield return [
				new Parameter (0, ReturnTypeForArray ("string", isNullable: false), "myParam"),
				"using var nsa_myParam = NSArray.FromStrings (myParam);",
				true];

			// nullable string []
			yield return [
				new Parameter (0, ReturnTypeForArray ("string", isNullable: true), "myParam"),
				"var nsa_myParam = myParam is null ? null : NSArray.FromStrings (myParam);",
				false];

			yield return [
				new Parameter (0, ReturnTypeForArray ("string", isNullable: true), "myParam"),
				"using var nsa_myParam = myParam is null ? null : NSArray.FromStrings (myParam);",
				true];

			// nsstrings

			yield return [
				new Parameter (0, ReturnTypeForArray ("NSString", isNullable: false), "myParam"),
				"var nsa_myParam = NSArray.FromNSObjects (myParam);",
				false];

			yield return [
				new Parameter (0, ReturnTypeForArray ("NSString", isNullable: false), "myParam"),
				"using var nsa_myParam = NSArray.FromNSObjects (myParam);",
				true];

			yield return [
				new Parameter (0, ReturnTypeForArray ("NSString", isNullable: true), "myParam"),
				"var nsa_myParam = myParam is null ? null : NSArray.FromNSObjects (myParam);",
				false];

			yield return [
				new Parameter (0, ReturnTypeForArray ("NSString", isNullable: true), "myParam"),
				"using var nsa_myParam = myParam is null ? null : NSArray.FromNSObjects (myParam);",
				true];


		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[ClassData (typeof (TestDataGetNSArrayAuxVariableTest))]
	void GetNSArrayAuxVariableTests (in Parameter parameter, string? expectedDeclaration, bool withUsing)
	{
		var declaration = GetNSArrayAuxVariable (in parameter, withUsing: withUsing);
		if (expectedDeclaration is null) {
			Assert.Null (declaration);
		} else {
			Assert.NotNull (declaration);
			Assert.Equal (expectedDeclaration, declaration.ToString ());
		}
	}
}
