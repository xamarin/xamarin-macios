using System;

using Microsoft.Build.Framework;

using Xamarin.Messaging.Build.Client;

namespace Xamarin.MacDev.Tasks {
	public class DetectSdkLocations : DetectSdkLocationsCoreTaskBase, ICancelableTask {
		const string SdkVersionDefaultValue = "default";

		public override bool Execute ()
		{
			if (!ShouldExecuteRemotely ())
				return base.Execute ();

			// The new targets do not support the "default" value for the MtouchSdkVersion
			// So we fix it to not break existing projects that has this value defined in the .csproj
			if (!string.IsNullOrEmpty (SdkVersion) && SdkVersionDefaultValue.Equals (SdkVersion, StringComparison.OrdinalIgnoreCase))
				SdkVersion = string.Empty;

			return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;
		}

		public void Cancel ()
		{
			if (ShouldExecuteRemotely ())
				BuildConnection.CancelAsync (BuildEngine4).Wait ();
		}
	}
}
