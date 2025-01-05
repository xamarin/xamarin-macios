#pragma warning disable APL0003
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.Availability;
using Microsoft.Macios.Generator.DataModel;
using ObjCBindings;
using ObjCRuntime;
using Xamarin.Tests;
using Xamarin.Utils;
using Xunit;
using Property = Microsoft.Macios.Generator.DataModel.Property;

namespace Microsoft.Macios.Generator.Tests.DataModel;

public class PropertyTests : BaseGeneratorTestClass {
	[Fact]
	public void CompareDiffName ()
	{
		var x = new Property ("First", "string", false, false, new (), [], [], []);
		var y = new Property ("Second", "string", false, false, new (), [], [], []);

		Assert.False (x.Equals (y));
		Assert.False (y.Equals (x));
		Assert.False (x == y);
		Assert.True (x != y);
	}

	[Fact]
	public void CompareDiffType ()
	{
		var x = new Property ("First", "string", false, false, new (), [], [], []);
		var y = new Property ("First", "int", false, false, new (), [], [], []);

		Assert.False (x.Equals (y));
		Assert.False (y.Equals (x));
		Assert.False (x == y);
		Assert.True (x != y);
	}

	[Fact]
	public void CompareDiffAttrs ()
	{
		var x = new Property ("First", "string", false, false, new (), [
			new ("Attr1"),
			new ("Attr2"),
		], [], []);
		var y = new Property ("First", "int", false, false, new (), [
			new ("Attr2"),
		], [], []);

		Assert.False (x.Equals (y));
		Assert.False (y.Equals (x));
		Assert.False (x == y);
		Assert.True (x != y);
	}

	[Fact]
	public void CompareDiffModifiers ()
	{
		var x = new Property ("First", "string", false, false, new (), [
			new ("Attr1"),
			new ("Attr2"),
		], [
			SyntaxFactory.Token (SyntaxKind.AbstractKeyword)
		], []);
		var y = new Property ("First", "int", false, false, new (), [
			new ("Attr1"),
			new ("Attr2"),
		], [
			SyntaxFactory.Token (SyntaxKind.PublicKeyword)
		], []);

		Assert.False (x.Equals (y));
		Assert.False (y.Equals (x));
		Assert.False (x == y);
		Assert.True (x != y);
	}

	[Fact]
	public void CompareDiffAccessors ()
	{
		var x = new Property ("First", "string", false, false, new (), [
			new ("Attr1"),
			new ("Attr2"),
		], [
			SyntaxFactory.Token (SyntaxKind.PublicKeyword)
		], [
			new (AccessorKind.Getter, new (), [], []),
			new (AccessorKind.Setter, new (), [], []),
		]);
		var y = new Property ("First", "int", false, false, new (), [
			new ("Attr1"),
			new ("Attr2"),
		], [
			SyntaxFactory.Token (SyntaxKind.PublicKeyword)
		], [
			new (AccessorKind.Getter, new (), [], []),
		]);

		Assert.False (x.Equals (y));
		Assert.False (y.Equals (x));
		Assert.False (x == y);
		Assert.True (x != y);
	}

	[Fact]
	public void CompareEquals ()
	{
		var x = new Property ("First", "string", false, false, new (), [
			new ("Attr1"),
			new ("Attr2"),
		], [
			SyntaxFactory.Token (SyntaxKind.PublicKeyword)
		], [
			new (AccessorKind.Getter, new (), [], []),
			new (AccessorKind.Setter, new (), [], []),
		]);
		var y = new Property ("First", "string", false, false, new (), [
			new ("Attr1"),
			new ("Attr2"),
		], [
			SyntaxFactory.Token (SyntaxKind.PublicKeyword)
		], [
			new (AccessorKind.Getter, new (), [], []),
			new (AccessorKind.Setter, new (), [], []),
		]);

		Assert.True (x.Equals (y));
		Assert.True (y.Equals (x));
		Assert.True (x == y);
		Assert.False (x != y);
	}

	[Fact]
	public void IsNotification ()
	{
		var x = new Property (
			name: "First",
			type: "string",
			isBlittable: false,
			isSmartEnum: false,
			symbolAvailability: new (), attributes: [
				new ("Attr1"),
				new ("Attr2"),
			], modifiers: [
				SyntaxFactory.Token (SyntaxKind.PublicKeyword)
			], accessors: [
				new (AccessorKind.Getter, new (), [], []),
				new (AccessorKind.Setter, new (), [], []),
			]
		) {
			ExportFieldData = null
		};

		Assert.False (x.IsNotification);

		x = new Property (
			name: "First",
			type: "string",
			isBlittable: false,
			isSmartEnum: false,
			symbolAvailability: new (), attributes: [
				new ("Attr1"),
				new ("Attr2"),
			], modifiers: [
				SyntaxFactory.Token (SyntaxKind.PublicKeyword)
			], accessors: [
				new (AccessorKind.Getter, new (), [], []),
				new (AccessorKind.Setter, new (), [], []),
			]
		) {
			ExportFieldData = new ExportData<Field> ("name", ArgumentSemantic.None, Field.Notification)
		};
	}

	class TestDataFromPropertyDeclaration : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			const string automaticGetter = @"
using System;

namespace Test;

public class TestClass {

	public string Name { get; }
}
";
			yield return [
				automaticGetter,
				new Property (
					name: "Name",
					type: "string",
					isBlittable: false,
					isSmartEnum: false,
					symbolAvailability: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (AccessorKind.Getter, new (), [], [])
					])
			];

			const string notificationProperty = @"
using System;
using ObjCBindings;

namespace Test;

public class TestClass {

	[Export<Field>(""name"", Flags = Field.Notification)]
	public string Name { get; }
}
";
			yield return [
				notificationProperty,
				new Property (
					name: "Name",
					type: "string",
					isBlittable: false,
					isSmartEnum: false,
					symbolAvailability: new (),
					attributes: [
						new ("ObjCBindings.ExportAttribute<ObjCBindings.Field>", ["name", "ObjCBindings.Field.Notification"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (AccessorKind.Getter, new (), [], [])
					]) {
					ExportFieldData = new ("name", ArgumentSemantic.None, Field.Notification)
				}
			];

			const string automaticGetterSetter = @"
using System;

namespace Test;

public class TestClass {

	internal string Name { get; set; }
}
";

			yield return [
				automaticGetterSetter,
				new Property (
					name: "Name",
					type: "string",
					isBlittable: false,
					isSmartEnum: false,
					symbolAvailability: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.InternalKeyword),
					],
					accessors: [
						new (AccessorKind.Getter, new (), [], []),
						new (AccessorKind.Setter, new (), [], [])
					])
			];

			const string manualGetter = @"
namespace Test;

public class TestClass {
	const string name = ""Test"";
	public string Name { get { return name; } }
}
";

			yield return [
				manualGetter,
				new Property (
					name: "Name",
					type: "string",
					isBlittable: false,
					isSmartEnum: false,
					symbolAvailability: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (AccessorKind.Getter, new (), [], []),
					])
			];

			const string expressionGetter = @"
namespace Test;

public class TestClass {
	const string name = ""Test"";
	public string Name => name;
}
";

			yield return [
				expressionGetter,
				new Property (
					name: "Name",
					type: "string",
					isBlittable: false,
					isSmartEnum: false,
					symbolAvailability: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (AccessorKind.Getter, new (), [], []),
					])
			];

			const string expressionGetterSetter = @"
namespace Test;

public class TestClass {
	const string name = ""Test"";
	public string Name {
		get => name;
		set => name = value;
	}
}
";
			yield return [
				expressionGetterSetter,
				new Property (
					name: "Name",
					type: "string",
					isBlittable: false,
					isSmartEnum: false,
					symbolAvailability: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (AccessorKind.Getter, new (), [], []),
						new (AccessorKind.Setter, new (), [], []),
					])
			];

			const string manualGetterSetter = @"
namespace Test;

public class TestClass {
	const string name = ""Test"";
	public string Name {
		get { return name; }
		set { name = value; }
	}
}
";

			yield return [
				manualGetterSetter,
				new Property (
					name: "Name",
					type: "string",
					isBlittable: false,
					isSmartEnum: false,
					symbolAvailability: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (AccessorKind.Getter, new (), [], []),
						new (AccessorKind.Setter, new (), [], []),
					])
			];

			const string internalSetter = @"
namespace Test;

public class TestClass {
	const string name = ""Test"";
	public string Name {
		get { return name; }
		internal set { name = value; }
	}
}
";

			yield return [
				internalSetter,
				new Property (
					name: "Name",
					type: "string",
					isBlittable: false,
					isSmartEnum: false,
					symbolAvailability: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (AccessorKind.Getter, new (), [], []),
						new (AccessorKind.Setter, new (), [], [
							SyntaxFactory.Token (SyntaxKind.InternalKeyword),
						]),
					])
			];

			const string propertyWithAttribute = @"
using System.Runtime.Versioning;
namespace Test;

public class TestClass {
	const string name = ""Test"";

	[SupportedOSPlatform (""ios"")]
	public string Name {
		get { return name; }
		set { name = value; }
	}
}
";

			var propertyAvailabilityBuilder = SymbolAvailability.CreateBuilder ();
			propertyAvailabilityBuilder.Add (new SupportedOSPlatformData ("ios"));

			yield return [
				propertyWithAttribute,
				new Property (
					name: "Name",
					type: "string",
					isBlittable: false,
					isSmartEnum: false,
					symbolAvailability: propertyAvailabilityBuilder.ToImmutable (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (AccessorKind.Getter, new (), [], []),
						new (AccessorKind.Setter, new (), [], []),
					])
			];

			const string propertyGetterWithAttribute = @"
using System.Runtime.Versioning;
namespace Test;

public class TestClass {
	const string name = ""Test"";

	[SupportedOSPlatform (""ios"")]
	public string Name {
		[SupportedOSPlatform (""ios17.0"")]
		get { return name; }
		set { name = value; }
	}
}
";

			var getterAvailabilityBuilder = SymbolAvailability.CreateBuilder ();
			var setterAvailabilityBuilder = SymbolAvailability.CreateBuilder ();
			getterAvailabilityBuilder.Add (new SupportedOSPlatformData ("ios17.0"));
			yield return [
				propertyGetterWithAttribute,
				new Property (
					name: "Name",
					type: "string",
					isBlittable: false,
					isSmartEnum: false,
					symbolAvailability: propertyAvailabilityBuilder.ToImmutable (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (AccessorKind.Getter, getterAvailabilityBuilder.ToImmutable (), [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
						], []),
						new (AccessorKind.Setter, new (), [], []),
					])
			];

			const string propertyWithGetterAndSetterWithAttribute = @"
using System.Runtime.Versioning;
namespace Test;

public class TestClass {
	const string name = ""Test"";

	[SupportedOSPlatform (""ios"")]
	public string Name {
		[SupportedOSPlatform (""ios17.0"")]
		get { return name; }
		[SupportedOSPlatform (""ios18.0"")]
		set { name = value; }
	}
}
";

			getterAvailabilityBuilder.Clear ();
			setterAvailabilityBuilder.Clear ();
			getterAvailabilityBuilder.Add (new SupportedOSPlatformData ("ios17.0"));
			setterAvailabilityBuilder.Add (new SupportedOSPlatformData ("ios18.0"));

			yield return [
				propertyWithGetterAndSetterWithAttribute,
				new Property (
					name: "Name",
					type: "string",
					isBlittable: false,
					isSmartEnum: false,
					symbolAvailability: propertyAvailabilityBuilder.ToImmutable (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (AccessorKind.Getter, getterAvailabilityBuilder.ToImmutable (), [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
						], []),
						new (AccessorKind.Setter, setterAvailabilityBuilder.ToImmutable (), [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios18.0"]),
						], []),
					])
			];

			const string propertyWithCustomType = @"
using System.Runtime.Versioning;

namespace Utils {
	public class MyClass {}
}

namespace Test {
	public class TestClass {
		Utils.MyClass name = new (); 

		[SupportedOSPlatform (""ios"")]
		public Utils.MyClass Name {
			[SupportedOSPlatform (""ios17.0"")]
			get { return name; }
			[SupportedOSPlatform (""ios18.0"")]
			set { name = value; }
		}
	}
}
";
			getterAvailabilityBuilder.Clear ();
			setterAvailabilityBuilder.Clear ();
			getterAvailabilityBuilder.Add (new SupportedOSPlatformData ("ios17.0"));
			setterAvailabilityBuilder.Add (new SupportedOSPlatformData ("ios18.0"));

			yield return [
				propertyWithCustomType,
				new Property (
					name: "Name",
					type: "Utils.MyClass",
					isBlittable: false,
					isSmartEnum: false,
					symbolAvailability: propertyAvailabilityBuilder.ToImmutable (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (AccessorKind.Getter, getterAvailabilityBuilder.ToImmutable (), [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
						], []),
						new (AccessorKind.Setter, setterAvailabilityBuilder.ToImmutable (), [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios18.0"]),
						], []),
					])
			];
		}

		IEnumerator IEnumerable.GetEnumerator ()
			=> GetEnumerator ();
	}

	[Theory]
	[AllSupportedPlatformsClassData<TestDataFromPropertyDeclaration>]
	void FromPropertyDeclaration (ApplePlatform platform, string inputText, Property expected)
	{
		var (compilation, syntaxTrees) = CreateCompilation (platform, sources: inputText);
		Assert.Single (syntaxTrees);
		var semanticModel = compilation.GetSemanticModel (syntaxTrees [0]);
		var declaration = syntaxTrees [0].GetRoot ()
			.DescendantNodes ().OfType<PropertyDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (declaration);
		Assert.True (Property.TryCreate (declaration, semanticModel, out var changes));
		Assert.NotNull (changes);
		Assert.Equal (expected, changes);
	}
}
