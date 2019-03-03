using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Xamarin;

namespace xharness
{
	public class BCLTestInfo
	{
		public Harness Harness;
		public string MonoPath { get { return Harness.MONO_PATH; } }
		public string TestName;

		static readonly Dictionary<string, string[]> ignored_tests =  new Dictionary<string, string[]> { 
			{ "Microsoft.Win32", null },
			{ "System.Diagnostics.Contracts", null },
			{ "System.Runtime.Remoting", null },
			{ "System.Security.AccessControl", null },
			{ "System.Security.Permissions", null },
			{ "System.Security.Policy", null },
			{ "System.Reflection.Emit", null },
			// System
			{ "System.CodeDom", null },
			{ "System.Configuration", null },
			{ "System.IO.Ports", null },
			{ "System.Web", new [] { "System_test.dll.sources" } },
			{ "Microsoft.CSharp", null },
			{ "Microsoft.VisualBasic", null },
			// System.Core
			{ "System.IO.Pipes", null },
			// System.Data
			{ "System.Data.Odbc", null },
			{ "System.Data.OleDb", null },
			// System.Web.Services
			{ "System.Web.Services.Configuration", null }
		};

		public BCLTestInfo (Harness harness, string testName)
		{
			Harness = harness;
			TestName = testName;
		}

		protected void Process (string test_sources, IEnumerable<string> test_files, string condition, StringBuilder [] sb, int split_count)
		{
			test_files = test_files.Where ((v) => !string.IsNullOrEmpty (v) && !v.StartsWith ("#", StringComparison.Ordinal));

			// Split the directories of the test files into 'split_count' number of chunks
			var files_path = Path.GetDirectoryName (test_sources).Replace ("/", "\\");
			var test_dirs = test_files.Select ((arg) =>
			{
				return Path.GetDirectoryName (arg);
			}).Distinct ().OrderBy ((dir) => dir).ToArray ();
			var split_dirs = new HashSet<string> [sb.Length];
			var consumed_count = 0;
			for (int i = 0; i < split_count; i++) {
				split_dirs [i] = new HashSet<string> (test_dirs.Skip (consumed_count).Take (test_dirs.Count () / split_count));
				consumed_count += split_dirs [i].Count;
			}
			split_dirs [split_count] = new HashSet<string> ();

			// Hardcode some logic to make sure things continue to build when split in multiple assemblies.

			// The tests for System.Collections.Concurrent, System.Threading and System.Threading.Tasks
			// have code that depend on eachother, so put those directories in the same chunk.
			var concurrent = split_dirs.FirstOrDefault ((v) => v.Contains ("System.Collections.Concurrent"));
			if (concurrent != null) {
				foreach (var dirs in split_dirs) {
					if (concurrent == dirs) {
						dirs.Add ("System.Threading");
						dirs.Add ("System.Threading.Tasks");
					} else {
						dirs.Remove ("System.Threading");
						dirs.Remove ("System.Threading.Tasks");
					}
				}
			}

			// System.Resources have tests that depend on resources being the same assembly as
			// the executing test, so just put those tests in the main assembly.
			var resources = split_dirs.FirstOrDefault ((v) => v.Contains ("System.Resources"));
			if (resources != null) {
				resources.Remove ("System.Resources");
				split_dirs [split_count].Add ("System.Resources");
			}

			// There are also System.Reflection tests that depend on GetExecutingAssembly and
			// expect it to be an executable assembly, so leave those in the main project as well.
			var reflection = split_dirs.FirstOrDefault ((v) => v.Contains ("System.Reflection"));
			if (reflection != null) {
				reflection.Remove ("System.Reflection");
				split_dirs [split_count].Add ("System.Reflection");
			}

			// Put each file in the corresponding project, depending on the directory for that file.
			foreach (var s in test_files) {
				if (string.IsNullOrEmpty (s))
					continue;

				if (IsNotSupported (test_sources, s))
					continue;

				var dir = Path.GetDirectoryName (s);
				var idx = Array.FindIndex (split_dirs, (v) => v.Contains (dir));

				sb [idx].AppendFormat ("    <Compile Include=\"{0}\\Test\\{1}\" Condition=\"{2}\">\r\n", files_path, s.Replace ("/", "\\").Trim (), condition);

				var link_path = Path.GetDirectoryName (s);
				if (string.IsNullOrEmpty (link_path) || link_path [0] == '.')
					sb [idx].AppendFormat ("      <Link>{0}</Link>\r\n", Path.GetFileName (s));
				else
					sb [idx].AppendFormat ("      <Link>{0}\\{1}</Link>\r\n", link_path, Path.GetFileName (s));

				sb [idx].AppendFormat ("    </Compile>\r\n");
			}
		}

		public virtual void Convert ()
		{
			var testName = TestName == "mscorlib" ? "corlib" : TestName;
			var main_test_sources = Path.Combine (MonoPath, "mcs", "class", testName, testName + "_test.dll.sources");
			var main_test_files = File.ReadAllLines (main_test_sources);
			var watch_test_sources = Path.Combine (MonoPath, "mcs", "class", testName, testName + "_test.dll.sources");
			var watch_test_files = File.ReadAllLines (watch_test_sources).Where ((arg) => !string.IsNullOrEmpty (arg));
			var template_path = Path.Combine (Harness.RootDirectory, "bcl-test", TestName, TestName + ".csproj.template");
			var csproj_input = File.ReadAllText (template_path);
			var project_path = Path.Combine (Harness.RootDirectory, "bcl-test", TestName, TestName + ".csproj");
			var csproj_output = project_path;

			var split_count = testName == "corlib" ? 2 : 1; // split corlib tests into two (library) projects, it won't build for watchOS/device otherwise (the test assembly ends up getting too big).
			var sb = new StringBuilder [split_count + 1]; // the last one is for the main project
			for (int i = 0; i < split_count + 1; i++)
				sb [i] = new StringBuilder ();

			Process (main_test_sources, main_test_files, "'$(TargetFrameworkIdentifier)' == 'MonoTouch' Or '$(TargetFrameworkIdentifier)' == 'Xamarin.iOS' Or '$(TargetFrameworkIdentifier)' == 'Xamarin.TVOS'", sb, split_count);
			Process (watch_test_sources, watch_test_files, "'$(TargetFrameworkIdentifier)' == 'Xamarin.WatchOS'", sb, split_count);

			if (split_count > 1) {
				var split_template = File.ReadAllText (Path.Combine (Harness.RootDirectory, "bcl-test", TestName, TestName + "-split.csproj.template"));
				for (int i = 0; i < split_count; i++) {
					var split_output = Path.Combine (Harness.RootDirectory, "bcl-test", TestName, TestName + "-" + i + ".csproj");
					Harness.Save (split_template.Replace ("#SPLIT#", (i + 1).ToString ()).Replace ("#FILES#", sb [i].ToString ()), split_output);
				}
				Harness.Save (csproj_input.Replace ("#FILES#", sb [sb.Length - 1].ToString ()), csproj_output);
			} else {
				Harness.Save (csproj_input.Replace ("#FILES#", sb [0].ToString ()), csproj_output);
			}
		}

		bool IsNotSupported (string sourcesFile, string path)
		{
			foreach (var p in ignored_tests) {
				if (path.Contains (p.Key)) {
					if (p.Value == null)
						return true;

					foreach (var assembly in p.Value) {
						if (sourcesFile.Contains (Path.DirectorySeparatorChar + assembly))
							return true;
					}
				}
			}

			return false;
		}
	}

	public class MacBCLTestInfo : BCLTestInfo
	{
		public MacFlavors Flavor { get; private set; }

		public MacBCLTestInfo (Harness harness, string testName, MacFlavors flavor) : base (harness, testName)
		{
			if (flavor == MacFlavors.All || flavor == MacFlavors.NonSystem)
				throw new ArgumentException ("Each target must be a specific flavor");

			Flavor = flavor;
		}

		public string FlavorSuffix => Flavor == MacFlavors.Full ? "-full" : "-modern";
		public string ProjectSuffix =>  "-mac" + FlavorSuffix + ".csproj";
		public string ProjectPath => Path.Combine (Harness.RootDirectory, "bcl-test", TestName, TestName + ProjectSuffix);
		public string TemplatePath => Path.Combine (Harness.RootDirectory, "bcl-test", TestName, TestName + "-mac.csproj.template");

		public override void Convert () 
		{
			var inputProject = new XmlDocument ();

			var xml = File.ReadAllText (TemplatePath);
			xml = xml.Replace ("#FILES#", GetFileList ());
			inputProject.LoadXmlWithoutNetworkAccess (xml);

			switch (Flavor) {
			case MacFlavors.Modern:
				inputProject.SetTargetFrameworkIdentifier ("Xamarin.Mac");
				inputProject.SetTargetFrameworkVersion ("v2.0");
				inputProject.RemoveNode ("UseXamMacFullFramework");
				inputProject.AddAdditionalDefines ("MOBILE;XAMMAC");
				inputProject.AddReference ("Mono.Security");
				break;
			case MacFlavors.Full:
				inputProject.AddAdditionalDefines ("XAMMAC_4_5");
				break;
			default:
				throw new NotImplementedException (Flavor.ToString ());
			}
			inputProject.SetOutputPath ("bin\\$(Platform)\\$(Configuration)" + FlavorSuffix);
			inputProject.SetIntermediateOutputPath ("obj\\$(Platform)\\$(Configuration)" + FlavorSuffix);
			inputProject.SetAssemblyName (inputProject.GetAssemblyName () + FlavorSuffix);

			Harness.Save (inputProject, ProjectPath);
		}

		string GetFileList ()
		{
			var testName = TestName == "mscorlib" ? "corlib" : TestName;
			var main_test_sources = Path.Combine (MonoPath, "mcs", "class", testName, testName + "_test.dll.sources");
			var main_test_files = File.ReadAllLines (main_test_sources);

			var sb = new StringBuilder[2] { new StringBuilder (), new StringBuilder () };
			Process (main_test_sources, main_test_files, "", sb, 1);
			return sb[0].ToString ();
		}
	}
}
