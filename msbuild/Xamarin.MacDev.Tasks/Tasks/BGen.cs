// Copyright (C) 2011,2012 Xamarin, Inc. All rights reserved.

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.Build.Tasks;

using Xamarin.Utils;
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

		[Required]
		public string BaseLibDll { get; set; } = string.Empty;

		public string AttributeAssembly { get; set; } = string.Empty;

		[Required]
		public ITaskItem? CompiledApiDefinitionAssembly { get; set; }

		public bool EmitDebugInformation { get; set; }

		public string ExtraArgs { get; set; } = string.Empty;

		[Required]
		public string GeneratedSourcesDir { get; set; } = string.Empty;

		[Required]
		public string GeneratedSourcesFileList { get; set; } = string.Empty;

		public string Namespace { get; set; } = string.Empty;

		public ITaskItem [] NativeLibraries { get; set; } = Array.Empty<ITaskItem> ();

		public string OutputAssembly { get; set; } = string.Empty;

		public bool ProcessEnums { get; set; }

		[Required]
		public string ProjectDir { get; set; } = string.Empty;

		public ITaskItem [] References { get; set; } = Array.Empty<ITaskItem> ();

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

			var compiledApiDefinitionAssembly = CompiledApiDefinitionAssembly?.ItemSpec;
#if NET
			if (!string.IsNullOrEmpty (compiledApiDefinitionAssembly))
#else
			if (compiledApiDefinitionAssembly is not null && !string.IsNullOrEmpty (compiledApiDefinitionAssembly))
#endif
				cmd.Add ($"/compiled-api-definition-assembly:{compiledApiDefinitionAssembly}");

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

			if (!string.IsNullOrEmpty (Namespace))
				cmd.Add ($"/ns:{Namespace}");

			//cmd.AppendSwitch ("/e");

			if (AdditionalLibPaths is not null) {
				foreach (var item in AdditionalLibPaths)
					cmd.Add ($"/lib:{Path.GetFullPath (item.ItemSpec)}");
			}

			HandleReferences (cmd);

			if (!string.IsNullOrEmpty (GeneratedSourcesDir))
				cmd.Add ($"/tmpdir:{Path.GetFullPath (GeneratedSourcesDir)}");

			if (!string.IsNullOrEmpty (GeneratedSourcesFileList))
				cmd.Add ($"/sourceonly:{Path.GetFullPath (GeneratedSourcesFileList)}");

			cmd.Add ($"/target-framework={TargetFrameworkMoniker}");

			if (!string.IsNullOrEmpty (ExtraArgs)) {
				var extraArgs = StringUtils.ParseArguments (ExtraArgs);
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
				if (!string.IsNullOrEmpty (target)) {
					var d = Path.GetDirectoryName (target)!;
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

			VerbosityUtils.RenderVerbosity (cmd, Verbosity);

			return CommandLineArgumentBuilder.CreateResponseFile (this, ResponseFilePath, cmd, null);
		}

		public override bool Execute ()
		{
			var compiledApiDefinitionAssembly = CompiledApiDefinitionAssembly;
			if (compiledApiDefinitionAssembly is null || string.IsNullOrEmpty (compiledApiDefinitionAssembly.ItemSpec)) {
				Log.LogError ("A compiled API definition assembly is required.");
				return false;
			}
			if (string.IsNullOrEmpty (GeneratedSourcesDir) || string.IsNullOrEmpty (GeneratedSourcesFileList)) {
				Log.LogError ("A generated sources directory and file list are required.");
				return false;
			}

			if (ShouldExecuteRemotely ()) {
				try {
					BGenToolPath = PlatformPath.GetPathForCurrentPlatform (BGenToolPath);
					BaseLibDll = PlatformPath.GetPathForCurrentPlatform (BaseLibDll);

					TaskItemFixer.FixItemSpecs (Log, item => OutputPath, References.Where (x => !x.IsFrameworkItem ()).ToArray ());
					TaskItemFixer.FixItemSpecs (Log, item => OutputPath, new [] { compiledApiDefinitionAssembly });

					if (ExecuteRemotely (out var taskRunner)) {
						GetGeneratedSourcesAsync (taskRunner).Wait ();
						return true;
					}

					return false;
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

			var bgenPath = PathUtils.ConvertToMacPath (BGenToolPath);
			var bgenExe = PathUtils.ConvertToMacPath (BGenToolExe);
			var bgen = Path.Combine (bgenPath, bgenExe);
			var args = GenerateCommandLineArguments ();
			args.Insert (0, bgen);
			var executable = this.GetDotNetPath ();
			if (Log.HasLoggedErrors)
				return false;

			cancellationTokenSource = new CancellationTokenSource ();
			ExecuteAsync (executable, args, environment: env, cancellationToken: cancellationTokenSource.Token).Wait ();
			return !Log.HasLoggedErrors;
		}

		public bool ShouldCopyToBuildServer (ITaskItem item) => !item.IsFrameworkItem ();

		public bool ShouldCreateOutputFile (ITaskItem item) => false;

		public IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied ()
		{
			var compiledApiDefinitionAssembly = CompiledApiDefinitionAssembly?.ItemSpec;
			if (!string.IsNullOrEmpty (compiledApiDefinitionAssembly)) {
				var documentationFile = Path.ChangeExtension (compiledApiDefinitionAssembly, ".xml");
				if (File.Exists (documentationFile))
					yield return new TaskItem (documentationFile);
			}

			if (ObjectiveCLibraries is null)
				yield break;

			foreach (var item in ObjectiveCLibraries) {
				var linkWithFileName = String.Concat (Path.GetFileNameWithoutExtension (item.ItemSpec), ".linkwith.cs");
				yield return new TaskItem (linkWithFileName);
			}
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
