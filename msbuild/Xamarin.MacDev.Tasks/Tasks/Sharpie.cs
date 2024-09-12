using System;
using System.IO;
using System.Runtime.InteropServices;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Xamarin.MacDev.Tasks
{
	public class Sharpie : XamarinToolTaskWithOutput
	{
		public override string TaskPrefix => "SHRP";

		protected override string ToolName => "sharpie";

		public string Command { get; set; } = string.Empty;

		public string Arguments { get; set; } = string.Empty;

		public string Namespace { get; set; } = string.Empty;

		public string Sdk { get; set; } = string.Empty;

		public string Scope { get; set; } = string.Empty;

		public string OutputPath { get; set; } = string.Empty;

		public ITaskItem[] Headers { get; set; } = Array.Empty<ITaskItem> ();


		const string ClassicXIAssembly = "/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/64bits/iOS/Xamarin.iOS.dll";

		protected override string GenerateFullPathToTool ()
		{
			return Path.Combine ("/usr", "local", "bin", ToolExe);
		}

		protected override string GenerateCommandLineCommands ()
		{
			var cmd = new CommandLineBuilder ();

			if (!string.IsNullOrEmpty (Command))
				cmd.AppendSwitch (Command);

			if (!string.IsNullOrEmpty (Namespace))
				cmd.AppendSwitchIfNotNull ("--namespace ", Namespace);

			if (!string.IsNullOrEmpty (Sdk))
				cmd.AppendSwitchIfNotNull ("--sdk ", Sdk);

			if (!string.IsNullOrEmpty (Scope))
				cmd.AppendSwitchIfNotNull ("--scope ", Scope);

			if (!string.IsNullOrEmpty (OutputPath))
				cmd.AppendSwitchIfNotNull ("--output ", OutputPath);

			cmd.AppendSwitchIfNotNull (string.Empty, Headers, delimiter: " ");

			if (!string.IsNullOrEmpty (Arguments))
				cmd.AppendSwitch (Arguments);

			return cmd.ToString ();
		}

		public override bool RunTask ()
		{
			if (RuntimeInformation.IsOSPlatform (OSPlatform.OSX))
			{
				if (!File.Exists (GenerateFullPathToTool ())) {
					// TODO loc
					Log.LogError ($"{TaskPrefix}1000 {{0}}", $"Unable to find the executable \"{ToolName}\". Please install Objective-Sharpie: https://aka.ms/objective-sharpie.");
					return false;
				}

				if (!File.Exists (ClassicXIAssembly)) {
					Log.LogError ($"{TaskPrefix}1001 {{0}}", $"The \"{ToolName}\" tool requires a classic Xamarin.iOS installation. Please install Xamarin.iOS: https://github.com/xamarin/xamarin-macios/blob/main/DOWNLOADS.md");
					return false;
				}

				return base.RunTask ();
			}
			else
			{
				Log.LogWarning ($"{TaskPrefix}5000 {{0}}", $"Skipping attempt to run \"{ToolName}\" with arguments \"{GenerateCommandLineCommands ()}\". The \"@(MaciOSXcodeProject)\" build action is only supported on macOS.");
				return true;
			}
		}

	}
}
