using System;
using System.Linq;
using System.Threading;
using Foundation;
using System.Collections.Generic;
using Xamarin.iOS.UnitTests;
using System.Reflection;
using Xamarin.iOS.UnitTests.XUnit;
using Xamarin.iOS.UnitTests.NUnit;
using BCLTests;
using System.IO;
using NotificationCenter;
using UIKit;

[Register ("TodayViewController")]
public partial class TodayViewController : UIViewController, INCWidgetProviding {
	bool running;
	TestRunner runner;

	protected TodayViewController (IntPtr handle) : base (handle) { }

	internal static IEnumerable<TestAssemblyInfo> GetTestAssemblies ()
	{
		// var t = Path.GetFileName (typeof (ActivatorCas).Assembly.Location);
		foreach (var name in RegisterType.TypesToRegister.Keys) {
			var a = Assembly.Load (name);
			if (a is null) {
				Console.WriteLine ($"# WARNING: Unable to load assembly {name}.");
				continue;
			}
			yield return new TestAssemblyInfo (a, name);
		}
	}

	[Export ("widgetPerformUpdateWithCompletionHandler:")]
	public void WidgetPerformUpdate (Action<NCUpdateResult> completionHandler)
	{
		var options = ApplicationOptions.Current;
		TextWriter writer = null;
		if (!string.IsNullOrEmpty (options.LogFile))
			writer = new StreamWriter (options.LogFile);

		// we generate the logs in two different ways depending if the generate xml flag was
		// provided. If it was, we will write the xml file to the tcp writer if present, else
		// we will write the normal console output using the LogWriter
		var logger = (writer is null || options.EnableXml) ? new LogWriter () : new LogWriter (writer);
		logger.MinimumLogLevel = MinimumLogLevel.Info;
		var testAssemblies = GetTestAssemblies ();
		runner = RegisterType.IsXUnit ? (TestRunner) new XUnitTestRunner (logger) : new NUnitTestRunner (logger);
		var categories = RegisterType.IsXUnit ?
			new List<string> {
				"failing",
				"nonmonotests",
				"outerloop",
				"nonosxtests"
			} :
			new List<string> {
				"MobileNotWorking",
				"NotOnMac",
				"NotWorking",
				"ValueAdd",
				"CAS",
				"InetAccess",
				"NotWorkingLinqInterpreter",
				"BitcodeNotSupported",
			};

		if (RegisterType.IsXUnit) {
			// special case when we are using the xunit runner,
			// there is a trait we are not interested in which is 
			// the Benchmark one
			var xunitRunner = runner as XUnitTestRunner;
			xunitRunner.AddFilter (XUnitFilter.CreateTraitFilter ("Benchmark", "true", true));
		}
		// add category filters if they have been added
		runner.SkipCategories (categories);

		// if we have ignore files, ignore those tests
		var skippedTests = IgnoreFileParser.ParseContentFiles (NSBundle.MainBundle.BundlePath);
		if (skippedTests.Any ()) {
			// ensure that we skip those tests that have been passed via the ignore files
			runner.SkipTests (skippedTests);
		}

		ThreadPool.QueueUserWorkItem ((v) => {
			BeginInvokeOnMainThread (async () => {
				await runner.Run (testAssemblies).ConfigureAwait (false); ;
			});
		});

		completionHandler (NCUpdateResult.NewData);
	}
}
