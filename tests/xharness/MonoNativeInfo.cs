//
// MonoNativeInfo.cs
//
// Author:
//       Martin Baulig <mabaul@microsoft.com>
//
// Copyright (c) 2018 Xamarin Inc. (http://www.xamarin.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.IO;
using System.Xml;

namespace xharness
{
	public enum MonoNativeFlavor
	{
		None,
		Compat,
		Unified
	}

	public enum MonoNativeLinkMode
	{
		None,
		Static,
		Dynamic,
		Symlink
	}

	public static class MonoNativeHelper
	{
		public static void AddProjectDefines (
			XmlDocument project, MonoNativeFlavor flavor, MonoNativeLinkMode link,
			string platform, string config)
		{
			AddProjectDefines (project, flavor, platform, config);
			AddProjectDefines (project, link, platform, config);
		}

		public static void AddProjectDefines (
			XmlDocument project, MonoNativeFlavor flavor,
			string platform = null, string config = null)
		{
			switch (flavor) {
			case MonoNativeFlavor.Compat:
				if (platform != null)
					project.AddAdditionalDefines ("MONO_NATIVE_COMPAT", platform, config);
				else
					project.AddAdditionalDefines ("MONO_NATIVE_COMPAT");
				break;
			case MonoNativeFlavor.Unified:
				if (platform != null)
					project.AddAdditionalDefines ("MONO_NATIVE_UNIFIED", platform, config);
				else
					project.AddAdditionalDefines ("MONO_NATIVE_UNIFIED");
				break;
			default:
				throw new Exception ($"Unknown MonoNativeFlavor: {flavor}");
			}
		}

		public static void AddProjectDefines (
			XmlDocument project, MonoNativeLinkMode link,
			string platform, string config)
		{
			switch (link) {
			case MonoNativeLinkMode.Static:
				project.AddAdditionalDefines ("MONO_NATIVE_STATIC", platform, config);
				project.RemoveDefines ("MONO_NATIVE_DYNAMIC; MONO_NATIVE_SYMLINK", platform, config);
				break;
			case MonoNativeLinkMode.Dynamic:
				project.AddAdditionalDefines ("MONO_NATIVE_DYNAMIC", platform, config);
				project.RemoveDefines ("MONO_NATIVE_STATIC; MONO_NATIVE_SYMLINK", platform, config);
				break;
			case MonoNativeLinkMode.Symlink:
				project.AddAdditionalDefines ("MONO_NATIVE_SYMLINK", platform, config);
				project.RemoveDefines ("MONO_NATIVE_MONO_NATIVE_STATIC; MONO_NATIVE_DYNAMIC", platform, config);
				break;
			default:
				throw new Exception ($"Unknown MonoNativeLinkMode: {link}");
			}
		}
	}

	public class MonoNativeInfo
	{
		public Harness Harness { get; }
		public MonoNativeFlavor Flavor { get; }

		public MonoNativeInfo (Harness harness, MonoNativeFlavor flavor)
		{
			Harness = harness;
			Flavor = flavor;
		}

		string NativeFlavorSuffix => Flavor == MonoNativeFlavor.Compat ? "-compat" : "-unified";
		public virtual string FlavorSuffix => NativeFlavorSuffix;
		public string ProjectName => "mono-native" + NativeFlavorSuffix;
		public string ProjectPath => Path.Combine (Harness.RootDirectory, "mono-native", "mono-native" + FlavorSuffix + ".csproj");
		public string TemplatePath => Path.Combine (Harness.RootDirectory, "mono-native", "mono-native" + (Harness.Mac ? "-mac" : string.Empty) + ".csproj.template");

		public void Convert ()
		{
			var inputProject = new XmlDocument ();

			var xml = File.ReadAllText (TemplatePath);
			inputProject.LoadXmlWithoutNetworkAccess (xml);
			inputProject.SetOutputPath ("bin\\$(Platform)\\$(Configuration)" + FlavorSuffix);
			inputProject.SetIntermediateOutputPath ("obj\\$(Platform)\\$(Configuration)" + FlavorSuffix);
			inputProject.SetAssemblyName (inputProject.GetAssemblyName () + FlavorSuffix);

			AddProjectDefines (inputProject);

			Convert (inputProject);

			Console.Error.WriteLine ($"CONVERT: {Harness.Mac} {ProjectPath}");

			Harness.Save (inputProject, ProjectPath);
		}

		protected virtual void Convert (XmlDocument inputProject)
		{
		}

		public void AddProjectDefines (XmlDocument project)
		{
			MonoNativeHelper.AddProjectDefines (project, Flavor);
			return;

			switch (Flavor) {
			case MonoNativeFlavor.Compat:
				project.AddAdditionalDefines ("MONO_NATIVE_COMPAT");
				break;
			case MonoNativeFlavor.Unified:
				project.AddAdditionalDefines ("MONO_NATIVE_UNIFIED");
				break;
			default:
				throw new Exception ($"Unknown MonoNativeFlavor: {Flavor}");
			}
		}
	}

	public class MacMonoNativeInfo : MonoNativeInfo
	{
		public MacFlavors MacFlavor { get; set; }

		public override string FlavorSuffix => base.FlavorSuffix + (MacFlavor == MacFlavors.Full ? "-full" : "-modern");

		public MacMonoNativeInfo (Harness harness, MonoNativeFlavor flavor, MacFlavors macFlavor)
			: base (harness, flavor)
		{
			if (macFlavor == MacFlavors.All)
				throw new ArgumentException ("Each target must be a specific flavor");

			MacFlavor = macFlavor;
		}

		protected override void Convert (XmlDocument inputProject)
		{
			switch (MacFlavor) {
			case MacFlavors.Modern:
				inputProject.SetTargetFrameworkIdentifier ("Xamarin.Mac");
				inputProject.SetTargetFrameworkVersion ("v2.0");
				inputProject.RemoveNode ("UseXamMacFullFramework");
				inputProject.AddAdditionalDefines ("MOBILE;XAMMAC");
				inputProject.AddReference ("Mono.Security");
				break;
			case MacFlavors.Full:
				inputProject.AddAdditionalDefines ("XAMMAC_4_5");
				break;
			}

			base.Convert (inputProject);
		}
	}
}
