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

using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;
using Microsoft.DotNet.XHarness.iOS.Shared.Hardware;

namespace Xharness {
	public enum MonoNativeFlavor
	{
		None,
		Compat,
		Unified,
	}

	public enum MonoNativeLinkMode
	{
		None,
		Static,
		Dynamic,
		Symlink,
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

		public static void RemoveSymlinkMode (XmlDocument project)
		{
			AddProjectDefines (project, MonoNativeLinkMode.Static, "iPhone", "Debug");
			AddProjectDefines (project, MonoNativeLinkMode.Static, "iPhoneSimulator", "Debug");
		}

		public static string GetMinimumOSVersion (DevicePlatform platform, MonoNativeFlavor flavor)
		{
			switch (flavor) {
			case MonoNativeFlavor.Compat:
				switch (platform) {
				case DevicePlatform.iOS:
					return "8.0";
				case DevicePlatform.tvOS:
					return "9.0";
				case DevicePlatform.watchOS:
					return "2.0";
				case DevicePlatform.macOS:
					return "10.9";
				default:
					throw new Exception ($"Unknown DevicePlatform: {platform}");
				}
			case MonoNativeFlavor.Unified:
				switch (platform) {
				case DevicePlatform.iOS:
					return "10.0";
				case DevicePlatform.tvOS:
					return "10.1"; // Can't use 10.0 due to http://openradar.appspot.com/radar?id=4966840983879680.
				case DevicePlatform.watchOS:
					return "4.0";
				case DevicePlatform.macOS:
					return "10.12";
				default:
					throw new Exception ($"Unknown DevicePlatform: {platform}");
				}
			default:
				throw new Exception ($"Unknown MonoNativeFlavor: {flavor}");
			}
		}
	}

	public class MonoNativeInfo
	{
		public Harness Harness { get; }
		public MonoNativeFlavor Flavor { get; }
		protected virtual DevicePlatform DevicePlatform {  get { return DevicePlatform.iOS; } }

		public MonoNativeInfo (Harness harness, MonoNativeFlavor flavor)
		{
			Harness = harness;
			Flavor = flavor;
		}

		public string FlavorSuffix => Flavor == MonoNativeFlavor.Compat ? "-compat" : "-unified";
		public string ProjectName => "mono-native" + FlavorSuffix;
		public string ProjectPath => Path.Combine (Harness.RootDirectory, "mono-native", TemplateName + FlavorSuffix + ".csproj");
		string TemplateName => "mono-native" + TemplateSuffix;
		public string TemplatePath => Path.Combine (Harness.RootDirectory, "mono-native", TemplateName + ".csproj.template");
		protected virtual string TemplateSuffix => string.Empty;

		public void Convert ()
		{
			var inputProject = new XmlDocument ();

			var xml = File.ReadAllText (TemplatePath);
			inputProject.LoadXmlWithoutNetworkAccess (xml);
			inputProject.SetOutputPath ("bin\\$(Platform)\\$(Configuration)" + FlavorSuffix);
			inputProject.SetIntermediateOutputPath ("obj\\$(Platform)\\$(Configuration)" + FlavorSuffix);
			inputProject.SetAssemblyName (inputProject.GetAssemblyName () + FlavorSuffix);

			var template_info_plist = Path.Combine (Path.GetDirectoryName (TemplatePath), inputProject.GetInfoPListInclude ());
			var target_info_plist = Path.Combine (Path.GetDirectoryName (template_info_plist), "Info" + TemplateSuffix + FlavorSuffix + ".plist");
			SetInfoPListMinimumOSVersion (template_info_plist, target_info_plist);
			inputProject.FixInfoPListInclude (FlavorSuffix, newName: Path.GetFileName (target_info_plist));

			AddProjectDefines (inputProject);

			Harness.Save (inputProject, ProjectPath);
		}

		public void AddProjectDefines (XmlDocument project)
		{
			MonoNativeHelper.AddProjectDefines (project, Flavor);
		}

		public XmlDocument SetInfoPListMinimumOSVersion (string template_plist, string target_plist)
		{
			var template_info_plist = template_plist;
			var info_plist = new XmlDocument ();
			info_plist.LoadWithoutNetworkAccess (template_info_plist);
			SetInfoPListMinimumOSVersion (info_plist, MonoNativeHelper.GetMinimumOSVersion (DevicePlatform, Flavor));
			Harness.Save (info_plist, target_plist);
			return info_plist;
		}

		public virtual void SetInfoPListMinimumOSVersion (XmlDocument info_plist, string version)
		{
			info_plist.SetMinimumOSVersion (version);
		}
	}

	public class MacMonoNativeInfo : MonoNativeInfo
	{
		protected override string TemplateSuffix => "-mac";
		protected override DevicePlatform DevicePlatform { get { return DevicePlatform.macOS; } }

		public MacMonoNativeInfo (Harness harness, MonoNativeFlavor flavor)
			: base (harness, flavor)
		{
		}

		public override void SetInfoPListMinimumOSVersion (XmlDocument info_plist, string version)
		{
			info_plist.SetMinimummacOSVersion (version);
		}
	}
}
