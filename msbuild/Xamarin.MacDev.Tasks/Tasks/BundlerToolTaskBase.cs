using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev;
using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	// A class to express what's shared/common between mtouch and mmp.
	public abstract class BundlerToolTaskBase : XamarinToolTask {

		#region Inputs
		[Required]
		public string AppBundleDir { get; set; }

		public string ArchiveSymbols { get; set; }

		[Required]
		public bool Debug { get; set; }

		[Required]
		public bool EnableSGenConc { get; set; }

		public string ExtraArgs { get; set; }

		[Required]
		public string HttpClientHandler { get; set; }

		public string I18n { get; set; }

		public string IntermediateOutputPath { get; set; }

		public bool IsAppExtension { get; set; }

		[Required]
		public string LinkMode { get; set; }

		[Required]
		public ITaskItem MainAssembly { get; set; }

		[Required]
		public string MinimumOSVersion { get; set; }

		public ITaskItem [] NativeReferences { get; set; }

		// Note: This property is used by XVS in order to calculate the Mac-equivalent paths for the MainAssembly and possibly other properties.
		[Required]
		public string OutputPath { get; set; }

		[Required]
		public bool Profiling { get; set; }

		[Required]
		public ITaskItem [] References { get; set; }

		public string Registrar { get; set; }

		[Required]
		public string ResponseFilePath { get; set; }

		[Required]
		public string SdkRoot { get; set; }

		[Required]
		public string SdkVersion { get; set; }

		public int Verbosity { get; set; }

		[Required]
		public string XamarinSdkRoot { get; set; }
		#endregion

		protected override string GenerateFullPathToTool ()
		{
			if (!string.IsNullOrEmpty (ToolPath))
				return Path.Combine (ToolPath, ToolExe);

			return Path.Combine (XamarinSdkRoot, "bin", ToolExe);
		}

		// Returns command line arguments common to both mtouch and mmp
		protected CommandLineArgumentBuilder GenerateCommandLineArguments ()
		{
			var args = new CommandLineArgumentBuilder ();

			if (!string.IsNullOrEmpty (Registrar))
				args.AddLine ($"--registrar:" + Registrar);

			if (bool.TryParse (ArchiveSymbols?.Trim (), out var msym))
				args.AddLine ($"--msym={(msym ? "yes" : "no")}");

			if (Debug)
				args.AddLine ("--debug");

			if (EnableSGenConc)
				args.AddLine ("--sgen-conc");

			args.AddLine ("/target-framework:" + TargetFrameworkMoniker);

			args.AddLine (string.Format ("--http-message-handler={0}", HttpClientHandler));

			if (!string.IsNullOrEmpty (I18n))
				args.AddQuotedLine ($"--i18n={I18n}");

			if (!string.IsNullOrEmpty (IntermediateOutputPath)) {
				Directory.CreateDirectory (IntermediateOutputPath);

				args.AddQuotedLine ($"--cache={Path.GetFullPath (IntermediateOutputPath)}");
			}

			args.AddQuotedLine ($"--root-assembly={Path.GetFullPath (MainAssembly.ItemSpec)}");

			if (Profiling)
				args.AddLine ("--profiling");

			args.AddQuotedLine ($"--sdkroot={SdkRoot}");

			args.AddQuotedLine ($"--targetver={MinimumOSVersion}");

			var v = VerbosityUtils.Merge (ExtraArgs, (LoggerVerbosity) Verbosity);
			foreach (var arg in v)
				args.AddLine (arg);

			return args;
		}
	}
}
