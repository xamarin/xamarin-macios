using System;
//using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev.Tasks;
using Xamarin.MacDev;
using Xamarin.Utils;

namespace Xamarin.iOS.Tasks {
	public abstract class FindWatchOS2AppBundleTaskBase : XamarinTask {
		#region Inputs

		[Required]
		public ITaskItem [] WatchAppReferences { get; set; }

		#endregion Inputs

		#region Outputs

		[Output]
		public string WatchOS2AppBundle { get; set; }
		
		[Output]
		public bool HasWatchKitStub { get; set; }

		#endregion

		public override bool Execute ()
		{
			var pwd = PathUtils.ResolveSymbolicLinks (Environment.CurrentDirectory);

			if (WatchAppReferences.Length > 0) {
				WatchOS2AppBundle = PathUtils.AbsoluteToRelative (pwd, PathUtils.ResolveSymbolicLinks (WatchAppReferences [0].ItemSpec));
				HasWatchKitStub = System.IO.Directory.Exists (System.IO.Path.Combine (WatchOS2AppBundle, "_WatchKitStub"));
				return true;
			}

			return !Log.HasLoggedErrors;
		}
	}
}
