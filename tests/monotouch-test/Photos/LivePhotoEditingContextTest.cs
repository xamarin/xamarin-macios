#if !__TVOS__ && !__WATCHOS__

using System;
using System.Reflection;
#if XAMCORE_2_0
using Foundation;
using ObjCRuntime;
using Photos;
using CoreGraphics;
using RectangleF = CoreGraphics.CGRect;
using SizeF = CoreGraphics.CGSize;
using PointF = CoreGraphics.CGPoint;
#else
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
using MonoTouch.Photos;
using System.Drawing;
using MonoTouch.AssetsLibrary;
using nfloat=global::System.Single;
using nint=global::System.Int32;
using nuint=global::System.UInt32;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.Photos {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class PHLivePhotoEditingContextTest {

		[SetUp]
		public void Setup ()
		{
			if (!TestRuntime.CheckXcodeVersion (8, 0))
				Assert.Inconclusive ("Requires Xcode 8+ or later");
		}

		static NSError error_faker;

		static PHLivePhotoFrameProcessingBlock2 managed = (IPHLivePhotoFrame frame, ref NSError error) => {
			error = error_faker;
			return null;
		};

		delegate IntPtr DPHLivePhotoFrameProcessingBlock2 (IntPtr block, IntPtr frame, ref IntPtr error);

#if !MONOMAC
		// on macOS `initWithLivePhotoEditingInput:` returns `nil` and we throw
		[Test]
#endif
		public void Linker ()
		{
			using (var cei = new PHContentEditingInput ())
			using (var lpec = new PHLivePhotoEditingContext (cei)) {
				// not much but it means the linker cannot remove it
				Assert.Null (lpec.FrameProcessor2, "FrameProcessor2");
			}
		}

		[Test]
		public unsafe void FrameProcessingBlock2 ()
		{
			if (!Runtime.DynamicRegistrationSupported)
				Assert.Ignore ("This test requires support for the dynamic registrar to setup the block");

			var t = typeof (NSObject).Assembly.GetType ("ObjCRuntime.Trampolines/SDPHLivePhotoFrameProcessingBlock2");
			Assert.NotNull (t, "SDPHLivePhotoFrameProcessingBlock2");

			var m = t.GetMethod ("Invoke", BindingFlags.Static | BindingFlags.NonPublic);
			Assert.NotNull (m, "Invoke");
			var d = m.CreateDelegate (typeof (DPHLivePhotoFrameProcessingBlock2));

			Action userDelegate = new Action (() => Console.WriteLine ("Hello world!"));

			BlockLiteral bl = new BlockLiteral ();
			bl.SetupBlock (d, managed);
			try {
				var block = &bl;
				var b = (IntPtr) block;

				// simulate a call that does not produce an error
				var args = new object [] { b, IntPtr.Zero, IntPtr.Zero };
				error_faker = null;
				Assert.That (m.Invoke (null, args), Is.EqualTo (IntPtr.Zero), "1");

				// simulate a call that does produce an error
				error_faker = new NSError ((NSString) "domain", 42);
				Assert.That (m.Invoke (null, args), Is.EqualTo (IntPtr.Zero), "2");
				Assert.That (args [2], Is.EqualTo (error_faker.Handle), "error");
			}
			finally {
				bl.CleanupBlock ();
			}
 		}
	}
}

#endif
