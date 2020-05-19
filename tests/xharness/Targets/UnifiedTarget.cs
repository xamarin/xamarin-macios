using System;
using System.IO;
using System.Xml;
using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Hardware;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;

namespace Xharness.Targets {
	public class UnifiedTarget : iOSTarget
	{
		// special cases for the BCL applications
		
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
			if (TargetDirectory.Contains ("bcl-test")) {
				if (TestProject.Name.StartsWith ("mscorlib", StringComparison.Ordinal))
					Name = TestProject.Name;
				else {
					var bclIndex = TestProject.Name.IndexOf ("BCL", StringComparison.Ordinal);
					// most of the BCL test are grouped, but there are a number that are not, in those cases remove the "{testype} Mono " prefix
					Name = (bclIndex == -1) ? TestProject.Name.Substring (TestProject.Name.IndexOf ("Mono ", StringComparison.Ordinal) + "Mono ".Length) : TestProject.Name.Substring (bclIndex);
				}
			}  else
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
				return "";
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

			inputProject.Save (ProjectPath, (l,m) => Harness.Log (l,m));

			XmlDocument info_plist = new XmlDocument ();
			var target_info_plist = Path.Combine (TargetDirectory, "Info" + Suffix + ".plist");
			info_plist.LoadWithoutNetworkAccess (Path.Combine (TargetDirectory, "Info.plist"));
			info_plist.SetMinimumOSVersion (GetMinimumOSVersion (info_plist.GetMinimumOSVersion ()));
			info_plist.Save (target_info_plist, (l,m) => Harness.Log (l,m));
		}
	}
}
