// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using System.Collections.Generic;
using Microsoft.Macios.Generator.DataModel;
using Xunit;
using static Microsoft.Macios.Generator.Tests.TestDataFactory;

namespace Microsoft.Macios.Generator.Tests.DataModel;

public class ParameterTests {

	class TestDataGetVariableName : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			var exampleParameter = new Parameter (0, ReturnTypeForBool (), "firstParameter");
			yield return [exampleParameter, Parameter.VariableType.Handle, $"{exampleParameter.Name}__handle__"];
			yield return [exampleParameter, Parameter.VariableType.BlockLiteral, $"block_ptr_{exampleParameter.Name}"];
			yield return [exampleParameter, Parameter.VariableType.PrimitivePointer, $"converted_{exampleParameter.Name}"];
			yield return [exampleParameter, Parameter.VariableType.NSArray, $"nsa_{exampleParameter.Name}"];
			yield return [exampleParameter, Parameter.VariableType.NSString, $"ns{exampleParameter.Name}"];
			yield return [exampleParameter, Parameter.VariableType.NSStringStruct, $"_s{exampleParameter.Name}"];
			yield return [exampleParameter, Parameter.VariableType.BindFrom, $"nsb_{exampleParameter.Name}"];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[ClassData (typeof (TestDataGetVariableName))]
	void GetNameForVariableTypeTests (Parameter parameter, Parameter.VariableType variableType, string expectedName)
		=> Assert.Equal (expectedName, parameter.GetNameForVariableType (variableType));

	class TestDataNeedsNullCheckTests : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			yield return [
				new Parameter (0, ReturnTypeForBool (), "firstParameter"),
				false,
			];

			yield return [
				new Parameter (0, ReturnTypeForInt (), "firstParameter"),
				false,
			];

			yield return [
				new Parameter (0, ReturnTypeForInt (isNullable: true), "firstParameter"),
				false,
			];

			yield return [
				new Parameter (0, ReturnTypeForString (), "firstParameter"),
				true,
			];

			yield return [
				new Parameter (0, ReturnTypeForStruct ("MyStruct"), "firstParameter"),
				false,
			];

			yield return [
				new Parameter (0, ReturnTypeForClass ("MyClass"), "firstParameter") {
					ReferenceKind = ReferenceKind.Ref
				},
				false,
			];

			yield return [
				new Parameter (0, ReturnTypeForArray ("MyClass"), "firstParameter"),
				true,
			];

			yield return [
				new Parameter (0, ReturnTypeForInterface ("IMyClass"), "firstParameter"),
				true,
			];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[ClassData (typeof (TestDataNeedsNullCheckTests))]
	void NeedsNullCheckTests (Parameter parameter, bool expectedNeedsNullCheck)
		=> Assert.Equal (expectedNeedsNullCheck, parameter.NeedsNullCheck);
}
