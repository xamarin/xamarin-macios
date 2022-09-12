using NUnit.Framework;

namespace Xamarin.MacDev.Tasks
{
	[TestFixture ("iPhone")]
	[TestFixture ("iPhoneSimulator")]
	public class IBToolLinking : ProjectTest
	{
		public IBToolLinking (string platform) : base (platform)
		{
		}

		[Test]
		public void BuildTest ()
		{
			BuildProject ("MyIBToolLinkTest");
		}
	}
}
