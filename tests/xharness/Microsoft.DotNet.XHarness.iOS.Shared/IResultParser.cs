using System.Collections.Generic;
using System.IO;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;

namespace Microsoft.DotNet.XHarness.iOS.Shared {

	// Interface that represents an object that know how to parse results and generate timeout/crash/build errors so
	// that CIs like VSTS and helix can parse them.
	public interface IResultParser {

		// generates a xml result that will consider to be an error by the CI. Allows to catch errors in cases in which we are not talking about a test
		// failure perse but the situation in which the app could not be built, timeout or crashed.
		void GenerateFailure (ILogs logs, string source, string appName, string variation, string title, string message, string stderrPath, XmlResultJargon jargon);
		
		void GenerateFailure (ILogs logs, string source, string appName, string variation, string title, string message, StreamReader stderrReader, XmlResultJargon jargon);

		// updates a given xml result to contain a list of attachments. This is useful for CI to be able to add logs as part of the attachments of a failing test.
		void UpdateMissingData (string source, string destination, string applicationName, IEnumerable<string> attachments);

		// ensures that the given path contains a valid xml result and set the type of xml jargon found in the file.
		bool IsValidXml (string path, out XmlResultJargon type);

		// takes a xml file and removes any extra data that makes the test result not to be a pure xml result for the given jargon.
		void CleanXml (string source, string destination);

		// Returns the path to be used for the given jargon.
		string GetXmlFilePath (string path, XmlResultJargon xmlType);

		// parses the xml of the given jargon, create a human readable result and returns a result line with the summary of what was
		// parsed.
		(string resultLine, bool failed) GenerateHumanReadableResults (string source, string destination, XmlResultJargon xmlType);

		// generated a human readable test report.
		void GenerateTestReport (StreamWriter writer, string resultsPath, XmlResultJargon xmlType);
	}
}
