/*
# Expected files:
# Expected files:
#
#  $Env:BUILD_SOURCESDIRECTORY/xamarin-macios/jenkins-results/windows-remote-dotnet-tests.trx"
#  $(Build.SourcesDirectory)/xamarin-macios/jenkins-results/windows-dotnet-tests.trx"
#  $Env:BUILD_SOURCESDIRECTORY/xamarin-macios/jenkins-results/windows/bgen-tests/results.trx
#
#  $Env:BUILD_SOURCESDIRECTORY\xamarin-macios\jenkins-results\windows-remote-logs.zip
#
*/

using System.IO;
using System.Text;
using System.Xml;

public class Program {
	static string GetOutcomeColor (string outcome)
	{
		switch (outcome.ToLower ()) {
		case "passed":
		case "completed":
			return "green";
		default:
			return "red";
		}
	}

	static string FormatHtml (string text)
	{
		text = text.Replace ("\r", "");
		text = text.Replace ("&", "&amp;");
		text = text.Replace ("<", "&lt;");
		text = text.Replace (">", "&gt;");
		text = text.Replace ("  ", "&nbsp;&nbsp;");
		text = text.Replace (" &nbsp;", "&nbsp;&nbsp;");
		text = text.Replace ("&nbsp; ", "&nbsp;&nbsp;");
		text = text.Replace ("\n", "<br />\n");
		return text;
	}

	static string GetSourcesDirectory ()
	{
		var pwd = Environment.CurrentDirectory!;
		var dir = pwd;
		while (true) {
			if (Directory.Exists (Path.Combine (dir, ".git")))
				return dir;
			var parentDir = Path.GetDirectoryName (dir);
			if (string.IsNullOrEmpty (parentDir) || parentDir == dir || parentDir.Length <= 2)
				throw new Exception ($"Unable to find a .git subdirectory in any directory up the directory hierarchy from {pwd}");
			dir = parentDir;
		}
		throw new Exception ($"Unable to find a .git subdirectory in any directory up the directory hierarchy from {pwd}");
	}

	public static int Main (string [] args)
	{
		var sourcesDirectory = GetSourcesDirectory ();
		var allTestsSucceeded = true;
		var outputDirectory = Path.Combine (sourcesDirectory, "jenkins-results");
		var indexFile = Path.Combine (outputDirectory, "index.html");
		var summaryFile = Path.Combine (sourcesDirectory, "tests", "TestSummary.md");

		var trxFiles = new [] {
			new { Name = "Remote .NET tests", TestResults = Path.Combine (outputDirectory, "windows-remote-dotnet-tests.trx") },
			new { Name = "Local .NET tests", TestResults = Path.Combine (outputDirectory, "windows-dotnet-tests.trx") },
			new { Name = "Generator tests", TestResults = Path.Combine (outputDirectory, "windows", "bgen-tests", "results.trx") },
		};

		var extraFiles = new []{
			Path.Combine(outputDirectory, "windows-remote-logs.zip"),
		};

		var indexContents = new StringBuilder ();
		var summaryContents = new StringBuilder ();

		indexContents.AppendLine ($"<!DOCTYPE html>");
		indexContents.AppendLine ($"<html>");
		indexContents.AppendLine ($"  <head>");
		indexContents.AppendLine ($"    <meta charset=\"utf-8\"/>");
		indexContents.AppendLine ($"    <title>Test results</title>");
		indexContents.AppendLine ($"    <style>");
		indexContents.AppendLine ($"      .pdiv {{");
		indexContents.AppendLine ($"        display: table;");
		indexContents.AppendLine ($"        padding-top: 10px;");
		indexContents.AppendLine ($"      }}");
		indexContents.AppendLine ($"    </style>");
		indexContents.AppendLine ($"  </head>");
		indexContents.AppendLine ($"  <body>");
		indexContents.AppendLine ($"    <h1>Test results</h1>");
		foreach (var trx in trxFiles) {
			var name = trx.Name;
			var path = trx.TestResults;
			string? outcome;
			var messageLines = new List<string> ();

			try {
				var xml = new XmlDocument ();
				xml.Load (path);
				outcome = xml.SelectSingleNode ("/*[local-name() = 'TestRun']/*[local-name() = 'ResultSummary']")?.Attributes? ["outcome"]?.Value;
				if (outcome is null) {
					outcome = $"Could not find outcome in trx file {path}";
				} else {
					var failedTests = xml.SelectNodes ("/*[local-name() = 'TestRun']/*[local-name() = 'Results']/*[local-name() = 'UnitTestResult'][@outcome != 'Passed']")?.Cast<XmlNode> ();
					if (failedTests?.Any () == true) {
						messageLines.Add ("        <ul>");
						foreach (var node in failedTests) {
							var testName = node.Attributes? ["testName"]?.Value ?? "<unknown test name>";
							var testOutcome = node.Attributes? ["outcome"]?.Value ?? "<unknown test outcome>";
							var testMessage = node.SelectSingleNode ("*[local-name() = 'Output']/*[local-name() = 'ErrorInfo']/*[local-name() = 'Message']")?.InnerText;
							if (string.IsNullOrEmpty (testMessage)) {
								messageLines.Add ($"        <li>{testName} (<span style='color: {GetOutcomeColor (testOutcome)}'>{testOutcome}</span>)</li>");
							} else {
								messageLines.Add ($"        <li>{testName} (<span style='color: {GetOutcomeColor (testOutcome)}'>{testOutcome}</span>)</li>");
								messageLines.Add ($"        <div class='pdiv' style='margin-left: 20px;'>");
								messageLines.Add (FormatHtml (testMessage));
								messageLines.Add ($"        </div>");
							}
						}
						messageLines.Add ("        </ul>");
						allTestsSucceeded = false;
					} else if (outcome != "Completed" && outcome != "Passed") {
						messageLines.Add ($"    Failed to find any test failures in the trx file {path}");
					}
				}
				var htmlPath = Path.ChangeExtension (path, "html");
				if (File.Exists (htmlPath)) {
					var relativeHtmlPath = Path.GetRelativePath (outputDirectory, htmlPath);
					messageLines.Add ($"Html results: <a href='{relativeHtmlPath}'>{Path.GetFileName (relativeHtmlPath)}</a>");
				}
			} catch (Exception e) {
				outcome = "Failed to parse test results";
				messageLines.Add ($"<div>{FormatHtml (e.ToString ())}</div>");
				allTestsSucceeded = false;
			}

			indexContents.AppendLine ($"    <div class='pdiv'><span>{name} (</span><span style='color: {GetOutcomeColor (outcome)}'>{outcome}</span><span>)</span></div>");
			if (messageLines.Any ()) {
				indexContents.AppendLine ("    <div class='pdiv' style='margin-left: 20px;'>");
				foreach (var line in messageLines)
					indexContents.AppendLine ($"      {line}");
				indexContents.AppendLine ("    </div>");
			}
		}
		var existingExtraFiles = extraFiles.Where (File.Exists).ToList ();
		if (existingExtraFiles.Any ()) {
			indexContents.AppendLine ($"    <div class='pdiv'>Extra files:</div>");
			indexContents.AppendLine ($"    <ul>");
			foreach (var ef in existingExtraFiles) {
				var relative = Path.GetRelativePath (outputDirectory, ef);
				indexContents.AppendLine ($"      <li><a href='{relative}'>{Path.GetFileName (ef)}</a></li>");
			}
			indexContents.AppendLine ($"    </ul>");
		}
		indexContents.AppendLine ($"  </body>");
		indexContents.AppendLine ($"</html>");

		if (allTestsSucceeded) {
			summaryContents.AppendLine ($"# :tada: All {trxFiles.Length} tests passed :tada:");
		} else {
			summaryContents.AppendLine ($"# :tada: All {trxFiles.Length} tests passed :tada:");
		}


		Directory.CreateDirectory (outputDirectory);
		File.WriteAllText (indexFile, indexContents.ToString ());
		File.WriteAllText (summaryFile, summaryContents.ToString ());

		Console.WriteLine ($"Created {indexFile} successfully.");
		Console.WriteLine (indexContents);
		Console.WriteLine ($"Created {summaryFile} successfully.");
		Console.WriteLine (summaryContents);
		Console.WriteLine ($"All tests succeeded: {allTestsSucceeded}");
		return allTestsSucceeded ? 0 : 1;
	}
}
