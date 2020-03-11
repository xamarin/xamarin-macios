using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Xharness.BCLTestImporter.Templates.Managed {

	// template project that uses the Xamarin.iOS and Xamarin.Mac frameworks
	// to create a testing application for given xunit and nunit test assemblies
	public class XamariniOSTemplate : ITemplatedProject {
		static string srcResourcePrefix = "Xharness.BCLTestImporter.Templates.Managed.Resources.src.";
		static string [] [] srcDirectories = new [] {
			new [] { "common", },
			new [] { "common", "TestRunner" },
			new [] { "common", "TestRunner", "Core" },
			new [] { "common", "TestRunner", "NUnit" },
			new [] { "common", "TestRunner", "xUnit" },
			new [] { "iOSApp" },
			new [] { "macOS" },
			new [] { "today" },
			new [] { "tvOSApp" },
			new [] { "watchOS" },
			new [] { "watchOS", "App" },
			new [] { "watchOS" ,"App", "Images.xcassets" },
			new [] { "watchOS", "App", "Images.xcassets", "AppIcons.appiconset" },
			new [] { "watchOS", "App", "Resources" },
			new [] { "watchOS", "App", "Resources", "Images.xcassets" },
			new [] { "watchOS", "App", "Resources", "Images.xcassets", "AppIcons.appiconset" },
			new [] { "watchOS", "Container" },
			new [] { "watchOS", "Container", "Resources" },
			new [] { "watchOS", "Container", "Resources", "Images.xcassets" },
			new [] { "watchOS", "Container", "Resources", "Images.xcassets", "AppIcons.appiconset" },
			new [] { "watchOS", "Extension" }
		};

		static readonly Dictionary<Platform, string> plistTemplateMatches = new Dictionary<Platform, string> {
			{Platform.iOS, "Managed.iOS.plist.in"},
			{Platform.TvOS, "Managed.tvOS.plist.in"},
			{Platform.WatchOS, "Managed.watchOS.plist.in"},
			{Platform.MacOSFull, "Managed.macOS.plist.in"},
			{Platform.MacOSModern, "Managed.macOS.plist.in"},
		};
		static readonly Dictionary<Platform, string> projectTemplateMatches = new Dictionary<Platform, string> {
			{Platform.iOS, "Managed.iOS.csproj.in"},
			{Platform.TvOS, "Managed.tvOS.csproj.in"},
			{Platform.WatchOS, "Managed.watchOS.csproj.in"},
			{Platform.MacOSFull, "Managed.macOS.csproj.in"},
			{Platform.MacOSModern, "Managed.macOS.csproj.in"},
		};
		static readonly Dictionary<WatchAppType, string> watchOSProjectTemplateMatches = new Dictionary<WatchAppType, string>
		{
			{ WatchAppType.App, "Managed.watchOS.App.csproj.in"},
			{ WatchAppType.Extension, "Managed.watchOS.Extension.csproj.in"}
		};



		static readonly Dictionary<WatchAppType, string> watchOSPlistTemplateMatches = new Dictionary<WatchAppType, string> {
			{WatchAppType.App, "Managed.watchOS.App.plist.in"},
			{WatchAppType.Extension, "Managed.watchOS.Extension.plist.in"}
		};

		Stream GetTemplateStream (string templateName)
		{
			var name = GetType ().Assembly.GetManifestResourceNames ().Where (a => a.EndsWith (templateName, StringComparison.Ordinal)).FirstOrDefault ();
			return GetType ().Assembly.GetManifestResourceStream (name);
		}

		public Stream GetPlistTemplate (Platform platform) => GetTemplateStream (plistTemplateMatches [platform]);

		public Stream GetPlistTemplate (WatchAppType appType) => GetTemplateStream (watchOSPlistTemplateMatches [appType]);

		public Stream GetProjectTemplate (Platform platform) => GetTemplateStream (projectTemplateMatches [platform]);

		public Stream GetProjectTemplate (WatchAppType appType) => GetTemplateStream (watchOSProjectTemplateMatches [appType]);

		void BuildSrcTree (string srcOuputPath)
		{
			// loop over the known paths, and build them accodringly
			foreach (var components in srcDirectories) {
				var completePathComponents = new [] { srcOuputPath }.Concat (components).ToArray ();
				var path = Path.Combine (completePathComponents);
				Directory.CreateDirectory (path);
			}
		}

		string CalculateDestinationPath (string srcOuputPath, string resourceFullName)
		{
			// we do know that we don't care about our prefix
			var resourceName = resourceFullName.Substring (srcResourcePrefix.Length);
			// icon sets are special, they have a dot, which is also a dot in the resources :/
			var iconSetSubPath = "Images.xcassets.AppIcons.appiconset";
			int lastIndex = resourceName.LastIndexOf (iconSetSubPath);
			if (lastIndex >= 0) {
				var partialPath = Path.Combine (resourceName.Substring (0, lastIndex).Replace ('.', Path.DirectorySeparatorChar), "Images.xcassets", "AppIcons.appiconset");
				// substring up to the iconset path, replace . for PathSeparator, add icon set + name
				resourceName = Path.Combine (partialPath, resourceName.Substring (partialPath.Length + 1));
			} else {
				// replace all . for the path separator, since that is how resource names are built
				lastIndex = resourceName.LastIndexOf ('.');
				if (resourceFullName.Contains (".designer.cs"))
					lastIndex = resourceName.LastIndexOf (".designer.cs");
				if (lastIndex > 0) {
					var partialPath = resourceName.Substring (0, lastIndex).Replace ('.', Path.DirectorySeparatorChar);
					resourceName = partialPath + resourceName.Substring (partialPath.Length);
				}
			}
			return Path.Combine (srcOuputPath, resourceName);
		}

		public async Task GenerateSource (string srcOuputPath)
		{
			// mk the expected directories
			BuildSrcTree (srcOuputPath);
			// the code is simple, we are going to look for all the resources that we know are src and will write a
			// copy of the stream in the designated output path
			var resources = GetType ().Assembly.GetManifestResourceNames ().Where (a => a.StartsWith (srcResourcePrefix));

			// we need to be smart, since the resource name != the path
			foreach (var r in resources) {
				var path = CalculateDestinationPath (srcOuputPath, r);
				// copy the stream
				using (var sourceReader = GetType ().Assembly.GetManifestResourceStream (r))
				using (var destination = File.Create (path)) {
					await sourceReader.CopyToAsync (destination);
				}
			}
		}
	}
}
