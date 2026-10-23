using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;

using Microsoft.Build.Framework;
using Microsoft.Build.Tasks;
using Microsoft.Build.Utilities;

using Xamarin.Bundler;
using Xamarin.Localization.MSBuild;
using Xamarin.Messaging.Build.Client;
using Xamarin.Utils;

#nullable enable

namespace Xamarin.MacDev.Tasks {
	public abstract class XamarinTask : Task, IHasSessionId, ICustomLogger, IToolLog {

		public string SessionId { get; set; } = string.Empty;

		public string TargetFrameworkMoniker { get; set; } = string.Empty;

		public string SdkDevPath { get; set; } = string.Empty;

		int? verbosity;
		public int Verbosity {
			get {
				if (!verbosity.HasValue)
					verbosity = VerbosityUtils.GetVerbosityLevel (Environment.CommandLine);
				return verbosity.Value;
			}
			set => verbosity = value;
		}

		public string GetSdkDevPath ()
		{
			if (string.IsNullOrEmpty (SdkDevPath)) {
				Log.LogError (MSBStrings.E7169, /* The task '{0}' requires the property '{1}' to be set. Please file an issue at https://github.com/dotnet/macios/issues/new/choose. */ GetType ().Name, "SdkDevPath");
				return "";
			}

			return SdkDevPath;
		}

		XcodeLocator? xcodeLocator = null;
		public XcodeLocator GetXcodeLocator (bool initialDiscovery = false, Action<XcodeLocator>? preprocess = null)
		{
			if (xcodeLocator is null) {
				if (!initialDiscovery && string.IsNullOrEmpty (SdkDevPath)) {
					Log.LogError (MSBStrings.E7169, /* The task '{0}' requires the property '{1}' to be set. Please file an issue at https://github.com/dotnet/macios/issues/new/choose. */ GetType ().Name, "SdkDevPath");
				}

				var xcodeLocator = new XcodeLocator (this);
				preprocess?.Invoke (xcodeLocator);
				if (!xcodeLocator.TryLocatingXcode (SdkDevPath))
					Log.LogError (MSBStrings.E0086 /* Could not find a valid Xcode developer path */);
				this.xcodeLocator = xcodeLocator;
			}
			return xcodeLocator;
		}

		protected void SetXcodeLocator (XcodeLocator xcodeLocator)
		{
			this.xcodeLocator = xcodeLocator;
		}

		IAppleSdk? currentSdk;
		public IAppleSdk CurrentSdk {
			get {
				if (currentSdk is null) {
					var xcodeLocator = GetXcodeLocator ();
					switch (Platform) {
					case ApplePlatform.iOS:
						currentSdk = new AppleIPhoneSdk (xcodeLocator.DeveloperRoot, xcodeLocator.DeveloperRootVersionPlist);
						break;
					case ApplePlatform.TVOS:
						currentSdk = new AppleTVOSSdk (xcodeLocator.DeveloperRoot, xcodeLocator.DeveloperRootVersionPlist);
						break;
					case ApplePlatform.MacCatalyst:
					case ApplePlatform.MacOSX:
						currentSdk = new MacOSXSdk (xcodeLocator.DeveloperRoot, xcodeLocator.DeveloperRootVersionPlist);
						break;
					default:
						throw new InvalidOperationException (string.Format (MSBStrings.InvalidPlatform, Platform));
					}
				}
				return currentSdk;
			}
		}

		void VerifyTargetFrameworkMoniker ()
		{
			if (!string.IsNullOrEmpty (TargetFrameworkMoniker))
				return;
			Log.LogError (MSBStrings.E7169, /* The task '{0}' requires the property '{1}' to be set. Please file an issue at https://github.com/dotnet/macios/issues/new/choose. */ GetType ().Name, "TargetFrameworkMoniker");
		}

		public string Product {
			get {
				return "Microsoft." + PlatformName;
			}
		}

		ApplePlatform? platform;
		public ApplePlatform Platform {
			get {
				if (!platform.HasValue) {
					VerifyTargetFrameworkMoniker ();
					platform = PlatformFrameworkHelper.GetFramework (TargetFrameworkMoniker);
				}
				return platform.Value;
			}
		}

		TargetFramework? target_framework;
		public TargetFramework TargetFramework {
			get {
				if (!target_framework.HasValue) {
					VerifyTargetFrameworkMoniker ();
					target_framework = TargetFramework.Parse (TargetFrameworkMoniker);
				}
				return target_framework.Value;
			}
		}

		public string PlatformName {
			get {
				switch (Platform) {
				case ApplePlatform.iOS:
					return "iOS";
				case ApplePlatform.TVOS:
					return "tvOS";
				case ApplePlatform.MacOSX:
					return "macOS";
				case ApplePlatform.MacCatalyst:
					return "MacCatalyst";
				default:
					throw new InvalidOperationException (string.Format (MSBStrings.InvalidPlatform, Platform));
				}
			}
		}

		public string DotNetVersion {
			get {
				switch (Platform) {
				case ApplePlatform.iOS:
					return VersionConstants.Microsoft_iOS_Version;
				case ApplePlatform.MacCatalyst:
					return VersionConstants.Microsoft_MacCatalyst_Version;
				case ApplePlatform.MacOSX:
					return VersionConstants.Microsoft_macOS_Version;
				case ApplePlatform.TVOS:
					return VersionConstants.Microsoft_tvOS_Version;
				default:
					throw new InvalidOperationException (string.Format (MSBStrings.InvalidPlatform, Platform));
				}
			}
		}

		protected string GetSdkPlatform (bool isSimulator)
		{
			return PlatformFrameworkHelper.GetSdkPlatform (Platform, isSimulator);
		}

		internal protected System.Threading.Tasks.Task<Execution> ExecuteAsync (string fileName, IList<string> arguments, Dictionary<string, string?>? environment = null, bool showErrorIfFailure = true, string? workingDirectory = null, CancellationToken? cancellationToken = null)
		{
			return ExecuteAsync (this, fileName, arguments, SdkDevPath, environment, showErrorIfFailure, workingDirectory, cancellationToken);
		}

		static int executionCounter;
		static async System.Threading.Tasks.Task<Execution> ExecuteAsync (Task task, string fileName, IList<string> arguments, string? sdkDevPath = null, Dictionary<string, string?>? environment = null, bool showErrorIfFailure = true, string? workingDirectory = null, CancellationToken? cancellationToken = null)
		{
			var log = task.Log;
			// Create a new dictionary if we're given one, to make sure we don't change the caller's dictionary.
			var launchEnvironment = environment is null ? new Dictionary<string, string?> () : new Dictionary<string, string?> (environment);
			if (!string.IsNullOrEmpty (sdkDevPath))
				launchEnvironment ["DEVELOPER_DIR"] = sdkDevPath;

			if (Environment.OSVersion.Platform == PlatformID.MacOSX && string.IsNullOrEmpty (sdkDevPath)) {
				log.LogWarning (MSBStrings.E7164 /* The task '{0}' is trying to call an external process, but a path to Xcode has not been provided. Please file an issue at https://github.com/dotnet/macios/issues/new/choose. */, task.GetType ().Name);
				log.LogMessage (MessageImportance.Low, Environment.StackTrace);
			}

			var currentId = Interlocked.Increment (ref executionCounter);
			log.LogMessage (MessageImportance.Normal, MSBStrings.M0001, currentId, fileName, StringUtils.FormatArguments (arguments)); // Started external tool execution #{0}: {1} {2}
			if (!string.IsNullOrEmpty (workingDirectory)) {
				log.LogMessage (MessageImportance.Low, "    Working directory: {0}", workingDirectory);
			} else {
				log.LogMessage (MessageImportance.Low, "    Current directory: {0}", Environment.CurrentDirectory);
			}
			if (launchEnvironment?.Any () == true) {
				log.LogMessage (MessageImportance.Low, "    With environment:");
				foreach (var kvp in launchEnvironment) {
					log.LogMessage (MessageImportance.Low, "        {0}={1}", kvp.Key, kvp.Value);
				}
			}
			var rv = await Execution.RunAsync (fileName, arguments, environment: launchEnvironment, workingDirectory: workingDirectory, cancellationToken: cancellationToken);
			log.LogMessage (rv.ExitCode == 0 ? MessageImportance.Low : MessageImportance.High, MSBStrings.M0002, currentId, rv.Duration, rv.ExitCode); // Finished external tool execution #{0} in {1} and with exit code {2}.

			// Show the output
			var output = rv.Output.MergedOutput;
			if (output.Length > 0) {
				var importance = MessageImportance.Low;
				if (rv.ExitCode != 0)
					importance = showErrorIfFailure ? MessageImportance.High : MessageImportance.Normal;
				log.LogMessage (importance, output);
			}

			if (showErrorIfFailure && rv.ExitCode != 0) {
				var stderr = rv.Output.StandardError.Trim ();
				if (stderr.Length > 1024)
					stderr = stderr.Substring (0, 1024);
				if (string.IsNullOrEmpty (stderr)) {
					log.LogError (MSBStrings.E0117, /* {0} exited with code {1} */ fileName == "xcrun" ? arguments [0] : fileName, rv.ExitCode);
				} else {
					log.LogError (MSBStrings.E0118, /* {0} exited with code {1}:\n{2} */ fileName == "xcrun" ? arguments [0] : fileName, rv.ExitCode, stderr);
				}
			}

			return rv;
		}

		public bool ShouldExecuteRemotely () => this.ShouldExecuteRemotely (SessionId);

		protected bool ExecuteRemotely ()
		{
			return ExecuteRemotely (out var _);
		}

#if NET
		protected bool ExecuteRemotely ([NotNullWhen (true)] out TaskRunner? taskRunner, Action<TaskRunner>? preprocessTaskRunner = null)
#else
		protected bool ExecuteRemotely (out TaskRunner taskRunner, Action<TaskRunner>? preprocessTaskRunner = null)
#endif
		{
			return ExecuteRemotely (this, out taskRunner, preprocessTaskRunner);
		}

		internal static bool ExecuteRemotely<T> (T task) where T : Task, IHasSessionId
		{
			return ExecuteRemotely (task, out var _, null);
		}

#if NET
		internal static bool ExecuteRemotely<T> (T task, [NotNullWhen (true)] out TaskRunner? taskRunner, Action<TaskRunner>? preprocessTaskRunner = null) where T : Task, IHasSessionId
#else
		internal static bool ExecuteRemotely<T> (T task, out TaskRunner taskRunner, Action<TaskRunner>? preprocessTaskRunner = null) where T : Task, IHasSessionId
#endif
		{
#if NET
			taskRunner = null;
#else
			taskRunner = null!;
#endif

			try {
				taskRunner = new TaskRunner (task.SessionId, task.BuildEngine4);

				if (preprocessTaskRunner is not null)
					preprocessTaskRunner (taskRunner);

				var rv = taskRunner.RunAsync (task).Result;
				if (!rv && !task.Log.HasLoggedErrors) {
					// if we failed to execute remotely, but no error has been reported, then report an error
					task.Log.LogError (MSBStrings.E7162 /* Unable to execute this task remotely for unknown reasons. The build log may have more information. */);
				}
				return rv;
			} catch (Exception ex) {
				task.Log.LogErrorFromException (ex);

				return false;
			}
		}

		internal static bool CopyInputsToRemoteServerAsync<T> (T task) where T : Task, IHasSessionId
		{
			try {
				var rv = new TaskRunner (task.SessionId, task.BuildEngine4).CopyInputsAsync2 (task).Result;
				if (!rv)
					task.Log.LogError (MSBStrings.E7163 /* Unable to copy the inputs to this task to the remote build server for unknown reasons. The build log may have more information. */);
				return rv;
			} catch (Exception ex) {
				task.Log.LogErrorFromException (ex);
				return false;
			}
		}

		protected string GetNonEmptyStringOrFallback (ITaskItem item, string metadataName, string fallbackValue, string? fallbackName = null, bool required = false)
		{
			return GetNonEmptyStringOrFallback (item, metadataName, out var _, fallbackValue, fallbackName, required);
		}

		protected string GetNonEmptyStringOrFallback (ITaskItem item, string metadataName, out bool foundInMetadata, string fallbackValue, string? fallbackName = null, bool required = false)
		{
			var metadataValue = item.GetMetadata (metadataName);
			if (!string.IsNullOrEmpty (metadataValue)) {
				foundInMetadata = true;
				return metadataValue;
			}
			if (required && string.IsNullOrEmpty (fallbackValue))
				Log.LogError (MSBStrings.E7085 /* The "{0}" task was not given a value for the required parameter "{1}", nor was there a "{2}" metadata on the resource {3}. */, GetType ().Name, fallbackName ?? metadataName, metadataName, item.ItemSpec);
			foundInMetadata = false;
			return fallbackValue;
		}

		protected internal static IEnumerable<ITaskItem> CreateItemsForAllFilesRecursively (params string [] directories)
		{
			return CreateItemsForAllFilesRecursively ((IEnumerable<string>?) directories);
		}

		protected internal static IEnumerable<ITaskItem> CreateItemsForAllFilesRecursively (IEnumerable<string>? directories)
		{
			if (directories is null)
				yield break;

			foreach (var dir in directories) {
				// Don't try to find files if we don't have a directory in the first place (or it doesn't exist).
				if (!Directory.Exists (dir))
					continue;

				foreach (var file in Directory.EnumerateFiles (dir, "*", SearchOption.AllDirectories))
					yield return new TaskItem (file);
			}
		}

		protected internal static IEnumerable<ITaskItem> CreateItemsForAllFilesRecursively (IEnumerable<ITaskItem>? directories)
		{
			return CreateItemsForAllFilesRecursively (directories?.Select (v => v.ItemSpec));
		}

		internal static async global::System.Threading.Tasks.Task CopyFilesToWindowsAsync (Task task, TaskRunner runner, params IEnumerable<ITaskItem> items)
		{
			foreach (var item in items) {
				task.Log.LogMessage (MessageImportance.Low, $"Copying {item.ItemSpec} from the remote Mac to Windows");
				await runner.GetFileAsync (task, item.ItemSpec).ConfigureAwait (false);
			}
		}

		internal global::System.Threading.Tasks.Task CopyFilesToWindowsAsync (TaskRunner runner, params IEnumerable<ITaskItem> items)
		{
			return CopyFilesToWindowsAsync (this, runner, items);
		}

		/// <summary>
		///     Computes the executable to launch given the specified tool in the Xcode's toolchain:
		///     * If <paramref name="toolPathOverride" /> is specified, return that.
		///     * Otherwise return 'xcrun', and then inject the tool name as the first argument.
		/// <summary>
		/// <param name="arguments">The arguments to pass to the tool.</param>
		/// <param name="toolName">The name of the tool (aka executable) to execute.</param>
		/// <param name="toolPathOverride">If specified, an override for the path to the tool.</param>
		protected static string GetExecutable (List<string> arguments, string toolName, string? toolPathOverride)
		{
#if NET
			if (string.IsNullOrEmpty (toolPathOverride)) {
#else
			if (toolPathOverride is null || string.IsNullOrEmpty (toolPathOverride)) {
#endif
				arguments.Insert (0, toolName);
				return "xcrun";
			}
			return toolPathOverride;
		}

		#region Xamarin.MacDev.ICustomLogger
		void ICustomLogger.LogError (string message, Exception? ex)
		{
			if (!string.IsNullOrEmpty (message))
				Log.LogError (message);
			if (ex is ProductException pe) {
				LogDiagnostic (pe);
			} else if (ex is not null) {
				Log.LogErrorFromException (ex, showStackTrace: true, showDetail: true, file: null);
			}
		}

		void ICustomLogger.LogWarning (string messageFormat, params object? [] args)
		{
			Log.LogWarning (messageFormat, args);
		}

		void ICustomLogger.LogInfo (string messageFormat, params object? [] args)
		{
			Log.LogMessage (MessageImportance.Normal, messageFormat, args);
		}

		void ICustomLogger.LogDebug (string messageFormat, params object? [] args)
		{
			Log.LogMessage (MessageImportance.Low, messageFormat, args);
		}
		#endregion

		#region Xamarin.Bundler.IToolLog
		void IToolLog.Log (string message)
		{
			((ICustomLogger) this).LogInfo (message);
		}

		void IToolLog.LogError (string message)
		{
			((ICustomLogger) this).LogError (message, null);
		}

		void IToolLog.LogException (Exception exception)
		{
			if (exception is ProductException pe) {
				LogDiagnostic (pe);
			} else {
				((ICustomLogger) this).LogError ($"Unexpected exception '{GetType ().Name}': {exception.Message}", exception);
			}
		}

		void IToolLog.LogError (ProductException exception)
		{
			LogDiagnostic (exception);
		}

		void IToolLog.LogWarning (ProductException exception)
		{
			LogDiagnostic (exception);
		}

		protected void LogDiagnostic (ProductException exception)
		{
			switch (exception.GetWarningLevel (this)) {
			case Xamarin.Bundler.ErrorHelper.WarningLevel.Warning:
				Log.LogWarning (exception.Code, exception.FileName, exception.LineNumber, exception.Message);
				break;
			case Xamarin.Bundler.ErrorHelper.WarningLevel.Error:
				Log.LogError (exception.Code, exception.FileName, exception.LineNumber, exception.Message);
				break;
			case Xamarin.Bundler.ErrorHelper.WarningLevel.Disable:
			default:
				Log.LogMessage (MessageImportance.Low, exception.Code, exception.FileName, exception.LineNumber, exception.Message);
				break;
			}
		}
		#endregion
	}
}
