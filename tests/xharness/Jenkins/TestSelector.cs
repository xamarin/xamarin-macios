#nullable enable

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Microsoft.DotNet.XHarness.iOS.Shared.Logging;

namespace Xharness.Jenkins {
	
	/// <summary>
	/// Allows to select the tests to be ran depending on certain conditions such as labels of modified files.
	/// </summary>
	class TestSelector {

		#region private vars
		
		readonly Jenkins jenkins;
		readonly IVersionControlSystem vcs;

		ILog MainLog => jenkins.MainLog;
		IHarness Harness => jenkins.Harness;

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
		
		static readonly Dictionary<TestSelection, string[]> selectionPrefixesMap = new Dictionary<TestSelection, string []> {
			[TestSelection.Mtouch] = new [] {
				"tests/mtouch",
				"tests/common",
				"tools/mtouch",
				"tools/common",
				"tools/linker",
				"src/ObjCRuntime/Registrar.cs",
				"mk/mono.mk",
				"msbuild",
				"runtime"},
			[TestSelection.Mmp] = new [] {
				"tests/mmptest",
				"tests/common",
				"tools/mmp",
				"tools/common",
				"tools/linker",
				"src/ObjCRuntime/Registrar.cs",
				"mk/mono.mk",
				"msbuild"},
			[TestSelection.Bcl] = new [] {
				"tests/bcl-test",
				"tests/common",
				"mk/mono.mk"},
			[TestSelection.Btouch] = btouchPrefixes,
			[TestSelection.MacBindingProject] = macBindingProject,
			[TestSelection.Xtro] = new [] {
				"tests/xtro-sharpie",
				"src",
				"Make.config"},
			[TestSelection.Cecil] = new [] {
				"tests/cecil-tests",
				"src",
				"Make.config"},
			[TestSelection.DotNet] = new [] {
				"dotnet",
				"msbuild",
				"tests/dotnet"},
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
		
		void SetEnabled (IEnumerable<string> files, TestSelection selection, string [] prefixes)
		{
			MainLog.WriteLine ($"Checking if test {selection.ToString ()} should be enabled according to the modified files.");
			foreach (var file in files) {
				MainLog.WriteLine ($"Checking for file {file}"); 
				foreach (var prefix in prefixes) {
					if (file.StartsWith (prefix, StringComparison.Ordinal)) {
						jenkins.TestSelection |= selection;
						MainLog.WriteLine ("Enabled '{0}' tests because the modified file '{1}' matches prefix '{2}'", selection.ToString (), file, prefix);
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
				jenkins.TestSelection &= ~selection;
				return true;
			} else if (labels.Contains ("run-" + testname + "-tests")) {
				MainLog.WriteLine ("Enabled '{0}' tests because the label 'run-{0}-tests' is set.", testname);
				jenkins.TestSelection |= selection;
				return true;
			}
			// respect any default value
			return false;
		}

		internal void SelectTestsByModifiedFiles (int pullRequest)
		{
			// toArray so that we do not always enumerate all the time.
			var files = vcs.GetModifiedFiles (pullRequest).ToArray ();

			MainLog.WriteLine ("Found {0} modified file(s) in the pull request #{1}.", files.Count (), pullRequest);
			foreach (var f in files)
				MainLog.WriteLine ("    {0}", f);
			
			foreach (var selection in selectionPrefixesMap.Keys) {
				SetEnabled (files, selection, selectionPrefixesMap[selection]);
			}
		}

		internal void SelectTestsByLabel (int pullRequest)
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
			SetEnabled (labels, "mtouch", TestSelection.Mtouch);
			SetEnabled (labels, "mmp", TestSelection.Mmp);
			SetEnabled (labels, "bcl", TestSelection.Bcl);
			SetEnabled (labels, "bcl-xunit", TestSelection.BclXUnit);
			SetEnabled (labels, "bcl-nunit", TestSelection.BclNUnit);
			SetEnabled (labels, "mscorlib", TestSelection.Mscorlib);
			SetEnabled (labels, "btouch", TestSelection.Btouch);
			SetEnabled (labels, "mac-binding-project", TestSelection.MacBindingProject);
			SetEnabled (labels, "ios-extensions", TestSelection.iOSExtensions);
			SetEnabled (labels, "device", TestSelection.Device);
			SetEnabled (labels, "xtro", TestSelection.Xtro);
			SetEnabled (labels, "cecil", TestSelection.Cecil);
			SetEnabled (labels, "old-simulator", TestSelection.OldiOSSimulator);
			SetEnabled (labels, "all", TestSelection.All);

			// enabled by default
			SetEnabled (labels, "ios-32", TestSelection.iOS32);
			SetEnabled (labels, "ios-64", TestSelection.iOS64);
			SetEnabled (labels, "ios", TestSelection.iOS); // Needs to be set after `ios-32` and `ios-64` (because it can reset them)
			SetEnabled (labels, "tvos", TestSelection.tvOS);
			SetEnabled (labels, "watchos", TestSelection.watchOS);
			SetEnabled (labels, "mac", TestSelection.MacOS);
			SetEnabled (labels, "ios-msbuild", TestSelection.iOSMSBuild);
			SetEnabled (labels, "ios-simulator", TestSelection.Simulator);
			SetEnabled (labels, "non-monotouch", TestSelection.NonMonotouch);
			SetEnabled (labels, "monotouch", TestSelection.Monotouch);

			if (SetEnabled (labels, "system-permission", TestSelection.SystemPermissionTests))
				Harness.IncludeSystemPermissionTests = jenkins.TestSelection.HasFlag (TestSelection.SystemPermissionTests);

			// docs is a bit special:
			// - can only be executed if the Xamarin-specific parts of the build is enabled
			// - enabled by default if the current branch is main (or, for a pull request, if the target branch is main)
			var changed = SetEnabled (labels, "docs", TestSelection.Docs);
			if (Harness.ENABLE_XAMARIN) {
				if (!changed) { // don't override any value set using labels
					var branchName = Environment.GetEnvironmentVariable ("BRANCH_NAME");
					var logMessage = "Enabled 'docs' tests because the current branch is 'main'.";
					if (string.IsNullOrEmpty (branchName) && pullRequest > 0) {
						branchName = vcs.GetPullRequestTargetBranch (pullRequest);
						logMessage = "Enabled 'docs' tests because the target branch is 'main'.";
					}
					if (!string.IsNullOrEmpty(branchName) && branchName == "main") {
							jenkins.TestSelection |= TestSelection.Docs;
							MainLog.WriteLine (logMessage);
					}
				}
			} else {
				if (jenkins.TestSelection.HasFlag(TestSelection.Docs)) {
					jenkins.TestSelection &= ~TestSelection.Docs; // could have been enabled by 'run-all-tests', so disable it if we can't run it.
					MainLog.WriteLine ("Disabled 'docs' tests because the Xamarin-specific parts of the build are not enabled.");
				}
			}

			// old simulator tests is also a bit special:
			// - enabled by default if using a beta Xcode, otherwise disabled by default
			changed = SetEnabled (labels, "old-simulator", TestSelection.OldiOSSimulator);
			if (!changed && jenkins.IsBetaXcode) {
				jenkins.TestSelection |= TestSelection.OldiOSSimulator;
				MainLog.WriteLine ("Enabled 'old-simulator' tests because we're using a beta Xcode.");
			}
		}
		
		public void SelectTests ()
		{
			if (!int.TryParse (Environment.GetEnvironmentVariable ("ghprbPullId"), out int pullRequest))
				MainLog.WriteLine ("The environment variable 'ghprbPullId' was not found, so no pull requests will be checked for test selection.");

			// First check if can auto-select any tests based on which files were modified.
			// This will only enable additional tests, never disable tests.
			if (pullRequest > 0)
				SelectTestsByModifiedFiles (pullRequest);
			
			// Then we check for labels. Labels are manually set, so those override
			// whatever we did automatically.
			SelectTestsByLabel (pullRequest);

			DisableKnownFailingDeviceTests ();

			if (!Harness.INCLUDE_IOS) {
				MainLog.WriteLine ("The iOS build is disabled, so any iOS tests will be disabled as well.");
				jenkins.TestSelection &= ~TestSelection.iOS;
			}

			if (!Harness.INCLUDE_WATCH) {
				MainLog.WriteLine ("The watchOS build is disabled, so any watchOS tests will be disabled as well.");
				jenkins.TestSelection &= ~TestSelection.watchOS;
			}

			if (!Harness.INCLUDE_TVOS) {
				MainLog.WriteLine ("The tvOS build is disabled, so any tvOS tests will be disabled as well.");
				jenkins.TestSelection &= ~TestSelection.tvOS;
			}

			if (!Harness.INCLUDE_MAC) {
				MainLog.WriteLine ("The macOS build is disabled, so any macOS tests will be disabled as well.");
				jenkins.TestSelection &= ~TestSelection.MacOS;
			}
		}
	}
}
