using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

using Microsoft.DotNet.XHarness.Common.Logging;
#nullable enable

namespace Xharness.Jenkins.TestTasks {
	abstract class AppleTestTask : TestTasks {
		public Jenkins Jenkins { get; private set; }
		public IHarness Harness { get { return Jenkins.Harness; } }
		public override string RootDirectory => HarnessConfiguration.RootDirectory;

		public override IResourceManager ResourceManager => Jenkins.ResourceManager;

		public override string LogDirectory {
			get {
				var rv = Path.Combine (Jenkins.LogDirectory, TestName.Replace (" ", "-"), ID.ToString ());
				Directory.CreateDirectory (rv);
				return rv;
			}
		}

		public AppleTestTask (Jenkins jenkins)
		{
			Jenkins = jenkins ?? throw new ArgumentNullException (nameof (jenkins));
		}

		public override void GenerateReport () => Jenkins.GenerateReport ();

		protected override void WriteLineToRunnerLog (string message) => Harness.HarnessLog.WriteLine (message);

		public override void SetEnvironmentVariables (Process process)
		{
			var xcodeRoot = Jenkins.Harness.XcodeRoot;

			process.StartInfo.EnvironmentVariables ["RootTestsDirectory"] = HarnessConfiguration.RootDirectory;

			switch (Platform) {
			case TestPlatform.iOS:
			case TestPlatform.iOS_Unified:
			case TestPlatform.iOS_Unified32:
			case TestPlatform.iOS_Unified64:
			case TestPlatform.iOS_TodayExtension64:
			case TestPlatform.tvOS:
			case TestPlatform.watchOS:
			case TestPlatform.watchOS_32:
			case TestPlatform.watchOS_64_32:
			case TestPlatform.MacCatalyst:
				process.StartInfo.EnvironmentVariables ["MD_APPLE_SDK_ROOT"] = xcodeRoot;
				process.StartInfo.EnvironmentVariables ["MD_MTOUCH_SDK_ROOT"] = Path.Combine (Jenkins.Harness.IOS_DESTDIR, "Library", "Frameworks", "Xamarin.iOS.framework", "Versions", "Current");
				process.StartInfo.EnvironmentVariables ["TargetFrameworkFallbackSearchPaths"] = Path.Combine (Jenkins.Harness.IOS_DESTDIR, "Library", "Frameworks", "Mono.framework", "External", "xbuild-frameworks");
				process.StartInfo.EnvironmentVariables ["MSBuildExtensionsPathFallbackPathsOverride"] = Path.Combine (Jenkins.Harness.IOS_DESTDIR, "Library", "Frameworks", "Mono.framework", "External", "xbuild");
				break;
			case TestPlatform.Mac:
			case TestPlatform.Mac_Modern:
			case TestPlatform.Mac_Full:
			case TestPlatform.Mac_System:
				process.StartInfo.EnvironmentVariables ["MD_APPLE_SDK_ROOT"] = xcodeRoot;
				process.StartInfo.EnvironmentVariables ["TargetFrameworkFallbackSearchPaths"] = Path.Combine (Jenkins.Harness.MAC_DESTDIR, "Library", "Frameworks", "Mono.framework", "External", "xbuild-frameworks");
				process.StartInfo.EnvironmentVariables ["MSBuildExtensionsPathFallbackPathsOverride"] = Path.Combine (Jenkins.Harness.MAC_DESTDIR, "Library", "Frameworks", "Mono.framework", "External", "xbuild");
				process.StartInfo.EnvironmentVariables ["XamarinMacFrameworkRoot"] = Path.Combine (Jenkins.Harness.MAC_DESTDIR, "Library", "Frameworks", "Xamarin.Mac.framework", "Versions", "Current");
				process.StartInfo.EnvironmentVariables ["XAMMAC_FRAMEWORK_PATH"] = Path.Combine (Jenkins.Harness.MAC_DESTDIR, "Library", "Frameworks", "Xamarin.Mac.framework", "Versions", "Current");
				break;
			case TestPlatform.All:
				// Don't set:
				//     MSBuildExtensionsPath 
				//     TargetFrameworkFallbackSearchPaths
				// because these values used by both XM and XI and we can't set it to two different values at the same time.
				// Any test that depends on these values should not be using 'TestPlatform.All'
				process.StartInfo.EnvironmentVariables ["MD_APPLE_SDK_ROOT"] = xcodeRoot;
				process.StartInfo.EnvironmentVariables ["MD_MTOUCH_SDK_ROOT"] = Path.Combine (Jenkins.Harness.IOS_DESTDIR, "Library", "Frameworks", "Xamarin.iOS.framework", "Versions", "Current");
				process.StartInfo.EnvironmentVariables ["XamarinMacFrameworkRoot"] = Path.Combine (Jenkins.Harness.MAC_DESTDIR, "Library", "Frameworks", "Xamarin.Mac.framework", "Versions", "Current");
				process.StartInfo.EnvironmentVariables ["XAMMAC_FRAMEWORK_PATH"] = Path.Combine (Jenkins.Harness.MAC_DESTDIR, "Library", "Frameworks", "Xamarin.Mac.framework", "Versions", "Current");
				break;
			default:
				throw new NotImplementedException ($"Unknown test platform: {Platform}");
			}

			foreach (var kvp in Environment) {
				if (kvp.Value is null) {
					process.StartInfo.EnvironmentVariables.Remove (kvp.Key);
				} else {
					process.StartInfo.EnvironmentVariables [kvp.Key] = kvp.Value;
				}
			}
		}

		public override void LogEvent (ILog log, string text, params object [] args)
		{
			base.LogEvent (log, text, args);
			Jenkins.MainLog.WriteLine (text, args);
		}

		protected override Task<IAcquiredResource> NotifyAndAcquireDesktopResourceAsync ()
		{
			return NotifyBlockingWaitAsync (SupportsParallelExecution ? ResourceManager.DesktopResource.AcquireConcurrentAsync () : ResourceManager.DesktopResource.AcquireExclusiveAsync ());
		}

	}
}
