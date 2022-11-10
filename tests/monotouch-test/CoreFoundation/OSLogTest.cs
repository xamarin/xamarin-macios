using Foundation;

using NUnit.Framework;

namespace MonoTouchFixtures.CoreFoundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class OSLogTest {

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (8, 0);
		}

		[Test]
		public void Default ()
		{
			global::CoreFoundation.OSLog.Default.Log (global::CoreFoundation.OSLogLevel.Default, "monotouch-test / Default / Default");
			// this will show in the application output (e.g. inside VSfM)
		}

		[Test]
		public void Custom ()
		{
			using (var log = new global::CoreFoundation.OSLog ("subsystem", "category")) {
				log.Log (global::CoreFoundation.OSLogLevel.Error, "monotouch-test /  custom / Debug");
				// this will show in the application output (e.g. inside VSfM)
				// and also inside Console.app under the simulator/device
			}
		}
	}
}
