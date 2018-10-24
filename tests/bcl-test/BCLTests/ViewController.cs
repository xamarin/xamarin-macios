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
 		
 		public ViewController ()
		{
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

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			var options = ApplicationOptions.Current;
			TcpTextWriter writer = null;
			if (!string.IsNullOrEmpty (options.HostName))
				writer = new TcpTextWriter (options.HostName, options.HostPort);
			var logger = (writer == null) ? new LogWriter () : new LogWriter (writer);
			logger.MinimumLogLevel = MinimumLogLevel.Info;
			// Perform any additional setup after loading the view, typically from a nib.
			var testAssemblies = GetTestAssemblies ();
			Xamarin.iOS.UnitTests.TestRunner runner;
			if (RegisterType.IsXUnit)
				runner = new XUnitTestRunner (logger);
			else
				runner = new NUnitTestRunner (logger);
			
			runner.Run ((IList<TestAssemblyInfo>)testAssemblies);
			string resultsFilePath = runner.WriteResultsToFile ();
			logger.Info ($"Xml result can be found {resultsFilePath}");
			logger.Info ($"Tests run: {runner.TotalTests} Passed: {runner.PassedTests} Inconclusive: {runner.InconclusiveTests} Failed: {runner.FailedTests} Ignored: {runner.SkippedTests}");

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
