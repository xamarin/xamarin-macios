#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.DotNet.XHarness.Common.Logging;

namespace Xharness.Jenkins {

	class TestSelection {
		TestLabel selection =
			TestLabel.None |
			TestLabel.tvOS |
			TestLabel.watchOS |
			TestLabel.Msbuild |
			TestLabel.iOSSimulator |
			TestLabel.Monotouch |
			TestLabel.NonMonotouch |
			TestLabel.Dotnet |
			TestLabel.MacCatalyst;

		public bool ForceExtensionBuildOnly { get; set; }

		public void SetEnabled (TestLabel label, bool enable)
		{
			if (enable) {
				selection |= label;
			} else {
				selection &= ~label;
			}
		}
		
		public void SetEnabled (string label, bool value)
		{
			var testLabel = label.GetLabel ();
			SetEnabled (testLabel, value);
		}

		public bool IsEnabled (TestLabel label)
			=> selection.HasFlag (label);
	}
	/// <summary>
	/// Allows to select the tests to be ran depending on certain conditions such as labels of modified files.
	/// </summary>
	class TestSelector {

		#region private vars
		
		readonly Jenkins jenkins;
		readonly IVersionControlSystem vcs;

		ILog MainLog => jenkins.MainLog;
		IHarness Harness => jenkins.Harness;
		
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
			MainLog.WriteLine ($"Checking if test {testname} should be enabled according to the modified files.");

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
				MainLog.WriteLine ($"Checking for file {file}"); 
				for (var i = 0; i < filenames.Length; i++) {
					var prefix = filenames [i];
					if (file.StartsWith (prefix, StringComparison.Ordinal)) {
						selection.SetEnabled (testname, true);
						MainLog.WriteLine ("Enabled '{0}' tests because the modified file '{1}' matches prefix '{2}'", testname, file, prefix);
						return;
					}

					if (regexes [i]?.IsMatch (file) == true) {
						selection.SetEnabled (testname, true);
						MainLog.WriteLine ("Enabled '{0}' tests because the modified file '{1}' matches regex '{2}'", testname, file, prefix);
						return;
					}
				}
			}
		}
		
		// Returns true if the value was changed.
		bool SetEnabled (HashSet<string> labels, string testname, TestSelection selection)
		{
			if (labels.Contains ("skip-" + testname + "-tests")) {
				MainLog.WriteLine ("Disabled '{0}' tests because the label 'skip-{0}-tests' is set.", testname);
				if (testname == "ios") {
					selection.SetEnabled (TestLabel.iOS64, false);
					selection.SetEnabled (TestLabel.iOS32, false);
				}

				selection.SetEnabled (testname, false);
				return true;
			}

			if (labels.Contains ("run-" + testname + "-tests")) {
				MainLog.WriteLine ("Enabled '{0}' tests because the label 'run-{0}-tests' is set.", testname);
				if (testname == "ios") {
					selection.SetEnabled (TestLabel.iOS64, true);
					selection.SetEnabled (TestLabel.iOS32, true);
				}

				selection.SetEnabled (testname, true);
				return true;
			}

			if (labels.Contains ("skip-all-tests")) {
				MainLog.WriteLine ("Disabled '{0}' tests because the label 'skip-all-tests' is set.", testname);
				selection.SetEnabled (testname, false);
				return true;
			}

			if (labels.Contains ("run-all-tests")) {
				MainLog.WriteLine ("Enabled '{0}' tests because the label 'run-all-tests' is set.", testname);
				selection.SetEnabled (testname, true);
				return true;
			}
			// respect any default value
			return false;
		}

		void SelectTestsByModifiedFiles (int pullRequest, TestSelection selection)
		{
			// toArray so that we do not always enumerate all the time.
			var files = vcs.GetModifiedFiles (pullRequest).ToArray ();

			MainLog.WriteLine ("Found {0} modified file(s) in the pull request #{1}.", files.Count (), pullRequest);
			foreach (var f in files)
				MainLog.WriteLine ("    {0}", f);
			
			SetEnabled (files, mtouchPrefixes, "mtouch", selection);
			SetEnabled (files, mmpPrefixes, "mmp", selection);
			SetEnabled (files, bclPrefixes, "bcl", selection);
			SetEnabled (files, btouchPrefixes, "btouch", selection);
			SetEnabled (files, macBindingProject, "mac-binding-project", selection);
			SetEnabled (files, xtroPrefixes, "xtro", selection);
			SetEnabled (files, cecilPrefixes, "cecil", selection);
			SetEnabled (files, dotnetFilenames, "dotnet", selection);
			SetEnabled (files, msbuildFilenames, "msbuild", selection);
			// xharness will run all tests, but we do not want to run the device tests
			// if they were not selected
			var devicesEnabled = selection.IsEnabled (TestLabel.Device);
			SetEnabled (files, xharnessPrefix, "all", selection);
			selection.SetEnabled (TestLabel.Device, devicesEnabled);
		}

		void SelectTestsByLabel (int pullRequest, TestSelection selection)
		{
			var labels = new HashSet<string> ();
			if (Harness.Labels.Any ()) {
				labels.UnionWith (Harness.Labels);
				MainLog.WriteLine ($"{Harness.Labels.Count} label(s) were passed on the command line.");
			} else {
				MainLog.WriteLine ($"No labels were passed on the command line.");
			}
			if (pullRequest > 0) {
				var lbls = vcs.GetLabels (pullRequest);
				if (lbls.Any ()) {
					labels.UnionWith (lbls);
					MainLog.WriteLine ($"Found {lbls.Count ()} label(s) in the pull request #{pullRequest}: {string.Join (", ", lbls)}");
				} else {
					MainLog.WriteLine ($"No labels were found in the pull request #{pullRequest}.");
				}
			}
			var env_labels = Environment.GetEnvironmentVariable ("XHARNESS_LABELS");
			if (!string.IsNullOrEmpty (env_labels)) {
				var lbls = env_labels.Split (new char [] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				labels.UnionWith (lbls);
				MainLog.WriteLine ($"Found {lbls.Count ()} label(s) in the environment variable XHARNESS_LABELS: {string.Join (", ", lbls)}");
			} else {
				MainLog.WriteLine ($"No labels were in the environment variable XHARNESS_LABELS.");
			}

			var custom_labels_file = Path.Combine (HarnessConfiguration.RootDirectory, "..", "jenkins", "custom-labels.txt");
			if (File.Exists (custom_labels_file)) {
				var custom_labels = File.ReadAllLines (custom_labels_file).Select ((v) => v.Trim ()).Where (v => v.Length > 0 && v [0] != '#');
				if (custom_labels.Count () > 0) {
					labels.UnionWith (custom_labels);
					MainLog.WriteLine ($"Found {custom_labels.Count ()} label(s) in {custom_labels_file}: {string.Join (", ", custom_labels)}");
				} else {
					MainLog.WriteLine ($"No labels were in {custom_labels_file}.");
				}
			} else {
				MainLog.WriteLine ($"The custom labels file {custom_labels_file} does not exist.");
			}

			MainLog.WriteLine ($"In total found {labels.Count ()} label(s): {string.Join (", ", labels.ToArray ())}");

			// disabled by default
			SetEnabled (labels, "mtouch", selection); 
			SetEnabled (labels, "mmp", selection); 
			SetEnabled (labels, "bcl", selection); 
			SetEnabled (labels, "bcl-xunit", selection); 
			SetEnabled (labels, "bcl-nunit", selection); 
			SetEnabled (labels, "mscorlib", selection); 
			SetEnabled (labels, "btouch", selection); 
			SetEnabled (labels, "mac-binding-project", selection); 
			SetEnabled (labels, "ios-extensions", selection); 
			SetEnabled (labels, "device", selection); 
			SetEnabled (labels, "xtro", selection); 
			SetEnabled (labels, "cecil", selection); 
			SetEnabled (labels, "old-simulator", selection); 
			SetEnabled (labels, "dotnet", selection); 
			SetEnabled (labels, "all", selection); 

			// enabled by default
			SetEnabled (labels, "ios-32",  selection); 
			SetEnabled (labels, "ios-64", selection); 
			SetEnabled (labels, "ios", selection); 
			SetEnabled (labels, "tvos", selection); 
			SetEnabled (labels, "watchos", selection); 
			SetEnabled (labels, "mac", selection); 
			SetEnabled (labels, "msbuild", selection); 
			SetEnabled (labels, "ios-simulator", selection); 
			SetEnabled (labels, "non-monotouch", selection); 
			SetEnabled (labels, "monotouch", selection); 

			if (SetEnabled (labels, "system-permission", selection))
				Harness.IncludeSystemPermissionTests = selection.IsEnabled (TestLabel.SystemPermission); 

			// docs is a bit special:
			// - can only be executed if the Xamarin-specific parts of the build is enabled
			// - enabled by default if the current branch is main (or, for a pull request, if the target branch is main)
			var changed = SetEnabled (labels, "docs", selection);
			if (Harness.ENABLE_XAMARIN) {
				if (!changed) { // don't override any value set using labels
					var branchName = Environment.GetEnvironmentVariable ("BRANCH_NAME");
					if (!string.IsNullOrEmpty (branchName)) {
						selection.SetEnabled (TestLabel.Docs, branchName == "main");
						if (selection.IsEnabled (TestLabel.Docs))
							MainLog.WriteLine ("Enabled 'docs' tests because the current branch is 'main'.");
					} else if (pullRequest > 0) {
						selection.SetEnabled (TestLabel.Docs, vcs.GetPullRequestTargetBranch (pullRequest) == "main");
						if (selection.IsEnabled (TestLabel.Docs))
							MainLog.WriteLine ("Enabled 'docs' tests because the target branch is 'main'.");
					}
				}
			} else {
				if (selection.IsEnabled (TestLabel.Docs)) {
					selection.SetEnabled (TestLabel.Docs, false); // could have been enabled by 'run-all-tests', so disable it if we can't run it.
					MainLog.WriteLine ("Disabled 'docs' tests because the Xamarin-specific parts of the build are not enabled.");
				}
			}

			// old simulator tests is also a bit special:
			// - enabled by default if using a beta Xcode, otherwise disabled by default
			changed = SetEnabled (labels, "old-simulator", selection);
			if (!changed && jenkins.IsBetaXcode) {
				selection.SetEnabled (TestLabel.OldiOSSimulator, true);
				MainLog.WriteLine ("Enabled 'old-simulator' tests because we're using a beta Xcode.");
			}
		}
		
		public void SelectTests (TestSelection selection)
		{
			if (!int.TryParse (Environment.GetEnvironmentVariable ("PR_ID"), out int pullRequest))
				MainLog.WriteLine ("The environment variable 'PR_ID' was not found, so no pull requests will be checked for test selection.");

			// First check if can auto-select any tests based on which files were modified.
			// This will only enable additional tests, never disable tests.
			if (pullRequest > 0)
				SelectTestsByModifiedFiles (pullRequest, selection);
			
			// Then we check for labels. Labels are manually set, so those override
			// whatever we did automatically.
			SelectTestsByLabel (pullRequest, selection);

			DisableKnownFailingDeviceTests ();

			if (!Harness.INCLUDE_IOS) {
				MainLog.WriteLine ("The iOS build is disabled, so any iOS tests will be disabled as well.");
				selection.SetEnabled (TestLabel.iOS, false);
				selection.SetEnabled (TestLabel.iOS64, false);
				selection.SetEnabled (TestLabel.iOS32, false);
			}

			if (!Harness.INCLUDE_WATCH) {
				MainLog.WriteLine ("The watchOS build is disabled, so any watchOS tests will be disabled as well.");
				selection.SetEnabled (TestLabel.watchOS, false);
			}

			if (!Harness.INCLUDE_TVOS) {
				MainLog.WriteLine ("The tvOS build is disabled, so any tvOS tests will be disabled as well.");
				selection.SetEnabled (TestLabel.tvOS, false);
			}

			if (!Harness.INCLUDE_MAC) {
				MainLog.WriteLine ("The macOS build is disabled, so any macOS tests will be disabled as well.");
				selection.SetEnabled (TestLabel.Mac, false);
			}

			if (!Harness.ENABLE_DOTNET) {
				MainLog.WriteLine ("The .NET build is disabled, so any .NET tests will be disabled as well.");
				selection.SetEnabled (TestLabel.Dotnet, false);
			}
		}
	}
}
