//
// Unit tests for CTFontDescriptor
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

using System;
using System.Threading;
using System.Threading.Tasks;

using Foundation;
using CoreText;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
using ObjCRuntime;
using NUnit.Framework;
using System.Linq;

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
				Assert.That (font.Size, Is.EqualTo ((nfloat) 16), "Size");
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
			using (var font = new CTFont (rare_on_fd, 13)) {
				var set_feature = font.GetFeatureSettings () [0];

				Assert.That (set_feature.FeatureGroup, Is.EqualTo (FontFeatureGroup.Ligatures), "#1");
				Assert.That (set_feature.FeatureWeak, Is.EqualTo ((int) CTFontFeatureLigatures.Selector.RareLigaturesOn), "#2");
			}
		}

		[Test]
		public void MatchFontDescriptors ()
		{
			var fda1 = new CTFontDescriptorAttributes () {
				Name = "Helvetica",
			};
			using var desc1 = new CTFontDescriptor (fda1);
			var tcs = new TaskCompletionSource<bool> ();
			var rv = CTFontDescriptor.MatchFontDescriptors (new CTFontDescriptor [] { desc1 }, null, (CTFontDescriptorMatchingState state, CTFontDescriptorMatchingProgress progress) => {
				try {
					if (state == CTFontDescriptorMatchingState.Finished) {
						Assert.AreEqual (1, progress.Result.Length, "Result.Length");
						Assert.AreEqual (fda1.Name, progress.Result [0].GetAttributes ().Name, "Result[0].Name");
						tcs.TrySetResult (true);
					}
				} catch (Exception e) {
					tcs.TrySetException (e);
				}
				return true;
			});
			Assert.IsTrue (rv, "Return value");
			TestRuntime.RunAsync (TimeSpan.FromSeconds (30), tcs.Task);
		}
	}
}
