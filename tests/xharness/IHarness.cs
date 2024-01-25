using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.DotNet.XHarness.Common;
using Microsoft.DotNet.XHarness.Common.Logging;
using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Listeners;

namespace Xharness {

	/// <summary>
	/// Interface that represents the harness class that contains all the needed info to execute the tests.
	/// </summary>
	public interface IHarness {

		#region Properties
		HarnessAction Action { get; }
		IFileBackedLog HarnessLog { get; set; }
		int Verbosity { get; }
		HashSet<string> Labels { get; }
		XmlResultJargon XmlJargon { get; }
		IResultParser ResultParser { get; }
		AppBundleLocator AppBundleLocator { get; }
		ITunnelBore TunnelBore { get; }
		string XIBuildPath { get; }
		List<iOSTestProject> IOSTestProjects { get; }
		List<MacTestProject> MacTestProjects { get; }
		string WatchOSContainerTemplate { get; }
		string WatchOSAppTemplate { get; }
		string WatchOSExtensionTemplate { get; }
		string TodayContainerTemplate { get; }
		string TodayExtensionTemplate { get; }
		string BCLTodayExtensionTemplate { get; }
		string MONO_PATH { get; }
		string TVOS_MONO_PATH { get; }
		bool INCLUDE_IOS { get; }
		bool INCLUDE_TVOS { get; }
		bool INCLUDE_WATCH { get; }
		bool INCLUDE_MAC { get; }
		bool INCLUDE_MACCATALYST { get; }
		string JENKINS_RESULTS_DIRECTORY { get; }
		string MAC_DESTDIR { get; }
		string IOS_DESTDIR { get; }
		string MONO_IOS_SDK_DESTDIR { get; }
		string MONO_MAC_SDK_DESTDIR { get; }
		bool ENABLE_DOTNET { get; }
		bool INCLUDE_XAMARIN_LEGACY { get; }
		string SYSTEM_MONO { get; set; }
		string DOTNET_DIR { get; set; }
		string DOTNET_TFM { get; }
		string XcodeRoot { get; }
		string LogDirectory { get; }
		double Timeout { get; }
		double LaunchTimeout { get; } // in minutes
		bool DryRun { get; } // Most things don't support this. If you need it somewhere, implement it!
		string JenkinsConfiguration { get; }
		Dictionary<string, string> EnvironmentVariables { get; }
		string MarkdownSummaryPath { get; }
		string PeriodicCommand { get; }
		string PeriodicCommandArguments { get; }
		TimeSpan PeriodicCommandInterval { get; }
		bool? IncludeSystemPermissionTests { get; set; }
		bool InCI { get; }
		bool UseTcpTunnel { get; }
		bool UseGroupedApps { get; }
		string VSDropsUri { get; }
		bool DisableWatchOSOnWrench { get; }

		#endregion

		#region methods

		bool GetIncludeSystemPermissionTests (TestPlatform platform, bool device);
		void Save (StringWriter doc, string path);
		void Log (int minLevel, string message, params object [] args);
		void Log (string message);
		void Log (string message, params object [] args);
		string GetDotNetExecutable (string directory);

		#endregion
	}
}
