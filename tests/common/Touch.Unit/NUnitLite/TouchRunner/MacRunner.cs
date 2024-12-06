using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

using AppKit;
using Foundation;
using CoreGraphics;

namespace MonoTouch.NUnit.UI {
	public class MacRunner : BaseTouchRunner {
		// The exitProcess callback must not return. The boolean parameter specifies whether the test run succeeded or not.
		public static async Task<int> MainAsync (IList<string> arguments, bool requiresNSApplicationRun, Action<int> exitProcess, params Assembly[] assemblies)
		{
			var success = false;

			NSApplication.Init ();

			var options = new TouchOptions (arguments);
			TouchOptions.Current = options;

			if (requiresNSApplicationRun) {
				var app = NSApplication.SharedApplication;
				// This window will let us stop the NSApplication's run loop after the test run
				app.InvokeOnMainThread (async () => {
					var rect = new CGRect (0, 0, 200, 50);
					var window = new NSWindow (rect, NSWindowStyle.Titled | NSWindowStyle.Closable | NSWindowStyle.Miniaturizable | NSWindowStyle.Resizable, NSBackingStore.Buffered, false);
					window.Title = "Running tests...";
					window.MakeKeyAndOrderFront (app);

					// run the tests
					success = await RunTestsAsync (options, assemblies);

					app.BeginInvokeOnMainThread (() => {
						// First try to stop the app nicely. This only works if the app is showing a window.
						app.Stop (app);
						// Stopping the run loop only works if something else is processed by the run loop, so add an event.
						app.PostEvent (NSEvent.OtherEvent (NSEventType.ApplicationDefined, CGPoint.Empty, (NSEventModifierMask) 0, 0, 0, null, 0, 0, 0), true);
						// And try something else too in case the application-defined event doesn't work.
						app.AbortModal ();
					});

					// However, let's create a fallback and forcefully exit the app if being nice didn't work.
					var timeout = 3;
					var timer = NSTimer.CreateScheduledTimer (timeout, (v) => {
						Console.WriteLine ($"The app didn't exit its run loop within {timeout} seconds, will now forcefully exit.");
						exitProcess (success ? 0 : 1);
					});
				});
				app.Run ();
			} else {
				success = await RunTestsAsync (options, assemblies);
			}

			return success ? 0 : 1;
		}

		static async Task<bool> RunTestsAsync (TouchOptions options, Assembly[] assemblies)
		{
			var runner = new MacRunner ();
			if (assemblies == null || assemblies.Length == 0)
				assemblies = AppDomain.CurrentDomain.GetAssemblies ();

			foreach (var asm in assemblies)
				runner.Load (asm);

			await runner.RunAsync ();

			return !runner.Result.IsFailure ();
		}

		protected override void WriteDeviceInformation (TextWriter writer)
		{
			var piinfo = NSProcessInfo.ProcessInfo;
			writer.WriteLine ("[macOS: {0}]", piinfo.OperatingSystemVersionString);
		}
	}
}
