using System.IO;
using System.Xml;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;

namespace Microsoft.DotNet.XHarness.iOS.Shared {

	public interface IAppBundleInformationParser {
		AppBundleInformation ParseFromProject (string projectFilePath, TestTarget target, string buildConfiguration);
	}

	public class AppBundleInformationParser : IAppBundleInformationParser {
		public AppBundleInformation ParseFromProject (string projectFilePath, TestTarget target, string buildConfiguration)
		{
			var csproj = new XmlDocument ();
			csproj.LoadWithoutNetworkAccess (projectFilePath);

			string appName = csproj.GetAssemblyName ();
			string info_plist_path = csproj.GetInfoPListInclude ();

			var info_plist = new XmlDocument ();
			string plistPath = Path.Combine (Path.GetDirectoryName (projectFilePath), info_plist_path.Replace ('\\', Path.DirectorySeparatorChar));
			info_plist.LoadWithoutNetworkAccess (plistPath);

			string bundleIdentifier = info_plist.GetCFBundleIdentifier ();

			Extension? extension = null;
			string extensionPointIdentifier = info_plist.GetNSExtensionPointIdentifier ();
			if (!string.IsNullOrEmpty (extensionPointIdentifier))
				extension = extensionPointIdentifier.ParseFromNSExtensionPointIdentifier ();

			var platform = target.IsSimulator () ? "iPhoneSimulator" : "iPhone";

			string appPath = Path.Combine (Path.GetDirectoryName (projectFilePath),
				csproj.GetOutputPath (platform, buildConfiguration).Replace ('\\', Path.DirectorySeparatorChar),
				appName + (extension != null ? ".appex" : ".app"));

			if (!Directory.Exists (appPath))
				throw new DirectoryNotFoundException ($"The app bundle directory `{appPath}` does not exist");

			string launchAppPath = target.ToRunMode () == RunMode.WatchOS
				? Directory.GetDirectories (Path.Combine (appPath, "Watch"), "*.app") [0]
				: appPath;

			return new AppBundleInformation (appName, bundleIdentifier, appPath, launchAppPath, extension);
		}
	}
}
