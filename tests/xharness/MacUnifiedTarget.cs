using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace xharness
{
	public class MacUnifiedTarget : MacTarget
	{
		public bool Mobile { get; private set; }
		public bool System { get; set; }

		// Optional
		public MacBCLTestInfo BCLInfo { get; set; }
		bool IsBCL => BCLInfo != null;

		bool SkipProjectGeneration;
		bool SkipSuffix;

		public MacUnifiedTarget (bool mobile, bool thirtyTwoBit, bool shouldSkipProjectGeneration = false, bool skipSuffix = false) : base ()
		{
			Mobile = mobile;
			ThirtyTwoBit = thirtyTwoBit;
			SkipProjectGeneration = shouldSkipProjectGeneration;
			SkipSuffix = skipSuffix;
		}

		protected override void CalculateName ()
		{
			base.CalculateName ();

			if (IsBCL)
				Name = Name + BCLInfo.FlavorSuffix;
			if (MonoNativeInfo != null)
				Name = Name + MonoNativeInfo.FlavorSuffix;
		}

		public override bool ShouldSkipProjectGeneration
		{
			get
			{
				return SkipProjectGeneration;
			}
		}

		public override string Suffix {
			get {
				if (SkipProjectGeneration)
					return "";
				if (MonoNativeInfo != null) {
					if (System)
						return MonoNativeInfo.FlavorSuffix + "-system";
					return MonoNativeInfo.FlavorSuffix + (ThirtyTwoBit ? "-32" : "");
				}
				if (System)
					return "-system";
				var suffix = (Mobile ? "" : "XM45") + (ThirtyTwoBit ? "-32" : "");
				return "-unified" + (IsBCL ? "" : suffix);
			}
		}

		public override string MakefileWhereSuffix {
			get {
				if (System)
					return "system";
				string suffix = (Mobile ? "" : "XM45") + (ThirtyTwoBit ? "32" : "");
				return "unified" + (IsBCL ? "" : suffix);
			}
		}
			
		protected override string ProjectTypeGuids {
			get {
				return "{A3F8F2AB-B479-4A4A-A458-A89E7DC349F1};" + LanguageGuid;
			}
		}

		protected override string BindingsProjectTypeGuids {
			get {
				return "{810C163F-4746-4721-8B8E-88A3673A62EA}";
			}
		}

		protected override string TargetFrameworkIdentifier {
			get {
				return "Xamarin.Mac";
			}
		}

		protected override string Imports {
			get {
				return IsFSharp ? "Mac\\Xamarin.Mac.FSharp.targets" : "Mac\\Xamarin.Mac.CSharp.targets";
			}
		}

		protected override string BindingsImports {
			get {
				return "Mac\\Xamarin.Mac.ObjcBinding.CSharp";
			}
		}

		protected override string AdditionalDefines {
			get {
				return "XAMCORE_2_0";
			}
		}

		public override bool IsMultiArchitecture {
			get {
				return true;
			}
		}

		public override string Platform {
			get {
				return "mac";
			}
		}

		public override string DefaultAssemblyReference { get { return "XamMac"; } }

		public override IEnumerable<string> ReferenceToRemove { get { yield return "System.Drawing"; } }

		public override bool ShouldSetTargetFrameworkIdentifier { get { return Mobile; } }

		public override Dictionary<string, string> NewPropertiesToAdd 
		{
			get 
			{
				var props = new Dictionary<string, string> ();


				if (System) {
					props.Add ("TargetFrameworkVersion", "v4.7.1");
					props.Add ("MonoBundlingExtraArgs", "--embed-mono=no");
				} else if (Mobile)
				{
					props.Add ("TargetFrameworkVersion", "v2.0");
				}
				else
				{
					props.Add ("TargetFrameworkVersion", "v4.5");
					props.Add ("UseXamMacFullFramework", "true");
				}

				props.Add ("XamMacArch", ThirtyTwoBit ? "i386" : "x86_64");
				return props;
			}
		}

		protected override string GetMinimumOSVersion (string templateMinimumOSVersion)
		{
			if (MonoNativeInfo == null)
				return templateMinimumOSVersion;
			switch (MonoNativeInfo.Flavor) {
			case MonoNativeFlavor.Compat:
				return "10.9";
			case MonoNativeFlavor.Unified:
				return "10.12";
			default:
				throw new Exception ($"Unknown MonoNativeFlavor: {MonoNativeInfo.Flavor}");
			}
		}

		protected override void ProcessProject ()
		{
			base.ProcessProject ();

			if (MonoNativeInfo == null)
				return;

			MonoNativeInfo.AddProjectDefines (inputProject);
			inputProject.AddAdditionalDefines ("MONO_NATIVE_MAC");

			XmlDocument info_plist = new XmlDocument ();
			var target_info_plist = Path.Combine (TargetDirectory, "Info" + Suffix + ".plist");
			info_plist.LoadWithoutNetworkAccess (Path.Combine (TargetDirectory, "Info-mac.plist"));
			BundleIdentifier = info_plist.GetCFBundleIdentifier ();
			var plist_min_version = info_plist.GetPListStringValue ("LSMinimumSystemVersion");
			info_plist.SetPListStringValue ("LSMinimumSystemVersion", GetMinimumOSVersion (plist_min_version));

			inputProject.FixInfoPListInclude (Suffix);

			Harness.Save (info_plist, target_info_plist);
		}
	}
}
