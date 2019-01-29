// Copyright (C) 2011,2012 Xamarin, Inc. All rights reserved.

using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Xamarin.MacDev.Tasks {
	public abstract class BTouchTaskBase : ToolTask {

		public string SessionId { get; set; }

		public string OutputPath { get; set; }

		[Required]
		public string BTouchToolPath { get; set; }

		[Required]
		public string BTouchToolExe { get; set; }

		public ITaskItem[] ObjectiveCLibraries { get; set; }

		public ITaskItem[] AdditionalLibPaths { get; set; }

		public bool AllowUnsafeBlocks { get; set; }

		[Required]
		public string BaseLibDll { get; set; }

		[Required]
		public ITaskItem[] ApiDefinitions { get; set; }

		public ITaskItem[] CoreSources { get; set; }

		public string DefineConstants { get; set; }

		public bool EmitDebugInformation { get; set; }

		public string ExtraArgs { get; set; }

		public string GeneratedSourcesDir { get; set; }

		public string GeneratedSourcesFileList { get; set; }

		public string Namespace { get; set; }

		public ITaskItem[] NativeLibraries { get; set; }

		public string OutputAssembly { get; set; }

		public bool ProcessEnums { get; set; }

		[Required]
		public string ProjectDir { get; set; }

		public ITaskItem[] References { get; set; }

		public ITaskItem[] Resources { get; set; }

		public ITaskItem[] Sources { get; set; }

		[Required]
		public string TargetFrameworkIdentifier { get; set; }

		protected override string ToolName {
			get { return Path.GetFileNameWithoutExtension (ToolExe); }
		}

		protected override string GenerateFullPathToTool ()
		{
			return Path.Combine (ToolPath, ToolExe);
		}

		protected virtual void HandleReferences (CommandLineBuilder cmd)
		{
			if (References != null) {
				foreach (var item in References)
					cmd.AppendSwitchIfNotNull ("-r ", Path.GetFullPath (item.ItemSpec));
			}
		}

		protected override string GenerateCommandLineCommands ()
		{
			var cmd = new CommandLineBuilder ();

			#if DEBUG
			cmd.AppendSwitch ("/v");
			#endif

			cmd.AppendSwitch ("/nostdlib");
			cmd.AppendSwitchIfNotNull ("/baselib:", BaseLibDll);
			cmd.AppendSwitchIfNotNull ("/out:", OutputAssembly);

			string dir;
			if (!string.IsNullOrEmpty (BaseLibDll)) {
				dir = Path.GetDirectoryName (BaseLibDll);
				cmd.AppendSwitchIfNotNull ("/lib:", dir);
				cmd.AppendSwitchIfNotNull ("/r:", Path.Combine (dir, "mscorlib.dll"));
			}

			if (ProcessEnums)
				cmd.AppendSwitch ("/process-enums");

			if (EmitDebugInformation)
				cmd.AppendSwitch ("/debug");

			if (AllowUnsafeBlocks)
				cmd.AppendSwitch ("/unsafe");

			cmd.AppendSwitchIfNotNull ("/ns:", Namespace);

			if (!string.IsNullOrEmpty (DefineConstants)) {
				var strv = DefineConstants.Split (new [] { ';' });
				var sanitized = new List<string> ();

				foreach (var str in strv) {
					if (str != string.Empty)
						sanitized.Add (str);
				}

				if (sanitized.Count > 0)
					cmd.AppendSwitchIfNotNull ("/d:", string.Join (";", sanitized.ToArray ()));
			}

			//cmd.AppendSwitch ("/e");

			foreach (var item in ApiDefinitions)
				cmd.AppendFileNameIfNotNull (Path.GetFullPath (item.ItemSpec));

			if (CoreSources != null) {
				foreach (var item in CoreSources)
					cmd.AppendSwitchIfNotNull ("/s:", Path.GetFullPath (item.ItemSpec));
			}

			if (Sources != null) {
				foreach (var item in Sources)
					cmd.AppendSwitchIfNotNull ("/x:", Path.GetFullPath (item.ItemSpec));
			}

			if (AdditionalLibPaths != null) {
				foreach (var item in AdditionalLibPaths)
					cmd.AppendSwitchIfNotNull ("/lib:", Path.GetFullPath (item.ItemSpec));
			}

			HandleReferences (cmd);

			if (Resources != null) {
				foreach (var item in Resources) {
					var args = new List<string> ();
					string id;

					args.Add (item.ToString ());
					id = item.GetMetadata ("LogicalName");
					if (!string.IsNullOrEmpty (id))
						args.Add (id);

					cmd.AppendSwitchIfNotNull ("/res:", args.ToArray (), ",");
				}
			}

			if (NativeLibraries != null) {
				foreach (var item in NativeLibraries) {
					var args = new List<string> ();
					string id;

					args.Add (item.ToString ());
					id = item.GetMetadata ("LogicalName");
					if (string.IsNullOrEmpty (id))
						id = Path.GetFileName (args[0]);
					args.Add (id);

					cmd.AppendSwitchIfNotNull ("/link-with:", args.ToArray (), ",");
				}
			}

			if (GeneratedSourcesDir != null)
				cmd.AppendSwitchIfNotNull ("/tmpdir:", Path.GetFullPath (GeneratedSourcesDir));

			if (GeneratedSourcesFileList != null)
				cmd.AppendSwitchIfNotNull ("/sourceonly:", Path.GetFullPath (GeneratedSourcesFileList));

			cmd.AppendSwitch (GetTargetFrameworkArgument ());

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
					// { "solutiondir",  proj.ParentSolution != null ? proj.ParentSolution.BaseDirectory : proj.BaseDirectory },
				};
				// OutputAssembly is optional so it can be null
				if (target != null) {
					var d = Path.GetDirectoryName (target);
					var n = Path.GetFileName (target);
					customTags.Add ("targetpath", Path.Combine (d, n));
					customTags.Add ("targetdir", d);
					customTags.Add ("targetname", n);
					customTags.Add ("targetext", Path.GetExtension (target));
				}

				for (int i = 0; i < extraArgs.Length; i++) {
					var argument = extraArgs[i];
					cmd.AppendTextUnquoted (" ");
					cmd.AppendTextUnquoted (StringParserService.Parse (argument, customTags));
				}
			}

			return cmd.ToString ();
		}

		protected virtual string GetTargetFrameworkArgument ()
		{
			switch (TargetFrameworkIdentifier) {
				case "MonoTouch":
				case "Xamarin.iOS":
				case "Xamarin.TVOS":
				case "Xamarin.WatchOS":
					return $"/target-framework={TargetFrameworkIdentifier},v1.0";
				default:
					Log.LogError ($"Unknown target framework identifier: {TargetFrameworkIdentifier}.");
					return string.Empty;
			}
		}

		public override bool Execute ()
		{
			ToolExe = BTouchToolExe;
			ToolPath = BTouchToolPath;

			if (!string.IsNullOrEmpty (SessionId) &&
			    !string.IsNullOrEmpty (GeneratedSourcesDir) &&
			    !Directory.Exists (GeneratedSourcesDir)) {
				Directory.CreateDirectory (GeneratedSourcesDir);
			}

			if (ApiDefinitions.Length == 0) {
				Log.LogError ("No API definition file specified.");
				return false;
			}

			return base.Execute ();
		}
	}
}
