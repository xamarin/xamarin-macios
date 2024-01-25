using System.Collections.Generic;
using System.Linq;

using Microsoft.Build.Framework;

using Xamarin.Messaging.Build.Client;

namespace Microsoft.Build.Tasks {
	public class Move : MoveTaskBase, ITaskCallback {
		public override bool Execute ()
		{
			if (this.ShouldExecuteRemotely (SessionId))
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			return base.Execute ();
		}

		public bool ShouldCopyToBuildServer (ITaskItem item) => false;

		public bool ShouldCreateOutputFile (ITaskItem item) => true;

		public IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied () => Enumerable.Empty<ITaskItem> ();
	}
}
