using System.IO;

using Microsoft.Build.Framework;

using Xamarin.Localization.MSBuild;
using Xamarin.Messaging.Build.Client;

namespace Xamarin.MacDev.Tasks {
	public class PrepareResourceRules : XamarinTask, ICancelableTask {
		#region Inputs

		[Required]
		public string AppBundleDir { get; set; } = string.Empty;

		public string ResourceRules { get; set; } = string.Empty;

		[Required]
		public string SdkVersion { get; set; } = string.Empty;

		#endregion

		#region Outputs

		[Output]
		public string PreparedResourceRules { get; set; } = string.Empty;

		#endregion

		public override bool Execute ()
		{
			if (ShouldExecuteRemotely ())
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			if (!string.IsNullOrEmpty (ResourceRules)) {
				var destination = Path.Combine (AppBundleDir, "ResourceRules.plist");

				if (!File.Exists (ResourceRules)) {
					Log.LogError (MSBStrings.E0065, ResourceRules);
					return false;
				}

				File.Copy (ResourceRules, destination, true);

				PreparedResourceRules = destination;
			} else {
				PreparedResourceRules = string.Empty;
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
