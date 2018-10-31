using System;
using System.IO;
using System.Xml;

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
			switch (MonoNativeInfo.Flavor) {
			case MonoNativeFlavor.Compat:
				return "8.0";
			case MonoNativeFlavor.Unified:
				return "10.0";
			default:
				throw new Exception ($"Unknown MonoNativeFlavor: {MonoNativeInfo.Flavor}");
			}
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
			base.ExecuteInternal ();
		}
	}
}

