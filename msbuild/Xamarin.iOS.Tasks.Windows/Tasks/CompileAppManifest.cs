using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.IO;
using System.Linq;
using Xamarin.iOS.Tasks.Windows.Properties;
using Xamarin.MacDev;

#nullable enable

namespace Xamarin.iOS.HotRestart.Tasks
{
	public class CompileAppManifest : Task
	{
		static readonly string[] IgnorePlistKeys = {
		   "XSAppIconAssets",
		   "CFBundleIconName",
		   "XSLaunchImageAssets",
		   "UIMainStoryboardFile",
		   "UIFileSharingEnabled",
		   "UILaunchStoryboardName",
		   "UIMainStoryboardFile~ipad",
		   "UIMainStoryboardFile~iphone",
		   "CFBundleIdentifier",
		   "CFBundleExecutable"
		};

		#region Inputs

		[Required]
		public string AppBundlePath { get; set; } = String.Empty;

		[Required]
		public string AppManifestPath { get; set; } = String.Empty;

		public string ApplicationTitle { get; set; } = String.Empty;

		#endregion

		public override bool Execute()
		{
			try
			{
				var preBuiltInfoPlistPath = Path.Combine(AppBundlePath, "Info.plist");

				if (!File.Exists(preBuiltInfoPlistPath))
				{
					throw new Exception(string.Format(Resources.CompileAppManifest_MissinInfoPList, preBuiltInfoPlistPath));
				}

				var infoPlist = PDictionary.FromFile(AppManifestPath);
				var preBuiltInfoPlist = PDictionary.FromFile(preBuiltInfoPlistPath);

				foreach (var item in infoPlist)
				{
					if (!IgnorePlistKeys.Contains(item.Key))
					{
						if (preBuiltInfoPlist.ContainsKey(item.Key))
						{
							preBuiltInfoPlist.Remove(item.Key);
						}

						preBuiltInfoPlist.Add(item.Key, item.Value.Clone());
					}
				}

				if (!string.IsNullOrEmpty (ApplicationTitle)) {
					preBuiltInfoPlist[ManifestKeys.CFBundleDisplayName] = ApplicationTitle;
				}

				preBuiltInfoPlist.Save(preBuiltInfoPlistPath, binary: true);

				return true;
			}
			catch (Exception ex)
			{
				Log.LogErrorFromException(ex);

				return false;
			}
		}
	}
}
