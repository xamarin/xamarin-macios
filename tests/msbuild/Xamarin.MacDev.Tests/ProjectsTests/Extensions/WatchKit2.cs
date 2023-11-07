using System;
using System.IO;

using NUnit.Framework;

using Xamarin.Tests;
using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	[TestFixture ("iPhone")]
	[TestFixture ("iPhoneSimulator")]
	public class WatchKit2 : ExtensionTestBase {

		public WatchKit2 (string platform) : base (platform)
		{
		}

		[Test]
		public void BasicTest ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.WatchOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			BuildExtension ("MyWatchApp2", "MyWatchKit2Extension");
		}

		public override string TargetFrameworkIdentifier {
			get {
				return "Xamarin.WatchOS";
			}
		}
	}
}
