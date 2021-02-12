using System.Collections.Generic;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Xamarin.Messaging.Build.Client;

namespace Xamarin.iOS.Tasks
{
	public class GetPropertyValue : GetPropertyValueTaskBase, ITaskCallback, ICancelableTask
	{
		public override bool Execute ()
		{
			if (!string.IsNullOrEmpty (SessionId))
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			return base.Execute ();
		}

		public bool ShouldCopyToBuildServer (ITaskItem item) => true;

		public bool ShouldCreateOutputFile (ITaskItem item) => false;

		public IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied ()
		{
			var result = new List<ITaskItem> ();

			if (FileName != null) {
				var rootDirectoryName = Path.GetDirectoryName (FileName.ItemSpec);
				var rootFileName = Path.GetFileNameWithoutExtension (FileName.ItemSpec);
				var rootFileNameExtension = Path.GetExtension (FileName.ItemSpec);

				var beforeFileName = Path.Combine (rootDirectoryName, string.Concat (rootFileName, ".", "Before", rootFileNameExtension));

				if (File.Exists (beforeFileName)) {
					result.Add (new TaskItem (beforeFileName));
				}

				var afterFileName = Path.Combine (rootDirectoryName, string.Concat (rootFileName, ".", "After", rootFileNameExtension));

				if (File.Exists (afterFileName)) {
					result.Add (new TaskItem (afterFileName));
				}

				result.Add (new TaskItem (Path.Combine (rootDirectoryName, "Xamarin.Shared.props")));
			}

			return result.ToArray ();
		}

		public void Cancel ()
		{
			if (!string.IsNullOrEmpty (SessionId))
				BuildConnection.CancelAsync (SessionId, BuildEngine4).Wait ();
		}
	}
}
