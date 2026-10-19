//
// Unit tests for CTFrame
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

using CoreText;
using CoreGraphics;

namespace MonoTouchFixtures.CoreText {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CTFrameTests {

		[Test] // #4677
		public void GetPathTest ()
		{
			using (var framesetter = new CTFramesetter (new NSAttributedString ("Testing, testing, 1, 2, 3..."))) {
				using (var frame = framesetter.GetFrame (new NSRange (0, 0), new CGPath (), null)) {
					using (var f = frame.GetPath ()) {
					}
					using (var f = frame.GetPath ()) {
						Console.WriteLine (f.BoundingBox);
					}
				}
			}
		}

		[TestCase (0, 1)]
		[TestCase (1, 1)]
		[TestCase (1, 2)]
		[TestCase (0, 0)]
		[TestCase (1, 0)]
		[TestCase (2, 0)]
		public void GetLineOrigins (int location, int length)
		{
			using var text = new NSAttributedString ("First line\nSecond line\nThird line");
			using var framesetter = new CTFramesetter (text);
			using var path = new CGPath ();
			path.AddRect (new CGRect (0, 0, 500, 500));
			using var frame = framesetter.GetFrame (new NSRange (0, 0), path, null);
			var lines = frame.GetLines ();
			try {
				Assert.That (lines.Length, Is.EqualTo (3), "Line count");
			} finally {
				foreach (var line in lines)
					line.Dispose ();
			}

			var allOrigins = new CGPoint [3];
			frame.GetLineOrigins (new NSRange (0, 0), allOrigins);
			Assert.That (allOrigins [0].Y, Is.GreaterThan (allOrigins [1].Y), "First line above second");
			Assert.That (allOrigins [1].Y, Is.GreaterThan (allOrigins [2].Y), "Second line above third");

			var count = length == 0 ? allOrigins.Length - location : length;
			var origins = new CGPoint [count];
			frame.GetLineOrigins (new NSRange (location, length), origins);
			for (var i = 0; i < count; i++)
				Assert.That (origins [i], Is.EqualTo (allOrigins [location + i]), $"Origin {i}");

			var tooSmall = Assert.Throws<ArgumentException> (() => frame.GetLineOrigins (new NSRange (location, length), new CGPoint [count - 1]));
			Assert.That (tooSmall.ParamName, Is.EqualTo ("origins"), "Small buffer parameter");
			var nullBuffer = Assert.Throws<ArgumentNullException> (() => frame.GetLineOrigins (new NSRange (location, length), null));
			Assert.That (nullBuffer.ParamName, Is.EqualTo ("origins"), "Null buffer parameter");
		}

		[Test]
		public void CTTypesetterCreateTest ()
		{
			TestRuntime.AssertXcodeVersion (10, 0);
			using (var framesetter = new CTFramesetter (new NSAttributedString ("Testing, testing, 1, 2, 3...")))
			using (var type = framesetter.GetTypesetter ())
			using (var newFrame = CTFramesetter.Create (type)) {
				Assert.That (type, Is.Not.Null, "Create");
				var type2 = newFrame.GetTypesetter ();
				Assert.That (type, Is.Not.Null, "type2");
				Assert.That (type2.Handle, Is.EqualTo (type.Handle), "Same typesetter");
			}
		}
	}
}
