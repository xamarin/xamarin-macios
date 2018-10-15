using System;
using System.IO;
using System.Linq;
using System.Collections;

using NUnit.Framework;

using Microsoft.Build.BuildEngine;

namespace Xamarin.iOS.Tasks
{
	[TestFixture]
	public class Bug60536 : ProjectTest
	{
		public Bug60536 () : base ("iPhoneSimulator")
		{
		}

		[Test]
		public void TestACToolTaskCatchesJsonException ()
		{
			var platform = "iPhoneSimulator";
			var config = "Debug";
			var target = "Build";
			var clean = true;

			var mtouchPaths = SetupProjectPaths ("Bug60536", "../", true, platform, config);
			var csproj = mtouchPaths["project_csprojpath"];

			var project = SetupProject (Engine, csproj);

			AppBundlePath = mtouchPaths ["app_bundlepath"];
			Engine.GlobalProperties.SetProperty("Platform", platform);
			Engine.GlobalProperties.SetProperty("Configuration", config);

			if (clean) {
				RunTarget (project, "Clean");
				Assert.IsFalse (Directory.Exists (AppBundlePath), "App bundle exists after cleanup: {0} ", AppBundlePath);
				Assert.IsFalse (Directory.Exists (AppBundlePath + ".dSYM"), "App bundle .dSYM exists after cleanup: {0} ", AppBundlePath + ".dSYM");
				Assert.IsFalse (Directory.Exists (AppBundlePath + ".mSYM"), "App bundle .mSYM exists after cleanup: {0} ", AppBundlePath + ".mSYM");

				var baseDir = Path.GetDirectoryName (csproj);
				var objDir = Path.Combine (baseDir, "obj", platform, config);
				var binDir = Path.Combine (baseDir, "bin", platform, config);

				if (Directory.Exists (objDir)) {
					var path = Directory.EnumerateFiles (objDir, "*.*", SearchOption.AllDirectories).FirstOrDefault ();
					Assert.IsNull (path, "File not cleaned: {0}", path);
				}

				if (Directory.Exists (binDir)) {
					// Note: the .dSYM/Contents string match is a work-around for xbuild which is broken (wrongly interprets %(Directory))
					var path = Directory.EnumerateFiles (binDir, "*.*", SearchOption.AllDirectories).FirstOrDefault (x => x.IndexOf (".dSYM/Contents/", StringComparison.Ordinal) == -1);
					Assert.IsNull (path, "File not cleaned: {0}", path);
				}
			}

			project = SetupProject (Engine, mtouchPaths.ProjectCSProjPath);

			Engine.BuildProject (project, new [] { target }, new Hashtable { {"Platform", "iPhone"} }, BuildSettings.None);
			if (Engine.Logger.ErrorEvents.Count != 1) {
				string messages = string.Empty;
				if (Engine.Logger.ErrorEvents.Count > 0)
					messages = "\n\t" + string.Join ("\n\t", Engine.Logger.ErrorEvents.Select ((v) => v.Message).ToArray ());
				Assert.AreEqual (1, Engine.Logger.ErrorEvents.Count, "#RunTarget-ErrorCount" + messages);
			}

			var expectedFile = Path.Combine ("obj", platform, config, "actool", "cloned-assets", "Assets.xcassets", "AppIcon.appiconset", "Contents.json");
			Assert.AreEqual (expectedFile, Engine.Logger.ErrorEvents[0].File, "File");
			Assert.AreEqual (197, Engine.Logger.ErrorEvents[0].LineNumber, "LineNumber");
			Assert.AreEqual (2, Engine.Logger.ErrorEvents[0].ColumnNumber, "ColumnNumber");
			Assert.AreEqual (197, Engine.Logger.ErrorEvents[0].EndLineNumber, "EndLineNumber");
			Assert.AreEqual (2, Engine.Logger.ErrorEvents[0].EndColumnNumber, "EndColumnNumber");
			Assert.AreEqual ("Unexpected character ']'. At line 197, column 2.", Engine.Logger.ErrorEvents[0].Message, "Message");
		}
	}
}
