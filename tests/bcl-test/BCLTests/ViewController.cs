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

		static string testsFolderName = "Tests";
		
		internal static IEnumerable<TestAssemblyInfo> GetTestAssemblies ()
 		{
			// look in the app bundle data folder, grap all the assemblies
			// load them and return them accordingly
			var dataPath = Path.Combine (NSBundle.MainBundle.BundlePath, testsFolderName);
			if (!Directory.Exists (dataPath))
				throw new InvalidOperationException ("Test could not be loaded because they were not added in the app bundle.");
			foreach (var f in Directory.GetFiles (dataPath)) {
				var a = Assembly.LoadFile (f);
				if (a == null) {
					Console.WriteLine ($"# WARNING: Unable to load assembly: {f}");
 					continue;
				}
				yield return new TestAssemblyInfo (a, f);
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
			runner.Run ((System.Collections.Generic.IList<Xamarin.iOS.UnitTests.TestAssemblyInfo>)testAssemblies);
			
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}
