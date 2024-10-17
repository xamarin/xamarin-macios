using System.Collections;
using System.Collections.Generic;
using Xunit;
using Xamarin.Utils;

namespace Microsoft.Macios.Generator.Tests.SmartEnum;


/// <summary>
///  Test all the field generation code.
/// </summary>
public class SmartEnumTests : BaseGeneratorTestClass {
	public class TestDataGenerator : BaseTestDataGenerator, IEnumerable<object []> {
		readonly List<(ApplePlatform Platform, string ClassName, string BindingFile, string OutputFile)> _data = new ()
		{
			(ApplePlatform.iOS, "AVCaptureDeviceTypeExtensions", "AVCaptureDeviceTypeEnum.cs", "ExpectedAVCaptureDeviceTypeEnum.cs" ),
			(ApplePlatform.iOS, "AVCaptureSystemPressureLevelExtensions", "AVCaptureSystemPressureLevel.cs", "ExpectedAVCaptureSystemPressureLevel.cs" ),
			(ApplePlatform.iOS, "AVMediaCharacteristicsExtensions", "AVMediaCharacteristics.cs", "ExpectediOSAVMediaCharacteristics.cs" ),
			(ApplePlatform.MacOSX, "AVMediaCharacteristicsExtensions", "AVMediaCharacteristics.cs", "ExpectedMacOSAVMediaCharacteristics.cs" ),
			(ApplePlatform.MacOSX, "CustomLibraryEnumExtensions", "CustomLibraryEnum.cs", "ExpectedCustomLibraryEnum.cs" ),
		};

		public IEnumerator<object []> GetEnumerator ()
		{
			foreach (var testData in _data) {
				yield return [
					testData.Platform,
					testData.ClassName,
					testData.BindingFile,
					ReadFileAsString (testData.BindingFile),
					testData.OutputFile,
					ReadFileAsString (testData.OutputFile)];
			}
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[ClassData (typeof (TestDataGenerator))]
	public void ExtensionGenerationTests (ApplePlatform platform, string className, string inputFileName, string inputText, string outputFileName, string expectedOutputText)
		=> CompareGeneratedCode (platform, className, inputFileName, inputText, outputFileName, expectedOutputText);
}
