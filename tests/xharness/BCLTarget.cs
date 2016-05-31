using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace xharness
{
	public class BCLTarget
	{
		public Harness Harness;
		public string MonoPath; // MONO_PATH
		public string WatchMonoPath; // WATCH_MONO_PATH
		public string TVOSMonoPath; // TVOS_MONO_PATH
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

		public BCLTarget ()
		{
		}

		public void Convert ()
		{
			var testName = TestName == "mscorlib" ? "corlib" : TestName;
			var main_test_sources = Path.Combine (MonoPath, "mcs", "class", testName, testName + "_test.dll.sources");
			var main_test_files = File.ReadAllLines (main_test_sources);
			var watch_test_sources = Path.Combine (WatchMonoPath, "mcs", "class", testName, testName + "_test.dll.sources");
			var watch_test_files = File.ReadAllLines (watch_test_sources);
			var tvos_test_sources = Path.Combine (TVOSMonoPath, "mcs", "class", testName, testName + "_test.dll.sources");
			var tvos_test_files = File.ReadAllLines (tvos_test_sources);
			var template_path = Path.Combine (Harness.RootDirectory, "bcl-test", TestName, TestName + ".csproj.template");
			var csproj_input = File.ReadAllText (template_path);
			var project_path = Path.Combine (Harness.RootDirectory, "bcl-test", TestName, TestName + ".csproj");
			var csproj_output = project_path;

			var sb = new StringBuilder ();

			var files_path = Path.GetDirectoryName (main_test_sources).Replace ("/", "\\");
			foreach (var s in main_test_files) {
				if (string.IsNullOrEmpty (s))
					continue;

				if (IsNotSupported (main_test_sources, s))
					continue;

				sb.AppendFormat ("    <Compile Include=\"{0}\\Test\\{1}\" Condition=\"'$(TargetFrameworkIdentifier)' == 'MonoTouch' Or '$(TargetFrameworkIdentifier)' == 'Xamarin.iOS'\">\r\n", files_path, s.Replace ("/", "\\").Trim ());

				var link_path = Path.GetDirectoryName (s);
				if (string.IsNullOrEmpty (link_path) || link_path [0] == '.')
					sb.AppendFormat ("      <Link>{0}</Link>\r\n", Path.GetFileName (s));
				else
					sb.AppendFormat ("      <Link>{0}\\{1}</Link>\r\n", link_path, Path.GetFileName (s));

				sb.AppendFormat ("    </Compile>\r\n");
			}

			var watch_files_path = Path.GetDirectoryName (watch_test_sources).Replace ("/", "\\");
			foreach (var s in watch_test_files) {
				if (string.IsNullOrEmpty (s))
					continue;

				if (IsNotSupported (watch_test_sources, s))
					continue;

				sb.AppendFormat ("    <Compile Include=\"{0}\\Test\\{1}\" Condition=\"'$(TargetFrameworkIdentifier)' == 'Xamarin.WatchOS'\">\r\n", watch_files_path, s.Replace ("/", "\\").Trim ());

				var link_path = Path.GetDirectoryName (s);
				if (string.IsNullOrEmpty (link_path) || link_path [0] == '.')
					sb.AppendFormat ("      <Link>{0}</Link>\r\n", Path.GetFileName (s));
				else
					sb.AppendFormat ("      <Link>{0}\\{1}</Link>\r\n", link_path, Path.GetFileName (s));

				sb.AppendFormat ("    </Compile>\r\n");
			}

			var tvos_files_path = Path.GetDirectoryName (tvos_test_sources).Replace ("/", "\\");
			foreach (var s in tvos_test_files) {
				if (string.IsNullOrEmpty (s))
					continue;

				if (IsNotSupported (tvos_test_sources, s))
					continue;

				sb.AppendFormat ("    <Compile Include=\"{0}\\Test\\{1}\" Condition=\"'$(TargetFrameworkIdentifier)' == 'Xamarin.TVOS'\">\r\n", tvos_files_path, s.Replace ("/", "\\").Trim ());

				var link_path = Path.GetDirectoryName (s);
				if (string.IsNullOrEmpty (link_path) || link_path [0] == '.')
					sb.AppendFormat ("      <Link>{0}</Link>\r\n", Path.GetFileName (s));
				else
					sb.AppendFormat ("      <Link>{0}\\{1}</Link>\r\n", link_path, Path.GetFileName (s));

				sb.AppendFormat ("    </Compile>\r\n");
			}

			Harness.Save (csproj_input.Replace ("#FILES#", sb.ToString ()), csproj_output);
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
}

