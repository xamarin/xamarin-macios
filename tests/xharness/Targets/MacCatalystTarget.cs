using System;
using System.IO;
using System.Xml;

using Microsoft.DotNet.XHarness.iOS.Shared.Hardware;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;

namespace Xharness.Targets {
	public class MacCatalystTarget : UnifiedTarget {
		public override string DotNetSdk => throw new NotImplementedException ();

		public override string RuntimeIdentifier => throw new NotImplementedException ();

		public override DevicePlatform ApplePlatform => throw new NotImplementedException ();

		public override string PlatformString => "MacCatalyst";

		public override string TargetFramework => "net6.0-maccatalyst";

		public override string TargetFrameworkForNuGet => throw new NotImplementedException ();

		public override string Suffix {
			get {
				return "-maccatalyst";
			}
		}

		public override string ExtraLinkerDefsSuffix {
			get {
				return "-maccatalyst";
			}
		}

		public override string ProjectFileSuffix {
			get {
				return Suffix;
			}
		}

		protected override string TargetFrameworkIdentifier {
			get {
				return "Xamarin.MacCatalyst";
			}
		}

		protected override string Imports {
			get {
				return IsFSharp ? "MacCatalyst\\Xamarin.MacCatalyst.FSharp.targets" : "MacCatalyst\\Xamarin.MacCatalyst.CSharp.targets";
			}
		}

		protected override string BindingsImports {
			get {
				return IsFSharp ? "MacCatalyst\\Xamarin.MacCatalyst.ObjCBinding.FSharp.targets" : "MacCatalyst\\Xamarin.MacCatalyst.ObjCBinding.CSharp.targets";
			}
		}

		public override string SimulatorArchitectures {
			get {
				return "x86_64";
			}
		}

		public override string DeviceArchitectures {
			get {
				return "NotApplicable";
			}
		}

		protected override string GetMinimumOSVersion (string templateMinimumOSVersion)
		{
			return "10.15";
		}

		protected override int [] UIDeviceFamily {
			get {
				return new int [] { 2 };
			}
		}

		protected override string AdditionalDefines {
			get {
				return "XAMCORE_3_0;__MACCATALYST__";
			}
		}

		public override string Platform {
			get {
				return "maccatalyst";
			}
		}

		protected override bool SupportsBitcode {
			get {
				return false;
			}
		}

		protected override void ProcessProject ()
		{
			base.ProcessProject ();

			inputProject.ResolveAllPaths (TemplateProjectPath);

			// I couldn't figure out why the NUnit package reference doesn't end up referencing nunit.framework.dll, so instead reference nunit.framework.dll directly
			var itemGroup = inputProject.SelectNodes ("//*[local-name() = 'ItemGroup' and not(@Condition)]") [0];
			var xmlElement = AddElement (inputProject, itemGroup, "PackageReference");
			AddAttribute (inputProject, xmlElement, "Include", "NUnit");
			AddAttribute (inputProject, xmlElement, "Version", "3.12.0");
			AddAttribute (inputProject, xmlElement, "GeneratePathProperty", "true");

			xmlElement = AddElement (inputProject, itemGroup, "Reference");
			AddAttribute (inputProject, xmlElement, "Include", "$(PkgNUnit)\\lib\\netstandard2.0\\nunit.framework.dll");
		}

		static XmlElement AddElement (XmlDocument doc, XmlNode parent, string element)
		{
			var xmlElement = doc.CreateElement (element, doc.GetNamespace ());
			parent.AppendChild (xmlElement);
			return xmlElement;
		}

		static void AddAttribute (XmlDocument doc, XmlNode node, string attribute, string value)
		{
			var attrib = doc.CreateAttribute (attribute);
			attrib.Value = value;
			node.Attributes.Append (attrib);
		}

		protected override void UpdateInfoPList ()
		{
			var info_plist = new XmlDocument ();
			var target_info_plist = Path.Combine (TargetDirectory, "Info" + Suffix + ".plist");
			info_plist.LoadWithoutNetworkAccess (Path.Combine (TargetDirectory, OriginalInfoPListInclude));
			BundleIdentifier = info_plist.GetCFBundleIdentifier ();

			// Remove MinimumOSVersion
			var node = info_plist.SelectSingleNode ("//dict/key[text()='MinimumOSVersion']");
			node.ParentNode.RemoveChild (node.NextSibling);
			node.ParentNode.RemoveChild (node);

			// Add LSMinimumSystemVersion
			info_plist.SetMinimummacOSVersion (GetMinimumOSVersion (null));

			info_plist.SetUIDeviceFamily (UIDeviceFamily);
			info_plist.Save (target_info_plist, Harness);
		}
	}
}
