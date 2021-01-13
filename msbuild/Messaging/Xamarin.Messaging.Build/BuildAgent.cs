using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Messaging.Build.Contracts;
using Xamarin.Messaging.Client;

namespace Xamarin.Messaging.Build
{
	public class BuildAgent : Agent
	{
		public BuildAgent(ITopicGenerator topicGenerator, string version = null, string versionInfo = null) : base(topicGenerator)
		{
			Version = string.IsNullOrEmpty(version) ? GetVersion() : version;
			VersionInfo = string.IsNullOrEmpty(versionInfo) ? GetInformationalVersion() : versionInfo;
		}

		public override string Name => BuildAgentInfo.Instance.Name;

		public override string Version { get; }

		public override string VersionInfo { get; }

		protected override Task InitializeAsync()
		{
			SetLicenceEnvironmentVariables();

			return Task.FromResult(true);
		}

		protected override async Task RegisterCustomHandlersAsync(MessageHandlerManager manager)
		{
			await TryRegisterHandlerAsync(new ExecuteTaskMessageHandler())
				.ConfigureAwait(continueOnCapturedContext: false);
			await TryRegisterHandlerAsync(new CompareFilesMessageHandler())
				.ConfigureAwait(continueOnCapturedContext: false);
			await TryRegisterHandlerAsync(new CopyItemMessageHandler())
				.ConfigureAwait(continueOnCapturedContext: false);
			await TryRegisterHandlerAsync(new GetItemMessageHandler())
			   .ConfigureAwait(continueOnCapturedContext: false);

			await TryRegisterHandlerAsync (new VerifyXcodeVersionMessageHandler ())
				.ConfigureAwait (continueOnCapturedContext: false);
			await TryRegisterHandlerAsync (new GetUniversalTargetIdentifierMessageHandler ())
				.ConfigureAwait (continueOnCapturedContext: false);
		}

		protected override async Task SendCustomLogFileMessagesAsync()
		{
			var stderrLogFile = string.Format("{0}.stderr.log", TracingConfiguration.GetLogFileNameWithoutExtension());

			await Client.SendAsync(new LogFileMessage
			{
				LogFile = stderrLogFile
			}).ConfigureAwait(continueOnCapturedContext: false);
		}

		void SetLicenceEnvironmentVariables()
		{
			Environment.SetEnvironmentVariable("VSIDE", "true");

			var assemblyLocation = this.GetType().Assembly.Location;
			var path = Path.GetDirectoryName(assemblyLocation);

			AddValueToEnvVariable("MONO_GAC_PREFIX", path);
			AddValueToEnvVariable("PATH", path);
			AddValueToEnvVariable("PKG_CONFIG_PATH", path);
		}

		void AddValueToEnvVariable(string envVarName, string value)
		{
			var envValue = Environment.GetEnvironmentVariable(envVarName);

			if (string.IsNullOrEmpty(envValue))
			{
				envValue = string.Empty;
			}

			if (!envValue.Contains(value))
			{
				Environment.SetEnvironmentVariable(envVarName, (!string.IsNullOrEmpty(envValue) ? envValue + ":" : string.Empty) + value);
			}
		}

		string GetVersion() => ThisAssembly.Git.BaseVersion.Major + "." + ThisAssembly.Git.BaseVersion.Minor + "." + ThisAssembly.Git.BaseVersion.Patch + "." + ThisAssembly.Git.Commits;

		string GetInformationalVersion () => GetVersion () + "-" + ThisAssembly.Git.Branch + "+" + ThisAssembly.Git.Commit;
	}
}