using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.Localization.MSBuild;
using Xamarin.Messaging.Build.Client;
using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	public class MetalLib : XamarinTask, ITaskCallback {
		CancellationTokenSource? cancellationTokenSource;

		#region Inputs

		[Required]
		public ITaskItem [] Items { get; set; } = Array.Empty<ITaskItem> ();

		public string MetalLibPath { get; set; } = string.Empty;

		[Required]
		public string OutputLibrary { get; set; } = string.Empty;

		[Required]
		public string SdkDevPath { get; set; } = string.Empty;

		[Required]
		public string SdkRoot { get; set; } = string.Empty;

		#endregion

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

			var dir = Path.GetDirectoryName (OutputLibrary);
			Directory.CreateDirectory (dir);

			var env = new Dictionary<string, string?> {
				{ "SDKROOT", SdkRoot },
			};

			var args = new List<string> ();
			args.Add ("-o");
			args.Add (OutputLibrary);
			foreach (var item in Items)
				args.Add (item.ItemSpec);

			var executable = GetExecutable (args, "metallib", MetalLibPath);

			cancellationTokenSource = new CancellationTokenSource ();
			ExecuteAsync (Log, executable, args, environment: env, cancellationToken: cancellationTokenSource.Token).Wait ();

			return !Log.HasLoggedErrors;
		}

		public bool ShouldCopyToBuildServer (ITaskItem item) => false;

		public bool ShouldCreateOutputFile (ITaskItem item) => true;

		public IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied () => Enumerable.Empty<ITaskItem> ();

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
