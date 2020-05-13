using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev.Tasks;
using Xamarin.Localization.MSBuild;

namespace Xamarin.iOS.Tasks
{
	public abstract class GetFilesTaskBase : XamarinTask
	{
		[Required]
		public string Path { get; set; }

		public string Pattern { get; set; }

		public string Option { get; set; }

		public string Exclude { get; set; }

		[Output]
		public ITaskItem[] Files { get; set; }

		public override bool Execute ()
		{
			var path = Path.Replace ('\\', '/').TrimEnd ('/');
			var exclude = new HashSet<string> ();
			var items = new List<ITaskItem> ();
			IEnumerable<string> files;
			SearchOption option;

			if (!string.IsNullOrEmpty (Option)) {
				if (!Enum.TryParse (Option, out option)) {
					Log.LogError (MSBStrings.E0050, Option);
					return false;
				}
			} else {
				option = SearchOption.TopDirectoryOnly;
			}

			if (!Directory.Exists (path)) {
				Files = items.ToArray ();

				return !Log.HasLoggedErrors;
			}

			if (!string.IsNullOrEmpty (Pattern))
				files = Directory.EnumerateFiles (path, Pattern, option);
			else
				files = Directory.EnumerateFiles (path, "*.*", option);

			if (!string.IsNullOrEmpty (Exclude)) {
				foreach (var rpath in Exclude.Split (new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries)) {
					var full = System.IO.Path.Combine (path, rpath.Replace ('\\', '/'));
					exclude.Add (full);
				}
			}

			foreach (var file in files) {
				if (!exclude.Contains (file))
					items.Add (new TaskItem (file));
			}

			Files = items.ToArray ();

			return !Log.HasLoggedErrors;
		}
	}
}
