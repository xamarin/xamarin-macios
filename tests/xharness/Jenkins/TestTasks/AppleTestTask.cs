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
			case TestPlatform.tvOS:
			case TestPlatform.MacCatalyst:
				process.StartInfo.EnvironmentVariables ["MD_APPLE_SDK_ROOT"] = xcodeRoot;
				break;
			case TestPlatform.Mac:
				process.StartInfo.EnvironmentVariables ["MD_APPLE_SDK_ROOT"] = xcodeRoot;
				break;
			case TestPlatform.All:
				// Don't set:
				//     MSBuildExtensionsPath 
				//     TargetFrameworkFallbackSearchPaths
				// because these values used by both XM and XI and we can't set it to two different values at the same time.
				// Any test that depends on these values should not be using 'TestPlatform.All'
				process.StartInfo.EnvironmentVariables ["MD_APPLE_SDK_ROOT"] = xcodeRoot;
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
