using System;
using System.IO;
using System.Xml;

using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Hardware;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;

namespace Xharness.Targets {
	public class UnifiedTarget : iOSTarget {
		// special cases for the BCL applications

		public override string Suffix {
			get {
				return "-ios";
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
				return "x86_64";
			}
		}

		public override string DeviceArchitectures {
			get {
				return "ARM64";
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
			} else
				base.CalculateName ();
		}

		protected override string GetMinimumOSVersion (string templateMinimumOSVersion)
		{
			if (MonoNativeInfo is null)
				return templateMinimumOSVersion;
			return MonoNativeHelper.GetMinimumOSVersion (DevicePlatform.iOS);
		}

		protected override int [] UIDeviceFamily {
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
				return string.Empty;
			}
		}

		public override string DotNetSdk => "Microsoft.iOS.Sdk";
		public override string RuntimeIdentifier => "iossimulator-x64";
		public override DevicePlatform ApplePlatform => DevicePlatform.iOS;
		public override string TargetFramework => DotNetTfm + "-ios";
		public override string TargetFrameworkForNuGet => "xamarinios10";
	}
}
