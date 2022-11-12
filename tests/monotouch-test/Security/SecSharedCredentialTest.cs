#if __IOS__
using System;
using System.Threading;

using Foundation;
using Security;
using ObjCRuntime;
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.Security {

	[TestFixture]
	// we want the test to be availble if we use the linker
	[Preserve (AllMembers = true)]
	public class SecSharedCredentialTest {

		string domainName;
		string account;
		string password;
		AutoResetEvent waitEvent;

		[SetUp]
		public void SetUp ()
		{
			// The API here was introduced to Mac Catalyst later than for the other frameworks, so we have this additional check
			TestRuntime.AssertSystemVersion (ApplePlatform.MacCatalyst, 14, 0, throwIfOtherPlatform: false);

			domainName = "com.xamarin.monotouch-test";
			account = "twitter";
			password = "12345678";
			waitEvent = new AutoResetEvent (false);
		}

		[Test]
		public void AddSharedWebCredentialNullDomain ()
		{
			domainName = null;
			Action<NSError> handler = (NSError e) => {
			};
			Assert.Throws<ArgumentNullException> (() => SecSharedCredential.AddSharedWebCredential (domainName, account, password, handler));
		}

		[Test]
		public void AddSharedWebCredentialNullAccount ()
		{
			account = null;
			Action<NSError> handler = (NSError e) => {
			};
			Assert.Throws<ArgumentNullException> (() => SecSharedCredential.AddSharedWebCredential (domainName, account, password, handler));
		}

		[Test]
		// We do not want to block for a long period of time if the event is not set.
		// We are testing the fact that the trampoline works.
		[Timeout (5000)]
		public void AddSharedWebCredentialNotNullPassword ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 8, 0, throwIfOtherPlatform: false);

			Action<NSError> handler = (NSError e) => {
				// we do nothing, if we did block the test should be interactive because a dialog is shown.
			};
			SecSharedCredential.AddSharedWebCredential (domainName, account, password, handler);
		}

		[Test]
		// We do not want to block for a long period of time if the event is not set.
		// We are testing the fact that the trampoline works.
		[Timeout (5000)]
		public void AddSharedWebCredentialNullPassword ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 8, 0, throwIfOtherPlatform: false);

			password = null;
			Action<NSError> handler = (NSError e) => {
				// we do nothing, if we did block the test should be interactive because a dialog is shown.
			};
			SecSharedCredential.AddSharedWebCredential (domainName, account, password, handler);
		}

		[Test]
		public void CreateSharedWebCredentialPassword ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 8, 0, throwIfOtherPlatform: false);

			var pwd = SecSharedCredential.CreateSharedWebCredentialPassword ();
			Assert.IsNotNull (pwd);
		}

	}
}
#endif // __IOS__
