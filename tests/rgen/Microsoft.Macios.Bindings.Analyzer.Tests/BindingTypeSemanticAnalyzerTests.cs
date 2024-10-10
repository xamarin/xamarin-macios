using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Xamarin.Utils;
using Xunit;

namespace Microsoft.Macios.Bindings.Analyzer.Tests;

public class BindingTypeSemanticAnalyzerTests : BaseGeneratorWithAnalyzerTestClass{

	[Theory]
	[InlineData (ApplePlatform.iOS)]
	[InlineData (ApplePlatform.TVOS)]
	[InlineData (ApplePlatform.MacOSX)]
	[InlineData (ApplePlatform.MacCatalyst)]
	public async Task BindingTypeMustBePartial (ApplePlatform platform)
	{
		const string inputText = @"
namespace Test {
	[BindingType]
	public class Examples {
	}
}
";

		var compilation = CreateCompilation (nameof (CompareGeneratedCode), platform, inputText);
		var diagnostics = await RunAnalyzer (new BindingTypeSemanticAnalyzer (), compilation);
		Assert.Single (diagnostics);
		// verify the diagnostic message
		var location = diagnostics [0].Location;
		VerifyDiagnosticMessage (diagnostics[0], BindingTypeSemanticAnalyzer.DiagnosticId,
			DiagnosticSeverity.Error, "The binding type 'Test.Examples' must declared as a partial class");
	}
}
