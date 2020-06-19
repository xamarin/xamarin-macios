using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.Build.Framework;

namespace Xamarin.MacDev.Tasks {
	public abstract class LinkNativeCodeTaskBase : XamarinTask {

#region Inputs
		[Required]
		public string SdkDevPath { get; set; }

		[Required]
		public bool SdkIsSimulator { get; set; }

		[Required]
		public string SdkRoot { get; set; }

		[Required]
		public string OutputFile { get; set; }

		[Required]
		public ITaskItem [] ObjectFiles { get; set; }

		[Required]
		public string MinimumOSVersion { get; set; }

		public ITaskItem[] Frameworks { get; set; }
		public ITaskItem [] WeakFrameworks { get; set; }
#endregion

		public override bool Execute ()
		{
			var arguments = new List<string> ();
			arguments.Add ("clang");

			arguments.Add (PlatformFrameworkHelper.GetMinimumVersionArgument (TargetFrameworkMoniker, SdkIsSimulator, MinimumOSVersion));

			arguments.Add ("-isysroot");
			arguments.Add (SdkRoot);

			if (Frameworks != null) {
				foreach (var fw in Frameworks) {
					arguments.Add ("-framework");
					arguments.Add (fw.ItemSpec);
				}
			}

			if (WeakFrameworks != null) {
				foreach (var fw in WeakFrameworks) {
					arguments.Add ("-weak_framework");
					arguments.Add (fw.ItemSpec);
				}
			}

			if (ObjectFiles != null)
				foreach (var obj in ObjectFiles)
					arguments.Add (Path.GetFullPath (obj.ItemSpec));

			arguments.Add ("-o");
			arguments.Add (OutputFile);

			ExecuteAsync ("xcrun", arguments, sdkDevPath: SdkDevPath).Wait ();

			return !Log.HasLoggedErrors;
		}
	}
}

