using System;
using System.IO;

using NUnit.Framework;

namespace Xamarin.iOS.Tasks
{
	[TestFixture]
	public class DetectSdkLocationsTaskTests : TestBase
	{
		[Test]
		public void InvalidXamarinSdkRoot ()
		{
			var task = CreateTask<DetectSdkLocations> ();
			task.XamarinSdkRoot = "XYZ";
			task.TargetFrameworkIdentifier = "Xamarin.iOS";
			task.Execute ();

			Assert.AreEqual ("XYZ", task.XamarinSdkRoot, "#1");
		}

		[Test]
		public void InexistentSDKVersion ()
		{
			var task = CreateTask<DetectSdkLocations> ();
			task.SdkVersion = "4.0";
			task.TargetFrameworkIdentifier = "Xamarin.iOS";
			Assert.IsTrue (task.Execute (), "4.0 Execute");

			Assert.AreNotEqual ("4.0", task.SdkVersion, "#1");

			task.SdkVersion = "44.0";
			Assert.IsFalse (task.Execute (), "44.0 Execute");
		}
	}
}

