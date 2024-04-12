using System;
using System.IO;

using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using System.Text;

using Xamarin.Utils;
using Xamarin.Localization.MSBuild;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Xamarin.MacDev.Tasks {
	public abstract class ALToolTaskBase : XamarinToolTask {
		string sdkDevPath;
		StringBuilder toolOutput;

		[Required]
		public string Username { get; set; }

		[Required]
		public string Password { get; set; }

		[Required]
		public string FilePath { get; set; }

		protected override string ToolName {
			get { return "altool"; }
		}

		[Required]
		public string SdkDevPath {
			get { return sdkDevPath; }
			set {
				sdkDevPath = value;
			}
		}

		string DevicePlatformBinDir {
			get { return Path.Combine (SdkDevPath, "usr", "bin"); }
		}

		protected abstract string ALToolAction { get; }

		public override bool Execute ()
		{
			toolOutput = new StringBuilder ();

			base.Execute ();

			LogErrorsFromOutput (toolOutput.ToString ());

			return !HasLoggedErrors;
		}

		protected override string GenerateFullPathToTool ()
		{
			if (!string.IsNullOrEmpty (ToolPath))
				return Path.Combine (ToolPath, ToolExe);

			var path = Path.Combine (DevicePlatformBinDir, ToolExe);

			return File.Exists (path) ? path : ToolExe;
		}

		protected override string GenerateCommandLineCommands ()
		{
			var args = new CommandLineArgumentBuilder ();

			args.Add (ALToolAction);
			args.Add ("--file");
			args.AddQuoted (FilePath);
			args.Add ("--type");
			args.AddQuoted (GetFileTypeValue ());
			args.Add ("--username");
			args.AddQuoted (Username);
			args.Add ("--password");
			args.AddQuoted (Password);
			args.Add ("--output-format");
			args.Add ("xml");

			return args.ToString ();
		}

		protected override void LogEventsFromTextOutput (string singleLine, MessageImportance messageImportance)
		{
			toolOutput.Append (singleLine);
			Log.LogMessage (messageImportance, "{0}", singleLine);
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

		void LogErrorsFromOutput (string output)
		{
			try {
				if (string.IsNullOrEmpty (output))
					return;

				var plist = PObject.FromString (output) as PDictionary;
				var errors = PObject.Create (PObjectType.Array) as PArray;
				var message = PObject.Create (PObjectType.String) as PString;

				if ((plist?.TryGetValue ("product-errors", out errors) == true)) {
					foreach (var error in errors) {
						var dict = error as PDictionary;
						if (dict?.TryGetValue ("message", out message) == true) {
							Log.LogError (ToolName, null, null, null, 0, 0, 0, 0, "{0}", message.Value);
						}
					}
				}
			} catch (Exception ex) {
				Log.LogWarning (MSBStrings.W0095, ex.Message, output);
			}
		}
	}
}
