using System.IO;

using NUnit.Framework;

using Xamarin.Tests;
using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	[TestFixture ("iPhone")]
	[TestFixture ("iPhoneSimulator")]
	public class LinkedAssets : ProjectTest {
		static readonly string [] IconNames = { "AppIcon60x60@2x.png" };

		public LinkedAssets (string platform) : base (platform)
		{
		}

		[Test]
		public void BuildTest ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			BuildProject ("MyLinkedAssets");

			foreach (var name in IconNames) {
				var path = Path.Combine (AppBundlePath, name);

				Assert.That (path, Does.Exist, "The expected icon `{0}' does not exist.", name);
			}
		}
	}
}
