using System;
using System.IO;

using NUnit.Framework;

using Xamarin.iOS.Tasks;

namespace Xamarin.MacDev.Tasks
{
	[TestFixture]
	public class DetectSdkLocationsTaskTests : TestBase
	{
		[Test]
		public void InvalidXamarinSdkRoot ()
		{
			var task = CreateTask<DetectSdkLocations> ();
			task.XamarinSdkRoot = "XYZ";
			task.TargetFrameworkMoniker = "Xamarin.iOS,v1.0";
			task.Execute ();

			Assert.AreEqual ("XYZ", task.XamarinSdkRoot, "#1");
		}

		[Test]
		public void InexistentSDKVersion ()
		{
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
