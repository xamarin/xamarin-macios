using System;

namespace xharness
{
	public class UnifiedTarget : Target
	{
		public override string Suffix {
			get {
				return "-unified";
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

		protected override string GetMinimumOSVersion(string templateMinimumOSVersion)
		{
			return templateMinimumOSVersion;
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

		public override string MakefileWhereSuffix {
			get {
				return "unified";
			}
		}

		protected override bool SupportsBitcode {
			get {
				return true;
			}
		}
	}
}

