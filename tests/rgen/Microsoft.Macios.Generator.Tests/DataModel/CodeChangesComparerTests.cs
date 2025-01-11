// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
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
			bindingInfo: new (BindingType.SmartEnum, new ()),
			name: "name1",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name1",
			symbolAvailability: new ());
		var changes2 = new CodeChanges (
			bindingInfo: new (BindingType.SmartEnum, new ()),
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
			bindingInfo: new (BindingType.SmartEnum, new ()),
			name: "name1",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name1",
			symbolAvailability: new ()) {
			Base = "Base1"
		};
		var changes2 = new CodeChanges (
			bindingInfo: new (BindingType.SmartEnum, new ()),
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
			bindingInfo: new (BindingType.SmartEnum, new ()),
			name: "name1",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name1",
			symbolAvailability: new ()) {
			Interfaces = ["IBase1"]
		};
		var changes2 = new CodeChanges (
			bindingInfo: new (BindingType.SmartEnum, new ()),
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
			bindingInfo: new (BindingType.SmartEnum, new ()),
			name: "name1",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name1",
			symbolAvailability: new ());
		var changes2 = new CodeChanges (
			bindingInfo: new (BindingType.SmartEnum, new ()),
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
			bindingInfo: new (BindingType.SmartEnum, new ()),
			name: "name1",
			@namespace: ["NS1"],
			fullyQualifiedSymbol: "NS.name1",
			symbolAvailability: new ());
		var changes2 = new CodeChanges (
			bindingInfo: new (BindingType.SmartEnum, new ()),
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
			bindingInfo: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: new ());
		var changes2 = new CodeChanges (
			bindingInfo: new (BindingType.Protocol, new ()),
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
			bindingInfo: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: new ());
		var changes2 = new CodeChanges (
			bindingInfo: new (BindingType.SmartEnum, new ()),
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
			bindingInfo: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: new ()) {
			Attributes = [
				new AttributeCodeChange (name: "name", arguments: ["arg1", "arg2"])
			],
		};
		var changes2 = new CodeChanges (
			bindingInfo: new (BindingType.SmartEnum, new ()),
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
			bindingInfo: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: new ());
		var changes2 = new CodeChanges (
			bindingInfo: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: new ()) {
			EnumMembers = [
				new EnumMember (
					name: "name",
					libraryName: "Test",
					libraryPath: "/path/to/library",
					fieldData: new (),
					symbolAvailability: new (),
					attributes: [])
			],
		};
		Assert.False (comparer.Equals (changes1, changes2));
	}

	[Fact]
	public void CompareDifferentMembers ()
	{
		var changes1 = new CodeChanges (
			bindingInfo: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: new ()) {
			EnumMembers = [
				new EnumMember (
					name: "name",
					libraryName: "Test",
					libraryPath: "/path/to/library",
					fieldData: new (),
					symbolAvailability: new (),
					attributes: [])
			],
		};
		var changes2 = new CodeChanges (
			bindingInfo: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: new ()) {
			EnumMembers = [
				new EnumMember (
					name: "name2",
					libraryName: "Test",
					libraryPath: "/path/to/library",
					fieldData: new (),
					symbolAvailability: new (),
					attributes: [])
			],
		};
		Assert.False (comparer.Equals (changes1, changes2));
	}

	[Fact]
	public void CompareDifferentPropertyLength ()
	{
		var changes1 = new CodeChanges (
			bindingInfo: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: new ()) {
			EnumMembers = [],
			Properties = []
		};
		var changes2 = new CodeChanges (
			bindingInfo: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: new ()) {
			EnumMembers = [],
			Properties = [
				new (
					name: "Name",
					returnType: new ("Utils.MyClass"),
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
							],
							modifiers: []
						),
						new (
							accessorKind: AccessorKind.Setter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios18.0"]),
							],
							modifiers: []
						),
					])
			]
		};

		Assert.False (comparer.Equals (changes1, changes2));
	}

	[Fact]
	public void CompareSamePropertiesDiffOrder ()
	{
		var changes1 = new CodeChanges (
			bindingInfo: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"], fullyQualifiedSymbol: "NS.name", symbolAvailability: new ()) {
			EnumMembers = [],
			Properties = [
				new (
					name: "Surname",
					returnType: new ("string"),
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
							],
							modifiers: []
						),
						new (
							accessorKind: AccessorKind.Setter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [],
							modifiers: []
						),
					]),
				new (
					name: "Name",
					returnType: new ("Utils.MyClass"),
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
							],
							modifiers: []
						),
						new (
							accessorKind: AccessorKind.Setter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios18.0"]),
							],
							modifiers: []
						),
					]),
			]
		};
		var changes2 = new CodeChanges (
			bindingInfo: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: new ()) {
			EnumMembers = [],
			Properties = [
				new (
					name: "Name",
					returnType: new ("Utils.MyClass"),
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
							],
							modifiers: []
						),
						new (
							accessorKind: AccessorKind.Setter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios18.0"]),
							],
							modifiers: []
						),
					]),
				new (
					name: "Surname",
					returnType: new ("string"),
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
							],
							modifiers: []
						),
						new (
							accessorKind: AccessorKind.Setter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [],
							modifiers: []
						),
					]),
			]
		};
		Assert.True (comparer.Equals (changes1, changes2));
	}

	[Fact]
	public void CompareDifferentProperties ()
	{
		var changes1 = new CodeChanges (
			bindingInfo: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: new ()) {
			EnumMembers = [],
			Properties = [
				new (
					name: "Surname",
					returnType: new ("string"),
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
							],
							modifiers: []
						),
						new (
							accessorKind: AccessorKind.Setter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [],
							modifiers: []
						),
					]),
				new (
					name: "Name",
					returnType: new ("string"),
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
							],
							modifiers: []
						),
						new (
							accessorKind: AccessorKind.Setter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios18.0"]),
							],
							modifiers: []
						),
					]),
			]
		};
		var changes2 = new CodeChanges (
			bindingInfo: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: new ()) {
			EnumMembers = [],
			Properties = [
				new (
					name: "Name",
					returnType: new ("Utils.MyClass"),
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
							], modifiers: []
						),
						new (
							accessorKind: AccessorKind.Setter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios18.0"]),
							],
							modifiers: []
						),
					]),
				new (
					name: "Surname",
					returnType: new ("string"),
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
							],
							modifiers: []
						),
						new (
							accessorKind: AccessorKind.Setter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [],
							modifiers: []
						),
					]),
			]
		};
		Assert.False (comparer.Equals (changes1, changes2));
	}

	[Fact]
	public void CompareDifferentEventsLength ()
	{
		var changes1 = new CodeChanges (
			bindingInfo: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: new ()) {
			EnumMembers = [],
			Properties = [
				new (
					name: "Surname",
					returnType: new ("string"),
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
							],
							modifiers: []
						),
						new (
							accessorKind: AccessorKind.Setter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [],
							modifiers: []
						),
					]),
				new (
					name: "Name",
					returnType: new ("Utils.MyClass"),
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
							],
							modifiers: []
						),
						new (
							accessorKind: AccessorKind.Setter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios18.0"]),
							],
							modifiers: []
						),
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
						new (
							accessorKind: AccessorKind.Add,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [],
							modifiers: []),
					]),
			]
		};
		var changes2 = new CodeChanges (
			bindingInfo: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: new ()) {
			EnumMembers = [],
			Properties = [
				new (
					name: "Name",
					returnType: new ("Utils.MyClass"),
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
							],
							modifiers: []
						),
						new (
							accessorKind: AccessorKind.Setter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios18.0"]),
							],
							modifiers: []
						),
					]),
				new (
					name: "Surname",
					returnType: new ("string"),
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
							],
							modifiers: []
						),
						new (
							accessorKind: AccessorKind.Setter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [],
							modifiers: []
						),
					]),
			]
		};
		Assert.False (comparer.Equals (changes1, changes2));
	}

	[Fact]
	public void CompareSameEventsDiffOrder ()
	{
		var changes1 = new CodeChanges (
			bindingInfo: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: new ()) {
			EnumMembers = [],
			Properties = [
				new (
					name: "Surname",
					returnType: new ("string"),
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
							],
							modifiers: []
						),
						new (
							accessorKind: AccessorKind.Setter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [],
							modifiers: []
						),
					]),
				new (
					name: "Name",
					returnType: new ("Utils.MyClass"),
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
							],
							modifiers: []
						),
						new (
							accessorKind: AccessorKind.Setter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios18.0"]),
							],
							modifiers: []
						),
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
						new (
							accessorKind: AccessorKind.Add,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [],
							modifiers: []),
					]),
				new (
					name: "MyEvent2",
					type: "System.EventHandler",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [],
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
							modifiers: []
						),
					]),
			]
		};
		var changes2 = new CodeChanges (
			bindingInfo: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: new ()) {
			EnumMembers = [],
			Properties = [
				new (
					name: "Name",
					returnType: new ("Utils.MyClass"),
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
							],
							modifiers: []
						),
						new (
							accessorKind: AccessorKind.Setter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios18.0"]),
							],
							modifiers: []
						),
					]),
				new (
					name: "Surname",
					returnType: new ("string"),
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
							],
							modifiers: []
						),
						new (
							accessorKind: AccessorKind.Setter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [],
							modifiers: []
						),
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
							modifiers: []
						),
					]),
				new (
					name: "MyEvent",
					type: "System.EventHandler",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [],
					accessors: [
						new (
							accessorKind: AccessorKind.Add,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [],
							modifiers: []
						),
					]),
			]
		};

		Assert.True (comparer.Equals (changes1, changes2));
	}

	[Fact]
	public void CompareDifferentEvents ()
	{
		var changes1 = new CodeChanges (
			bindingInfo: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: new ()) {
			EnumMembers = [],
			Properties = [
				new (
					name: "Surname",
					returnType: new ("string"),
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
							],
							modifiers: []
						),
						new (
							accessorKind: AccessorKind.Setter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [],
							modifiers: []
						),
					]),
				new (
					name: "Name",
					returnType: new ("Utils.MyClass"),
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
							],
							modifiers: []
						),
						new (
							accessorKind: AccessorKind.Setter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios18.0"]),
							],
							modifiers: []
						),
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
						new (
							accessorKind: AccessorKind.Add,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [],
							modifiers: []),
					]),
			]
		};
		var changes2 = new CodeChanges (
			bindingInfo: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: new ()) {
			EnumMembers = [],
			Properties = [
				new (
					name: "Name",
					returnType: new ("Utils.MyClass"),
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
							],
							modifiers: []
						),
						new (
							accessorKind: AccessorKind.Setter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios18.0"]),
							],
							modifiers: []
						),
					]),
				new (
					name: "Surname",
					returnType: new ("string"),
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
							],
							modifiers: []
						),
						new (
							accessorKind: AccessorKind.Setter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [],
							modifiers: []
						),
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
						new (
							accessorKind: AccessorKind.Add,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [],
							modifiers: []
						),
					]),
			]
		};
		Assert.False (comparer.Equals (changes1, changes2));
	}

	[Fact]
	public void CompareDifferentMethodsLength ()
	{
		var changes1 = new CodeChanges (
			bindingInfo: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: new ()) {
			EnumMembers = [],
			Properties = [
				new (
					name: "Surname",
					returnType: new ("string"),
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
							],
							modifiers: []
						),
						new (
							accessorKind: AccessorKind.Setter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [],
							modifiers: []
						),
					]),
				new (
					name: "Name",
					returnType: new ("Utils.MyClass"),
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
							],
							modifiers: []
						),
						new (
							accessorKind: AccessorKind.Setter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios18.0"]),
							],
							modifiers: []
						),
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
						new (
							accessorKind: AccessorKind.Add,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [],
							modifiers: []
						),
					]),
				new (
					name: "MyEvent2",
					type: "System.EventHandler",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [],
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
							modifiers: []
						),
					]),
			],
			Methods = [
				new (
					type: "NS.MyClass",
					name: "TryGetString",
					returnType: new (
						type: "bool",
						isNullable: false,
						isBlittable: false,
						isSmartEnum: false,
						isArray: false,
						isReferenceType: false
					),
					symbolAvailability: new (),
					exportMethodData: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (position: 0, type: "string?", name: "example", isBlittable: false) {
							IsNullable = true,
							ReferenceKind = ReferenceKind.Out,
						},
					]
				),
				new Method (
					type: "NS.MyClass",
					name: "MyMethod",
					returnType: new ("void"),
					symbolAvailability: new (),
					exportMethodData: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (position: 0, type: "NS.CustomType", name: "input", isBlittable: false)
					]
				)
			]
		};
		var changes2 = new CodeChanges (
			bindingInfo: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: new ()) {
			EnumMembers = [],
			Properties = [
				new (
					name: "Name",
					returnType: new ("Utils.MyClass"),
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
							],
							modifiers: []
						),
						new (
							accessorKind: AccessorKind.Setter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios18.0"]),
							],
							modifiers: []
						),
					]),
				new (
					name: "Surname",
					returnType: new ("string"),
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
							],
							modifiers: []
						),
						new (
							accessorKind: AccessorKind.Setter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [],
							modifiers: []
						),
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
					]),
				new (
					name: "MyEvent",
					type: "System.EventHandler",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [],
					accessors: [
						new (
							accessorKind: AccessorKind.Add,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [],
							modifiers: []
						),
					]),
			],
			Methods = [
				new (
					type: "NS.MyClass",
					name: "TryGetString",
					returnType: new (
						type: "bool",
						isNullable: false,
						isBlittable: false,
						isSmartEnum: false,
						isArray: false,
						isReferenceType: false
					),
					symbolAvailability: new (),
					exportMethodData: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (position: 0, type: "string?", name: "example", isBlittable: false) {
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
			bindingInfo: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: new ()) {
			EnumMembers = [],
			Properties = [
				new (
					name: "Surname",
					returnType: new ("string"),
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
							],
							modifiers: []
						),
						new (
							accessorKind: AccessorKind.Setter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [],
							modifiers: []
						),
					]),
				new (
					name: "Name",
					returnType: new ("Utils.MyClass"),
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
							],
							modifiers: []
						),
						new (
							accessorKind: AccessorKind.Setter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios18.0"]),
							],
							modifiers: []
						),
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
						new (
							accessorKind: AccessorKind.Add,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [],
							modifiers: []
						),
					]),
				new (
					name: "MyEvent2",
					type: "System.EventHandler",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [],
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
							modifiers: []
						),
					]),
			],
			Methods = [
				new (
					type: "NS.MyClass",
					name: "TryGetString",
					returnType: new (
						type: "bool",
						isNullable: false,
						isBlittable: false,
						isSmartEnum: false,
						isArray: false,
						isReferenceType: false
					),
					symbolAvailability: new (),
					exportMethodData: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (position: 0, type: "string?", name: "example", isBlittable: false) {
							IsNullable = true,
							ReferenceKind = ReferenceKind.Out,
						},
					]
				),
				new Method (
					type: "NS.MyClass",
					name: "MyMethod",
					returnType: new ("void"),
					symbolAvailability: new (),
					exportMethodData: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (position: 0, type: "NS.CustomType", name: "input", isBlittable: false)
					]
				)
			]
		};
		var changes2 = new CodeChanges (
			bindingInfo: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: new ()) {
			EnumMembers = [],
			Properties = [
				new (
					name: "Name",
					returnType: new ("Utils.MyClass"),
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
							],
							modifiers: []
						),
						new (
							accessorKind: AccessorKind.Setter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios18.0"]),
							],
							modifiers: []
						),
					]),
				new (
					name: "Surname",
					returnType: new ("string"),
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
							],
							modifiers: []
						),
						new (
							accessorKind: AccessorKind.Setter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [],
							modifiers: []
						),
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
							modifiers: []
						),
					]),
				new (
					name: "MyEvent",
					type: "System.EventHandler",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [],
					accessors: [
						new (
							accessorKind: AccessorKind.Add,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [],
							modifiers: []
						),
					]),
			],
			Methods = [
				new Method (
					type: "NS.MyClass",
					name: "MyMethod",
					returnType: new ("void"),
					symbolAvailability: new (),
					exportMethodData: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (position: 0, type: "NS.CustomType", name: "input", isBlittable: false)
					]
				),
				new (
					type: "NS.MyClass",
					name: "TryGetString",
					returnType: new (
						type: "bool",
						isNullable: false,
						isBlittable: false,
						isSmartEnum: false,
						isArray: false,
						isReferenceType: false
					),
					symbolAvailability: new (),
					exportMethodData: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (position: 0, type: "string?", name: "example", isBlittable: false) {
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
			bindingInfo: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: new ()) {
			EnumMembers = [],
			Properties = [
				new (
					name: "Surname",
					returnType: new ("string"),
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
							],
							modifiers: []
						),
						new (
							accessorKind: AccessorKind.Setter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [],
							modifiers: []
						),
					]),
				new (
					name: "Name",
					returnType: new ("Utils.MyClass"),
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
							],
							modifiers: []
						),
						new (
							accessorKind: AccessorKind.Setter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios18.0"]),
							],
							modifiers: []
						),
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
						new (
							accessorKind: AccessorKind.Add,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [],
							modifiers: []
						),
					]),
				new (
					name: "MyEvent2",
					type: "System.EventHandler",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [],
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
							modifiers: []
						),
					]),
			],
			Methods = [
				new Method (
					type: "NS.MyClass",
					name: "MyMethod",
					returnType: new ("void"),
					symbolAvailability: new (),
					exportMethodData: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (position: 0, type: "NS.CustomType", name: "input", isBlittable: false),
					]
				),
			]
		};
		var changes2 = new CodeChanges (
			bindingInfo: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: new ()) {
			EnumMembers = [],
			Properties = [
				new (
					name: "Name",
					returnType: new ("Utils.MyClass"),
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
							],
							modifiers: []
						),
						new (
							accessorKind: AccessorKind.Setter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios18.0"]),
							],
							modifiers: []
						),
					]),
				new (
					name: "Surname",
					returnType: new ("string"),
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
							],
							modifiers: []
						),
						new (
							accessorKind: AccessorKind.Setter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [],
							modifiers: []
						),
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
							modifiers: []
						),
					]),
				new (
					name: "MyEvent",
					type: "System.EventHandler",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [],
					accessors: [
						new (
							accessorKind: AccessorKind.Add,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [],
							modifiers: []
						),
					]),
			],
			Methods = [
				new (
					type: "NS.MyClass",
					name: "TryGetString",
					returnType: new (
						type: "bool",
						isNullable: false,
						isBlittable: false,
						isSmartEnum: false,
						isArray: false,
						isReferenceType: false
					),
					symbolAvailability: new (),
					exportMethodData: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (position: 0, type: "string?", name: "example", isBlittable: false) {
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
			bindingInfo: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: builder.ToImmutable ()) {
			EnumMembers = [],
			Properties = [
				new (
					name: "Surname",
					returnType: new ("string"),
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
							],
							modifiers: []
						),
						new (
							accessorKind: AccessorKind.Setter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [],
							modifiers: []
						),
					]),
				new (
					name: "Name",
					returnType: new ("Utils.MyClass"),
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
							],
							modifiers: []
						),
						new (
							accessorKind: AccessorKind.Setter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios18.0"]),
							],
							modifiers: []
						),
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
						new (
							accessorKind: AccessorKind.Add,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [],
							modifiers: []
						),
					]),
				new (
					name: "MyEvent2",
					type: "System.EventHandler",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [],
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
							modifiers: []
						),
					]),
			],
			Methods = [
				new (
					type: "NS.MyClass",
					name: "TryGetString",
					returnType: new (
						type: "bool",
						isNullable: false,
						isBlittable: false,
						isSmartEnum: false,
						isArray: false,
						isReferenceType: false
					),
					symbolAvailability: new (),
					exportMethodData: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (position: 0, type: "string?", name: "example", isBlittable: false) {
							IsNullable = true,
							ReferenceKind = ReferenceKind.Out,
						},
					]
				),
				new Method (
					type: "NS.MyClass",
					name: "MyMethod",
					returnType: new ("void"),
					symbolAvailability: new (),
					exportMethodData: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (position: 0, type: "NS.CustomType", name: "input", isBlittable: false)
					]
				)
			]
		};
		var changes2 = new CodeChanges (
			bindingInfo: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: new ()) {
			EnumMembers = [],
			Properties = [
				new (
					name: "Name",
					returnType: new ("Utils.MyClass"),
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
							],
							modifiers: []
						),
						new (
							accessorKind: AccessorKind.Setter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios18.0"]),
							],
							modifiers: []
						),
					]),
				new (
					name: "Surname",
					returnType: new ("string"),
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
							], modifiers: []),
						new (AccessorKind.Setter, new (), null, [], []),
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
							modifiers: []
						),
					]),
				new (
					name: "MyEvent",
					type: "System.EventHandler",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [],
					accessors: [
						new (
							accessorKind: AccessorKind.Add,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [],
							modifiers: []
						),
					]),
			],
			Methods = [
				new Method (
					type: "NS.MyClass",
					name: "MyMethod",
					returnType: new ("void"),
					symbolAvailability: new (),
					exportMethodData: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (position: 0, type: "NS.CustomType", name: "input", isBlittable: false)
					]
				),
				new (
					type: "NS.MyClass",
					name: "TryGetString",
					returnType: new (
						type: "bool",
						isNullable: false,
						isBlittable: false,
						isSmartEnum: false,
						isArray: false,
						isReferenceType: false
					),
					symbolAvailability: new (),
					exportMethodData: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (position: 0, type: "string?", name: "example", isBlittable: false) {
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
			bindingInfo: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: builder.ToImmutable ()) {
			EnumMembers = [],
			Properties = [
				new (
					name: "Surname",
					returnType: new ("string"),
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
							],
							modifiers: []
						),
						new (
							accessorKind: AccessorKind.Setter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [],
							modifiers: []
						),
					]),
				new (
					name: "Name",
					returnType: new ("Utils.MyClass"),
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
							],
							modifiers: []
						),
						new (
							accessorKind: AccessorKind.Setter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios18.0"]),
							],
							modifiers: []
						),
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
						new (
							accessorKind: AccessorKind.Add,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [],
							modifiers: []
						),
					]),
				new (
					name: "MyEvent2",
					type: "System.EventHandler",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [],
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
							modifiers: []
						),
					]),
			],
			Methods = [
				new (
					type: "NS.MyClass",
					name: "TryGetString",
					returnType: new (
						type: "bool",
						isNullable: false,
						isBlittable: false,
						isSmartEnum: false,
						isArray: false,
						isReferenceType: false
					),
					symbolAvailability: new (),
					exportMethodData: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (position: 0, type: "string?", name: "example", isBlittable: false) {
							IsNullable = true,
							ReferenceKind = ReferenceKind.Out,
						},
					]
				),
				new Method (
					type: "NS.MyClass",
					name: "MyMethod",
					returnType: new ("void"),
					symbolAvailability: new (),
					exportMethodData: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (position: 0, type: "NS.CustomType", name: "input", isBlittable: false)
					]
				)
			]
		};
		var changes2 = new CodeChanges (
			bindingInfo: new (BindingType.SmartEnum, new ()),
			name: "name",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name",
			symbolAvailability: builder.ToImmutable ()) {
			EnumMembers = [],
			Properties = [
				new (
					name: "Name",
					returnType: new ("Utils.MyClass"),
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
							],
							modifiers: []
						),
						new (
							accessorKind: AccessorKind.Setter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios18.0"]),
							],
							modifiers: []
						),
					]),
				new (
					name: "Surname",
					returnType: new ("string"),
					symbolAvailability: new (),
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
							], modifiers: []
						),
						new (
							accessorKind: AccessorKind.Setter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [],
							modifiers: []
						),
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
							modifiers: []
						),
					]),
				new (
					name: "MyEvent",
					type: "System.EventHandler",
					symbolAvailability: new (),
					attributes: [],
					modifiers: [],
					accessors: [
						new (
							accessorKind: AccessorKind.Add,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [],
							modifiers: []
						),
					]),
			],
			Methods = [
				new Method (
					type: "NS.MyClass",
					name: "MyMethod",
					returnType: new ("void"),
					symbolAvailability: new (),
					exportMethodData: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (position: 0, type: "NS.CustomType", name: "input", isBlittable: false)
					]
				),
				new (
					type: "NS.MyClass",
					name: "TryGetString",
					returnType: new (
						type: "bool",
						isNullable: false,
						isBlittable: false,
						isSmartEnum: false,
						isArray: false,
						isReferenceType: false
					),
					symbolAvailability: new (),
					exportMethodData: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					parameters: [
						new (position: 0, type: "string?", name: "example", isBlittable: false) {
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
