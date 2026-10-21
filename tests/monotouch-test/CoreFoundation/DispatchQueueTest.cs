//
// Unit tests for DispatchQueue
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2018 Microsoft Corp. All rights reserved.
//

using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
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
#if !XAMCORE_5_0
		[TestCase (0, false)]
		[TestCase (1, false)]
		[TestCase (8, false)]
		[TestCase (8, true)]
		public void Submit (int iterations, bool concurrent)
		{
			var queue = new DispatchQueue ("Submit", concurrent);
			var reference = new WeakReference (queue);
			var visits = new int [iterations];
			var collected = 0;
			queue.Submit ((int index) => {
				GC.Collect ();
				GC.WaitForPendingFinalizers ();
				if (!reference.IsAlive)
					Interlocked.Increment (ref collected);
				Interlocked.Increment (ref visits [index]);
			}, iterations);
			Assert.That (collected, Is.EqualTo (0), "Queue must remain alive during all iterations");
			for (var i = 0; i < visits.Length; i++)
				Assert.That (visits [i], Is.EqualTo (1), $"Iteration {i} must run exactly once before Submit returns");
		}
#endif // !XAMCORE_5_0

		[TestCase (0, false)]
		[TestCase (1, false)]
		[TestCase (8, false)]
		[TestCase (8, true)]
		public void SubmitNative (int iterations, bool concurrent)
		{
			var queue = new DispatchQueue ("SubmitNative", concurrent);
			var reference = new WeakReference (queue);
			var visits = new int [iterations];
			var collected = 0;
			queue.Submit ((nint index) => {
				GC.Collect ();
				GC.WaitForPendingFinalizers ();
				if (!reference.IsAlive)
					Interlocked.Increment (ref collected);
				Interlocked.Increment (ref visits [index]);
			}, (nint) iterations);
			Assert.That (collected, Is.EqualTo (0), "Queue must remain alive during all iterations");
			for (var i = 0; i < visits.Length; i++)
				Assert.That (visits [i], Is.EqualTo (1), $"Iteration {i} must run exactly once before Submit returns");
		}

		[Test]
		public void SubmitOverloadResolution ()
		{
			using var queue = new DispatchQueue ("SubmitOverloadResolution");
			object index = null;
			queue.Submit (iteration => index = iteration, 1);
			Assert.That (index, Is.TypeOf<nint> (), "An untyped callback must select the native-sized overload");
		}

		[Test]
		public void SubmitNullAction ()
		{
			using var queue = new DispatchQueue ("SubmitNullAction");
			Assert.Throws<ArgumentNullException> (() => queue.Submit ((Action<nint>) null, (nint) 0), "Native");
#if !XAMCORE_5_0
			Assert.Throws<ArgumentNullException> (() => queue.Submit ((Action<int>) null, 0L), "Compatibility");
#endif // !XAMCORE_5_0
		}

		[TestCase (false)]
#if !XAMCORE_5_0
		[TestCase (true)]
#endif // !XAMCORE_5_0
		public void SubmitZeroReleasesCallbackState (bool compatibility)
		{
			var reference = SubmitZeroWithCapturedState (compatibility);
			GC.Collect ();
			GC.WaitForPendingFinalizers ();
			GC.Collect ();
			Assert.That (reference.IsAlive, Is.False, "Zero iterations must release the callback state");
		}

		[MethodImpl (MethodImplOptions.NoInlining)]
		static WeakReference SubmitZeroWithCapturedState (bool compatibility)
		{
			using var queue = new DispatchQueue ("SubmitZeroWithCapturedState");
			var state = new int [1];
			var reference = new WeakReference (state);
#if !XAMCORE_5_0
			if (compatibility)
				queue.Submit ((int index) => state [0] = index, 0L);
			else
#endif // !XAMCORE_5_0
				queue.Submit ((nint index) => state [0] = (int) index, (nint) 0);
			return reference;
		}

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
