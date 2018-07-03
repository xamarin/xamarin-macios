//
// Unit tests for NSString
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2011-2012 Xamarin Inc. All rights reserved.
//

using System;
#if !__WATCHOS__
using System.Drawing;
#endif
#if XAMCORE_2_0
using Foundation;
#if MONOMAC
using AppKit;
using UIFont = AppKit.NSFont;
using UIStringAttributes = AppKit.NSStringAttributes;
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
	public class StringTest {
		
		[Test]
		[ExpectedException (typeof (ArgumentNullException))]
		public void Compare_Null ()
		{
			using (NSString s = new NSString ("s")) {
				s.Compare (null);
			}
		}
		
		[Test]
		public void Compare ()
		{
			using (NSString s1 = new NSString ("Sebastien")) 
			using (NSString s2 = new NSString ("Sébastien")) 
			{
				Assert.That (s1.Compare (s1), Is.EqualTo (NSComparisonResult.Same), "Same");
				Assert.That (s1.Compare (s2), Is.EqualTo (NSComparisonResult.Ascending), "Ascending");
				Assert.That (s2.Compare (s1), Is.EqualTo (NSComparisonResult.Descending), "Descending");
			}
		}
		
		[Test]
		public void Compare_Options ()
		{
			using (NSString s1 = new NSString ("Sebastien")) 
			using (NSString s2 = new NSString ("Sébastien")) 
			{
				Assert.That (s1.Compare (s2, NSStringCompareOptions.DiacriticInsensitiveSearch), 
					Is.EqualTo (NSComparisonResult.Same), "DiacriticInsensitiveSearch");
			}
		}

		[Test]
		public void Compare_Range ()
		{
			using (NSString s1 = new NSString ("Bastien")) 
			using (NSString s2 = new NSString ("Sébastien")) 
			{
				NSRange r = new NSRange (2, s2.Length - 2);
				Assert.That (s2.Compare (s1, NSStringCompareOptions.CaseInsensitiveSearch, r), 
					Is.EqualTo (NSComparisonResult.Same), "skip accent");
			}
		}

		[Test]
		public void Compare_Locale ()
		{
			using (NSString s1 = new NSString ("sebastien")) 
			using (NSString s2 = new NSString ("Sébastien")) 
			{
				NSStringCompareOptions options = NSStringCompareOptions.DiacriticInsensitiveSearch | NSStringCompareOptions.CaseInsensitiveSearch;
				NSRange r = new NSRange (0, s2.Length);
				Assert.That (s1.Compare (s2, options, r, null), 
					Is.EqualTo (NSComparisonResult.Same), "null");
				Assert.That (s1.Compare (s2, options, r, NSLocale.SystemLocale), 
					Is.EqualTo (NSComparisonResult.Same), "SystemLocale");
			}
		}
		
		[Test]
		// requested in http://bugzilla.xamarin.com/show_bug.cgi?id=1870
		public void Replace_Range ()
		{
			using (NSString s1 = new NSString ("Sebastien")) 
			using (NSString s2 = new NSString ("é")) 
			using (NSString s3 = new NSString ("sébastien"))
			using (NSString result = s1.Replace (new NSRange (1, 1), s2))
			{
				NSStringCompareOptions options = NSStringCompareOptions.CaseInsensitiveSearch;
				Assert.That (result.Compare (s3, options), 
					Is.EqualTo (NSComparisonResult.Same), "Replace");
			}
		}

		//No Mac version of DrawString with those parameters
#if !__TVOS__ && !__WATCHOS__ && !MONOMAC
		[Test]
		[Culture ("en")] // fails for some cultures, e.g. ar-AE
		public void DrawString_7 ()
		{
			nfloat actualFontSize = 12;
			var f = UIFont.BoldSystemFontOfSize (actualFontSize);
			try {
				using (NSString s = new NSString ("s")) {
					SizeF size = s.DrawString (PointF.Empty, 20, f, 6, ref actualFontSize, UILineBreakMode.MiddleTruncation, UIBaselineAdjustment.None);
					Assert.That (actualFontSize, Is.EqualTo ((nfloat) 12), "actualFontSize");
					Assert.That (size.Width, Is.InRange ((nfloat) 6f, (nfloat) 7f), "Width");
					Assert.That (size.Height, Is.InRange ((nfloat) 14f, (nfloat) 15f), "Height");
				}
				using (NSString s = new NSString ("saterlipopette")) {
					SizeF size = s.DrawString (PointF.Empty, 20, f, 6, ref actualFontSize, UILineBreakMode.MiddleTruncation, UIBaselineAdjustment.None);
					Assert.That (actualFontSize, Is.EqualTo ((nfloat) 6), "actualFontSize-2");
					Assert.That (size.Width, Is.InRange ((nfloat) 17f, (nfloat) 19f), "Width-2");
					Assert.That (size.Height, Is.InRange ((nfloat) 7f, (nfloat) 8f), "Height-2");
				}
			} catch {
				Console.WriteLine ("DrawString_7: actualFontSize: {0} font: {1}", actualFontSize, f);
				throw;
			}
		}

		//No Mac ersion of StringSize with those parmaters
		[Test]
		[Culture ("en")] // fails for some cultures, e.g. ar-AE
		public void StringSize_5 ()
		{
			nfloat actualFontSize = 12;
			var f = UIFont.BoldSystemFontOfSize (actualFontSize);
			try {
				using (NSString s = new NSString ("s")) {
					SizeF size = s.StringSize (f, 6, ref actualFontSize, 10, UILineBreakMode.MiddleTruncation);
					Assert.That (actualFontSize, Is.EqualTo ((nfloat) 12), "actualFontSize");
					Assert.That (size.Width, Is.InRange ((nfloat) 6f, (nfloat) 7f), "Width");
					Assert.That (size.Height, Is.InRange ((nfloat) 14f, (nfloat) 15f), "Height");
				}
				using (NSString s = new NSString ("saterlipopette")) {
					SizeF size = s.StringSize (f, 6, ref actualFontSize, 10, UILineBreakMode.MiddleTruncation);
					Assert.That (actualFontSize, Is.EqualTo ((nfloat) 6), "actualFontSize-2");
					Assert.That (size.Width, Is.InRange ((nfloat) 5f, (nfloat) 10f), "Width-2");
					Assert.That (size.Height, Is.InRange ((nfloat) 14f, (nfloat) 15f), "Height-2");
				}
			} catch {
				Console.WriteLine ("StringSize_5: actualFontSize: {0} font: {1}", actualFontSize, f);
				throw;
			}
		}
#endif // !__TVOS__ && !__WATCHOS__ && !MONOMAC

		[Test]
		public void PathExtensions ()
		{
			using (NSString s = new NSString ("/dir/file.ext")) {
				Assert.That (s.PathExtension.ToString (), Is.EqualTo ("ext"), "PathExtension");
				var components = s.PathComponents;
				Assert.That (components.Length, Is.EqualTo (3), "PathComponents");
				Assert.That (components [0], Is.EqualTo ("/"), "PathComponents-0");
				Assert.That (components [1], Is.EqualTo ("dir"), "PathComponents-1");
				Assert.That (components [2], Is.EqualTo ("file.ext"), "PathComponents-2");
				Assert.That (s.LastPathComponent.ToString (), Is.EqualTo (components [2]), "LastPathComponent");
			}
		}

		[Test]
		public void DrawingExtensions ()
		{
			TestRuntime.AssertXcodeVersion (5, 0);
			TestRuntime.AssertMacSystemVersion (10, 11, throwIfOtherPlatform: false);

			using (var s = new NSString ("foo")) {
				NSStringDrawingOptions options = NSStringDrawingOptions.OneShot;
				var attrib = new UIStringAttributes ();
				using (var dict = new NSDictionary ()) {
					Assert.DoesNotThrow (() => s.GetBoundingRect (new SizeF (5, 5), options, attrib, null), "GetBoundingRect 1");
					Assert.DoesNotThrow (() => s.WeakGetBoundingRect (new SizeF (5, 5), options, dict, null), "WeakGetBoundingRect 1");
					Assert.DoesNotThrow (() => s.DrawString (new RectangleF (0, 0, 10, 10), options, attrib, null), "DrawString 1");
					Assert.DoesNotThrow (() => s.WeakDrawString (new RectangleF (0, 0, 10, 10), options, dict, null), "WeakDrawString 1");
#if !MONOMAC //WeakDrawString on mac doesn't have versions with these parameters
					Assert.DoesNotThrow (() => s.WeakDrawString (new RectangleF (0, 0, 10, 10), dict), "WeakDrawString 2");
					Assert.DoesNotThrow (() => s.WeakDrawString (new PointF (0, 0), dict), "WeakDrawString 3");
#endif
				}
			}
		}

		[Test]
		public void ReleaseEmptyString ()
		{
			NSString.Empty.DangerousRelease ();
			NSString.Empty.DangerousRelease ();
			NSString.Empty.DangerousRelease ();
			NSString.Empty.DangerousRelease ();
			NSString.Empty.DangerousRelease ();

#if XAMCORE_2_0
			Assert.That (NSString.Empty.RetainCount, Is.EqualTo (nuint.MaxValue), "RetainCount");
#else
			Assert.That (NSString.Empty.RetainCount, Is.EqualTo (-1), "RetainCount");
#endif
			Assert.That (NSString.Empty.Compare (new NSString (string.Empty)), Is.EqualTo (NSComparisonResult.Same), "Same");
		}

		[Test]
		public void Equality ()
		{
			using (var s1 = new NSString ("\u00f6"))	// o-umlaut
			using (var s2 = new NSString ("o\u0308")) {	// o + combining diaeresis
				// since ObjC thinks it's different
				Assert.That (s1.GetHashCode (), Is.Not.EqualTo (s2.GetHashCode ()), "GetHashCode");
				// then it's "correct" to return false for equality
				Assert.False (s1.Equals ((object) s2), "Equal(object)");
				Assert.False (s1.Equals ((NSObject) s2), "Equal(NSObject)");
				Assert.False (s1.Equals ((NSString) s2), "Equal(NSString)");
				Assert.False (NSString.Equals (s1, s2), "static");
				// and people need to call compare
				Assert.That (s1.Compare (s2), Is.EqualTo (NSComparisonResult.Same), "Same");
			}
		}

#if !MONOMAC // NSImage uses different methods
		[Test]
		public void FromData ()
		{
			UIImage img = UIImage.FromFile ("basn3p08.png");
			using (NSData imageData = img.AsPNG ()) {
				using (var str = NSString.FromData (imageData, NSStringEncoding.UTF8)) {
					Assert.IsNull (str, "NSDataFromImage");
				}
			}
			Assert.Throws<ArgumentNullException> (() => {
				NSString.FromData (null, NSStringEncoding.UTF8);
			}, "NSDataNull");
		}
#endif
	}
}
