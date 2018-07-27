using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;

#if XAMCORE_2_0
#if !__TVOS__ && !__WATCHOS__ && !MONOMAC
using AddressBook;
using AddressBookUI;
#endif
using Foundation;
#if MONOMAC
using AppKit;
using UIColor = AppKit.NSColor;
using PlatformException = ObjCRuntime.RuntimeException;
using NativeException = Foundation.ObjCException;
#else
using UIKit;
using PlatformException = ObjCRuntime.RuntimeException;
using NativeException = Foundation.MonoTouchException;
#endif
using ObjCRuntime;
#if !__TVOS__
using MapKit;
#endif
#if !__WATCHOS__
using CoreAnimation;
#endif
using CoreGraphics;
using CoreLocation;
#if !__TVOS__
using Contacts;
#endif
#else
using MonoTouch;
using MonoTouch.AddressBook;
using MonoTouch.AddressBookUI;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.MapKit;
using MonoTouch.CoreAnimation;
using MonoTouch.CoreGraphics;
using MonoTouch.CoreLocation;
using MonoTouch.UIKit;
using PlatformException=MonoTouch.RuntimeException;
using NativeException=MonoTouch.Foundation.MonoTouchException;
#endif
using OpenTK;
using NUnit.Framework;
using Bindings.Test;

#if XAMCORE_2_0
using RectangleF = CoreGraphics.CGRect;
using SizeF = CoreGraphics.CGSize;
using PointF = CoreGraphics.CGPoint;
using CategoryAttribute = ObjCRuntime.CategoryAttribute;
#else
using nfloat=global::System.Single;
using nint=global::System.Int32;
using nuint=global::System.UInt32;
using CategoryAttribute=MonoTouch.ObjCRuntime.CategoryAttribute;
#endif

using XamarinTests.ObjCRuntime;

namespace MonoTouchFixtures.ObjCRuntime {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class RegistrarTest {
#if !XAMCORE_2_0
		// This should throw an exception at build time (device) or at registration time (startup on simulator).
		// Putting it here for now so I don't forget.
		[Export ()]
		public bool? IsRegistered  { get; set; }
#endif

		public static Registrars CurrentRegistrar {
			get {
				return RegistrarSharedTest.CurrentRegistrar;
			}
		}

		[Test]
		public void RegistrarRemoval ()
		{
			// define set by xharness when creating test variations.
			// It's not safe to remove the dynamic registrar in monotouch-test (by design; some of the tested API makes it unsafe, and the linker correcty detects this),
			// so the dynamic registrar will only be removed if manually requested.
			// Also removal of the dynamic registrar is not supported in XM
#if OPTIMIZEALL && !__MACOS__
			var shouldBeRemoved = true;
#else
			var shouldBeRemoved = false;
#endif
			Assert.AreEqual (shouldBeRemoved, typeof (NSObject).Assembly.GetType ("Registrar.Registrar") == null, "Registrar removal");
			Assert.AreEqual (shouldBeRemoved, typeof (NSObject).Assembly.GetType ("Registrar.DynamicRegistrar") == null, "DynamicRegistrar removal");
		}

#if !MONOMAC
		[Test]
		public void TestProperties ()
		{
			RegistrarTestClass obj = new RegistrarTestClass ();
			IntPtr receiver = obj.Handle;
			int dummy = 314;
			
			// readonly, attribute on property
			CallProperty (receiver, "Property1", ref obj.called_Property1Getter, "#Instance-1-r");
			CallProperty (receiver, "setProperty1:", ref dummy, "#Instance-1-w", true);
			// rw, attribute on property
			CallProperty (receiver, "Property2", ref obj.called_Property2Getter, "#Instance-2-r");
			CallProperty (receiver, "setProperty2:", ref obj.called_Property2Setter, "#Instance-2-w");
			// writeonly, attribute on property
			//CallProperty (receiver, "Property3", ref dummy, "#Instance-3-r", true);
			CallProperty (receiver, "setProperty3:", ref obj.called_Property3Setter, "#Instance-3-w");
			
			// readonly, atteribute on getter
			CallProperty (receiver, "Property4", ref obj.called_Property4Getter, "#Instance-4-r");
			CallProperty (receiver, "setProperty4:", ref dummy, "#Instance-4-w", true);
			// rw, attribyte on getter/setter
			CallProperty (receiver, "Property5", ref obj.called_Property5Getter, "#Instance-5-r");
			CallProperty (receiver, "setProperty5:", ref dummy, "#Instance-5-w1", true);
			CallProperty (receiver, "WProperty5:", ref obj.called_Property5Setter, "#Instance-5-w2");
			// writeonly, attribute on setter
			CallProperty (receiver, "setProperty6:", ref dummy, "#Instance-6-r", true);
			CallProperty (receiver, "Property6:", ref obj.called_Property6Setter, "#Instance-6-w");
		}
		
		[Test]
		public void TestStaticProperties ()
		{
			IntPtr receiver = Class.GetHandle ("RegistrarTestClass");
			int dummy = 314;
			
			RegistrarTestClass.called_StaticProperty1Getter = 0;
			RegistrarTestClass.called_StaticProperty2Getter = 0;
			RegistrarTestClass.called_StaticProperty4Getter = 0;
			RegistrarTestClass.called_StaticProperty5Getter = 0;
			RegistrarTestClass.called_StaticProperty2Setter = 0;
			RegistrarTestClass.called_StaticProperty3Setter = 0;
			RegistrarTestClass.called_StaticProperty5Setter = 0;
			RegistrarTestClass.called_StaticProperty6Setter = 0;
			
			// readonly, attribute on property
			CallProperty (receiver, "StaticProperty1", ref RegistrarTestClass.called_StaticProperty1Getter, "#Static-1-r");
			CallProperty (receiver, "setStaticProperty1:", ref dummy, "#Static-1-w", true);
			// rw, attribute on property
			CallProperty (receiver, "StaticProperty2", ref RegistrarTestClass.called_StaticProperty2Getter, "#Static-2-r");
			CallProperty (receiver, "setStaticProperty2:", ref RegistrarTestClass.called_StaticProperty2Setter, "#Static-2-w");
			// writeonly, attribute on property
			CallProperty (receiver, "StaticProperty3", ref dummy, "#Static-3-r", true);
			CallProperty (receiver, "setStaticProperty3:", ref RegistrarTestClass.called_StaticProperty3Setter, "#Static-3-w");
			
			// readonly, atteribute on getter
			CallProperty (receiver, "StaticProperty4", ref RegistrarTestClass.called_StaticProperty4Getter, "#Static-4-r");
			CallProperty (receiver, "setStaticProperty4:", ref dummy, "#Static-4-w", true);
			// rw, attribyte on getter/setter
			CallProperty (receiver, "StaticProperty5", ref RegistrarTestClass.called_StaticProperty5Getter, "#Static-5-r");
			CallProperty (receiver, "setStaticProperty5:", ref dummy, "#Static-5-w1", true);
			CallProperty (receiver, "WStaticProperty5:", ref RegistrarTestClass.called_StaticProperty5Setter, "#Static-5-w2");
			// writeonly, attribute on setter
			CallProperty (receiver, "setStaticProperty6:", ref dummy, "#Static-6-r", true);
			CallProperty (receiver, "StaticProperty6:", ref RegistrarTestClass.called_StaticProperty6Setter, "#Static-6-w");
		}
#endif

		void CallProperty (IntPtr receiver, string selector, ref int called_var, string id, bool expectFailure = false)
		{
#if !MONOMAC
			if (Runtime.Arch == Arch.DEVICE && expectFailure) {
				Console.WriteLine ("Skipping '{0}', it's expected to throw a 'Selector not found exception', but on device it seems to crash instead", selector);
				return;
			}
#endif

			try {
				Messaging.bool_objc_msgSend (receiver, new Selector (selector).Handle);
				Assert.That (!expectFailure, id + "-expected-failure-but-succeeded");
				Assert.That (called_var == 1, id + "-called-var");
			} catch (NativeException ex) {
				Assert.That (expectFailure, id + "-expected-success-but-failed: " + ex.Message);
			}
		}
		
		[Test]
		public void TestINativeObject ()
		{
			IntPtr receiver = Class.GetHandle ("RegistrarTestClass");
			IntPtr ptr;
			CGPath path;
			
			if ((CurrentRegistrar & Registrars.AllStatic) == 0)
				Assert.Ignore ("This test only passes with the static registrars.");
			
			Assert.False (Messaging.bool_objc_msgSend_IntPtr (receiver, new Selector ("INativeObject1:").Handle, IntPtr.Zero), "#a1");
			Assert.True (Messaging.bool_objc_msgSend_IntPtr (receiver, new Selector ("INativeObject1:").Handle, new CGPath ().Handle), "#a2");
			
			Assert.That (Messaging.IntPtr_objc_msgSend_bool (receiver, new Selector ("INativeObject2:").Handle, false), Is.EqualTo (IntPtr.Zero), "#b1");
			Assert.That (Messaging.IntPtr_objc_msgSend_bool (receiver, new Selector ("INativeObject2:").Handle, true), Is.Not.EqualTo (IntPtr.Zero), "#b2");
			
			void_objc_msgSend_out_IntPtr_bool (receiver, new Selector ("INativeObject3:create:").Handle, out ptr, true);
			Assert.That (ptr, Is.Not.EqualTo (IntPtr.Zero), "#c1");
			void_objc_msgSend_out_IntPtr_bool (receiver, new Selector ("INativeObject3:create:").Handle, out ptr, false);
			Assert.That (ptr, Is.EqualTo (IntPtr.Zero), "#c2");
			
			path = null;
			ptr = IntPtr.Zero;
			Assert.False (bool_objc_msgSend_ref_intptr (receiver, new Selector ("INativeObject4:").Handle, ref ptr), "#d1");
			Assert.That (ptr, Is.EqualTo (IntPtr.Zero), "#d2");
			path = new CGPath ();
			ptr = path.Handle;
			Assert.True (bool_objc_msgSend_ref_intptr (receiver, new Selector ("INativeObject4:").Handle, ref ptr), "#d3");
			Assert.That (ptr, Is.EqualTo (path.Handle), "#d4");
			
			ptr = Messaging.IntPtr_objc_msgSend_bool (receiver, new Selector ("INativeObject5:").Handle, false);
			Assert.That (ptr, Is.EqualTo (IntPtr.Zero), "#e1");
			ptr = Messaging.IntPtr_objc_msgSend_bool (receiver, new Selector ("INativeObject5:").Handle, true);
			Assert.That (ptr, Is.Not.EqualTo (IntPtr.Zero), "#e2");
			path = new CGPath (ptr);
			path.AddArc (1, 2, 3, 4, 5, false); // this should crash if we get back a bogus ptr
		}

		[Test]
		public void TestVirtual ()
		{
			// bug #4426
			DerivedRegistrar1 d1 = new DerivedRegistrar1 ();
			DerivedRegistrar2 d2 = new DerivedRegistrar2 ();
			Selector sel = new Selector ("VirtualMethod");
			
			string a = NSString.FromHandle (Messaging.IntPtr_objc_msgSend (d1.Handle, sel.Handle)).ToString ();
			string b = NSString.FromHandle (Messaging.IntPtr_objc_msgSend (d2.Handle, sel.Handle)).ToString ();
			
			Assert.That (a, Is.EqualTo (d1.GetType ().Name), "#a");
			Assert.That (b, Is.EqualTo (d2.GetType ().Name), "#b");
		}

		[Test]
		public void TestOutNSString ()
		{
			var obj = new RegistrarTestClass ();
			var sel = new Selector ("testOutNSString:");
			IntPtr ptr;

			void_objc_msgSend_out_IntPtr (obj.Handle, sel.Handle, out ptr);

			Assert.AreEqual ("Santa is coming", NSString.FromHandle (ptr), "#santa");
		}

		[Test]
		public void TestInheritedStaticMethods ()
		{
			// bug #6170
			int rv;

			rv = Messaging.int_objc_msgSend (Class.GetHandle (typeof(StaticBaseClass)), Selector.GetHandle ("foo"));
			Assert.AreEqual (rv, 314, "#base");
			rv = Messaging.int_objc_msgSend (Class.GetHandle (typeof(StaticDerivedClass)), Selector.GetHandle ("foo"));
			Assert.AreEqual (rv, 314, "#derived");
		}

		[Test]
		public void TestStructAndOut ()
		{
			var obj = new RegistrarTestClass ();
			var sel = new Selector ("testOutParametersWithStructs:in:out:");
			NSError value = new NSError ();
			IntPtr ptr;
			SizeF size = new SizeF (1, 2);

			void_objc_msgSend_SizeF_IntPtr_out_IntPtr (obj.Handle, sel.Handle, size, value.Handle, out ptr);

			Assert.AreEqual (value.Handle, ptr, "#1");
		}

#if !__TVOS__ && !__WATCHOS__ && !MONOMAC
		[MonoPInvokeCallback (typeof (DActionArity1V1))]
		static void DActionArity1V1Func (IntPtr block, UIBackgroundFetchResult result)
		{
		}

		[Test]
		public void TestAction ()
		{
			if (!Runtime.DynamicRegistrationSupported)
				Assert.Ignore ("This test requires the dynamic registrar to be available.");

			using (var obj = new RegistrarTestClass ()) {
				var sel = new Selector ("testAction:");
				var block = new BlockLiteral ();
				var tramp = new DActionArity1V1 (DActionArity1V1Func);
				var del = new DActionArity1V1 (DActionArity1V1Func);
				block.SetupBlock (tramp, del);
				void_objc_msgSend_ref_BlockLiteral (obj.Handle, sel.Handle, ref block);
				block.CleanupBlock ();
			}
		}
#endif // !__TVOS__ && !__WATCHOS__

		class TS1 : NSObject {}
		class TS2 : NSObject {}
		class TS3 : NSObject {}
		class TS4 : NSObject {}
		class TS5 : NSObject {}
		class TS6 : NSObject {}
		class TS7 : NSObject {}
		class TS8 : NSObject {}
		class TS9 : NSObject {}
		class TS10 : NSObject {}

		static bool tested_thread_safety;

		[Test]
		public void TestThreadSafety ()
		{
			// Create X number of threads.
			// Each thread waits for a signal from the main thread to go ahead with the actual test.

			if (tested_thread_safety)
				Assert.Ignore ("This test can only be executed once. To run it again restart the test app.");
			tested_thread_safety = true;

			var threads = new Thread [5];
			var types = new Type [] { typeof(TS1), typeof(TS2), typeof(TS3), typeof(TS4), typeof(TS5), typeof (TS6), typeof (TS7), typeof (TS8), typeof (TS9), typeof (TS10) };
			var exceptions = new List<Exception> ();
			var wait = new ManualResetEvent (false);
			var start_counter = new CountdownEvent (threads.Length);
			var end_counter = new CountdownEvent (threads.Length);

			for (int i = 0; i < threads.Length; i++) {
				threads [i] = new Thread (() => {
					try {
						start_counter.Signal (); // signal "I'm ready"
						wait.WaitOne (); // wait for go-ahead
						// Do the actual test.
						// We fetch the class handle for the types in question in several threads at once.
						// This will cause the registrar to try to register the type, but that should only
						// be done once.
						foreach (var t in types)
							Class.GetHandle (t);
					} catch (Exception ex) {
						lock (exceptions)
							exceptions.Add (ex);
					} finally {
						end_counter.Signal (); // signal "I'm done"
					}
				}) {
					IsBackground = true,
				};
				threads [i].Start ();
			}

			// Wait for X "I'm ready" signals
			Assert.IsTrue (start_counter.Wait (1000), "all threads didn't spin up in 1s");

			wait.Set (); // let the threads go wild.

			Assert.IsTrue (end_counter.Wait (5000), "all threads didn't finish testing in 5s");

			for (int i = 0; i < threads.Length; i++) {
				Assert.IsTrue (threads [i].Join (1000), "join #" + i.ToString ());
			}

			if (exceptions.Count > 0) {
				Assert.Fail ("Expected no exceptions, but got:\n{0}",
					new AggregateException (exceptions).ToString ());
			}
		}

		[Test]
		public void TestRetainReturnValue ()
		{
			IntPtr ptr;

			// The NSAutoreleasePool is to flush any pending 'autorelease' calls.
			// We fetch the managed object to make sure there are no lingering
			// managed objects (otherwise we wouldn't know if the GC had freed
			// the managed object or not).

			using (var obj = new RegistrarTestClass ()) {

				using (var pool = new NSAutoreleasePool ())
					ptr = Messaging.IntPtr_objc_msgSend (obj.Handle, Selector.GetHandle ("testRetainArray"));
				using (var rv = Runtime.GetNSObject (ptr)) {
					Assert.AreEqual (2, rv.RetainCount, "array");
					Assert.AreSame (typeof(NSArray), rv.GetType (), "array type");
					rv.DangerousRelease ();
				}

				using (var pool = new NSAutoreleasePool ())
					ptr = Messaging.IntPtr_objc_msgSend (obj.Handle, Selector.GetHandle ("testReturnINativeObject"));
				using (var rv = Runtime.GetNSObject (ptr)) {
					Assert.AreEqual (2, rv.RetainCount, "inativeobject");
					Assert.AreSame (typeof(NSObject), rv.GetType (), "inativeobject type");
					rv.DangerousRelease ();
				}

				using (var pool = new NSAutoreleasePool ())
					ptr = Messaging.IntPtr_objc_msgSend (obj.Handle, Selector.GetHandle ("testRetainNSObject"));
				using (var rv = Runtime.GetNSObject (ptr)) {
					Assert.AreEqual (2, rv.RetainCount, "nsobject");
					Assert.AreSame (typeof(NSObject), rv.GetType (), "nsobject type");
					rv.DangerousRelease ();
				}

				using (var pool = new NSAutoreleasePool ())
					ptr = Messaging.IntPtr_objc_msgSend (obj.Handle, Selector.GetHandle ("testRetainString"));
				using (var rv = Runtime.GetNSObject (ptr)) {
					Assert.AreEqual (2, rv.RetainCount, "string");
					Assert.IsTrue (rv is NSString, "string type");
					rv.DangerousRelease ();
				}
			}

			using (var obj = new DerivedRegistrar1 ()) {
				using (var pool = new NSAutoreleasePool ())
					ptr = Messaging.IntPtr_objc_msgSend (obj.Handle, Selector.GetHandle ("testOverriddenRetainNSObject"));
				using (var rv = Runtime.GetNSObject (ptr)) {
					Assert.AreEqual (2, rv.RetainCount, "overridden nsobject");
					Assert.AreSame (typeof(NSObject), rv.GetType (), "overridden nsobject type");
					rv.DangerousRelease ();
				}

			}
		}

		class Props : NSObject {
			[Export ("myProp")]
			public string MyProp {
				get;
				set;
			}
		}

		[Test]
		public void TestObjCProperties ()
		{
			var class_handle = Class.GetHandle (typeof(Props));
			Assert.AreNotEqual (IntPtr.Zero, class_getProperty (class_handle, "myProp"));
		}

		[DllImport ("/usr/lib/libobjc.dylib")]
		static extern IntPtr class_getProperty (IntPtr cls, string name);

		[Test]
		public void TestNonVirtualProperty ()
		{
			using (var obj = new DerivedRegistrar1 ()) {
				Assert.IsTrue (Messaging.bool_objc_msgSend (obj.Handle, Selector.GetHandle ("b1")));
			}
		}

		[Test]
		public void TestGeneric ()
		{
			var g1 = new GenericTestClass<string> ();
			var g2 = new GenericTestClass<object> ();
			var g3 = new DerivedGenericTestClass<string> ();
			var g4 = new DerivedGenericTestClass<object> ();
			var sel = new Selector ("GetTypeFullName").Handle;

			string t1 = NSString.FromHandle (Messaging.IntPtr_objc_msgSend (g1.Handle, sel)).ToString ();
			string t2 = NSString.FromHandle (Messaging.IntPtr_objc_msgSend (g2.Handle, sel)).ToString ();
			string t3 = NSString.FromHandle (Messaging.IntPtr_objc_msgSend (g3.Handle, sel)).ToString ();
			string t4 = NSString.FromHandle (Messaging.IntPtr_objc_msgSend (g4.Handle, sel)).ToString ();

			Assert.AreEqual (g1.GetTypeFullName (), t1, "#t1");
			Assert.AreEqual (g2.GetTypeFullName (), t2, "#t2");
			Assert.AreEqual (g3.GetTypeFullName (), t3, "#t3");
			Assert.AreEqual (g4.GetTypeFullName (), t4, "#t4");

			var openClass = Class.GetHandle ("Open_1");
			var handle = Messaging.IntPtr_objc_msgSend (openClass, Selector.GetHandle ("alloc"));
			try {
				handle = Messaging.IntPtr_objc_msgSend (handle, Selector.GetHandle ("init"));
				Assert.Fail ("Expected [[Open_1 alloc] init] to fail.");
			} catch (PlatformException mex) {
				Assert.AreEqual ("Cannot construct an instance of the type 'MonoTouchFixtures.ObjCRuntime.RegistrarTest+Open`1' from Objective-C because the type is generic.", mex.Message);
			} finally {
				Messaging.void_objc_msgSend (handle, Selector.GetHandle ("release")); // or should this be dealloc directly?
			}

			Assert.DoesNotThrow (() => new Open<string> (), "Create managed open instance");

			// It's possible to create an instance of a closed class.
			var closedClass = Class.GetHandle (typeof (Closed));
			handle = Messaging.IntPtr_objc_msgSend (closedClass, Selector.GetHandle ("alloc"));
			handle = Messaging.IntPtr_objc_msgSend (handle, Selector.GetHandle ("init"));
			Messaging.void_objc_msgSend (handle, Selector.GetHandle ("release"));

			Assert.DoesNotThrow (() => new Closed (), "Created managed closed instance");
		}

		[Test]
		public void TestNestedGenericType ()
		{
			var foo = new NestedParent<NSObject>.Nested ();
			var obj = new NSObject ();
			Messaging.void_objc_msgSend_IntPtr (foo.Handle, Selector.GetHandle ("foo:"), obj.Handle);
			obj.Dispose ();
			foo.Dispose ();
		}

		[Test]
		public void TestInstanceMethodOnOpenGenericType ()
		{
			{
				var foo = new Open<NSSet, string> ();

				var view = new NSSet ();
				var expectedU = typeof(NSSet);
				var expectedV = typeof(string);
				Messaging.void_objc_msgSend_IntPtr (foo.Handle, Selector.GetHandle ("bar:"), IntPtr.Zero);
				Assert.IsNull (foo.LastArg);
				Assert.AreEqual (expectedU, foo.UType);
				Assert.AreEqual (expectedV, foo.VType);
				Messaging.void_objc_msgSend_IntPtr (foo.Handle, Selector.GetHandle ("bar:"), view.Handle);
				Assert.AreSame (view, foo.LastArg);
				Assert.AreEqual (expectedU, foo.UType);
				Assert.AreEqual (expectedV, foo.VType);

				var arr = NSArray.FromNSObjects (view);
				Messaging.void_objc_msgSend_IntPtr (foo.Handle, Selector.GetHandle ("zap:"), IntPtr.Zero);
				Assert.IsNull (foo.LastArg);
				Assert.AreEqual (expectedU, foo.UType);
				Assert.AreEqual (expectedV, foo.VType);
				Messaging.void_objc_msgSend_IntPtr (foo.Handle, Selector.GetHandle ("zap:"), arr.Handle);
				Assert.AreSame (view, ((object[])foo.LastArg) [0]);
				Assert.AreEqual (expectedU, foo.UType);
				Assert.AreEqual (expectedV, foo.VType);

				Assert.AreEqual (IntPtr.Zero, Messaging.IntPtr_objc_msgSend (foo.Handle, Selector.GetHandle ("xyz")), "xyz");
				Assert.IsNull (foo.LastArg);
				Assert.AreEqual (expectedU, foo.UType);
				Assert.AreEqual (expectedV, foo.VType);

				Assert.AreEqual (IntPtr.Zero, Messaging.IntPtr_objc_msgSend (foo.Handle, Selector.GetHandle ("barzap")), "barzap");
				Assert.IsNull (foo.LastArg);
				Assert.AreEqual (expectedU, foo.UType);
				Assert.AreEqual (expectedV, foo.VType);

				Messaging.void_objc_msgSend_IntPtr (foo.Handle, Selector.GetHandle ("setBarzap:"), IntPtr.Zero);
				Assert.IsNull (foo.LastArg);
				Assert.AreEqual (expectedU, foo.UType);
				Assert.AreEqual (expectedV, foo.VType);

				Messaging.void_objc_msgSend_IntPtr (foo.Handle, Selector.GetHandle ("setBarzap:"), view.Handle);
				Assert.AreSame (view, foo.LastArg);
				Assert.AreEqual (expectedU, foo.UType);
				Assert.AreEqual (expectedV, foo.VType);

				arr.Dispose ();
				view.Dispose ();
				foo.Dispose ();
			}

			{
				var foo = new Open<NSObject, int> ();

				var view = new NSObject ();
				var expectedU = typeof(NSObject);
				var expectedV = typeof(int);
				Messaging.void_objc_msgSend_IntPtr (foo.Handle, Selector.GetHandle ("bar:"), IntPtr.Zero);
				Assert.IsNull (foo.LastArg);
				Assert.AreEqual (expectedU, foo.UType);
				Assert.AreEqual (expectedV, foo.VType);
				Messaging.void_objc_msgSend_IntPtr (foo.Handle, Selector.GetHandle ("bar:"), view.Handle);
				Assert.AreSame (view, foo.LastArg);
				Assert.AreEqual (expectedU, foo.UType);
				Assert.AreEqual (expectedV, foo.VType);

				var arr = NSArray.FromNSObjects (view);
				Messaging.void_objc_msgSend_IntPtr (foo.Handle, Selector.GetHandle ("zap:"), IntPtr.Zero);
				Assert.IsNull (foo.LastArg);
				Assert.AreEqual (expectedU, foo.UType);
				Assert.AreEqual (expectedV, foo.VType);
				Messaging.void_objc_msgSend_IntPtr (foo.Handle, Selector.GetHandle ("zap:"), arr.Handle);
				Assert.AreSame (view, ((object[])foo.LastArg) [0]);
				Assert.AreEqual (expectedU, foo.UType);
				Assert.AreEqual (expectedV, foo.VType);

				Assert.AreEqual (IntPtr.Zero, Messaging.IntPtr_objc_msgSend (foo.Handle, Selector.GetHandle ("xyz")), "xyz");
				Assert.IsNull (foo.LastArg);
				Assert.AreEqual (expectedU, foo.UType);
				Assert.AreEqual (expectedV, foo.VType);

				Assert.AreEqual (IntPtr.Zero, Messaging.IntPtr_objc_msgSend (foo.Handle, Selector.GetHandle ("barzap")), "barzap");
				Assert.IsNull (foo.LastArg);
				Assert.AreEqual (expectedU, foo.UType);
				Assert.AreEqual (expectedV, foo.VType);

				Messaging.void_objc_msgSend_IntPtr (foo.Handle, Selector.GetHandle ("setBarzap:"), IntPtr.Zero);
				Assert.IsNull (foo.LastArg);
				Assert.AreEqual (expectedU, foo.UType);
				Assert.AreEqual (expectedV, foo.VType);

				Messaging.void_objc_msgSend_IntPtr (foo.Handle, Selector.GetHandle ("setBarzap:"), view.Handle);
				Assert.AreSame (view, foo.LastArg);
				Assert.AreEqual (expectedU, foo.UType);
				Assert.AreEqual (expectedV, foo.VType);

				arr.Dispose ();
				view.Dispose ();
				foo.Dispose ();
			}
		}

#if !__WATCHOS__ && !MONOMAC
		[Test]
		public void TestGenericUIView ()
		{
			using (var iview = new NullableIntView (new RectangleF (0, 0, 100, 100))) {
				using (var strview = new StringView (new RectangleF (0, 0, 100, 100))) {
					Messaging.void_objc_msgSend_RectangleF (iview.Handle, Selector.GetHandle ("drawRect:"), RectangleF.Empty);
					Assert.AreEqual (typeof(int?), iview.TypeT, "int?");
					Assert.AreEqual ("NullableIntView", iview.TypeName, "int? typename");
					Messaging.void_objc_msgSend_RectangleF (strview.Handle, Selector.GetHandle ("drawRect:"), RectangleF.Empty);
					Assert.AreEqual (typeof(string), strview.TypeT, "string");
					Assert.AreEqual ("StringView", strview.TypeName, "string typename");
				}
			}
		}
#endif // !__WATCHOS__

#if !__WATCHOS__ && !MONOMAC
		[Test]
		public void TestNativeEnum ()
		{
			//public virtual void TestNativeEnum1 (UITextWritingDirection twd)
			using (var obj = new RegistrarTestClass ()) {
				if (IntPtr.Size == 4) {
					Messaging.void_objc_msgSend_int (obj.Handle, Selector.GetHandle ("testNativeEnum1:"), (int) UITextWritingDirection.RightToLeft);
				} else {
					Messaging.void_objc_msgSend_long (obj.Handle, Selector.GetHandle ("testNativeEnum1:"), (long)UITextWritingDirection.RightToLeft);
				}

				if (IntPtr.Size == 4) {
					Messaging.void_objc_msgSend_int_int_long (obj.Handle, Selector.GetHandle ("testNativeEnum1:"), (int) UITextWritingDirection.RightToLeft, 31415, 3141592);
				} else {
					Messaging.void_objc_msgSend_long_int_long (obj.Handle, Selector.GetHandle ("testNativeEnum1:"), (long) UITextWritingDirection.RightToLeft, 31415, 3141592);
				}

				if (IntPtr.Size == 4) {
					Assert.AreEqual ((int) UIPopoverArrowDirection.Right, Messaging.int_objc_msgSend (obj.Handle, Selector.GetHandle ("testNativeEnum2")), "testNativeEnum2");
					Messaging.void_objc_msgSend_int (obj.Handle, Selector.GetHandle ("setTestNativeEnum2:"), (int) UIPopoverArrowDirection.Left);
				} else {
					Assert.AreEqual ((long) UIPopoverArrowDirection.Right, Messaging.long_objc_msgSend (obj.Handle, Selector.GetHandle ("testNativeEnum2")), "testNativeEnum2");
					Messaging.void_objc_msgSend_long (obj.Handle, Selector.GetHandle ("setTestNativeEnum2:"), (long) UIPopoverArrowDirection.Left);
				}
			}
		}
#endif // !__WATCHOS__

		[Test]
		public void Bug23289 ()
		{
			using (var obj = new RegistrarTestClass ()) {
				using (var arr = new NSMutableArray (1)) {
					var cl = Messaging.IntPtr_objc_msgSend (Class.GetHandle (typeof (CLLocation)), Selector.GetHandle ("alloc"));
					cl = Messaging.IntPtr_objc_msgSend_double_double (cl, Selector.GetHandle ("initWithLatitude:longitude:"), 1, 2);
					Messaging.void_objc_msgSend_IntPtr (arr.Handle, Selector.GetHandle ("addObject:"), cl);
					Messaging.bool_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("testBug23289:"), arr.Handle);
					Messaging.void_objc_msgSend (cl, Selector.GetHandle ("release"));
				}
			}
		}

		[Test]
		public void TestCGPointParameter ()
		{
			using (var obj = new RegistrarTestClass ()) {
				var pnt1 = new PointF (123, 456);
				PointF pnt2 = new PointF ();
				void_objc_msgSend_CGPoint_ref_CGPoint (obj.Handle, Selector.GetHandle ("testCGPoint:out:"), pnt1, ref pnt2);
				Assert.AreEqual (123, pnt2.X, "X");
				Assert.AreEqual (456, pnt2.Y, "Y");
			}
		}

		[Test]
		public void ExportedGenericsTest ()
		{
			using (var obj = new RegistrarTestClass ()) {
				var rv = Runtime.GetNSObject<NSArray<NSString>> (Messaging.IntPtr_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("fetchNSArrayOfNSString:"), IntPtr.Zero));
				Assert.IsNotNull (rv, "method");

				using (var number_array = NSArray<NSNumber>.FromNSObjects ((NSNumber) 314)) {
					rv = Runtime.GetNSObject<NSArray<NSString>> (Messaging.IntPtr_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("fetchNSArrayOfNSString:"), number_array.Handle));
					Assert.IsNotNull (rv, "method param");
				}

				rv = Runtime.GetNSObject<NSArray<NSString>> (Messaging.IntPtr_objc_msgSend (obj.Handle, Selector.GetHandle ("nSArrayOfNSString")));
				Assert.IsNotNull (rv, "property");

				Messaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("setNSArrayOfNSString:"), IntPtr.Zero);
				Messaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("setNSArrayOfNSString:"), rv.Handle);

				var rv2 = Runtime.GetNSObject<NSArray<NSArray<NSString>>> (Messaging.IntPtr_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("fetchComplexGenericType:"), IntPtr.Zero));
				Assert.IsNotNull (rv2, "complex");

				using (var complex = new NSArray<NSDictionary<NSString, NSArray<NSNumber>>> ()) {
					Runtime.GetNSObject<NSArray<NSArray<NSString>>> (Messaging.IntPtr_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("fetchComplexGenericType:"), complex.Handle));
					Assert.IsNotNull (rv2, "complex param");
				}
			}
		}

		const string LIBOBJC_DYLIB = "/usr/lib/libobjc.dylib";
		
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		extern static void void_objc_msgSend_out_IntPtr (IntPtr receiver, IntPtr selector, out IntPtr value);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		extern static void void_objc_msgSend_ref_IntPtr (IntPtr receiver, IntPtr selector, ref IntPtr value);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		extern static void void_objc_msgSend_out_IntPtr_bool (IntPtr receiver, IntPtr selector, out IntPtr path, bool create);
		
		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		extern static bool bool_objc_msgSend_ref_intptr (IntPtr receiver, IntPtr selector, ref IntPtr path);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		extern static void void_objc_msgSend_SizeF_IntPtr_out_IntPtr (IntPtr receiver, IntPtr selector, SizeF size, IntPtr input, out IntPtr value);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		extern static void void_objc_msgSend_ref_BlockLiteral (IntPtr receiver, IntPtr selector, ref BlockLiteral block);

		[DllImport (LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		extern static void void_objc_msgSend_CGPoint_ref_CGPoint (IntPtr receiver, IntPtr selector, PointF p1, ref PointF p2);

#region Exported type
		[Register ("RegistrarTestClass")]
		class RegistrarTestClass : NSObject
		{
			public virtual bool B1 {
				[Export ("b1")]
				get {
					return false;
				}
			}

			// static properties
			public static int called_StaticProperty1Getter;
			[Export ("StaticProperty1")]
			static bool StaticProperty1 {
				get {
					called_StaticProperty1Getter++;
					return true;
				}
			}
			
			public static int called_StaticProperty2Getter;
			public static int called_StaticProperty2Setter;
			[Export ("StaticProperty2")]
			static bool StaticProperty2 {
				get {
					called_StaticProperty2Getter++;
					return true;
				}
				set {
					called_StaticProperty2Setter++;
				}
			}
			
			public static int called_StaticProperty3Setter;
			[Export ("StaticProperty3")]
			static bool StaticProperty3 {
				set {
					called_StaticProperty3Setter++;
				}
			}
			
			public static int called_StaticProperty4Getter;
			static bool StaticProperty4 {
				[Export ("StaticProperty4")]
				get {
					called_StaticProperty4Getter++;
					return true;
				}
			}
			
			public static int called_StaticProperty5Getter;
			public static int called_StaticProperty5Setter;
			static bool StaticProperty5 {
				[Export ("StaticProperty5")]
				get {
					called_StaticProperty5Getter++;
					return true;
				}
				[Export ("WStaticProperty5:")] // can't use same name as getter, and don't use the default "set_" prefix either (to ensure we're not exporting the default prefix always)
				set {
					called_StaticProperty5Setter++;
				}
			}
			
			public static int called_StaticProperty6Setter;
			static bool StaticProperty6 {
				[Export ("StaticProperty6:")]
				set {
					called_StaticProperty6Setter++;
				}
			}
			
			// instance properties
			public int called_Property1Getter;
			[Export ("Property1")]
			bool Property1 {
				get {
					called_Property1Getter++;
					return true;
				}
			}
			
			public int called_Property2Getter;
			public int called_Property2Setter;
			[Export ("Property2")]
			bool Property2 {
				get {
					called_Property2Getter++;
					return true;
				}
				set {
					called_Property2Setter++;
				}
			}
			
			public int called_Property3Setter;
			[Export ("Property3")]
			bool Property3 {
				set {
					called_Property3Setter++;
				}
			}
			
			public int called_Property4Getter;
			bool Property4 {
				[Export ("Property4")]
				get {
					called_Property4Getter++;
					return true;
				}
			}
			
			public int called_Property5Getter;
			public int called_Property5Setter;
			bool Property5 {
				[Export ("Property5")]
				get {
					called_Property5Getter++;
					return true;
				}
				[Export ("WProperty5:")] // can't use same name as getter, and don't use the default "set_" prefix either (to ensure we're not exporting the default prefix always)
				set {
					called_Property5Setter++;
				}
			}
			
			public int called_Property6Setter;
			bool Property6 {
				[Export ("Property6:")]
				set {
					called_Property6Setter++;
				}
			}
			
			[Export ("INativeObject1:")]
			static bool INativeObject1 (CGPath img /*CGPath is a INativeObject */)
			{
				return img != null;
			}
			
			[Export ("INativeObject2:")]
			static CGPath INativeObject2 (bool create)
			{
				return create ? new CGPath () : null;
			}
			
			[Export ("INativeObject3:create:")]
			static void INativeObject3 (out CGPath path, bool create)
			{
				path = create ? new CGPath () : null;
			}
			
			[Export ("INativeObject4:")]
			static bool INativeObject4 (ref CGPath path)
			{
				return path != null;
			}
			
			[Export ("INativeObject5:")]
			static CGPath INativeObject5 (bool create)
			{
				return create ? new CGPath () : null;
			}
			
			[Export ("VirtualMethod")]
			public virtual string VirtualMethod ()
			{
				return "base";
			}
			
			[Export ("testNSAction:")]
			public void TestNSAction (Action action)
			{
			}

			[Export ("testOutNSString:")]
			public void TestOutNSString (out string value)
			{
				value = "Santa is coming";
			}

			[Export ("testOutParametersWithStructs:in:out:")]
			public void TestOutParameters (SizeF a, NSError @in, out NSError value)
			{
				// bug 16078
				value = @in;
			}

#if !__TVOS__ && !__WATCHOS__ && !MONOMAC
			[Export ("testAction:")]
			public void TestAction ([BlockProxy (typeof (NIDActionArity1V1))] Action<UIBackgroundFetchResult> action)
			{
				// bug ?
			}
#endif // !__TVOS__ && !__WATCHOS__

			[return: ReleaseAttribute ()]
			[Export ("testRetainArray")]
			public NSObject[] TestRetainArray ()
			{
				return new NSObject[] { new NSObject () };
			}

			[Export ("testBug23289:")]
			public bool TestBug23289 (CLLocation [] array)
			{
				return true;
			}

			[return: ReleaseAttribute ()]
			[Export ("testReturnINativeObject")]
			public INativeObject TestRetainINativeObject ()
			{
				return new NSObject ();
			}

			[return: ReleaseAttribute ()]
			[Export ("testRetainNSObject")]
			public NSObject TestRetainNSObject ()
			{
				return new NSObject ();
			}

			[return: ReleaseAttribute ()]
			[Export ("testRetainString")]
			public string TestRetainString ()
			{
				return "some string that does not match a constant NSString";
			}

			[return: ReleaseAttribute ()]
			[Export ("testOverriddenRetainNSObject")]
			public virtual NSObject TestOverriddenRetainNSObject ()
			{
				return new NSObject ();
			}

#if !__WATCHOS__ && !MONOMAC
			[Export ("testNativeEnum1:")]
			public virtual void TestNativeEnum1 (UITextWritingDirection twd)
			{
				Assert.That (Enum.GetValues (typeof (UITextWritingDirection)), Contains.Item (twd), "TestNativeEnum1");
			}

			public virtual UIPopoverArrowDirection TestNativeEnum2 {
				[Export ("testNativeEnum2")]
				get {
					return UIPopoverArrowDirection.Right;
				}
				[Export ("setTestNativeEnum2:")]
				set {
					Assert.AreEqual (UIPopoverArrowDirection.Left, value, "setTestNativeEnum2:");
				}
			}


			[Export ("testNativeEnum3:a:b:")]
			public virtual void TestNativeEnum1 (UITextWritingDirection twd, int a, long b)
			{
				Assert.That (Enum.GetValues (typeof (UITextWritingDirection)), Contains.Item (twd), "TestNativeEnum3");
				Assert.AreEqual (31415, a, "TestNativeEnum3 a");
				Assert.AreEqual (3141592, b, "TestNativeEnum3 b");
			}
#endif // !__WATCHOS__

			[Export ("testCGPoint:out:")]
			public void TestCGPoint (PointF pnt, ref PointF pnt2)
			{
				pnt2.X = pnt.X;
				pnt2.Y = pnt.Y;
			}
#if !__WATCHOS__ && !MONOMAC
			[Export ("arrayOfINativeObject")]
			public IUIKeyInput[] NativeObjects { get { return null; } }
#endif // !__WATCHOS__

			[Export ("fetchNSArrayOfNSString:")]
			NSArray<NSString> FetchNSArrayOfNSString (NSArray<NSNumber> p0)
			{
				return NSArray<NSString>.FromNSObjects ((NSString) "abc");
			}

			[Export ("fetchComplexGenericType:")]
			NSArray<NSArray<NSString>> FetchComplexGenericType (NSArray<NSDictionary<NSString, NSArray<NSNumber>>> p0)
			{
				return new NSArray<NSArray<NSString>> ();
			}

			[Export ("nSArrayOfNSString")]
			NSArray<NSString> NSArrayOfNSString {
				get {
					return new NSArray<NSString> ();
				}
				set {
				}
			}
		}

#if !__TVOS__ && !__WATCHOS__ && !MONOMAC
		[UnmanagedFunctionPointerAttribute (CallingConvention.Cdecl)]
		internal delegate void DActionArity1V1 (IntPtr block, UIBackgroundFetchResult obj);

		//
		// This class bridges native block invocations that call into C#
		//
		static internal class SDActionArity1V1 {
			static internal readonly DActionArity1V1 Handler = Invoke;

			[MonoPInvokeCallback (typeof (DActionArity1V1))]
			static unsafe void Invoke (IntPtr block, UIBackgroundFetchResult obj) {
				var descriptor = (BlockLiteral *) block;
				var del = (global::System.Action<UIBackgroundFetchResult>) (descriptor->Target);
				if (del != null)
					del (obj);
			}
		} /*		 class SDActionArity1V1 */

		internal class NIDActionArity1V1 {
			IntPtr blockPtr;
			DActionArity1V1 invoker;

			[Preserve (Conditional=true)]
			public unsafe NIDActionArity1V1 (BlockLiteral *block)
			{
				blockPtr = (IntPtr) block;
				invoker = block->GetDelegateForBlock<DActionArity1V1> ();
			}
			[Preserve (Conditional=true)]
			public unsafe static global::System.Action<UIBackgroundFetchResult> Create (IntPtr block)
			{
				return new NIDActionArity1V1 ((BlockLiteral *) block).Invoke;
			}

			[Preserve (Conditional=true)]
			unsafe void Invoke (UIBackgroundFetchResult obj)
			{
				invoker (blockPtr, obj);
			}
		} /*		 class NIDActionArity1V1 */
#endif // !__TVOS__


		[Register ("StaticBaseClass")]
		class StaticBaseClass : NSObject
		{
			[Export ("foo")]
			public static int Foo ()
			{
				return 314;
			}
		}

		[Register ("StaticDerivedClass")]
		class StaticDerivedClass : StaticBaseClass
		{
		}
		
		[Register ("DerivedRegistrar1")]
		class DerivedRegistrar1 : RegistrarTestClass
		{
			public override string VirtualMethod ()
			{
				return "DerivedRegistrar1";
			}

			public override NSObject TestOverriddenRetainNSObject ()
			{
				return base.TestOverriddenRetainNSObject ();
			}

			[Export ("b1")]
			public new bool B1 {
				get {
					return true;
				}
			}
		}
		
		[Register ("DerivedRegistrar2")]
		class DerivedRegistrar2 : DerivedRegistrar1
		{
			public override string VirtualMethod ()
			{
				return "DerivedRegistrar2";
			}
		}

		[Register ("GenericBaseClass")]
		class GenericBaseClass : NSObject
		{
			[Export ("GetTypeFullName")]
			public virtual string GetTypeFullName ()
			{
				return GetType ().FullName;
			}
		}

		[Register ("Open_1")]
		class Open<T> : NSObject {}
		class Closed : Open<NSSet>
		{
			[Export ("foo")]
			public void Foo ()
			{
			}

			[Export ("bar")]
			public static void Bar ()
			{
			}

			[Export ("zap:")]
			public void Zap (string arg)
			{
			}
		}

		class Open<U, V> : NSObject where U: NSObject
		{
			public object LastArg;
			public object UType;
			public object VType;

			[Export ("bar:")]
			public void Bar (U arg)
			{
				UType = typeof(U);
				VType = typeof(V);
				LastArg = arg;
			}

			[Export ("zap:")]
			public void Zap (U[] arg)
			{
				UType = typeof(U);
				VType = typeof(V);
				LastArg = arg;
			}

			[Export ("xyz")]
			public U XyZ ()
			{
				UType = typeof(U);
				VType = typeof(V);
				LastArg = null;
				return null;
			}

			[Export ("barzap")]
			public U BarZap { 
				get { 
					UType = typeof(U);
					VType = typeof(V);
					LastArg = null;
					return null; 
				} 
				set { 
					UType = typeof(U);
					VType = typeof(V);
					LastArg = value;
				}
			}
		}

		// This T is also valid/usable
		class Open1<T>  : NSObject where T : NSObject {}
		class Open2<T>  : NSObject where T : C {}
		class C : NSObject {}

		class ClosedGenericParameter : NSObject {
			// TODO: create test for this once we can call delegates with a null function pointer.
			[Export ("foo:")]
			public void Foo (Action<string> func) {}
		}

		[Register ("GenericTestClass")]
		class GenericTestClass<T> : GenericBaseClass
		{
			public GenericTestClass ()
			{
			}

			[Export ("initWithA:")]
			public GenericTestClass (int a)
			{
			}

			[Export ("initWithB:")]
			public GenericTestClass (long b)
			{
			}

			public override string GetTypeFullName ()
			{
				return typeof (T).FullName;
			}
		}

		class DerivedGenericTestClass<T> : GenericTestClass<T> 
		{
			public override string GetTypeFullName ()
			{
				return base.GetTypeFullName ();
			}
		}

		[Test]
		public void TestRegisteredName ()
		{
			Assert.AreEqual ("MonoTouchFixtures_ObjCRuntime_RegistrarTest_ConstrainedGenericType_1", new Class (typeof(ConstrainedGenericType<>)).Name);
			Assert.AreEqual ("MonoTouchFixtures_ObjCRuntime_RegistrarTest_ConstrainedGenericType_1", new Class (typeof(ConstrainedGenericType<NSSet>)).Name);
			Assert.AreEqual ("MonoTouchFixtures_ObjCRuntime_RegistrarTest_NestedParent_1_Nested", new Class (typeof(NestedParent<NSObject>.Nested)).Name);
			Assert.AreEqual ("UnderlyingEnumValues", new Class (typeof(UnderlyingEnumValues)).Name);
			Assert.AreEqual ("MonoTouchFixtures_ObjCRuntime_RegistrarTest_Nested1_Dummy", new Class (typeof(Nested1.Dummy)).Name);
			Assert.AreEqual ("MonoTouchFixtures_ObjCRuntime_RegistrarTest_C", new Class (typeof (C)).Name);
		}

		void ThrowsICEIfDebug (TestDelegate code, string message, bool execute_release_mode = true)
		{
// The type checks have been disabled for now.
//#if DEBUG
//			Assert.Throws<InvalidCastException> (code, message);
//#else
			if (execute_release_mode)
				Assert.DoesNotThrow (code, message);
//#endif
		}

		[Test]
		public void TestConstrainedGenericType ()
		{
			IntPtr value;

			using (var obj = new ConstrainedGenericType<NSSet> ()) {
				using (var view = new NSSet ()) {
					using (var nsobj = new NSObject ()) {
						// m1
						Messaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("m1:"), IntPtr.Zero);
						Messaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("m1:"), view.Handle);
						ThrowsICEIfDebug (() => Messaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("m1:"), nsobj.Handle), "m1: ICE");

						// m2
						value = IntPtr.Zero;
						void_objc_msgSend_out_IntPtr (obj.Handle, Selector.GetHandle ("m2:"), out value);
						Assert.AreEqual (IntPtr.Zero, value);

						value = view.Handle;
						void_objc_msgSend_out_IntPtr (obj.Handle, Selector.GetHandle ("m2:"), out value);
						Assert.AreEqual (IntPtr.Zero, value);

						value = new IntPtr (0xdeadbeef);
						void_objc_msgSend_out_IntPtr (obj.Handle, Selector.GetHandle ("m2:"), out value);
						Assert.AreEqual (IntPtr.Zero, value);

						// m3
						value = IntPtr.Zero;
						void_objc_msgSend_ref_IntPtr (obj.Handle, Selector.GetHandle ("m3:"), ref value);
						Assert.AreEqual (IntPtr.Zero, value);

						value = view.Handle;
						void_objc_msgSend_ref_IntPtr (obj.Handle, Selector.GetHandle ("m3:"), ref value);
						Assert.AreEqual (view.Handle, value);

						value = nsobj.Handle;
						ThrowsICEIfDebug (() => void_objc_msgSend_ref_IntPtr (obj.Handle, Selector.GetHandle ("m3:"), ref value), "m3 ICE");

						// m4
						Messaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("m4:"), IntPtr.Zero);
						ThrowsICEIfDebug (() => Messaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("m4:"), nsobj.Handle), "m4 ICE", false);
						using (var arr = NSArray.FromNSObjects (nsobj)) {
							ThrowsICEIfDebug (() => Messaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("m4:"), arr.Handle), "m4 ICE 2");
						}

						using (var arr = NSArray.FromNSObjects (view)) {
							Messaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("m4:"), arr.Handle);
						}

						// r1
						Assert.AreEqual (IntPtr.Zero, Messaging.IntPtr_objc_msgSend (obj.Handle, Selector.GetHandle ("r1")));

						// r2
						Assert.AreEqual (IntPtr.Zero, Messaging.IntPtr_objc_msgSend (obj.Handle, Selector.GetHandle ("r2")));

						// p1
						Assert.AreEqual (IntPtr.Zero, Messaging.IntPtr_objc_msgSend (obj.Handle, Selector.GetHandle ("p1")));
						Messaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("setP1:"), IntPtr.Zero);
						Messaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("setP1:"), view.Handle);
						ThrowsICEIfDebug (() => Messaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("setP1:"), nsobj.Handle), "setP1: ICE");

						// p2
						Assert.AreEqual (IntPtr.Zero, Messaging.IntPtr_objc_msgSend (obj.Handle, Selector.GetHandle ("p2")));
						Messaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("setP2:"), IntPtr.Zero);
						ThrowsICEIfDebug (() => Messaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("setP2:"), nsobj.Handle), "setP2: ICE", false);

						using (var arr = NSArray.FromNSObjects (nsobj)) {
							ThrowsICEIfDebug (() => Messaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("setP2:"), arr.Handle), "setP2: ICE2");
						}

						using (var arr = NSArray.FromNSObjects (view)) {
							Messaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("setP2:"), arr.Handle);
						}

					}
				}
			}
		}

#if !__WATCHOS__ && !MONOMAC
		[Test]
		public void TestCopyWithZone ()
		{
			using (var cc = new CopyClass ()) {
				Assert.AreEqual (cc.Handle, Messaging.IntPtr_objc_msgSend_IntPtr (cc.Handle, Selector.GetHandle ("copyWithZone:"), IntPtr.Zero), "a");
				Assert.IsFalse (cc.had_zone.Value, "had_zone");
			}
		}

		// This verifies that a very incorrect implementation of NSTableViewCell isn't shared with UITableViewCell.
		class CopyClass : UITableViewCell, INSCopying {
			public bool? had_zone;

			[Export ("copyWithZone:")]
			public NSObject Copy (NSZone zone)
			{
				had_zone = zone != null;
				DangerousRetain ();
				return this;
			}
		}
#endif // !__WATCHOS__

		[Test]
		public void TestProtocolRegistration ()
		{
			var iProtocol = typeof (IProtocol).FullName.Replace (".", "_").Replace ("+", "_");
			Assert.AreNotEqual (IntPtr.Zero, Runtime.GetProtocol (iProtocol), "IProtocol");
			Assert.IsTrue (Messaging.bool_objc_msgSend_IntPtr (Class.GetHandle (typeof (MyProtocolImplementation)), Selector.GetHandle ("conformsToProtocol:"), Runtime.GetProtocol (iProtocol)), "Interface/IProtocol");
#if !__TVOS__ && !__WATCHOS__ && !MONOMAC
			Assert.IsTrue (Messaging.bool_objc_msgSend_IntPtr (Class.GetHandle (typeof (Test24970)), Selector.GetHandle ("conformsToProtocol:"), Protocol.GetHandle ("UIApplicationDelegate")), "UIApplicationDelegate/17669");
#endif
			// We don't support [Adopts] (yet at least).
//			Assert.IsTrue (Messaging.bool_objc_msgSend_IntPtr (Class.GetHandle (typeof (ConformsToProtocolTestClass)), Selector.GetHandle ("conformsToProtocol:"), Runtime.GetProtocol ("NSCoding")), "Adopts/ConformsToProtocolTestClass");
		}

		[Test]
		public void TestTypeEncodings ()
		{
			var cl = new Class (typeof (TestTypeEncodingsClass));
			var sig = Runtime.GetNSObject<NSMethodSignature> (Messaging.IntPtr_objc_msgSend_IntPtr (cl.Handle, Selector.GetHandle ("methodSignatureForSelector:"), Selector.GetHandle ("foo::::::::::::::::")));
#if MONOMAC
			var boolEncoding = "c";
#else
			var boolEncoding = (IntPtr.Size == 8 || TrampolineTest.IsArmv7k) ? "B" : "c";

#endif
			var exp = new string [] { "@", ":", "^v", "C", "c", "s", "s", "S", "i", "I", "q", "Q", "f", "d", boolEncoding, "@", ":", "#" };

			Assert.AreEqual (exp.Length, sig.NumberOfArguments, "NumberOfArguments");
//			for (uint i = 0; i < exp.Length; i++) {
//				var p = Marshal.PtrToStringAuto (sig.GetArgumentType (i));
//				Console.WriteLine ("{0}: {1}", i, p);
//			}
			for (uint i = 0; i < exp.Length; i++) {
				var p = Marshal.PtrToStringAuto (sig.GetArgumentType (i));
				Assert.AreEqual (exp [i], p, "#{0}", i);
			}
		}

		class TestTypeEncodingsClass : NSObject 
		{
			[Export ("foo::::::::::::::::")]
			public static void Foo (IntPtr p1, byte p2, sbyte p3, char p4, short p5, ushort p6, int p7, uint p8, long p9, ulong p10, float p11, double p12, bool p13, string p14, Selector p15, Class p16)
			{

			}
		}

#if !__TVOS__ // No MapKit in TVOS
#if !__WATCHOS__ // WatchOS has MapKit, but not MKMapView
#if XAMCORE_2_0
		[Test]
		public void TestNativeObjectArray ()
		{
			using (var i1 = new MKPointAnnotation ()) {
				using (var i2 = new MKPointAnnotation ()) {
					using (var array = NSArray.FromObjects (i1, i2)) {
						using (var obj = new NativeObjectArrayType ()) {
							Messaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("addAnnotations:"), array.Handle);
							Assert.AreEqual (2, obj.Annotations.Length, "length");
							Assert.AreSame (i1, obj.Annotations [0], "i1");
							Assert.AreSame (i2, obj.Annotations [1], "i2");
						}
					}
				}
			}

			using (var array = new NSMutableArray ()) {
				using (var i1 = new MKPointAnnotation ()) {
					using (var i2 = new MKPointAnnotation ()) {
						array.Add (i1);
						array.Add (i2);
					}
				}

				using (var obj = new NativeObjectArrayType ()) {
					Messaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("addAnnotations:"), array.Handle);
					Assert.AreEqual (2, obj.Annotations.Length, "length #2");
					Assert.IsNotNull (obj.Annotations [0], "i1 #2");
					Assert.IsNotNull (obj.Annotations [1], "i2 #2");
				}
			}
		}

		public class NativeObjectArrayType : NSObject
		{
			public IMKAnnotation[] Annotations;

			[Export ("addAnnotations:")]
			public void AddAnnotations (params IMKAnnotation[] annotations)
			{
				this.Annotations = annotations;
			}
		}
#endif
#endif // !__WATCHOS__
#endif // !__TVOS__

		class ConstrainedGenericType<T> : NSObject where T: NSObject
		{
			[Export ("m1:")]
			public void M1 (T t) { }

			[Export ("m2:")]
			public void M2 (out T t) { t = null; }

			[Export ("m3:")]
			public void M3 (ref T t) { }

			[Export ("m4:")]
			public void M4 (T[] t) { }

			[Export ("r1")]
			public T R1 () { return null; }

			[Export ("r2")]
			public T[] R2 () { return null; }

			[Export ("p1")]
			public T P1 { get { return null; } set { } }

			[Export ("p2")]
			public T[] P2 { get { return null; } set { } }
		}

		class Generic2<T> : NSObject where T: NSObject
		{
			public string Method;

			[Export ("M1")]
			public T M1 ()
			{
				Method = "M1";
				return null;
			}

			[Export ("M2:")]
			public void M2 (T arg1)
			{
				Method = "M2";
			}
		}

		class NestedParent<T> where T:NSObject {
			public class Nested : NSObject {
				[Export ("foo:")]
				public void Foo (T foo) {
				}
			}
		}


#if !__WATCHOS__ && !MONOMAC
		class CustomView<T> : UIView
		{
			public object TypeName;
			public object TypeT;

			public CustomView (RectangleF rect) : base (rect) {}
			public override void Draw (RectangleF rect)
			{
				TypeT = typeof (T);
				TypeName = GetType ().Name;
			}
		}

		class StringView : CustomView<string> {
			public StringView (RectangleF rect) : base (rect) {}
		}

		class NullableIntView : CustomView<int?> {
			public NullableIntView (RectangleF rect) : base (rect) {}
		}
#endif // !__WATCHOS__

		class GenericConstrainedBase<T> : NSObject where T: NSObject
		{
			public T FooT;
			public string FooType;

			[Export ("foo:")]
			public virtual void Foo (T obj)
			{
				FooT = obj;
				FooType = "Base";
			}
		}

		class GenericConstrainedDerived<T> : GenericConstrainedBase<T> where T:NSObject
		{
			public override void Foo (T obj)
			{
				FooT = obj;
				FooType = "Derived";
			}
		}

		// Due to how registration works, this may throw an exception at startup (if the test fails)
		// which will eventually prevent other unit tests from passing. Unfortunately there is no
		// easy way to tell if *this* class was registered properly (we can check for subsequent classes
		// from the same assembly, but alas 'subsequent' is undefined, since the order classes are
		// registered is not defined). Hopefully an exception message in the application output
		// and some other tests failing will be enough.
		class OutletTestClass : NSObject {
			[Outlet]
			public string Foo { get; set; }
		}

		[Test]
		public void GenericVirtualTest ()
		{
			using (var obj = new GenericConstrainedDerived<NSObject> ()) {
				Messaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("foo:"), obj.Handle);
				Assert.AreEqual ("Derived", obj.FooType, "Derived");
				Assert.AreSame (obj, obj.FooT, "obj");
			}
		}

		class AnyT<T> : NSObject {
		}

		[Test]
		public void ConformsToProtocolTest ()
		{
			using (var obj = new AnyT<object> ()) {
				Messaging.bool_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("conformsToProtocol:"), Runtime.GetProtocol ("NSObject"));
			}
		}

		[Test]
		public void ConformsToProtocolTest2 ()
		{
			using (var obj = new ConformsToProtocolTestClass<NSFileHandle> ()) {
				Assert.IsTrue (Messaging.bool_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("conformsToProtocol:"), Runtime.GetProtocol ("NSCoding")));
				Assert.IsFalse (Messaging.bool_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("conformsToProtocol:"), Runtime.GetProtocol ("NSCopying")));
			}

			using (var obj = new ConformsToProtocolTestClass ()) {
				Assert.IsTrue (Messaging.bool_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("conformsToProtocol:"), Runtime.GetProtocol ("NSCoding")));
				Assert.IsFalse (Messaging.bool_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("conformsToProtocol:"), Runtime.GetProtocol ("NSCopying")));
			}
		}

		[Adopts ("NSCoding")]
		public class ConformsToProtocolTestClass : NSObject {
		}

		[Adopts ("NSCoding")]
		public class ConformsToProtocolTestClass<T> : NSObject where T: NSObject {
		}

		[Register ("UnderlyingEnumValues")]
		class UnderlyingEnumValues : NSObject 
		{
			enum B : byte { a };
			enum SB : sbyte { a };
			enum S : short { a };
			enum US : ushort { a };
			enum I : int { a };
			enum UI : uint { a };
			enum L : long { a };
			enum UL : ulong { a };

			[Export ("Foo:a:b:c:d:e:f:g:h")]
			void Foo (B b, SB sb, S s, US us, I i, UI ui, L l, UL ul)
			{
			}
		}

		// It should be possible to export two identically named nested types.
		// Note that this is will fail during build (static registrar) or startup (dynamic registrar) (if it's broken of course).
		class Nested1 {
			public class Dummy : NSObject { }
		}
		class Nested2 {
			class Dummy : NSObject { }
		}
#endregion

		[Protocol]
		interface IProtocol {
			[Export ("foo")]
			[Preserve (Conditional = true)]
			int Foo ();

			[Preserve (Conditional = true)]
			int Bar { [Export ("bar")] get; [Export ("setBar:")] set; }

			[Preserve]
			[Export ("block", ArgumentSemantic.Retain)]
			Action Block { get; set; }
		}

		class MyProtocolImplementation : NSObject, IProtocol {
			public int Foo ()
			{
				return 31415;
			}

			public int Bar {
				get { return 31415926; }
				set { }
			}

			public Action Block {
				get { return null; }
				set { }
			}
		}

		[Test]
		public void IProtocolTest ()
		{
			var o = new MyProtocolImplementation ();
			Assert.AreEqual (31415, Messaging.int_objc_msgSend (o.Handle, Selector.GetHandle ("foo")), "#method");
			Assert.AreEqual (31415926, Messaging.int_objc_msgSend (o.Handle, Selector.GetHandle ("bar")), "#getter");
			Assert.DoesNotThrow (() => { Messaging.void_objc_msgSend_int (o.Handle, Selector.GetHandle ("setBar:"), 2); }, "#setter");
		}


#if !MONOMAC
		[Test]
		public void FakeTypeTest ()
		{
			IntPtr obj2_ptr = IntPtr.Zero;

			try {
				using (var obj1 = new FakeType1 ()) {
					var cls = Class.GetHandle ("FakeType2");
					obj2_ptr = Messaging.IntPtr_objc_msgSend (Class.GetHandle ("FakeType2"), Selector.GetHandle ("alloc"));
					obj2_ptr = Messaging.IntPtr_objc_msgSend (obj2_ptr, Selector.GetHandle ("init"));
					Assert.AreNotEqual (IntPtr.Zero, obj2_ptr, "not zero");
					Messaging.bool_objc_msgSend_IntPtr (obj1.Handle, Selector.GetHandle ("fakeTypeTest:"), obj2_ptr);
				}
			} finally {
				Messaging.void_objc_msgSend (obj2_ptr, Selector.GetHandle ("release"));
			}
		}

		[Register ("FakeType1")]
		class FakeType1 : NSObject {
			public FakeType1 (IntPtr ptr)
				: base (ptr)
			{
			}

			public FakeType1 ()
			{
			}

			[Export ("fakeTypeTest:")]
			public bool FakeTypeTest (FakeType1 ft)
			{
				var cls = new Class (Messaging.IntPtr_objc_msgSend (ft.Handle, Selector.GetHandle ("class")));
				Assert.AreEqual ("FakeType2", cls.Name);
				return true;
			}
		}

		// There's a FakeType2 class defined in libtest.m
		// It's defined there in order to be able to create
		// an instance of a native class without a managed
		// peer.

		public void Test_D ()
		{
			using (var tc = new ObjCRegistrarTest ()) {
				Assert.AreEqual (tc.Pd1, 0, "Pd1");
				Assert.AreEqual (0, tc.D (), "1");
				tc.Pd1 = 1.2;
				Assert.AreEqual (1.2, tc.D (), "2");
				Assert.AreEqual (tc.Pd1, 1.2, "Pd1");
			}
		}

		public class TestClass : ObjCRegistrarTest
		{
		}

		[Test]
		public void IdAsIntPtrTest ()
		{
			using (var obj = new IdAsIntPtrClass ()) {
				Messaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("idAsIntPtr:"), IntPtr.Zero);
			}
		}

		public class IdAsIntPtrClass : ObjCProtocolTest
		{
			[Export ("idAsIntPtr:")]
			public new void IdAsIntPtr (IntPtr id)
			{
				Assert.AreEqual (IntPtr.Zero, id, "Zero");
			}
		}

		[Test]
		public void OutNSErrorOnStack1 ()
		{
			using (var obj = new OutNSErrorOnStackClass ()) {
				Messaging.void_objc_msgSend_int_int_int_int_int_int_IntPtr (obj.Handle, Selector.GetHandle ("outNSErrorOnStack:i:i:i:i:i:err:"), 0, 0, 0, 0, 0, 0, IntPtr.Zero);

				var ptr = Marshal.AllocHGlobal (IntPtr.Size);
				Marshal.WriteIntPtr (ptr, IntPtr.Zero);
				Console.WriteLine ("ptr: 0x{0} = 0x{1}", ptr.ToString ("x"), Marshal.ReadIntPtr (ptr));
				Messaging.void_objc_msgSend_int_int_int_int_int_int_IntPtr (obj.Handle, Selector.GetHandle ("outNSErrorOnStack:i:i:i:i:i:err:"), 0, 0, 0, 0, 0, 0, ptr);
				Assert.AreEqual (IntPtr.Zero, Marshal.ReadIntPtr (ptr), "#1");
				Marshal.FreeHGlobal (ptr);

				ptr = IntPtr.Zero;
				unsafe {
					IntPtr* ptrFixed = &ptr;
					Console.WriteLine ("ptr: 0x{0}", ptr.ToString ("x"));
					Messaging.void_objc_msgSend_int_int_int_int_int_int_IntPtr (obj.Handle, Selector.GetHandle ("outNSErrorOnStack:i:i:i:i:i:err:"), 0, 0, 0, 0, 0, 0, (IntPtr) ptrFixed);
					Assert.AreEqual (IntPtr.Zero, ptr, "#2");
				}
			}
		}

		[Test]
		public void OutNSErrorOnStack2 ()
		{
			using (var obj = new OutNSErrorOnStackClass ()) {
				Messaging.void_objc_msgSend_IntPtr_IntPtr_IntPtr_long_int_IntPtr (obj.Handle, Selector.GetHandle ("outNSErrorOnStack:obj:obj:int64:i:err:"), IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, 1, 2, IntPtr.Zero);

				var ptr = Marshal.AllocHGlobal (IntPtr.Size);
				Marshal.WriteIntPtr (ptr, IntPtr.Zero);
				Console.WriteLine ("ptr: 0x{0} = 0x{1}", ptr.ToString ("x"), Marshal.ReadIntPtr (ptr));
				Messaging.void_objc_msgSend_IntPtr_IntPtr_IntPtr_long_int_IntPtr (obj.Handle, Selector.GetHandle ("outNSErrorOnStack:obj:obj:int64:i:err:"), IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, 1, 2, ptr);
				Assert.AreEqual (IntPtr.Zero, Marshal.ReadIntPtr (ptr), "#1");
				Marshal.FreeHGlobal (ptr);

				ptr = IntPtr.Zero;
				unsafe {
					IntPtr* ptrFixed = &ptr;
					Console.WriteLine ("ptr: 0x{0}", ptr.ToString ("x"));
					Messaging.void_objc_msgSend_IntPtr_IntPtr_IntPtr_long_int_IntPtr (obj.Handle, Selector.GetHandle ("outNSErrorOnStack:obj:obj:int64:i:err:"), IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, 1, 2, (IntPtr) ptrFixed);
					Assert.AreEqual (IntPtr.Zero, ptr, "#2");
				}
			}
		}

		public class OutNSErrorOnStackClass : ObjCRegistrarTest
		{
			public override void OutNSErrorOnStack (int i1, int i2, int i3, int i4, int i5, int i6, out NSError error)
			{
				error = null;
			}

			public override void OutNSErrorOnStack (NSObject i1, NSObject i2, NSObject i3, long i4, int i5, out NSError error)
			{
				Assert.AreEqual (i4, 1, "#long");
				Assert.AreEqual (i5, 2, "#int");
				error = null;
			}
		}

#endif

#if !__TVOS__ && !__WATCHOS__ && !MONOMAC
		class Test24970 : UIApplicationDelegate {
			// This method uses the [Transient] attribute.
			public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations (UIApplication application, UIWindow forWindow)
			{
				throw new NotImplementedException ();
			}
		}

		[Test]
		public void CustomAppDelegatePerformFetchTest ()
		{
			if (!Runtime.DynamicRegistrationSupported)
				Assert.Ignore ("This test requires the dynamic registrar to be available.");

			using (var obj = new CustomApplicationDelegate ()) {
				BlockLiteral block = new BlockLiteral ();
				var performed = false;
				Action<UIBackgroundFetchResult> del = (v) => {
					performed = true;
				};

				block.SetupBlock (CustomApplicationDelegate.SDActionArity1V42.Handler, del);

				Messaging.void_objc_msgSend_IntPtr_ref_BlockLiteral (obj.Handle, Selector.GetHandle ("application:performFetchWithCompletionHandler:"), UIApplication.SharedApplication.Handle, ref block);

				block.CleanupBlock ();

				Assert.IsTrue (performed);
			}
		}

		class CustomApplicationDelegate : UIApplicationDelegate {
			[Export ("application:performFetchWithCompletionHandler:")]
			public new void PerformFetch (UIApplication application, [BlockProxy (typeof (NIDActionArity1V42))] Action<UIBackgroundFetchResult> completionHandler)
			{
				completionHandler (UIBackgroundFetchResult.NoData);
			}

			[UnmanagedFunctionPointerAttribute (CallingConvention.Cdecl)]
			internal delegate void DActionArity1V42 (IntPtr block, nuint obj);

			internal static class SDActionArity1V42 {
				static internal readonly DActionArity1V42 Handler = Invoke;

				[MonoPInvokeCallback (typeof (DActionArity1V42))]
				static unsafe void Invoke (IntPtr block, nuint obj) {
					var descriptor = (BlockLiteral *) block;
					var del = (global::System.Action<UIBackgroundFetchResult>) (descriptor->Target);
					if (del != null)
						del ((UIBackgroundFetchResult) (global::System.UInt64) obj);
				}
			} /* class SDActionArity1V42 */

			internal class NIDActionArity1V42 {
				IntPtr blockPtr;
				DActionArity1V42 invoker;

				[Preserve (Conditional=true)]
				public unsafe NIDActionArity1V42 (BlockLiteral *block)
				{
					blockPtr = _Block_copy ((IntPtr) block);
					invoker = block->GetDelegateForBlock<DActionArity1V42> ();
				}

				[Preserve (Conditional=true)]
				~NIDActionArity1V42 ()
				{
					_Block_release (blockPtr);
				}

				[Preserve (Conditional=true)]
				public unsafe static Action<UIBackgroundFetchResult> Create (IntPtr block)
				{
					return new NIDActionArity1V42 ((BlockLiteral *) block).Invoke;
				}

				[Preserve (Conditional=true)]
				unsafe void Invoke (UIBackgroundFetchResult obj)
				{
					invoker (blockPtr, (nuint) (UInt64) obj);
				}

				[DllImport ("/usr/lib/libobjc.dylib")]
				static extern IntPtr _Block_copy (IntPtr ptr);

				[DllImport ("/usr/lib/libobjc.dylib")]
				static extern void _Block_release (IntPtr ptr);
			}

		}
#endif // !__TVOS__ && !__WATCHOS__
		
		[Protocol]
		[Model]
		class Test25781 : NSObject { }
		class Test25781D : Test25781 {}

		[Test]
		public void TestProtocolAndRegister ()
		{
			// Having [Protocol] [Register] (and nothing else), doesn't make any sense.
			// Yet we've created these in btouch, so we need to define what they
			// actually do (nothing at all).

			Assert.AreEqual (IntPtr.Zero, Class.GetHandle ("TestProtocolRegister"));

			// However deriving from those nonsensical classes must do something
			// (at the very least because anything else would be a breaking change).
			Assert.AreNotEqual (IntPtr.Zero, Class.GetHandle ("DerivedTestProtocolRegister"));
		}

		[Protocol]
		[Register ("TestProtocolRegister")]
		class TestProtocolRegister : NSObject {}

		[Register ("DerivedTestProtocolRegister")]
		class DerivedTestProtocolRegister : TestProtocolRegister {}

		class D1 : NSObject {
			public string ctor1;

			[Export ("initWithFoo:")]
			public D1 (int foo)
			{
				ctor1 = "foo";
			}

			[Export ("initWithBar:")]
			public D1 (long bar)
			{
				ctor1 = "bar";
			}
		}

		class D2 : D1 {
			public readonly int Value = 3;
			public string ctor2;

			[Export ("initWithFoo:")]
			public D2 (int foo) : base (foo)
			{
				ctor2 = "foo";
			}
		}

		class E1 : NSObject {
			protected E1 (IntPtr ptr) : base (ptr)
			{
			}

			[Export ("initWithFoo:")]
			public E1 (int foo)
			{
			}

			[Export ("initWithBar:")]
			public E1 (long bar)
			{
			}
		}

		class E2 : E1 {
			public readonly int Value = 3;

			protected E2 (IntPtr ptr) : base (ptr)
			{
			}

			[Export ("initWithFoo:")]
			public E2 (int foo) : base (foo)
			{
			}

			[Export ("M1:")]
			public int M1 (int v)
			{
				return v;
			}
		}

		class G1<T> : NSObject {
			protected G1 (IntPtr ptr) : base (ptr)
			{
			}

			[Export ("M1:")]
			public int M1 (int v)
			{
				return v;
			}
		}

		class G2 : G1<int> {
			public readonly int Value = 3;

			protected G2 (IntPtr ptr) : base (ptr)
			{
			}

			[Export ("M2:")]
			public int M2 (int v)
			{
				return v;
			}
		}

		[Test]
		public void BlockCollection ()
		{
			Exception ex = null;
			int initialFreedCount = ObjCBlockTester.FreedBlockCount;
			var thread = new Thread (() => {
				try {
					using (var obj = new Xamarin.BindingTests.RegistrarBindingTest.BlockCallbackTester ()) {
						for (int i = 0; i < 10000; i++)
							obj.TestFreedBlocks ();
					}
				} catch (Exception e) {
					ex = e;
				}
			});
			thread.Start ();
			thread.Join ();
			GC.Collect ();
			GC.WaitForPendingFinalizers ();
			TestRuntime.RunAsync (DateTime.Now.AddSeconds (30), () => { }, () => ObjCBlockTester.FreedBlockCount > initialFreedCount);
			Assert.IsNull (ex, "No exceptions");
			Assert.That (ObjCBlockTester.FreedBlockCount, Is.GreaterThan (initialFreedCount), "freed blocks");
		}

		[Test]
		public void TestCtors ()
		{
			IntPtr ptr = IntPtr.Zero;

			try {
				// ctor inheritance: derived + base class ctors are called.
				ptr = Messaging.IntPtr_objc_msgSend (Class.GetHandle (typeof (D2)), Selector.GetHandle ("alloc"));
				ptr = Messaging.IntPtr_objc_msgSend_int (ptr, Selector.GetHandle ("initWithFoo:"), 1);
				var obj = Runtime.GetNSObject<D2> (ptr);
				Assert.AreEqual (3, obj.Value, "a");
				Assert.AreEqual ("foo", obj.ctor1, "a ctor1");
				Assert.AreEqual ("foo", obj.ctor2, "a ctor2");
			} finally {
				Messaging.void_objc_msgSend (ptr, Selector.GetHandle ("release"));
			}

			try {
				// ctor inheritance: initWithBar: is only defined in the base class (managed base class)
				// In this case we create a managed wrapper of the base class, not the derived class. I'm not
				// sure this is by design or by accident, but that's how we're behaving right now at least
				ptr = Messaging.IntPtr_objc_msgSend (Class.GetHandle (typeof (D2)), Selector.GetHandle ("alloc"));
				ptr = Messaging.IntPtr_objc_msgSend_long (ptr, Selector.GetHandle ("initWithBar:"), 2);
				// Unable to cast object of type 'AppDelegate+D1' to type 'AppDelegate+D2'
				Assert.Throws<InvalidCastException> (() => Runtime.GetNSObject<D2> (ptr), "b ex");
				var obj = Runtime.GetNSObject<D1> (ptr);
				Assert.AreEqual ("bar", obj.ctor1, "b ctor1");
			} finally {
				Messaging.void_objc_msgSend (ptr, Selector.GetHandle ("release"));
			}

			try {
				// ctor inheritance: init is defined in the native base class.
				// Since no managed ctor apply, we need the (IntPtr) ctor (in this case there isn't one)
				ptr = Messaging.IntPtr_objc_msgSend (Class.GetHandle (typeof (D2)), Selector.GetHandle ("alloc"));
				ptr = Messaging.IntPtr_objc_msgSend (ptr, Selector.GetHandle ("init"));
				// Failed to marshal the Objective-C object 0x7adf5920 (type: AppDelegate_D2). Could not find an existing managed instance for this object, nor was it possible to create a new managed instance (because the type 'AppDelegate+D2' does not have a constructor that takes one IntPtr argument).
				Assert.Throws<RuntimeException> (() => Runtime.GetNSObject<D2> (ptr), "c");
			} finally {
				Messaging.void_objc_msgSend (ptr, Selector.GetHandle ("release"));
			}

			try {
				// ctor inheritance: init is defined in the native base class.
				// Since no managed ctor apply, we need the (IntPtr) ctor (which is provided in this case)
				ptr = Messaging.IntPtr_objc_msgSend (Class.GetHandle (typeof (E2)), Selector.GetHandle ("alloc"));
				ptr = Messaging.IntPtr_objc_msgSend (ptr, Selector.GetHandle ("init"));
				var obj = Runtime.GetNSObject<E2> (ptr);
				Assert.AreEqual (3, obj.Value, "d");
			} finally {
				Messaging.void_objc_msgSend (ptr, Selector.GetHandle ("release"));
			}

			try {
				// ctor inheritance: only applicable ctor is the native one.
				// No managed ctor is called (and no wrapper is created) when the native object is created.
				// Instead the managed wrapper is created (and managed (IntPtr) ctor called) when
				// we first need it.
				ptr = Messaging.IntPtr_objc_msgSend (Class.GetHandle (typeof (E2)), Selector.GetHandle ("alloc"));
				ptr = Messaging.IntPtr_objc_msgSend (ptr, Selector.GetHandle ("init"));
				Assert.IsNull (Runtime.TryGetNSObject (ptr), "e null");
				int rv = Messaging.int_objc_msgSend_int (ptr, Selector.GetHandle ("M1:"), 31415);
				Assert.IsNotNull (Runtime.TryGetNSObject (ptr), "e not null");
				Assert.AreEqual (31415, rv, "e1");
				var obj = Runtime.GetNSObject<E2> (ptr);
				Assert.AreEqual (3, obj.Value, "e2");
			} finally {
				Messaging.void_objc_msgSend (ptr, Selector.GetHandle ("release"));
			}

			try {
				// ctor inheritance: only applicable ctor is the native one.
				// No managed ctor is called (and no wrapper is created) when the native object is created.
				// Instead the managed wrapper is created (and managed (IntPtr) ctor called) when
				// we first need it.
				// In this case the invoked managed method (which creates the wrapper) is defined
				// in a subclass of a generic type.
				ptr = Messaging.IntPtr_objc_msgSend (Class.GetHandle (typeof (G2)), Selector.GetHandle ("alloc"));
				ptr = Messaging.IntPtr_objc_msgSend (ptr, Selector.GetHandle ("init"));
				Assert.IsNull (Runtime.TryGetNSObject (ptr), "f null");
				int rv = Messaging.int_objc_msgSend_int (ptr, Selector.GetHandle ("M1:"), 31415);
				Assert.IsNotNull (Runtime.TryGetNSObject (ptr), "f not null");
				Assert.AreEqual (31415, rv, "f1");
				var obj = Runtime.GetNSObject<G2> (ptr);
				Assert.AreEqual (3, obj.Value, "f2");
			} finally {
				Messaging.void_objc_msgSend (ptr, Selector.GetHandle ("release"));
			}

			try {
				// ctor inheritance: only applicable ctor is the native one.
				// No managed ctor is called (and no wrapper is created) when the native object is created.
				// Instead the managed wrapper is created (and managed (IntPtr) ctor called) when
				// we first need it.
				// In this case the invoked managed method (which creates the wrapper) is defined
				// in a generic type.
				ptr = Messaging.IntPtr_objc_msgSend (Class.GetHandle (typeof (G2)), Selector.GetHandle ("alloc"));
				ptr = Messaging.IntPtr_objc_msgSend (ptr, Selector.GetHandle ("init"));
				Assert.IsNull (Runtime.TryGetNSObject (ptr), "g null");
				int rv = Messaging.int_objc_msgSend_int (ptr, Selector.GetHandle ("M2:"), 31415);
				Assert.IsNotNull (Runtime.TryGetNSObject (ptr), "g not null");
				Assert.AreEqual (31415, rv, "g1");
				var obj = Runtime.GetNSObject<G2> (ptr);
				Assert.AreEqual (3, obj.Value, "g2");
			} finally {
				Messaging.void_objc_msgSend (ptr, Selector.GetHandle ("release"));
			}
		}

#if !__WATCHOS__ && !MONOMAC
		class Bug28757A : NSObject, IUITableViewDataSource
		{
			public virtual nint RowsInSection (UITableView tableView, nint section)
			{
				return 1;
			}
			public UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				return null;
			}
		}

		class Bug28757B : Bug28757A
		{
			public override nint RowsInSection (UITableView tableView, nint section)
			{
				return 2;
			}
		}
#endif // !__WATCHOS__

#if !__WATCHOS__ && !MONOMAC
		[Test]
		public void TestInheritedProtocols ()
		{
			using (var obj = new Bug28757B ()) {
				Assert.AreEqual (2, Messaging.nint_objc_msgSend_IntPtr_nint (obj.Handle, Selector.GetHandle ("tableView:numberOfRowsInSection:"), IntPtr.Zero, 0), "#test");
			}
		}
#endif // !__WATCHOS

#if !__WATCHOS__ && !MONOMAC
		[Test]
		public void InOutProtocolMethodArgument ()
		{
			using (var obj = new Scroller ()) {
				var velocity = new PointF (1, 2);
				var targetContentOffset = new PointF (3, 4);
				Messaging.void_objc_msgSend_IntPtr_PointF_ref_PointF (obj.Handle, Selector.GetHandle ("scrollViewWillEndDragging:withVelocity:targetContentOffset:"), IntPtr.Zero, velocity, ref targetContentOffset);
				Console.WriteLine (targetContentOffset);
				Assert.AreEqual ("{X=123, Y=345}", targetContentOffset.ToString (), "ref output");
			}
		}
#endif // !__WATCHOS

#if !__WATCHOS__ && !MONOMAC
		class Scroller : NSObject, IUIScrollViewDelegate
		{
			[Export ("scrollViewWillEndDragging:withVelocity:targetContentOffset:")]
			public void WillEndDragging (UIScrollView scrollView, PointF velocity, ref PointF targetContentOffset)
			{
				Assert.AreEqual ("{X=1, Y=2}", velocity.ToString (), "velocity");
				Assert.AreEqual ("{X=3, Y=4}", targetContentOffset.ToString (), "targetContentOffset");
				targetContentOffset = new PointF (123, 345);
			}
		}
#endif // !__WATCHOS__

#if !__TVOS__ && !__WATCHOS__ && !MONOMAC// No ABPeoplePickerNavigationControllerDelegate
		[Test]
		public void VoidPtrToINativeObjectArgument ()
		{
			using (var obj = new ABPeoplePickerNavigationControllerDelegateImpl ()) {
				using (var person = new ABPerson ()) {
					Messaging.void_objc_msgSend_IntPtr_IntPtr (obj.Handle, Selector.GetHandle ("peoplePickerNavigationController:didSelectPerson:"), IntPtr.Zero, person.Handle);
					Assert.AreEqual (person.Handle, obj.personHandle, "1");
				}
			}
		}

		class ABPeoplePickerNavigationControllerDelegateImpl : ABPeoplePickerNavigationControllerDelegate
		{
			public IntPtr personHandle;
			public override void DidSelectPerson (ABPeoplePickerNavigationController peoplePicker, ABPerson selectedPerson)
			{
				personHandle = selectedPerson.Handle;
			}
		}
#endif // !__TVOS__

#if !__TVOS__ // No Contacts framework in TVOS
#if XAMCORE_2_0 // The Contacts framework is Unified only
		[Test]
		public void GenericAPI ()
		{
			TestRuntime.AssertXcodeVersion (7, 0);

			using (var contact = new CNMutableContact ()) {
				var dt = new NSDateComponents () {
					Year = 1923,
				};
				var handle = Messaging.IntPtr_objc_msgSend_IntPtr_IntPtr (Class.GetHandle (typeof (CNLabeledValue<>)), Selector.GetHandle ("labeledValueWithLabel:value:"), IntPtr.Zero, dt.Handle);
				var array = Messaging.IntPtr_objc_msgSend_IntPtr (Class.GetHandle (typeof(NSArray)), Selector.GetHandle ("arrayWithObject:"), handle);
				Messaging.void_objc_msgSend_IntPtr (contact.Handle, Selector.GetHandle ("setDates:"), array);

				Assert.AreEqual (1923, contact.Dates [0].Value.Year, "Dates");
			}

			using (var contact = new SubclassedContact ()) {
				var dates = Messaging.IntPtr_objc_msgSend (contact.Handle, Selector.GetHandle ("dates"));
				var obj = Runtime.GetNSObject (dates);
				Assert.AreEqual (typeof (NSArray), obj.GetType (), "2 date type");
				var arr = (NSArray) obj;
				Assert.AreEqual (1, arr.Count, "2 count");
			}

			using (var contact = new SubclassedContact ()) {
				var dates = Messaging.IntPtr_objc_msgSend (contact.Handle, Selector.GetHandle ("dates"));
				var arr = NSArray.ArrayFromHandle<CNLabeledValue<NSDateComponents>> (dates);
				Assert.AreEqual (1, arr.Length, "3 length");
			}
		}

		class SubclassedContact : CNContact {
			public override CNLabeledValue<NSDateComponents> [] Dates {
				get {
					return new CNLabeledValue<NSDateComponents> [] {
						new CNLabeledValue<NSDateComponents> ("label", new NSDateComponents ()
						{
							Day = 24,
						})
					};
				}
			}
		}
#endif // XAMCORE_2_0
#endif // !__TVOS__

		[Test]
		public void Bug34224 ()
		{
			using (var obj = new Bug34224Class ()) {
				IntPtr ptr = new IntPtr (123);
				Messaging.void_objc_msgSend_ref_IntPtr (obj.Handle, Selector.GetHandle ("ref:"), ref ptr);
				Assert.AreEqual (new IntPtr (456), ptr, "# ref");

				Messaging.void_objc_msgSend_out_IntPtr (obj.Handle, Selector.GetHandle ("out:"), out ptr);
				Assert.AreEqual (new IntPtr (567), ptr, "# out");
			}
		}

		// Bug 34224
		class Bug34224Class : NSObject {
			[Export ("ref:")]
			public void Ref (ref IntPtr p1)
			{
				Assert.AreEqual (new IntPtr (123), p1, "ref C");
				p1 = new IntPtr (456);
			}

			[Export ("out:")]
			public void Out (out IntPtr p1)
			{
				p1 = new IntPtr (567);
			}
		}

		class Bug34440Class : NSObject {
			[Export ("bug34440")]
			Selector Bug34440 ()
			{
				return new Selector ("bug34440");
			}

			[Export ("classReturn")]
			Class ClassReturn ()
			{
				return new Class (typeof (Bug34440Class));
			}
		}

		[Test]
		public void SelectorReturnValue ()
		{
			using (var obj = new Bug34440Class ()) {
				var ptr = Messaging.IntPtr_objc_msgSend (obj.Handle, Selector.GetHandle ("bug34440"));
				Assert.AreEqual (Selector.GetHandle ("bug34440"), ptr, "selector");
				ptr = Messaging.IntPtr_objc_msgSend (obj.Handle, Selector.GetHandle ("classReturn"));
				Assert.AreEqual (Class.GetHandle (typeof (Bug34440Class)), ptr, "class");
			}
		}

		[Test]
		public void BlockReturnTest ()
		{
			using (var obj = new BlockReturnTestClass ()) {
				Assert.IsTrue (obj.TestBlocks (), "TestBlocks");
			}
		}

		class BlockReturnTestClass : ObjCRegistrarTest {
			public override RegistrarTestBlock MethodReturningBlock ()
			{
				return v => {
					Assert.AreEqual (0xdeadf00d, v, "input");
					return 0x1337b001;
				};
			}

			public override RegistrarTestBlock PropertyReturningBlock {
				get {
					return v => {
						Assert.AreEqual (0xdeadf11d, v, "input");
						return 0x7b001133;
					};
				}
			}
		}

		[Test]
		public void PropertySetters ()
		{
			var cls = Class.GetHandle (typeof (PropertySetterTestClass));
			Assert.AreNotEqual (IntPtr.Zero, class_getInstanceMethod (cls, Selector.GetHandle ("setá:")), "a 1");
			using (var obj = new PropertySetterTestClass ()) {
				obj.SetValueForKey (new NSString ("AAA"), (NSString) "á");
				Assert.AreEqual ("AAA", (string) (NSString) obj.ValueForKey ((NSString) "á"), "A getvalue");
				Assert.AreEqual ("AAA", obj.A, "A setvalue");

				obj.SetValueForKey (new NSString ("BBB"), (NSString) "b");
				Assert.AreEqual ("BBB", (string) (NSString) obj.ValueForKey ((NSString) "b"), "B getvalue");
				Assert.AreEqual ("BBB", obj.B, "B setvalue");
			}
		}

		class PropertySetterTestClass : NSObject {
			[Export ("á")]
			public string A { get; set; }

			[Export ("b")]
			public string B { get; set; }
		}

		[Test]
		public void ConstructorChaining ()
		{
			using (var obj = new CtorChaining2 (2)) {
				Assert.IsTrue (obj.InitCalled, "Init called");
				Assert.IsTrue (obj.InitCallsInitCalled, "InitCallsInit called");
			}
		}

		class CtorChaining2 : CtorChaining1
		{
			public CtorChaining2 ()
			{
			}

			public CtorChaining2 (int value)
				: base (value)
			{
			}
		}

		[DllImport ("/usr/lib/libobjc.dylib")]
		static extern IntPtr class_getInstanceMethod (IntPtr cls, IntPtr sel);

#if !MONOMAC // Registrar_OutExportDerivedClass is from fsharp tests
		[Test]
		public void OutOverriddenWithoutOutAttribute ()
		{
			using (var tmp = new NSObject ()) {
				using (var obj = new Registrar_OutExportDerivedClass ()) {
					IntPtr tmpH = tmp.Handle;
					var rv = Messaging.IntPtr_objc_msgSend_ref_IntPtr (obj.Handle, Selector.GetHandle ("func:"), ref tmpH);
					Assert.AreEqual (IntPtr.Zero, tmpH, "input");
					Assert.AreEqual (IntPtr.Zero, rv, "output");
				}
			}
		}
#endif

		class ProtocolArgumentClass : NSObject
		{
			[Export("someMethod:")]
			IntPtr SomeMethod (Protocol protocol)
			{
				return protocol.Handle;
			}
		}

		[Test]
		public void ProtocolArgument ()
		{
			using (var obj = new ProtocolArgumentClass ()) {
				var nsobjProtocol = Protocol.GetHandle ("NSObject");
				var ptr = Messaging.IntPtr_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("someMethod:"), nsobjProtocol);
				Assert.AreEqual (nsobjProtocol, ptr, "result");
				Assert.AreNotEqual (IntPtr.Zero, ptr, "nsobject");
			}
		}

		class Bug41319 : NSObject
		{
			[Export ("initWithCoder:")]
			public Bug41319 (NSCoder coder)
			{
			}
		}

#if !XAMCORE_2_0
		class Bug42454 : NSUrlProtocol
		{
			[Export ("initWithRequest:cachedResponse:client:")]
			public Bug42454 (NSUrlRequest request, NSCachedUrlResponse response, NSUrlProtocolClient client)
			{
				throw new NotImplementedException ();
			}
		}
#endif

#if debug_code
		static void DumpClass (Type type)
		{
			var cls = Class.GetHandle (type);
			Console.WriteLine ("Type: {0} => Class: {1}", type.FullName, class_getName (cls));

			uint count;
			var methods = class_copyMethodList (cls, out count);
			Console.WriteLine ("    {0} methods.", count);
			for (uint i = 0; i < count; i++) {
				var method = Marshal.ReadIntPtr (methods, (int) (IntPtr.Size * i));
				Console.WriteLine ("    #{0}: Name: {1} Type Encoding: {2}", i + 1, Selector.FromHandle (method_getName (method)).Name, method_getTypeEncoding (method));
			}
		}

		[DllImport ("/usr/lib/libobjc.dylib")]
		static extern IntPtr class_copyMethodList (IntPtr cls, out uint outCount);

		[DllImport ("/usr/lib/libobjc.dylib", EntryPoint = "class_getName")]
		static extern IntPtr _class_getName (IntPtr cls);

		static string class_getName (IntPtr cls)
		{
			return Marshal.PtrToStringAnsi (_class_getName (cls));
		}

		[DllImport ("/usr/lib/libobjc.dylib", EntryPoint = "method_getTypeEncoding")]
		static extern IntPtr _method_getTypeEncoding (IntPtr method);

		static string method_getTypeEncoding (IntPtr method)
		{
			return Marshal.PtrToStringAnsi (_method_getTypeEncoding (method));
		}

		[DllImport ("/usr/lib/libobjc.dylib")]
		static extern IntPtr method_getName (IntPtr method);
#endif

		[Test]
		public void TestConstrainedGenericType2 ()
		{
			TestRuntime.AssertXcodeVersion (8, 0);
			using (var m = new NSMeasurement<NSUnitTemperature> (2, NSUnitTemperature.Fahrenheit)) {
			}
		}

		class NullOutParameters : ObjCRegistrarTest
		{
			public override void V (out NSObject n1, out NSString n2)
			{
				n1 = null;
				n2 = null;
			}
		}

		[Test]
		public void TestNullOutParameters ()
		{
			using (var obj = new NullOutParameters ())
				obj.Invoke_V_null_out ();
		}

		[Test]
		public unsafe void ByrefParameter ()
		{
			using (var obj = new ByrefParameterTest ()) {
				using (var param = new NSObject ()) {
					// We want an instance of an NSObject subclass that doesn't have a managed wrapper, so we create a native NSString handle.
					IntPtr handle = NSString.CreateNative ("ByrefParameter");
					Messaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("doSomething:"), new IntPtr (&handle));
					NSString.ReleaseNative (handle);
				}
			}
		}

		class ByrefParameterTest : NSObject {
			[Export ("doSomething:")]
			public void DoSomething (ref NSString str)
			{
				Assert.IsNotNull (str, "NonNull NSString&");
				Assert.AreEqual ("ByrefParameter", str.ToString ());
			}
		}
	}

#if !__WATCHOS__
	[Category (typeof (CALayer))]
	static class CALayerColorsHelpers
	{
		[Export ("setBorderUIColor:")]
		static void BorderUIColor (this CALayer self, UIColor borderColor)
		{
			self.BorderColor = borderColor.CGColor;
		}
	}
#endif // !__WATCHOS__


	[TestFixture]
	[Preserve (AllMembers = true)]
	public class BlockSignatureTest
	{
		[StructLayout (LayoutKind.Sequential)]
		struct BlockDescriptor2
		{
			public IntPtr reserved;
			public IntPtr size;
			public IntPtr copy_helper;
			public IntPtr dispose;
			public IntPtr signature;
		}

		[StructLayout (LayoutKind.Sequential)]
		struct BlockLiteral2
		{
			public IntPtr isa;
			public /*BlockFlags*/ int flags;
			public int reserved;
			public IntPtr invoke;
			public IntPtr block_descriptor;
			public IntPtr local_handle;
			public IntPtr global_handle;
		}

		[UnmanagedFunctionPointerAttribute (CallingConvention.Cdecl)]
		internal delegate void DActionArity1V1 (IntPtr block, IntPtr obj);
		static internal class SDActionArity1V1
		{
			static internal readonly DActionArity1V1 Handler = Invoke;

			[MonoPInvokeCallback (typeof (DActionArity1V1))]
			public static unsafe void Invoke (IntPtr block, IntPtr obj)
			{
				throw new NotImplementedException ();
			}
		}

		[UnmanagedFunctionPointerAttribute (CallingConvention.Cdecl)]
		[UserDelegateType (typeof (Action<NSObject>))]
		internal delegate void DActionArity1V2 (IntPtr block, IntPtr obj);
		static internal class SDActionArity1V2
		{
			static internal readonly DActionArity1V2 Handler = Invoke;

			[MonoPInvokeCallback (typeof (DActionArity1V2))]
			public static unsafe void Invoke (IntPtr block, IntPtr obj)
			{
				throw new NotImplementedException ();
			}
		}

		unsafe string GetBlockSignature (BlockLiteral block)
		{
			BlockLiteral2* blockptr = (BlockLiteral2*) &block;
			BlockDescriptor2* descptr = (BlockDescriptor2*) blockptr->block_descriptor;
			return Marshal.PtrToStringAuto (descptr->signature);
		}
	
		[Test]
		public void WithoutUserDelegateTypeAttribute ()
		{
			var block = new BlockLiteral ();
			var tramp = new DActionArity1V1 (SDActionArity1V1.Invoke);
			Action<NSObject> del = (v) => { };
			if (Runtime.DynamicRegistrationSupported) {
				block.SetupBlock (tramp, del);
				Assert.AreEqual ("v@:^v^v", GetBlockSignature (block), "a");
				block.CleanupBlock ();
			} else {
				Assert.Throws<RuntimeException> (() => block.SetupBlock (tramp, del));
			}
		}

		[Test]
		public void WithUserDelegateTypeAttribute ()
		{
			var block = new BlockLiteral ();
			var tramp = new DActionArity1V2 (SDActionArity1V2.Invoke);
			Action<NSObject> del = (v) => { };
			if (Runtime.DynamicRegistrationSupported) {
				block.SetupBlock (tramp, del);
				Assert.AreEqual ("v@?@", GetBlockSignature (block), "a");
				block.CleanupBlock ();
			} else {
				// The linker is able to rewrite calls to BlockLiteral.SetupBlock to BlockLiteral.SetupBlockImpl (which works without the dynamic registrar),
				// but that will only happen if the code is linked, and monotouch-test is only SdkLinked. Thus this code will throw an exception
				Assert.Throws<RuntimeException> (() => block.SetupBlock (tramp, del));
			}
		}
	}

	[Preserve]
	class OverloadByStaticity : NSObject
	{
		// Two Objective-C methods can have the same selector if one is static and the other instance.
		[Export ("method")]
		public void InstanceMethod () { }

		[Export ("method")]
		public static void StaticMethod () { }
	}

	// It should be possible to use a protocol with a member we can't use yet (because its signature uses a type not in the current SDK)
	[Protocol]
	[ProtocolMember (IsRequired = false, IsProperty = false, IsStatic = false, Name = "DetectPremonition", Selector = "detectPremonition:", ParameterType = new Type [] { typeof (FutureClass) }, ParameterByRef = new bool [] { false })]
	public interface ISomeDelegate : INativeObject, IDisposable
	{
	}
	[Introduced (PlatformName.MacOSX, 100, 0)]
	[Introduced (PlatformName.iOS, 100, 0)]
	[Introduced (PlatformName.TvOS, 100, 0)]
	[Introduced (PlatformName.WatchOS, 100, 0)]
	public class FutureClass : NSObject
	{
	}
	[Preserve]
	public class SomeConsumer : NSObject, ISomeDelegate
	{
	}
}
