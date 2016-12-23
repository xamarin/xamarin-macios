using System;
using System.IO;

using NUnit.Framework;
using Xamarin.MacDev;
using System.Diagnostics;

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
			
			this.BuildExtension ("MyWatchApp2", "MyWatchKit2Extension", Platform, "Debug");
		}

		public override string TargetFrameworkIdentifier {
			get {
				return "Xamarin.WatchOS";
			}
		}
	}
}

