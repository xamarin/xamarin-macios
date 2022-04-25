using System.IO;
using System.Linq;
using System.Collections.Generic;

using Xamarin.MacDev.Tasks;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Xamarin.Messaging.Build.Client;

namespace Xamarin.MacDev.Tasks
{
	public class CreateBindingResourcePackage : CreateBindingResourcePackageBase, ITaskCallback, ICancelableTask
	{
		public override bool Execute ()
		{
			if (!ShouldExecuteRemotely ())
				return base.Execute ();

			var taskRunner = new TaskRunner (SessionId, BuildEngine4);

			var success = taskRunner.RunAsync (this).Result;

			if (success) {
				Log.LogWarning ("Successful!");
				TransferBindingResourcePackagesToWindowsAsync (taskRunner, Log).Wait ();
			} else {
				Log.LogWarning ("Not successful!");
			}

			return success;
		}

		public IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied ()
		{
			if (NativeReferences == null)
				yield break;

			foreach (var nativeRef in NativeReferences
				.Where (x => Directory.Exists (x.ItemSpec))
				.Select (x => x.ItemSpec))
				foreach (var item in GetItemsFromNativeReference (nativeRef))
					yield return item;
		}

		public bool ShouldCopyToBuildServer (ITaskItem item) => true;

		public bool ShouldCreateOutputFile (ITaskItem item) => true;

		public void Cancel ()
		{
			if (ShouldExecuteRemotely ())
				BuildConnection.CancelAsync (SessionId, BuildEngine4).Wait ();
		}

		IEnumerable<TaskItem> GetItemsFromNativeReference (string folderPath)
		{
			foreach (var file in Directory
				.EnumerateFiles (folderPath, "*", SearchOption.AllDirectories)
				.Select (x => new TaskItem (x)))
				yield return file;
		}

		async System.Threading.Tasks.Task TransferBindingResourcePackagesToWindowsAsync (TaskRunner taskRunner, TaskLoggingHelper log)
		{
			log.LogWarning ("Transferring...");
			foreach (var package in PackagedFiles) {
				var localRelativePath = GetLocalRelativePath (package);
				log.LogWarning ($"Transferring {localRelativePath}...");
				await taskRunner.GetFileAsync (localRelativePath).ConfigureAwait (continueOnCapturedContext: false);
			}
			log.LogWarning ($"Done transferring!");
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
