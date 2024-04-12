using System;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev;
using Xamarin.Localization.MSBuild;
using Xamarin.Messaging.Build.Client;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Xamarin.MacDev.Tasks {
	public class GetNativeExecutableName : XamarinTask, ICancelableTask {
		#region Inputs

		[Required]
		public string AppManifest { get; set; }

		#endregion

		#region Outputs

		[Output]
		public string ExecutableName { get; set; }

		#endregion

		public override bool Execute ()
		{
			if (ShouldExecuteRemotely ())
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			PDictionary plist;

			try {
				plist = PDictionary.FromFile (AppManifest);
			} catch (Exception ex) {
				Log.LogError (MSBStrings.E0055, ex.Message);
				return false;
			}

			ExecutableName = plist.GetCFBundleExecutable ();

			return !Log.HasLoggedErrors;
		}

		public void Cancel ()
		{
			if (ShouldExecuteRemotely ())
				BuildConnection.CancelAsync (BuildEngine4).Wait ();
		}
	}
}
