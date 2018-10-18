using System.IO;

using NUnit.Framework;

namespace Xamarin.iOS.Tasks
{
	[TestFixture ("iPhone")]
	[TestFixture ("iPhoneSimulator")]
	public class SiriIntents : ProjectTest
	{
		public SiriIntents (string platform) : base (platform)
		{
		}

		[Test]
		public void BuildTest ()
		{
			BuildProject ("MyIntentsApp", Platform, "Debug");

			var path = Path.Combine (AppBundlePath, "Intents.intentdefinition");

			Assert.IsTrue (File.Exists (path), "`Intents.intentdefinition' does not exist.");
		}
	}
}
