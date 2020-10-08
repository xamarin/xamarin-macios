using System;
using System.IO;

using NUnit.Framework;

namespace Xamarin.iOS.Tasks {
	[TestFixture ("iPhone")] // Not working yet (native linker error)
	[TestFixture ("iPhoneSimulator")]
	public class WatchKit2 : ExtensionTestBase {

		public WatchKit2 (string platform) : base(platform)
		{
		}

		[Test]
		public void BasicTest () 
		{
			if (!Xamarin.Tests.Configuration.include_watchos)
				Assert.Ignore ("WatchOS is not enabled");

			BuildExtension ("MyWatchApp2", "MyWatchKit2Extension", Platform, "Debug");

			if (Platform == "iPhone") {
				// make sure the dSYMs exist
				var appexDsymDir = Path.GetFullPath (Path.Combine (AppBundlePath, "..", "MyWatchKit2Extension.appex.dSYM"));

				Assert.IsTrue (Directory.Exists (appexDsymDir), "MyWatchKit2Extension dSYMs not found");
			}
		}

		public override string TargetFrameworkIdentifier {
			get {
				return "Xamarin.WatchOS";
			}
		}
	}
}

