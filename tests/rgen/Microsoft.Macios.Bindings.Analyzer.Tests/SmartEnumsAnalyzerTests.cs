// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Xamarin.Tests;
using Xamarin.Utils;
using Xunit;

namespace Microsoft.Macios.Bindings.Analyzer.Tests;

public class SmartEnumsAnalyzerTests : BaseGeneratorWithAnalyzerTestClass {
	[Theory]
	[AllSupportedPlatforms]
	public async Task SmartEnumMustHaveFieldAttribute (ApplePlatform platform)
	{
		const string inputText = @"
using Foundation;
using ObjCRuntime;
using ObjCBindings;

namespace AVFoundation;

[BindingType]
public enum AVCaptureSystemPressureExampleLevel {
	[Field<EnumValue> (""AVCaptureSystemPressureLevelNominal"")]
	Nominal,

	[Field<EnumValue> (""AVCaptureSystemPressureLevelFair"")]
	Fair,

	[Field<EnumValue> (""AVCaptureSystemPressureLevelSerious"")]
	Serious,

	[Field<EnumValue> (""AVCaptureSystemPressureLevelCritical"")]
	Critical,

	// missing field attribute, should be an error
	Shutdown,
}
";

		var (compilation, _) = CreateCompilation (platform, sources: inputText);
		var diagnostics = await RunAnalyzer (new SmartEnumsAnalyzer (), compilation);
		var analyzerDiagnotics = diagnostics
			.Where (d => d.Id == SmartEnumsAnalyzer.RBI0002.Id).ToArray ();
		Assert.Single (analyzerDiagnotics);
		// verify the diagnostic message
		VerifyDiagnosticMessage (analyzerDiagnotics [0], SmartEnumsAnalyzer.RBI0002.Id,
			DiagnosticSeverity.Error,
			"The enum value 'AVFoundation.AVCaptureSystemPressureExampleLevel.Shutdown' must be tagged with a Field<EnumValue> attribute");
	}

	const string emptyIdentifier = @"
using Foundation;
using ObjCRuntime;
using ObjCBindings;

namespace AVFoundation;

[BindingType]
public enum AVCaptureSystemPressureExampleLevel {
	[Field<EnumValue> (""       "")]
	Shutdown,
}";

	const string notValidIdentifierQuotes = @"
using Foundation;
using ObjCRuntime;
using ObjCBindings;

namespace AVFoundation;

[BindingType]
public enum AVCaptureSystemPressureExampleLevel {
	// empty field, this should be an error
	[Field<EnumValue> (""Weird\""Name"")]
	Shutdown,
}";

	const string notValidIdentifierKeyword = @"
using Foundation;
using ObjCRuntime;
using ObjCBindings;

namespace AVFoundation;

[BindingType]
public enum AVCaptureSystemPressureExampleLevel {
	// empty field, this should be an error
	[Field<EnumValue> (""class"")]
	Shutdown,
}";

	const string notValidIdentifierStartNumber = @"
using Foundation;
using ObjCRuntime;
using ObjCBindings;

namespace AVFoundation;

[BindingType]
public enum AVCaptureSystemPressureExampleLevel {
	// empty field, this should be an error
	[Field<EnumValue> (""42Values"")]
	Shutdown,
}";

	const string notValidIdentifierNewLines = @"
using Foundation;
using ObjCRuntime;
using ObjCBindings;

namespace AVFoundation;

[BindingType]
public enum AVCaptureSystemPressureExampleLevel {
	// empty field, this should be an error
	[Field<EnumValue> (""The\nValues"")]
	Shutdown,
}";

	[Theory]
	[AllSupportedPlatforms (emptyIdentifier, "       ")]
	[AllSupportedPlatforms (notValidIdentifierQuotes, "Weird\"Name")]
	[AllSupportedPlatforms (notValidIdentifierKeyword, "class")]
	[AllSupportedPlatforms (notValidIdentifierStartNumber, "42Values")]
	[AllSupportedPlatforms (notValidIdentifierNewLines, "The\nValues")]
	public async Task SmartEnumSymbolMustBeValidIdentifier (ApplePlatform platform, string inputText, string fieldValue)
	{
		var (compilation, _) = CreateCompilation (platform, sources: inputText);
		var diagnostics = await RunAnalyzer (new SmartEnumsAnalyzer (), compilation);
		var analyzerDiagnotics = diagnostics
			.Where (d => d.Id == SmartEnumsAnalyzer.RBI0004.Id).ToArray ();
		Assert.Single (analyzerDiagnotics);
		VerifyDiagnosticMessage (analyzerDiagnotics [0], SmartEnumsAnalyzer.RBI0004.Id,
			DiagnosticSeverity.Error,
			$"The enum value 'AVFoundation.AVCaptureSystemPressureExampleLevel.Shutdown' backing field '{fieldValue}' is not a valid identifier");
	}

	const string appleFrameworkLib = @"
using Foundation;
using ObjCRuntime;
using ObjCBindings;

namespace AVFoundation;

[BindingType]
public enum AVCaptureSystemPressureExampleLevel {
	[Field<EnumValue> (""AVCaptureSystemPressureLevelNominal"")]
	Nominal,

	[Field<EnumValue> (""AVCaptureSystemPressureLevelFair"")]
	Fair,

	[Field<EnumValue> (""AVCaptureSystemPressureLevelSerious"")]
	Serious,

	[Field<EnumValue> (""AVCaptureSystemPressureLevelCritical"")]
	Critical,

	// do not do this with apple frameworks
	[Field<EnumValue> (""AVCaptureSystemPressureLevelShutdown"", ""/path/to/not/needed/lib"")]
	Shutdown,
}";

	const string appleFrameworkLibNamedParameter = @"
using Foundation;
using ObjCRuntime;
using ObjCBindings;

namespace AVFoundation;

[BindingType]
public enum AVCaptureSystemPressureExampleLevel {
	[Field<EnumValue> (""AVCaptureSystemPressureLevelNominal"")]
	Nominal,

	[Field<EnumValue> (""AVCaptureSystemPressureLevelFair"")]
	Fair,

	[Field<EnumValue> (""AVCaptureSystemPressureLevelSerious"")]
	Serious,

	[Field<EnumValue> (""AVCaptureSystemPressureLevelCritical"")]
	Critical,

	// do not do this with apple frameworks
	[Field<EnumValue> (""AVCaptureSystemPressureLevelShutdown"", LibraryName = ""/path/to/not/needed/lib"")]
	Shutdown,
}";

	[Theory]
	[AllSupportedPlatforms (appleFrameworkLib)]
	[AllSupportedPlatforms (appleFrameworkLibNamedParameter)]
	public async Task SmartEnumAppleFrameworkNotLibrary (ApplePlatform platform, string inputText)
	{
		var (compilation, _) = CreateCompilation (platform, sources: inputText);
		var diagnostics = await RunAnalyzer (new SmartEnumsAnalyzer (), compilation);

		var analyzerDiagnotics = diagnostics
			.Where (d => d.Id == SmartEnumsAnalyzer.RBI0006.Id).ToArray ();
		Assert.Single (analyzerDiagnotics);
		VerifyDiagnosticMessage (analyzerDiagnotics [0], SmartEnumsAnalyzer.RBI0006.Id,
			DiagnosticSeverity.Warning,
			"The Field attribute for the enum value 'AVFoundation.AVCaptureSystemPressureExampleLevel.Shutdown' must not provide a value for 'LibraryName'");
	}

	const string customLibraryMissingLibraryName = @"
using System;
using System.Runtime.Versioning;
using Foundation;
using ObjCRuntime;
using ObjCBindings;

namespace CustomLibrary;

[BindingType]
public enum CustomLibraryEnum {
	[Field<EnumValue> (""None"", ""/path/to/customlibrary.framework"")]
	None,
	[Field<EnumValue> (""Medium"", ""/path/to/customlibrary.framework"")]
	Medium,
	// missing lib, this is an error
	[Field<EnumValue> (""High"")]
	High,
}
";

	const string customLibraryEmptyLibraryName = @"
using System;
using System.Runtime.Versioning;
using Foundation;
using ObjCRuntime;
using ObjCBindings;

namespace CustomLibrary;

[BindingType]
public enum CustomLibraryEnum {
	[Field<EnumValue> (""None"", ""/path/to/customlibrary.framework"")]
	None,
	[Field<EnumValue> (""Medium"", ""/path/to/customlibrary.framework"")]
	Medium,
	// empty lib, this is an error
	[Field<EnumValue> (""High"", ""   "")]
	High,
}
";

	const string customLibraryEmptyLibraryNameParameter = @"
using System;
using System.Runtime.Versioning;
using Foundation;
using ObjCRuntime;
using ObjCBindings;

namespace CustomLibrary;

[BindingType]
public enum CustomLibraryEnum {
	[Field<EnumValue> (""None"", ""/path/to/customlibrary.framework"")]
	None,
	[Field<EnumValue> (""Medium"", ""/path/to/customlibrary.framework"")]
	Medium,
	// empty lib, this is an error
	[Field<EnumValue> (""High"", LibraryName = ""   "")]
	High,
}
";

	[Theory]
	[AllSupportedPlatforms (customLibraryMissingLibraryName)]
	[AllSupportedPlatforms (customLibraryEmptyLibraryName)]
	[AllSupportedPlatforms (customLibraryEmptyLibraryNameParameter)]
	public async Task SmartEnumThirdPartyLibrary (ApplePlatform platform, string inputText)
	{
		var (compilation, _) = CreateCompilation (platform, sources: inputText);
		var diagnostics = await RunAnalyzer (new SmartEnumsAnalyzer (), compilation);
		var analyzerDiagnotics = diagnostics
			.Where (d => d.Id == SmartEnumsAnalyzer.RBI0005.Id).ToArray ();
		Assert.Single (analyzerDiagnotics);
		VerifyDiagnosticMessage (analyzerDiagnotics [0], SmartEnumsAnalyzer.RBI0005.Id,
			DiagnosticSeverity.Error,
			"The field attribute for the enum value 'CustomLibrary.CustomLibraryEnum.High' must set the property 'LibraryName'");
	}

	[Theory]
	[AllSupportedPlatforms]
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
		var (compilation, _) = CreateCompilation (platform, sources: inputText);
		var diagnostics = await RunAnalyzer (new SmartEnumsAnalyzer (), compilation);
		var analyzerDiagnotics = diagnostics
			.Where (d => d.Id == SmartEnumsAnalyzer.RBI0002.Id).ToArray ();
		// we should have a diagnostic for each enum value
		Assert.Equal (3, analyzerDiagnotics.Length);
	}

	[Theory]
	[AllSupportedPlatforms]
	public async Task SmartEnumFieldNotDuplicated (ApplePlatform platform)
	{
		const string inputText = @"
using Foundation;
using ObjCRuntime;
using ObjCBindings;

namespace AVFoundation;

[BindingType]
public enum AVCaptureSystemPressureExampleLevel {
	[Field<EnumValue> (""AVCaptureSystemPressureLevelNominal"")]
	Nominal,

	// duplicated, this should be an error
	[Field<EnumValue> (""AVCaptureSystemPressureLevelNominal"")]
	Fair,

	[Field<EnumValue> (""AVCaptureSystemPressureLevelSerious"")]
	Serious,

	[Field<EnumValue> (""AVCaptureSystemPressureLevelCritical"")]
	Critical,

	[Field<EnumValue> (""AVCaptureSystemPressureLevelShutdown"")]
	Shutdown,
}";

		var (compilation, _) = CreateCompilation (platform, sources: inputText);
		var diagnostics = await RunAnalyzer (new SmartEnumsAnalyzer (), compilation);
		var analyzerDiagnotics = diagnostics
			.Where (d => d.Id == SmartEnumsAnalyzer.RBI0003.Id).ToArray ();
		Assert.Single (analyzerDiagnotics);
		VerifyDiagnosticMessage (analyzerDiagnotics [0], SmartEnumsAnalyzer.RBI0003.Id,
			DiagnosticSeverity.Error,
			"The backing field 'AVFoundation.AVCaptureSystemPressureExampleLevel.Fair' for the enum value 'AVCaptureSystemPressureLevelNominal' is already in use for the enum value 'AVFoundation.AVCaptureSystemPressureExampleLevel.Fair'");
	}

	[Theory]
	[AllSupportedPlatforms]
	public async Task SmartEnumWrongFieldAttributesFlag (ApplePlatform platform)
	{
		const string inputText = @"
using Foundation;
using ObjCRuntime;
using ObjCBindings;

namespace AVFoundation;

[BindingType]
public enum AVCaptureSystemPressureExampleLevel {
	// we are using the wrong enum value here, this should be an error
	[Field<StringComparison> (""TheField"")]
	Shutdown,
}";
		var (compilation, _) = CreateCompilation (platform, sources: inputText);
		var diagnostics = await RunAnalyzer (new SmartEnumsAnalyzer (), compilation);
		var analyzerDiagnotics = diagnostics
			.Where (d => d.Id == SmartEnumsAnalyzer.RBI0007.Id).ToArray ();
		// we should have a diagnostic for each enum value
		Assert.Single (analyzerDiagnotics);

		VerifyDiagnosticMessage (analyzerDiagnotics [0], SmartEnumsAnalyzer.RBI0007.Id,
			DiagnosticSeverity.Error,
			"Used attribute 'ObjCBindings.FieldAttribute<StringComparison>' on enum value 'AVFoundation.AVCaptureSystemPressureExampleLevel.Shutdown' when 'ObjCBindings.FieldAttribute<ObjCBindings.EnumValue>' was expected");
	}
}
