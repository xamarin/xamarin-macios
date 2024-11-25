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

	// returns a node that matches the given node type from an example syntax tree
	T GetSyntaxNode<T> (ApplePlatform platform) where T : BaseTypeDeclarationSyntax
	{
		var attrsText = @"
using System;

namespace ObjCBindings;
public class SimpleAttribute : Attribute {
}

public class AttributeWithParams : Attribute {
	public AttributeWithParams (string name, int value) {
	}
}
";
		var inputText = @"
using System;
using Foundation;
using ObjCBindings;

namespace AVFoundation;

[SimpleAttribute, AttributeWithParams (""first"", 2)]
public class TestClass {

	[SimpleAttribute, AttributeWithParams (""first"", 2)]
	public void SayHello () {
	}
} 

[SimpleAttribute, AttributeWithParams (""first"", 2)]
public enum TestEnum {
	[SimpleAttribute, AttributeWithParams (""first"", 2)]
	First,	
}

[SimpleAttribute, AttributeWithParams (""first"", 2)]
public interface IInterface {
	[SimpleAttribute, AttributeWithParams (""first"", 2)]
	public void SayHello ();
}
";
		var (_, sourceTrees) =
			CreateCompilation (nameof (CodeChangesComparerTests), platform, attrsText, inputText);
		Assert.Equal (2, sourceTrees.Length);
		// get the declarations we want to work with and the semantic model
		var nodes = sourceTrees [1].GetRoot ().DescendantNodes ().ToArray ();
		var declarationNode = nodes
			.OfType<T> ()
			.FirstOrDefault ();
		Assert.NotNull (declarationNode);
		return declarationNode;
	}

	[Theory]
	[AllSupportedPlatforms]
	public void CompareDifferentFullyQualifiedSymbol (ApplePlatform platform)
	{
		var node = GetSyntaxNode<ClassDeclarationSyntax> (platform);
		var changes1 = new CodeChanges (BindingType.SmartEnum, "name1", node);
		var changes2 = new CodeChanges (BindingType.SmartEnum, "name2", node);
		Assert.False (comparer.Equals (changes1, changes2));
	}

	[Theory]
	[AllSupportedPlatforms]
	public void CompareDifferentBindingType (ApplePlatform platform)
	{
		var node = GetSyntaxNode<ClassDeclarationSyntax> (platform);
		var changes1 = new CodeChanges (BindingType.SmartEnum, "name", node);
		var changes2 = new CodeChanges (BindingType.Unknown, "name", node);
		Assert.False (comparer.Equals (changes1, changes2));
	}

	[Theory]
	[AllSupportedPlatforms]
	public void CompareDifferentSymbolDeclaration (ApplePlatform platform)
	{
		var node1 = GetSyntaxNode<ClassDeclarationSyntax> (platform);
		var node2 = GetSyntaxNode<EnumDeclarationSyntax> (platform);
		var changes1 = new CodeChanges (BindingType.SmartEnum, "name", node1);
		var changes2 = new CodeChanges (BindingType.SmartEnum, "name", node2);
		Assert.False (comparer.Equals (changes1, changes2));
	}

	[Theory]
	[AllSupportedPlatforms]
	public void CompareDifferentAttributesLength (ApplePlatform platform)
	{
		var node = GetSyntaxNode<ClassDeclarationSyntax> (platform);
		var changes1 = new CodeChanges (BindingType.SmartEnum, "name", node);
		var changes2 = new CodeChanges (BindingType.SmartEnum, "name", node) {
			Attributes = [
				new AttributeCodeChange ("name", ["arg1", "arg2"])
			]
		};
		Assert.False (comparer.Equals (changes1, changes2));
	}

	[Theory]
	[AllSupportedPlatforms]
	public void CompareDifferentAttributes (ApplePlatform platform)
	{
		var node = GetSyntaxNode<ClassDeclarationSyntax> (platform);
		var changes1 = new CodeChanges (BindingType.SmartEnum, "name", node) {
			Attributes = [
				new AttributeCodeChange ("name", ["arg1", "arg2"])
			],
		};
		var changes2 = new CodeChanges (BindingType.SmartEnum, "name", node) {
			Attributes = [
				new AttributeCodeChange ("name2", ["arg1", "arg2"])
			],
		};
		Assert.False (comparer.Equals (changes1, changes2));
	}

	[Theory]
	[AllSupportedPlatforms]
	public void CompareDifferentMembersLength (ApplePlatform platform)
	{
		var node = GetSyntaxNode<ClassDeclarationSyntax> (platform);
		var changes1 = new CodeChanges (BindingType.SmartEnum, "name", node);
		var changes2 = new CodeChanges (BindingType.SmartEnum, "name", node) {
			EnumMembers = [
				new EnumMember ("name", [])
			],
		};
		Assert.False (comparer.Equals (changes1, changes2));
	}

	[Theory]
	[AllSupportedPlatforms]
	public void CompareDifferentMembers (ApplePlatform platform)
	{
		var node = GetSyntaxNode<ClassDeclarationSyntax> (platform);
		var changes1 = new CodeChanges (BindingType.SmartEnum, "name", node) {
			EnumMembers = [
				new EnumMember ("name", [])
			],
		};
		var changes2 = new CodeChanges (BindingType.SmartEnum, "name", node) {
			EnumMembers = [
				new EnumMember ("name2", [])
			],
		};
		Assert.False (comparer.Equals (changes1, changes2));
	}

	[Theory]
	[AllSupportedPlatforms]
	public void CompareDifferentPropertyLength (ApplePlatform platform)
	{
		var node = GetSyntaxNode<ClassDeclarationSyntax> (platform);
		var changes1 = new CodeChanges (BindingType.SmartEnum, "name", node) { EnumMembers = [], Properties = [] };
		var changes2 = new CodeChanges (BindingType.SmartEnum, "name", node) {
			EnumMembers = [],
			Properties = [
				new (
					name: "Name",
					type: "Utils.MyClass",
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (AccessorKind.Getter, [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
						], []),
						new (AccessorKind.Setter, [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios18.0"]),
						], []),
					])
			]
		};

		Assert.False (comparer.Equals (changes1, changes2));
	}

	[Theory]
	[AllSupportedPlatforms]
	public void CompareSamePropertiesDiffOrder (ApplePlatform platform)
	{
		var node = GetSyntaxNode<ClassDeclarationSyntax> (platform);
		var changes1 = new CodeChanges (BindingType.SmartEnum, "name", node) {
			EnumMembers = [],
			Properties = [
				new (
					name: "Surname",
					type: "string",
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (AccessorKind.Getter, [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
						], []),
						new (AccessorKind.Setter, [], []),
					]),
				new (
					name: "Name",
					type: "Utils.MyClass",
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (AccessorKind.Getter, [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
						], []),
						new (AccessorKind.Setter, [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios18.0"]),
						], []),
					]),
			]
		};
		var changes2 = new CodeChanges (BindingType.SmartEnum, "name", node) {
			EnumMembers = [],
			Properties = [
				new (
					name: "Name",
					type: "Utils.MyClass",
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (AccessorKind.Getter, [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
						], []),
						new (AccessorKind.Setter, [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios18.0"]),
						], []),
					]),
				new (
					name: "Surname",
					type: "string",
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (AccessorKind.Getter, [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
						], []),
						new (AccessorKind.Setter, [], []),
					]),
			]
		};
		Assert.True (comparer.Equals (changes1, changes2));
	}

	[Theory]
	[AllSupportedPlatforms]
	public void CompareDifferentProperties (ApplePlatform platform)
	{
		var node = GetSyntaxNode<ClassDeclarationSyntax> (platform);
		var changes1 = new CodeChanges (BindingType.SmartEnum, "name", node) {
			EnumMembers = [],
			Properties = [
				new (
					name: "Surname",
					type: "string",
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (AccessorKind.Getter, [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
						], []),
						new (AccessorKind.Setter, [], []),
					]),
				new (
					name: "Name",
					type: "string",
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (AccessorKind.Getter, [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
						], []),
						new (AccessorKind.Setter, [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios18.0"]),
						], []),
					]),
			]
		};
		var changes2 = new CodeChanges (BindingType.SmartEnum, "name", node) {
			EnumMembers = [],
			Properties = [
				new (
					name: "Name",
					type: "Utils.MyClass",
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (AccessorKind.Getter, [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
						], []),
						new (AccessorKind.Setter, [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios18.0"]),
						], []),
					]),
				new (
					name: "Surname",
					type: "string",
					attributes: [
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (AccessorKind.Getter, [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
						], []),
						new (AccessorKind.Setter, [], []),
					]),
			]
		};
		Assert.False (comparer.Equals (changes1, changes2));
	}

	[Theory]
	[AllSupportedPlatforms]
	public void CompareDifferentEventsLength (ApplePlatform platform)
	{
		var node = GetSyntaxNode<ClassDeclarationSyntax> (platform);
		var changes1 = new CodeChanges (BindingType.SmartEnum, "name", node) {
			EnumMembers = [],
			Properties = [
				new(
					name: "Surname",
					type: "string",
					attributes: [
						new("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new(AccessorKind.Getter, [
							new("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
						], []),
						new(AccessorKind.Setter, [], []),
					]),
				new(
					name: "Name",
					type: "Utils.MyClass",
					attributes: [
						new("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new(AccessorKind.Getter, [
							new("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
						], []),
						new(AccessorKind.Setter, [
							new("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios18.0"]),
						], []),
					]),
			],
			Events = [
				new(
					name: "MyEvent",
					type: "System.EventHandler",
					attributes: [],
					modifiers: [],
					accessors: [
						new(AccessorKind.Add, [], []),
					]),
			]
		};
		var changes2 = new CodeChanges (BindingType.SmartEnum, "name", node) {
			EnumMembers = [],
			Properties = [
				new(
					name: "Name",
					type: "Utils.MyClass",
					attributes: [
						new("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new(AccessorKind.Getter, [
							new("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
						], []),
						new(AccessorKind.Setter, [
							new("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios18.0"]),
						], []),
					]),
				new(
					name: "Surname",
					type: "string",
					attributes: [
						new("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new(AccessorKind.Getter, [
							new("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
						], []),
						new(AccessorKind.Setter, [], []),
					]),
			]
		};
		Assert.False (comparer.Equals (changes1, changes2));
	}

	[Theory]
	[AllSupportedPlatforms]
	public void CompareSameEventsDiffOrder (ApplePlatform platform)
	{
		var node = GetSyntaxNode<ClassDeclarationSyntax> (platform);
		var changes1 = new CodeChanges (BindingType.SmartEnum, "name", node) {
			EnumMembers = [],
			Properties = [
				new(
					name: "Surname",
					type: "string",
					attributes: [
						new("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new(AccessorKind.Getter, [
							new("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
						], []),
						new(AccessorKind.Setter, [], []),
					]),
				new(
					name: "Name",
					type: "Utils.MyClass",
					attributes: [
						new("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new(AccessorKind.Getter, [
							new("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
						], []),
						new(AccessorKind.Setter, [
							new("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios18.0"]),
						], []),
					]),
			],
			Events = [
				new(
					name: "MyEvent",
					type: "System.EventHandler",
					attributes: [],
					modifiers: [],
					accessors: [
						new(AccessorKind.Add, [], []),
					]),
				new(
					name: "MyEvent2",
					type: "System.EventHandler",
					attributes: [],
					modifiers: [],
					accessors: [
						new(AccessorKind.Add, [], []),
						new(AccessorKind.Remove, [], []),
					]),
			]
		};
		var changes2 = new CodeChanges (BindingType.SmartEnum, "name", node) {
			EnumMembers = [],
			Properties = [
				new(
					name: "Name",
					type: "Utils.MyClass",
					attributes: [
						new("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new(AccessorKind.Getter, [
							new("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
						], []),
						new(AccessorKind.Setter, [
							new("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios18.0"]),
						], []),
					]),
				new(
					name: "Surname",
					type: "string",
					attributes: [
						new("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new(AccessorKind.Getter, [
							new("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
						], []),
						new(AccessorKind.Setter, [], []),
					]),
			],
			Events = [
				new(
					name: "MyEvent2",
					type: "System.EventHandler",
					attributes: [],
					modifiers: [],
					accessors: [
						new(AccessorKind.Add, [], []),
						new(AccessorKind.Remove, [], []),
					]),
				new(
					name: "MyEvent",
					type: "System.EventHandler",
					attributes: [],
					modifiers: [],
					accessors: [
						new(AccessorKind.Add, [], []),
					]),
			]
		};

		Assert.True (comparer.Equals (changes1, changes2));
	}

	[Theory]
	[AllSupportedPlatforms]
	public void CompareDifferentEvents (ApplePlatform platform)
	{
		var node = GetSyntaxNode<ClassDeclarationSyntax> (platform);
		var changes1 = new CodeChanges (BindingType.SmartEnum, "name", node) {
			EnumMembers = [],
			Properties = [
				new(
					name: "Surname",
					type: "string",
					attributes: [
						new("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new(AccessorKind.Getter, [
							new("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
						], []),
						new(AccessorKind.Setter, [], []),
					]),
				new(
					name: "Name",
					type: "Utils.MyClass",
					attributes: [
						new("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new(AccessorKind.Getter, [
							new("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
						], []),
						new(AccessorKind.Setter, [
							new("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios18.0"]),
						], []),
					]),
			],
			Events = [
				new(
					name: "MyEvent",
					type: "System.EventHandler",
					attributes: [],
					modifiers: [],
					accessors: [
						new(AccessorKind.Add, [], []),
					]),
			]
		};
		var changes2 = new CodeChanges (BindingType.SmartEnum, "name", node) {
			EnumMembers = [],
			Properties = [
				new(
					name: "Name",
					type: "Utils.MyClass",
					attributes: [
						new("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new(AccessorKind.Getter, [
							new("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
						], []),
						new(AccessorKind.Setter, [
							new("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios18.0"]),
						], []),
					]),
				new(
					name: "Surname",
					type: "string",
					attributes: [
						new("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
					],
					accessors: [
						new(AccessorKind.Getter, [
							new("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
						], []),
						new(AccessorKind.Setter, [], []),
					]),
			],
			Events = [
				new(
					name: "MyEvent",
					type: "System.EventHandler",
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (SyntaxKind.InternalKeyword),
					],
					accessors: [
						new(AccessorKind.Add, [], []),
					]),
			]
		};
		Assert.False (comparer.Equals (changes1, changes2));
	}
}
