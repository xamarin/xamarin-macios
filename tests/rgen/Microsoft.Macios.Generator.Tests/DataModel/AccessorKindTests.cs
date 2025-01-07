using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Macios.Generator.DataModel;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.DataModel;

public class AccessorKindTests {

	[Theory]
	[InlineData (SyntaxKind.GetAccessorDeclaration, AccessorKind.Getter)]
	[InlineData (SyntaxKind.SetAccessorDeclaration, AccessorKind.Setter)]
	[InlineData (SyntaxKind.InitAccessorDeclaration, AccessorKind.Initializer)]
	[InlineData (SyntaxKind.AddAccessorDeclaration, AccessorKind.Add)]
	[InlineData (SyntaxKind.RemoveAccessorDeclaration, AccessorKind.Remove)]
	[InlineData (SyntaxKind.Argument, AccessorKind.Unknown)]
	[InlineData (SyntaxKind.Interpolation, AccessorKind.Unknown)]
	[InlineData (SyntaxKind.None, AccessorKind.Unknown)]
	void ToAccessorKind (SyntaxKind token, AccessorKind expected)
		=> Assert.Equal (token.ToAccessorKind (), expected);
}
