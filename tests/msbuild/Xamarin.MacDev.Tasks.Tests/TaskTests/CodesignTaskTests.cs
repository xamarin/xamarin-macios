// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.Build.Utilities;

using NUnit.Framework;

using Xamarin.Tests;
using Xamarin.Utils;

#nullable enable

namespace Xamarin.MacDev.Tasks {
	[TestFixture]
	public class CodesignTaskTests : TestBase {
		[Test]
		public void EntitlementsChangesInvalidateStamp ()
		{
			Configuration.IgnoreIfNotOnMacOS ();

			var directory = Cache.CreateTemporaryDirectory ();
			var appBundle = Path.Combine (directory, "App.app");
			var appExecutable = Path.Combine (appBundle, "App");
			var entitlements = Path.Combine (directory, "Entitlements.xcent");
			var stampFile = Path.Combine (directory, "codesign.stamp");

			Directory.CreateDirectory (appBundle);
			File.WriteAllText (appExecutable, "executable");
			File.WriteAllText (entitlements, "initial");

			var resource = CreateCodesignItem (appBundle, entitlements, stampFile);
			ExecuteCodesign (resource);
			var initialStampTimestamp = File.GetLastWriteTimeUtc (stampFile);

			ExecuteCodesign (resource);
			Assert.That (File.GetLastWriteTimeUtc (stampFile), Is.EqualTo (initialStampTimestamp), "Unchanged entitlements");

			File.WriteAllText (entitlements, "updated");
			Configuration.Touch (entitlements);
			ExecuteCodesign (resource);
			Assert.That (File.GetLastWriteTimeUtc (stampFile), Is.GreaterThan (initialStampTimestamp), "Updated entitlements");
		}

		[Test]
		public void NestedEntitlementsChangesInvalidateParentStamp ()
		{
			Configuration.IgnoreIfNotOnMacOS ();

			var directory = Cache.CreateTemporaryDirectory ();
			var appBundle = Path.Combine (directory, "App.app");
			var extensionBundle = Path.Combine (appBundle, "PlugIns", "Extension.appex");
			var appEntitlements = Path.Combine (directory, "AppEntitlements.xcent");
			var extensionEntitlements = Path.Combine (directory, "ExtensionEntitlements.xcent");
			var appStampFile = Path.Combine (directory, "app-codesign.stamp");
			var extensionStampFile = Path.Combine (directory, "extension-codesign.stamp");

			Directory.CreateDirectory (extensionBundle);
			File.WriteAllText (Path.Combine (appBundle, "App"), "app executable");
			File.WriteAllText (Path.Combine (extensionBundle, "Extension"), "extension executable");
			File.WriteAllText (appEntitlements, "app entitlements");
			File.WriteAllText (extensionEntitlements, "initial extension entitlements");

			var appResource = CreateCodesignItem (appBundle, appEntitlements, appStampFile);
			var extensionResource = CreateCodesignItem (extensionBundle, extensionEntitlements, extensionStampFile);
			ExecuteCodesign (appResource, extensionResource);
			var initialAppStampTimestamp = File.GetLastWriteTimeUtc (appStampFile);
			var initialExtensionStampTimestamp = File.GetLastWriteTimeUtc (extensionStampFile);

			ExecuteCodesign (appResource, extensionResource);
			Assert.That (File.GetLastWriteTimeUtc (appStampFile), Is.EqualTo (initialAppStampTimestamp), "Unchanged parent");
			Assert.That (File.GetLastWriteTimeUtc (extensionStampFile), Is.EqualTo (initialExtensionStampTimestamp), "Unchanged child");

			File.WriteAllText (extensionEntitlements, "updated extension entitlements");
			Configuration.Touch (extensionEntitlements);
			ExecuteCodesign (appResource, extensionResource);
			Assert.That (File.GetLastWriteTimeUtc (extensionStampFile), Is.GreaterThan (initialExtensionStampTimestamp), "Updated child");
			Assert.That (File.GetLastWriteTimeUtc (appStampFile), Is.GreaterThan (initialAppStampTimestamp), "Updated parent");
		}

		static TaskItem CreateCodesignItem (string bundle, string entitlements, string stampFile)
		{
			return new TaskItem (bundle, new Dictionary<string, string> {
				{ "CodesignEntitlements", entitlements },
				{ "CodesignStampFile", stampFile },
			});
		}

		void ExecuteCodesign (params TaskItem [] resources)
		{
			var task = CreateTask<Codesign> ();
			task.CodesignAllocate = "/usr/bin/true";
			task.Resources = resources;
			task.SigningKey = "-";
			task.TargetFrameworkMoniker = TargetFramework.DotNet_MacCatalyst_String;
			task.ToolExe = "true";
			task.ToolPath = "/usr/bin";
			ExecuteTask (task);
		}
	}
}
