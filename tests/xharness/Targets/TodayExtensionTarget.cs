using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;
using Microsoft.DotNet.XHarness.iOS.Shared.Hardware;
using Microsoft.DotNet.XHarness.iOS.Shared;

namespace Xharness.Targets {
	public class TodayExtensionTarget : UnifiedTarget {
		public string AppName { get; private set; }
		public string ExtensionName { get; private set; }

		public string TodayContainerGuid { get; private set; }
		public string TodayExtensionGuid { get; private set; }

		public string TodayContainerProjectPath { get; private set; }
		public string TodayExtensionProjectPath { get; private set; }

		public override string Suffix {
			get {
				return "-today";
			}
		}

		public override string ExtraLinkerDefsSuffix {
			get {
				return "-today";
			}
		}

		public override string ProjectFileSuffix {
			get {
				return "-today";
			}
		}

		void CreateTodayContainerProject ()
		{
			var csproj = new XmlDocument ();
			var suffix = Suffix;
			csproj.LoadWithoutNetworkAccess (Path.Combine (Harness.TodayContainerTemplate, "TodayContainer.csproj"));
			csproj.SetOutputPath ("bin\\$(Platform)\\$(Configuration)" + suffix, false);
			csproj.SetIntermediateOutputPath ("obj\\$(Platform)\\$(Configuration)" + suffix);
			csproj.SetProjectReferenceValue ("TodayExtension.csproj", "Name", Path.GetFileNameWithoutExtension (TodayExtensionProjectPath));
			csproj.SetProjectReferenceValue ("TodayExtension.csproj", "Project", TodayExtensionGuid);
			csproj.SetProjectReferenceInclude ("TodayExtension.csproj", TodayExtensionProjectPath.Replace ('/', '\\'));
			csproj.FixCompileInclude ("Main.cs", Path.Combine (Harness.TodayContainerTemplate, "Main.cs").Replace ('/', '\\'));
			csproj.FixInfoPListInclude (suffix, Path.GetDirectoryName (TemplateProjectPath));
			TodayContainerGuid = "{" + Xharness.Harness.Helpers.GenerateStableGuid ().ToString ().ToUpper () + "}";
			ProjectGuid = TodayContainerGuid;
			csproj.SetProjectGuid (TodayContainerGuid);
			csproj.ResolveAllPaths (Harness.TodayContainerTemplate);
			csproj.Save (TodayContainerProjectPath, Harness);

			XmlDocument info_plist = new XmlDocument ();
			var target_info_plist = Path.Combine (TargetDirectory, $"Info{suffix}.plist");
			info_plist.LoadWithoutNetworkAccess (Path.Combine (Harness.TodayContainerTemplate, "Info.plist"));
			info_plist.SetCFBundleIdentifier (BundleIdentifier);
			info_plist.SetCFBundleName (Name);
			info_plist.SetMinimumOSVersion (GetMinimumOSVersion (Xamarin.SdkVersions.MiniOS));
			info_plist.Save (target_info_plist, Harness);
		}

		void CreateTodayExtensionProject ()
		{
			var csproj = inputProject;
			var suffix = Suffix + "-extension";
			csproj.SetProjectTypeGuids ("{EE2C853D-36AF-4FDB-B1AD-8E90477E2198};" + LanguageGuid);
			csproj.SetOutputPath ("bin\\$(Platform)\\$(Configuration)" + suffix);
			csproj.SetIntermediateOutputPath ("obj\\$(Platform)\\$(Configuration)" + suffix);
			csproj.SetImport (IsFSharp ? "$(MSBuildExtensionsPath)\\Xamarin\\iOS\\Xamarin.iOS.AppExtension.FSharp.targets" : "$(MSBuildExtensionsPath)\\Xamarin\\iOS\\Xamarin.iOS.AppExtension.CSharp.targets");
			csproj.FixInfoPListInclude (suffix, Path.GetDirectoryName (TemplateProjectPath));
			csproj.SetOutputType ("Library");
			csproj.AddAdditionalDefines ("XAMCORE_3_0;TODAY_EXTENSION");
			var ext = IsFSharp ? "fs" : "cs";
			// we have diff templates for the bcl tests because they use xunit/nunit and not monotouch nunit.
			csproj.AddCompileInclude ("TodayExtensionMain." + ext, Path.Combine (Harness.TodayExtensionTemplate, "TodayExtensionMain." + ext), true);
			csproj.AddInterfaceDefinition (Path.Combine (Harness.TodayExtensionTemplate, "TodayView.storyboard").Replace ('/', '\\'));
			csproj.SetExtraLinkerDefs ("extra-linker-defs" + ExtraLinkerDefsSuffix + ".xml");
			csproj.FixProjectReferences (Path.Combine (ProjectsDir, GetTargetSpecificDir ()), "-today", FixProjectReference);
			csproj.ResolveAllPaths (TemplateProjectPath);
			csproj.Save (TodayExtensionProjectPath, Harness);

			TodayExtensionGuid = csproj.GetProjectGuid ();

			XmlDocument info_plist = new XmlDocument ();
			var target_info_plist = Path.Combine (TargetDirectory, $"Info{suffix}.plist");
			var original_info_plist_include = HarnessConfiguration.EvaluateRootTestsDirectory (OriginalInfoPListInclude);
			info_plist.LoadWithoutNetworkAccess (Path.Combine (TargetDirectory, original_info_plist_include));
			BundleIdentifier = info_plist.GetCFBundleIdentifier () + "-today";
			info_plist.SetCFBundleIdentifier (BundleIdentifier + ".todayextension");
			info_plist.SetMinimumOSVersion (GetMinimumOSVersion (Xamarin.SdkVersions.MiniOS));
			info_plist.AddPListStringValue ("CFBundlePackageType", "XPC!");
			info_plist.SetCFBundleDisplayName (Name);
			info_plist.AddPListKeyValuePair ("NSExtension", "dict",
@"
        <key>NSExtensionMainStoryboard</key>
        <string>TodayView</string>
        <key>NSExtensionPointIdentifier</key>
        <string>com.apple.widget-extension</string>
    ");
			info_plist.Save (target_info_plist, Harness);
		}

		protected override void ExecuteInternal ()
		{
			ExtensionName = Name + " Today Extension";
			AppName = Name + " Today";

			var templateName = Path.GetFileName (TemplateProjectPath);
			if (templateName.EndsWith (".template", StringComparison.OrdinalIgnoreCase))
				templateName = Path.GetFileNameWithoutExtension (templateName);
			templateName = Path.GetFileNameWithoutExtension (templateName);

			switch (OutputType) {
			case "Exe":
				TodayExtensionProjectPath = Path.Combine (TargetDirectory, ProjectsDir, GetTargetSpecificDir ("extension"), templateName + Suffix + "-extension." + ProjectFileExtension);
				TodayContainerProjectPath = Path.Combine (TargetDirectory, ProjectsDir, GetTargetSpecificDir (), templateName + Suffix + "." + ProjectFileExtension);
				CreateTodayExtensionProject ();
				CreateTodayContainerProject ();
				break;
			case "Library":
				CreateLibraryProject ();
				break;
			default:
				throw new Exception (string.Format ("Unknown OutputType: {0}", OutputType));
			}
		}

		protected override string GetMinimumOSVersion (string templateMinimumOSVersion)
		{
			if (MonoNativeInfo is null)
				return templateMinimumOSVersion;
			return MonoNativeHelper.GetMinimumOSVersion (DevicePlatform.iOS);
		}

		public override IEnumerable<RelatedProject> GetRelatedProjects ()
		{
			return new RelatedProject [] {
				new RelatedProject { Guid = TodayExtensionGuid, ProjectPath = TodayExtensionProjectPath },
			};
		}
	}
}
