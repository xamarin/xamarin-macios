using System.Diagnostics;
using System.Xml;

#nullable enable

namespace Xamarin.Tests {
	[TestFixture]
	public class Xtro {
		[Test]
		public void RunTest ()
		{
			var dir = "/Users/rolf/work/maccore/xharness/xamarin-macios/tests/xtro-sharpie";
			var args = new [] {
				"-C", dir,
				"report-dotnet/report.zip",
				"-j", "8",
			};
			var rv = ExecutionHelper.Execute ("make", args);

			var report = Path.Combine (dir, "report-dotnet", "index.html");
			if (File.Exists (report)) {
				Console.WriteLine ($"Added {report} as attachment.");
				TestContext.AddTestAttachment (report, "HTML report");
			}

			var zippedReport = Path.Combine (dir, "report-dotnet", "report.zip");
			if (File.Exists (zippedReport)) {
				Console.WriteLine ($"Added {zippedReport} as attachment.");
				TestContext.AddTestAttachment (zippedReport, "HTML report (zipped)");
			}

			Assert.AreEqual (0, rv, "ExitCode");
		}
	}
}
