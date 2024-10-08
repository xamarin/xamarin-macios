/*
using System.Threading.Tasks;
using Xunit;
using Verifier =
	Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<
		Microsoft.Macios.Bindings.Analyzer.BindingTypeSyntaxAnalyzer>;

namespace Microsoft.Macios.Bindings.Analyzer.Tests;

public class BindingTypeSyntaxAnalyzerTests {
	[Fact]
	public async Task ClassWithMyCompanyTitle_AlertDiagnostic ()
	{
		const string text = @"
public class MyCompanyClass
{
}
";

		var expected = Verifier.Diagnostic ()
			.WithLocation (2, 14)
			.WithArguments ("MyCompanyClass");
		await Verifier.VerifyAnalyzerAsync (text, expected).ConfigureAwait (false);
	}
}
*/
