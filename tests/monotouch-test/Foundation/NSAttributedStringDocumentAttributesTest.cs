using System;

using CoreGraphics;
using Foundation;
using ObjCRuntime;

using NUnit.Framework;

using Xamarin.Utils;


#if MONOMAC
using XColor = AppKit.NSColor;
#else
using XColor = UIKit.UIColor;
#endif

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSAttributedStringDocumentAttributesTest {
		[Test]
		public void DefaultTabInterval ()
		{
			Exception ex;

			var attribs = new NSAttributedStringDocumentAttributes ();
			Assert.IsNull (attribs.DefaultTabInterval, "Initial");
			attribs.DefaultTabInterval = 0.5f;
			Assert.AreEqual (0.5f, attribs.DefaultTabInterval.Value, "Half");
			ex = Assert.Throws<ArgumentOutOfRangeException> (() => { attribs.DefaultTabInterval = -1; }, "Negative 1");
			Assert.AreEqual ("Value must be between 0 and 1 (Parameter 'value')\nActual value was -1.", ex.Message, "Negative 1 - Message");
			ex = Assert.Throws<ArgumentOutOfRangeException> (() => { attribs.DefaultTabInterval = 2; }, "Positive 2");
			Assert.AreEqual ("Value must be between 0 and 1 (Parameter 'value')\nActual value was 2.", ex.Message, "Positive 1 - Message");

			attribs.DefaultTabInterval = 0f;
			Assert.AreEqual (0f, attribs.DefaultTabInterval.Value, "Zero");
			attribs.DefaultTabInterval = 1f;
			Assert.AreEqual (1f, attribs.DefaultTabInterval.Value, "One");
			attribs.DefaultTabInterval = null;
			Assert.IsNull (attribs.DefaultTabInterval, "Null End");
		}

		[Test]
		public void HyphenationFactor ()
		{
			Exception ex;

			var attribs = new NSAttributedStringDocumentAttributes ();
			Assert.IsNull (attribs.HyphenationFactor, "Initial");
			attribs.HyphenationFactor = 0.5f;
			Assert.AreEqual (0.5f, attribs.HyphenationFactor.Value, "Half");
			ex = Assert.Throws<ArgumentOutOfRangeException> (() => { attribs.HyphenationFactor = -1; }, "Negative 1");
			Assert.AreEqual ("Value must be between 0 and 1 (Parameter 'value')\nActual value was -1.", ex.Message, "Negative 1 - Message");
			ex = Assert.Throws<ArgumentOutOfRangeException> (() => { attribs.HyphenationFactor = 2; }, "Positive 2");
			Assert.AreEqual ("Value must be between 0 and 1 (Parameter 'value')\nActual value was 2.", ex.Message, "Positive 1 - Message");

			attribs.HyphenationFactor = 0f;
			Assert.AreEqual (0f, attribs.HyphenationFactor.Value, "Zero");
			attribs.HyphenationFactor = 1f;
			Assert.AreEqual (1f, attribs.HyphenationFactor.Value, "One");
			attribs.HyphenationFactor = null;
			Assert.IsNull (attribs.HyphenationFactor, "Null End");
		}
	}
}
