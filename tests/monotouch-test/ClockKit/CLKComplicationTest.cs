#if __WATCHOS__
using System;
using NUnit.Framework;

using ClockKit;
using Foundation;

namespace MonoTouchFixtures.ClockKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CLKComplicationTest {

		[SetUp]
		public void Setup ()
		{
			TestRuntime.AssertXcodeVersion (12, 0);
		}

		[Test]
		public void GetAllComplicationFamiliesTest ()
		{
			var families = CLKComplication.GetAllComplicationFamilies ();
			Assert.IsNotNull (families, "Families should not be null.");
		}
	}
}
#endif
