using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.Build.Framework;

using Xamarin.Localization.MSBuild;

namespace Xamarin.MacDev.Tasks {
	public abstract class FindAotCompilerTaskBase : XamarinTask {
		[Required]
		public ITaskItem[] MonoAotCrossCompiler { get; set; }

		[Required]
		public string RuntimeIdentifier { get; set; }

		[Output]
		public string AotCompiler { get; set; }

		public override bool Execute ()
		{
			if (MonoAotCrossCompiler?.Length > 0) {
				var aotCompilerItem = MonoAotCrossCompiler.SingleOrDefault (v => v.GetMetadata ("RuntimeIdentifier") == RuntimeIdentifier);

				if (aotCompilerItem == null) {
					Log.LogMessage (MessageImportance.Low, "Unable to find the AOT compiler for the RuntimeIdentifier '{0}' in the MonoAotCrossCompiler item group", RuntimeIdentifier);
					AotCompiler = ComputeAotCompilerPath ();
				} else {
					AotCompiler = aotCompilerItem.ItemSpec;
				}
			} else {
				AotCompiler = ComputeAotCompilerPath ();
			}

			if (!File.Exists (AotCompiler))
				Log.LogError (MSBStrings.E7081 /*"The AOT compiler '{0}' does not exist." */, AotCompiler);

			return !Log.HasLoggedErrors;
		}

		// If we can't find the AOT compiler path in MonoAotCrossCompiler, evaluate a project file that does know how to find it.
		// This happens when executing remotely from Windows, because the MonoAotCrossCompiler item group will be empty in that case.
		string ComputeAotCompilerPath ()
		{
			var projectPath = Path.GetTempFileName ();
			var outputFile = Path.GetTempFileName ();

			File.Delete (projectPath);
			projectPath += ".csproj";
			var csproj = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<Project Sdk=""Microsoft.NET.Sdk"">
	<PropertyGroup>
		<TargetFramework>net6.0-{PlatformName}</TargetFramework>
	</PropertyGroup>
	<Target Name=""ComputeAotCompilerPath"">
		<PropertyGroup>
			<_AOTCompiler>@(MonoAotCrossCompiler->WithMetadataValue(""RuntimeIdentifier"", ""$(RuntimeIdentifier)""))</_AOTCompiler>
		</PropertyGroup>
		<WriteLinesToFile File=""$(OutputFilePath)"" Lines=""$(_AOTCompiler)"" />
	</Target>
</Project>
";
			File.WriteAllText (projectPath, csproj);

			var executable = Environment.GetEnvironmentVariable ("DOTNET_HOST_PATH");
			if (string.IsNullOrEmpty (executable))
				executable = "dotnet";

			var arguments = new List<string> ();
			arguments.Add ("build");
			arguments.Add ("/p:OutputFilePath=" + outputFile);
			arguments.Add ("/p:RuntimeIdentifier=" + RuntimeIdentifier);
			arguments.Add ("/t:ComputeAotCompilerPath");
			arguments.Add (projectPath);

			var environment = default (Dictionary<string, string>);
			var customHome = Environment.GetEnvironmentVariable ("DOTNET_CUSTOM_HOME");

			if (!string.IsNullOrEmpty (customHome)) {
				environment = new Dictionary<string, string> { { "HOME", customHome } };
			}

			try {
				ExecuteAsync (executable, arguments, environment: environment).Wait ();
				return File.ReadAllText (outputFile).Trim ();
			} finally {
				File.Delete (projectPath);
				File.Delete (outputFile);
			}
		}
	}
}

