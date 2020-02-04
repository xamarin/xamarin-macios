using System;
using System.IO;
using System.Xml;

namespace xharness {
	public static class XmlResultParser {

		public enum XmlResultType {
			TouchUnit,
			NUnit,
			xUnit,
		}

		// test if the file is valid xml, or at least, that can be read it.
		public static bool IsXml (string filePath)
		{
			if (!File.Exists (filePath))
				return false;

			using (var stream = File.OpenText (filePath)) {
				string line;
				while ((line = stream.ReadLine ()) != null) { // special case when get got the tcp connection
					if (line.Contains ("ping"))
						continue;
					return line.StartsWith ("<", StringComparison.Ordinal);
				}
			}
			return false;
		}

		public static XmlResultType GetXmlType (string filePath)
		{
			using (var stream = new StreamReader (filePath)) {
				var resultType = XmlResultType.TouchUnit; // default
									  // more fun, the first like of the stream, is a ping from the application to the tcp server, and that will not be parsable as
									  // xml, advance the reader one line.
				string line;
				while ((line = stream.ReadLine ()) != null) {
					if (line.Contains ("TouchUnitTestRun")) {
						resultType = XmlResultType.TouchUnit;
						break;
					}
					if (line.Contains ("nunit-version"))
						resultType = XmlResultType.NUnit;
					if (line.Contains ("xUnit")) {
						resultType = XmlResultType.xUnit;
						break;
					}
				}
				return resultType;
			}
		}

		static (string resultLine, bool failed) ParseTouchUnitXml (StreamReader stream, StreamWriter writer)
		{
			long total, errors, failed, notRun, inconclusive, ignored, skipped, invalid;
			total = errors = failed = notRun = inconclusive = ignored = skipped = invalid = 0L;

			using (var reader = XmlReader.Create (stream)) {
				while (reader.Read ()) {
					if (reader.NodeType == XmlNodeType.Element && reader.Name == "test-results") {
						total = long.Parse (reader ["total"]);
						errors = long.Parse (reader ["errors"]);
						failed = long.Parse (reader ["failures"]);
						notRun = long.Parse (reader ["not-run"]);
						inconclusive = long.Parse (reader ["inconclusive"]);
						ignored = long.Parse (reader ["ignored"]);
						skipped = long.Parse (reader ["skipped"]);
						invalid = long.Parse (reader ["invalid"]);
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
						total = long.Parse (reader ["total"]);
						errors = long.Parse (reader ["errors"]);
						failed = long.Parse (reader ["failures"]);
						notRun = long.Parse (reader ["not-run"]);
						inconclusive = long.Parse (reader ["inconclusive"]);
						ignored = long.Parse (reader ["ignored"]);
						skipped = long.Parse (reader ["skipped"]);
						invalid = long.Parse (reader ["invalid"]);
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
						total += long.Parse (reader ["total"]);
						errors += long.Parse (reader ["errors"]);
						failed += long.Parse (reader ["failed"]);
						skipped += long.Parse (reader ["skipped"]);
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

		public static string GetXmlFilePath (string path, XmlResultType xmlType)
		{

			using (var streamReaderTmp = new StreamReader (path)) {
				var fileName = Path.GetFileName (path);
				var newFilename = "";
				switch (xmlType) {
				case XmlResultType.TouchUnit:
				case XmlResultType.NUnit:
					newFilename = path.Replace (fileName, $"nunit-{fileName}");
					break;
				case XmlResultType.xUnit:
					newFilename = path.Replace (fileName, $"xunit-{fileName}");
					break;
				}
				return newFilename;
			}
		}

		public static void CleanXml (string source, string destination)
		{
			using (var reader = new StreamReader (source))
			using (var writer = new StreamWriter (destination)) {
				string line;
				while ((line = reader.ReadLine ()) != null) {
					if (line.StartsWith ("ping", StringComparison.InvariantCulture) || line.Contains ("TouchUnitTestRun") || line.Contains ("NUnitOutput") || line.Contains ("<!--")) {
						continue;
					}
					if (line.Contains ("TouchUnitExtraData")) // always last node in TouchUnit
						break;
					writer.WriteLine (line);
				}
			}
		}

		public static (string resultLine, bool failed) GenerateHumanReadableResults (string source, string destination, XmlResultType xmlType)
		{
			(string resultLine, bool failed) parseData;
			using (var reader = new StreamReader (source)) 
			using (var writer = new StreamWriter (destination, true)) {
				switch (xmlType) {
				case XmlResultType.TouchUnit:
					parseData = ParseTouchUnitXml (reader, writer);
					break;
				case XmlResultType.NUnit:
					parseData = ParseNUnitXml (reader, writer);
					break;
				case XmlResultType.xUnit:
					parseData = ParsexUnitXml (reader, writer);
					break;
				default:
					parseData = ("", true);
					break;
				}
			}
			return parseData;
		}
	}
}
