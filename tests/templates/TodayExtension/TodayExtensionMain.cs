using System;

using NotificationCenter;
using Foundation;
using UIKit;

using MonoTouch.NUnit.UI;
using NUnit.Framework;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Filters;

[Register ("TodayViewController")]
public partial class TodayViewController : UIViewController, INCWidgetProviding
{
	ConsoleRunner runner;

	protected TodayViewController (IntPtr handle) : base (handle) {}

	[Export ("widgetPerformUpdateWithCompletionHandler:")]
	public void WidgetPerformUpdate (Action<NCUpdateResult> completionHandler)
	{
		runner = new ConsoleRunner ();
		runner.Add (System.Reflection.Assembly.GetExecutingAssembly ());
		runner.Filter = new NotFilter (new CategoryExpression ("MobileNotWorking,NotOnMac,NotWorking,ValueAdd,CAS,InetAccess,NotWorkingLinqInterpreter").Filter);
		System.Threading.ThreadPool.QueueUserWorkItem ((v) =>
		{
			runner.LoadSync ();
			BeginInvokeOnMainThread (() =>
			{
				runner.AutoStart = true; // There's no UI for today extensions (yet), so unless we autostart the tests, there's no way to run them when running from the IDE.
				runner.AutoRun ();
			});
		});

		completionHandler (NCUpdateResult.NewData);
	}
}
