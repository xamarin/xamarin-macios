using System.IO;
using System.Linq;
using System.Collections.Generic;

using Xamarin.MacDev.Tasks;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Xamarin.Messaging.Build.Client;

namespace Xamarin.MacDev.Tasks {
	public class CreateBindingResourcePackage : CreateBindingResourcePackageBase, ITaskCallback, ICancelableTask {
		public override bool Execute ()
		{
			if (!ShouldExecuteRemotely ())
				return base.Execute ();

			var taskRunner = new TaskRunner (SessionId, BuildEngine4);

			var success = taskRunner.RunAsync (this).Result;

			if (success) {
				TransferBindingResourcePackagesToWindowsAsync (taskRunner).Wait ();
			}

			return success;
		}

		public IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied ()
		{
			return CreateItemsForAllFilesRecursively (NativeReferences);
		}

		public bool ShouldCopyToBuildServer (ITaskItem item) => true;

		public bool ShouldCreateOutputFile (ITaskItem item) => true;

		public void Cancel ()
		{
			if (ShouldExecuteRemotely ())
				BuildConnection.CancelAsync (BuildEngine4).Wait ();
		}

		async System.Threading.Tasks.Task TransferBindingResourcePackagesToWindowsAsync (TaskRunner taskRunner)
		{
			if (PackagedFiles is not null) {
				foreach (var package in PackagedFiles) {
					var localRelativePath = GetLocalRelativePath (package.ItemSpec);
					await taskRunner.GetFileAsync (this, localRelativePath).ConfigureAwait (continueOnCapturedContext: false);
				}
			}
		}

		string GetLocalRelativePath (string path)
		{
			// convert mac full path in windows relative path
			// must remove \users\{user}\Library\Caches\Xamarin\mtbs\builds\{appname}\{sessionid}\
			if (path.Contains (SessionId)) {
				var start = path.IndexOf (SessionId) + SessionId.Length + 1;

				return path.Substring (start);
			} else {
				return path;
			}
		}
	}
}
