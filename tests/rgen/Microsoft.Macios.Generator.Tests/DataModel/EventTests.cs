using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.DataModel;
using Xamarin.Tests;
using Xamarin.Utils;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.DataModel;

public class EventTests : BaseGeneratorTestClass {
	class TestDataFromPropertyDeclaration : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			const string automaticGetter = @"
using System;

namespace Test;

public class TestClass {

	public event EventHandler MyEvent { add; remove; }
}
";

			yield return [
				automaticGetter,
				new Event (
					name: "MyEvent",
					type: "System.EventHandler",
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (AccessorKind.Add, [], []),
						new (AccessorKind.Remove, [], []),
					]
				)
			];

			const string justAdder = @"
using System;

namespace Test;

public class TestClass {

	public event EventHandler MyEvent { add; }
}
";

			yield return [
				justAdder,
				new Event (
					name: "MyEvent",
					type: "System.EventHandler",
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (AccessorKind.Add, [], []),
					]
				)
			];

			const string internalRemove = @"
using System;

namespace Test;

public class TestClass {

	public event EventHandler MyEvent { add; internal remove; }
}
";

			yield return [
				internalRemove,
				new Event (
					name: "MyEvent",
					type: "System.EventHandler",
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (AccessorKind.Add, [], []),
						new (AccessorKind.Remove, [], [
							SyntaxFactory.Token (SyntaxKind.InternalKeyword)
						]),
					]
				)
			];

			const string customEventType = @"
using System;

namespace Test;

public class MyEventArgs : EventArgs
{
	public string Name { get; set; }
    public string Surname { get; set; }
}

public delegate void MyEventHandler(Object sender, MyEventArgs e);

public class TestClass {
	public event MyEventHandler MyEvent { add; internal remove; }
}
";

			yield return [
				customEventType,
				new Event (
					name: "MyEvent",
					type: "Test.MyEventHandler",
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (AccessorKind.Add, [], []),
						new (AccessorKind.Remove, [], [
							SyntaxFactory.Token (SyntaxKind.InternalKeyword)
						]),
					]
				)
			];
		}


		IEnumerator IEnumerable.GetEnumerator ()
			=> GetEnumerator ();
	}

	[Theory]
	[AllSupportedPlatformsClassData<TestDataFromPropertyDeclaration>]
	void FromEventDeclaration (ApplePlatform platform, string inputText, Event expected)
	{
		var (compilation, syntaxTrees) = CreateCompilation (nameof (FromEventDeclaration),
			platform, inputText);
		Assert.Single (syntaxTrees);
		var semanticModel = compilation.GetSemanticModel (syntaxTrees [0]);
		var declaration = syntaxTrees [0].GetRoot ()
			.DescendantNodes ().OfType<EventDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (declaration);
		Assert.True (Event.TryCreate (declaration, semanticModel, out var changes));
		Assert.NotNull (changes);
		Assert.Equal (expected, changes);
	}
}
