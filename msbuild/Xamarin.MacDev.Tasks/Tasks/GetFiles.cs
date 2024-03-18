using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev.Tasks;
using Xamarin.Localization.MSBuild;
using Xamarin.Messaging.Build.Client;

#nullable enable

namespace Xamarin.MacDev.Tasks {
	public abstract class GetFiles : XamarinTask, ITaskCallback, ICancelableTask {
		[Required]
		public string Path { get; set; } = string.Empty;

		public string Pattern { get; set; } = string.Empty;

		public string Option { get; set; } = string.Empty;

		public string Exclude { get; set; } = string.Empty;

		[Output]
		public ITaskItem [] Files { get; set; } = Array.Empty<ITaskItem> ();

		public override bool Execute ()
		{
			if (ShouldExecuteRemotely ())
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

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
				foreach (var rpath in Exclude.Split (new char [] { ';' }, StringSplitOptions.RemoveEmptyEntries)) {
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
