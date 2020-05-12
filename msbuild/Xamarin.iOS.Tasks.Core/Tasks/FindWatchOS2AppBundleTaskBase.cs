using System;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev.Tasks;
using Xamarin.MacDev;

namespace Xamarin.iOS.Tasks
{
	public abstract class FindWatchOS2AppBundleTaskBase : XamarinTask
	{
		#region Inputs

		[Required]
		public ITaskItem[] WatchAppReferences { get; set; }

		#endregion Inputs

		#region Outputs

		[Output]
		public string WatchOS2AppBundle { get; set; }

		#endregion

		public override bool Execute ()
		{
			var pwd = PathUtils.ResolveSymbolicLinks (Environment.CurrentDirectory);

			if (WatchAppReferences.Length > 0) {
				WatchOS2AppBundle = PathUtils.AbsoluteToRelative (pwd, PathUtils.ResolveSymbolicLinks (WatchAppReferences[0].ItemSpec));

				return true;
			}

			return !Log.HasLoggedErrors;
		}
	}
}
