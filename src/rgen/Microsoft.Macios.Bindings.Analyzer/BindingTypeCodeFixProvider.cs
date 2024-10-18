using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SyntaxFactory = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using SyntaxKind = Microsoft.CodeAnalysis.CSharp.SyntaxKind;

namespace Microsoft.Macios.Bindings.Analyzer;

/// <summary>
/// Code fix provider that adds the 'partial' modifier to the class decorated with BindingTypeAttribute.
/// </summary>
[ExportCodeFixProvider (LanguageNames.CSharp, Name = nameof (BindingTypeCodeFixProvider)), Shared]
public class BindingTypeCodeFixProvider : CodeFixProvider {
	// Specify the diagnostic IDs of analyzers that are expected to be linked.
	public sealed override ImmutableArray<string> FixableDiagnosticIds { get; } =
		ImmutableArray.Create (BindingTypeSemanticAnalyzer.RBI0001.Id);

	// If you don't need the 'fix all' behaviour, return null.
	public override FixAllProvider? GetFixAllProvider () => null;

	public sealed override async Task RegisterCodeFixesAsync (CodeFixContext context)
	{
		var diagnostic = context.Diagnostics.Single ();
		var diagnosticSpan = diagnostic.Location.SourceSpan;
		var root = await context.Document.GetSyntaxRootAsync (context.CancellationToken).ConfigureAwait (false);
		var diagnosticNode = root?.FindNode (diagnosticSpan);

		// To get the required metadata, we should match the Node to the specific type: 'ClassDeclarationSyntax'.
		if (diagnosticNode is not ClassDeclarationSyntax declaration)
			return;

		// Register a code action that will invoke the fix.
		context.RegisterCodeFix (
			CodeAction.Create (
				title: Resources.RBI0001CodeFixTitle,
				createChangedDocument: c => MakePartialClassAsync (context.Document, declaration, c),
				equivalenceKey: nameof (Resources.RBI0001CodeFixTitle)),
			diagnostic);
	}

	async Task<Document> MakePartialClassAsync (Document document,
		ClassDeclarationSyntax classDeclarationSyntax, CancellationToken cancellationToken)
	{
		var partialClass = classDeclarationSyntax.AddModifiers (SyntaxFactory.Token (SyntaxKind.PartialKeyword));
		var oldRoot = await document.GetSyntaxRootAsync (cancellationToken).ConfigureAwait (false);
		if (oldRoot is null)
			return document;

		var newRoot = oldRoot.ReplaceNode (classDeclarationSyntax, partialClass);
		return document.WithSyntaxRoot (newRoot);
	}
}
