using System;
using System.Collections;
using System.Linq;
using System.Threading;

using WatchKit;
using Foundation;

using NUnit.Framework.Internal.Filters;
using MonoTouch.NUnit.UI;

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

		[Outlet ("lblIgnored")]
		WatchKit.WKInterfaceLabel lblIgnored { get; set; }

		[Outlet ("lblInconclusive")]
		WatchKit.WKInterfaceLabel lblInconclusive { get; set; }

		[Outlet ("cmdRun")]
		WatchKit.WKInterfaceButton cmdRun { get; set; }

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
			runner.Filter = new NotFilter (new CategoryExpression ("MobileNotWorking,NotOnMac,NotWorking,ValueAdd,CAS,InetAccess,NotWorkingInterpreter").Filter);
			runner.Add (GetType ().Assembly);
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
				BeginInvokeOnMainThread (RenderResults);
				running = false;
			});
		}

		void RenderResults ()
		{
			lblSuccess.SetText (string.Format ("Passed: {0}/{1} {2}%", runner.PassedCount, runner.TestCount, 100 * runner.PassedCount / runner.TestCount));
			lblFailed.SetText (string.Format ("Failed: {0}/{1} {2}%", runner.FailedCount, runner.TestCount, 100 * runner.FailedCount / runner.TestCount));
			lblIgnored.SetText (string.Format ("Ignored: {0}/{1} {2}%", runner.IgnoredCount, runner.TestCount, 100 * runner.IgnoredCount / runner.TestCount));
			lblInconclusive.SetText (string.Format ("Inconclusive: {0}/{1} {2}%", runner.InconclusiveCount, runner.TestCount, 100 * runner.InconclusiveCount / runner.TestCount));
		}

		partial void RunTests (NSObject obj)
		{
			RunTests ();
		}
	}
}

