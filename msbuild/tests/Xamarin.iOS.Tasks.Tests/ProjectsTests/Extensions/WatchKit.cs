using System;
using System.IO;

using NUnit.Framework;
using Xamarin.MacDev;
using System.Diagnostics;
using Xamarin.Tests;

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
			this.BuildExtension ("MyWatchApp", "MyWatchKitExtension", Platform, "Debug", expectedErrorCount: 1,  additionalAsserts: (ProjectPaths mtouchPaths) =>
			{
				Assert.IsTrue (Directory.Exists (Path.Combine (mtouchPaths.AppBundlePath, "PlugIns", "MyWatchKitExtension.appex")), "appex");
				Assert.IsFalse (Directory.Exists (Path.Combine (mtouchPaths.AppBundlePath, "PlugIns", "MyWatchKitExtension.appex", "Frameworks")), "frameworks");
			});

			Assert.AreEqual ("Xamarin.iOS 14+ does not support watchOS 1 apps. Please migrate your project to watchOS 2+.", Engine.Logger.ErrorEvents[0].Message, "WK 1 error message");
		}
	}
}
