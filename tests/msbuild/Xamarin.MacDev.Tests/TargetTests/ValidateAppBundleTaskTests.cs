using System;
using System.IO;
using NUnit.Framework;

using Microsoft.Build.Evaluation;

using Xamarin.MacDev;

using Xamarin.Tests;
using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	[TestFixture]
	public class ValidateAppBundleTaskTests : ExtensionTestBase {
		string extensionBundlePath;
		string mainAppPlistPath;
		string extensionPlistPath;

		public ValidateAppBundleTaskTests ()
			: base ("iPhoneSimulator")
		{
		}

		[Test]
		public void MissingFiles ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			var paths = BuildExtension ("MyTabbedApplication", "MyActionExtension");
			extensionBundlePath = paths.AppBundlePath;
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
				RunTarget (MonoTouchProject, "_ValidateAppBundle", 1);
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
				RunTarget (MonoTouchProject, "_ValidateAppBundle", 1);
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
				RunTarget (MonoTouchProject, "_ValidateAppBundle", 0);
				Assert.AreEqual (2, Engine.Logger.WarningsEvents.Count, "#2");
			} finally {
				// Restore the contents
				File.WriteAllBytes (mainAppPlistPath, contents);
			}
		}
	}
}
