// Copyright (C) 2011,2012 Xamarin, Inc. All rights reserved.

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.Build.Tasks;

using Xamarin.Utils;
using Xamarin.Localization.MSBuild;

namespace Xamarin.MacDev.Tasks {
	public abstract class BTouchTaskBase : XamarinToolTask {

		public string OutputPath { get; set; }

		[Required]
		public string BTouchToolPath { get; set; }

		[Required]
		public string BTouchToolExe { get; set; }

		public ITaskItem [] ObjectiveCLibraries { get; set; }

		public ITaskItem [] AdditionalLibPaths { get; set; }

		public bool AllowUnsafeBlocks { get; set; }

		[Required]
		public string BaseLibDll { get; set; }

		[Required]
		public ITaskItem [] ApiDefinitions { get; set; }

		public string AttributeAssembly { get; set; }

		public ITaskItem CompiledApiDefinitionAssembly { get; set; }

		public ITaskItem [] CoreSources { get; set; }

		public string DefineConstants { get; set; }

		public bool EmitDebugInformation { get; set; }

		public string ExtraArgs { get; set; }

		public int Verbosity { get; set; }

		public string GeneratedSourcesDir { get; set; }

		public string GeneratedSourcesFileList { get; set; }

		public string Namespace { get; set; }

		public bool NoNFloatUsing { get; set; }

		public ITaskItem [] NativeLibraries { get; set; }

		public string OutputAssembly { get; set; }

		public bool ProcessEnums { get; set; }

		[Required]
		public string ProjectDir { get; set; }

		public ITaskItem [] References { get; set; }

		public ITaskItem [] Resources { get; set; }

		public ITaskItem [] Sources { get; set; }

		[Required]
		public string ResponseFilePath { get; set; }

		protected override string ToolName {
			get {
				if (IsDotNet)
					return Path.GetFileName (this.GetDotNetPath ());

				return Path.GetFileNameWithoutExtension (ToolExe);
			}
		}

		protected override string GenerateFullPathToTool ()
		{
			// If we're building a .NET app, executing bgen using the same
			// dotnet binary as we're executed with, instead of using the
			// wrapper bgen script, because that script will try to use the
			// system dotnet, which might not exist or not have the version we
			// need.
			if (IsDotNet)
				return this.GetDotNetPath ();

			return Path.Combine (ToolPath, ToolExe);
		}

		protected virtual void HandleReferences (CommandLineArgumentBuilder cmd)
		{
			if (References is not null) {
				foreach (var item in References)
					cmd.AddQuoted ("-r:" + Path.GetFullPath (item.ItemSpec));
			}
		}

		protected override string GenerateCommandLineCommands ()
		{
			var cmd = new CommandLineArgumentBuilder ();

#if DEBUG
			cmd.Add ("/v");
#endif

			if (CompiledApiDefinitionAssembly is not null)
				cmd.AddQuotedSwitchIfNotNull ("/compiled-api-definition-assembly:", CompiledApiDefinitionAssembly.ItemSpec);

			cmd.Add ("/nostdlib");
			cmd.AddQuotedSwitchIfNotNull ("/baselib:", BaseLibDll);
			cmd.AddQuotedSwitchIfNotNull ("/out:", OutputAssembly);

			cmd.AddQuotedSwitchIfNotNull ("/attributelib:", AttributeAssembly);

			string dir;
			if (!string.IsNullOrEmpty (BaseLibDll)) {
				dir = Path.GetDirectoryName (BaseLibDll);
				cmd.AddQuotedSwitchIfNotNull ("/lib:", dir);
			}

			if (ProcessEnums)
				cmd.Add ("/process-enums");

			if (EmitDebugInformation)
				cmd.Add ("/debug");

			if (AllowUnsafeBlocks)
				cmd.Add ("/unsafe");

			cmd.AddQuotedSwitchIfNotNull ("/ns:", Namespace);

			if (NoNFloatUsing)
				cmd.Add ("/no-nfloat-using:true");

			if (!string.IsNullOrEmpty (DefineConstants)) {
				var strv = DefineConstants.Split (new [] { ';' }, StringSplitOptions.RemoveEmptyEntries);
				foreach (var str in strv)
					cmd.AddQuoted ("/d:" + str);
			}

			//cmd.AppendSwitch ("/e");

			foreach (var item in ApiDefinitions)
				cmd.AddQuoted (Path.GetFullPath (item.ItemSpec));

			if (CoreSources is not null) {
				foreach (var item in CoreSources)
					cmd.AddQuoted ("/s:" + Path.GetFullPath (item.ItemSpec));
			}

			if (Sources is not null) {
				foreach (var item in Sources)
					cmd.AddQuoted ("/x:" + Path.GetFullPath (item.ItemSpec));
			}

			if (AdditionalLibPaths is not null) {
				foreach (var item in AdditionalLibPaths)
					cmd.AddQuoted ("/lib:" + Path.GetFullPath (item.ItemSpec));
			}

			HandleReferences (cmd);

			if (Resources is not null) {
				foreach (var item in Resources) {
					var argument = item.ToString ();
					var id = item.GetMetadata ("LogicalName");
					if (!string.IsNullOrEmpty (id))
						argument += "," + id;

					cmd.AddQuoted ("/res:" + argument);
				}
			}

			if (NativeLibraries is not null) {
				foreach (var item in NativeLibraries) {
					var argument = item.ToString ();
					var id = item.GetMetadata ("LogicalName");
					if (string.IsNullOrEmpty (id))
						id = Path.GetFileName (argument);

					cmd.AddQuoted ("/res:" + argument + "," + id);
				}
			}

			if (GeneratedSourcesDir is not null)
				cmd.AddQuoted ("/tmpdir:" + Path.GetFullPath (GeneratedSourcesDir));

			if (GeneratedSourcesFileList is not null)
				cmd.AddQuoted ("/sourceonly:" + Path.GetFullPath (GeneratedSourcesFileList));

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

			cmd.Add (VerbosityUtils.Merge (ExtraArgs, (LoggerVerbosity) Verbosity));

			var commandLine = cmd.CreateResponseFile (this, ResponseFilePath, null);
			if (IsDotNet)
				commandLine = StringUtils.Quote (Path.Combine (BTouchToolPath, BTouchToolExe)) + " " + commandLine;

			return commandLine.ToString ();
		}

		public override bool Execute ()
		{
			AttributeAssembly = PathUtils.ConvertToMacPath (AttributeAssembly);
			BaseLibDll = PathUtils.ConvertToMacPath (BaseLibDll);
			BTouchToolExe = PathUtils.ConvertToMacPath (BTouchToolExe);
			BTouchToolPath = PathUtils.ConvertToMacPath (BTouchToolPath);

			if (IsDotNet) {
				var customHome = Environment.GetEnvironmentVariable ("DOTNET_CUSTOM_HOME");

				if (!string.IsNullOrEmpty (customHome)) {
					EnvironmentVariables = EnvironmentVariables.CopyAndAdd ($"HOME={customHome}");
				}
			} else {
				ToolExe = BTouchToolExe;
				ToolPath = BTouchToolPath;
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

			return base.Execute ();
		}
	}
}
