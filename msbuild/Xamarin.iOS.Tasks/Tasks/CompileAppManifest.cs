using System.Collections.Generic;
using System.Linq;
using Microsoft.Build.Framework;
using Xamarin.Messaging.Build.Client;

#nullable enable

namespace Xamarin.iOS.Tasks
{
	public class CompileAppManifest : CompileAppManifestTaskCore, ITaskCallback, ICancelableTask
	{
		public override bool Execute ()
		{
			if (ShouldExecuteRemotely ())
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			return base.Execute ();
		}

		public bool ShouldCopyToBuildServer (ITaskItem item)
		{
			// We don't want to copy partial generated manifest files
			if (PartialAppManifests is not null && PartialAppManifests.Contains (item))
				return false;

			return true;
		}

		public bool ShouldCreateOutputFile (ITaskItem item) => true;

		public IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied () => Enumerable.Empty<ITaskItem> ();

		public void Cancel ()
		{
			if (ShouldExecuteRemotely ())
				BuildConnection.CancelAsync (SessionId, BuildEngine4).Wait ();
		}
	}
}
