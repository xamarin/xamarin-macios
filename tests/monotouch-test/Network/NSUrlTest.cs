#if !__WATCHOS__
using Foundation;

using Network;

using NUnit.Framework;

namespace MonoTouchFixtures.Network {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSUrlTest {
		[Test]
		public void ImplicitConversion ()
		{
			global::System.Uri uri = null;
			NSUrl sUrl = uri;
			Assert.IsNull (sUrl);
		}
	}
}
#endif
