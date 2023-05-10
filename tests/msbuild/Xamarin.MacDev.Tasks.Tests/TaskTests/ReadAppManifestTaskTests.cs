#nullable enable

using System;
using System.IO;
using System.Linq;
using Microsoft.Build.Utilities;
using NUnit.Framework;

using Xamarin.iOS.Tasks;
using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	[TestFixture]
	public class ReadAppManifestTaskTests : TestBase {
		ReadAppManifest CreateTask (ApplePlatform platform = ApplePlatform.iOS, Action<PDictionary>? createDictionary = null)
		{
			var tmpdir = Cache.CreateTemporaryDirectory ();

			var plistPath = Path.Combine (tmpdir, "TemporaryAppManifest.plist");
			var plist = new PDictionary ();
			if (createDictionary is not null)
				createDictionary (plist);
			plist.Save (plistPath);

			var task = CreateTask<ReadAppManifest> ();
			task.AppManifest = new TaskItem (plistPath);
			task.TargetFrameworkMoniker = TargetFramework.GetTargetFramework (platform, true).ToString ();

			return task;
		}

		[Test]
		public void MacCatalystVersionConversion ()
		{
			var task = CreateTask (platform: ApplePlatform.MacCatalyst, (plist) => {
				plist.SetMinimumSystemVersion ("10.15.2");
			});
			ExecuteTask (task);
			Assert.AreEqual ("13.3", task.MinimumOSVersion, "MinimumOSVersion");
		}

		[Test]
		public void MacCatalystVersionConversionError ()
		{
			var task = CreateTask (platform: ApplePlatform.MacCatalyst, (plist) => {
				plist.SetMinimumSystemVersion ("10.0");
			});
			ExecuteTask (task, expectedErrorCount: 1);
			Assert.That (Engine.Logger.ErrorEvents [0].Message, Does.StartWith ("Could not map the macOS version 10.0 to a corresponding Mac Catalyst version. Valid macOS versions are:"));
		}
	}
}
