// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.DataModel;
using Xamarin.Tests;
using Xamarin.Utils;
using Xunit;
using static Microsoft.Macios.Generator.Tests.TestDataFactory;

namespace Microsoft.Macios.Generator.Tests.DataModel;

public class DelegateInfoTests : BaseGeneratorTestClass {

	class TestDataFromMethodDeclaration : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			const string actionNoParam = @"
using System;

namespace NS {
	public class MyClass {
		public void MyMethod (Action cb) {}
	}
}
";

			yield return [
				actionNoParam,
				new Method (
					type: "NS.MyClass",
					name: "MyMethod",
					returnType: ReturnTypeForVoid (),
					symbolAvailability: new (),
					exportMethodData: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (
							position: 0,
							type: ReturnTypeForAction (),
							name: "cb"
						) {
							Delegate = new (
								type: "System.Action",
								name: "Invoke",
								returnType: "void",
								parameters: []
							)
						}
					]
				)
			];

			const string actionSingleParam = @"
using System;

namespace NS {
	public class MyClass {
		public void MyMethod (Action<string> cb) {}
	}
}
";

			yield return [
				actionSingleParam,
				new Method (
					type: "NS.MyClass",
					name: "MyMethod",
					returnType: ReturnTypeForVoid (),
					symbolAvailability: new (),
					exportMethodData: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (
							position: 0,
							type: ReturnTypeForAction ("string"),
							name: "cb"
						) {
							Delegate = new (
								type: "System.Action<string>",
								name: "Invoke",
								returnType: "void",
								parameters: [
									new (
										position: 0,
										type: "string",
										name: "obj",
										isBlittable: false
									),
								])
						}
					]
				)
			];

			const string actionSingleNullableParam = @"
using System;

namespace NS {
	public class MyClass {
		public void MyMethod (Action<string?> cb) {}
	}
}
";

			yield return [
				actionSingleNullableParam,
				new Method (
					type: "NS.MyClass",
					name: "MyMethod",
					returnType: ReturnTypeForVoid (),
					symbolAvailability: new (),
					exportMethodData: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (
							position: 0,
							type: ReturnTypeForAction("string?"),
							name: "cb"
						) {
							Delegate = new (
								type: "System.Action<string?>",
								name: "Invoke",
								returnType: "void",
								parameters: [
									new (
										position: 0,
										type: "string",
										name: "obj",
										isBlittable: false
									) {
										IsNullable = true
									},
								])
						}
					]
				)
			];
			
			const string actionMultiParam = @"
using System;

namespace NS {
	public class MyClass {
		public void MyMethod (Action<string, string> cb) {}
	}
}
";

			yield return [
				actionMultiParam,
				new Method (
					type: "NS.MyClass",
					name: "MyMethod",
					returnType: ReturnTypeForVoid (),
					symbolAvailability: new (),
					exportMethodData: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (
							position: 0,
							type: ReturnTypeForAction("string", "string"),
							name: "cb"
						) {
							Delegate = new (
								type: "System.Action<string, string>",
								name: "Invoke",
								returnType: "void",
								parameters: [
									new (
										position: 0,
										type: "string",
										name: "arg1",
										isBlittable: false
									),
									new (
										position: 1,
										type: "string",
										name: "arg2",
										isBlittable: false
									),
								])
						}
					]
				)
			];

			const string funcSingleParam = @"
using System;

namespace NS {
	public class MyClass {
		public void MyMethod (Func<string, string> cb) {}
	}
}
";

			yield return [
				funcSingleParam,
				new Method (
					type: "NS.MyClass",
					name: "MyMethod",
					returnType: ReturnTypeForVoid (),
					symbolAvailability: new (),
					exportMethodData: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (
							position: 0,
							type: ReturnTypeForFunc ("string", "string"),
							name: "cb"
						) {
							Delegate = new (
								type: "System.Func<string, string>",
								name: "Invoke",
								returnType: "string",
								parameters: [
									new (
										position: 0,
										type: "string",
										name: "arg",
										isBlittable: false
									),
								])
						}
					]
				)
			];

			const string funcMultiParam = @"
using System;

namespace NS {
	public class MyClass {
		public void MyMethod (Func<string, string, string> cb) {}
	}
}
";

			yield return [
				funcMultiParam,
				new Method (
					type: "NS.MyClass",
					name: "MyMethod",
					returnType: ReturnTypeForVoid (),
					symbolAvailability: new (),
					exportMethodData: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (
							position: 0,
							type: ReturnTypeForFunc ("string", "string", "string"),
							name: "cb"
						) {
							Delegate = new (
								type: "System.Func<string, string, string>",
								name: "Invoke",
								returnType: "string",
								parameters: [
									new (
										position: 0,
										type: "string",
										name: "arg1",
										isBlittable: false
									),
									new (
										position: 1,
										type: "string",
										name: "arg2",
										isBlittable: false
									),
								])
						}
					]
				)
			];

			const string customDelegate = @"
using System;

namespace NS {
	public class MyClass {
		public delegate int? Callback(string name, string? middleName, params string[] surname);

		public void MyMethod (Callback cb) {}
	}
}
";

			yield return [
				customDelegate,
				new Method (
					type: "NS.MyClass",
					name: "MyMethod",
					returnType: ReturnTypeForVoid (),
					symbolAvailability: new (),
					exportMethodData: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (
							position: 0,
							type: ReturnTypeForDelegate ("NS.MyClass.Callback"),
							name: "cb"
						) {
							Delegate = new (
								type: "NS.MyClass.Callback",
								name: "Invoke",
								returnType: "int?",
								parameters: [
									new (
										position: 0,
										type: "string",
										name: "name",
										isBlittable: false
									),
									new (
										position: 1,
										type: "string",
										name: "middleName",
										isBlittable: false
									) {
										IsNullable = true
									},
									new (
										position: 2,
										type: "string",
										name: "surname",
										isBlittable: false
									) {
										IsParams = true,
										IsArray = true,
									},
								])
						}
					]
				)
			];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[AllSupportedPlatformsClassData<TestDataFromMethodDeclaration>]
	void FromMethodDeclaration (ApplePlatform platform, string inputText, Method expected)
	{
		var (compilation, syntaxTrees) = CreateCompilation (platform, sources: inputText);
		Assert.Single (syntaxTrees);
		var semanticModel = compilation.GetSemanticModel (syntaxTrees [0]);
		var declaration = syntaxTrees [0].GetRoot ()
			.DescendantNodes ().OfType<MethodDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (declaration);
		Assert.True (Method.TryCreate (declaration, semanticModel, out var changes));
		Assert.NotNull (changes);
		Assert.Equal (expected, changes);
	}
}
