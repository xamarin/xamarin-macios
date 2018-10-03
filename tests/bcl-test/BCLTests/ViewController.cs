using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

using UIKit;
using Foundation;

using Xamarin.iOS.UnitTests;
using Xamarin.iOS.UnitTests.NUnit;
using MonoCasTests.System;

namespace BCLTests {
	public partial class ViewController : UIViewController {

		static List<string> NUnitTests = new List <string> {
			"MONOTOUCH_System_test.dll",
			"MONOTOUCH_corlib_test.dll",
		};


		internal static IEnumerable<TestAssemblyInfo> GetTestAssemblies ()
 		{
			// var t = Path.GetFileName (typeof (ActivatorCas).Assembly.Location);
			foreach (var f in NUnitTests) {
				var a = Assembly.Load (f);
				if (a == null) {
					Console.WriteLine ($"# WARNING: Unable to load assembly.");
 					continue;
				}
				yield return new TestAssemblyInfo (a, "MONOTOUCH_System_test.dll");
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
			Console.WriteLine ($"Total tests {runner.TotalTests}");
			Console.WriteLine ($"Failed tests {runner.FailedTests}");
			Console.WriteLine ($"Success tests {runner.PassedTests}");
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}
