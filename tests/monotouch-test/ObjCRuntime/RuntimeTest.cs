using System;
using System.Diagnostics;
#if !__WATCHOS__
using System.Drawing;
#endif
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

using CoreGraphics;
using Foundation;
using ObjCRuntime;
#if !__WATCHOS__
using SpriteKit;
#endif
#if !MONOMAC
using UIKit;
#endif
using NUnit.Framework;

using MonoTests.System.Net.Http;
using Xamarin.Utils;

namespace MonoTouchFixtures.ObjCRuntime {

	static class AssociatedObjects {
		public enum AssociationPolicy { // uintptr_t
			Assign = 0,
			RetainNonAtomic = 1,
			CopyNonAtomic = 3,
			Retain = 0x301,
			Copy = 0x303,
		}

		[DllImport (Messaging.LIBOBJC_DYLIB)]
		static extern void objc_setAssociatedObject (IntPtr obj, IntPtr key, IntPtr value, IntPtr policy);

		public static void SetAssociatedObject (this NSObject self, IntPtr key, NSObject value, AssociationPolicy policy)
		{
			objc_setAssociatedObject (self.Handle, key, value.Handle, new IntPtr ((int) policy));
		}

		public static IntPtr GetAssociatedPointer (this NSObject self, IntPtr key)
		{
			throw new NotImplementedException ();
		}

		public static void RemoveAssociatedObjects (this NSObject self)
		{
			throw new NotImplementedException ();
		}
	}

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class RuntimeTest {
		static bool connectMethodTestDone;
		[Test]
		public void ConnectMethodTest ()
		{
			if (connectMethodTestDone)
				Assert.Ignore ("This test can only be executed once, it modifies global state.");
			if (!Runtime.DynamicRegistrationSupported)
				Assert.Ignore ("This test requires support for dynamic registration.");
			connectMethodTestDone = true;
			// Bug 20013. This should not throw a KeyNotFoundException.
			Runtime.ConnectMethod (typeof (ConnectMethodClass).GetMethod ("Method"), new Selector ("method"));
		}

		class ConnectMethodClass : NSObject {
			public void Method () { }
		}

		[Test]
		public void GetNSObject_IntPtrZero ()
		{
			Assert.Null (Runtime.GetNSObject (IntPtr.Zero));
		}

		[Test]
		public void RegisterAssembly_null ()
		{
			Assert.Throws<ArgumentNullException> (() => Runtime.RegisterAssembly (null));
		}

#if __IOS__ && !__MACCATALYST__
		[Test]
		public void StartWWAN ()
		{
			Assert.Throws<ArgumentNullException> (delegate { Runtime.StartWWAN (null); }, "null");
			Assert.Throws<ArgumentException> (delegate { Runtime.StartWWAN (new Uri ("ftp://www.xamarin.com")); }, "ftp");
			Runtime.StartWWAN (new Uri ("http://www.xamarin.com"));
		}
#endif // __IOS__ && !__MACCATALYST__

		[Test]
		public void GetNSObject_Subclass ()
		{
			using (var c = new NSHttpCookie ("name", "value")) {
				// we want to ensure we get the NSMutableDictionary even if we ask for (the base) NSDictionary
				var d = Runtime.GetNSObject<NSDictionary> (Messaging.IntPtr_objc_msgSend (c.Handle, Selector.GetHandle ("properties")));
				Assert.That (d, Is.TypeOf<NSMutableDictionary> (), "NSMutableDictionary");
			}
		}

#if !__WATCHOS__
		[Test]
		public void GetNSObject_Different_Class ()
		{
			TestRuntime.AssertXcodeVersion (5, 0);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 9, throwIfOtherPlatform: false);

			IntPtr class_ptr = Class.GetHandle ("SKPhysicsBody");
			var size = new CGSize (3, 2);
			using (var body = Runtime.GetNSObject<SKPhysicsBody> (Messaging.IntPtr_objc_msgSend_CGSize (class_ptr, Selector.GetHandle ("bodyWithRectangleOfSize:"), size))) {
				// This would normally return a PKPhysicsBody which is not a subclass but answers the same selectors
				// as a SKPhysicsBody. That's an issue since we can't register PKPhysicsBody (Apple won't like it since
				// it's a private type) and the non-generic version of GetNSObject (and bindings) would throw an 
				// InvalidaCastException (since it's Class will resolve to NSObject)
				// note: that's the internal PhysicKit shared by UIKit and SpriteKit
				Assert.That (body, Is.TypeOf<SKPhysicsBody> (), "SKPhysicsBody");
			}
		}
#endif // !__WATCHOS__

		[Test]
		public void GetNSObject_Posing_Class ()
		{
			TestRuntime.AssertXcodeVersion (5, 0);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 9, throwIfOtherPlatform: false);

			NSUrlSession session = NSUrlSession.SharedSession;
			using (var request = new NSUrlRequest (new NSUrl (NetworkResources.MicrosoftUrl))) {
				// In iOS 8 the native CreateDownloadTask function returns an instance of a
				// __NSCFLocalDownloadTask, which does not derive from 
				// NSUrlSessionDownloadTask (which is the documented/expected return type from
				// CreateDownloadTask). But __NSCFLocalDownloadTask does override
				// isKindOfClass: to return true if asked about NSUrlSessionDownloadTask.
				// In other words it's posing as a NSUrlSessionDownloadTask.
				using (var o = session.CreateDownloadTask (request)) {
				}
			}
		}

		[Test]
		public void GetNSObject_T_SameHandleDifferentInstances ()
		{
			using var a = new NSDictionary<NSString, NSObject> ();
			using var b = new NSDictionary<NSString, NSString> ();
			using var c = new NSDictionary ();

			if (a.Handle != b.Handle || a.Handle != c.Handle)
				Assert.Ignore ("The dictionaries are actually different");

			var handle = a.Handle;

			Assert.DoesNotThrow (() => Runtime.GetNSObject<NSDictionary> (handle), "A");
			Assert.DoesNotThrow (() => Runtime.GetNSObject<NSDictionary<NSString, NSObject>> (handle), "B");
			Assert.DoesNotThrow (() => Runtime.GetNSObject<NSDictionary<NSString, NSString>> (handle), "C");
		}

		[Test]
		public void UsableUntilDead ()
		{
			// The test can be inconclusive once in a while.
			// 100 times in a row is a bit too much though.
			for (int i = 0; i < 100; i++) {
				if (UsableUntilDeadImpl ())
					return;
			}

			Assert.Inconclusive ("Failed to collect the notification object at least once in 100 runs.");
		}

		public bool UsableUntilDeadImpl ()
		{
			// This test ensure that the main thread can send messages to a garbage collected object,
			// until the final 'release' message for the managed reference has been sent
			// (on the main thread).

			var notifierHandle = IntPtr.Zero;

			//			bool isDeallocated = false;
			Action deallocated = () => {
				//Console.WriteLine ("Final release!");
				//				isDeallocated = true;
			};

			ManualResetEvent isCollected = new ManualResetEvent (false);
			Action collected = () => {
				//Console.WriteLine ("Garbage collected!");
				isCollected.Set ();
			};

			bool isNotified = false;
			Action notified = () => {
				//Console.WriteLine ("Notified");
				isNotified = true;
			};

			// Create an object whose handle we store in a local variable. We do not
			// store the object itself, since we want the object to be garbage collected.
			var t = new Thread (() => {
				var obj = new Notifier (collected, notified);
				ReleaseNotifier.NotifyWhenDeallocated (obj, deallocated);
				notifierHandle = obj.Handle;
			}) {
				IsBackground = true,
			};
			t.Start ();
			t.Join ();

			// Now we have a handle to an object that may be garbage collected at any time.
			int counter = 0;
			do {
				GC.Collect ();
				GC.WaitForPendingFinalizers ();
			} while (counter++ < 10 && !isCollected.WaitOne (10));

			// Now we have a handle to a garbage collected object.
			if (!isCollected.WaitOne (0)) {
				// Objects may randomly not end up collected (at least in Boehm), because
				// other objects may happen to contain a random value pointing to the
				// object we're waiting to become freed.
				return false;
			}

			// Send a message to the collected object.
			Messaging.void_objc_msgSend (notifierHandle, Selector.GetHandle ("notify"));
			Assert.IsTrue (isNotified, "notified");

			// We're done. Cleanup.
			NSRunLoop.Main.RunUntil (NSDate.Now.AddSeconds (0.1));
			// Don't verify cleanup, it's not consistent.
			// And in any case it's not what this test is about.
			//			Assert.IsTrue (isDeallocated, "released");

			return true;
		}

		class Notifier : NSObject {
			Action collected;
			Action notified;

			public Notifier (Action collected, Action notified)
			{
				this.collected = collected;
				this.notified = notified;
			}

			~Notifier ()
			{
				collected ();
			}

			[Export ("notify")]
			void Notify ()
			{
				Console.WriteLine ("{0} Notified", DateTime.Now.ToString ());
				notified ();
			}
		}

		class Level1 : NSObject { } // we need two levels of subclassing, since the XI will override 'release' on the first one, and we need to override it as well.
		class ReleaseNotifier : Level1 {
			Action deallocated;
			bool enabled;

			ReleaseNotifier (Action deallocated)
			{
				this.deallocated = deallocated;
			}

			[Export ("release")]
			new void Release ()
			{
				if (enabled)
					deallocated ();
				Messaging.objc_super super;
				super.Handle = Handle;
				super.SuperHandle = ClassHandle;
				Messaging.void_objc_msgSendSuper (ref super, Selector.GetHandle ("release"));
			}

			public static void NotifyWhenDeallocated (NSObject obj, Action deallocated)
			{
				// Add an associated object which will be 'released'd when the obj
				// to watch is deallocated. When 'release' is sent, then invoke
				// the 'deallocated' callback.
				var notifier = new ReleaseNotifier (deallocated);
				obj.SetAssociatedObject (IntPtr.Zero, notifier, AssociatedObjects.AssociationPolicy.Retain);
				notifier.Dispose (); // remove any managed references.
				notifier.enabled = true; // notify on the next 'release' message.
			}
		}

		[Test]
		public void FinalizationRaceCondition ()
		{
#if __WATCHOS__
			if (Runtime.Arch == Arch.DEVICE)
				Assert.Ignore ("This test uses too much memory for the watch.");
#endif

			NSDictionary dict = null;

			var thread = new Thread (() => {
				dict = new NSMutableDictionary ();
				dict ["Hello"] = new NSString (@"World");
				dict ["Bye"] = new NSString (@"Bye");
			}) {
				IsBackground = true
			};
			thread.Start ();
			thread.Join ();

			var getter1 = new Func<string, string> ((key) => dict [key] as NSString);
			var getter2 = new Func<string, NSString> ((key) => dict [key] as NSString);

			var broken = 0;
			var count = 0;

			thread = new Thread (() => {
				var watch = new Stopwatch ();
				watch.Start ();

				while (broken == 0 && watch.ElapsedMilliseconds < 10000) {
					// try getting using Systen.String key
					string hello = getter1 ("Hello");
					if (hello is null)
						broken = 1;

					string bye = getter1 ("Bye");
					if (bye is null)
						broken = 2;

					// try getting using NSString key
					string nHello = getter2 (new NSString (@"Hello"));
					string nBye = getter2 (new NSString (@"Bye"));

					if (nHello is null)
						broken = 3;

					if (nBye is null)
						broken = 4;

					count++;
				}
			}) {
				IsBackground = true,
			};
			thread.Start ();
			while (!thread.Join (1))
				NSRunLoop.Main.RunUntil (NSDate.Now.AddSeconds (0.1));

			Assert.AreEqual (0, broken, string.Format ("broken after {0} iterations", count));
		}

		[Test]
		public void ConnectMethod ()
		{
			if (!Runtime.DynamicRegistrationSupported)
				Assert.Ignore ("This test requires support for dynamic registration.");

			var minfo = typeof (RuntimeTest).GetMethod ("ConnectMethod");
			Assert.Throws<ArgumentNullException> (() => Runtime.ConnectMethod (null, new Selector ("")), "1");
			Assert.Throws<ArgumentNullException> (() => Runtime.ConnectMethod (minfo, null), "2");
			Assert.Throws<ArgumentNullException> (() => Runtime.ConnectMethod (null, minfo, new ExportAttribute ("foo")), "3");
			Assert.Throws<ArgumentNullException> (() => Runtime.ConnectMethod (typeof (RuntimeTest), null, new ExportAttribute ("foo")), "4");
			Assert.Throws<ArgumentNullException> (() => Runtime.ConnectMethod (typeof (RuntimeTest), minfo, (ExportAttribute) null), "5");

			Assert.Throws<ArgumentException> (() => Runtime.ConnectMethod (typeof (RuntimeTest), minfo, new Selector ("foo")), "6");
			Assert.Throws<ArgumentException> (() => Runtime.ConnectMethod (typeof (A), minfo, new Selector ("foo")), "7");
		}

		static bool connectMethod1Done;
		[Test]
		public void ConnectMethod1 ()
		{
			if (!Runtime.DynamicRegistrationSupported)
				Assert.Ignore ("This test requires support for dynamic registration.");

			if (connectMethod1Done)
				Assert.Ignore ("This is a one-shot test. Restart to run again.");
			connectMethod1Done = true;

			Runtime.ConnectMethod (typeof (A).GetMethod ("Test"), new Selector ("test1"));

			using (var a = new A ())
				Messaging.void_objc_msgSend (a.Handle, Selector.GetHandle ("test1"));
		}

		static bool connectMethod2Done;
		[Test]
		public void ConnectMethod2 ()
		{
			if (!Runtime.DynamicRegistrationSupported)
				Assert.Ignore ("This test requires support for dynamic registration.");

			if (connectMethod2Done)
				Assert.Ignore ("This is a one-shot test. Restart to run again.");
			connectMethod2Done = true;

			// the method is not declared on the type we're connecting to,but a completely different type.
			Runtime.ConnectMethod (typeof (A), typeof (RuntimeTest).GetMethod ("Test2"), new Selector ("test2"));

			Messaging.void_objc_msgSend (Class.GetHandle (typeof (A)), Selector.GetHandle ("test2"));
			Assert.IsTrue (calledTest2);
		}

		static bool calledTest2;
		public static void Test2 ()
		{
			calledTest2 = true;
		}


		static bool connectMethod3Done;
		[Test]
		public void ConnectMethod3 ()
		{
			if (!Runtime.DynamicRegistrationSupported)
				Assert.Ignore ("This test requires support for dynamic registration.");

			if (connectMethod3Done)
				Assert.Ignore ("This is a one-shot test. Restart to run again.");
			connectMethod3Done = true;

			Runtime.ConnectMethod (typeof (NSString), typeof (RuntimeTest).GetMethod ("Test3"), new Selector ("test3"));

			Messaging.void_objc_msgSend (Class.GetHandle (typeof (NSString)), Selector.GetHandle ("test3"));
			Assert.IsTrue (calledTest3);
		}

		static bool calledTest3;
		public static void Test3 ()
		{
			calledTest3 = true;
		}

		class A : NSObject {
			public void Test ()
			{
				Console.WriteLine ("Tested!");
			}
		}

		[Test]
		public void GetINativeObjectTest ()
		{
			// create a string and try to get it.
			IntPtr strptr = IntPtr.Zero;
			IntPtr valueptr;
			NSString obj;
			try {
				strptr = Marshal.StringToHGlobalAuto ("value");

				valueptr = Messaging.IntPtr_objc_msgSend_IntPtr (Class.GetHandle (typeof (NSString)), Selector.GetHandle ("stringWithUTF8String:"), strptr);
				Assert.Throws<RuntimeException> (() => Runtime.GetINativeObject<INativeObject> (valueptr, false), "INativeObject");

				valueptr = Messaging.IntPtr_objc_msgSend_IntPtr (Class.GetHandle (typeof (NSString)), Selector.GetHandle ("stringWithUTF8String:"), strptr);
				obj = Runtime.GetINativeObject<NSObject> (valueptr, false) as NSString;
				Assert.AreEqual ("value", (string) obj, "NSObject");

				valueptr = Messaging.IntPtr_objc_msgSend_IntPtr (Class.GetHandle (typeof (NSString)), Selector.GetHandle ("stringWithUTF8String:"), strptr);
				obj = Runtime.GetINativeObject<NSString> (valueptr, false) as NSString;
				Assert.AreEqual ("value", (string) obj, "NSString");

				valueptr = Messaging.IntPtr_objc_msgSend_IntPtr (Class.GetHandle (typeof (NSString)), Selector.GetHandle ("stringWithUTF8String:"), strptr);
				var nscopying = Runtime.GetINativeObject<INSCopying> (valueptr, false);
				Assert.NotNull (nscopying, "INSCopying");
			} finally {
				Marshal.FreeHGlobal (strptr);
			}
		}

		[Test]
		public void NSAutoreleasePoolInThreadPool ()
		{
			var count = 100;
			var counter = new CountdownEvent (count);
			var obj = new NSObject ();

			for (int i = 0; i < count; i++) {
				ThreadPool.QueueUserWorkItem ((v) => {
					obj.DangerousRetain ().DangerousAutorelease ();
					counter.Signal ();
				});
			}

			Assert.IsTrue (counter.Wait (TimeSpan.FromSeconds (5)), "timed out");
			// there is a race condition here: we don't necessarily know when the
			// threadpool's autorelease pools are freed (in theory we can have X
			// threadpool threads stopped just after the 'counter.Signal' line,
			// before unwinding to the autorelease pool's dispose frame). So 
			// assert that the retain count has decreased, but not that it has
			// decreased to 1
			var max_iterations = 100;
			var iterations = 0;
			while ((long) obj.RetainCount > (long) count / 2 && iterations++ < max_iterations) {
				Thread.Sleep (100);
			}
			Assert.That (obj.RetainCount, Is.Not.GreaterThan ((nuint) (count / 2)), "RC. Iterations: " + iterations);

			obj.Dispose ();
		}

		[Test]
		public void NSAutoreleasePoolInThread ()
		{
			var count = 10;
			var threads = new Thread [count];
			var obj = new NSObject ();

			for (int i = 0; i < count; i++) {
				threads [i] = new Thread ((v) => {
					obj.DangerousRetain ().DangerousAutorelease ();
				}) {
					Name = $"NSAutoreleasePoolInThread #{i}",
					IsBackground = true,
				};
				threads [i].Start ();
			}

			for (var i = 0; i < count; i++) {
				Assert.IsTrue (threads [i].Join (TimeSpan.FromSeconds (1)), $"Thread #{i}");
			}

			// Strangely enough there seems to be a race condition here, not all threads will necessarily
			// have completed the autorelease by this point. Some should have though, so assert that the object
			// was released on at least half the threads.
			Assert.That ((int) obj.RetainCount, Is.LessThan (count / 2), "RC");

			obj.Dispose ();
		}

		class ResurrectedObjectsDisposedTestClass : NSObject {
			[Export ("invokeMe:wait:")]
			static bool InvokeMe (NSObject obj, int invokerWait)
			{
				Thread.Sleep (invokerWait);

				return obj.Handle != IntPtr.Zero;
			}
		}

		[Test]
#if !MONOMAC || NET // Failing with 10 broken in legacy Xamarin.Mac
		[TestCase (typeof (NSObject))]
#endif
		[TestCase (typeof (ResurrectedObjectsDisposedTestClass))]
		public void ResurrectedObjectsDisposedTest (Type type)
		{
#if __WATCHOS__
			if (Runtime.Arch == Arch.DEVICE)
				Assert.Ignore ("This test uses too much memory for the watch.");
#endif

			var invokerClassHandle = Class.GetHandle (typeof (ResurrectedObjectsDisposedTestClass));

			// Create a number of native objects with no managed wrappers.
			// We create more than one to try to minimize the random effects
			// of the GC (a random value somewhere can keep a single object
			// from being collected, but the chances of X random values keeping
			// X objects from being collected are much lower).
			// Also the consequences if the GC doesn't collect an object is
			// that the test unexpectedly succeeds.
			// 10 objects seem to be a fine number that will cause pretty much
			// all test executions to fail.
			var objects = new IntPtr [10];
			for (int i = 0; i < objects.Length; i++)
				objects [i] = Messaging.IntPtr_objc_msgSend (Messaging.IntPtr_objc_msgSend (Class.GetHandle (type), Selector.GetHandle ("alloc")), Selector.GetHandle ("init"));

			// Create a thread that creates managed wrappers for all of the above native objects.
			// We do this on a separate thread so that the GC finds no pointers to the managed
			// objects in any thread.
			var t1 = new Thread (() => {
				for (int i = 0; i < objects.Length; i++)
					Messaging.bool_objc_msgSend_IntPtr_int (invokerClassHandle, Selector.GetHandle ("invokeMe:wait:"), objects [i], 0);
			});
			t1.Start ();
			t1.Join ();

			// Collect all those managed wrappers, and make sure their finalizers are executed.
			GC.Collect ();
			GC.WaitForPendingFinalizers ();

			// Now all the managed wrappers should be on NSObject's drain queue on the main thread.

			// Spin up a thread for every native object, and invoke a managed method
			// that will fetch the managed wrapper and then wait for a while (500ms),
			// before verifying that the managed object hasn't been disposed while waiting.
			var invokerWait = 500;
			var threads = new Thread [objects.Length];
			var brokenCount = 0;
			var countdown = new CountdownEvent (threads.Length);
			for (int t = 0; t < threads.Length; t++) {
				var tt = t;
				var thread = new Thread (() => {
					var ok = Messaging.bool_objc_msgSend_IntPtr_int (invokerClassHandle, Selector.GetHandle ("invokeMe:wait:"), objects [tt], invokerWait);
					if (!ok) {
						//Console.WriteLine ("Broken #{0}: 0x{1}", tt, objects [tt].ToString ("x"));
						Interlocked.Increment (ref brokenCount);
					}
					countdown.Signal ();
				});
				thread.Start ();
			}

			// Now all those threads should be waiting.

			// Run the runloop on the main thread, which will drain the managed wrappers
			// on NSObject's drain queue.
			while (!countdown.IsSet) {
				NSRunLoop.Current.RunUntil ((NSDate) DateTime.Now.AddSeconds (0.1));
			}

			// Release all the native objects we created
			for (int i = 0; i < objects.Length; i++)
				Messaging.void_objc_msgSend (objects [i], Selector.GetHandle ("release"));

			Assert.AreEqual (0, brokenCount, "broken count");
		}

		[Test]
		public void MX8029 ()
		{
			var handle = Messaging.IntPtr_objc_msgSend (Messaging.IntPtr_objc_msgSend (Class.GetHandle (typeof (Dummy)), Selector.GetHandle ("alloc")), Selector.GetHandle ("init"));
			try {
				try {
					Messaging.void_objc_msgSend_IntPtr (Class.GetHandle (typeof (Dummy)), Selector.GetHandle ("doSomethingElse:"), handle);
					Assert.Fail ("Expected an MX8029 exception (A)");
				} catch (RuntimeException mex) {
					Assert.That (mex.Code, Is.EqualTo (8029).Or.EqualTo (8027), "Exception code (A)");
					var failure = mex.ToString ();
					Assert.That (failure, Does.Contain ("Failed to marshal the Objective-C object"), "Failed to marshal (A)");
					Assert.That (failure, Does.Contain ("Additional information:"), "Additional information: (A)");
					Assert.That (failure, Does.Contain ("Selector: doSomethingElse:"), "Selector (A)");
					Assert.That (failure, Does.Contain ("DoSomethingElse"), "DoSomethingElse (A)");
				}

				try {
					Messaging.void_objc_msgSend_IntPtr (handle, Selector.GetHandle ("doSomething:"), handle);
					Assert.Fail ("Expected an MX8034 exception (B)");
				} catch (RuntimeException mex) {
					Assert.That (mex.Code, Is.EqualTo (8034).Or.EqualTo (8027), "Exception code (B)");
					var failure = mex.ToString ();
					Assert.That (failure, Does.Contain ("Failed to marshal the Objective-C object"), "Failed to marshal (B)");
					Assert.That (failure, Does.Contain ("Additional information:"), "Additional information: (B)");
					Assert.That (failure, Does.Contain ("Selector: doSomething:"), "Selector (B)");
					Assert.That (failure, Does.Contain ("DoSomething"), "DoSomething (B)");
				}
			} finally {
				Messaging.void_objc_msgSend (handle, Selector.GetHandle ("release"));
			}
		}

#if DYNAMIC_REGISTRAR
		[Test]
		public void MX8029_b ()
		{
			try {
				using (var arr = new NSArray ()) {
					Messaging.void_objc_msgSend_IntPtr (Class.GetHandle (typeof (Dummy)), Selector.GetHandle ("setIntArray:"), arr.Handle);
					Assert.Fail ("An exception should have been thrown");
				}
			} catch (RuntimeException re) {
				Assert.AreEqual (8029, re.Code, "Code");
				string expectedExceptionMessage;
				if (TestRuntime.IsCoreCLR) {
					expectedExceptionMessage = @"Unable to marshal the array parameter #1 whose managed type is 'System.Int32[]' to managed.
Additional information:
	Selector: setIntArray:
	Method: System.Void MonoTouchFixtures.ObjCRuntime.RuntimeTest+Dummy.SetIntArray (System.Int32[])
";
				} else {
					expectedExceptionMessage = @"Unable to marshal the array parameter #1 whose managed type is 'System.Int32[]' to managed.
Additional information:
	Selector: setIntArray:
	Method: MonoTouchFixtures.ObjCRuntime.RuntimeTest/Dummy:SetIntArray (int[])
";
				}
				Assert.AreEqual (expectedExceptionMessage, re.Message, "Message");
				var inner = (RuntimeException)re.InnerException;
				Assert.AreEqual (8031, inner.Code, "Inner Code");
				Assert.AreEqual ("Unable to convert from an NSArray to a managed array of System.Int32.", inner.Message, "Inner Message");
			}
		}

		[Test]
		public void MX8033 ()
		{
			try {
				Messaging.IntPtr_objc_msgSend (Class.GetHandle (typeof (Dummy)), Selector.GetHandle ("intArray"));
				Assert.Fail ("An exception should have been thrown");
			} catch (RuntimeException re) {
				Assert.AreEqual (8033, re.Code, "Code");
				string expectedExceptionMessage;
				if (TestRuntime.IsCoreCLR) {
					expectedExceptionMessage = @"Unable to marshal the return value of type 'System.Int32[]' to Objective-C.
Additional information:
	Selector: intArray
	Method: System.Int32[] MonoTouchFixtures.ObjCRuntime.RuntimeTest+Dummy.GetIntArray ()
";
				} else {
					expectedExceptionMessage = @"Unable to marshal the return value of type 'System.Int32[]' to Objective-C.
Additional information:
	Selector: intArray
	Method: MonoTouchFixtures.ObjCRuntime.RuntimeTest/Dummy:GetIntArray ()
";
				}
				Assert.AreEqual (expectedExceptionMessage, re.Message, "Message");
				var inner = (RuntimeException) re.InnerException;
				Assert.AreEqual (8032, inner.Code, "Inner Code");
				Assert.AreEqual ("Unable to convert from a managed array of System.Int32 to an NSArray.", inner.Message, "Inner Message");
			}
		}
#endif

		// does not have an IntPtr constructor
		class Dummy : NSObject {
			[Export ("initWithFoo:")]
			public Dummy (int foo)
			{
				// Prevent the default ctor from being exported.
			}

			[Export ("doSomething:")]
			public void DoSomething (Dummy dummy)
			{
			}

			[Export ("doSomethingElse:")]
			public static void DoSomethingElse (Dummy dummy)
			{
			}

#if DYNAMIC_REGISTRAR
			// This function makes the static registrar show an error, so it's only built when using the dynamic registrar.
			[Export ("intArray")]
			static int[] GetIntArray ()
			{
				return new int [] { };
			}
			// This function makes the static registrar show an error, so it's only built when using the dynamic registrar.
			[Export ("setIntArray:")]
			static void SetIntArray (int[] array)
			{
				Assert.Fail ("This method should never be called.");
			}
#endif
		}

		[Test]
		public void GetINativeObject_ForcedType ()
		{
			using (var str = new NSString ("hello world")) {
				NSDate date;

				Assert.Throws<InvalidCastException> (() => Runtime.GetINativeObject<NSDate> (str.Handle, false), "EX1"); //
				Assert.Throws<InvalidCastException> (() => Runtime.GetINativeObject<NSDate> (str.Handle, false, false), "EX2"); //

				// Making a string quack like a date.
				// This is a big hack, but hopefully it works well enough for this particular test case.
				// Just don't inspect the date variable in a debugger (it will most likely crash the app).
				date = Runtime.GetINativeObject<NSDate> (str.Handle, true, false);
				Assert.AreEqual (date.Handle, str.Handle, "Same native pointer");
				date.Dispose ();
				date = null;
			}
		}

		[Test]
		public void ToggleRef_NonToggledObjectsShouldBeCollected ()
		{
			// This test verifies that toggleable objects that aren't toggled aren't kept alive.
			// We create a number of managed NSFileManager instance, get a native reference to each of them,
			// and then we verify that the managed instance is collected.
			var counter = 100;
			var handles = new GCHandle [counter];
			var pointers = new IntPtr [counter];

			var t = new Thread (() => {
				for (var i = 0; i < counter; i++) {
					var obj = new NSFileManager ();
					// do not toggle
					obj.DangerousRetain (); // obtain a native reference
					handles [i] = GCHandle.Alloc (obj, GCHandleType.Weak);
					pointers [i] = obj.Handle;
				}
			}) {
				IsBackground = true,
				Name = "ToggleRef_NonToggledObjectsShouldBeCollected",
			};
			t.Start ();
			Assert.IsTrue (t.Join (TimeSpan.FromSeconds (10)), "Background thread completion");

			var checkForCollectedManagedObjects = new Func<bool> (() => {
				GC.Collect ();
				GC.WaitForPendingFinalizers ();
				for (var i = 0; i < counter; i++) {
					if (handles [i].Target is null)
						return true;
				}
				return false;
			});

			// Iterate over the runloop in case something has to happen on the main thread for the objects to be collected.
			TestRuntime.RunAsync (TimeSpan.FromSeconds (5), checkForCollectedManagedObjects);

			Assert.IsTrue (checkForCollectedManagedObjects (), "Any collected objects");

			for (var i = 0; i < counter; i++) {
				var obj = Runtime.GetNSObject (pointers [i]);
				Assert.IsNotNull (obj, $"Object #{i} couldn't be resurrected");
				obj.DangerousRelease (); // release the native reference
				obj.Dispose ();
				handles [i].Free ();
			}
		}

		[Test]
		public void ToggleRef_ToggledObjectsShouldNotBeCollected ()
		{
			// This test verifies that toggleable objects that are toggled are kept alive while the native peer is alive.
			// We create a number of managed NSFileManager instance, get a native reference to each of them,
			// and then we verify that the managed instance won't be collected.
			//
			// NSFileManager instances are toggled when the [Weak]Delegate property is set.
			var del = new NSFileManagerDelegate ();
			var counter = 100;
			var handles = new GCHandle [counter];
			var t = new Thread (() => {
				for (var i = 0; i < counter; i++) {
					var obj = new NSFileManager ();
					obj.Delegate = del; // toggle
					obj.DangerousRetain (); // obtain a native reference
					handles [i] = GCHandle.Alloc (obj, GCHandleType.Weak);
				}
			}) {
				IsBackground = true,
				Name = "ToggleRef_ToggledObjectsShouldNotBeCollected",
			};
			t.Start ();
			Assert.IsTrue (t.Join (TimeSpan.FromSeconds (10)), "Background thread completion");

			TestRuntime.RunAsync (TimeSpan.FromSeconds (2), () => {
				// Iterate over the runloop a bit to make sure we're just not collecting because objects are queued on for things to happen on the main thread
				GC.Collect ();
				GC.WaitForPendingFinalizers ();
				return false;
			});

			for (var i = 0; i < counter; i++) {
				var obj = (NSFileManager) handles [i].Target;
				Assert.IsNotNull (obj, $"Object #{i} was unexpectedly collected.");
				obj.DangerousRelease (); // release the native reference
				obj.Dispose ();
				handles [i].Free ();
			}
		}

		[Test]
		public void CurrentDirectory ()
		{
			var expectedDirectory = (string) ((NSString) Environment.CurrentDirectory).ResolveSymlinksInPath ();

#if NET || !MONOMAC
			var actualDirectory = (string) ((NSString) NSBundle.MainBundle.BundlePath).ResolveSymlinksInPath ();
#else
			var actualDirectory = (string) ((NSString) NSBundle.MainBundle.ResourcePath).ResolveSymlinksInPath ();
#endif
			Assert.AreEqual (expectedDirectory, actualDirectory, "Current directory at launch");
		}

		[Test]
		public void CurrentDomain_BaseDirectory_Test ()
		{
			Assert.That (AppDomain.CurrentDomain.BaseDirectory, Is.Not.Null.And.Not.Empty, "AppDomain.CurrentDomain.BaseDirectory");
		}

		[Test]
		public void OriginalWorkingDirectoryTest ()
		{
			Assert.That (Runtime.OriginalWorkingDirectory, Is.Not.Null.And.Not.Empty, "OriginalWorkingDirectory");
		}

#if NET
		[Test]
		public void IntPtrCtor_1 ()
		{
			using var obj = Runtime.GetNSObject (IntPtrConstructor.New ());
			Assert.IsNotNull (obj, "NotNull");
			Assert.That (obj, Is.TypeOf<IntPtrConstructor> (), "Type");
			Assert.AreNotEqual (IntPtr.Zero, obj.Handle, "Handle");
		}

		[Test]
		public void IntPtrCtor_2 ()
		{
			using var obj = Runtime.GetNSObject<IntPtrConstructor> (IntPtrConstructor.New ());
			Assert.IsNotNull (obj, "NotNull");
			Assert.That (obj, Is.TypeOf<IntPtrConstructor> (), "Type");
			Assert.AreNotEqual (IntPtr.Zero, obj.Handle, "Handle");
		}

		[Test]
		public void IntPtrCtor_3 ()
		{
			using var obj = Runtime.GetINativeObject<IntPtrConstructor> (IntPtrConstructor.New (), false);
			Assert.IsNotNull (obj, "NotNull");
			Assert.That (obj, Is.TypeOf<IntPtrConstructor> (), "Type");
			Assert.AreNotEqual (IntPtr.Zero, obj.Handle, "Handle");
		}

		class IntPtrConstructor : NSObject {
			IntPtrConstructor (IntPtr handle) : base (handle) { }

			internal static IntPtr New ()
			{
				var class_handle = Class.GetHandle (typeof (IntPtrConstructor));
				var handle = Messaging.IntPtr_objc_msgSend (Messaging.IntPtr_objc_msgSend (class_handle, Selector.GetHandle ("alloc")), Selector.GetHandle ("init"));
				Messaging.void_objc_msgSend (handle, Selector.GetHandle ("autorelease"));
				return handle;
			}
		}

		[Test]
		public void IntPtrBoolCtor_1 ()
		{
			using var obj = Runtime.GetINativeObject<IntPtrBoolConstructor> ((IntPtr) 1234, false);
			Assert.IsNotNull (obj, "NotNull");
			Assert.That (obj, Is.TypeOf<IntPtrBoolConstructor> (), "Type");
			Assert.AreNotEqual (IntPtr.Zero, obj.Handle, "Handle");
		}

		class IntPtrBoolConstructor : DisposableObject {
			IntPtrBoolConstructor (IntPtr handle, bool owns)
				: base (handle, owns)
			{
			}
		}
#endif
	}

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class RuntimeTest_GetINativeObjectTest {

		[Test]
		public void GetINativeObjectTest_NSObject_Owns ()
		{
			var handle = CreateNativeNSObject ();
			Assert.AreEqual (1, GetRetainCount (handle), "A 1");
			using var obj = Runtime.GetINativeObject<NSObject> (handle, false, true);
			Assert.AreEqual (1, GetRetainCount (handle), "A 2");
			obj.DangerousRetain ();
			Assert.AreEqual (2, GetRetainCount (handle), "A 3");
			using var obj2 = Runtime.GetINativeObject<NSObject> (handle, false, true);
			Assert.AreEqual (1, GetRetainCount (handle), "A 4");
			using var obj3 = Runtime.GetINativeObject<NSObject> (handle, false, false);
			Assert.AreEqual (1, GetRetainCount (handle), "A 5");
			Assert.AreSame (obj, obj2, "A 6");
			Assert.AreSame (obj, obj3, "A 7");
		}

		[Test]
		public void GetINativeObjectTest_NSObject_Unowned ()
		{
			var handle = CreateNativeNSObject ();
			Assert.AreEqual (1, GetRetainCount (handle), "A 1");
			using var obj = Runtime.GetINativeObject<NSObject> (handle, false, false);
			Assert.AreEqual (2, GetRetainCount (handle), "A 2");
			obj.DangerousRelease ();
			Assert.AreEqual (1, GetRetainCount (handle), "A 3");
			obj.DangerousRetain ();
			Assert.AreEqual (2, GetRetainCount (handle), "A 4");
			using var obj2 = Runtime.GetINativeObject<NSObject> (handle, false, true);
			Assert.AreEqual (1, GetRetainCount (handle), "A 5");
			using var obj3 = Runtime.GetINativeObject<NSObject> (handle, false, false);
			Assert.AreEqual (1, GetRetainCount (handle), "A 6");
			Assert.AreSame (obj, obj2, "A 7");
			Assert.AreSame (obj, obj3, "A 8");
		}

		[Test]
		public void GetINativeObjectTest_INativeObject_Owns ()
		{
			var handle = CreateNativeBitmapContext ();
			Assert.AreEqual (1, CFGetRetainCount (handle), "A 1");
			using var obj = Runtime.GetINativeObject<CGBitmapContext> (handle, false, true);
			Assert.AreEqual (1, CFGetRetainCount (handle), "A 2");
			CGContextRetain (obj.Handle);
			Assert.AreEqual (2, CFGetRetainCount (handle), "A 3");
			using var obj2 = Runtime.GetINativeObject<CGBitmapContext> (handle, false, true); // does not decrease refcount because we return a new instance
			Assert.AreEqual (2, CFGetRetainCount (handle), "A 4");
			using var obj3 = Runtime.GetINativeObject<CGBitmapContext> (handle, false, false);
			Assert.AreEqual (3, CFGetRetainCount (handle), "A 5");
			Assert.AreNotSame (obj, obj2, "A 6");
			Assert.AreNotSame (obj, obj3, "A 7");
			obj3.Dispose ();
			Assert.AreEqual (2, CFGetRetainCount (handle), "A 8");
			obj2.Dispose ();
			Assert.AreEqual (1, CFGetRetainCount (handle), "A 9");
		}

		[Test]
		public void GetINativeObjectTest_INativeObject_Unowned ()
		{
			var handle = CreateNativeBitmapContext ();
			Assert.AreEqual (1, CFGetRetainCount (handle), "A 1");
			using var obj = Runtime.GetINativeObject<CGBitmapContext> (handle, false, false);
			Assert.AreEqual (2, CFGetRetainCount (handle), "A 2");
			CGContextRelease (obj.Handle);
			Assert.AreEqual (1, CFGetRetainCount (handle), "A 3");
			CGContextRetain (obj.Handle);
			Assert.AreEqual (2, CFGetRetainCount (handle), "A 4");
			using var obj2 = Runtime.GetINativeObject<CGBitmapContext> (handle, false, true); // does not decrease refcount because we return a new instance
			Assert.AreEqual (2, CFGetRetainCount (handle), "A 5");
			using var obj3 = Runtime.GetINativeObject<CGBitmapContext> (handle, false, false);
			Assert.AreEqual (3, CFGetRetainCount (handle), "A 6");
			Assert.AreNotSame (obj, obj2, "A 7");
			Assert.AreNotSame (obj, obj3, "A 8");
			obj3.Dispose ();
			Assert.AreEqual (2, CFGetRetainCount (handle), "A 9");
			obj2.Dispose ();
			Assert.AreEqual (1, CFGetRetainCount (handle), "A 10");
		}

		static IntPtr CreateNativeNSObject ()
		{
			var handle = Messaging.IntPtr_objc_msgSend (Class.GetHandle (typeof (NSObject)), Selector.GetHandle ("alloc"));
			return Messaging.IntPtr_objc_msgSend (handle, Selector.GetHandle ("init"));
		}

		static IntPtr CreateNativeBitmapContext ()
		{
			using var cs = CGColorSpace.CreateDeviceRGB ();
			var handle = CGBitmapContextCreate (IntPtr.Zero, 10, 10, 8, 40, cs.Handle, /* PremultipliedLast */ 1);
			if (handle == IntPtr.Zero)
				throw new InvalidOperationException ($"Unable to create CGBitmapContext.");
			return handle;
		}

		static long GetRetainCount (IntPtr handle)
		{
			if (handle == IntPtr.Zero)
				return -1;
			return (long) Messaging.IntPtr_objc_msgSend (handle, Selector.GetHandle ("retainCount"));
		}

		static long CFGetRetainCount (IntPtr handle)
		{
			if (handle == IntPtr.Zero)
				return -1;
			return (long) global::MonoTouchFixtures.CoreFoundation.ArrayTest.CFGetRetainCount (handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGContextRelease (/* CGImageRef */ IntPtr image);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGImageRef */ IntPtr CGContextRetain (/* CGImageRef */ IntPtr image);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static IntPtr CGBitmapContextCreate (/* void* */ IntPtr data, /* size_t */ nint width, /* size_t */ nint height, /* size_t */ nint bitsPerComponent,
			/* size_t */ nint bytesPerRow, /* CGColorSpaceRef */ IntPtr colorSpace, /* CGBitmapInfo = uint32_t */ uint bitmapInfo);

	}
}
