using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Hardware;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;

namespace Xharness.Targets {
	public class WatchOSTarget : iOSTarget {
		public string AppName { get; private set; }
		public string ExtensionName { get; private set; }

		public string WatchOSAppGuid { get; private set; }
		public string WatchOSExtensionGuid { get; private set; }
		public string WatchOSGuid { get { return ProjectGuid; } private set { ProjectGuid = value; } }

		public string WatchOSAppProjectPath { get; private set; }
		public string WatchOSExtensionProjectPath { get; private set; }
		public string WatchOSProjectPath { get { return ProjectPath; } private set { ProjectPath = value; } }

		public override string SimulatorArchitectures {
			get { return "x86_64"; }
		}

		public override string DeviceArchitectures {
			get { return "ARM64_32"; }
		}

		public override string DotNetSdk => "Microsoft.watchOS.Sdk";
		public override string RuntimeIdentifier => throw new NotImplementedException ();
		public override DevicePlatform ApplePlatform => DevicePlatform.watchOS;
		public override string TargetFramework => DotNetTfm + "-watchos";
		public override string TargetFrameworkForNuGet => "xamarinwatch10";

		void CreateWatchOSAppProject ()
		{
			var csproj = new XmlDocument ();
			var suffix = Suffix + "-app";
			csproj.LoadWithoutNetworkAccess (Path.Combine (Harness.WatchOSAppTemplate, "App.csproj"));
			csproj.FindAndReplace ("%WATCHAPP_PATH%", Path.GetFullPath (Harness.WatchOSAppTemplate).Replace ('/', '\\') + "\\");
			csproj.FindAndReplace ("%WATCHEXTENSION_CSPROJ%", WatchOSExtensionProjectPath);
			csproj.SetProjectReferenceValue (WatchOSExtensionProjectPath, "Project", WatchOSExtensionGuid);
			csproj.SetProjectReferenceValue (WatchOSExtensionProjectPath, "Name", Path.GetFileNameWithoutExtension (WatchOSExtensionProjectPath));
			WatchOSAppGuid = "{" + Xharness.Harness.Helpers.GenerateStableGuid ().ToString ().ToUpper () + "}";
			csproj.SetProjectGuid (WatchOSAppGuid);
			csproj.FixInfoPListInclude (suffix, Path.GetDirectoryName (TemplateProjectPath));
			csproj.ResolveAllPaths (TemplateProjectPath);
			csproj.Save (WatchOSAppProjectPath, Harness);

			XmlDocument info_plist = new XmlDocument ();
			var target_info_plist = Path.Combine (TargetDirectory, $"Info{Suffix}-app.plist");
			info_plist.LoadWithoutNetworkAccess (Path.Combine (Harness.WatchOSAppTemplate, "Info.plist"));
			info_plist.SetCFBundleIdentifier (BundleIdentifier + ".watchkitapp");
			info_plist.SetPListStringValue ("WKCompanionAppBundleIdentifier", BundleIdentifier);
			info_plist.SetPListStringValue ("CFBundleName", Name);
			info_plist.SetMinimumOSVersion (GetMinimumOSVersion (info_plist.GetMinimumOSVersion ()));
			info_plist.Save (target_info_plist, Harness);
		}

		void CreateWatchOSContainerProject ()
		{
			var csproj = new XmlDocument ();
			csproj.LoadWithoutNetworkAccess (Path.Combine (Harness.WatchOSContainerTemplate, "Container.csproj"));

			csproj.FindAndReplace ("%CONTAINER_PATH%", Path.GetFullPath (Harness.WatchOSContainerTemplate).Replace ('/', '\\') + "\\");
			csproj.FindAndReplace ("%WATCHAPP_CSPROJ%", WatchOSAppProjectPath);
			csproj.SetProjectReferenceValue (WatchOSAppProjectPath, "Name", Path.GetFileNameWithoutExtension (WatchOSAppProjectPath));
			WatchOSGuid = "{" + Xharness.Harness.Helpers.GenerateStableGuid ().ToString ().ToUpper () + "}";
			csproj.SetProjectGuid (WatchOSGuid);
			csproj.FixInfoPListInclude (Suffix, Path.GetDirectoryName (TemplateProjectPath));
			csproj.ResolveAllPaths (TemplateProjectPath);
			csproj.Save (WatchOSProjectPath, Harness);

			XmlDocument info_plist = new XmlDocument ();
			var target_info_plist = Path.Combine (TargetDirectory, $"Info{Suffix}.plist");
			info_plist.LoadWithoutNetworkAccess (Path.Combine (Harness.WatchOSContainerTemplate, "Info.plist"));
			info_plist.SetCFBundleIdentifier (BundleIdentifier);
			info_plist.SetCFBundleName (Name);
			info_plist.SetMinimumOSVersion (Xamarin.SdkVersions.LegacyMinWatchOS);
			info_plist.Save (target_info_plist, Harness);
		}

		void CreateWatchOSExtensionProject ()
		{
			var csproj = inputProject;
			var suffix = Suffix + "-extension";

			csproj.FixArchitectures (SimulatorArchitectures, DeviceArchitectures, "iPhoneSimulator", "Debug");
			csproj.FixArchitectures (SimulatorArchitectures, DeviceArchitectures, "iPhoneSimulator", "Release");
			csproj.FixArchitectures (SimulatorArchitectures, DeviceArchitectures, "iPhone", "Debug");
			csproj.FixArchitectures (SimulatorArchitectures, DeviceArchitectures, "iPhone", "Release");

			csproj.SetProjectTypeGuids ("{1E2E965C-F6D2-49ED-B86E-418A60C69EEF};" + LanguageGuid);
			csproj.SetOutputPath ("bin\\$(Platform)\\$(Configuration)" + suffix);
			csproj.SetIntermediateOutputPath ("obj\\$(Platform)\\$(Configuration)" + suffix);
			csproj.RemoveTargetFrameworkIdentifier ();
			csproj.SetPlatformAssembly ("Xamarin.WatchOS");
			csproj.SetImport (IsFSharp ? "$(MSBuildExtensionsPath)\\Xamarin\\WatchOS\\Xamarin.WatchOS.AppExtension.FSharp.targets" : "$(MSBuildExtensionsPath)\\Xamarin\\WatchOS\\Xamarin.WatchOS.AppExtension.CSharp.targets");
			csproj.FixProjectReferences (Path.Combine (ProjectsDir, GetTargetSpecificDir ()), "-watchos", FixProjectReference);

			csproj.FixInfoPListInclude (suffix, Path.GetDirectoryName (TemplateProjectPath));
			csproj.SetOutputType ("Library");
			csproj.AddAdditionalDefines ("XAMCORE_3_0;FEATURE_NO_BSD_SOCKETS;MONOTOUCH_WATCH;");
			csproj.RemoveReferences ("OpenTK-1.0");
			csproj.RemovePackageReference ("MonoTouch.Dialog");
			var ext = IsFSharp ? "fs" : "cs";
			csproj.AddCompileInclude ("InterfaceController." + ext, Path.Combine (Harness.WatchOSExtensionTemplate, "InterfaceController." + ext));
			csproj.SetExtraLinkerDefs ("extra-linker-defs" + ExtraLinkerDefsSuffix + ".xml");
			csproj.SetMtouchUseBitcode (true, "iPhone", "Release");
			csproj.SetMtouchUseLlvm (true, "iPhone", "Release");
			csproj.ResolveAllPaths (TemplateProjectPath);

			// Not linking a watch extensions requires passing -Os to the native compiler.
			// https://github.com/mono/mono/issues/9867
			var configurations = new string [] { "Debug", "Release" };
			foreach (var c in configurations) {
				var flags = "-fembed-bitcode-marker";
				if (csproj.GetMtouchLink ("iPhone", c) == "None")
					flags += " -Os";

				csproj.AddExtraMtouchArgs ($"--gcc_flags='{flags}'", "iPhone", c);
			}

			csproj.Save (WatchOSExtensionProjectPath, Harness);

			WatchOSExtensionGuid = csproj.GetProjectGuid ();

			XmlDocument info_plist = new XmlDocument ();
			var target_info_plist = Path.Combine (TargetDirectory, $"Info{Suffix}-extension.plist");
			var original_info_plist_include = HarnessConfiguration.EvaluateRootTestsDirectory (OriginalInfoPListInclude);
			info_plist.LoadWithoutNetworkAccess (Path.Combine (TargetDirectory, original_info_plist_include));
			BundleIdentifier = info_plist.GetCFBundleIdentifier () + "_watch";
			if (BundleIdentifier.Length >= 58)
				BundleIdentifier = BundleIdentifier.Substring (0, 57); // If the main app's bundle id is 58 characters (or sometimes more), then the watch extension crashes at launch. radar #29847128.
			info_plist.SetCFBundleIdentifier (BundleIdentifier + ".watchkitapp.watchkitextension");
			info_plist.SetMinimumOSVersion (GetMinimumOSVersion (Xamarin.SdkVersions.LegacyMinWatchOS));
			info_plist.SetUIDeviceFamily (4);
			info_plist.AddPListStringValue ("RemoteInterfacePrincipleClass", "InterfaceController");
			info_plist.AddPListKeyValuePair ("NSExtension", "dict", string.Format (
@"
    <key>NSExtensionAttributes</key>
    <dict>
    <key>WKAppBundleIdentifier</key>
    <string>{0}.watchkitapp</string>
    </dict>
    <key>NSExtensionPointIdentifier</key>
    <string>com.apple.watchkit</string>
", BundleIdentifier));
			if (!info_plist.ContainsKey ("NSAppTransportSecurity")) {
				info_plist.AddPListKeyValuePair ("NSAppTransportSecurity", "dict",
	@"
		  <key>NSAllowsArbitraryLoads</key>
		  <true/>
		");
			}
			info_plist.Save (target_info_plist, Harness);
		}

		protected override string Imports {
			get {
				return IsFSharp ? "$(MSBuildExtensionsPath)\\Xamarin\\WatchOS\\Xamarin.WatchOS.FSharp.targets" : "$(MSBuildExtensionsPath)\\Xamarin\\WatchOS\\Xamarin.WatchOS.CSharp.targets";
			}
		}

		protected override string BindingsImports {
			get {
				return IsFSharp ? "$(MSBuildExtensionsPath)\\Xamarin\\WatchOS\\Xamarin.WatchOS.ObjCBinding.FSharp.targets" : "$(MSBuildExtensionsPath)\\Xamarin\\WatchOS\\Xamarin.WatchOS.ObjCBinding.CSharp.targets";
			}
		}

		void CreateWatchOSLibraryProject ()
		{
			var csproj = inputProject;
			csproj.SetProjectTypeGuids ("{FC940695-DFE0-4552-9F25-99AF4A5619A1};" + LanguageGuid);
			csproj.SetOutputPath ("bin\\$(Platform)\\$(Configuration)" + Suffix);
			csproj.SetIntermediateOutputPath ("obj\\$(Platform)\\$(Configuration)" + Suffix);
			csproj.RemoveTargetFrameworkIdentifier ();
			csproj.SetPlatformAssembly ("Xamarin.WatchOS");
			csproj.SetImport (IsBindingProject ? BindingsImports : Imports);
			csproj.AddAdditionalDefines ("XAMCORE_3_0;MONOTOUCH_WATCH;");
			csproj.FixProjectReferences (Path.Combine (ProjectsDir, GetTargetSpecificDir ()), Suffix, FixProjectReference);
			csproj.RemovePackageReference ("MonoTouch.Dialog");
			csproj.SetExtraLinkerDefs ("extra-linker-defs" + ExtraLinkerDefsSuffix + ".xml");
			csproj.FixTestLibrariesReferences (Platform);
			csproj.ResolveAllPaths (TemplateProjectPath);
			csproj.Save (WatchOSProjectPath, Harness);

			WatchOSGuid = csproj.GetProjectGuid ();
		}

		protected override void ExecuteInternal ()
		{
			ExtensionName = Name + " Extension";
			AppName = Name + " App";

			var templateName = Path.GetFileName (TemplateProjectPath);
			if (templateName.EndsWith (".template", StringComparison.OrdinalIgnoreCase))
				templateName = Path.GetFileNameWithoutExtension (templateName);
			templateName = Path.GetFileNameWithoutExtension (templateName);

			switch (OutputType) {
			case "Exe":
				WatchOSExtensionProjectPath = Path.Combine (TargetDirectory, ProjectsDir, GetTargetSpecificDir ("extension"), templateName + Suffix + "-extension.csproj");
				WatchOSAppProjectPath = Path.Combine (TargetDirectory, ProjectsDir, GetTargetSpecificDir ("app"), templateName + Suffix + "-app.csproj");
				CreateWatchOSExtensionProject ();
				CreateWatchOSAppProject ();
				CreateWatchOSContainerProject ();
				break;
			case "Library":
				CreateWatchOSLibraryProject ();
				break;
			default:
				throw new Exception (string.Format ("Unknown OutputType: {0}", OutputType));
			}
		}

		protected override void CalculateName ()
		{
			if (TargetDirectory.Contains ("bcl-test"))
				Name = TestProject.Name;
			else
				base.CalculateName ();
		}

		protected override string GetMinimumOSVersion (string templateMinimumOSVersion)
		{
			if (MonoNativeInfo is null)
				return templateMinimumOSVersion;
			return MonoNativeHelper.GetMinimumOSVersion (DevicePlatform.watchOS);
		}

		public override string Suffix {
			get {
				return "-watchos";
			}
		}

		public override string ExtraLinkerDefsSuffix {
			get {
				return "-watchos";
			}
		}

		public override string Platform {
			get {
				return "watchos";
			}
		}

		public override IEnumerable<RelatedProject> GetRelatedProjects ()
		{
			return new RelatedProject [] {
				new RelatedProject { Guid = WatchOSExtensionGuid, ProjectPath = WatchOSExtensionProjectPath },
				new RelatedProject { Guid = WatchOSAppGuid, ProjectPath = WatchOSAppProjectPath },
			};
		}
	}
}
