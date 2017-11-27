#if __IOS__
using System;
using System.Threading;

#if XAMCORE_2_0
using Foundation;
using Security;
using ObjCRuntime;
#else
using MonoTouch;
using MonoTouch.Foundation;
using MonoTouch.Security;
#endif
using NUnit.Framework;

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
			domainName = "com.xamarin.monotouch-test";
			account = "twitter";
			password = "12345678";
			waitEvent = new AutoResetEvent (false);
		}

		[Test]
		[ExpectedException (typeof (ArgumentNullException))]
		public void AddSharedWebCredentialNullDomain ()
		{
			domainName = null;
			Action<NSError> handler = (NSError e) =>  {
				Assert.IsNull (e);
				waitEvent.Set ();
			};
			SecSharedCredential.AddSharedWebCredential (domainName, account, password, handler); 
			waitEvent.WaitOne ();
			Assert.Pass ("Block was correctly executed.");
		}

		[Test]
		[ExpectedException (typeof (ArgumentNullException))]
		public void AddSharedWebCredentialNullAccount ()
		{
			account = null;
			Action<NSError> handler = (NSError e) =>  {
				Assert.IsNull (e);
				waitEvent.Set ();
			};
			SecSharedCredential.AddSharedWebCredential (domainName, account, password, handler); 
			waitEvent.WaitOne ();
			Assert.Pass ("Block was correctly executed.");
		}

		[Test]
		// We do not want to block for a long period of time if the event is not set.
		// We are testing the fact that the trampoline works.
		[Timeout (5000)]
		public void AddSharedWebCredentialNotNullPassword ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (8, 0))
				Assert.Ignore ("Ignoring AddSharedWebCredentialNotNullPassword test: Requires iOS8+");

			Action<NSError> handler = (NSError e) =>  {
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
			if (!TestRuntime.CheckSystemAndSDKVersion (8, 0))
				Assert.Ignore ("Ignoring AddSharedWebCredentialNullPassword test: Requires iOS8+");

			password = null;
			Action<NSError> handler = (NSError e) =>  {
				// we do nothing, if we did block the test should be interactive because a dialog is shown.
			};
			SecSharedCredential.AddSharedWebCredential (domainName, account, password, handler); 
		}

		[Test]
		public void CreateSharedWebCredentialPassword ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (8, 0))
				Assert.Ignore ("Ignoring CreateSharedWebCredentialPassword test: Requires iOS8+");

			var pwd = SecSharedCredential.CreateSharedWebCredentialPassword ();
			Assert.IsNotNull (pwd);
		}

 	}
}
#endif // __IOS__
