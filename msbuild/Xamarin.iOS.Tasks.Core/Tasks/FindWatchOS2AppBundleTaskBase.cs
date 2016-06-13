using System;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev.Tasks;
using Xamarin.MacDev;

namespace Xamarin.iOS.Tasks
{
	public abstract class FindWatchOS2AppBundleTaskBase : Task
	{
		#region Inputs

		public string SessionId { get; set; }

		[Required]
		public ITaskItem[] WatchAppReferences { get; set; }

		#endregion Inputs

		#region Outputs

		[Output]
		public string WatchOS2AppBundle { get; set; }

		#endregion

		public override bool Execute ()
		{
			var pwd = PathUtils.ResolveSymbolicLink (Environment.CurrentDirectory);

			Log.LogTaskName ("FindWatchOS2AppBundle");
			Log.LogTaskProperty ("WatchAppReferences", WatchAppReferences);

			if (WatchAppReferences.Length > 0) {
				WatchOS2AppBundle = PathUtils.AbsoluteToRelative (pwd, PathUtils.ResolveSymbolicLink (WatchAppReferences[0].ItemSpec));

				return true;
			}

			return !Log.HasLoggedErrors;
		}
	}
}
