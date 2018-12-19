using System;

using CoreGraphics;
using CoreText;
using Foundation;
using UIKit;

using NUnit.Framework;
using MonoTouchFixtures.CoreGraphics;

namespace MonoTouchFixtures.CoreText {

	class MyOps : CTRunDelegateOperations {

		static public bool Ascent;
		static public bool Descent;
		static public bool Width;

		public MyOps ()
		{
			// to re-run the test
			Ascent = false;
			Descent = false;
			Width = false;
		}

		public override float GetAscent ()
		{
			Ascent = true;
			return base.GetAscent ();
		}

		public override float GetDescent ()
		{
			Descent = true;
			return base.GetDescent ();
		}

		public override float GetWidth ()
		{
			Width = true;
			return base.GetWidth ();
		}
	}

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class RunTest {

		[Test]
		public void CustomOps ()
		{
			using (var o = new MyOps ())
			using (var d = new CTRunDelegate (o)) {
				Assert.AreSame (o, d.Operations, "same");
			}
		}

		[Test]
		public void Runs ()
		{
			using (var mas = new NSMutableAttributedString ("Bonjour"))
			using (var rd = new CTRunDelegate (new MyOps ())) {
				var sa = new CTStringAttributes () {
					RunDelegate = rd,
				};
				mas.SetAttributes (sa, new NSRange (3, 3));
				using (var fs = new CTFramesetter (mas)) {
					Assert.True (MyOps.Ascent, "Ascent called");
					Assert.True (MyOps.Descent, "Descent called");
					Assert.True (MyOps.Width, "Width called");
				}
			}
		}
	}
}
