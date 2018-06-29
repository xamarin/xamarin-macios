//
// Unit tests for the registrars.
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

using System;
using System.Reflection;
using System.Runtime.InteropServices;
#if XAMCORE_2_0
using Foundation;
using ObjCRuntime;
#elif __IOS__
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
#else
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
#endif
using NUnit.Framework;

namespace XamarinTests.ObjCRuntime {
	[TestFixture]
	public class RegistrarSharedTest {
		public static Registrars CurrentRegistrar {
			get {
				return Registrar.CurrentRegistrar;
			}
		}

#if __UNIFIED__
		[Test]
		public void IntPtrCtor ()
		{
			IntPtr ptr = IntPtr.Zero;
			try {
				ptr = Messaging.IntPtr_objc_msgSend (Class.GetHandle (typeof (IntPtrCtorTestClass)), Selector.GetHandle ("alloc"));
				ptr = Messaging.IntPtr_objc_msgSend (ptr, Selector.GetHandle ("init"));
				var ex = Assert.Throws<RuntimeException> (() => Messaging.bool_objc_msgSend_IntPtr (ptr, Selector.GetHandle ("conformsToProtocol:"), IntPtr.Zero));
				var msg = string.Format ("Failed to marshal the Objective-C object 0x{0} (type: IntPtrCtorTestClass). Could not find an existing managed instance for this object, nor was it possible to create a new managed instance (because the type 'XamarinTests.ObjCRuntime.RegistrarSharedTest+IntPtrCtorTestClass' does not have a constructor that takes one IntPtr argument).", ptr.ToString ("x"));
				msg += "\nAdditional information:\n\tSelector: conformsToProtocol:\n\tMethod: ";
				// The difference between the registrars is basically whether this string
				// was constructed by native mono API or managed API.
				if (CurrentRegistrar == Registrars.Static) {
					msg += "Foundation.NSObject:InvokeConformsToProtocol (intptr)\n";
				} else {
					msg += "Foundation.NSObject.InvokeConformsToProtocol(IntPtr)\n";
				}
				Assert.AreEqual (msg, ex.Message, "#message");
			} finally {
				Messaging.void_objc_msgSend (ptr, Selector.GetHandle ("release"));
			}
		}
#endif

		[Register ("IntPtrCtorTestClass")]
		class IntPtrCtorTestClass : NSObject {
			[Export ("initWithFoo:")]
			public IntPtrCtorTestClass (int foo)
			{
				Console.WriteLine ("foo1");
			}
		}
	}
}
