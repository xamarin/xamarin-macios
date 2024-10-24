using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Extensions;
using Xamarin.Tests;
using Xamarin.Utils;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.Extensions;

public class BaseTypeDeclarationSyntaxExtensionsTests : BaseGeneratorTestClass {

	T GetDeclaration<T> (ApplePlatform platform, string inputText) where T : BaseTypeDeclarationSyntax
	{
		var (compilation, sourceTrees) = CreateCompilation (nameof (BaseTypeDeclarationSyntaxExtensionsTests), platform, inputText);
		Assert.Single (sourceTrees);
		var declaration = sourceTrees [0].GetRoot ()
			.DescendantNodes ()
			.OfType<T> ()
			.FirstOrDefault ();
		Assert.NotNull (declaration);
		return declaration;
	}

	[Theory]
	[PlatformInlineData (ApplePlatform.iOS)]
	[PlatformInlineData (ApplePlatform.TVOS)]
	[PlatformInlineData (ApplePlatform.MacCatalyst)]
	[PlatformInlineData (ApplePlatform.MacOSX)]
	public void GetFullyQualifiedIdentifierFileScopedNamespace (ApplePlatform platform)
	{
		const string inputText = @"
namespace Test;
public class Foo {
}
";
		var declaration = GetDeclaration<ClassDeclarationSyntax> (platform, inputText);
		Assert.Equal ("Test.Foo", declaration.GetFullyQualifiedIdentifier ());
	}

	[Theory]
	[PlatformInlineData (ApplePlatform.iOS)]
	[PlatformInlineData (ApplePlatform.TVOS)]
	[PlatformInlineData (ApplePlatform.MacCatalyst)]
	[PlatformInlineData (ApplePlatform.MacOSX)]
	public void GetFullyQualifiedIdentifierFileScopedNamespaceNestedClass (ApplePlatform platform)
	{
		const string inputText = @"
namespace Test;
public class Foo {
	public enum Bar {
	}	
}
";
		var declaration = GetDeclaration<EnumDeclarationSyntax> (platform, inputText);
		Assert.Equal ("Test.Foo.Bar", declaration.GetFullyQualifiedIdentifier ());
	}

	[Theory]
	[PlatformInlineData (ApplePlatform.iOS)]
	[PlatformInlineData (ApplePlatform.TVOS)]
	[PlatformInlineData (ApplePlatform.MacCatalyst)]
	[PlatformInlineData (ApplePlatform.MacOSX)]
	public void GetFullyQualifiedIdentifierNamespaceDeclaration (ApplePlatform platform)
	{
		const string inputText = @"
namespace Test {
	public class Foo {
	}
}
";
		var declaration = GetDeclaration<ClassDeclarationSyntax> (platform, inputText);
		Assert.Equal ("Test.Foo", declaration.GetFullyQualifiedIdentifier ());
	}

	[Theory]
	[PlatformInlineData (ApplePlatform.iOS)]
	[PlatformInlineData (ApplePlatform.TVOS)]
	[PlatformInlineData (ApplePlatform.MacCatalyst)]
	[PlatformInlineData (ApplePlatform.MacOSX)]
	public void GetFullyQualifiedIdentifierMultipleNamespaceDeclaration (ApplePlatform platform)
	{
		const string inputText = @"
namespace Test {
	public class Foo {}
}
namespace Test2 {
	public class Bar {}
}
";
		var declaration = GetDeclaration<ClassDeclarationSyntax> (platform, inputText);
		Assert.Equal ("Test.Foo", declaration.GetFullyQualifiedIdentifier ());
	}

	[Theory]
	[PlatformInlineData (ApplePlatform.iOS)]
	[PlatformInlineData (ApplePlatform.TVOS)]
	[PlatformInlineData (ApplePlatform.MacCatalyst)]
	[PlatformInlineData (ApplePlatform.MacOSX)]
	public void GetFullyQualifiedIdentifierNestedNamespaceDeclaration (ApplePlatform platform)
	{
		const string inputText = @"
namespace Foo {
	namespace Bar {
		public class Test {}
	}
}
";
		var declaration = GetDeclaration<ClassDeclarationSyntax> (platform, inputText);
		Assert.Equal ("Foo.Bar.Test", declaration.GetFullyQualifiedIdentifier ());
	}

	[Theory]
	[PlatformInlineData (ApplePlatform.iOS)]
	[PlatformInlineData (ApplePlatform.TVOS)]
	[PlatformInlineData (ApplePlatform.MacCatalyst)]
	[PlatformInlineData (ApplePlatform.MacOSX)]
	public void GetFullyQualifiedIdentifierNamespaceDeclarationNestedClass (ApplePlatform platform)
	{
		const string inputText = @"
namespace Foo {
	namespace Bar {
		public class Test {
			public enum Final {
				None,
			}
		}
	}
}
";
		var declaration = GetDeclaration<EnumDeclarationSyntax> (platform, inputText);
		Assert.Equal ("Foo.Bar.Test.Final", declaration.GetFullyQualifiedIdentifier ());
	}
}
