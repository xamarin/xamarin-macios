using System.Linq;
using System.Xml.Linq;

using Xamarin.MacDev;
using Xamarin.MacDev.Models;
using Xamarin.Tests;
using Xamarin.Utils;

#nullable enable

namespace Xamarin.Tests {
	[TestFixture]
	public class DotNetTestTest : TestBaseClass {
		[Test]
		[TestCase (ApplePlatform.iOS, "iostest", false)]
		[TestCase (ApplePlatform.iOS, "iostest", true)]
		[TestCase (ApplePlatform.TVOS, "tvostest", false)]
		[TestCase (ApplePlatform.TVOS, "tvostest", true)]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalysttest", false)]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalysttest", true)]
		[TestCase (ApplePlatform.MacOSX, "macostest", false)]
		[TestCase (ApplePlatform.MacOSX, "macostest", true)]
		public void DotNetTest (ApplePlatform platform, string template, bool fail)
		{
			Configuration.IgnoreIfIgnoredPlatform (platform);

			var tmpDir = Cache.CreateTemporaryDirectory ();
			var outputDir = Path.Combine (tmpDir, template);
			DotNet.AssertNew (outputDir, template);
			var proj = Path.Combine (outputDir, $"{template}.csproj");

			var isDesktop = platform == ApplePlatform.MacOSX || platform == ApplePlatform.MacCatalyst;
			var log = ConsoleLogger.Instance;
			var simService = new SimulatorService (log);
			SimulatorDeviceInfo? device = null;

			if (!isDesktop) {
				// Boot a simulator so that ComputeRunArguments can find a device
				var runtimeService = new RuntimeService (log);
				device = GetOrCreateDevice (platform, simService, runtimeService);

				if (!device.IsBooted)
					Assert.That (simService.Boot (device.Udid), Is.True, $"Failed to boot simulator {device.Udid}.");
			}

			try {
				// Replace generated tests with a single test
				var testFile = Path.Combine (outputDir, "Test1.cs");
				File.WriteAllText (testFile, $@"namespace {template};

[TestClass]
public sealed class Test1 {{
	[TestMethod]
	public void TestMethod1 ()
	{{
		{(fail ? "Assert.Fail (\"Expected test failure\");" : "")}
	}}
}}
");

				// Run 'dotnet test' directly using Execution.RunAsync.
				// dotnet test's MTP flow doesn't forward /p: properties to its internal
				// ComputeRunArguments MSBuild API call, so pass properties as environment
				// variables instead — MSBuild picks those up as global properties in the
				// child process.
				var env = new Dictionary<string, string?> ();
				env ["MSBuildSDKsPath"] = null;
				env ["MSBUILD_EXE_PATH"] = null;
				env ["UseFloatingTargetPlatformVersion"] = "true";
				var binlog = Path.Combine (outputDir, "log-test.binlog");
				var testArgs = new List<string> {
					"test",
					proj,
					$"/bl:{binlog}"
				};
				if (device is not null)
					testArgs.Add ($"--device:{device.Udid}");
				var testResult = Execution.RunAsync (DotNet.Executable, testArgs, env, Console.Out, workingDirectory: outputDir, timeout: TimeSpan.FromMinutes (10)).Result;
				Assert.That (testResult.ExitCode, Is.EqualTo (fail ? 2 : 0), $"'dotnet test' returned an unexpected exit code.\nBinlog: {binlog}\nOutput:\n{testResult.Output.MergedOutput}");
				Assert.That (testResult.Output.MergedOutput, Does.Contain ("total: 1"), "Exactly one test should run.");

				if (isDesktop && !fail) {
					var resultsPath = Path.Combine (outputDir, "Custom TestResults");
					testArgs.AddRange (["--results-directory", resultsPath, "--report-trx"]);
					for (var run = 0; run < 2; run++) {
						var result = Execution.RunAsync (DotNet.Executable, testArgs, env, Console.Out, workingDirectory: outputDir, timeout: TimeSpan.FromMinutes (10)).Result;
						Assert.That (result.ExitCode, Is.EqualTo (0), $"Run {run} with explicit reporting options failed:\n{result.Output.MergedOutput}");
						Assert.That (result.Output.MergedOutput, Does.Contain ("total: 1"), "Exactly one test should run.");
					}
					var reports = Directory.GetFiles (resultsPath, "*.trx");
					Assert.That (reports, Is.Not.Empty, "TRX reports should be written to the explicit results directory.");
					foreach (var report in reports) {
						var document = XDocument.Load (report);
						var counters = document.Descendants ().Single (v => v.Name.LocalName == "Counters");
						Assert.That ((string?) counters.Attribute ("total"), Is.EqualTo ("1"), report);
						Assert.That ((string?) counters.Attribute ("passed"), Is.EqualTo ("1"), report);
					}

					var appBundle = Directory.GetDirectories (Path.Combine (outputDir, "bin"), $"{template}.app", SearchOption.AllDirectories).Single ();
					Assert.That (Directory.GetDirectories (appBundle, "TestResults", SearchOption.AllDirectories), Is.Empty, "Results must not be written inside the app bundle.");
					var executable = Directory.GetFiles (appBundle, template, SearchOption.AllDirectories).Single ();
					var invalidResult = Execution.RunAsync (executable, ["--invalid-test-option"], env, Console.Out, workingDirectory: outputDir, timeout: TimeSpan.FromMinutes (1)).Result;
					Assert.That (invalidResult.ExitCode, Is.EqualTo (5), $"MTP command-line errors must retain their exit code:\n{invalidResult.Output.MergedOutput}");

					var mainFile = Path.Combine (outputDir, "Main.cs");
					var main = File.ReadAllText (mainFile);
					var emptyDocumentsMain = main.Replace ("Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments)", "\"\"");
					Assert.That (emptyDocumentsMain, Is.Not.EqualTo (main), "Force an empty documents path in the generated app.");
					File.WriteAllText (mainFile, emptyDocumentsMain);
					var fallbackPath = Path.Combine (Path.GetTempPath (), template, "TestResults");
					var previousReports = Directory.Exists (fallbackPath) ? Directory.GetFiles (fallbackPath, "*.trx") : [];
					var fallbackResult = Execution.RunAsync (DotNet.Executable, ["test", proj, $"/bl:{binlog}"], env, Console.Out, workingDirectory: outputDir, timeout: TimeSpan.FromMinutes (10)).Result;
					Assert.That (fallbackResult.ExitCode, Is.EqualTo (0), $"Run with an empty documents path failed:\n{fallbackResult.Output.MergedOutput}");
					Assert.That (fallbackResult.Output.MergedOutput, Does.Contain ("total: 1"), "Exactly one test should run.");
					var fallbackReports = Directory.GetFiles (fallbackPath, "*.trx").Except (previousReports).ToArray ();
					Assert.That (fallbackReports, Has.Length.EqualTo (1), "The temporary results directory should contain a new TRX report.");
					Assert.That (Directory.GetDirectories (appBundle, "TestResults", SearchOption.AllDirectories), Is.Empty, "Results must not be written inside the app bundle.");
					File.Delete (fallbackReports [0]);
				}
			} finally {
				if (device is not null)
					simService.Shutdown (device.Udid);
			}
		}

		static SimulatorDeviceInfo GetOrCreateDevice (ApplePlatform platform, SimulatorService simService, RuntimeService runtimeService)
		{
			var runtimePlatform = platform switch {
				ApplePlatform.iOS => "iOS",
				ApplePlatform.TVOS => "tvOS",
				_ => throw new ArgumentException ($"Unsupported platform: {platform}"),
			};

			// Find an existing available device for this platform
			var devices = simService.List (availableOnly: true);
			var platformDevices = devices
				.Where (d => string.Equals (d.Platform, runtimePlatform, StringComparison.OrdinalIgnoreCase))
				.OrderByDescending (d => d.OSVersion)
				.ToList ();

			if (platformDevices.Count > 0)
				return platformDevices [0];

			// No devices exist — find the best runtime and create one
			var runtimes = runtimeService.ListByPlatform (runtimePlatform, availableOnly: true);
			Assert.That (runtimes, Is.Not.Empty, $"No available {runtimePlatform} simulator runtimes found.");

			var bestRuntime = runtimes.OrderByDescending (r => r.Version).First ();

			var defaultDeviceType = platform switch {
				ApplePlatform.iOS => "com.apple.CoreSimulator.SimDeviceType.iPhone-16",
				ApplePlatform.TVOS => "com.apple.CoreSimulator.SimDeviceType.Apple-TV-4K-3rd-generation-4K",
				_ => throw new ArgumentException ($"Unsupported platform: {platform}"),
			};

			var udid = simService.Create ("test-device", defaultDeviceType, bestRuntime.Identifier);
			Assert.That (udid, Is.Not.Null, $"Failed to create {runtimePlatform} simulator device.");

			// Re-fetch the device info so we have the full object
			var created = simService.List (availableOnly: true).FirstOrDefault (d => d.Udid == udid);
			Assert.That (created, Is.Not.Null, $"Created simulator {udid} not found in device list.");
			return created!;
		}
	}
}
