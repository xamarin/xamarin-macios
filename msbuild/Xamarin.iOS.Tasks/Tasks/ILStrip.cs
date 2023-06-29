using System.Collections.Generic;
using System.Linq;
using Microsoft.Build.Tasks;
using Microsoft.Build.Framework;
using Xamarin.Messaging.Build.Client;

namespace Xamarin.MacDev.Tasks {
	public class ILStrip : ILStripTasks.ILStripBase, ITaskCallback {
		public override bool Execute ()
		{
			if (this.ShouldExecuteRemotely (SessionId))
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			return base.Execute ();
		}

		public bool ShouldCopyToBuildServer (ITaskItem item)
		{
			// Some assemblies are already on the Mac, and we have a 0-length
			// output file on Windows. We don't want to copy these files.
			// However, some assemblies have to be copied, because they don't
			// already exist on the Mac (typically resource assemblies). So
			// filter to assemblies with a non-zero length.

			var finfo = new FileInfo (item.ItemSpec);
			if (!finfo.Exists || finfo.Length == 0)
				return false;

			return true;
		}

		public bool ShouldCreateOutputFile (ITaskItem item) => true;

		public IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied () => Enumerable.Empty<ITaskItem> ();
	}
}
