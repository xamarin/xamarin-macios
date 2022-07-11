using System;
using System.IO;

using NUnit.Framework;

namespace Xamarin.iOS.Tasks {
	[TestFixture ("iPhone")]
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

			BuildExtension ("MyWatchApp2", "MyWatchKit2Extension");
		}

		public override string TargetFrameworkIdentifier {
			get {
				return "Xamarin.WatchOS";
			}
		}
	}
}
