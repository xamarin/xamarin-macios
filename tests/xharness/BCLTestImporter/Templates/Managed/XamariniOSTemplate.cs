using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Xharness.BCLTestImporter.Templates.Managed {

	// template project that uses the Xamarin.iOS and Xamarin.Mac frameworks
	// to create a testing application for given xunit and nunit test assemblies
	public class XamariniOSTemplate : ITemplatedProject {
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
	}
}
