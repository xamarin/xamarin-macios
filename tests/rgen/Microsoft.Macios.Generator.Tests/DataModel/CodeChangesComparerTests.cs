using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.Availability;
using Microsoft.Macios.Generator.DataModel;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.DataModel;

public class CodeChangesComparerTests : BaseGeneratorTestClass {
	readonly CodeChangesEqualityComparer comparer = new ();

	[Fact]
	public void CompareDifferentFullyQualifiedSymbol ()
	{
		var changes1 = new CodeChanges (
			bindingData: new (BindingType.SmartEnum, new ()),
			name: "name1",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name1",
			symbolAvailability: new ());
		var changes2 = new CodeChanges (
			bindingData: new (BindingType.SmartEnum, new ()),
			name: "name1",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name2",
			symbolAvailability: new ());
		Assert.False (comparer.Equals (changes1, changes2));
	}

	[Fact]
	public void CompareDifferentBase ()
	{
		var changes1 = new CodeChanges (
			bindingData: new (BindingType.SmartEnum, new ()),
			name: "name1",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name1",
			symbolAvailability: new ()) {
			Base = "Base1"
		};
		var changes2 = new CodeChanges (
			bindingData: new (BindingType.SmartEnum, new ()),
			name: "name1",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name1",
			symbolAvailability: new ()) {
			Base = "Base2"
		};
		Assert.False (comparer.Equals (changes1, changes2));
	}

	[Fact]
	public void CompareDifferentInterface ()
	{
		var changes1 = new CodeChanges (
			bindingData: new (BindingType.SmartEnum, new ()),
			name: "name1",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name1",
			symbolAvailability: new ()) {
			Interfaces = ["IBase1"]
		};
		var changes2 = new CodeChanges (
			bindingData: new (BindingType.SmartEnum, new ()),
			name: "name1",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name1",
			symbolAvailability: new ()) {
			Interfaces = ["IBase1", "IBase2"],
		};
		Assert.False (comparer.Equals (changes1, changes2));
	}

	[Fact]
	public void CompareDifferentNameSymbol ()
	{
		var changes1 = new CodeChanges (
			bindingData: new (BindingType.SmartEnum, new ()),
			name: "name1",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name1",
			symbolAvailability: new ());
		var changes2 = new CodeChanges (
			bindingData: new (BindingType.SmartEnum, new ()),
			name: "name2",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name1",
			symbolAvailability: new ());
		Assert.False (comparer.Equals (changes1, changes2));
	}

	[Fact]
	public void CompareDifferentNamespaceSymbol ()
	{
		var changes1 = new CodeChanges (
			bindingData: new (BindingType.SmartEnum, new ()),
			name: "name1",
			@namespace: ["NS1"],
			fullyQualifiedSymbol: "NS.name1",
			symbolAvailability: new ());
		var changes2 = new CodeChanges (
			bindingData: new (BindingType.SmartEnum, new ()),
			name: "name1",
			@namespace: ["NS2"],
			fullyQualifiedSymbol: "NS.name1",
			symbolAvailability: new ());
		Assert.False (comparer.Equals (changes1, changes2));
	}

	[Fact]
	public void CompareDifferentBindingType ()
	{
		var changes1 = new CodeChanges (
			bindingData: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: new ());
		var changes2 = new CodeChanges (
			bindingData: new (BindingType.Protocol, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: new ());
		Assert.False (comparer.Equals (changes1, changes2));
	}

	[Fact]
	public void CompareDifferentAttributesLength ()
	{
		var changes1 = new CodeChanges (
			bindingData: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: new ());
		var changes2 = new CodeChanges (
			bindingData: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: new ()) {
			Attributes = [
				new AttributeCodeChange (name: "name", arguments: ["arg1", "arg2"])
			]
		};
		Assert.False (comparer.Equals (changes1, changes2));
	}

	[Fact]
	public void CompareDifferentAttributes ()
	{
		var changes1 = new CodeChanges (
			bindingData: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: new ()) {
			Attributes = [
				new AttributeCodeChange (name: "name", arguments: ["arg1", "arg2"])
			],
		};
		var changes2 = new CodeChanges (
			bindingData: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: new ()) {
			Attributes = [
				new AttributeCodeChange (name: "name2", arguments: ["arg1", "arg2"])
			],
		};
		Assert.False (comparer.Equals (changes1, changes2));
	}

	[Fact]
	public void CompareDifferentMembersLength ()
	{
		var changes1 = new CodeChanges (
			bindingData: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: new ());
		var changes2 = new CodeChanges (
			bindingData: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: new ()) {
			EnumMembers = [
				new EnumMember (name: "name", fieldData: new (), symbolAvailability: new (), attributes: [])
			],
		};
		Assert.False (comparer.Equals (changes1, changes2));
	}

	[Fact]
	public void CompareDifferentMembers ()
	{
		var changes1 = new CodeChanges (
			bindingData: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: new ()) {
			EnumMembers = [
				new EnumMember (name: "name", fieldData: new (), symbolAvailability: new (), attributes: [])
			],
		};
		var changes2 = new CodeChanges (
			bindingData: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: new ()) {
			EnumMembers = [
				new EnumMember (name: "name2", fieldData: new (), symbolAvailability: new (), attributes: [])
			],
		};
		Assert.False (comparer.Equals (changes1, changes2));
	}

	[Fact]
	public void CompareDifferentPropertyLength ()
	{
		var changes1 = new CodeChanges (
			bindingData: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: new ()) {
			EnumMembers = [],
			Properties = []
		};
		var changes2 = new CodeChanges (
			bindingData: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: new ()) {
			EnumMembers = [],
			Properties = [
				new (
					name: "Name",
					type: "Utils.MyClass",
					isSmartEnum: false,
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
		var changes1 = new CodeChanges (
			bindingData: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"], fullyQualifiedSymbol: "NS.name", symbolAvailability: new ()) {
			EnumMembers = [],
			Properties = [
				new (
					name: "Surname",
					type: "string",
					isSmartEnum: false,
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
					isSmartEnum: false,
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
		var changes2 = new CodeChanges (
			bindingData: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: new ()) {
			EnumMembers = [],
			Properties = [
				new (
					name: "Name",
					type: "Utils.MyClass",
					isSmartEnum: false,
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
					isSmartEnum: false,
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
		var changes1 = new CodeChanges (
			bindingData: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: new ()) {
			EnumMembers = [],
			Properties = [
				new (
					name: "Surname",
					type: "string",
					isSmartEnum: false,
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
					isSmartEnum: false,
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
		var changes2 = new CodeChanges (
			bindingData: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: new ()) {
			EnumMembers = [],
			Properties = [
				new (
					name: "Name",
					type: "Utils.MyClass",
					isSmartEnum: false,
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
					isSmartEnum: false,
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
		var changes1 = new CodeChanges (
			bindingData: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: new ()) {
			EnumMembers = [],
			Properties = [
				new (
					name: "Surname",
					type: "string",
					isSmartEnum: false,
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
					isSmartEnum: false,
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
		var changes2 = new CodeChanges (
			bindingData: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: new ()) {
			EnumMembers = [],
			Properties = [
				new (
					name: "Name",
					type: "Utils.MyClass",
					isSmartEnum: false,
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
					isSmartEnum: false,
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
		var changes1 = new CodeChanges (
			bindingData: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: new ()) {
			EnumMembers = [],
			Properties = [
				new (
					name: "Surname",
					type: "string",
					isSmartEnum: false,
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
					isSmartEnum: false,
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
		var changes2 = new CodeChanges (
			bindingData: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: new ()) {
			EnumMembers = [],
			Properties = [
				new (
					name: "Name",
					type: "Utils.MyClass",
					isSmartEnum: false,
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
					isSmartEnum: false,
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
		var changes1 = new CodeChanges (
			bindingData: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: new ()) {
			EnumMembers = [],
			Properties = [
				new (
					name: "Surname",
					type: "string",
					isSmartEnum: false,
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
					isSmartEnum: false,
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
		var changes2 = new CodeChanges (
			bindingData: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: new ()) {
			EnumMembers = [],
			Properties = [
				new (
					name: "Name",
					type: "Utils.MyClass",
					isSmartEnum: false,
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
					isSmartEnum: false,
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
		var changes1 = new CodeChanges (
			bindingData: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: new ()) {
			EnumMembers = [],
			Properties = [
				new (
					name: "Surname",
					type: "string",
					isSmartEnum: false,
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
					isSmartEnum: false,
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
		var changes2 = new CodeChanges (
			bindingData: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: new ()) {
			EnumMembers = [],
			Properties = [
				new (
					name: "Name",
					type: "Utils.MyClass",
					isSmartEnum: false,
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
					isSmartEnum: false,
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
		var changes1 = new CodeChanges (
			bindingData: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: new ()) {
			EnumMembers = [],
			Properties = [
				new (
					name: "Surname",
					type: "string",
					isSmartEnum: false,
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
					isSmartEnum: false,
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
		var changes2 = new CodeChanges (
			bindingData: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: new ()) {
			EnumMembers = [],
			Properties = [
				new (
					name: "Name",
					type: "Utils.MyClass",
					isSmartEnum: false,
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
					isSmartEnum: false,
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
		var changes1 = new CodeChanges (
			bindingData: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: new ()) {
			EnumMembers = [],
			Properties = [
				new (
					name: "Surname",
					type: "string",
					isSmartEnum: false,
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
					isSmartEnum: false,
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
		var changes2 = new CodeChanges (
			bindingData: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: new ()) {
			EnumMembers = [],
			Properties = [
				new (
					name: "Name",
					type: "Utils.MyClass",
					isSmartEnum: false,
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
					isSmartEnum: false,
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
	public void CompareSameMethodsDiffAvailability ()
	{
		var builder = SymbolAvailability.CreateBuilder ();
		builder.Add (new SupportedOSPlatformData ("ios"));
		builder.Add (new SupportedOSPlatformData ("tvos"));
		var changes1 = new CodeChanges (
			bindingData: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: builder.ToImmutable ()) {
			EnumMembers = [],
			Properties = [
				new (
					name: "Surname",
					type: "string",
					isSmartEnum: false,
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
					isSmartEnum: false,
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
		var changes2 = new CodeChanges (
			bindingData: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: new ()) {
			EnumMembers = [],
			Properties = [
				new (
					name: "Name",
					type: "Utils.MyClass",
					isSmartEnum: false,
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
					isSmartEnum: false,
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

		Assert.False (comparer.Equals (changes1, changes2));
	}

	[Fact]
	public void CompareSameMethodsSameAvailability ()
	{
		var builder = SymbolAvailability.CreateBuilder ();
		builder.Add (new SupportedOSPlatformData ("ios"));
		builder.Add (new SupportedOSPlatformData ("tvos"));
		var changes1 = new CodeChanges (
			bindingData: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: builder.ToImmutable ()) {
			EnumMembers = [],
			Properties = [
				new (
					name: "Surname",
					type: "string",
					isSmartEnum: false,
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
					isSmartEnum: false,
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
		var changes2 = new CodeChanges (
			bindingData: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: builder.ToImmutable ()) {
			EnumMembers = [],
			Properties = [
				new (
					name: "Name",
					type: "Utils.MyClass",
					isSmartEnum: false,
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
					isSmartEnum: false,
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
}
