using System;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev;

namespace Xamarin.MacDev.Tasks
{
	public abstract class GetNativeExecutableNameTaskBase : Task
	{
		#region Inputs

		public string SessionId { get; set; }

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

			Log.LogTaskName ("GetNativeExecutableName");
			Log.LogTaskProperty ("AppManifest", AppManifest);

			try {
				plist = PDictionary.FromFile (AppManifest);
			} catch (Exception ex) {
				Log.LogError ("Could not load Info.plist: {0}", ex.Message);
				return false;
			}

			ExecutableName = plist.GetCFBundleExecutable ();

			return !Log.HasLoggedErrors;
		}
	}
}
