using System.IO;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev.Tasks;

namespace Xamarin.iOS.Tasks
{
	public class CollectFrameworksBase : Task
	{
		#region Inputs

		public string SessionId { get; set; }

		[Required]
		public string AppBundlePath { get; set; }

		#endregion

		#region Outputs

		[Output]
		public ITaskItem[] Frameworks { get; set; }

		#endregion

		public override bool Execute ()
		{
			var frameworks = new List<ITaskItem> ();

			Log.LogTaskName ("CollectFrameworks");
			Log.LogTaskProperty ("AppBundlePath", AppBundlePath);

			var fwDir = Path.Combine (AppBundlePath, "Frameworks");
			if (Directory.Exists (fwDir)) {
				foreach (var fw in Directory.GetDirectories (fwDir)) {
					if (Path.GetExtension (fw) != ".framework") {
						Log.LogWarning ("Found a directory within the Frameworks directory which is not a framework: {0}", fw);
						continue;
					}
					var fwName = Path.GetFileName (fw);
					var stem = fwName.Substring (0, fwName.Length - ".framework".Length);
					var fwBinary = Path.Combine (fw, stem);
					if (!File.Exists (fwBinary)) {
						Log.LogWarning ("The framework {0} does not contain a binary named {1}", Path.GetDirectoryName (fwBinary), Path.GetFileName (fwBinary));
						continue;
					}
					
					var framework = new TaskItem (fwBinary);
					frameworks.Add (framework);
				}
			} else {
				Log.LogMessage ("No Frameworks directory found.");
			}

			Frameworks = frameworks.ToArray ();

			return !Log.HasLoggedErrors;
		}
	}
}
