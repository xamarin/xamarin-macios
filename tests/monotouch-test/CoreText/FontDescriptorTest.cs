//
// Unit tests for CTFontDescriptor
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

using System;
#if XAMCORE_2_0
using Foundation;
using CoreText;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
#else
using MonoTouch.CoreText;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;
using System.Linq;

#if XAMCORE_2_0
using RectangleF=CoreGraphics.CGRect;
using SizeF=CoreGraphics.CGSize;
using PointF=CoreGraphics.CGPoint;
#else
using nfloat=global::System.Single;
using nint=global::System.Int32;
using nuint=global::System.UInt32;
#endif

namespace MonoTouchFixtures.CoreText {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class FontDescriptorTest {
		
		[Test]
		// http://stackoverflow.com/questions/9007991/monotouch-custom-font-with-attributes/9009161#9009161
		public void FromAttributes ()
		{
			CTFontDescriptorAttributes fda = new CTFontDescriptorAttributes () {
				FamilyName = "Courier",
				StyleName = "Bold",
				Size = 16.0f
			};
			using (CTFontDescriptor fd = new CTFontDescriptor (fda))
			using (CTFont font = new CTFont (fd, 0)) {
				// check that the created font match the descriptor's attributes
				Assert.That (font.FamilyName, Is.EqualTo ("Courier"), "FamilyName");
				Assert.That (font.FullName, Is.EqualTo ("Courier Bold"), "FullName");
				Assert.That (font.Size, Is.EqualTo ((nfloat)16), "Size");
				// that changed in iOS 8.3, there's an undocumented flag + MonoSpace (make sense) + bold
				Assert.True ((font.SymbolicTraits & CTFontSymbolicTraits.Bold) != 0, "SymbolicTraits");
			}
		}

#if __TVOS__ || __WATCHOS__
		[Ignore ("No font with ligatures are available on the platform")] // more details in https://bugzilla.xamarin.com/show_bug.cgi?id=58929
#endif
		[Test]
		public void WithFeature ()
		{
			var fontName = "HoeflerText-Regular";

			using (var font = new CTFont (fontName, 10)) {
				var f1 = font.GetFeatures ();
				var ligatures = f1.Where (l => l.FeatureGroup == FontFeatureGroup.Ligatures).First ();
				bool rare = ligatures.Selectors.Cast<CTFontFeatureLigatures> ().Any (l => l.Feature == CTFontFeatureLigatures.Selector.RareLigaturesOn);
				Assert.That (rare, Is.True, "RareLigaturesOn available");
				Assert.That (font.GetFeatureSettings (), Is.Empty, "No custom settings");

			}

			using (var fd = new CTFontDescriptor (fontName, 20))
			using (var rare_on_fd = fd.WithFeature (CTFontFeatureLigatures.Selector.RareLigaturesOn))
			using (var	font = new CTFont (rare_on_fd, 13)) {
				var set_feature = font.GetFeatureSettings ()[0];

				Assert.That (set_feature.FeatureGroup, Is.EqualTo (FontFeatureGroup.Ligatures), "#1");
				Assert.That (set_feature.FeatureWeak, Is.EqualTo ((int)CTFontFeatureLigatures.Selector.RareLigaturesOn), "#2");
			}
		}
	}
}
