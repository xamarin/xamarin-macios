#if __MACOS__
using System;
using NUnit.Framework;

using AppKit;
using ObjCRuntime;
using Foundation;
using CoreGraphics;

namespace Xamarin.Mac.Tests {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSStringTests {
		[Test]
		public void NSString_LineRangeForRange ()
		{
			// Test from http://stackoverflow.com/questions/1085524/how-to-count-the-number-of-lines-in-an-objective-c-string-nsstring
			NSString input = new NSString ("Hey\nHow\nYou\nDoing");
			int stringLength = (int) input.Length;
			int numberOfLines = 0;
			for (int index = 0; index < stringLength; numberOfLines++) {
				NSRange range = input.LineRangeForRange (new NSRange (index, 0));
				index = (int) (range.Location + range.Length);
			}
			Assert.AreEqual (4, numberOfLines);
		}

		[Test]
		public void NSString_GetLineStart ()
		{
			NSString input = new NSString ("Hey\nHow\nYou\nDoing");
			nuint start, lineEnd, contentsEnd;
			input.GetLineStart (out start, out lineEnd, out contentsEnd, new NSRange (5, 11));
			Assert.AreEqual ((nuint) 4, start);
			Assert.AreEqual ((nuint) 17, lineEnd);
			Assert.AreEqual ((nuint) 17, contentsEnd);
		}

		[Test]
		public void NSString_BoundingRectWithSize ()
		{
			NSString input = new NSString ("Hey\nHow\nYou\nDoing");
			CGRect rect = input.BoundingRectWithSize (new CGSize (20, 30), NSStringDrawingOptions.UsesLineFragmentOrigin | NSStringDrawingOptions.UsesFontLeading, new NSDictionary ());
			Assert.IsTrue (rect.Width > 0);
			Assert.IsTrue (rect.Height > 0);
		}

		[Test]
		public void NSString_CompareTo ()
		{
			using (var a = new NSString ("a"))
			using (var b = new NSString ("b"))
			using (var c = new NSString ("c")) {
				var tests = new NSString [] { c, a, b };

				Array.Sort (tests);

				Assert.AreSame (a, tests [0], "0");
				Assert.AreSame (b, tests [1], "1");
				Assert.AreSame (c, tests [2], "2");
			}
		}
	}

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSAttributedStringTests {
		[Test]
		public void NSAttributedString_BoundingRectWithSize ()
		{
			NSFont font = NSFont.FromFontName ("Arial", 40);
			NSAttributedString str = new NSAttributedString ("Hello World", font);
			CGRect rect = str.BoundingRectWithSize (new CGSize (20, 30), NSStringDrawingOptions.UsesLineFragmentOrigin | NSStringDrawingOptions.UsesFontLeading);
			Assert.IsTrue (rect.Width > 0);
			Assert.IsTrue (rect.Height > 0);
		}

		[Test]
		public void NSAttributedString_GetUrl ()
		{
			NSRange range;
			var str = new NSAttributedString ("Test string with url: http://www.google.com");
			var url = str.GetUrl (42, out range);

			Assert.IsNotNull (url);
			Assert.IsTrue (url.AbsoluteString == "http://www.google.com");
			Assert.IsTrue (range.Location == 22);
			Assert.IsTrue (range.Length == 21);
		}
	}
}
#endif // __MACOS__
