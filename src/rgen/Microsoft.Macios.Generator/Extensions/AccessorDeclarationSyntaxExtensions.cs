using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.DataModel;

namespace Microsoft.Macios.Generator.Extensions;

static class AccessorDeclarationSyntaxExtensions {

	public static ImmutableArray<AttributeCodeChange> GetAttributeCodeChanges (this AccessorDeclarationSyntax self,
		SemanticModel semanticModel) => AttributeCodeChange.From (self.AttributeLists, semanticModel);

}
