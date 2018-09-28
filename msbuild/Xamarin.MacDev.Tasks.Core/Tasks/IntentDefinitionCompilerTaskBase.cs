using System.IO;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Xamarin.MacDev.Tasks
{
	public abstract class IntentDefinitionCompilerTaskBase : ToolTask
	{
		#region Inputs

		public string SessionId { get; set; }

		[Required]
		public ITaskItem IntentDefinition { get; set; }

		[Required]
		public string IntermediateOutputPath { get; set; }

		[Required]
		public string ProjectDir { get; set; }

		[Required]
		public string ResourcePrefix { get; set; }

		[Required]
		public string SdkBinPath { get; set; }

		#endregion

		#region Outputs

		[Output]
		public ITaskItem BundleResource { get; set; }

		#endregion

		protected override string ToolName {
			get { return "intentbuilderc"; }
		}

		protected override string GenerateFullPathToTool ()
		{
			if (!string.IsNullOrEmpty (ToolPath))
				return Path.Combine (ToolPath, ToolExe);

			var path = Path.Combine (SdkBinPath, ToolExe);

			return File.Exists (path) ? path : ToolExe;
		}

		protected override string GenerateCommandLineCommands ()
		{
			var fileName = Path.GetFileName (IntentDefinition.ItemSpec);
			var args = new CommandLineArgumentBuilder ();

			args.AddQuoted (IntentDefinition.ItemSpec);
			args.AddQuoted (IntermediateOutputPath);
			args.AddQuoted (string.Empty);

			BundleResource = new TaskItem (Path.Combine (IntermediateOutputPath, fileName));
			BundleResource.SetMetadata ("LogicalName", fileName);

			return args.ToString ();
		}

		protected override void LogEventsFromTextOutput (string singleLine, MessageImportance messageImportance)
		{
			// TODO: do proper parsing of error messages and such
			Log.LogMessage (messageImportance, "{0}", singleLine);
		}

		public override bool Execute ()
		{
			var dir = Path.Combine (IntermediateOutputPath, ToolName);

			if (!Directory.Exists (dir))
				Directory.CreateDirectory (dir);

			return base.Execute ();
		}
	}
}
