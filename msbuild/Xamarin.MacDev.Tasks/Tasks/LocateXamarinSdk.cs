using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.Build.Framework;

using Xamarin.Localization.MSBuild;

#nullable enable

namespace Xamarin.MacDev.Tasks {
	public class LocateXamarinSdk : XamarinBuildTask {
		[Output]
		public string XamarinSdkRootDirectory { get; set; } = string.Empty;

		protected override bool ExecuteLocally ()
		{
			var targetName = "LocateXamarinSdk";
			var target = $@"<Target Name=""{targetName}"">
	<WriteLinesToFile File=""$(OutputFilePath)"" Lines=""$(_XamarinSdkRootDirectory)"" />
</Target>";

			XamarinSdkRootDirectory = ComputeValueUsingTarget (target, targetName);

			// Don't do anything else if something already went wrong (in particular don't check if the SDK directory exists).
			if (Log.HasLoggedErrors)
				return false;

			if (!Directory.Exists (XamarinSdkRootDirectory))
				Log.LogError (MSBStrings.E7120 /* The SDK directory {0} doesn't exist. */, XamarinSdkRootDirectory);

			return !Log.HasLoggedErrors;
		}
	}
}
