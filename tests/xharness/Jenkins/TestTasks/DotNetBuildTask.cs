using System.Collections.Generic;

namespace Xharness.Jenkins.TestTasks {
	class DotNetBuildTask : MSBuildTask {
		protected override string ToolName {
			get { return "dotnet"; }
		}

		protected override List<string> ToolArguments {
			get {
				var args = base.ToolArguments;
				// 'dotnet build' takes almost the same arguments as 'msbuild', so just massage a little bit.
				args.Remove ("--");
				args.Insert (0, "build");
				return args;
			}
		}
	}
}
