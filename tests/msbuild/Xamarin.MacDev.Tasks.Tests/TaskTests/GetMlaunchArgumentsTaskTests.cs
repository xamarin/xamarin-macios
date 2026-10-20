// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO;
using System.Linq;
using System.Text;

using Microsoft.Build.Utilities;

using NUnit.Framework;

using Xamarin.Tests;
using Xamarin.Utils;

#nullable enable

namespace Xamarin.MacDev.Tasks {
	[TestFixture]
	public class GetMlaunchArgumentsTaskTests : TestBase {

		[Test]
		public void SelectSimulatorDeviceUsesFirstAvailableSimulator ()
		{
			var task = CreateTask<GetMlaunchArguments> ();
			task.TargetFrameworkMoniker = TargetFramework.GetTargetFramework (ApplePlatform.iOS).ToString ();
			task.AppManifestPath = CreateAppManifest (1, 2);
			task.Devices = CreateDevices (
				("DEVICE-1", "Connected iPhone", "Device"),
				("SIM-2", "Preferred Simulator", "Simulator"),
				("SIM-1", "Another Simulator", "Simulator")
			);
			task.LaunchApp = "MySimpleApp.app";
			task.MlaunchPath = "/usr/bin/false";
			task.SdkIsSimulator = true;
			task.SdkVersion = "26.2";
			task.WaitForExit = true;

			ExecuteTask (task);

			Assert.That (task.MlaunchArguments, Does.Contain ("--device :v2:udid=SIM-2"));
		}

		[Test]
		public void ExplicitDeviceDoesNotRequireDeviceDiscovery ()
		{
			var task = CreateTask<GetMlaunchArguments> ();
			task.TargetFrameworkMoniker = TargetFramework.GetTargetFramework (ApplePlatform.iOS).ToString ();
			task.AppManifestPath = CreateAppManifest (1, 2);
			task.DeviceName = "explicit-device";
			task.InstallApp = "MySimpleApp.app";
			task.MlaunchPath = "/usr/bin/false";

			ExecuteTask (task);

			Assert.That (task.MlaunchArguments, Does.Contain ("--devname explicit-device"));
		}

		[Test]
		public void ErrorsIfDevicesItemGroupIsEmpty ()
		{
			var task = CreateTask<GetMlaunchArguments> ();
			task.TargetFrameworkMoniker = TargetFramework.GetTargetFramework (ApplePlatform.iOS).ToString ();
			task.AppManifestPath = CreateAppManifest (1, 2);
			task.Devices = CreateDevices (
				("DEVICE-2", "Connected iPhone", "Device")
			);
			task.DiscardedDevices = CreateDiscardedDevices (
				("SIM-1", "Unsupported Simulator", "Simulator", "Device is not an iPad, but the app only supports iPads"),
				("DEVICE-1", "Old Phone", "Device", "Device OS version '17.0' is lower than the app's minimum OS version '18.0'")
			);
			task.LaunchApp = "MySimpleApp.app";
			task.MlaunchPath = "/usr/bin/false";
			task.SdkIsSimulator = true;
			task.SdkVersion = "26.2";
			task.WaitForExit = true;

			ExecuteTask (task, expectedErrorCount: 1);

			Assert.That (Engine.Logger.ErrorEvents [0].Message, Does.Contain ("No applicable and available devices found."));
			Assert.That (Engine.Logger.ErrorEvents [0].Message, Does.Contain ("Unsupported Simulator (SIM-1): Device is not an iPad, but the app only supports iPads"));
			Assert.That (Engine.Logger.ErrorEvents [0].Message, Does.Not.Contain ("Connected iPhone"));
			Assert.That (Engine.Logger.ErrorEvents [0].Message, Does.Not.Contain ("Old Phone"));
		}

		[Test]
		public void HelpListsDiscardedDevicesWhenNoDevicesAreAvailable ()
		{
			var task = CreateTask<GetMlaunchArguments> ();
			task.TargetFrameworkMoniker = TargetFramework.GetTargetFramework (ApplePlatform.iOS).ToString ();
			task.AppManifestPath = CreateAppManifest (1, 2);
			task.Devices = CreateDevices (
				("DEVICE-2", "Connected iPhone", "Device")
			);
			task.DiscardedDevices = CreateDiscardedDevices (
				("SIM-1", "Unsupported Simulator", "Simulator", "Device is not an iPad, but the app only supports iPads"),
				("DEVICE-1", "Old Phone", "Device", "Device OS version '17.0' is lower than the app's minimum OS version '18.0'")
			);
			task.Help = "true";
			task.MlaunchPath = "/usr/bin/false";
			task.SdkIsSimulator = true;
			task.SdkVersion = "26.2";

			ExecuteTask (task);

			Assert.That (Engine.Logger.WarningsEvents [0].Message, Does.Contain ("The following devices were discarded:"));
			Assert.That (Engine.Logger.WarningsEvents [0].Message, Does.Contain ("Unsupported Simulator (SIM-1): Device is not an iPad, but the app only supports iPads"));
			Assert.That (Engine.Logger.WarningsEvents [0].Message, Does.Contain ("Connected iPhone"));
			Assert.That (Engine.Logger.WarningsEvents [0].Message, Does.Contain ("Old Phone (DEVICE-1): Device OS version '17.0' is lower than the app's minimum OS version '18.0'"));
		}

		static TaskItem [] CreateDevices (params (string Udid, string Name, string Type) [] devices)
		{
			return devices.Select (v => {
				return CreateDevice (v.Udid, v.Name, v.Type);
			}).ToArray ();
		}

		static TaskItem [] CreateDiscardedDevices (params (string Udid, string Name, string Type, string DiscardedReason) [] devices)
		{
			return devices.Select (v => CreateDevice (v.Udid, v.Name, v.Type, v.DiscardedReason)).ToArray ();
		}

		static TaskItem CreateDevice (string udid, string name, string type, string? discardedReason = null)
		{
			var item = new TaskItem (udid);
			item.SetMetadata ("Description", name);
			item.SetMetadata ("Name", name);
			item.SetMetadata ("Type", type);
			item.SetMetadata ("UDID", udid);
			if (!string.IsNullOrEmpty (discardedReason))
				item.SetMetadata ("DiscardedReason", discardedReason);
			return item;
		}

		static string CreateAppManifest (params int [] deviceFamilies)
		{
			var appManifestPath = Path.Combine (Cache.CreateTemporaryDirectory ("msbuild-tests"), "Info.plist");
			var plist = new StringBuilder ();
			plist.AppendLine (@"<?xml version=""1.0"" encoding=""UTF-8""?>");
			plist.AppendLine (@"<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">");
			plist.AppendLine (@"<plist version=""1.0"">");
			plist.AppendLine ("<dict>");
			plist.AppendLine ("\t<key>UIDeviceFamily</key>");
			plist.AppendLine ("\t<array>");
			foreach (var family in deviceFamilies)
				plist.AppendLine ($"\t\t<integer>{family}</integer>");
			plist.AppendLine ("\t</array>");
			plist.AppendLine ("</dict>");
			plist.AppendLine ("</plist>");
			File.WriteAllText (appManifestPath, plist.ToString ());
			return appManifestPath;
		}
	}
}
