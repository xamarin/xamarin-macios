using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Extensions;
using Xunit;

namespace Microsoft.Macios.Generator.Tests;

public class BaseTypeDeclarationSyntaxExtensionsTests {
	[Fact]
	public void GetFullyQualifiedIdentifierFileScopedNamespace ()
	{
		var compilationUnit = SyntaxFactory.CompilationUnit ()
			.WithMembers (
				SyntaxFactory.SingletonList<MemberDeclarationSyntax> (
					SyntaxFactory.FileScopedNamespaceDeclaration (
							SyntaxFactory.IdentifierName ("Test"))
						.WithMembers (
							SyntaxFactory.SingletonList<MemberDeclarationSyntax> (
								SyntaxFactory.ClassDeclaration ("Foo")
									.WithModifiers (
										SyntaxFactory.TokenList (
											SyntaxFactory.Token (SyntaxKind.PublicKeyword)))))))
			.NormalizeWhitespace ();
		var declaration = compilationUnit.SyntaxTree.GetRoot ()
			.DescendantNodes ()
			.OfType<ClassDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (declaration);
		Assert.Equal ("Test.Foo", declaration.GetFullyQualifiedIdentifier ());
	}

	[Fact]
	public void GetFullyQualifiedIdentifierFileScopedNamespaceNestedClass ()
	{
		var compilationUnit = SyntaxFactory.CompilationUnit ()
			.WithMembers (
				SyntaxFactory.SingletonList<MemberDeclarationSyntax> (
					SyntaxFactory.FileScopedNamespaceDeclaration (
							SyntaxFactory.IdentifierName ("Test"))
						.WithMembers (
							SyntaxFactory.SingletonList<MemberDeclarationSyntax> (
								SyntaxFactory.ClassDeclaration ("Foo")
									.WithModifiers (
										SyntaxFactory.TokenList (
											SyntaxFactory.Token (SyntaxKind.PublicKeyword)))
									.WithMembers (
										SyntaxFactory.SingletonList<MemberDeclarationSyntax> (
											SyntaxFactory.ClassDeclaration ("Bar")
												.WithModifiers (
													SyntaxFactory.TokenList (
														SyntaxFactory.Token (SyntaxKind.PublicKeyword)))))))))
			.NormalizeWhitespace ();
		var declaration = compilationUnit.SyntaxTree.GetRoot ()
			.DescendantNodes ()
			.OfType<ClassDeclarationSyntax> ()
			.LastOrDefault ();
		Assert.NotNull (declaration);
		Assert.Equal ("Test.Foo.Bar", declaration.GetFullyQualifiedIdentifier ());
	}

	[Fact]
	public void GetFullyQualifiedIdentifierNamespaceDeclaration ()
	{
		var compilationUnit = SyntaxFactory.CompilationUnit ()
			.WithMembers (
				SyntaxFactory.SingletonList<MemberDeclarationSyntax> (
					SyntaxFactory.NamespaceDeclaration (
							SyntaxFactory.IdentifierName ("Test"))
						.WithMembers (
							SyntaxFactory.SingletonList<MemberDeclarationSyntax> (
								SyntaxFactory.ClassDeclaration ("Foo")
									.WithModifiers (
										SyntaxFactory.TokenList (
											SyntaxFactory.Token (SyntaxKind.PublicKeyword)))))))
			.NormalizeWhitespace ();
		var declaration = compilationUnit.SyntaxTree.GetRoot ()
			.DescendantNodes ()
			.OfType<ClassDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (declaration);
		Assert.Equal ("Test.Foo", declaration.GetFullyQualifiedIdentifier ());
	}

	[Fact]
	public void GetFullyQualifiedIdentifierMultipleNamespaceDeclaration ()
	{
		var compilationUnit = SyntaxFactory.CompilationUnit ()
			.WithMembers (
				SyntaxFactory.List<MemberDeclarationSyntax> (
					new MemberDeclarationSyntax [] {
						SyntaxFactory.NamespaceDeclaration (
								SyntaxFactory.IdentifierName ("Test"))
							.WithMembers (
								SyntaxFactory.SingletonList<MemberDeclarationSyntax> (
									SyntaxFactory.ClassDeclaration ("Foo")
										.WithModifiers (
											SyntaxFactory.TokenList (
												SyntaxFactory.Token (SyntaxKind.PublicKeyword))))),
						SyntaxFactory.NamespaceDeclaration (
								SyntaxFactory.IdentifierName ("TestBar"))
							.WithMembers (
								SyntaxFactory.SingletonList<MemberDeclarationSyntax> (
									SyntaxFactory.ClassDeclaration ("Bar")
										.WithModifiers (
											SyntaxFactory.TokenList (
												SyntaxFactory.Token (SyntaxKind.PublicKeyword)))))
					}))
			.NormalizeWhitespace ();
		// get the first namespace, get the class and assert that only the first namespace is used
		var nsDeclaration = compilationUnit.SyntaxTree.GetRoot ()
			.DescendantNodes ()
			.OfType<NamespaceDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (nsDeclaration);
		var declaration = nsDeclaration.DescendantNodes ()
			.OfType<ClassDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (declaration);
		Assert.Equal ("Test.Foo", declaration.GetFullyQualifiedIdentifier ());
	}

	[Fact]
	public void GetFullyQualifiedIdentifierNestedNamespaceDeclaration ()
	{
		var compilationUnit = SyntaxFactory.CompilationUnit ()
			.WithMembers (
				SyntaxFactory.SingletonList<MemberDeclarationSyntax> (
					SyntaxFactory.NamespaceDeclaration (
							SyntaxFactory.IdentifierName ("Foo"))
						.WithMembers (
							SyntaxFactory.SingletonList<MemberDeclarationSyntax> (
								SyntaxFactory.NamespaceDeclaration (
										SyntaxFactory.IdentifierName ("Bar"))
									.WithMembers (
										SyntaxFactory.SingletonList<MemberDeclarationSyntax> (
											SyntaxFactory.ClassDeclaration ("Test")
												.WithModifiers (
													SyntaxFactory.TokenList (
														SyntaxFactory.Token (SyntaxKind.PublicKeyword)))))))))
			.NormalizeWhitespace ();
		var declaration = compilationUnit.SyntaxTree.GetRoot ()
			.DescendantNodes ()
			.OfType<ClassDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (declaration);
		Assert.Equal ("Foo.Bar.Test", declaration.GetFullyQualifiedIdentifier ());
	}

	[Fact]
	public void GetFullyQualifiedIdentifierNamespaceDeclarationNestedClass ()
	{
		var compilationUnit = SyntaxFactory.CompilationUnit ()
			.WithMembers (
				SyntaxFactory.SingletonList<MemberDeclarationSyntax> (
					SyntaxFactory.NamespaceDeclaration (
							SyntaxFactory.IdentifierName ("Foo"))
						.WithMembers (
							SyntaxFactory.SingletonList<MemberDeclarationSyntax> (
								SyntaxFactory.NamespaceDeclaration (
										SyntaxFactory.IdentifierName ("Bar"))
									.WithMembers (
										SyntaxFactory.SingletonList<MemberDeclarationSyntax> (
											SyntaxFactory.ClassDeclaration ("Test")
												.WithModifiers (
													SyntaxFactory.TokenList (
														SyntaxFactory.Token (SyntaxKind.PublicKeyword)))
												.WithMembers (
													SyntaxFactory.SingletonList<MemberDeclarationSyntax> (
														SyntaxFactory.ClassDeclaration ("Final")
															.WithModifiers (
																SyntaxFactory.TokenList (
																	SyntaxFactory.Token (SyntaxKind
																		.PublicKeyword)))))))))))
			.NormalizeWhitespace ();
		var declaration = compilationUnit.SyntaxTree.GetRoot ()
			.DescendantNodes ()
			.OfType<ClassDeclarationSyntax> ()
			.LastOrDefault ();
		Assert.NotNull (declaration);
		Assert.Equal ("Foo.Bar.Test.Final", declaration.GetFullyQualifiedIdentifier ());
	}
}
