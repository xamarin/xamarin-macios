using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Microsoft.Macios.Generator.Extensions;

static class SourceProductionContextExtensions {

	public static void ReportDiagnostics (this SourceProductionContext context, ImmutableArray<Diagnostic>? diagnostics)
	{
		if (diagnostics is null)
			return;

		foreach (Diagnostic diagnostic in diagnostics) {
			context.ReportDiagnostic (diagnostic);
		}
	}
}
