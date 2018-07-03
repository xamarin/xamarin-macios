//
// Unit tests for Dispatch
//
// Authors:
//	Miguel de Icaza  <miguel@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

using System;
using System.IO;
#if XAMCORE_2_0
using CoreFoundation;
using Foundation;
using ObjCRuntime;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
#else
using MonoTouch.CoreFoundation;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;
using System.Drawing;
using System.Threading;

#if XAMCORE_2_0
using RectangleF=CoreGraphics.CGRect;
using SizeF=CoreGraphics.CGSize;
using PointF=CoreGraphics.CGPoint;
#else
using nfloat=global::System.Single;
using nint=global::System.Int32;
using nuint=global::System.UInt32;
#endif

namespace MonoTouchFixtures.CoreFoundation {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class DispatchTests {
		
		static bool RunningOnSnowLeopard {
			get {
#if !MONOMAC
				if (Runtime.Arch == Arch.DEVICE)
					return false;
#endif
				return !File.Exists ("/usr/lib/system/libsystem_kernel.dylib");
			}
		}

#if !MONOMAC // UIKitThreadAccessException and NSStringDrawing.WeakDrawString don't exist on mac
		[Test]
		public void MainQueueDispatch ()
		{
#if !DEBUG
			Assert.Ignore ("UIKitThreadAccessException is not throw, by default, on release builds (removed by the linker)");
#endif
			if (RunningOnSnowLeopard)
				Assert.Ignore ("this test crash when executed with the iOS simulator on Snow Leopard");

			bool hit = false;
			// We need to check the UIKitThreadAccessException, but there are very few API
			// with that check on WatchOS. NSStringDrawing.WeakDrawString is one example, here we pass
			// it null for the parameter so that it immediately returns with an ArgumentNullException
			// instead of trying to load an image (which is not what we're testing). There
			// is also a test to ensure UIKitThreadAccessException is thrown if not on
			// the UI thread (so that we'll notice if the UIKitThreadAccessException is ever
			// removed from NSStringDrawing.WeakDrawString).
			var uiThread = Thread.CurrentThread;
			Thread queueThread = null;
			Thread mainQthread = null;
			Exception ex = null;
			Exception queue_ex = null;

			var defaultQ = DispatchQueue.GetGlobalQueue (DispatchQueuePriority.Default);
			defaultQ.DispatchAsync (delegate {	
				try {
					NSStringDrawing.WeakDrawString (null, PointF.Empty, null);
				} catch (Exception e) {
					queue_ex = e;
				}

				queueThread = Thread.CurrentThread;
				var mainQ = DispatchQueue.MainQueue;
				mainQ.DispatchAsync (delegate {
					mainQthread = Thread.CurrentThread;
					try {
						NSStringDrawing.WeakDrawString (null, PointF.Empty, null);
					} catch (Exception e) {
						ex = e;
					} finally {
						hit = true;
					}
				} );

			} );
			
			// Now wait for the above to actually run
			while (hit == false){
		        NSRunLoop.Current.RunUntil (NSDate.FromTimeIntervalSinceNow (0.5));
		    }
			Assert.IsNotNull (ex, "main ex");
			Assert.That (ex.GetType (), Is.SameAs (typeof (ArgumentNullException)), "no thread check hit");
			Assert.IsNotNull (queue_ex, "queue ex");
			Assert.That (queue_ex.GetType (), Is.SameAs (typeof (UIKitThreadAccessException)), "thread check hit");
			Assert.That (uiThread, Is.EqualTo (mainQthread), "mainq thread is equal to uithread");
			Assert.That (queueThread, Is.Not.EqualTo (mainQthread), "queueThread is not the same as the UI thread");
		}
#endif

		[Test]
		public void Current ()
		{
			Assert.That (DispatchQueue.CurrentQueue.Label, Is.EqualTo ("com.apple.main-thread"), "Current");
		}

		[Test]
		public void Default ()
		{
			var qname = "com.apple.root.default-priority";
#if __IOS__ 
			if (TestRuntime.CheckiOSSystemVersion (8, 0))
				qname = "com.apple.root.default-qos";
#elif __WATCHOS__ || __TVOS__
			qname = "com.apple.root.default-qos";
#elif __MACOS__
			if (TestRuntime.CheckMacSystemVersion (10, 10))
				qname = "com.apple.root.default-qos";
#endif
			Assert.That (DispatchQueue.DefaultGlobalQueue.Label, Is.EqualTo (qname), "Default");
		}

		[Test]
		public void SetTargetQueue ()
		{
			int n = 0;
			int ct = Thread.CurrentThread.ManagedThreadId;
			int t0 = ct;
			int t1 = ct;
			int t2 = ct;
			using (var q = new DispatchQueue ("my")) {
				Console.WriteLine ();
				q.DispatchAsync (delegate {
					t0 = Thread.CurrentThread.ManagedThreadId;
					n++;
				});
				DispatchQueue.DefaultGlobalQueue.SetTargetQueue (DispatchQueue.MainQueue);
				q.DispatchAsync (delegate {
					t1 = Thread.CurrentThread.ManagedThreadId;
					n++;
				});
				DispatchQueue.DefaultGlobalQueue.SetTargetQueue (null);
				q.DispatchAsync (delegate {
					t2 = Thread.CurrentThread.ManagedThreadId;
					n++;
				});
				Assert.That (q.Label, Is.EqualTo ("my"), "label");
			}
			while (n != 3)
				NSRunLoop.Current.RunUntil (NSDate.FromTimeIntervalSinceNow (1.0));
			// ensure async dispatches were done on another thread
			Assert.That (ct, Is.Not.EqualTo (t0), "t0");
			Assert.That (ct, Is.Not.EqualTo (t1), "t1");
			Assert.That (ct, Is.Not.EqualTo (t2), "t2");
		}

		[Test]
		public void Main ()
		{
			if (RunningOnSnowLeopard)
				Assert.Ignore ("Shows corruption (missing first 4 chars) when executed the iOS simulator on Snow Leopard");
			Assert.That (DispatchQueue.MainQueue.Label, Is.EqualTo ("com.apple.main-thread"), "Main");
		}

		[Test]
		public void GetGlobalQueue_Priority ()
		{
			string qdefault, qlow, qhigh;
			var newDefaults = false;
#if __IOS__
			if (TestRuntime.CheckiOSSystemVersion (8, 0))
				newDefaults = true;
#elif __WATCHOS__ || __TVOS__
			newDefaults = true;
#elif __MACOS__
			if (TestRuntime.CheckMacSystemVersion (10, 10))
				newDefaults = true;
#endif
			if (newDefaults) {
				qdefault = "com.apple.root.default-qos";
				qlow = "com.apple.root.utility-qos";
				qhigh = "com.apple.root.user-initiated-qos";
			} else {
				qdefault = "com.apple.root.default-priority";
				qlow = "com.apple.root.low-priority";
				qhigh = "com.apple.root.high-priority";
			}
			Assert.That (DispatchQueue.GetGlobalQueue (DispatchQueuePriority.Default).Label, Is.EqualTo (qdefault), "Default");
			Assert.That (DispatchQueue.GetGlobalQueue (DispatchQueuePriority.Low).Label, Is.EqualTo (qlow), "Low");
			Assert.That (DispatchQueue.GetGlobalQueue (DispatchQueuePriority.High).Label, Is.EqualTo (qhigh), "High");
		}

		[Test]
		public void NeverTooLate ()
		{
			Assert.That (DispatchTime.Now.Nanoseconds, Is.EqualTo (0), "Now");
			Assert.That (DispatchTime.Forever.Nanoseconds, Is.EqualTo (unchecked ((ulong) ~0)), "Forever");

			var dt = new DispatchTime (1);
			Assert.That (dt.Nanoseconds, Is.EqualTo (1), "1");

			dt = new DispatchTime (DispatchTime.Now, 0);
			Assert.That (dt.Nanoseconds, Is.Not.EqualTo (0), "!0");

			var dt2 = new DispatchTime (dt, Int32.MaxValue);
			Assert.That (dt2.Nanoseconds, Is.GreaterThan (dt.Nanoseconds), "later");
		}

#if !MONOMAC // UIKitThreadAccessException and NSStringDrawing.WeakDrawString don't exist on mac
		[Test]
		public void EverAfter ()
		{
#if !DEBUG
			Assert.Ignore ("UIKitThreadAccessException is not throw, by default, on release builds (removed by the linker)");
#endif
			if (RunningOnSnowLeopard)
				Assert.Ignore ("this test crash when executed with the iOS simulator on Snow Leopard");

			bool hit = false;
			// We need to check the UIKitThreadAccessException, but there are very few API
			// with that check on WatchOS. NSStringDrawing.WeakDrawString is one example, here we pass
			// it null for the parameter so that it immediately returns with an ArgumentNullException
			// instead of trying to load an image (which is not what we're testing). There
			// is also a test to ensure UIKitThreadAccessException is thrown if not on
			// the UI thread (so that we'll notice if the UIKitThreadAccessException is ever
			// removed from NSStringDrawing.WeakDrawString).
			var uiThread = Thread.CurrentThread;
			Thread queueThread = null;
			Thread mainQthread = null;
			Exception ex = null;
			Exception queue_ex = null;

			var defaultQ = DispatchQueue.GetGlobalQueue (DispatchQueuePriority.Default);
			defaultQ.DispatchAfter (new DispatchTime (DispatchTime.Now, 1000), delegate {	
				try {
					NSStringDrawing.WeakDrawString (null, PointF.Empty, null);
				} catch (Exception e) {
					queue_ex = e;
				}

				queueThread = Thread.CurrentThread;
				var mainQ = DispatchQueue.MainQueue;
				mainQ.DispatchAfter (DispatchTime.Now, delegate {
					mainQthread = Thread.CurrentThread;
					try {
						NSStringDrawing.WeakDrawString (null, PointF.Empty, null);
					} catch (Exception e) {
						ex = e;
					} finally {
						hit = true;
					}
				} );
			} );

			// Now wait for the above to actually run
			while (hit == false){
				NSRunLoop.Current.RunUntil (NSDate.FromTimeIntervalSinceNow (0.5));
			}
			Assert.IsNotNull (ex, "main ex");
			Assert.That (ex.GetType (), Is.SameAs (typeof (ArgumentNullException)), "no thread check hit");
			Assert.IsNotNull (queue_ex, "queue ex");
			Assert.That (queue_ex.GetType (), Is.SameAs (typeof (UIKitThreadAccessException)), "thread check hit");
			Assert.That (uiThread, Is.EqualTo (mainQthread), "mainq thread is equal to uithread");
			Assert.That (queueThread, Is.Not.EqualTo (mainQthread), "queueThread is not the same as the UI thread");
		}
#endif
	}
}
