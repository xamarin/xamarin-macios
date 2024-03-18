using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Text;
using Microsoft.DotNet.XHarness.iOS.Shared.Hardware;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;
using Xharness.Targets;

namespace Xharness {
	public static class SolutionGenerator {
		static void AddProjectToSolution (IHarness harness, string sln_path, TextWriter solution, string project_path, out string configurations)
		{
			var project = new XmlDocument ();
			project.LoadWithoutNetworkAccess (project_path);
			var guid = project.GetProjectGuid ();
			solution.WriteLine ("Project(\"{3}\") = \"{0}\", \"{1}\", \"{2}\"", Path.GetFileNameWithoutExtension (project_path), FixProjectPath (sln_path, project_path), guid, project_path.EndsWith (".csproj") ? Target.CSharpGuid : Target.FSharpGuid);
			solution.WriteLine ("EndProject");

			configurations = string.Format (
				"\t\t{0}.Debug|Any CPU.ActiveCfg = Debug|Any CPU\n" +
				"\t\t{0}.Debug|Any CPU.Build.0 = Debug|Any CPU\n" +
				"\t\t{0}.Debug|iPhoneSimulator.ActiveCfg = Debug|Any CPU\n" +
				"\t\t{0}.Debug|iPhoneSimulator.Build.0 = Debug|Any CPU\n" +
				"\t\t{0}.Debug|iPhone.ActiveCfg = Debug|Any CPU\n" +
				"\t\t{0}.Debug|iPhone.Build.0 = Debug|Any CPU\n" +
				"\t\t{0}.Release|Any CPU.ActiveCfg = Release|Any CPU\n" +
				"\t\t{0}.Release|Any CPU.Build.0 = Release|Any CPU\n" +
				"\t\t{0}.Release|iPhoneSimulator.ActiveCfg = Release|Any CPU\n" +
				"\t\t{0}.Release|iPhoneSimulator.Build.0 = Release|Any CPU\n" +
				"\t\t{0}.Release|iPhone.ActiveCfg = Release|Any CPU\n" +
				"\t\t{0}.Release|iPhone.Build.0 = Release|Any CPU\n", guid);
		}

		static string FixProjectPath (string sln_path, string project_path)
		{
			var sln_dir = Path.GetDirectoryName (sln_path);
			if (project_path.StartsWith (sln_dir + Path.PathSeparator, StringComparison.OrdinalIgnoreCase))
				project_path = project_path.Substring (sln_dir.Length + 1);
			project_path = project_path.Replace ('/', '\\');
			return project_path;
		}

		public static void CreateSolution (IHarness harness, IEnumerable<Target> targets, string infix, DevicePlatform testPlatform)
		{
			CreateSolution (harness, targets, null, infix, testPlatform);
		}

		public static void CreateSolution (IHarness harness, IEnumerable<Target> targets, Target exeTarget, string infix, DevicePlatform testPlatform)
		{
			var folders = new StringBuilder ();

			var srcDirectory = Path.Combine (HarnessConfiguration.RootDirectory, "..", "src");
			var sln_path = exeTarget is null ? Path.Combine (HarnessConfiguration.RootDirectory, "tests-" + infix + ".sln") : Path.Combine (Path.GetDirectoryName (exeTarget.ProjectPath), Path.GetFileNameWithoutExtension (exeTarget.ProjectPath) + ".sln");

			using (var writer = new StringWriter ()) {
				writer.WriteLine ();
				writer.WriteLine ("Microsoft Visual Studio Solution File, Format Version 11.00");
				writer.WriteLine ("# Visual Studio 2010");
				foreach (var target in targets) {
					var relatedProjects = target.GetRelatedProjects ();
					var hasRelatedProjects = relatedProjects is not null;
					var folderGuid = string.Empty;
					var useFolders = hasRelatedProjects && target.IsExe && exeTarget is null;

					if (hasRelatedProjects && target.IsExe) {
						if (exeTarget is null) {
							CreateSolution (harness, targets, target, infix, testPlatform); // create a solution for just this test project as well
						} else if (exeTarget != target) {
							continue;
						}
					}

					if (useFolders) {
						folderGuid = Guid.NewGuid ().ToString ().ToUpperInvariant ();
						writer.WriteLine ("Project(\"{{2150E333-8FDC-42A3-9474-1A3956D46DE8}}\") = \"{0}\", \"{0}\", \"{{{1}}}\"", target.Name, folderGuid);
						writer.WriteLine ("EndProject");
					}

					writer.WriteLine ("Project(\"{3}\") = \"{0}\", \"{1}\", \"{2}\"", Path.GetFileNameWithoutExtension (target.ProjectPath), FixProjectPath (sln_path, Path.GetFullPath (target.ProjectPath)), target.ProjectGuid, target.LanguageGuid);
					writer.WriteLine ("EndProject");

					if (hasRelatedProjects && target.IsExe) {
						foreach (var rp in relatedProjects) {
							writer.WriteLine ("Project(\"{3}\") = \"{0}\", \"{1}\", \"{2}\"", Path.GetFileNameWithoutExtension (rp.ProjectPath), FixProjectPath (sln_path, Path.GetFullPath (rp.ProjectPath)), rp.Guid, target.LanguageGuid);
							writer.WriteLine ("EndProject");
						}
					}

					if (useFolders) {
						folders.AppendFormat ("\t\t{0} = {{{1}}}\n", target.ProjectGuid, folderGuid);
						foreach (var rp in relatedProjects)
							folders.AppendFormat ("\t\t{0} = {{{1}}}\n", rp.Guid, folderGuid);
					}
				}

				// Add reference to the Touch.Client project
				string configuration;
				var proj_path = Path.GetFullPath (Path.Combine (srcDirectory, "..", "external", "Touch.Unit", "Touch.Client", testPlatform.AsString (), "Touch.Client-" + testPlatform.AsString () + ".csproj"));
				AddProjectToSolution (harness, sln_path, writer, proj_path, out configuration);

				writer.WriteLine ("Global");

				writer.WriteLine ("\tGlobalSection(SolutionConfigurationPlatforms) = preSolution");
				writer.WriteLine ("\t\tDebug|iPhoneSimulator = Debug|iPhoneSimulator");
				writer.WriteLine ("\t\tRelease|iPhoneSimulator = Release|iPhoneSimulator");
				writer.WriteLine ("\t\tDebug|iPhone = Debug|iPhone");
				writer.WriteLine ("\t\tRelease|iPhone = Release|iPhone");
				writer.WriteLine ("\t\tDebug|Any CPU = Debug|Any CPU");
				writer.WriteLine ("\t\tRelease|Any CPU = Release|Any CPU");
				writer.WriteLine ("\tEndGlobalSection");

				writer.WriteLine ("\tGlobalSection(ProjectConfigurationPlatforms) = postSolution");
				var exePlatforms = new string [] { "iPhone", "iPhoneSimulator" };
				var configurations = new string [] { "Debug", "Release" };
				foreach (var target in targets) {
					if (target.IsExe && exeTarget is not null && target != exeTarget)
						continue;

					foreach (var conf in configurations) {
						if (target.IsExe) {
							foreach (var platform in exePlatforms) {
								writer.WriteLine ("\t\t{0}.{1}|{2}.ActiveCfg = {1}|{2}", target.ProjectGuid, conf, platform);
								writer.WriteLine ("\t\t{0}.{1}|{2}.Build.0 = {1}|{2}", target.ProjectGuid, conf, platform);

							}
						} else {
							foreach (var platform in new string [] { "Any CPU", "iPhone", "iPhoneSimulator" }) {
								writer.WriteLine ("\t\t{0}.{1}|{2}.ActiveCfg = {1}|Any CPU", target.ProjectGuid, conf, platform);
								writer.WriteLine ("\t\t{0}.{1}|{2}.Build.0 = {1}|Any CPU", target.ProjectGuid, conf, platform);
							}
						}
					}

					if (target.IsExe) {
						var relatedProjects = target.GetRelatedProjects ();
						if (relatedProjects is not null) {
							foreach (var rp in relatedProjects) {
								foreach (var conf in configurations) {
									foreach (var platform in exePlatforms) {
										writer.WriteLine ("\t\t{0}.{1}|{2}.ActiveCfg = {1}|{2}", rp.Guid, conf, platform);
										writer.WriteLine ("\t\t{0}.{1}|{2}.Build.0 = {1}|{2}", rp.Guid, conf, platform);
									}
								}
							}
						}

					}

				}
				writer.Write (configuration);
				writer.WriteLine ("\tEndGlobalSection");

				if (folders.Length > 0) {
					writer.WriteLine ("\tGlobalSection(NestedProjects) = preSolution");
					writer.Write (folders.ToString ());
					writer.WriteLine ("\tEndGlobalSection");
				}

				writer.WriteLine ("EndGlobal");

				harness.Save (writer, sln_path);
			}
		}
	}
}
