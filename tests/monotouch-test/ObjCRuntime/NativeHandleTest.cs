using System;

using Foundation;
using ObjCRuntime;

using NUnit.Framework;

#if NET

namespace MonoTouchFixtures.ObjCRuntime {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NativeHandleTest {
		[Test]
		public unsafe void Operators ()
		{
			IntPtr value = new IntPtr (0xdadf00d);

			Assert.AreEqual (value, ((NativeHandle) value).Handle, "IntPtr -> NativeHandle");
			Assert.AreEqual (value, (IntPtr) new NativeHandle (value), "NativeHandle -> IntPtr");
			Assert.AreEqual (value, ((NativeHandle) ((void*) value)).Handle, "void* -> NativeHandle");
			Assert.AreEqual (value, (IntPtr) (void*) new NativeHandle (value), "NativeHandle -> void*");
		}
	}
}
#endif
