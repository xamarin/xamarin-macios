using System.IO;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev.Tasks;
using Xamarin.Localization.MSBuild;

namespace Xamarin.iOS.Tasks
{
	public abstract class CollectFrameworksBase : XamarinTask
	{
		#region Inputs

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

			var fwDir = Path.Combine (AppBundlePath, "Frameworks");
			if (Directory.Exists (fwDir)) {
				foreach (var fw in Directory.GetDirectories (fwDir)) {
					if (Path.GetExtension (fw) != ".framework") {
						Log.LogWarning (MSBStrings.W0103, fw);
						continue;
					}
					var fwName = Path.GetFileName (fw);
					var stem = fwName.Substring (0, fwName.Length - ".framework".Length);
					var fwBinary = Path.Combine (fw, stem);
					if (!File.Exists (fwBinary)) {
						Log.LogWarning (MSBStrings.W0104, Path.GetDirectoryName (fwBinary), Path.GetFileName (fwBinary));
						continue;
					}
					
					var framework = new TaskItem (fwBinary);
					frameworks.Add (framework);
				}
			} else {
				Log.LogMessage (MSBStrings.M0105);
			}

			Frameworks = frameworks.ToArray ();

			return !Log.HasLoggedErrors;
		}
	}
}
