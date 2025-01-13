// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections;
using System.Collections.Generic;
using Xamarin.Tests;
using Xunit;
using Xamarin.Utils;

namespace Microsoft.Macios.Generator.Tests.SmartEnum;


/// <summary>
///  Test all the field generation code.
/// </summary>
public class SmartEnumDiagnosticsTests : BaseGeneratorTestClass {
	public class TestDataGenerator : BaseTestDataGenerator, IEnumerable<object []> {
		readonly List<(ApplePlatform Platform, string ClassName, string BindingFile, string OutputFile, string? LibraryText)> _data = new ()
		{
			(ApplePlatform.iOS, "AVCaptureDeviceTypeExtensions", "AVCaptureDeviceTypeEnum.cs", "ExpectedAVCaptureDeviceTypeEnum.cs", null),
			(ApplePlatform.iOS, "AVCaptureSystemPressureLevelExtensions", "AVCaptureSystemPressureLevel.cs", "ExpectedAVCaptureSystemPressureLevel.cs", null),
			(ApplePlatform.iOS, "AVMediaCharacteristicsExtensions", "AVMediaCharacteristics.cs", "ExpectediOSAVMediaCharacteristics.cs", null),
			(ApplePlatform.TVOS, "AVCaptureDeviceTypeExtensions", "AVCaptureDeviceTypeEnum.cs", "ExpectedAVCaptureDeviceTypeEnum.cs", null),
			(ApplePlatform.TVOS, "AVCaptureSystemPressureLevelExtensions", "AVCaptureSystemPressureLevel.cs", "ExpectedAVCaptureSystemPressureLevel.cs", null),
			(ApplePlatform.TVOS, "AVMediaCharacteristicsExtensions", "AVMediaCharacteristics.cs", "ExpectediOSAVMediaCharacteristics.cs", null),
			(ApplePlatform.MacCatalyst, "AVCaptureDeviceTypeExtensions", "AVCaptureDeviceTypeEnum.cs", "ExpectedAVCaptureDeviceTypeEnum.cs", null),
			(ApplePlatform.MacCatalyst, "AVCaptureSystemPressureLevelExtensions", "AVCaptureSystemPressureLevel.cs", "ExpectedAVCaptureSystemPressureLevel.cs", null),
			(ApplePlatform.MacCatalyst, "AVMediaCharacteristicsExtensions", "AVMediaCharacteristics.cs", "ExpectediOSAVMediaCharacteristics.cs", null),
			(ApplePlatform.MacOSX, "AVMediaCharacteristicsExtensions", "AVMediaCharacteristics.cs", "ExpectedMacOSAVMediaCharacteristics.cs", null),
			(ApplePlatform.MacOSX, "CustomLibraryEnumExtensions", "CustomLibraryEnum.cs", "ExpectedCustomLibraryEnum.cs", "ExpectedCustomLibraryEnumLibrariesClass.cs"),
			(ApplePlatform.MacOSX, "CustomLibraryEnumInternalExtensions", "CustomLibraryEnumInternal.cs", "ExpectedCustomLibraryEnumInternal.cs", "ExpectedCustomLibraryEnumInternalLibrariesClass.cs"),
		};

		public IEnumerator<object []> GetEnumerator ()
		{
			foreach (var testData in _data) {
				var libraryText = string.IsNullOrEmpty (testData.LibraryText) ?
					null : ReadFileAsString (testData.LibraryText);
				if (Configuration.IsEnabled (testData.Platform))
					yield return [
						testData.Platform,
						testData.ClassName,
						testData.BindingFile,
						ReadFileAsString (testData.BindingFile),
						testData.OutputFile,
						ReadFileAsString (testData.OutputFile),
						libraryText!,
					];
			}
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[ClassData (typeof (TestDataGenerator))]
	public void ExtensionGenerationTests (ApplePlatform platform, string className, string inputFileName, string inputText, string outputFileName, string expectedOutputText, string? expectedLibraryText)
		=> CompareGeneratedCode (platform, className, inputFileName, inputText, outputFileName, expectedOutputText, expectedLibraryText);

}
