using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.Messaging.Build.Client;

#nullable enable

namespace Xamarin.MacDev.Tasks {
	public class SpotlightIndexer : XamarinTask, ICancelableTask {
		CancellationTokenSource? cancellationTokenSource;
		#region Inputs

		[Required]
		public string Input { get; set; } = string.Empty;

		public string MdimportPath { get; set; } = string.Empty;

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

			var args = new List<string> () {
				Input
			};
			var executable = GetExecutable (args, "mdimport", MdimportPath);
			cancellationTokenSource = new CancellationTokenSource ();
			ExecuteAsync (Log, executable, args, cancellationToken: cancellationTokenSource.Token).Wait ();
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
