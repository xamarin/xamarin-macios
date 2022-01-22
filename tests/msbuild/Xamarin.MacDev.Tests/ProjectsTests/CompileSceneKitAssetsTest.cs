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
			var proj = BuildProject ("MySceneKitApp");
			var appPath = proj.AppBundlePath;
			var scenePath = Path.GetFullPath (Path.Combine (appPath, "art.scnassets", "scene.scn"));

			var xml = Configuration.ReadPListAsXml (scenePath);
			Assert.That (xml, Does.Contain ("<string>art.scnassets/texture.png</string>"), "asset with path");
		}

		[Test]
		public void LibraryCompilation ()
		{
			var appName = "MySceneKitLibrary";

			Platform = "AnyCPU";
			var proj = SetupProjectPaths (appName);

			Engine.ProjectCollection.SetGlobalProperty ("Platform", Platform);
			Engine.ProjectCollection.SetGlobalProperty ("Configuration", Config);

			RunTarget (proj, "Build", 0);
		}
	}
}
