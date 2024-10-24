using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Extensions;
using Xamarin.Tests;
using Xamarin.Utils;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.Extensions;

public class MemberDeclarationSyntaxExtensionsTests : BaseGeneratorTestClass {
	[Theory]
	[PlatformInlineData (ApplePlatform.iOS)]
	[PlatformInlineData (ApplePlatform.TVOS)]
	[PlatformInlineData (ApplePlatform.MacOSX)]
	[PlatformInlineData (ApplePlatform.MacCatalyst)]
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
			CreateCompilation (nameof(MemberDeclarationSyntaxExtensionsTests), platform, attrsText, inputText);
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
}
