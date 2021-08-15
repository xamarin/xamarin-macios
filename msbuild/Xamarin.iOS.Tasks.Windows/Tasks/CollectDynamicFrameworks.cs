using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xamarin.iOS.Tasks.Windows.Properties;
using Xamarin.iOS.Windows;

namespace Xamarin.iOS.HotRestart.Tasks {
	public class CollectDynamicFrameworks : Task {
		#region Inputs

		[Required]
		public ITaskItem [] Frameworks { get; set; }

		#endregion

		#region Outputs

		[Output]
		public ITaskItem [] DynamicFrameworks { get; set; }

		#endregion

		public override bool Execute ()
		{
			var frameworks = new List<ITaskItem> ();
			var hotRestartClient = new HotRestartClient ();

			foreach (var framework in Frameworks.Where (f => Path.GetExtension (f.ItemSpec.TrimEnd ('\\')) == ".framework")) {
				framework.ItemSpec = framework.ItemSpec.TrimEnd ('\\');

				if (frameworks.Any (x => x.ItemSpec == framework.ItemSpec)) {
					continue;
				}

				var frameworkDirName = Path.GetFileName (framework.ItemSpec);

				try {
					var frameworkPath = Path.Combine (framework.ItemSpec, Path.GetFileNameWithoutExtension (frameworkDirName));

					hotRestartClient.LoadDynamicFramework (frameworkPath);
				} catch (AppleInvalidFrameworkException frameworkEx) {
					Log.LogMessage (MessageImportance.Normal, Resources.CollectDynamicFrameworks_InvalidFramework, Path.GetFileName (framework.ItemSpec), frameworkEx.Message);
					continue;
				} catch (Exception ex) {
					Log.LogErrorFromException (ex);
					break;
				}

				framework.SetMetadata ("FrameworkDir", $@"{frameworkDirName}\");

				frameworks.Add (framework);
			}

			DynamicFrameworks = frameworks.ToArray ();

			return !Log.HasLoggedErrors;
		}
	}
}
