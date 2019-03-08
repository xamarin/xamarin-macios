using System;
using System.IO;
using System.Xml;

using Xamarin;

namespace xharness
{
	public class UnifiedTarget : iOSTarget
	{
		public override string Suffix {
			get {
				return MonoNativeInfo != null ? MonoNativeInfo.FlavorSuffix : "-ios";
			}
		}

		public override string ExtraLinkerDefsSuffix {
			get {
				return string.Empty;
			}
		}

		protected override string ProjectTypeGuids {
			get {
				return "{FEACFBD2-3405-455C-9665-78FE426C6842};" + LanguageGuid;
			}
		}

		protected override string BindingsProjectTypeGuids {
			get {
				return "{8FFB629D-F513-41CE-95D2-7ECE97B6EEEC}";
			}
		}

		protected override string TargetFrameworkIdentifier {
			get {
				return "Xamarin.iOS";
			}
		}

		protected override string Imports {
			get {
				return IsFSharp ? "iOS\\Xamarin.iOS.FSharp.targets" : "iOS\\Xamarin.iOS.CSharp.targets";
			}
		}

		protected override string BindingsImports {
			get {
				return IsFSharp ? "iOS\\Xamarin.iOS.ObjCBinding.FSharp.targets" : "iOS\\Xamarin.iOS.ObjCBinding.CSharp.targets";
			}
		}

		public override string SimulatorArchitectures {
			get {
				return "i386, x86_64";
			}
		}

		public override string DeviceArchitectures {
			get {
				if (SupportsBitcode)
					return "ARM64";
				else
					return "ARMv7, ARM64";
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
			return MonoNativeHelper.GetMinimumOSVersion (DevicePlatform.iOS, MonoNativeInfo.Flavor);
		}

		protected override int[] UIDeviceFamily {
			get {
				return new int [] { 1, 2 };
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
				return "ios";
			}
		}

		public override string ProjectFileSuffix {
			get {
				if (MonoNativeInfo != null)
					return MonoNativeInfo.FlavorSuffix;
				return string.Empty;
			}
		}

		protected override bool SupportsBitcode {
			get {
				return true;
			}
		}

		protected override void ExecuteInternal ()
		{
			if (MonoNativeInfo == null)
				return;

			MonoNativeInfo.AddProjectDefines (inputProject);
			inputProject.AddAdditionalDefines ("MONO_NATIVE_IOS");

			inputProject.FixInfoPListInclude (Suffix);
			inputProject.SetExtraLinkerDefs ("extra-linker-defs" + ExtraLinkerDefsSuffix + ".xml");

			Harness.Save (inputProject, ProjectPath);

			XmlDocument info_plist = new XmlDocument ();
			var target_info_plist = Path.Combine (TargetDirectory, "Info" + Suffix + ".plist");
			info_plist.LoadWithoutNetworkAccess (Path.Combine (TargetDirectory, "Info.plist"));
			info_plist.SetMinimumOSVersion (GetMinimumOSVersion (info_plist.GetMinimumOSVersion ()));
			Harness.Save (info_plist, target_info_plist);
		}
	}
}

