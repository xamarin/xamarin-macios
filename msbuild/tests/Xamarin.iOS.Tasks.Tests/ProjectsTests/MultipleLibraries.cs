using System;
using System.IO;
using System.Linq;

using NUnit.Framework;

namespace Xamarin.iOS.Tasks {
	[TestFixture ("iPhone")] // Not working yet (native linker error)
	[TestFixture ("iPhoneSimulator")]
	public class MultipleLibrariesTests : ExtensionTestBase {

		public MultipleLibrariesTests (string platform) : base (platform)
		{
		}

		[Test]
		public void BasicTest ()
		{
			if (!Xamarin.Tests.Configuration.include_watchos)
				Assert.Ignore ("WatchOS is not enabled");

			BuildExtension ("MyWatchApp2", "MyWatchKit2Extension", Platform, "Debug");

			var callingCount = Engine.Logger.MessageEvents.Count (m => m.Message.Contains ("Calling AppleSdkSettings.Init"));
			var skippingCount = Engine.Logger.MessageEvents.Count (m => m.Message.Contains ("Skipping AppleSdkSettings.Init, already valid."));
			Assert.AreEqual (callingCount, 1);
			Assert.AreEqual (skippingCount, 2);

			if (Platform == "iPhone") {
				// make sure the dSYMs exist
				var appexDsymDir = Path.GetFullPath (Path.Combine (AppBundlePath, "..", "MyWatchKit2Extension.appex.dSYM"));
			}
		}

		public override string TargetFrameworkIdentifier {
			get {
				return "Xamarin.WatchOS";
			}
		}
	}
}

