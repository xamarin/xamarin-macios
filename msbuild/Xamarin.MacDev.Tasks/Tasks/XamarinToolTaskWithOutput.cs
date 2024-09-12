using System;
using System.IO;
using System.Text;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Xamarin.MacDev.Tasks
{
	public abstract class XamarinToolTaskWithOutput : XamarinToolTask
	{
		public abstract string TaskPrefix { get; }

		public string WorkingDirectory { get; set; } = Directory.GetCurrentDirectory ();

		StringBuilder toolOutput = new StringBuilder ();

		[Output]
		public string ConsoleOutput { get; set; } = string.Empty;

		public override bool Execute ()
		{
			try
			{
				bool taskResult = RunTask ();
				if (!taskResult && !string.IsNullOrEmpty (toolOutput.ToString ()))
				{
					Log.LogError ($"{TaskPrefix}0000 {{0}}", toolOutput.ToString ().Trim ());
				}
				ConsoleOutput = toolOutput.ToString ();
				toolOutput.Clear ();
				return taskResult;
			}
			catch (Exception ex)
			{
				Log.LogError ($"{TaskPrefix}0100 {{0}}", ex.ToString ());
				return false;
			}
		}

		protected override void LogEventsFromTextOutput (string singleLine, MessageImportance messageImportance)
		{
			base.LogEventsFromTextOutput (singleLine, messageImportance);
			toolOutput.AppendLine (singleLine);
		}

		protected override string GetWorkingDirectory ()
		{
			return WorkingDirectory;
		}

		public virtual bool RunTask () => base.Execute ();

	}
}
