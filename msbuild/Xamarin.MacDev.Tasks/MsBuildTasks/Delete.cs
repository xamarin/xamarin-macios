using System.Collections.Generic;
using System.Linq;

using Microsoft.Build.Framework;

using Xamarin.Messaging.Build.Client;

namespace Microsoft.Build.Tasks {
	public class Delete : DeleteBase, ITaskCallback {
		public override bool Execute ()
		{
			var result = base.Execute ();

			if (!this.ShouldExecuteRemotely (SessionId)) {
				return result;
			}

			var client = BuildConnection
				.GetAsync (BuildEngine4)
				.Result
				.GetClient (SessionId);

			if (!client.IsConnected) {
				return result;
			}

			client.DeleteFilesAsync (Files.Select (x => x.ItemSpec).ToArray ()).Wait ();

			return result;
		}

		public bool ShouldCopyToBuildServer (ITaskItem item) => false;

		public bool ShouldCreateOutputFile (ITaskItem item) => false;

		public IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied () => Enumerable.Empty<ITaskItem> ();
	}
}
