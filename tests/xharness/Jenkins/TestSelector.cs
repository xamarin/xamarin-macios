#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.DotNet.XHarness.Common.Execution;
using Microsoft.DotNet.XHarness.Common.Logging;

namespace Xharness.Jenkins {

	/// <summary>
	/// Allows to select the tests to be ran depending on certain conditions such as labels of modified files.
	/// </summary>
	class TestSelector {

		#region private vars
		
		readonly Jenkins jenkins;
		readonly IProcessManager processManager;
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
		};
		static readonly string [] msbuildFilenames = {
			"msbuild",
			"tests/msbuild",
		};

		#endregion

		public TestSelector (Jenkins jenkins, IProcessManager processManager, IVersionControlSystem versionControlSystem)
		{
			this.jenkins = jenkins;
			this.processManager = processManager;
			this.vcs = versionControlSystem;
		}
		
		void DisableKnownFailingDeviceTests ()
		{
			// https://github.com/xamarin/maccore/issues/1008
			jenkins.ForceExtensionBuildOnly = true;
		}
		
		// 'filenames' is a list of filename prefixes, unless the name has a star character, in which case it's interpreted as a regex expression.
		void SetEnabled (IEnumerable<string> files, string [] filenames, string testname, ref bool value)
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
						value = true;
						MainLog.WriteLine ("Enabled '{0}' tests because the modified file '{1}' matches prefix '{2}'", testname, file, prefix);
						return;
					} else if (regexes [i]?.IsMatch (file) == true) {
						value = true;
						MainLog.WriteLine ("Enabled '{0}' tests because the modified file '{1}' matches regex '{2}'", testname, file, prefix);
						return;
					}
				}
			}
		}
		
		// Returns true if the value was changed.
		bool SetEnabled (HashSet<string> labels, string testname, ref bool value)
		{
			if (labels.Contains ("skip-" + testname + "-tests")) {
				MainLog.WriteLine ("Disabled '{0}' tests because the label 'skip-{0}-tests' is set.", testname);
				if (testname == "ios")
					jenkins.IncludeiOS32 = jenkins.IncludeiOS64 = false;
				value = false;
				return true;
			} else if (labels.Contains ("run-" + testname + "-tests")) {
				MainLog.WriteLine ("Enabled '{0}' tests because the label 'run-{0}-tests' is set.", testname);
				if (testname == "ios")
					jenkins.IncludeiOS32 = jenkins.IncludeiOS64 = true;
				value = true;
				return true;
			} else if (labels.Contains ("skip-all-tests")) {
				MainLog.WriteLine ("Disabled '{0}' tests because the label 'skip-all-tests' is set.", testname);
				value = false;
				return true;
			} else if (labels.Contains ("run-all-tests")) {
				MainLog.WriteLine ("Enabled '{0}' tests because the label 'run-all-tests' is set.", testname);
				value = true;
				return true;
			}
			// respect any default value
			return false;
		}

		void SelectTestsByModifiedFiles (int pullRequest)
		{
			// toArray so that we do not always enumerate all the time.
			var files = vcs.GetModifiedFiles (pullRequest).ToArray ();

			MainLog.WriteLine ("Found {0} modified file(s) in the pull request #{1}.", files.Count (), pullRequest);
			foreach (var f in files)
				MainLog.WriteLine ("    {0}", f);
			
			SetEnabled (files, mtouchPrefixes, "mtouch", ref jenkins.IncludeMtouch);
			SetEnabled (files, mmpPrefixes, "mmp", ref jenkins.IncludeMmpTest);
			SetEnabled (files, bclPrefixes, "bcl", ref jenkins.IncludeBcl);
			SetEnabled (files, btouchPrefixes, "btouch", ref jenkins.IncludeBtouch);
			SetEnabled (files, macBindingProject, "mac-binding-project", ref jenkins.IncludeMacBindingProject);
			SetEnabled (files, xtroPrefixes, "xtro", ref jenkins.IncludeXtro);
			SetEnabled (files, cecilPrefixes, "cecil", ref jenkins.IncludeCecil);
			SetEnabled (files, dotnetFilenames, "dotnet", ref jenkins.IncludeDotNet);
			SetEnabled (files, msbuildFilenames, "msbuild", ref jenkins.IncludeMSBuild);
		}

		void SelectTestsByLabel (int pullRequest)
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
			SetEnabled (labels, "mtouch", ref jenkins.IncludeMtouch);
			SetEnabled (labels, "mmp", ref jenkins.IncludeMmpTest);
			SetEnabled (labels, "bcl", ref jenkins.IncludeBcl);
			SetEnabled (labels, "bcl-xunit", ref jenkins.IncludeBCLxUnit);
			SetEnabled (labels, "bcl-nunit", ref jenkins.IncludeBCLNUnit);
			SetEnabled (labels, "mscorlib", ref jenkins.IncludeMscorlib);
			SetEnabled (labels, "btouch", ref jenkins.IncludeBtouch);
			SetEnabled (labels, "mac-binding-project", ref jenkins.IncludeMacBindingProject);
			SetEnabled (labels, "ios-extensions", ref jenkins.IncludeiOSExtensions);
			SetEnabled (labels, "device", ref jenkins.IncludeDevice);
			SetEnabled (labels, "xtro", ref jenkins.IncludeXtro);
			SetEnabled (labels, "cecil", ref jenkins.IncludeCecil);
			SetEnabled (labels, "old-simulator", ref jenkins.IncludeOldSimulatorTests);
			SetEnabled (labels, "dotnet", ref jenkins.IncludeDotNet);
			SetEnabled (labels, "all", ref jenkins.IncludeAll);

			// enabled by default
			SetEnabled (labels, "ios-32", ref jenkins.IncludeiOS32);
			SetEnabled (labels, "ios-64", ref jenkins.IncludeiOS64);
			SetEnabled (labels, "ios", ref jenkins.IncludeiOS); // Needs to be set after `ios-32` and `ios-64` (because it can reset them)
			SetEnabled (labels, "tvos", ref jenkins.IncludetvOS);
			SetEnabled (labels, "watchos", ref jenkins.IncludewatchOS);
			SetEnabled (labels, "mac", ref jenkins.IncludeMac);
			SetEnabled (labels, "msbuild", ref jenkins.IncludeMSBuild);
			SetEnabled (labels, "ios-simulator", ref jenkins.IncludeSimulator);
			SetEnabled (labels, "non-monotouch", ref jenkins.IncludeNonMonotouch);
			SetEnabled (labels, "monotouch", ref jenkins.IncludeMonotouch);

			bool inc_permission_tests = false;
			if (SetEnabled (labels, "system-permission", ref inc_permission_tests))
				Harness.IncludeSystemPermissionTests = inc_permission_tests;

			// docs is a bit special:
			// - can only be executed if the Xamarin-specific parts of the build is enabled
			// - enabled by default if the current branch is main (or, for a pull request, if the target branch is main)
			var changed = SetEnabled (labels, "docs", ref jenkins.IncludeDocs);
			if (Harness.ENABLE_XAMARIN) {
				if (!changed) { // don't override any value set using labels
					var branchName = Environment.GetEnvironmentVariable ("BRANCH_NAME");
					if (!string.IsNullOrEmpty (branchName)) {
						jenkins.IncludeDocs = branchName == "main";
						if (jenkins.IncludeDocs)
							MainLog.WriteLine ("Enabled 'docs' tests because the current branch is 'main'.");
					} else if (pullRequest > 0) {
						jenkins.IncludeDocs = vcs.GetPullRequestTargetBranch (pullRequest) == "main";
						if (jenkins.IncludeDocs)
							MainLog.WriteLine ("Enabled 'docs' tests because the target branch is 'main'.");
					}
				}
			} else {
				if (jenkins.IncludeDocs) {
					jenkins.IncludeDocs = false; // could have been enabled by 'run-all-tests', so disable it if we can't run it.
					MainLog.WriteLine ("Disabled 'docs' tests because the Xamarin-specific parts of the build are not enabled.");
				}
			}

			// old simulator tests is also a bit special:
			// - enabled by default if using a beta Xcode, otherwise disabled by default
			changed = SetEnabled (labels, "old-simulator", ref jenkins.IncludeOldSimulatorTests);
			if (!changed && jenkins.IsBetaXcode) {
				jenkins.IncludeOldSimulatorTests = true;
				MainLog.WriteLine ("Enabled 'old-simulator' tests because we're using a beta Xcode.");
			}
		}
		
		public void SelectTests ()
		{
			if (!int.TryParse (Environment.GetEnvironmentVariable ("PrID"), out int pullRequest))
				MainLog.WriteLine ("The environment variable 'PrID' was not found, so no pull requests will be checked for test selection.");

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
				jenkins.IncludeiOS = false;
				jenkins.IncludeiOS64 = false;
				jenkins.IncludeiOS32 = false;
			}

			if (!Harness.INCLUDE_WATCH) {
				MainLog.WriteLine ("The watchOS build is disabled, so any watchOS tests will be disabled as well.");
				jenkins.IncludewatchOS = false;
			}

			if (!Harness.INCLUDE_TVOS) {
				MainLog.WriteLine ("The tvOS build is disabled, so any tvOS tests will be disabled as well.");
				jenkins.IncludetvOS = false;
			}

			if (!Harness.INCLUDE_MAC) {
				MainLog.WriteLine ("The macOS build is disabled, so any macOS tests will be disabled as well.");
				jenkins.IncludeMac = false;
			}
		}
	}
}
