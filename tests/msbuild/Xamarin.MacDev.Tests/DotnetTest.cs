using System;
using System.IO;

using Xamarin.Tests;

using NUnit.Framework;

namespace Xamarin.iOS.Tasks {

	// This test builds test projects using both MSBuild (old-style) and .NET (new-style), and
	// compares the resulting .app directories.
	[TestFixture]
	//[TestFixture ("iPhone", "Debug")] // TODO: device support hasn't been implemented yet in .NET
	//[TestFixture ("iPhone", "Release")] // TODO: device support hasn't been implemented yet in .NET
	[TestFixture ("iPhoneSimulator", "Debug")]
	[TestFixture ("iPhoneSimulator", "Release")]
	public class DotNetTest : ProjectTest {
		public DotNetTest (string platform, string configuration)
			: base (platform, configuration)
		{
		}

		string tfi;
		public override string TargetFrameworkIdentifier {
			get {
				return tfi ?? base.TargetFrameworkIdentifier;
			}
		}

		[Test]
		// [TestCase ("Bug60536", 1)] // TODO: .NET version succeeds when it shouldn't
		[TestCase ("AppWithExtraArgumentThatOverrides")]
		[TestCase ("My Spaced App")]
		[TestCase ("MyAppWithPackageReference")]
		[TestCase ("MyCoreMLApp")]
		[TestCase ("MyIBToolLinkTest")]
		[TestCase ("MyiOSAppWithBinding")]
		// [TestCase ("MyLinkedAssets")] // TODO: Requires fat apps, which has not been implemented yet
		[TestCase ("MyMasterDetailApp")]
		[TestCase ("MyMetalGame")]
		// [TestCase ("MyOpenGLApp")] // TODO: Requires OpenTK-1.0.dll, which has not been implemented yet
		[TestCase ("MyReleaseBuild")]
		[TestCase ("MySceneKitApp")]
		[TestCase ("MySingleView")]
		[TestCase ("MySpriteKitGame")]
		[TestCase ("MyTabbedApplication")]
		[TestCase ("MyTVApp")]
		[TestCase ("MyTVMetalGame")]
		// [TestCase ("MyWatch2Container")] // TODO: Requires watchOS support, which has not been implemented yet
		[TestCase ("MyWebViewApp")]
		public void CompareBuilds (string project, int expectedErrorCount = 0)
		{
			Configuration.AssertDotNetAvailable ();

			tfi = "Xamarin.iOS";
			switch (project) {
			case "MyMetalGame":
				if (Platform == "iPhoneSimulator")
					Assert.Ignore ("The iOS Simulator does not support metal. Build for a device instead.");
				break;
			case "MyTVApp":
			case "MyTVMetalGame":
				tfi = "Xamarin.TVOS";
				break;
			}

			ClearTestDirectory ();

			Console.WriteLine ("Building dotnet");
			Mode = ExecutionMode.DotNet;
			BuildProject (project, clean: false, expectedErrorCount: expectedErrorCount);
			Console.WriteLine ("Done building dotnet");
			var dotnet_bundle = AppBundlePath;

			Console.WriteLine ("Building net461");
			Mode = ExecutionMode.MSBuild;
			var net461 = GetTestDirectory (forceClone: true);
			switch (project) {
			case "MyXamarinFormsApp":
				NugetRestore (Path.Combine (net461, project, "MyXamarinFormsAppNS", "MyXamarinFormsAppNS.csproj"));
				break;
			case "MyAppWithPackageReference":
				NugetRestore (Path.Combine (net461, "MyExtensionWithPackageReference", "MyExtensionWithPackageReference.csproj"));
				break;
			}
			BuildProject (project, nuget_restore: true, expectedErrorCount: expectedErrorCount);
			Console.WriteLine ("Done building net461");
			var net461_bundle = AppBundlePath;

			if (expectedErrorCount == 0)
				DotNet.CompareApps (net461_bundle, dotnet_bundle);
		}
	}
}
