using System;
using System.Collections.Generic;
using System.IO;
using xharness;

namespace BCLTestImporter {


	public struct BCLProjectPaths {
		// contains the path of the Info.plist template to use
		public string PListTemplatePath;
		// contains the path of the project template to use
		public string ProjectTemplatePath;
		// contains the path where the project should be written too
		public string ProjectPath;
		// contains the path where the generated files should be written too
		public string GeneratedSubdirPath;
		// contains the path where the assets directory will be found
		public string AssetsDirectoryPath;
		// contains the path to which the RegisterType.cs file should be written
		public string RegisterTypePath;
		// contains the path to witch the Info.plist file should be written
		public string PListPath;

		public WatchOSProjectPaths WatchOSProjectPaths; 

		public override string ToString ()
		{
			return $"PListTemplate: '{PListTemplatePath}' ProjectTemplatePath: '{ProjectTemplatePath}' ProjectPath: '{ProjectPath}' GeneratedSubdirPath: {GeneratedSubdirPath} AssetsDirecotry: '{AssetsDirectoryPath}' RegisterTypesPath: '{RegisterTypePath}' PListPath: '{PListPath}'";
		}

	}
	
	public struct WatchOSProjectPaths {
	
		static readonly Dictionary<WatchAppType, string> watchOSPlistTemplateMatches = new Dictionary<WatchAppType, string> {
			{WatchAppType.App, "Info-watchos-app.plist.in"},
			{WatchAppType.Extension, "Info-watchos-extension.plist.in"}
		};
		
		public string PlistTemplateRootPath;
		public string GeneratedSubdirPath;
		public string ProjectName;
		
		/// <summary>
		/// Returns the path to the template to be used for the given watch app type.
		/// </summary>
		/// <returns>The path to the appropiate path.</returns>
		/// <param name="appType">The type of watchOS application.</param>
		public string GetPListTemplatePath (WatchAppType appType) => Path.Combine (PlistTemplateRootPath, watchOSPlistTemplateMatches [appType]);
		
		public string GetPListPath (WatchAppType appType)
		{
			switch (appType) {
			case WatchAppType.App:
				return Path.Combine (GeneratedSubdirPath, "Info-watchos-app.plist");
			default:
				return Path.Combine (GeneratedSubdirPath, "Info-watchos-extension.plist");
			}
		}
		
		/// <summary>
		/// Returns the project path to be used depending on the watchOS app type being generated. 
		/// </summary>
		/// <returns>The project path.</returns>
		/// <param name="generatedDir">The path for the generated code..</param>
		/// <param name="projectName">Project name.</param>
		/// <param name="appType">App type.</param>
		public string GetProjectPath (WatchAppType appType)
		{
			switch (appType) {
			case WatchAppType.App:
				return Path.Combine (GeneratedSubdirPath, $"{ProjectName}-watchos-app.csproj");
			default:
				return Path.Combine (GeneratedSubdirPath, $"{ProjectName}-watchos-extension.csproj");
			}
		}
	}
	
	/// <summary>
	/// The path manager is the class that takes care of managing the different
	/// paths that are generated in order to create the new projects.
	/// </summary>
	public class BCLPathManager {

		readonly string outputDir;
		readonly string projectTemplateRootPath;
		readonly string plistTemplateRootPath;

		static readonly Dictionary<Platform, string> plistTemplateMatches = new Dictionary<Platform, string> {
			{Platform.iOS, "Info.plist.in"},
			{Platform.TvOS, "Info-tv.plist.in"},
			{Platform.WatchOS, "Info-watchos.plist.in"},
			{Platform.MacOSFull, "Info-mac.plist.in"},
			{Platform.MacOSModern, "Info-mac.plist.in"},
		};

		static readonly Dictionary<Platform, string> projectTemplateMatches = new Dictionary<Platform, string> {
			{Platform.iOS, "BCLTests.csproj.in"},
			{Platform.TvOS, "BCLTests-tv.csproj.in"},
			{Platform.WatchOS, "BCLTests-watchos.csproj.in"},
			{Platform.MacOSFull, "BCLTests-mac.csproj.in"},
			{Platform.MacOSModern, "BCLTests-mac.csproj.in"},
		};

		static readonly Dictionary<WatchAppType, string> watchOSProjectTemplateMatches = new Dictionary<WatchAppType, string>
		{
			{ WatchAppType.App, "BCLTests-watchos-app.csproj.in"},
			{ WatchAppType.Extension, "BCLTests-watchos-extension.csproj.in"}
		};

		public static string BCLAssetsDirectory => Path.Combine (Harness.RootDirectory, "bcl-test", "Assets.xcassets");

		// useful properties for well-known paths
		string generatedCodePathRoot;
		public string GeneratedCodePathRoot {
			get {
				if (generatedCodePathRoot == null) {
					generatedCodePathRoot = Path.Combine (outputDir, "generated");
					if (!Directory.Exists (generatedCodePathRoot))
						Directory.CreateDirectory (generatedCodePathRoot);
				}
				return generatedCodePathRoot;
			}
		}
		public string WatchContainerTemplatePath => Path.Combine (outputDir, "templates", "watchOS", "Container").Replace ("/", "\\");
		public string WatchAppTemplatePath => Path.Combine (outputDir, "templates", "watchOS", "App").Replace ("/", "\\");
		public string WatchExtensionTemplatePath => Path.Combine (outputDir, "templates", "watchOS", "Extension").Replace ("/", "\\");
		
		/// <summary>
		/// Initializes a new instance of the path manager that will build all the required paths based on
		/// the provided output directory
		/// </summary>
		/// <param name="outputDirectory">Ouput directory.</param>
		public BCLPathManager (string outputDirectory, string projectTemplateRoot, string plistTemplateRoot)
		{
			if (string.IsNullOrEmpty (outputDirectory))
				throw new ArgumentException (nameof (outputDirectory));
			if (string.IsNullOrEmpty (projectTemplateRoot))
				throw new ArgumentException (projectTemplateRoot);
			if (string.IsNullOrEmpty (plistTemplateRoot))
				throw new ArgumentException (nameof (plistTemplateRoot));

			outputDir = outputDirectory;
			projectTemplateRootPath = projectTemplateRoot;
			plistTemplateRootPath = plistTemplateRoot;
		}
		
		/// <summary>
		/// Returns the path to the project template to use for the given watchOS app type.
		/// </summary>
		/// <returns>The path to the projects template.</returns>
		/// <param name="appType">watchOS app type targeted.</param>
		public string GetProjectTemplate (WatchAppType appType) => Path.Combine (projectTemplateRootPath, watchOSProjectTemplateMatches [appType]);
		
		/// <summary>
		/// Gets the project template.
		/// </summary>
		/// <returns>The project template.</returns>
		/// <param name="platform">Platform.</param>
		public string GetProjectTemplate (Platform platform) => Path.Combine (projectTemplateRootPath, projectTemplateMatches [platform]);
		
		/// <summary>
		/// Returns the path to the template to be used for the given platform.
		/// </summary>
		/// <returns>The path to the plist template.</returns>
		/// <param name="platform">The platform targeted by the project.</param>
		public string GetPListTemplatePath (Platform platform) => Path.Combine (plistTemplateRootPath, plistTemplateMatches [platform]);
		
		
		/// <summary>
		/// Returns the path to be used to store the projects plist file depending on the platform.
		/// </summary>
		/// <param name="rootDir">The root dir to use.</param>
		/// <param name="platform">The platform that is supported by the project.</param>
		/// <returns></returns>
		public static string GetPListPath (string rootDir, Platform platform)
		{
			switch (platform) {
			case Platform.iOS:
				return Path.Combine (rootDir, "Info.plist");
			case Platform.TvOS:
				return Path.Combine (rootDir, "Info-tv.plist");
			case Platform.WatchOS:
				return Path.Combine (rootDir, "Info-watchos.plist");
			case Platform.MacOSFull:
			case Platform.MacOSModern:
				return Path.Combine (rootDir, "Info-mac.plist");
			default:
				return Path.Combine (rootDir, "Info.plist");
			}
		}

		/// <summary>
		/// Returns the path to be used to store the project file depending on the platform.
		/// </summary>
		/// <param name="projectName">The name of the project being generated.</param>
		/// <param name="platform">The supported platform by the project.</param>
		/// <returns></returns>
		public string GetProjectPath (string generatedDir, string projectName, Platform platform)
		{
			switch (platform) {
			case Platform.iOS:
				return Path.Combine (generatedDir, $"{projectName}.csproj");
			case Platform.TvOS:
				return Path.Combine (generatedDir, $"{projectName}-tvos.csproj");
			case Platform.WatchOS:
				return Path.Combine (generatedDir, $"{projectName}-watchos.csproj");
			case Platform.MacOSFull:
				return Path.Combine (generatedDir, $"{projectName}-mac-full.csproj");
			case Platform.MacOSModern:
				return Path.Combine (generatedDir, $"{projectName}-mac-modern.csproj");
			default:
				return null;
			}
		}

		public BCLProjectPaths GetProjectPaths (BCLTestProjectDefinition projectDefinition, Platform platform)
		{
			string codeGeneratedSubdir = "";
			switch (platform) {
			case Platform.iOS:
				codeGeneratedSubdir = Path.Combine (GeneratedCodePathRoot, projectDefinition.Name, "ios");
				break;
			case Platform.TvOS:
				codeGeneratedSubdir = Path.Combine (GeneratedCodePathRoot, projectDefinition.Name, "tv");
				break;
			case Platform.WatchOS:
				codeGeneratedSubdir = Path.Combine (GeneratedCodePathRoot, projectDefinition.Name, "watch");
				break;
			case Platform.MacOSFull:
			case Platform.MacOSModern:
				codeGeneratedSubdir = Path.Combine (GeneratedCodePathRoot, projectDefinition.Name, "mac");
				break;
			}
			if (!Directory.Exists (codeGeneratedSubdir))
				Directory.CreateDirectory (codeGeneratedSubdir);
			return new BCLProjectPaths {
				PListTemplatePath = GetPListTemplatePath (platform),
				ProjectTemplatePath = GetProjectTemplate (platform),
				ProjectPath = GetProjectPath (codeGeneratedSubdir, projectDefinition.Name, platform),
				GeneratedSubdirPath = codeGeneratedSubdir,
				AssetsDirectoryPath = codeGeneratedSubdir,
				RegisterTypePath = Path.Combine (codeGeneratedSubdir, "RegisterType.cs"),
				PListPath = GetPListPath (codeGeneratedSubdir, platform),
				WatchOSProjectPaths = (platform != Platform.WatchOS)?
					new WatchOSProjectPaths () :
					new WatchOSProjectPaths {
						PlistTemplateRootPath = plistTemplateRootPath,
						GeneratedSubdirPath = codeGeneratedSubdir,
						ProjectName = projectDefinition.Name,
					}
			};
		}
	}
}
