using System;
using System.Collections.Generic;

namespace xharness
{
	public class MacUnifiedTarget : MacTarget
	{
		public bool Mobile { get; private set; }

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
				string suffix = (Mobile ? "" : "XM45") + (ThirtyTwoBit ? "-32" : "");
				return "-unified" + (IsBCL ? "" : suffix);
			}
		}

		public override string MakefileWhereSuffix {
			get {
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


				if (Mobile)
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
	}
}
