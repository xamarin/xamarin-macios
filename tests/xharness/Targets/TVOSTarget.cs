using System.Collections.Generic;
using System.IO;
using System.Xml;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;
using Microsoft.DotNet.XHarness.iOS.Shared.Hardware;
using Microsoft.DotNet.XHarness.iOS.Shared;

namespace Xharness.Targets {
	public class TVOSTarget : iOSTarget {
		public override string Suffix {
			get {
				return "-tvos";
			}
		}

		public override string ExtraLinkerDefsSuffix {
			get {
				return "-tvos";
			}
		}

		protected override string BindingsProjectTypeGuids {
			get {
				return "{4A1ED743-3331-459B-915A-4B17C7B6DBB6}";
			}
		}

		protected override string ProjectTypeGuids {
			get {
				return "{06FA79CB-D6CD-4721-BB4B-1BD202089C55}";
			}
		}

		protected override string TargetFrameworkIdentifier {
			get {
				return "Xamarin.TVOS";
			}
		}

		protected override string Imports {
			get {
				return IsFSharp ? "TVOS\\Xamarin.TVOS.FSharp.targets" : "TVOS\\Xamarin.TVOS.CSharp.targets";
			}
		}

		protected override string BindingsImports {
			get {
				return IsFSharp ? "TVOS\\Xamarin.TVOS.ObjCBinding.FSharp.targets" : "TVOS\\Xamarin.TVOS.ObjCBinding.CSharp.targets";
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

		protected override string GetMinimumOSVersion (string templateMinimumOSVersion)
		{
			if (MonoNativeInfo is null)
				return Xamarin.SdkVersions.MinTVOS;
			return MonoNativeHelper.GetMinimumOSVersion (DevicePlatform.tvOS);
		}

		protected override int [] UIDeviceFamily {
			get {
				return new int [] { 3 };
			}
		}

		protected override string AdditionalDefines {
			get {
				return "XAMCORE_3_0;MONOTOUCH_TV;";
			}
		}

		public override string Platform {
			get {
				return "tvos";
			}
		}

		public override string DotNetSdk => "Microsoft.tvOS.Sdk";
		public override string RuntimeIdentifier => "tvossimulator-x64";
		public override DevicePlatform ApplePlatform => DevicePlatform.tvOS;
		public override string TargetFramework => DotNetTfm + "-tvos";
		public override string TargetFrameworkForNuGet => "xamarintvos10";

		static Dictionary<string, string> project_guids = new Dictionary<string, string> ();

		protected override void ProcessProject ()
		{
			base.ProcessProject ();

			var srcDirectory = Path.Combine (HarnessConfiguration.RootDirectory, "..", "src");

			inputProject.AddExtraMtouchArgs ("--bitcode:asmonly", "iPhone", "Release");
			inputProject.SetMtouchUseLlvm (true, "iPhone", "Release");
			inputProject.ResolveAllPaths (TemplateProjectPath);

			// Remove bitcode from executables, since we don't need it for testing, and it makes test apps bigger (and the Apple TV might refuse to install them).
			var configurations = new string [] { "Debug", "Release" };
			foreach (var c in configurations) {
				inputProject.AddExtraMtouchArgs ($"--gcc_flags=-fembed-bitcode-marker", "iPhone", c);
			}
		}
	}
}
