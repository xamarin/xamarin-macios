//
// Unit tests for the registrars.
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
using Foundation;
using ObjCRuntime;
using NUnit.Framework;

namespace XamarinTests.ObjCRuntime {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class RegistrarSharedTest {
		public static Registrars CurrentRegistrar {
			get {
				return Registrar.CurrentRegistrar;
			}
		}

		[Test]
		public void IntPtrCtor ()
		{
			IntPtr ptr = IntPtr.Zero;
			try {
				ptr = Messaging.IntPtr_objc_msgSend (Class.GetHandle (typeof (IntPtrCtorTestClass)), Selector.GetHandle ("alloc"));
				ptr = Messaging.IntPtr_objc_msgSend (ptr, Selector.GetHandle ("init"));
				var ex = Assert.Throws<RuntimeException> (() => Messaging.bool_objc_msgSend_IntPtr (ptr, Selector.GetHandle ("testMethod:"), IntPtr.Zero));
				var lines = new List<string> ();
#if NET
				lines.Add (string.Format ("Failed to marshal the Objective-C object 0x{0} (type: IntPtrCtorTestClass). Could not find an existing managed instance for this object, nor was it possible to create a new managed instance (because the type 'XamarinTests.ObjCRuntime.RegistrarSharedTest+IntPtrCtorTestClass' does not have a constructor that takes one NativeHandle argument).", ptr.ToString ("x")));
#else
				lines.Add (string.Format ("Failed to marshal the Objective-C object 0x{0} (type: IntPtrCtorTestClass). Could not find an existing managed instance for this object, nor was it possible to create a new managed instance (because the type 'XamarinTests.ObjCRuntime.RegistrarSharedTest+IntPtrCtorTestClass' does not have a constructor that takes one IntPtr argument).", ptr.ToString ("x")));
#endif
				lines.Add ("Additional information:");
				lines.Add ("Selector: testMethod:");
				lines.Add ("TestMethod");
				foreach (var line in lines)
					Assert.That (ex.ToString (), Does.Contain (line), "#message");
			} finally {
				Messaging.void_objc_msgSend (ptr, Selector.GetHandle ("release"));
			}
		}

		[Register ("IntPtrCtorTestClass")]
		class IntPtrCtorTestClass : NSObject {
			[Export ("initWithFoo:")]
			public IntPtrCtorTestClass (int foo)
			{
				Console.WriteLine ("foo1");
			}

			[Export ("testMethod:")]
			public void TestMethod (IntPtrCtorTestClass p)
			{
			}
		}
	}
}
