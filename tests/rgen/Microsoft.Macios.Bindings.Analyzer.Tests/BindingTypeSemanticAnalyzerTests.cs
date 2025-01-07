using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Xamarin.Tests;
using Xamarin.Utils;
using Xunit;

namespace Microsoft.Macios.Bindings.Analyzer.Tests;

public class BindingTypeSemanticAnalyzerTests : BaseGeneratorWithAnalyzerTestClass {

	[Theory]
	[AllSupportedPlatforms]
	public async Task BindingTypeMustBePartial (ApplePlatform platform)
	{
		const string inputText = @"
using ObjCBindings;

namespace Test {
	[BindingType]
	public class Examples {
	}
}
";

		var (compilation, _) = CreateCompilation (platform, sources: inputText);
		var diagnostics = await RunAnalyzer (new BindingTypeSemanticAnalyzer (), compilation);
		var analyzerDiagnotics = diagnostics
			.Where (d => d.Id == BindingTypeSemanticAnalyzer.RBI0001.Id).ToArray ();
		Assert.Single (analyzerDiagnotics);
		// verify the diagnostic message
		VerifyDiagnosticMessage (analyzerDiagnotics [0], BindingTypeSemanticAnalyzer.RBI0001.Id,
			DiagnosticSeverity.Error, "The binding type 'Test.Examples' must declared as a partial class");
	}
}
