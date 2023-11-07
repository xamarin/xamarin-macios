using NUnit.Framework;

using Xamarin.Tests;
using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	[TestFixture ("iPhone")]
	[TestFixture ("iPhoneSimulator")]
	public class IBToolLinking : ProjectTest {
		public IBToolLinking (string platform) : base (platform)
		{
		}

		[Test]
		public void BuildTest ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			BuildProject ("MyIBToolLinkTest");
		}
	}
}
