//
// Unit tests for NSMutableString
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

using System;
using System.Drawing;
#if XAMCORE_2_0
using Foundation;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
#else
using MonoTouch.Foundation;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

#if XAMCORE_2_0
using RectangleF=CoreGraphics.CGRect;
using SizeF=CoreGraphics.CGSize;
using PointF=CoreGraphics.CGPoint;
#else
using nfloat=global::System.Single;
using nint=global::System.Int32;
using nuint=global::System.UInt32;
#endif

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MutableStringTest {

		[Test]
		public void Insert ()
		{
			using (var s = new NSMutableString (-1)) {
				s.SetString ((NSString)"Hello World");
				s.Insert ((NSString)"!", s.Length);
				Assert.That (s.ToString (), Is.EqualTo ("Hello World!"), "end");

				Assert.Throws<ArgumentOutOfRangeException> (delegate {
					s.Insert ((NSString)"@", -1);
				}, "negative");
				Assert.Throws<ArgumentOutOfRangeException> (delegate {
					s.Insert ((NSString)"oops", s.Length + 1);
				}, "out of range");
			}
		}

		[Test]
		public void Replace ()
		{
			using (var s = new NSMutableString (0)) {
				s.SetString ((NSString)"Hello World");

				var number = s.ReplaceOcurrences ((NSString)"World", (NSString)"Xamarin", 
					NSStringCompareOptions.CaseInsensitiveSearch, new NSRange (0, s.Length));
#if XAMCORE_2_0
				Assert.That (number, Is.EqualTo ((nuint) 1), "Number of replacements");
#else
				Assert.That (number, Is.EqualTo (1), "Number of replacements");
#endif
				Assert.That (s.ToString (), Is.EqualTo ("Hello Xamarin"), "replaced");

				Assert.Throws<ArgumentOutOfRangeException> (delegate {
					s.ReplaceOcurrences ((NSString)"Xamarin", (NSString)"World!", 
					                     NSStringCompareOptions.CaseInsensitiveSearch, new NSRange (0, s.Length + 1));
				}, "bad 1");

				Assert.Throws<ArgumentOutOfRangeException> (delegate {
					s.ReplaceOcurrences ((NSString)"Xamarin", (NSString)"World!", 
					                     NSStringCompareOptions.CaseInsensitiveSearch, new NSRange (1, s.Length));
				}, "bad 2");
			}
		}

		[Test]
		public void Delete ()
		{
			using (var s = new NSMutableString (0)) {
				s.SetString ((NSString)"Hello World");
				s.DeleteCharacters (new NSRange (5, 6));
				Assert.That (s.ToString (), Is.EqualTo ("Hello"), "deleetd");

				Assert.Throws<ArgumentOutOfRangeException> (delegate {
					s.DeleteCharacters (new NSRange (0, s.Length + 1));
				}, "bad 1");

				Assert.Throws<ArgumentOutOfRangeException> (delegate {
					s.DeleteCharacters (new NSRange (1, s.Length));
				}, "bad 2");
			}
		}
	}
}
