using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Messaging.Build.Contracts;
using Xamarin.Messaging.Client;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Xamarin.Messaging.Build {
	public class BuildAgent : Agent {
		readonly AgentInfo buildAgentInfo;

		public BuildAgent (ITopicGenerator topicGenerator, string version = null, string versionInfo = null) : base (topicGenerator)
		{
			Version = string.IsNullOrEmpty (version) ? GetVersion () : version;
			VersionInfo = string.IsNullOrEmpty (versionInfo) ? GetInformationalVersion () : versionInfo;

			buildAgentInfo = new BuildAgentInfo (Version);
		}

		public override string Name => buildAgentInfo.Name;

		public override string Version { get; }

		public override string VersionInfo { get; }

		protected override Task OnStartingAsync ()
		{
			topicGenerator.AddReplacement ("{AgentVersion}", Version);

			return Task.CompletedTask;
		}

		protected override Task InitializeAsync ()
		{
			SetLicenseEnvironmentVariables ();

			return Task.FromResult (true);
		}

		protected override async Task RegisterCustomHandlersAsync (MessageHandlerManager manager)
		{
			await TryRegisterHandlerAsync (new ExecuteTaskMessageHandler ())
				.ConfigureAwait (continueOnCapturedContext: false);
			await TryRegisterHandlerAsync (new CompareFilesMessageHandler ())
				.ConfigureAwait (continueOnCapturedContext: false);
			await TryRegisterHandlerAsync (new CopyItemMessageHandler ())
				.ConfigureAwait (continueOnCapturedContext: false);
			await TryRegisterHandlerAsync (new GetItemMessageHandler ())
			   .ConfigureAwait (continueOnCapturedContext: false);

			await TryRegisterHandlerAsync (new VerifyXcodeVersionMessageHandler ())
				.ConfigureAwait (continueOnCapturedContext: false);
			await TryRegisterHandlerAsync (new GetUniversalTargetIdentifierMessageHandler ())
				.ConfigureAwait (continueOnCapturedContext: false);
		}

		protected override async Task SendCustomLogFileMessagesAsync ()
		{
			var stderrLogFile = string.Format ("{0}.stderr.log", Tracing.GetLogFileNameWithoutExtension ());

			await Client.SendAsync (new LogFileMessage {
				LogFile = stderrLogFile
			}).ConfigureAwait (continueOnCapturedContext: false);
		}

		void SetLicenseEnvironmentVariables ()
		{
			Environment.SetEnvironmentVariable ("VSIDE", "true");

			var assemblyLocation = this.GetType ().Assembly.Location;
			var path = Path.GetDirectoryName (assemblyLocation);

			AddValueToEnvVariable ("MONO_GAC_PREFIX", path);
			AddValueToEnvVariable ("PATH", path);
			AddValueToEnvVariable ("PKG_CONFIG_PATH", path);
		}

		void AddValueToEnvVariable (string envVarName, string value)
		{
			var envValue = Environment.GetEnvironmentVariable (envVarName);

			if (string.IsNullOrEmpty (envValue)) {
				envValue = string.Empty;
			}

			if (!envValue.Contains (value)) {
				Environment.SetEnvironmentVariable (envVarName, (!string.IsNullOrEmpty (envValue) ? envValue + ":" : string.Empty) + value);
			}
		}

		// The version is a bit complicated, because we're building this assembly once, and use it for all our platforms.
		// And since the platforms don't have the same version between them, there's no single version number that's valid for all platforms...
		// So we just pick the iOS version, which will always be the correct version for remote Mac builds, because only iOS is supported for that scenario.
		string GetVersion () => VersionConstants.Microsoft_iOS_Standard_Version;

		string GetInformationalVersion () => GetVersion () + "-" + VersionConstants.Branch + "+" + VersionConstants.Commit;
	}
}
