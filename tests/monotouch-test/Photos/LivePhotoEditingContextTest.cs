#if !__TVOS__ && !__WATCHOS__

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using Photos;
using NUnit.Framework;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace MonoTouchFixtures.Photos {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public unsafe class PHLivePhotoEditingContextTest {

		[SetUp]
		public void Setup ()
		{
			if (!TestRuntime.CheckXcodeVersion (8, 0))
				Assert.Inconclusive ("Requires Xcode 8+ or later");
		}

		static NSError error_faker;

#if NET
		static PHLivePhotoFrameProcessingBlock managed = (IPHLivePhotoFrame frame, ref NSError error) => {
#else
		static PHLivePhotoFrameProcessingBlock2 managed = (IPHLivePhotoFrame frame, ref NSError error) => {
#endif
			error = error_faker;
			return null;
		};

		delegate NativeHandle DPHLivePhotoFrameProcessingBlock2 (IntPtr block, NativeHandle frame, NativeHandle* error);

#if !MONOMAC
		// on macOS `initWithLivePhotoEditingInput:` returns `nil` and we throw
		[Test]
#endif
		public void Linker ()
		{
			using (var cei = new PHContentEditingInput ())
			using (var lpec = new PHLivePhotoEditingContext (cei)) {
				// not much but it means the linker cannot remove it
#if NET
				Assert.Null (lpec.FrameProcessor, "FrameProcessor");
#else
				Assert.Null (lpec.FrameProcessor2, "FrameProcessor2");
#endif
			}
		}

		[Test]
		public unsafe void FrameProcessingBlock2 ()
		{
			if (!Runtime.DynamicRegistrationSupported)
				Assert.Ignore ("This test requires support for the dynamic registrar to setup the block");

#if NET
			var t = typeof (NSObject).Assembly.GetType ("ObjCRuntime.Trampolines+SDPHLivePhotoFrameProcessingBlock");
#else
			var t = typeof (NSObject).Assembly.GetType ("ObjCRuntime.Trampolines+SDPHLivePhotoFrameProcessingBlock2");
#endif
			Assert.NotNull (t, "SDPHLivePhotoFrameProcessingBlock2");

			var m = t.GetMethod ("Invoke", BindingFlags.Static | BindingFlags.NonPublic);
			Assert.NotNull (m, "Invoke");
			var d = m.CreateDelegate (typeof (DPHLivePhotoFrameProcessingBlock2));
#if NET
			var fptr = m.MethodHandle.GetFunctionPointer ();
			var del = new DPHLivePhotoFrameProcessingBlock2 ((IntPtr a, NativeHandle b, NativeHandle* c) => (NativeHandle) global::Bindings.Test.CFunctions.x_call_func_3 (fptr, (IntPtr) a, (IntPtr) b, (IntPtr) (void*) c));
#else
			var del = (DPHLivePhotoFrameProcessingBlock2) d;
#endif

#if NET
			using var bl = new BlockLiteral ((void*) fptr, managed, t, "Invoke");
#else
			using var bl = new BlockLiteral ();
			bl.SetupBlock (d, managed);
#endif
			var block = &bl;
			var b = (IntPtr) block;

			// simulate a call that does not produce an error
			error_faker = null;
			Assert.That (del (b, NativeHandle.Zero, null), Is.EqualTo (NativeHandle.Zero), "1");

			// simulate a call that does produce an error
			error_faker = new NSError ((NSString) "domain", 42);
			NativeHandle ptr = NativeHandle.Zero;
			Assert.That (del (b, NativeHandle.Zero, &ptr), Is.EqualTo (NativeHandle.Zero), "2");
			Assert.That ((IntPtr) ptr, Is.EqualTo ((IntPtr) error_faker.Handle), "error 2");
		}
	}
}

#endif
