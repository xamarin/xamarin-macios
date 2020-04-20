using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Xamarin.Localization.MSBuild;

using Xamarin.MacDev;
using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	public abstract class XcodeToolTask : XamarinToolTask {
		#region Inputs
		[Required]
		public string SdkDevPath { get; set; }

		[Required]
		public bool SdkIsSimulator { get; set; }

		[Required]
		public string SdkRoot { get; set; }

		[Required]
		public string SdkVersion { get; set; }
		#endregion

		protected sealed override string GenerateFullPathToTool ()
		{
			return ToolName;
		}

		protected sealed override string ToolName {
			get { return "xcrun"; }
		}

		protected abstract string XcodeToolName { get; }
		protected abstract void GetArguments (IList<string> arguments);

		public override bool Execute ()
		{
			var env = new List<string> ();
			if (EnvironmentVariables != null)
				env.AddRange (EnvironmentVariables);
			env.Add ($"DEVELOPER_DIR={SdkDevPath}");
			EnvironmentVariables = env.ToArray ();

			return base.Execute ();
		}

		protected sealed override string GenerateCommandLineCommands ()
		{
			var list = new List<string> ();
			list.Add (XcodeToolName);
			GetArguments (list);
			return StringUtils.FormatArguments (list);
		}
	}
}
