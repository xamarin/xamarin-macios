using System.IO;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Xamarin.MacDev.Tasks
{
	public abstract class CodesignTaskBase : ToolTask
	{
		#region Inputs

		public string SessionId { get; set; }

		[Required]
		public string CodesignAllocate { get; set; }

		public bool DisableTimestamp { get; set; }

		public string Entitlements { get; set; }

		public string Keychain { get; set; }

		[Required]
		public string Resource { get; set; }

		public string ResourceRules { get; set; }

		[Required]
		public string SigningKey { get; set; }

		public string ExtraArgs { get; set; }

		public bool IsAppExtension { get; set; }

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
			var args = new ProcessArgumentBuilder ();

			args.Add ("-v");
			args.Add ("--force");

			if (IsAppExtension)
				args.Add ("--deep");
			
			args.Add ("--sign");
			args.AddQuoted (SigningKey);

			if (!string.IsNullOrEmpty (Keychain)) {
				args.Add ("--keychain");
				args.AddQuoted (Path.GetFullPath (Keychain));
			}

			if (!string.IsNullOrEmpty (ResourceRules)) {
				args.Add ("--resource-rules");
				args.AddQuoted (Path.GetFullPath (ResourceRules));
			}

			if (!string.IsNullOrEmpty (Entitlements)) {
				args.Add ("--entitlements");
				args.AddQuoted (Path.GetFullPath (Entitlements));
			}

			if (DisableTimestamp)
				args.Add ("--timestamp=none");

			if (!string.IsNullOrEmpty (ExtraArgs))
				args.Add (ExtraArgs);

			args.AddQuoted (Path.GetFullPath (Resource));

			return args.ToString ();
		}

		protected override void LogEventsFromTextOutput (string singleLine, MessageImportance messageImportance)
		{
			// TODO: do proper parsing of error messages and such
			Log.LogMessage (messageImportance, "{0}", singleLine);
		}

		public override bool Execute ()
		{
			Log.LogTaskName ("Codesign");
			Log.LogTaskProperty ("CodesignAllocate", CodesignAllocate);
			Log.LogTaskProperty ("DisableTimestamp", DisableTimestamp);
			Log.LogTaskProperty ("Entitlements", Entitlements);
			Log.LogTaskProperty ("Keychain", Keychain);
			Log.LogTaskProperty ("Resource", Resource);
			Log.LogTaskProperty ("ResourceRules", ResourceRules);
			Log.LogTaskProperty ("SigningKey", SigningKey);
			Log.LogTaskProperty ("ExtraArgs", ExtraArgs);
			Log.LogTaskProperty ("IsAppExtension", IsAppExtension);

			EnvironmentVariables = new string[] {
				"CODESIGN_ALLOCATE=" + CodesignAllocate
			};

			return base.Execute ();
		}
	}
}
