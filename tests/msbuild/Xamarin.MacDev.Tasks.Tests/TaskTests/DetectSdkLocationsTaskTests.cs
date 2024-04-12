using System;
using System.IO;

using NUnit.Framework;

using Xamarin.Tests;
using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	[TestFixture]
	public class DetectSdkLocationsTaskTests : TestBase {
		[Test]
		public void InvalidXamarinSdkRoot ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			var task = CreateTask<DetectSdkLocations> ();
			task.XamarinSdkRoot = "XYZ";
			task.TargetFrameworkMoniker = "Xamarin.iOS,v1.0";
			task.Execute ();

			Assert.AreEqual ("XYZ", task.XamarinSdkRoot, "#1");
		}

		[Test]
		public void InexistentSDKVersion ()
		{
			Configuration.AssertLegacyXamarinAvailable ();
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			var task = CreateTask<DetectSdkLocations> ();
			task.SdkVersion = "4.0";
			task.TargetFrameworkMoniker = "Xamarin.iOS,v1.0";
			Assert.IsTrue (task.Execute (), "4.0 Execute");

			Assert.AreNotEqual ("4.0", task.SdkVersion, "#1");

			task.SdkVersion = "44.0";
			Assert.IsFalse (task.Execute (), "44.0 Execute");
		}
	}
}
