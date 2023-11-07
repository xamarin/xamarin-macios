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
	public enum MonoNativeLinkMode {
		None,
		Static,
		Dynamic,
		Symlink,
	}

	public static class MonoNativeHelper {
		public static void AddProjectDefines (XmlDocument project, MonoNativeLinkMode link)
		{
			switch (link) {
			case MonoNativeLinkMode.Static:
				project.AddTopLevelProperty ("MonoNativeMode", "MONO_NATIVE_STATIC");
				break;
			case MonoNativeLinkMode.Dynamic:
				project.AddTopLevelProperty ("MonoNativeMode", "MONO_NATIVE_DYNAMIC");
				break;
			case MonoNativeLinkMode.Symlink:
				project.AddTopLevelProperty ("MonoNativeMode", "MONO_NATIVE_SYMLINK");
				break;
			default:
				throw new Exception ($"Unknown MonoNativeLinkMode: {link}");
			}
		}

		public static string GetMinimumOSVersion (DevicePlatform platform)
		{
			switch (platform) {
			case DevicePlatform.iOS:
				return Xamarin.SdkVersions.MiniOS;
			case DevicePlatform.tvOS:
				return Xamarin.SdkVersions.MinTVOS;
			case DevicePlatform.watchOS:
				return Xamarin.SdkVersions.MinWatchOS;
			case DevicePlatform.macOS:
				return Xamarin.SdkVersions.MinOSX;
			default:
				throw new Exception ($"Unknown DevicePlatform: {platform}");
			}
		}
	}

	public class MonoNativeInfo {
		Action<int, string> log;
		public DevicePlatform DevicePlatform { get; set; }
		string rootDirectory;

		public MonoNativeInfo (DevicePlatform platform, string rootDirectory, Action<int, string> logAction = null)
		{
			DevicePlatform = platform;
			this.log = logAction;
			this.rootDirectory = rootDirectory ?? throw new ArgumentNullException (nameof (rootDirectory));
		}

		public string FlavorSuffix => "-unified";
		public string ProjectName => "mono-native" + FlavorSuffix;
		public string ProjectPath => Path.Combine (rootDirectory, "mono-native", DevicePlatform.ToString (), FlavorSuffix.TrimStart ('-'), TemplateName + FlavorSuffix + ".csproj");
		string TemplateName => "mono-native";
		public string TemplatePath => Path.Combine (rootDirectory, "mono-native", DevicePlatform.ToString (), TemplateName + ".csproj.template");

		public void Convert ()
		{
			var inputProject = new XmlDocument ();

			var xml = File.ReadAllText (TemplatePath);
			inputProject.LoadXmlWithoutNetworkAccess (xml);
			inputProject.SetAssemblyName (inputProject.GetAssemblyName () + FlavorSuffix);
			inputProject.AddAdditionalDefines ("MONO_NATIVE_UNIFIED");
			inputProject.ResolveAllPaths (TemplatePath);

			var template_info_plist = HarnessConfiguration.EvaluateRootTestsDirectory (inputProject.GetInfoPListInclude ().Replace ('\\', '/'));
			var target_info_plist = Path.Combine (Path.GetDirectoryName (ProjectPath), "Info" + FlavorSuffix + ".plist");
			SetInfoPListMinimumOSVersion (template_info_plist, target_info_plist);
			target_info_plist = HarnessConfiguration.InjectRootTestsDirectory (target_info_plist);
			inputProject.FixInfoPListInclude (FlavorSuffix, newName: target_info_plist);

			inputProject.Save (ProjectPath, log);
		}

		public XmlDocument SetInfoPListMinimumOSVersion (string template_plist, string target_plist)
		{
			var template_info_plist = template_plist;
			var info_plist = new XmlDocument ();
			info_plist.LoadWithoutNetworkAccess (template_info_plist);
			SetInfoPListMinimumOSVersion (info_plist, MonoNativeHelper.GetMinimumOSVersion (DevicePlatform));
			info_plist.Save (target_plist, log);
			return info_plist;
		}

		public virtual void SetInfoPListMinimumOSVersion (XmlDocument info_plist, string version)
		{
			if (DevicePlatform == DevicePlatform.macOS) {
				info_plist.SetMinimummacOSVersion (version);
			} else {
				info_plist.SetMinimumOSVersion (version);
			}
		}
	}
}
