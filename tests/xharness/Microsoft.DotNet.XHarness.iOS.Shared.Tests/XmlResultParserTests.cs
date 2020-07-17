using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Moq;
using NUnit.Framework;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;

namespace Microsoft.DotNet.XHarness.iOS.Shared.Tests {

	[TestFixture]
	public class XmlResultParserTests {

		static Dictionary<XmlResultJargon, Action<string, string, string, string, string, string, string, int>> ValidationMap = new Dictionary<XmlResultJargon, Action<string, string, string, string, string, string, string, int>> {
			[XmlResultJargon.NUnitV2] = ValidateNUnitV2Failure,
			[XmlResultJargon.NUnitV3] = ValidateNUnitV3Failure,
			[XmlResultJargon.xUnit] = ValidatexUnitFailure,
		};

		XmlResultParser resultParser;

		[SetUp]
		public void SetUp ()
		{
			resultParser = new XmlResultParser ();
		}

		[TearDown]
		public void TearDown ()
		{
			resultParser = null;
		}

		string CreateResultSample (XmlResultJargon jargon, bool includePing = false)
		{
			string sampleFileName = null;
			switch (jargon) {
			case XmlResultJargon.NUnitV2:
				sampleFileName = "NUnitV2Sample.xml";
				break;
			case XmlResultJargon.NUnitV3:
				sampleFileName = "NUnitV3Sample.xml";
				break;
			case XmlResultJargon.TouchUnit:
				sampleFileName = "TouchUnitSample.xml";
				break;
			case XmlResultJargon.xUnit:
				sampleFileName = "xUnitSample.xml";
				break;
			}
			Assert.NotNull (sampleFileName, "sample filename");
			var name = GetType ().Assembly.GetManifestResourceNames ().Where (a => a.EndsWith (sampleFileName, StringComparison.Ordinal)).FirstOrDefault ();
			var tempPath = Path.GetTempFileName ();
			using (var outputStream = new StreamWriter (tempPath))
			using (var sampleStream = new StreamReader (GetType ().Assembly.GetManifestResourceStream (name))) {
				if (includePing) {
					outputStream.WriteLine ("ping");
				}
				string line;
				while ((line = sampleStream.ReadLine ()) != null)
					outputStream.WriteLine (line);
			}
			return tempPath;
		}

		[Test]
		public void IsValidXmlMissingFileTest ()
		{
			var path = Path.GetTempFileName ();
			File.Delete (path);
			Assert.IsFalse (resultParser.IsValidXml (path, out var jargon), "missing file");
		}

		[TestCase (XmlResultJargon.NUnitV2)]
		[TestCase (XmlResultJargon.NUnitV3)]
		[TestCase (XmlResultJargon.TouchUnit)]
		[TestCase (XmlResultJargon.xUnit)]
		public void IsValidXmlTest (XmlResultJargon jargon)
		{
			var path = CreateResultSample (jargon);
			Assert.IsTrue (resultParser.IsValidXml (path, out var resultJargon), "is valid");
			Assert.AreEqual (jargon, resultJargon, "jargon");
			File.Delete (path);
		}


		[TestCase ("nunit-", XmlResultJargon.NUnitV2)]
		[TestCase ("nunit-", XmlResultJargon.NUnitV2)]
		[TestCase ("nunit-", XmlResultJargon.TouchUnit)]
		[TestCase ("xunit-", XmlResultJargon.xUnit)]
		public void GetXmlFilePathTest (string prefix, XmlResultJargon jargon)
		{
			var orignialPath = "/path/to/a/xml/result.xml";
			var xmlPath = resultParser.GetXmlFilePath (orignialPath, jargon);
			var fileName = Path.GetFileName (xmlPath);
			StringAssert.StartsWith (prefix, fileName, "xml prefix");
		}

		[TestCase (XmlResultJargon.NUnitV3)]
		[TestCase (XmlResultJargon.NUnitV2)]
		[TestCase (XmlResultJargon.xUnit)]
		public void CleanXmlPingTest (XmlResultJargon jargon)
		{
			var path = CreateResultSample (jargon, includePing: true);
			var cleanPath = path + "_clean";
			resultParser.CleanXml (path, cleanPath);
			Assert.IsTrue (resultParser.IsValidXml (cleanPath, out var resultJargon), "is valid");
			Assert.AreEqual (jargon, resultJargon, "jargon");
			File.Delete (path);
			File.Delete (cleanPath);
		}

		[Test]
		public void CleanXmlTouchUnitTest ()
		{
			// similar to CleanXmlPingTest but using TouchUnit, so we do not want to see the extra nodes
			var path = CreateResultSample (XmlResultJargon.TouchUnit, includePing: true);
			var cleanPath = path + "_clean";
			resultParser.CleanXml (path, cleanPath);
			Assert.IsTrue (resultParser.IsValidXml (cleanPath, out var resultJargon), "is valid");
			Assert.AreEqual (XmlResultJargon.NUnitV2, resultJargon, "jargon");
			// load the xml, ensure we do not have the nodes we removed
			var doc = XDocument.Load (cleanPath);
			Assert.IsFalse (doc.Descendants ().Where (e => e.Name == "TouchUnitTestRun").Any (), "TouchUnitTestRun");
			Assert.IsFalse (doc.Descendants ().Where (e => e.Name == "NUnitOutput").Any (), "NUnitOutput");
			File.Delete (path);
			File.Delete (cleanPath);
		}

		[Test]
		public void UpdateMissingDataTest () // only works with NUnitV3
		{
			string appName = "TestApp";
			var path = CreateResultSample (XmlResultJargon.NUnitV3);
			var cleanPath = path + "_clean";
			resultParser.CleanXml (path, cleanPath);
			var updatedXml = path + "_updated";
			var logs = new [] { "/first/path", "/second/path", "/last/path" };
			resultParser.UpdateMissingData (cleanPath, updatedXml, appName, logs);
			// assert that the required info was updated
			Assert.IsTrue (File.Exists (updatedXml), "file exists");
			var doc = XDocument.Load (updatedXml);
			var testSuiteElements = doc.Descendants ().Where (e => e.Name == "test-suite" && e.Attribute ("type")?.Value == "Assembly");
			// assert root node contains the attachments
			var rootNode = testSuiteElements.FirstOrDefault ();
			Assert.IsNotNull (rootNode, "Root node");
			var attachments = rootNode.Descendants ().Where (e => e.Name == "attachment");
			var failureCount = rootNode.Descendants ().Where (e => e.Name == "test-case" && e.Attribute ("result").Value == "Failed").Count ();
			Assert.AreEqual (logs.Length * (failureCount + 1), attachments.Count (), "attachment count");

			// assert that name and full name are present and are the app name
			foreach (var node in testSuiteElements) {
				Assert.AreEqual (appName, node.Attribute ("name").Value, "name");
				Assert.AreEqual (appName, node.Attribute ("fullname").Value, "fullname");
			}
			File.Delete (path);
			File.Delete (cleanPath);
			File.Delete (updatedXml);
		}

		[Test]
		public void GetVSTSFileNameTest ()
		{
			var path = Path.GetTempFileName ();
			var newPath = XmlResultParser.GetVSTSFilename (path);
			StringAssert.StartsWith ("vsts-", Path.GetFileName (newPath));
			File.Delete (path);
		}

		static void ValidateNUnitV2Failure (string src, string appName, string variation, string title, string message, string stderrMessage, string xmlPath, int _)
		{
			// load the doc and ensure that all the data is correct setup
			var doc = XDocument.Load (xmlPath);
			var testResultsNodes = doc.Descendants ().Where (e => e.Name == "test-results");
			Assert.AreEqual (1, testResultsNodes.Count ());
			var rootNode = testResultsNodes.FirstOrDefault ();
			Assert.AreEqual (title, rootNode.Attribute ("name").Value, "title");
			Assert.AreEqual ("1", rootNode.Attribute ("total").Value, "total");
			Assert.AreEqual ("0", rootNode.Attribute ("errors").Value, "errors");
			Assert.AreEqual ("1", rootNode.Attribute ("failures").Value, "failures");
			// ensure we do have a test result with the failure data
			var testResult = doc.Descendants ().Where (e => e.Name == "test-suite" && e.Attribute ("type").Value == "TestFixture");
			Assert.AreEqual (1, testResult.Count (), "test results");
		}

		static void ValidateNUnitV3Failure (string src, string appName, string variation, string title, string message, string stderrMessage, string xmlPath, int attachemntsCount)
		{
			var doc = XDocument.Load (xmlPath);
			// get test-run and verify attrs
			var testResultNodes = doc.Descendants ().Where (e => e.Name == "test-run");
			Assert.AreEqual (1, testResultNodes.Count (), "test-result");
			var testResultNode = testResultNodes.FirstOrDefault ();
			Assert.AreEqual (title, testResultNode.Attribute ("name").Value, "name");
			Assert.AreEqual ("1", testResultNode.Attribute ("testcasecount").Value, "testcasecount");
			Assert.AreEqual ("Failed", testResultNode.Attribute ("result").Value, "result");
			Assert.AreEqual ("1", testResultNode.Attribute ("total").Value, "total");
			Assert.AreEqual ("0", testResultNode.Attribute ("passed").Value, "passed");
			Assert.AreEqual ("1", testResultNode.Attribute ("failed").Value, "failed");
			Assert.AreEqual ("1", testResultNode.Attribute ("asserts").Value, "asserts");
			// important attrs for the import, if they miss, we wont be able to add the files to vsts
			Assert.IsNotNull (testResultNode.Attribute ("run-date").Value);
			Assert.IsNotNull (testResultNode.Attribute ("start-time").Value);
			// get the test-suite and verify the name and fullname are correct
			var testSuite = testResultNode.Descendants ().Where (e => e.Name == "test-suite" && e.Attribute ("type").Value == "TestFixture").FirstOrDefault ();
			Assert.IsNotNull (testSuite, "testSuite");
			Assert.AreEqual (title, testSuite.Attribute ("name").Value, "test suite name");
			Assert.AreEqual (title, testSuite.Attribute ("fullname").Value, "test suite full name");
			// verify the test case
			var testCase = testSuite.Descendants ().Where (e => e.Name == "test-case").FirstOrDefault ();
			Assert.IsNotNull (testCase, "test case");
			Assert.AreEqual ("Failed", testCase.Attribute ("result").Value, "test case result");
			// validate that we do have attachments
			var attachmentsNode = testCase.Descendants ().Where (e => e.Name == "attachments").FirstOrDefault ();
			Assert.IsNotNull (attachmentsNode, "attachments");
			var attachments = attachmentsNode.Descendants ().Where (e => e.Name == "attachment");
			Assert.AreEqual (attachemntsCount, attachments.Count (), "attachments count");
		}

		static void ValidatexUnitFailure (string src, string appName, string variation, string title, string message, string stderrMessage, string xmlPath, int _)
		{
			var doc = XDocument.Load (xmlPath);
			// get the assemlby and validate its attrs
			var assemblies = doc.Descendants ().Where (e => e.Name == "assembly");
			Assert.AreEqual (1, assemblies.Count (), "assemblies count");
			var assemblyNode = assemblies.FirstOrDefault ();
			Assert.AreEqual (title, assemblyNode.Attribute ("name").Value, "name");
			Assert.AreEqual ("1", assemblyNode.Attribute ("total").Value, "total");
			Assert.AreEqual ("1", assemblyNode.Attribute ("failed").Value, "failed");
			Assert.AreEqual ("0", assemblyNode.Attribute ("passed").Value, "passed");
			var collections = assemblyNode.Descendants ().Where (e => e.Name == "collection");
			Assert.AreEqual (1, collections.Count (), "collections count");
			var collectionNode = collections.FirstOrDefault ();
			// assert the collection attrs
			Assert.AreEqual ("1", collectionNode.Attribute ("failed").Value, "failed");
			Assert.AreEqual ("0", collectionNode.Attribute ("passed").Value, "passed");
		}

		[TestCase (XmlResultJargon.NUnitV2)]
		[TestCase (XmlResultJargon.NUnitV3)]
		[TestCase (XmlResultJargon.xUnit)]
		public void GenerateFailureTest (XmlResultJargon jargon)
		{
			var src = "test-case";
			var appName = "MyUnitTest";
			var variation = "Debug";
			var title = "Testing";
			var message = "This is a test";
			var stderrMessage = "Something went very wrong";

			var stderrPath = Path.GetTempFileName ();

			// write the message in the stderrParh that should be read
			using (var writer = new StreamWriter (stderrPath)) {
				writer.WriteLine (stderrMessage);
			}

			// create a path with data in it
			var logs = new Mock<ILogs> ();
			var tmpLogMock = new Mock<ILog> ();
			var xmlLogMock = new Mock<ILog> ();

			var tmpPath = Path.GetTempFileName ();
			var finalPath = Path.GetTempFileName ();

			// create a number of fake logs to be added to the failure
			var logsDir = Path.GetTempFileName ();
			File.Delete (logsDir);
			Directory.CreateDirectory (logsDir);
			var failureLogs = new [] { "first.txt", "second.txt", "last.txt" };

			foreach (var file in failureLogs) {
				var path = Path.Combine (logsDir, file);
				File.WriteAllText (path, "");
			}

			// expect the creation of the two diff xml file logs
			_ = logs.Setup (l => l.Create (It.IsAny<string> (), "Failure Log tmp", null)).Returns (tmpLogMock.Object);
			_ = logs.Setup (l => l.Create (It.IsAny<string> (), LogType.XmlLog.ToString (), null)).Returns (xmlLogMock.Object);
			if (jargon == XmlResultJargon.NUnitV3) {
				_ = logs.Setup (l => l.Directory).Returns (logsDir);
				_ = tmpLogMock.Setup (tmpLog => tmpLog.FullPath).Returns (tmpPath);

			}

			// return the two temp files so that we can later validate that everything is present
			_ = xmlLogMock.Setup (xmlLog => xmlLog.FullPath).Returns (finalPath);

			resultParser.GenerateFailure (logs.Object, src, appName, variation, title, message, stderrPath, jargon);

			// actual assertions do happen in the validation functions
			ValidationMap [jargon] (src, appName, variation, title, message, stderrMessage, finalPath, failureLogs.Length);

			// verify that we are correctly adding the logs
			logs.Verify (l => l.Create (It.IsAny<string> (), It.IsAny<string> (), null), jargon == XmlResultJargon.NUnitV3 ? Times.AtMost (2) : Times.AtMostOnce ());
			if (jargon == XmlResultJargon.NUnitV3) {
				logs.Verify (l => l.Directory, Times.Once);
				tmpLogMock.Verify (l => l.FullPath, Times.AtLeastOnce);

			}

			xmlLogMock.Verify (l => l.FullPath, Times.AtLeastOnce);

			// clean files
			File.Delete (stderrPath);
			File.Delete (tmpPath);
			File.Delete (finalPath);
			Directory.Delete (logsDir, true);
		}

		/// <summary>
		/// https://github.com/xamarin/xamarin-macios/issues/8214
		/// </summary>
		[Test]
		public void Issue8214Test ()
		{
			string expectedResultLine = "Tests run: 2376 Passed: 2301 Inconclusive: 13 Failed: 1 Ignored: 74";
			// get the sample that was added to the issue to validate that we do parse the resuls correctly and copy it to a local
			// path to be parsed
			var name = GetType ().Assembly.GetManifestResourceNames ().Where (a => a.EndsWith ("Issue8214.xml", StringComparison.Ordinal)).FirstOrDefault ();
			var tempPath = Path.GetTempFileName ();
			var destinationFile = Path.GetTempFileName ();
			using (var outputStream = new StreamWriter (tempPath))
			using (var sampleStream = new StreamReader (GetType ().Assembly.GetManifestResourceStream (name))) {
				string line;
				while ((line = sampleStream.ReadLine ()) != null)
					outputStream.WriteLine (line);
			}
			var (resultLine, failed) = resultParser.GenerateHumanReadableResults (tempPath, destinationFile, XmlResultJargon.NUnitV3);
			Assert.IsTrue (failed, "failed");
			Assert.AreEqual (expectedResultLine, resultLine, "resultLine");
			// verify that the destination does contain the result line
			string resultLineInDestinationFile = null;
			using (var resultReader = new StreamReader (destinationFile)) {
				string line;
				while ((line = resultReader.ReadLine ()) != null) {
					if (line.Contains ("Tests run:")) {
						resultLineInDestinationFile = line;
						break;
					}
				}
			}
			Assert.IsNotNull (resultLineInDestinationFile, "result file result line");
			Assert.AreEqual (expectedResultLine, resultLineInDestinationFile, "content result file result line");
		}

		[Test]
		public void NUnitV2GenerateTestReport ()
		{
			using var writer = new StringWriter ();
			using var stream = GetType ().Assembly.GetManifestResourceStream ("Microsoft.DotNet.XHarness.iOS.Shared.Tests.Samples.NUnitV2SampleFailure.xml");
			using var reader = new StreamReader (stream);
			resultParser.GenerateTestReport (writer, reader, XmlResultJargon.NUnitV2);
			var expectedOutput =
@"<div style='padding-left: 15px;'>
<ul>
<li>
ErrorTest1: <div style='padding-left: 15px;'>
Multiline<br/>
error<br/>
message</div>
</li>
<li>
ErrorTest2: SingleLineErrorMessage</li>
<li>
NUnit.Tests.Assemblies.MockTestFixture.FailingTest: Intentional failure</li>
</ul>
</div>
";
			Assert.AreEqual (expectedOutput, writer.ToString (), "Output");
		}
	}
}
