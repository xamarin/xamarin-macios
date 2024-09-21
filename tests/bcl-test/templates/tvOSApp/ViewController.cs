using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using UIKit;
using ObjCRuntime;

using Xamarin.iOS.UnitTests;
using Xamarin.iOS.UnitTests.NUnit;
using BCLTests.TestRunner.Core;
using Xamarin.iOS.UnitTests.XUnit;
using System.Threading.Tasks;
using System.IO;
using Foundation;
using NUnit.Framework.Internal.Filters;

namespace BCLTests {
	public partial class ViewController : UIViewController {

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
			if (!string.IsNullOrEmpty (options.HostName)) {
				try {
					writer = new TcpTextWriter (options.HostName, options.HostPort, options.UseTcpTunnel);
				} catch (Exception ex) {
					Console.WriteLine ("Network error: Cannot connect to {0}:{1}: {2}. Continuing on console.", options.HostName, options.HostPort, ex);
					writer = null; // will default to the console
				}
			}

			// we generate the logs in two different ways depending if the generate xml flag was
			// provided. If it was, we will write the xml file to the tcp writer if present, else
			// we will write the normal console output using the LogWriter
			var logger = (writer is null || options.EnableXml) ? new LogWriter () : new LogWriter (writer);
			logger.MinimumLogLevel = MinimumLogLevel.Info;
			var testAssemblies = GetTestAssemblies ();
			var runner = RegisterType.IsXUnit ? (Xamarin.iOS.UnitTests.TestRunner) new XUnitTestRunner (logger) : new NUnitTestRunner (logger);
			var categories = await IgnoreFileParser.ParseTraitsContentFileAsync (NSBundle.MainBundle.BundlePath, RegisterType.IsXUnit);
			// add category filters if they have been added
			runner.SkipCategories (categories);

			// if we have ignore files, ignore those tests
			var skippedTests = await IgnoreFileParser.ParseContentFilesAsync (NSBundle.MainBundle.BundlePath);
			if (skippedTests.Any ()) {
				// ensure that we skip those tests that have been passed via the ignore files
				runner.SkipTests (skippedTests);
			}

			await runner.Run (testAssemblies).ConfigureAwait (false);
			Xamarin.iOS.UnitTests.TestRunner.Jargon jargon = Xamarin.iOS.UnitTests.TestRunner.Jargon.NUnitV3;
			switch (options.XmlVersion) {
			default:
			case XmlVersion.NUnitV2:
				jargon = Xamarin.iOS.UnitTests.TestRunner.Jargon.NUnitV2;
				break;
			case XmlVersion.NUnitV3:
				jargon = Xamarin.iOS.UnitTests.TestRunner.Jargon.NUnitV3;
				break;
			}
			if (options.EnableXml) {
				runner.WriteResultsToFile (writer ?? Console.Out, jargon);
				logger.Info ("Xml file was written to the tcp listener.");
			} else {
				string resultsFilePath = runner.WriteResultsToFile (jargon);
				logger.Info ($"Xml result can be found {resultsFilePath}");
			}

			logger.Info ($"Tests run: {runner.TotalTests} Passed: {runner.PassedTests} Inconclusive: {runner.InconclusiveTests} Failed: {runner.FailedTests} Ignored: {runner.FilteredTests}");
			if (options.TerminateAfterExecution)
				BeginInvokeOnMainThread (TerminateWithSuccess);

		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}
