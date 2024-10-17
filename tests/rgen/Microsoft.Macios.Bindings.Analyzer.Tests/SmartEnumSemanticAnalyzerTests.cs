using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Xamarin.Tests;
using Xamarin.Utils;
using Xunit;

namespace Microsoft.Macios.Bindings.Analyzer.Tests;

public class SmartEnumSemanticAnalyzerTests : BaseGeneratorWithAnalyzerTestClass {

	[Theory]
	[PlatformInlineData (ApplePlatform.iOS)]
	[PlatformInlineData (ApplePlatform.TVOS)]
	[PlatformInlineData (ApplePlatform.MacOSX)]
	[PlatformInlineData (ApplePlatform.MacCatalyst)]
	public async Task SmartEnumMustHaveFieldAttribute (ApplePlatform platform)
	{
		const string inputText = @"
using Foundation;
using ObjCRuntime;
using ObjCBindings;

namespace AVFoundation;

[BindingType]
public enum AVCaptureSystemPressureExampleLevel {
	[Field (""AVCaptureSystemPressureLevelNominal"")]
	Nominal,

	[Field (""AVCaptureSystemPressureLevelFair"")]
	Fair,

	[Field (""AVCaptureSystemPressureLevelSerious"")]
	Serious,

	[Field (""AVCaptureSystemPressureLevelCritical"")]
	Critical,

	// missing field attribute, should be an error
	Shutdown,
}
";

		var compilation = CreateCompilation (nameof (SmartEnumSemanticAnalyzerTests), platform, inputText);
		var diagnostics = await RunAnalyzer (new SmartEnumSemanticAnalyzer (), compilation);
		var analyzerDiagnotics = diagnostics
			.Where (d => d.Id == SmartEnumSemanticAnalyzer.RBI0002.Id).ToArray ();
		Assert.Single (analyzerDiagnotics);
		// verify the diagnostic message
		VerifyDiagnosticMessage (analyzerDiagnotics [0], SmartEnumSemanticAnalyzer.RBI0002.Id,
			DiagnosticSeverity.Error, "The enum value 'AVFoundation.AVCaptureSystemPressureExampleLevel.Shutdown' must be tagged with a FieldAttribute.");
	}

	[Theory]
	[PlatformInlineData (ApplePlatform.iOS)]
	[PlatformInlineData (ApplePlatform.TVOS)]
	[PlatformInlineData (ApplePlatform.MacOSX)]
	[PlatformInlineData (ApplePlatform.MacCatalyst)]
	public async Task SmartEnumSymbolMustBeCorrect (ApplePlatform platform)
	{
		const string inputText = @"
using Foundation;
using ObjCRuntime;
using ObjCBindings;

namespace AVFoundation;

[BindingType]
public enum AVCaptureSystemPressureExampleLevel {
	[Field (""AVCaptureSystemPressureLevelNominal"")]
	Nominal,

	[Field (""AVCaptureSystemPressureLevelFair"")]
	Fair,

	[Field (""AVCaptureSystemPressureLevelSerious"")]
	Serious,

	[Field (""AVCaptureSystemPressureLevelCritical"")]
	Critical,

	// empty field, this should be an error
	[Field (""       "")]
	Shutdown,
}";

		var compilation = CreateCompilation (nameof (SmartEnumSemanticAnalyzerTests), platform, inputText);
		var diagnostics = await RunAnalyzer (new SmartEnumSemanticAnalyzer (), compilation);
		var analyzerDiagnotics = diagnostics
			.Where (d => d.Id == SmartEnumSemanticAnalyzer.RBI0003.Id).ToArray ();
		Assert.Single (analyzerDiagnotics);
		VerifyDiagnosticMessage (analyzerDiagnotics [0], SmartEnumSemanticAnalyzer.RBI0003.Id,
			DiagnosticSeverity.Error, "The enum value 'AVFoundation.AVCaptureSystemPressureExampleLevel.Shutdown' backing field is an empty string.");
	}

	const string AppleFrameworkLib = @"
using Foundation;
using ObjCRuntime;
using ObjCBindings;

namespace AVFoundation;

[BindingType]
public enum AVCaptureSystemPressureExampleLevel {
	[Field (""AVCaptureSystemPressureLevelNominal"")]
	Nominal,

	[Field (""AVCaptureSystemPressureLevelFair"")]
	Fair,

	[Field (""AVCaptureSystemPressureLevelSerious"")]
	Serious,

	[Field (""AVCaptureSystemPressureLevelCritical"")]
	Critical,

	// do not do this with apple frameworks
	[Field (""AVCaptureSystemPressureLevelShutdown"", ""/path/to/not/needed/lib"")]
	Shutdown,
}";

	const string AppleFrameworkLibNamedParameter = @"
using Foundation;
using ObjCRuntime;
using ObjCBindings;

namespace AVFoundation;

[BindingType]
public enum AVCaptureSystemPressureExampleLevel {
	[Field (""AVCaptureSystemPressureLevelNominal"")]
	Nominal,

	[Field (""AVCaptureSystemPressureLevelFair"")]
	Fair,

	[Field (""AVCaptureSystemPressureLevelSerious"")]
	Serious,

	[Field (""AVCaptureSystemPressureLevelCritical"")]
	Critical,

	// do not do this with apple frameworks
	[Field (""AVCaptureSystemPressureLevelShutdown"", LibraryName = ""/path/to/not/needed/lib"")]
	Shutdown,
}";

	[Theory]
	[PlatformInlineData (ApplePlatform.iOS, AppleFrameworkLib)]
	[PlatformInlineData (ApplePlatform.iOS, AppleFrameworkLibNamedParameter)]
	[PlatformInlineData (ApplePlatform.TVOS, AppleFrameworkLib)]
	[PlatformInlineData (ApplePlatform.TVOS, AppleFrameworkLibNamedParameter)]
	[PlatformInlineData (ApplePlatform.MacOSX, AppleFrameworkLib)]
	[PlatformInlineData (ApplePlatform.MacOSX, AppleFrameworkLibNamedParameter)]
	[PlatformInlineData (ApplePlatform.MacCatalyst, AppleFrameworkLib)]
	[PlatformInlineData (ApplePlatform.MacCatalyst, AppleFrameworkLibNamedParameter)]
	public async Task SmartEnumAppleFrameworkNotLibrary (ApplePlatform platform, string inputText)
	{
		var compilation = CreateCompilation (nameof (SmartEnumSemanticAnalyzerTests), platform, inputText);
		var diagnostics = await RunAnalyzer (new SmartEnumSemanticAnalyzer(), compilation);

		var analyzerDiagnotics = diagnostics
			.Where (d => d.Id == SmartEnumSemanticAnalyzer.RBI0005.Id).ToArray ();
		Assert.Single (analyzerDiagnotics);
		VerifyDiagnosticMessage (analyzerDiagnotics [0], SmartEnumSemanticAnalyzer.RBI0005.Id,
			DiagnosticSeverity.Warning, "The backing Field of 'AVFoundation.AVCaptureSystemPressureExampleLevel.Shutdown' should not provide a LibraryName.");
	}

	const string CustomLibraryMissingLibraryName = @"
using System;
using System.Runtime.Versioning;
using Foundation;
using ObjCRuntime;
using ObjCBindings;

namespace CustomLibrary;

[BindingType]
public enum CustomLibraryEnum {
	[Field (""None"", ""/path/to/customlibrary.framework"")]
	None,
	[Field (""Medium"", ""/path/to/customlibrary.framework"")]
	Medium,
	// missing lib, this is an error
	[Field (""High"")]
	High,
}
";
	const string CustomLibraryEmptyLibraryName = @"
using System;
using System.Runtime.Versioning;
using Foundation;
using ObjCRuntime;
using ObjCBindings;

namespace CustomLibrary;

[BindingType]
public enum CustomLibraryEnum {
	[Field (""None"", ""/path/to/customlibrary.framework"")]
	None,
	[Field (""Medium"", ""/path/to/customlibrary.framework"")]
	Medium,
	// empty lib, this is an error
	[Field (""High"", ""   "")]
	High,
}
";

	const string CustomLibraryEmptyLibraryNameParameter = @"
using System;
using System.Runtime.Versioning;
using Foundation;
using ObjCRuntime;
using ObjCBindings;

namespace CustomLibrary;

[BindingType]
public enum CustomLibraryEnum {
	[Field (""None"", ""/path/to/customlibrary.framework"")]
	None,
	[Field (""Medium"", ""/path/to/customlibrary.framework"")]
	Medium,
	// empty lib, this is an error
	[Field (""High"", LibraryName = ""   "")]
	High,
}
";

	[Theory]
	[PlatformInlineData (ApplePlatform.iOS, CustomLibraryMissingLibraryName)]
	[PlatformInlineData (ApplePlatform.iOS, CustomLibraryEmptyLibraryName)]
	[PlatformInlineData (ApplePlatform.iOS, CustomLibraryEmptyLibraryNameParameter)]
	[PlatformInlineData (ApplePlatform.TVOS, CustomLibraryMissingLibraryName)]
	[PlatformInlineData (ApplePlatform.TVOS, CustomLibraryEmptyLibraryName)]
	[PlatformInlineData (ApplePlatform.TVOS, CustomLibraryEmptyLibraryNameParameter)]
	[PlatformInlineData (ApplePlatform.MacOSX, CustomLibraryMissingLibraryName)]
	[PlatformInlineData (ApplePlatform.MacOSX, CustomLibraryEmptyLibraryName)]
	[PlatformInlineData (ApplePlatform.MacOSX, CustomLibraryEmptyLibraryNameParameter)]
	[PlatformInlineData (ApplePlatform.MacCatalyst, CustomLibraryMissingLibraryName)]
	[PlatformInlineData (ApplePlatform.MacCatalyst, CustomLibraryEmptyLibraryName)]
	[PlatformInlineData (ApplePlatform.MacCatalyst, CustomLibraryEmptyLibraryNameParameter)]
	public async Task SmartEnumThirdPartyLibrary (ApplePlatform platform, string inputText)
	{
		var compilation = CreateCompilation (nameof (SmartEnumSemanticAnalyzerTests), platform, inputText);
		var diagnostics = await RunAnalyzer (new SmartEnumSemanticAnalyzer(), compilation);
		var analyzerDiagnotics = diagnostics
			.Where (d => d.Id == SmartEnumSemanticAnalyzer.RBI0004.Id).ToArray ();
		Assert.Single (analyzerDiagnotics);
		VerifyDiagnosticMessage (analyzerDiagnotics [0], SmartEnumSemanticAnalyzer.RBI0004.Id,
			DiagnosticSeverity.Error, "The enum value 'CustomLibrary.CustomLibraryEnum.High' backing field must provide a library name.");
	}

	[Theory]
	[PlatformInlineData (ApplePlatform.iOS)]
	[PlatformInlineData (ApplePlatform.TVOS)]
	[PlatformInlineData (ApplePlatform.MacOSX)]
	[PlatformInlineData (ApplePlatform.MacCatalyst)]
	public async Task SmartEnumNoFieldAttributes (ApplePlatform platform)
	{
		const string inputText = @"
using System;
using System.Runtime.Versioning;
using Foundation;
using ObjCRuntime;
using ObjCBindings;

namespace CustomLibrary;

[BindingType]
public enum CustomLibraryEnum {
	None,
	Medium,
	High,
}
";
		var compilation = CreateCompilation (nameof (SmartEnumSemanticAnalyzerTests), platform, inputText);
		var diagnostics = await RunAnalyzer (new SmartEnumSemanticAnalyzer(), compilation);
		var analyzerDiagnotics = diagnostics
			.Where (d => d.Id == SmartEnumSemanticAnalyzer.RBI0002.Id).ToArray ();
		// we should have a diagnostic for each enum value
		Assert.Equal (3, analyzerDiagnotics.Length);
	}
}
