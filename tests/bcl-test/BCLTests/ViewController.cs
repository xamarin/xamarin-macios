using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

using UIKit;
using Foundation;

using Xamarin.iOS.UnitTests;
using Xamarin.iOS.UnitTests.NUnit;

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

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.
			var testAssemblies = GetTestAssemblies ();
			foreach (var a in testAssemblies)
				Console.WriteLine (a.FullPath);
			Console.WriteLine ("Assemblies loaded.");
			var runner = new NUnitTestRunner (new LogWriter ());
			runner.LogTag = "NUnit";
			runner.Run ((IList<TestAssemblyInfo>)testAssemblies);
			string resultsFilePath = runner.WriteResultsToFile ();

			Console.WriteLine ($"Passed: {runner.PassedTests}, Failed: {runner.FailedTests}, Skipped: {runner.SkippedTests}, Inconclusive: {runner.InconclusiveTests}, Total: {runner.TotalTests}, Filtered: {runner.FilteredTests}");
			Console.WriteLine ($"Results can be found {resultsFilePath}");

		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}
