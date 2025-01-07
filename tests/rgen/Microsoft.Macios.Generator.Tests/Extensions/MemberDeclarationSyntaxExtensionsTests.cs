using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Extensions;
using Xamarin.Tests;
using Xamarin.Utils;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.Extensions;

public class MemberDeclarationSyntaxExtensionsTests : BaseGeneratorTestClass {
	[Theory]
	[AllSupportedPlatforms]
	public void GetAttributeCodeChanges (ApplePlatform platform)
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
		// create a compilation unit and get the diff syntax node, semantic model and expected attr result
		var (compilation, sourceTrees) =
			CreateCompilation (platform, sources: [attrsText, inputText]);
		Assert.Equal (2, sourceTrees.Length);
		// get the declarations we want to work with and the semantic model
		var nodes = sourceTrees [1].GetRoot ().DescendantNodes ().ToArray ();
		var classDeclaration = nodes
			.OfType<ClassDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (classDeclaration);
		var methodDeclaration = classDeclaration.DescendantNodes ()
			.OfType<MethodDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (methodDeclaration);
		var enumDeclaration = nodes
			.OfType<EnumDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (enumDeclaration);
		var enumValueDeclaration = enumDeclaration.DescendantNodes ()
			.OfType<EnumMemberDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (enumValueDeclaration);
		var interfaceDeclaration = nodes
			.OfType<InterfaceDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (interfaceDeclaration);
		var semanticModel = compilation.GetSemanticModel (sourceTrees [1]);
		var declarations = new MemberDeclarationSyntax [] {
			classDeclaration, methodDeclaration, enumDeclaration, enumValueDeclaration, interfaceDeclaration
		};

		foreach (var declarationSyntax in declarations) {
			var attributeCodeChanges = declarationSyntax.GetAttributeCodeChanges (semanticModel);
			Assert.Equal (2, attributeCodeChanges.Length);
			// assert the values of each of the attr
			Assert.Equal ("ObjCBindings.SimpleAttribute", attributeCodeChanges [0].Name);
			Assert.Empty (attributeCodeChanges [0].Arguments);
			Assert.Equal ("ObjCBindings.AttributeWithParams", attributeCodeChanges [1].Name);
			Assert.Equal (2, attributeCodeChanges [1].Arguments.Length);
			Assert.Equal ("first", attributeCodeChanges [1].Arguments [0]);
			Assert.Equal ("2", attributeCodeChanges [1].Arguments [1]);
		}
	}

	[Theory]
	[AllSupportedPlatforms]
	public void GetAttributeCodeChangesSameAttrDiffText (ApplePlatform platform)
	{
		const string inputText = @"
using System;
using System.ComponentModel;
using Foundation;
using ObjCBindings;

namespace AVFoundation;

public class TestClass {

	[EditorBrowsableAttribute (EditorBrowsableState.Never)]
	public void SayHello () {
	}

	[System.ComponentModel.EditorBrowsableAttribute (System.ComponentModel.EditorBrowsableState.Never)]
	public void SayGoodbye () {
	}
} 
";
		var (compilation, sourceTrees) =
			CreateCompilation (platform, sources: inputText);
		Assert.Single (sourceTrees);
		// get the declarations we want to work with and the semantic model
		var declarations = sourceTrees [0].GetRoot ()
			.DescendantNodes ()
			.OfType<MethodDeclarationSyntax> ()
			.ToArray ();
		Assert.Equal (2, declarations.Length);
		// get the attr for each of the declaration and assert they are the same attrs
		var semanticModel = compilation.GetSemanticModel (sourceTrees [0]);
		var sayHelloAttrs = declarations [0].GetAttributeCodeChanges (semanticModel);
		var sayGoodbyeAttrs = declarations [0].GetAttributeCodeChanges (semanticModel);
		Assert.Single (sayHelloAttrs);
		Assert.Single (sayGoodbyeAttrs);
		Assert.Equal (sayHelloAttrs [0].Name, sayGoodbyeAttrs [0].Name);
		Assert.Equal (sayHelloAttrs [0].Arguments [0], sayGoodbyeAttrs [0].Arguments [0]);
	}

	[Theory]
	[AllSupportedPlatforms]
	public void GetAttributeCodeChangesTypeOf (ApplePlatform platform)
	{

		var attrsText = @"
using System;

namespace ObjCBindings;

public class AttributeWithParams : Attribute {
	Type _type;
	public AttributeWithParams (Type type) {
		_type = type;
	}
}
";
		var inputText = @"
using System;
using Foundation;
using ObjCBindings;

namespace AVFoundation;

public class TestClass {

	[AttributeWithParams (typeof(TestClass))]
	public void SayHello () {
	}
} 
";
		// create a compilation unit and get the diff syntax node, semantic model and expected attr result
		var (compilation, sourceTrees) =
			CreateCompilation (platform, sources: [attrsText, inputText]);
		Assert.Equal (2, sourceTrees.Length);
		// get the declarations we want to work with and the semantic model
		var nodes = sourceTrees [1].GetRoot ().DescendantNodes ().ToArray ();
		var methodDeclarationSyntax = nodes
			.OfType<MethodDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (methodDeclarationSyntax);
		var semanticModel = compilation.GetSemanticModel (sourceTrees [1]);
		var methodAttrs = methodDeclarationSyntax.GetAttributeCodeChanges (semanticModel);
		Assert.Single (methodAttrs);
		Assert.Equal ("ObjCBindings.AttributeWithParams", methodAttrs [0].Name);
		Assert.Equal ("AVFoundation.TestClass", methodAttrs [0].Arguments [0]);
	}
}
