using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Xamarin.Localization.MSBuild;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Xamarin.MacDev.Tasks {
	public class FindILLink : XamarinBuildTask {
		[Output]
		public string ILLinkPath { get; set; }

		protected override bool ExecuteLocally ()
		{
			var targetName = "ComputeILLinkTaskPath";
			var target = $@"<Target Name=""{targetName}"">
	<WriteLinesToFile File=""$(OutputFilePath)"" Lines=""$(ILLinkTasksAssembly)"" />
</Target>";

			var illinkTaskPath = ComputeValueUsingTarget (target, targetName);

			// Don't do anything else if something already went wrong (in particular don't check if illink.dll exists).
			if (Log.HasLoggedErrors)
				return false;

			if (!string.IsNullOrEmpty (illinkTaskPath))
				ILLinkPath = Path.Combine (Path.GetDirectoryName (illinkTaskPath), "illink.dll");

			if (!File.Exists (ILLinkPath))
				Log.LogError (MSBStrings.E7115 /*"The illink assembly doesn't exist: '{0}'" */, ILLinkPath);

			return !Log.HasLoggedErrors;
		}
	}
}

