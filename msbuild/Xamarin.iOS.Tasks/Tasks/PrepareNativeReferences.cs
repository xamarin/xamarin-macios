using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Xamarin.MacDev.Tasks;
using Xamarin.Messaging.Build.Client;

namespace Xamarin.iOS.Tasks
{
	public class PrepareNativeReferences : PrepareNativeReferencesTaskBase, ITaskCallback, ICancelableTask
	{
		public override bool Execute ()
		{
			if (!ShouldExecuteRemotely ())
				return base.Execute ();

			var taskRunner = new TaskRunner (SessionId, BuildEngine4);

			try {
				var success = taskRunner.RunAsync (this).Result;

				if (success && LinkWithAttributes != null)
					taskRunner.GetFileAsync (LinkWithAttributes.ItemSpec).Wait ();

				return success;
			} catch (Exception ex) {
				Log.LogErrorFromException (ex);

				return false;
			}
		}

		public bool ShouldCopyToBuildServer (ITaskItem item) => true;

		public bool ShouldCreateOutputFile (ITaskItem item) => false;

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

		public void Cancel ()
		{
			if (ShouldExecuteRemotely ())
				BuildConnection.CancelAsync (SessionId, BuildEngine4).Wait ();
		}

		IEnumerable<TaskItem> GetItemsFromNativeReference (string folderPath)
		{
			foreach (var file in Directory.EnumerateFiles (folderPath, "*", SearchOption.AllDirectories)
				.Select (x => new TaskItem (x)))
				yield return file;
		}
	}
}
