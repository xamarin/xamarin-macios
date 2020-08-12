using System;
using System.IO;
using System.Runtime.InteropServices;

using CoreFoundation;
using Foundation;
using ObjCRuntime;

using NUnit.Framework;

namespace MonoTouchFixtures.CoreFoundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class OSLogTest {

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (8,0);
		}

		[Test]
		public void Default ()
		{
			OSLog.Default.Log (OSLogLevel.Default, "monotouch-test / Default / Default");
			// this will show in the application output (e.g. inside VSfM)
		}

		[Test]
		public void Custom ()
		{
			using (var log = new OSLog ("subsystem", "category")) {
				log.Log (OSLogLevel.Error, "monotouch-test /  custom / Debug");
				// this will show in the application output (e.g. inside VSfM)
				// and also inside Console.app under the simulator/device
			}
		}
	}
}
