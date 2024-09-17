using System;
using System.Collections.Generic;

using Microsoft.Build.Framework;

namespace Xamarin.MacDev.Tasks {
	public class CreateXcFramework : XcodeBuildTask {

		// Task input parameters
		public ITaskItem [] FrameworkArchives { get; set; } = Array.Empty<ITaskItem> ();

		public string FrameworkName { get; set; } = string.Empty;


		protected override string Command { get; set; } = "-create-xcframework";

		protected override IList<string> GenerateCommandLineCommands ()
		{
			var args = new List<string> ();

			foreach (var frameworkArchive in FrameworkArchives) {
				args.Add ("-archive");
				args.Add (frameworkArchive.ItemSpec);
				args.Add ("-framework");
				args.Add (FrameworkName);
			}

			if (!string.IsNullOrEmpty (OutputPath)) {
				args.Add ("-output");
				args.Add (OutputPath);
			}

			return args;
		}

	}
}
