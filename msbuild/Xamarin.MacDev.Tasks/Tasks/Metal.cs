using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev;
using Xamarin.MacDev.Tasks;
using Xamarin.Utils;
using Xamarin.Localization.MSBuild;
using Xamarin.Messaging.Build.Client;

namespace Xamarin.MacDev.Tasks {
	public class Metal : XamarinTask {
		CancellationTokenSource? cancellationTokenSource;

		#region Inputs

		[Required]
		public string IntermediateOutputPath { get; set; } = string.Empty;

		public string MetalPath { get; set; } = string.Empty;

		[Required]
		public string MinimumOSVersion { get; set; } = string.Empty;

		[Required]
		public string ProjectDir { get; set; } = string.Empty;

		[Required]
		public string ResourcePrefix { get; set; } = string.Empty;

		[Required]
		public string SdkDevPath { get; set; } = string.Empty;

		[Required]
		public string SdkVersion { get; set; } = string.Empty;

		[Required]
		public bool SdkIsSimulator { get; set; }

		[Required]
		public string SdkRoot { get; set; } = string.Empty;

		[Required]
		public ITaskItem? SourceFile { get; set; }

		#endregion

		[Output]
		public ITaskItem? OutputFile { get; set; }

		static string GetExecutable (List<string> arguments, string toolName, string toolPathOverride)
		{
			if (string.IsNullOrEmpty (toolPathOverride)) {
				arguments.Insert (0, toolName);
				return "xcrun";
			}
			return toolPathOverride;
		}

		public override bool Execute ()
		{
			if (ShouldExecuteRemotely ())
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			var env = new Dictionary<string, string?> {
				{ "SDKROOT", SdkRoot },
			};

			var prefixes = BundleResource.SplitResourcePrefixes (ResourcePrefix);
			var intermediate = Path.Combine (IntermediateOutputPath, MetalPath);
			var logicalName = BundleResource.GetLogicalName (this, ProjectDir, prefixes, SourceFile!);
			var path = Path.Combine (intermediate, logicalName);
			var args = new List<string> ();
			var dir = Path.GetDirectoryName (path);

			Directory.CreateDirectory (dir);

			OutputFile = new TaskItem (Path.ChangeExtension (path, ".air"));
			OutputFile.SetMetadata ("LogicalName", Path.ChangeExtension (logicalName, ".air"));

			var executable = GetExecutable (args, "metal", MetalPath);

			args.Add ("-arch");
			args.Add ("air64");
			args.Add ("-emit-llvm");
			args.Add ("-c");
			args.Add ("-gline-tables-only");
			args.Add ("-ffast-math");

			args.Add ("-serialize-diagnostics");
			args.Add (Path.ChangeExtension (path, ".dia"));

			args.Add ("-o");
			args.Add (Path.ChangeExtension (path, ".air"));

			if (Platform == ApplePlatform.MacCatalyst) {
				args.Add ($"-target");
				args.Add ($"air64-apple-ios{MinimumOSVersion}-macabi");
			} else {
				args.Add (PlatformFrameworkHelper.GetMinimumVersionArgument (TargetFrameworkMoniker, SdkIsSimulator, MinimumOSVersion));
			}
			args.Add (SourceFile!.ItemSpec);

			cancellationTokenSource = new CancellationTokenSource ();
			ExecuteAsync (Log, executable, args, environment: env, cancellationToken: cancellationTokenSource.Token).Wait ();

			return !Log.HasLoggedErrors;
		}

		public void Cancel ()
		{
			if (ShouldExecuteRemotely ()) {
				BuildConnection.CancelAsync (BuildEngine4).Wait ();
			} else {
				cancellationTokenSource?.Cancel ();
			}
		}
	}
}
