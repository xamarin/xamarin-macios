using System.Collections;
using System.Collections.Generic;
using Xamarin.Tests;
using Xamarin.Utils;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.Classes;

public class ClassGenerationTests : BaseGeneratorTestClass {
	
	public class TestDataGenerator : BaseTestDataGenerator, IEnumerable<object []> {
		readonly List<(ApplePlatform Platform, string ClassName, string BindingFile, string OutputFile, string? LibraryText)> _data = new ()
		{
			(ApplePlatform.iOS, "AVAudioPcmBuffer", "AVAudioPcmBufferNoDefaultCtr.cs", "ExpectedAVAudioPcmBufferNoDefaultCtr.cs", null),
			(ApplePlatform.MacOSX, "AVAudioPcmBuffer", "AVAudioPcmBufferNoDefaultCtr.cs", "ExpectedAVAudioPcmBufferNoDefaultCtr.cs", null),
			(ApplePlatform.iOS, "AVAudioPcmBuffer", "AVAudioPcmBufferDefaultCtr.cs", "ExpectedAVAudioPcmBufferDefaultCtr.cs", null),
			(ApplePlatform.MacOSX, "AVAudioPcmBuffer", "AVAudioPcmBufferDefaultCtr.cs", "ExpectedAVAudioPcmBufferDefaultCtr.cs", null),
			(ApplePlatform.iOS, "AVAudioPcmBuffer", "AVAudioPcmBufferNoNativeName.cs", "ExpectedAVAudioPcmBufferNoNativeName.cs", null),
			(ApplePlatform.MacOSX, "AVAudioPcmBuffer", "AVAudioPcmBufferNoNativeName.cs", "ExpectedAVAudioPcmBufferNoNativeName.cs", null),

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
	public void GenerationTests (ApplePlatform platform, string className, string inputFileName, string inputText, string outputFileName, string expectedOutputText, string? expectedLibraryText)
		=> CompareGeneratedCode (platform, className, inputFileName, inputText, outputFileName, expectedOutputText, expectedLibraryText);
	
}
