using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.Availability;
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
					symbolAvailability: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Add,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [],
							modifiers: []),
						new (
							accessorKind: AccessorKind.Remove,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [],
							modifiers: []
						),
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
					symbolAvailability: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Add,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [],
							modifiers: []
						),
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
					symbolAvailability: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Add,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [],
							modifiers: []
						),
						new (
							accessorKind: AccessorKind.Remove,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [],
							modifiers: [
								SyntaxFactory.Token (SyntaxKind.InternalKeyword)
							]
						),
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
					symbolAvailability: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Add,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [],
							modifiers: []
						),
						new (
							accessorKind: AccessorKind.Remove,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [],
							modifiers: [
								SyntaxFactory.Token (SyntaxKind.InternalKeyword)
							]
						),
					]
				)
			];

			const string eventAvailability = @"
using System.Runtime.Versioning;
using System;

namespace Test;

public class TestClass {

	[SupportedOSPlatform (""ios17.0"")]
	public event EventHandler MyEvent { add; }
}
";
			var eventAvailabilityBuilder = SymbolAvailability.CreateBuilder ();
			eventAvailabilityBuilder.Add (new SupportedOSPlatformData ("ios17.0"));

			yield return [
				eventAvailability,
				new Event (
					name: "MyEvent",
					type: "System.EventHandler",
					symbolAvailability: eventAvailabilityBuilder.ToImmutable (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Add
							, symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [],
							modifiers: []
						),
					]
				)
			];

			const string eventAccessorAvailability = @"
using System.Runtime.Versioning;
using System;

namespace Test;

public class TestClass {

	[SupportedOSPlatform (""ios"")]
	public event EventHandler MyEvent { 
		[SupportedOSPlatform (""ios17.0"")]
		add; 
		[SupportedOSPlatform (""ios17.0"")]
		remove;}
}
";
			eventAvailabilityBuilder.Clear ();
			eventAvailabilityBuilder.Add (new SupportedOSPlatformData ("ios"));

			var accessorAvailabilityBuilder = SymbolAvailability.CreateBuilder ();
			accessorAvailabilityBuilder.Add (new SupportedOSPlatformData ("ios17.0"));


			yield return [
				eventAccessorAvailability,
				new Event (
					name: "MyEvent",
					type: "System.EventHandler",
					symbolAvailability: eventAvailabilityBuilder.ToImmutable (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Add,
							symbolAvailability: accessorAvailabilityBuilder.ToImmutable (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
							],
							modifiers: []
						),
						new (
							accessorKind: AccessorKind.Remove,
							symbolAvailability: accessorAvailabilityBuilder.ToImmutable (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
							],
							modifiers: []
						),
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
		var (compilation, syntaxTrees) = CreateCompilation (platform, sources: inputText);
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
