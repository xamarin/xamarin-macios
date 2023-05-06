using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Xml;
using Microsoft.DotNet.XHarness.Common.Execution;
using Microsoft.DotNet.XHarness.Common.Logging;

namespace Xharness {
	/// <summary>
	/// API to interact with the vcs used in the project. It is used to indentify those modified files in a PR and
	/// choose which tests to execute.
	/// </summary>
	public interface IVersionControlSystem {

		string GetPullRequestTargetBranch (int pullRequest);
		IEnumerable<string> GetLabels (int pullRequest);
		IEnumerable<string> GetModifiedFiles (int pullRequest);
	}

	public class GitHub : IVersionControlSystem {

		readonly IHarness harness;
		readonly IProcessManager processManager;

		public GitHub (IHarness harness, IProcessManager processManager)
		{
			if (harness is null)
				throw new ArgumentNullException (nameof (harness));
			if (processManager is null)
				throw new ArgumentNullException (nameof (processManager));
			this.harness = harness;
			this.processManager = processManager;
		}

		static WebClient CreateClient ()
		{
			var client = new WebClient ();
			client.Headers.Add (HttpRequestHeader.UserAgent, "xamarin");
			var xharness_github_token = Environment.GetEnvironmentVariable ("GITHUB_TOKEN");
			if (!string.IsNullOrEmpty (xharness_github_token))
				client.Headers.Add (HttpRequestHeader.Authorization, xharness_github_token);
			return client;
		}

		public string GetPullRequestTargetBranch (int pullRequest)
		{
			if (pullRequest <= 0)
				return string.Empty;

			var info = DownloadPullRequestInfo (pullRequest);
			if (info.Length == 0)
				return string.Empty;

			using (var reader = JsonReaderWriterFactory.CreateJsonReader (info, new XmlDictionaryReaderQuotas ())) {
				var doc = new XmlDocument ();
				doc.Load (reader);
				return doc.SelectSingleNode ("/root/base/ref").InnerText;
			}
		}

		public IEnumerable<string> GetLabels (int pullRequest)
		{
			var info = DownloadPullRequestInfo (pullRequest);
			using (var reader = JsonReaderWriterFactory.CreateJsonReader (info, new XmlDictionaryReaderQuotas ())) {
				var doc = new XmlDocument ();
				doc.Load (reader);
				var rv = new List<string> ();
				foreach (XmlNode node in doc.SelectNodes ("/root/labels/item/name")) {
					rv.Add (node.InnerText);
				}
				return rv;
			}
		}

		public IEnumerable<string> GetModifiedFiles (int pullRequest)
		{
			var path = Path.Combine (harness.LogDirectory, "pr" + pullRequest + "-files.log");
			if (!File.Exists (path)) {
				var rv = GetModifiedFilesLocally (pullRequest);
				if (rv is null || rv.Count () == 0) {
					rv = GetModifiedFilesRemotely (pullRequest);
					if (rv is null)
						rv = new string [] { };
				}

				File.WriteAllLines (path, rv.ToArray ());
				return rv;
			}

			return File.ReadAllLines (path);
		}

		IEnumerable<string> GetModifiedFilesRemotely (int pullRequest)
		{
			var path = Path.Combine (harness.LogDirectory, "pr" + pullRequest + "-remote-files.log");
			if (!File.Exists (path)) {
				Directory.CreateDirectory (harness.LogDirectory);
				using (var client = CreateClient ()) {
					var rv = new List<string> ();
					var url = $"https://api.github.com/repos/xamarin/xamarin-macios/pulls/{pullRequest}/files?per_page=100"; // 100 items per page is max
					do {
						byte [] data;
						try {
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
							if (link is not null) {
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
					} while (url is not null);
					File.WriteAllLines (path, rv.ToArray ());
					return rv;
				}
			}

			return File.ReadAllLines (path);
		}

		IEnumerable<string> GetModifiedFilesLocally (int pullRequest)
		{
			var base_commit = $"origin/pull/{pullRequest}/merge^";
			var head_commit = $"origin/pull/{pullRequest}/merge";

			harness.Log ("Fetching modified files for commit range {0}..{1}", base_commit, head_commit);

			if (string.IsNullOrEmpty (head_commit) || string.IsNullOrEmpty (base_commit))
				return null;

			using (var git = new Process ()) {
				git.StartInfo.FileName = "git";
				git.StartInfo.Arguments = $"diff-tree --no-commit-id --name-only -r {base_commit}..{head_commit}";
				var output = new MemoryLog () {
					Timestamp = false // ensure we do not add the timestap or the logic for the file check will be hard and having it adds no value
				};
				var rv = processManager.RunAsync (git, harness.HarnessLog, stdoutLog: output, stderrLog: output).Result;
				if (rv.Succeeded)
					return output.ToString ().Split (new char [] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

				harness.Log ("Could not fetch commit range:");
				harness.Log (output.ToString ());

				return null;
			}
		}

		byte [] DownloadPullRequestInfo (int pullRequest)
		{
			var path = Path.Combine (harness.LogDirectory, "pr" + pullRequest + ".log");
			if (!File.Exists (path)) {
				Directory.CreateDirectory (harness.LogDirectory);
				using (var client = CreateClient ()) {
					byte [] data;
					try {
						data = client.DownloadData ($"https://api.github.com/repos/xamarin/xamarin-macios/pulls/{pullRequest}");
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
