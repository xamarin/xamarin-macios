using System.Collections.Generic;
using System.Linq;

using Microsoft.Build.Framework;

using Xamarin.Messaging.Build.Client;

namespace Microsoft.Build.Tasks {
	public class MakeDir : MakeDirBase, ITaskCallback {
		public override bool Execute ()
		{
			var result = base.Execute ();

			if (this.ShouldExecuteRemotely (SessionId))
				result = new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			return result;
		}

		public bool ShouldCopyToBuildServer (ITaskItem item) => true;

		public bool ShouldCreateOutputFile (ITaskItem item) => false;

		public IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied () => Enumerable.Empty<ITaskItem> ();
	}
}
