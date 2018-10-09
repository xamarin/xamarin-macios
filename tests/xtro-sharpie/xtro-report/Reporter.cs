using System;
using System.Collections.Generic;
using System.IO;

namespace Extrospection {
	class Reporter {

		static string InputDirectory { get; set; }
		static string ReportFolder { get; set; }

		static List<string> Frameworks = new List<string> ();

		static readonly string [] Platforms = new [] { "iOS", "tvOS", "watchOS", "macOS" };

		public static bool ProcessFramework (string framework)
		{
			bool data = false;
			// merge the shared and specialized ignore data into a single html page
			List<string> ignore = new List<string> ();
			ignore.Add ($"<h1>{framework}</h1>"); 
			var filename = Path.Combine (InputDirectory, $"common-{framework}.ignore");
			if (File.Exists (filename)) {
				data = true;
				ignore.Add ("<h2>Common (shared) ignored results</h2>");
				ignore.Add ("<xmp>");
				foreach (var line in File.ReadAllLines (filename)) {
					ignore.Add (line);
				}
				ignore.Add ("</xmp>");
			}
			foreach (var platform in Platforms) {
				filename = Path.Combine (InputDirectory, $"{platform}-{framework}.ignore");
				if (File.Exists (filename)) {
					data = true;
					ignore.Add ($"<h2>{platform} specific ignored results</h2>");
					ignore.Add ("<xmp>");
					foreach (var line in File.ReadAllLines (filename)) {
						ignore.Add (line);
					}
					ignore.Add ("</xmp>");
				}
			}
			var output = Path.Combine (ReportFolder, framework) + ".ignore.html";
			File.WriteAllLines (output, ignore);
			return data;
		}

		public static int ProcessFile (string filename)
		{
			if (!File.Exists (filename))
				return 0;
			int count = 0;
			var output = Path.Combine (ReportFolder, Path.GetFileName (filename)) + ".html";
			var name = Path.GetFileNameWithoutExtension (filename);
			List<string> html = new List<string> ();
			html.Add ($"<html><head><title>{name}</title></head>");
			html.Add ($"<body><h1>{name}</h1><xmp>");
			foreach (var line in File.ReadAllLines (filename)) {
				html.Add (line); 
				if ((line.Length > 0) && (line [0] == '!'))
					count++;
			}
			html.Add ("</xmp></body></html>");
			File.WriteAllLines (output, html);
			return count;
		}

		static void AddFramework (string file)
		{
			var filename = Path.GetFileNameWithoutExtension (file);
			var fx = filename.Substring (filename.IndexOf ('-') + 1);
			if (!Frameworks.Contains (fx))
				Frameworks.Add (fx); 
		}

		public static int Main (string [] args)
		{
			InputDirectory = args.Length == 0 ? "." : args [0];

			// collapse the ignored entries on jenkins bots - focus in on what's needs fixing (for the PR) and the work 'to do'
			bool full = String.IsNullOrEmpty (Environment.GetEnvironmentVariable ("JENKINS_SERVER_COOKIE"));

			int width = 100 / ((full ? 2 : 1) + (full ? 3 : 2) * Platforms.Length);

			var allfiles = new List<string> ();

			ReportFolder = args.Length > 1 ? args [1] : "report";
			Directory.CreateDirectory (ReportFolder);
			var log = new StreamWriter (Path.Combine (ReportFolder, "index.html"));

			log.WriteLine ("<html><head><title>Extrospection results</title></head>");
			log.WriteLine ("<body><h1>Extrospection results</h1>");

			log.WriteLine ("<table border='0' cellpadding='4' cellspacing='0'>");

			log.WriteLine ("<thead>");
			log.WriteLine ("<tr>");
			log.WriteLine ("<td rowspan='3' bgcolor='lightgrey'>Frameworks</td>");
			if (full)
				log.WriteLine ($"<td align='center' bgcolor='green' colspan='{Platforms.Length + 1}'>REVIEWED (ignored)</td>"); 
			log.WriteLine ($"<td align='center' bgcolor='red' colspan='{Platforms.Length}'>FIXME (unclassified)</td>"); 
			log.WriteLine ($"<td align='center' bgcolor='orange' colspan='{Platforms.Length}'>TODO (milestone)</td>"); 
			log.WriteLine ("</tr>");

			log.WriteLine ("<tr>");
			if (full)
				log.WriteLine ($"<td align='center' bgcolor='green' width='{width}%'>Common</td>");
			foreach (var platform in Platforms) {
				if (full)
					log.WriteLine ($"<td align='center' bgcolor='green' width='{width}%'>{platform}</td>");
				var files = Directory.GetFiles (InputDirectory, $"{platform}-*.ignore");
				foreach (var file in files) {
					AddFramework (file);
				}
			}
			foreach (var platform in Platforms) {
				log.WriteLine ($"<td align='center' bgcolor='red' width='{width}%'>{platform}</td>");
				var files = Directory.GetFiles (InputDirectory, $"{platform}-*.unclassified");
				foreach (var file in files) {
					allfiles.Add (file);
					AddFramework (file);
				}
				var todos = Directory.GetFiles (InputDirectory, $"{platform}-*.todo");
				foreach (var file in todos) {
					AddFramework (file);
				}
			}
			foreach (var platform in Platforms)
				log.WriteLine ($"<td align='center' bgcolor='orange' width='{width}%'>{platform}</td>");
			log.WriteLine ("</tr>");

			var cols = (full ? 3 : 2) * Platforms.Length + (full ? 1 : 0);
			log.WriteLine ("<tr>");
			log.WriteLine ($"<td colspan='{cols + 1}' cellspanning='4'></td>");
			log.WriteLine ("</tr>");

			log.WriteLine ("</thead>");

			var ignored = new int [Platforms.Length + 1];
			var unclassified = new int [Platforms.Length];
			var todo = new int [Platforms.Length];
			int errors = 0;

			Frameworks.Sort ();
			foreach (var fx in Frameworks) {
				if (Helpers.Filter (fx))
					continue;
				log.WriteLine ("<tr>");
				log.Write ("<td>");
				if (!full && ProcessFramework (fx))
					log.Write ($"<a href=\"{fx}.ignore.html\">{fx}</a>");
				else
					log.Write (fx);
				log.WriteLine ("</td>");
				if (full) {
					string filename = $"common-{fx}.ignore";
					var count = ProcessFile (filename);
					log.Write ("<td align='center' ");
					if (count < 1)
						log.Write ("bgcolor='lightgreen'>-</td>");
					else
						log.Write ($"bgcolor='green'><a href=\"{filename}.html\">{count}</a>");
					log.WriteLine ("</td>");
					ignored [0] += count;
					for (int i = 0; i < Platforms.Length; i++) {
						filename = $"{Platforms [i]}-{fx}.ignore";
						count = ProcessFile (filename);
						log.Write ("<td align='center' ");
						if (count < 1)
							log.Write ("bgcolor='lightgreen'>-");
						else
							log.Write ($"bgcolor='green'><a href=\"{filename}.html\">{count}</a>");
						log.WriteLine ("</td>");
						ignored [i + 1] += count;
					}
				}
				for (int i = 0; i < Platforms.Length; i++) {
					string filename = $"{Platforms [i]}-{fx}.unclassified";
					var count = ProcessFile (Path.Combine (InputDirectory, filename));
					log.Write ("<td align='center'");
					if (count < 1)
						log.Write (" bgcolor='salmon'>-");
					else
						log.Write ($"bgcolor='red'><a href=\"{filename}.html\">{count}</a>");
					log.WriteLine ("</td>");
					unclassified [i] += count;
					errors += count;
				}
				for (int i = 0; i < Platforms.Length; i++) {
					string filename = $"{Platforms [i]}-{fx}.todo";
					var count = ProcessFile (Path.Combine (InputDirectory, filename));
					log.Write ("<td align='center' ");
					if (count <= 0)
						log.Write ("bgcolor='peachpuff'>-");
					else
						log.Write ($"bgcolor='orange'><a href=\"{filename}.html\">{count}</a>");
					log.WriteLine ("</td>");
					todo [i] += count;
				}
				log.WriteLine ("</tr>");
			}
			log.WriteLine ("<tfoot>");
			log.WriteLine ("<tr>");
			log.WriteLine ($"<td colspan='{cols}' cellspanning='4'></td>");
			log.WriteLine ("</tr>");
			log.WriteLine ("<tr>");
			log.WriteLine ("<td>Total (per platform)</td>");
			var total_ignored = 0;
			if (full) {
				for (int i = 0; i < Platforms.Length + 1; i++) {
					log.Write ("<td align='center' ");
					var count = ignored [i];
					if (count <= 0)
						log.Write ("bgcolor='lightgreen'>-</td>");
					else
						log.Write ($"bgcolor='green'>{count}</a>");
					log.WriteLine ("</td>");
					total_ignored += count;
				}
			}
			var total_unclassfied = 0;
			for (int i = 0; i < Platforms.Length; i++) {
				log.Write ("<td align='center' ");
				var count = unclassified [i];
				if (count <= 0)
					log.Write ("bgcolor='salmon'>-</td>");
				else
					log.Write ($"bgcolor='red'>{count}</a>");
				log.WriteLine ("</td>");
				total_unclassfied += count;
			}
			var total_todo = 0;
			for (int i = 0; i < Platforms.Length; i++) {
				log.Write ("<td align='center' ");
				var count = todo [i];
				if (count <= 0)
					log.Write ("bgcolor='peachpuff'>-</td>");
				else
					log.Write ($"bgcolor='orange'>{count}</a>");
				log.WriteLine ("</td>");
				total_todo += count;
			}
			log.WriteLine ("</tr>");
			log.WriteLine ("<tr>");
			log.WriteLine ("<td>Total (per state)</td>");
			if (full)
				log.WriteLine ($"<td align='center' bgcolor='green' colspan='5'>{total_ignored}</td>");
			log.WriteLine ($"<td align='center' bgcolor='red' colspan='4'>{total_unclassfied}</td>");
			log.WriteLine ($"<td align='center' bgcolor='orange' colspan='4'>{total_todo}</td>");
			log.WriteLine ("</tr>");

			log.WriteLine ("<tr>");
			log.WriteLine ($"<td colspan='{cols}' cellspanning='4'></td>");
			log.WriteLine ("</tr>");

			log.WriteLine ("<tr>");
			log.WriteLine ($"<td align='center' colspan='{cols + 1}' ");
			if (total_unclassfied == 0)
				log.WriteLine ($"bgcolor='lightgreen'>SUCCESS");
			else
				log.WriteLine ($"bgcolor='red'>FAILURE");
			log.WriteLine ($"</td>");
			log.WriteLine ("</tr>");

			log.WriteLine ("</tfoot>");
			log.WriteLine ("</table>");

			log.WriteLine ("</body>");
			log.WriteLine ("</html>");
			log.Flush ();

			Console.WriteLine ($"@MonkeyWrench: SetSummary: {errors} unclassified found.");
			return errors == 0 ? 0 : 1;
		}
	}
}
