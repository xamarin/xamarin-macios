using System;
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
						Logger.OnInfo ("\t[INFO] {0}", result.Message);
				}

				Logger.OnInfo (LogTag, $"{result.Test.FullName} : {result.Duration.TotalMilliseconds} ms");
				if (GCAfterEachFixture)
					GC.Collect ();
			} else {
				Action<string, string> log = Logger.OnInfo;
				StringBuilder failedMessage = null;

				ExecutedTests++;
				if (result.ResultState.Status == TestStatus.Passed) {
					Logger.OnInfo (LogTag, $"\t{result.ResultState.ToString ()}");
					PassedTests++;
				} else if (result.ResultState.Status == TestStatus.Failed) {
					Logger.OnError (LogTag, "\t[FAIL]");
					log = Logger.OnError;
					failedMessage = new StringBuilder ();
					failedMessage.Append (result.Test.FullName);
					if (result.Test.FixtureType != null)
						failedMessage.Append ($" ({result.Test.FixtureType.Assembly.GetName ().Name})");
					failedMessage.AppendLine ();
					FailedTests++;
				} else {
					string status;
					switch (result.ResultState.Status) {
						case TestStatus.Skipped:
							SkippedTests++;
							status = "SKIPPED";
							break;

						case TestStatus.Inconclusive:
							InconclusiveTests++;
							status = "INCONCLUSIVE";
							break;

						default:
							status = "UNKNOWN";
							break;
					}
					Logger.OnInfo (LogTag, $"\t[{status}]");
				}

				string message = result.Message?.Replace ("\r\n", "\\r\\n");
				if (!String.IsNullOrEmpty (message)) {
					log (LogTag, $" : {message}");
					if (failedMessage != null)
						failedMessage.AppendLine (message);
				}

				string stacktrace = result.StackTrace;
				if (!String.IsNullOrEmpty (result.StackTrace)) {
					log (LogTag, result.StackTrace);
					if (failedMessage != null) {
						failedMessage.AppendLine ();
						failedMessage.AppendLine (result.StackTrace);
					}
				}

				if (failedMessage != null) {
					FailureInfos.Add (new TestFailureInfo {
						TestName = result.Test.FullName,
						Message = failedMessage.ToString ()
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
				Logger.OnInfo (LogTag, $"  {kind}: {l}");
			}
		}

		public void TestStarted (ITest test)
		{
			if (test == null)
				return;

			if (!String.IsNullOrEmpty (TestsRootDirectory))
				System.Environment.CurrentDirectory = TestsRootDirectory;

			if (test is TestSuite) {
				Logger.OnInfo (LogTag, test.Name);
			} else
				Logger.OnInfo (LogTag, $"{test.Name} ");
		}

		public override string WriteResultsToFile ()
		{
			if (results == null)
				return String.Empty;

			string ret = GetResultsFilePath ();
			if (String.IsNullOrEmpty (ret))
				return String.Empty;
			
			var resultsXml = new NUnit2XmlOutputWriter (DateTime.UtcNow);
			resultsXml.WriteResultFile (results, ret);

			return ret;
		}
	}
}
