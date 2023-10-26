using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.Build.Framework;

using Xamarin.Localization.MSBuild;

#nullable enable

namespace Xamarin.MacDev.Tasks {
	public class FindMlaunch : XamarinBuildTask {
		[Output]
		public string MlaunchPath { get; set; } = string.Empty;

		protected override bool ExecuteLocally ()
		{
			var targetName = "ComputeMLaunchPath";
			var target = $@"<Target Name=""{targetName}"">
	<WriteLinesToFile File=""$(OutputFilePath)"" Lines=""$(MlaunchPath)"" />
</Target>";

			MlaunchPath = ComputeValueUsingTarget (target, targetName);

			// Don't do anything else if something already went wrong (in particular don't check if mlaunch exists).
			if (Log.HasLoggedErrors)
				return false;

			if (!File.Exists (MlaunchPath))
				Log.LogError (MSBStrings.E7120 /*"The mlaunch tool doesn't exist: '{0}'" */, MlaunchPath);

			return !Log.HasLoggedErrors;
		}
	}
}
