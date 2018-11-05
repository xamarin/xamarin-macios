using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace xharness
{
	public class WatchOSTarget : iOSTarget
	{
		public string AppName { get; private set; }
		public string ExtensionName { get; private set; }

		public string WatchOSAppGuid { get; private set; }
		public string WatchOSExtensionGuid { get; private set; }
		public string WatchOSGuid { get { return ProjectGuid; } private set { ProjectGuid = value; } }

		public string WatchOSAppProjectPath { get; private set; }
		public string WatchOSExtensionProjectPath { get; private set; }
		public string WatchOSProjectPath { get { return ProjectPath; } private set { ProjectPath = value; } }

		void CreateWatchOSAppProject ()
		{
			var csproj = new XmlDocument ();
			var suffix = Suffix + "-app";
			csproj.LoadWithoutNetworkAccess (Path.Combine (Harness.WatchOSAppTemplate, "App.csproj"));
			csproj.FindAndReplace ("%WATCHAPP_PATH%", Path.GetFullPath (Harness.WatchOSAppTemplate).Replace ('/', '\\') + "\\");
			csproj.FindAndReplace ("%WATCHEXTENSION_CSPROJ%", Path.GetFileName (WatchOSExtensionProjectPath));
			csproj.SetProjectReferenceValue (Path.GetFileName (WatchOSExtensionProjectPath), "Project", WatchOSExtensionGuid);
			csproj.SetProjectReferenceValue (Path.GetFileName (WatchOSExtensionProjectPath), "Name", Path.GetFileNameWithoutExtension (WatchOSExtensionProjectPath));
			WatchOSAppGuid = "{" + Harness.NewStableGuid ().ToString ().ToUpper () + "}";
			csproj.SetProjectGuid (WatchOSAppGuid);
			csproj.FixInfoPListInclude (suffix);
			if (MonoNativeInfo != null) {
				MonoNativeInfo.AddProjectDefines (csproj);
				csproj.AddAdditionalDefines ("MONO_NATIVE_WATCH");
			}
			Harness.Save (csproj, WatchOSAppProjectPath);

			XmlDocument info_plist = new XmlDocument ();
			var target_info_plist = Path.Combine (TargetDirectory, $"Info{Suffix}-app.plist");
			info_plist.LoadWithoutNetworkAccess (Path.Combine (Harness.WatchOSAppTemplate, "Info.plist"));
			info_plist.SetCFBundleIdentifier (BundleIdentifier + ".watchkitapp");
			info_plist.SetPListStringValue ("WKCompanionAppBundleIdentifier", BundleIdentifier);
			info_plist.SetPListStringValue ("CFBundleName", Name);
			info_plist.SetMinimumOSVersion (GetMinimumOSVersion (info_plist.GetMinimumOSVersion ()));
			Harness.Save (info_plist, target_info_plist);
		}

		void CreateWatchOSContainerProject ()
		{
			var csproj = new XmlDocument ();
			csproj.LoadWithoutNetworkAccess (Path.Combine (Harness.WatchOSContainerTemplate, "Container.csproj"));

			csproj.FindAndReplace ("%CONTAINER_PATH%", Path.GetFullPath (Harness.WatchOSContainerTemplate).Replace ('/', '\\') + "\\");
			csproj.FindAndReplace ("%WATCHAPP_CSPROJ%", Path.GetFileName (WatchOSAppProjectPath));
			csproj.SetProjectReferenceValue (Path.GetFileName (WatchOSAppProjectPath), "Name", Path.GetFileNameWithoutExtension (WatchOSAppProjectPath));
			WatchOSGuid = "{" + Harness.NewStableGuid ().ToString ().ToUpper () + "}";
			csproj.SetProjectGuid (WatchOSGuid);
			csproj.FixInfoPListInclude (Suffix);
			if (MonoNativeInfo != null) {
				MonoNativeInfo.AddProjectDefines (csproj);
				csproj.AddAdditionalDefines ("MONO_NATIVE_WATCH");
			}
			Harness.Save (csproj, WatchOSProjectPath);

			XmlDocument info_plist = new XmlDocument ();
			var target_info_plist = Path.Combine (TargetDirectory, $"Info{Suffix}.plist");
			info_plist.LoadWithoutNetworkAccess (Path.Combine (Harness.WatchOSContainerTemplate, "Info.plist"));
			info_plist.SetCFBundleIdentifier (BundleIdentifier);
			info_plist.SetCFBundleName (Name);
			info_plist.SetMinimumOSVersion ("9.0");
			Harness.Save (info_plist, target_info_plist);
		}

		void CreateWatchOSExtensionProject ()
		{
			var csproj = inputProject;
			var suffix = Suffix + "-extension";
			csproj.SetProjectTypeGuids ("{1E2E965C-F6D2-49ED-B86E-418A60C69EEF};" + LanguageGuid);
			csproj.SetOutputPath ("bin\\$(Platform)\\$(Configuration)" + suffix);
			csproj.SetIntermediateOutputPath ("obj\\$(Platform)\\$(Configuration)" + suffix);
			csproj.RemoveTargetFrameworkIdentifier ();
			csproj.SetPlatformAssembly ("Xamarin.WatchOS");
			csproj.SetImport (IsFSharp ? "$(MSBuildExtensionsPath)\\Xamarin\\WatchOS\\Xamarin.WatchOS.AppExtension.FSharp.targets" : "$(MSBuildExtensionsPath)\\Xamarin\\WatchOS\\Xamarin.WatchOS.AppExtension.CSharp.targets");
			csproj.FixProjectReferences ("-watchos");
			csproj.FixArchitectures ("i386", "ARMv7k");
			csproj.FixInfoPListInclude (suffix);
			csproj.SetOutputType ("Library");
			csproj.AddAdditionalDefines ("BITCODE", "iPhone", "Release");
			csproj.AddAdditionalDefines ("XAMCORE_2_0;XAMCORE_3_0;FEATURE_NO_BSD_SOCKETS");
			csproj.RemoveReferences ("OpenTK-1.0");
			var ext = IsFSharp ? "fs" : "cs";
			csproj.AddCompileInclude ("InterfaceController." + ext, Path.Combine (Harness.WatchOSExtensionTemplate, "InterfaceController." + ext));
			csproj.SetExtraLinkerDefs ("extra-linker-defs" + ExtraLinkerDefsSuffix + ".xml");
			csproj.SetMtouchUseBitcode (true, "iPhone", "Release");
			csproj.SetMtouchUseLlvm (true, "iPhone", "Release");

			if (MonoNativeInfo != null) {
				MonoNativeInfo.AddProjectDefines (csproj);
				csproj.AddAdditionalDefines ("MONO_NATIVE_WATCH");
			}

			// Not linking a watch extensions requires passing -Os to the native compiler.
			// https://github.com/mono/mono/issues/9867
			var configurations = new string [] { "Debug", "Debug32", "Release", "Release32", "Release-bitcode" };
			foreach (var c in configurations)
				if (csproj.GetMtouchLink ("iPhone", c) == "None")
					csproj.AddExtraMtouchArgs ("--gcc_flags=-Os", "iPhone", c);

			Harness.Save (csproj, WatchOSExtensionProjectPath);

			WatchOSExtensionGuid = csproj.GetProjectGuid ();

			XmlDocument info_plist = new XmlDocument ();
			var target_info_plist = Path.Combine (TargetDirectory, $"Info{Suffix}-extension.plist");
			info_plist.LoadWithoutNetworkAccess (Path.Combine (TargetDirectory, "Info.plist"));
			BundleIdentifier = info_plist.GetCFBundleIdentifier () + "-watch";
			if (BundleIdentifier.Length >= 58)
				BundleIdentifier = BundleIdentifier.Substring (0, 57); // If the main app's bundle id is 58 characters (or sometimes more), then the watch extension crashes at launch. radar #29847128.
			info_plist.SetCFBundleIdentifier (BundleIdentifier + ".watchkitapp.watchkitextension");
			info_plist.SetMinimumOSVersion (GetMinimumOSVersion ("2.0"));
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
			Harness.Save (info_plist, target_info_plist);
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
			csproj.AddAdditionalDefines ("XAMCORE_2_0;XAMCORE_3_0");
			csproj.FixProjectReferences (Suffix);
			csproj.SetExtraLinkerDefs ("extra-linker-defs" + ExtraLinkerDefsSuffix + ".xml");
			csproj.FixTestLibrariesReferences (Platform);
			Harness.Save (csproj, WatchOSProjectPath);

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
				WatchOSExtensionProjectPath = Path.Combine (TargetDirectory, templateName + Suffix + "-extension.csproj");
				WatchOSAppProjectPath = Path.Combine (TargetDirectory, templateName + Suffix + "-app.csproj");
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
			base.CalculateName ();
			if (MonoNativeInfo != null)
				Name = Name + MonoNativeInfo.FlavorSuffix;
		}

		protected override string GetMinimumOSVersion (string templateMinimumOSVersion)
		{
			if (MonoNativeInfo == null)
				return templateMinimumOSVersion;
			switch (MonoNativeInfo.Flavor) {
			case MonoNativeFlavor.Compat:
				return "2.0";
			case MonoNativeFlavor.Unified:
				return "4.0";
			default:
				throw new Exception ($"Unknown MonoNativeFlavor: {MonoNativeInfo.Flavor}");
			}
		}

		public override string Suffix {
			get {
				return MonoNativeInfo != null ? MonoNativeInfo.FlavorSuffix + "-watchos" : "-watchos";
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

		protected override bool SupportsBitcode {
			get {
				return true;
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

