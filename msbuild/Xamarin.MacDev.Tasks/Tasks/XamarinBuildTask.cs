using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Xamarin.Localization.MSBuild;
using Threading = System.Threading.Tasks;

namespace Xamarin.MacDev.Tasks {
	public abstract class XamarinBuildTask : XamarinTask {
		public bool KeepTemporaryOutput { get; set; }

		[Required]
		public string RuntimeIdentifier { get; set; }

		/// <summary>
		/// Runs the target passed in computeValueTarget and returns its result.
		/// The target must write the result into a text file using $(OutputFilePath) as path.
		/// </summary>
		/// <returns></returns>
		protected string ComputeValueUsingTarget (string computeValueTarget, string targetName)
		{
			var projectPath = Path.GetTempFileName ();

			File.Delete (projectPath);
			projectPath += ".csproj";

			var csproj = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<Project Sdk=""Microsoft.NET.Sdk"">
	<PropertyGroup>
		<TargetFramework>net{TargetFramework.Version}-{PlatformName}</TargetFramework>
	</PropertyGroup>
	{computeValueTarget}
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
				return ExecuteBuildAsync (dotnetPath, projectPath, targetName, environment).Result;
			} finally {
				if (KeepTemporaryOutput) {
					Log.LogMessage (MessageImportance.Normal, $"Temporary project for the {targetName} task: {projectPath}");
				} else {
					File.Delete (projectPath);
				}
			}
		}

		async Threading.Task<string> ExecuteBuildAsync (string dotnetPath, string projectPath, string targetName, Dictionary<string, string> environment)
		{
			var outputFile = Path.GetTempFileName ();
			var binlog = GetTempBinLog ();
			var arguments = new List<string> ();

			arguments.Add ("build");
			arguments.Add ("/p:OutputFilePath=" + outputFile);
			arguments.Add ("/p:RuntimeIdentifier=" + RuntimeIdentifier);
			arguments.Add ($"/t:{targetName}");
			arguments.Add ("/bl:" + binlog);
			arguments.Add (projectPath);

			try {
				await ExecuteAsync (dotnetPath, arguments, environment: environment);

				return File.ReadAllText (outputFile).Trim ();
			} finally {
				if (KeepTemporaryOutput) {
					Log.LogMessage (MessageImportance.Normal, $"Temporary output for the {targetName} task: {outputFile}");
					Log.LogMessage (MessageImportance.Normal, $"Temporary build log for the {targetName} task: {binlog}");
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

