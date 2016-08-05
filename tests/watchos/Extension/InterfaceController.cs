using System;
using System.Collections;
using System.Linq;
using System.Threading;

using WatchKit;
using Foundation;

using NUnit.Framework.Internal.Filters;
using MonoTouch.NUnit.UI;

public static partial class TestLoader
{
	static partial void AddTestAssembliesImpl (BaseTouchRunner runner);

	public static void AddTestAssemblies (BaseTouchRunner runner)
	{
		AddTestAssembliesImpl (runner);
	}
}

namespace monotouchtestWatchKitExtension
{
	[Register ("InterfaceController")]
	public partial class InterfaceController : WKInterfaceController
	{
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
			ObjCRuntime.Runtime.MarshalManagedException += (object sender, ObjCRuntime.MarshalManagedExceptionEventArgs args) =>
			{
				Console.WriteLine ("Managed exception: {0}", args.Exception);
			};
			ObjCRuntime.Runtime.MarshalObjectiveCException += (object sender, ObjCRuntime.MarshalObjectiveCExceptionEventArgs args) =>
			{
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
			runner = new WatchOSRunner ();
			runner.Filter = new NotFilter (new CategoryExpression ("MobileNotWorking,NotOnMac,NotWorking,ValueAdd,CAS,InetAccess,NotWorkingInterpreter,RequiresBSDSockets").Filter);
			runner.Add (GetType ().Assembly);
			TestLoader.AddTestAssemblies (runner);
			ThreadPool.QueueUserWorkItem ((v) =>
			{
				runner.LoadSync ();
				BeginInvokeOnMainThread (() =>
				{
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
			lblStatus.SetText ("Running");
			BeginInvokeOnMainThread (() => {
				runner.Run ();

				cmdRun.SetEnabled (true);
				lblStatus.SetText ("Done");
				running = false;
				RenderResults ();
			});
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

