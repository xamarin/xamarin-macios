using CoreData;
using Foundation;
using NUnit.Framework;

namespace MonoTouchFixtures.CoreData {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSQueryGenerationTokenTest {
		[Test]
		public void EncodeWithCoderTest ()
		{
			// Added test to ensure we do support NSCoding even when introspection fails.
			if (TestRuntime.CheckXcodeVersion (9, 0)) {
				using (var data = new NSMutableData ())
				using (var archiver = new NSKeyedArchiver (data))
				using (var coder = new NSCoder ()) {
					NSQueryGenerationToken.CurrentToken.EncodeTo (archiver);
				}
			} else {
				Assert.Ignore ("NSCoding is not supported prior Xcode 9.");
			}
		}
	}
}
