using System.Collections.Generic;
using System.Linq;
using Microsoft.Build.Framework;
using Xamarin.Messaging.Build.Client;

namespace Xamarin.iOS.Tasks
{
	public class OptimizeImage : OptimizeImageTaskBase, ITaskCallback
	{
		public override bool Execute ()
		{
			if (!string.IsNullOrEmpty (SessionId))
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			return base.Execute ();
		}

		public bool ShouldCopyToBuildServer (ITaskItem item) => !OutputImages.Contains (item);

		public bool ShouldCreateOutputFile (ITaskItem item) => true;

		public IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied () => Enumerable.Empty<ITaskItem> ();

		public override void Cancel ()
		{
			base.Cancel ();

			if (!string.IsNullOrEmpty(SessionId))
				BuildConnection.CancelAsync (SessionId, BuildEngine4).Wait ();
		}
	}
}
