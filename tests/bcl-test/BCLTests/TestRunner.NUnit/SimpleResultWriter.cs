using System;
using System.IO;
using System.Net.Mime;
using NUnit.Framework.Api;
using NUnit.Framework.Internal;

namespace  Xamarin.iOS.UnitTests.NUnit {
	/// <summary>
	/// Simple writer that outputs the results as normal text.
	/// </summary>
	public class SimpleResultWriter {


		public void WriteResultFile (ITestResult r, TextWriter writer)
		{
			// writer the test results using the given writer

			TestResult result = r as TestResult;
			writer.WriteLine ("Hello!!");
			if (result.Test is TestSuite) {
				if (result.ResultState.Status != TestStatus.Failed &&
					result.ResultState.Status != TestStatus.Skipped &&
					result.ResultState.Status != TestStatus.Passed &&
					result.ResultState.Status != TestStatus.Inconclusive)
					writer.WriteLine ("\t[INFO] {0}", result.Message);

				string name = result.Test.Name;
				if (!string.IsNullOrEmpty (name))
					writer.WriteLine ($"{name} : {result.Duration.TotalMilliseconds} ms");
			} else {
				switch (result.ResultState.Status) {
				case TestStatus.Passed:
					writer.Write ("\t[PASS] ");
					break;
				case TestStatus.Skipped:
					writer.Write ("\t[IGNORED] ");
					break;
				case TestStatus.Failed:
					writer.Write ("\t[FAIL] ");
					break;
				case TestStatus.Inconclusive:
					writer.Write ("\t[INCONCLUSIVE] ");
					break;
				default:
					writer.Write ("\t[INFO] ");
					break;
				}
				writer.Write (result.Test.FixtureType.Name);
				writer.Write (".");
				writer.Write (result.Test.Name);

				var message = result.Message;
				if (!string.IsNullOrEmpty (message)) {
					writer.Write (" : {0}", message.Replace ("\r\n", "\\r\\n"));
				}
				writer.WriteLine ();

				string stacktrace = result.StackTrace;
				if (string.IsNullOrEmpty (result.StackTrace)) return;

				string [] lines = stacktrace.Split (new char [] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
				foreach (var line in lines)
					writer.WriteLine ("\t\t{0}", line);
			}
		}
	}
}
