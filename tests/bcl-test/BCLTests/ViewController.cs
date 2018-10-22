using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

using UIKit;
using Foundation;

using Xamarin.iOS.UnitTests;
using Xamarin.iOS.UnitTests.NUnit;
using BCLTests.TestRunner.Core;
using System.Runtime.InteropServices;
using Xamarin.iOS.UnitTests.XUnit;

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
 		
		protected ViewController (IntPtr handle) : base (handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		[DllImport ("libc")]
		static extern void exit (int code);
		protected virtual void TerminateWithSuccess ()
		{
			// For WatchOS we're terminating the extension, not the watchos app itself.
			Console.WriteLine ("Exiting test run with success");
			exit (0);
		}
		
		// executes the nunit runner with the loaded test assemblies.
		public void ExecuteNUnitRunner (ApplicationOptions options, IEnumerable<TestAssemblyInfo> testAssemblies)
		{
			var runner = new NUnitTestRunner (new LogWriter ()); // log to the host port etc.. not to the console
			if (!string.IsNullOrEmpty(options.HostName))
				runner.Writer = new TcpTextWriter (options.HostName, options.HostPort); // this has to be passed via mtouch in the cmd
			runner.LogTag = "NUnit";
			runner.Run ((IList<TestAssemblyInfo>)testAssemblies);
			string resultsFilePath = runner.WriteResultsToFile ();

			Console.WriteLine ($"Passed: {runner.PassedTests}, Failed: {runner.FailedTests}, Skipped: {runner.SkippedTests}, Inconclusive: {runner.InconclusiveTests}, Total: {runner.TotalTests}, Filtered: {runner.FilteredTests}");
			Console.WriteLine ($"Results can be found {resultsFilePath}");
		}
		
		public void ExecutexUnitRunner (ApplicationOptions options, IEnumerable<TestAssemblyInfo> testAssemblies)
		{
			var runner = new XUnitTestRunner (new LogWriter ());
			if (!string.IsNullOrEmpty (options.HostName))
				runner.Writer = new TcpTextWriter (options.HostName, options.HostPort); // this has to be passed via mtouch in the cmd
			runner.ResultFileFormat = XUnitResultFileFormat.XunitV2; 
			runner.Run ((IList<TestAssemblyInfo>)testAssemblies);
			string resultsFilePath = runner.WriteResultsToFile ();

			Console.WriteLine ($"Passed: {runner.PassedTests}, Failed: {runner.FailedTests}, Skipped: {runner.SkippedTests}, Inconclusive: {runner.InconclusiveTests}, Total: {runner.TotalTests}, Filtered: {runner.FilteredTests}");
			Console.WriteLine ($"Results can be found {resultsFilePath}");
			
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			var options = ApplicationOptions.Current;
			// Perform any additional setup after loading the view, typically from a nib.
			var testAssemblies = GetTestAssemblies ();
			if (RegisterType.IsXUnit)
				ExecutexUnitRunner (options, testAssemblies);
			else
				ExecuteNUnitRunner (options, testAssemblies);

			if (options.TerminateAfterExecution)
				exit (0);

		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}
