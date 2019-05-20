#!/usr/bin/env /Library/Frameworks/Mono.framework/Commands/csharp
using System.IO;
using System.Text;
using System.Xml;

var args = Environment.GetCommandLineArgs ();
// first arg is "/Library/Frameworks/Mono.framework/Versions/4.8.0/lib/mono/4.5/csharp.exe"
// second arg the script itself
// then comes the ones we care about, the path with all the xunit and nunit files

if (args.Length < 3) {
    Console.WriteLine ($"Missing path for the directory that contains the test results files.");
    Environment.Exit (-1);
}

var testsDirectory = args [2];
if (!Directory.Exists (testsDirectory)) {
    Console.WriteLine ($"Could not find directory {testsDirectory}");
    Environment.Exit (-1);
}

var testFilePattern = "test-*.xml";
try {
    var files = Directory.GetFiles (testsDirectory, testFilePattern, SearchOption.AllDirectories);
    if (files.Length == 0) {
        Console.WriteLine ($"Not files were found with the pattern '{testFilePattern}'");
        Environment.Exit (-1);
    }

    if (files.Length == 1) {
        // we only have a single tests, so we do not need to do anything
        Console.WriteLine ("Only one file was found, doing nothing.");
        Environment.Exit (0);
    }

    // get the total errors, failures, etc.. from all the files, this will be used to later
    // write the main test-results node using the addition of all of them
    var errors = 0L;
    var inconclusive = 0L;
    var ignored = 0L;
    var invalid = 0L;
    var notRun = 0L;
    var total = 0L;
    var failures = 0L;
    var skipped = 0L;
    var date = DateTime.Today;
    TimeSpan? time = null;

    foreach ( var filePath in files) {
        using (var stream = new StreamReader (filePath))
        using (var reader = XmlReader.Create (stream)) {
            while (reader.Read ()) {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "test-results") {
                    // read the attrs and get out of the loop
                    errors += Convert.ToInt64 (reader ["errors"]);
                    inconclusive += Convert.ToInt64 (reader ["inconclusive"]);
                    ignored += Convert.ToInt64 (reader ["ignored"]);
                    invalid += Convert.ToInt64 (reader ["invalid"]);
                    notRun += Convert.ToInt64 (reader ["not-run"]);
                    total += Convert.ToInt64 (reader ["total"]);
                    failures += Convert.ToInt64 (reader ["failures"]);
                    skipped += Convert.ToInt64 (reader ["skipped"]);
                    if (time.HasValue)
                        time = time.Value.Add (TimeSpan.Parse (reader ["time"]));
                    else
                        time = TimeSpan.Parse (reader ["time"]);
                    break; // while loop exit
                }
            }
        }
    }

    // create a writer for the final solution
    var outPutPath = "tests-grouped-result.xml";

    if (File.Exists (outPutPath))
        File.Delete (outPutPath);

    using (var writer = File.CreateText (outPutPath)) {
        writer.WriteLine ("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        writer.WriteLine ($"<test-results name=\"Test results\" errors=\"{errors}\" inconclusive=\"{inconclusive}\" ignored=\"{ignored}\" invalid=\"{invalid}\" not-run=\"{notRun}\" date=\"{date}\" time=\"{time.Value}\" total=\"{total}\" failures=\"{failures}\" skipped=\"{skipped}\">");
        writer.WriteLine ("<environment os-version=\"unknown\" platform=\"unknown\" cwd=\"unknown\" machine-name=\"unknown\" user=\"unknown\" user-domain=\"unknown\" nunit-version=\"xUnit.net 2.4.0.4049\" clr-version=\"64-bit .NET Standard [collection-per-class, non-parallel]\"></environment>");
        writer.WriteLine ("<culture-info current-culture=\"unknown\" current-uiculture=\"unknown\" />");
        foreach (var filePath in files) {
            using (var stream = new StreamReader (filePath))
            using (var reader = XmlReader.Create (stream)) {
                while (reader.Read ()) {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "test-suite" && (reader ["type"] == "Assemblies" || reader ["type"] == "TestSuite")) {
                        // add the current node to the writer
                        writer.WriteLine (reader.ReadOuterXml ());
                    }
                }
            }
        }
        writer.WriteLine ("</test-results>");
    }
    Console.WriteLine ($"File merge completed to file {outPutPath}");

} catch (Exception e) {
    Console.WriteLine ($"Got {e.Message}");
    Environment.Exit (-1);
}


Environment.Exit (0);
