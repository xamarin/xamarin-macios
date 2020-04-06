namespace Microsoft.DotNet.XHarness.iOS.Shared {
	public class AppBundleInformation {
		public string AppName { get; }
		public string BundleIdentifier { get; }
		public string AppPath { get; }
		public string Variation { get; set; }
		public string LaunchAppPath { get; }
		public Extension? Extension { get; }

		public AppBundleInformation (string appName, string bundleIdentifier, string appPath, string launchAppPath, Extension? extension)
		{
			AppName = appName;
			BundleIdentifier = bundleIdentifier;
			AppPath = appPath;
			LaunchAppPath = launchAppPath;
			Extension = extension;
		}
	}
}
