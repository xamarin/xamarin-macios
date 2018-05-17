using System;
using System.IO;

using NUnit.Framework;
using Xamarin.MacDev;
using System.Diagnostics;

namespace Xamarin.iOS.Tasks {
	[TestFixture ("iPhone")]
	[TestFixture ("iPhoneSimulator")]
	public class WatchKit : ExtensionTestBase {

		public WatchKit (string platform) : base(platform)
		{
		}

		[Test]
		public void BasicTest () 
		{
			this.BuildExtension ("MyWatchApp", "MyWatchKitExtension", Platform, "Debug", additionalAsserts: (ProjectPaths mtouchPaths) =>
			{
				Assert.IsTrue (Directory.Exists (Path.Combine (mtouchPaths.AppBundlePath, "PlugIns", "MyWatchKitExtension.appex")), "appex");
				Assert.IsFalse (Directory.Exists (Path.Combine (mtouchPaths.AppBundlePath, "PlugIns", "MyWatchKitExtension.appex", "Frameworks")), "frameworks");
			});
		}

		[Test]
		public void InvalidBundleIdTest ()
		{
			var mtouchPaths = SetupProjectPaths ("MyWatchApp", platform: Platform);
			using (var xiproj = XIProject.Clone (mtouchPaths.ProjectPath, "MyWatchKitExtension", "MyWatchKitApp")) {
				mtouchPaths = SetupProjectPaths ("MyWatchApp", "MyWatchApp", xiproj.ProjectDirectory, platform: Platform);

				var appInfoPath = Path.Combine (mtouchPaths.ProjectPath, "Info.plist");
				var appInfoContents = File.ReadAllText (appInfoPath);
				if (!appInfoContents.Contains ("<string>com.xamarin.MyWatchApp</string>"))
					Assert.Fail ("Info.plist did not contain '<string>com.xamarin.MyWatchApp</string>'");
				File.WriteAllText (appInfoPath, appInfoContents.Replace ("<string>com.xamarin.MyWatchApp</string>", "<string>com.xamarin.MyWatchAppX</string>"));

				var proj = SetupProject (Engine, mtouchPaths.ProjectCSProjPath);
				Engine.ProjectCollection.SetGlobalProperty ("Platform", Platform);
				AppBundlePath = mtouchPaths ["app_bundlepath"];
				RunTarget (proj, "Build", 2);
				Assert.AreEqual ("The App Extension 'WatchExtension' has an invalid CFBundleIdentifier (com.xamarin.MyWatchApp.WatchExtension), it does not begin with the main app bundle's CFBundleIdentifier (com.xamarin.MyWatchAppX).", Engine.Logger.ErrorEvents [0].Message, "#1");
				Assert.AreEqual ("The Watch App 'WatchApp' has an invalid WKCompanionAppBundleIdentifier value ('com.xamarin.MyWatchApp'), it does not match the main app bundle's CFBundleIdentifier ('com.xamarin.MyWatchAppX').", Engine.Logger.ErrorEvents [1].Message, "#2");
			}
		}

		[Test]
		public void CreateIpa () 
		{
			if (Platform == "iPhoneSimulator")
				return; // this is a device-only test.

			const string hostAppName = "MyWatchApp";
//			string extensionName = "MyWatchKitExtension";
			const string configuration = "AppStore";

			var mtouchPaths = SetupProjectPaths (hostAppName, "../", true, Platform, configuration);
			var proj = SetupProject (Engine, mtouchPaths.ProjectCSProjPath);

			AppBundlePath = mtouchPaths.AppBundlePath;

			Engine.ProjectCollection.SetGlobalProperty ("Platform", Platform);
			Engine.ProjectCollection.SetGlobalProperty ("BuildIpa", "true");
			Engine.ProjectCollection.SetGlobalProperty ("IpaIncludeArtwork", "true");
			Engine.ProjectCollection.SetGlobalProperty ("CodesignProvision", "Automatic"); // Provisioning profile
			Engine.ProjectCollection.SetGlobalProperty ("CodesignKey", "iPhone Developer");
			Engine.ProjectCollection.SetGlobalProperty ("Configuration", configuration);

			RunTarget (proj, "Clean");
			Assert.IsFalse (Directory.Exists (AppBundlePath), "{1}: App bundle exists after cleanup: {0} ", AppBundlePath, Platform);

			proj = SetupProject (Engine, mtouchPaths.ProjectCSProjPath);
			RunTarget (proj, "Build");

			var plist = PDictionary.FromFile (Path.Combine (AppBundlePath, "Info.plist"));
			Assert.IsTrue (plist.ContainsKey ("CFBundleExecutable"));
			Assert.IsTrue (plist.ContainsKey ("CFBundleVersion"));
			Assert.IsNotEmpty (((PString)plist["CFBundleExecutable"]).Value);
			Assert.IsNotEmpty (((PString)plist["CFBundleVersion"]).Value);

			var ipaPath = Path.Combine (mtouchPaths.ProjectBinPath, hostAppName +  ".ipa");
			var payloadPath = "Payload/";
			var watchkitSupportPath = "WatchKitSupport/";

			Assert.IsTrue (File.Exists (ipaPath), "IPA package does not exist: {0}", ipaPath);

			var startInfo = new ProcessStartInfo ("/usr/bin/zipinfo", "-1 \"" + ipaPath + "\"");
			startInfo.RedirectStandardOutput = true;
			startInfo.UseShellExecute = false;
			var process = new Process ();
			process.StartInfo = startInfo;
			process.Start ();
			var output = process.StandardOutput.ReadToEnd ();
			process.WaitForExit ();

			var lines = output.Split (new char [] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

			Assert.Contains (payloadPath, lines, payloadPath + " does not exist");
			Assert.Contains (watchkitSupportPath, lines, watchkitSupportPath + " does not exist");

			string wkPath = "WatchKitSupport/WK";
			Assert.Contains (wkPath, lines, wkPath + " does not exist");

			var ipaIncludeArtwork = proj.GetPropertyValue ("IpaIncludeArtwork");
			Assert.IsTrue (output.Contains ("iTunesMetadata.plist"), string.Format ("The ipa should contain at least one iTunesMetadata.plist file if we are using an AppStore config and IpaIncludeArtwork is true. IpaIncludeArtwork: {0}", ipaIncludeArtwork));

			RunTarget (proj, "Clean");
			Assert.IsFalse (File.Exists (ipaPath), "IPA package still exists after Clean: {0}", ipaPath);
		}
	}
}
