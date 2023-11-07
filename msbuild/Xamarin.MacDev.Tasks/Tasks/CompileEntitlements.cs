using System.Collections.Generic;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Xamarin.MacDev.Tasks;
using Xamarin.Messaging.Build.Client;

namespace Xamarin.MacDev.Tasks {
	public class CompileEntitlements : CompileEntitlementsTaskBase, ITaskCallback, ICancelableTask {
		protected override string DefaultEntitlementsPath {
			get {
				if (ShouldExecuteRemotely ()) {
					return "Entitlements.plist";
				}

				return base.DefaultEntitlementsPath;
			}
		}

		public override bool Execute ()
		{
			if (ShouldExecuteRemotely ())
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			return base.Execute ();
		}

		public bool ShouldCopyToBuildServer (ITaskItem item) => true;

		public bool ShouldCreateOutputFile (ITaskItem item) => true;

		public IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied ()
		{
			if (!string.IsNullOrEmpty (Entitlements))
				yield return new TaskItem (Entitlements);
			else
				yield return new TaskItem (DefaultEntitlementsPath);
		}

		public void Cancel ()
		{
			if (ShouldExecuteRemotely ())
				BuildConnection.CancelAsync (BuildEngine4).Wait ();
		}
	}
}
