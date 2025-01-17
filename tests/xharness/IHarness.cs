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
		List<TestProject> TestProjects { get; }
		bool INCLUDE_IOS { get; }
		bool INCLUDE_TVOS { get; }
		bool INCLUDE_MAC { get; }
		bool INCLUDE_MACCATALYST { get; }
		string JENKINS_RESULTS_DIRECTORY { get; }
		string DOTNET_DIR { get; set; }
		string DOTNET_TFM { get; }
		Version DotNetVersion { get; }
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
		string VSDropsUri { get; }

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
