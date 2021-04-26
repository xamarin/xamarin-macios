using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

using AppKit;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Exporters.Json;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess.Emit;
using Perfolizer.Horology;

namespace PerfTest {
	static class MainClass {
		static int Main (string [] args)
		{
			try {
				// The call to NSApplication.Init is required
				NSApplication.Init ();
				BenchmarkRunner.Run (typeof (MainClass).Assembly, new Config ());
			} catch (Exception e) {
				Console.WriteLine ("Unhandled exception: {0}", e);
				return 1;
			}

			return 0;
		}
	}

	public class Config : ManualConfig {
		public Config ()
		{
			AddJob (Job.ShortRun
				.WithLaunchCount (1)
				.WithToolchain (new InProcessEmitToolchain (TimeSpan.FromHours (1), true))
				.WithId ("InProcess")
			);

			var artifactsPath = Environment.GetEnvironmentVariable ("BENCHMARK_OUTPUT_PATH");
			if (!string.IsNullOrEmpty (artifactsPath))
				WithArtifactsPath (artifactsPath);

			WithOption (ConfigOptions.DisableOptimizationsValidator, true);
			WithOption (ConfigOptions.JoinSummary, true);

			AddLogger (DefaultConfig.Instance.GetLoggers ().ToArray ()); // manual config has no loggers by default
			AddExporter (DefaultConfig.Instance.GetExporters ().Where (v => !(v is CsvExporter)).ToArray ()); // manual config has no exporters by default

			var csv = new CsvExporter (
				CsvSeparator.Semicolon,
				new SummaryStyle (CultureInfo.CurrentCulture, true, SizeUnit.B, TimeUnit.Nanosecond, false, true)
			);
			AddExporter (csv);
			AddExporter (JsonExporter.Full);

			AddColumnProvider (DefaultConfig.Instance.GetColumnProviders ().ToArray ()); // manual config has no columns by default
		}
	}
}
