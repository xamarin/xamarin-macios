// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#pragma warning disable APL0003
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Macios.Generator.DataModel;
using ObjCRuntime;
using Xunit;
using static Microsoft.Macios.Generator.Tests.TestDataFactory;

namespace Microsoft.Macios.Generator.Tests.DataModel.MethodTests;

public class GeneralMethodTests {

	[Fact]
	public void IsThreadSafe ()
	{
		var method = new Method (
			type: "NS.MyClass",
			name: "MyMethod",
			returnType: ReturnTypeForVoid (),
			symbolAvailability: new (),
			exportMethodData: new ("selector", ArgumentSemantic.None, ObjCBindings.Method.IsThreadSafe),
			attributes: [],
			modifiers: [
				SyntaxFactory.Token (SyntaxKind.PublicKeyword),
			],
			parameters: []
		);
		Assert.True (method.IsThreadSafe);

		method = new Method (
			type: "NS.MyClass",
			name: "MyMethod",
			returnType: ReturnTypeForVoid (),
			symbolAvailability: new (),
			exportMethodData: new ("selector", ArgumentSemantic.None, ObjCBindings.Method.Default),
			attributes: [],
			modifiers: [
				SyntaxFactory.Token (SyntaxKind.PublicKeyword),
			],
			parameters: []
		);
		Assert.False (method.IsThreadSafe);
	}
}
