using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace xharness {
	public static class XmlResultParser {

		public enum Jargon {
			TouchUnit,
			NUnit,
			xUnit,
			Missing,
		}

		// test if the file is valid xml, or at least, that can be read it.
		public static bool IsValidXml (string path, out Jargon type)
		{
			type = Jargon.Missing;
			if (!File.Exists (path))
				return false;

			using (var stream = File.OpenText (path)) {
				string line;
				while ((line = stream.ReadLine ()) != null) { // special case when get got the tcp connection
					if (line.Contains ("ping"))
						continue;
					if (line.Contains ("TouchUnitTestRun")) {
						type = Jargon.TouchUnit;
						return true;
					}
					if (line.Contains ("nunit-version")) {
						type = Jargon.NUnit;
						return true;
					}
					if (line.Contains ("xUnit")) {
						type = Jargon.xUnit;
						return true;
					}
				}
			}
			return false;
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
						long.TryParse (reader ["skipped"], out skipped );
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

		public static string GetXmlFilePath (string path, Jargon xmlType)
		{
			var fileName = Path.GetFileName (path);
			switch (xmlType) {
			case Jargon.TouchUnit:
			case Jargon.NUnit:
				return path.Replace (fileName, $"nunit-{fileName}");
			case Jargon.xUnit:
				return path.Replace (fileName, $"xunit-{fileName}");
			default:
				return path;
			}
		}

		public static void CleanXml (string source, string destination)
		{
			using (var reader = new StreamReader (source))
			using (var writer = new StreamWriter (destination)) {
				string line;
				while ((line = reader.ReadLine ()) != null) {
					if (line.StartsWith ("ping", StringComparison.Ordinal) || line.Contains ("TouchUnitTestRun") || line.Contains ("NUnitOutput") || line.Contains ("<!--")) {
						continue;
					}
					if (line == "") // remove white lines, some files have them
						continue;
					if (line.Contains ("TouchUnitExtraData")) // always last node in TouchUnit
						break;
					writer.WriteLine (line);
				}
			}
		}

		public static (string resultLine, bool failed) GenerateHumanReadableResults (string source, string destination, Jargon xmlType)
		{
			(string resultLine, bool failed) parseData;
			using (var reader = new StreamReader (source)) 
			using (var writer = new StreamWriter (destination, true)) {
				switch (xmlType) {
				case Jargon.TouchUnit:
					parseData = ParseTouchUnitXml (reader, writer);
					break;
				case Jargon.NUnit:
					parseData = ParseNUnitXml (reader, writer);
					break;
				case Jargon.xUnit:
					parseData = ParsexUnitXml (reader, writer);
					break;
				default:
					parseData = ("", true);
					break;
				}
			}
			return parseData;
		}

		static void GenerateNUnitTestReport (StreamWriter writer, XmlReader reader)
		{
			while (reader.Read ()) {

				if (reader.NodeType == XmlNodeType.Element && reader.Name == "test-suite" && (reader ["type"] == "TestFixture" || reader ["type"] == "TestCollection")) {

					long.TryParse (reader ["errors"], out var errors);
					long.TryParse (reader ["failed"], out var failed);
					if (errors == 0 && failed == 0) {
						// if we do not have any errors, return, nothing to be written here
						return;
					}
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
							writer.Write (test_name.AsHtmlFormat ());
							// read to the message of the error and get it
							reader.ReadToDescendant ("message");
							var message = reader.ReadElementContentAsString ();
							if (!string.IsNullOrEmpty (message)) {
								writer.Write (": ");
								writer.Write (message.AsHtmlFormat ());
							}
							writer.WriteLine ("<br />");
							writer.WriteLine ("</li>");
							break;
						}
					} while (reader.ReadToNextSibling ("test-case"));
				}
			}
		}

		static void GeneratexUnitTestReport (StreamWriter writer, XmlReader reader)
		{
			var failedTests = new List<(string name, string message)> ();
			// xUnit is not as nice and does not provide the final result in a top node,
			// we need to look in all the collections and find all the failed thests, this is really bad :/
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
							var name =  reader ["name"];
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
					writer.Write (name.AsHtmlFormat ());
					if (!string.IsNullOrEmpty (message)) {
						writer.Write (": ");
						writer.Write (message.AsHtmlFormat ());
					}
				}
				writer.WriteLine ("<br />");
				writer.WriteLine ("</li>");
			}
		}

		public static void GenerateTestReport (StreamWriter writer, string resultsPath, Jargon xmlType)
		{
			using (var stream = new StreamReader (resultsPath))
			using (var reader = XmlReader.Create (stream)) {
				switch (xmlType) {
				case Jargon.NUnit:
				case Jargon.TouchUnit:
					GenerateNUnitTestReport (writer, reader);
					break;
				case Jargon.xUnit:
					GeneratexUnitTestReport (writer, reader);
					break;
				default:
					writer.WriteLine ($"<span style='padding-left: 15px;'>Could not parse {resultsPath}: Not supported format.</span><br />");
					break;
				}
			}
		}
	}
}
