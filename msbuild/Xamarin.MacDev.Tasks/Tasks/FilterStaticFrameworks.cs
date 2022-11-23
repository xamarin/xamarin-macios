using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.Build.Tasks;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.Messaging.Build.Client;

#nullable enable

namespace Xamarin.MacDev.Tasks {
	public class FilterStaticFrameworks : FilterStaticFrameworksTaskBase, ITaskCallback {
		public override bool Execute ()
		{
			if (ShouldExecuteRemotely ())
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			return base.Execute ();
		}

		public bool ShouldCopyToBuildServer (ITaskItem item) => true;

		public bool ShouldCreateOutputFile (ITaskItem item) => false;

		public IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied ()
		{
			if (FrameworkToPublish is not null) {
				foreach (var item in FrameworkToPublish) {
					var fw = item.ItemSpec;
					// Copy all the files from the framework to the mac (copying only the executable won't work if it's just a symlink to elsewhere)
					if (File.Exists (fw))
						fw = Path.GetDirectoryName (fw);
					if (!Directory.Exists (fw))
						continue;
					foreach (var file in Directory.EnumerateFiles (fw, "*.*", SearchOption.AllDirectories)) {
						yield return new TaskItem (file);
					}
				}
			}
		}
	}
}
