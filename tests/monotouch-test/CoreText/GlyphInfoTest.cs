using System;

using CoreText;

using Foundation;

using NUnit.Framework;

namespace MonoTouchFixtures.CoreText {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class GlyphInfoTest {

		[Test]
		public void GlyphInfo ()
		{
			using (var f = new CTFont ("ArialMY", 24))
			using (var g = new CTGlyphInfo (64, f, "Foo")) {
				Assert.That (g.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");

				if (TestRuntime.CheckXcodeVersion (11, 0)) {
					Assert.That (g.GetGlyph (), Is.EqualTo (64), "GetGlyph");
				}
			}
		}
	}
}
