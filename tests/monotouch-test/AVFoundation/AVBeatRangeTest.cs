using Foundation;
using AVFoundation;
using NUnit.Framework;
namespace MonoTouchFixtures.AVFoundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AVBeatRangeTest {

		[Test]
		public void ConstructorTest ()
		{
			double start = 2.3;
			double length = 30.90;

			var range = new AVBeatRange (start, length);

			Assert.AreEqual (start, range.Start, "Wrong Start value.");
			Assert.AreEqual (length, range.Length, "Wrong Length value.");
		}

		[Test]
		public void AreEqualTrueTest ()
		{
			double start = 2.3;
			double length = 30.90;
			var range1 = new AVBeatRange (start, length);
			var range2 = new AVBeatRange (start, length);

			Assert.True (range1 == range2, "range1 == range2");
			Assert.True (range1.Equals (range2), "range1.Equals (range1)");
			Assert.False (range1 != range2, "range1 != range2");
		}

		[Test]
		public void AreEqualFalseTest ()
		{
			var range1 = new AVBeatRange (90, 12);
			var range2 = new AVBeatRange (range1.Start * 2, range1.Length * 2);
			Assert.False (range1 == range2, "range1 == range2");
			Assert.False (range1.Equals (range2), "range1.Equals (range2)");
			Assert.True (range1 != range2, "range1 != range2");
		}

		[Test]
		public void AreEqualDiffType ()
		{
			var range = new AVBeatRange (90, 12);
			var str = new NSString ("Foo");
			Assert.False (range.Equals ((object) str));
		}
	}
}
