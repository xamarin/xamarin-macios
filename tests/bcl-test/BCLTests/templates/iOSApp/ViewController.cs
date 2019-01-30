using System;
using System.Reflection;
using System.Collections.Generic;

using UIKit;
using ObjCRuntime;

using Xamarin.iOS.UnitTests;
using Xamarin.iOS.UnitTests.NUnit;
using BCLTests.TestRunner.Core;
using Xamarin.iOS.UnitTests.XUnit;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using Foundation;
using NUnit.Framework.Internal.Filters;

namespace BCLTests {
	public partial class ViewController : UIViewController {

		internal static IEnumerable<TestAssemblyInfo> GetTestAssemblies ()
 		{
			// var t = Path.GetFileName (typeof (ActivatorCas).Assembly.Location);
			foreach (var name in RegisterType.TypesToRegister.Keys) {
				var a = RegisterType.TypesToRegister [name].Assembly;
				if (a == null) {
					Console.WriteLine ($"# WARNING: Unable to load assembly {name}.");
 					continue;
				} else {
					Console.WriteLine ($"Loading assembly: {name}.");
				}
				yield return new TestAssemblyInfo (a, name);
			}
 		}
 		
 		public ViewController ()
		{
		}
		
		protected ViewController (IntPtr handle) : base (handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

#if __WATCH__
		[DllImport ("libc")]
		static extern void exit (int code);
		protected virtual void TerminateWithSuccess ()
		{
			// For WatchOS we're terminating the extension, not the watchos app itself.
			Console.WriteLine ("Exiting test run with success");
			exit (0);
		}
#else
		protected virtual void TerminateWithSuccess ()
		{
			// For WatchOS we're terminating the extension, not the watchos app itself.
			Console.WriteLine ("Exiting test run with success");
			Selector s = new Selector ("terminateWithSuccess");
			UIApplication.SharedApplication.PerformSelector (s, UIApplication.SharedApplication, 0);
		}
#endif

		public async override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			var options = ApplicationOptions.Current;
			TcpTextWriter writer = null;
			if (!string.IsNullOrEmpty (options.HostName))
				writer = new TcpTextWriter (options.HostName, options.HostPort);

			// we generate the logs in two different ways depending if the generate xml flag was
			// provided. If it was, we will write the xml file to the tcp writer if present, else
			// we will write the normal console output using the LogWriter
			var logger = (writer == null || options.EnableXml) ? new LogWriter () : new LogWriter (writer);
			logger.MinimumLogLevel = MinimumLogLevel.Info;
			var testAssemblies = GetTestAssemblies ();
			var runner = RegisterType.IsXUnit ? (Xamarin.iOS.UnitTests.TestRunner) new XUnitTestRunner (logger) : new NUnitTestRunner (logger);
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
			var skippedTests = await IgnoreFileParser.ParseContentFilesAsync (NSBundle.MainBundle.BundlePath);
			if (skippedTests.Any ()) {
				// ensure that we skip those tests that have been passed via the ignore files
				runner.SkipTests (skippedTests);
			}

			runner.Run ((IList<TestAssemblyInfo>)testAssemblies);
			if (options.EnableXml) {
				runner.WriteResultsToFile (writer);
				logger.Info ("Xml file was written to the tcp listener.");
			} else {
				string resultsFilePath = runner.WriteResultsToFile ();
				logger.Info ($"Xml result can be found {resultsFilePath}");
			}
			
			logger.Info ($"Tests run: {runner.TotalTests} Passed: {runner.PassedTests} Inconclusive: {runner.InconclusiveTests} Failed: {runner.FailedTests} Ignored: {runner.SkippedTests}");
			if (options.TerminateAfterExecution)
				TerminateWithSuccess ();

		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}
