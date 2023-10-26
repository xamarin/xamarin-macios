using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Tasks;
using Xamarin.Localization.MSBuild;
using Xamarin.Messaging.Build.Client;
using Threading = System.Threading.Tasks;

#nullable enable

namespace Xamarin.MacDev.Tasks {
	public abstract class XamarinBuildTask : XamarinTask, ITaskCallback, ICancelableTask {
		public bool KeepTemporaryOutput { get; set; }

		[Required]
		public string RuntimeIdentifier { get; set; } = string.Empty;

		enum FetchMode {
			Property,
			Item,
		}

		/// <summary>
		/// Fetches the specified property. An error will already have been logged if returning false.
		/// </summary>
		/// <returns>
		/// The specified property value.
		/// </returns>
		protected bool TryGetProperty (string name, string? targetName, [NotNullWhen (true)] out string? value) => TryGet (FetchMode.Property, name, targetName, out value);
		protected bool TryGetProperty (string name, [NotNullWhen (true)] out string? value) => TryGetProperty (name, null, out value);

		/// <summary>
		/// Fetches the specified item. An error will already have been logged if returning false.
		/// </summary>
		/// <returns>
		/// Json with the specified item(s).
		/// </returns>
		protected bool TryGetItem (string name, string? targetName, [NotNullWhen (true)] out string? value) => TryGet (FetchMode.Item, name, targetName, out value);
		protected bool TryGetItem (string name, [NotNullWhen (true)] out string? value) => TryGetProperty (name, null, out value);

		bool TryGet (FetchMode mode, string name, string? targetName, [NotNullWhen (true)] out string? value)
		{
			value = null;

			var projectDirectory = Path.GetTempFileName ();
			File.Delete (projectDirectory);
			Directory.CreateDirectory (projectDirectory);
			var projectPath = Path.Combine (projectDirectory, name + ".csproj");

			var csproj = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<Project Sdk=""Microsoft.NET.Sdk"">
	<PropertyGroup>
		<TargetFramework>net{TargetFramework.Version}-{PlatformName}</TargetFramework>
	</PropertyGroup>
</Project>
";
			File.WriteAllText (projectPath, csproj);

			var dotnetPath = this.GetDotNetPath ();
			var environment = default (Dictionary<string, string?>);
			var customHome = Environment.GetEnvironmentVariable ("DOTNET_CUSTOM_HOME");

			if (!string.IsNullOrEmpty (customHome)) {
				environment = new Dictionary<string, string?> { { "HOME", customHome } };
			}

			try {
				ExecuteRestoreAsync (dotnetPath, projectPath, name, environment).Wait ();

				// Don't try to run 'dotnet build' if restore failed.
				if (Log.HasLoggedErrors)
					return false;

				value = ExecuteBuildAsync (dotnetPath, projectPath, mode, name, targetName, environment).Result;
			} finally {
				if (KeepTemporaryOutput) {
					Log.LogMessage (MessageImportance.Normal, $"Temporary project directory for the {targetName} task: {projectDirectory}");
				} else {
					Directory.Delete (projectDirectory, true);
				}
			}

			return !Log.HasLoggedErrors;
		}

		async Threading.Task<bool> ExecuteRestoreAsync (string dotnetPath, string projectPath, string name, Dictionary<string, string?>? environment)
		{
			var projectDirectory = Path.GetDirectoryName (projectPath);
			var binlog = Path.Combine (projectDirectory, name + "-restore.binlog");
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
				await ExecuteAsync (dotnetPath, arguments, environment: environment, showErrorIfFailure: true);
			} finally {
				if (KeepTemporaryOutput) {
					Log.LogMessage (MessageImportance.Normal, $"Temporary restore log for the {GetType ().Name} task: {binlog}");
				} else {
					File.Delete (binlog);
				}
			}
			return !Log.HasLoggedErrors;
		}

		async Threading.Task<string> ExecuteBuildAsync (string dotnetPath, string projectPath, FetchMode mode, string name, string? targetName, Dictionary<string, string?>? environment)
		{
			var projectDirectory = Path.GetDirectoryName (projectPath);
			var binlog = Path.Combine (projectDirectory, name + "-build.binlog");
			var arguments = new List<string> ();

			arguments.Add ("build");
			arguments.Add ("/p:RuntimeIdentifier=" + RuntimeIdentifier);
			if (!string.IsNullOrEmpty (targetName))
				arguments.Add ($"/t:{targetName}");
			arguments.Add ("/bl:" + binlog);
			switch (mode) {
			case FetchMode.Property:
				arguments.Add ($"/getProperty:{name}");
				break;
			case FetchMode.Item:
				arguments.Add ($"/getItem:{name}");
				break;
			default:
				throw new InvalidOperationException ($"Unknown fetch mode: {mode}");
			}
			arguments.Add (projectPath);

			var rv = await ExecuteAsync (dotnetPath, arguments, environment: environment, showErrorIfFailure: true);
			if (rv.ExitCode == 0)
				return rv.StandardOutput!.ToString ();
			return string.Empty;
		}

		public override sealed bool Execute ()
		{
			if (this.ShouldExecuteRemotely (SessionId))
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;
			return ExecuteLocally ();
		}

		protected abstract bool ExecuteLocally ();

		public virtual IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied () => Enumerable.Empty<ITaskItem> ();

		public virtual bool ShouldCopyToBuildServer (ITaskItem item) => false;

		public virtual bool ShouldCreateOutputFile (ITaskItem item) => false;

		public virtual void Cancel ()
		{
			if (ShouldExecuteRemotely ())
				BuildConnection.CancelAsync (BuildEngine4).Wait ();
		}
	}
}

