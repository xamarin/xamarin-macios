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
using Xamarin.Utils;
using Xamarin.Localization.MSBuild;

namespace Xamarin.Mac.Tasks {
	public class Mmp : BundlerToolTaskBase {
		protected override string ToolName {
			get { return "mmp"; }
		}

		public bool IsXPCService { get; set; }

		public string ApplicationName { get; set; }
		public string Architecture { get; set; }

		public string AotMode { get; set; }
		public bool HybridAOT { get; set; }
		public string ExplicitAotAssemblies { get; set; }

		[Required]
		public string CustomBundleName { get; set; }

		protected override bool ValidateParameters ()
		{
			XamMacArch arch;

			return Enum.TryParse (Architecture, true, out arch);
		}

		protected override string GenerateCommandLineCommands ()
		{
			var args = GenerateCommandLineArguments ();

			if (!string.IsNullOrEmpty (OutputPath))
				args.AddQuotedLine ("/output:" + Path.GetFullPath (OutputPath));

			if (!string.IsNullOrEmpty (ApplicationName))
				args.AddQuotedLine ("/name:" + ApplicationName);

			XamMacArch arch;
			if (!Enum.TryParse (Architecture, true, out arch))
				arch = XamMacArch.Default;

			if (arch == XamMacArch.Default)
				arch = XamMacArch.x86_64;

			List<string> allSupportedArchs = new List<string> ();
			if (arch.HasFlag (XamMacArch.i386))
				allSupportedArchs.Add ("i386");

			if (arch.HasFlag (XamMacArch.x86_64))
				allSupportedArchs.Add ("x86_64");

			if (arch.HasFlag (XamMacArch.ARM64))
				allSupportedArchs.Add ("arm64");

			args.AddLine ($"/abi:{string.Join (",", allSupportedArchs)}");

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

			if (!string.IsNullOrEmpty (AotMode) && AotMode != "None") {
				var aot = $"--aot:{AotMode.ToLower ()}";
				if (HybridAOT)
					aot += "|hybrid";

				if (!string.IsNullOrEmpty (ExplicitAotAssemblies))
					aot += $",{ExplicitAotAssemblies}";

				args.AddLine (aot);
			}

			if (References is not null) {
				foreach (var asm in References)
					args.AddQuotedLine ("/reference:" + Path.GetFullPath (asm.ItemSpec));
			}

			if (NativeReferences is not null) {
				foreach (var nr in NativeReferences)
					args.AddQuotedLine ("/native-reference:" + Path.GetFullPath (nr.ItemSpec));
			}

			if (IsAppExtension)
				args.AddQuotedLine ("/extension");
			if (IsXPCService)
				args.AddQuotedLine ("/xpc");

			return args.CreateResponseFile (this, ResponseFilePath, ExtraArgs is null ? null : CommandLineArgumentBuilder.Parse (ExtraArgs));
		}

		public override bool Execute ()
		{
			if (!base.Execute ())
				return false;

			return !Log.HasLoggedErrors;
		}

		protected override void LogEventsFromTextOutput (string singleLine, MessageImportance messageImportance)
		{
			try { // We first try to use the base logic, which shows up nicely in XS.
				base.LogEventsFromTextOutput (singleLine, messageImportance);
			} catch { // But when that fails, just output the message to the command line and XS will output it raw
				Log.LogMessage (messageImportance, "{0}", singleLine);
			}
		}
	}
}
