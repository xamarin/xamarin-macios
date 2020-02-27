using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using xharness;

using static xharness.XmlResultParser;

namespace Xharness.Tests {

	[TestFixture]
	public class XmlResultParserTests {

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
			Assert.IsFalse (XmlResultParser.IsValidXml (path, out var jargon), "missing file");
		}

		[TestCase (XmlResultJargon.NUnitV2)]
		[TestCase (XmlResultJargon.NUnitV3)]
		[TestCase (XmlResultJargon.TouchUnit)]
		[TestCase (XmlResultJargon.xUnit)]
		public void IsValidXmlTest (XmlResultJargon jargon)
		{
			var path = CreateResultSample (jargon);
			Assert.IsTrue (XmlResultParser.IsValidXml (path, out var resultJargon), "is valid");
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
			var xmlPath = XmlResultParser.GetXmlFilePath (orignialPath, jargon);
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
			XmlResultParser.CleanXml (path, cleanPath);
			Assert.IsTrue (XmlResultParser.IsValidXml (cleanPath, out var resultJargon), "is valid");
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
			XmlResultParser.CleanXml (path, cleanPath);
			Assert.IsTrue (XmlResultParser.IsValidXml (cleanPath, out var resultJargon), "is valid");
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
			XmlResultParser.CleanXml (path, cleanPath);
			var updatedXml = path + "_updated";
			var logs = new [] { "/first/path", "/second/path", "/last/path" };
			XmlResultParser.UpdateMissingData (cleanPath, updatedXml, appName, logs);
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
	}
}
