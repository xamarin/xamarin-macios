using System;

using Foundation;
using AppKit;
using System.IO;

namespace ServiceModel_Test {
	public partial class AppDelegate : NSApplicationDelegate {
		public AppDelegate ()
		{
		}

		public override void DidFinishLaunching (NSNotification notification)
		{
			var fileName = "../../../../../TestResult.txt";
			if (File.Exists (fileName))
				File.Delete (fileName);

			using (TextWriter writer = File.CreateText (fileName)) {
				var x = new System.ServiceModel.CommunicationException ();
				writer.WriteLine ("Test Passed: " + x.ToString ());
			}

			Environment.Exit (0);
		}
	}
}
