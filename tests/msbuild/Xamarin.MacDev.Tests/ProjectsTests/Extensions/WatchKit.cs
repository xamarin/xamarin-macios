using NUnit.Framework;

using Xamarin.Tests;
using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	[TestFixture ("iPhone")]
	[TestFixture ("iPhoneSimulator")]
	public class WatchKit : ExtensionTestBase {

		public WatchKit (string platform) : base (platform)
		{
		}

		[Test]
		public void BasicTest ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.WatchOS);
			Configuration.AssertLegacyXamarinAvailable ();

			BuildExtension ("MyWatchApp", "MyWatchKitExtension", expectedErrorCount: 1);
			Assert.AreEqual ("Xamarin.iOS 14+ does not support watchOS 1 apps. Please migrate your project to watchOS 2+.", Engine.Logger.ErrorEvents [0].Message, "WK 1 error message");
		}
	}
}
