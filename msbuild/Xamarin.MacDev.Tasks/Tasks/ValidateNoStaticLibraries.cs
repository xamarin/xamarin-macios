#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.Localization.MSBuild;
using Xamarin.Messaging.Build.Client;
using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	public class ValidateNoStaticLibraries : Task {
		public string SkipStaticLibraryValidation { get; set; } = string.Empty;
		public ITaskItem [] ValidateItems { get; set; } = Array.Empty<ITaskItem> ();

		public override bool Execute ()
		{
			bool onlyWarn = false;
			switch (SkipStaticLibraryValidation.ToLowerInvariant ()) {
			case "true":
			case "disable":
				return true;
			case "":
			case "error":
			case "false":
				onlyWarn = false;
				break;
			case "warn":
				onlyWarn = true;
				break;
			default:
				Log.LogError (7142, null, MSBStrings.E7142, SkipStaticLibraryValidation); // Unknown value for 'SkipStaticLibraryValidation': {0}. Valid values are: 'true', 'false', 'warn'.
				return false;
			}

			foreach (var item in ValidateItems) {
				var path = item.ItemSpec;
				if (Directory.Exists (path))
					continue; // directories are neither static libraries nor object files.

				if (!File.Exists (path)) {
					if (onlyWarn) {
						Log.LogWarning (158, path, MSBStrings.E0158 /* The file '{0}' does not exist. */, path);
					} else {
						Log.LogError (158, path, MSBStrings.E0158 /* The file '{0}' does not exist. */, path);
					}
					continue;
				}

				if (!MachO.IsStaticLibraryOrObjectFile (path, throw_if_error: false, out var objectFile))
					continue;

				if (objectFile) {
					if (onlyWarn) {
						Log.LogWarning (7143, item.ItemSpec, MSBStrings.E7143, path); // The file {0} is an object file, and an object files are not supported with Hot Restart.
					} else {
						Log.LogError (7143, item.ItemSpec, MSBStrings.E7143, path); // The file {0} is an object file, and an object files are not supported with Hot Restart.
					}
				} else {
					if (onlyWarn) {
						Log.LogWarning (7141, item.ItemSpec, MSBStrings.E7141, path); // The library {0} is a static library, and static libraries are not supported with Hot Restart.
					} else {
						Log.LogError (7141, item.ItemSpec, MSBStrings.E7141, path); // The library {0} is a static library, and static libraries are not supported with Hot Restart.
					}
				}
			}

			return !Log.HasLoggedErrors;
		}
	}
}
