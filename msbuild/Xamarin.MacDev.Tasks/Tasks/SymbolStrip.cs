using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

using Parallel = System.Threading.Tasks.Parallel;
using ParallelOptions = System.Threading.Tasks.ParallelOptions;

using Microsoft.Build.Framework;
using Xamarin.Messaging.Build.Client;

#nullable enable

namespace Xamarin.MacDev.Tasks {
	public class SymbolStrip : XamarinParallelTask, ITaskCallback {
		#region Inputs

		[Required]
		public ITaskItem [] Executable { get; set; } = Array.Empty<ITaskItem> ();

		public string StripPath { get; set; } = string.Empty;

		// This can also be specified as metadata on the Executable item (as 'SymbolFile')
		public string SymbolFile { get; set; } = string.Empty;

		// This can also be specified as metadata on the Executable item (as 'Kind')
		public string Kind { get; set; } = string.Empty;
		#endregion

		bool GetIsFramework (ITaskItem item)
		{
			var value = GetNonEmptyStringOrFallback (item, "Kind", Kind);
			return string.Equals (value, "Framework", StringComparison.OrdinalIgnoreCase);
		}

		static string GetExecutable (List<string> arguments, string toolName, string toolPathOverride)
		{
			if (string.IsNullOrEmpty (toolPathOverride)) {
				arguments.Insert (0, toolName);
				return "xcrun";
			}
			return toolPathOverride;
		}

		void ExecuteStrip (ITaskItem item)
		{
			var args = new List<string> ();
			var executable = GetExecutable (args, "strip", StripPath);

			var symbolFile = GetNonEmptyStringOrFallback (item, "SymbolFile", SymbolFile);
			if (!string.IsNullOrEmpty (symbolFile)) {
				args.Add ("-i");
				args.Add ("-s");
				args.Add (symbolFile);
			}

			if (GetIsFramework (item)) {
				// Only remove debug symbols from frameworks.
				args.Add ("-S");
				args.Add ("-x");
			}

			args.Add (Path.GetFullPath (item.ItemSpec));

			ExecuteAsync (executable, args).Wait ();
		}

		public override bool Execute ()
		{
			if (ShouldExecuteRemotely ())
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			ForEach (Executable, (item) => {
				ExecuteStrip (item);
			});

			return !Log.HasLoggedErrors;
		}

		public bool ShouldCopyToBuildServer (ITaskItem item) => false;

		public bool ShouldCreateOutputFile (ITaskItem item) => false;

		public IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied () => Enumerable.Empty<ITaskItem> ();
	}
}
