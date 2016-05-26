using System;
using System.Collections.Generic;

namespace xharness
{
	public class MacUnifiedTarget : MacTarget
	{
		public bool Mobile { get; private set; }

		bool SkipProjectGeneration;

		public MacUnifiedTarget (bool mobile, bool shouldSkipProjectGeneration = false) : base ()
		{
			Mobile = mobile;
			SkipProjectGeneration = shouldSkipProjectGeneration;
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
				return "-unified" + (Mobile ? "" : "XM45");
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

		public override string MakefileWhereSuffix {
			get {
				return "unified" + (Mobile ? "" : "XM45");
			}
		}

		public override string DefaultAssemblyReference { get { return "XamMac"; } }

		public override IEnumerable<string> ReferenceToRemove { get { yield return "System.Drawing"; } }

		public override bool ShouldSetTargetFrameworkIdentifier { get { return Mobile; } }

		public override Dictionary<string, string> NewPropertiesToAdd 
		{
			get 
			{
				if (Mobile)
					return new Dictionary<string, string> () { { "TargetFrameworkVersion", "v2.0" } };
				else
					return new Dictionary<string, string> () { { "TargetFrameworkVersion", "v4.5" }, { "UseXamMacFullFramework", "true" } };
			}
		}
	}
}
