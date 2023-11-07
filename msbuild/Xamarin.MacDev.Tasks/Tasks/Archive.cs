using Microsoft.Build.Framework;
using Xamarin.Messaging.Build.Client;
using Xamarin.iOS.Tasks;

namespace Xamarin.MacDev.Tasks {
	public class Archive : ArchiveTaskBase, ICancelableTask {
		public override bool Execute ()
		{
			if (!ShouldExecuteRemotely ())
				return base.Execute ();

			if (AppExtensionReferences is not null)
				TaskItemFixer.ReplaceItemSpecsWithBuildServerPath (AppExtensionReferences, SessionId);

			return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;
		}

		public void Cancel ()
		{
			if (ShouldExecuteRemotely ())
				BuildConnection.CancelAsync (BuildEngine4).Wait ();
		}
	}
}
