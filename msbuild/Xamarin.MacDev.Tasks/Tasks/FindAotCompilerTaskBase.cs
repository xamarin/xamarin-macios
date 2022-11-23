using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Xamarin.Localization.MSBuild;
using Threading = System.Threading.Tasks;

namespace Xamarin.MacDev.Tasks {
	public abstract class FindAotCompilerTaskBase : XamarinTask {
		[Required]
		public ITaskItem [] MonoAotCrossCompiler { get; set; }

		public bool KeepTemporaryOutput { get; set; }

		[Required]
		public string RuntimeIdentifier { get; set; }

		[Output]
		public string AotCompiler { get; set; }

		public override bool Execute ()
		{
			if (MonoAotCrossCompiler?.Length > 0 && string.IsNullOrEmpty (Environment.GetEnvironmentVariable ("XAMARIN_FORCE_AOT_COMPILER_PATH_COMPUTATION"))) {
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

			File.Delete (projectPath);
			projectPath += ".csproj";

			var csproj = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<Project Sdk=""Microsoft.NET.Sdk"">
	<PropertyGroup>
		<TargetFramework>net{TargetFramework.Version}-{PlatformName}</TargetFramework>
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

			var dotnetPath = Environment.GetEnvironmentVariable ("DOTNET_HOST_PATH");

			if (string.IsNullOrEmpty (dotnetPath)) {
				dotnetPath = "dotnet";
			}

			var environment = default (Dictionary<string, string>);
			var customHome = Environment.GetEnvironmentVariable ("DOTNET_CUSTOM_HOME");

			if (!string.IsNullOrEmpty (customHome)) {
				environment = new Dictionary<string, string> { { "HOME", customHome } };
			}

			try {
				ExecuteRestoreAsync (dotnetPath, projectPath, environment).Wait ();

				return ExecuteBuildAsync (dotnetPath, projectPath, environment).Result;
			} finally {
				if (KeepTemporaryOutput) {
					Log.LogMessage (MessageImportance.Normal, $"Temporary project for the FindAotCompiler task: {projectPath}");
				} else {
					File.Delete (projectPath);
				}
			}
		}

		async Threading.Task ExecuteRestoreAsync (string dotnetPath, string projectPath, Dictionary<string, string> environment)
		{
			var binlog = GetTempBinLog ();
			var arguments = new List<string> ();

			arguments.Add ("restore");

			var dotnetDir = Path.GetDirectoryName (dotnetPath);
			var configFile = Path.Combine (dotnetDir, "NuGet.config");

			if (File.Exists (configFile)) {
				arguments.Add ("/p:RestoreConfigFile=" + configFile);
			}

			arguments.Add ("/bl:" + binlog);
			arguments.Add (projectPath);

			try {
				await ExecuteAsync (dotnetPath, arguments, environment: environment);
			} finally {
				if (KeepTemporaryOutput) {
					Log.LogMessage (MessageImportance.Normal, $"Temporary restore log for the FindAotCompiler task: {binlog}");
				} else {
					File.Delete (binlog);
				}
			}
		}

		async Threading.Task<string> ExecuteBuildAsync (string dotnetPath, string projectPath, Dictionary<string, string> environment)
		{
			var outputFile = Path.GetTempFileName ();
			var binlog = GetTempBinLog ();
			var arguments = new List<string> ();

			arguments.Add ("build");
			arguments.Add ("/p:OutputFilePath=" + outputFile);
			arguments.Add ("/p:RuntimeIdentifier=" + RuntimeIdentifier);
			arguments.Add ("/t:ComputeAotCompilerPath");
			arguments.Add ("/bl:" + binlog);
			arguments.Add (projectPath);

			try {
				await ExecuteAsync (dotnetPath, arguments, environment: environment);

				return File.ReadAllText (outputFile).Trim ();
			} finally {
				if (KeepTemporaryOutput) {
					Log.LogMessage (MessageImportance.Normal, $"Temporary output for the FindAotCompiler task: {outputFile}");
					Log.LogMessage (MessageImportance.Normal, $"Temporary build log for the FindAotCompiler task: {binlog}");
				} else {
					File.Delete (outputFile);
					File.Delete (binlog);
				}
			}
		}

		string GetTempBinLog ()
		{
			var binlog = Path.GetTempFileName ();

			File.Delete (binlog);
			binlog += ".binlog";

			return binlog;
		}
	}
}

