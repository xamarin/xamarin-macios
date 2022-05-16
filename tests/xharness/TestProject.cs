#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.DotNet.XHarness.Common.Execution;
using Microsoft.DotNet.XHarness.Common.Logging;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;
using Xamarin;
using Xharness.Jenkins.TestTasks;

namespace Xharness {
	public class TestProject {
		XmlDocument? xml;
		bool generate_variations = true;

		public TestPlatform TestPlatform;
		public TestLabel Label;
		public string Path;
		public string? SolutionPath;
		public string? Name;
		public bool IsExecutableProject;
		public bool IsNUnitProject;
		public bool IsDotNetProject;
		public string []? Configurations;
		public Func<Task>? Dependency;
		public string? FailureMessage;
		public bool RestoreNugetsInProject = true;
		public string? MTouchExtraArgs;
		public double TimeoutMultiplier = 1;
		public bool? Ignore;

		public IEnumerable<TestProject>? ProjectReferences;

		// Optional
		public MonoNativeInfo? MonoNativeInfo { get; set; }

		public TestProject (TestLabel label, string path, bool isExecutableProject = true)
		{
			Label = label;
			Path = path;
			IsExecutableProject = isExecutableProject;
		}

		public virtual bool GenerateVariations { get => generate_variations; set => generate_variations = value; }

		public XmlDocument Xml {
			get {
				if (xml == null) {
					xml = new XmlDocument ();
					xml.LoadWithoutNetworkAccess (Path);
				}
				return xml;
			}
		}

		public virtual TestProject Clone ()
		{
			return CompleteClone (new TestProject (Label, Path, IsExecutableProject));
		}

		protected virtual TestProject CompleteClone (TestProject rv)
		{
			rv.Path = Path;
			rv.IsExecutableProject = IsExecutableProject;
			rv.IsDotNetProject = IsDotNetProject;
			rv.RestoreNugetsInProject = RestoreNugetsInProject;
			rv.Name = Name;
			rv.MTouchExtraArgs = MTouchExtraArgs;
			rv.TimeoutMultiplier = TimeoutMultiplier;
			rv.Ignore = Ignore;
			rv.TestPlatform = TestPlatform;
			return rv;
		}

		public Task CreateCopyAsync (ILog log, IProcessManager processManager, ITestTask test, string rootDirectory)
		{
			var pr = new Dictionary<string, TestProject> ();
			return CreateCopyAsync (log, processManager, test, rootDirectory, pr);
		}

		static SemaphoreSlim ls_files_semaphore = new SemaphoreSlim (1);

		async Task<string[]> ListFilesAsync (ILog log, string test_dir, IProcessManager processManager)
		{
			var acquired = await ls_files_semaphore.WaitAsync (TimeSpan.FromMinutes (5));
			try {
				if (!acquired)
					log.WriteLine ($"Unable to acquire lock to run 'git ls-files {test_dir}' in 5 minutes; will try to run anyway.");
				using var process = new Process ();
				process.StartInfo.FileName = "git";
				process.StartInfo.Arguments = "ls-files";
				process.StartInfo.WorkingDirectory = test_dir;
				var stdout = new MemoryLog () { Timestamp = false };
				var result = await processManager.RunAsync (process, stdout, stdout, stdout, timeout: TimeSpan.FromSeconds (60));
				if (!result.Succeeded)
					throw new Exception ($"Failed to list the files in the directory {test_dir} (TimedOut: {result.TimedOut} ExitCode: {result.ExitCode}):\n{stdout}");
				return stdout.ToString ().Split ('\n');
			} finally {
				if (acquired)
					ls_files_semaphore.Release ();
			}
		}

		async Task CreateCopyAsync (ILog log, IProcessManager processManager, ITestTask test, string rootDirectory, Dictionary<string, TestProject> allProjectReferences)
		{
			var directory = Cache.CreateTemporaryDirectory (test.TestName ?? System.IO.Path.GetFileNameWithoutExtension (Path));
			Directory.CreateDirectory (directory);
			var original_path = Path;
			Path = System.IO.Path.Combine (directory, System.IO.Path.GetFileName (Path));

			await Task.Yield ();

			XmlDocument doc;
			doc = new XmlDocument ();
			doc.LoadWithoutNetworkAccess (original_path);

			var variableSubstitution = new Dictionary<string, string> ();
			variableSubstitution.Add ("RootTestsDirectory", rootDirectory);

			lock (GetType ()) {
				InlineSharedImports (doc, original_path, variableSubstitution, rootDirectory);
			}

			doc.ResolveAllPaths (original_path, variableSubstitution);

			// Replace RootTestsDirectory with a constant value, so that any relative paths don't end up wrong.
			var rootTestsDirectoryNode = doc.SelectSingleNode ("/Project/PropertyGroup/RootTestsDirectory");
			if (rootTestsDirectoryNode != null)
				rootTestsDirectoryNode.InnerText = rootDirectory;

			if (doc.IsDotNetProject ()) {
				if (test.ProjectPlatform == "iPhone") {
					switch (test.Platform) {
					case TestPlatform.iOS:
					case TestPlatform.iOS_Unified:
					case TestPlatform.iOS_Unified64:
					case TestPlatform.iOS_TodayExtension64:
						doc.SetTopLevelPropertyGroupValue ("RuntimeIdentifier", "ios-arm64");
						break;
					case TestPlatform.iOS_Unified32:
						doc.SetTopLevelPropertyGroupValue ("RuntimeIdentifier", "ios-arm");
						break;
					case TestPlatform.tvOS:
						doc.SetTopLevelPropertyGroupValue ("RuntimeIdentifier", "tvos-arm64");
						break;
					case TestPlatform.watchOS:
					case TestPlatform.watchOS_32:
					case TestPlatform.watchOS_64_32:
						doc.SetTopLevelPropertyGroupValue ("RuntimeIdentifier", "watchos-arm");
						break;
					}
				}

				if (doc.GetEnableDefaultItems () != false) {
					// Many types of files below the csproj directory are included by default,
					// which means that we have to include them manually in the cloned csproj,
					// because the cloned project is stored in a very different directory.
					var test_dir = System.IO.Path.GetDirectoryName (original_path);

					var files = await ListFilesAsync (log, test_dir, processManager);
					foreach (var file in files) {
						var ext = System.IO.Path.GetExtension (file);
						var full_path = System.IO.Path.Combine (test_dir, file);
						var windows_file = full_path.Replace ('/', '\\');

						if (file.Contains (".xcasset")) {
							doc.AddInclude ("ImageAsset", file, windows_file, true);
							continue;
						}

						switch (ext.ToLowerInvariant ()) {
						case ".cs":
							doc.AddInclude ("Compile", file, windows_file, true);
							break;
						case ".plist":
							doc.AddInclude ("None", file, windows_file, true);
							break;
						case ".storyboard":
							doc.AddInclude ("InterfaceDefinition", file, windows_file, true);
							break;
						case ".gitignore":
						case ".csproj":
						case ".fsproj":
						case ".props": // Directory.Build.props
						case "": // Makefile
							break; // ignore these files
						default:
							Console.WriteLine ($"Unknown file: {file} (extension: {ext}). There might be a default inclusion behavior for this file.");
							break;
						}
					}
				}

				// The global.json and NuGet.config files make sure we use the locally built packages.
				var dotnet_test_dir = System.IO.Path.Combine (test.RootDirectory, "dotnet");
				var global_json = System.IO.Path.Combine (dotnet_test_dir, "global.json");
				var nuget_config = System.IO.Path.Combine (dotnet_test_dir, "NuGet.config");
				var target_directory = directory;
				File.Copy (global_json, System.IO.Path.Combine (target_directory, System.IO.Path.GetFileName (global_json)), true);
				File.Copy (nuget_config, System.IO.Path.Combine (target_directory, System.IO.Path.GetFileName (nuget_config)), true);
			}

			var projectReferences = new List<TestProject> ();
			foreach (var pr in doc.GetProjectReferences ()) {
				var prPath = pr.Replace ('\\', '/');
				if (!allProjectReferences.TryGetValue (prPath, out var tp)) {
					tp = new TestProject (Label, pr.Replace ('\\', '/'));
					tp.TestPlatform = TestPlatform;
					await tp.CreateCopyAsync (log, processManager, test, rootDirectory, allProjectReferences);
					allProjectReferences.Add (prPath, tp);
				}
				doc.SetProjectReferenceInclude (pr, tp.Path.Replace ('/', '\\'));
				projectReferences.Add (tp);
			}
			this.ProjectReferences = projectReferences;

			doc.Save (Path);
		}

		void InlineSharedImports (XmlDocument doc, string original_path, Dictionary<string, string> variableSubstitution, string rootDirectory)
		{
			// Find Import nodes that point to a shared code file, load that shared file and inject it here.
			var nodes = doc.SelectNodes ("//*[local-name() = 'Import']");
			foreach (XmlNode node in nodes) {
				if (node is null)
					continue;

				var project = node.Attributes ["Project"].Value.Replace ('\\', '/');
				var projectName = System.IO.Path.GetFileName (project);
				if (projectName != "shared.csproj" && projectName != "shared.fsproj" && projectName != "shared-dotnet.csproj")
					continue;

				if (TestPlatform == TestPlatform.None)
					throw new InvalidOperationException ($"The project '{original_path}' did not set the TestPlatform property.");

				project = project.Replace ("$(RootTestsDirectory)", rootDirectory);

				var sharedProjectPath = System.IO.Path.Combine (System.IO.Path.GetDirectoryName (original_path), project);
				// Check for variables that won't work correctly if the shared code is moved to a different file
				var xml = File.ReadAllText (sharedProjectPath);
				xml = xml.Replace ("$(MSBuildThisFileDirectory)", System.IO.Path.GetDirectoryName (sharedProjectPath));
				if (xml.Contains ("$(MSBuildThis"))
					throw new InvalidOperationException ($"Can't use MSBuildThis* variables in shared MSBuild test code: {sharedProjectPath}");

				var import = new XmlDocument ();
				import.LoadXmlWithoutNetworkAccess (xml);
				// Inline any shared imports in the inlined shared import too
				InlineSharedImports (import, sharedProjectPath, variableSubstitution, rootDirectory);
				var importNodes = import.SelectSingleNode ("/Project").ChildNodes;
				var previousNode = node;
				foreach (XmlNode importNode in importNodes) {
					var importedNode = doc.ImportNode (importNode, true);
					previousNode.ParentNode.InsertAfter (importedNode, previousNode);
					previousNode = importedNode;
				}
				node.ParentNode.RemoveChild (node);

				variableSubstitution ["_PlatformName"] = TestPlatform.ToPlatformName ();
				variableSubstitution = doc.CollectAndEvaluateTopLevelProperties (variableSubstitution);
			}
		}

		public override string ToString ()
		{
			return Name ?? base.ToString ();
		}
	}

}
