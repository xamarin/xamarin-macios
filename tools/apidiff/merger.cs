using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

#nullable enable

class Merger {

	static string GetVersion (string line)
	{
		var end = line.LastIndexOf ('"');
		var start = line.LastIndexOf ('"', end - 1) + 1;
		return line.Substring (start, end - start);
	}

	public static void Process (string platform, string path, string os, string? destinationPath)
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
			if (new FileInfo (file).Length == 0)
				continue;
			// skip everything before and including title (single #) from each file, we already have one
			string? foundTitle = null;
			foreach (var line in File.ReadAllLines (file)) {
				if (foundTitle is not null) {
					content.WriteLine (line);
					if (line == "#### Type Changed: ObjCRuntime.Constants") {
						lookForVersion = true;
					} else if (line.StartsWith ("#### ")) {
						lookForVersion = false;
					}
					if (lookForVersion) {
						if (line.StartsWith ("-public const string Version = ", StringComparison.Ordinal))
							from = GetVersion (line);
						if (line.StartsWith ("+public const string Version = ", StringComparison.Ordinal))
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
		var hasVersions = from != "unknown" && to != "unknown";

		// https://github.com/MicrosoftDocs/xamarin-docs/blob/live/contributing-guidelines/template.md#file-name
		var filename = $"{os}-{from}-{to}".Replace ('.', '-').ToLowerInvariant () + ".md";
		byte []? digest = null;
		using (var md = SHA256.Create ())
			digest = md.ComputeHash (Encoding.UTF8.GetBytes (filename));
		// (not cryptographically) unique (but good enough) for each filename - so document remains with the same id when it's updated/regenerated
		var guid = new Guid (digest [0..16]);

		var headers = new StringWriter ();
		var title = $"{platform} SDK API diff";
		if (hasVersions) {
			title += $": {from} vs {to}";
		}
		// https://github.com/MicrosoftDocs/xamarin-docs/blob/live/contributing-guidelines/template.md#metadata
		headers.WriteLine ("---");
		headers.WriteLine ($"title: \"{title}\"");
		if (hasVersions) {
			headers.WriteLine ($"description: List of API changes between {platform} versions {from} and {to}.");
		} else {
			headers.WriteLine ($"description: List of API changes for {platform}.");
		}
		headers.WriteLine ($"author: spouliot");
		headers.WriteLine ($"ms.author: sepoulio");
		headers.WriteLine ($"ms.date: {DateTime.Now.ToString ("d", new CultureInfo ("en-US"))}");
		headers.WriteLine ($"ms.topic: article");
		headers.WriteLine ($"ms.assetid: {guid.ToString ().ToLowerInvariant ()}");
		headers.WriteLine ($"ms.prod: xamarin");
		headers.WriteLine ("---");
		headers.WriteLine ();
		headers.WriteLine ($"# {title}");
		headers.WriteLine ();

		var filePath = destinationPath;
		File.WriteAllText (filePath, headers.ToString ());

		var alldiffs = content.ToString ();
		if (alldiffs.Length == 0)
			alldiffs = "No changes were found between both versions."; // should not happen for releases (versions change)
		File.AppendAllText (filePath, alldiffs);
	}

	public static int Main (string [] args)
	{
		try {
			// if calling merger.cs from tools/apidiff/Makefile, we want to pass in the destinationPath 'arg [3]'
			// to make sure diffs go to the correct location
			Process (args [0], args [1], args [2], args.Length > 3 ? args [3] : null);
			return 0;
		} catch (Exception e) {
			Console.WriteLine (e);
			return 1;
		}
	}
}
