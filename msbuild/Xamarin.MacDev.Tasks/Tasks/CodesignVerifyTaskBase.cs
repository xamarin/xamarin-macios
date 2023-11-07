using System;
using System.IO;

using Microsoft.Build.Framework;

using Xamarin.Localization.MSBuild;
using Xamarin.Messaging.Build.Client;
using Xamarin.Utils;

#nullable enable

namespace Xamarin.MacDev.Tasks {
	public class CodesignVerify : XamarinToolTask {
		#region Inputs

		[Required]
		public string CodesignAllocate { get; set; } = string.Empty;

		[Required]
		public string Resource { get; set; } = string.Empty;

		#endregion

		protected override string ToolName {
			get { return "codesign"; }
		}

		protected override string GenerateFullPathToTool ()
		{
			if (!string.IsNullOrEmpty (ToolPath))
				return Path.Combine (ToolPath, ToolExe);

			var path = Path.Combine ("/usr/bin", ToolExe);

			return File.Exists (path) ? path : ToolExe;
		}

		protected override string GenerateCommandLineCommands ()
		{
			var args = new CommandLineArgumentBuilder ();

			args.Add ("--verify");
			args.Add ("-vvvv");

			switch (Platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.TVOS:
			case ApplePlatform.WatchOS:
				args.AddQuoted ("-R=anchor apple generic and certificate 1[field.1.2.840.113635.100.6.2.1] exists and (certificate leaf[field.1.2.840.113635.100.6.1.2] exists or certificate leaf[field.1.2.840.113635.100.6.1.4] exists)");
				break;
			case ApplePlatform.MacCatalyst:
			case ApplePlatform.MacOSX:
				args.Add ("--deep");
				break;
			default:
				throw new InvalidOperationException (string.Format (MSBStrings.InvalidPlatform, Platform));
			}

			args.AddQuoted (Resource);

			return args.ToString ();
		}

		protected override void LogEventsFromTextOutput (string singleLine, MessageImportance messageImportance)
		{
			// TODO: do proper parsing of error messages and such
			Log.LogMessage (messageImportance, "{0}", singleLine);
		}

		public override bool Execute ()
		{
			if (ShouldExecuteRemotely ())
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			EnvironmentVariables = new string [] {
				"CODESIGN_ALLOCATE=" + CodesignAllocate
			};

			return base.Execute ();
		}

		public override void Cancel ()
		{
			if (ShouldExecuteRemotely ())
				BuildConnection.CancelAsync (BuildEngine4).Wait ();

			base.Cancel ();
		}
	}
}
