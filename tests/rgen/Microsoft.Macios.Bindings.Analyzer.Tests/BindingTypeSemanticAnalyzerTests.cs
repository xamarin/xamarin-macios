using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Xamarin.Tests;
using Xamarin.Utils;
using Xunit;

namespace Microsoft.Macios.Bindings.Analyzer.Tests;

public class BindingTypeSemanticAnalyzerTests : BaseGeneratorWithAnalyzerTestClass {

	[Theory]
	[PlatformInlineData (ApplePlatform.iOS)]
	[PlatformInlineData (ApplePlatform.TVOS)]
	[PlatformInlineData (ApplePlatform.MacOSX)]
	[PlatformInlineData (ApplePlatform.MacCatalyst)]
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

		var compilation = CreateCompilation (nameof (CompareGeneratedCode), platform, inputText);
		var diagnostics = await RunAnalyzer (new BindingTypeSemanticAnalyzer (), compilation);
		Assert.Single (diagnostics);
		// verify the diagnostic message
		var location = diagnostics [0].Location;
		VerifyDiagnosticMessage (diagnostics [0], BindingTypeSemanticAnalyzer.DiagnosticId,
			DiagnosticSeverity.Error, "The binding type 'Test.Examples' must declared as a partial class");
	}
}
