using System;
using System.Collections.Generic;
using System.Threading;

using WatchKit;
using Foundation;

using MonoTouch.NUnit.UI;

public static partial class TestLoader {
	static partial void AddTestAssembliesImpl (BaseTouchRunner runner);

	public static void AddTestAssemblies (BaseTouchRunner runner)
	{
		AddTestAssembliesImpl (runner);
	}
}

namespace monotouchtestWatchKitExtension {
	[Register ("InterfaceController")]
	public partial class InterfaceController : WKInterfaceController {
		WatchOSRunner runner;
		bool running;

		[Action ("runTests:")]
		partial void RunTests (NSObject obj);

		[Outlet ("lblStatus")]
		WatchKit.WKInterfaceLabel lblStatus { get; set; }

		[Outlet ("lblSuccess")]
		WatchKit.WKInterfaceLabel lblSuccess { get; set; }

		[Outlet ("lblFailed")]
		WatchKit.WKInterfaceLabel lblFailed { get; set; }

		[Outlet ("lblIgnInc")]
		WatchKit.WKInterfaceLabel lblIgnInc { get; set; }

		[Outlet ("cmdRun")]
		WatchKit.WKInterfaceButton cmdRun { get; set; }

		static InterfaceController ()
		{
			ObjCRuntime.Runtime.MarshalManagedException += (object sender, ObjCRuntime.MarshalManagedExceptionEventArgs args) => {
				Console.WriteLine ("Managed exception: {0}", args.Exception);
			};
			ObjCRuntime.Runtime.MarshalObjectiveCException += (object sender, ObjCRuntime.MarshalObjectiveCExceptionEventArgs args) => {
				Console.WriteLine ("Objective-C exception: {0}", args.Exception);
			};
		}

		public InterfaceController (IntPtr handle) : base (handle)
		{
		}

		public override void Awake (NSObject context)
		{
			base.Awake (context);

			BeginInvokeOnMainThread (LoadTests);
		}

		void LoadTests ()
		{
			var excludeCategories = new [] {
				"MobileNotWorking",
				"NotOnMac",
				"NotWorking",
				"ValueAdd",
				"CAS",
				"InetAccess",
				"NotWorkingLinqInterpreter",
				"RequiresBSDSockets",
				"BitcodeNotSupported",
			};
			runner = new WatchOSRunner ();
			runner.ExcludedCategories = new HashSet<string> (excludeCategories);
			runner.Add (GetType ().Assembly);
			TestLoader.AddTestAssemblies (runner);
			ThreadPool.QueueUserWorkItem ((v) => {
				runner.LoadSync ();
				BeginInvokeOnMainThread (() => {
					lblStatus.SetText (string.Format ("{0} tests", runner.TestCount));
					RenderResults ();
					cmdRun.SetEnabled (true);
					cmdRun.SetHidden (false);

					runner.AutoRun ();
				});
			});
		}

		void RunTests ()
		{
			if (running) {
				Console.WriteLine ("Already running");
				return;
			}
			running = true;
			cmdRun.SetEnabled (false);
			lblStatus.SetText ("Running in background");

			var timer = NSTimer.CreateRepeatingScheduledTimer (TimeSpan.FromSeconds (1), (v) => RenderResults ());
			var runnerThread = new Thread (() => {
				runner.Run ();

				InvokeOnMainThread (() => {
					cmdRun.SetEnabled (true);
					lblStatus.SetText ("Done");
					running = false;
					timer.Dispose ();
					RenderResults ();
				});
			}) {
				IsBackground = true,
			};
			runnerThread.Start ();
		}

		void RenderResults ()
		{
			if (runner.TestCount == 0)
				return;

			lblSuccess.SetText (string.Format ("P: {0}/{1} {2}%", runner.PassedCount, runner.TestCount, 100 * runner.PassedCount / runner.TestCount));
			lblFailed.SetText (string.Format ("F: {0}/{1} {2}%", runner.FailedCount, runner.TestCount, 100 * runner.FailedCount / runner.TestCount));
			lblIgnInc.SetText (string.Format ("I: {0}/{1} {2}%", (runner.IgnoredCount + runner.InconclusiveCount), runner.TestCount, 100 * (runner.IgnoredCount + runner.InconclusiveCount) / runner.TestCount));

			if (running == false && runner.PassedCount > 0) {
				if (runner.FailedCount == 0) {
					lblSuccess.SetTextColor (UIKit.UIColor.Green);
					lblStatus.SetTextColor (UIKit.UIColor.Green);
					lblStatus.SetText ("Success");
				}
				if (runner.FailedCount > 0) {
					lblFailed.SetTextColor (UIKit.UIColor.Red);
					lblStatus.SetTextColor (UIKit.UIColor.Red);
					lblStatus.SetText ("Failed");
				}
			}
		}

		partial void RunTests (NSObject obj)
		{
			RunTests ();
		}
	}
}
