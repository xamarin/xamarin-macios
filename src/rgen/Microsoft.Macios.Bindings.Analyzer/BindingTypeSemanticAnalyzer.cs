using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.Macios.Bindings.Analyzer.Extensions;

namespace Microsoft.Macios.Bindings.Analyzer;

/// <summary>
/// Analyzer that ensures that the types that have been declared as binding types are partial and follow the correct
/// pattern.
/// </summary>
[DiagnosticAnalyzer (LanguageNames.CSharp)]
public class BindingTypeSemanticAnalyzer : DiagnosticAnalyzer, IBindingTypeAnalyzer<ClassDeclarationSyntax> {

	internal static readonly DiagnosticDescriptor RBI0001 = new (
		"RBI0001",
		new LocalizableResourceString (nameof (Resources.RBI0001Title), Resources.ResourceManager, typeof (Resources)),
		new LocalizableResourceString (nameof (Resources.RBI0001MessageFormat), Resources.ResourceManager, typeof (Resources)),
		"Usage",
		DiagnosticSeverity.Error,
		isEnabledByDefault: true,
		description: new LocalizableResourceString (nameof (Resources.RBI0001Description), Resources.ResourceManager, typeof (Resources))
	);

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [RBI0001];

	public override void Initialize (AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis (GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution ();
		context.RegisterSyntaxNodeAction (AnalysisContext, SyntaxKind.ClassDeclaration);
	}

	void AnalysisContext (SyntaxNodeAnalysisContext context)
		=> this.AnalyzeBindingType (context);

	public ImmutableArray<Diagnostic> Analyze (PlatformName _, ClassDeclarationSyntax declarationNode, INamedTypeSymbol symbol)
	{
		if (declarationNode.Modifiers.Any (x => x.IsKind (SyntaxKind.PartialKeyword)))
			return [];

		var diagnostic = Diagnostic.Create (RBI0001,
			declarationNode.Identifier.GetLocation (), // point to where the 'class' keyword is used
			symbol.ToDisplayString ());
		return [diagnostic];

	}
}
