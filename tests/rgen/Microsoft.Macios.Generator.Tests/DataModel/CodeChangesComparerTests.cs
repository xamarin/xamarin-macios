using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.DataModel;
using Xamarin.Tests;
using Xamarin.Utils;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.DataModel;

public class CodeChangesComparerTests : BaseGeneratorTestClass {
	readonly CodeChangesEqualityComparer comparer = new ();

	[Fact]
	public void CompareDifferentFullyQualifiedSymbol ()
	{
		var changes1 = new CodeChanges (BindingType.SmartEnum, "name1");
		var changes2 = new CodeChanges (BindingType.SmartEnum, "name2");
		Assert.False (comparer.Equals (changes1, changes2));
	}

	[Fact]
	public void CompareDifferentBindingType ()
	{
		var changes1 = new CodeChanges (BindingType.SmartEnum, "name");
		var changes2 = new CodeChanges (BindingType.Unknown, "name");
		Assert.False (comparer.Equals (changes1, changes2));
	}

	[Fact]
	public void CompareDifferentAttributesLength ()
	{
		var changes1 = new CodeChanges (BindingType.SmartEnum, "name");
		var changes2 = new CodeChanges (BindingType.SmartEnum, "name") {
			Attributes = [
				new AttributeCodeChange ("name", ["arg1", "arg2"])
			]
		};
		Assert.False (comparer.Equals (changes1, changes2));
	}

	[Fact]
	public void CompareDifferentAttributes ()
	{
		var changes1 = new CodeChanges (BindingType.SmartEnum, "name") {
			Attributes = [
				new AttributeCodeChange ("name", ["arg1", "arg2"])
			],
		};
		var changes2 = new CodeChanges (BindingType.SmartEnum, "name") {
			Attributes = [
				new AttributeCodeChange ("name2", ["arg1", "arg2"])
			],
		};
		Assert.False (comparer.Equals (changes1, changes2));
	}

	[Fact]
	public void CompareDifferentMembersLength ()
	{
		var changes1 = new CodeChanges (BindingType.SmartEnum, "name");
		var changes2 = new CodeChanges (BindingType.SmartEnum, "name") {
			EnumMembers = [
				new EnumMember ("name", new (), [])
			],
		};
		Assert.False (comparer.Equals (changes1, changes2));
	}

	[Fact]
	public void CompareDifferentMembers ()
	{
		var changes1 = new CodeChanges (BindingType.SmartEnum, "name") {
			EnumMembers = [
				new EnumMember ("name", new (), [])
			],
		};
		var changes2 = new CodeChanges (BindingType.SmartEnum, "name") {
			EnumMembers = [
				new EnumMember ("name2", new (), [])
			],
		};
		Assert.False (comparer.Equals (changes1, changes2));
	}

	[Fact]
	public void CompareDifferentPropertyLength ()
	{
		var changes1 = new CodeChanges (BindingType.SmartEnum, "name") {
			EnumMembers = [],
			Properties = []
		};
		var changes2 = new CodeChanges (BindingType.SmartEnum, "name") {
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

		Assert.False (comparer.Equals (changes1, changes2));
	}

	[Fact]
	public void CompareSamePropertiesDiffOrder ()
	{
		var changes1 = new CodeChanges (BindingType.SmartEnum, "name") {
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
		var changes2 = new CodeChanges (BindingType.SmartEnum, "name") {
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
		Assert.True (comparer.Equals (changes1, changes2));
	}

	[Fact]
	public void CompareDifferentProperties ()
	{
		var changes1 = new CodeChanges (BindingType.SmartEnum, "name") {
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
		var changes2 = new CodeChanges (BindingType.SmartEnum, "name") {
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
		Assert.False (comparer.Equals (changes1, changes2));
	}

	[Fact]
	public void CompareDifferentEventsLength ()
	{
		var changes1 = new CodeChanges (BindingType.SmartEnum, "name") {
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
			Events = [
				new (
					name: "MyEvent",
					type: "System.EventHandler",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [],
					accessors: [
						new (AccessorKind.Add, new (), [], []),
					]),
			]
		};
		var changes2 = new CodeChanges (BindingType.SmartEnum, "name") {
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
		Assert.False (comparer.Equals (changes1, changes2));
	}

	[Fact]
	public void CompareSameEventsDiffOrder ()
	{
		var changes1 = new CodeChanges (BindingType.SmartEnum, "name") {
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
			Events = [
				new (
					name: "MyEvent",
					type: "System.EventHandler",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [],
					accessors: [
						new (AccessorKind.Add, new (), [], []),
					]),
				new (
					name: "MyEvent2",
					type: "System.EventHandler",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [],
					accessors: [
						new (AccessorKind.Add, new (), [], []),
						new (AccessorKind.Remove, new (), [], []),
					]),
			]
		};
		var changes2 = new CodeChanges (BindingType.SmartEnum, "name") {
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
			Events = [
				new (
					name: "MyEvent2",
					type: "System.EventHandler",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [],
					accessors: [
						new (AccessorKind.Add, new (), [], []),
						new (AccessorKind.Remove, new (), [], []),
					]),
				new (
					name: "MyEvent",
					type: "System.EventHandler",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [],
					accessors: [
						new (AccessorKind.Add, new (), [], []),
					]),
			]
		};

		Assert.True (comparer.Equals (changes1, changes2));
	}

	[Fact]
	public void CompareDifferentEvents ()
	{
		var changes1 = new CodeChanges (BindingType.SmartEnum, "name") {
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
			Events = [
				new (
					name: "MyEvent",
					type: "System.EventHandler",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [],
					accessors: [
						new (AccessorKind.Add, new (), [], []),
					]),
			]
		};
		var changes2 = new CodeChanges (BindingType.SmartEnum, "name") {
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
			Events = [
				new (
					name: "MyEvent",
					type: "System.EventHandler",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.InternalKeyword),
					],
					accessors: [
						new (AccessorKind.Add, new (), [], []),
					]),
			]
		};
		Assert.False (comparer.Equals (changes1, changes2));
	}

	[Fact]
	public void CompareDifferentMethodsLength ()
	{
		var changes1 = new CodeChanges (BindingType.SmartEnum, "name") {
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
			Events = [
				new (
					name: "MyEvent",
					type: "System.EventHandler",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [],
					accessors: [
						new (AccessorKind.Add, new (), [], []),
					]),
				new (
					name: "MyEvent2",
					type: "System.EventHandler",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [],
					accessors: [
						new (AccessorKind.Add, new (), [], []),
						new (AccessorKind.Remove, new (), [], []),
					]),
			],
			Methods = [
				new (
					type: "NS.MyClass",
					name: "TryGetString",
					returnType: "bool",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (0, "string?", "example") {
							IsNullable = true,
							ReferenceKind = ReferenceKind.Out,
						},
					]
				),
				new Method (
					type: "NS.MyClass",
					name: "MyMethod",
					returnType: "void",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (0, "NS.CustomType", "input")
					]
				)
			]
		};
		var changes2 = new CodeChanges (BindingType.SmartEnum, "name") {
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
			Events = [
				new (
					name: "MyEvent2",
					type: "System.EventHandler",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [],
					accessors: [
						new (AccessorKind.Add, new (), [], []),
						new (AccessorKind.Remove, new (), [], []),
					]),
				new (
					name: "MyEvent",
					type: "System.EventHandler",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [],
					accessors: [
						new (AccessorKind.Add, new (), [], []),
					]),
			],
			Methods = [
				new (
					type: "NS.MyClass",
					name: "TryGetString",
					returnType: "bool",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (0, "string?", "example") {
							IsNullable = true,
							ReferenceKind = ReferenceKind.Out,
						},
					]
				),
			]
		};

		Assert.False (comparer.Equals (changes1, changes2));
	}

	[Fact]
	public void CompareSameMethodsDiffOrder ()
	{
		var changes1 = new CodeChanges (BindingType.SmartEnum, "name") {
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
			Events = [
				new (
					name: "MyEvent",
					type: "System.EventHandler",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [],
					accessors: [
						new (AccessorKind.Add, new (), [], []),
					]),
				new (
					name: "MyEvent2",
					type: "System.EventHandler",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [],
					accessors: [
						new (AccessorKind.Add, new (), [], []),
						new (AccessorKind.Remove, new (), [], []),
					]),
			],
			Methods = [
				new (
					type: "NS.MyClass",
					name: "TryGetString",
					returnType: "bool",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (0, "string?", "example") {
							IsNullable = true,
							ReferenceKind = ReferenceKind.Out,
						},
					]
				),
				new Method (
					type: "NS.MyClass",
					name: "MyMethod",
					returnType: "void",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (0, "NS.CustomType", "input")
					]
				)
			]
		};
		var changes2 = new CodeChanges (BindingType.SmartEnum, "name") {
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
			Events = [
				new (
					name: "MyEvent2",
					type: "System.EventHandler",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [],
					accessors: [
						new (AccessorKind.Add, new (), [], []),
						new (AccessorKind.Remove, new (), [], []),
					]),
				new (
					name: "MyEvent",
					type: "System.EventHandler",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [],
					accessors: [
						new (AccessorKind.Add, new (), [], []),
					]),
			],
			Methods = [
				new Method (
					type: "NS.MyClass",
					name: "MyMethod",
					returnType: "void",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (0, "NS.CustomType", "input")
					]
				),
				new (
					type: "NS.MyClass",
					name: "TryGetString",
					returnType: "bool",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (0, "string?", "example") {
							IsNullable = true,
							ReferenceKind = ReferenceKind.Out,
						},
					]
				),
			]
		};

		Assert.True (comparer.Equals (changes1, changes2));
	}

	[Fact]
	public void CompareDifferentMethods ()
	{
		var changes1 = new CodeChanges (BindingType.SmartEnum, "name") {
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
			Events = [
				new (
					name: "MyEvent",
					type: "System.EventHandler",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [],
					accessors: [
						new (AccessorKind.Add, new (), [], []),
					]),
				new (
					name: "MyEvent2",
					type: "System.EventHandler",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [],
					accessors: [
						new (AccessorKind.Add, new (), [], []),
						new (AccessorKind.Remove, new (), [], []),
					]),
			],
			Methods = [
				new Method (
					type: "NS.MyClass",
					name: "MyMethod",
					returnType: "void",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (0, "NS.CustomType", "input")
					]
				),
			]
		};
		var changes2 = new CodeChanges (BindingType.SmartEnum, "name") {
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
			Events = [
				new (
					name: "MyEvent2",
					type: "System.EventHandler",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [],
					accessors: [
						new (AccessorKind.Add, new (), [], []),
						new (AccessorKind.Remove, new (), [], []),
					]),
				new (
					name: "MyEvent",
					type: "System.EventHandler",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [],
					accessors: [
						new (AccessorKind.Add, new (), [], []),
					]),
			],
			Methods = [
				new (
					type: "NS.MyClass",
					name: "TryGetString",
					returnType: "bool",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (0, "string?", "example") {
							IsNullable = true,
							ReferenceKind = ReferenceKind.Out,
						},
					]
				),
			]
		};

		Assert.False (comparer.Equals (changes1, changes2));
	}
}
