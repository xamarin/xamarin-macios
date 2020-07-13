using System;
using System.Collections.Generic;
using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Hardware;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;

namespace Xharness.Targets
{
	public class MacTarget : Target
	{
		public MacFlavors Flavor { get; private set; }
		public bool Modern => Flavor == MacFlavors.Modern;
		public bool System => Flavor == MacFlavors.System;
		public bool Full => Flavor == MacFlavors.Full;

		public MacTarget (MacFlavors flavor)
		{
			Flavor = flavor;
		}

		protected override void CalculateName ()
		{
			base.CalculateName ();

			if (MonoNativeInfo != null)
				Name = Name + MonoNativeInfo.FlavorSuffix;
		}

		public override string Suffix {
			get {
				switch (Flavor) {
				case MacFlavors.Modern:
					return string.Empty;
				case MacFlavors.Full:
					return "-full";
				case MacFlavors.System:
					return "-system";
				default:
					throw new NotImplementedException ($"Suffix for {Flavor}");
				}
			}
		}

		public override string MakefileWhereSuffix {
			get {
				switch (Flavor) {
				case MacFlavors.Modern:
					return "modern";
				case MacFlavors.Full:
					return "full";
				case MacFlavors.System:
					return "system";
				default:
					throw new NotImplementedException ($"Suffix for {Flavor}");
				}
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
				var rv = "";

				if (Full)
					rv += "XAMMAC_4_5";

				return rv;
			}
		}

		protected override string RemoveDefines {
			get {
				var rv = string.Empty;

				if (!Modern)
					rv += ";XAMMAC;MOBILE";

				return rv;
			}
		}

		public override string Platform {
			get {
				return "mac";
			}
		}

		public override string DotNetSdk => "Microsoft.macOS.Sdk";
		public override string RuntimeIdentifier => "osx-x64";
		public override DevicePlatform ApplePlatform => DevicePlatform.macOS;
		public override string TargetFrameworkForNuGet => "xamarinmac10";

		public MonoNativeInfo MonoNativeInfo { get; set; }

		protected override bool FixProjectReference (string name, out string fixed_name)
		{
			fixed_name = null;
			switch (name) {
			case "GuiUnit_NET_4_5":
				if (Flavor == MacFlavors.Full || Flavor == MacFlavors.System)
					return false;
				fixed_name = "GuiUnit_xammac_mobile";
				return true;
			case "GuiUnit_xammac_mobile":
				if (Flavor == MacFlavors.Modern)
					return false;
				fixed_name = "GuiUnit_NET_4_5";
				return true;
			default:
				return base.FixProjectReference (name, out fixed_name);
			}
		}

		public string SimplifiedName {
			get {
				return Name.EndsWith ("-mac", StringComparison.Ordinal) ? Name.Substring (0, Name.Length - 4) : Name;
			}
		}

		public override string DefaultAssemblyReference { get { return "Xamarin.Mac"; } }

		public override IEnumerable<string> ReferenceToRemove { get { yield return "System.Drawing"; } }

		public override bool ShouldSetTargetFrameworkIdentifier { get { return Modern; } }

		public override Dictionary<string, string> NewPropertiesToAdd 
		{
			get 
			{
				var props = new Dictionary<string, string> ();


				if (System) {
					props.Add ("TargetFrameworkVersion", "v4.7.1");
					props.Add ("MonoBundlingExtraArgs", "--embed-mono=no");
				} else if (Modern)
				{
					props.Add ("TargetFrameworkVersion", "v2.0");
				}
				else
				{
					props.Add ("TargetFrameworkVersion", "v4.5");
					props.Add ("UseXamMacFullFramework", "true");
				}

				props.Add ("XamMacArch", "x86_64");
				return props;
			}
		}

		public override HashSet<string> PropertiesToRemove {
			get {
				if (!ShouldSetTargetFrameworkIdentifier)
					return new HashSet<string> { "TargetFrameworkIdentifier" };
				return null;
			}
		}

		protected override void PostProcessExecutableProject ()
		{
			base.PostProcessExecutableProject ();

			ProjectGuid = "{" + Helpers.GenerateStableGuid ().ToString ().ToUpper () + "}";
			inputProject.SetProjectGuid (ProjectGuid);
		}
	}
}
