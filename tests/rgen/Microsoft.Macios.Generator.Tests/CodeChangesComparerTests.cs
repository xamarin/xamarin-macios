using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

namespace Microsoft.Macios.Generator.Tests;

public class CodeChangesComparerTests {
	readonly BaseTypeDeclarationSyntax syntax;
	readonly CompilationUnitSyntax compilationUnitSyntax;
	readonly CodeChangesComparer comparer;

	public CodeChangesComparerTests ()
	{
		comparer = new ();
		// no, you do not need to this by hand, visit https://roslynquoter.azurewebsites.net to generate the syntax
		compilationUnitSyntax = SyntaxFactory.CompilationUnit ()
			.WithMembers (
				SyntaxFactory.List (
					new MemberDeclarationSyntax [] {
						SyntaxFactory.PropertyDeclaration (
								SyntaxFactory.IdentifierName ("namespace"),
								SyntaxFactory.Identifier ("Test"))
							.WithAccessorList (
								SyntaxFactory.AccessorList ()
									.WithCloseBraceToken (
										SyntaxFactory.MissingToken (SyntaxKind.CloseBraceToken))),
						SyntaxFactory.EnumDeclaration ("Test")
							.WithModifiers (
								SyntaxFactory.TokenList (
									SyntaxFactory.Token (SyntaxKind.PublicKeyword)))
							.WithMembers (
								SyntaxFactory.SeparatedList<EnumMemberDeclarationSyntax> (
									new SyntaxNodeOrToken [] {
										SyntaxFactory.EnumMemberDeclaration (
											SyntaxFactory.Identifier ("First")),
										SyntaxFactory.Token (SyntaxKind.CommaToken),
										SyntaxFactory.EnumMemberDeclaration (
											SyntaxFactory.Identifier ("Second")),
										SyntaxFactory.Token (SyntaxKind.CommaToken),
										SyntaxFactory.EnumMemberDeclaration (
											SyntaxFactory.Identifier ("Last")),
										SyntaxFactory.Token (SyntaxKind.CommaToken)
									})),
						SyntaxFactory.ClassDeclaration ("MyTestClass")
							.WithModifiers (
								SyntaxFactory.TokenList (
									SyntaxFactory.Token (SyntaxKind.PublicKeyword)))
							.WithMembers (
								SyntaxFactory.SingletonList<MemberDeclarationSyntax> (
									SyntaxFactory.PropertyDeclaration (
											SyntaxFactory.PredefinedType (
												SyntaxFactory.Token (SyntaxKind.IntKeyword)),
											SyntaxFactory.Identifier ("Count"))
										.WithModifiers (
											SyntaxFactory.TokenList (
												SyntaxFactory.Token (SyntaxKind.PublicKeyword)))
										.WithAccessorList (
											SyntaxFactory.AccessorList (
												SyntaxFactory.List<AccessorDeclarationSyntax> (
													new AccessorDeclarationSyntax [] {
														SyntaxFactory.AccessorDeclaration (
																SyntaxKind.GetAccessorDeclaration)
															.WithSemicolonToken (
																SyntaxFactory.Token (SyntaxKind.SemicolonToken)),
														SyntaxFactory.AccessorDeclaration (
																SyntaxKind.SetAccessorDeclaration)
															.WithSemicolonToken (
																SyntaxFactory.Token (SyntaxKind.SemicolonToken))
													})))
										.WithInitializer (
											SyntaxFactory.EqualsValueClause (
												SyntaxFactory.LiteralExpression (
													SyntaxKind.NumericLiteralExpression,
													SyntaxFactory.Literal (0))))
										.WithSemicolonToken (
											SyntaxFactory.Token (SyntaxKind.SemicolonToken))))
							.WithCloseBraceToken (
								SyntaxFactory.Token (
									SyntaxFactory.TriviaList (),
									SyntaxKind.CloseBraceToken,
									SyntaxFactory.TriviaList (
										SyntaxFactory.Trivia (
											SyntaxFactory.SkippedTokensTrivia ()
												.WithTokens (
													SyntaxFactory.TokenList (
														SyntaxFactory.Token (SyntaxKind.CloseBraceToken)))))))
					}))
			.NormalizeWhitespace ();
		syntax = compilationUnitSyntax.SyntaxTree.GetRoot ()
			.DescendantNodes ().OfType<EnumDeclarationSyntax> ().FirstOrDefault ()!;
	}

	[Fact]
	public void CompareDiffQualifiedName ()
	{
		var codeChange1 = new CodeChanges ("codeChange1", syntax, ["First", "Second", "Last"]);
		var codeChange2 = new CodeChanges ("codeChange2", syntax, ["First", "Second", "Last"]);
		Assert.False (comparer.Equals (codeChange1, codeChange2));
	}

	[Fact]
	public void CompareDiffMembers ()
	{
		var codeChange1 = new CodeChanges ("codeChange1", syntax, ["First", "Second", "Last"]);
		var codeChange2 = new CodeChanges ("codeChange1", syntax, ["First", "Second"]);
		Assert.False (comparer.Equals (codeChange1, codeChange2));
	}

	[Fact]
	public void CompareSameMembers ()
	{
		// add a diff declaration syntax, but keep the name and the members
		var secondSyntax = compilationUnitSyntax.SyntaxTree.GetRoot ().DescendantNodes ()
			.OfType<ClassDeclarationSyntax> ().FirstOrDefault ()!;
		var codeChange1 = new CodeChanges ("codeChange1", syntax, ["First", "Second", "Last"]);
		var codeChange2 = new CodeChanges ("codeChange1", secondSyntax, ["First", "Second", "Last"]);
		Assert.True (comparer.Equals (codeChange1, codeChange2));
	}

	[Fact]
	public void CompareSameMembersUnsorted ()
	{
		var codeChange1 = new CodeChanges ("codeChange1", syntax, ["First", "Second", "Last"]);
		var codeChange2 = new CodeChanges ("codeChange1", syntax, ["Last", "First", "Second"]);
		Assert.True (comparer.Equals (codeChange1, codeChange2));
	}
}
