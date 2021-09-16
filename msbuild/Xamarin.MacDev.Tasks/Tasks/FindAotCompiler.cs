using System.Linq;
using System.Collections.Generic;

using Xamarin.Messaging.Build.Client;
using Microsoft.Build.Framework;

namespace Xamarin.MacDev.Tasks
{
	public class FindAotCompiler : FindAotCompilerTaskBase, ITaskCallback, ICancelableTask {
		public override bool Execute ()
		{
			bool result;

			if (ShouldExecuteRemotely ()) {
				result = new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;
			} else {
				result = base.Execute ();
			}

			return result;
		}

		public void Cancel ()
		{
			if (ShouldExecuteRemotely ())
				BuildConnection.CancelAsync (SessionId, BuildEngine4).Wait ();
		}

		public bool ShouldCopyToBuildServer (ITaskItem item)
		{
			return false;
		}

		public bool ShouldCreateOutputFile (ITaskItem item) => false;

		public IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied () => Enumerable.Empty<ITaskItem> ();
	}
}

