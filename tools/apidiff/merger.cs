using System;
using System.Globalization;
using System.IO;

class Merger {

	static string GetVersion (string line)
	{
		var end = line.LastIndexOf ('"');
		var start = line.LastIndexOf ('"', end - 1) + 1;
		return line.Substring (start, end - start);
	}

	public static void Process (string platform, string path, string os)
	{
		if (!Directory.Exists (path))
			throw new DirectoryNotFoundException (path);

		var content = new StringWriter ();
		var files = Directory.GetFileSystemEntries (path, "*.md");
		Array.Sort (files);
		string from = "unknown";
		string to = "unknown";
		bool lookForVersion = false;
		foreach (var file in files) {
			// skip everything before and including title (single #) from each file, we already have one
			string foundTitle = null;
			foreach (var line in File.ReadAllLines (file)) {
				if (foundTitle != null) {
					content.WriteLine (line);
					if (line == "#### Type Changed: ObjCRuntime.Constants") {
						lookForVersion = true;
					} else if (line.StartsWith ("#### ")) {
						lookForVersion = false;
					}
					if (lookForVersion) {
						if (line.StartsWith ("-public const string Version = "))
							from = GetVersion (line);
						if (line.StartsWith ("+public const string Version = "))
							to = GetVersion (line);
					}
				} else if (line.StartsWith ("## ")) {
					// everything with ## becomes an entry in the ToC
					foundTitle = line.Substring (3);
					content.WriteLine ($"<a name=\"{foundTitle}\" />");
					content.WriteLine (); // required empty line
					content.WriteLine (line);
				}
			}
		}

		var headers = new StringWriter ();
		var title = $"{platform} SDK API diff: {from} vs {to}";
		// https://github.com/MicrosoftDocs/xamarin-docs/blob/live/contributing-guidelines/template.md#metadata
		headers.WriteLine ("---");
		headers.WriteLine ($"title: \"{title}\"");
		headers.WriteLine ($"description: List of API changes between {platform} versions {from} and {to}.");
		headers.WriteLine ($"author: spouliot");
		headers.WriteLine ($"ms.author: sepoulio");
		headers.WriteLine ($"ms.date: {DateTime.Now.ToString ("d", new CultureInfo ("en-US"))}");
		headers.WriteLine ($"ms.topic: article");
		headers.WriteLine ($"ms.assetid: {Guid.NewGuid ().ToString ().ToLowerInvariant ()}");
		headers.WriteLine ($"ms.prod: xamarin");
		headers.WriteLine ("---");
		headers.WriteLine ();
		headers.WriteLine ($"# {title}");
		headers.WriteLine ();

		// https://github.com/MicrosoftDocs/xamarin-docs/blob/live/contributing-guidelines/template.md#file-name
		var filename = $"{os}-{from}-{to}".Replace ('.', '-').ToLowerInvariant () + ".md";
		File.WriteAllText (filename, headers.ToString ());

		var alldiffs = content.ToString ();
		if (alldiffs.Length == 0)
			alldiffs = "No changes were found between both versions."; // should not happen for releases (versions change)
		File.AppendAllText (filename, alldiffs);
		Console.WriteLine ($"@MonkeyWrench: AddFile: {Path.GetFullPath (filename)}");
	}

	public static int Main (string [] args)
	{
		try {
			Process (args [0], args [1], args [2]);
			return 0;
		} catch (Exception e) {
			Console.WriteLine (e);
			return 1;
		}
	}
}