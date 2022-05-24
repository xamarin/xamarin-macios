#nullable enable

using System;
using System.Collections.Generic;
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
			// there are two possible cases, either we are setting a test label OR a 
			if (label.TryGetLabel (out TestLabel tLabel)) {
				SetEnabled (tLabel, value);
				// some labels overlap, not idea, but is just a few
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
		
		readonly Jenkins? jenkins;
		readonly IVersionControlSystem vcs;

		ILog? MainLog => jenkins?.MainLog;
		IHarness? Harness => jenkins?.Harness;
		
		// We select tests based on a prefix of the modified files.
		// Add entries here to check for more prefixes.
		static readonly string [] mtouchPrefixes = {
			"tests/mtouch",
			"tests/common",
			"tools/mtouch",
			"tools/common",
			"tools/linker",
			"src/ObjCRuntime/Registrar.cs",
			"mk/mono.mk",
			"msbuild",
			"runtime",
		};
		static readonly string[] mmpPrefixes = {
			"tests/mmptest",
			"tests/common",
			"tools/mmp",
			"tools/common",
			"tools/linker",
			"src/ObjCRuntime/Registrar.cs",
			"mk/mono.mk",
			"msbuild",
		};
		static readonly string[] bclPrefixes = {
			"tests/bcl-test",
			"tests/common",
			"mk/mono.mk",
		};
		static readonly string [] btouchPrefixes = {
			"src/btouch.cs",
			"src/generator.cs",
			"src/generator-",
			"src/Makefile.generator",
			"tests/bgen",
			"tests/generator",
			"tests/common",
		};
		static readonly string [] macBindingProject = new [] {
			"msbuild",
			"tests/mac-binding-project",
			"tests/common/mac",
		}.Intersect (btouchPrefixes).ToArray ();
		
		static readonly string [] xtroPrefixes = {
			"tests/xtro-sharpie",
			"src",
			"Make.config",
		};
		static readonly string [] cecilPrefixes = {
			"tests/cecil-tests",
			"src",
			"Make.config",
		};
		static readonly string [] dotnetFilenames = {
			"msbuild",
			".*dotnet.*",
			"eng", // bumping .NET modifies files in this directory
		};
		static readonly string [] msbuildFilenames = {
			"msbuild",
			"tests/msbuild",
		};

		static readonly string [] xharnessPrefix = {
			"tests/xharness",
		};

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
		
		// 'filenames' is a list of filename prefixes, unless the name has a star character, in which case it's interpreted as a regex expression.
		void SetEnabled (IEnumerable<string> files, string [] filenames, string testname, TestSelection selection)
		{
			MainLog?.WriteLine ($"Checking if test {testname} should be enabled according to the modified files.");

			// Compute any regexes we might need out of the loop.
			var regexes = new Regex [filenames.Length];
			for (var i = 0; i < filenames.Length; i++) {
				// If the prefix contains a star, treat it is as a regex.
				if (filenames [i].IndexOf ('*') == -1)
					continue;

				var regex = new Regex (filenames [i]);
				regexes [i] = regex;
			}

			foreach (var file in files) {
				MainLog?.WriteLine ($"Checking for file {file}"); 
				for (var i = 0; i < filenames.Length; i++) {
					var prefix = filenames [i];
					if (file.StartsWith (prefix, StringComparison.Ordinal)) {
						selection.SetEnabled (testname, true);
						MainLog?.WriteLine ("Enabled '{0}' tests because the modified file '{1}' matches prefix '{2}'", testname, file, prefix);
						return;
					}

					if (regexes [i]?.IsMatch (file) == true) {
						selection.SetEnabled (testname, true);
						MainLog?.WriteLine ("Enabled '{0}' tests because the modified file '{1}' matches regex '{2}'", testname, file, prefix);
						return;
					}
				}
			}
		}
		
		// Returns true if the value was changed.
		bool SetEnabled (HashSet<string> labels, string testname, TestSelection selection)
		{
			if (labels.Contains ("skip-" + testname + "-tests")) {
				MainLog?.WriteLine ("Disabled '{0}' tests because the label 'skip-{0}-tests' is set.", testname);
				if (testname == "ios") {
					selection.SetEnabled (PlatformLabel.iOS64, false);
					selection.SetEnabled (PlatformLabel.iOS32, false);
				}

				selection.SetEnabled (testname, false);
				return true;
			}

			if (labels.Contains ("run-" + testname + "-tests")) {
				MainLog?.WriteLine ("Enabled '{0}' tests because the label 'run-{0}-tests' is set.", testname);
				if (testname == "ios") {
					selection.SetEnabled (PlatformLabel.iOS64, true);
					selection.SetEnabled (PlatformLabel.iOS32, true);
				}

				selection.SetEnabled (testname, true);
				return true;
			}

			if (labels.Contains ("skip-all-tests")) {
				MainLog?.WriteLine ("Disabled '{0}' tests because the label 'skip-all-tests' is set.", testname);
				selection.SelectedTests = TestLabel.None;
				return true;
			}

			if (labels.Contains ("run-all-tests")) {
				MainLog?.WriteLine ("Enabled '{0}' tests because the label 'run-all-tests' is set.", testname);
				selection.SelectedTests = TestLabel.All;
				return true;
			}
			// respect any default value
			return false;
		}

		void SelectTestsByModifiedFiles (int pullRequest, TestSelection selection)
		{
			// toArray so that we do not always enumerate all the time.
			var files = vcs.GetModifiedFiles (pullRequest).ToArray ();

			MainLog?.WriteLine ("Found {0} modified file(s) in the pull request #{1}.", files.Count (), pullRequest);
			foreach (var f in files)
				MainLog?.WriteLine ("    {0}", f);
			
			SetEnabled (files, mtouchPrefixes, "mtouch", selection);
			SetEnabled (files, mmpPrefixes, "mmp", selection);
			SetEnabled (files, bclPrefixes, "bcl", selection);
			SetEnabled (files, btouchPrefixes, "btouch", selection);
			SetEnabled (files, macBindingProject, "mac-binding-project", selection);
			SetEnabled (files, xtroPrefixes, "xtro", selection);
			SetEnabled (files, cecilPrefixes, "cecil", selection);
			SetEnabled (files, dotnetFilenames, "dotnet", selection);
			SetEnabled (files, msbuildFilenames, "msbuild", selection);
			SetEnabled (files, xharnessPrefix, "all", selection);
		}

		IEnumerable<string> GetPullRequestLabels (int pullRequest)
		{
			if (pullRequest > 0) {
				var labels = vcs.GetLabels (pullRequest);
				if (labels.Any ()) {
					MainLog?.WriteLine ($"Found {labels.Count ()} label(s) in the pull request #{pullRequest}: {string.Join (", ", labels)}");
					return labels;
				}
			}

			MainLog?.WriteLine ($"No labels were found in the pull request #{pullRequest}.");
			return Array.Empty<string> ();
		}

		IEnumerable<string> GetEnviromentLabels ()
		{
			var envLabels = Environment.GetEnvironmentVariable ("XHARNESS_LABELS");
			if (!string.IsNullOrEmpty (envLabels)) {
				var labels = envLabels.Split (new char [] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				MainLog?.WriteLine ($"Found {labels.Count ()} label(s) in the environment variable XHARNESS_LABELS: {string.Join (", ", labels)}");
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
				if (customLabels.Count () > 0) {
					MainLog?.WriteLine ($"Found {customLabels.Count ()} label(s) in {customLabelsFile}: {string.Join (", ", customLabels)}");
					return customLabels;
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
			labels.UnionWith (GetEnviromentLabels ());
			labels.UnionWith (GetCustomFileLabels ());
			
			MainLog?.WriteLine ($"In total found {labels.Count ()} label(s): {string.Join (", ", labels.ToArray ())}");
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
			var regexp = new Regex("(skip-|run-)(.*)(-tests)");
			foreach (var label in labels) {
				// use a regexp to parse the label, the possibilities are
				// skip-*-tests
				// run-*-tests
				// if we have a match, we decide if we skip or run and set the label accordingly
				if (!regexp.IsMatch (label))
					continue;
				MainLog?.WriteLine ($"Label {label} matches regexp.");
				var match = regexp.Match (label);
				var run = match.Groups [1].Value == "run-";
				MainLog?.WriteLine ($"Setting label {match.Groups [2].Value} to be enabled: {run}");
				labelSelectedTests.Add (match.Groups[2].Value, run);
				MainLog?.WriteLine ($"Selected tests are {selection.SelectedTests}");
			}

			// special case to consider, since might set or remove all tests, we need to process it first
			if (labelSelectedTests.ContainsKey ("all")) {
				selection.SetEnabled ("all", labelSelectedTests["all"]);
				labelSelectedTests.Remove ("all");
			}

			foreach (var entry in labelSelectedTests) {
				selection.SetEnabled (entry.Key, entry.Value);
			}

			Harness.IncludeSystemPermissionTests = selection.IsEnabled (TestLabel.SystemPermission); 

			// docs is a bit special:
			// - can only be executed if the Xamarin-specific parts of the build is enabled
			// - enabled by default if the current branch is main (or, for a pull request, if the target branch is main)
			if (Harness.ENABLE_XAMARIN) {
				if (!labelSelectedTests.ContainsKey ("docs")) { // don't override any value set using labels
					var branchName = Environment.GetEnvironmentVariable ("BRANCH_NAME");
					if (!string.IsNullOrEmpty (branchName)) {
						selection.SetEnabled (TestLabel.Docs, branchName == "main");
						if (selection.IsEnabled (TestLabel.Docs))
							MainLog?.WriteLine ("Enabled 'docs' tests because the current branch is 'main'.");
					} else if (prTarget is not null) {
						selection.SetEnabled (TestLabel.Docs, prTarget == "main");
						if (selection.IsEnabled (TestLabel.Docs))
							MainLog?.WriteLine ("Enabled 'docs' tests because the target branch is 'main'.");
					}
				}
			} else {
				if (selection.IsEnabled (TestLabel.Docs)) {
					selection.SetEnabled (TestLabel.Docs, false); // could have been enabled by 'run-all-tests', so disable it if we can't run it.
					MainLog?.WriteLine ("Disabled 'docs' tests because the Xamarin-specific parts of the build are not enabled.");
				}
			}

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

			// First check if can auto-select any tests based on which files were modified.
			// This will only enable additional tests, never disable tests
// TODO: This was disabled for testing.	
//			if (pullRequest > 0)
//				SelectTestsByModifiedFiles (pullRequest, selection);
			
			// Then we check for labels. Labels are manually set, so those override
			// whatever we did automatically.
			var labels = GetAllLabels (pullRequest);
			SelectTestsByLabel (labels, selection, pullRequest > 0 ?
				vcs.GetPullRequestTargetBranch (pullRequest): null);

			DisableKnownFailingDeviceTests ();

			if (!Harness.INCLUDE_IOS) {
				MainLog?.WriteLine ("The iOS build is disabled, so any iOS tests will be disabled as well.");
				selection.SetEnabled (PlatformLabel.iOS, false);
				selection.SetEnabled (PlatformLabel.iOS64, false);
				selection.SetEnabled (PlatformLabel.iOS32, false);
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

			if (!Harness.ENABLE_DOTNET) {
				MainLog?.WriteLine ("The .NET build is disabled, so any .NET tests will be disabled as well.");
				selection.SetEnabled (PlatformLabel.Dotnet, false);
			}
		}
	}
}
