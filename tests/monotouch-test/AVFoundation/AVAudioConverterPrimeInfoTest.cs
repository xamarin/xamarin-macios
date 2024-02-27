using Foundation;
using AVFoundation;
using NUnit.Framework;
namespace MonoTouchFixtures.AVFoundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AVAudioConverterPrimeInfoTest {

		[Test]
		public void ConstructorTest ()
		{
			uint leading = 2;
			uint trailing = 30;

			var info = new AVAudioConverterPrimeInfo (leading, trailing);

			Assert.AreEqual (leading, info.LeadingFrames, "Wrong LeadingFrames value.");
			Assert.AreEqual (trailing, info.TrailingFrames, "Wrong TrailingFrames value.");
		}

		[Test]
		public void AreEqualTrueTest ()
		{
			uint leading = 2;
			uint trainling = 20;
			var info1 = new AVAudioConverterPrimeInfo (leading, trainling);
			var info2 = new AVAudioConverterPrimeInfo (leading, trainling);

			Assert.True (info1 == info2, "info1 == info2");
			Assert.True (info1.Equals (info2), "info1.Equals (info2)");
			Assert.False (info1 != info2, "info1 != info2");
		}

		[Test]
		public void AreEqualFalseTest ()
		{
			var info1 = new AVAudioConverterPrimeInfo (2, 30);
			var info2 = new AVAudioConverterPrimeInfo (info1.LeadingFrames * 2, info1.TrailingFrames * 2);
			Assert.False (info1 == info2, "info1 == info2");
			Assert.False (info1.Equals (info2), "info1.Equals (info2)");
			Assert.True (info1 != info2, "info1 != info2");
		}

		[Test]
		public void AreEqualDiffType ()
		{
			var info = new AVAudioConverterPrimeInfo (2, 20);
			var str = new NSString ("Foo");
			Assert.False (info.Equals ((object) str));
		}
	}
}
