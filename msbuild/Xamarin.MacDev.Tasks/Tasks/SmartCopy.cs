using System.Collections.Generic;
using System.Linq;
using Microsoft.Build.Framework;
using Xamarin.Messaging.Build.Client;

namespace Xamarin.MacDev.Tasks
{
	public class SmartCopy : SmartCopyTaskBase, ITaskCallback, ICancelableTask
	{
		public override bool Execute ()
		{
			if (!string.IsNullOrEmpty (SessionId)) {
				var taskRunner = new TaskRunner (SessionId, BuildEngine4);

				taskRunner.FixReferencedItems (SourceFiles);

				return taskRunner.RunAsync (this).Result;
			}

			return base.Execute ();
		}

		public void Cancel ()
		{
			if (!string.IsNullOrEmpty (SessionId))
				BuildConnection.CancelAsync (SessionId, BuildEngine4).Wait ();
		}

		public bool ShouldCopyToBuildServer (ITaskItem item) => false;

		public bool ShouldCreateOutputFile (ITaskItem item) => true;

		public IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied () => Enumerable.Empty<ITaskItem> ();
	}
}
