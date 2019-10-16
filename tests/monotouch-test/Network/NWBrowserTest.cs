#if !__WATCHOS__
using System;
using System.Collections.Generic;
using System.Threading;
#if XAMCORE_2_0
using CoreFoundation;
using Foundation;
using Network;
using ObjCRuntime;
using Security;
#else
using MonoTouch.CoreFoundation;
using MonoTouch.Foundation;
using MonoTouch.Network;
using MonoTouch.Security;
#endif

using NUnit.Framework;

namespace MonoTouchFixtures.Network {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NWBrowserTest {

		NWBrowserDescriptor descriptor;
		NWBrowser browser;
		string type = "_ssh._tcp.";
		string domain = "local.";

		[TestFixtureSetUp]
		public void Init () => TestRuntime.AssertXcodeVersion (11, 0);

		[SetUp]
		public void SetUp ()
		{
			descriptor = NWBrowserDescriptor.CreateBonjourService (type, domain);
			browser = new NWBrowser (descriptor);
			browser.SetDispatchQueue (DispatchQueue.DefaultGlobalQueue);
		}

		[TearDown]
		public void TearDown ()
		{
			descriptor.Dispose ();
			browser.Dispose ();
		}

		[Test]
		public void TestConstructorNullParameters ()
		{
			using (var otherBrowser = new NWBrowser (descriptor)) {
				Assert.IsNotNull (otherBrowser.Descriptor, "Descriptor");
				// we expect the default parameters
				Assert.IsNotNull (otherBrowser.Parameters, "Parameters");
			}
		}

		[Test]
		public void TestDispatchQueuPropertyNull () => Assert.Throws<ArgumentNullException> (() => { browser.SetDispatchQueue (null); });

		[Test]
		public void TestStart ()
		{
			Assert.IsFalse (browser.IsActive, "Idle");
			browser.Start ();
			Assert.IsTrue (browser.IsActive, "Active");
			browser.Cancel ();
			Assert.IsFalse (browser.IsActive, "Cancel");
		}

		[Test]
		public void TestStartNoQ () {
			using (var newBrowser = new NWBrowser (descriptor))
				Assert.Throws<InvalidOperationException> (() => newBrowser.Start ());
		}

		// ATTENTION!!! This ignore has to be removed when the Xcode11.2 branch is merged, the test there if fixed and should ne executed.
		[Ignore ("This tests crashes because the API is missused. This has been fixed in PR: https://github.com/xamarin/xamarin-macios/pull/7214")]
		[Test]
		public void TestStateChangesHandler ()
		{
			var e = new AutoResetEvent (false);
			browser.SetStateChangesHandler ((st, er) => {
				Assert.IsNotNull (st, "State");
				Assert.IsNull (er, "Error");
				e.Set ();
			});
			browser.Start ();
			e.WaitOne ();
			browser.Cancel ();
		}
	}
}
#endif