using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.DataModel;
using Xamarin.Tests;
using Xamarin.Utils;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.DataModel;

public class CodeChangesComparerTests : BaseGeneratorTestClass {
	readonly CodeChangesComparer comparer = new ();

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
		var changes1 = new CodeChanges (BindingType.SmartEnum, "name1", node, [], []);
		var changes2 = new CodeChanges (BindingType.SmartEnum, "name2", node, [], []);
		Assert.False (comparer.Equals (changes1, changes2));
	}

	[Theory]
	[AllSupportedPlatforms]
	public void CompareDifferentBindingType (ApplePlatform platform)
	{
		var node = GetSyntaxNode<ClassDeclarationSyntax> (platform);
		var changes1 = new CodeChanges (BindingType.SmartEnum, "name", node, [], []);
		var changes2 = new CodeChanges (BindingType.Unknown, "name", node, [], []);
		Assert.False (comparer.Equals (changes1, changes2));
	}

	[Theory]
	[AllSupportedPlatforms]
	public void CompareDifferentSymbolDeclaration (ApplePlatform platform)
	{
		var node1 = GetSyntaxNode<ClassDeclarationSyntax> (platform);
		var node2 = GetSyntaxNode<EnumDeclarationSyntax> (platform);
		var changes1 = new CodeChanges (BindingType.SmartEnum, "name", node1, [], []);
		var changes2 = new CodeChanges (BindingType.SmartEnum, "name", node2, [], []);
		Assert.False (comparer.Equals (changes1, changes2));
	}

	[Theory]
	[AllSupportedPlatforms]
	public void CompareDifferentAttributesLength (ApplePlatform platform)
	{
		var node = GetSyntaxNode<ClassDeclarationSyntax> (platform);
		var changes1 = new CodeChanges (BindingType.SmartEnum, "name", node, [], []);
		var changes2 = new CodeChanges (BindingType.SmartEnum, "name", node, [
			new AttributeCodeChange ("name", ["arg1", "arg2"])
		], []);
		Assert.False (comparer.Equals (changes1, changes2));
	}

	[Theory]
	[AllSupportedPlatforms]
	public void CompareDifferentAttributes (ApplePlatform platform)
	{
		var node = GetSyntaxNode<ClassDeclarationSyntax> (platform);
		var changes1 = new CodeChanges (BindingType.SmartEnum, "name", node, [
			new AttributeCodeChange ("name", ["arg1", "arg2"])
		], []);
		var changes2 = new CodeChanges (BindingType.SmartEnum, "name", node, [
			new AttributeCodeChange ("name2", ["arg1", "arg2"])
		], []);
		Assert.False (comparer.Equals (changes1, changes2));
	}

	[Theory]
	[AllSupportedPlatforms]
	public void CompareDifferentMembersLength (ApplePlatform platform)
	{
		var node = GetSyntaxNode<ClassDeclarationSyntax> (platform);
		var changes1 = new CodeChanges (BindingType.SmartEnum, "name", node, [], []);
		var changes2 = new CodeChanges (BindingType.SmartEnum, "name", node, [], [
			new MemberCodeChange ("name", [])
		]);
		Assert.False (comparer.Equals (changes1, changes2));
	}

	[Theory]
	[AllSupportedPlatforms]
	public void CompareDifferentMembers (ApplePlatform platform)
	{
		var node = GetSyntaxNode<ClassDeclarationSyntax> (platform);
		var changes1 = new CodeChanges (BindingType.SmartEnum, "name", node, [], [
			new MemberCodeChange ("name", [])
		]);
		var changes2 = new CodeChanges (BindingType.SmartEnum, "name", node, [], [
			new MemberCodeChange ("name2", [])
		]);
		Assert.False (comparer.Equals (changes1, changes2));
	}
}
