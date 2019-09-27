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
			browser.DispatchQueue = DispatchQueue.DefaultGlobalQueue;
		}

		[TearDown]
		public void TearDown ()
		{
			descriptor?.Dispose ();
			browser?.Dispose ();
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
		public void TestDispatchQueuPropertyNull () => Assert.Throws<ArgumentNullException> (() => { browser.DispatchQueue = null; });

		[Test]
		public void TestStart ()
		{
			browser.Start ();
			Assert.IsTrue (browser.Started);
			browser.Cancel ();
		}

		[Test]
		public void TestStartNoQ () {
			using (var newBrowser = new NWBrowser (descriptor))
				Assert.Throws<InvalidOperationException> (() => newBrowser.Start ());
		}

		[Test]
		public void TestStateChangesHandler ()
		{
			var e = new AutoResetEvent (false);
			browser.StateChangesHandler = (st, er) => {
				Assert.IsNotNull (st, "State");
				Assert.IsNull (er, "Error");
				e.Set ();
			};
			browser.Start ();
			e.WaitOne ();
			browser.Cancel ();
		}
	}
}
#endif