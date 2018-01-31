//
// Unit tests for the linker's OptimizeGeneratedCodeSubStep
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2012-2013, 2016 Xamarin Inc. All rights reserved.
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
	public class OptimizeGeneratedCodeTest : BaseOptimizeGeneratedCodeTest {
		
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

		[Test]
		public void IntPtrSizeTest ()
		{
			var S8methods = new MethodInfo []
			{
				GetType ().GetMethod (nameof (Size8Test), BindingFlags.NonPublic | BindingFlags.Instance),
				GetType ().GetMethod (nameof (Size8Test_Optimizable), BindingFlags.NonPublic | BindingFlags.Instance)
			};
			var S4methods = new MethodInfo []
			{
				GetType ().GetMethod (nameof (Size4Test), BindingFlags.NonPublic | BindingFlags.Instance),
				GetType ().GetMethod (nameof (Size4Test_Optimizable), BindingFlags.NonPublic | BindingFlags.Instance)
			};
			MethodInfo[] passingMethods = null;
			MethodInfo[] failingMethods = null;
			switch (IntPtr.Size) {
			case 4:
				Size4Test ();
				Size4Test_Optimizable ();
				passingMethods = S4methods;
				failingMethods = S8methods;
				break;
			case 8:
				Size8Test ();
				Size8Test_Optimizable ();
				passingMethods = S8methods;
				failingMethods = S4methods;
				break;
			default:
				Assert.Fail ("Invalid size: {0}", IntPtr.Size);
				break;
			}

#if !DEBUG
			// Verify that the passing method is completely empty (save for nop instructions and a final ret instruction).
			// Unfortunately in debug mode csc produces IL sequences the optimizer doesn't understand (a lot of unnecessary instructions),
			// which means we can only check this in release mode. Also on device this will probably always pass,
			// since we strip assemblies (and the methods will always be empty), but running the test shouldn't hurt.
			foreach (var passingMethod in passingMethods) {
				IEnumerable<ILInstruction> passingInstructions = new ILReader (passingMethod);
				passingInstructions = passingInstructions.Where ((v) => v.OpCode.Name != "nop");
				Assert.AreEqual (1, passingInstructions.Count (), "empty body");
			}
			foreach (var failingMethod in failingMethods) {
				IEnumerable<ILInstruction> failingInstructions = new ILReader (failingMethod);
				failingInstructions = failingInstructions.Where ((v) => v.OpCode.Name != "nop");
				Assert.That (failingInstructions.Count (), Is.GreaterThan (1), "non-empty body");
			}
#endif
		}

		[CompilerGenerated] // Trigger optimizations
		[Foundation.Export ("alsoRequiredForOptimizations")]
		void Size8Test ()
		{
			// Everything in this method should be optimized away (when building for 64-bits)
			if (IntPtr.Size != 8)
				throw new NUnit.Framework.Internal.NUnitException ("1");
			if (IntPtr.Size == 4)
				throw new NUnit.Framework.Internal.NUnitException ("2");
			if (IntPtr.Size > 8)
				throw new NUnit.Framework.Internal.NUnitException ("3");
			if (IntPtr.Size < 8)
				throw new NUnit.Framework.Internal.NUnitException ("4");
			if (IntPtr.Size >= 9)
				throw new NUnit.Framework.Internal.NUnitException ("5");
			if (IntPtr.Size <= 7)
				throw new NUnit.Framework.Internal.NUnitException ("6");
		}

		[CompilerGenerated] // Trigger optimizations
		[Foundation.Export ("alsoRequiredForOptimizations")]
		void Size4Test ()
		{
			// Everything in this method should be optimized away (when building for 32-bits)
			if (IntPtr.Size != 4)
				throw new NUnit.Framework.Internal.NUnitException ("1");
			if (IntPtr.Size == 8)
				throw new NUnit.Framework.Internal.NUnitException ("2");
			if (IntPtr.Size > 4)
				throw new NUnit.Framework.Internal.NUnitException ("3");
			if (IntPtr.Size < 4)
				throw new NUnit.Framework.Internal.NUnitException ("4");
			if (IntPtr.Size >= 5)
				throw new NUnit.Framework.Internal.NUnitException ("5");
			if (IntPtr.Size <= 3)
				throw new NUnit.Framework.Internal.NUnitException ("6");
		}

		[BindingImplAttribute (BindingImplOptions.Optimizable)]
		void Size8Test_Optimizable ()
		{
			// Everything in this method should be optimized away (when building for 64-bits)
			if (IntPtr.Size != 8)
				throw new NUnit.Framework.Internal.NUnitException ("1");
			if (IntPtr.Size == 4)
				throw new NUnit.Framework.Internal.NUnitException ("2");
			if (IntPtr.Size > 8)
				throw new NUnit.Framework.Internal.NUnitException ("3");
			if (IntPtr.Size < 8)
				throw new NUnit.Framework.Internal.NUnitException ("4");
			if (IntPtr.Size >= 9)
				throw new NUnit.Framework.Internal.NUnitException ("5");
			if (IntPtr.Size <= 7)
				throw new NUnit.Framework.Internal.NUnitException ("6");
		}

		[BindingImplAttribute (BindingImplOptions.Optimizable)]
		void Size4Test_Optimizable ()
		{
			// Everything in this method should be optimized away (when building for 32-bits)
			if (IntPtr.Size != 4)
				throw new NUnit.Framework.Internal.NUnitException ("1");
			if (IntPtr.Size == 8)
				throw new NUnit.Framework.Internal.NUnitException ("2");
			if (IntPtr.Size > 4)
				throw new NUnit.Framework.Internal.NUnitException ("3");
			if (IntPtr.Size < 4)
				throw new NUnit.Framework.Internal.NUnitException ("4");
			if (IntPtr.Size >= 5)
				throw new NUnit.Framework.Internal.NUnitException ("5");
			if (IntPtr.Size <= 3)
				throw new NUnit.Framework.Internal.NUnitException ("6");
		}
#endif
	}
}