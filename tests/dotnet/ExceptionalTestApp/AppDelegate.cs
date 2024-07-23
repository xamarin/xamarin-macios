using System;
using System.Runtime.InteropServices;

using Foundation;

namespace MySimpleApp {
	public class Program {
		static int Main (string [] args)
		{
			GC.KeepAlive (typeof (NSObject)); // prevent linking away the platform assembly

			var testCaseString = Environment.GetEnvironmentVariable ("EXCEPTIONAL_TEST_CASE");
			if (string.IsNullOrEmpty (testCaseString)) {
				Console.WriteLine ($"The environment variable EXCEPTIONAL_TEST_CASE wasn't set.");
				return 2;
			}
			var testCase = int.Parse (testCaseString);
			switch (testCase) {
			case 1:
				AppDomain.CurrentDomain.UnhandledException += (object sender, UnhandledExceptionEventArgs e) => {
					if (e.ExceptionObject is TestCaseException) {
						Console.WriteLine (Environment.GetEnvironmentVariable ("MAGIC_WORD"));
					} else {
						Console.WriteLine ($"Unexpected exception type: {e.ExceptionObject?.GetType ()}");
					}
					Environment.Exit (0);
				};
				throw new TestCaseException ();
			default:
				Console.WriteLine ($"Unknown test case: {testCase}");
				return 3;
			}

			return 1;
		}
	}
}

class TestCaseException : Exception {
	public TestCaseException ()
		: base ("Testing, testing")
	{
	}
}
