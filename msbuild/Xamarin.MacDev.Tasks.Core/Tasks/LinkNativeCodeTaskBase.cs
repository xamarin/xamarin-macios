using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.Build.Framework;

namespace Xamarin.MacDev.Tasks {
	public abstract class LinkNativeCodeTaskBase : XamarinTask {

#region Inputs
		public ITaskItem[] LinkWithLibraries { get; set; }

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

			bool hasDylibs = false;
			if (LinkWithLibraries != null) {
				foreach (var libSpec in LinkWithLibraries) {
					var lib = Path.GetFullPath (libSpec.ItemSpec);
					var libExtension = Path.GetExtension (lib).ToLowerInvariant ();
					switch (libExtension) {
					case ".a":
						arguments.Add (lib);
						break;
					case ".dylib":
						arguments.Add ("-L" + Path.GetDirectoryName (lib));
						var libName = Path.GetFileNameWithoutExtension (lib);
						if (libName.StartsWith ("lib", StringComparison.Ordinal))
							libName = libName.Substring (3);
						arguments.Add ("-l" + libName);
						hasDylibs = true;
						break;
					case ".framework":
						arguments.Add ("-F" + Path.GetDirectoryName (lib));
						arguments.Add ("-framework");
						arguments.Add (Path.GetFileNameWithoutExtension (lib));
						break;
					default:
						Log.LogError ($"Unknown library extension {libExtension} to link with for {lib}.");
						return false;
					}
				}
			}

			if (hasDylibs) {
				arguments.Add ("-rpath");
				arguments.Add ("@executable_path");
			}

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

