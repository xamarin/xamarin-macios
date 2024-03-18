using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev.Tasks;
using Xamarin.Localization.MSBuild;
using Xamarin.Messaging.Build.Client;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Xamarin.MacDev.Tasks {
	public class GetDirectories : XamarinTask, ITaskCallback, ICancelableTask {
		[Required]
		public string Path { get; set; }

		public string Pattern { get; set; }

		public string Option { get; set; }

		public string Exclude { get; set; }

		[Output]
		public ITaskItem [] Directories { get; set; }

		public override bool Execute ()
		{
			if (ShouldExecuteRemotely ())
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			var path = Path.Replace ('\\', '/').TrimEnd ('/');
			var exclude = new HashSet<string> ();
			var items = new List<ITaskItem> ();
			IEnumerable<string> dirs;
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
				Directories = items.ToArray ();

				return !Log.HasLoggedErrors;
			}

			if (!string.IsNullOrEmpty (Pattern))
				dirs = Directory.EnumerateDirectories (path, Pattern, option);
			else
				dirs = Directory.EnumerateDirectories (path, "*.*", option);

			if (!string.IsNullOrEmpty (Exclude)) {
				foreach (var rpath in Exclude.Split (new char [] { ';' }, StringSplitOptions.RemoveEmptyEntries)) {
					var full = System.IO.Path.Combine (path, rpath.Replace ('\\', '/'));
					exclude.Add (full);
				}
			}

			foreach (var dir in dirs) {
				if (!exclude.Contains (dir))
					items.Add (new TaskItem (dir));
			}

			Directories = items.ToArray ();

			return !Log.HasLoggedErrors;
		}

		public IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied () => Enumerable.Empty<ITaskItem> ();

		public bool ShouldCopyToBuildServer (ITaskItem item) => false;

		public bool ShouldCreateOutputFile (ITaskItem item) => false;

		public void Cancel ()
		{
			if (ShouldExecuteRemotely ())
				BuildConnection.CancelAsync (BuildEngine4).Wait ();
		}
	}
}
