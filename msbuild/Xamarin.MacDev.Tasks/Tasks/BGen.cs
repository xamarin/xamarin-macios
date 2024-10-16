// Copyright (C) 2011,2012 Xamarin, Inc. All rights reserved.

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.Build.Tasks;

using Xamarin.Utils;
using Xamarin.Localization.MSBuild;
using Xamarin.Messaging;
using Xamarin.Messaging.Build.Client;

#nullable enable

namespace Xamarin.MacDev.Tasks {
	public class BGen : XamarinTask, ICancelableTask {
		CancellationTokenSource? cancellationTokenSource;

		public string OutputPath { get; set; } = string.Empty;

		[Required]
		public string BGenToolPath { get; set; } = string.Empty;

		[Required]
		public string BGenToolExe { get; set; } = string.Empty;

		public ITaskItem [] ObjectiveCLibraries { get; set; } = Array.Empty<ITaskItem> ();

		public ITaskItem [] AdditionalLibPaths { get; set; } = Array.Empty<ITaskItem> ();

		public bool AllowUnsafeBlocks { get; set; }

		[Required]
		public string BaseLibDll { get; set; } = string.Empty;

		[Required]
		public ITaskItem [] ApiDefinitions { get; set; } = Array.Empty<ITaskItem> ();

		public string AttributeAssembly { get; set; } = string.Empty;

		public ITaskItem? CompiledApiDefinitionAssembly { get; set; }

		public ITaskItem [] CoreSources { get; set; } = Array.Empty<ITaskItem> ();

		public string DefineConstants { get; set; } = string.Empty;

		public bool EmitDebugInformation { get; set; }

		public string ExtraArgs { get; set; } = string.Empty;

		public int Verbosity { get; set; }

		public string GeneratedSourcesDir { get; set; } = string.Empty;

		public string GeneratedSourcesFileList { get; set; } = string.Empty;

		public string Namespace { get; set; } = string.Empty;

		public bool NoNFloatUsing { get; set; }

		public ITaskItem [] NativeLibraries { get; set; } = Array.Empty<ITaskItem> ();

		public string OutputAssembly { get; set; } = string.Empty;

		public bool ProcessEnums { get; set; }

		[Required]
		public string ProjectDir { get; set; } = string.Empty;

		public ITaskItem [] References { get; set; } = Array.Empty<ITaskItem> ();

		public ITaskItem [] Resources { get; set; } = Array.Empty<ITaskItem> ();

		public ITaskItem [] Sources { get; set; } = Array.Empty<ITaskItem> ();

		[Required]
		public string ResponseFilePath { get; set; } = string.Empty;

		protected virtual void HandleReferences (List<string> cmd)
		{
			if (References is not null) {
				foreach (var item in References)
					cmd.Add ($"-r:{Path.GetFullPath (item.ItemSpec)}");
			}
		}

		public virtual List<string> GenerateCommandLineArguments ()
		{
			var cmd = new List<string> ();

#if DEBUG
			cmd.Add ("/v");
#endif

			if (CompiledApiDefinitionAssembly is not null)
				cmd.Add ($"/compiled-api-definition-assembly:{CompiledApiDefinitionAssembly.ItemSpec}");

			cmd.Add ("/nostdlib");
			if (!string.IsNullOrEmpty (BaseLibDll))
				cmd.Add ($"/baselib:{BaseLibDll}");
			if (!string.IsNullOrEmpty (OutputAssembly))
				cmd.Add ($"/out:{OutputAssembly}");

			cmd.Add ($"/attributelib:{AttributeAssembly}");

			if (!string.IsNullOrEmpty (BaseLibDll)) {
				var dir = Path.GetDirectoryName (BaseLibDll);
				cmd.Add ($"/lib:{dir}");
			}

			if (ProcessEnums)
				cmd.Add ("/process-enums");

			if (EmitDebugInformation)
				cmd.Add ("/debug");

			if (AllowUnsafeBlocks)
				cmd.Add ("/unsafe");

			cmd.Add ($"/ns:{Namespace}");

			if (NoNFloatUsing)
				cmd.Add ("/no-nfloat-using:true");

			if (!string.IsNullOrEmpty (DefineConstants)) {
				var strv = DefineConstants.Split (new [] { ';' }, StringSplitOptions.RemoveEmptyEntries);
				foreach (var str in strv)
					cmd.Add ($"/d:{str}");
			}

			//cmd.AppendSwitch ("/e");

			foreach (var item in ApiDefinitions)
				cmd.Add (Path.GetFullPath (item.ItemSpec));

			if (CoreSources is not null) {
				foreach (var item in CoreSources)
					cmd.Add ($"/s:{Path.GetFullPath (item.ItemSpec)}");
			}

			if (Sources is not null) {
				foreach (var item in Sources)
					cmd.Add ($"/x:{Path.GetFullPath (item.ItemSpec)}");
			}

			if (AdditionalLibPaths is not null) {
				foreach (var item in AdditionalLibPaths)
					cmd.Add ($"/lib:{Path.GetFullPath (item.ItemSpec)}");
			}

			HandleReferences (cmd);

			if (Resources is not null) {
				foreach (var item in Resources) {
					var argument = item.ToString ();
					var id = item.GetMetadata ("LogicalName");
					if (!string.IsNullOrEmpty (id))
						argument += "," + id;

					cmd.Add ($"/res:{argument}");
				}
			}

			if (NativeLibraries is not null) {
				foreach (var item in NativeLibraries) {
					var argument = item.ToString ();
					var id = item.GetMetadata ("LogicalName");
					if (string.IsNullOrEmpty (id))
						id = Path.GetFileName (argument);

					cmd.Add ($"/res:{argument},{id}");
				}
			}

			if (GeneratedSourcesDir is not null)
				cmd.Add ($"/tmpdir:{Path.GetFullPath (GeneratedSourcesDir)}");

			if (GeneratedSourcesFileList is not null)
				cmd.Add ($"/sourceonly:{Path.GetFullPath (GeneratedSourcesFileList)}");

			cmd.Add ($"/target-framework={TargetFrameworkMoniker}");

			if (!string.IsNullOrEmpty (ExtraArgs)) {
				var extraArgs = CommandLineArgumentBuilder.Parse (ExtraArgs);
				var target = OutputAssembly;
				string projectDir;

				if (ProjectDir.StartsWith ("~/", StringComparison.Ordinal)) {
					// Note: Since the Visual Studio plugin doesn't know the user's home directory on the Mac build host,
					// it simply uses paths relative to "~/". Expand these paths to their full path equivalents.
					var home = Environment.GetFolderPath (Environment.SpecialFolder.UserProfile);

					projectDir = Path.Combine (home, ProjectDir.Substring (2));
				} else {
					projectDir = ProjectDir;
				}

				var customTags = new Dictionary<string, string> (StringComparer.OrdinalIgnoreCase) {
					{ "projectdir",   projectDir },
					// Apparently msbuild doesn't propagate the solution path, so we can't get it.
					// { "solutiondir",  proj.ParentSolution is not null ? proj.ParentSolution.BaseDirectory : proj.BaseDirectory },
				};
				// OutputAssembly is optional so it can be null
				if (target is not null) {
					var d = Path.GetDirectoryName (target);
					var n = Path.GetFileName (target);
					customTags.Add ("targetpath", Path.Combine (d, n));
					customTags.Add ("targetdir", d);
					customTags.Add ("targetname", n);
					customTags.Add ("targetext", Path.GetExtension (target));
				}

				for (int i = 0; i < extraArgs.Length; i++) {
					var argument = extraArgs [i];
					cmd.Add (StringParserService.Parse (argument, customTags));
				}
			}

			cmd.AddRange (VerbosityUtils.Merge (ExtraArgs, (LoggerVerbosity) Verbosity));

			return CommandLineArgumentBuilder.CreateResponseFile (this, ResponseFilePath, cmd, null);
		}

		public override bool Execute ()
		{
			if (ShouldExecuteRemotely ()) {
				try {
					BGenToolPath = PlatformPath.GetPathForCurrentPlatform (BGenToolPath);
					BaseLibDll = PlatformPath.GetPathForCurrentPlatform (BaseLibDll);

					TaskItemFixer.FixFrameworkItemSpecs (Log, item => OutputPath, TargetFramework.Identifier, References.Where (x => x.IsFrameworkItem ()).ToArray ());
					TaskItemFixer.FixItemSpecs (Log, item => OutputPath, References.Where (x => !x.IsFrameworkItem ()).ToArray ());

					var taskRunner = new TaskRunner (SessionId, BuildEngine4);
					var success = taskRunner.RunAsync (this).Result;

					if (success)
						GetGeneratedSourcesAsync (taskRunner).Wait ();

					return success;
				} catch (Exception ex) {
					Log.LogErrorFromException (ex);

					return false;
				}
			}

			AttributeAssembly = PathUtils.ConvertToMacPath (AttributeAssembly);
			BaseLibDll = PathUtils.ConvertToMacPath (BaseLibDll);

			var customHome = Environment.GetEnvironmentVariable ("DOTNET_CUSTOM_HOME");
			var env = new Dictionary<string, string?> ();
			if (!string.IsNullOrEmpty (customHome)) {
				env ["HOME"] = customHome;
			}

			if (!string.IsNullOrEmpty (SessionId) &&
				!string.IsNullOrEmpty (GeneratedSourcesDir) &&
				!Directory.Exists (GeneratedSourcesDir)) {
				Directory.CreateDirectory (GeneratedSourcesDir);
			}

			if (ApiDefinitions.Length == 0) {
				Log.LogError (MSBStrings.E0097);
				return false;
			}

			var executablePath = PathUtils.ConvertToMacPath (BGenToolPath);
			var executableExe = PathUtils.ConvertToMacPath (BGenToolExe);
			var executable = Path.Combine (executablePath, executableExe);
			var args = GenerateCommandLineArguments ();
			if (Log.HasLoggedErrors)
				return false;

			cancellationTokenSource = new CancellationTokenSource ();
			ExecuteAsync (Log, executable, args, environment: env, cancellationToken: cancellationTokenSource.Token).Wait ();
			return !Log.HasLoggedErrors;
		}

		public bool ShouldCopyToBuildServer (ITaskItem item) => !item.IsFrameworkItem ();

		public bool ShouldCreateOutputFile (ITaskItem item) => false;

		public IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied ()
		{
			if (ObjectiveCLibraries is null)
				return new ITaskItem [0];

			return ObjectiveCLibraries.Select (item => {
				var linkWithFileName = String.Concat (Path.GetFileNameWithoutExtension (item.ItemSpec), ".linkwith.cs");
				return new TaskItem (linkWithFileName);
			}).ToArray ();
		}

		public void Cancel ()
		{
			if (ShouldExecuteRemotely ()) {
				BuildConnection.CancelAsync (BuildEngine4).Wait ();
			} else {
				cancellationTokenSource?.Cancel ();
			}
		}

		async System.Threading.Tasks.Task GetGeneratedSourcesAsync (TaskRunner taskRunner)
		{
			await taskRunner.GetFileAsync (this, GeneratedSourcesFileList).ConfigureAwait (continueOnCapturedContext: false);

			var localGeneratedSourcesFileNames = new List<string> ();
			var generatedSourcesFileNames = File.ReadAllLines (GeneratedSourcesFileList);

			foreach (var generatedSourcesFileName in generatedSourcesFileNames) {
				var localRelativePath = GetLocalRelativePath (generatedSourcesFileName);

				await taskRunner.GetFileAsync (this, localRelativePath).ConfigureAwait (continueOnCapturedContext: false);

				var localGeneratedSourcesFileName = PlatformPath.GetPathForCurrentPlatform (localRelativePath);

				localGeneratedSourcesFileNames.Add (localGeneratedSourcesFileName);
			}

			File.WriteAllLines (GeneratedSourcesFileList, localGeneratedSourcesFileNames);
		}

		string GetLocalRelativePath (string path)
		{
			// convert mac full path in windows relative path
			// must remove \users\{user}\Library\Caches\Xamarin\mtbs\builds\{appname}\{sessionid}\
			if (path.Contains (SessionId)) {
				var start = path.IndexOf (SessionId) + SessionId.Length + 1;

				return path.Substring (start);
			} else {
				return path;
			}
		}
	}
}
