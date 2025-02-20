// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.Macios.Generator;
using Microsoft.Macios.Generator.Tests;
using Xunit;

namespace Microsoft.Macios.Bindings.Analyzer.Tests;

public class BaseGeneratorWithAnalyzerTestClass : BaseGeneratorTestClass {

	protected Task<ImmutableArray<Diagnostic>> RunAnalyzer<T> (T analyzer, Compilation compilation)
		where T : DiagnosticAnalyzer
	{

		var driver = CSharpGeneratorDriver.Create (new BindingSourceGeneratorGenerator ());
		var compilationWithAnalyzers =
			// run generators on the compilation
			RunGeneratorsAndUpdateCompilation (driver, compilation, out _)
				// attach analyzers
				.WithAnalyzers (ImmutableArray.Create<DiagnosticAnalyzer> (analyzer));
		return compilationWithAnalyzers.GetAllDiagnosticsAsync ();
	}

	protected static void VerifyDiagnosticMessage (Diagnostic diagnostic, string diagnosticId,
		DiagnosticSeverity severity, string message)
	{
		Assert.Equal (diagnosticId, diagnostic.Id);
		Assert.Equal (severity, diagnostic.Severity);
		Assert.Equal (message, diagnostic.GetMessage ());
	}
	protected static void VerifyDiagnosticMessage (Diagnostic diagnostic, string diagnosticId, string message)
	{
		Assert.Equal (diagnosticId, diagnostic.Id);
		Assert.Equal (message, diagnostic.GetMessage ());
	}
}
