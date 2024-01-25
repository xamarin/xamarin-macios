#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using Microsoft.DotNet.XHarness.Common.Logging;

namespace Xharness.Jenkins {

	class TestSelection {
		TestLabel selection =
			TestLabel.None |
			TestLabel.Msbuild |
			TestLabel.Monotouch |
			TestLabel.DotnetTest;

		PlatformLabel platform =
			PlatformLabel.None |
			PlatformLabel.tvOS |
			PlatformLabel.watchOS |
			PlatformLabel.iOS |
			PlatformLabel.iOSSimulator |
			PlatformLabel.MacCatalyst |
			PlatformLabel.LegacyXamarin |
			PlatformLabel.Dotnet;

		public bool ForceExtensionBuildOnly { get; set; }

		public TestLabel SelectedTests {
			get => selection;
			set => selection = value;
		}

		public PlatformLabel SelectedPlatforms => platform;

		public void SetEnabled (TestLabel label, bool enable)
		{
			if (enable) {
				selection |= label;
			} else {
				selection &= ~label;
			}
		}

		public void SetEnabled (PlatformLabel label, bool enable)
		{
			if (enable) {
				platform |= label;
			} else {
				platform &= ~label;
			}
		}

		public void SetEnabled (string label, bool value)
		{
			// there are two possible cases, either we are setting a test label OR a platform
			if (label.TryGetLabel<TestLabel> (out var tLabel)) {
				SetEnabled (tLabel, value);
				// some labels overlap, not ideal, but is just a few
				switch (tLabel) {
				case TestLabel.All:
					SetEnabled (PlatformLabel.All, value);
					break;
				}
				return;
			}

			if (label.TryGetLabel (out PlatformLabel pLabel)) {
				SetEnabled (pLabel, value);
				return;
			}

			throw new InvalidOperationException ($"Unknown label '{label}'");
		}

		public bool IsEnabled (TestLabel label)
			=> selection.HasFlag (label);

		public bool IsEnabled (PlatformLabel label)
			=> platform.HasFlag (label);
	}
	/// <summary>
	/// Allows to select the tests to be ran depending on certain conditions such as labels of modified files.
	/// </summary>
	class TestSelector {

		#region private vars

		readonly Jenkins jenkins;
		readonly IVersionControlSystem vcs;

		ILog? MainLog => jenkins?.MainLog;
		IHarness Harness => jenkins.Harness;

		#endregion

		public TestSelector (Jenkins jenkins, IVersionControlSystem versionControlSystem)
		{
			this.jenkins = jenkins;
			this.vcs = versionControlSystem;
		}

		void DisableKnownFailingDeviceTests ()
		{
			// https://github.com/xamarin/maccore/issues/1008
			jenkins.ForceExtensionBuildOnly = true;
		}

		IEnumerable<string> GetPullRequestLabels (int pullRequest)
		{
			if (pullRequest > 0) {
				var labels = vcs.GetLabels (pullRequest);
				var pullRequestLabels = labels as string [] ?? labels.ToArray ();
				if (pullRequestLabels.Length > 0) {
					MainLog?.WriteLine ($"Found {pullRequestLabels.Length} label(s) in the pull request #{pullRequest}: {string.Join (", ", pullRequestLabels)}");
					return pullRequestLabels;
				}
			}

			MainLog?.WriteLine ($"No labels were found in the pull request #{pullRequest}.");
			return Array.Empty<string> ();
		}

		IEnumerable<string> GetEnvironmentLabels ()
		{
			var envLabels = Environment.GetEnvironmentVariable ("XHARNESS_LABELS");
			if (!string.IsNullOrEmpty (envLabels)) {
				var labels = envLabels.Split (new char [] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				MainLog?.WriteLine ($"Found {labels.Length} label(s) in the environment variable XHARNESS_LABELS: {string.Join (", ", labels)}");
				return labels;
			}

			MainLog?.WriteLine ($"No labels were in the environment variable XHARNESS_LABELS.");
			return Array.Empty<string> ();
		}

		IEnumerable<string> GetCustomFileLabels ()
		{
			var customLabelsFile = Path.Combine (HarnessConfiguration.RootDirectory, "..", "jenkins", "custom-labels.txt");
			if (File.Exists (customLabelsFile)) {
				var customLabels = File.ReadAllLines (customLabelsFile).Select ((v) => v.Trim ()).Where (v => v.Length > 0 && v [0] != '#');
				var customFileLabels = customLabels as string [] ?? customLabels.ToArray ();
				if (customFileLabels.Length > 0) {
					MainLog?.WriteLine ($"Found {customFileLabels.Length} label(s) in {customLabelsFile}: {string.Join (", ", customFileLabels)}");
					return customFileLabels;
				}
				MainLog?.WriteLine ($"No labels were in {customLabelsFile}.");
			} else {
				MainLog?.WriteLine ($"The custom labels file {customLabelsFile} does not exist.");
			}

			return Array.Empty<string> ();
		}

		IEnumerable<string> GetHarnessLabels ()
		{
			if (Harness is not null && Harness.Labels.Any ()) {
				MainLog?.WriteLine ($"{Harness.Labels.Count} label(s) were passed on the command line.");
				return Harness.Labels;
			}

			MainLog?.WriteLine ($"No labels were passed on the command line.");
			return Array.Empty<string> ();
		}

		HashSet<string> GetAllLabels (int pullRequest)
		{
			var labels = new HashSet<string> ();
			labels.UnionWith (GetHarnessLabels ());
			labels.UnionWith (GetPullRequestLabels (pullRequest));
			labels.UnionWith (GetEnvironmentLabels ());
			labels.UnionWith (GetCustomFileLabels ());

			MainLog?.WriteLine ($"In total found {labels.Count} label(s): {string.Join (", ", labels.ToArray ())}");
			return labels;
		}

		public void SelectTestsByLabel (HashSet<string> labels, TestSelection selection, string? prTarget = null)
		{
			// when parsing the labels, order matters, for example:
			// 1. skip-all-tests, run-ios-tests
			// 2. run-ios-tests, skip-all-tests
			// the first example means that we start with all the tests disabled, and then we enable the ios tests
			// the second examples means that we have all tests disable because the skip-all-tests is last
			// In order to make this work we need to get the first label, decide if it is skip or run, then set it
			var labelSelectedTests = new Dictionary<string, bool> ();
			var regexp = new Regex ("(skip-|run-)(.*)(-tests)");
			foreach (var label in labels) {
				// use a regexp to parse the label, the possibilities are
				// skip-*-tests
				// run-*-tests
				// if we have a match, we decide if we skip or run and set the label accordingly
				if (!regexp.IsMatch (label))
					continue;
				MainLog?.WriteLine ($"Label {label} matches regexp.");
				var match = regexp.Match (label);
				var matchedLabel = match.Groups [2].Value;
				var run = match.Groups [1].Value == "run-";
				MainLog?.WriteLine ($"Setting label {matchedLabel} to be enabled: {run}");
				if (labelSelectedTests.ContainsKey (matchedLabel)) {
					// if we find a label more than once, we or the value so that we always run the tests if there
					// was a single run-* match
					labelSelectedTests [matchedLabel] |= run;
				} else {
					labelSelectedTests.Add (matchedLabel, run);
				}
				MainLog?.WriteLine ($"Selected tests are {selection.SelectedTests}");
			}

			// special case to consider, since might set or remove all tests, we need to process it first
			if (labelSelectedTests.ContainsKey ("all")) {
				selection.SetEnabled ("all", labelSelectedTests ["all"]);
				labelSelectedTests.Remove ("all");
			}

			foreach (var entry in labelSelectedTests) {
				selection.SetEnabled (entry.Key, entry.Value);
			}

			Harness.IncludeSystemPermissionTests = selection.IsEnabled (TestLabel.SystemPermission);

			// old simulator tests is also a bit special:
			// - enabled by default if using a beta Xcode, otherwise disabled by default
			if (!labelSelectedTests.ContainsKey ("old-simulator") && jenkins.IsBetaXcode) {
				selection.SetEnabled (PlatformLabel.OldiOSSimulator, true);
				MainLog?.WriteLine ("Enabled 'old-simulator' tests because we're using a beta Xcode.");
			}
		}

		public void SelectTests (TestSelection selection)
		{
			if (!int.TryParse (Environment.GetEnvironmentVariable ("PR_ID"), out int pullRequest))
				MainLog?.WriteLine ("The environment variable 'PR_ID' was not found, so no pull requests will be checked for test selection.");

			// Then we check for labels. Labels are manually set, so those override
			// whatever we did automatically.
			var labels = GetAllLabels (pullRequest);
			SelectTestsByLabel (labels, selection, pullRequest > 0 ?
				vcs.GetPullRequestTargetBranch (pullRequest) : null);

			DisableKnownFailingDeviceTests ();

			if (!Harness.INCLUDE_IOS) {
				MainLog?.WriteLine ("The iOS build is disabled, so any iOS tests will be disabled as well.");
				selection.SetEnabled (PlatformLabel.iOS, false);
			}

			if (!Harness.INCLUDE_WATCH) {
				MainLog?.WriteLine ("The watchOS build is disabled, so any watchOS tests will be disabled as well.");
				selection.SetEnabled (PlatformLabel.watchOS, false);
			}

			if (!Harness.INCLUDE_TVOS) {
				MainLog?.WriteLine ("The tvOS build is disabled, so any tvOS tests will be disabled as well.");
				selection.SetEnabled (PlatformLabel.tvOS, false);
			}

			if (!Harness.INCLUDE_MAC) {
				MainLog?.WriteLine ("The macOS build is disabled, so any macOS tests will be disabled as well.");
				selection.SetEnabled (PlatformLabel.Mac, false);
			}

			if (!Harness.INCLUDE_MACCATALYST) {
				MainLog?.WriteLine ("The Mac Catalyst build is disabled, so any Mac Catalyst tests will be disabled as well.");
				selection.SetEnabled (PlatformLabel.MacCatalyst, false);
			}

			if (!Harness.ENABLE_DOTNET) {
				MainLog?.WriteLine ("The .NET build is disabled, so any .NET tests will be disabled as well.");
				selection.SetEnabled (PlatformLabel.Dotnet, false);
			}

			if (!Harness.INCLUDE_XAMARIN_LEGACY) {
				MainLog?.WriteLine ("The legacy Xamarin build is disabled, so any legacy Xamarin tests will be disabled as well.");
				selection.SetEnabled (PlatformLabel.LegacyXamarin, false);
				selection.SetEnabled (PlatformLabel.watchOS, false);
				selection.SetEnabled (TestLabel.Bcl, false);
				selection.SetEnabled (TestLabel.InstallSource, false);
				selection.SetEnabled (TestLabel.Mmp, false);
				selection.SetEnabled (TestLabel.Mononative, false);
				selection.SetEnabled (TestLabel.Mtouch, false);
				selection.SetEnabled (TestLabel.Xammac, false);
			}

			MainLog?.WriteLine ($"Final test selection: tests: {selection.SelectedTests} platforms: {selection.SelectedPlatforms}");
		}
	}
}
