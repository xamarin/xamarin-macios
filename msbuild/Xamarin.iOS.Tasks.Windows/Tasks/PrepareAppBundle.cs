using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using System.IO;
using System.IO.Compression;

using Xamarin.iOS.Windows;
using Xamarin.MacDev;

#nullable enable

namespace Xamarin.iOS.HotRestart.Tasks {
	public class PrepareAppBundle : Task {
		#region Inputs

		[Required]
		public string AppBundleName { get; set; } = string.Empty;

		[Required]
		public string SessionId { get; set; } = string.Empty;

		[Required]
		public bool ShouldExtract { get; set; }

		public string PreBuiltAppBundlePath { get; set; } = string.Empty;

		#endregion

		#region Outputs

		[Output]
		public string AppBundlePath { get; set; } = string.Empty;

		#endregion

		public override bool Execute ()
		{
			if (string.IsNullOrEmpty (AppBundlePath))
				AppBundlePath = HotRestartContext.Default.GetAppBundlePath (AppBundleName, SessionId.Substring (0, 8));

			if (ShouldExtract) {
				if (Directory.Exists (AppBundlePath))
					Directory.Delete (AppBundlePath, true);

				Log.LogMessage (MessageImportance.Low, $"Extracting '{PreBuiltAppBundlePath}' into {AppBundlePath}.");
				ZipFile.ExtractToDirectory (PreBuiltAppBundlePath, AppBundlePath);

				// Ensure the archived-expanded-entitlements.xcent is empty. If there are any entitlements in that file, the new signature will be invalid
				new PDictionary ().Save (Path.Combine (AppBundlePath, "archived-expanded-entitlements.xcent"));
			}

			return true;
		}
	}
}
