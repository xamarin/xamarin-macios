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
using CoreGraphics;
using CoreFoundation;
using Foundation;
using ObjCRuntime;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
using NUnit.Framework;
using System.Threading;
using Xamarin.Utils;

namespace MonoTouchFixtures.CoreFoundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class DispatchTests {
#if !MONOMAC // UIKitThreadAccessException and NSStringDrawing.WeakDrawString don't exist on mac
		[Test]
		public void MainQueueDispatch ()
		{
#if !DEBUG || OPTIMIZEALL
			Assert.Ignore ("UIKitThreadAccessException is not throw, by default, on release builds (removed by the linker)");
#endif
			bool hit = false;
			// We need to check the UIKitThreadAccessException, but there are very few API
			// with that check on WatchOS. NSStringDrawing.WeakDrawString is one example, here we pass
			// it null for the parameter so that it immediately returns with a NullReferenceException
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
			defaultQ.DispatchAsync (delegate
			{
				try {
					NSStringDrawing.WeakDrawString (null, CGPoint.Empty, null);
				} catch (Exception e) {
					queue_ex = e;
				}

				queueThread = Thread.CurrentThread;
				var mainQ = DispatchQueue.MainQueue;
				mainQ.DispatchAsync (delegate
				{
					mainQthread = Thread.CurrentThread;
					try {
						NSStringDrawing.WeakDrawString (null, CGPoint.Empty, null);
					} catch (Exception e) {
						ex = e;
					} finally {
						hit = true;
					}
				});

			});

			// Now wait for the above to actually run
			while (hit == false) {
				NSRunLoop.Current.RunUntil (NSDate.FromTimeIntervalSinceNow (0.5));
			}
			Assert.IsNotNull (ex, "main ex");
			Assert.That (ex.GetType (), Is.SameAs (typeof (NullReferenceException)), "no thread check hit");
			Assert.IsNotNull (queue_ex, "queue ex");
			Assert.That (queue_ex.GetType (), Is.SameAs (typeof (UIKitThreadAccessException)), "thread check hit");
			Assert.That (uiThread, Is.EqualTo (mainQthread), "mainq thread is equal to uithread");
			Assert.That (queueThread, Is.Not.EqualTo (mainQthread), "queueThread is not the same as the UI thread");
		}

		[Test]
		public void MainQueueDispatchQualityOfService ()
		{
#if !DEBUG || OPTIMIZEALL
			Assert.Ignore ("UIKitThreadAccessException is not throw, by default, on release builds (removed by the linker)");
#endif
			bool hit = false;
			// We need to check the UIKitThreadAccessException, but there are very few API
			// with that check on WatchOS. NSStringDrawing.WeakDrawString is one example, here we pass
			// it null for the parameter so that it immediately returns with a NullReferenceException
			// instead of trying to load an image (which is not what we're testing). There
			// is also a test to ensure UIKitThreadAccessException is thrown if not on
			// the UI thread (so that we'll notice if the UIKitThreadAccessException is ever
			// removed from NSStringDrawing.WeakDrawString).
			var uiThread = Thread.CurrentThread;
			Thread queueThread = null;
			Thread mainQthread = null;
			Exception ex = null;
			Exception queue_ex = null;

			var defaultQ = DispatchQueue.GetGlobalQueue (DispatchQualityOfService.Default);
			defaultQ.DispatchAsync (delegate
			{
				try {
					NSStringDrawing.WeakDrawString (null, CGPoint.Empty, null);
				} catch (Exception e) {
					queue_ex = e;
				}

				queueThread = Thread.CurrentThread;
				var mainQ = DispatchQueue.MainQueue;
				mainQ.DispatchAsync (delegate
				{
					mainQthread = Thread.CurrentThread;
					try {
						NSStringDrawing.WeakDrawString (null, CGPoint.Empty, null);
					} catch (Exception e) {
						ex = e;
					} finally {
						hit = true;
					}
				});

			});

			// Now wait for the above to actually run
			while (hit == false) {
				NSRunLoop.Current.RunUntil (NSDate.FromTimeIntervalSinceNow (0.5));
			}
			Assert.IsNotNull (ex, "main ex");
			Assert.That (ex.GetType (), Is.SameAs (typeof (NullReferenceException)), "no thread check hit");
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
			if (TestRuntime.CheckSystemVersion (ApplePlatform.iOS, 8, 0))
				qname = "com.apple.root.default-qos";
#elif __WATCHOS__ || __TVOS__
			qname = "com.apple.root.default-qos";
#elif __MACOS__
			if (TestRuntime.CheckSystemVersion (ApplePlatform.MacOSX, 10, 10))
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
				q.DispatchAsync (delegate
				{
					t0 = Thread.CurrentThread.ManagedThreadId;
					n++;
				});
				DispatchQueue.DefaultGlobalQueue.SetTargetQueue (DispatchQueue.MainQueue);
				q.DispatchAsync (delegate
				{
					t1 = Thread.CurrentThread.ManagedThreadId;
					n++;
				});
				DispatchQueue.DefaultGlobalQueue.SetTargetQueue (null);
				q.DispatchAsync (delegate
				{
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
			Assert.That (DispatchQueue.MainQueue.Label, Is.EqualTo ("com.apple.main-thread"), "Main");
		}

		[Test]
		public void GetGlobalQueue_Priority ()
		{
			// values changes in OS versions (and even in arch) but we only want to make sure we get a valid string so the prefix is enough
			Assert.True (DispatchQueue.GetGlobalQueue (DispatchQueuePriority.Default).Label.StartsWith ("com.apple.root."), "Default");
			Assert.True (DispatchQueue.GetGlobalQueue (DispatchQueuePriority.Low).Label.StartsWith ("com.apple.root."), "Low");
			Assert.True (DispatchQueue.GetGlobalQueue (DispatchQueuePriority.High).Label.StartsWith ("com.apple.root."), "High");
		}

		[Test]
		public void GetGlobalQueue_QualityOfService ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 10, throwIfOtherPlatform: false);

			// values changes in OS versions (and even in arch) but we only want to make sure we get a valid string so the prefix is enough
			Assert.True (DispatchQueue.GetGlobalQueue (DispatchQualityOfService.Default).Label.StartsWith ("com.apple.root."), "Default");
			Assert.True (DispatchQueue.GetGlobalQueue (DispatchQualityOfService.Utility).Label.StartsWith ("com.apple.root."), "Low");
			Assert.True (DispatchQueue.GetGlobalQueue (DispatchQualityOfService.UserInitiated).Label.StartsWith ("com.apple.root."), "High");
		}

		[Test]
		public void NeverTooLate ()
		{
			Assert.That (DispatchTime.Now.Nanoseconds, Is.EqualTo (0), "Now");
			Assert.That (DispatchTime.Forever.Nanoseconds, Is.EqualTo (unchecked((ulong) ~0)), "Forever");

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
#if !DEBUG || OPTIMIZEALL
			Assert.Ignore ("UIKitThreadAccessException is not throw, by default, on release builds (removed by the linker)");
#endif
			bool hit = false;
			// We need to check the UIKitThreadAccessException, but there are very few API
			// with that check on WatchOS. NSStringDrawing.WeakDrawString is one example, here we pass
			// it null for the parameter so that it immediately returns with a NullReferenceException
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
			defaultQ.DispatchAfter (new DispatchTime (DispatchTime.Now, 1000), delegate
			{
				try {
					NSStringDrawing.WeakDrawString (null, CGPoint.Empty, null);
				} catch (Exception e) {
					queue_ex = e;
				}

				queueThread = Thread.CurrentThread;
				var mainQ = DispatchQueue.MainQueue;
				mainQ.DispatchAfter (DispatchTime.Now, delegate
				{
					mainQthread = Thread.CurrentThread;
					try {
						NSStringDrawing.WeakDrawString (null, CGPoint.Empty, null);
					} catch (Exception e) {
						ex = e;
					} finally {
						hit = true;
					}
				});
			});

			// Now wait for the above to actually run
			while (hit == false) {
				NSRunLoop.Current.RunUntil (NSDate.FromTimeIntervalSinceNow (0.5));
			}
			Assert.IsNotNull (ex, "main ex");
			Assert.That (ex.GetType (), Is.SameAs (typeof (NullReferenceException)), "no thread check hit");
			Assert.IsNotNull (queue_ex, "queue ex");
			Assert.That (queue_ex.GetType (), Is.SameAs (typeof (UIKitThreadAccessException)), "thread check hit");
			Assert.That (uiThread, Is.EqualTo (mainQthread), "mainq thread is equal to uithread");
			Assert.That (queueThread, Is.Not.EqualTo (mainQthread), "queueThread is not the same as the UI thread");
		}

		[Test]
		public void EverAfterQualityOfService ()
		{
#if !DEBUG || OPTIMIZEALL
			Assert.Ignore ("UIKitThreadAccessException is not throw, by default, on release builds (removed by the linker)");
#endif
			bool hit = false;
			// We need to check the UIKitThreadAccessException, but there are very few API
			// with that check on WatchOS. NSStringDrawing.WeakDrawString is one example, here we pass
			// it null for the parameter so that it immediately returns with a NullReferenceException
			// instead of trying to load an image (which is not what we're testing). There
			// is also a test to ensure UIKitThreadAccessException is thrown if not on
			// the UI thread (so that we'll notice if the UIKitThreadAccessException is ever
			// removed from NSStringDrawing.WeakDrawString).
			var uiThread = Thread.CurrentThread;
			Thread queueThread = null;
			Thread mainQthread = null;
			Exception ex = null;
			Exception queue_ex = null;

			var defaultQ = DispatchQueue.GetGlobalQueue (DispatchQualityOfService.Default);
			defaultQ.DispatchAfter (new DispatchTime (DispatchTime.Now, 1000), delegate
			{
				try {
					NSStringDrawing.WeakDrawString (null, CGPoint.Empty, null);
				} catch (Exception e) {
					queue_ex = e;
				}

				queueThread = Thread.CurrentThread;
				var mainQ = DispatchQueue.MainQueue;
				mainQ.DispatchAfter (DispatchTime.Now, delegate
				{
					mainQthread = Thread.CurrentThread;
					try {
						NSStringDrawing.WeakDrawString (null, CGPoint.Empty, null);
					} catch (Exception e) {
						ex = e;
					} finally {
						hit = true;
					}
				});
			});

			// Now wait for the above to actually run
			while (hit == false) {
				NSRunLoop.Current.RunUntil (NSDate.FromTimeIntervalSinceNow (0.5));
			}
			Assert.IsNotNull (ex, "main ex");
			Assert.That (ex.GetType (), Is.SameAs (typeof (NullReferenceException)), "no thread check hit");
			Assert.IsNotNull (queue_ex, "queue ex");
			Assert.That (queue_ex.GetType (), Is.SameAs (typeof (UIKitThreadAccessException)), "thread check hit");
			Assert.That (uiThread, Is.EqualTo (mainQthread), "mainq thread is equal to uithread");
			Assert.That (queueThread, Is.Not.EqualTo (mainQthread), "queueThread is not the same as the UI thread");
		}
#endif
	}
}
