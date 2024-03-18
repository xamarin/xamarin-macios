using System;
using Microsoft.DotNet.XHarness.iOS.Shared;
using System.Threading.Tasks;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;
using System.IO;
using System.Xml;

using Xharness.Targets;

#nullable enable

namespace Xharness {
	public static class IAppBundleInformationParserExtensions {
		// This is a copy of this method: https://github.com/dotnet/xharness/blob/aa434d0c7e6eb46df1ec11b3c63add37d835c4d0/src/Microsoft.DotNet.XHarness.iOS.Shared/AppBundleInformationParser.cs#L43-L103
		// And then augmented to handle that the path to the Info.plist might have $(RootTestsDirectory) in it.
		public async static Task<AppBundleInformation> ParseFromProject2 (this IAppBundleInformationParser @this, IAppBundleLocator? _appBundleLocator, string projectFilePath, TestTarget target, string buildConfiguration)
		{
			var csproj = new XmlDocument ();
			csproj.LoadWithoutNetworkAccess (projectFilePath);

			var projectDirectory = Path.GetDirectoryName (projectFilePath) ?? throw new DirectoryNotFoundException ($"Cannot find directory of project '{projectFilePath}'");

			var appName = csproj.GetAssemblyName ();
			var infoPlistPath = csproj.GetInfoPListInclude () ?? throw new InvalidOperationException ("Couldn't locate PList include tag");

			var infoPlist = new XmlDocument ();
			var plistPath = Path.Combine (projectDirectory, HarnessConfiguration.EvaluateRootTestsDirectory (infoPlistPath.Replace ('\\', Path.DirectorySeparatorChar)));
			infoPlist.LoadWithoutNetworkAccess (plistPath);

			var bundleIdentifier = infoPlist.GetCFBundleIdentifier ();
			var bundleExecutable = infoPlist.GetCFBundleExecutable ();

			Extension? extension = null;
			var extensionPointIdentifier = infoPlist.GetNSExtensionPointIdentifier ();
			if (!string.IsNullOrEmpty (extensionPointIdentifier)) {
				extension = extensionPointIdentifier.ParseFromNSExtensionPointIdentifier ();
			}

			var platform = target.IsSimulator () ? "iPhoneSimulator" : "iPhone";

			string? appPath = null;
			if (_appBundleLocator is not null)
				appPath = await _appBundleLocator.LocateAppBundle (csproj, projectFilePath, target, buildConfiguration);

			appPath ??= csproj.GetOutputPath (platform, buildConfiguration)?.Replace ('\\', Path.DirectorySeparatorChar);

			appPath = Path.Combine (
				projectDirectory,
				appPath ?? string.Empty,
				appName + (extension is not null ? ".appex" : ".app"));

			var arch = csproj.GetMtouchArch (platform, buildConfiguration);

			var supports32 = arch is not null && (Contains (arch, "ARMv7") || Contains (arch, "i386"));

			if (!Directory.Exists (appPath))
				throw new DirectoryNotFoundException ($"The app bundle directory `{appPath}` does not exist");

			var launchAppPath = target.ToRunMode () == RunMode.WatchOS
				? Directory.GetDirectories (Path.Combine (appPath, "Watch"), "*.app") [0]
				: appPath;

			return new AppBundleInformation (
				appName,
				bundleIdentifier,
				appPath,
				launchAppPath,
				supports32,
				extension,
				bundleExecutable);
		}

		// This method was added because .NET Standard 2.0 doesn't have case ignorant Contains() for String.
		static bool Contains (string haystack, string needle)
		{
			return haystack.IndexOf (needle, StringComparison.InvariantCultureIgnoreCase) > -1;
		}
	}
}

