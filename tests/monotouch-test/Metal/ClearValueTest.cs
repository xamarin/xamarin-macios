
#if !__WATCHOS__

using System;

using Foundation;
using Metal;

using NUnit.Framework;

namespace MonoTouchFixtures.Metal {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ClearValueTest {
		[Test]
		public void Constructor ()
		{
			MTLClearValue value;

			value = new MTLClearValue ();

			Assert.AreEqual (0, value.Color.Alpha, "1-color-alpha");
			Assert.AreEqual (0, value.Color.Blue, "1-color-blue");
			Assert.AreEqual (0, value.Color.Green, "1-color-green");
			Assert.AreEqual (0, value.Color.Red, "1-color-red");
			Assert.AreEqual (0, value.Depth, "1-depth");
			Assert.AreEqual (0, value.Stencil, "1-stencil");

			value = new MTLClearValue (0.2f);

			Assert.AreEqual (0.2f, value.Depth, "2-depth");

			value = new MTLClearValue (123);

			Assert.AreEqual (123, value.Stencil, "3-stencil");

			value = new MTLClearValue (-2);

			Assert.AreEqual (-2, value.Depth, "4-depth");

			value = new MTLClearValue (new MTLClearColor (1, 2, 3, 4));

			Assert.AreEqual (4, value.Color.Alpha, "5-color-alpha");
			Assert.AreEqual (3, value.Color.Blue, "5-color-blue");
			Assert.AreEqual (2, value.Color.Green, "5-color-green");
			Assert.AreEqual (1, value.Color.Red, "5-color-red");
		}
	}
}

#endif // !__WATCHOS__
