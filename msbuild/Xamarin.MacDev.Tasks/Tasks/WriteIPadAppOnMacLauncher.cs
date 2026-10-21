// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO;
using System.Text;

using Microsoft.Build.Framework;

namespace Xamarin.MacDev.Tasks {
	public class WriteIPadAppOnMacLauncher : XamarinTask {
		[Required]
		public string OutputFile { get; set; } = "";

		[Required]
		public ITaskItem [] Arguments { get; set; } = [];

		public override bool Execute ()
		{
			// iOS runtimes only pass app-arg options through to managed Main.
			var script = new StringBuilder ("for arg do\nshift\nset -- \"$@\" \"--app-arg=$arg\"\ndone\nexec /usr/bin/open");
			foreach (var argument in Arguments) {
				script.Append (" '");
				script.Append (argument.GetMetadata ("Argument").Replace ("'", "'\\''"));
				script.Append ("'");
			}
			script.Append (" --args \"$@\"\n");
			File.WriteAllText (OutputFile, script.ToString ());
			return true;
		}
	}
}
