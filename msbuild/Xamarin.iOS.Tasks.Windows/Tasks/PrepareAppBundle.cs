using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.IO;
using System.IO.Compression;
using Xamarin.iOS.Windows;
using Xamarin.MacDev;

namespace Xamarin.iOS.HotRestart.Tasks {
	public class PrepareAppBundle : Task {
		#region Inputs

		[Required]
		public string AppBundleName { get; set; }

		[Required]
		public string SessionId { get; set; }

		[Required]
		public bool ShouldExtract { get; set; }

		#endregion

		#region Outputs

		[Output]
		public string AppBundlePath { get; set; }

		#endregion

		public override bool Execute ()
		{
			AppBundlePath = HotRestartContext.Default.GetAppBundlePath (AppBundleName, SessionId.Substring (0, 8));

			if (!Directory.Exists (AppBundlePath) && ShouldExtract) {
				var preBuiltAppBundlePath = Path.Combine (
					Path.GetDirectoryName (typeof (PrepareAppBundle).Assembly.Location),
					"Xamarin.PreBuilt.iOS.app.zip");

				ZipFile.ExtractToDirectory (preBuiltAppBundlePath, AppBundlePath);

				// Ensure the archived-expanded-entitlements.xcent is empty. If there are any entitlements in that file, the new signature will be invalid
				new PDictionary ().Save (Path.Combine (AppBundlePath, "archived-expanded-entitlements.xcent"));

				File.WriteAllText (Path.Combine (AppBundlePath, "Extracted"), string.Empty);
			}

			return true;
		}
	}
}
