using System;
using System.IO;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev.Tasks;
using Xamarin.MacDev;

namespace Xamarin.iOS.Tasks
{
	public abstract class FindWatchOS1AppExtensionBundleTaskBase : XamarinTask
	{
		#region Inputs

		[Required]
		public ITaskItem[] AppExtensionReferences { get; set; }

		#endregion Inputs

		#region Outputs

		[Output]
		public string WatchOS1AppExtensionBundle { get; set; }

		#endregion

		public override bool Execute ()
		{
			var pwd = PathUtils.ResolveSymbolicLinks (Environment.CurrentDirectory);

			for (int i = 0; i < AppExtensionReferences.Length; i++) {
				var plist = PDictionary.FromFile (Path.Combine (AppExtensionReferences[i].ItemSpec, "Info.plist"));
				PString expectedBundleIdentifier, bundleIdentifier, extensionPoint;
				PDictionary extension, attributes;

				if (!plist.TryGetValue ("NSExtension", out extension))
					continue;

				if (!extension.TryGetValue ("NSExtensionPointIdentifier", out extensionPoint))
					continue;

				if (extensionPoint.Value != "com.apple.watchkit")
					continue;

				// Okay, we've found the WatchKit App Extension...
				if (!extension.TryGetValue ("NSExtensionAttributes", out attributes))
					continue;

				if (!attributes.TryGetValue ("WKAppBundleIdentifier", out expectedBundleIdentifier))
					continue;

				// Scan the *.app subdirectories to find the WatchApp bundle...
				foreach (var bundle in Directory.GetDirectories (AppExtensionReferences[i].ItemSpec, "*.app")) {
					if (!File.Exists (Path.Combine (bundle, "Info.plist")))
						continue;

					plist = PDictionary.FromFile (Path.Combine (bundle, "Info.plist"));

					if (!plist.TryGetValue ("CFBundleIdentifier", out bundleIdentifier))
						continue;

					if (bundleIdentifier.Value != expectedBundleIdentifier.Value)
						continue;
					
					WatchOS1AppExtensionBundle = PathUtils.AbsoluteToRelative (pwd, PathUtils.ResolveSymbolicLinks (bundle));

					return true;
				}
			}

			return !Log.HasLoggedErrors;
		}
	}
}
