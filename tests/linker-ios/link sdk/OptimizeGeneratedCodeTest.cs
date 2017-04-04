//
// Unit tests for the linker's OptimizeGeneratedCodeSubStep
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2012-2013, 2016 Xamarin Inc. All rights reserved.
//

using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#if !__WATCHOS__
using System.Drawing;
#endif
#if XAMCORE_2_0
using Foundation;
using ObjCRuntime;
using UIKit;
#else
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

#if XAMCORE_2_0
using SizeF=CoreGraphics.CGSize;
using RectangleF=CoreGraphics.CGRect;
#endif

namespace Linker.Shared {

	partial class NotPreserved {

#if !__WATCHOS__
		public void Bug11452 ()
		{
			var button = new UIButton ();
			button.TouchCancel += delegate {
				if (Runtime.Arch == Arch.SIMULATOR) {
					// kaboom
				}
			};
		}
#endif // !__WATCHOS__
	}

	class NSNotPreserved : NSObject {
#if !__WATCHOS__
		public void Bug11452 ()
		{
			var button = new UIButton ();
			button.TouchCancel += delegate {
				if (Runtime.Arch == Arch.SIMULATOR) {
					// kaboom
				}
			};
		}
#endif // !__WATCHOS__
	}

	[TestFixture]
	// we want the test to be availble if we use the linker
	[Preserve (AllMembers = true)]
	public class OptimizeGeneratedCodeTest {
		
		// tests related to IL re-writting inside OptimizeGeneratedCodeSubStep
		
		// note: the following tests don't really ensure the IL code is ok -
		// the best way to be sure if decompiling and reviewing the IL. OTOH
		// it's pretty likely to crash if the IL was badly rewritten so running
		// them makes me feel better ;-)
		
#if !__TVOS__ && !__WATCHOS__
		[Test]
		public void IsNewRefcountEnabled ()
		{
			using (UIWebView wv = new UIWebView ()) {
				Assert.Null (wv.Request, "IsNewRefcountEnabled");
			}
		}

		class MyUIWebViewDelegate : UIWebViewDelegate {
		}
		
		[Test]
		public void MarkDirty ()
		{
			using (UIWebView wv = new UIWebView ())
			using (MyUIWebViewDelegate del = new MyUIWebViewDelegate ()) {
				wv.WeakDelegate = del;
				Assert.That (wv.WeakDelegate, Is.EqualTo (del), "MarkDirty");
			}
		}

		// this has a (single) "if (Runtime.Arch == Arch.DEVICE)" condition

		[Test]
		public void SingleRuntimeArchDevice ()
		{
			SizeF empty = SizeF.Empty;
			using (UIView v = new UIView ())
			using (UIFont font = UIFont.SystemFontOfSize (12f)) {
				SizeF size = "MonoTouch".StringSize (font);
				Assert.False (size.IsEmpty, "!Empty");
			}
		}
#endif // !__TVOS__
		
		// this has 2 "if (Runtime.Arch == Arch.DEVICE)" conditions separated
		// by "if (IsDirectBinding)" so modifying IL is a bit more tricky - so
		// testing this, linked on both the simulator and on device is important

#if !__WATCHOS__
		[Test]
		public void DoubleRuntimeArchDevice ()
		{
			SizeF empty = SizeF.Empty;
			using (UIView v = new UIView ()) {
				Assert.True (v.SizeThatFits (empty).IsEmpty, "Empty");
			}
		}
#endif // !__WATCHOS__

		// some UIImage bindings are now decorated with [Autorelease] and that 
		// MUST be considered since it adds a try/finally for the C# using

		[Test]
		public void Autorelease ()
		{
			using (UIImage img = new UIImage ()) {
				// those are the two UIImage instance methods decorated with [Autorelease]
#if !__TVOS__
				img.StretchableImage (10, 10);
#endif
				img.CreateResizableImage (new UIEdgeInsets (1, 2, 3, 4));
				// note: return value is null for iOS7 (and was non-null before it)
				// anyway we care about not crashing due to the linker optimizing the IL, not the return values
			}
		}

#if !__WATCHOS__
		[Test]
		public void AnonymousDelegate ()
		{
			// anonymous delegates are decorated with [CompilerGenerated] attributes but must 
			// not be processed since the IL inside them is not compiler generated
			new NotPreserved ().Bug11452 ();

			using (var ns = new NSNotPreserved ()) {
				ns.Bug11452 ();
			}
		}
#endif // !__WATCHOS__

#if LINKALL
		[Test]
		public void FinallyTest ()
		{
			// bug #26415
			FinallyTestMethod ();
			Assert.IsTrue (finally_invoked);
		}

		bool finally_invoked;
		[Export ("finallyTestMethod")]
		[CompilerGenerated]
		public IntPtr FinallyTestMethod ()
		{
			try {
				if (IntPtr.Size == 8) {
					return IntPtr.Zero;
				} else {
					return IntPtr.Zero;
				}
			} finally {
				finally_invoked = true;
			}
		}
#endif
	}
}