//
// Unit tests for Blocks
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

#if XAMCORE_2_0
using Foundation;
using ObjCRuntime;
#else
using MonoTouch;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.ObjCRuntime {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class BlocksTest {
		[DllImport ("/usr/lib/libobjc.dylib")]
		extern static IntPtr objc_getClass (string name);

		[DllImport("/usr/lib/libobjc.dylib")]
		static extern IntPtr imp_implementationWithBlock(ref BlockLiteral block);

		[DllImport ("/usr/lib/libobjc.dylib")]
		static extern bool class_addMethod (IntPtr cls, IntPtr name, IntPtr imp, string types);

		[Test]
		public void TestSetupBlock ()
		{
			using (var obj = new TestClass ()) {
				TestClass.OnCallback = ((IntPtr blockArgument, IntPtr self, IntPtr argument) => 
					{
						Assert.AreNotEqual (IntPtr.Zero, blockArgument, "block");
						Assert.AreEqual (obj.Handle, self, "self");
						Assert.AreEqual (argument, (IntPtr) 0x12345678, "argument");
					});
				Messaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("testBlocks:"), (IntPtr) 0x12345678);
			}
		}

		class TestClass : NSObject {
			[MonoPInvokeCallback (typeof (TestBlockCallbackDelegate))]
			static void TestBlockCallback (IntPtr block, IntPtr self, IntPtr argument)
			{
				OnCallback (block, self, argument);
			}

			static TestBlockCallbackDelegate callback = new TestBlockCallbackDelegate (TestBlockCallback);

			public delegate void TestBlockCallbackDelegate (IntPtr block, IntPtr self, IntPtr argument);
			public static TestBlockCallbackDelegate OnCallback;

			static TestClass ()
			{
				var cls = Class.GetHandle (typeof (TestClass));
				var block = new BlockLiteral ();
				block.SetupBlock (callback, null);
				var imp = imp_implementationWithBlock (ref block);
				class_addMethod (cls, Selector.GetHandle ("testBlocks:"), imp, "v@:^v");
			}
		}
	}
}
