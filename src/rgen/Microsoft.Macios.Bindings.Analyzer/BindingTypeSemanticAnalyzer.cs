using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Microsoft.Macios.Bindings.Analyzer;

/// <summary>
/// Analyzer that ensures that the types that have been declared as binding types are partial and follow the correct
/// pattern.
/// </summary>
[DiagnosticAnalyzer (LanguageNames.CSharp)]
public class BindingTypeSemanticAnalyzer : DiagnosticAnalyzer {
	internal const string DiagnosticId = "RBI0001";
	static readonly LocalizableString Title = new LocalizableResourceString (nameof (Resources.RBI0001Title),
		Resources.ResourceManager, typeof (Resources));
	static readonly LocalizableString MessageFormat =
		new LocalizableResourceString (nameof (Resources.RBI0001MessageFormat), Resources.ResourceManager,
			typeof (Resources));
	static readonly LocalizableString Description =
		new LocalizableResourceString (nameof (Resources.RBI0001Description), Resources.ResourceManager,
			typeof (Resources));
	const string Category = "Usage";

	static readonly DiagnosticDescriptor RBI0001 = new (DiagnosticId, Title, MessageFormat, Category,
		DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description);

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
		ImmutableArray.Create (RBI0001);

	public override void Initialize (AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis (GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution ();
		context.RegisterSyntaxNodeAction (AnalysisContext, SyntaxKind.ClassDeclaration);
	}

	void AnalysisContext (SyntaxNodeAnalysisContext context)
	{
		// only care about classes
		if (context.Node is not ClassDeclarationSyntax classDeclarationNode)
			return;

		var classSymbol = context.SemanticModel.GetDeclaredSymbol (classDeclarationNode);
		if (classSymbol is null)
			return;

		var boundAttributes = classSymbol.GetAttributes ();
		if (boundAttributes.Length == 0) {
			return;
		}

		// the c# syntax is a a list of lists of attributes. That is why we need to iterate through the list of lists
		foreach (var attributeData in boundAttributes) {
			// based on the type use the correct parser to retrieve the data
			var attributeType = attributeData.AttributeClass?.ToDisplayString ();
			switch (attributeType) {
			case "ObjCBindings.BindingTypeAttribute":
				// validate that the class is partial, else we need to report an error
				if (!classDeclarationNode.Modifiers.Any (x => x.IsKind (SyntaxKind.PartialKeyword))) {
					var diagnostic = Diagnostic.Create (RBI0001,
						classDeclarationNode.Identifier.GetLocation (), // point to where the 'class' keyword is used
						classSymbol.ToDisplayString ());
					context.ReportDiagnostic (diagnostic);
				}
				break;
			}
		}
	}
}
