using System;
using System.IO;

using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace Xamarin.MacDev.Tasks
{
	public abstract class ALToolTaskBase : ToolTask
	{
		string sdkDevPath;

		public string SessionId { get; set; }

		[Required]
		public string Username { get ; set; }

		[Required]
		public string Password { get ; set; }

		[Required]
		public string FilePath { get; set; }

		[Required]
		public virtual PlatformName FileType { get ; set; }

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
			Log.LogMessage (messageImportance, "{0}", singleLine);
		}

		string GetFileTypeValue ()
		{
			switch(FileType) {
				case PlatformName.MacOSX: return "osx";
				case PlatformName.TvOS: return "appletvos";
				case PlatformName.iOS: return "ios";
				default: throw new NotSupportedException ("Provided file type is not supported by altool");
			}
		}
	}
}