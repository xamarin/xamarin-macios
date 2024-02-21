using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

using NUnit.Framework;

using Xamarin.Tests;

namespace Samples {
	public class SampleTest {
		public ProjectInfo Project;
		public string Solution;
		public bool BuildSolution;
		public string KnownFailure;
		public string CodesignKey;
		public string [] DebugConfigurations;
		public string [] ReleaseConfigurations;
		public string [] Platforms;

		// for various reasons (build'ability, compatibility, performance) it can be
		// better to build a subset of a solution
		// e.g. `nuget restore` requires removing the projects from the .sln
		public string [] RemoveProjects;
	}

	public class SampleTestData {
		public SampleTest SampleTest;
		public string Configuration;
		public string Platform;
		public TimeSpan Timeout;

		public override string ToString ()
		{
			if (string.IsNullOrEmpty (Platform))
				return $"{SampleTest.Project.Title}: {Configuration}";
			return $"{SampleTest.Project.Title}: {Configuration}|{Platform}";
		}
	}

	public class ProjectInfo {
		public string Title;
		public string RelativePath;
		public string FullPath;
		public bool IsExecutable;
		public string [] Imports;
		public TestPlatform Platform;

		public bool IsApplicable (bool assert)
		{
			if (!IsExecutable) {
				if (assert)
					Assert.Ignore ("Project is not an executable project");
				return false;
			}

			if (Platform == TestPlatform.None) {
				if (assert)
					Assert.Ignore ("Project is not an Xamarin.iOS/Xamarin.Mac/Xamarin.WatchOS/Xamarin.TVOS project. Imports:\n\t{0}", string.Join ("\t\n", Imports));
				return false;
			}

			if (Platform == TestPlatform.watchOS) {
				if (assert)
					Assert.Ignore ("Project is a watchOS app"); // no need to build watchOS apps, they're built as part of their containing iOS project.

				return false;
			}

			return true;
		}
	}

	public abstract class SampleTester : BaseTester {

		public static TimeSpan DefaultTimeout { get; } = TimeSpan.FromMinutes (5);

		protected SampleTester ()
		{
		}

		protected SampleTester (string repo, string hash)
			: base (repo, hash)
		{
		}

		static ProjectInfo GetProjectInfo (string relative_path, string full_path)
		{
			var xml = File.ReadAllText (full_path);
			var info = new ProjectInfo ();
			info.FullPath = full_path;
			info.RelativePath = relative_path;
			info.IsExecutable = xml.Contains ("<OutputType>Exe</OutputType>");

			var xml_lines = xml.Split ('\n');
			var xml_imports = xml_lines.
				Where ((v) => v.Contains ("<Import Project=")).
				Select ((v) => v.Split ('"') [1]);
			info.Imports = xml_imports.ToArray ();

			var test_platform = TestPlatform.None;
			if (xml_imports.Any ((v) => v.Contains ("Xamarin.iOS"))) {
				test_platform = TestPlatform.iOS;
			} else if (xml_imports.Any ((v) => v.Contains ("Xamarin.TVOS"))) {
				test_platform = TestPlatform.tvOS;
			} else if (xml_imports.Any ((v) => v.Contains ("Xamarin.WatchOS"))) {
				test_platform = TestPlatform.watchOS;
			} else if (xml_imports.Any ((v) => v.Contains ("Xamarin.Mac"))) {
				test_platform = TestPlatform.macOS;
			} else {
				test_platform = TestPlatform.None;
			}
			info.Platform = test_platform;

			return info;
		}

		public static Dictionary<string, string> GetEnvironmentVariables (TestPlatform platform)
		{
			var environment_variables = new Dictionary<string, string> ();
			environment_variables ["MD_APPLE_SDK_ROOT"] = Path.GetDirectoryName (Path.GetDirectoryName (Configuration.XcodeLocation));
			switch (platform) {
			case TestPlatform.iOS:
			case TestPlatform.tvOS:
			case TestPlatform.watchOS:
				environment_variables ["MD_MTOUCH_SDK_ROOT"] = Path.Combine (Configuration.IOS_DESTDIR, "Library", "Frameworks", "Xamarin.iOS.framework", "Versions", "Current");
				environment_variables ["TargetFrameworkFallbackSearchPaths"] = Path.Combine (Configuration.IOS_DESTDIR, "Library", "Frameworks", "Mono.framework", "External", "xbuild-frameworks");
				environment_variables ["MSBuildExtensionsPathFallbackPathsOverride"] = Path.Combine (Configuration.IOS_DESTDIR, "Library", "Frameworks", "Mono.framework", "External", "xbuild");
				break;
			case TestPlatform.macOS:
				environment_variables ["TargetFrameworkFallbackSearchPaths"] = Path.Combine (Configuration.MAC_DESTDIR, "Library", "Frameworks", "Mono.framework", "External", "xbuild-frameworks");
				environment_variables ["MSBuildExtensionsPathFallbackPathsOverride"] = Path.Combine (Configuration.MAC_DESTDIR, "Library", "Frameworks", "Mono.framework", "External", "xbuild");
				environment_variables ["XamarinMacFrameworkRoot"] = Path.Combine (Configuration.MAC_DESTDIR, "Library", "Frameworks", "Xamarin.Mac.framework", "Versions", "Current");
				environment_variables ["XAMMAC_FRAMEWORK_PATH"] = Path.Combine (Configuration.MAC_DESTDIR, "Library", "Frameworks", "Xamarin.Mac.framework", "Versions", "Current");
				break;
			default:
				throw new NotImplementedException (platform.ToString ());
			}
			return environment_variables;
		}

		[Test]
		public void BuildSample ([ValueSource ("GetSampleData")] SampleTestData sampleTestData)
		{
			try {
				var data = sampleTestData.SampleTest;
				if (!string.IsNullOrEmpty (data.KnownFailure))
					Assert.Ignore (data.KnownFailure);

				switch (data.Project.Platform) {
				case TestPlatform.iOS:
					if (!Configuration.include_ios)
						Assert.Ignore ("iOS support has been disabled.");
					break;
				case TestPlatform.tvOS:
					if (!Configuration.include_tvos)
						Assert.Ignore ("tvOS support has been disabled");
					break;
				case TestPlatform.watchOS:
					if (!Configuration.include_watchos)
						Assert.Ignore ("watchOS support has been disabled");
					break;
				case TestPlatform.macOS:
					if (!Configuration.include_mac)
						Assert.Ignore ("macOS support has been disabled");
					break;
				default:
					throw new NotImplementedException (sampleTestData.Platform.ToString ());
				}

				var file_to_build = sampleTestData.SampleTest.Project.RelativePath;
				var target = string.Empty;
				if (data.BuildSolution) {
					file_to_build = data.Solution;
					target = Path.GetFileNameWithoutExtension (data.Project.RelativePath).Replace ('.', '_');
				}

				var repo = CloneRepo ();
				file_to_build = Path.Combine (repo, file_to_build);

				if (data.RemoveProjects is not null) {
					if (String.IsNullOrEmpty (data.Solution))
						Assert.Fail ("'RemoveProjects' used without a 'Solution' path!");
					var sln_path = Path.Combine (repo, data.Solution);
					var filtered_sln = new List<string> (File.ReadAllLines (sln_path));
					foreach (var p in data.RemoveProjects) {
						for (int i = 0; i < filtered_sln.Count; i++) {
							var line = filtered_sln [i];
							if (line.StartsWith ("Project(", StringComparison.Ordinal)) {
								if (line.Contains ($") = \"{p}\", \"")) {
									filtered_sln.RemoveAt (i);
									filtered_sln.RemoveAt (i); // EndProject (same `i` as things moved up)
									break;
								}
							}
						}
					}
					File.WriteAllLines (sln_path, filtered_sln);
				}

				ProcessHelper.BuildSolution (file_to_build, sampleTestData.Platform, sampleTestData.Configuration, GetEnvironmentVariables (data.Project.Platform), sampleTestData.Timeout, target, data.CodesignKey);
				Console.WriteLine ("✅ {0} succeeded.", TestContext.CurrentContext.Test.FullName);
			} catch (Exception e) {
				Console.WriteLine ("❌ {0} failed: {1}", TestContext.CurrentContext.Test.FullName, e.Message);
				throw;
			}
		}

		static Dictionary<string, ProjectInfo []> projects = new Dictionary<string, ProjectInfo []> ();
		protected static ProjectInfo [] GetExecutableProjects (string org, string repo, string hash, string default_branch)
		{
			if (!projects.TryGetValue (repo, out var rv)) {
				var project_paths = GitHub.GetProjects (org, repo, hash, default_branch);

				// We can filter out project we don't care about.
				rv = project_paths.
					Select ((v) => GetProjectInfo (v, Path.Combine (GitHub.CloneRepository (org, repo, hash, default_branch, false), v))).
					Where ((v) => v.IsApplicable (false)).
					ToArray ();

				projects [repo] = rv;
			}
			return rv;
		}

		protected static IEnumerable<SampleTestData> GetSampleTestData (Dictionary<string, SampleTest> samples, string org, string repo, string hash, string default_branch, TimeSpan timeout)
		{
			var defaultDebugConfigurations = new string [] { "Debug" };
			var defaultReleaseConfigurations = new string [] { "Release" };

			if (samples is null) {
				samples = new Dictionary<string, SampleTest> ();
			} else {
				samples = new Dictionary<string, SampleTest> (samples);
			}

			// If a project's filename is unique in this repo, use the filename (without extension) as the name of the test.
			// Otherwise use the project's relative path.
			var executable_projects = GetExecutableProjects (org, repo, hash, default_branch);
			var duplicateProjects = executable_projects.GroupBy ((v) => Path.GetFileNameWithoutExtension (v.RelativePath)).Where ((v) => v.Count () > 1);
			foreach (var group in duplicateProjects) {
				foreach (var proj in group) {
					proj.Title = proj.RelativePath;
				}
			}
			foreach (var proj in executable_projects) {
				if (proj.Title is null) {
					proj.Title = Path.GetFileNameWithoutExtension (proj.RelativePath);
				}
			}

			var platform_filter = Environment.GetEnvironmentVariable ("TEST_PLATFORM_FILTER_EXPRESSION");
			var config_filter = Environment.GetEnvironmentVariable ("TEST_CONFIG_FILTER_EXPRESSION");
			var name_filter = Environment.GetEnvironmentVariable ("TEST_NAME_FILTER_EXPRESSION");

			IEnumerable<T> filter<T> (string name, string proj, IEnumerable<T> input, string filter_expression, Func<T, string> tostring)
			{
				if (string.IsNullOrEmpty (filter_expression))
					return input;

				var filtered = input.Where ((v) => Regex.IsMatch (tostring (v), filter_expression));
				var removed = input.Where ((v) => !filtered.Contains (v));
				if (removed.Any ()) {
					return filtered;
				}
				return input;
			}

			// Create the test variations for each project.
			foreach (var proj in filter ("name", "*", executable_projects, name_filter, (v) => Path.GetFileName (v.RelativePath))) {
				if (!samples.TryGetValue (proj.RelativePath, out var sample))
					samples [proj.RelativePath] = sample = new SampleTest ();
				sample.Project = proj;
				IEnumerable<string> platforms = sample.Platforms;
				if (platforms is null) {
					switch (proj.Platform) {
					case TestPlatform.iOS:
					case TestPlatform.tvOS:
						platforms = new string [] { "iPhone", "iPhoneSimulator" };
						break;
					case TestPlatform.macOS:
						platforms = new string [] { "" };
						break;
					case TestPlatform.watchOS:
					default:
						throw new NotImplementedException (proj.Platform.ToString ());
					}
				}

				foreach (var platform in filter ("platform", proj.Title, platforms, platform_filter, (v) => v)) {
					var configs = new List<string> ();
					configs.AddRange (sample.DebugConfigurations ?? defaultDebugConfigurations);
					configs.AddRange (sample.ReleaseConfigurations ?? defaultReleaseConfigurations);
					foreach (var config in filter ("config", proj.Title, configs, config_filter, (v) => v)) {
						yield return new SampleTestData { SampleTest = sample, Configuration = config, Platform = platform, Timeout = timeout };
					}
				}
			}
		}

		string CloneRepo ()
		{
			return GitHub.CloneRepository (Org, Repository, Hash, DefaultBranch);
		}
	}

	[TestFixture]
	public class BaselineTester {
		[Test]
		public void DeviceDebug ()
		{
			var sln = Path.Combine (Configuration.SourceRoot, "tests", "sampletester", "BaselineTest", "BaselineTest.sln");
			GitHub.CleanRepository (Path.GetDirectoryName (sln));
			ProcessHelper.BuildSolution (sln, "iPhone", "Debug", SampleTester.GetEnvironmentVariables (TestPlatform.iOS), SampleTester.DefaultTimeout);
		}

	}
}
