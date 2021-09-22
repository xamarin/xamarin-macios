#if !__WATCHOS__
using Foundation;
using CoreAnimation;
using NUnit.Framework;

namespace MonoTouchFixtures.CoreAnimation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CAFrameRateRangeTest {

		[SetUp]
		public void SetUp ()
			=> TestRuntime.AssertXcodeVersion (13,0);

		[Test]
		public void IsEqualToTest ()
		{
			var first = new CAFrameRateRange (1.0f, 1.0f, 1.0f);
			var second = new CAFrameRateRange (1.5f, 1.0f, 1.0f);
			
			Assert.IsFalse (first.IsEqualTo (second), "Not equal");
			Assert.IsTrue (first.IsEqualTo (first), "Equal");
		}

		[Test]
		public void DefaultTest ()
		{
			var defaultValue = CAFrameRateRange.Default;
			Assert.IsTrue (defaultValue.IsEqualTo (defaultValue), "Default");
		}
	}
}
#endif
