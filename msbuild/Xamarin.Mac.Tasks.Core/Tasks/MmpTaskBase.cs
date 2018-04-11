//
// MmpTask.cs
//
// Author:
//   Aaron Bockover <abock@xamarin.com>
//
// Copyright 2014 Xamarin Inc.

using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev.Tasks;
using Xamarin.MacDev;

namespace Xamarin.Mac.Tasks
{
	public class MmpTaskBase : ToolTask
	{
		protected override string ToolName {
			get { return "mmp"; }
		}

		public string SessionId { get; set; }

		[Required]
		public string AppBundleDir { get; set; }

		[Required]
		public string FrameworkRoot { get; set; }

		[Required]
		public string OutputPath { get; set; }

		[Required]
		public ITaskItem ApplicationAssembly { get; set; }

		[Required]
		public string HttpClientHandler { get; set; }

		[Required]
		public string TargetFrameworkIdentifier { get; set; }

		[Required]
		public string TargetFrameworkVersion { get; set; }

		[Required]
		public string SdkRoot {	get; set; }

		[Required]
		public ITaskItem AppManifest { get; set; }

		[Required]
		public string SdkVersion { get; set; }

		public bool IsAppExtension { get; set; }

		[Required]
		public bool EnableSGenConc { get; set; }

		public bool UseXamMacFullFramework { get; set; }

		public string ApplicationName { get; set; }
		public string ArchiveSymbols { get; set; }
		public string Architecture { get; set; }
		public string LinkMode { get; set; }
		public bool Debug { get; set; }
		public bool Profiling { get; set; }
		public string I18n { get; set; }
		public string ExtraArguments { get; set; }

		public string AotScope { get; set; }
		public bool HybridAotOption { get; set; }
		public string ExplicitAotAssemblies { get; set; }

		public ITaskItem [] ExplicitReferences { get; set; }
		public ITaskItem [] NativeReferences { get; set; }

		[Required]
		public string ResponseFilePath { get; set; }

		public string IntermediateOutputPath { get; set; }

		[Output]
		public ITaskItem[] NativeLibraries { get; set; }
		
		protected override string GenerateFullPathToTool ()
		{
			return Path.Combine (FrameworkRoot, "bin", "mmp");
		}

		protected override bool ValidateParameters ()
		{
			XamMacArch arch;

			return Enum.TryParse (Architecture, true, out arch);
		}

		protected override string GenerateCommandLineCommands ()
		{
			var args = new CommandLineArgumentBuilder ();
			bool msym;

			args.AddLine ("/verbose");

			if (Debug)
				args.AddLine ("/debug");

			if (!string.IsNullOrEmpty (OutputPath))
				args.AddQuotedLine ("/output:" + Path.GetFullPath (OutputPath));

			if (!string.IsNullOrEmpty (ApplicationName))
				args.AddQuotedLine ("/name:" + ApplicationName);

			if (TargetFrameworkIdentifier == "Xamarin.Mac")
				args.AddLine ("/profile:Xamarin.Mac,Version=v2.0,Profile=Mobile");
			else if (UseXamMacFullFramework)
				args.AddLine ($"/profile:Xamarin.Mac,Version={TargetFrameworkVersion},Profile=Full");
			else
				args.AddLine ($"/profile:Xamarin.Mac,Version={TargetFrameworkVersion},Profile=System");

			XamMacArch arch;
			if (!Enum.TryParse (Architecture, true, out arch))
				arch = XamMacArch.Default;

			if (arch == XamMacArch.Default)
				arch = XamMacArch.x86_64;

			if (arch.HasFlag (XamMacArch.i386))
				args.AddLine ("/arch:i386");

			if (arch.HasFlag (XamMacArch.x86_64))
				args.AddLine ("/arch:x86_64");

			if (!string.IsNullOrEmpty (ArchiveSymbols) && bool.TryParse (ArchiveSymbols.Trim (), out msym))
				args.AddLine ("--msym:" + (msym ? "yes" : "no"));

			args.AddLine (string.Format ("--http-message-handler={0}", HttpClientHandler));

			if (AppManifest != null) {
				try {
					var plist = PDictionary.FromFile (AppManifest.ItemSpec);

					PString v;
					string minimumDeploymentTarget;

					if (!plist.TryGetValue (ManifestKeys.LSMinimumSystemVersion, out v) || string.IsNullOrEmpty (v.Value))
						minimumDeploymentTarget = SdkVersion;
					else
						minimumDeploymentTarget = v.Value;

					args.AddLine (string.Format("/minos={0}", minimumDeploymentTarget));
				}
				catch (Exception ex) {
					Log.LogWarning (null, null, null, AppManifest.ItemSpec, 0, 0, 0, 0, "Error loading '{0}': {1}", AppManifest.ItemSpec, ex.Message);
				}
			}

			if (Profiling)
				args.AddLine ("/profiling");

			if (EnableSGenConc)
				args.AddLine ("/sgen-conc");

			switch ((LinkMode ?? string.Empty).ToLower ()) {
			case "full":
				break;
			case "sdkonly":
				args.AddLine ("/linksdkonly");
				break;
			case "platform":
				args.AddLine ("/linkplatform");
				break;
			default:
				args.AddLine ("/nolink");
				break;
			}

			if (!string.IsNullOrEmpty (AotScope) && AotScope != "None") {
				var aot = $"--aot:{AotScope.ToLower ()}";
				if (HybridAotOption)
					aot += "|hybrid";

				if (!string.IsNullOrEmpty (ExplicitAotAssemblies))
					aot += $",{ExplicitAotAssemblies}";

				args.AddLine (aot);
			}

			if (!string.IsNullOrEmpty (I18n))
				args.AddQuotedLine ("/i18n:" + I18n);

			if (ExplicitReferences != null) {
				foreach (var asm in ExplicitReferences)
					args.AddQuotedLine ("/assembly:" + Path.GetFullPath (asm.ItemSpec));
			}

			if (!string.IsNullOrEmpty (ApplicationAssembly.ItemSpec)) {
				args.AddQuotedLine ("/root-assembly:" + Path.GetFullPath (ApplicationAssembly.ItemSpec));
			}

			if (!string.IsNullOrWhiteSpace (ExtraArguments))
				args.AddLine (ExtraArguments);

			if (NativeReferences != null) {
				foreach (var nr in NativeReferences)
					args.AddQuotedLine ("/native-reference:" + Path.GetFullPath (nr.ItemSpec));
			}
				
			if (IsAppExtension)
				args.AddQuotedLine ("/extension");

			args.AddQuotedLine ("/sdkroot:" + SdkRoot);

			if (!string.IsNullOrEmpty (IntermediateOutputPath)) {
				Directory.CreateDirectory (IntermediateOutputPath);

				args.AddQuotedLine ("--cache:" + Path.GetFullPath (IntermediateOutputPath));
			}

			// Generate a response file
			var responseFile = Path.GetFullPath (ResponseFilePath);

			if (File.Exists (responseFile))
				File.Delete (responseFile);

			try {
				using (var fs = File.Create (responseFile)) {
					using (var writer = new StreamWriter (fs))
						writer.Write (args);
				}
			} catch (Exception ex) {
				Log.LogWarning ("Failed to create response file '{0}': {1}", responseFile, ex);
			}

			// Use only the response file
			args = new CommandLineArgumentBuilder ();
			args.AddQuotedLine ($"@{responseFile}");

			return args.ToString ();
		}

		string GetMonoBundleDirName ()
		{
			if (!string.IsNullOrEmpty (ExtraArguments)) {
				var args = CommandLineArgumentBuilder.Parse (ExtraArguments);

				for (int i = 0; i < args.Length; i++) {
					string arg;

					if (string.IsNullOrEmpty (args[i]))
						continue;

					if (args[i][0] == '/') {
						arg = args[i].Substring (1);
					} else if (args[i][0] == '-') {
						if (args[i].Length >= 2 && args[i][1] == '-')
							arg = args[i].Substring (2);
						else
							arg = args[i].Substring (1);
					} else {
						continue;
					}

					if (arg.StartsWith ("custom_bundle_name:", StringComparison.Ordinal) ||
					    arg.StartsWith ("custom_bundle_name=", StringComparison.Ordinal))
						return arg.Substring ("custom_bundle_name=".Length);

					if (arg == "custom_bundle_name" && i + 1 < args.Length)
						return args[i + 1];
				}
			}

			return "MonoBundle";
		}

		public override bool Execute ()
		{
			if (!base.Execute ())
				return false;

			var monoBundleDir = Path.Combine (AppBundleDir, "Contents", GetMonoBundleDirName ());

			try {
				var nativeLibrariesPath = Directory.EnumerateFiles (monoBundleDir, "*.dylib", SearchOption.AllDirectories);
				var nativeLibraryItems = new List<ITaskItem> ();

				foreach (var nativeLibrary in nativeLibrariesPath) {
					nativeLibraryItems.Add (new TaskItem (nativeLibrary));
				}

				NativeLibraries = nativeLibraryItems.ToArray ();
			} catch (Exception ex) {
				Log.LogError (null, null, null, AppBundleDir, 0, 0, 0, 0, "Could not get native libraries: {0}", ex.Message);
				return false;
			}

			return !Log.HasLoggedErrors;
		}

		protected override void LogEventsFromTextOutput (string singleLine, MessageImportance messageImportance)
		{
			try { // We first try to use the base logic, which shows up nicely in XS.
				base.LogEventsFromTextOutput (singleLine, messageImportance);
			}
			catch { // But when that fails, just output the message to the command line and XS will output it raw
				Log.LogMessage (messageImportance, "{0}", singleLine);
			}
		}
	}
}
