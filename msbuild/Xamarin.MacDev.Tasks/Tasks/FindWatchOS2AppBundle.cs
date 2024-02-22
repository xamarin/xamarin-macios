using System;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev.Tasks;
using Xamarin.MacDev;
using Xamarin.Messaging.Build.Client;
using Xamarin.Utils;

#nullable enable

namespace Xamarin.MacDev.Tasks {
	public class FindWatchOS2AppBundle : XamarinTask, ICancelableTask {
		#region Inputs

		[Required]
		public ITaskItem [] WatchAppReferences { get; set; } = Array.Empty<ITaskItem> ();

		#endregion Inputs

		#region Outputs

		[Output]
		public string WatchOS2AppBundle { get; set; } = string.Empty;

		#endregion

		public override bool Execute ()
		{
			if (ShouldExecuteRemotely ()) {
				var taskRunner = new TaskRunner (SessionId, BuildEngine4);
				taskRunner.FixReferencedItems (this, WatchAppReferences);
				return taskRunner.RunAsync (this).Result;
			}

			var pwd = PathUtils.ResolveSymbolicLinks (Environment.CurrentDirectory);

			if (WatchAppReferences.Length > 0) {
				WatchOS2AppBundle = PathUtils.AbsoluteToRelative (pwd, PathUtils.ResolveSymbolicLinks (WatchAppReferences [0].ItemSpec));

				return true;
			}

			return !Log.HasLoggedErrors;
		}

		public void Cancel ()
		{
			if (ShouldExecuteRemotely ())
				BuildConnection.CancelAsync (BuildEngine4).Wait ();
		}
	}
}
