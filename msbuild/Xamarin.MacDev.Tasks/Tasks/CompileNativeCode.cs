using System.Collections.Generic;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Xamarin.Messaging.Build.Client;

namespace Xamarin.MacDev.Tasks {
	public class CompileNativeCode : CompileNativeCodeTaskBase, ICancelableTask, ITaskCallback {
		public override bool Execute ()
		{
			if (!ShouldExecuteRemotely ())
				return base.Execute ();

			foreach (var info in CompileInfo) {
				var outputFile = info.GetMetadata ("OutputFile");

				if (!string.IsNullOrEmpty (outputFile))
					info.SetMetadata ("OutputFile", outputFile.Replace ("\\", "/"));
			}

			return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;
		}

		public bool ShouldCopyToBuildServer (ITaskItem item) => false;

		public bool ShouldCreateOutputFile (ITaskItem item) => true;

		public IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied ()
		{
			if (IncludeDirectories is not null) {
				foreach (var dir in IncludeDirectories) {
					foreach (var file in Directory.EnumerateFiles (dir.ItemSpec, "*.*", SearchOption.AllDirectories)) {
						yield return new TaskItem (file);
					}
				}
			}
		}

		public void Cancel ()
		{
			if (ShouldExecuteRemotely ())
				BuildConnection.CancelAsync (BuildEngine4).Wait ();
		}
	}
}
