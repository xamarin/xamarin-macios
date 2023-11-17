using System;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev;
using Xamarin.Localization.MSBuild;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Xamarin.MacDev.Tasks {
	public abstract class GetNativeExecutableNameTaskBase : XamarinTask {
		#region Inputs

		[Required]
		public string AppManifest { get; set; }

		#endregion

		#region Outputs

		[Output]
		public string ExecutableName { get; set; }

		#endregion

		public override bool Execute ()
		{
			PDictionary plist;

			try {
				plist = PDictionary.FromFile (AppManifest);
			} catch (Exception ex) {
				Log.LogError (MSBStrings.E0055, ex.Message);
				return false;
			}

			ExecutableName = plist.GetCFBundleExecutable ();

			return !Log.HasLoggedErrors;
		}
	}
}
