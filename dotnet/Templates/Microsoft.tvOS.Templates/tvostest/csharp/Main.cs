using Microsoft.Testing.Extensions;
using Microsoft.Testing.Platform.Builder;
using Microsoft.Testing.Platform.Extensions;
using Microsoft.Testing.Platform.Extensions.Messages;
using tvOSTest1;

[assembly: Parallelize (Scope = ExecutionScope.MethodLevel)]

// UIApplication.Main() provides a proper UIKit run loop,
// preventing tvOS watchdog kills during long test runs.
UIApplication.Main (args, null, typeof (AppDelegate));

[Register ("AppDelegate")]
class AppDelegate : UIApplicationDelegate {
	public override UISceneConfiguration GetConfiguration (UIApplication application,
		UISceneSession connectingSceneSession, UISceneConnectionOptions options)
	{
		return new UISceneConfiguration ("Default Configuration", connectingSceneSession.Role);
	}
}

[Register ("SceneDelegate")]
class SceneDelegate : UIResponder, IUIWindowSceneDelegate {
	[Export ("window")]
	public UIWindow? Window { get; set; }

	[Export ("scene:willConnectToSession:options:")]
	public void WillConnect (UIScene scene, UISceneSession session, UISceneConnectionOptions connectionOptions)
	{
		if (scene is not UIWindowScene windowScene) {
			Console.WriteLine ($"Unexpected scene type: {scene.GetType ().FullName}");
			Environment.Exit (1);
			return;
		}

		Window = new UIWindow (windowScene) {
			RootViewController = new UIViewController (),
		};
		Window.MakeKeyAndVisible ();

		var consumer = new ResultConsumer ();

		Task.Run (async () => {
			try {
				var documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
				var resultsPath = string.IsNullOrEmpty (documentsPath)
					? Path.Combine (Path.GetTempPath (), "tvOSTest1", "TestResults")
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

				using ITestApplication app = await builder.BuildAsync ();
				var exitCode = await app.RunAsync ();
				// UIApplication.Main() keeps the process alive, so exit explicitly
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
			}
			return Task.CompletedTask;
		}
	}
}
