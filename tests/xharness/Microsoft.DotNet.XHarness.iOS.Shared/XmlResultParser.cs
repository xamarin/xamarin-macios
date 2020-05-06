#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;

namespace Microsoft.DotNet.XHarness.iOS.Shared {

	public enum XmlResultJargon {
		TouchUnit,
		NUnitV2,
		NUnitV3,
		xUnit,
		Missing,
	}

	public class XmlResultParser : IResultParser {

		// test if the file is valid xml, or at least, that can be read it.
		public bool IsValidXml (string path, out XmlResultJargon type)
		{
			type = XmlResultJargon.Missing;
			if (!File.Exists (path))
				return false;

			using (var stream = File.OpenText (path)) {
				string line;
				while ((line = stream.ReadLine ()) != null) { // special case when get got the tcp connection
					if (line.Contains ("ping"))
						continue;
					if (line.Contains ("test-run")) { // first element of the NUnitV3 test collection
						type = XmlResultJargon.NUnitV3;
						return true;
					}
					if (line.Contains ("TouchUnitTestRun")) {
						type = XmlResultJargon.TouchUnit;
						return true;
					}
					if (line.Contains ("test-results")) { // first element of the NUnitV3 test collection
						type = XmlResultJargon.NUnitV2;
						return true;
					}
					if (line.Contains ("<assemblies>")) { // first element of the xUnit test collection
						type = XmlResultJargon.xUnit;
						return true;
					}
				}
			}
			return false;
		}

		static (string resultLine, bool failed) ParseNUnitV3Xml (StreamReader stream, StreamWriter writer)
		{
			long testcasecount, passed, failed, inconclusive, skipped;
			bool failedTestRun = false; // result = "Failed"
			testcasecount = passed = failed = inconclusive = skipped = 0L;

			using (var reader = XmlReader.Create (stream)) {
				while (reader.Read ()) {
					if (reader.NodeType == XmlNodeType.Element && reader.Name == "test-run") {
						long.TryParse (reader ["testcasecount"], out testcasecount);
						long.TryParse (reader ["passed"], out passed);
						long.TryParse (reader ["failed"], out failed);
						long.TryParse (reader ["inconclusive"], out inconclusive);
						long.TryParse (reader ["skipped"], out skipped);
						failedTestRun = failed != 0;
					}
					if (reader.NodeType == XmlNodeType.Element && reader.Name == "test-suite" && (reader ["type"] == "TestFixture" || reader ["type"] == "ParameterizedFixture")) {
						var testCaseName = reader ["fullname"];
						writer.WriteLine (testCaseName);
						var time = reader.GetAttribute ("time") ?? "0"; // some nodes might not have the time :/
																		// get the first node and then move in the siblings of the same type
						reader.ReadToDescendant ("test-case");
						do {
							if (reader.Name != "test-case")
								break;
							// read the test cases in the current node
							var status = reader ["result"];
							switch (status) {
							case "Passed":
								writer.Write ("\t[PASS] ");
								break;
							case "Skipped":
								writer.Write ("\t[IGNORED] ");
								break;
							case "Error":
							case "Failed":
								writer.Write ("\t[FAIL] ");
								break;
							case "Inconclusive":
								writer.Write ("\t[INCONCLUSIVE] ");
								break;
							default:
								writer.Write ("\t[INFO] ");
								break;
							}
							writer.Write (reader ["name"]);
							if (status == "Failed") { //  we need to print the message
								reader.ReadToDescendant ("failure");
								reader.ReadToDescendant ("message");
								writer.Write ($" : {reader.ReadElementContentAsString ()}");
								reader.ReadToNextSibling ("stack-trace");
								writer.Write ($" : {reader.ReadElementContentAsString ()}");
							}
							if (status == "Skipped") { // nice to have the skip reason
								reader.ReadToDescendant ("reason");
								reader.ReadToDescendant ("message");
								writer.Write ($" : {reader.ReadElementContentAsString ()}");
							}
							// add a new line
							writer.WriteLine ();
						} while (reader.ReadToNextSibling ("test-case"));
						writer.WriteLine ($"{testCaseName} {time} ms");
					}
				}
			}
			var resultLine = $"Tests run: {testcasecount} Passed: {passed} Inconclusive: {inconclusive} Failed: {failed} Ignored: {skipped + inconclusive}";
			writer.WriteLine (resultLine);
			return (resultLine, failedTestRun);
		}

		static (string resultLine, bool failed) ParseTouchUnitXml (StreamReader stream, StreamWriter writer)
		{
			long total, errors, failed, notRun, inconclusive, ignored, skipped, invalid;
			total = errors = failed = notRun = inconclusive = ignored = skipped = invalid = 0L;

			using (var reader = XmlReader.Create (stream)) {
				while (reader.Read ()) {
					if (reader.NodeType == XmlNodeType.Element && reader.Name == "test-results") {
						long.TryParse (reader ["total"], out total);
						long.TryParse (reader ["errors"], out errors);
						long.TryParse (reader ["failures"], out failed);
						long.TryParse (reader ["not-run"], out notRun);
						long.TryParse (reader ["inconclusive"], out inconclusive);
						long.TryParse (reader ["ignored"], out ignored);
						long.TryParse (reader ["skipped"], out skipped);
						long.TryParse (reader ["invalid"], out invalid);
					}
					if (reader.NodeType == XmlNodeType.Element && reader.Name == "TouchUnitExtraData") {
						// move fwd to get to the CData
						if (reader.Read ())
							writer.Write (reader.Value);
					}
				}
			}
			var passed = total - errors - failed - notRun - inconclusive - ignored - skipped - invalid;
			var resultLine = $"Tests run: {total} Passed: {passed} Inconclusive: {inconclusive} Failed: {failed + errors} Ignored: {ignored + skipped + invalid}";
			return (resultLine, total == 0 || errors != 0 || failed != 0);
		}

		static (string resultLine, bool failed) ParseNUnitXml (StreamReader stream, StreamWriter writer)
		{
			long total, errors, failed, notRun, inconclusive, ignored, skipped, invalid;
			total = errors = failed = notRun = inconclusive = ignored = skipped = invalid = 0L;
			XmlReaderSettings settings = new XmlReaderSettings ();
			settings.ValidationType = ValidationType.None;
			using (var reader = XmlReader.Create (stream, settings)) {
				while (reader.Read ()) {
					if (reader.NodeType == XmlNodeType.Element && reader.Name == "test-results") {
						long.TryParse (reader ["total"], out total);
						long.TryParse (reader ["errors"], out errors);
						long.TryParse (reader ["failures"], out failed);
						long.TryParse (reader ["not-run"], out notRun);
						long.TryParse (reader ["inconclusive"], out inconclusive);
						long.TryParse (reader ["ignored"], out ignored);
						long.TryParse (reader ["skipped"], out skipped);
						long.TryParse (reader ["invalid"], out invalid);
					}
					if (reader.NodeType == XmlNodeType.Element && reader.Name == "test-suite" && (reader ["type"] == "TestFixture" || reader ["type"] == "TestCollection")) {
						var testCaseName = reader ["name"];
						writer.WriteLine (testCaseName);
						var time = reader.GetAttribute ("time") ?? "0"; // some nodes might not have the time :/
																		// get the first node and then move in the siblings of the same type
						reader.ReadToDescendant ("test-case");
						do {
							if (reader.Name != "test-case")
								break;
							// read the test cases in the current node
							var status = reader ["result"];
							switch (status) {
							case "Success":
								writer.Write ("\t[PASS] ");
								break;
							case "Ignored":
								writer.Write ("\t[IGNORED] ");
								break;
							case "Error":
							case "Failure":
								writer.Write ("\t[FAIL] ");
								break;
							case "Inconclusive":
								writer.Write ("\t[INCONCLUSIVE] ");
								break;
							default:
								writer.Write ("\t[INFO] ");
								break;
							}
							writer.Write (reader ["name"]);
							if (status == "Failure" || status == "Error") { //  we need to print the message
								reader.ReadToDescendant ("message");
								writer.Write ($" : {reader.ReadElementContentAsString ()}");
								reader.ReadToNextSibling ("stack-trace");
								writer.Write ($" : {reader.ReadElementContentAsString ()}");
							}
							// add a new line
							writer.WriteLine ();
						} while (reader.ReadToNextSibling ("test-case"));
						writer.WriteLine ($"{testCaseName} {time} ms");
					}
				}
			}
			var passed = total - errors - failed - notRun - inconclusive - ignored - skipped - invalid;
			string resultLine = $"Tests run: {total} Passed: {passed} Inconclusive: {inconclusive} Failed: {failed + errors} Ignored: {ignored + skipped + invalid}";
			writer.WriteLine (resultLine);

			return (resultLine, total == 0 | errors != 0 || failed != 0);
		}

		static (string resultLine, bool failed) ParsexUnitXml (StreamReader stream, StreamWriter writer)
		{
			long total, errors, failed, notRun, inconclusive, ignored, skipped, invalid;
			total = errors = failed = notRun = inconclusive = ignored = skipped = invalid = 0L;
			using (var reader = XmlReader.Create (stream)) {
				while (reader.Read ()) {
					if (reader.NodeType == XmlNodeType.Element && reader.Name == "assembly") {
						long.TryParse (reader ["total"], out var assemblyCount);
						total += assemblyCount;
						long.TryParse (reader ["errors"], out var assemblyErrors);
						errors += assemblyErrors;
						long.TryParse (reader ["failed"], out var assemblyFailures);
						failed += assemblyFailures;
						long.TryParse (reader ["skipped"], out var assemblySkipped);
						skipped += assemblySkipped;
					}
					if (reader.NodeType == XmlNodeType.Element && reader.Name == "collection") {
						var testCaseName = reader ["name"].Replace ("Test collection for ", "");
						writer.WriteLine (testCaseName);
						var time = reader.GetAttribute ("time") ?? "0"; // some nodes might not have the time :/
																		// get the first node and then move in the siblings of the same type
						reader.ReadToDescendant ("test");
						do {
							if (reader.Name != "test")
								break;
							// read the test cases in the current node
							var status = reader ["result"];
							switch (status) {
							case "Pass":
								writer.Write ("\t[PASS] ");
								break;
							case "Skip":
								writer.Write ("\t[IGNORED] ");
								break;
							case "Fail":
								writer.Write ("\t[FAIL] ");
								break;
							default:
								writer.Write ("\t[FAIL] ");
								break;
							}
							writer.Write (reader ["name"]);
							if (status == "Fail") { //  we need to print the message
								reader.ReadToDescendant ("message");
								writer.Write ($" : {reader.ReadElementContentAsString ()}");
								reader.ReadToNextSibling ("stack-trace");
								writer.Write ($" : {reader.ReadElementContentAsString ()}");
							}
							// add a new line
							writer.WriteLine ();
						} while (reader.ReadToNextSibling ("test"));
						writer.WriteLine ($"{testCaseName} {time} ms");
					}
				}
			}
			var passed = total - errors - failed - notRun - inconclusive - ignored - skipped - invalid;
			string resultLine = $"Tests run: {total} Passed: {passed} Inconclusive: {inconclusive} Failed: {failed + errors} Ignored: {ignored + skipped + invalid}";
			writer.WriteLine (resultLine);

			return (resultLine, total == 0 | errors != 0 || failed != 0);
		}

		public string GetXmlFilePath (string path, XmlResultJargon xmlType)
		{
			var fileName = Path.GetFileName (path);
			switch (xmlType) {
			case XmlResultJargon.TouchUnit:
			case XmlResultJargon.NUnitV2:
			case XmlResultJargon.NUnitV3:
				return path.Replace (fileName, $"nunit-{fileName}");
			case XmlResultJargon.xUnit:
				return path.Replace (fileName, $"xunit-{fileName}");
			default:
				return path;
			}
		}

		public void CleanXml (string source, string destination)
		{
			using (var reader = new StreamReader (source))
			using (var writer = new StreamWriter (destination)) {
				string line;
				while ((line = reader.ReadLine ()) != null) {
					if (line.StartsWith ("ping", StringComparison.Ordinal) || line.Contains ("TouchUnitTestRun") || line.Contains ("NUnitOutput") || line.Contains ("<!--")) continue;
					if (line == "") // remove white lines, some files have them
						continue;
					if (line.Contains ("TouchUnitExtraData")) // always last node in TouchUnit
						break;
					writer.WriteLine (line);
				}
			}
		}

		public (string resultLine, bool failed) GenerateHumanReadableResults (string source, string destination, XmlResultJargon xmlType)
		{
			(string resultLine, bool failed) parseData;
			using (var reader = new StreamReader (source))
			using (var writer = new StreamWriter (destination, true)) {
				switch (xmlType) {
				case XmlResultJargon.TouchUnit:
					parseData = ParseTouchUnitXml (reader, writer);
					break;
				case XmlResultJargon.NUnitV2:
					parseData = ParseNUnitXml (reader, writer);
					break;
				case XmlResultJargon.NUnitV3:
					parseData = ParseNUnitV3Xml (reader, writer);
					break;
				case XmlResultJargon.xUnit:
					parseData = ParsexUnitXml (reader, writer);
					break;
				default:
					parseData = ("", true);
					break;
				}
			}
			return parseData;
		}

		static void GenerateNUnitV2TestReport (StreamWriter writer, XmlReader reader)
		{
			while (reader.Read ()) {

				if (reader.NodeType == XmlNodeType.Element && reader.Name == "test-suite" && (reader ["type"] == "TestFixture" || reader ["type"] == "TestCollection")) {

					long.TryParse (reader ["errors"], out var errors);
					long.TryParse (reader ["failed"], out var failed);
					if (errors == 0 && failed == 0)                         // if we do not have any errors, return, nothing to be written here
						return;
					writer.WriteLine ("<div style='padding-left: 15px;'>");
					writer.WriteLine ("<ul>");

					reader.ReadToDescendant ("test-case");
					do {
						if (reader.Name != "test-case")
							break;
						// read the test cases in the current node
						var status = reader ["result"];
						switch (status) { // only interested in errors
						case "Error":
						case "Failure":
							writer.WriteLine ("<li>");
							var test_name = reader ["name"];
							writer.Write (test_name.AsHtml ());
							// read to the message of the error and get it
							reader.ReadToDescendant ("message");
							var message = reader.ReadElementContentAsString ();
							if (!string.IsNullOrEmpty (message)) {
								writer.Write (": ");
								writer.Write (message.AsHtml ());
							}
							writer.WriteLine ("<br />");
							writer.WriteLine ("</li>");
							break;
						}
					} while (reader.ReadToNextSibling ("test-case"));
					writer.WriteLine ("</ul>");
					writer.WriteLine ("</div>");
				}
			}
		}

		static void GenerateNUnitV3TestReport (StreamWriter writer, XmlReader reader)
		{
			List<(string name, string message)> failedTests = new List<(string name, string message)> ();
			while (reader.Read ()) {
				if (reader.NodeType == XmlNodeType.Element && reader.Name == "test-run") {
					long.TryParse (reader ["failed"], out var failed);
					if (failed == 0)
						break;
				}

				if (reader.NodeType == XmlNodeType.Element && reader.Name == "test-suite" && (reader ["type"] == "TestFixture" || reader ["type"] == "ParameterizedFixture")) {
					var testCaseName = reader ["fullname"];
					reader.ReadToDescendant ("test-case");
					do {
						if (reader.Name != "test-case")
							break;
						// read the test cases in the current node
						var status = reader ["result"];
						switch (status) {
						case "Error":
						case "Failed":
							var name = reader ["name"];
							reader.ReadToDescendant ("message");
							var message = reader.ReadElementContentAsString ();
							failedTests.Add ((name, message));
							break;

						}
					} while (reader.ReadToNextSibling ("test-case"));
				}
			}
			if (failedTests.Count > 0) {
				writer.WriteLine ("<div style='padding-left: 15px;'>");
				writer.WriteLine ("<ul>");
				foreach (var (name, message) in failedTests) {
					writer.WriteLine ("<li>");
					writer.Write (name.AsHtml ());
					if (!string.IsNullOrEmpty (message)) {
						writer.Write (": ");
						writer.Write (message.AsHtml ());
					}
					writer.WriteLine ("<br />");
					writer.WriteLine ("</li>");
				}
				writer.WriteLine ("</ul>");
				writer.WriteLine ("</div>");
			}
		}

		static void GeneratexUnitTestReport (StreamWriter writer, XmlReader reader)
		{
			var failedTests = new List<(string name, string message)> ();
			// xUnit is not as nice and does not provide the final result in a top node,
			// we need to look in all the collections and find all the failed tests, this is really bad :/
			while (reader.Read ()) {
				if (reader.NodeType == XmlNodeType.Element && reader.Name == "collection") {
					reader.ReadToDescendant ("test");
					do {
						if (reader.Name != "test")
							break;
						// read the test cases in the current node
						var status = reader ["result"];
						switch (status) {
						case "Fail":
							var name = reader ["name"];
							reader.ReadToDescendant ("message");
							var message = reader.ReadElementContentAsString ();
							failedTests.Add ((name, message));
							break;
						}
					} while (reader.ReadToNextSibling ("test"));
				}
			}
			if (failedTests.Count > 0) {
				writer.WriteLine ("<div style='padding-left: 15px;'>");
				writer.WriteLine ("<ul>");
				foreach (var (name, message) in failedTests) {
					writer.WriteLine ("<li>");
					writer.Write (name.AsHtml ());
					if (!string.IsNullOrEmpty (message)) {
						writer.Write (": ");
						writer.Write (message.AsHtml ());
					}
					writer.WriteLine ("<br />");
					writer.WriteLine ("</li>");
				}
				writer.WriteLine ("</ul>");
				writer.WriteLine ("</div>");
			}
		}

		public void GenerateTestReport (StreamWriter writer, string resultsPath, XmlResultJargon xmlType)
		{
			using (var stream = new StreamReader (resultsPath))
			using (var reader = XmlReader.Create (stream)) {
				switch (xmlType) {
				case XmlResultJargon.NUnitV2:
				case XmlResultJargon.TouchUnit:
					GenerateNUnitV2TestReport (writer, reader);
					break;
				case XmlResultJargon.xUnit:
					GeneratexUnitTestReport (writer, reader);
					break;
				case XmlResultJargon.NUnitV3:
					GenerateNUnitV3TestReport (writer, reader);
					break;
				default:
					writer.WriteLine ($"<span style='padding-left: 15px;'>Could not parse {resultsPath}: Not supported format.</span><br />");
					break;
				}
			}
		}

		// get the file, parse it and add the attachments to the first node found
		public void UpdateMissingData (string source, string destination, string applicationName, IEnumerable<string> attachments)
		{
			// we could do this with a XmlReader and a Writer, but might be to complicated to get right, we pay with performance what we
			// cannot pay with brain cells.
			var doc = XDocument.Load (source);
			var attachmentsElement = new XElement ("attachments");
			foreach (var path in attachments) {
				// we do not add a description, VSTS ignores that :/
				attachmentsElement.Add (new XElement ("attachment",
					new XElement ("filePath", path)));
			}

			var testSuitesElements = doc.Descendants ().Where (e => e.Name == "test-suite" && e.Attribute ("type")?.Value == "Assembly");
			if (!testSuitesElements.Any ())
				return;

			// add the attachments to the first test-suite, this will add the attachmnets to it, which will be added to the test-run, the pipeline
			// SHOULD NOT merge runs, else this upload will be really hard to use. Also, just to one of them, else we have duplicated logs.
			testSuitesElements.FirstOrDefault ().Add (attachmentsElement);

			foreach (var suite in testSuitesElements) {
				suite.SetAttributeValue ("name", applicationName);
				suite.SetAttributeValue ("fullname", applicationName); // docs say just name, but I've seen the fullname instead, docs usually lie
				// add also the attachments to all the failing tests, this will make the life of the person monitoring easier, since
				// he will see the logs directly from the attachment page
				var tests = suite.Descendants ().Where (e => e.Name == "test-case" && e.Attribute ("result").Value == "Failed");
				foreach (var t in tests) {
					t.Add (attachmentsElement);
				}
			}

			doc.Save (destination);
		}

		static void WriteAttributes (XmlWriter writer, params (string name, string data) [] attrs)
		{
			foreach (var (name, data) in attrs) {
				writer.WriteAttributeString (name, data);
			}
		}

		static void WriteNUnitV2TestSuiteAttributes (XmlWriter writer, string title) => WriteAttributes (writer,
			("name", title),
			("executed", "True"),
			("result", "Failure"),
			("success", "False"),
			("time", "0"),
			("asserts", "1"));

		static void WriteNUnitV2TestCase (XmlWriter writer, string title, string message, StreamReader stderr)
		{
			writer.WriteStartElement ("test-case");
			WriteAttributes (writer,
				("name", title),
				("executed", "True"),
				("result", "Failure"),
				("success", "False"),
				("time", "0"),
				("asserts", "1")
			);
			writer.WriteStartElement ("failure");
			writer.WriteStartElement ("message");
			writer.WriteCData (message);
			writer.WriteEndElement (); // message
			writer.WriteStartElement ("stack-trace");
			writer.WriteCData (stderr.ReadToEnd ());
			writer.WriteEndElement (); // stack-trace
			writer.WriteEndElement (); // failure
			writer.WriteEndElement (); // test-case
		}

		static void GenerateNUnitV2Failure (XmlWriter writer, string title, string message, StreamReader stderr)
		{
			writer.WriteStartElement ("test-results");
			WriteAttributes (writer,
				("name", title),
				("total", "1"),
				("errors", "0"),
				("failures", "1"),
				("not-run", "0"),
				("inconclusive", "0"),
				("ignored", "0"),
				("skipped", "0"),
				("invalid", "0"),
				("date", XmlConvert.ToString (DateTime.Now, "yyyy-MM-dd")));

			// we are not writting the env and the cunture info since the VSTS uploader does not care
			writer.WriteStartElement ("test-suite");
			writer.WriteAttributeString ("type", "Assembly");
			WriteNUnitV2TestSuiteAttributes (writer, title);
			writer.WriteStartElement ("results");
			writer.WriteStartElement ("test-suite");
			writer.WriteAttributeString ("type", "TestFixture");
			WriteNUnitV2TestSuiteAttributes (writer, title);
			writer.WriteStartElement ("results");
			WriteNUnitV2TestCase (writer, title, message, stderr);
			writer.WriteEndElement (); // results
			writer.WriteEndElement (); // test-suite TextFixture
			writer.WriteEndElement (); // results
			writer.WriteEndElement (); // test-suite Assembly
			writer.WriteEndElement (); // test-results
		}

		static void WriteNUnitV3TestSuiteAttributes (XmlWriter writer, string title) => WriteAttributes (writer,
			("id", "1"),
			("name", title),
			("testcasecount", "1"),
			("result", "Failed"),
			("time", "0"),
			("total", "1"),
			("passed", "0"),
			("failed", "1"),
			("inconclusive", "0"),
			("skipped", "0"),
			("asserts", "0"));

		static void WriteFailure (XmlWriter writer, string message, StreamReader? stderr = null)
		{
			writer.WriteStartElement ("failure");
			writer.WriteStartElement ("message");
			writer.WriteCData (message);
			writer.WriteEndElement (); // message
			if (stderr != null) {
				writer.WriteStartElement ("stack-trace");
				writer.WriteCData (stderr.ReadToEnd ());
				writer.WriteEndElement (); //stack trace
			}
			writer.WriteEndElement (); // failure
		}

		static void GenerateNUnitV3Failure (XmlWriter writer, string title, string message, StreamReader stderr)
		{
			var date = DateTime.Now;
			writer.WriteStartElement ("test-run");
			// defualt values for the crash
			WriteAttributes (writer,
				("name", title),
				("testcasecount", "1"),
				("result", "Failed"),
				("time", "0"),
				("total", "1"),
				("passed", "0"),
				("failed", "1"),
				("inconclusive", "0"),
				("skipped", "0"),
				("asserts", "1"),
				("run-date", XmlConvert.ToString (date, "yyyy-MM-dd")),
				("start-time", date.ToString ("HH:mm:ss"))
			);
			writer.WriteStartElement ("test-suite");
			writer.WriteAttributeString ("type", "Assembly");
			WriteNUnitV3TestSuiteAttributes (writer, title);
			WriteFailure (writer, "Child test failed");
			writer.WriteStartElement ("test-suite");
			WriteAttributes (writer,
				("name", title),
				("fullname", title),
				("type", "TestFixture"),
				("testcasecount", "1"),
				("result", "Failed"),
				("time", "0"),
				("total", "1"),
				("passed", "0"),
				("failed", "1"),
				("inconclusive", "0"),
				("skipped", "0"),
				("asserts", "1"));
			writer.WriteStartElement ("test-case");
			WriteAttributes (writer,
				("id", "1"),
				("name", title),
				("fullname", title),
				("result", "Failed"),
				("time", "0"),
				("asserts", "1"));
			WriteFailure (writer, message, stderr);
			writer.WriteEndElement (); // test-case
			writer.WriteEndElement (); // test-suite = TestFixture
			writer.WriteEndElement (); // test-suite = Assembly
			writer.WriteEndElement (); // test-run
		}

		static void GeneratexUnitFailure (XmlWriter writer, string title, string message, StreamReader stderr)
		{
			writer.WriteStartElement ("assemblies");
			writer.WriteStartElement ("assembly");
			WriteAttributes (writer,
				("name", title),
				("environment", "64-bit .NET Standard [collection-per-class, non-parallel]"),
				("test-framework", "xUnit.net 2.4.1.0"),
				("run-date", XmlConvert.ToString (DateTime.Now, "yyyy-MM-dd")),
				("total", "1"),
				("passed", "0"),
				("failed", "1"),
				("skipped", "0"),
				("time", "0"),
				("errors", "0"));
			writer.WriteStartElement ("collection");
			WriteAttributes (writer,
				("total", "1"),
				("passed", "0"),
				("failed", "1"),
				("skipped", "0"),
				("name", title),
				("time", "0"));
			writer.WriteStartElement ("test");
			WriteAttributes (writer,
				("name", title),
				("type", "TestApp"),
				("method", "Run"),
				("time", "0"),
				("result", "Fail"));
			WriteFailure (writer, message, stderr);
			writer.WriteEndElement (); // test
			writer.WriteEndElement (); // collection
			writer.WriteEndElement (); // assembly
			writer.WriteEndElement (); // assemblies
		}

		static void GenerateFailureXml (string destination, string title, string message, StreamReader stderrReader, XmlResultJargon jargon)
		{
			XmlWriterSettings settings = new XmlWriterSettings { Indent = true };
			using (var stream = File.CreateText (destination))
			using (var xmlWriter = XmlWriter.Create (stream, settings)) {
				xmlWriter.WriteStartDocument ();
				switch (jargon) {
				case XmlResultJargon.NUnitV2:
					GenerateNUnitV2Failure (xmlWriter, title, message, stderrReader);
					break;
				case XmlResultJargon.NUnitV3:
					GenerateNUnitV3Failure (xmlWriter, title, message, stderrReader);
					break;
				case XmlResultJargon.xUnit:
					GeneratexUnitFailure (xmlWriter, title, message, stderrReader);
					break;
				}
				xmlWriter.WriteEndDocument ();
			}
		}

		public void GenerateFailure (ILogs logs, string source, string appName, string variation, string title,
			string message, StreamReader stderr, XmlResultJargon jargon)
		{
			// VSTS does not provide a nice way to report build errors, create a fake
			// test result with a failure in the case the build did not work
			var failureLogXml = logs.Create ($"vsts-nunit-{source}-{Helpers.Timestamp}.xml", LogType.XmlLog.ToString ());
			if (jargon == XmlResultJargon.NUnitV3) {
				var failureXmlTmp = logs.Create ($"nunit-{source}-{Helpers.Timestamp}.tmp", "Failure Log tmp");
				GenerateFailureXml (failureXmlTmp.FullPath, title, message, stderr, jargon);
				// add the required attachments and the info of the application that failed to install
				var failure_logs = Directory.GetFiles (logs.Directory).Where (p => !p.Contains ("nunit")); // all logs but ourself
				UpdateMissingData (failureXmlTmp.FullPath, failureLogXml.FullPath, $"{appName} {variation}", failure_logs);
			} else {
				GenerateFailureXml (failureLogXml.FullPath, title, message, stderr, jargon);
			}
		}
		public void GenerateFailure (ILogs logs, string source, string appName, string variation, string title, string message, string stderrPath, XmlResultJargon jargon)
		{
			using var stderrReader = new StreamReader (stderrPath);
			GenerateFailure (logs, source, appName, variation, title, message, stderrReader, jargon);
		}

		public static string GetVSTSFilename (string filename)
			=> Path.Combine (Path.GetDirectoryName (filename), $"vsts-{Path.GetFileName (filename)}");
	}
}
