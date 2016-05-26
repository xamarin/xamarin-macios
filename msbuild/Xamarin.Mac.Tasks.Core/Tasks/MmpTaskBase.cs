//
// MmpTask.cs
//
// Author:
//   Aaron Bockover <abock@xamarin.com>
//
// Copyright 2014 Xamarin Inc.

using System;
using System.IO;

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
		public string FrameworkRoot { get; set; }

		[Required]
		public string OutputPath { get; set; }

		[Required]
		public string ApplicationAssembly { get; set; }

		[Required]
		public string HttpClientHandler { get; set; }

		[Required]
		public string TargetFrameworkIdentifier { get; set; }

		[Required]
		public string TargetFrameworkVersion { get; set; }

		[Required]
		public string TLSProvider {	get; set; }

		[Required]
		public string SdkRoot {	get; set; }

		[Required]
		public ITaskItem AppManifest { get; set; }

		[Required]
		public string SdkVersion { get; set; }

		public bool IsAppExtension { get; set; }

		public bool UseXamMacFullFramework { get; set; }

		public string ApplicationName { get; set; }
		public string Architecture { get; set; }
		public string LinkMode { get; set; }
		public bool Debug { get; set; }
		public bool Profiling { get; set; }
		public string I18n { get; set; }
		public string ExtraArguments { get; set; }

		public string [] ExplicitReferences { get; set; }
		public string [] NativeReferences { get; set; }

		public string IntermediateOutputPath { get; set; }
		
		protected override string GenerateFullPathToTool ()
		{
			return Path.Combine (FrameworkRoot, "bin", "mmp");
		}

		protected override bool ValidateParameters ()
		{
			XamMacArch arch;
			return Enum.TryParse<XamMacArch> (Architecture, true, out arch);
		}

		protected override string GenerateCommandLineCommands ()
		{
			var args = new ProcessArgumentBuilder ();

			args.Add ("/verbose");

			if (Debug)
				args.Add ("/debug");

			if (!string.IsNullOrEmpty (OutputPath))
				args.AddQuoted ("/output:" + Path.GetFullPath (OutputPath));

			if (!string.IsNullOrEmpty (ApplicationName))
				args.AddQuoted ("/name:" + ApplicationName);

			if (TargetFrameworkIdentifier == "Xamarin.Mac")
				args.Add ("/profile:Xamarin.Mac");
			else if (TargetFrameworkVersion.StartsWith ("v", StringComparison.Ordinal))
				args.Add ("/profile:" + TargetFrameworkVersion.Substring (1));

			if (TargetFrameworkIdentifier == "Xamarin.Mac" || UseXamMacFullFramework) {
				XamMacArch arch;
				if (!Enum.TryParse<XamMacArch> (Architecture, true, out arch))
					arch = XamMacArch.Default;

				if (arch == XamMacArch.Default)
					arch = XamMacArch.x86_64;

				if (arch.HasFlag (XamMacArch.i386))
					args.Add ("/arch:i386");

				if (arch.HasFlag (XamMacArch.x86_64))
					args.Add ("/arch:x86_64");
			}
			else {
				args.Add ("/arch:i386");
			}

			args.Add (string.Format ("--http-message-handler={0}", HttpClientHandler));

			if (AppManifest != null) {
				try {
					var plist = PDictionary.FromFile (AppManifest.ItemSpec);

					PString v;
					string minimumDeploymentTarget;

					if (!plist.TryGetValue (ManifestKeys.LSMinimumSystemVersion, out v) || string.IsNullOrEmpty (v.Value))
						minimumDeploymentTarget = SdkVersion;
					else
						minimumDeploymentTarget = v.Value;

					args.Add (string.Format("/minos={0}", minimumDeploymentTarget));
				}
				catch (Exception ex) {
					Log.LogWarning (null, null, null, AppManifest.ItemSpec, 0, 0, 0, 0, "Error loading '{0}': {1}", AppManifest.ItemSpec, ex.Message);
				}
			}

			if (TargetFrameworkIdentifier == "Xamarin.Mac" && !string.IsNullOrEmpty (TLSProvider))
				args.Add (string.Format ("--tls-provider={0}", TLSProvider.ToLowerInvariant()));

			if (Profiling)
				args.Add ("/profiling");

			switch ((LinkMode ?? String.Empty).ToLower ()) {
			case "full":
				break;
			case "sdkonly":
				args.Add ("/linksdkonly");
				break;
			default:
				args.Add ("/nolink");
				break;
			}

			if (!string.IsNullOrEmpty (I18n))
				args.AddQuoted ("/i18n:" + I18n);

			if (ExplicitReferences != null) {
				foreach (var asm in ExplicitReferences)
					args.AddQuoted ("/assembly:" + Path.GetFullPath (asm));
			}

			if (!string.IsNullOrEmpty (ApplicationAssembly)) {
				args.AddQuoted (Path.GetFullPath (ApplicationAssembly + (IsAppExtension ? ".dll" : ".exe")));
			}

			if (!string.IsNullOrWhiteSpace (ExtraArguments))
				args.Add (ExtraArguments);

			if (NativeReferences != null) {
				foreach (var nr in NativeReferences)
					args.AddQuoted ("/native-reference:" + Path.GetFullPath (nr));
			}
				
			if (IsAppExtension)
				args.AddQuoted ("/extension");

			args.Add ("/sdkroot");
			args.AddQuoted (SdkRoot);

			if (!string.IsNullOrEmpty (IntermediateOutputPath)) {
				Directory.CreateDirectory (IntermediateOutputPath);

				args.Add ("--cache");
				args.AddQuoted (Path.GetFullPath (IntermediateOutputPath));
			}

			return args.ToString ();
		}

		public override bool Execute ()
		{
			Log.LogTaskName ("Mmp");
			Log.LogTaskProperty ("ApplicationAssembly", ApplicationAssembly + (IsAppExtension ? ".dll" : ".exe"));
			Log.LogTaskProperty ("ApplicationName", ApplicationName);
			Log.LogTaskProperty ("Architecture", Architecture);
			Log.LogTaskProperty ("Debug", Debug);
			Log.LogTaskProperty ("ExplicitReferences", ExplicitReferences);
			Log.LogTaskProperty ("ExtraArguments", ExtraArguments);
			Log.LogTaskProperty ("FrameworkRoot", FrameworkRoot);
			Log.LogTaskProperty ("I18n", I18n);
			Log.LogTaskProperty ("IntermediateOutputPath", IntermediateOutputPath);
			Log.LogTaskProperty ("LinkMode", LinkMode);
			Log.LogTaskProperty ("OutputPath", OutputPath);
			Log.LogTaskProperty ("SdkRoot", SdkRoot);
			Log.LogTaskProperty ("TargetFrameworkIdentifier", TargetFrameworkIdentifier);
			Log.LogTaskProperty ("TargetFrameworkVersion", TargetFrameworkVersion);
			Log.LogTaskProperty ("TLSProvider", TLSProvider);
			Log.LogTaskProperty ("UseXamMacFullFramework", UseXamMacFullFramework);
			Log.LogTaskProperty ("Profiling", Profiling);
			Log.LogTaskProperty ("AppManifest", AppManifest);
			Log.LogTaskProperty ("SdkVersion", SdkVersion);
			Log.LogTaskProperty ("NativeReferences", NativeReferences);
			Log.LogTaskProperty ("IsAppExtension", IsAppExtension);

			return base.Execute ();
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
