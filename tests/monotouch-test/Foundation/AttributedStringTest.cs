using System;
using NUnit.Framework;
#if XAMCORE_2_0
using Foundation;
#if MONOMAC
using AppKit;
using UIColor = AppKit.NSColor;
#else
using UIKit;
#endif
using CoreGraphics;
using ObjCRuntime;
#if !__WATCHOS__
using CoreText;
#endif
#else
using MonoTouch;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
using MonoTouch.CoreText;
#endif

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
	public class AttributedStringTest {

		CGColor red, yellow;
		bool failEnum, t1, t2, tFont1, tFont2;
		
#if !__WATCHOS__
		[Test]
		public void Attributes ()
		{
			red = UIColor.Red.CGColor;
			yellow = UIColor.Yellow.CGColor;
			
			var j = new NSMutableAttributedString ("Hello", new CTStringAttributes() { ForegroundColor = red });
			j.Append (new NSMutableAttributedString ("12345", new CTStringAttributes() { ForegroundColor = yellow }));
			j.EnumerateAttributes (new NSRange (0, 10), NSAttributedStringEnumeration.None, cb);
			Assert.True (t1);
			Assert.True (t2);
			Assert.False (failEnum);
			Assert.True (tFont1);
			Assert.True (tFont2);
		}
#endif // !__WATCHOS__
		
		void cb (NSDictionary attrs, NSRange range, ref bool stop)
		{
			stop = false;
			if (range.Location == 0){
				if (range.Length == 5){
					t1 = true;
					tFont1 = attrs.ContainsKey (new NSString ("CTForegroundColor"));
				}
			} else if (range.Location == 5){
				if (range.Length == 5){
					t2 = true;
					tFont2 = attrs.ContainsKey (new NSString ("CTForegroundColor"));
				}
			} else
				failEnum = true;
		}

		[Test]
		public void Fields ()
		{
			// fields are not available in iOS (at least up to 5.1.1)
			// this test will fail if this ever change in the future
			IntPtr lib = Dlfcn.dlopen (Constants.FoundationLibrary, 0);
			try {
				Assert.That (Dlfcn.dlsym (lib, "NSFontAttributeName"), Is.EqualTo (IntPtr.Zero), "NSFontAttributeName");
				Assert.That (Dlfcn.dlsym (lib, "NSLinkAttributeName"), Is.EqualTo (IntPtr.Zero), "NSLinkAttributeName");
				Assert.That (Dlfcn.dlsym (lib, "NSUnderlineStyleAttributeName"), Is.EqualTo (IntPtr.Zero), "NSUnderlineStyleAttributeName");
				Assert.That (Dlfcn.dlsym (lib, "NSStrikethroughStyleAttributeName"), Is.EqualTo (IntPtr.Zero), "NSStrikethroughStyleAttributeName");
				Assert.That (Dlfcn.dlsym (lib, "NSStrokeWidthAttributeName"), Is.EqualTo (IntPtr.Zero), "NSStrokeWidthAttributeName");
				Assert.That (Dlfcn.dlsym (lib, "NSParagraphStyleAttributeName"), Is.EqualTo (IntPtr.Zero), "NSParagraphStyleAttributeName");
				Assert.That (Dlfcn.dlsym (lib, "NSForegroundColorAttributeName"), Is.EqualTo (IntPtr.Zero), "NSForegroundColorAttributeName");
				Assert.That (Dlfcn.dlsym (lib, "NSBackgroundColorAttributeName"), Is.EqualTo (IntPtr.Zero), "NSBackgroundColorAttributeName");
				Assert.That (Dlfcn.dlsym (lib, "NSLigatureAttributeName"), Is.EqualTo (IntPtr.Zero), "NSLigatureAttributeName");
				Assert.That (Dlfcn.dlsym (lib, "NSObliquenessAttributeName"), Is.EqualTo (IntPtr.Zero), "NSObliquenessAttributeName");
			}
			finally {
				Dlfcn.dlclose (lib);
			}
		}

#if !XAMCORE_2_0
		[Test]
		public void UIKitAttachmentConveniences_Old ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (7,0))
				Assert.Inconclusive ("requires iOS7+");

			NSAttributedString as1 = null;
			// it's weird looking to use an instance (even null) so you can call a *static* category method...
			using (var ta = new NSTextAttachment (null, null))
			using (var as2 = as1.FromTextAttachment (ta)) {
				Assert.That (as2.Length, Is.EqualTo ((nint) 1), "Length");
				Assert.That (as2.Value [0], Is.EqualTo ((char)0xFFFC), "NSAttachmentCharacter");
			}
		}
#endif

#if !__WATCHOS__
		[Test]
		public void UIKitAttachmentConveniences_New ()
		{
			TestRuntime.AssertiOSSystemVersion (7, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertMacSystemVersion (10, 11, throwIfOtherPlatform: false);

			// so we added custom code calling the (old) category helper - but we had to pick a different name
			using (var ta = new NSTextAttachment (null, null))
			using (var as2 = NSAttributedString.FromAttachment (ta)) {
				Assert.That (as2.Length, Is.EqualTo ((nint) 1), "Length");
				Assert.That (as2.Value [0], Is.EqualTo ((char)0xFFFC), "NSAttachmentCharacter");
			}
		}
#endif // !__WATCHOS__

		[Test]
		public void InitWith ()
		{
			using (var s1 = new NSAttributedString ("string")) {
				// initWithString: does not respond (see dontlink.app) but it works
				Assert.That (s1.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle-1");

				using (var d = new NSDictionary ())
				using (var s2 = new NSAttributedString ("string", d)) {
					// initWithString:attributes: does not respond (see dontlink.app) but it works
					Assert.That (s2.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle-2");
				}

				using (var s3 = new NSAttributedString (s1)) {
					// initWithAttributedString: does not respond (see dontlink.app) but it works
					Assert.That (s3.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle-3");
				}
			}
		}

		[Test]
		public void MutableCopy ()
		{
			using (var s1 = new NSAttributedString ("string")) {
				using (var copy = s1.MutableCopy ())
					Assert.That (copy.RetainCount, Is.EqualTo ((nint) 1), "Copy retaincount 1");
				using (var copy = ((INSMutableCopying) s1).MutableCopy (NSZone.Default))
					Assert.That (copy.RetainCount, Is.EqualTo ((nint) 1), "Copy retaincount 2");
			}
		}

		[Test]
		public void NullDictionary ()
		{
			using (var s = new NSAttributedString ("string", (NSDictionary) null)) {
				Assert.That (s.Handle, Is.Not.EqualTo (IntPtr.Zero));
			}
		}

#if !__WATCHOS__
		[Test]
		public void IndirectNullDictionary ()
		{
			// that will call NSAttributedString.ToDictionary which may return null (if empty)
			using (var s = new NSAttributedString ("string", foregroundColor: null)) {
				Assert.That (s.Handle, Is.Not.EqualTo (IntPtr.Zero));
			}
		}
#endif // !__WATCHOS__
	}
}
