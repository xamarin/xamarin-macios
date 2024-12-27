using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Macios.Generator.DataModel;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.DataModel;

public class CodeChangesEqualityComparerTests : BaseGeneratorTestClass {
	readonly CodeChangesEqualityComparer equalityComparer = new ();

	[Fact]
	public void CompareDifferentFullyQualifiedSymbol ()
	{
		var changes1 = new CodeChanges (BindingType.SmartEnum, "name1", ["NS"], "NS.name1"); ;
		var changes2 = new CodeChanges (BindingType.SmartEnum, "name2", ["NS"], "NS.name2"); ;
		Assert.False (equalityComparer.Equals (changes1, changes2));
	}

	[Fact]
	public void CompareDifferentBindingType ()
	{
		var changes1 = new CodeChanges (BindingType.SmartEnum, "name", ["NS"], "NS.name");
		var changes2 = new CodeChanges (BindingType.Unknown, "name", ["NS"], "NS.name");
		Assert.False (equalityComparer.Equals (changes1, changes2));
	}

	[Fact]
	public void CompareDifferentAttributesLength ()
	{
		var changes1 = new CodeChanges (BindingType.SmartEnum, "name", ["NS"], "NS.name");
		var changes2 = new CodeChanges (BindingType.SmartEnum, "name", ["NS"], "NS.name") {
			Attributes = [
				new AttributeCodeChange ("name", ["arg1", "arg2"])
			]
		};
		Assert.False (equalityComparer.Equals (changes1, changes2));
	}

	[Fact]
	public void CompareDifferentAttributes ()
	{
		var changes1 = new CodeChanges (BindingType.SmartEnum, "name", ["NS"], "NS.name") {
			Attributes = [
				new AttributeCodeChange ("name", ["arg1", "arg2"])
			],
		};
		var changes2 = new CodeChanges (BindingType.SmartEnum, "name", ["NS"], "NS.name") {
			Attributes = [
				new AttributeCodeChange ("name2", ["arg1", "arg2"])
			],
		};
		Assert.False (equalityComparer.Equals (changes1, changes2));
	}

	[Fact]
	public void CompareDifferentMembersLength ()
	{
		var changes1 = new CodeChanges (BindingType.SmartEnum, "name", ["NS"], "NS.name");
		var changes2 = new CodeChanges (BindingType.SmartEnum, "name", ["NS"], "NS.name") {
			EnumMembers = [
				new EnumMember ("name", new (), new (), [])
			],
		};
		Assert.False (equalityComparer.Equals (changes1, changes2));
	}

	[Fact]
	public void CompareDifferentMembers ()
	{
		var changes1 = new CodeChanges (BindingType.SmartEnum, "name", ["NS"], "NS.name") {
			EnumMembers = [
				new EnumMember ("name", new (), new (), [])
			],
		};
		var changes2 = new CodeChanges (BindingType.SmartEnum, "name", ["NS"], "NS.name") {
			EnumMembers = [
				new EnumMember ("name2", new (), new (), [])
			],
		};
		Assert.False (equalityComparer.Equals (changes1, changes2));
	}

	[Fact]
	public void CompareDifferentPropertyLength ()
	{
		var changes1 = new CodeChanges (BindingType.SmartEnum, "name", ["NS"], "NS.name") { EnumMembers = [], Properties = [] };
		var changes2 = new CodeChanges (BindingType.SmartEnum, "name", ["NS"], "NS.name") {
			EnumMembers = [],
			Properties = [
				new (
					name: "Name",
					type: "Utils.MyClass",
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (AccessorKind.Getter, new (), [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
						], []),
						new (AccessorKind.Setter, new (), [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios18.0"]),
						], []),
					])
			]
		};

		Assert.False (equalityComparer.Equals (changes1, changes2));
	}

	[Fact]
	public void CompareSamePropertiesDiffOrder ()
	{
		var changes1 = new CodeChanges (BindingType.SmartEnum, "name", ["NS"], "NS.name") {
			EnumMembers = [],
			Properties = [
				new (
					name: "Surname",
					type: "string",
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (AccessorKind.Getter, new (), [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
						], []),
						new (AccessorKind.Setter, new (), [], []),
					]),
				new (
					name: "Name",
					type: "Utils.MyClass",
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (AccessorKind.Getter, new (), [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
						], []),
						new (AccessorKind.Setter, new (), [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios18.0"]),
						], []),
					]),
			]
		};
		var changes2 = new CodeChanges (BindingType.SmartEnum, "name", ["NS"], "NS.name") {
			EnumMembers = [],
			Properties = [
				new (
					name: "Name",
					type: "Utils.MyClass",
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (AccessorKind.Getter, new (), [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
						], []),
						new (AccessorKind.Setter, new (), [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios18.0"]),
						], []),
					]),
				new (
					name: "Surname",
					type: "string",
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (AccessorKind.Getter, new (), [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
						], []),
						new (AccessorKind.Setter, new (), [], []),
					]),
			]
		};
		Assert.True (equalityComparer.Equals (changes1, changes2));
	}

	[Fact]
	public void CompareDifferentProperties ()
	{
		var changes1 = new CodeChanges (BindingType.SmartEnum, "name", ["NS"], "NS.name") {
			EnumMembers = [],
			Properties = [
				new (
					name: "Surname",
					type: "string",
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (AccessorKind.Getter, new (), [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
						], []),
						new (AccessorKind.Setter, new (), [], []),
					]),
				new (
					name: "Name",
					type: "string",
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (AccessorKind.Getter, new (), [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
						], []),
						new (AccessorKind.Setter, new (), [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios18.0"]),
						], []),
					]),
			]
		};
		var changes2 = new CodeChanges (BindingType.SmartEnum, "name", ["NS"], "NS.name") {
			EnumMembers = [],
			Properties = [
				new (
					name: "Name",
					type: "Utils.MyClass",
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (AccessorKind.Getter, new (), [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
						], []),
						new (AccessorKind.Setter, new (), [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios18.0"]),
						], []),
					]),
				new (
					name: "Surname",
					type: "string",
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (AccessorKind.Getter, new (), [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
						], []),
						new (AccessorKind.Setter, new (), [], []),
					]),
			]
		};
		Assert.False (equalityComparer.Equals (changes1, changes2));
	}

	[Fact]
	public void CompareDifferentConstructorLength ()
	{
		var changes1 = new CodeChanges (BindingType.SmartEnum, "name", ["NS"], "NS.name") {
			EnumMembers = [],
			Properties = [
				new (
					name: "Surname",
					type: "string",
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (AccessorKind.Getter, new (), [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
						], []),
						new (AccessorKind.Setter, new (), [], []),
					]),
				new (
					name: "Name",
					type: "Utils.MyClass",
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (AccessorKind.Getter, new (), [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
						], []),
						new (AccessorKind.Setter, new (), [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios18.0"]),
						], []),
					]),
			],
			Constructors = [],
		};
		var changes2 = new CodeChanges (BindingType.SmartEnum, "name", ["NS"], "NS.name") {
			EnumMembers = [],
			Properties = [
				new (
					name: "Name",
					type: "Utils.MyClass",
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (AccessorKind.Getter, new (), [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
						], []),
						new (AccessorKind.Setter, new (), [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios18.0"]),
						], []),
					]),
				new (
					name: "Surname",
					type: "string",
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (AccessorKind.Getter, new (), [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
						], []),
						new (AccessorKind.Setter, new (), [], []),
					]),
			],
			Constructors = [
				new ("MyClass", new (), [], [], [])
			],
		};
		Assert.False (equalityComparer.Equals (changes1, changes2));
	}

	[Fact]
	public void CompareDifferentConstructors ()
	{
		var changes1 = new CodeChanges (BindingType.SmartEnum, "name", ["NS"], "NS.name") {
			EnumMembers = [],
			Properties = [
				new (
					name: "Surname",
					type: "string",
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (AccessorKind.Getter, new (), [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
						], []),
						new (AccessorKind.Setter, new (), [], []),
					]),
				new (
					name: "Name",
					type: "Utils.MyClass",
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (AccessorKind.Getter, new (), [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
						], []),
						new (AccessorKind.Setter, new (), [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios18.0"]),
						], []),
					]),
			],
			Constructors = [
				new ("MyClass", new (), [], [], [])
			],
		};
		var changes2 = new CodeChanges (BindingType.SmartEnum, "name", ["NS"], "NS.name") {
			EnumMembers = [],
			Properties = [
				new (
					name: "Name",
					type: "Utils.MyClass",
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (AccessorKind.Getter, new (), [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
						], []),
						new (AccessorKind.Setter, new (), [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios18.0"]),
						], []),
					]),
				new (
					name: "Surname",
					type: "string",
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (AccessorKind.Getter, new (), [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
						], []),
						new (AccessorKind.Setter, new (), [], []),
					]),
			],
			Constructors = [
				new ("MyClass",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [],
					parameters: [
						new (0, "string", "name"),
					])
			],
		};
		Assert.False (equalityComparer.Equals (changes1, changes2));
	}

	[Fact]
	public void CompareSameConstructors ()
	{
		var changes1 = new CodeChanges (BindingType.SmartEnum, "name", ["NS"], "NS.name") {
			EnumMembers = [],
			Properties = [
				new (
					name: "Surname",
					type: "string",
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (AccessorKind.Getter, new (), [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
						], []),
						new (AccessorKind.Setter, new (), [], []),
					]),
				new (
					name: "Name",
					type: "Utils.MyClass",
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (AccessorKind.Getter, new (), [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
						], []),
						new (AccessorKind.Setter, new (), [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios18.0"]),
						], []),
					]),
			],
			Constructors = [
				new Constructor ("MyClass", new (), [], [], []),
				new ("MyClass",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [],
					parameters: [
						new (0, "string", "name"),
					])
			],
		};
		var changes2 = new CodeChanges (BindingType.SmartEnum, "name", ["NS"], "NS.name") {
			EnumMembers = [],
			Properties = [
				new (
					name: "Name",
					type: "Utils.MyClass",
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (AccessorKind.Getter, new (), [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
						], []),
						new (AccessorKind.Setter, new (), [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios18.0"]),
						], []),
					]),
				new (
					name: "Surname",
					type: "string",
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (AccessorKind.Getter, new (), [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
						], []),
						new (AccessorKind.Setter, new (), [], []),
					]),
			],
			Constructors = [
				new Constructor ("MyClass", new (), [], [], []),
				new ("MyClass",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [],
					parameters: [
						new (0, "string", "name"),
					])
			],
		};
		Assert.True (equalityComparer.Equals (changes1, changes2));
	}

	[Fact]
	public void CompareSameConstructorsDiffOrder ()
	{
		var changes1 = new CodeChanges (BindingType.SmartEnum, "name", ["NS"], "NS.name") {
			EnumMembers = [],
			Properties = [
				new (
					name: "Surname",
					type: "string",
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (AccessorKind.Getter, new (), [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
						], []),
						new (AccessorKind.Setter, new (), [], []),
					]),
				new (
					name: "Name",
					type: "Utils.MyClass",
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (AccessorKind.Getter, new (), [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
						], []),
						new (AccessorKind.Setter, new (), [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios18.0"]),
						], []),
					]),
			],
			Constructors = [
				new ("MyClass",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [],
					parameters: [
						new (0, "string", "name"),
					]),
				new ("MyClass", new (), [], [], []),
			],
		};
		var changes2 = new CodeChanges (BindingType.SmartEnum, "name", ["NS"], "NS.name") {
			EnumMembers = [],
			Properties = [
				new (
					name: "Name",
					type: "Utils.MyClass",
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (AccessorKind.Getter, new (), [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
						], []),
						new (AccessorKind.Setter, new (), [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios18.0"]),
						], []),
					]),
				new (
					name: "Surname",
					type: "string",
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (AccessorKind.Getter, new (), [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
						], []),
						new (AccessorKind.Setter, new (), [], []),
					]),
			],
			Constructors = [
				new ("MyClass", new (), [], [], []),
				new ("MyClass",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [],
					parameters: [
						new (0, "string", "name"),
					]),
			],
		};
		Assert.True (equalityComparer.Equals (changes1, changes2));
	}
}
