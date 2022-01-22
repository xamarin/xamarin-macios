using System.IO;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.Localization.MSBuild;

namespace Xamarin.MacDev.Tasks
{
	public abstract class CollectFrameworksBase : XamarinTask
	{
		#region Inputs

		[Required]
		public string FrameworksDirectory { get; set; }

		#endregion

		#region Outputs

		[Output]
		public ITaskItem[] Frameworks { get; set; }

		#endregion

		public override bool Execute ()
		{
			var frameworks = new List<ITaskItem> ();

			if (Directory.Exists (FrameworksDirectory)) {
				foreach (var fw in Directory.GetDirectories (FrameworksDirectory)) {
					if (Path.GetExtension (fw) != ".framework") {
						Log.LogWarning (MSBStrings.W0103, fw);
						continue;
					}
					var stem = Path.GetFileNameWithoutExtension (fw);
					var fwBinary = Path.Combine (fw, stem);
					if (!File.Exists (fwBinary)) {
						Log.LogWarning (MSBStrings.W0104, fw, stem);
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
