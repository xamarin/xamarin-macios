using Foundation;
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CachedUrlResponseTest {

		[Test]
		public void ConstructorTest ()
		{
			// See https://bugzilla.xamarin.com/show_bug.cgi?id=3835
			using (var response = new NSUrlResponse ())
			using (var data = new NSData ()) {

				// Test that UserInfo is NullAllowed
				using (var res1 = new NSCachedUrlResponse (response, data, null, NSUrlCacheStoragePolicy.Allowed)) {
					Assert.That (res1.StoragePolicy, Is.EqualTo (NSUrlCacheStoragePolicy.Allowed), "StoragePolicy-1");
					Assert.Null (res1.UserInfo, "UserInfo-1");
				}

				using (var res2 = new NSCachedUrlResponse (response, data)) {
					Assert.That (res2.StoragePolicy, Is.EqualTo (NSUrlCacheStoragePolicy.Allowed), "StoragePolicy-2");
					Assert.Null (res2.UserInfo, "UserInfo-2");
				}
			}
		}
	}
}
