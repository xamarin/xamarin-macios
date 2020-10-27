using System;
using System.IO;
using NUnit.Framework;

using Microsoft.Build.Evaluation;

using Xamarin.MacDev;

namespace Xamarin.iOS.Tasks
{
	[TestFixture]
	public class ValidateAppBundleTaskTests : ExtensionTestBase
	{
		string extensionBundlePath;
		string mainAppPlistPath;
		string extensionPlistPath;

		Project project;

		public ValidateAppBundleTaskTests ()
			: base ("iPhoneSimulator")
		{
		}

		[Test]
		public void MissingFiles ()
		{
			project = BuildExtension ("MyTabbedApplication", "MyActionExtension", Platform, "Debug");
			extensionBundlePath = AppBundlePath;
			mainAppPlistPath = Path.Combine (AppBundlePath, "Info.plist");
			extensionPlistPath = Path.Combine (extensionBundlePath, "Info.plist");

			MissingPlist_Extension ();
			MissingPlist_MainApp ();
			NotMatching_VersionBuildNumbers ();
		}

		void MissingPlist_MainApp ()
		{
			var contents = File.ReadAllBytes (mainAppPlistPath);
			try {
				File.Delete (mainAppPlistPath);
				Engine.Logger.Clear ();
				RunTarget (project, "_ValidateAppBundle", 1);
				Assert.IsTrue (Engine.Logger.ErrorEvents.Count > 0, "#2");
			} finally {
				// Restore the contents
				File.WriteAllBytes (mainAppPlistPath, contents);
			}
		}

		void MissingPlist_Extension ()
		{
			var contents = File.ReadAllBytes (extensionPlistPath);
			try { 
				File.Delete (extensionPlistPath);
				Engine.Logger.Clear ();
				RunTarget (project, "_ValidateAppBundle", 1);
				Assert.IsTrue (Engine.Logger.ErrorEvents.Count > 0, "#2");
			} finally {
				// Restore the contents
				File.WriteAllBytes (extensionPlistPath, contents);
			}
		}

		// Xambug #38673
		void NotMatching_VersionBuildNumbers ()
		{
			var contents = File.ReadAllBytes (mainAppPlistPath);
			var sourcePlist = PDictionary.FromFile (mainAppPlistPath);
			try {
				// Warning: The App Extension has a CFBundleShortVersionString
				// that does not match the main app bundle's CFBundleShortVersionString.
				sourcePlist.SetCFBundleShortVersionString ("1");

				// Warning: The App Extension has a CFBundleVersion
				// that does not match the main app bundle's CFBundleVersion.
				sourcePlist.SetCFBundleVersion ("1");

				sourcePlist.Save (mainAppPlistPath);
				Engine.Logger.Clear ();
				RunTarget (project, "_ValidateAppBundle", 0);
				Assert.AreEqual (2, Engine.Logger.WarningsEvents.Count, "#2");
			} finally {
				// Restore the contents
				File.WriteAllBytes (mainAppPlistPath, contents);
			}
		}
	}
}
