using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Xml;
using System.Text;

namespace xharness
{
	public static class GitHub
	{
		public static IEnumerable<string> GetLabels (Harness harness, int pull_request)
		{
			var path = Path.Combine (harness.LogDirectory, "pr" + pull_request + "-labels.log");
			if (!File.Exists (path)) {
				Directory.CreateDirectory (harness.LogDirectory);
				using (var client = new WebClient ()) {
					// FIXME: github returns results in pages of 30 elements
					byte [] data;
					try {
						client.Headers.Add (HttpRequestHeader.UserAgent, "xamarin");
						data = client.DownloadData ($"https://api.github.com/repos/xamarin/xamarin-macios/issues/{pull_request}/labels");
					} catch (WebException we) {
						harness.Log ("Could not load pull request labels: {0}\n{1}", we, new StreamReader (we.Response.GetResponseStream ()).ReadToEnd ());
						File.WriteAllText (path, string.Empty);
						return new string [] { };
					}
					var reader = JsonReaderWriterFactory.CreateJsonReader (data, new XmlDictionaryReaderQuotas ());
					var doc = new XmlDocument ();
					doc.Load (reader);
					var rv = new List<string> ();
					foreach (XmlNode node in doc.SelectNodes ("/root/item/name")) {
						rv.Add (node.InnerText);
					}
					File.WriteAllLines (path, rv.ToArray ());
					return rv;
				}
			}

			return File.ReadAllLines (path);
		}
	}
}
