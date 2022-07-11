using NUnit.Framework;

namespace Xamarin.iOS.Tasks {
	[TestFixture ("iPhone")]
	[TestFixture ("iPhoneSimulator")]
	public class ActionTests : ExtensionTestBase
	{
		public ActionTests (string platform) : base (platform)      
		{
		}

		[Test]
		public void BasicTest ()
		{
			BuildExtension ("MyTabbedApplication", "MyActionExtension");
		}
	}
}
