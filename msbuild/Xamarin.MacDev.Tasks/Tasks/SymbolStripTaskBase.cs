using System;
using System.IO;
using System.Collections.Generic;

using Parallel = System.Threading.Tasks.Parallel;
using ParallelOptions = System.Threading.Tasks.ParallelOptions;

using Microsoft.Build.Framework;

#nullable enable

namespace Xamarin.MacDev.Tasks {
	public abstract class SymbolStripTaskBase : XamarinTask {
		#region Inputs

		[Required]
		public ITaskItem [] Executable { get; set; } = Array.Empty<ITaskItem> ();

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

		void ExecuteStrip (ITaskItem item)
		{
			var args = new List<string> ();

			args.Add ("strip");

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

			ExecuteAsync ("xcrun", args).Wait ();
		}

		public override bool Execute ()
		{
			Parallel.ForEach (Executable, new ParallelOptions { MaxDegreeOfParallelism = Math.Max (Environment.ProcessorCount / 2, 1) }, (item) => {
				ExecuteStrip (item);
			});

			return !Log.HasLoggedErrors;
		}
	}
}
