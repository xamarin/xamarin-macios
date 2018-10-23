using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using Foundation;

using NUnitLite.Runner;
using NUnit.Framework.Api;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.WorkItems;
using NUnit.Framework.Internal.Filters;

using NUnitTest = NUnit.Framework.Internal.Test;

namespace Xamarin.iOS.UnitTests.NUnit
{
	public class NUnitTestRunner : TestRunner, ITestListener
	{
		Dictionary<string, object> builderSettings;
		TestSuiteResult results;

		public ITestFilter Filter { get; set; } = TestFilter.Empty;
		public bool GCAfterEachFixture { get; set; }

		protected override string ResultsFileName { get; set; } = "TestResults.NUnit.xml";

		public NUnitTestRunner (LogWriter logger) : base (logger)
		{
			builderSettings = new Dictionary<string, object> (StringComparer.OrdinalIgnoreCase);
		}

		public override void Run (IList<TestAssemblyInfo> testAssemblies)
		{
			if (testAssemblies == null)
				throw new ArgumentNullException (nameof (testAssemblies));
			
			var builder = new NUnitLiteTestAssemblyBuilder ();
			var runner = new NUnitLiteTestAssemblyRunner (builder, new FinallyDelegate ());
			var testSuite = new TestSuite (NSBundle.MainBundle.BundleIdentifier);
			results = new TestSuiteResult (testSuite);

			foreach (TestAssemblyInfo assemblyInfo in testAssemblies) {
				if (assemblyInfo == null || assemblyInfo.Assembly == null)
					continue;
				
				if (!runner.Load (assemblyInfo.Assembly, builderSettings)) {
					OnWarning ($"Failed to load tests from assembly '{assemblyInfo.Assembly}");
					continue;
				}
				if (runner.LoadedTest is NUnitTest tests)
					testSuite.Add (tests);
				
				// Messy API. .Run returns ITestResult which is, in reality, an instance of TestResult since that's
				// what WorkItem returns and we need an instance of TestResult to add it to TestSuiteResult. So, cast
				// the return to TestResult and hope for the best.
				ITestResult result = null;
				try {
					OnAssemblyStart (assemblyInfo.Assembly);
					result = runner.Run (this, Filter);
				} finally {
					OnAssemblyFinish (assemblyInfo.Assembly);
				}

				if (result == null)
					continue;

				var testResult = result as TestResult;
				if (testResult == null)
					throw new InvalidOperationException ($"Unexpected test result type '{result.GetType ()}'");
				results.AddResult (testResult);
			}

			LogFailureSummary ();
		}

		public bool Pass (ITest test)
		{
			return true;
		}

		public void TestFinished (ITestResult result)
		{
			if (result.Test is TestSuite) {
				//if (!result.IsError && !result.IsFailure && !result.IsSuccess && !result.Executed)
				//Writer.WriteLine ("\t[INFO] {0}", result.Message);
				if (result.ResultState.Status != TestStatus.Failed &&
					result.ResultState.Status != TestStatus.Skipped &&
					result.ResultState.Status != TestStatus.Passed &&
					result.ResultState.Status != TestStatus.Inconclusive) {
						Logger.OnInfo ($"\t[INFO] {result.Message}");
				}
				
				string name = result.Test.Name;
				if (!String.IsNullOrEmpty (name))
					Logger.OnInfo ($"{name} : {result.Duration.TotalMilliseconds} ms\n");
					
				if (GCAfterEachFixture)
					GC.Collect ();
			} else {
				var sb = new StringBuilder ();
				switch (result.ResultState.Status) {
				case TestStatus.Passed:
					sb.Append ("\t[PASS] ");
					PassedTests++;
					break;
				case TestStatus.Skipped:
					sb.Append ("\t[IGNORED] ");
					SkippedTests++;
					break;
				case TestStatus.Failed:
					sb.Append ("\t[FAIL] ");
					FailedTests++;
					break;
				case TestStatus.Inconclusive:
					sb.Append ("\t[INCONCLUSIVE] ");
					InconclusiveTests++;
					break;
				default:
					sb.Append ("\t[INFO] ");
					break;
				}
				sb.Append (result.Test.FixtureType.Name);
				sb.Append (".");
				sb.Append (result.Test.Name);
				string message = result.Message;
				if (!string.IsNullOrEmpty (message)) {
					message = message.Replace ("\r\n", "\\r\\n");
					sb.Append ($" : {message}");
				}
				Logger.OnInfo (sb.ToString ());
				string stacktrace = result.StackTrace;
				if (!string.IsNullOrEmpty (result.StackTrace)) {
					string[] lines = stacktrace.Split (new char [] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
					foreach (string line in lines)
						Logger.OnInfo ($"\t\t{line}");
				}
				
				if (result.ResultState.Status == TestStatus.Failed) {
					FailureInfos.Add (new TestFailureInfo {
						TestName = result.Test.FullName,
						Message = sb.ToString ()
					});
				}
			}
			
		}

		public void TestOutput (TestOutput testOutput)
		{
			if (testOutput == null || String.IsNullOrEmpty (testOutput.Text))
				return;
			
			string kind = testOutput.Type.ToString ();
			foreach (string l in testOutput.Text.Split ('\n')) {
				Logger.OnInfo ($"  {kind}: {l}");
			}
		}

		public void TestStarted (ITest test)
		{
			if (test == null)
				return;

			if (!string.IsNullOrEmpty (TestsRootDirectory))
				Environment.CurrentDirectory = TestsRootDirectory;

			if (test is TestSuite) {
				Logger.OnInfo (test.Name);
			}
		}

		public override string WriteResultsToFile ()
		{
			if (results == null)
				return string.Empty;
				
			string ret = GetResultsFilePath ();
 			if (string.IsNullOrEmpty (ret))
 				return string.Empty;
 			
 				
 			var resultsXml = new NUnit2XmlOutputWriter (DateTime.UtcNow);
 			resultsXml.WriteResultFile (results, ret);

 			return ret;
		}
	}
}
