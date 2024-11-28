using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using System.Text;

using Xamarin.Utils;
using Xamarin.Localization.MSBuild;
using Xamarin.Messaging.Build.Client;

namespace Xamarin.MacDev.Tasks {
	public abstract class ALToolTaskBase : XamarinTask, ICancelableTask {
		CancellationTokenSource? cancellationTokenSource;

		public string AltoolPath { get; set; } = string.Empty;

		[Required]
		public string Username { get; set; } = string.Empty;

		[Required]
		public string Password { get; set; } = string.Empty;

		[Required]
		public string FilePath { get; set; } = string.Empty;

		static string GetExecutable (List<string> arguments, string toolName, string toolPathOverride)
		{
			if (string.IsNullOrEmpty (toolPathOverride)) {
				arguments.Insert (0, toolName);
				return "xcrun";
			}
			return toolPathOverride;
		}

		[Required]
		public string SdkDevPath { get; set; } = string.Empty;

		protected abstract string ALToolAction { get; }

		public override bool Execute ()
		{
			if (ShouldExecuteRemotely ())
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			var args = GenerateCommandLineCommands ();
			var executable = GetExecutable (args, "altool", AltoolPath);

			if (Log.HasLoggedErrors)
				return false;

			cancellationTokenSource = new CancellationTokenSource ();
			var rv = ExecuteAsync (Log, executable, args, sdkDevPath: SdkDevPath, cancellationToken: cancellationTokenSource.Token).Result;
			LogErrorsFromOutput (rv.StandardOutput?.ToString ());
			return !Log.HasLoggedErrors;
		}

		protected virtual List<string> GenerateCommandLineCommands ()
		{
			var args = new List<string> ();

			args.Add (ALToolAction);
			args.Add ("--file");
			args.Add (FilePath);
			args.Add ("--type");
			args.Add (GetFileTypeValue ());
			args.Add ("--username");
			args.Add (Username);
			args.Add ("--password");
			args.Add (Password);
			args.Add ("--output-format");
			args.Add ("xml");

			return args;
		}

		string GetFileTypeValue ()
		{
			switch (Platform) {
			case ApplePlatform.MacOSX: return "osx";
			case ApplePlatform.TVOS: return "appletvos";
			case ApplePlatform.iOS: return "ios";
			default: throw new NotSupportedException ($"Provided file type '{Platform}' is not supported by altool");
			}
		}

		void LogErrorsFromOutput (string? output)
		{
			try {
#if NET
				if (string.IsNullOrEmpty (output))
					return;
#else
				if (output is null || string.IsNullOrEmpty (output))
					return;
#endif

				var plist = PObject.FromString (output) as PDictionary;
				var errors = PObject.Create (PObjectType.Array) as PArray;
				var message = PObject.Create (PObjectType.String) as PString;

				if ((plist?.TryGetValue ("product-errors", out errors) == true)) {
					foreach (var error in errors) {
						var dict = error as PDictionary;
						if (dict?.TryGetValue ("message", out message) == true) {
							Log.LogError ("altool", null, null, null, 0, 0, 0, 0, "{0}", message.Value);
						}
					}
				}
			} catch (Exception ex) {
				Log.LogWarning (MSBStrings.W0095, ex.Message, output);
			}
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
