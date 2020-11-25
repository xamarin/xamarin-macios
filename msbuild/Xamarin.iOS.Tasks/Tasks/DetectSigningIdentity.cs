using System.Collections.Generic;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Xamarin.Messaging.Build.Client;

namespace Xamarin.iOS.Tasks
{
	public class DetectSigningIdentity : DetectSigningIdentityTaskCore, ITaskCallback, ICancelableTask
	{
		public override bool Execute ()
		{
			if (!string.IsNullOrEmpty (SessionId))
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			return base.Execute ();
		}

		public bool ShouldCopyToBuildServer (ITaskItem item) => true;

		public bool ShouldCreateOutputFile (ITaskItem item) => true;

		public IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied () => new ITaskItem [] { new TaskItem (AppManifest) };

		public void Cancel ()
		{
			if (!string.IsNullOrEmpty (SessionId))
				BuildConnection.CancelAsync (SessionId, BuildEngine4).Wait ();
		}
	}
}
