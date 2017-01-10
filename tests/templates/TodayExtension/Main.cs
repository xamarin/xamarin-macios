using System;

using NotificationCenter;
using Foundation;
using UIKit;

using MonoTouch.NUnit.UI;
using NUnit.Framework;
using NUnit.Framework.Internal;

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
		System.Threading.ThreadPool.QueueUserWorkItem ((v) =>
		{
			runner.LoadSync ();
			BeginInvokeOnMainThread (() =>
			{
				runner.AutoRun ();
			});
		});

		completionHandler (NCUpdateResult.NewData);
	}
}
