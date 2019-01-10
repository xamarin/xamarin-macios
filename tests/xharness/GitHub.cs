using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Xml;
using System.Text;
using Xamarin;

namespace xharness
{
	public static class GitHub
	{
		public static string GetPullRequestTargetBranch (Harness harness, int pull_request)
		{
			if (pull_request <= 0)
				return string.Empty;

			var info = DownloadPullRequestInfo (harness, pull_request);
			if (info.Length == 0)
				return string.Empty;

			using (var reader = JsonReaderWriterFactory.CreateJsonReader (info, new XmlDictionaryReaderQuotas ())) {
				var doc = new XmlDocument ();
				doc.Load (reader);
				return doc.SelectSingleNode ("/root/base/ref").InnerText;
			}
		}

		public static IEnumerable<string> GetLabels (Harness harness, int pull_request)
		{
			var info = DownloadPullRequestInfo (harness, pull_request);
			using (var reader = JsonReaderWriterFactory.CreateJsonReader (info, new XmlDictionaryReaderQuotas ())) {
				var doc = new XmlDocument ();
				doc.Load (reader);
				var rv = new List<string> ();
				foreach (XmlNode node in doc.SelectNodes ("/root/labels/name")) {
					rv.Add (node.InnerText);
				}
				return rv;
			}
		}

		public static IEnumerable<string> GetModifiedFiles (Harness harness, int pull_request)
		{
			var path = Path.Combine (harness.LogDirectory, "pr" + pull_request + "-files.log");
			if (!File.Exists (path)) {
				var rv = GetModifiedFilesLocally (harness, pull_request);
				if (rv == null || rv.Count () == 0) {
					rv = GetModifiedFilesRemotely (harness, pull_request);
					if (rv == null)
						rv = new string [] { };
				}

				File.WriteAllLines (path, rv.ToArray ());
				return rv;
			}

			return File.ReadAllLines (path);
		}

		static IEnumerable<string> GetModifiedFilesRemotely (Harness harness, int pull_request)
		{
			var path = Path.Combine (harness.LogDirectory, "pr" + pull_request + "-remote-files.log");
			if (!File.Exists (path)) {
				Directory.CreateDirectory (harness.LogDirectory);
				using (var client = new WebClient ()) {
					var rv = new List<string> ();
					var url = $"https://api.github.com/repos/xamarin/xamarin-macios/pulls/{pull_request}/files?per_page=100"; // 100 items per page is max
					do {
						byte [] data;
						try {
							client.Headers.Add (HttpRequestHeader.UserAgent, "xamarin");
							data = client.DownloadData (url);
						} catch (WebException we) {
							harness.Log ("Could not load pull request files: {0}\n{1}", we, new StreamReader (we.Response.GetResponseStream ()).ReadToEnd ());
							File.WriteAllText (path, string.Empty);
							return new string [] { };
						}
						var reader = JsonReaderWriterFactory.CreateJsonReader (data, new XmlDictionaryReaderQuotas ());
						var doc = new XmlDocument ();
						doc.Load (reader);
						foreach (XmlNode node in doc.SelectNodes ("/root/item/filename")) {
							rv.Add (node.InnerText);
						}

						url = null;

						var link = client.ResponseHeaders ["Link"];
						try {
							if (link != null) {
								var ltIdx = link.IndexOf ('<');
								var gtIdx = link.IndexOf ('>', ltIdx + 1);
								while (ltIdx >= 0 && gtIdx > ltIdx) {
									var linkUrl = link.Substring (ltIdx + 1, gtIdx - ltIdx - 1);
									if (link [gtIdx + 1] != ';')
										break;
									var commaIdx = link.IndexOf (',', gtIdx + 1);
									string rel;
									if (commaIdx != -1) {
										rel = link.Substring (gtIdx + 3, commaIdx - gtIdx - 3);
									} else {
										rel = link.Substring (gtIdx + 3);
									}

									if (rel == "rel=\"next\"") {
										url = linkUrl;
										break;
									}

									if (commaIdx == -1)
										break;

									ltIdx = link.IndexOf ('<', commaIdx);
									gtIdx = link.IndexOf ('>', ltIdx + 1);
								}
							}
						} catch (Exception e) {
							harness.Log ("Could not paginate github response: {0}: {1}", link, e.Message);
						}
					} while (url != null);
					File.WriteAllLines (path, rv.ToArray ());
					return rv;
				}
			}

			return File.ReadAllLines (path);
		}

		static IEnumerable<string> GetModifiedFilesLocally (Harness harness, int pull_request)
		{
			var base_commit = $"origin/pr/{pull_request}/merge^";
			var head_commit = $"origin/pr/{pull_request}/merge";

			harness.Log ("Fetching modified files for commit range {0}..{1}", base_commit, head_commit);

			if (string.IsNullOrEmpty (head_commit) || string.IsNullOrEmpty (base_commit))
				return null;

			using (var git = new Process ()) {
				git.StartInfo.FileName = "git";
				git.StartInfo.Arguments = $"diff-tree --no-commit-id --name-only -r {base_commit}..{head_commit}";
				var output = new StringWriter ();
				var rv = git.RunAsync (harness.HarnessLog, output, output).Result;
				if (rv.Succeeded)
					return output.ToString ().Split (new char [] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

				harness.Log ("Could not fetch commit range:");
				harness.Log (output.ToString ());

				return null;
			}
		}

		static byte[] DownloadPullRequestInfo (Harness harness, int pull_request)
		{
			var path = Path.Combine (harness.LogDirectory, "pr" + pull_request + ".log");
			if (!File.Exists (path)) {
				Directory.CreateDirectory (harness.LogDirectory);
				using (var client = new WebClient ()) {
					byte [] data;
					try {
						client.Headers.Add (HttpRequestHeader.UserAgent, "xamarin");
						data = client.DownloadData ($"https://api.github.com/repos/xamarin/xamarin-macios/pulls/{pull_request}");
						File.WriteAllBytes (path, data);
						return data;
					} catch (WebException we) {
						harness.Log ("Could not load pull request info: {0}\n{1}", we, new StreamReader (we.Response.GetResponseStream ()).ReadToEnd ());
						File.WriteAllText (path, string.Empty);
						return new byte [0];
					}
				}
			}
			return File.ReadAllBytes (path);
		}
	}
}
