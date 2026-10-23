using Microsoft.Testing.Extensions;
using Microsoft.Testing.Platform.Builder;
using Microsoft.Testing.Platform.Extensions;
using Microsoft.Testing.Platform.Extensions.Messages;
using macOSTest1;

[assembly: Parallelize (Scope = ExecutionScope.MethodLevel)]

// NSApplication.Main() provides a proper AppKit run loop.
NSApplication.Init ();

var app = NSApplication.SharedApplication;
app.Delegate = new AppDelegate ();
app.Run ();

[Register ("AppDelegate")]
class AppDelegate : NSApplicationDelegate {
	NSWindow? _window;

	public override void DidFinishLaunching (NSNotification notification)
	{
		_window = new NSWindow (
			new CoreGraphics.CGRect (0, 0, 600, 400),
			NSWindowStyle.Titled | NSWindowStyle.Closable | NSWindowStyle.Resizable,
			NSBackingStore.Buffered,
			false
		) {
			Title = "Running tests...",
		};

		var label = new NSTextField {
			Editable = false,
			Bordered = false,
			BackgroundColor = NSColor.WindowBackground,
			StringValue = "Running tests...\n",
			TranslatesAutoresizingMaskIntoConstraints = false,
		};
		_window.ContentView!.AddSubview (label);
		label.TopAnchor.ConstraintEqualTo (_window.ContentView.TopAnchor, 8).Active = true;
		label.LeadingAnchor.ConstraintEqualTo (_window.ContentView.LeadingAnchor, 8).Active = true;
		label.TrailingAnchor.ConstraintEqualTo (_window.ContentView.TrailingAnchor, -8).Active = true;

		_window.Center ();
		_window.MakeKeyAndOrderFront (this);

		var consumer = new ResultConsumer ();
		consumer.StatusChanged += line =>
			NSRunLoop.Main.InvokeOnMainThread (() => label.StringValue += line + "\n");

		Task.Run (async () => {
			try {
				var documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
				var resultsPath = string.IsNullOrEmpty (documentsPath)
					? Path.Combine (Path.GetTempPath (), "macOSTest1", "TestResults")
					: Path.Combine (documentsPath, "TestResults");

				// Forward args from 'dotnet test', which sends the MTP protocol config
				// (--server / --dotnet-test-pipe) via the command line.
				// Environment.GetCommandLineArgs()[0] is the executable path, not an argument.
				string [] cliArgs = Environment.GetCommandLineArgs ();
				string [] mtpArgs = cliArgs.Length > 1 ? cliArgs [1..] : [];
				// MTP relaunches the app with these options, so only add defaults once.
				List<string> testArgs = [.. mtpArgs];
				if (!testArgs.Contains ("--results-directory"))
					testArgs.AddRange (["--results-directory", resultsPath]);
				if (!testArgs.Contains ("--report-trx"))
					testArgs.Add ("--report-trx");
				var builder = await TestApplication.CreateBuilderAsync ([.. testArgs]);
				builder.AddMSTest (() => [typeof (Test1).Assembly]);
				builder.AddTrxReportProvider ();
				builder.TestHost.AddDataConsumer (_ => consumer);

				using ITestApplication testApp = await builder.BuildAsync ();
				var exitCode = await testApp.RunAsync ();
				// NSApplication.Run() keeps the process alive, so exit explicitly
				Environment.Exit (exitCode != 0 ? exitCode : consumer.Failed > 0 ? 1 : 0);
			} catch (Exception ex) {
				Console.WriteLine ($"Error running tests: {ex}");
				Environment.Exit (1);
			}
		});
	}

	class ResultConsumer : IDataConsumer {
		int _passed, _failed, _skipped;
		public int Passed => _passed;
		public int Failed => _failed;
		public int Skipped => _skipped;
		public string? TrxReportPath;
		public event Action<string>? StatusChanged;

		public string Uid => nameof (ResultConsumer);
		public string DisplayName => nameof (ResultConsumer);
		public string Description => "";
		public string Version => "1.0";
		public Task<bool> IsEnabledAsync () => Task.FromResult (true);

		public Type [] DataTypesConsumed => [typeof (TestNodeUpdateMessage), typeof (SessionFileArtifact)];

		public Task ConsumeAsync (IDataProducer dataProducer, IData value, CancellationToken cancellationToken)
		{
			if (value is SessionFileArtifact artifact) {
				TrxReportPath = artifact.FileInfo.FullName;

				Console.WriteLine ($"Results: passed={Passed}, failed={Failed}, skipped={Skipped}");
				Console.WriteLine ($"TRX report: {TrxReportPath}");
				StatusChanged?.Invoke ($"\n✅ {Passed} passed  ❌ {Failed} failed  ⏭️ {Skipped} skipped");
			} else if (value is TestNodeUpdateMessage { TestNode: var node }) {
				var state = node.Properties.SingleOrDefault<TestNodeStateProperty> ();
				string? outcome = state switch {
					PassedTestNodeStateProperty => "passed",
					FailedTestNodeStateProperty or ErrorTestNodeStateProperty
						or TimeoutTestNodeStateProperty => "failed",
					SkippedTestNodeStateProperty => "skipped",
					_ => null
				};
				if (outcome is null)
					return Task.CompletedTask;

				_ = outcome switch { "passed" => Interlocked.Increment (ref _passed), "failed" => Interlocked.Increment (ref _failed), _ => Interlocked.Increment (ref _skipped) };

				var id = node.Properties.SingleOrDefault<TestMethodIdentifierProperty> ();
				var testName = id is not null ? $"{id.Namespace}.{id.TypeName}.{id.MethodName}" : node.DisplayName;
				Console.WriteLine ($"[{outcome.ToUpperInvariant ()}] {testName}");

				var icon = outcome switch { "passed" => "✅", "failed" => "❌", _ => "⏭️" };
				StatusChanged?.Invoke ($"{icon} {testName}");
			}
			return Task.CompletedTask;
		}
	}
}
