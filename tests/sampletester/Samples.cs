using System.Collections.Generic;

using NUnit.Framework;

namespace Samples {
	[Category (CATEGORY)]
	public class IosSampleTester : SampleTester {
		const string ORG = "xamarin";
		const string REPO = "ios-samples"; // monotouch-samples redirects to ios-samples
		const string CATEGORY = "iossamples"; // categories can't contain dashes
		const string HASH = "1d0f3270c394e9c15c014813e804972b17ce3e48";

		static Dictionary<string, SampleTest> test_data = new Dictionary<string, SampleTest> {
				// Build solution instead of csproj
				{ "BindingSample/XMBindingLibrarySample/XMBindingLibrarySample.csproj", new SampleTest { BuildSolution = true, Solution = "BindingSample/BindingSample.sln" } },
				{ "BouncingGameCompleteiOS/BouncingGame.iOS/BouncingGame.iOS.csproj", new SampleTest { BuildSolution = true, Solution = "BouncingGameCompleteiOS/BouncingGame.sln" } },
				{ "BouncingGameEmptyiOS/BouncingGame.iOS/BouncingGame.iOS.csproj", new SampleTest { BuildSolution = true, Solution = "BouncingGameEmptyiOS/BouncingGame.sln" } },
				{ "FileSystemSampleCode/FileSystem/FileSystem.csproj", new SampleTest { BuildSolution = true, Solution = "FileSystemSampleCode/WorkingWithTheFileSystem.sln" } },
				{ "InfColorPicker/InfColorPickerBinding/InfColorPickerSample/InfColorPickerSample.csproj", new SampleTest { BuildSolution = true, Solution = "InfColorPicker/InfColorPickerBinding/InfColorPickerBinding.sln" } },
				{ "ios10/ElizaChat/ElizaChat/ElizaChat.csproj", new SampleTest { BuildSolution = true, Solution = "ios10/ElizaChat/ElizaChat.sln" } },
				{ "ios10/IceCreamBuilder/IceCreamBuilder/IceCreamBuilder.csproj", new SampleTest { BuildSolution = true, Solution = "ios10/IceCreamBuilder/IceCreamBuilder.sln" } },
				{ "ios11/ARKitPlacingObjects/PlacingObjects/PlacingObjects.csproj", new SampleTest { BuildSolution = true, Solution = "ios11/ARKitPlacingObjects/PlacingObjects.sln" } },
				{ "ios11/WeatherWidget/WeatherWidget/WeatherWidget.csproj", new SampleTest { BuildSolution = true, Solution = "ios11/WeatherWidget/WeatherWidget.sln" } },
				{ "ios12/SoupChef/SoupChef/SoupChef.csproj", new SampleTest { BuildSolution = true, Solution = "ios12/SoupChef/SoupChef.sln" } },
				{ "ios12/XamarinShot/XamarinShot/XamarinShot.csproj", new SampleTest { BuildSolution = true, Solution = "ios12/XamarinShot/XamarinShot.sln", Platforms = new string [] { "iPhone" } } }, // Requires Metal, which doesn't work/build in the simulator.
				{ "ios8/Lister/Lister/Lister.csproj", new SampleTest { BuildSolution = true, Solution = "ios8/Lister/Lister.sln" } },
				{ "ios8/MetalBasic3D/MetalBasic3D/MetalBasic3D.csproj", new SampleTest { Platforms = new string [] { "iPhone" } } }, // Requires Metal, which doesn't work/build in the simulator.
				{ "ios8/MetalImageProcessing/MetalImageProcessing/MetalImageProcessing.csproj", new SampleTest { Platforms = new string [] { "iPhone" } } }, // Requires Metal, which doesn't work/build in the simulator.
				{ "ios8/MetalTexturedQuad/MetalTexturedQuad/MetalTexturedQuad.csproj", new SampleTest { Platforms = new string [] { "iPhone" } } }, // Requires Metal, which doesn't work/build in the simulator.
				{ "ios9/iTravel/iTravel/iTravel.csproj", new SampleTest { BuildSolution = true, Solution = "ios9/iTravel/iTravel.sln" } },
				{ "Profiling/MemoryDemo/MemoryDemo/MemoryDemo.csproj", new SampleTest { BuildSolution = true, Solution = "Profiling/MemoryDemo/MemoryDemo.sln", DebugConfigurations = new string [] { "Before-Debug", "After-Debug" }, ReleaseConfigurations = new string [] { "Before-Release", "After-Release" } } },
				{ "WalkingGameCompleteiOS/WalkingGame.iOS/WalkingGame.iOS.csproj", new SampleTest { BuildSolution = true, Solution = "WalkingGameCompleteiOS/WalkingGame.sln" } },
				{ "WalkingGameEmptyiOS/WalkingGame.iOS/WalkingGame.iOS.csproj", new SampleTest { BuildSolution = true, Solution = "WalkingGameEmptyiOS/WalkingGame.sln" } },
				{ "watchOS/WatchConnectivity/WatchConnectivity/WatchConnectivity.csproj", new SampleTest { BuildSolution = true, Solution = "watchOS/WatchConnectivity/WatchConnectivity.sln" } },
				{ "watchOS/WatchKitCatalog/WatchKitCatalog/WatchKitCatalog.csproj", new SampleTest { BuildSolution = true, Solution = "watchOS/WatchKitCatalog/WatchKitCatalog.sln" } },

				// known failures
				{ "ios9/Emporium/Emporium/Emporium.csproj", new SampleTest { BuildSolution = true, Solution = "ios9/Emporium/Emporium.sln", KnownFailure = "error : Xcode 10 does not support watchOS 1 apps. Either upgrade to watchOS 2 apps, or use an older version of Xcode." } },
				{ "WatchKit/GpsWatch/GpsWatch/MainApp.csproj", new SampleTest { KnownFailure = "error : Xcode 10 does not support watchOS 1 apps. Either upgrade to watchOS 2 apps, or use an older version of Xcode." } },
				{ "WatchKit/WatchNotifications/WatchNotifications_iOS/WatchNotifications_iOS.csproj", new SampleTest { KnownFailure = "error : Xcode 10 does not support watchOS 1 apps. Either upgrade to watchOS 2 apps, or use an older version of Xcode." } },
				{ "PassKit/PassLibrary/PassLibrary.csproj", new SampleTest { BuildSolution = true, Solution = "PassKit/PassLibrary/PassLibrary.sln", KnownFailure = "Requires custom provisioning to get a proper pass." } },

			};

		static IEnumerable<SampleTestData> GetSampleData ()
		{
			return GetSampleTestData (test_data, ORG, REPO, HASH);
		}
	}

	[Category (CATEGORY)]
	public class MacIosSampleTester : SampleTester {
		const string ORG = "xamarin";
		const string REPO = "mac-ios-samples";
		const string CATEGORY = "maciossamples"; // categories can't contain dashes
		const string HASH = "2ab4faf9254cecdf5766af573e508f9ac8691663";

		static Dictionary<string, SampleTest> test_data = new Dictionary<string, SampleTest> {
				// Build solution instead of csproj
				{ "ExceptionMarshaling/ExceptionMarshaling.Mac.csproj", new SampleTest { BuildSolution = true, Solution = "ExceptionMarshaling/ExceptionMarshaling.sln" } },
				{ "Fox2/Fox2.macOS/Fox2.macOS.csproj", new SampleTest { BuildSolution = true, Solution = "Fox2/Fox2.sln" } },
				{ "MetalKitEssentials/MetalKitEssentials.iOS/MetalKitEssentials.iOS.csproj", new SampleTest { BuildSolution = true, Solution = "MetalKitEssentials/MetalKitEssentials.sln", Platforms = new string [] { "iPhone" } } }, // Requires Metal, which doesn't work/build in the simulator.
				{ "SceneKitReel/SceneKitReelMac/SceneKitReelMac.csproj", new SampleTest { BuildSolution = true, Solution = "SceneKitReel/SceneKitReel.sln" } },
			};

		static IEnumerable<SampleTestData> GetSampleData ()
		{
			return GetSampleTestData (test_data, ORG, REPO, HASH);
		}
	}

	[Category (CATEGORY)]
	public class MacSampleTester : SampleTester {
		const string ORG = "xamarin";
		const string REPO = "mac-samples";
		const string CATEGORY = "macsamples"; // categories can't contain dashes
		const string HASH = "6f905972c98e64759ff84a25e4e2b42366fa197b";

		static Dictionary<string, SampleTest> test_data = new Dictionary<string, SampleTest> {
		};

		static IEnumerable<SampleTestData> GetSampleData ()
		{
			return GetSampleTestData (test_data, ORG, REPO, HASH);
		}
	}

	[Category (CATEGORY)]
	public class MobileSampleTester : SampleTester {
		const string ORG = "xamarin";
		const string REPO = "mobile-samples";
		const string CATEGORY = "mobilesamples"; // categories can't contain dashes
		const string HASH = "45182f311db77bb05971a22d58edfdef44a4b657";

		static Dictionary<string, SampleTest> test_data = new Dictionary<string, SampleTest> {
				// Build solution instead of csproj
				{ "BouncingGame/BouncingGame/BouncingGame.iOS/BouncingGame.iOS.csproj", new SampleTest { BuildSolution = true, Solution = "BouncingGame/BouncingGame.sln" } },
				{ "CCAction/ActionProject/ActionProject.iOS/ActionProject.iOS.csproj", new SampleTest { BuildSolution = true, Solution = "CCAction/ActionProject.sln" } },
				{ "CCRenderTexture/RenderTextureExample/RenderTextureExample.iOS/RenderTextureExample.iOS.csproj", new SampleTest { BuildSolution = true, Solution = "CCRenderTexture/RenderTextureExample.sln" } },
				{ "EmbeddedResources/EmbeddedResources/EmbeddedResources.iOS.csproj", new SampleTest { BuildSolution = true, Solution = "EmbeddedResources/EmbeddedResources.sln" } },
				{ "FruityFalls/FruityFalls/FruityFalls.iOS/FruityFalls.iOS.csproj", new SampleTest { BuildSolution = true, Solution = "FruityFalls/FruityFalls.sln" } },
				{ "LivePlayer/BasicCalculator/Calculator.iOS/Calculator.iOS.csproj", new SampleTest { BuildSolution = true, Solution = "LivePlayer/BasicCalculator/Calculator.sln" } },
				{ "MonoGameTvOs/MonoGameTvOs/MonoGameTvOs.csproj", new SampleTest { BuildSolution = true, Solution = "MonoGameTvOs/MonoGameTvOs.sln" } },
				{ "SpriteSheetDemo/iOS/SpriteSheetDemo.iOS.csproj", new SampleTest { BuildSolution = true, Solution = "SpriteSheetDemo/SpriteSheetDemo.sln" } },
				{ "TaskyPortable/TaskyiOS/TaskyiOS.csproj", new SampleTest { BuildSolution = true, Solution = "TaskyPortable/TaskyPortable.sln" } },
				{ "TipCalc/TipCalc-UI-iOS/TipCalc-UI-iOS.csproj", new SampleTest { BuildSolution = true, Solution = "TipCalc/TipCalc.sln" } },
				
				// Known failures
				{ "RazorTodo/RazorNativeTodo/RazorNativeTodo.iOS/RazorNativeTodo.iOS.csproj", new SampleTest { BuildSolution = true, Solution = "RazorTodo/RazorNativeTodo/RazorNativeTodo.sln", KnownFailure = "There's a Xamarin.Android project in the solution, and I can't figure out how to build only the Xamarin.iOS project." } },
				{ "RazorTodo/RazorTodo/RazorTodo.iOS/RazorTodo.iOS.csproj", new SampleTest { BuildSolution = true, Solution = "RazorTodo/RazorTodo/RazorTodo.sln", KnownFailure = "There's a Xamarin.Android project in the solution, and I can't figure out how to build only the Xamarin.iOS project." } },
				{ "VisualBasic/TaskyPortableVB/TaskyiOS/TaskyiOS.csproj", new SampleTest { KnownFailure = "VisualBasic not supported on macOS: error MSB4057: The target \"Build\" does not exist in the project." } },
				{ "VisualBasic/XamarinFormsVB/XamarinForms.iOS/XamarinForms.iOS.csproj", new SampleTest { KnownFailure = "VisualBasic not supported on macOS." } },
				{ "WebServices/WebServiceSamples/WebServices.RxNorm/src/WebServices.RxNormSample/WebServices.RxNormSample.csproj", new SampleTest { KnownFailure = "Xamarin.iOS Classic isn't supported anymore." } },
			};

		static IEnumerable<SampleTestData> GetSampleData ()
		{
			return GetSampleTestData (test_data, ORG, REPO, HASH);
		}
	}

	[Category (CATEGORY)]
	public class PrebuiltAppTester : SampleTester {
		const string ORG = "xamarin";
		const string REPO = "prebuilt-apps";
		const string CATEGORY = "prebuiltapps"; // categories can't contain dashes
		const string HASH = "40da1283722df96e81efb5c62364d05e5bd3dd76";

		static Dictionary<string, SampleTest> test_data = new Dictionary<string, SampleTest> {
				// Known failures
				{ "FieldService/FieldService.iOS/FieldService.iOS.csproj", new SampleTest { KnownFailure = "The sample uses Xamarin Components which don't work anymore." } },
			};

		static IEnumerable<SampleTestData> GetSampleData ()
		{
			return GetSampleTestData (test_data, ORG, REPO, HASH);
		}
	}

	[Category (CATEGORY)]
	public class XamarinFormsTester : SampleTester {
		const string ORG = "xamarin";
		const string REPO = "xamarin-forms-samples";
		const string CATEGORY = "xamarinformssamples"; // categories can't contain dashes
		const string HASH = "206f4c3a2be1e988eda2ad9130a37019c60f1c7e";

		static Dictionary<string, SampleTest> test_data = new Dictionary<string, SampleTest> {
				// Build solution instead of csproj.
				{ "WebServices/TodoWCF/iOS/TodoWCF.iOS.csproj", new SampleTest { BuildSolution = true, Solution = "WebServices/TodoWCF/TodoWCF.sln" } },
			};

		static IEnumerable<SampleTestData> GetSampleData ()
		{
			return GetSampleTestData (test_data, ORG, REPO, HASH);
		}
	}

	[Category (CATEGORY)]
	public class XamarinFormsBooksTester : SampleTester {
		const string ORG = "xamarin";
		const string REPO = "xamarin-forms-book-samples";
		const string CATEGORY = "xamarinformsbookssamples"; // categories can't contain dashes
		const string HASH = "dc2bf3a47a46068cfc22b4163c06e218821adb81";

		static Dictionary<string, SampleTest> test_data = new Dictionary<string, SampleTest> {
				// Build solution instead of csproj,
				{ "Chapter20/TextFileAsync/TextFileAsync/TextFileAsync.iOS/TextFileAsync.iOS.csproj", new SampleTest { BuildSolution = true, Solution = "Chapter20/TextFileAsync/TextFileAsync.sln" } },
				{ "Chapter24/NoteTaker/NoteTaker/NoteTaker.iOS/NoteTaker.iOS.csproj", new SampleTest { BuildSolution = true, Solution = "Chapter24/NoteTaker/NoteTaker.sln" } },
				{ "Chapter27/BouncingBall/BouncingBall/BouncingBall.iOS/BouncingBall.iOS.csproj", new SampleTest { BuildSolution = true, Solution = "Chapter27/BouncingBall/BouncingBall.sln" } },
				{ "Chapter27/EllipseDemo/EllipseDemo/EllipseDemo.iOS/EllipseDemo.iOS.csproj", new SampleTest { BuildSolution = true, Solution = "Chapter27/EllipseDemo/EllipseDemo.sln" } },
				{ "Chapter27/StepSliderDemo/StepSliderDemo/StepSliderDemo.iOS/StepSliderDemo.iOS.csproj", new SampleTest { BuildSolution = true, Solution = "Chapter27/StepSliderDemo/StepSliderDemo.sln" } },
				{ "Chapter28/MapDemos/MapDemos/MapDemos.iOS/MapDemos.iOS.csproj", new SampleTest { BuildSolution = true, Solution = "Chapter28/MapDemos/MapDemos.sln" } },
				{ "Chapter28/WhereAmI/WhereAmI/WhereAmI.iOS/WhereAmI.iOS.csproj", new SampleTest { BuildSolution = true, Solution = "Chapter28/WhereAmI/WhereAmI.sln" } },

				// Known failures
				{ "Chapter02/FS/Greetings/Greetings.iOS/Greetings.iOS.fsproj", new SampleTest { KnownFailure = "https://github.com/xamarin/xamarin-forms-book-samples/pull/57" } },
				{ "Chapter02/FS/Hello/Hello.iOS/Hello.iOS.fsproj", new SampleTest { KnownFailure = "https://github.com/xamarin/xamarin-forms-book-samples/pull/57" } },
		};

		static IEnumerable<SampleTestData> GetSampleData ()
		{
			return GetSampleTestData (test_data, ORG, REPO, HASH);
		}
	}

	[Category (CATEGORY)]
	public class EmbeddedFrameworksTester : SampleTester {
		const string ORG = "rolfbjarne";
		const string REPO = "embedded-frameworks";
		const string CATEGORY = "embeddedframeworks"; // categories can't contain dashes
		const string HASH = "faaad6f9dcda53b2c49cec567eca769cb534307f";

		static Dictionary<string, SampleTest> test_data = new Dictionary<string, SampleTest> {
				// Known failures
				{ "simpleapp-with-framework/simpleapp-with-framework/simpleapp-with-framework.csproj", new SampleTest { BuildSolution = true, Solution = "simpleapp-with-framework/simpleapp-with-framework.sln" } },
			};

		static IEnumerable<SampleTestData> GetSampleData ()
		{
			return GetSampleTestData (test_data, ORG, REPO, HASH);
		}
	}

}
