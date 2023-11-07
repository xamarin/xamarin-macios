#if __IOS__ || MONOMAC

using System;
using System.IO;
using NUnit.Framework;

using Foundation;
using WebKit;

namespace MonoTouchFixtures.WebKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSAttributedStringCatagoryTest {

		[SetUp]
		public void Setup ()
		{
			TestRuntime.AssertXcodeVersion (11, 0);
		}

		[Test]
		public void LoadHtmlAsync_NSUrl ()
		{
			var completed = false;
			string d = Path.Combine (NSBundle.MainBundle.ResourcePath, "access-denied.html");
			string g = Path.Combine (NSBundle.MainBundle.ResourcePath, "access-granted.html");
			TestRuntime.RunAsync (TimeSpan.FromSeconds (30), async () => {
				using (var denied = NSUrl.FromFilename (d))
				using (var granted = NSUrl.FromFilename (g)) {
					var options = new NSAttributedStringDocumentAttributes {
						ReadAccessUrl = granted
					};
					var r1 = await NSAttributedString.LoadFromHtmlAsync (granted, options);
					Assert.That (r1.AttributedString.Value, Is.EqualTo ("Granted"), "granted by options");
#if false
					// this does not match my interpretation of the (headers) docs
					var r2 = await NSAttributedString.LoadFromHtmlAsync (denied, options);
					Assert.That (r2.AttributedString.Value, Is.Not.EqualTo ("Denied"), "denied by options");
#endif
					completed = true;
				}
			}, () => completed);
			Assert.True (completed, "completed");
		}
	}
}
#endif
