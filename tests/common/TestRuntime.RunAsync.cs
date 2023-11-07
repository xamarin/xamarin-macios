#if !__WATCHOS__
#define CAN_SHOW_ASYNC_UI
#endif

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

using CoreGraphics;
using Foundation;

using NUnit.Framework;

#if HAS_UIKIT
using UIKit;
using XColor = UIKit.UIColor;
using XImage = UIKit.UIImage;
using XImageView = UIKit.UIImageView;
using XViewController = UIKit.UIViewController;
#elif HAS_APPKIT
using AppKit;
using XColor = AppKit.NSColor;
using XImage = AppKit.NSImage;
using XImageView = AppKit.NSImageView;
using XViewController = AppKit.NSViewController;
#else
using XImage = Foundation.NSObject;
#endif

#nullable enable

partial class TestRuntime {

	public static bool RunAsync (TimeSpan timeout, Task task, XImage? imageToShow = null)
	{
		return RunAsync (timeout, task, Task.CompletedTask, imageToShow);
	}

	public static bool RunAsync (TimeSpan timeout, Func<bool> check_completed, XImage? imageToShow = null)
	{
		return RunAsync (timeout, Task.CompletedTask, check_completed, imageToShow);
	}

	public static bool RunAsync (TimeSpan timeout, Func<Task> startTask, Func<bool> check_completed, XImage? imageToShow = null)
	{
		return RunAsync (timeout, startTask (), check_completed, imageToShow);
	}

	public static bool RunAsync (TimeSpan timeout, Func<Task> startTask, Task completionTask, XImage? imageToShow = null)
	{
		return RunAsync (timeout, startTask (), completionTask, imageToShow);
	}

	public static bool RunAsync (TimeSpan timeout, Action action, Func<bool> check_completed, XImage? imageToShow = null)
	{
		var actionTask = Task.Factory.StartNew (
			action,
			CancellationToken.None,
			TaskCreationOptions.None,
			TaskScheduler.FromCurrentSynchronizationContext ()
		);
		return RunAsync (timeout, actionTask, check_completed, imageToShow);
	}

	public static bool RunAsync (TimeSpan timeout, Task startTask, Func<bool> check_completed, XImage? imageToShow = null)
	{
		var completionTaskSource = new TaskCompletionSource<bool> ();

		var checkCompletionTimer = NSTimer.CreateRepeatingScheduledTimer (0.1, (NSTimer timer) => {
			if (check_completed ()) {
				completionTaskSource.SetResult (true);
				timer.Invalidate ();
			}
		});

		try {
			return RunAsync (timeout, startTask, completionTaskSource.Task, imageToShow);
		} finally {
			checkCompletionTimer.Invalidate ();
		}
	}

	public static bool RunAsync (TimeSpan timeout, Task startTask, Task completionTask, XImage? imageToShow = null)
	{
		var rv = TryRunAsync (timeout, startTask, completionTask, imageToShow, out var exception);
		if (exception is not null)
			throw exception;
		return rv;
	}

	public static bool TryRunAsync (TimeSpan timeout, Func<Task> startTask, out Exception? exception)
	{
		return TryRunAsync (timeout, startTask (), Task.CompletedTask, null, out exception);
	}

	public static bool TryRunAsync (TimeSpan timeout, Task startTask, out Exception? exception)
	{
		return TryRunAsync (timeout, startTask, Task.CompletedTask, null, out exception);
	}

	public static bool TryRunAsync (TimeSpan timeout, Task startTask, Task completionTask, out Exception? exception)
	{
		return TryRunAsync (timeout, startTask, completionTask, null, out exception);
	}

	// This function returns 'false' if the timeout was hit before the two tasks completed.
	// This is a bit unconventional: in particular 'true' will return if any of the tasks threw an exception.
	public static bool TryRunAsync (TimeSpan timeout, Task startTask, Task completionTask, XImage? imageToShow, out Exception? exception)
	{
#if CAN_SHOW_ASYNC_UI
		using var ui = ShowAsyncUI (imageToShow);
#endif // CAN_SHOW_ASYNC_UI

		exception = null;

		try {
			var runLoop = NSRunLoop.Main;
			if (!runLoop.RunUntil (startTask, timeout))
				return false;
			startTask.GetAwaiter ().GetResult (); // Trigger any captured exceptions.

			if (!runLoop.RunUntil (completionTask, timeout))
				return false;
			completionTask.GetAwaiter ().GetResult (); // Trigger any captured exceptions.
		} catch (ResultStateException) {
			// Don't capture any NUnit-related exceptions, those should bubble up and terminate the test accordingly.
			throw;
		} catch (Exception e) {
			exception = e;
			// We return 'true' here, because we didn't time out.
		}

		return true;
	}

#if CAN_SHOW_ASYNC_UI
	static IDisposable ShowAsyncUI (XImage? imageToShow = null)
	{
		var state = new AsyncState ();
		state.Show (imageToShow);
		return state;
	}

	class AsyncState : IDisposable {
#if HAS_UIKIT
		UIViewController? initialRootViewController;
		UIWindow? window;
		UINavigationController? navigation;
#else
		NSWindow? window;
#endif // HAS_UIKIT

		public void Show (XImage? imageToShow)
		{
			var vc = new AsyncController (imageToShow);

#if HAS_UIKIT
			window = UIApplication.SharedApplication.KeyWindow;
			initialRootViewController = window.RootViewController;
			navigation = initialRootViewController as UINavigationController;

			// Pushing something to a navigation controller doesn't seem to work on phones
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
				navigation = null;

			if (navigation is not null) {
				navigation.PushViewController (vc, false);
			} else {
				window.RootViewController = vc;
			}
#else
			var size = new CGRect (0, 0, 300, 300);
			var loc = new CGPoint ((NSScreen.MainScreen.Frame.Width - size.Width) / 2, (NSScreen.MainScreen.Frame.Height - size.Height) / 2);
			window = new NSWindow (size, NSWindowStyle.Closable | NSWindowStyle.Miniaturizable | NSWindowStyle.Resizable | NSWindowStyle.Titled, NSBackingStore.Retained, false);
			window.SetFrameOrigin (loc);
			window.ContentViewController = vc;
			window.MakeKeyAndOrderFront (null);
#endif // HAS_UIKIT
		}

		public void Hide ()
		{
			if (window is null)
				return;

#if HAS_UIKIT
			if (navigation is not null) {
				navigation.PopViewController (false);
			} else {
				window.RootViewController = initialRootViewController;
			}
#else
			window.Close ();
			window.Dispose ();
#endif // HAS_UIKIT

			window = null;
		}

		public void Dispose ()
		{
			Hide ();
		}
	}

	class AsyncController : XViewController {
		XImage? imageToShow;
		static int counter;

		public AsyncController (XImage? imageToShow = null)
		{
			this.imageToShow = imageToShow;
			counter++;
		}

#if !HAS_UIKIT
		public override void LoadView ()
		{
			View = new NSView ();
		}
#endif // !HAS_UIKIT

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			XColor backgroundColor;
			switch (counter % 2) {
			case 0:
				backgroundColor = XColor.Yellow;
				break;
			default:
				backgroundColor = XColor.LightGray;
				break;
			}

#if HAS_UIKIT
			View.BackgroundColor = backgroundColor;
#else
			View.WantsLayer = true;
			View.Layer.BackgroundColor = backgroundColor.CGColor;
#endif // HAS_UIKIT

			if (imageToShow is not null) {
				var imgView = new XImageView (View.Bounds);
				imgView.Image = imageToShow;
#if HAS_UIKIT
				imgView.ContentMode = UIViewContentMode.Center;
#endif // HAS_UIKIT
				View.AddSubview (imgView);
			}
		}
	}
#endif // CAN_SHOW_ASYNC_UI
}

namespace Foundation {
	public static class NSRunLoop_Extensions {
		// Returns true if task completed before the timeout,
		// otherwise returns false
		public static bool RunUntil (this NSRunLoop self, Task task, TimeSpan timeout)
		{
			var start = Stopwatch.StartNew ();
			while (true) {
				if (task.IsCompleted)
					return true;
				if (timeout <= start.Elapsed)
					return false;
				self.RunUntil (NSDate.Now.AddSeconds (0.1));
			}
		}
	}
}
