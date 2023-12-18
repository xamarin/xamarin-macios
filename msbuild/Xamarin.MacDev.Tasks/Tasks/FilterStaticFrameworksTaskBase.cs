#nullable enable

using System;
using System.IO;
using System.Linq;

using Microsoft.Build.Framework;

using Xamarin.Localization.MSBuild;
using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	// This task takes an itemgroup of frameworks, and filters out frameworks that aren't dynamic libraries.
	public abstract class FilterStaticFrameworksTaskBase : XamarinTask {
		public bool OnlyFilterFrameworks { get; set; }

		[Output]
		public ITaskItem []? FrameworkToPublish { get; set; }

		public override bool Execute ()
		{
			if (FrameworkToPublish is not null && FrameworkToPublish.Length > 0) {
				var list = FrameworkToPublish.ToList ();
				for (var i = list.Count - 1; i >= 0; i--) {
					var item = list [i];
					var frameworkExecutablePath = PathUtils.ConvertToMacPath (item.ItemSpec);
					try {
						if (frameworkExecutablePath.EndsWith (".framework", StringComparison.OrdinalIgnoreCase) && Directory.Exists (frameworkExecutablePath)) {
							frameworkExecutablePath = Path.Combine (frameworkExecutablePath, Path.GetFileNameWithoutExtension (frameworkExecutablePath));
						}

						if (OnlyFilterFrameworks && !Path.GetDirectoryName (frameworkExecutablePath).EndsWith (".framework", StringComparison.OrdinalIgnoreCase)) {
							Log.LogMessage (MessageImportance.Low, $"Skipped processing {item.ItemSpec} because it's not a framework");
							continue;
						}

						if (!File.Exists (frameworkExecutablePath)) {
							Log.LogError (158, frameworkExecutablePath, MSBStrings.E0158 /* The file '{0}' does not exist. */, frameworkExecutablePath);
							continue;
						}

						if (MachO.IsDynamicFramework (frameworkExecutablePath))
							continue;
					} catch (Exception e) {
						Log.LogError (7091, frameworkExecutablePath, MSBStrings.E7092 /* File '{0}' is not a valid framework: {1} */, frameworkExecutablePath, e.Message);
						continue;
					}

					Log.LogWarning (7091, frameworkExecutablePath, MSBStrings.W7091 /* "The framework {0} is a framework of static libraries, and will not be copied to the app." */, Path.GetDirectoryName (frameworkExecutablePath));
					list.RemoveAt (i);
				}

				// Copy back the list if anything was removed from it
				if (FrameworkToPublish.Length != list.Count)
					FrameworkToPublish = list.ToArray ();
			}

			return !Log.HasLoggedErrors;
		}
	}
}
