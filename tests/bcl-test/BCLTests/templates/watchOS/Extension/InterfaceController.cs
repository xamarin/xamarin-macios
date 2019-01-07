using System;
using System.Collections;
using System.Linq;
using System.Threading;

using WatchKit;
using Foundation;

using NUnit.Framework.Internal.Filters;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Xamarin.iOS.UnitTests;
using System.Reflection;
using Xamarin.iOS.UnitTests.XUnit;
using Xamarin.iOS.UnitTests.NUnit;
using BCLTests;
using BCLTests.TestRunner.Core;
using System.IO;
using System.Threading.Tasks;

namespace monotouchtestWatchKitExtension
{
	[Register ("InterfaceController")]
	public partial class InterfaceController : WKInterfaceController
	{
		bool running;
		Xamarin.iOS.UnitTests.TestRunner runner;

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
		
		[DllImport ("libc")]
		static extern void exit (int code);
		protected virtual void TerminateWithSuccess ()
		{
			// For WatchOS we're terminating the extension, not the watchos app itself.
			Console.WriteLine ("Exiting test run with success");
			exit (0);
		}

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

			BeginInvokeOnMainThread (RunTests);
		}

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
 		
		void RunTests ()
		{
			var options = ApplicationOptions.Current;
			TextWriter writer = null;
			if (!string.IsNullOrEmpty (options.HostName) && string.IsNullOrEmpty(options.LogFile))
				writer = new HttpTextWriter () { HostName = options.HostName, Port = options.HostPort };
			if (!string.IsNullOrEmpty (options.LogFile))
				writer = new StreamWriter (options.LogFile);

			// we generate the logs in two different ways depending if the generate xml flag was
			// provided. If it was, we will write the xml file to the tcp writer if present, else
			// we will write the normal console output using the LogWriter
			var logger = (writer == null || options.EnableXml) ? new LogWriter () : new LogWriter (writer);
			logger.MinimumLogLevel = MinimumLogLevel.Info;
			var testAssemblies = GetTestAssemblies ();
			if (RegisterType.IsXUnit)
				runner = new XUnitTestRunner (logger);
			else
				runner = new NUnitTestRunner (logger);
			
			var skippedTests = IgnoreFileParser.ParseContentFiles (NSBundle.MainBundle.BundlePath);
			if (skippedTests.Any ()) {
				// ensure that we skip those tests that have been passed via the ignore files
				runner.SkipTests (skippedTests);
			}
			
			ThreadPool.QueueUserWorkItem ((v) =>
			{
				BeginInvokeOnMainThread (() =>
				{
					lblStatus.SetText (string.Format ("{0} tests", runner.TotalTests));
					runner.Run ((IList<TestAssemblyInfo>)testAssemblies);
					RenderResults ();
					cmdRun.SetEnabled (true);
					cmdRun.SetHidden (false);
					if (options.EnableXml) {
						runner.WriteResultsToFile (writer);
						logger.Info ("Xml file was written to the http listener.");
					} else {
						string resultsFilePath = runner.WriteResultsToFile ();
						logger.Info ($"Xml result can be found {resultsFilePath}");
					}
					logger.Info ($"Tests run: {runner.TotalTests} Passed: {runner.PassedTests} Inconclusive: {runner.InconclusiveTests} Failed: {runner.FailedTests} Ignored: {runner.SkippedTests}");
					if (options.TerminateAfterExecution)
						TerminateWithSuccess ();
				});
			});
		}

		void RenderResults ()
		{
			var options = ApplicationOptions.Current;

			if (runner.TotalTests == 0)
				return;

			lblSuccess.SetText (string.Format ("P: {0}/{1} {2}%", runner.PassedTests, runner.TotalTests, 100 * runner.PassedTests / runner.TotalTests));
			lblFailed.SetText (string.Format ("F: {0}/{1} {2}%", runner.FailedTests, runner.TotalTests, 100 * runner.FailedTests / runner.TotalTests));
			lblIgnInc.SetText (string.Format ("I: {0}/{1} {2}%", (runner.SkippedTests + runner.InconclusiveTests), runner.TotalTests, 100 * (runner.SkippedTests + runner.InconclusiveTests) / runner.TotalTests));

			if (running == false && runner.PassedTests > 0) {
				if (runner.FailedTests == 0) {
					lblSuccess.SetTextColor (UIKit.UIColor.Green);
					lblStatus.SetTextColor (UIKit.UIColor.Green);
					lblStatus.SetText ("Success");
				}
				if (runner.FailedTests > 0) {
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

class NameStartsWithFilter : NUnit.Framework.Internal.TestFilter
{
	public char FirstChar;
	public char LastChar;

	public override bool Match (NUnit.Framework.Api.ITest test)
	{
		if (test is NUnit.Framework.Internal.TestAssembly)
			return true;

		var method = test as NUnit.Framework.Internal.TestMethod;
		if (method != null)
			return Match (method.Parent);
		
		var name = !string.IsNullOrEmpty (test.Name) ? test.Name : test.FullName;
		bool rv;
		if (string.IsNullOrEmpty (name)) {
			rv = true;
		} else {
			var z = Char.ToUpperInvariant (name [0]);
			rv = z >= Char.ToUpperInvariant (FirstChar) && z <= Char.ToUpperInvariant (LastChar);
		}

		return rv;
	}

	public override bool Pass (NUnit.Framework.Api.ITest test)
	{
		return Match (test);
	}
}
