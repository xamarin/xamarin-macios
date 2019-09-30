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
	public class NWListenerTest {

		NWListener listener;
		NWParameters parameters;

		[SetUp]
		public void SetUp ()
		{
			parameters = new NWParameters ();
			listener = NWListener.Create (parameters);
		}

		[TearDown]
		public void TearDown ()
		{
			listener?.Dispose ();
			parameters?.Dispose ();
		}

		[Test]
		public void TestConnectionLimit ()
		{
			TestRuntime.AssertXcodeVersion (11, 0);

			var defaultValue = 4294967295; // got it from running the code, if changes we will have an error.
			Assert.AreEqual (defaultValue, listener.ConnectionLimit);
			listener.ConnectionLimit = 10;
			Assert.AreEqual (10, listener.ConnectionLimit, "New value was not stored.");
		}
	}
}
#endif