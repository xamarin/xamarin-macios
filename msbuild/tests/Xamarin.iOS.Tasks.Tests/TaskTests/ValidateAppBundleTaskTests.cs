using System.IO;
using NUnit.Framework;
using Xamarin.MacDev;

namespace Xamarin.iOS.Tasks
{
	[TestFixture]
	public class ValidateAppBundleTaskTests : ExtensionTestBase
	{
		ValidateAppBundleTask task;

		string extensionBundlePath;

		string mainAppPlistPath;
		string extensionPlistPath;

		PDictionary sourcePlist;

		public override void Setup ()
		{
			base.Setup ();

			var extensionName = "MyActionExtension";
			BuildExtension ("MyTabbedApplication", extensionName, "iPhoneSimulator", "Debug");

			task = CreateTask<ValidateAppBundleTask> ();
			task.AppBundlePath = AppBundlePath;
			task.SdkIsSimulator = true;
			task.TargetFrameworkIdentifier = "Xamarin.iOS";

			extensionBundlePath = Path.Combine (AppBundlePath, "PlugIns", extensionName + ".appex");

			mainAppPlistPath = Path.Combine (AppBundlePath, "Info.plist");
			extensionPlistPath = Path.Combine (extensionBundlePath, "Info.plist");

			sourcePlist = PDictionary.FromFile (mainAppPlistPath);
		}

		[Test]
		public void MissingPlist_MainApp ()
		{
			File.Delete (mainAppPlistPath);
			Assert.IsFalse (task.Execute (), "#1");
			Assert.IsTrue (Engine.Logger.ErrorEvents.Count > 0, "#2");
		}

		[Test]
		public void MissingPlist_Extension ()
		{
			File.Delete (extensionPlistPath);
			Assert.IsFalse (task.Execute (), "#1");
			Assert.IsTrue (Engine.Logger.ErrorEvents.Count > 0, "#2");
		}

		[Test (Description = "Xambug #38673")]
		public void NotMatching_VersionBuildNumbers ()
		{
			var warningCount = Engine.Logger.WarningsEvents.Count;

			// Warning: The App Extension has a CFBundleShortVersionString
			// that does not match the main app bundle's CFBundleShortVersionString.
			sourcePlist.SetCFBundleShortVersionString ("1");
			warningCount++;

			// Warning: The App Extension has a CFBundleVersion
			// that does not match the main app bundle's CFBundleVersion.
			sourcePlist.SetCFBundleVersion ("1");
			warningCount++;

			sourcePlist.Save (mainAppPlistPath);
			Assert.True (task.Execute (), "#1"); // No build error.
			Assert.AreEqual (warningCount, Engine.Logger.WarningsEvents.Count, "#2");
		}
	}
}

