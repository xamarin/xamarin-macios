using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;


using Xunit;
using Xunit.Abstractions;

namespace Xamarin.iOS.UnitTests.XUnit
{
	public class XUnitTestRunner : TestRunner
	{
		readonly TestMessageSink messageSink;

		XElement assembliesElement;
		List<XUnitFilter> filters;

		public XUnitResultFileFormat ResultFileFormat { get; set; } = XUnitResultFileFormat.XunitV2;
		public AppDomainSupport AppDomainSupport { get; set; } = AppDomainSupport.Denied;
		protected override string ResultsFileName { get; set; } = "TestResults.xUnit.xml";

		public XUnitTestRunner (LogWriter logger) : base (logger)
		{
			messageSink = new TestMessageSink ();

			messageSink.Diagnostics.DiagnosticMessageEvent += HandleDiagnosticMessage;
			messageSink.Diagnostics.ErrorMessageEvent += HandleDiagnosticErrorMessage;

			messageSink.Discovery.DiscoveryCompleteMessageEvent += HandleDiscoveryCompleteMessage;
			messageSink.Discovery.TestCaseDiscoveryMessageEvent += HandleDiscoveryTestCaseMessage;

			messageSink.Runner.TestAssemblyDiscoveryFinishedEvent += HandleTestAssemblyDiscoveryFinished;
			messageSink.Runner.TestAssemblyDiscoveryStartingEvent += HandleTestAssemblyDiscoveryStarting;
			messageSink.Runner.TestAssemblyExecutionFinishedEvent += HandleTestAssemblyExecutionFinished;
			messageSink.Runner.TestAssemblyExecutionStartingEvent += HandleTestAssemblyExecutionStarting;
			messageSink.Runner.TestExecutionSummaryEvent += HandleTestExecutionSummary;

			messageSink.Execution.AfterTestFinishedEvent += (MessageHandlerArgs <IAfterTestFinished> args) => HandleEvent ("AfterTestFinishedEvent", args, HandleAfterTestFinished);
			messageSink.Execution.AfterTestStartingEvent += (MessageHandlerArgs <IAfterTestStarting> args) => HandleEvent ("AfterTestStartingEvent", args, HandleAfterTestStarting);
			messageSink.Execution.BeforeTestFinishedEvent += (MessageHandlerArgs<IBeforeTestFinished> args) => HandleEvent ("BeforeTestFinishedEvent", args, HandleBeforeTestFinished);
			messageSink.Execution.BeforeTestStartingEvent += (MessageHandlerArgs<IBeforeTestStarting> args) => HandleEvent ("BeforeTestStartingEvent", args, HandleBeforeTestStarting);
			messageSink.Execution.TestAssemblyCleanupFailureEvent += (MessageHandlerArgs<ITestAssemblyCleanupFailure> args) => HandleEvent ("TestAssemblyCleanupFailureEvent", args, HandleTestAssemblyCleanupFailure);
			messageSink.Execution.TestAssemblyFinishedEvent += (MessageHandlerArgs<ITestAssemblyFinished> args) => HandleEvent ("TestAssemblyFinishedEvent", args, HandleTestAssemblyFinished);
			messageSink.Execution.TestAssemblyStartingEvent += (MessageHandlerArgs<ITestAssemblyStarting> args) => HandleEvent ("TestAssemblyStartingEvent", args, HandleTestAssemblyStarting);
			messageSink.Execution.TestCaseCleanupFailureEvent += (MessageHandlerArgs<ITestCaseCleanupFailure> args) => HandleEvent ("TestCaseCleanupFailureEvent", args, HandleTestCaseCleanupFailure);
			messageSink.Execution.TestCaseFinishedEvent += (MessageHandlerArgs<ITestCaseFinished> args) => HandleEvent ("TestCaseFinishedEvent", args, HandleTestCaseFinished);
			messageSink.Execution.TestCaseStartingEvent += (MessageHandlerArgs<ITestCaseStarting> args) => HandleEvent ("TestStartingEvent", args, HandleTestCaseStarting);
			messageSink.Execution.TestClassCleanupFailureEvent += (MessageHandlerArgs<ITestClassCleanupFailure> args) => HandleEvent ("TestClassCleanupFailureEvent", args, HandleTestClassCleanupFailure);
			messageSink.Execution.TestClassConstructionFinishedEvent += (MessageHandlerArgs<ITestClassConstructionFinished> args) => HandleEvent ("TestClassConstructionFinishedEvent", args, HandleTestClassConstructionFinished);
			messageSink.Execution.TestClassConstructionStartingEvent += (MessageHandlerArgs<ITestClassConstructionStarting> args) => HandleEvent ("TestClassConstructionStartingEvent", args, HandleTestClassConstructionStarting);
			messageSink.Execution.TestClassDisposeFinishedEvent += (MessageHandlerArgs<ITestClassDisposeFinished> args) => HandleEvent ("TestClassDisposeFinishedEvent", args, HandleTestClassDisposeFinished);
			messageSink.Execution.TestClassDisposeStartingEvent += (MessageHandlerArgs<ITestClassDisposeStarting> args) => HandleEvent ("TestClassDisposeStartingEvent", args, HandleTestClassDisposeStarting);
			messageSink.Execution.TestClassFinishedEvent += (MessageHandlerArgs<ITestClassFinished> args) => HandleEvent ("TestClassFinishedEvent", args, HandleTestClassFinished);
			messageSink.Execution.TestClassStartingEvent += (MessageHandlerArgs<ITestClassStarting> args) => HandleEvent ("TestClassStartingEvent", args, HandleTestClassStarting);
			messageSink.Execution.TestCleanupFailureEvent += (MessageHandlerArgs<ITestCleanupFailure> args) => HandleEvent ("TestCleanupFailureEvent", args, HandleTestCleanupFailure);
			messageSink.Execution.TestCollectionCleanupFailureEvent += (MessageHandlerArgs<ITestCollectionCleanupFailure> args) => HandleEvent ("TestCollectionCleanupFailureEvent", args, HandleTestCollectionCleanupFailure);
			messageSink.Execution.TestCollectionFinishedEvent += (MessageHandlerArgs<ITestCollectionFinished> args) => HandleEvent ("TestCollectionFinishedEvent", args, HandleTestCollectionFinished);
			messageSink.Execution.TestCollectionStartingEvent += (MessageHandlerArgs<ITestCollectionStarting> args) => HandleEvent ("TestCollectionStartingEvent", args, HandleTestCollectionStarting);
			messageSink.Execution.TestFailedEvent += (MessageHandlerArgs<ITestFailed> args) => HandleEvent ("TestFailedEvent", args, HandleTestFailed);
			messageSink.Execution.TestFinishedEvent += (MessageHandlerArgs<ITestFinished> args) => HandleEvent ("TestFinishedEvent", args, HandleTestFinished);
			messageSink.Execution.TestMethodCleanupFailureEvent += (MessageHandlerArgs<ITestMethodCleanupFailure> args) => HandleEvent ("TestMethodCleanupFailureEvent", args, HandleTestMethodCleanupFailure);
			messageSink.Execution.TestMethodFinishedEvent += (MessageHandlerArgs<ITestMethodFinished> args) => HandleEvent ("TestMethodFinishedEvent", args, HandleTestMethodFinished);
			messageSink.Execution.TestMethodStartingEvent += (MessageHandlerArgs<ITestMethodStarting> args) => HandleEvent ("TestMethodStartingEvent", args, HandleTestMethodStarting);
			messageSink.Execution.TestOutputEvent += (MessageHandlerArgs<ITestOutput> args) => HandleEvent ("TestOutputEvent", args, HandleTestOutput);
			messageSink.Execution.TestPassedEvent += (MessageHandlerArgs<ITestPassed> args) => HandleEvent ("TestPassedEvent", args, HandleTestPassed);
			messageSink.Execution.TestSkippedEvent += (MessageHandlerArgs<ITestSkipped> args) => HandleEvent ("TestSkippedEvent", args, HandleTestSkipped);
			messageSink.Execution.TestStartingEvent += (MessageHandlerArgs<ITestStarting> args) => HandleEvent ("TestStartingEvent", args, HandleTestStarting);
		}

		public void SetFilters (List<XUnitFilter> newFilters)
		{
			if (newFilters == null) {
				filters = null;
				return;
			}

			if (filters == null)
				filters = new List<XUnitFilter> ();

			filters.AddRange (newFilters);
		}

		void HandleEvent<T> (string name, MessageHandlerArgs <T> args, Action <MessageHandlerArgs<T>> actualHandler) where T: class, IMessageSinkMessage
		{
			try {
				actualHandler (args);
			} catch (Exception ex) {
				OnError ($"Handler for event {name} failed with exception");
				OnError (ex.ToString ());
			}
		}

		void HandleTestStarting (MessageHandlerArgs<ITestStarting> args)
		{
			if (args == null || args.Message == null)
				return;
			
			OnDebug ("Test starting");
			LogTestDetails (args.Message.Test, log: OnDebug);
			ReportTestCases ("   Associated", args.Message.TestCases, args.Message.TestCase, OnDiagnostic);
		}

		void HandleTestSkipped (MessageHandlerArgs <ITestSkipped> args)
		{
			if (args == null || args.Message == null)
				return;

			SkippedTests++;
			OnInfo ($"\t[IGNORED] {args.Message.TestCase.DisplayName}");
			LogTestDetails (args.Message.Test, log: OnDebug);
			LogTestOutput (args.Message, log: OnDiagnostic);
			ReportTestCases ("   Associated", args.Message.TestCases, log: OnDiagnostic);
		}

		void HandleTestPassed (MessageHandlerArgs <ITestPassed> args)
		{
			if (args == null || args.Message == null)
				return;

			PassedTests++;
			OnInfo ($"\t[PASS] {args.Message.TestCase.DisplayName}");
			LogTestDetails (args.Message.Test, log: OnDebug);
			LogTestOutput (args.Message, log: OnDiagnostic);
			ReportTestCases ("   Associated", args.Message.TestCases, log: OnDiagnostic);
		}

		void HandleTestOutput (MessageHandlerArgs <ITestOutput> args)
		{
			if (args == null || args.Message == null)
				return;

			OnInfo (args.Message.Output);
		}

		void HandleTestMethodStarting (MessageHandlerArgs <ITestMethodStarting> args)
		{
			if (args == null || args.Message == null)
				return;

			OnDebug ("Test method starting");
			LogTestMethodDetails (args.Message.TestMethod.Method, log: OnDebug);
			LogTestClassDetails (args.Message.TestMethod.TestClass, log: OnDebug);
			ReportTestCases ("   Associated", args.Message.TestCases, log: OnDiagnostic);
		}

		void HandleTestMethodFinished (MessageHandlerArgs <ITestMethodFinished> args)
		{
			if (args == null || args.Message == null)
				return;

			OnDebug ("Test method finished");
			LogTestMethodDetails (args.Message.TestMethod.Method, log: OnDebug);
			LogTestClassDetails (args.Message.TestMethod.TestClass, log: OnDebug);
			LogSummary (args.Message, log: OnDebug);
			ReportTestCases ("   Associated", args.Message.TestCases, log: OnDiagnostic);
		}

		void HandleTestMethodCleanupFailure (MessageHandlerArgs <ITestMethodCleanupFailure> args)
		{
			if (args == null || args.Message == null)
				return;

			OnError ($"Test method cleanup failure{GetAssemblyInfo (args.Message.TestAssembly)}");
			LogTestMethodDetails (args.Message.TestMethod.Method, log: OnError);
			LogTestClassDetails (args.Message.TestMethod.TestClass, log: OnError);
			ReportTestCases ("   Associated", args.Message.TestCases, log: OnDiagnostic);
			LogFailureInformation (args.Message, log: OnError);
		}

		void HandleTestFinished (MessageHandlerArgs <ITestFinished> args)
		{
			if (args == null || args.Message == null)
				return;

			ExecutedTests++;
			OnDiagnostic ("Test finished");
			LogTestDetails (args.Message.Test, log: OnDiagnostic);
			LogTestOutput (args.Message, log: OnDiagnostic);
			ReportTestCases ("   Associated", args.Message.TestCases, args.Message.TestCase, OnDiagnostic);
		}

		void HandleTestFailed (MessageHandlerArgs<ITestFailed> args)
		{
			if (args == null || args.Message == null)
				return;

			FailedTests++;
			string assemblyInfo = GetAssemblyInfo (args.Message.TestAssembly);
			var sb = new StringBuilder ($"\t[FAIL] {args.Message.TestCase.DisplayName}");
			LogTestDetails (args.Message.Test, OnError, sb);
			sb.AppendLine ();
			if (!string.IsNullOrEmpty (assemblyInfo))
				sb.AppendLine ($"   Assembly: {assemblyInfo}");
			LogSourceInformation (args.Message.TestCase.SourceInformation, OnError, sb);
			LogFailureInformation (args.Message, OnError, sb);
			sb.AppendLine ();
			LogTestOutput (args.Message, OnError, sb);
			sb.AppendLine ();
			if (args.Message.TestCase.Traits != null && args.Message.TestCase.Traits.Count > 0) {
				foreach (var kvp in args.Message.TestCase.Traits) {
					string message = $"   Test trait name: {kvp.Key}";
					OnError (message);
					sb.AppendLine (message);

					foreach (string v in kvp.Value) {
						message = $"      value: {v}";
						OnError (message);
						sb.AppendLine (message);
					}
				}
				sb.AppendLine ();
			}
			ReportTestCases ("   Associated", args.Message.TestCases, args.Message.TestCase, OnDiagnostic);

			FailureInfos.Add (new TestFailureInfo {
				TestName = args.Message.Test?.DisplayName,
				Message = sb.ToString ()
			});
			OnInfo ($"\t[FAIL] {args.Message.Test.TestCase.DisplayName}");
			OnInfo (sb.ToString ());
		}

		void HandleTestCollectionStarting (MessageHandlerArgs <ITestCollectionStarting> args)
		{
			if (args == null || args.Message == null)
				return;

			OnInfo ($"\n{args.Message.TestCollection.DisplayName}");
			OnDebug ("Test collection starting");
			LogTestCollectionDetails (args.Message.TestCollection, log: OnDebug);
			ReportTestCases ("   Associated", args.Message.TestCases, log: OnDiagnostic);
		}

		void HandleTestCollectionFinished (MessageHandlerArgs <ITestCollectionFinished> args)
		{
			if (args == null || args.Message == null)
				return;

			OnDebug ("Test collection finished");
			LogSummary (args.Message, log: OnDebug);
			LogTestCollectionDetails (args.Message.TestCollection, log: OnDebug);
			ReportTestCases ("   Associated", args.Message.TestCases, log: OnDiagnostic);
		}

		void HandleTestCollectionCleanupFailure (MessageHandlerArgs <ITestCollectionCleanupFailure> args)
		{
			if (args == null || args.Message == null)
				return;

			OnError ("Error during test collection cleanup");
			LogTestCollectionDetails (args.Message.TestCollection, log:OnError);
			ReportTestCases ("   Associated", args.Message.TestCases, log: OnError);
			LogFailureInformation (args.Message, log: OnError);
		}

		void HandleTestCleanupFailure (MessageHandlerArgs <ITestCleanupFailure> args)
		{
			if (args == null || args.Message == null)
				return;

			OnError ($"Test cleanup failure{GetAssemblyInfo (args.Message.TestAssembly)}");
			LogTestDetails (args.Message.Test, log: OnError);
			ReportTestCases ("   Associated", args.Message.TestCases, log: OnError);
			LogFailureInformation (args.Message, log: OnError);
		}

		void HandleTestClassStarting (MessageHandlerArgs <ITestClassStarting> args)
		{
			if (args == null || args.Message == null)
				return;

			OnDiagnostic ("Test class starting");
			LogTestClassDetails (args.Message.TestClass, log: OnDiagnostic);
		}

		void HandleTestClassFinished (MessageHandlerArgs <ITestClassFinished> args)
		{
			if (args == null || args.Message == null)
				return;
			
			OnDebug ("Test class finished");
			OnInfo ($"{args.Message.TestClass.Class.Name} {args.Message.ExecutionTime} ms");
			LogTestClassDetails (args.Message.TestClass, OnDebug);
			ReportTestCases ("   Associated", args.Message.TestCases, log: OnDiagnostic);
		}

		void HandleTestClassDisposeStarting (MessageHandlerArgs <ITestClassDisposeStarting> args)
		{
			if (args == null || args.Message == null)
				return;

			OnDiagnostic ("Test class dispose starting");
			LogTestDetails (args.Message.Test, log: OnDiagnostic);
			ReportTestCases ("   Associated", args.Message.TestCases, log: OnDiagnostic);
		}

		void HandleTestClassDisposeFinished (MessageHandlerArgs <ITestClassDisposeFinished> args)
		{
			if (args == null || args.Message == null)
				return;

			OnDiagnostic ("Test class dispose finished");
			LogTestDetails (args.Message.Test, log: OnDiagnostic);
			ReportTestCases ("   Associated", args.Message.TestCases, log: OnDiagnostic);
		}

		void HandleTestClassConstructionStarting (MessageHandlerArgs <ITestClassConstructionStarting> args)
		{
			if (args == null || args.Message == null)
				return;

			OnDiagnostic ("Test class construction starting");
			LogTestDetails (args.Message.Test, OnDiagnostic);
			ReportTestCases ("   Associated", args.Message.TestCases, args.Message.TestCase, OnDiagnostic);
		}

		void HandleTestClassConstructionFinished (MessageHandlerArgs <ITestClassConstructionFinished> args)
		{
			if (args == null || args.Message == null)
				return;

			OnDiagnostic ("Test class construction finished");
			LogTestDetails (args.Message.Test, log: OnDiagnostic);
			ReportTestCases ("   Associated", args.Message.TestCases, args.Message.TestCase, OnDiagnostic);
		}

		void HandleTestClassCleanupFailure (MessageHandlerArgs <ITestClassCleanupFailure> args)
		{
			if (args == null || args.Message == null)
				return;

			OnError ($"Test class cleanup error{GetAssemblyInfo (args.Message.TestAssembly)}");
			LogTestClassDetails (args.Message.TestClass, log: OnError);
			LogTestCollectionDetails (args.Message.TestCollection, log: OnError);
			ReportTestCases ("   Associated", args.Message.TestCases, log: OnError);
			LogFailureInformation (args.Message, log: OnError);
		}

		void HandleTestCaseStarting (MessageHandlerArgs <ITestCaseStarting> args)
		{
			if (args == null || args.Message == null)
				return;

			OnDiagnostic ("Test case starting");
			ReportTestCase ("   Starting", args.Message.TestCase, log: OnDiagnostic);
			ReportTestCases ("   Associated", args.Message.TestCases, args.Message.TestCase, OnDiagnostic);
		}

		void HandleTestCaseFinished (MessageHandlerArgs <ITestCaseFinished> args)
		{
			if (args == null || args.Message == null)
				return;

			OnDebug ("Test case finished executing");
			ReportTestCase ("   Finished", args.Message.TestCase, log: OnDebug);
			ReportTestCases ("   Associated", args.Message.TestCases, args.Message.TestCase, OnDebug);
			LogSummary (args.Message, log: OnDebug);
		}

		void HandleTestCaseCleanupFailure (MessageHandlerArgs <ITestCaseCleanupFailure> args)
		{
			if (args == null || args.Message == null)
				return;

			OnError ("Test case cleanup failure");
			ReportTestCase ("   Failed", args.Message.TestCase, log: OnError);
			ReportTestCases ("   Associated", args.Message.TestCases, args.Message.TestCase, OnError);
			LogFailureInformation (args.Message, log: OnError);
		}

		void HandleTestAssemblyStarting (MessageHandlerArgs <ITestAssemblyStarting> args)
		{
			if (args == null || args.Message == null)
				return;

			OnInfo ($"[Test environment: {args.Message.TestEnvironment}]");
			OnInfo ($"[Test framework: {args.Message.TestFrameworkDisplayName}]");
			LogAssemblyInformation (args.Message, log: OnDebug);
			ReportTestCases ("   Associated", args.Message.TestCases, log: OnDebug);
		}

		void HandleTestAssemblyFinished (MessageHandlerArgs <ITestAssemblyFinished> args)
		{
			if (args == null || args.Message == null)
				return;

			TotalTests = args.Message.TestsRun; // HACK: We are not counting correctly all the tests
			OnDebug ("Execution process for assembly finished");
			LogAssemblyInformation (args.Message, log: OnDebug);
			LogSummary (args.Message, log: OnDebug);
			ReportTestCases ("   Associated", args.Message.TestCases, log: OnDiagnostic);
		}

		void HandleTestAssemblyCleanupFailure (MessageHandlerArgs <ITestAssemblyCleanupFailure> args)
		{
			if (args == null || args.Message == null)
				return;

			OnError ("Assembly cleanup failure");
			LogAssemblyInformation (args.Message, OnError);
			ReportTestCases ("   Associated", args.Message.TestCases, log: OnError);
			LogFailureInformation (args.Message, log: OnError);
		}

		void HandleBeforeTestStarting (MessageHandlerArgs <IBeforeTestStarting> args)
		{
			if (args == null || args.Message == null)
				return;

			OnDiagnostic ($"'Before' method for test '{args.Message.Test.DisplayName}' starting");
		}

		void HandleBeforeTestFinished (MessageHandlerArgs <IBeforeTestFinished> args)
		{
			if (args == null || args.Message == null)
				return;

			OnDiagnostic ($"'Before' method for test '{args.Message.Test.DisplayName}' finished");
		}

		void HandleAfterTestStarting (MessageHandlerArgs <IAfterTestStarting> args)
		{
			if (args == null || args.Message == null)
				return;

			OnDiagnostic ($"'After' method for test '{args.Message.Test.DisplayName}' starting");
		}

		void HandleAfterTestFinished (MessageHandlerArgs <IAfterTestFinished> args)
		{
			if (args == null || args.Message == null)
				return;

			OnDiagnostic ($"'After' method for test '{args.Message.Test.DisplayName}' finished");
		}

		void HandleTestExecutionSummary (MessageHandlerArgs <ITestExecutionSummary> args)
		{
			if (args == null || args.Message == null)
				return;

			OnInfo ("All tests finished");
			OnInfo ($"    Elapsed time: {args.Message.ElapsedClockTime}");

			if (args.Message.Summaries == null || args.Message.Summaries.Count == 0)
				return;

			foreach (KeyValuePair <string, ExecutionSummary> summary in args.Message.Summaries) {
				OnInfo (String.Empty);
				OnInfo ($" Assembly: {summary.Key}");
				LogSummary (summary.Value, log: OnDebug);
			}
		}

		void HandleTestAssemblyExecutionStarting (MessageHandlerArgs <ITestAssemblyExecutionStarting> args)
		{
			if (args == null || args.Message == null)
				return;

			OnInfo ($"Execution starting for assembly {args.Message.Assembly.AssemblyFilename}");
		}

		void HandleTestAssemblyExecutionFinished (MessageHandlerArgs <ITestAssemblyExecutionFinished> args)
		{
			if (args == null || args.Message == null)
				return;

			OnInfo ($"Execution finished for assembly {args.Message.Assembly.AssemblyFilename}");
			LogSummary (args.Message.ExecutionSummary, log: OnDebug);
		}

		void HandleTestAssemblyDiscoveryStarting (MessageHandlerArgs <ITestAssemblyDiscoveryStarting> args)
		{
			if (args == null || args.Message == null)
				return;

			OnInfo ($"Discovery for assembly {args.Message.Assembly.AssemblyFilename} starting");
			OnInfo ($"   Will use AppDomain: {args.Message.AppDomain.YesNo ()}");
		}

		void HandleTestAssemblyDiscoveryFinished (MessageHandlerArgs <ITestAssemblyDiscoveryFinished> args)
		{
			if (args == null || args.Message == null)
				return;

			OnInfo ($"Discovery for assembly {args.Message.Assembly.AssemblyFilename} finished");
			OnInfo ($"   Test cases discovered: {args.Message.TestCasesDiscovered}");
			OnInfo ($"   Test cases to run: {args.Message.TestCasesToRun}");
		}

		void HandleDiagnosticMessage (MessageHandlerArgs <IDiagnosticMessage> args)
		{
			if (args == null || args.Message == null)
				return;

			OnDiagnostic (args.Message.Message);
		}

		void HandleDiagnosticErrorMessage (MessageHandlerArgs <IErrorMessage> args)
		{
			if (args == null || args.Message == null)
				return;

			LogFailureInformation (args.Message);
		}

		void HandleDiscoveryCompleteMessage (MessageHandlerArgs <IDiscoveryCompleteMessage> args)
		{
			if (args == null || args.Message == null)
				return;

			OnInfo ("Discovery complete");
		}

		void HandleDiscoveryTestCaseMessage (MessageHandlerArgs <ITestCaseDiscoveryMessage> args)
		{
			if (args == null || args.Message == null)
				return;

			ITestCase singleTestCase = args.Message.TestCase;
			ReportTestCases ("Discovered", args.Message.TestCases, log: OnInfo, ignore: (ITestCase tc) => tc == singleTestCase);
			ReportTestCase ("Discovered", singleTestCase, log: OnInfo);
		}

		void ReportTestCases (string verb, IEnumerable<ITestCase> testCases, ITestCase ignoreTestCase, Action<string> log = null)
		{
			ReportTestCases (verb, testCases, log, (ITestCase tc) => ignoreTestCase == tc);
		}

		void ReportTestCases (string verb, IEnumerable<ITestCase> testCases, Action<string> log = null, Func<ITestCase, bool> ignore = null)
		{
			if (testCases == null)
				return;

			foreach (ITestCase tc in testCases) {
				if (ignore != null && ignore (tc))
					continue;
				ReportTestCase (verb, tc, log);
			}
		}

		void ReportTestCase (string verb, ITestCase testCase, Action<string> log = null)
		{
			if (testCase == null)
				return;

			EnsureLogger (log)($"{verb} test case: {testCase.DisplayName}");
		}

		void LogAssemblyInformation (ITestAssemblyMessage message,  Action<string> log = null, StringBuilder sb = null)
		{
			if (message == null)
				return;
			
			do_log ($"[Assembly name: {message.TestAssembly.Assembly.Name}]", log, sb);
			do_log ($"[Assembly path: {message.TestAssembly.Assembly.AssemblyPath}]", OnDiagnostic, sb);
		}

		void LogFailureInformation (IFailureInformation info, Action<string> log = null, StringBuilder sb = null)
		{
			if (info == null)
				return;
			
			string message = ExceptionUtility.CombineMessages (info);
			do_log ($"   Exception messages: {message}", log, sb);

			string traces = ExceptionUtility.CombineStackTraces (info);
			do_log ($"   Exception stack traces: {traces}", log, sb);
		}

		Action<string> EnsureLogger (Action<string> log)
		{
			return log ?? OnInfo;
		}

		void LogTestMethodDetails (IMethodInfo method, Action<string> log = null, StringBuilder sb = null)
		{
			log = EnsureLogger (log);

			//log ($"   Test method name: {method.Type.Name}.{method.Name}");
		}

		void LogTestOutput (ITestFinished test, Action<string> log = null, StringBuilder sb = null)
		{
			LogTestOutput (test.ExecutionTime, test.Output, log, sb);
		}

		void LogTestOutput (ITestResultMessage test, Action<string> log = null, StringBuilder sb = null)
		{
			LogTestOutput (test.ExecutionTime, test.Output, log, sb);
		}

		void LogTestOutput (decimal executionTime, string output, Action<string> log = null, StringBuilder sb = null)
		{
			do_log ($"   Execution time: {executionTime}", log, sb);
			if (!String.IsNullOrEmpty (output)) {
				do_log (" **** Output start ****", log, sb);
				foreach (string line in output.Split ('\n'))
					do_log (line, log, sb);
				do_log (" **** Output end ****", log, sb);
			}
		}

		void LogTestCollectionDetails (ITestCollection collection, Action<string> log = null, StringBuilder sb = null)
		{
			do_log ($"   Test collection: {collection.DisplayName}", log, sb);
		}

		void LogTestClassDetails (ITestClass klass, Action<string> log = null, StringBuilder sb = null)
		{
			do_log ($"   Class name: {klass.Class.Name}", log, sb);
			do_log ($"   Class assembly: {klass.Class.Assembly.Name}", OnDebug, sb);
			do_log ($"   Class assembly path: {klass.Class.Assembly.AssemblyPath}", OnDebug, sb);
		}

		void LogTestDetails (ITest test, Action<string> log = null, StringBuilder sb = null)
		{
			do_log ($"   Test name: {test.DisplayName}", log, sb);
			if (String.Compare (test.DisplayName, test.TestCase.DisplayName, StringComparison.Ordinal) != 0)
				do_log ($"   Test case: {test.TestCase.DisplayName}", log, sb);
		}

		void LogSummary (IFinishedMessage summary, Action<string> log = null, StringBuilder sb = null)
		{
			do_log ($"   Time: {summary.ExecutionTime}", log, sb);
			do_log ($"   Total tests run: {summary.TestsRun}", log, sb);
			do_log ($"   Skipped tests: {summary.TestsSkipped}", log, sb);
			do_log ($"   Failed tests: {summary.TestsFailed}", log, sb);
		}

		void LogSummary (ExecutionSummary summary, Action<string> log = null, StringBuilder sb = null)
		{
			do_log ($"   Time: {summary.Time}", log, sb);
			do_log ($"   Total tests run: {summary.Total}", log, sb);
			do_log ($"   Total errors: {summary.Errors}", log, sb);
			do_log ($"   Skipped tests: {summary.Skipped}", log, sb);
			do_log ($"   Failed tests: {summary.Failed}", log, sb);
		}

		void LogSourceInformation (ISourceInformation source, Action<string> log = null, StringBuilder sb = null)
		{
			if (source == null || String.IsNullOrEmpty (source.FileName))
				return;

			string location = source.FileName;
			if (source.LineNumber != null && source.LineNumber >= 0)
				location += $":{source.LineNumber}";

			do_log ($"   Source: {location}", log, sb);
			sb?.AppendLine ();
		}

		string GetAssemblyInfo (ITestAssembly assembly)
		{
			string name = assembly?.Assembly?.Name?.Trim ();
			if (String.IsNullOrEmpty (name))
				return name;
			return $" [{name}]";
		}

		void do_log (string message, Action<string> log = null, StringBuilder sb = null)
		{
			log = EnsureLogger (log);

			if (sb != null)
				sb.Append (message);
			log (message);
		}

		public override void Run (IList <TestAssemblyInfo> testAssemblies)
		{
			if (testAssemblies == null)
				throw new ArgumentNullException (nameof (testAssemblies));

			assembliesElement = new XElement ("assemblies");
			foreach (TestAssemblyInfo assemblyInfo in testAssemblies) {
				if (assemblyInfo == null || assemblyInfo.Assembly == null)
					continue;
				
				if (String.IsNullOrEmpty (assemblyInfo.FullPath)) {
					OnWarning ($"Assembly '{assemblyInfo.Assembly}' cannot be found on the filesystem. xUnit requires access to actual on-disk file.");
					continue;
				}

				OnInfo ($"Assembly: {assemblyInfo.Assembly} ({assemblyInfo.FullPath})");
				XElement assemblyElement = null;
				try {
					OnAssemblyStart (assemblyInfo.Assembly);
					assemblyElement = Run (assemblyInfo.Assembly, assemblyInfo.FullPath);
				} finally {
					OnAssemblyFinish (assemblyInfo.Assembly);
					if (assemblyElement != null)
						assembliesElement.Add (assemblyElement);
				}
			}

			LogFailureSummary ();
		}

		public override string WriteResultsToFile ()
		{
			if (assembliesElement == null)
				return String.Empty;

			string outputFilePath = GetResultsFilePath ();
			var settings = new XmlWriterSettings { Indent = true };
			using (var xmlWriter = XmlWriter.Create (outputFilePath, settings)) {
				switch (ResultFileFormat) {
				case XUnitResultFileFormat.XunitV2:
					assembliesElement.Save (xmlWriter);
					break;
				case XUnitResultFileFormat.NUnit:
					Transform_Results ("NUnitXml.xslt", assembliesElement, xmlWriter); // TODO: Add resource
					break;

				default:
					throw new InvalidOperationException ($"Result output format '{ResultFileFormat}' is not currently supported");
				}
			}
			
			return outputFilePath;
		}

		void Transform_Results (string xsltResourceName, XElement element, XmlWriter writer)
		{
			var xmlTransform = new System.Xml.Xsl.XslCompiledTransform ();

			using (Stream xsltStream = GetType ().Assembly.GetManifestResourceStream ($"Xamarin.Android.UnitTests.XUnit.{xsltResourceName}")) {
				using (XmlReader xsltReader = XmlReader.Create (xsltStream)) {
					using (XmlReader xmlReader = element.CreateReader ()) {
						xmlTransform.Load (xsltReader);
						xmlTransform.Transform (xmlReader, writer);
					}
				}
			}
		}

		protected virtual Stream GetConfigurationFileStream (Assembly assembly)
		{
			if (assembly == null)
				throw new ArgumentNullException (nameof (assembly));

			string path = assembly.Location?.Trim ();
			if (String.IsNullOrEmpty (path))
				return null;

			path = Path.Combine (path, ".xunit.runner.json");
			if (!File.Exists (path))
				return null;

			return File.OpenRead (path);
		}

		protected virtual TestAssemblyConfiguration GetConfiguration (Assembly assembly)
		{
			if (assembly == null)
				throw new ArgumentNullException (nameof (assembly));

			Stream configStream = GetConfigurationFileStream (assembly);
			if (configStream != null) {
				using (configStream) {
					return ConfigReader.Load (configStream);
				}
			}

			return null;
		}

		protected virtual ITestFrameworkDiscoveryOptions GetFrameworkOptionsForDiscovery (TestAssemblyConfiguration configuration)
		{
			if (configuration == null)
				throw new ArgumentNullException (nameof (configuration));

			return TestFrameworkOptions.ForDiscovery (configuration);
		}

		protected virtual ITestFrameworkExecutionOptions GetFrameworkOptionsForExecution (TestAssemblyConfiguration configuration)
		{
			if (configuration == null)
				throw new ArgumentNullException (nameof (configuration));

			return TestFrameworkOptions.ForExecution (configuration);
		}

		XElement Run (Assembly assembly, string assemblyPath)
		{
			using (var frontController = new XunitFrontController (AppDomainSupport, assemblyPath, null, false)) {
				using (var discoverySink = new TestDiscoverySink ()) {
					var configuration = GetConfiguration (assembly) ?? new TestAssemblyConfiguration ();
					ITestFrameworkDiscoveryOptions discoveryOptions = GetFrameworkOptionsForDiscovery (configuration);
					discoveryOptions.SetSynchronousMessageReporting (true);
					Logger.OnDebug ($"Starting test discovery in the '{assembly}' assembly");
					frontController.Find (false, discoverySink, discoveryOptions);
					Logger.OnDebug ($"Test discovery in assembly '{assembly}' completed");
					discoverySink.Finished.WaitOne ();

					if (discoverySink.TestCases == null || discoverySink.TestCases.Count == 0) {
						Logger.Info ("No test cases discovered");
						return null;
					}

					TotalTests += discoverySink.TestCases.Count;
					List<ITestCase> testCases;
					if (filters != null && filters.Count > 0) {
						testCases = discoverySink.TestCases.Where (tc => IsIncluded (tc)).ToList ();
						FilteredTests += discoverySink.TestCases.Count - testCases.Count;
					} else
						testCases = discoverySink.TestCases;

					var assemblyElement = new XElement ("assembly");
					IExecutionSink resultsSink = new DelegatingExecutionSummarySink (messageSink, null, null);
					resultsSink = new DelegatingXmlCreationSink (resultsSink, assemblyElement);
					ITestFrameworkExecutionOptions executionOptions = GetFrameworkOptionsForExecution (configuration);
					executionOptions.SetDisableParallelization (!RunInParallel);
					executionOptions.SetSynchronousMessageReporting (true);

					try {
						frontController.RunTests (testCases, resultsSink, executionOptions);
						resultsSink.Finished.WaitOne ();
					} finally {
						resultsSink.Dispose ();
					}

					return assemblyElement;
				}
			}
		}

		bool IsIncluded (ITestCase testCase)
		{
			if (testCase.Traits == null || testCase.Traits.Count == 0)
				return true;
			
			foreach (XUnitFilter filter in filters) {
				List<string> values;
				if (filter == null)
					continue;

				if (filter.FilterType == XUnitFilterType.Trait) {
					if (!testCase.Traits.TryGetValue (filter.TraitName, out values))
						continue;

					if (values == null || values.Count == 0) {
						// We have no values and the filter doesn't specify one - that means we match on
						// the trait name only.
						if (String.IsNullOrEmpty (filter.TraitValue))
							return !filter.Exclude;
						continue;
					}

					if (values.Contains (filter.TraitValue, StringComparer.OrdinalIgnoreCase))
						return !filter.Exclude;
					continue;
				}

				if (filter.FilterType == XUnitFilterType.TypeName) {
					Logger.Info ($"IsIncluded: filter: '{filter.TestCaseName}', test case name: {testCase.DisplayName}"); 
					if (String.Compare (testCase.DisplayName, filter.TestCaseName, StringComparison.OrdinalIgnoreCase) == 0)
						return !filter.Exclude;
					continue;
				}

				throw new InvalidOperationException ($"Unsupported filter type {filter.FilterType}");
			}

			return true;
		}
	}
}
