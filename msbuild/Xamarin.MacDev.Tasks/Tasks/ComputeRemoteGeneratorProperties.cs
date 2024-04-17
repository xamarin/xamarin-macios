using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using Microsoft.Build.Framework;
using Microsoft.Build.Tasks;

using Xamarin.Localization.MSBuild;
using Xamarin.Messaging.Build.Client;
using Xamarin.Utils;

#nullable enable

namespace Xamarin.MacDev.Tasks {
	public class ComputeRemoteGeneratorProperties : XamarinTask, ITaskCallback, ICancelableTask {
		// Inputs
		[Required]
		public string IntermediateOutputPath { get; set; } = string.Empty;

		public string TargetFrameworkIdentifier { get; set; } = string.Empty;

		// Outputs
		[Output]
		public string BaseLibDllPath { get; set; } = string.Empty;

		[Output]
		public string BTouchToolExe { get; set; } = string.Empty;

		[Output]
		public string BTouchToolPath { get; set; } = string.Empty;

		[Output]
		public string DotNetCscCompiler { get; set; } = string.Empty;

		[Output]
		public string GeneratorAttributeAssembly { get; set; } = string.Empty;

		public override bool Execute ()
		{
			if (ShouldExecuteRemotely ())
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			ComputeProperties ();

			return !Log.HasLoggedErrors;
		}

		// Several properties can only be calculated when executed on the mac, so here
		// we execute a sub-build that computes those properties.
		void ComputeProperties ()
		{
			var projectDir = Path.GetFullPath (Path.Combine (IntermediateOutputPath, $"ComputeRemoteGeneratorProperties"));
			var projectPath = Path.Combine (projectDir, "ComputeRemoteGeneratorProperties.csproj");
			var outputFile = Path.Combine (projectDir, "ComputedProperties.txt");
			var binlog = Path.Combine (projectDir, "ComputeRemoteGeneratorProperties.binlog");
			string executable;
			string csproj;

			if (Directory.Exists (projectDir))
				Directory.Delete (projectDir, true);
			Directory.CreateDirectory (projectDir);

			if (IsDotNet) {
				csproj = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<Project Sdk=""Microsoft.NET.Sdk"">
	<PropertyGroup>
		<TargetFramework>net{TargetFramework.Version}-{PlatformName.ToLower ()}</TargetFramework>
		<IsBindingProject>true</IsBindingProject>
	</PropertyGroup>
</Project>";
			} else {
				string targetsFile;
				switch (Platform) {
				case ApplePlatform.iOS:
					targetsFile = $"$(MSBuildExtensionsPath)/Xamarin/iOS/Xamarin.iOS.ObjCBinding.CSharp.targets";
					break;
				case ApplePlatform.TVOS:
					targetsFile = $"$(MSBuildExtensionsPath)/Xamarin/iOS/Xamarin.TVOS.ObjCBinding.CSharp.targets";
					break;
				case ApplePlatform.WatchOS:
					targetsFile = $"$(MSBuildExtensionsPath)/Xamarin/iOS/Xamarin.WatchOS.ObjCBinding.CSharp.targets";
					break;
				case ApplePlatform.MacOSX:
					targetsFile = "$(MSBuildExtensionsPath)/Xamarin/Mac/Xamarin.Mac.ObjCBinding.CSharp.targets";
					break;
				default:
					throw new InvalidOperationException (string.Format (MSBStrings.InvalidPlatform, Platform));
				}
				csproj = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<Project DefaultTargets=""Build"" ToolsVersion=""4.0"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <Import Project=""{targetsFile}"" />
</Project>";
			}
			File.WriteAllText (projectPath, csproj);

			var arguments = new List<string> ();
			var environment = default (Dictionary<string, string?>);

			if (IsDotNet) {
				executable = this.GetDotNetPath ();

				arguments.Add ("build");

				var customHome = Environment.GetEnvironmentVariable ("DOTNET_CUSTOM_HOME");

				if (!string.IsNullOrEmpty (customHome)) {
					environment = new Dictionary<string, string?> { { "HOME", customHome } };
				}
			} else {
				executable = "/Library/Frameworks/Mono.framework/Commands/msbuild";
			}

			arguments.Add ("/p:OutputFilePath=" + outputFile);
			arguments.Add ("/t:_WriteRemoteGeneratorProperties");
			if (!string.IsNullOrEmpty (TargetFrameworkIdentifier))
				arguments.Add ($"/p:TargetFrameworkIdentifier={TargetFrameworkIdentifier}");
			arguments.Add ($"/p:_PlatformName={PlatformName}");
			arguments.Add ($"/bl:{binlog}");

			arguments.Add (projectPath);

			ExecuteAsync (executable, arguments, environment: environment).Wait ();
			if (!File.Exists (outputFile)) {
				Log.LogError (MSBStrings.E7120 /* Unable to compute the remote generator properties. Please file an issue at https://github.com/xamarin/xamarin-macios/issues/new and attach the following file: {0} */, binlog);
				return;
			}
			var computedPropertes = File.ReadAllLines (outputFile);
			foreach (var line in computedPropertes) {
				var property = line.Substring (0, line.IndexOf ('='));
				var value = line.Substring (property.Length + 1);

				Log.LogMessage (MessageImportance.Low, $"Computed the property {property}={value}");

				switch (property) {
				case "BaseLibDllPath":
					BaseLibDllPath = value;
					break;
				case "BTouchToolExe":
					BTouchToolExe = value;
					break;
				case "BTouchToolPath":
					BTouchToolPath = value;
					break;
				case "_DotNetCscCompiler":
					DotNetCscCompiler = value;
					break;
				case "_GeneratorAttributeAssembly":
					GeneratorAttributeAssembly = value;
					break;
				default:
					Log.LogError (MSBStrings.E7101 /* Unknown property '{0}' with value '{1}'. */, property, value);
					break;
				}
			}
		}

		public void Cancel ()
		{
			if (ShouldExecuteRemotely ())
				BuildConnection.CancelAsync (BuildEngine4).Wait ();
		}

		public bool ShouldCopyToBuildServer (ITaskItem item)
		{
			return false;
		}

		public bool ShouldCreateOutputFile (ITaskItem item) => false;

		public IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied () => Enumerable.Empty<ITaskItem> ();
	}
}
