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

			ExecuteCodesign (appBundle, entitlements, stampFile);
			var initialStampTimestamp = File.GetLastWriteTimeUtc (stampFile);

			ExecuteCodesign (appBundle, entitlements, stampFile);
			Assert.That (File.GetLastWriteTimeUtc (stampFile), Is.EqualTo (initialStampTimestamp), "Unchanged entitlements");

			File.WriteAllText (entitlements, "updated");
			Configuration.Touch (entitlements);
			ExecuteCodesign (appBundle, entitlements, stampFile);
			Assert.That (File.GetLastWriteTimeUtc (stampFile), Is.GreaterThan (initialStampTimestamp), "Updated entitlements");
		}

		void ExecuteCodesign (string appBundle, string entitlements, string stampFile)
		{
			var resource = new TaskItem (appBundle, new Dictionary<string, string> {
				{ "CodesignEntitlements", entitlements },
				{ "CodesignStampFile", stampFile },
			});
			var task = CreateTask<Codesign> ();
			task.CodesignAllocate = "/usr/bin/true";
			task.Resources = [resource];
			task.SigningKey = "-";
			task.TargetFrameworkMoniker = TargetFramework.DotNet_MacCatalyst_String;
			task.ToolExe = "true";
			task.ToolPath = "/usr/bin";
			ExecuteTask (task);
		}
	}
}
