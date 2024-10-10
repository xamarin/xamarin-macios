using System;
using System.Collections.Generic;

namespace Xamarin.MacDev.Tasks {
	public abstract class XcodeBuildTask : XamarinTask {

		// Task input parameters
		public string SdkDevPath { get; set; } = string.Empty;

		public string WorkingDirectory { get; set; } = string.Empty;

		public string OutputPath { get; set; } = string.Empty;


		protected virtual string Command { get; set; } = string.Empty;

		protected virtual IList<string> GenerateCommandLineCommands () => new List<string> ();

		public override bool Execute ()
		{
			var args = new List<string> ();
			args.Add ("xcodebuild");
			if (!string.IsNullOrEmpty (Command)) {
				args.Add (Command);
			}
			args.AddRange (GenerateCommandLineCommands ());

			ExecuteAsync ("xcrun", args, sdkDevPath: SdkDevPath, mergeOutput: false, workingDirectory: WorkingDirectory).Wait ();
			return !Log.HasLoggedErrors;
		}

	}
}
