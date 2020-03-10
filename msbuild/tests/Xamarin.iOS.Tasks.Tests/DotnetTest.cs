using System;
using System.IO;
using System.Linq;

using Xamarin.Tests;

using NUnit.Framework;

namespace Xamarin.iOS.Tasks {

	[TestFixture]
	[TestFixture ("iPhone", "Debug")]
	[TestFixture ("iPhone", "Release")]
	[TestFixture ("iPhoneSimulator", "Debug")]
	[TestFixture ("iPhoneSimulator", "Release")]
	public class DotnetTest : ProjectTest {
		public string Configuration;

		public DotnetTest (string platform, string configuration)
			: base (platform)
		{
			Configuration = configuration;
		}


		string tfi;
		public override string TargetFrameworkIdentifier {
			get {
				return tfi ?? base.TargetFrameworkIdentifier;
			}
		}

		[Test]
		////[TestCase ("Bug60536")] // Supposed to fail the build
		[TestCase ("AppWithExtraArgumentThatOverrides")]
		[TestCase ("My Spaced App")]
		[TestCase ("MyAppWithPackageReference")]
		[TestCase ("MyCoreMLApp")]
		[TestCase ("MyIBToolLinkTest")]
		[TestCase ("MyiOSAppWithBinding")]
		[TestCase ("MyLinkedAssets")]
		[TestCase ("MyMasterDetailApp")]
		[TestCase ("MyMetalGame")]
		[TestCase ("MyOpenGLApp")]
		[TestCase ("MyReleaseBuild")]
		[TestCase ("MySceneKitApp")]
		[TestCase ("MySingleView")]
		[TestCase ("MySpriteKitGame")]
		[TestCase ("MyTabbedApplication")]
		[TestCase ("MyTVApp")]
		[TestCase ("MyWatch2Container")]
		[TestCase ("MyWebViewApp")]
		[TestCase ("MyXamarinFormsApp")]
		public void CompareBuilds (string project)
		{
			var net461 = GetTestDirectory ("net461");
			var dotnet = GetTestDirectory ("dotnet");
			Xamarin.Tests.Configuration.FixupTestFiles (dotnet, "dotnet5");

			tfi = "Xamarin.iOS";
			switch (project) {
			case "MyXamarinFormsApp":
				NugetRestore (Path.Combine (net461, project, "MyXamarinFormsAppNS", "MyXamarinFormsAppNS.csproj"));
				break;
			case "MyAppWithPackageReference":
				NugetRestore (Path.Combine (net461, "MyExtensionWithPackageReference", "MyExtensionWithPackageReference.csproj"));
				break;
			case "MyMetalGame":
				if (Platform == "iPhoneSimulator")
					Assert.Ignore ("The iOS Simulator does not support metal. Build for a device instead.");
				break;
			case "MyTVApp":
				tfi = "Xamarin.TVOS";
				break;
			}

			Console.WriteLine ("Building net461");
			BuildProject (project, Platform, Configuration, projectBaseDir: net461, use_dotnet: false, nuget_restore: true);
			Console.WriteLine ("Done building net461");
			var net461_bundle = AppBundlePath;

			Console.WriteLine ("Building dotnet");
			BuildProject (project, Platform, Configuration, projectBaseDir: dotnet, use_dotnet: true);
			Console.WriteLine ("Done building dotnet");
			var dotnet_bundle = AppBundlePath;

			DotNet.CompareApps (net461_bundle, dotnet_bundle);
		}
	}
}
