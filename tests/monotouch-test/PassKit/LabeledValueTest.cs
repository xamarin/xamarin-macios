#if __IOS__

using System;
using Foundation;
using PassKit;
using NUnit.Framework;

namespace MonoTouchFixtures.PassKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class PKLabeledValueTest {

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (8, 1);
		}

		[Test]
		public void Constructor ()
		{
			using (var lv = new PKLabeledValue ("", "")) {
				Assert.That (lv.Label, Is.Empty, "Label1");
				Assert.That (lv.Value, Is.Empty, "Value1");
			}
			using (var lv = new PKLabeledValue ("Label", "Value")) {
				Assert.That (lv.Label, Is.EqualTo ("Label"), "Label2");
				Assert.That (lv.Value, Is.EqualTo ("Value"), "Value2");
			}
		}
	}
}

#endif
