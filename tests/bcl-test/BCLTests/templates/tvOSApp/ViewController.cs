using System;
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

namespace BCLTests {
	public partial class ViewController : UIViewController {

		internal static IEnumerable<TestAssemblyInfo> GetTestAssemblies ()
 		{
			// var t = Path.GetFileName (typeof (ActivatorCas).Assembly.Location);
			foreach (var name in RegisterType.TypesToRegister.Keys) {
				var a = Assembly.Load (name);
				if (a == null) {
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

		public async Task<string[]> GetSkippedTests ()
		{
			var ignoredFiles = new List<string> ();
			// the project generator added the required resources,
			// we extract them, parse them and add the result
			var executingAssembly = Assembly.GetExecutingAssembly ();
			foreach (var resourceName in executingAssembly.GetManifestResourceNames ()) {
				if (resourceName.EndsWith (".ignore", StringComparison.Ordinal)) {
					using (var stream = executingAssembly.GetManifestResourceStream(resourceName))
					using (var reader = new StreamReader (stream)) {
						var ignored = await IgnoreFileParser.ParseStream (reader);
						// we could have more than one file, lets add them
						ignoredFiles.AddRange (ignored);
					}
				}
			}
			return ignoredFiles.ToArray ();
		}
		
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
			Xamarin.iOS.UnitTests.TestRunner runner;
			if (RegisterType.IsXUnit)
				runner = new XUnitTestRunner (logger);
			else
				runner = new NUnitTestRunner (logger);

			var skippedTests = await GetSkippedTests ();
			if (skippedTests.Length > 0) {
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
