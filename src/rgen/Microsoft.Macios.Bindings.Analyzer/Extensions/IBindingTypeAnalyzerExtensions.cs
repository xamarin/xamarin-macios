using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.Macios.Generator;
using Microsoft.Macios.Generator.Extensions;
using Diagnostic = Microsoft.CodeAnalysis.Diagnostic;

namespace Microsoft.Macios.Bindings.Analyzer.Extensions;

public static class BindingTypeAnalyzerExtensions {
	public static void AnalyzeBindingType<T> (this IBindingTypeAnalyzer<T> self, SyntaxNodeAnalysisContext context) where T : BaseTypeDeclarationSyntax
	{
		// calculate the current compilation platform name
		if (context.Node is not T declarationNode)
			return;

		var declaredSymbol = context.SemanticModel.GetDeclaredSymbol (declarationNode);
		if (declaredSymbol is null)
			return;

		var boundAttributes = declaredSymbol.GetAttributes ();
		if (boundAttributes.Length == 0) {
			// do nothing since our generator only cares about declared types with the BindingType attribute
			return;
		}

		// the c# syntax is a a list of lists of attributes. That is why we need to iterate through the list of lists
		foreach (var attributeData in boundAttributes) {
			// based on the type use the correct parser to retrieve the data
			var attributeType = attributeData.AttributeClass?.ToDisplayString ();
			switch (attributeType) {
			case AttributesNames.BindingAttribute:
				// validate that the class is partial, else we need to report an error
				var diagnostics = self.Analyze (context.Compilation.GetCurrentPlatform (),
					declarationNode, declaredSymbol);
				foreach (var diagnostic in diagnostics)
					context.ReportDiagnostic (diagnostic);
				break;
			}
		}
	}
}
