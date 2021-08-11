using System.IO;
using System.Linq;
using System.Collections.Generic;

using Xamarin.MacDev.Tasks;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Xamarin.Messaging.Build.Client;

namespace Xamarin.iOS.Tasks
{
	public class CreateBindingResourcePackage : CreateBindingResourcePackageBase, ITaskCallback, ICancelableTask
	{
		public override bool Execute ()
		{
			if (ShouldExecuteRemotely ())
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			return base.Execute ();
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
	}
}
