// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.Build.Utilities;

using NUnit.Framework;

using Xamarin.Tests;
using Xamarin.Utils;

#nullable enable

namespace Xamarin.MacDev.Tasks {
	[TestFixture]
	public class CodesignTaskTests : TestBase {
		[TestCase ("resource fork, Finder information, or similar detritus not allowed", 1, 1)]
		[TestCase ("resource fork, Finder information, or similar detritus not allowed", 1, 2)]
		[TestCase ("resource fork, Finder information, or similar detritus not allowed", 0, 1)]
		[TestCase ("unrelated signing failure", 1, 1)]
		[TestCase ("", 1, 1)]
		public void ExtendedAttributesDiagnostic (string error, int exitCode, int resourceCount)
		{
			Configuration.IgnoreIfNotOnMacOS ();

			var directory = Cache.CreateTemporaryDirectory ();
			var tool = Path.Combine (directory, "codesign");
			File.WriteAllText (tool, $"#!/bin/sh\nprintf '%s' '{error}' >&2\nexit {exitCode}\n");
			Assert.That (ExecutionHelper.Execute ("/bin/chmod", new [] { "+x", tool }, out var output), Is.EqualTo (0), output.ToString ());

			var resources = new TaskItem [resourceCount];
			for (var i = 0; i < resourceCount; i++) {
				var appBundle = Path.Combine (directory, $"App {i}.app");
				Directory.CreateDirectory (appBundle);
				File.WriteAllText (Path.Combine (appBundle, "App"), "executable");
				resources [i] = new TaskItem (appBundle, new Dictionary<string, string> {
					{ "CodesignStampFile", Path.Combine (directory, $"codesign-{i}.stamp") },
				});
			}

			var task = CreateTask<Codesign> ();
			task.CodesignAllocate = "/usr/bin/true";
			task.Resources = resources;
			task.SigningKey = "-";
			task.TargetFrameworkMoniker = TargetFramework.DotNet_MacCatalyst_String;
			task.ToolPath = directory;

			var expectHint = exitCode != 0 && error.Contains ("resource fork");
			var expectedErrorCount = exitCode == 0 ? 0 : resourceCount * (expectHint ? 3 : 2);
			ExecuteTask (task, expectedErrorCount: expectedErrorCount);

			var hints = Engine.Logger.ErrorEvents.Where (e => e.Message?.Contains ("com.apple.FinderInfo") == true).ToArray ();
			Assert.That (hints.Length, Is.EqualTo (expectHint ? resourceCount : 0), "Extended attributes diagnostics");
			foreach (var resource in resources) {
				if (expectHint) {
					var hint = hints.Single (e => e.Message?.Contains (resource.ItemSpec) == true);
					Assert.That (hint.Message, Does.Contain ("com.apple.ResourceFork"), "Resource fork attribute");
					Assert.That (hint.Message, Does.Contain ($"xattr -lr \"{resource.ItemSpec}\""), "Locate affected files");
					Assert.That (hint.Message, Does.Contain ("xattr -cr"), "Remove attributes");
					Assert.That (hint.Message, Does.Contain ("original source files"), "Fix source files");
					Assert.That (hint.Message, Does.Contain ("https://developer.apple.com/library/archive/qa/qa1940/_index.html"), "Documentation");
				}
				Assert.That (File.Exists (resource.GetMetadata ("CodesignStampFile")), Is.EqualTo (exitCode == 0), "Stamp file");
			}
			if (exitCode != 0 && error.Length > 0)
				Assert.That (Engine.Logger.ErrorEvents.Count (e => e.Message?.Contains (error) == true), Is.EqualTo (resourceCount * 2), "Original codesign errors");
		}

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
