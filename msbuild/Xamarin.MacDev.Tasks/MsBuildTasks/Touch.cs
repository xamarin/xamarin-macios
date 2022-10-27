using System.Collections.Generic;
using System.Linq;
using Microsoft.Build.Framework;
using Xamarin.Messaging.Build.Client;

namespace Microsoft.Build.Tasks {
	public class Touch : TouchBase, ITaskCallback {
		public override bool Execute ()
		{
			bool result;

			if (this.ShouldExecuteRemotely (SessionId))
				result = new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;
			else
				result = base.Execute ();

			return result;
		}

		public IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied () => Enumerable.Empty<ITaskItem> ();

		public bool ShouldCopyToBuildServer (ITaskItem item) => false;

		public bool ShouldCreateOutputFile (ITaskItem item) => true;
	}
}
