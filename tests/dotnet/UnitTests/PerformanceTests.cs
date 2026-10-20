// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;

namespace Xamarin.Tests {
	[TestFixture]
	public class PerformanceTests : TestBaseClass {
		[Test]
		[TestCase (ApplePlatform.iOS)]
		[TestCase (ApplePlatform.MacOSX)]
		public void PrepareAssemblies (ApplePlatform platform)
		{
			PrepareAssembliesImpl (platform);
		}

		void PrepareAssembliesImpl (ApplePlatform platform)
		{
			Configuration.IgnoreIfIgnoredPlatform (platform);

			string runtimeIdentifiers;
			switch (platform) {
			case ApplePlatform.iOS:
				runtimeIdentifiers = "iossimulator-arm64;ios-arm64";
				break;
			case ApplePlatform.TVOS:
				runtimeIdentifiers = "tvossimulator-arm64;tvos-arm64";
				break;
			case ApplePlatform.MacCatalyst:
				runtimeIdentifiers = "maccatalyst-arm64";
				break;
			case ApplePlatform.MacOSX:
				runtimeIdentifiers = "osx-arm64";
				break;
			default:
				Assert.Inconclusive ($"Don't know which runtime identifiers to use for {platform}");
				return;
			}

			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var enablePerformanceTests = Environment.GetEnvironmentVariable ("ENABLE_PERFORMANCE_TESTS");
			if (string.IsNullOrEmpty (enablePerformanceTests))
				Assert.Ignore ("Test ignored because ENABLE_PERFORMANCE_TESTS is not set.");

			var watch = Stopwatch.StartNew ();
			var projects = new string [] { "MySimpleApp", "monotouch-test" };
			var results = new List<PrepareAssembliesTestResult> ();
			var attempts = 3;
			var report = new StringBuilder ();
			foreach (var project in projects) {
				var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var _);

				foreach (var rid in runtimeIdentifiers.Split (';')) {
					foreach (var linkMode in new string [] { "None", "SdkOnly" }) {
						var result = new PrepareAssembliesTestResult {
							Project = project,
							RuntimeIdentifier = rid,
							LinkMode = linkMode,
						};
						foreach (var propertyValue in new bool [] { false, true }) {
							var properties = GetDefaultProperties (rid);
							properties ["PrepareAssemblies"] = propertyValue.ToString ();
							properties ["MtouchLink"] = linkMode;
							properties ["UseMonoRuntime"] = "false";

							for (var i = 1; i <= attempts; i++) {
								Clean (project_path);
								var rv = DotNet.AssertBuild (project_path, properties);
								result.GetBuildTimes (propertyValue).Times.Add (rv);
							}
						}

						results.Add (result);
					}
				}
			}
			var orderedResults = results.OrderBy (v => v.Project).ThenBy (v => v.RuntimeIdentifier).ThenBy (v => v.LinkMode).ToList ();

			report.AppendLine ($"# PrepareAssemblies performance report ({platform.AsString ()}/CoreCLR)");
			report.AppendLine ();
			report.AppendLine ("## Summary");
			report.AppendLine ();
			var summaryHeaders = new [] { "Project", "Runtime identifier", "Link mode", "PrepareAssemblies=false", "PrepareAssemblies=true", "Difference" };
			var summaryRows = new List<string []> ();
			foreach (var result in orderedResults) {
				summaryRows.Add (new [] {
					result.Project,
					result.RuntimeIdentifier,
					result.LinkMode,
					FormatTimeSpan (result.GetBuildTimes (false).Average),
					FormatTimeSpan (result.GetBuildTimes (true).Average),
					FormatTimeSpan (result.AverageDifference),
				});
			}
			AppendMarkdownTable (report, summaryHeaders, summaryRows);
			report.AppendLine ();

			foreach (var result in orderedResults) {
				report.AppendLine ($"## {result.Project} ({result.RuntimeIdentifier} / LinkMode={result.LinkMode})");
				report.AppendLine ();
				var disabledTimes = result.GetBuildTimes (false).Times;
				var enabledTimes = result.GetBuildTimes (true).Times;
				var runCount = Math.Max (disabledTimes.Count, enabledTimes.Count);
				var detailHeaders = new [] { "Run", "Disabled", "Enabled", "Disabled binlog", "Enabled binlog" };
				var detailRows = new List<string []> ();
				for (var i = 0; i < runCount; i++) {
					var disabled = i < disabledTimes.Count ? disabledTimes [i] : null;
					var enabled = i < enabledTimes.Count ? enabledTimes [i] : null;
					detailRows.Add (new [] {
						$"#{i + 1}",
						disabled is not null ? FormatTimeSpan (disabled.Duration) : "",
						enabled is not null ? FormatTimeSpan (enabled.Duration) : "",
						disabled?.BinLogPath ?? "",
						enabled?.BinLogPath ?? "",
					});
				}
				detailRows.Add (new [] {
					"**Average**",
					FormatTimeSpan (result.GetBuildTimes (false).Average),
					FormatTimeSpan (result.GetBuildTimes (true).Average),
					"",
					"",
				});
				AppendMarkdownTable (report, detailHeaders, detailRows);
				report.AppendLine ();
				report.AppendLine ($"Difference (of average): {FormatTimeSpan (result.AverageDifference)}");
				report.AppendLine ();
			}

			report.AppendLine ($"Total duration: {FormatTimeSpan (watch.Elapsed)}");
			Console.WriteLine (report);

			if (enablePerformanceTests.Contains ("%SPEC%")) {
				for (var i = 1; i < 256; i++) {
					var path = enablePerformanceTests.Replace ("%SPEC%", $"{platform.AsString ()}-CoreCLR-{i}");
					if (File.Exists (path))
						continue;
					File.WriteAllText (path, report.ToString ());
					Console.WriteLine ($"Wrote report to: {path}");
					break;
				}
			}
		}

		static string FormatTimeSpan (TimeSpan value)
		{
			// A custom TimeSpan format string doesn't include the sign, so handle negative values ourselves.
			var sign = value < TimeSpan.Zero ? "-" : " ";
			return sign + value.Duration ().ToString (@"hh\:mm\:ss\.ff");
		}

		static void AppendMarkdownTable (StringBuilder report, string [] headers, List<string []> rows)
		{
			var widths = new int [headers.Length];
			for (var i = 0; i < headers.Length; i++)
				widths [i] = headers [i].Length;
			foreach (var row in rows) {
				for (var i = 0; i < row.Length; i++)
					widths [i] = Math.Max (widths [i], row [i].Length);
			}

			report.AppendLine ("| " + string.Join (" | ", headers.Select ((h, i) => h.PadRight (widths [i]))) + " |");
			report.AppendLine ("| " + string.Join (" | ", widths.Select (w => new string ('-', w))) + " |");
			foreach (var row in rows)
				report.AppendLine ("| " + string.Join (" | ", row.Select ((c, i) => c.PadRight (widths [i]))) + " |");
		}

		class PrepareAssembliesTestResult {
			public required string Project;
			public required string RuntimeIdentifier;
			public required string LinkMode;
			BuildTimes EnabledBuildTimes = new ();
			BuildTimes DisabledBuildTimes = new ();

			public BuildTimes GetBuildTimes (bool enabled) => enabled ? EnabledBuildTimes : DisabledBuildTimes;
			public TimeSpan AverageDifference => EnabledBuildTimes.Average - DisabledBuildTimes.Average;
		}

		class BuildTimes {
			public List<ExecutionResult> Times = new ();
			public TimeSpan Average => TimeSpan.FromTicks ((long) Times.Average (v => v.Duration.Ticks));
		}
	}
}

