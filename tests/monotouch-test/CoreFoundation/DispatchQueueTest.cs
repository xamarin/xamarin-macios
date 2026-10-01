//
// Unit tests for DispatchQueue
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2018 Microsoft Corp. All rights reserved.
//

using System.IO;
using System.Threading.Tasks;

#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
using Xamarin.Utils;

namespace MonoTouchFixtures.CoreFoundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class DispatchQueueTests {
		[Test]
		public void CtorWithAttributes ()
		{
			TestRuntime.AssertXcodeVersion (8, 0);

			using (var queue = new DispatchQueue ("1", new DispatchQueue.Attributes {
				AutoreleaseFrequency = DispatchQueue.AutoreleaseFrequency.Inherit,
			})) {
				Assert.That (queue.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle 1");
			}

			using (var queue = new DispatchQueue ("2", new DispatchQueue.Attributes {
				IsInitiallyInactive = true,
			})) {
				queue.Activate (); // must activate the queue before it can be released according to Apple's documentation
				Assert.That (queue.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle 2");
			}

			using (var queue = new DispatchQueue ("3", new DispatchQueue.Attributes {
				QualityOfService = DispatchQualityOfService.Utility,
			})) {
				Assert.That (queue.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle 3");
				Assert.That (queue.QualityOfService, Is.EqualTo (DispatchQualityOfService.Utility), "QualityOfService 3");
			}

			using (var target_queue = new DispatchQueue ("4 - target")) {
				using (var queue = new DispatchQueue ("4", new DispatchQueue.Attributes {
					QualityOfService = DispatchQualityOfService.Background,
					AutoreleaseFrequency = DispatchQueue.AutoreleaseFrequency.WorkItem,
					RelativePriority = -1,
				}, target_queue)) {
					Assert.That (queue.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle 4");
					Assert.That (queue.GetQualityOfService (out var relative_priority), Is.EqualTo (DispatchQualityOfService.Background), "QualityOfService 4");
					Assert.That (relative_priority, Is.EqualTo (-1), "RelativePriority 4");
				}
			}
		}

		[Test]
		public void Specific ()
		{
			using (var queue = new DispatchQueue ("Specific")) {
				var key = (IntPtr) 0x31415926;
				queue.SetSpecific (key, "hello world");
				Assert.That (queue.GetSpecific (key), Is.EqualTo ("hello world"), "Key");
			}
		}

		[Test]
		public void DispatchSync ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 8, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 10, throwIfOtherPlatform: false);

			using (var queue = new DispatchQueue ("DispatchSync")) {
				var called = false;
				var callback = new Action (() => called = true);
				queue.DispatchSync (callback);
				Assert.That (called, Is.True, "Called");

				called = false;
				using (var dg = new DispatchBlock (callback))
					queue.DispatchSync (dg);
				Assert.That (called, Is.True, "Called DispatchBlock");
			}
		}

		[Test]
		public void DispatchBarrierSync ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 8, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 10, throwIfOtherPlatform: false);

			using (var queue = new DispatchQueue ("DispatchBarrierSync")) {
				var called = false;
				var callback = new Action (() => called = true);
				queue.DispatchBarrierSync (callback);
				Assert.That (called, Is.True, "Called");

				called = false;
				using (var dg = new DispatchBlock (callback))
					queue.DispatchBarrierSync (dg);
				Assert.That (called, Is.True, "Called DispatchBlock");
			}
		}

		[Test]
		public void DispatchAsync ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 8, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 10, throwIfOtherPlatform: false);

			using (var queue = new DispatchQueue ("DispatchAsync")) {
				{
					var called = new TaskCompletionSource<bool> ();
					var callback = new Action (() => called.SetResult (true));
					queue.DispatchAsync (callback);
					TestRuntime.RunAsync (TimeSpan.FromSeconds (5), called.Task);
					Assert.That (called.Task.Result, Is.True, "Called");
				}
				{
					var called = new TaskCompletionSource<bool> ();
					var callback = new Action (() => called.SetResult (true));
					using (var dg = new DispatchBlock (callback)) {
						queue.DispatchAsync (dg);
						dg.Wait (TimeSpan.FromSeconds (5));
					}
					Assert.That (called.Task.Result, Is.True, "Called DispatchBlock");
				}
			}
		}

		[Test]
		public void DispatchBarrierAsync ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 8, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 10, throwIfOtherPlatform: false);

			using (var queue = new DispatchQueue ("DispatchBarrierAsync")) {
				{
					var called = new TaskCompletionSource<bool> ();
					var callback = new Action (() => called.SetResult (true));
					queue.DispatchBarrierAsync (callback);
					TestRuntime.RunAsync (TimeSpan.FromSeconds (5), called.Task);
					Assert.That (called.Task.Result, Is.True, "Called");
				}
				{
					var called = new TaskCompletionSource<bool> ();
					var callback = new Action (() => called.SetResult (true));
					using (var dg = new DispatchBlock (callback)) {
						queue.DispatchBarrierAsync (dg);
						dg.Wait (TimeSpan.FromSeconds (5));
					}
					Assert.That (called.Task.Result, Is.True, "Called DispatchBlock");
				}
			}
		}

		[Test]
		public void MainQueue ()
		{
			Assert.That (DispatchQueue.MainQueue, Is.EqualTo (DispatchQueue.CurrentQueue), "MainQueue");
		}

#if __MACOS__
		[Test]
		public void GlobalQueueAfterAsyncWindows ()
		{
			// Exceed the 64-worker dispatch limit observed when window animations stall on Tahoe.
			for (var i = 0; i < 128; i++)
				Assert.That (TestRuntime.RunAsync (TimeSpan.FromSeconds (5), Task.CompletedTask), Is.True, $"Window {i}");

			var completed = new TaskCompletionSource<bool> ();
			DispatchQueue.DefaultGlobalQueue.DispatchAsync (() => completed.SetResult (true));
			Assert.That (completed.Task.Wait (TimeSpan.FromSeconds (5)), Is.True, "Global queue remained responsive after closing async-test windows");
		}
#endif
	}
}
