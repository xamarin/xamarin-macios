using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.IO;
using System.IO.Compression;

namespace Xamarin.iOS.HotRestart.Tasks
{
	public class PrepareAppBundle : Task
	{
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

		//readonly string BundlesPath = Path.Combine(Path.GetTempPath(), "Xamarin", "HotRestart", "Bundles");

		public override bool Execute()
		{
			// TODO: get paths from the Hot Restart library
			var BundlesPath = string.Empty;
			Directory.CreateDirectory(BundlesPath);

			// TODO: get paths from the Hot Restart library
			AppBundlePath = string.Empty; //Path.Combine(BundlesPath, ThisAssembly.Version, SessionId.Substring(0, 8), $"{AppBundleName}.app");
			if (!Directory.Exists(AppBundlePath) && ShouldExtract)
			{
				var preBuiltAppBundlePath = Path.Combine(
					Path.GetDirectoryName(typeof(PrepareHotRestartAppBundle).Assembly.Location),
					"Xamarin.PreBuilt.iOS.app.zip");

				ZipFile.ExtractToDirectory(preBuiltAppBundlePath, AppBundlePath);

				File.WriteAllText(Path.Combine(AppBundlePath, "Extracted"), string.Empty);
			}

			return true;
		}
	}
}
