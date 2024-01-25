using System;
using System.IO;
using System.Linq;

using Microsoft.Build.Framework;

using Xamarin.Localization.MSBuild;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Xamarin.MacDev.Tasks {
	public class FindAotCompiler : XamarinBuildTask {
		[Required]
		public ITaskItem [] MonoAotCrossCompiler { get; set; }

		[Output]
		public string AotCompiler { get; set; }

		protected override bool ExecuteLocally ()
		{
			// If we can't find the AOT compiler path in MonoAotCrossCompiler, evaluate a project file that does know how to find it.
			// This happens when executing remotely from Windows, because the MonoAotCrossCompiler item group will be empty in that case.
			var targetName = "ComputeAotCompilerPath";
			var target = $@"<Target Name=""{targetName}"">
	<PropertyGroup>
		<_XamarinAOTCompiler>@(MonoAotCrossCompiler->WithMetadataValue(""RuntimeIdentifier"", ""$(RuntimeIdentifier)""))</_XamarinAOTCompiler>
	</PropertyGroup>
	<WriteLinesToFile File=""$(OutputFilePath)"" Lines=""$(_XamarinAOTCompiler)"" />
</Target>";

			if (MonoAotCrossCompiler?.Length > 0 && string.IsNullOrEmpty (Environment.GetEnvironmentVariable ("XAMARIN_FORCE_AOT_COMPILER_PATH_COMPUTATION"))) {
				var aotCompilerItem = MonoAotCrossCompiler.SingleOrDefault (v => v.GetMetadata ("RuntimeIdentifier") == RuntimeIdentifier);

				if (aotCompilerItem is null) {
					Log.LogMessage (MessageImportance.Low, "Unable to find the AOT compiler for the RuntimeIdentifier '{0}' in the MonoAotCrossCompiler item group", RuntimeIdentifier);
					AotCompiler = ComputeValueUsingTarget (target, targetName);
				} else {
					AotCompiler = aotCompilerItem.ItemSpec;
				}
			} else {
				AotCompiler = ComputeValueUsingTarget (target, targetName);
			}

			// Don't check if the aot compiler exists if an error was already reported.
			if (Log.HasLoggedErrors)
				return false;

			if (!File.Exists (AotCompiler))
				Log.LogError (MSBStrings.E7081 /*"The AOT compiler '{0}' does not exist." */, AotCompiler);

			return !Log.HasLoggedErrors;
		}
	}
}

