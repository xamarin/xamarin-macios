using System.IO;

using NUnit.Framework;

namespace Xamarin.iOS.Tasks
{
	[TestFixture ("iPhone")]
	[TestFixture ("iPhoneSimulator")]
	public class LinkedAssets : ProjectTest
	{
		static readonly string[] IconNames = { "AppIcon29x29.png", "AppIcon29x29@2x.png", "AppIcon40x40@2x.png", "AppIcon57x57.png", "AppIcon57x57@2x.png", "AppIcon60x60@2x.png" };

		public LinkedAssets (string platform) : base (platform)
		{
		}

		[Test]
		public void BuildTest ()
		{
			BuildProject ("MyLinkedAssets");

			foreach (var name in IconNames) {
				var path = Path.Combine (AppBundlePath, name);

				Assert.IsTrue (File.Exists (path), "The expected icon `{0}' does not exist.", name);
			}
		}
	}
}
