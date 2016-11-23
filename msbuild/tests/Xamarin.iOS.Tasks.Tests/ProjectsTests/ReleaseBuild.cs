using NUnit.Framework;

namespace Xamarin.iOS.Tasks
{
	[TestFixture ("iPhone")]
	public class ReleaseBuild : ProjectTest
	{
		public ReleaseBuild (string platform) : base (platform)
		{
		}

		[Test]
		public void BuildTest ()
		{
			BuildProject ("MyReleaseBuild", Platform, "Release");
		}
	}
}
