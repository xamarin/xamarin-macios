#if __IOS__

using System;

using CoreFoundation;
using Foundation;
using MetricKit;
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.MetricKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MetricManagerTest {

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (11, 0);
		}

		[Test]
		public void MakeLogHandle ()
		{
			using (var ns = new NSString ("xamarin"))
			using (var lh = MXMetricManager.MakeLogHandle (ns)) {
				Assert.That (lh.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
			}
		}
	}
}

#endif
