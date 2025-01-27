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
	
	class TestDataCodeChangesFromClassDeclaration : IEnumerable<object []> {
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
	[ClassData(typeof(TestDataCodeChangesFromClassDeclaration))]
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
}
