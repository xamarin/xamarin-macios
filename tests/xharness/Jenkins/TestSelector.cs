#nullable enable

using System;
using System.IO;
using System.Linq;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using System.Collections.Generic;

using Microsoft.DotNet.XHarness.iOS.Shared.Logging;

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
		Harness Harness => jenkins.Harness;
		
		// disabled by default
		static readonly Dictionary<string, TestSelection> testNameSelectionMatch = new Dictionary<string, TestSelection> {
			["ios-32"] = TestSelection.iOS32,
			["ios-64"] = TestSelection.iOS64,
			["ios"] = TestSelection.iOS,
			["tvos"] = TestSelection.tvOS,
			["watchos"] = TestSelection.watchOS,
			["mac"] = TestSelection.MacOS,
			["ios-msbuild"] = TestSelection.iOSMSBuild,
			["ios-simulator"] = TestSelection.Simulator,
			["non-monotouch"] =  TestSelection.NonMonotouch,
			["monotouch"] = TestSelection.Monotouch,
			["mtouch"] =  TestSelection.Mtouch,
			["mmp"] = TestSelection.Mmp,
			["bcl"] = TestSelection.Bcl,
			["bcl-xunit"] = TestSelection.BclXUnit,
			["bcl-nunit"] = TestSelection.BclNUnit,
			["mscorlib"] = TestSelection.Mscorlib,
			["btouch"] = TestSelection.Btouch,
			["mac-binding-project"] = TestSelection.MacBindingProject,
			["ios-extensions"] = TestSelection.iOSExtensions,
			["device"] = TestSelection.Device,
			["xtro"] = TestSelection.Xtro,
			["cecil"] = TestSelection.Cecil,
			["all"] = TestSelection.All,
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

		void SelectTestsByModifiedFiles (int pullRequest)
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

			var custom_labels_file = Path.Combine (Harness.RootDirectory, "..", "jenkins", "custom-labels.txt");
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

			foreach (var keyValuePair in testNameSelectionMatch) {
				if (keyValuePair.Key == "all")
					continue; // all is special since it can enable or disable all tests, so we check its settings once we have checked all of the others
				SetEnabled (labels, keyValuePair.Key, keyValuePair.Value);
			}

			SetEnabled (labels, "all", TestSelection.All);

			// special case since we are not working with the TestSelection flag in jenkins
			Harness.IncludeSystemPermissionTests = labels.Contains ("run-system-permission-tests") || jenkins.TestSelection.HasFlag (TestSelection.All);

			// docs is a bit special:
			// - can only be executed if the Xamarin-specific parts of the build is enabled
			// - enabled by default if the current branch is main (or, for a pull request, if the target branch is main)
			var changed = SetEnabled (labels, "docs", TestSelection.Docs) || jenkins.TestSelection.HasFlag (TestSelection.All);
			if (Harness.ENABLE_XAMARIN) {
				if (!changed) {
					var branchName = Environment.GetEnvironmentVariable ("BRANCH_NAME");
					if (!string.IsNullOrEmpty (branchName)) {
						if (branchName == "main") {
							jenkins.TestSelection |= TestSelection.Docs;
						} else {
							jenkins.TestSelection &= ~TestSelection.Docs;
						}
						if (jenkins.TestSelection.HasFlag (TestSelection.Docs))
							MainLog.WriteLine ("Enabled 'docs' tests because the current branch is 'main'.");
					} else if (pullRequest > 0) {
						if (vcs.GetPullRequestTargetBranch (pullRequest) == "main") {
							jenkins.TestSelection |= TestSelection.Docs;
						} else {
							jenkins.TestSelection &= ~TestSelection.Docs;
						}
						if (jenkins.TestSelection.HasFlag (TestSelection.Docs))
							MainLog.WriteLine ("Enabled 'docs' tests because the target branch is 'main'.");
					}
				}
			} else {
				if (jenkins.TestSelection.HasFlag (TestSelection.Docs)) {
					jenkins.TestSelection &= ~TestSelection.Docs; // could have been enabled by 'run-all-tests', so disable it if we can't run it.
					MainLog.WriteLine ("Disabled 'docs' tests because the Xamarin-specific parts of the build are not enabled.");
				}
			}

			// old simulator tests is also a bit special:
			// - enabled by default if using a beta Xcode, otherwise disabled by default
			changed = SetEnabled (labels, "old-simulator", TestSelection.OldiOSSimulator) || jenkins.TestSelection.HasFlag (TestSelection.All);
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
