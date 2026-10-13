using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Threading;

using MapKit;
#if HAS_ADDRESSBOOK
using AddressBook;
#endif
#if HAS_ADDRESSBOOKUI
using AddressBookUI;
#endif
#if MONOMAC
using AppKit;
using UIColor = AppKit.NSColor;
#else
using UIKit;
#endif
using PlatformException = ObjCRuntime.RuntimeException;
using NativeException = ObjCRuntime.ObjCException;
using CoreAnimation;
using CoreBluetooth;
using CoreGraphics;
using CoreLocation;
#if !__TVOS__
using Contacts;
#endif
#if HAS_COREMIDI
using CoreMidi;
#endif
#if !__TVOS__
using WebKit;
#endif
using Bindings.Test;

using RectangleF = CoreGraphics.CGRect;
using SizeF = CoreGraphics.CGSize;
using PointF = CoreGraphics.CGPoint;
using CategoryAttribute = ObjCRuntime.CategoryAttribute;

using XamarinTests.ObjCRuntime;
using Xamarin.Utils;

namespace MonoTouchFixtures.ObjCRuntime {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class RegistrarTest {
		public static Registrars CurrentRegistrar {
			get {
				return RegistrarSharedTest.CurrentRegistrar;
			}
		}

		[Test]
		public void EventTestCustomType ()
		{
			var earthDestroyed = 0;
			using var vogons = new Hitchhiker ();
			vogons.BuildIntergalacticHighway += (object sender, EventArgs ea) => {
				earthDestroyed++;
			};
			vogons.DestroyEarth ();
			Assert.That (earthDestroyed, Is.EqualTo (1), "Event raised");

			vogons.BuildHighway ();
			Assert.That (earthDestroyed, Is.EqualTo (2), "Event raised");
		}

		[Test]
		public void EventTestOSType ()
		{
			using var mgr = new CBCentralManager (new DispatchQueue ("com.xamarin.tests.ios-plain"));
			// The bug happens when there's no event listener for 'UpdatedState', which is a required protocol method.
			// mgr.UpdatedState += (sender, e) => { };

			// attach at least one event handler, to create an instance of the internal class that transforms protocol callbacks into events
			mgr.DiscoveredPeripheral += (sender, e) => { };

			// mimic what CBCentralManager does, instead of having to spin up the entire Bluetooth stack.
			Messaging.void_objc_msgSend_IntPtr (mgr.Delegate.Handle, Selector.GetHandle ("centralManagerDidUpdateState:"), IntPtr.Zero);
		}

		[Test]
		public void EventTestSdkType ()
		{
			var eventRaised = false;
			using var cache = new NSCache ();
			cache.WillEvictObject += (object sender, NSObjectEventArgs ea) => {
				eventRaised = true;
			};
			cache.SetObjectForKey (new NSObject (), new NSObject ());
			cache.RemoveAllObjects ();
			Assert.That (eventRaised, Is.True, "Event raised");
		}

		[Test]
		public void NSRangeOutParameter ()
		{
			using var obj = new NSRangeOutParameterClass ();
			var a = new _LongNSRange (-1, -2);
			var c = new _LongNSRange (-5, -6);
			Messaging.void_objc_msgSend_NSRange_out_NSRange_ref_NSRange (obj.Handle, Selector.GetHandle ("passRange:getRange:refRange:"), a, out var b, ref c);
			Assert.That ((long) (-1), Is.EqualTo (a.Location), "post a Location");
			Assert.That ((long) (-2), Is.EqualTo (a.Length), "post a Length");
			Assert.That ((long) 3, Is.EqualTo (b.Location), "post b Location");
			Assert.That ((long) 4, Is.EqualTo (b.Length), "post b Length");
			Assert.That ((long) 5, Is.EqualTo (c.Location), "post c Location");
			Assert.That ((long) 6, Is.EqualTo (c.Length), "post c Length");
		}

		class NSRangeOutParameterClass : NSObject {
			[Export ("passRange:getRange:refRange:")]
			public void DoIt (_LongNSRange a, out _LongNSRange b, ref _LongNSRange c)
			{
				Assert.That ((long) (-1), Is.EqualTo (a.Location), "a Location");
				Assert.That ((long) (-2), Is.EqualTo (a.Length), "a Length");
				Assert.That ((long) (-5), Is.EqualTo (c.Location), "c Location");
				Assert.That ((long) (-6), Is.EqualTo (c.Length), "c Length");

				a = new _LongNSRange (1, 2);
				b = new _LongNSRange (3, 4);
				c = new _LongNSRange (5, 6);
			}
		}

		// Returns "" at runtime, but the linker can't constant-fold this, which prevents
		// its dataflow analysis from resolving type names passed to Assembly.GetType.
		[MethodImpl (MethodImplOptions.NoInlining)]
		static string GetEmptyString () => string.Intern ("");
		static string WorkAroundLinkerHeuristics => GetEmptyString ();

		[Test]
		[UnconditionalSuppressMessage ("Trimming", "IL2026", Justification = "This test verifies linker behavior, and as such any behavioral difference when the trimmer is enabled is exactly what it's looking for.")]
		public void RegistrarRemoval ()
		{
#if NATIVEAOT && NET11_0_OR_GREATER
			// ILLink isn't executed when using NativeAOT on .NET 11+ (ILC does all the trimming), and even
			// though ILC trims away all the dynamic registrar's code, it still keeps the reflection metadata
			// for its types: monotouch-test sets $(IlcGenerateCompleteTypeMetadata)=true (NUnit requires it),
			// which promotes any type referenced from a member that's kept (ObjCRuntime.Runtime has a static
			// field of type Registrar.DynamicRegistrar) to complete type metadata. So looking for the types
			// says nothing about whether the dynamic registrar was removed; verify that dynamic registration
			// has been turned off instead.
			Assert.That (Runtime.DynamicRegistrationSupported, Is.False, "Dynamic registration support");
#else
			// define set by xharness when creating test variations.
			// It's not safe to remove the dynamic registrar in monotouch-test (by design; some of the tested API makes it unsafe, and the linker correctly detects this),
			// so the dynamic registrar will only be removed if manually requested.
			// Also removal of the dynamic registrar is not supported in XM
#if (OPTIMIZEALL && !__MACOS__) || NATIVEAOT
			var shouldBeRemoved = true;
#else
			var shouldBeRemoved = false;
#endif
			Assert.That (typeof (NSObject).Assembly.GetType ("Registrar.Registrar" + WorkAroundLinkerHeuristics) is null, Is.EqualTo (shouldBeRemoved), "Registrar removal");
			Assert.That (typeof (NSObject).Assembly.GetType ("Registrar.DynamicRegistrar" + WorkAroundLinkerHeuristics) is null, Is.EqualTo (shouldBeRemoved), "DynamicRegistrar removal");
#endif
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
#if !MONOMAC && !__MACCATALYST__
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
			var receiver = Class.GetHandle ("RegistrarTestClass");
			NativeHandle ptr;
			CGPath path;

			if (!global::XamarinTests.ObjCRuntime.Registrar.IsStaticRegistrar)
				Assert.Ignore ("This test only passes with the static registrars.");

			Assert.That (Messaging.bool_objc_msgSend_IntPtr (receiver, new Selector ("INativeObject1:").Handle, NativeHandle.Zero), Is.False, "#a1");
			Assert.That (Messaging.bool_objc_msgSend_IntPtr (receiver, new Selector ("INativeObject1:").Handle, new CGPath ().Handle), Is.True, "#a2");

			Assert.That ((NativeHandle) Messaging.IntPtr_objc_msgSend_bool (receiver, new Selector ("INativeObject2:").Handle, false), Is.EqualTo (NativeHandle.Zero), "#b1");
			ptr = Messaging.IntPtr_objc_msgSend_bool (receiver, new Selector ("INativeObject2:").Handle, true);
			Assert.That ((NativeHandle) ptr, Is.Not.EqualTo (NativeHandle.Zero), "#b2");
			CGPathRelease (ptr);

			void_objc_msgSend_out_IntPtr_bool (receiver, new Selector ("INativeObject3:create:").Handle, out ptr, true);
			Assert.That (ptr, Is.Not.EqualTo (NativeHandle.Zero), "#c1");
			void_objc_msgSend_out_IntPtr_bool (receiver, new Selector ("INativeObject3:create:").Handle, out ptr, false);
			Assert.That (ptr, Is.EqualTo (NativeHandle.Zero), "#c2");

			path = null;
			ptr = NativeHandle.Zero;
			Assert.That (bool_objc_msgSend_ref_intptr (receiver, new Selector ("INativeObject4:").Handle, ref ptr), Is.False, "#d1");
			Assert.That (ptr, Is.EqualTo (NativeHandle.Zero), "#d2");
			path = new CGPath ();
			ptr = path.Handle;
			Assert.That (bool_objc_msgSend_ref_intptr (receiver, new Selector ("INativeObject4:").Handle, ref ptr), Is.True, "#d3");
			Assert.That (ptr, Is.EqualTo (path.Handle), "#d4");

			ptr = Messaging.IntPtr_objc_msgSend_bool (receiver, new Selector ("INativeObject5:").Handle, false);
			Assert.That (ptr, Is.EqualTo (NativeHandle.Zero), "#e1");
			ptr = Messaging.IntPtr_objc_msgSend_bool (receiver, new Selector ("INativeObject5:").Handle, true);
			Assert.That (ptr, Is.Not.EqualTo (NativeHandle.Zero), "#e2");
			path = Runtime.GetINativeObject<CGPath> (ptr, false);
			path.AddArc (1, 2, 3, 4, 5, false); // this should crash if we get back a bogus ptr
			CGPathRelease (ptr);
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

			void_objc_msgSend_out_IntPtr (obj.Handle, sel.Handle, out var ptr);

			Assert.That (NSString.FromHandle (ptr), Is.EqualTo ("Santa is coming"), "#santa");
		}

		[Test]
		public void TestInheritedStaticMethods ()
		{
			// bug #6170
			int rv;

			rv = Messaging.int_objc_msgSend (Class.GetHandle (typeof (StaticBaseClass)), Selector.GetHandle ("foo"));
			Assert.That (rv, Is.EqualTo (314), "#base");
			rv = Messaging.int_objc_msgSend (Class.GetHandle (typeof (StaticDerivedClass)), Selector.GetHandle ("foo"));
			Assert.That (rv, Is.EqualTo (314), "#derived");
		}

		[Test]
		public void TestStructAndOut ()
		{
			var obj = new RegistrarTestClass ();
			var sel = new Selector ("testOutParametersWithStructs:in:out:");
			NSError value = new NSError ();
			NativeHandle ptr;
			SizeF size = new CGSize (1, 2);

			void_objc_msgSend_SizeF_IntPtr_out_IntPtr (obj.Handle, sel.Handle, size, value.Handle, out ptr);

			Assert.That (ptr, Is.EqualTo (value.Handle), "#1");
		}

#if !__TVOS__ && !MONOMAC
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
#endif // !__TVOS__ && !MONOMAC

		class TS1 : NSObject { }
		class TS2 : NSObject { }
		class TS3 : NSObject { }
		class TS4 : NSObject { }
		class TS5 : NSObject { }
		class TS6 : NSObject { }
		class TS7 : NSObject { }
		class TS8 : NSObject { }
		class TS9 : NSObject { }
		class TS10 : NSObject { }

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
			var types = new Type [] { typeof (TS1), typeof (TS2), typeof (TS3), typeof (TS4), typeof (TS5), typeof (TS6), typeof (TS7), typeof (TS8), typeof (TS9), typeof (TS10) };
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
			Assert.That (start_counter.Wait (1000), Is.True, "all threads didn't spin up in 1s");

			wait.Set (); // let the threads go wild.

			Assert.That (end_counter.Wait (5000), Is.True, "all threads didn't finish testing in 5s");

			for (int i = 0; i < threads.Length; i++) {
				Assert.That (threads [i].Join (1000), Is.True, "join #" + i.ToString ());
			}

			if (exceptions.Count > 0) {
				Assert.Fail ($"Expected no exceptions, but got:\n{new AggregateException (exceptions).ToString ()}");
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
					Assert.That (rv.RetainCount, Is.EqualTo ((nuint) 2), "array");
					Assert.That (rv.GetType (), Is.SameAs (typeof (NSArray)), "array type");
					rv.DangerousRelease ();
				}

				using (var pool = new NSAutoreleasePool ())
					ptr = Messaging.IntPtr_objc_msgSend (obj.Handle, Selector.GetHandle ("testReturnINativeObject"));
				using (var rv = Runtime.GetNSObject (ptr)) {
					Assert.That (rv.RetainCount, Is.EqualTo ((nuint) 2), "inativeobject");
					Assert.That (rv.GetType (), Is.SameAs (typeof (NSObject)), "inativeobject type");
					rv.DangerousRelease ();
				}

				using (var pool = new NSAutoreleasePool ())
					ptr = Messaging.IntPtr_objc_msgSend (obj.Handle, Selector.GetHandle ("testRetainNSObject"));
				using (var rv = Runtime.GetNSObject (ptr)) {
					Assert.That (rv.RetainCount, Is.EqualTo ((nuint) 2), "nsobject");
					Assert.That (rv.GetType (), Is.SameAs (typeof (NSObject)), "nsobject type");
					rv.DangerousRelease ();
				}

				using (var pool = new NSAutoreleasePool ())
					ptr = Messaging.IntPtr_objc_msgSend (obj.Handle, Selector.GetHandle ("testRetainString"));
				using (var rv = Runtime.GetNSObject (ptr)) {
					Assert.That (rv.RetainCount, Is.EqualTo ((nuint) 2), "string");
					Assert.That (rv is NSString, Is.True, "string type");
					rv.DangerousRelease ();
				}
			}

			using (var obj = new DerivedRegistrar1 ()) {
				using (var pool = new NSAutoreleasePool ())
					ptr = Messaging.IntPtr_objc_msgSend (obj.Handle, Selector.GetHandle ("testOverriddenRetainNSObject"));
				using (var rv = Runtime.GetNSObject (ptr)) {
					Assert.That (rv.RetainCount, Is.EqualTo ((nuint) 2), "overridden nsobject");
					Assert.That (rv.GetType (), Is.SameAs (typeof (NSObject)), "overridden nsobject type");
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
			var class_handle = Class.GetHandle (typeof (Props));
			Assert.That (class_getProperty (class_handle, "myProp"), Is.Not.EqualTo (IntPtr.Zero));
		}

		[DllImport ("/usr/lib/libobjc.dylib")]
		static extern IntPtr class_getProperty (IntPtr cls, string name);

		[DllImport ("/usr/lib/libobjc.dylib")]
		internal static extern IntPtr object_getClass (IntPtr obj);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPathRelease (/* CGPathRef */ IntPtr path);

		[Test]
		public void TestNonVirtualProperty ()
		{
			using (var obj = new DerivedRegistrar1 ()) {
				Assert.That (Messaging.bool_objc_msgSend (obj.Handle, Selector.GetHandle ("b1")), Is.True);
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

			Assert.That (t1, Is.EqualTo (g1.GetTypeFullName ()), "#t1");
			Assert.That (t2, Is.EqualTo (g2.GetTypeFullName ()), "#t2");
			Assert.That (t3, Is.EqualTo (g3.GetTypeFullName ()), "#t3");
			Assert.That (t4, Is.EqualTo (g4.GetTypeFullName ()), "#t4");

			var openClass = Class.GetHandle ("Open_1");
			var handle = Messaging.IntPtr_objc_msgSend (openClass, Selector.GetHandle ("alloc"));
			try {
				handle = Messaging.IntPtr_objc_msgSend (handle, Selector.GetHandle ("init"));
				Assert.Fail ("Expected [[Open_1 alloc] init] to fail.");
			} catch (PlatformException mex) {
				Assert.That (mex.ToString (), Does.Contain ("Cannot construct an instance of the type 'MonoTouchFixtures.ObjCRuntime.RegistrarTest+Open`1' from Objective-C because the type is generic."), "Exception message");
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
				var expectedU = typeof (NSSet);
				var expectedV = typeof (string);
				Messaging.void_objc_msgSend_IntPtr (foo.Handle, Selector.GetHandle ("bar:"), IntPtr.Zero);
				Assert.That (foo.LastArg, Is.Null);
				Assert.That (foo.UType, Is.EqualTo (expectedU));
				Assert.That (foo.VType, Is.EqualTo (expectedV));
				Messaging.void_objc_msgSend_IntPtr (foo.Handle, Selector.GetHandle ("bar:"), view.Handle);
				Assert.That (foo.LastArg, Is.SameAs (view));
				Assert.That (foo.UType, Is.EqualTo (expectedU));
				Assert.That (foo.VType, Is.EqualTo (expectedV));

				var arr = NSArray.FromNSObjects (view);
				Messaging.void_objc_msgSend_IntPtr (foo.Handle, Selector.GetHandle ("zap:"), IntPtr.Zero);
				Assert.That (foo.LastArg, Is.Null);
				Assert.That (foo.UType, Is.EqualTo (expectedU));
				Assert.That (foo.VType, Is.EqualTo (expectedV));
				Messaging.void_objc_msgSend_IntPtr (foo.Handle, Selector.GetHandle ("zap:"), arr.Handle);
				Assert.That (((object []) foo.LastArg) [0], Is.SameAs (view));
				Assert.That (foo.UType, Is.EqualTo (expectedU));
				Assert.That (foo.VType, Is.EqualTo (expectedV));

				Assert.That (Messaging.IntPtr_objc_msgSend (foo.Handle, Selector.GetHandle ("xyz")), Is.EqualTo (IntPtr.Zero), "xyz");
				Assert.That (foo.LastArg, Is.Null);
				Assert.That (foo.UType, Is.EqualTo (expectedU));
				Assert.That (foo.VType, Is.EqualTo (expectedV));

				Assert.That (Messaging.IntPtr_objc_msgSend (foo.Handle, Selector.GetHandle ("barzap")), Is.EqualTo (IntPtr.Zero), "barzap");
				Assert.That (foo.LastArg, Is.Null);
				Assert.That (foo.UType, Is.EqualTo (expectedU));
				Assert.That (foo.VType, Is.EqualTo (expectedV));

				Messaging.void_objc_msgSend_IntPtr (foo.Handle, Selector.GetHandle ("setBarzap:"), IntPtr.Zero);
				Assert.That (foo.LastArg, Is.Null);
				Assert.That (foo.UType, Is.EqualTo (expectedU));
				Assert.That (foo.VType, Is.EqualTo (expectedV));

				Messaging.void_objc_msgSend_IntPtr (foo.Handle, Selector.GetHandle ("setBarzap:"), view.Handle);
				Assert.That (foo.LastArg, Is.SameAs (view));
				Assert.That (foo.UType, Is.EqualTo (expectedU));
				Assert.That (foo.VType, Is.EqualTo (expectedV));

				arr.Dispose ();
				view.Dispose ();
				foo.Dispose ();
			}

			{
				var foo = new Open<NSObject, int> ();

				var view = new NSObject ();
				var expectedU = typeof (NSObject);
				var expectedV = typeof (int);
				Messaging.void_objc_msgSend_IntPtr (foo.Handle, Selector.GetHandle ("bar:"), IntPtr.Zero);
				Assert.That (foo.LastArg, Is.Null);
				Assert.That (foo.UType, Is.EqualTo (expectedU));
				Assert.That (foo.VType, Is.EqualTo (expectedV));
				Messaging.void_objc_msgSend_IntPtr (foo.Handle, Selector.GetHandle ("bar:"), view.Handle);
				Assert.That (foo.LastArg, Is.SameAs (view));
				Assert.That (foo.UType, Is.EqualTo (expectedU));
				Assert.That (foo.VType, Is.EqualTo (expectedV));

				var arr = NSArray.FromNSObjects (view);
				Messaging.void_objc_msgSend_IntPtr (foo.Handle, Selector.GetHandle ("zap:"), IntPtr.Zero);
				Assert.That (foo.LastArg, Is.Null);
				Assert.That (foo.UType, Is.EqualTo (expectedU));
				Assert.That (foo.VType, Is.EqualTo (expectedV));
				Messaging.void_objc_msgSend_IntPtr (foo.Handle, Selector.GetHandle ("zap:"), arr.Handle);
				Assert.That (((object []) foo.LastArg) [0], Is.SameAs (view));
				Assert.That (foo.UType, Is.EqualTo (expectedU));
				Assert.That (foo.VType, Is.EqualTo (expectedV));

				Assert.That (Messaging.IntPtr_objc_msgSend (foo.Handle, Selector.GetHandle ("xyz")), Is.EqualTo (IntPtr.Zero), "xyz");
				Assert.That (foo.LastArg, Is.Null);
				Assert.That (foo.UType, Is.EqualTo (expectedU));
				Assert.That (foo.VType, Is.EqualTo (expectedV));

				Assert.That (Messaging.IntPtr_objc_msgSend (foo.Handle, Selector.GetHandle ("barzap")), Is.EqualTo (IntPtr.Zero), "barzap");
				Assert.That (foo.LastArg, Is.Null);
				Assert.That (foo.UType, Is.EqualTo (expectedU));
				Assert.That (foo.VType, Is.EqualTo (expectedV));

				Messaging.void_objc_msgSend_IntPtr (foo.Handle, Selector.GetHandle ("setBarzap:"), IntPtr.Zero);
				Assert.That (foo.LastArg, Is.Null);
				Assert.That (foo.UType, Is.EqualTo (expectedU));
				Assert.That (foo.VType, Is.EqualTo (expectedV));

				Messaging.void_objc_msgSend_IntPtr (foo.Handle, Selector.GetHandle ("setBarzap:"), view.Handle);
				Assert.That (foo.LastArg, Is.SameAs (view));
				Assert.That (foo.UType, Is.EqualTo (expectedU));
				Assert.That (foo.VType, Is.EqualTo (expectedV));

				arr.Dispose ();
				view.Dispose ();
				foo.Dispose ();
			}
		}

#if !MONOMAC
		[Test]
		public void TestGenericUIView ()
		{
			using (var iview = new NullableIntView (new CGRect (0, 0, 100, 100))) {
				using (var strview = new StringView (new CGRect (0, 0, 100, 100))) {
					Messaging.void_objc_msgSend_CGRect (iview.Handle, Selector.GetHandle ("drawRect:"), CGRect.Empty);
					Assert.That (iview.TypeT, Is.EqualTo (typeof (int?)), "int?");
					Assert.That (iview.TypeName, Is.EqualTo ("NullableIntView"), "int? typename");
					Messaging.void_objc_msgSend_CGRect (strview.Handle, Selector.GetHandle ("drawRect:"), CGRect.Empty);
					Assert.That (strview.TypeT, Is.EqualTo (typeof (string)), "string");
					Assert.That (strview.TypeName, Is.EqualTo ("StringView"), "string typename");
				}
			}
		}
#endif // !MONOMAC

#if !MONOMAC
		[Test]
		public void TestNativeEnum ()
		{
			var nativeEnumValue = NSWritingDirection.RightToLeft;
			//public virtual void TestNativeEnum1 (UITextWritingDirection twd)
			using (var obj = new RegistrarTestClass ()) {
				if (IntPtr.Size == 4) {
					Messaging.void_objc_msgSend_int (obj.Handle, Selector.GetHandle ("testNativeEnum1:"), (int) nativeEnumValue);
				} else {
					Messaging.void_objc_msgSend_long (obj.Handle, Selector.GetHandle ("testNativeEnum1:"), (long) nativeEnumValue);
				}

				if (IntPtr.Size == 4) {
					Messaging.void_objc_msgSend_int_int_long (obj.Handle, Selector.GetHandle ("testNativeEnum1:"), (int) nativeEnumValue, 31415, 3141592);
				} else {
					Messaging.void_objc_msgSend_long_int_long (obj.Handle, Selector.GetHandle ("testNativeEnum1:"), (long) nativeEnumValue, 31415, 3141592);
				}

				if (IntPtr.Size == 4) {
					Assert.That (Messaging.int_objc_msgSend (obj.Handle, Selector.GetHandle ("testNativeEnum2")), Is.EqualTo ((int) UIPopoverArrowDirection.Right), "testNativeEnum2");
					Messaging.void_objc_msgSend_int (obj.Handle, Selector.GetHandle ("setTestNativeEnum2:"), (int) UIPopoverArrowDirection.Left);
				} else {
					Assert.That (Messaging.long_objc_msgSend (obj.Handle, Selector.GetHandle ("testNativeEnum2")), Is.EqualTo ((long) UIPopoverArrowDirection.Right), "testNativeEnum2");
					Messaging.void_objc_msgSend_long (obj.Handle, Selector.GetHandle ("setTestNativeEnum2:"), (long) UIPopoverArrowDirection.Left);
				}
			}
		}
#endif // !MONOMAC

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
				var pnt1 = new CGPoint (123, 456);
				PointF pnt2 = new CGPoint ();
				void_objc_msgSend_CGPoint_ref_CGPoint (obj.Handle, Selector.GetHandle ("testCGPoint:out:"), pnt1, ref pnt2);
				Assert.That (pnt2.X, Is.EqualTo ((nfloat) 123), "X");
				Assert.That (pnt2.Y, Is.EqualTo ((nfloat) 456), "Y");
			}
		}

		[Test]
		public void ExportedGenericsTest ()
		{
			using (var obj = new RegistrarTestClass ()) {
				var rv = Runtime.GetNSObject<NSArray<NSString>> (Messaging.IntPtr_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("fetchNSArrayOfNSString:"), IntPtr.Zero));
				Assert.That (rv, Is.Not.Null, "method");

				using (var number_array = NSArray<NSNumber>.FromNSObjects ((NSNumber) 314)) {
					rv = Runtime.GetNSObject<NSArray<NSString>> (Messaging.IntPtr_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("fetchNSArrayOfNSString:"), number_array.Handle));
					Assert.That (rv, Is.Not.Null, "method param");
				}

				rv = Runtime.GetNSObject<NSArray<NSString>> (Messaging.IntPtr_objc_msgSend (obj.Handle, Selector.GetHandle ("nSArrayOfNSString")));
				Assert.That (rv, Is.Not.Null, "property");

				Messaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("setNSArrayOfNSString:"), IntPtr.Zero);
				Messaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("setNSArrayOfNSString:"), rv.Handle);

				var rv2 = Runtime.GetNSObject<NSArray<NSArray<NSString>>> (Messaging.IntPtr_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("fetchComplexGenericType:"), IntPtr.Zero));
				Assert.That (rv2, Is.Not.Null, "complex");

				using (var complex = new NSArray<NSDictionary<NSString, NSArray<NSNumber>>> ()) {
					Runtime.GetNSObject<NSArray<NSArray<NSString>>> (Messaging.IntPtr_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("fetchComplexGenericType:"), complex.Handle));
					Assert.That (rv2, Is.Not.Null, "complex param");
				}
			}
		}

		const string LIBOBJC_DYLIB = "/usr/lib/libobjc.dylib";

		[DllImport (LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
		extern static void void_objc_msgSend_out_IntPtr (IntPtr receiver, IntPtr selector, out NativeHandle value);

		[DllImport (LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
		extern static void void_objc_msgSend_ref_IntPtr (IntPtr receiver, IntPtr selector, ref NativeHandle value);

		[DllImport (LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
		extern static void void_objc_msgSend_out_IntPtr_bool (IntPtr receiver, IntPtr selector, out NativeHandle path, bool create);

		[DllImport (LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
		[return: MarshalAs (UnmanagedType.U1)]
		extern static bool bool_objc_msgSend_ref_intptr (IntPtr receiver, IntPtr selector, ref NativeHandle path);

		[DllImport (LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
		extern static void void_objc_msgSend_SizeF_IntPtr_out_IntPtr (IntPtr receiver, IntPtr selector, SizeF size, IntPtr input, out NativeHandle value);

		[DllImport (LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
		extern static void void_objc_msgSend_ref_BlockLiteral (IntPtr receiver, IntPtr selector, ref BlockLiteral block);

		[DllImport (LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
		extern static void void_objc_msgSend_CGPoint_ref_CGPoint (IntPtr receiver, IntPtr selector, PointF p1, ref PointF p2);

		#region Exported type
		[Register ("RegistrarTestClass")]
		class RegistrarTestClass : NSObject {
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
				return img is not null;
			}

			[Export ("INativeObject2:")]
			[return: ReleaseAttribute] // can't return an INativeObject without retaining it (we can autorelease NSObjects, but that doesn't work for INativeObjects)
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
				return path is not null;
			}

			[Export ("INativeObject5:")]
			[return: ReleaseAttribute] // can't return an INativeObject without retaining it (we can autorelease NSObjects, but that doesn't work for INativeObjects)
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

#if !__TVOS__ && !MONOMAC
			[Export ("testAction:")]
			public void TestAction ([BlockProxy (typeof (NIDActionArity1V1))] Action<UIBackgroundFetchResult> action)
			{
				// bug ?
			}
#endif // !__TVOS__ && !MONOMAC

			[return: ReleaseAttribute ()]
			[Export ("testRetainArray")]
			public NSObject [] TestRetainArray ()
			{
				return new NSObject [] { new NSObject () };
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

#if !MONOMAC
			[Export ("testNativeEnum1:")]
			public virtual void TestNativeEnum1 (NSWritingDirection twd)
			{
				Assert.That (Enum.GetValues<NSWritingDirection> ().Contains (twd), "TestNativeEnum1");
			}

			public virtual UIPopoverArrowDirection TestNativeEnum2 {
				[Export ("testNativeEnum2")]
				get {
					return UIPopoverArrowDirection.Right;
				}
				[Export ("setTestNativeEnum2:")]
				set {
					Assert.That (value, Is.EqualTo (UIPopoverArrowDirection.Left), "setTestNativeEnum2:");
				}
			}


			[Export ("testNativeEnum3:a:b:")]
			public virtual void TestNativeEnum1 (NSWritingDirection twd, int a, long b)
			{
				Assert.That (Enum.GetValues<NSWritingDirection> (), Contains.Item (twd), "TestNativeEnum3");
				Assert.That (a, Is.EqualTo (31415), "TestNativeEnum3 a");
				Assert.That (b, Is.EqualTo (3141592), "TestNativeEnum3 b");
			}
#endif // !MONOMAC

			[Export ("testCGPoint:out:")]
			public void TestCGPoint (PointF pnt, ref PointF pnt2)
			{
				pnt2.X = pnt.X;
				pnt2.Y = pnt.Y;
			}
#if !MONOMAC
			[Export ("arrayOfINativeObject")]
			public IUIKeyInput [] NativeObjects { get { return null; } }
#endif // !MONOMAC

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

#if !__TVOS__ && !MONOMAC
		[UnmanagedFunctionPointerAttribute (CallingConvention.Cdecl)]
		internal delegate void DActionArity1V1 (IntPtr block, UIBackgroundFetchResult obj);

		//
		// This class bridges native block invocations that call into C#
		//
		static internal class SDActionArity1V1 {
			static internal readonly DActionArity1V1 Handler = Invoke;

			[MonoPInvokeCallback (typeof (DActionArity1V1))]
			static unsafe void Invoke (IntPtr block, UIBackgroundFetchResult obj)
			{
				var descriptor = (BlockLiteral*) block;
				var del = (global::System.Action<UIBackgroundFetchResult>) (descriptor->Target);
				if (del is not null)
					del (obj);
			}
		} /*		 class SDActionArity1V1 */

		internal class NIDActionArity1V1 {
			IntPtr blockPtr;
			DActionArity1V1 invoker;

			[Preserve (Conditional = true)]
			public unsafe NIDActionArity1V1 (BlockLiteral* block)
			{
				blockPtr = (IntPtr) block;
				invoker = block->GetDelegateForBlock<DActionArity1V1> ();
			}
			[Preserve (Conditional = true)]
			public unsafe static global::System.Action<UIBackgroundFetchResult> Create (IntPtr block)
			{
				return new NIDActionArity1V1 ((BlockLiteral*) block).Invoke;
			}

			[Preserve (Conditional = true)]
			unsafe void Invoke (UIBackgroundFetchResult obj)
			{
				invoker (blockPtr, obj);
			}
		} /*		 class NIDActionArity1V1 */
#endif // !__TVOS__ && !MONOMAC


		[Register ("StaticBaseClass")]
		class StaticBaseClass : NSObject {
			[Export ("foo")]
			public static int Foo ()
			{
				return 314;
			}
		}

		[Register ("StaticDerivedClass")]
		class StaticDerivedClass : StaticBaseClass {
		}

		[Register ("DerivedRegistrar1")]
		class DerivedRegistrar1 : RegistrarTestClass {
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
		class DerivedRegistrar2 : DerivedRegistrar1 {
			public override string VirtualMethod ()
			{
				return "DerivedRegistrar2";
			}
		}

		[Register ("GenericBaseClass")]
		class GenericBaseClass : NSObject {
			[Export ("GetTypeFullName")]
			public virtual string GetTypeFullName ()
			{
				return GetType ().FullName;
			}
		}

		[Register ("Open_1")]
		class Open<T> : NSObject { }
		class Closed : Open<NSSet> {
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

		class Open<U, V> : NSObject where U : NSObject {
			public object LastArg;
			public object UType;
			public object VType;

			[Export ("bar:")]
			public void Bar (U arg)
			{
				UType = typeof (U);
				VType = typeof (V);
				LastArg = arg;
			}

			[Export ("zap:")]
			public void Zap (U [] arg)
			{
				UType = typeof (U);
				VType = typeof (V);
				LastArg = arg;
			}

			[Export ("xyz")]
			public U XyZ ()
			{
				UType = typeof (U);
				VType = typeof (V);
				LastArg = null;
				return null;
			}

			[Export ("barzap")]
			public U BarZap {
				get {
					UType = typeof (U);
					VType = typeof (V);
					LastArg = null;
					return null;
				}
				set {
					UType = typeof (U);
					VType = typeof (V);
					LastArg = value;
				}
			}
		}

		// This T is also valid/usable
		class Open1<T> : NSObject where T : NSObject { }
		class Open2<T> : NSObject where T : C { }
		class C : NSObject { }

		class ClosedGenericParameter : NSObject {
			// TODO: create test for this once we can call delegates with a null function pointer.
			[Export ("foo:")]
			public void Foo (Action<string> func) { }
		}

		[Register ("GenericTestClass")]
		class GenericTestClass<T> : GenericBaseClass {
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

		class DerivedGenericTestClass<T> : GenericTestClass<T> {
			public override string GetTypeFullName ()
			{
				return base.GetTypeFullName ();
			}
		}

		[Test]
		public void TestRegisteredName ()
		{
			Assert.That (new Class (typeof (ConstrainedGenericType<>)).Name, Is.EqualTo ("MonoTouchFixtures_ObjCRuntime_RegistrarTest_ConstrainedGenericType_1"));
			Assert.That (new Class (typeof (ConstrainedGenericType<NSSet>)).Name, Is.EqualTo ("MonoTouchFixtures_ObjCRuntime_RegistrarTest_ConstrainedGenericType_1"));
			Assert.That (new Class (typeof (NestedParent<NSObject>.Nested)).Name, Is.EqualTo ("MonoTouchFixtures_ObjCRuntime_RegistrarTest_NestedParent_1_Nested"));
			Assert.That (new Class (typeof (UnderlyingEnumValues)).Name, Is.EqualTo ("UnderlyingEnumValues"));
			Assert.That (new Class (typeof (Nested1.Dummy)).Name, Is.EqualTo ("MonoTouchFixtures_ObjCRuntime_RegistrarTest_Nested1_Dummy"));
			Assert.That (new Class (typeof (C)).Name, Is.EqualTo ("MonoTouchFixtures_ObjCRuntime_RegistrarTest_C"));
		}

		void ThrowsICEIfDebug (TestDelegate code, string message, bool execute_release_mode = true)
		{
			if (TestRuntime.IsCoreCLR ||
				global::XamarinTests.ObjCRuntime.Registrar.CurrentRegistrar == Registrars.ManagedStatic ||
				global::XamarinTests.ObjCRuntime.Registrar.CurrentRegistrar == Registrars.TrimmableStatic) {
				if (execute_release_mode) {
					// In CoreCLR will either throw an ArgumentException:
					//     <System.ArgumentException: Object of type 'Foundation.NSObject' cannot be converted to type 'Foundation.NSSet'.
					// or a RuntimeException:
					//     <ObjCRuntime.RuntimeException: Failed to marshal the value at index 0.
					// or an InvalidCastException
					//    System.InvalidCastException: Unable to cast object of type 'Foundation.NSObject' to type 'Foundation.NSSet'.
					var noException = false;
					try {
						code ();
						noException = true;
					} catch (ArgumentException) {
						// OK
					} catch (RuntimeException) {
						// OK
					} catch (InvalidCastException) {
						// OK
					} catch (Exception e) {
						Assert.Fail ($"Unexpectedly failed with exception of type {e.GetType ()} - expected either ArgumentException or RuntimeException: {message}");
					}
					if (noException)
						Assert.Fail ($"Unexpectedly no exception occured: {message}");
				}
				return;
			}

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
			NativeHandle value;

			using (var obj = new ConstrainedGenericType<NSSet> ()) {
				using (var view = new NSSet ()) {
					using (var nsobj = new NSObject ()) {
						// m1
						Messaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("m1:"), NativeHandle.Zero);
						Messaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("m1:"), view.Handle);
						ThrowsICEIfDebug (() => Messaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("m1:"), nsobj.Handle), "m1: ICE");

						// m2
						value = NativeHandle.Zero;
						void_objc_msgSend_out_IntPtr (obj.Handle, Selector.GetHandle ("m2:"), out value);
						Assert.That (value, Is.EqualTo (NativeHandle.Zero));

						value = view.Handle;
						void_objc_msgSend_out_IntPtr (obj.Handle, Selector.GetHandle ("m2:"), out value);
						Assert.That (value, Is.EqualTo (NativeHandle.Zero));

						value = (NativeHandle) new IntPtr ((unchecked((int) 0xdeadbeef)));
						void_objc_msgSend_out_IntPtr (obj.Handle, Selector.GetHandle ("m2:"), out value);
						Assert.That (value, Is.EqualTo (NativeHandle.Zero));

						// m3
						value = NativeHandle.Zero;
						void_objc_msgSend_ref_IntPtr (obj.Handle, Selector.GetHandle ("m3:"), ref value);
						Assert.That (value, Is.EqualTo (NativeHandle.Zero));

						value = view.Handle;
						void_objc_msgSend_ref_IntPtr (obj.Handle, Selector.GetHandle ("m3:"), ref value);
						Assert.That (value, Is.EqualTo (view.Handle));

						value = nsobj.Handle;
						ThrowsICEIfDebug (() => void_objc_msgSend_ref_IntPtr (obj.Handle, Selector.GetHandle ("m3:"), ref value), "m3 ICE");

						// m4
						Messaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("m4:"), NativeHandle.Zero);
						ThrowsICEIfDebug (() => Messaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("m4:"), nsobj.Handle), "m4 ICE", false);
						using (var arr = NSArray.FromNSObjects (nsobj)) {
							ThrowsICEIfDebug (() => Messaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("m4:"), arr.Handle), "m4 ICE 2");
						}

						using (var arr = NSArray.FromNSObjects (view)) {
							Messaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("m4:"), arr.Handle);
						}

						// r1
						Assert.That ((NativeHandle) Messaging.IntPtr_objc_msgSend (obj.Handle, Selector.GetHandle ("r1")), Is.EqualTo (NativeHandle.Zero));

						// r2
						Assert.That ((NativeHandle) Messaging.IntPtr_objc_msgSend (obj.Handle, Selector.GetHandle ("r2")), Is.EqualTo (NativeHandle.Zero));

						// p1
						Assert.That ((NativeHandle) Messaging.IntPtr_objc_msgSend (obj.Handle, Selector.GetHandle ("p1")), Is.EqualTo (NativeHandle.Zero));
						Messaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("setP1:"), NativeHandle.Zero);
						Messaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("setP1:"), view.Handle);
						ThrowsICEIfDebug (() => Messaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("setP1:"), nsobj.Handle), "setP1: ICE");

						// p2
						Assert.That ((NativeHandle) Messaging.IntPtr_objc_msgSend (obj.Handle, Selector.GetHandle ("p2")), Is.EqualTo (NativeHandle.Zero));
						Messaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("setP2:"), NativeHandle.Zero);
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

#if !MONOMAC
		[Test]
		public void TestCopyWithZone ()
		{
			using (var cc = new CopyClass ()) {
				Assert.That ((NativeHandle) Messaging.IntPtr_objc_msgSend_IntPtr (cc.Handle, Selector.GetHandle ("copyWithZone:"), NativeHandle.Zero), Is.EqualTo (cc.Handle), "a");
				Assert.That (cc.had_zone.Value, Is.False, "had_zone");
			}
		}

		// This verifies that a very incorrect implementation of NSTableViewCell isn't shared with UITableViewCell.
		class CopyClass : UITableViewCell, INSCopying {
			public bool? had_zone;

			[Export ("copyWithZone:")]
			public NSObject Copy (NSZone zone)
			{
				had_zone = zone is not null;
				DangerousRetain ();
				return this;
			}
		}
#endif // !MONOMAC

		[Test]
		public void TestProtocolRegistration ()
		{
			var iProtocol = typeof (IProtocol).FullName.Replace (".", "_").Replace ("+", "_");
			Assert.That (Runtime.GetProtocol (iProtocol), Is.Not.EqualTo (IntPtr.Zero), "IProtocol");
			Assert.That (Messaging.bool_objc_msgSend_IntPtr (Class.GetHandle (typeof (MyProtocolImplementation)), Selector.GetHandle ("conformsToProtocol:"), Runtime.GetProtocol (iProtocol)), Is.True, "Interface/IProtocol");
#if !__TVOS__ && !MONOMAC
			Assert.That (Messaging.bool_objc_msgSend_IntPtr (Class.GetHandle (typeof (Test24970)), Selector.GetHandle ("conformsToProtocol:"), Protocol.GetHandle ("UIApplicationDelegate")), Is.True, "UIApplicationDelegate/17669");
#endif
			// We don't support [Adopts] (yet at least).
			//			Assert.That (Messaging.bool_objc_msgSend_IntPtr (Class.GetHandle (typeof (ConformsToProtocolTestClass)), Selector.GetHandle ("conformsToProtocol:"), Runtime.GetProtocol ("NSCoding")), Is.True, "Adopts/ConformsToProtocolTestClass");
		}

		[Test]
		public void TestTypeEncodings ()
		{
			var cl = new Class (typeof (TestTypeEncodingsClass));
			var sig = Runtime.GetNSObject<NSMethodSignature> (Messaging.IntPtr_objc_msgSend_IntPtr (cl.Handle, Selector.GetHandle ("methodSignatureForSelector:"), Selector.GetHandle ("foo::::::::::::::::")));
#if MONOMAC || __MACCATALYST__
			var boolEncoding = TrampolineTest.IsArm64CallingConvention ? "B" : "c";
#else
			var boolEncoding = (IntPtr.Size == 8 || TrampolineTest.IsArmv7k || TrampolineTest.IsArm64CallingConvention) ? "B" : "c";

#endif
			var exp = new string [] { "@", ":", "^v", "C", "c", "s", "s", "S", "i", "I", "q", "Q", "f", "d", boolEncoding, "@", ":", "#" };

			Assert.That (sig.NumberOfArguments, Is.EqualTo ((nuint) exp.Length), "NumberOfArguments");
			//			for (uint i = 0; i < exp.Length; i++) {
			//				var p = Marshal.PtrToStringAuto (sig.GetArgumentType (i));
			//				Console.WriteLine ("{0}: {1}", i, p);
			//			}
			for (uint i = 0; i < exp.Length; i++) {
				var p = Marshal.PtrToStringAuto (sig.GetArgumentType (i));
				Assert.That (p, Is.EqualTo (exp [i]), $"#{i}");
			}
		}

		class TestTypeEncodingsClass : NSObject {
			[Export ("foo::::::::::::::::")]
			public static void Foo (IntPtr p1, byte p2, sbyte p3, char p4, short p5, ushort p6, int p7, uint p8, long p9, ulong p10, float p11, double p12, bool p13, string p14, Selector p15, Class p16)
			{

			}
		}

#if !__TVOS__ // No MapKit in TVOS
		[Test]
		public void TestNativeObjectArray ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 9, throwIfOtherPlatform: false);

			using (var i1 = new MKPointAnnotation ()) {
				using (var i2 = new MKPointAnnotation ()) {
					using (var array = NSArray.FromObjects (i1, i2)) {
						using (var obj = new NativeObjectArrayType ()) {
							Messaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("addAnnotations:"), array.Handle);
							Assert.That (obj.Annotations.Length, Is.EqualTo (2), "length");
							Assert.That (obj.Annotations [0], Is.SameAs (i1), "i1");
							Assert.That (obj.Annotations [1], Is.SameAs (i2), "i2");
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
					Assert.That (obj.Annotations.Length, Is.EqualTo (2), "length #2");
					Assert.That (obj.Annotations [0], Is.Not.Null, "i1 #2");
					Assert.That (obj.Annotations [1], Is.Not.Null, "i2 #2");
				}
			}
		}

		public class NativeObjectArrayType : NSObject {
			public IMKAnnotation [] Annotations;

			[Export ("addAnnotations:")]
			public void AddAnnotations (params IMKAnnotation [] annotations)
			{
				this.Annotations = annotations;
			}
		}
#endif // !__TVOS__

		class ConstrainedGenericType<T> : NSObject where T : NSObject {
			[Export ("m1:")]
			public void M1 (T t) { }

			[Export ("m2:")]
			public void M2 (out T t) { t = null; }

			[Export ("m3:")]
			public void M3 (ref T t) { }

			[Export ("m4:")]
			public void M4 (T [] t) { }

			[Export ("r1")]
			public T R1 () { return null; }

			[Export ("r2")]
			public T [] R2 () { return null; }

			[Export ("p1")]
			public T P1 { get { return null; } set { } }

			[Export ("p2")]
			public T [] P2 { get { return null; } set { } }
		}

		class Generic2<T> : NSObject where T : NSObject {
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

		class NestedParent<T> where T : NSObject {
			public class Nested : NSObject {
				[Export ("foo:")]
				public void Foo (T foo)
				{
				}
			}
		}


#if !MONOMAC
		class CustomView<T> : UIView {
			public object TypeName;
			public object TypeT;

			public CustomView (RectangleF rect) : base (rect) { }
			public override void Draw (RectangleF rect)
			{
				TypeT = typeof (T);
				TypeName = GetType ().Name;
			}
		}

		class StringView : CustomView<string> {
			public StringView (RectangleF rect) : base (rect) { }
		}

		class NullableIntView : CustomView<int?> {
			public NullableIntView (RectangleF rect) : base (rect) { }
		}
#endif // !MONOMAC

		class GenericConstrainedBase<T> : NSObject where T : NSObject {
			public T FooT;
			public string FooType;

			[Export ("foo:")]
			public virtual void Foo (T obj)
			{
				FooT = obj;
				FooType = "Base";
			}
		}

		class GenericConstrainedDerived<T> : GenericConstrainedBase<T> where T : NSObject {
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
				Assert.That (obj.FooType, Is.EqualTo ("Derived"), "Derived");
				Assert.That (obj.FooT, Is.SameAs (obj), "obj");
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
				Assert.That (Messaging.bool_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("conformsToProtocol:"), Runtime.GetProtocol ("NSCoding")), Is.True);
				Assert.That (Messaging.bool_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("conformsToProtocol:"), Runtime.GetProtocol ("NSCopying")), Is.False);
			}

			using (var obj = new ConformsToProtocolTestClass ()) {
				Assert.That (Messaging.bool_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("conformsToProtocol:"), Runtime.GetProtocol ("NSCoding")), Is.True);
				Assert.That (Messaging.bool_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("conformsToProtocol:"), Runtime.GetProtocol ("NSCopying")), Is.False);
			}
		}

		[Adopts ("NSCoding")]
		public class ConformsToProtocolTestClass : NSObject {
		}

		[Adopts ("NSCoding")]
		public class ConformsToProtocolTestClass<T> : NSObject where T : NSObject {
		}

		[Register ("UnderlyingEnumValues")]
		internal class UnderlyingEnumValues : NSObject {
			[Export ("Foo:a:b:c:d:e:f:g:h")]
			void Foo (EnumB b, EnumSB sb, EnumS s, EnumUS us, EnumI i, EnumUI ui, EnumL l, EnumUL ul)
			{
			}

			[Export ("ByRef:a:b:c:d:e:f:g:")]
			void ByRef (ref EnumB b, ref EnumSB sb, ref EnumS s, ref EnumUS us, ref EnumI i, ref EnumUI ui, ref EnumL l, ref EnumUL ul)
			{
				b = EnumB.b;
				sb = EnumSB.b;
				s = EnumS.b;
				us = EnumUS.b;
				i = EnumI.b;
				ui = EnumUI.b;
				l = EnumL.b;
				ul = EnumUL.b;
			}

			[Export ("Out:a:b:c:d:e:f:g:")]
			void Out (out EnumB b, out EnumSB sb, out EnumS s, out EnumUS us, out EnumI i, out EnumUI ui, out EnumL l, out EnumUL ul)
			{
				b = EnumB.b;
				sb = EnumSB.b;
				s = EnumS.b;
				us = EnumUS.b;
				i = EnumI.b;
				ui = EnumUI.b;
				l = EnumL.b;
				ul = EnumUL.b;
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
			Assert.That (Messaging.int_objc_msgSend (o.Handle, Selector.GetHandle ("foo")), Is.EqualTo (31415), "#method");
			Assert.That (Messaging.int_objc_msgSend (o.Handle, Selector.GetHandle ("bar")), Is.EqualTo (31415926), "#getter");
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
					Assert.That (obj2_ptr, Is.Not.EqualTo (IntPtr.Zero), "not zero");
					Messaging.bool_objc_msgSend_IntPtr (obj1.Handle, Selector.GetHandle ("fakeTypeTest:"), obj2_ptr);
				}
			} finally {
				Messaging.void_objc_msgSend (obj2_ptr, Selector.GetHandle ("release"));
			}
		}

		[Register ("FakeType1")]
		class FakeType1 : NSObject {
			public FakeType1 (NativeHandle ptr)
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
				Assert.That (cls.Name, Is.EqualTo ("FakeType2"));
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
				Assert.That (tc.Pd1, Is.EqualTo (0), "Pd1");
				Assert.That (tc.D (), Is.EqualTo (0), "1");
				tc.Pd1 = 1.2;
				Assert.That (tc.D (), Is.EqualTo (1.2), "2");
				Assert.That (1.2, Is.EqualTo (tc.Pd1), "Pd1");
			}
		}

		public class TestClass : ObjCRegistrarTest {
		}

		[Test]
		public void IdAsIntPtrTest ()
		{
			using (var obj = new IdAsIntPtrClass ()) {
				Messaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("idAsIntPtr:"), IntPtr.Zero);
			}
		}

		public class IdAsIntPtrClass : ObjCProtocolTest {
			[Export ("idAsIntPtr:")]
			public new void IdAsIntPtr (IntPtr id)
			{
				Assert.That (id, Is.EqualTo (IntPtr.Zero), "Zero");
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
				Assert.That (Marshal.ReadIntPtr (ptr), Is.EqualTo (IntPtr.Zero), "#1");
				Marshal.FreeHGlobal (ptr);

				ptr = IntPtr.Zero;
				unsafe {
					IntPtr* ptrFixed = &ptr;
					Console.WriteLine ("ptr: 0x{0}", ptr.ToString ("x"));
					Messaging.void_objc_msgSend_int_int_int_int_int_int_IntPtr (obj.Handle, Selector.GetHandle ("outNSErrorOnStack:i:i:i:i:i:err:"), 0, 0, 0, 0, 0, 0, (IntPtr) ptrFixed);
					Assert.That (ptr, Is.EqualTo (IntPtr.Zero), "#2");
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
				Assert.That (Marshal.ReadIntPtr (ptr), Is.EqualTo (IntPtr.Zero), "#1");
				Marshal.FreeHGlobal (ptr);

				ptr = IntPtr.Zero;
				unsafe {
					IntPtr* ptrFixed = &ptr;
					Console.WriteLine ("ptr: 0x{0}", ptr.ToString ("x"));
					Messaging.void_objc_msgSend_IntPtr_IntPtr_IntPtr_long_int_IntPtr (obj.Handle, Selector.GetHandle ("outNSErrorOnStack:obj:obj:int64:i:err:"), IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, 1, 2, (IntPtr) ptrFixed);
					Assert.That (ptr, Is.EqualTo (IntPtr.Zero), "#2");
				}
			}
		}

		public class OutNSErrorOnStackClass : ObjCRegistrarTest {
			public override void OutNSErrorOnStack (int i1, int i2, int i3, int i4, int i5, int i6, out NSError error)
			{
				error = null;
			}

			public override void OutNSErrorOnStack (NSObject i1, NSObject i2, NSObject i3, long i4, int i5, out NSError error)
			{
				Assert.That (i4, Is.EqualTo (1), "#long");
				Assert.That (i5, Is.EqualTo (2), "#int");
				error = null;
			}
		}

#endif

#if !__TVOS__ && !MONOMAC
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

				Assert.That (performed, Is.True);
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
				static unsafe void Invoke (IntPtr block, nuint obj)
				{
					var descriptor = (BlockLiteral*) block;
					var del = (global::System.Action<UIBackgroundFetchResult>) (descriptor->Target);
					if (del is not null)
						del ((UIBackgroundFetchResult) (global::System.UInt64) obj);
				}
			} /* class SDActionArity1V42 */

			internal class NIDActionArity1V42 {
				IntPtr blockPtr;
				DActionArity1V42 invoker;

				[Preserve (Conditional = true)]
				public unsafe NIDActionArity1V42 (BlockLiteral* block)
				{
					blockPtr = _Block_copy ((IntPtr) block);
					invoker = block->GetDelegateForBlock<DActionArity1V42> ();
				}

				[Preserve (Conditional = true)]
				~NIDActionArity1V42 ()
				{
					_Block_release (blockPtr);
				}

				[Preserve (Conditional = true)]
				public unsafe static Action<UIBackgroundFetchResult> Create (IntPtr block)
				{
					return new NIDActionArity1V42 ((BlockLiteral*) block).Invoke;
				}

				[Preserve (Conditional = true)]
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
#endif // !__TVOS__ && !MONOMAC

		[Protocol]
		[Model]
		class Test25781 : NSObject { }
		class Test25781D : Test25781 { }

		[Test]
		public void TestProtocolAndRegister ()
		{
			// Having [Protocol] [Register] (and nothing else), doesn't make any sense.
			// Yet we've created these in btouch, so we need to define what they
			// actually do (nothing at all).

			Assert.That (Class.GetHandle ("TestProtocolRegister"), Is.EqualTo (NativeHandle.Zero));

			// However deriving from those nonsensical classes must do something
			// (at the very least because anything else would be a breaking change).
			Assert.That (Class.GetHandle ("DerivedTestProtocolRegister"), Is.Not.EqualTo (NativeHandle.Zero));
		}

		[Protocol]
		[Register ("TestProtocolRegister")]
		class TestProtocolRegister : NSObject { }

		[Register ("DerivedTestProtocolRegister")]
		class DerivedTestProtocolRegister : TestProtocolRegister { }

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
			protected E1 (NativeHandle ptr) : base (ptr)
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

			protected E2 (NativeHandle ptr) : base (ptr)
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
			protected G1 (NativeHandle ptr) : base (ptr)
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

			protected G2 (NativeHandle ptr) : base (ptr)
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
			}) {
				IsBackground = true,
			};
			thread.Start ();
			Assert.That (thread.Join (TimeSpan.FromSeconds (30)), Is.True, "Thread.Join timed out");
			GC.Collect ();
			GC.WaitForPendingFinalizers ();
			TestRuntime.RunAsync (TimeSpan.FromSeconds (30), () => { }, () => ObjCBlockTester.FreedBlockCount > initialFreedCount);
			Assert.That (ex, Is.Null, "No exceptions");
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
				Assert.That (obj.Value, Is.EqualTo (3), "a");
				Assert.That (obj.ctor1, Is.EqualTo ("foo"), "a ctor1");
				Assert.That (obj.ctor2, Is.EqualTo ("foo"), "a ctor2");
			} finally {
				Messaging.void_objc_msgSend (ptr, Selector.GetHandle ("release"));
			}

			try {
				// ctor inheritance: initWithBar: is only defined in the base class (managed base class)
				// In this case we create a managed wrapper of the base class, not the derived class. I'm not
				// sure this is by design or by accident, but that's how we're behaving right now at least
				ptr = Messaging.IntPtr_objc_msgSend (Class.GetHandle (typeof (D2)), Selector.GetHandle ("alloc"));
				ptr = Messaging.IntPtr_objc_msgSend_long (ptr, Selector.GetHandle ("initWithBar:"), 2);
				// Failed to marshal the Objective-C object 0x60000230a410 (type: MonoTouchFixtures_ObjCRuntime_RegistrarTest_D2). Could not find an existing managed instance for this object, nor was it possible to create a new managed instance (because the type 'MonoTouchFixtures.ObjCRuntime.RegistrarTest+D2' does not have a constructor that takes one NativeHandle argument
				var ex = Assert.Throws<RuntimeException> (() => Runtime.GetNSObject<D2> (ptr), "b ex");
				Assert.That (ex.Message, Does.Contain ("Could not find an existing managed instance for this object, nor was it possible to create a new managed instance (because the type 'MonoTouchFixtures.ObjCRuntime.RegistrarTest+D2' does not have a constructor that takes one"), "Exception message");
				var obj = Runtime.GetNSObject<D1> (ptr);
				Assert.That (obj.ctor1, Is.EqualTo ("bar"), "b ctor1");
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
				Assert.That (obj.Value, Is.EqualTo (3), "d");
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
				Assert.That (Runtime.TryGetNSObject (ptr), Is.Null, "e null");
				int rv = Messaging.int_objc_msgSend_int (ptr, Selector.GetHandle ("M1:"), 31415);
				Assert.That (Runtime.TryGetNSObject (ptr), Is.Not.Null, "e not null");
				Assert.That (rv, Is.EqualTo (31415), "e1");
				var obj = Runtime.GetNSObject<E2> (ptr);
				Assert.That (obj.Value, Is.EqualTo (3), "e2");
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
				Assert.That (Runtime.TryGetNSObject (ptr), Is.Null, "f null");
				int rv = Messaging.int_objc_msgSend_int (ptr, Selector.GetHandle ("M1:"), 31415);
				Assert.That (Runtime.TryGetNSObject (ptr), Is.Not.Null, "f not null");
				Assert.That (rv, Is.EqualTo (31415), "f1");
				var obj = Runtime.GetNSObject<G2> (ptr);
				Assert.That (obj.Value, Is.EqualTo (3), "f2");
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
				Assert.That (Runtime.TryGetNSObject (ptr), Is.Null, "g null");
				int rv = Messaging.int_objc_msgSend_int (ptr, Selector.GetHandle ("M2:"), 31415);
				Assert.That (Runtime.TryGetNSObject (ptr), Is.Not.Null, "g not null");
				Assert.That (rv, Is.EqualTo (31415), "g1");
				var obj = Runtime.GetNSObject<G2> (ptr);
				Assert.That (obj.Value, Is.EqualTo (3), "g2");
			} finally {
				Messaging.void_objc_msgSend (ptr, Selector.GetHandle ("release"));
			}
		}

		// This test uses Assembly.LoadFrom, which isn't supported with NativeAOT
#if __MACOS__ && !NATIVEAOT
		[Test]
		[UnconditionalSuppressMessage ("Trimming", "IL2026", Justification = "This test loads an assembly dynamically, so it's expected to not be trimmer safe. It works though, so unless something changes, we're going to assume it's trimmer-compatible.")]
		[UnconditionalSuppressMessage ("Trimming", "IL2072", Justification = "This test loads an assembly dynamically, so it's expected to not be trimmer safe. It works though, so unless something changes, we're going to assume it's trimmer-compatible.")]
		public void CustomUserTypeWithDynamicallyLoadedAssembly ()
		{
			if (!global::Xamarin.Tests.Configuration.TryGetRootPath (out var rootPath))
				Assert.Ignore ("This test must be executed a source checkout.");

			var customTypeAssemblyPath = global::System.IO.Path.Combine (rootPath, "tests", "test-libraries", "custom-type-assembly", ".libs", "dotnet", "macos", "custom-type-assembly.dll");
			Assert.That (customTypeAssemblyPath, Does.Exist, "existence");

			var size = 10;
			var handles = new GCHandle [size];
			var array = new NSMutableArray ();

			// Create a bunch instances of a custom object the static registrar didn't know about at build time.
			// We do this on a different thread to prevent the GC from finding these instances on the main thread's stack.
			var thread = new Thread (() => {
				var customTypeAssembly = global::System.Reflection.Assembly.LoadFrom (customTypeAssemblyPath);
				var customType = customTypeAssembly.GetType ("MyCustomType");
				for (var i = 0; i < size; i++) {
					var obj = (NSObject) global::System.Activator.CreateInstance (customType);
					array.Add (obj);
					handles [i] = GCHandle.Alloc (obj, GCHandleType.Weak);
				}
				// Run the GC a couple of times.
				GC.Collect ();
				GC.WaitForPendingFinalizers ();
				GC.Collect ();
				GC.WaitForPendingFinalizers ();
			}) {
				IsBackground = true,
				Name = "CustomUserTypeWithDynamicallyLoadedAssembly",
			};
			thread.Start ();
			Assert.That (thread.Join (TimeSpan.FromSeconds (30)), Is.True, "Background thread done");

			// Run the main loop for a little while.
			var counter = size;
			TestRuntime.RunAsync (TimeSpan.FromSeconds (10), () => { }, () => counter-- <= 0);

			// Verify that none of the managed instances have been collected by the GC:
			for (var i = 0; i < size; i++) {
				Assert.That (handles [i].Target, Is.Not.Null, $"Target #{i}");
				((NSObject) handles [i].Target).Dispose ();
			}

			// Make sure the GC doesn't collect our array of custom objects.
			array.Dispose ();
			GC.KeepAlive (array);
		}
#endif

#if !MONOMAC
		class Bug28757A : NSObject, IUITableViewDataSource {
			public virtual nint RowsInSection (UITableView tableView, nint section)
			{
				return 1;
			}
			public UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				return null;
			}
		}

		class Bug28757B : Bug28757A {
			public override nint RowsInSection (UITableView tableView, nint section)
			{
				return 2;
			}
		}
#endif // !MONOMAC

#if !MONOMAC
		[Test]
		public void TestInheritedProtocols ()
		{
			using (var obj = new Bug28757B ()) {
				Assert.That (Messaging.nint_objc_msgSend_IntPtr_nint (obj.Handle, Selector.GetHandle ("tableView:numberOfRowsInSection:"), IntPtr.Zero, 0), Is.EqualTo ((nint) 2), "#test");
			}
		}
#endif // !MONOMAC

#if !MONOMAC
		[Test]
		public void InOutProtocolMethodArgument ()
		{
			using (var obj = new Scroller ()) {
				var velocity = new CGPoint (1, 2);
				var targetContentOffset = new CGPoint (3, 4);
				Messaging.void_objc_msgSend_IntPtr_CGPoint_ref_CGPoint (obj.Handle, Selector.GetHandle ("scrollViewWillEndDragging:withVelocity:targetContentOffset:"), IntPtr.Zero, velocity, ref targetContentOffset);
				Console.WriteLine (targetContentOffset);
				Assert.That (targetContentOffset.ToString (), Is.EqualTo ("{123, 345}"), "ref output");
			}
		}
#endif // !MONOMAC

#if !MONOMAC
		class Scroller : NSObject, IUIScrollViewDelegate {
			[Export ("scrollViewWillEndDragging:withVelocity:targetContentOffset:")]
			public void WillEndDragging (UIScrollView scrollView, PointF velocity, ref PointF targetContentOffset)
			{
				Assert.That (velocity.ToString (), Is.EqualTo ("{1, 2}"), "velocity");
				Assert.That (targetContentOffset.ToString (), Is.EqualTo ("{3, 4}"), "targetContentOffset");
				targetContentOffset = new CGPoint (123, 345);
			}
		}
#endif // !MONOMAC

#if HAS_ADDRESSBOOK && HAS_ADDRESSBOOKUI
		[Test]
		public void VoidPtrToINativeObjectArgument ()
		{
			// The API here was introduced to Mac Catalyst later than for the other frameworks, so we have this additional check
			TestRuntime.AssertSystemVersion (ApplePlatform.MacCatalyst, 14, 0, throwIfOtherPlatform: false);

			using (var obj = new ABPeoplePickerNavigationControllerDelegateImpl ()) {
				using (var person = new ABPerson ()) {
					Messaging.void_objc_msgSend_IntPtr_IntPtr (obj.Handle, Selector.GetHandle ("peoplePickerNavigationController:didSelectPerson:"), IntPtr.Zero, person.Handle);
					Assert.That (obj.personHandle, Is.EqualTo (person.Handle), "1");
				}
			}
		}

		class ABPeoplePickerNavigationControllerDelegateImpl : ABPeoplePickerNavigationControllerDelegate {
			public NativeHandle personHandle;
			public override void DidSelectPerson (ABPeoplePickerNavigationController peoplePicker, ABPerson selectedPerson)
			{
				personHandle = selectedPerson.Handle;
			}
		}
#endif // HAS_ADDRESSBOOKUI

#if !__TVOS__ // No Contacts framework in TVOS
		[Test]
		public void GenericAPI ()
		{
			TestRuntime.AssertXcodeVersion (7, 0);

			using (var contact = new CNMutableContact ()) {
				var dt = new NSDateComponents () {
					Year = 1923,
				};
				var handle = Messaging.IntPtr_objc_msgSend_IntPtr_IntPtr (Class.GetHandle (typeof (CNLabeledValue<>)), Selector.GetHandle ("labeledValueWithLabel:value:"), IntPtr.Zero, dt.Handle);
				var array = Messaging.IntPtr_objc_msgSend_IntPtr (Class.GetHandle (typeof (NSArray)), Selector.GetHandle ("arrayWithObject:"), handle);
				Messaging.void_objc_msgSend_IntPtr (contact.Handle, Selector.GetHandle ("setDates:"), array);

				Assert.That (contact.Dates [0].Value.Year, Is.EqualTo ((nint) 1923), "Dates");
			}

			using (var contact = new SubclassedContact ()) {
				var dates = Messaging.IntPtr_objc_msgSend (contact.Handle, Selector.GetHandle ("dates"));
				var obj = Runtime.GetNSObject (dates);
				Assert.That (obj.GetType (), Is.EqualTo (typeof (NSArray)), "2 date type");
				var arr = (NSArray) obj;
				Assert.That (arr.Count, Is.EqualTo ((nuint) 1), "2 count");
			}

			using (var contact = new SubclassedContact ()) {
				var dates = Messaging.IntPtr_objc_msgSend (contact.Handle, Selector.GetHandle ("dates"));
				var arr = NSArray.ArrayFromHandle<CNLabeledValue<NSDateComponents>> (dates);
				Assert.That (arr.Length, Is.EqualTo (1), "3 length");
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
#endif // !__TVOS__

		[Test]
		public void Bug34224 ()
		{
			using (var obj = new Bug34224Class ()) {
				IntPtr ptr = new IntPtr (123);
				Messaging.void_objc_msgSend_ref_IntPtr (obj.Handle, Selector.GetHandle ("ref:"), ref ptr);
				Assert.That (ptr, Is.EqualTo (new IntPtr (456)), "# ref");

				Messaging.void_objc_msgSend_out_IntPtr (obj.Handle, Selector.GetHandle ("out:"), out ptr);
				Assert.That (ptr, Is.EqualTo (new IntPtr (567)), "# out");
			}
		}

		// Bug 34224
		class Bug34224Class : NSObject {
			[Export ("ref:")]
			public void Ref (ref IntPtr p1)
			{
				Assert.That (p1, Is.EqualTo (new IntPtr (123)), "ref C");
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
				var ptr = (IntPtr) Messaging.IntPtr_objc_msgSend (obj.Handle, Selector.GetHandle ("bug34440"));
				Assert.That (ptr, Is.EqualTo (Selector.GetHandle ("bug34440")), "selector");
				ptr = Messaging.IntPtr_objc_msgSend (obj.Handle, Selector.GetHandle ("classReturn"));
				Assert.That ((IntPtr) ptr, Is.EqualTo ((IntPtr) Class.GetHandle (typeof (Bug34440Class))), "class");
			}
		}

		[Test]
		public void BlockReturnTest ()
		{
			using (var obj = new BlockReturnTestClass ()) {
				Assert.That (obj.TestBlocks (), Is.True, "TestBlocks");
			}
		}

		class BlockReturnTestClass : ObjCRegistrarTest {
			public override RegistrarTestBlock MethodReturningBlock ()
			{
				return v => {
					Assert.That (v, Is.EqualTo (0xdeadf00d), "input");
					return 0x1337b001;
				};
			}

			public override RegistrarTestBlock PropertyReturningBlock {
				get {
					return v => {
						Assert.That (v, Is.EqualTo (0xdeadf11d), "input");
						return 0x7b001133;
					};
				}
			}
		}

		[Test]
		public void PropertySetters ()
		{
			var cls = Class.GetHandle (typeof (PropertySetterTestClass));
			Assert.That (class_getInstanceMethod (cls, Selector.GetHandle ("setá:")), Is.Not.EqualTo (IntPtr.Zero), "a 1");
			using (var obj = new PropertySetterTestClass ()) {
				obj.SetValueForKey (new NSString ("AAA"), (NSString) "á");
				Assert.That ((string) (NSString) obj.ValueForKey ((NSString) "á"), Is.EqualTo ("AAA"), "A getvalue");
				Assert.That (obj.A, Is.EqualTo ("AAA"), "A setvalue");

				obj.SetValueForKey (new NSString ("BBB"), (NSString) "b");
				Assert.That ((string) (NSString) obj.ValueForKey ((NSString) "b"), Is.EqualTo ("BBB"), "B getvalue");
				Assert.That (obj.B, Is.EqualTo ("BBB"), "B setvalue");
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
			CtorChaining2.InitializedInstance = null;
			try {
				using (var obj = new CtorChaining2 (2)) {
					Assert.That (obj.InitCalled, Is.True, "Init called");
					Assert.That (obj.InitCallsInitCalled, Is.True, "InitCallsInit called");
					Assert.That (CtorChaining2.InitializedInstance, Is.SameAs (obj), "Initialized instance");
					Assert.That (obj.RetainCount, Is.EqualTo ((nuint) 1), "Retain count");
				}
			} finally {
				CtorChaining2.InitializedInstance = null;
			}
		}

		class CtorChaining2 : CtorChaining1 {
			public static CtorChaining2 InitializedInstance;

			public CtorChaining2 ()
			{
				InitializedInstance = this;
			}

			public CtorChaining2 (int value)
				: base (value)
			{
			}
		}

		[DllImport ("/usr/lib/libobjc.dylib")]
		static extern IntPtr class_getInstanceMethod (IntPtr cls, IntPtr sel);

		[Test]
		public void OutOverriddenWithoutOutAttribute ()
		{
			using (var tmp = new NSObject ()) {
				using (var obj = new Registrar_OutExportDerivedClass ()) {
					IntPtr tmpH = tmp.Handle;
					var rv = Messaging.IntPtr_objc_msgSend_ref_IntPtr (obj.Handle, Selector.GetHandle ("func:"), ref tmpH);
					Assert.That (tmpH, Is.EqualTo (IntPtr.Zero), "input");
					Assert.That (rv, Is.EqualTo (IntPtr.Zero), "output");
				}
			}
		}

		class ProtocolArgumentClass : NSObject {
			[Export ("someMethod:")]
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
				Assert.That (ptr, Is.EqualTo (nsobjProtocol), "result");
				Assert.That (ptr, Is.Not.EqualTo (IntPtr.Zero), "nsobject");
			}
		}

		class Bug41319 : NSObject {
			[Export ("initWithCoder:")]
			public Bug41319 (NSCoder coder)
			{
			}
		}

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

		class NullOutParameters : ObjCRegistrarTest {
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
				Assert.That (str, Is.Not.Null, "NonNull NSString&");
				Assert.That (str.ToString (), Is.EqualTo ("ByrefParameter"));
			}
		}

		class ArrayTester : ObjCRegistrarTest {
			public string [] ManagedStringArrayProperty;
			public NSObject [] ManagedNSObjectArrayProperty;
			public INSCoding [] ManagedINSCodingArrayProperty;

			public override string [] StringArrayProperty {
				get {
					ManagedStringArrayProperty = base.StringArrayProperty;
					return ManagedStringArrayProperty;
				}
				set {
					base.StringArrayProperty = value;
					ManagedStringArrayProperty = value;
				}
			}

			public override string [] GetStringArrayMethod ()
			{
				ManagedStringArrayProperty = base.GetStringArrayMethod ();
				return ManagedStringArrayProperty;
			}

			public override void SetStringArrayMethod (string [] array)
			{
				ManagedStringArrayProperty = array;
				base.SetStringArrayMethod (ManagedStringArrayProperty);
			}

			public override NSObject [] NSObjectArrayProperty {
				get {
					ManagedNSObjectArrayProperty = base.NSObjectArrayProperty;
					return ManagedNSObjectArrayProperty;
				}
				set {
					base.NSObjectArrayProperty = value;
					ManagedNSObjectArrayProperty = value;
				}
			}

			public override NSObject [] GetNSObjectArrayMethod ()
			{
				ManagedNSObjectArrayProperty = base.GetNSObjectArrayMethod ();
				return ManagedNSObjectArrayProperty;
			}

			public override void SetNSObjectArrayMethod (NSObject [] array)
			{
				ManagedNSObjectArrayProperty = array;
				base.SetNSObjectArrayMethod (array);
			}

			public override INSCoding [] INSCodingArrayProperty {
				get {
					ManagedINSCodingArrayProperty = base.INSCodingArrayProperty;
					return ManagedINSCodingArrayProperty;
				}
				set {
					base.INSCodingArrayProperty = value;
					ManagedINSCodingArrayProperty = value;
				}
			}

			public override INSCoding [] GetINSCodingArrayMethod ()
			{
				ManagedINSCodingArrayProperty = base.GetINSCodingArrayMethod ();
				return INSCodingArrayProperty;
			}

			public override void SetINSCodingArrayMethod (INSCoding [] array)
			{
				INSCodingArrayProperty = array;
				base.SetINSCodingArrayMethod (array);
			}
		}


		[Test]
		public void TestStringArray ()
		{
			var items = 10000; // Big enough to make the GC run while we're marshalling.
			var array = new string [items];
			for (var i = 0; i < array.Length; i++)
				array [i] = i.ToString ();

			using (var obj = new ObjCRegistrarTest ()) {
				obj.StringArrayProperty = array;
				Assert.That (obj.StringArrayProperty, Is.EqualTo (array), "1");
				obj.SetStringArrayMethod (null);
				Assert.That (obj.StringArrayProperty, Is.Null, "2");
				obj.SetStringArrayMethod (array);
				Assert.That (obj.StringArrayProperty, Is.EqualTo (array), "3");
				var rv = obj.GetStringArrayMethod ();
				Assert.That (rv, Is.EqualTo (array), "4");
			}

			using (var arrayObj = NSArray.FromStrings (array)) {
				using (var obj = new ArrayTester ()) {
					obj.ManagedStringArrayProperty = null;
					Messaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("setStringArrayProperty:"), arrayObj.Handle);
					Assert.That (array, Is.EqualTo (obj.ManagedStringArrayProperty), "1B");
					obj.ManagedStringArrayProperty = null;
					Messaging.IntPtr_objc_msgSend (obj.Handle, Selector.GetHandle ("stringArrayProperty"));
					Assert.That (array, Is.EqualTo (obj.ManagedStringArrayProperty), "2B");

					obj.ManagedStringArrayProperty = null;
					Messaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("setStringArrayMethod:"), arrayObj.Handle);
					Assert.That (array, Is.EqualTo (obj.ManagedStringArrayProperty), "3B");
					obj.ManagedStringArrayProperty = null;
					Messaging.IntPtr_objc_msgSend (obj.Handle, Selector.GetHandle ("getStringArrayMethod"));
					Assert.That (array, Is.EqualTo (obj.ManagedStringArrayProperty), "4B");
				}
			}
		}

		[Test]
		public void TestNSObjectArray ()
		{
			var items = 10000; // Big enough to make the GC run while we're marshalling.
			var array = new NSObject [items];
			for (var i = 0; i < array.Length; i++)
				array [i] = NSNumber.FromInt32 (i);

			using (var obj = new ObjCRegistrarTest ()) {
				obj.NSObjectArrayProperty = array;
				Assert.That (obj.NSObjectArrayProperty, Is.EqualTo (array), "1");
				obj.SetNSObjectArrayMethod (null);
				Assert.That (obj.NSObjectArrayProperty, Is.Null, "2");
				obj.SetNSObjectArrayMethod (array);
				Assert.That (obj.NSObjectArrayProperty, Is.EqualTo (array), "3");
				var rv = obj.GetNSObjectArrayMethod ();
				Assert.That (rv, Is.EqualTo (array), "4");
			}

			using (var arrayObj = NSArray.FromNSObjects (array)) {
				using (var obj = new ArrayTester ()) {
					obj.ManagedNSObjectArrayProperty = null;
					Messaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("setNsobjectArrayProperty:"), arrayObj.Handle);
					Assert.That (array, Is.EqualTo (obj.ManagedNSObjectArrayProperty), "1B");
					obj.ManagedNSObjectArrayProperty = null;
					Messaging.IntPtr_objc_msgSend (obj.Handle, Selector.GetHandle ("nsobjectArrayProperty"));
					Assert.That (array, Is.EqualTo (obj.ManagedNSObjectArrayProperty), "2B");

					obj.ManagedNSObjectArrayProperty = null;
					Messaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("setNSObjectArrayMethod:"), arrayObj.Handle);
					Assert.That (array, Is.EqualTo (obj.ManagedNSObjectArrayProperty), "3B");
					obj.ManagedNSObjectArrayProperty = null;
					Messaging.IntPtr_objc_msgSend (obj.Handle, Selector.GetHandle ("getNSObjectArrayMethod"));
					Assert.That (array, Is.EqualTo (obj.ManagedNSObjectArrayProperty), "4B");
				}
			}
		}

		[Test]
		public void TestINSCodingArray ()
		{
			var items = 10000; // Big enough to make the GC run while we're marshalling.
			var array = new INSCoding [items];
			for (var i = 0; i < array.Length; i++)
				array [i] = NSNumber.FromInt32 (i);

			using (var obj = new ObjCRegistrarTest ()) {
				obj.INSCodingArrayProperty = array;
				Assert.That (obj.INSCodingArrayProperty, Is.EqualTo (array), "1");
				obj.SetINSCodingArrayMethod (null);
				Assert.That (obj.INSCodingArrayProperty, Is.Null, "2");
				obj.SetINSCodingArrayMethod (array);
				Assert.That (obj.INSCodingArrayProperty, Is.EqualTo (array), "3");
				var rv = obj.GetINSCodingArrayMethod ();
				Assert.That (rv, Is.EqualTo (array), "4");
			}

			using (var arrayObj = NSArray.FromNSObjects<INSCoding> (array)) {
				using (var obj = new ArrayTester ()) {
					obj.ManagedINSCodingArrayProperty = null;
					Messaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("setINSCodingArrayProperty:"), arrayObj.Handle);
					Assert.That (array, Is.EqualTo (obj.ManagedINSCodingArrayProperty), "1B");
					obj.ManagedINSCodingArrayProperty = null;
					Messaging.IntPtr_objc_msgSend (obj.Handle, Selector.GetHandle ("INSCodingArrayProperty"));
					Assert.That (array, Is.EqualTo (obj.ManagedINSCodingArrayProperty), "2B");

					obj.ManagedINSCodingArrayProperty = null;
					Messaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("setINSCodingArrayMethod:"), arrayObj.Handle);
					Assert.That (array, Is.EqualTo (obj.ManagedINSCodingArrayProperty), "3B");
					obj.ManagedINSCodingArrayProperty = null;
					Messaging.IntPtr_objc_msgSend (obj.Handle, Selector.GetHandle ("getINSCodingArrayMethod"));
					Assert.That (array, Is.EqualTo (obj.ManagedINSCodingArrayProperty), "4B");
				}
			}
		}
		[Test]
		public void RefOutTest_CFBundle ()
		{
			var refValue = NativeHandle.Zero;
			var outValue = NativeHandle.Zero;

			using (var obj = new RefOutParametersSubclass ()) {
				var sel = Selector.GetHandle ("testCFBundle:a:b:");
				var dummyObj = CFBundle.GetMain ();
				CFBundle refObj = null;
				CFBundle outObj = null;
				int action;

				/// 1: set both to null
				action = 1;

				// native
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				obj.TestCFBundle (action << 0, ref refObj, out outObj);
				Assert.That (refObj, Is.Null, "CFBundle-1A-ref");
				Assert.That (outObj, Is.Null, "CFBundle-1A-out");

				// managed
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				obj.TestCFBundle (action << 8, ref refObj, out outObj);
				Assert.That (refObj, Is.Null, "CFBundle-1M-ref");
				Assert.That (outObj, Is.Null, "CFBundle-1M-out");

				// direct native
				refValue = dummyObj.Handle; // set to non-null
				outValue = dummyObj.Handle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 0, ref refValue, out outValue);
				Assert.That (refValue, Is.EqualTo (NativeHandle.Zero), "CFBundle-1DA-ref");
				Assert.That (outValue, Is.EqualTo (NativeHandle.Zero), "CFBundle-1DA-out");

				// direct managed
				refValue = dummyObj.Handle; // set to non-null
				outValue = dummyObj.Handle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 8, ref refValue, out outValue);
				Assert.That (refValue, Is.EqualTo (NativeHandle.Zero), "CFBundle-1DM-ref");
				Assert.That (outValue, Is.EqualTo (NativeHandle.Zero), "CFBundle-1DM-out");

				/// 2: verify that refValue points to something
				action = 2;

				// native
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				obj.TestCFBundle (action << 0, ref refObj, out outObj);
				Assert.That (refObj.Handle, Is.EqualTo (dummyObj.Handle), "CFBundle-2A-ref");
				Assert.That (refObj, Is.SameAs (dummyObj), "CBundle-2A-ref-same");
				Assert.That (outObj, Is.Null, "CFBundle-2A-out");

				// managed
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				obj.TestCFBundle (action << 8, ref refObj, out outObj);
				Assert.That (refObj.Handle, Is.EqualTo (dummyObj.Handle), "CFBundle-2M-ref");
				Assert.That (refObj, Is.SameAs (dummyObj), "CBundle-2M-ref-same");
				Assert.That (outObj, Is.Null, "CFBundle-2M-out");

				// direct native
				refValue = dummyObj.Handle; // set to non-null
				outValue = dummyObj.Handle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 0, ref refValue, out outValue);
				Assert.That (refValue, Is.EqualTo (dummyObj.Handle), "CFBundle-2DA-ref");
				Assert.That (outValue, Is.EqualTo (NativeHandle.Zero), "CFBundle-2DA-out");

				// direct managed
				refValue = dummyObj.Handle; // set to non-null
				outValue = dummyObj.Handle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 8, ref refValue, out outValue);
				Assert.That (refValue, Is.EqualTo (dummyObj.Handle), "CFBundle-2DM-ref");
				Assert.That (outValue, Is.EqualTo (NativeHandle.Zero), "CFBundle-2DM-out");


				/// 3 set both parameteres to the same pointer of a CFBundle
				action = 3;

				// native
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				obj.TestCFBundle (action << 0, ref refObj, out outObj);
				Assert.That (refObj.Handle, Is.EqualTo (dummyObj.Handle), "CFBundle-3A-ref");
				Assert.That (refObj, Is.SameAs (dummyObj), "CBundle-3A-ref-same");
				Assert.That (outObj.Handle, Is.EqualTo (dummyObj.Handle), "CFBundle-3A-out");
				Assert.That (outObj, Is.Not.SameAs (dummyObj), "CBundle-3A-ref-out");

				// managed
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				obj.TestCFBundle (action << 8, ref refObj, out outObj);
				Assert.That (refObj.Handle, Is.EqualTo (dummyObj.Handle), "CFBundle-3M-ref");
				Assert.That (refObj, Is.Not.SameAs (dummyObj), "CBundle-3M-ref-same");
				Assert.That (outObj.Handle, Is.EqualTo (dummyObj.Handle), "CFBundle-3M-out");
				Assert.That (outObj, Is.Not.SameAs (dummyObj), "CBundle-3M-ref-out");

				// direct native
				refValue = dummyObj.Handle; // set to non-null
				outValue = dummyObj.Handle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 0, ref refValue, out outValue);
				Assert.That (refValue, Is.EqualTo (dummyObj.Handle), "CFBundle-3DA-ref");
				Assert.That (outValue, Is.EqualTo (dummyObj.Handle), "CFBundle-3DA-out");

				// direct managed
				refValue = dummyObj.Handle; // set to non-null
				outValue = dummyObj.Handle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 8, ref refValue, out outValue);
				Assert.That (refValue, Is.EqualTo (dummyObj.Handle), "CFBundle-3DM-ref");
				Assert.That (outValue, Is.EqualTo (dummyObj.Handle), "CFBundle-3DM-out");


				/// 4 set both parameteres to different pointers of a CFBundle
				action = 4;

				// native
				refObj = null; // set to null
				outObj = null; // set to null
				obj.TestCFBundle (action << 0, ref refObj, out outObj);
				Assert.That (refObj.Handle, Is.Not.EqualTo (NativeHandle.Zero), "CFBundle-4A-ref");
				Assert.That (outObj.Handle, Is.Not.EqualTo (NativeHandle.Zero), "CFBundle-4A-out");
				Assert.That (outObj.Handle, Is.Not.EqualTo (refObj.Handle), "CBundle-4A-ref-distinct");

				// managed
				refObj = null; // set to null
				outObj = null; // set to null
				obj.TestCFBundle (action << 8, ref refObj, out outObj);
				Assert.That (refObj.Handle, Is.Not.EqualTo (NativeHandle.Zero), "CFBundle-4M-ref");
				Assert.That (outObj.Handle, Is.Not.EqualTo (NativeHandle.Zero), "CFBundle-4M-out");
				Assert.That (outObj.Handle, Is.Not.EqualTo (refObj.Handle), "CBundle-4M-ref-distinct");

				// direct native
				refValue = NativeHandle.Zero; // set to null
				outValue = NativeHandle.Zero; // set to null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 0, ref refValue, out outValue);
				Assert.That (refValue, Is.Not.EqualTo (NativeHandle.Zero), "CFBundle-4DA-ref");
				Assert.That (outValue, Is.Not.EqualTo (NativeHandle.Zero), "CFBundle-4DA-out");
				Assert.That (outValue, Is.Not.EqualTo (refValue), "CBundle-4DA-ref-distinct");

				// direct managed
				refValue = NativeHandle.Zero; // set to null
				outValue = NativeHandle.Zero; // set to null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 8, ref refValue, out outValue);
				Assert.That (refValue, Is.Not.EqualTo (NativeHandle.Zero), "CFBundle-4DM-ref");
				Assert.That (outValue, Is.Not.EqualTo (NativeHandle.Zero), "CFBundle-4DM-out");
				Assert.That (outValue, Is.Not.EqualTo (refValue), "CBundle-4DM-ref-distinct");
			}
		}

		[Test]
		public void RefOutTest_INSCoding ()
		{
			var refValue = NativeHandle.Zero;
			var outValue = NativeHandle.Zero;

			using (var obj = new RefOutParametersSubclass ()) {
				var sel = Selector.GetHandle ("testINSCoding:a:b:");
				INSCoding dummyObj = new NSString ("Dummy obj");
				INSCoding refObj = null;
				INSCoding outObj = null;
				int action;

				/// 1: set both to null
				action = 1;

				// native
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				obj.TestINSCoding (action << 0, ref refObj, out outObj);
				Assert.That (refObj, Is.Null, "NSCoding-1A-ref");
				Assert.That (outObj, Is.Null, "NSCoding-1A-out");

				// managed
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				obj.TestINSCoding (action << 8, ref refObj, out outObj);
				Assert.That (refObj, Is.Null, "NSCoding-1M-ref");
				Assert.That (outObj, Is.Null, "NSCoding-1M-out");

				// direct native
				refValue = dummyObj.Handle; // set to non-null
				outValue = dummyObj.Handle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 0, ref refValue, out outValue);
				Assert.That (refValue, Is.EqualTo (NativeHandle.Zero), "NSCoding-1DA-ref");
				Assert.That (outValue, Is.EqualTo (NativeHandle.Zero), "NSCoding-1DA-out");

				// direct managed
				refValue = dummyObj.Handle; // set to non-null
				outValue = dummyObj.Handle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 8, ref refValue, out outValue);
				Assert.That (refValue, Is.EqualTo (NativeHandle.Zero), "NSCoding-1DM-ref");
				Assert.That (outValue, Is.EqualTo (NativeHandle.Zero), "NSCoding-1DM-out");

				/// 2: verify that refValue points to something
				action = 2;

				// native
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				obj.TestINSCoding (action << 0, ref refObj, out outObj);
				Assert.That (refObj.Handle, Is.EqualTo (dummyObj.Handle), "NSCoding-2A-ref");
				Assert.That (refObj, Is.SameAs (dummyObj), "NSCoding-2A-ref-same");
				Assert.That (outObj, Is.Null, "NSCoding-2A-out");

				// managed
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				obj.TestINSCoding (action << 8, ref refObj, out outObj);
				Assert.That (refObj.Handle, Is.EqualTo (dummyObj.Handle), "NSCoding-2M-ref");
				Assert.That (refObj, Is.SameAs (dummyObj), "NSCoding-2M-ref-same");
				Assert.That (outObj, Is.Null, "NSCoding-2M-out");

				// direct native
				refValue = dummyObj.Handle; // set to non-null
				outValue = dummyObj.Handle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 0, ref refValue, out outValue);
				Assert.That (refValue, Is.EqualTo (dummyObj.Handle), "NSCoding-2DA-ref");
				Assert.That (outValue, Is.EqualTo (NativeHandle.Zero), "NSCoding-2DA-out");

				// direct managed
				refValue = dummyObj.Handle; // set to non-null
				outValue = dummyObj.Handle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 8, ref refValue, out outValue);
				Assert.That (refValue, Is.EqualTo (dummyObj.Handle), "NSCoding-2DM-ref");
				Assert.That (outValue, Is.EqualTo (NativeHandle.Zero), "NSCoding-2DM-out");


				/// 3 set both parameteres to the same pointer of a NSCoding
				action = 3;

				// native
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				obj.TestINSCoding (action << 0, ref refObj, out outObj);
				Assert.That (refObj.Handle, Is.Not.EqualTo (dummyObj.Handle), "NSCoding-3A-ref");
				Assert.That (refObj, Is.Not.SameAs (dummyObj), "NSCoding-3A-ref-same");
				Assert.That (outObj.Handle, Is.Not.EqualTo (dummyObj.Handle), "NSCoding-3A-out");
				Assert.That (outObj, Is.Not.SameAs (dummyObj), "NSCoding-3A-ref-out");
				Assert.That (outObj.Handle, Is.EqualTo (refObj.Handle), "NSCoding-3A-out-ref-eq");
				Assert.That (outObj, Is.Not.SameAs (refObj), "NSCoding-3A-ref-out-not-safe");
				Assert.That (refObj.GetType ().FullName, Does.Contain ("CodingWrapper"), "NSCoding-3A-ref-wrapper-type");
				Assert.That (outObj.GetType ().FullName, Does.Contain ("CodingWrapper"), "NSCoding-3A-ref-wrapper-type");

				// managed
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				obj.TestINSCoding (action << 8, ref refObj, out outObj);
				Assert.That (refObj.Handle, Is.Not.EqualTo (dummyObj.Handle), "NSCoding-3M-ref");
				Assert.That (refObj, Is.Not.SameAs (dummyObj), "NSCoding-3M-ref-same");
				Assert.That (outObj.Handle, Is.Not.EqualTo (dummyObj.Handle), "NSCoding-3M-out");
				Assert.That (outObj, Is.Not.SameAs (dummyObj), "NSCoding-3M-ref-out");
				Assert.That (outObj.Handle, Is.EqualTo (refObj.Handle), "NSCoding-3M-out-ref-eq");
				Assert.That (outObj, Is.SameAs (refObj), "NSCoding-3M-ref-out-not-safe");
				Assert.That (refObj, Is.TypeOf<NSString> (), "NSCoding-3M-ref-wrapper-type");
				Assert.That (outObj, Is.TypeOf<NSString> (), "NSCoding-3M-ref-wrapper-type");

				// direct native
				refValue = dummyObj.Handle; // set to non-null
				outValue = dummyObj.Handle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 0, ref refValue, out outValue);
				Assert.That (refValue, Is.Not.EqualTo (dummyObj.Handle), "NSCoding-3DA-ref");
				Assert.That (outValue, Is.Not.EqualTo (dummyObj.Handle), "NSCoding-3DA-out");
				Assert.That (outObj, Is.SameAs (refObj), "NSCoding-3DA-out-ref-same");
				Assert.That (refObj, Is.TypeOf<NSString> (), "NSCoding-3DA-ref-wrapper-type");
				Assert.That (outObj, Is.TypeOf<NSString> (), "NSCoding-3DA-ref-wrapper-type");

				// direct managed
				refValue = dummyObj.Handle; // set to non-null
				outValue = dummyObj.Handle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 8, ref refValue, out outValue);
				Assert.That (refValue, Is.Not.EqualTo (dummyObj.Handle), "NSCoding-3DM-ref");
				Assert.That (outValue, Is.Not.EqualTo (dummyObj.Handle), "NSCoding-3DM-out");
				Assert.That (outObj, Is.SameAs (refObj), "NSCoding-3DM-out-ref-eq");
				Assert.That (refObj, Is.TypeOf<NSString> (), "NSCoding-3DM-ref-wrapper-type");
				Assert.That (outObj, Is.TypeOf<NSString> (), "NSCoding-3DM-ref-wrapper-type");


				/// 4 set both parameteres to different pointers of a NSCoding
				action = 4;

				// native
				refObj = null; // set to null
				outObj = null; // set to null
				obj.TestINSCoding (action << 0, ref refObj, out outObj);
				Assert.That (refObj.Handle, Is.Not.EqualTo (NativeHandle.Zero), "NSCoding-4A-ref");
				Assert.That (outObj.Handle, Is.Not.EqualTo (NativeHandle.Zero), "NSCoding-4A-out");
				Assert.That (outObj.Handle, Is.Not.EqualTo (refObj.Handle), "NSCoding-4A-ref-distinct");
				Assert.That (refObj.GetType ().FullName, Does.Contain ("CodingWrapper"), "NSCoding-4A-ref-wrapper-type");
				Assert.That (outObj.GetType ().FullName, Does.Contain ("CodingWrapper"), "NSCoding-4A-ref-wrapper-type");

				// managed
				refObj = null; // set to null
				outObj = null; // set to null
				obj.TestINSCoding (action << 8, ref refObj, out outObj);
				Assert.That (refObj.Handle, Is.Not.EqualTo (NativeHandle.Zero), "NSCoding-4M-ref");
				Assert.That (outObj.Handle, Is.Not.EqualTo (NativeHandle.Zero), "NSCoding-4M-out");
				Assert.That (outObj.Handle, Is.Not.EqualTo (refObj.Handle), "NSCoding-4M-ref-distinct");
				Assert.That (refObj, Is.TypeOf<NSString> (), "NSCoding-4M-ref-wrapper-type");
				Assert.That (outObj, Is.TypeOf<NSString> (), "NSCoding-4M-ref-wrapper-type");

				// direct native
				refValue = NativeHandle.Zero; // set to null
				outValue = NativeHandle.Zero; // set to null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 0, ref refValue, out outValue);
				Assert.That (refValue, Is.Not.EqualTo (NativeHandle.Zero), "NSCoding-4DA-ref");
				Assert.That (outValue, Is.Not.EqualTo (NativeHandle.Zero), "NSCoding-4DA-out");
				Assert.That (outValue, Is.Not.EqualTo (refValue), "NSCoding-4DA-ref-distinct");
				Assert.That (refObj, Is.TypeOf<NSString> (), "NSCoding-4DA-ref-wrapper-type");
				Assert.That (outObj, Is.TypeOf<NSString> (), "NSCoding-4DA-ref-wrapper-type");

				// direct managed
				refValue = NativeHandle.Zero; // set to null
				outValue = NativeHandle.Zero; // set to null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 8, ref refValue, out outValue);
				Assert.That (refValue, Is.Not.EqualTo (NativeHandle.Zero), "NSCoding-4DM-ref");
				Assert.That (outValue, Is.Not.EqualTo (NativeHandle.Zero), "NSCoding-4DM-out");
				Assert.That (outValue, Is.Not.EqualTo (refValue), "NSCoding-4DM-ref-distinct");
				Assert.That (refObj, Is.TypeOf<NSString> (), "NSCoding-4DM-ref-wrapper-type");
				Assert.That (outObj, Is.TypeOf<NSString> (), "NSCoding-4DM-ref-wrapper-type");
			}
		}

		[Test]
		public void RefOutTest_NSObject ()
		{
			var refValue = NativeHandle.Zero;
			var outValue = NativeHandle.Zero;

			using (var obj = new RefOutParametersSubclass ()) {
				var sel = Selector.GetHandle ("testNSObject:a:b:");
				NSObject dummyObj = new NSString ("Dummy obj");
				NSObject refObj = null;
				NSObject outObj = null;
				int action;

				/// 1: set both to null
				action = 1;

				// native
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				obj.TestNSObject (action << 0, ref refObj, out outObj);
				Assert.That (refObj, Is.Null, "NSObject-1A-ref");
				Assert.That (outObj, Is.Null, "NSObject-1A-out");

				// managed
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				obj.TestNSObject (action << 8, ref refObj, out outObj);
				Assert.That (refObj, Is.Null, "NSObject-1M-ref");
				Assert.That (outObj, Is.Null, "NSObject-1M-out");

				// direct native
				refValue = dummyObj.Handle; // set to non-null
				outValue = dummyObj.Handle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 0, ref refValue, out outValue);
				Assert.That (refValue, Is.EqualTo (NativeHandle.Zero), "NSObject-1DA-ref");
				Assert.That (outValue, Is.EqualTo (NativeHandle.Zero), "NSObject-1DA-out");

				// direct managed
				refValue = dummyObj.Handle; // set to non-null
				outValue = dummyObj.Handle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 8, ref refValue, out outValue);
				Assert.That (refValue, Is.EqualTo (NativeHandle.Zero), "NSObject-1DM-ref");
				Assert.That (outValue, Is.EqualTo (NativeHandle.Zero), "NSObject-1DM-out");

				/// 2: verify that refValue points to something
				action = 2;

				// native
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				obj.TestNSObject (action << 0, ref refObj, out outObj);
				Assert.That (refObj.Handle, Is.EqualTo (dummyObj.Handle), "NSObject-2A-ref");
				Assert.That (refObj, Is.SameAs (dummyObj), "NSObject-2A-ref-same");
				Assert.That (outObj, Is.Null, "NSObject-2A-out");

				// managed
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				obj.TestNSObject (action << 8, ref refObj, out outObj);
				Assert.That (refObj.Handle, Is.EqualTo (dummyObj.Handle), "NSObject-2M-ref");
				Assert.That (refObj, Is.SameAs (dummyObj), "NSObject-2M-ref-same");
				Assert.That (outObj, Is.Null, "NSObject-2M-out");

				// direct native
				refValue = dummyObj.Handle; // set to non-null
				outValue = dummyObj.Handle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 0, ref refValue, out outValue);
				Assert.That (refValue, Is.EqualTo (dummyObj.Handle), "NSObject-2DA-ref");
				Assert.That (outValue, Is.EqualTo (NativeHandle.Zero), "NSObject-2DA-out");

				// direct managed
				refValue = dummyObj.Handle; // set to non-null
				outValue = dummyObj.Handle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 8, ref refValue, out outValue);
				Assert.That (refValue, Is.EqualTo (dummyObj.Handle), "NSObject-2DM-ref");
				Assert.That (outValue, Is.EqualTo (NativeHandle.Zero), "NSObject-2DM-out");


				/// 3 set both parameteres to the same pointer of a NSObject
				action = 3;

				// native
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				obj.TestNSObject (action << 0, ref refObj, out outObj);
				Assert.That (refObj.Handle, Is.Not.EqualTo (dummyObj.Handle), "NSObject-3A-ref");
				Assert.That (outObj.Handle, Is.Not.EqualTo (dummyObj.Handle), "NSObject-3A-out");
				Assert.That (outObj, Is.SameAs (refObj), "NSObject-3A-ref-out-not-safe");
				Assert.That (refObj, Is.TypeOf<NSObject> (), "NSObject-3A-ref-wrapper-type");
				Assert.That (outObj, Is.TypeOf<NSObject> (), "NSObject-3A-ref-wrapper-type");

				// managed
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				obj.TestNSObject (action << 8, ref refObj, out outObj);
				Assert.That (refObj.Handle, Is.Not.EqualTo (dummyObj.Handle), "NSObject-3M-ref");
				Assert.That (outObj.Handle, Is.Not.EqualTo (dummyObj.Handle), "NSObject-3M-out");
				Assert.That (outObj, Is.SameAs (refObj), "NSObject-3M-ref-out-not-safe");
				Assert.That (refObj, Is.TypeOf<NSObject> (), "NSObject-3M-ref-wrapper-type");
				Assert.That (outObj, Is.TypeOf<NSObject> (), "NSObject-3M-ref-wrapper-type");

				// direct native
				refValue = dummyObj.Handle; // set to non-null
				outValue = dummyObj.Handle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 0, ref refValue, out outValue);
				Assert.That (refValue, Is.Not.EqualTo (dummyObj.Handle), "NSObject-3DA-ref");
				Assert.That (outValue, Is.Not.EqualTo (dummyObj.Handle), "NSObject-3DA-out");
				Assert.That (outValue, Is.EqualTo (refValue), "NSObject-3DA-out-ref-same");
				Assert.That (Runtime.GetNSObject (refValue), Is.TypeOf<NSObject> (), "NSObject-3DA-ref-wrapper-type");
				Assert.That (Runtime.GetNSObject (outValue), Is.TypeOf<NSObject> (), "NSObject-3DA-ref-wrapper-type");

				// direct managed
				refValue = dummyObj.Handle; // set to non-null
				outValue = dummyObj.Handle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 8, ref refValue, out outValue);
				Assert.That (refValue, Is.Not.EqualTo (dummyObj.Handle), "NSObject-3DM-ref");
				Assert.That (outValue, Is.Not.EqualTo (dummyObj.Handle), "NSObject-3DM-out");
				Assert.That (outValue, Is.EqualTo (refValue), "NSObject-3DM-out-ref-eq");
				Assert.That (Runtime.GetNSObject (refValue), Is.TypeOf<NSObject> (), "NSObject-3DM-ref-wrapper-type");
				Assert.That (Runtime.GetNSObject (outValue), Is.TypeOf<NSObject> (), "NSObject-3DM-ref-wrapper-type");


				/// 4 set both parameteres to different pointers of a NSObject
				action = 4;

				// native
				refObj = null; // set to null
				outObj = null; // set to null
				obj.TestNSObject (action << 0, ref refObj, out outObj);
				Assert.That (refObj.Handle, Is.Not.EqualTo (NativeHandle.Zero), "NSObject-4A-ref");
				Assert.That (outObj.Handle, Is.Not.EqualTo (NativeHandle.Zero), "NSObject-4A-out");
				Assert.That (outObj.Handle, Is.Not.EqualTo (refObj.Handle), "NSObject-4A-ref-distinct");
				Assert.That (refObj, Is.TypeOf<NSObject> (), "NSObject-4A-ref-wrapper-type");
				Assert.That (outObj, Is.TypeOf<NSObject> (), "NSObject-4A-ref-wrapper-type");

				// managed
				refObj = null; // set to null
				outObj = null; // set to null
				obj.TestNSObject (action << 8, ref refObj, out outObj);
				Assert.That (refObj.Handle, Is.Not.EqualTo (NativeHandle.Zero), "NSObject-4M-ref");
				Assert.That (outObj.Handle, Is.Not.EqualTo (NativeHandle.Zero), "NSObject-4M-out");
				Assert.That (outObj.Handle, Is.Not.EqualTo (refObj.Handle), "NSObject-4M-ref-distinct");
				Assert.That (refObj, Is.TypeOf<NSObject> (), "NSObject-4M-ref-wrapper-type");
				Assert.That (outObj, Is.TypeOf<NSObject> (), "NSObject-4M-ref-wrapper-type");

				// direct native
				refValue = NativeHandle.Zero; // set to null
				outValue = NativeHandle.Zero; // set to null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 0, ref refValue, out outValue);
				Assert.That (refValue, Is.Not.EqualTo (NativeHandle.Zero), "NSObject-4DA-ref");
				Assert.That (outValue, Is.Not.EqualTo (NativeHandle.Zero), "NSObject-4DA-out");
				Assert.That (outValue, Is.Not.EqualTo (refValue), "NSObject-4DA-ref-distinct");
				Assert.That (Runtime.GetNSObject (refValue), Is.TypeOf<NSObject> (), "NSObject-4DA-ref-wrapper-type");
				Assert.That (Runtime.GetNSObject (outValue), Is.TypeOf<NSObject> (), "NSObject-4DA-ref-wrapper-type");

				// direct managed
				refValue = NativeHandle.Zero; // set to null
				outValue = NativeHandle.Zero; // set to null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 8, ref refValue, out outValue);
				Assert.That (refValue, Is.Not.EqualTo (NativeHandle.Zero), "NSObject-4DM-ref");
				Assert.That (outValue, Is.Not.EqualTo (NativeHandle.Zero), "NSObject-4DM-out");
				Assert.That (outValue, Is.Not.EqualTo (refValue), "NSObject-4DM-ref-distinct");
				Assert.That (Runtime.GetNSObject (refValue), Is.TypeOf<NSObject> (), "NSObject-4DM-ref-wrapper-type");
				Assert.That (Runtime.GetNSObject (outValue), Is.TypeOf<NSObject> (), "NSObject-4DM-ref-wrapper-type");
			}
		}

		[Test]
		public void RefOutTest_NSValue ()
		{
			var refValue = NativeHandle.Zero;
			var outValue = NativeHandle.Zero;

			using (var obj = new RefOutParametersSubclass ()) {
				var sel = Selector.GetHandle ("testNSValue:a:b:");
				var dummyObj = NSValue.FromMKCoordinate (new CLLocationCoordinate2D (3, 14));
				NSValue refObj = null;
				NSValue outObj = null;
				int action;

				/// 1: set both to null
				action = 1;

				// native
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				obj.TestValue (action << 0, ref refObj, out outObj);
				Assert.That (refObj, Is.Null, "NSValue-1A-ref");
				Assert.That (outObj, Is.Null, "NSValue-1A-out");

				// managed
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				obj.TestValue (action << 8, ref refObj, out outObj);
				Assert.That (refObj, Is.Null, "NSValue-1M-ref");
				Assert.That (outObj, Is.Null, "NSValue-1M-out");

				// direct native
				refValue = dummyObj.Handle; // set to non-null
				outValue = dummyObj.Handle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 0, ref refValue, out outValue);
				Assert.That (refValue, Is.EqualTo (NativeHandle.Zero), "NSValue-1DA-ref");
				Assert.That (outValue, Is.EqualTo (NativeHandle.Zero), "NSValue-1DA-out");

				// direct managed
				refValue = dummyObj.Handle; // set to non-null
				outValue = dummyObj.Handle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 8, ref refValue, out outValue);
				Assert.That (refValue, Is.EqualTo (NativeHandle.Zero), "NSValue-1DM-ref");
				Assert.That (outValue, Is.EqualTo (NativeHandle.Zero), "NSValue-1DM-out");

				/// 2: verify that refValue points to something
				action = 2;

				// native
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				obj.TestValue (action << 0, ref refObj, out outObj);
				Assert.That (refObj.Handle, Is.EqualTo (dummyObj.Handle), "NSValue-2A-ref");
				Assert.That (refObj, Is.SameAs (dummyObj), "NSValue-2A-ref-same");
				Assert.That (outObj, Is.Null, "NSValue-2A-out");

				// managed
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				obj.TestValue (action << 8, ref refObj, out outObj);
				Assert.That (refObj.Handle, Is.EqualTo (dummyObj.Handle), "NSValue-2M-ref");
				Assert.That (refObj, Is.SameAs (dummyObj), "NSValue-2M-ref-same");
				Assert.That (outObj, Is.Null, "NSValue-2M-out");

				// direct native
				refValue = dummyObj.Handle; // set to non-null
				outValue = dummyObj.Handle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 0, ref refValue, out outValue);
				Assert.That (refValue, Is.EqualTo (dummyObj.Handle), "NSValue-2DA-ref");
				Assert.That (outValue, Is.EqualTo (NativeHandle.Zero), "NSValue-2DA-out");

				// direct managed
				refValue = dummyObj.Handle; // set to non-null
				outValue = dummyObj.Handle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 8, ref refValue, out outValue);
				Assert.That (refValue, Is.EqualTo (dummyObj.Handle), "NSValue-2DM-ref");
				Assert.That (outValue, Is.EqualTo (NativeHandle.Zero), "NSValue-2DM-out");


				/// 3 set both parameteres to the same pointer of a NSValue
				action = 3;

				// native
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				obj.TestValue (action << 0, ref refObj, out outObj);
				Assert.That (refObj.Handle, Is.Not.EqualTo (dummyObj.Handle), "NSValue-3A-ref");
				Assert.That (outObj.Handle, Is.Not.EqualTo (dummyObj.Handle), "NSValue-3A-out");
				Assert.That (outObj, Is.SameAs (refObj), "NSValue-3A-ref-out-not-safe");
				Assert.That (refObj, Is.TypeOf<NSValue> (), "NSValue-3A-ref-wrapper-type");
				Assert.That (outObj, Is.TypeOf<NSValue> (), "NSValue-3A-ref-wrapper-type");

				// managed
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				obj.TestValue (action << 8, ref refObj, out outObj);
				Assert.That (refObj.Handle, Is.Not.EqualTo (dummyObj.Handle), "NSValue-3M-ref");
				Assert.That (outObj.Handle, Is.Not.EqualTo (dummyObj.Handle), "NSValue-3M-out");
				Assert.That (outObj, Is.SameAs (refObj), "NSValue-3M-ref-out-not-safe");
				Assert.That (refObj, Is.TypeOf<NSValue> (), "NSValue-3M-ref-wrapper-type");
				Assert.That (outObj, Is.TypeOf<NSValue> (), "NSValue-3M-ref-wrapper-type");

				// direct native
				refValue = dummyObj.Handle; // set to non-null
				outValue = dummyObj.Handle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 0, ref refValue, out outValue);
				Assert.That (refValue, Is.Not.EqualTo (dummyObj.Handle), "NSValue-3DA-ref");
				Assert.That (outValue, Is.Not.EqualTo (dummyObj.Handle), "NSValue-3DA-out");
				Assert.That (outValue, Is.EqualTo (refValue), "NSValue-3DA-out-ref-same");
				Assert.That (Runtime.GetNSObject (refValue), Is.TypeOf<NSValue> (), "NSValue-3DA-ref-wrapper-type");
				Assert.That (Runtime.GetNSObject (outValue), Is.TypeOf<NSValue> (), "NSValue-3DA-ref-wrapper-type");

				// direct managed
				refValue = dummyObj.Handle; // set to non-null
				outValue = dummyObj.Handle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 8, ref refValue, out outValue);
				Assert.That (refValue, Is.Not.EqualTo (dummyObj.Handle), "NSValue-3DM-ref");
				Assert.That (outValue, Is.Not.EqualTo (dummyObj.Handle), "NSValue-3DM-out");
				Assert.That (outValue, Is.EqualTo (refValue), "NSValue-3DM-out-ref-eq");
				Assert.That (Runtime.GetNSObject (refValue), Is.TypeOf<NSValue> (), "NSValue-3DM-ref-wrapper-type");
				Assert.That (Runtime.GetNSObject (outValue), Is.TypeOf<NSValue> (), "NSValue-3DM-ref-wrapper-type");


				/// 4 set both parameteres to different pointers of a NSValue
				action = 4;

				// native
				refObj = null; // set to null
				outObj = null; // set to null
				obj.TestValue (action << 0, ref refObj, out outObj);
				Assert.That (refObj.Handle, Is.Not.EqualTo (NativeHandle.Zero), "NSValue-4A-ref");
				Assert.That (outObj.Handle, Is.Not.EqualTo (NativeHandle.Zero), "NSValue-4A-out");
				Assert.That (outObj.Handle, Is.Not.EqualTo (refObj.Handle), "NSValue-4A-ref-distinct");
				Assert.That (refObj, Is.TypeOf<NSValue> (), "NSValue-4A-ref-wrapper-type");
				Assert.That (outObj, Is.TypeOf<NSValue> (), "NSValue-4A-ref-wrapper-type");

				// managed
				refObj = null; // set to null
				outObj = null; // set to null
				obj.TestValue (action << 8, ref refObj, out outObj);
				Assert.That (refObj.Handle, Is.Not.EqualTo (NativeHandle.Zero), "NSValue-4M-ref");
				Assert.That (outObj.Handle, Is.Not.EqualTo (NativeHandle.Zero), "NSValue-4M-out");
				Assert.That (outObj.Handle, Is.Not.EqualTo (refObj.Handle), "NSValue-4M-ref-distinct");
				Assert.That (refObj, Is.TypeOf<NSValue> (), "NSValue-4M-ref-wrapper-type");
				Assert.That (outObj, Is.TypeOf<NSValue> (), "NSValue-4M-ref-wrapper-type");

				// direct native
				refValue = NativeHandle.Zero; // set to null
				outValue = NativeHandle.Zero; // set to null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 0, ref refValue, out outValue);
				Assert.That (refValue, Is.Not.EqualTo (NativeHandle.Zero), "NSValue-4DA-ref");
				Assert.That (outValue, Is.Not.EqualTo (NativeHandle.Zero), "NSValue-4DA-out");
				Assert.That (outValue, Is.Not.EqualTo (refValue), "NSValue-4DA-ref-distinct");
				Assert.That (Runtime.GetNSObject (refValue), Is.TypeOf<NSValue> (), "NSValue-4DA-ref-wrapper-type");
				Assert.That (Runtime.GetNSObject (outValue), Is.TypeOf<NSValue> (), "NSValue-4DA-ref-wrapper-type");

				// direct managed
				refValue = NativeHandle.Zero; // set to null
				outValue = NativeHandle.Zero; // set to null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 8, ref refValue, out outValue);
				Assert.That (refValue, Is.Not.EqualTo (NativeHandle.Zero), "NSValue-4DM-ref");
				Assert.That (outValue, Is.Not.EqualTo (NativeHandle.Zero), "NSValue-4DM-out");
				Assert.That (outValue, Is.Not.EqualTo (refValue), "NSValue-4DM-ref-distinct");
				Assert.That (Runtime.GetNSObject (refValue), Is.TypeOf<NSValue> (), "NSValue-4DM-ref-wrapper-type");
				Assert.That (Runtime.GetNSObject (outValue), Is.TypeOf<NSValue> (), "NSValue-4DM-ref-wrapper-type");
			}
		}

		[Test]
		public void RefOutTest_String ()
		{
			var refValue = NativeHandle.Zero;
			var outValue = NativeHandle.Zero;

			using (var obj = new RefOutParametersSubclass ()) {
				var sel = Selector.GetHandle ("testString:a:b:");
				var dummyObj = "dummy string";
				var dummyObjHandle = NSString.CreateNative (dummyObj, true);
				string refObj = null;
				string outObj = null;
				int action;

				/// 1: set both to null
				action = 1;

				// native
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				obj.TestString (action << 0, ref refObj, out outObj);
				Assert.That (refObj, Is.Null, "String-1A-ref");
				Assert.That (outObj, Is.Null, "String-1A-out");

				// managed
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				obj.TestString (action << 8, ref refObj, out outObj);
				Assert.That (refObj, Is.Null, "String-1M-ref");
				Assert.That (outObj, Is.Null, "String-1M-out");

				// direct native
				refValue = dummyObjHandle; // set to non-null
				outValue = dummyObjHandle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 0, ref refValue, out outValue);
				Assert.That (refValue, Is.EqualTo (NativeHandle.Zero), "String-1DA-ref");
				Assert.That (outValue, Is.EqualTo (NativeHandle.Zero), "String-1DA-out");

				// direct managed
				refValue = dummyObjHandle; // set to non-null
				outValue = dummyObjHandle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 8, ref refValue, out outValue);
				Assert.That (refValue, Is.EqualTo (NativeHandle.Zero), "String-1DM-ref");
				Assert.That (outValue, Is.EqualTo (NativeHandle.Zero), "String-1DM-out");

				/// 2: verify that refValue points to something
				action = 2;

				// native
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				obj.TestString (action << 0, ref refObj, out outObj);
				Assert.That (refObj, Is.EqualTo (dummyObj), "String-2A-ref");
				Assert.That (outObj, Is.Null, "String-2A-out");

				// managed
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				obj.TestString (action << 8, ref refObj, out outObj);
				Assert.That (refObj, Is.EqualTo (dummyObj), "String-2M-ref");
				Assert.That (outObj, Is.Null, "String-2M-out");

				// direct native
				refValue = dummyObjHandle; // set to non-null
				outValue = dummyObjHandle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 0, ref refValue, out outValue);
				Assert.That (NSString.FromHandle (refValue), Is.EqualTo (dummyObj), "String-2DA-ref");
				Assert.That (outValue, Is.EqualTo (NativeHandle.Zero), "String-2DA-out");

				// direct managed
				refValue = dummyObjHandle; // set to non-null
				outValue = dummyObjHandle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 8, ref refValue, out outValue);
				Assert.That (NSString.FromHandle (refValue), Is.EqualTo (dummyObj), "String-2DM-ref");
				Assert.That (outValue, Is.EqualTo (NativeHandle.Zero), "String-2DM-out");


				/// 3 set both parameteres to the same pointer of a String
				action = 3;

				// native
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				obj.TestString (action << 0, ref refObj, out outObj);
				Assert.That (refObj, Is.EqualTo ("A constant native string"), "String-3A-ref");
				Assert.That (outObj, Is.EqualTo ("A constant native string"), "String-3A-out");

				// managed
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				obj.TestString (action << 8, ref refObj, out outObj);
				Assert.That (refObj, Is.EqualTo ("A constant managed string"), "String-3M-ref");
				Assert.That (outObj, Is.EqualTo ("A constant managed string"), "String-3M-out");

				// direct native
				refValue = dummyObjHandle; // set to non-null
				outValue = dummyObjHandle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 0, ref refValue, out outValue);
				Assert.That (outValue, Is.Not.EqualTo (refValue), "String-3DA-eq"); // The managed roundtrip means 'outValue' is re-created (because it's nulled out upon entering managed code)
				Assert.That (NSString.FromHandle (refValue), Is.EqualTo ("A constant native string"), "String-3DA-ref");
				Assert.That (NSString.FromHandle (outValue), Is.EqualTo ("A constant native string"), "String-3DA-out");

				// direct managed
				refValue = dummyObjHandle; // set to non-null
				outValue = dummyObjHandle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 8, ref refValue, out outValue);
				Assert.That (outValue, Is.Not.EqualTo (refValue), "String-3DM-eq"); // The managed roundtrip means 'outValue' is re-created (because it's nulled out upon entering managed code)
				Assert.That (NSString.FromHandle (refValue), Is.EqualTo ("A constant managed string"), "String-3DM-ref");
				Assert.That (NSString.FromHandle (outValue), Is.EqualTo ("A constant managed string"), "String-3DM-out");


				/// 4 set both parameteres to different pointers of a String
				action = 4;

				// native
				refObj = null; // set to null
				outObj = null; // set to null
				obj.TestString (action << 0, ref refObj, out outObj);
				Assert.That (refObj, Is.EqualTo ("Hello Xamarin"), "String-4A-ref-value");
				Assert.That (outObj, Is.EqualTo ("Hello Microsoft"), "String-4A-out-value");

				// managed
				refObj = null; // set to null
				outObj = null; // set to null
				obj.TestString (action << 8, ref refObj, out outObj);
				Assert.That (refObj, Is.EqualTo ("Hello Xamarin from managed"), "String-4M-ref-value");
				Assert.That (outObj, Is.EqualTo ("Hello Microsoft from managed"), "String-4M-out-value");

				// direct native
				refValue = IntPtr.Zero; // set to null
				outValue = IntPtr.Zero; // set to null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 0, ref refValue, out outValue);
				Assert.That (NSString.FromHandle (refValue), Is.EqualTo ("Hello Xamarin"), "String-4DA-ref-value");
				Assert.That (NSString.FromHandle (outValue), Is.EqualTo ("Hello Microsoft"), "String-4DA-out-value");

				// direct managed
				refValue = IntPtr.Zero; // set to null
				outValue = IntPtr.Zero; // set to null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 8, ref refValue, out outValue);
				Assert.That (NSString.FromHandle (refValue), Is.EqualTo ("Hello Xamarin from managed"), "String-4DM-ref-value");
				Assert.That (NSString.FromHandle (outValue), Is.EqualTo ("Hello Microsoft from managed"), "String-4DM-out-value");
			}
		}

		[Test]
		public unsafe void RefOutTest_Int ()
		{
			using (var obj = new RefOutParametersSubclass ()) {
				var sel = Selector.GetHandle ("testInt:a:b:c:");
				var dummyObj = 314;
				int refObj = 0;
				int outObj = 0;
				int ptrObj = 0;
				int action;

				/// 1: set both to 0
				action = 1;

				// native
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				ptrObj = dummyObj; // set to non-null
				obj.TestInt (action << 0, ref refObj, out outObj, &ptrObj);
				Assert.That (refObj, Is.EqualTo (0), "Int-1A-ref");
				Assert.That (outObj, Is.EqualTo (0), "Int-1A-out");
				Assert.That (ptrObj, Is.EqualTo (0), "Int-1A-ptr");

				// managed
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				ptrObj = dummyObj; // set to non-null
				obj.TestInt (action << 8, ref refObj, out outObj, &ptrObj);
				Assert.That (refObj, Is.EqualTo (0), "Int-1M-ref");
				Assert.That (outObj, Is.EqualTo (0), "Int-1M-out");
				Assert.That (ptrObj, Is.EqualTo (0), "Int-1M-ptr");

				// direct native
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				ptrObj = dummyObj; // set to non-null
				Messaging.void_objc_msgSend_int_int_int_int (obj.Handle, sel, action << 0, ref refObj, out outObj, &ptrObj);
				Assert.That (refObj, Is.EqualTo (0), "Int-1DA-ref");
				Assert.That (outObj, Is.EqualTo (0), "Int-1DA-out");
				Assert.That (ptrObj, Is.EqualTo (0), "Int-1DA-ptr");

				// direct managed
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				ptrObj = dummyObj; // set to non-null
				Messaging.void_objc_msgSend_int_int_int_int (obj.Handle, sel, action << 8, ref refObj, out outObj, &ptrObj);
				Assert.That (refObj, Is.EqualTo (0), "Int-1DM-ref");
				Assert.That (outObj, Is.EqualTo (0), "Int-1DM-out");
				Assert.That (ptrObj, Is.EqualTo (0), "Int-1DM-ptr");

				/// 2: N/A for testInt

				/// 3 set both parameteres to the same pointer of a Int
				action = 3;

				// native
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				ptrObj = dummyObj; // set to non-null
				obj.TestInt (action << 0, ref refObj, out outObj, &ptrObj);
				Assert.That (refObj, Is.Not.EqualTo (dummyObj), "Int-3A-ref");
				Assert.That (outObj, Is.Not.EqualTo (dummyObj), "Int-3A-out");
				Assert.That (ptrObj, Is.Not.EqualTo (dummyObj), "Int-3A-ptr");
				Assert.That (outObj, Is.EqualTo (refObj), "Int-3A-out-ref-eq");
				Assert.That (ptrObj, Is.EqualTo (refObj), "Int-3A-out-ptr-eq");

				// managed
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				ptrObj = dummyObj; // set to non-null
				obj.TestInt (action << 8, ref refObj, out outObj, &ptrObj);
				Assert.That (refObj, Is.Not.EqualTo (dummyObj), "Int-3M-ref");
				Assert.That (outObj, Is.Not.EqualTo (dummyObj), "Int-3M-out");
				Assert.That (ptrObj, Is.Not.EqualTo (dummyObj), "Int-3M-ptr");
				Assert.That (outObj, Is.EqualTo (refObj), "Int-3M-out-ref-eq");
				Assert.That (ptrObj, Is.EqualTo (refObj), "Int-3M-out-ptr-eq");

				// direct native
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				ptrObj = dummyObj; // set to non-null
				Messaging.void_objc_msgSend_int_int_int_int (obj.Handle, sel, action << 0, ref refObj, out outObj, &ptrObj);
				Assert.That (refObj, Is.Not.EqualTo (dummyObj), "Int-3DA-ref");
				Assert.That (outObj, Is.Not.EqualTo (dummyObj), "Int-3DA-out");
				Assert.That (ptrObj, Is.Not.EqualTo (dummyObj), "Int-3DA-ptr");
				Assert.That (outObj, Is.EqualTo (refObj), "Int-3DA-out-ref-same");
				Assert.That (ptrObj, Is.EqualTo (refObj), "Int-3DA-out-ptr-same");

				// direct managed
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				ptrObj = dummyObj; // set to non-null
				Messaging.void_objc_msgSend_int_int_int_int (obj.Handle, sel, action << 8, ref refObj, out outObj, &ptrObj);
				Assert.That (refObj, Is.Not.EqualTo (dummyObj), "Int-3DM-ref");
				Assert.That (outObj, Is.Not.EqualTo (dummyObj), "Int-3DM-out");
				Assert.That (ptrObj, Is.Not.EqualTo (dummyObj), "Int-3DM-ptr");
				Assert.That (outObj, Is.EqualTo (refObj), "Int-3DM-out-ref-eq");
				Assert.That (ptrObj, Is.EqualTo (refObj), "Int-3DM-out-ptr-eq");


				/// 4 set both parameteres to different pointers of a Int
				action = 4;

				// native
				refObj = 0; // set to 0
				outObj = 0; // set to 0
				ptrObj = 0; // set to 0
				obj.TestInt (action << 0, ref refObj, out outObj, &ptrObj);
				Assert.That (refObj, Is.Not.EqualTo (0), "Int-4A-ref");
				Assert.That (outObj, Is.Not.EqualTo (0), "Int-4A-out");
				Assert.That (ptrObj, Is.Not.EqualTo (0), "Int-4A-ptr");
				Assert.That (outObj, Is.Not.EqualTo (refObj), "Int-4A-ref-distinct");
				Assert.That (ptrObj, Is.Not.EqualTo (refObj), "Int-4A-ptr-distinct");

				// managed
				refObj = 0; // set to 0
				outObj = 0; // set to 0
				ptrObj = 0; // set to 0
				obj.TestInt (action << 8, ref refObj, out outObj, &ptrObj);
				Assert.That (refObj, Is.Not.EqualTo (0), "Int-4M-ref");
				Assert.That (outObj, Is.Not.EqualTo (0), "Int-4M-out");
				Assert.That (ptrObj, Is.Not.EqualTo (0), "Int-4M-ptr");
				Assert.That (outObj, Is.Not.EqualTo (refObj), "Int-4M-ref-distinct");
				Assert.That (ptrObj, Is.Not.EqualTo (refObj), "Int-4M-ptr-distinct");

				// direct native
				refObj = 0; // set to 0
				outObj = 0; // set to 0
				ptrObj = 0; // set to 0
				Messaging.void_objc_msgSend_int_int_int_int (obj.Handle, sel, action << 0, ref refObj, out outObj, &ptrObj);
				Assert.That (refObj, Is.Not.EqualTo (0), "Int-4DA-ref");
				Assert.That (outObj, Is.Not.EqualTo (0), "Int-4DA-out");
				Assert.That (ptrObj, Is.Not.EqualTo (0), "Int-4DA-ptr");
				Assert.That (outObj, Is.Not.EqualTo (refObj), "Int-4DA-ref-distinct");
				Assert.That (ptrObj, Is.Not.EqualTo (refObj), "Int-4DA-ptr-distinct");
				Assert.That (refObj, Is.EqualTo (3141592), "Int-4DA-ref-value");
				Assert.That (outObj, Is.EqualTo (2718282), "Int-4DA-out-value");
				Assert.That (ptrObj, Is.EqualTo (5772156), "Int-4DA-ptr-value");

				// direct managed
				refObj = 0; // set to 0
				outObj = 0; // set to 0
				ptrObj = 0; // set to 0
				Messaging.void_objc_msgSend_int_int_int_int (obj.Handle, sel, action << 8, ref refObj, out outObj, &ptrObj);
				Assert.That (refObj, Is.Not.EqualTo (0), "Int-4DM-ref");
				Assert.That (outObj, Is.Not.EqualTo (0), "Int-4DM-out");
				Assert.That (ptrObj, Is.Not.EqualTo (0), "Int-4DM-ptr");
				Assert.That (outObj, Is.Not.EqualTo (refObj), "Int-4DM-ref-distinct");
				Assert.That (ptrObj, Is.Not.EqualTo (refObj), "Int-4DM-ptr-distinct");
				Assert.That (refObj, Is.EqualTo (3141592), "Int-4DM-ref-value");
				Assert.That (outObj, Is.EqualTo (2718282), "Int-4DM-out-value");
				Assert.That (ptrObj, Is.EqualTo (5772156), "Int-4DM-ptr-value");
			}
		}

		[Test]
		public void RefOutTest_Sel ()
		{
			var refValue = IntPtr.Zero;
			var outValue = IntPtr.Zero;

			using (var obj = new RefOutParametersSubclass ()) {
				var sel = Selector.GetHandle ("testSelector:a:b:");
				var dummyObj = new Selector ("dummy string");
				var dummyObjHandle = dummyObj.Handle;
				Selector refObj = null;
				Selector outObj = null;
				TestMethod<Selector> test = obj.TestSelector;
				int action;

				/// 1: set both to null
				action = 1;

				// native
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				test (action << 0, ref refObj, out outObj);
				Assert.That (refObj, Is.Null, "Selector-1A-ref");
				Assert.That (outObj, Is.Null, "Selector-1A-out");

				// managed
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				test (action << 8, ref refObj, out outObj);
				Assert.That (refObj, Is.Null, "Selector-1M-ref");
				Assert.That (outObj, Is.Null, "Selector-1M-out");

				// direct native
				refValue = dummyObjHandle; // set to non-null
				outValue = dummyObjHandle; // set to non-null
				IntPtr x = Marshal.AllocHGlobal (16);
				//Marshal.WriteIntPtr (x, (IntPtr) 0xdeadf00d);
				//Marshal.WriteIntPtr (x, 8, (IntPtr) 0xbabebabe);
				//Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 0, x, x);
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 0, ref refValue, out outValue);
				Assert.That (refValue, Is.EqualTo (IntPtr.Zero), "Selector-1DA-ref");
				Assert.That (outValue, Is.EqualTo (IntPtr.Zero), "Selector-1DA-out");

				// direct managed
				refValue = dummyObjHandle; // set to non-null
				outValue = dummyObjHandle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 8, ref refValue, out outValue);
				Assert.That (refValue, Is.EqualTo (IntPtr.Zero), "Selector-1DM-ref");
				Assert.That (outValue, Is.EqualTo (IntPtr.Zero), "Selector-1DM-out");

				/// 2: verify that refValue points to something
				action = 2;

				// native
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				test (action << 0, ref refObj, out outObj);
				Assert.That (refObj, Is.EqualTo (dummyObj), "Selector-2A-ref");
				Assert.That (outObj, Is.Null, "Selector-2A-out");

				// managed
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				test (action << 8, ref refObj, out outObj);
				Assert.That (refObj, Is.EqualTo (dummyObj), "Selector-2M-ref");
				Assert.That (outObj, Is.Null, "Selector-2M-out");

				// direct native
				refValue = dummyObjHandle; // set to non-null
				outValue = dummyObjHandle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 0, ref refValue, out outValue);
				Assert.That (refValue, Is.EqualTo ((IntPtr) dummyObj.Handle), "Selector-2DA-ref");
				Assert.That (outValue, Is.EqualTo (IntPtr.Zero), "Selector-2DA-out");

				// direct managed
				refValue = dummyObjHandle; // set to non-null
				outValue = dummyObjHandle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 8, ref refValue, out outValue);
				Assert.That (refValue, Is.EqualTo ((IntPtr) dummyObj.Handle), "Selector-2DM-ref");
				Assert.That (outValue, Is.EqualTo (IntPtr.Zero), "Selector-2DM-out");


				/// 3 set both parameteres to the same selector
				action = 3;

				// native
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				test (action << 0, ref refObj, out outObj);
				Assert.That ((IntPtr) refObj.Handle, Is.EqualTo (Selector.GetHandle ("testSelector")), "Selector-3A-ref");
				Assert.That ((IntPtr) outObj.Handle, Is.EqualTo (Selector.GetHandle ("testSelector")), "Selector-3A-out");

				// managed
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				test (action << 8, ref refObj, out outObj);
				Assert.That ((IntPtr) refObj.Handle, Is.EqualTo (Selector.GetHandle ("testManagedSelector")), "Selector-3M-ref");
				Assert.That ((IntPtr) outObj.Handle, Is.EqualTo (Selector.GetHandle ("testManagedSelector")), "Selector-3M-out");

				// direct native
				refValue = dummyObjHandle; // set to non-null
				outValue = dummyObjHandle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 0, ref refValue, out outValue);
				Assert.That (outValue, Is.EqualTo (refValue), "Selector-3DA-eq"); // The managed roundtrip means 'outValue' is re-created (because it's nulled out upon entering managed code), but because selectors are unique, we get back the same pointer.
				Assert.That (refValue, Is.EqualTo (Selector.GetHandle ("testSelector")), "Selector-3DA-ref");
				Assert.That (outValue, Is.EqualTo (Selector.GetHandle ("testSelector")), "Selector-3DA-out");

				// direct managed
				refValue = dummyObjHandle; // set to non-null
				outValue = dummyObjHandle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 8, ref refValue, out outValue);
				Assert.That (outValue, Is.EqualTo (refValue), "Selector-3DM-eq"); // The managed roundtrip means 'outValue' is re-created (because it's nulled out upon entering managed code), but because selectors are unique, we get back the same pointer.
				Assert.That (refValue, Is.EqualTo (Selector.GetHandle ("testManagedSelector")), "Selector-3DM-ref");
				Assert.That (outValue, Is.EqualTo (Selector.GetHandle ("testManagedSelector")), "Selector-3DM-out");


				/// 4 set both parameteres to different selectors
				action = 4;

				// native
				refObj = null; // set to null
				outObj = null; // set to null
				test (action << 0, ref refObj, out outObj);
				Assert.That ((IntPtr) refObj.Handle, Is.EqualTo (Selector.GetHandle ("testSelector:a:")), "Selector-4A-ref-value");
				Assert.That ((IntPtr) outObj.Handle, Is.EqualTo (Selector.GetHandle ("testSelector:b:")), "Selector-4A-out-value");

				// managed
				refObj = null; // set to null
				outObj = null; // set to null
				test (action << 8, ref refObj, out outObj);
				Assert.That ((IntPtr) refObj.Handle, Is.EqualTo (Selector.GetHandle ("testManagedSelectorA")), "Selector-4M-ref-value");
				Assert.That ((IntPtr) outObj.Handle, Is.EqualTo (Selector.GetHandle ("testManagedSelectorB")), "Selector-4M-out-value");

				// direct native
				refValue = IntPtr.Zero; // set to null
				outValue = IntPtr.Zero; // set to null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 0, ref refValue, out outValue);
				Assert.That (refValue, Is.EqualTo (Selector.GetHandle ("testSelector:a:")), "Selector-4DA-ref-value");
				Assert.That (outValue, Is.EqualTo (Selector.GetHandle ("testSelector:b:")), "Selector-4DA-out-value");

				// direct managed
				refValue = IntPtr.Zero; // set to null
				outValue = IntPtr.Zero; // set to null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 8, ref refValue, out outValue);
				Assert.That (refValue, Is.EqualTo (Selector.GetHandle ("testManagedSelectorA")), "Selector-4DM-ref-value");
				Assert.That (outValue, Is.EqualTo (Selector.GetHandle ("testManagedSelectorB")), "Selector-4DM-out-value");
			}
		}

		[Test]
		public void RefOutTest_Class ()
		{
			var refValue = NativeHandle.Zero;
			var outValue = NativeHandle.Zero;

			using (var obj = new RefOutParametersSubclass ()) {
				var sel = Selector.GetHandle ("testClass:a:b:");
				var dummyObj = new Class (typeof (NSObject));
				var dummyObjHandle = dummyObj.Handle;
				Class refObj = null;
				Class outObj = null;
				TestMethod<Class> test = obj.TestClass;
				int action;

				/// 1: set both to null
				action = 1;

				// native
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				test (action << 0, ref refObj, out outObj);
				Assert.That (refObj, Is.Null, "Class-1A-ref");
				Assert.That (outObj, Is.Null, "Class-1A-out");

				// managed
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				test (action << 8, ref refObj, out outObj);
				Assert.That (refObj, Is.Null, "Class-1M-ref");
				Assert.That (outObj, Is.Null, "Class-1M-out");

				// direct native
				refValue = dummyObjHandle; // set to non-null
				outValue = dummyObjHandle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 0, ref refValue, out outValue);
				Assert.That (refValue, Is.EqualTo (NativeHandle.Zero), "Class-1DA-ref");
				Assert.That (outValue, Is.EqualTo (NativeHandle.Zero), "Class-1DA-out");

				// direct managed
				refValue = dummyObjHandle; // set to non-null
				outValue = dummyObjHandle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 8, ref refValue, out outValue);
				Assert.That (refValue, Is.EqualTo (NativeHandle.Zero), "Class-1DM-ref");
				Assert.That (outValue, Is.EqualTo (NativeHandle.Zero), "Class-1DM-out");

				/// 2: verify that refValue points to something
				action = 2;

				// native
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				test (action << 0, ref refObj, out outObj);
				Assert.That (refObj, Is.EqualTo (dummyObj), "Class-2A-ref");
				Assert.That (outObj, Is.Null, "Class-2A-out");

				// managed
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				test (action << 8, ref refObj, out outObj);
				Assert.That (refObj, Is.EqualTo (dummyObj), "Class-2M-ref");
				Assert.That (outObj, Is.Null, "Class-2M-out");

				// direct native
				refValue = dummyObjHandle; // set to non-null
				outValue = dummyObjHandle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 0, ref refValue, out outValue);
				Assert.That (refValue, Is.EqualTo (dummyObj.Handle), "Class-2DA-ref");
				Assert.That (outValue, Is.EqualTo (NativeHandle.Zero), "Class-2DA-out");

				// direct managed
				refValue = dummyObjHandle; // set to non-null
				outValue = dummyObjHandle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 8, ref refValue, out outValue);
				Assert.That (refValue, Is.EqualTo (dummyObj.Handle), "Class-2DM-ref");
				Assert.That (outValue, Is.EqualTo (NativeHandle.Zero), "Class-2DM-out");


				/// 3 set both parameteres to the same Class
				action = 3;

				// native
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				test (action << 0, ref refObj, out outObj);
				Assert.That (refObj.Handle, Is.EqualTo (Class.GetHandle ("NSString")), "Class-3A-ref");
				Assert.That (outObj.Handle, Is.EqualTo (Class.GetHandle ("NSString")), "Class-3A-out");

				// managed
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				test (action << 8, ref refObj, out outObj);
				Assert.That (refObj.Handle, Is.EqualTo (Class.GetHandle (typeof (SomeConsumer))), "Class-3M-ref");
				Assert.That (outObj.Handle, Is.EqualTo (Class.GetHandle (typeof (SomeConsumer))), "Class-3M-out");

				// direct native
				refValue = dummyObjHandle; // set to non-null
				outValue = dummyObjHandle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 0, ref refValue, out outValue);
				Assert.That (outValue, Is.EqualTo (refValue), "String-3DA-eq"); // The managed roundtrip means 'outValue' is re-created (because it's nulled out upon entering managed code), but since Class instances are singletons, we get back the same value.
				Assert.That (refValue, Is.EqualTo (Class.GetHandle ("NSString")), "Class-3DA-ref");
				Assert.That (outValue, Is.EqualTo (Class.GetHandle ("NSString")), "Class-3DA-out");

				// direct managed
				refValue = dummyObjHandle; // set to non-null
				outValue = dummyObjHandle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 8, ref refValue, out outValue);
				Assert.That (outValue, Is.EqualTo (refValue), "Class-3DM-eq"); // The managed roundtrip means 'outValue' is re-created (because it's nulled out upon entering managed code), but since Class instances are singletons, we get back the same value.
				Assert.That (refValue, Is.EqualTo (Class.GetHandle (typeof (SomeConsumer))), "Class-3DM-ref");
				Assert.That (outValue, Is.EqualTo (Class.GetHandle (typeof (SomeConsumer))), "Class-3DM-out");


				/// 4 set both parameteres to different Classes
				action = 4;

				// native
				refObj = null; // set to null
				outObj = null; // set to null
				test (action << 0, ref refObj, out outObj);
				Assert.That (refObj.Handle, Is.EqualTo (Class.GetHandle ("NSBundle")), "Class-4A-ref-value");
				Assert.That (outObj.Handle, Is.EqualTo (Class.GetHandle ("NSDate")), "Class-4A-out-value");

				// managed
				refObj = null; // set to null
				outObj = null; // set to null
				test (action << 8, ref refObj, out outObj);
				Assert.That (refObj.Handle, Is.EqualTo (Class.GetHandle (typeof (RefOutParametersSubclass))), "Class-4M-ref-value");
				Assert.That (outObj.Handle, Is.EqualTo (Class.GetHandle ("RefOutParameters")), "Class-4M-out-value");

				// direct native
				refValue = IntPtr.Zero; // set to null
				outValue = IntPtr.Zero; // set to null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 0, ref refValue, out outValue);
				Assert.That (refValue, Is.EqualTo (Class.GetHandle ("NSBundle")), "Class-4DA-ref-value");
				Assert.That (outValue, Is.EqualTo (Class.GetHandle ("NSDate")), "Class-4DA-out-value");

				// direct managed
				refValue = IntPtr.Zero; // set to null
				outValue = IntPtr.Zero; // set to null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 8, ref refValue, out outValue);
				Assert.That (refValue, Is.EqualTo (Class.GetHandle (typeof (RefOutParametersSubclass))), "Class-4DM-ref-value");
				Assert.That (outValue, Is.EqualTo (Class.GetHandle ("RefOutParameters")), "Class-4DM-out-value");
			}
		}

		void AssertAreEqual (INativeObject [] expected, INativeObject [] actual, string msg)
		{
			if (expected is null && actual is null)
				return;
			if (expected is null ^ actual is null)
				Assert.Fail ($"One is null and the other is not. Expected: {expected} Actual: {actual}. " + msg);
			Assert.That (actual.Length, Is.EqualTo (expected.Length), "Length." + msg);
			for (int i = 0; i < expected.Length; i++) {
				Assert.That (actual [i].Handle, Is.EqualTo (expected [i].Handle), $"Index #{i}: {msg}");
			}
		}

		void AssertAreNotEqual (INativeObject [] expected, INativeObject [] actual, string msg)
		{
			if (expected is null && actual is null)
				Assert.Fail ("Both are null. " + msg);
			if (expected is null ^ actual is null)
				return;
			if (expected.Length != actual.Length)
				return;
			for (int i = 0; i < Math.Min (actual.Length, expected.Length); i++) {
				if (expected [i].Handle != actual [i].Handle)
					return;
			}
			Assert.Fail ("Both arrays are equal. " + msg);
		}

		[Test]
		public unsafe void RefOutTest_INSCodingArray ()
		{
			IntPtr refValue = IntPtr.Zero;
			IntPtr outValue = IntPtr.Zero;

			using (var obj = new RefOutParametersSubclass ()) {
				var sel = Selector.GetHandle ("testINSCodingArray:a:b:");
				var dummyObj = new INSCoding [] { new NSString ("Dummy obj") };
				var dummyArray = NSArray.FromNSObjects<INSCoding> (dummyObj);
				INSCoding [] refObj = null;
				INSCoding [] outObj = null;
				TestMethod<INSCoding []> test = obj.TestINSCodingArray;
				int action;

				/// 1: set both to null
				action = 1;

				// native
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				test (action << 0, ref refObj, out outObj);
				Assert.That (refObj, Is.Null, "NSCodingArray-1A-ref");
				Assert.That (outObj, Is.Null, "NSCodingArray-1A-out");

				// managed
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				test (action << 8, ref refObj, out outObj);
				Assert.That (refObj, Is.Null, "NSCodingArray-1M-ref");
				Assert.That (outObj, Is.Null, "NSCodingArray-1M-out");

				// direct native
				refValue = dummyArray.Handle; // set to non-null
				outValue = dummyArray.Handle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 0, ref refValue, out outValue);
				Assert.That (refValue, Is.EqualTo (IntPtr.Zero), "NSCodingArray-1DA-ref");
				Assert.That (outValue, Is.EqualTo (IntPtr.Zero), "NSCodingArray-1DA-out");

				// direct managed
				refValue = dummyArray.Handle; // set to non-null
				outValue = dummyArray.Handle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 8, ref refValue, out outValue);
				Assert.That (refValue, Is.EqualTo (IntPtr.Zero), "NSCodingArray-1DM-ref");
				Assert.That (outValue, Is.EqualTo (IntPtr.Zero), "NSCodingArray-1DM-out");

				/// 2: verify that refValue points to something
				action = 2;

				// native
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				test (action << 0, ref refObj, out outObj);
				AssertAreEqual (dummyObj, refObj, "NSCodingArray-2A-ref");
				Assert.That (refObj, Is.SameAs (dummyObj), "NSCodingArray-2A-ref-same");
				Assert.That (outObj, Is.Null, "NSCodingArray-2A-out");

				// managed
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				test (action << 8, ref refObj, out outObj);
				AssertAreEqual (dummyObj, refObj, "NSCodingArray-2M-ref");
				Assert.That (refObj, Is.SameAs (dummyObj), "NSCodingArray-2M-ref-same");
				Assert.That (outObj, Is.Null, "NSCodingArray-2M-out");

				// direct native
				refValue = dummyArray.Handle; // set to non-null
				outValue = dummyArray.Handle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 0, ref refValue, out outValue);
				AssertAreEqual (dummyObj, NSArray.ArrayFromHandle<INSCoding> (refValue), "NSCodingArray-2DA-ref");
				Assert.That (outValue, Is.EqualTo (IntPtr.Zero), "NSCodingArray-2DA-out");

				// direct managed
				refValue = dummyArray.Handle; // set to non-null
				outValue = dummyArray.Handle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 8, ref refValue, out outValue);
				AssertAreEqual (dummyObj, NSArray.ArrayFromHandle<INSCoding> (refValue), "NSCodingArray-2DM-ref");
				Assert.That (outValue, Is.EqualTo (IntPtr.Zero), "NSCodingArray-2DM-out");


				/// 3 set both parameters to the same pointer of an NSCodingArray array
				action = 3;

				// native
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				test (action << 0, ref refObj, out outObj);
				Assert.That (refObj, Is.Not.SameAs (dummyObj), "NSCodingArray-3A-ref-same");
				Assert.That (outObj, Is.Not.SameAs (dummyObj), "NSCodingArray-3A-ref-out");
				AssertAreEqual (refObj, outObj, "NSCodingArray-3A-out-ref-eq");
				Assert.That (outObj, Is.Not.SameAs (refObj), "NSCodingArray-3A-ref-out-not-safe");
				Assert.That (refObj [0].GetType ().FullName, Does.Contain ("CodingWrapper"), "NSCodingArray-3A-ref-wrapper-type");
				Assert.That (outObj [0].GetType ().FullName, Does.Contain ("CodingWrapper"), "NSCodingArray-3A-ref-wrapper-type");

				// managed
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				test (action << 8, ref refObj, out outObj);
				Assert.That (refObj, Is.Not.SameAs (dummyObj), "NSCodingArray-3M-ref-same");
				Assert.That (outObj, Is.Not.SameAs (dummyObj), "NSCodingArray-3M-ref-out");
				AssertAreEqual (refObj, outObj, "NSCodingArray-3M-ref-out-not-safe");
				Assert.That (refObj [0], Is.TypeOf<NSString> (), "NSCodingArray-3M-ref-wrapper-type");
				Assert.That (outObj [0], Is.TypeOf<NSString> (), "NSCodingArray-3M-ref-wrapper-type");

				// direct native
				refValue = dummyArray.Handle; // set to non-null
				outValue = dummyArray.Handle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 0, ref refValue, out outValue);
				Assert.That (outValue, Is.Not.SameAs (refValue), "NSCodingArray-3DA-out-ref-not-same");
				AssertAreEqual (refObj, outObj, "NSCodingArray-3DA-out-ref-equal");
				Assert.That (refObj [0], Is.TypeOf<NSString> (), "NSCodingArray-3DA-ref-wrapper-type");
				Assert.That (outObj [0], Is.TypeOf<NSString> (), "NSCodingArray-3DA-ref-wrapper-type");

				// direct managed
				refValue = dummyArray.Handle; // set to non-null
				outValue = dummyArray.Handle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 8, ref refValue, out outValue);
				Assert.That (outValue, Is.Not.SameAs (refValue), "NSCodingArray-3DM-out-ref-not-same");
				AssertAreEqual (refObj, outObj, "NSCodingArray-3DM-out-ref-equal");
				Assert.That (refObj [0], Is.TypeOf<NSString> (), "NSCodingArray-3DM-ref-wrapper-type");
				Assert.That (outObj [0], Is.TypeOf<NSString> (), "NSCodingArray-3DM-ref-wrapper-type");


				/// 4 set both parameteres to different pointers of a NSCodingArray
				action = 4;

				// native
				refObj = null; // set to null
				outObj = null; // set to null
				test (action << 0, ref refObj, out outObj);
				Assert.That (refObj, Is.Not.Null, "NSCodingArray-4A-ref");
				Assert.That (outObj, Is.Not.Null, "NSCodingArray-4A-out");
				AssertAreNotEqual (refObj, outObj, "NSCodingArray-4A-ref-distinct");
				Assert.That (refObj [0].GetType ().FullName, Does.Contain ("NSNumber").Or.Contain ("CodingWrapper"), "NSCodingArray-4A-ref-wrapper-type");
				Assert.That (outObj [0].GetType ().FullName, Does.Contain ("NSNumber").Or.Contain ("CodingWrapper"), "NSCodingArray-4A-ref-wrapper-type");

				// managed
				refObj = null; // set to null
				outObj = null; // set to null
				test (action << 8, ref refObj, out outObj);
				AssertAreNotEqual (refObj, outObj, "NSCodingArray-4M-ref-distinct");
				Assert.That (refObj [0], Is.TypeOf<NSString> (), "NSCodingArray-4M-ref-wrapper-type");
				Assert.That (outObj [0], Is.TypeOf<NSString> (), "NSCodingArray-4M-ref-wrapper-type");

				// direct native
				refValue = IntPtr.Zero; // set to null
				outValue = IntPtr.Zero; // set to null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 0, ref refValue, out outValue);
				AssertAreNotEqual (NSArray.ArrayFromHandle<INSCoding> (refValue), NSArray.ArrayFromHandle<INSCoding> (outValue), "NSCodingArray-4DA-ref-distinct");
				Assert.That (refObj [0], Is.TypeOf<NSString> (), "NSCodingArray-4DA-ref-wrapper-type");
				Assert.That (outObj [0], Is.TypeOf<NSString> (), "NSCodingArray-4DA-ref-wrapper-type");

				// direct managed
				refValue = IntPtr.Zero; // set to null
				outValue = IntPtr.Zero; // set to null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 8, ref refValue, out outValue);
				AssertAreNotEqual (NSArray.ArrayFromHandle<INSCoding> (refValue), NSArray.ArrayFromHandle<INSCoding> (outValue), "NSCodingArray-4DM-ref-distinct");
				Assert.That (refObj [0], Is.TypeOf<NSString> (), "NSCodingArray-4DM-ref-wrapper-type");
				Assert.That (outObj [0], Is.TypeOf<NSString> (), "NSCodingArray-4DM-ref-wrapper-type");
			}
		}

		delegate void TestMethod<T> (int action, ref T refObj, out T outObj);

		[Test]
		public unsafe void RefOutTest_NSObjectArray ()
		{
			IntPtr refValue = IntPtr.Zero;
			IntPtr outValue = IntPtr.Zero;

			using (var obj = new RefOutParametersSubclass ()) {
				var sel = Selector.GetHandle ("testNSObjectArray:a:b:");
				var dummyObj = new NSObject [] { new NSString ("Dummy obj") };
				var dummyArray = NSArray.FromNSObjects<NSObject> (dummyObj);
				NSObject [] refObj = null;
				NSObject [] outObj = null;
				TestMethod<NSObject []> test = obj.TestNSObjectArray;
				int action;

				/// 1: set both to null
				action = 1;

				// native
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				test (action << 0, ref refObj, out outObj);
				Assert.That (refObj, Is.Null, "NSObjectArray-1A-ref");
				Assert.That (outObj, Is.Null, "NSObjectArray-1A-out");

				// managed
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				test (action << 8, ref refObj, out outObj);
				Assert.That (refObj, Is.Null, "NSObjectArray-1M-ref");
				Assert.That (outObj, Is.Null, "NSObjectArray-1M-out");

				// direct native
				refValue = dummyArray.Handle; // set to non-null
				outValue = dummyArray.Handle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 0, ref refValue, out outValue);
				Assert.That (refValue, Is.EqualTo (IntPtr.Zero), "NSObjectArray-1DA-ref");
				Assert.That (outValue, Is.EqualTo (IntPtr.Zero), "NSObjectArray-1DA-out");

				// direct managed
				refValue = dummyArray.Handle; // set to non-null
				outValue = dummyArray.Handle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 8, ref refValue, out outValue);
				Assert.That (refValue, Is.EqualTo (IntPtr.Zero), "NSObjectArray-1DM-ref");
				Assert.That (outValue, Is.EqualTo (IntPtr.Zero), "NSObjectArray-1DM-out");

				/// 2: verify that refValue points to something
				action = 2;

				// native
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				test (action << 0, ref refObj, out outObj);
				AssertAreEqual (dummyObj, refObj, "NSObjectArray-2A-ref");
				Assert.That (refObj, Is.SameAs (dummyObj), "NSObjectArray-2A-ref-same");
				Assert.That (outObj, Is.Null, "NSObjectArray-2A-out");

				// managed
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				test (action << 8, ref refObj, out outObj);
				AssertAreEqual (dummyObj, refObj, "NSObjectArray-2M-ref");
				Assert.That (refObj, Is.SameAs (dummyObj), "NSObjectArray-2M-ref-same");
				Assert.That (outObj, Is.Null, "NSObjectArray-2M-out");

				// direct native
				refValue = dummyArray.Handle; // set to non-null
				outValue = dummyArray.Handle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 0, ref refValue, out outValue);
				AssertAreEqual (dummyObj, NSArray.ArrayFromHandle<NSObject> (refValue), "NSObjectArray-2DA-ref");
				Assert.That (outValue, Is.EqualTo (IntPtr.Zero), "NSObjectArray-2DA-out");

				// direct managed
				refValue = dummyArray.Handle; // set to non-null
				outValue = dummyArray.Handle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 8, ref refValue, out outValue);
				AssertAreEqual (dummyObj, NSArray.ArrayFromHandle<NSObject> (refValue), "NSObjectArray-2DM-ref");
				Assert.That (outValue, Is.EqualTo (IntPtr.Zero), "NSObjectArray-2DM-out");


				/// 3 set both parameters to the same pointer of an NSObjectArray array
				action = 3;

				// native
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				test (action << 0, ref refObj, out outObj);
				Assert.That (refObj, Is.Not.SameAs (dummyObj), "NSObjectArray-3A-ref-same");
				Assert.That (outObj, Is.Not.SameAs (dummyObj), "NSObjectArray-3A-ref-out");
				AssertAreEqual (refObj, outObj, "NSObjectArray-3A-out-ref-eq");
				Assert.That (outObj, Is.Not.SameAs (refObj), "NSObjectArray-3A-ref-out-not-safe");
				Assert.That (refObj, Is.EquivalentTo (new NSObject [] { (NSString) "Hello", (NSString) "World" }), "NSObjectArray-3A-ref-equiv");
				Assert.That (outObj, Is.EquivalentTo (new NSObject [] { (NSString) "Hello", (NSString) "World" }), "NSObjectArray-3A-obj-equiv");

				// managed
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				test (action << 8, ref refObj, out outObj);
				Assert.That (refObj, Is.Not.SameAs (dummyObj), "NSObjectArray-3M-ref-same");
				Assert.That (outObj, Is.Not.SameAs (dummyObj), "NSObjectArray-3M-ref-out");
				AssertAreEqual (refObj, outObj, "NSObjectArray-3M-ref-out-not-safe");
				Assert.That (refObj, Is.EquivalentTo (new NSObject [] { (NSString) "Hello", (NSString) "World", (NSString) "from", (NSString) "managed" }), "NSObjectArray-3M-ref-equiv");
				Assert.That (outObj, Is.EquivalentTo (new NSObject [] { (NSString) "Hello", (NSString) "World", (NSString) "from", (NSString) "managed" }), "NSObjectArray-3M-obj-equiv");

				// direct native
				refValue = dummyArray.Handle; // set to non-null
				outValue = dummyArray.Handle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 0, ref refValue, out outValue);
				Assert.That (outValue, Is.Not.EqualTo (refValue), "NSObjectArray-3DA-out-ref-not-same");
				refObj = NSArray.ArrayFromHandle<NSObject> (refValue);
				outObj = NSArray.ArrayFromHandle<NSObject> (outValue);
				Assert.That (refObj, Is.EquivalentTo (new NSObject [] { (NSString) "Hello", (NSString) "World" }), "NSObjectArray-3DA-ref-equiv");
				Assert.That (outObj, Is.EquivalentTo (new NSObject [] { (NSString) "Hello", (NSString) "World" }), "NSObjectArray-3DA-obj-equiv");

				// direct managed
				refValue = dummyArray.Handle; // set to non-null
				outValue = dummyArray.Handle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 8, ref refValue, out outValue);
				Assert.That (outValue, Is.Not.EqualTo (refValue), "NSObjectArray-3DM-out-ref-not-same");
				refObj = NSArray.ArrayFromHandle<NSObject> (refValue);
				outObj = NSArray.ArrayFromHandle<NSObject> (outValue);
				Assert.That (refObj, Is.EquivalentTo (new NSObject [] { (NSString) "Hello", (NSString) "World", (NSString) "from", (NSString) "managed" }), "NSObjectArray-3DM-ref-equiv");
				Assert.That (outObj, Is.EquivalentTo (new NSObject [] { (NSString) "Hello", (NSString) "World", (NSString) "from", (NSString) "managed" }), "NSObjectArray-3DM-obj-equiv");


				/// 4 set both parameteres to different pointers of a NSObjectArray
				action = 4;

				// native
				refObj = null; // set to null
				outObj = null; // set to null
				test (action << 0, ref refObj, out outObj);
				Assert.That (refObj, Is.EquivalentTo (new NSObject [] { NSNumber.FromInt32 (3), NSNumber.FromInt32 (14) }), "NSObjectArray-4A-ref-equiv");
				Assert.That (outObj, Is.EquivalentTo (new NSObject [] { (NSString) "Hello", (NSString) "Xamarin" }), "NSObjectArray-4A-obj-equiv");

				// managed
				refObj = null; // set to null
				outObj = null; // set to null
				test (action << 8, ref refObj, out outObj);
				AssertAreNotEqual (refObj, outObj, "NSObjectArray-4M-ref-distinct");
				Assert.That (refObj, Is.EquivalentTo (new NSObject [] { NSNumber.FromInt32 (2), NSNumber.FromInt32 (71) }), "NSObjectArray-4M-ref-equiv");
				Assert.That (outObj, Is.EquivalentTo (new NSObject [] { (NSString) "Hello", (NSString) "Microsoft", (NSString) "from", (NSString) "managed" }), "NSObjectArray-4M-obj-equiv");

				// direct native
				refValue = IntPtr.Zero; // set to null
				outValue = IntPtr.Zero; // set to null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 0, ref refValue, out outValue);
				refObj = NSArray.ArrayFromHandle<NSObject> (refValue);
				outObj = NSArray.ArrayFromHandle<NSObject> (outValue);
				Assert.That (refObj, Is.EquivalentTo (new NSObject [] { NSNumber.FromInt32 (3), NSNumber.FromInt32 (14) }), "NSObjectArray-4DA-ref-equiv");
				Assert.That (outObj, Is.EquivalentTo (new NSObject [] { (NSString) "Hello", (NSString) "Xamarin" }), "NSObjectArray-4DA-obj-equiv");

				// direct managed
				refValue = IntPtr.Zero; // set to null
				outValue = IntPtr.Zero; // set to null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 8, ref refValue, out outValue);
				refObj = NSArray.ArrayFromHandle<NSObject> (refValue);
				outObj = NSArray.ArrayFromHandle<NSObject> (outValue);
				Assert.That (refObj, Is.EquivalentTo (new NSObject [] { NSNumber.FromInt32 (2), NSNumber.FromInt32 (71) }), "NSObjectArray-4DM-ref-equiv");
				Assert.That (outObj, Is.EquivalentTo (new NSObject [] { (NSString) "Hello", (NSString) "Microsoft", (NSString) "from", (NSString) "managed" }), "NSObjectArray-4DM-obj-equiv");
			}
		}

		[Test]
		public unsafe void RefOutTest_NSValueArray ()
		{
			IntPtr refValue = IntPtr.Zero;
			IntPtr outValue = IntPtr.Zero;

			using (var obj = new RefOutParametersSubclass ()) {
				var sel = Selector.GetHandle ("testNSValueArray:a:b:");
				var dummyObj = new NSValue [] { NSValue.FromMKCoordinate (new CLLocationCoordinate2D (3, 14)) };
				var dummyArray = NSArray.FromNSObjects<NSValue> (dummyObj);
				NSValue [] refObj = null;
				NSValue [] outObj = null;
				TestMethod<NSValue []> test = obj.TestNSValueArray;
				int action;

				/// 1: set both to null
				action = 1;

				// native
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				test (action << 0, ref refObj, out outObj);
				Assert.That (refObj, Is.Null, "NSValueArray-1A-ref");
				Assert.That (outObj, Is.Null, "NSValueArray-1A-out");

				// managed
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				test (action << 8, ref refObj, out outObj);
				Assert.That (refObj, Is.Null, "NSValueArray-1M-ref");
				Assert.That (outObj, Is.Null, "NSValueArray-1M-out");

				// direct native
				refValue = dummyArray.Handle; // set to non-null
				outValue = dummyArray.Handle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 0, ref refValue, out outValue);
				Assert.That (refValue, Is.EqualTo (IntPtr.Zero), "NSValueArray-1DA-ref");
				Assert.That (outValue, Is.EqualTo (IntPtr.Zero), "NSValueArray-1DA-out");

				// direct managed
				refValue = dummyArray.Handle; // set to non-null
				outValue = dummyArray.Handle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 8, ref refValue, out outValue);
				Assert.That (refValue, Is.EqualTo (IntPtr.Zero), "NSValueArray-1DM-ref");
				Assert.That (outValue, Is.EqualTo (IntPtr.Zero), "NSValueArray-1DM-out");

				/// 2: verify that refValue points to something
				action = 2;

				// native
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				test (action << 0, ref refObj, out outObj);
				AssertAreEqual (dummyObj, refObj, "NSValueArray-2A-ref");
				Assert.That (refObj, Is.SameAs (dummyObj), "NSValueArray-2A-ref-same");
				Assert.That (outObj, Is.Null, "NSValueArray-2A-out");

				// managed
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				test (action << 8, ref refObj, out outObj);
				AssertAreEqual (dummyObj, refObj, "NSValueArray-2M-ref");
				Assert.That (refObj, Is.SameAs (dummyObj), "NSValueArray-2M-ref-same");
				Assert.That (outObj, Is.Null, "NSValueArray-2M-out");

				// direct native
				refValue = dummyArray.Handle; // set to non-null
				outValue = dummyArray.Handle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 0, ref refValue, out outValue);
				AssertAreEqual (dummyObj, NSArray.ArrayFromHandle<NSValue> (refValue), "NSValueArray-2DA-ref");
				Assert.That (outValue, Is.EqualTo (IntPtr.Zero), "NSValueArray-2DA-out");

				// direct managed
				refValue = dummyArray.Handle; // set to non-null
				outValue = dummyArray.Handle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 8, ref refValue, out outValue);
				AssertAreEqual (dummyObj, NSArray.ArrayFromHandle<NSValue> (refValue), "NSValueArray-2DM-ref");
				Assert.That (outValue, Is.EqualTo (IntPtr.Zero), "NSValueArray-2DM-out");


				/// 3 set both parameters to the same pointer of an NSValueArray array
				action = 3;

				// native
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				test (action << 0, ref refObj, out outObj);
				Assert.That (refObj, Is.Not.SameAs (dummyObj), "NSValueArray-3A-ref-same");
				Assert.That (outObj, Is.Not.SameAs (dummyObj), "NSValueArray-3A-ref-out");
				AssertAreEqual (refObj, outObj, "NSValueArray-3A-out-ref-eq");
				Assert.That (outObj, Is.Not.SameAs (refObj), "NSValueArray-3A-ref-out-not-safe");
				Assert.That (refObj [0], Is.TypeOf<NSValue> (), "NSValueArray-3A-ref-wrapper-type");
				Assert.That (outObj [0], Is.TypeOf<NSValue> (), "NSValueArray-3A-ref-wrapper-type");

				// managed
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				test (action << 8, ref refObj, out outObj);
				Assert.That (refObj, Is.Not.SameAs (dummyObj), "NSValueArray-3M-ref-same");
				Assert.That (outObj, Is.Not.SameAs (dummyObj), "NSValueArray-3M-ref-out");
				AssertAreEqual (refObj, outObj, "NSValueArray-3M-ref-out-not-safe");
				Assert.That (refObj [0], Is.TypeOf<NSValue> (), "NSValueArray-3M-ref-wrapper-type");
				Assert.That (outObj [0], Is.TypeOf<NSValue> (), "NSValueArray-3M-ref-wrapper-type");

				// direct native
				refValue = dummyArray.Handle; // set to non-null
				outValue = dummyArray.Handle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 0, ref refValue, out outValue);
				Assert.That (outValue, Is.Not.SameAs (refValue), "NSValueArray-3DA-out-ref-not-same");
				AssertAreEqual (refObj, outObj, "NSValueArray-3DA-out-ref-equal");
				Assert.That (refObj [0], Is.TypeOf<NSValue> (), "NSValueArray-3DA-ref-wrapper-type");
				Assert.That (outObj [0], Is.TypeOf<NSValue> (), "NSValueArray-3DA-ref-wrapper-type");

				// direct managed
				refValue = dummyArray.Handle; // set to non-null
				outValue = dummyArray.Handle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 8, ref refValue, out outValue);
				Assert.That (outValue, Is.Not.SameAs (refValue), "NSValueArray-3DM-out-ref-not-same");
				AssertAreEqual (refObj, outObj, "NSValueArray-3DM-out-ref-equal");
				Assert.That (refObj [0], Is.TypeOf<NSValue> (), "NSValueArray-3DM-ref-wrapper-type");
				Assert.That (outObj [0], Is.TypeOf<NSValue> (), "NSValueArray-3DM-ref-wrapper-type");


				/// 4 set both parameteres to different pointers of a NSValueArray
				action = 4;

				// native
				refObj = null; // set to null
				outObj = null; // set to null
				test (action << 0, ref refObj, out outObj);
				Assert.That (refObj, Is.Not.Null, "NSValueArray-4A-ref");
				Assert.That (outObj, Is.Not.Null, "NSValueArray-4A-out");
				AssertAreNotEqual (refObj, outObj, "NSValueArray-4A-ref-distinct");
				Assert.That (refObj [0], Is.TypeOf<NSValue> (), "NSValueArray-4A-ref-wrapper-type");
				Assert.That (outObj [0], Is.TypeOf<NSValue> (), "NSValueArray-4A-ref-wrapper-type");

				// managed
				refObj = null; // set to null
				outObj = null; // set to null
				test (action << 8, ref refObj, out outObj);
				AssertAreNotEqual (refObj, outObj, "NSValueArray-4M-ref-distinct");
				Assert.That (refObj [0], Is.TypeOf<NSValue> (), "NSValueArray-4M-ref-wrapper-type");
				Assert.That (outObj [0], Is.TypeOf<NSValue> (), "NSValueArray-4M-ref-wrapper-type");

				// direct native
				refValue = IntPtr.Zero; // set to null
				outValue = IntPtr.Zero; // set to null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 0, ref refValue, out outValue);
				AssertAreNotEqual (NSArray.ArrayFromHandle<NSValue> (refValue), NSArray.ArrayFromHandle<NSValue> (outValue), "NSValueArray-4DA-ref-distinct");
				Assert.That (refObj [0], Is.TypeOf<NSValue> (), "NSValueArray-4DA-ref-wrapper-type");
				Assert.That (outObj [0], Is.TypeOf<NSValue> (), "NSValueArray-4DA-ref-wrapper-type");

				// direct managed
				refValue = IntPtr.Zero; // set to null
				outValue = IntPtr.Zero; // set to null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 8, ref refValue, out outValue);
				AssertAreNotEqual (NSArray.ArrayFromHandle<NSValue> (refValue), NSArray.ArrayFromHandle<NSValue> (outValue), "NSValueArray-4DM-ref-distinct");
				Assert.That (refObj [0], Is.TypeOf<NSValue> (), "NSValueArray-4DM-ref-wrapper-type");
				Assert.That (outObj [0], Is.TypeOf<NSValue> (), "NSValueArray-4DM-ref-wrapper-type");
			}
		}

		[Test]
		public unsafe void RefOutTest_StringArray ()
		{
			var refValue = NativeHandle.Zero;
			var outValue = NativeHandle.Zero;

			using (var obj = new RefOutParametersSubclass ()) {
				var sel = Selector.GetHandle ("testStringArray:a:b:");
				var dummyObj = new string [] { "Hello" };
				var dummyArray = NSArray.FromStrings (dummyObj);
				string [] refObj = null;
				string [] outObj = null;
				TestMethod<string []> test = obj.TestStringArray;
				int action;

				/// 1: set both to null
				action = 1;

				// native
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				test (action << 0, ref refObj, out outObj);
				Assert.That (refObj, Is.Null, "NSStringArray-1A-ref");
				Assert.That (outObj, Is.Null, "NSStringArray-1A-out");

				// managed
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				test (action << 8, ref refObj, out outObj);
				Assert.That (refObj, Is.Null, "NSStringArray-1M-ref");
				Assert.That (outObj, Is.Null, "NSStringArray-1M-out");

				// direct native
				refValue = dummyArray.Handle; // set to non-null
				outValue = dummyArray.Handle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 0, ref refValue, out outValue);
				Assert.That (refValue, Is.EqualTo (NativeHandle.Zero), "NSStringArray-1DA-ref");
				Assert.That (outValue, Is.EqualTo (NativeHandle.Zero), "NSStringArray-1DA-out");

				// direct managed
				refValue = dummyArray.Handle; // set to non-null
				outValue = dummyArray.Handle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 8, ref refValue, out outValue);
				Assert.That (refValue, Is.EqualTo (NativeHandle.Zero), "NSStringArray-1DM-ref");
				Assert.That (outValue, Is.EqualTo (NativeHandle.Zero), "NSStringArray-1DM-out");

				/// 2: verify that refValue points to something
				action = 2;

				// native
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				test (action << 0, ref refObj, out outObj);
				Assert.That (dummyObj, Is.EquivalentTo (refObj), "NSStringArray-2A-ref");
				Assert.That (refObj, Is.SameAs (dummyObj), "NSStringArray-2A-ref-same");
				Assert.That (outObj, Is.Null, "NSStringArray-2A-out");

				// managed
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				test (action << 8, ref refObj, out outObj);
				Assert.That (dummyObj, Is.EquivalentTo (refObj), "NSStringArray-2M-ref");
				Assert.That (refObj, Is.SameAs (dummyObj), "NSStringArray-2M-ref-same");
				Assert.That (outObj, Is.Null, "NSStringArray-2M-out");

				// direct native
				refValue = dummyArray.Handle; // set to non-null
				outValue = dummyArray.Handle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 0, ref refValue, out outValue);
				Assert.That (dummyObj, Is.EquivalentTo (NSArray.StringArrayFromHandle (refValue)), "NSStringArray-2DA-ref");
				Assert.That (refValue, Is.EqualTo (dummyArray.Handle), "NSStringArray-2DA-ref");
				Assert.That (outValue, Is.EqualTo (NativeHandle.Zero), "NSStringArray-2DA-out");

				// direct managed
				refValue = dummyArray.Handle; // set to non-null
				outValue = dummyArray.Handle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 8, ref refValue, out outValue);
				Assert.That (dummyObj, Is.EquivalentTo (NSArray.StringArrayFromHandle (refValue)), "NSStringArray-2DM-ref");
				Assert.That (refValue, Is.EqualTo (dummyArray.Handle), "NSStringArray-2DM-ref");
				Assert.That (outValue, Is.EqualTo (NativeHandle.Zero), "NSStringArray-2DM-out");


				/// 3 set both parameters to the same pointer of an NSStringArray array
				action = 3;

				// native
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				test (action << 0, ref refObj, out outObj);
				Assert.That (outObj, Is.EquivalentTo (new string [] { "Hello", "World" }), "NSStringArray-3A-out-contents");
				Assert.That (refObj, Is.EquivalentTo (new string [] { "Hello", "World" }), "NSStringArray-3A-ref-contents");

				// managed
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				test (action << 8, ref refObj, out outObj);
				Assert.That (outObj, Is.EquivalentTo (new string [] { "Hello", "Managed", "World" }), "NSStringArray-3M-out-contents");
				Assert.That (refObj, Is.EquivalentTo (new string [] { "Hello", "Managed", "World" }), "NSStringArray-3M-ref-contents");

				// direct native
				refValue = dummyArray.Handle; // set to non-null
				outValue = dummyArray.Handle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 0, ref refValue, out outValue);
				refObj = NSArray.StringArrayFromHandle (refValue);
				outObj = NSArray.StringArrayFromHandle (outValue);
				Assert.That (outObj, Is.EquivalentTo (new string [] { "Hello", "World" }), "NSStringArray-3DA-out-contents");
				Assert.That (refObj, Is.EquivalentTo (new string [] { "Hello", "World" }), "NSStringArray-3DA-ref-contents");

				// direct managed
				refValue = dummyArray.Handle; // set to non-null
				outValue = dummyArray.Handle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 8, ref refValue, out outValue);
				refObj = NSArray.StringArrayFromHandle (refValue);
				outObj = NSArray.StringArrayFromHandle (outValue);
				Assert.That (outObj, Is.EquivalentTo (new string [] { "Hello", "Managed", "World" }), "NSStringArray-3DM-out-contents");
				Assert.That (refObj, Is.EquivalentTo (new string [] { "Hello", "Managed", "World" }), "NSStringArray-3DM-ref-contents");


				/// 4 set both parameteres to different pointers of a NSStringArray
				action = 4;

				// native
				refObj = null; // set to null
				outObj = null; // set to null
				test (action << 0, ref refObj, out outObj);
				Assert.That (refObj, Is.Not.Null, "NSStringArray-4A-ref");
				Assert.That (outObj, Is.Not.Null, "NSStringArray-4A-out");
				Assert.That (refObj, Is.EquivalentTo (new string [] { "Hello", "Microsoft" }), "NSStringArray-4A-ref-equiv");
				Assert.That (outObj, Is.EquivalentTo (new string [] { "Hello", "Xamarin" }), "NSStringArray-4A-obj-equiv");

				// managed
				refObj = null; // set to null
				outObj = null; // set to null
				test (action << 8, ref refObj, out outObj);
				Assert.That (refObj, Is.EquivalentTo (new string [] { "Hello", "Microsoft", "from", "managed" }), "NSStringArray-4M-ref-equiv");
				Assert.That (outObj, Is.EquivalentTo (new string [] { "Hello", "Xamarin", "from", "managed" }), "NSStringArray-4M-obj-equiv");

				// direct native
				refValue = IntPtr.Zero; // set to null
				outValue = IntPtr.Zero; // set to null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 0, ref refValue, out outValue);
				refObj = NSArray.StringArrayFromHandle (refValue);
				outObj = NSArray.StringArrayFromHandle (outValue);
				Assert.That (refObj, Is.EquivalentTo (new string [] { "Hello", "Microsoft" }), "NSStringArray-4DA-ref-equiv");
				Assert.That (outObj, Is.EquivalentTo (new string [] { "Hello", "Xamarin" }), "NSStringArray-4DA-obj-equiv");

				// direct managed
				refValue = IntPtr.Zero; // set to null
				outValue = IntPtr.Zero; // set to null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 8, ref refValue, out outValue);
				refObj = NSArray.StringArrayFromHandle (refValue);
				outObj = NSArray.StringArrayFromHandle (outValue);
				Assert.That (refObj, Is.EquivalentTo (new string [] { "Hello", "Microsoft", "from", "managed" }), "NSStringArray-4DM-ref-equiv");
				Assert.That (outObj, Is.EquivalentTo (new string [] { "Hello", "Xamarin", "from", "managed" }), "NSStringArray-4DM-obj-equiv");

				/// 5 change a value in the ref input
				/// Here we verify that changing an element in the input array is not copied back to the ref argument.
				/// If an element needs to be changed, a new array must be created, and elements copied over, including the modified one.
				action = 5;

				// Only managed calls can be done because we need to use an NSMutableArray, and the binding code creates NSArrays when marshalling to native code.
				Func<string [], NSMutableArray> fromStrings = (arr) => {
					var rv = new NSMutableArray ((nuint) arr.Length);
					for (int i = 0; i < arr.Length; i++)
						rv.Add ((NSString) arr [i]);
					return rv;
				};

				// managed
				refObj = new string [] { "Hello", "World" };
				test (action << 8, ref refObj, out var _);
				// Here the array changed, because we didn't go through any binding code at all
				Assert.That (refObj, Is.EquivalentTo (new string [] { "Hello", "Universe" }), "NSStringArray-5M-ref-equiv");

				// direct managed
				refObj = new string [] { "Hello", "World" };
				using (var arr = fromStrings (refObj)) {
					refValue = arr.Handle;
					Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 8, ref refValue, out var _);
					refObj = NSArray.StringArrayFromHandle (refValue);
					Assert.That (refObj, Is.EquivalentTo (new string [] { "Hello", "World" }), "NSStringArray-5DM-ref-equiv");
				}
			}
		}

		[Test]
		public unsafe void RefOutTest_ClassArray ()
		{
			var refValue = NativeHandle.Zero;
			var outValue = NativeHandle.Zero;

			using (var obj = new RefOutParametersSubclass ()) {
				var sel = Selector.GetHandle ("testClassArray:a:b:");
				var dummyObj = new Class [] { new Class (typeof (NSObject)) };
				var dummyArray = NSArray.FromIntPtrs (new NativeHandle [] { Class.GetHandle (typeof (NSObject)) });
				Class [] refObj = null;
				Class [] outObj = null;
				TestMethod<Class []> test = obj.TestClassArray;
				int action;

				/// 1: set both to null
				action = 1;

				// native
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				test (action << 0, ref refObj, out outObj);
				Assert.That (refObj, Is.Null, "NSClassArray-1A-ref");
				Assert.That (outObj, Is.Null, "NSClassArray-1A-out");

				// managed
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				test (action << 8, ref refObj, out outObj);
				Assert.That (refObj, Is.Null, "NSClassArray-1M-ref");
				Assert.That (outObj, Is.Null, "NSClassArray-1M-out");

				// direct native
				refValue = dummyArray.Handle; // set to non-null
				outValue = dummyArray.Handle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 0, ref refValue, out outValue);
				Assert.That (refValue, Is.EqualTo (NativeHandle.Zero), "NSClassArray-1DA-ref");
				Assert.That (outValue, Is.EqualTo (NativeHandle.Zero), "NSClassArray-1DA-out");

				// direct managed
				refValue = dummyArray.Handle; // set to non-null
				outValue = dummyArray.Handle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 8, ref refValue, out outValue);
				Assert.That (refValue, Is.EqualTo (NativeHandle.Zero), "NSClassArray-1DM-ref");
				Assert.That (outValue, Is.EqualTo (NativeHandle.Zero), "NSClassArray-1DM-out");

				/// 2: verify that refValue points to something
				action = 2;

				// native
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				test (action << 0, ref refObj, out outObj);
				Assert.That (dummyObj, Is.EquivalentTo (refObj), "NSClassArray-2A-ref");
				Assert.That (refObj, Is.SameAs (dummyObj), "NSClassArray-2A-ref-same");
				Assert.That (outObj, Is.Null, "NSClassArray-2A-out");

				// managed
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				test (action << 8, ref refObj, out outObj);
				Assert.That (dummyObj, Is.EquivalentTo (refObj), "NSClassArray-2M-ref");
				Assert.That (refObj, Is.SameAs (dummyObj), "NSClassArray-2M-ref-same");
				Assert.That (outObj, Is.Null, "NSClassArray-2M-out");

				// direct native
				refValue = dummyArray.Handle; // set to non-null
				outValue = dummyArray.Handle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 0, ref refValue, out outValue);
				Assert.That (dummyObj, Is.EquivalentTo (NSArray.ArrayFromHandle<Class> (refValue)), "NSClassArray-2DA-ref");
				Assert.That (refValue, Is.EqualTo (dummyArray.Handle), "NSClassArray-2DA-ref");
				Assert.That (outValue, Is.EqualTo (NativeHandle.Zero), "NSClassArray-2DA-out");

				// direct managed
				refValue = dummyArray.Handle; // set to non-null
				outValue = dummyArray.Handle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 8, ref refValue, out outValue);
				Assert.That (dummyObj, Is.EquivalentTo (NSArray.ArrayFromHandle<Class> (refValue)), "NSClassArray-2DM-ref");
				Assert.That (refValue, Is.EqualTo (dummyArray.Handle), "NSClassArray-2DM-ref");
				Assert.That (outValue, Is.EqualTo (NativeHandle.Zero), "NSClassArray-2DM-out");


				/// 3 set both parameters to the same pointer of an Class array
				action = 3;

				// native
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				test (action << 0, ref refObj, out outObj);
				Assert.That (outObj, Is.EquivalentTo (new Class [] { new Class (typeof (NSString)), new Class (typeof (NSDate)) }), "NSClassArray-3A-out-contents");
				Assert.That (refObj, Is.EquivalentTo (new Class [] { new Class (typeof (NSString)), new Class (typeof (NSDate)) }), "NSClassArray-3A-ref-contents");

				// managed
				refObj = dummyObj; // set to non-null
				outObj = dummyObj; // set to non-null
				test (action << 8, ref refObj, out outObj);
				Assert.That (outObj, Is.EquivalentTo (new Class [] { new Class (typeof (NSSet)), new Class (typeof (NSDictionary)) }), "NSClassArray-3M-out-contents");
				Assert.That (refObj, Is.EquivalentTo (new Class [] { new Class (typeof (NSSet)), new Class (typeof (NSDictionary)) }), "NSClassArray-3M-ref-contents");

				// direct native
				refValue = dummyArray.Handle; // set to non-null
				outValue = dummyArray.Handle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 0, ref refValue, out outValue);
				refObj = NSArray.ArrayFromHandle<Class> (refValue);
				outObj = NSArray.ArrayFromHandle<Class> (outValue);
				Assert.That (outObj, Is.EquivalentTo (new Class [] { new Class (typeof (NSString)), new Class (typeof (NSDate)) }), "NSClassArray-3DA-out-contents");
				Assert.That (refObj, Is.EquivalentTo (new Class [] { new Class (typeof (NSString)), new Class (typeof (NSDate)) }), "NSClassArray-3DA-ref-contents");

				// direct managed
				refValue = dummyArray.Handle; // set to non-null
				outValue = dummyArray.Handle; // set to non-null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 8, ref refValue, out outValue);
				refObj = NSArray.ArrayFromHandle<Class> (refValue);
				outObj = NSArray.ArrayFromHandle<Class> (outValue);
				Assert.That (outObj, Is.EquivalentTo (new Class [] { new Class (typeof (NSSet)), new Class (typeof (NSDictionary)) }), "NSClassArray-3DM-out-contents");
				Assert.That (refObj, Is.EquivalentTo (new Class [] { new Class (typeof (NSSet)), new Class (typeof (NSDictionary)) }), "NSClassArray-3DM-ref-contents");


				/// 4 set both parameteres to different pointers of a NSClassArray
				action = 4;

				// native
				refObj = null; // set to null
				outObj = null; // set to null
				test (action << 0, ref refObj, out outObj);
				Assert.That (refObj, Is.Not.Null, "NSClassArray-4A-ref");
				Assert.That (outObj, Is.Not.Null, "NSClassArray-4A-out");
				Assert.That (refObj, Is.EquivalentTo (new Class [] { new Class (typeof (NSString)), new Class (typeof (NSValue)) }), "NSClassArray-4A-ref-equiv");
				Assert.That (outObj, Is.EquivalentTo (new Class [] { new Class (typeof (NSData)), new Class (typeof (NSDate)) }), "NSClassArray-4A-obj-equiv");

				// managed
				refObj = null; // set to null
				outObj = null; // set to null
				test (action << 8, ref refObj, out outObj);
				Assert.That (refObj, Is.EquivalentTo (new Class [] { new Class (typeof (NSSet)), new Class (typeof (NSMutableSet)) }), "NSClassArray-4M-ref-equiv");
				Assert.That (outObj, Is.EquivalentTo (new Class [] { new Class (typeof (NSDictionary)), new Class (typeof (NSMutableDictionary)) }), "NSClassArray-4M-obj-equiv");

				// direct native
				refValue = IntPtr.Zero; // set to null
				outValue = IntPtr.Zero; // set to null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 0, ref refValue, out outValue);
				refObj = NSArray.ArrayFromHandle<Class> (refValue);
				outObj = NSArray.ArrayFromHandle<Class> (outValue);
				Assert.That (refObj, Is.EquivalentTo (new Class [] { new Class (typeof (NSString)), new Class (typeof (NSValue)) }), "NSClassArray-4DA-ref-equiv");
				Assert.That (outObj, Is.EquivalentTo (new Class [] { new Class (typeof (NSData)), new Class (typeof (NSDate)) }), "NSClassArray-4DA-obj-equiv");

				// direct managed
				refValue = IntPtr.Zero; // set to null
				outValue = IntPtr.Zero; // set to null
				Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 8, ref refValue, out outValue);
				refObj = NSArray.ArrayFromHandle<Class> (refValue);
				outObj = NSArray.ArrayFromHandle<Class> (outValue);
				Assert.That (refObj, Is.EquivalentTo (new Class [] { new Class (typeof (NSSet)), new Class (typeof (NSMutableSet)) }), "NSClassArray-4DM-ref-equiv");
				Assert.That (outObj, Is.EquivalentTo (new Class [] { new Class (typeof (NSDictionary)), new Class (typeof (NSMutableDictionary)) }), "NSClassArray-4DM-obj-equiv");

				/// 5 change a value in the ref input
				/// Here we verify that changing an element in the input array is not copied back to the ref argument.
				/// If an element needs to be changed, a new array must be created, and elements copied over, including the modified one.
				action = 5;

				// Only managed calls can be done because we need to use an NSMutableArray, and the binding code creates NSArrays when marshalling to native code.
				Func<Class [], NSMutableArray> fromValues = (arr) => {
					return new NSMutableArray<Class> (arr);
				};

				// managed
				refObj = new Class [] { new Class (typeof (NSObject)), new Class (typeof (NSObject)) };
				test (action << 8, ref refObj, out var _);
				// Here the array changed, because we didn't go through any binding code at all
				Assert.That (refObj, Is.EquivalentTo (new Class [] { new Class (typeof (NSMutableString)), new Class (typeof (NSObject)) }), "NSClassArray-5M-ref-equiv");

				// direct managed
				refObj = new Class [] { new Class (typeof (NSObject)), new Class (typeof (NSObject)) };
				using (var arr = fromValues (refObj)) {
					refValue = arr.Handle;
					Messaging.void_objc_msgSend_int_IntPtr_IntPtr (obj.Handle, sel, action << 8, ref refValue, out var _);
					refObj = NSArray.ArrayFromHandle<Class> (refValue);
					Assert.That (refObj, Is.EquivalentTo (new Class [] { new Class (typeof (NSObject)), new Class (typeof (NSObject)) }), "NSClassArray-5DM-ref-equiv");
				}
			}
		}

		[Test]
		public void MethodEncodings ()
		{
			using (var met = new MethodEncodingsTests ()) {
				IntPtr obj1 = IntPtr.Zero, obj2 = IntPtr.Zero, obj3 = IntPtr.Zero, obj4 = IntPtr.Zero, obj5 = IntPtr.Zero, obj6 = IntPtr.Zero, obj7 = IntPtr.Zero;
				Messaging.void_objc_msgSend_IntPtr_IntPtr_IntPtr_IntPtr_IntPtr_IntPtr_IntPtr (met.Handle, Selector.GetHandle ("methodEncodings:obj2:obj3:obj4:obj5:obj6:obj7:"), ref obj1, ref obj2, ref obj3, ref obj4, ref obj5, ref obj6, ref obj7);
				NSObject o1 = Runtime.GetNSObject (obj1);
				NSObject o2 = Runtime.GetNSObject (obj2);
				NSObject o3 = Runtime.GetNSObject (obj3);
				NSObject o4 = Runtime.GetNSObject (obj4);
				NSObject o5 = Runtime.GetNSObject (obj5);
				NSObject o6 = Runtime.GetNSObject (obj6);
				NSObject o7 = Runtime.GetNSObject (obj7);
				Assert.That (o1, Is.Not.Null, "O1");
				Assert.That (o2, Is.Not.Null, "O2");
				Assert.That (o3, Is.Not.Null, "O3");
				Assert.That (o4, Is.Not.Null, "O4");
				Assert.That (o5, Is.Not.Null, "O5");
				Assert.That (o6, Is.Not.Null, "O6");
				Assert.That (o7, Is.Not.Null, "O7");
			}
		}

		[Test]
		public void MethodEncodings2 ()
		{
			using (var met = new MethodEncodingsTests ()) {
				nint obj1 = 1;
				nint obj2 = 2;
				nint obj3 = 3;
				nint obj4 = 4;
				CGRect obj5 = new CGRect (1, 2, 3, 4);
				nint obj6 = 6;
				Messaging.void_objc_msgSend_IntPtr_IntPtr_IntPtr_IntPtr_CGRect_IntPtr (met.Handle, Selector.GetHandle ("setPtrPropertyCGRect:p2:p3:p4:p5:p6:"), obj1, obj2, obj3, obj4, ref obj5, obj6);
				Assert.That (obj5, Is.EqualTo (new CGRect (5, 6, 7, 8)), "rv");
			}
		}

		class MethodEncodingsTests : NSObject, IObjCProtocolTest {
			[Export ("methodEncodings:obj2:obj3:obj4:obj5:obj6:obj7:")]
			public void GetMethodEncodings (ref NSObject obj1, ref NSObject obj2, ref NSObject obj3, ref NSObject obj4, ref NSObject obj5, ref NSObject obj6, ref NSObject obj7)
			{
				Assert.That (obj1, Is.Null, "obj1");
				Assert.That (obj2, Is.Null, "obj2");
				Assert.That (obj3, Is.Null, "obj3");
				Assert.That (obj4, Is.Null, "obj4");
				Assert.That (obj5, Is.Null, "obj5");
				Assert.That (obj6, Is.Null, "obj6");
				Assert.That (obj7, Is.Null, "obj7");
				obj1 = new NSObject ();
				obj2 = new NSObject ();
				obj3 = new NSObject ();
				obj4 = new NSObject ();
				obj5 = new NSObject ();
				obj6 = new NSObject ();
				obj7 = new NSObject ();
			}

			[Export ("setPtrPropertyCGRect:p2:p3:p4:p5:p6:")]
			void SetPtrPropertyCGRect (nint p1, nint p2, nint p3, nint p4, ref global::CoreGraphics.CGRect p5, nint p6)
			{
				Assert.That (p1, Is.EqualTo ((nint) 1), "1");
				Assert.That (p2, Is.EqualTo ((nint) 2), "2");
				Assert.That (p3, Is.EqualTo ((nint) 3), "3");
				Assert.That (p4, Is.EqualTo ((nint) 4), "4");
				Assert.That (p5, Is.EqualTo (new CGRect (1, 2, 3, 4)), "5a");
				Assert.That (p6, Is.EqualTo ((nint) 6), "6");

				p5 = new CGRect (5, 6, 7, 8);
			}
		}

		class RefOutParametersSubclass : BI1064.RefOutParameters {
			public override void TestCFBundle (int action, ref CFBundle refValue, out CFBundle outValue)
			{
				var managedAction = (action & 0xFF00) >> 8;
				switch (managedAction) {
				case 0: // call native
					base.TestCFBundle (action, ref refValue, out outValue);
					break;
				case 1: // set both to null
					refValue = null;
					outValue = null;
					break;
				case 2: // verify that refValue points to something
					Assert.That (refValue, Is.Not.Null, "2");
					outValue = null; // compiler-enforced
					break;
				case 3: // set both parameteres to the same pointer of a CFBundle
					var obj = global::CoreFoundation.CFBundle.GetMain ();
					refValue = obj;
					outValue = obj;
					break;
				case 4: // set both parameteres to different pointers of a CFBundle
					refValue = global::CoreFoundation.CFBundle.GetAll () [0];
					outValue = global::CoreFoundation.CFBundle.GetAll () [1];
					break;
				default:
					throw new NotImplementedException ();
				}
			}

			public override void TestINSCoding (int action, ref INSCoding refValue, out INSCoding outValue)
			{
				var managedAction = (action & 0xFF00) >> 8;
				switch (managedAction) {
				case 0: // call native
					base.TestINSCoding (action, ref refValue, out outValue);
					break;
				case 1: // set both to null
					refValue = null;
					outValue = null;
					break;
				case 2: // verify that refValue points to something
					Assert.That (refValue, Is.Not.Null, "2");
					outValue = null; // compiler-enforced
					break;
				case 3: // set both parameteres to the same pointer of an NSString
					var obj = new NSString ("A managed string");
					refValue = obj;
					outValue = obj;
					break;
				case 4: // set both parameteres to different pointers of an NSString
					refValue = new NSString ("A managed ref string");
					outValue = new NSString ("A managed out string");
					break;
				default:
					throw new NotImplementedException ();
				}
			}

			public override void TestNSObject (int action, ref NSObject refValue, out NSObject outValue)
			{
				var managedAction = (action & 0xFF00) >> 8;
				switch (managedAction) {
				case 0: // call native
					base.TestNSObject (action, ref refValue, out outValue);
					break;
				case 1: // set both to null
					refValue = null;
					outValue = null;
					break;
				case 2: // verify that refValue points to something
					Assert.That (refValue, Is.Not.Null, "2");
					outValue = null; // compiler-enforced
					break;
				case 3: // set both parameteres to the same pointer of an NSObject
					var obj = new NSObject ();
					refValue = obj;
					outValue = obj;
					break;
				case 4: // set both parameteres to different objects
					refValue = new NSObject ();
					outValue = new NSObject ();
					break;
				default:
					throw new NotImplementedException ();
				}
			}

			public override void TestValue (int action, ref NSValue refValue, out NSValue outValue)
			{
				var managedAction = (action & 0xFF00) >> 8;
				switch (managedAction) {
				case 0: // call native
					base.TestValue (action, ref refValue, out outValue);
					break;
				case 1: // set both to null
					refValue = null;
					outValue = null;
					break;
				case 2: // verify that refValue points to something
					Assert.That (refValue, Is.Not.Null, "2");
					outValue = null; // compiler-enforced
					break;
				case 3: // set both parameteres to the same pointer of an NSObject
					var obj = NSValue.FromMKCoordinate (new CLLocationCoordinate2D (3, 14));
					refValue = obj;
					outValue = obj;
					break;
				case 4: // set both parameteres to different objects
					refValue = NSValue.FromMKCoordinate (new CLLocationCoordinate2D (3, 14));
					outValue = NSValue.FromMKCoordinate (new CLLocationCoordinate2D (2, 71));
					break;
				default:
					throw new NotImplementedException ();
				}
			}

			public override void TestString (int action, ref string refValue, out string outValue)
			{
				var managedAction = (action & 0xFF00) >> 8;
				switch (managedAction) {
				case 0: // call native
					base.TestString (action, ref refValue, out outValue);
					break;
				case 1: // set both to null
					refValue = null;
					outValue = null;
					break;
				case 2: // verify that refValue points to something
					Assert.That (refValue, Is.Not.Null, "2");
					outValue = null; // compiler-enforced
					break;
				case 3: // set both parameteres to the same pointer of an NSObject
					var obj = "A constant managed string";
					refValue = obj;
					outValue = obj;
					break;
				case 4: // set both parameteres to different objects
					refValue = "Hello Xamarin from managed";
					outValue = "Hello Microsoft from managed";
					break;
				default:
					throw new NotImplementedException ();
				}
			}

			public unsafe override void TestInt (int action, ref int refValue, out int outValue, int* ptrValue)
			{
				var managedAction = (action & 0xFF00) >> 8;
				switch (managedAction) {
				case 0: // call native
					base.TestInt (action, ref refValue, out outValue, ptrValue);
					break;
				case 1: // set both to null
					refValue = 0;
					outValue = 0;
					*ptrValue = 0;
					break;
				case 3: // set both parameteres to the same value
					refValue = 314159;
					outValue = 314159;
					*ptrValue = 314159;
					break;
				case 4: // set both parameteres to different values
					refValue = 3141592;
					outValue = 2718282;
					*ptrValue = 5772156;
					break;
				default:
					throw new NotImplementedException ();
				}
			}

			public override void TestSelector (int action, ref Selector refValue, out Selector outValue)
			{
				var managedAction = (action & 0xFF00) >> 8;
				switch (managedAction) {
				case 0: // call native
					base.TestSelector (action, ref refValue, out outValue);
					break;
				case 1: // set both to null
					refValue = null;
					outValue = null;
					break;
				case 2: // verify that refValue points to something
					Assert.That (refValue, Is.Not.Null, "TestSelector: 2");
					outValue = null;
					break;
				case 3: // set both parameteres to the same value
					var obj = new Selector ("testManagedSelector");
					refValue = obj;
					outValue = obj;
					break;
				case 4: // set both parameteres to different values
					refValue = new Selector ("testManagedSelectorA");
					outValue = new Selector ("testManagedSelectorB");
					break;
				default:
					throw new NotImplementedException ();
				}
			}

			public override void TestClass (int action, ref Class refValue, out Class outValue)
			{
				var managedAction = (action & 0xFF00) >> 8;
				switch (managedAction) {
				case 0: // call native
					base.TestClass (action, ref refValue, out outValue);
					break;
				case 1: // set both to null
					refValue = null;
					outValue = null;
					break;
				case 2: // verify that refValue points to something
					Assert.That (refValue, Is.Not.Null);
					outValue = null;
					break;
				case 3: // set both parameteres to the same value
					var obj = new Class (typeof (SomeConsumer));
					refValue = obj;
					outValue = obj;
					break;
				case 4: // set both parameteres to different values
					refValue = new Class (typeof (RefOutParametersSubclass));
					outValue = new Class (typeof (BI1064.RefOutParameters));
					break;
				default:
					throw new NotImplementedException ();
				}
			}

			public override void TestINSCodingArray (int action, ref INSCoding [] refValue, out INSCoding [] outValue)
			{
				var managedAction = (action & 0xFF00) >> 8;
				switch (managedAction) {
				case 0: // call native
					base.TestINSCodingArray (action, ref refValue, out outValue);
					break;
				case 1: // set both to null
					refValue = null;
					outValue = null;
					break;
				case 2: // verify that refValue points to something
					Assert.That (refValue, Is.Not.Null, "2");
					outValue = null; // compiler-enforced
					break;
				case 3: // set both parameteres to the same pointer of an NSObject
					var obj = new INSCoding [] { (NSString) "A constant managed string" };
					refValue = obj;
					outValue = obj;
					break;
				case 4: // set both parameteres to different objects
					refValue = new INSCoding [] { (NSString) "Hello Xamarin from managed" };
					outValue = new INSCoding [] { (NSString) "Hello Microsoft from managed" };
					break;
				default:
					throw new NotImplementedException ();
				}
			}

			public override void TestNSObjectArray (int action, ref NSObject [] refValue, out NSObject [] outValue)
			{
				var managedAction = (action & 0xFF00) >> 8;
				switch (managedAction) {
				case 0: // call native
					base.TestNSObjectArray (action, ref refValue, out outValue);
					break;
				case 1: // set both to null
					refValue = null;
					outValue = null;
					break;
				case 2: // verify that refValue points to something
					Assert.That (refValue, Is.Not.Null, "2");
					outValue = null; // compiler-enforced
					break;
				case 3: // set both parameteres to the same pointer of an NSObject
					var obj = new NSObject [] { (NSString) "Hello", (NSString) "World", (NSString) "from", (NSString) "managed" };
					refValue = obj;
					outValue = obj;
					break;
				case 4: // set both parameteres to different objects
					refValue = new NSObject [] { NSNumber.FromInt32 (2), NSNumber.FromInt32 (71) };
					outValue = new NSObject [] { (NSString) "Hello", (NSString) "Microsoft", (NSString) "from", (NSString) "managed" };
					break;
				default:
					throw new NotImplementedException ();
				}
			}

			public override void TestNSValueArray (int action, ref NSValue [] refValue, out NSValue [] outValue)
			{
				var managedAction = (action & 0xFF00) >> 8;
				switch (managedAction) {
				case 0: // call native
					base.TestNSValueArray (action, ref refValue, out outValue);
					break;
				case 1: // set both to null
					refValue = null;
					outValue = null;
					break;
				case 2: // verify that refValue points to something
					Assert.That (refValue, Is.Not.Null, "2");
					outValue = null; // compiler-enforced
					break;
				case 3: // set both parameteres to the same pointer of an NSObject
					var obj = new NSValue [] { NSValue.FromMKCoordinate (new CLLocationCoordinate2D (3, 14)), NSValue.FromMKCoordinate (new CLLocationCoordinate2D (2, 71)) };
					refValue = obj;
					outValue = obj;
					break;
				case 4: // set both parameteres to different objects
					refValue = new NSValue [] { NSValue.FromMKCoordinate (new CLLocationCoordinate2D (3, 14)), NSValue.FromMKCoordinate (new CLLocationCoordinate2D (15, 92)) };
					outValue = new NSValue [] { NSValue.FromMKCoordinate (new CLLocationCoordinate2D (2, 71)), NSValue.FromMKCoordinate (new CLLocationCoordinate2D (82, 82)) };
					break;
				default:
					throw new NotImplementedException ();
				}
			}

			public override void TestStringArray (int action, ref string [] refValue, out string [] outValue)
			{
				var managedAction = (action & 0xFF00) >> 8;
				switch (managedAction) {
				case 0: // call native
					base.TestStringArray (action, ref refValue, out outValue);
					break;
				case 1: // set both to null
					refValue = null;
					outValue = null;
					break;
				case 2: // verify that refValue points to something
					Assert.That (refValue, Is.Not.Null, "2");
					outValue = null; // compiler-enforced
					break;
				case 3: // set both parameteres to the same pointer of an NSObject
					var obj = new string [] { "Hello", "Managed", "World" };
					refValue = obj;
					outValue = obj;
					break;
				case 4: // set both parameteres to different objects
					refValue = new string [] { "Hello", "Microsoft", "from", "managed" };
					outValue = new string [] { "Hello", "Xamarin", "from", "managed" };
					break;
				case 5: // change a value in the ref input
					refValue [1] = "Universe";
					outValue = null;
					break;
				default:
					throw new NotImplementedException ();
				}
			}

			public override void TestClassArray (int action, ref Class [] refValue, out Class [] outValue)
			{
				var managedAction = (action & 0xFF00) >> 8;
				switch (managedAction) {
				case 0: // call native
					base.TestClassArray (action, ref refValue, out outValue);
					break;
				case 1: // set both to null
					refValue = null;
					outValue = null;
					break;
				case 2: // verify that refValue points to something
					Assert.That (refValue, Is.Not.Null, "2");
					outValue = null; // compiler-enforced
					break;
				case 3: // set both parameteres to the same pointer of an NSObject
					var obj = new Class [] { new Class (typeof (NSSet)), new Class (typeof (NSDictionary)) };
					refValue = obj;
					outValue = obj;
					break;
				case 4: // set both parameteres to different objects
					refValue = new Class [] { new Class (typeof (NSSet)), new Class (typeof (NSMutableSet)) };
					outValue = new Class [] { new Class (typeof (NSDictionary)), new Class (typeof (NSMutableDictionary)) };
					break;
				case 5: // change a value in the ref input
					refValue [1] = new Class (typeof (NSMutableString));
					outValue = null;
					break;
				default:
					throw new NotImplementedException ();
				}
			}

		}

#if !__TVOS__ // No WebKit on tvOS
		[Test]
		public void GenericClassWithUnrelatedGenericDelegate ()
		{
			using (var obj = new GenericWebNavigationThingie<NSObject> ()) {
				var handler_called = false;
				Action<WKNavigationActionPolicy> handler = new Action<WKNavigationActionPolicy> ((v) => {
					handler_called = true;
				});
				var block = new BlockLiteral ();
				var tramp = new DActionArity1V3 (SDActionArity1V3.Invoke);
				if (Runtime.DynamicRegistrationSupported) {
					block.SetupBlock (tramp, handler);
					Messaging.void_objc_msgSend_IntPtr_IntPtr_BlockLiteral (obj.Handle, Selector.GetHandle ("webView:decidePolicyForNavigationAction:decisionHandler:"), IntPtr.Zero, IntPtr.Zero, ref block);
					block.CleanupBlock ();
					Assert.That (handler_called, Is.True, "Handler called");
				} else {
					Assert.Throws<RuntimeException> (() => block.SetupBlock (tramp, handler));
				}
			}
		}

		[UnmanagedFunctionPointerAttribute (CallingConvention.Cdecl)]
		internal delegate void DActionArity1V3 (IntPtr block, nint value);
		static internal class SDActionArity1V3 {
			static internal readonly DActionArity1V3 Handler = Invoke;

			[MonoPInvokeCallback (typeof (DActionArity1V3))]
			public static unsafe void Invoke (IntPtr block, nint value)
			{
				var del = BlockLiteral.GetTarget<Action<WKNavigationActionPolicy>> (block);
				if (del is not null)
					del ((WKNavigationActionPolicy) (long) value);
			}
		}
#endif // !__TVOS__

		[Test]
		public void RefEnumValues ()
		{
			EnumB b = 0;
			EnumSB sb = 0;
			EnumS s = 0;
			EnumUS us = 0;
			EnumI i = 0;
			EnumUI ui = 0;
			EnumL l = 0;
			EnumUL ul = 0;

			using (var obj = new UnderlyingEnumValues ()) {
				b = 0; sb = 0; s = 0; us = 0; i = 0; ui = 0; l = 0; ul = 0;
				Messaging.void_objc_msgSend_ref_byte_ref_sbyte_ref_short_ref_ushort_ref_int_ref_uint_ref_long_ref_ulong (obj.Handle, Selector.GetHandle ("ByRef:a:b:c:d:e:f:g:"), ref b, ref sb, ref s, ref us, ref i, ref ui, ref l, ref ul);
				Assert.That (b, Is.EqualTo (EnumB.b), "ref: B");
				Assert.That (sb, Is.EqualTo (EnumSB.b), "ref: SB");
				Assert.That (s, Is.EqualTo (EnumS.b), "ref: S");
				Assert.That (us, Is.EqualTo (EnumUS.b), "ref: US");
				Assert.That (i, Is.EqualTo (EnumI.b), "ref: I");
				Assert.That (ui, Is.EqualTo (EnumUI.b), "ref: UI");
				Assert.That (l, Is.EqualTo (EnumL.b), "ref: L");
				Assert.That (ul, Is.EqualTo (EnumUL.b), "ref: UL");

				b = 0; sb = 0; s = 0; us = 0; i = 0; ui = 0; l = 0; ul = 0;
				Messaging.void_objc_msgSend_out_byte_out_sbyte_out_short_out_ushort_out_int_out_uint_out_long_out_ulong (obj.Handle, Selector.GetHandle ("Out:a:b:c:d:e:f:g:"), out b, out sb, out s, out us, out i, out ui, out l, out ul);
				Assert.That (b, Is.EqualTo (EnumB.b), "out: B");
				Assert.That (sb, Is.EqualTo (EnumSB.b), "out: SB");
				Assert.That (s, Is.EqualTo (EnumS.b), "out: S");
				Assert.That (us, Is.EqualTo (EnumUS.b), "out: US");
				Assert.That (i, Is.EqualTo (EnumI.b), "out: I");
				Assert.That (ui, Is.EqualTo (EnumUI.b), "out: UI");
				Assert.That (l, Is.EqualTo (EnumL.b), "out: L");
				Assert.That (ul, Is.EqualTo (EnumUL.b), "out: UL");
			}
		}

#if HAS_UIKIT
		[Test]
		public void ProtocolsTrimmedAway ()
		{
			PreserveIUIApplicationDelegate (null);

			// A little indirection to try to make the trimmer not be helpful and preserve all the methods on IUIApplicationDelegate.
			AssertMemberCount (typeof (IUIApplicationDelegate));
		}

		[UnconditionalSuppressMessage ("Trimming", "IL2070", Justification = "This test verifies that all members have been correctly preserved, and fails if that's not the case. It'll thus fail if the trimmer removed anything it didn't expect - and is then technically trimmer safe in that it's aware and react accordingly if there's any behavioral change when the trimmer is enabled.")]
		void AssertMemberCount (Type type)
		{
			var members = type.GetMembers (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
#if NATIVEAOT && NET11_0_OR_GREATER
			// ILLink isn't executed when using NativeAOT on .NET 11+ (ILC does all the trimming), and
			// monotouch-test tells ILC to keep complete reflection metadata (NUnit requires
			// $(IlcGenerateCompleteTypeMetadata)=true and $(IlcTrimMetadata)=false), so even though ILC
			// trims away the code, the reflection metadata for the members is still there.
			var expectNoMembers = false;
#elif OPTIMIZEALL || NATIVEAOT
			var expectNoMembers = true;
#elif !__MACOS__
			var expectNoMembers = global::XamarinTests.ObjCRuntime.Registrar.IsStaticRegistrar && TestRuntime.IsLinkAny;
#else
			var expectNoMembers = false;
#endif
			if (expectNoMembers) {
				Assert.That (members.Length, Is.EqualTo (0), $"All members should be trimmed away in {type.FullName}:\n\t{string.Join ("\n\t", members.Select (v => v.ToString ()))}");
			} else {
				Assert.That (members.Length, Is.Not.EqualTo (0), $"All members should not be trimmed away in {type.FullName}");
			}
		}

		void PreserveIUIApplicationDelegate (IUIApplicationDelegate obj)
		{
			GC.KeepAlive (obj);
		}
#endif // HAS_UIKIT

#if HAS_UIKIT
		// This is a test case for https://github.com/dotnet/macios/issues/23070.
		// It only needs to exist in the source code, it doesn't have to do anything.
		private void Issue23070TestCase (UIView origin, UIView target)
		{
			origin.LeftAnchor.ConstraintEqualTo (target.LeftAnchor);
		}
#endif // HAS_UIKIT
	}

	[Category (typeof (CALayer))]
	static class CALayerColorsHelpers {
		[Export ("setBorderUIColor:")]
		static void BorderUIColor (this CALayer self, UIColor borderColor)
		{
			self.BorderColor = borderColor.CGColor;
		}
	}


	[TestFixture]
	[Preserve (AllMembers = true)]
	public class BlockSignatureTest {
		[StructLayout (LayoutKind.Sequential)]
		struct BlockDescriptor2 {
			public IntPtr reserved;
			public IntPtr size;
			public IntPtr copy_helper;
			public IntPtr dispose;
			public IntPtr signature;
		}

		[StructLayout (LayoutKind.Sequential)]
		struct BlockLiteral2 {
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
		static internal class SDActionArity1V1 {
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
		static internal class SDActionArity1V2 {
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
				Assert.That (GetBlockSignature (block), Is.EqualTo ("v@:^v^v"), "a");
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
				Assert.That (GetBlockSignature (block), Is.EqualTo ("v@?@"), "a");
				block.CleanupBlock ();
			} else {
				// The linker is able to rewrite calls to BlockLiteral.SetupBlock to BlockLiteral.SetupBlockImpl (which works without the dynamic registrar),
				// but that will only happen if the code is linked, and monotouch-test is only SdkLinked. Thus this code will throw an exception
				Assert.Throws<RuntimeException> (() => block.SetupBlock (tramp, del));
			}
		}
	}

	[Preserve]
	class OverloadByStaticity : NSObject {
		// Two Objective-C methods can have the same selector if one is static and the other instance.
		[Export ("method")]
		public void InstanceMethod () { }

		[Export ("method")]
		public static void StaticMethod () { }
	}

	// It should be possible to use a protocol with a member we can't use yet (because its signature uses a type not in the current SDK)
	[Protocol]
	[ProtocolMember (IsRequired = false, IsProperty = false, IsStatic = false, Name = "DetectPremonition", Selector = "detectPremonition:", ParameterType = new Type [] { typeof (FutureClass) }, ParameterByRef = new bool [] { false })]
	public interface ISomeDelegate : INativeObject, IDisposable {
	}

	[SupportedOSPlatform ("macos100.0")]
	[SupportedOSPlatform ("ios100.0")]
	[SupportedOSPlatform ("tvos100.0")]
	[SupportedOSPlatform ("maccatalyst100.0")]
	public class FutureClass : NSObject {
	}
	[Preserve]
	public class SomeConsumer : NSObject, ISomeDelegate {
	}

	public delegate void ACompletionHandler (string strArg, NSError error);

	// https://github.com/dotnet/macios/issues/7733
	[Preserve]
	public class GHIssue7733 : NSObject {
		[Export ("doSomeWork:completion:")]
		public virtual void DoWork (string who, ACompletionHandler completion)
		{

		}
	}

#if !__TVOS__ // No WebKit on tvOS
	[Preserve]
	public class GenericWebNavigationThingie<WebViewModel> : NSObject, IWKNavigationDelegate {
		[Export ("webView:decidePolicyForNavigationAction:decisionHandler:")]
		public void DecidePolicy (WKWebView webView, WKNavigationAction navigationAction, Action<WKNavigationActionPolicy> decisionHandler)
		{
			decisionHandler (WKNavigationActionPolicy.Allow);
		}
	}
#endif

	// These classes implement Metal* protocols, so that the generated registrar code includes the corresponding Metal* headers.
	// https://github.com/dotnet/macios/issues/4422
	class MetalKitTypesInTheSimulator : NSObject, global::MetalKit.IMTKViewDelegate {
		public void Draw (global::MetalKit.MTKView view)
		{
			throw new NotImplementedException ();
		}

		public void DrawableSizeWillChange (global::MetalKit.MTKView view, CGSize size)
		{
			throw new NotImplementedException ();
		}
	}
	class MetalTypesInTheSimulator : NSObject, global::Metal.IMTLDrawable {
		public void Present ()
		{
			throw new NotImplementedException ();
		}

		public void Present (double presentationTime)
		{
			throw new NotImplementedException ();
		}

		public void PresentAfter (double presentationTime)
		{
			throw new NotImplementedException ();
		}

		public void AddPresentedHandler (Action<global::Metal.IMTLDrawable> block)
		{
			throw new NotImplementedException ();
		}

		public double PresentedTime { get => throw new NotImplementedException (); }

		public nuint DrawableId { get => throw new NotImplementedException (); }
	}
#if !__TVOS__ // MetalPerformanceShaders isn't available in the tvOS simulator either
	class MetalPerformanceShadersTypesInTheSimulator : NSObject, global::MetalPerformanceShaders.IMPSDeviceProvider {
		public global::Metal.IMTLDevice GetMTLDevice ()
		{
			throw new NotImplementedException ();
		}

		public void Present (double presentationTime)
		{
			throw new NotImplementedException ();
		}
	}
#endif // !__TVOS__

#if HAS_COREMIDI && !__TVOS__
	// This type exports methods with 'MidiThruConnectionEndpoint' parameters, which is a struct with different casing in Objective-C ("MIDI...")
	class ExportedMethodWithStructWithManagedCasing : NSObject {
		[Export ("doSomething:")]
		public void DoSomething (MidiThruConnectionEndpoint arg) { }

		[Export ("doSomething2:")]
		public void DoSomething2 (ref MidiThruConnectionEndpoint arg) { }

		[Export ("doSomething3")]
		public MidiThruConnectionEndpoint DoSomething3 () { return default (MidiThruConnectionEndpoint); }

		[Export ("doSomething4:")]
		public void DoSomething4 (out MidiThruConnectionEndpoint arg) { arg = default (MidiThruConnectionEndpoint); }
	}
#endif
}
