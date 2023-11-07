using System.Collections.Generic;
using System.IO;

namespace Xharness.TestImporter.Xamarin {

	/// <summary>
	/// Implemenation of the assembly locator that will return the root path of the mono bcl artifact.
	/// </summary>
	public class AssemblyLocator : IAssemblyLocator {

		#region static vars

		static readonly Dictionary<Platform, string> downloadPartialPath = new Dictionary<Platform, string> {
			{Platform.iOS, "ios-bcl"},
			{Platform.WatchOS, "ios-bcl"},
			{Platform.TvOS, "ios-bcl"},
			{Platform.MacOSFull, "mac-bcl"},
			{Platform.MacOSModern, "mac-bcl"},
		};
		static readonly Dictionary<Platform, string> platformPathMatch = new Dictionary<Platform, string> {
			{Platform.iOS, "monotouch"},
			{Platform.WatchOS, "monotouch_watch"},
			{Platform.TvOS, "monotouch_tv"},
			{Platform.MacOSFull, "xammac_net_4_5"},
			{Platform.MacOSModern, "xammac"},
		};

		#endregion

		public string iOSMonoSDKPath { get; set; }
		public string MacMonoSDKPath { get; set; }

		public string GetAssembliesRootLocation (Platform platform)
		{
			switch (platform) {
			case Platform.iOS:
			case Platform.TvOS:
			case Platform.WatchOS:
				// simply, try to find the dir with the pattern
				return iOSMonoSDKPath;
			case Platform.MacOSFull:
			case Platform.MacOSModern:
				return MacMonoSDKPath;
			default:
				return null;
			}
		}

		public string GetAssembliesLocation (Platform platform)
		{
			var downloadsPath = GetAssembliesRootLocation (platform);

			switch (platform) {
			case Platform.MacOSFull:
			case Platform.MacOSModern:
				return Path.Combine (downloadsPath, "mac-bcl", platformPathMatch [platform], "tests");
			case Platform.iOS:
			case Platform.TvOS:
			case Platform.WatchOS:
				return Path.Combine (downloadsPath, "ios-bcl", platformPathMatch [platform], "tests");
			}

			return null;
		}

		public string GetHintPathForReferenceAssembly (string assembly, Platform platform)
		{
			var hintPath = Path.Combine (GetAssembliesRootLocation (platform), downloadPartialPath [platform], platformPathMatch [platform], $"{assembly}.dll");
			if (File.Exists (hintPath)) {
				return hintPath;
			} else {
				// we could be referencing a dll in the test dir, lets test that
				hintPath = Path.Combine (GetAssembliesRootLocation (platform), downloadPartialPath [platform], platformPathMatch [platform], "tests", $"{assembly}.dll");
			}
			return File.Exists (hintPath) ? hintPath : null;
		}

		public string GetTestingFrameworkDllPath (string assembly, Platform platform)
		{
			var downloadPath = GetAssembliesRootLocation (platform);
			switch (platform) {
			case Platform.iOS:
			case Platform.TvOS:
			case Platform.WatchOS:
				// depends of the assembly name:
				if (assembly == "nunitlite.dll")
					return Path.Combine (downloadPath, "ios-bcl", platformPathMatch [platform], assembly).Replace ('/', '\\');
				else
					return Path.Combine (downloadPath, "ios-bcl", platformPathMatch [platform], "tests", assembly).Replace ('/', '\\');
			case Platform.MacOSFull:
			case Platform.MacOSModern:
				if (assembly == "nunitlite.dll")
					return Path.Combine (downloadPath, "mac-bcl", platformPathMatch [platform], assembly).Replace ('/', '\\');
				else
					return Path.Combine (downloadPath, "mac-bcl", platformPathMatch [platform], "tests", assembly).Replace ('/', '\\');
			}
			return "";
		}

	}
}
