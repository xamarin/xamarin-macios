using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

using NUnit.Framework;

using Xamarin.Tests;

namespace Xamarin.iOS.Tasks
{
	// [TestFixture ("iPhone")] // Skip this to speed things up a bit.
	[TestFixture ("iPhoneSimulator")]
	public class CompileSceneKitAssetsTest : ProjectTest
	{
		public CompileSceneKitAssetsTest (string platform) : base (platform)
		{
		}

		[Test]
		public void Compilation ()
		{
			var csproj = BuildProject ("MySceneKitApp", Platform, "Debug", clean: true);
			var appPath = Path.Combine (Path.GetDirectoryName (csproj), "bin", Platform, "Debug", "MySceneKitApp.app");
			var scenePath = Path.GetFullPath (Path.Combine (appPath, "art.scnassets", "scene.scn"));

			var xml = Configuration.ReadPListAsXml (scenePath);
			Assert.That (xml, Does.Contain ("<string>art.scnassets/texture.png</string>"), "asset with path");
		}

		[Test]
		public void LibraryCompilation ()
		{
			var appName = "MySceneKitLibrary";
			var platform = "AnyCPU";
			var config = "Debug";

			var mtouchPaths = SetupProjectPaths (appName, "../", true, platform, config);
			var proj = SetupProject (Engine, mtouchPaths.ProjectCSProjPath);

			Engine.ProjectCollection.SetGlobalProperty ("Platform", platform);
			Engine.ProjectCollection.SetGlobalProperty ("Configuration", config);

			RunTarget (proj, "Build", 0);
		}
	}
}
