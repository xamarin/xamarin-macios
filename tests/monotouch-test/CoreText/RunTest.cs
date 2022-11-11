#if !__WATCHOS__

using System;

using CoreGraphics;
using CoreText;
using Foundation;
using ObjCRuntime;

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

#if NET
		public override nfloat GetAscent ()
#else
		public override float GetAscent ()
#endif
		{
			Ascent = true;
			return base.GetAscent ();
		}

#if NET
		public override nfloat GetDescent ()
#else
		public override float GetDescent ()
#endif
		{
			Descent = true;
			return base.GetDescent ();
		}

#if NET
		public override nfloat GetWidth ()
#else
		public override float GetWidth ()
#endif
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

		[Test]
		public void GetBaseAdvancesAndOrigins ()
		{
			TestRuntime.AssertXcodeVersion (11, 0);

			using (var attributedString = new NSAttributedString ("Hello world."))
			using (var line = new CTLine (attributedString)) {
				var runs = line.GetGlyphRuns ();
				Assert.That (runs.Length, Is.EqualTo (1), "runs");
				runs [0].GetBaseAdvancesAndOrigins (new NSRange (0, 10), out var advances, out var origins);
				Assert.IsNotNull (advances, "advances");
				Assert.IsNotNull (origins, "origins");
			}
		}
	}
}

#endif
