using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.DotNet.XHarness.Common.Logging;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;

namespace Xharness.Jenkins.TestTasks {
	class NUnitExecuteTask : RunTestTask {
		public string TestLibrary;
		public string TestExecutable;
		public bool ProduceHtmlReport = true;
		public bool InProcess;

		public NUnitExecuteTask (Jenkins jenkins, BuildToolTask build_task, IMlaunchProcessManager processManager)
			: base (jenkins, build_task, processManager)
		{
		}

		public static bool TryGetNUnitVersion (ILog log, string csproj, out string version, out bool isPackageRef)
		{
			const string default_nunit_version = "3.9.0";

			isPackageRef = false;
			version = string.Empty;

			var packages_conf = Path.Combine (Path.GetDirectoryName (csproj), "packages.config");
			if (!File.Exists (packages_conf)) {
				var xml = new XmlDocument ();
				xml.LoadWithoutNetworkAccess (csproj);
				var packageref = xml.SelectSingleNode ("//*[local-name()='PackageReference' and (@Include = 'NUnit.ConsoleRunner' or @Include = 'NUnit.Runners')]");
				if (packageref is not null) {
					isPackageRef = true;
					version = packageref.Attributes ["Version"].InnerText;
					log.WriteLine ("Found PackageReference in {0} for NUnit.ConsoleRunner {1}", csproj, version);
				} else {
					version = default_nunit_version;
					log.WriteLine ("No packages.config found for {0}: assuming nunit version is {1}", csproj, version);
				}
			} else {
				using (var str = new StreamReader (packages_conf)) {
					using (var reader = XmlReader.Create (str)) {
						while (reader.Read ()) {
							if (reader.NodeType != XmlNodeType.Element)
								continue;
							if (reader.Name != "package")
								continue;
							var id = reader.GetAttribute ("id");
							if (id != "NUnit.ConsoleRunner" && id != "NUnit.Runners")
								continue;
							version = reader.GetAttribute ("version");
							break;
						}
					}
				}
				if (version == string.Empty) {
					version = default_nunit_version;
					log.WriteLine ("Could not find the NUnit.ConsoleRunner element in {0}, using the default version ({1})", packages_conf, version);
				} else {
					log.WriteLine ("Found the NUnit.ConsoleRunner/NUnit.Runners element in {0} for {2}, version is: {1}", packages_conf, version, csproj);
				}
			}

			return true;
		}

		public static bool TryGetNUnitExecutionSettings (ILog log, string csproj, string testLibrary, out string testExecutable, out string workingDirectory)
		{
			if (!TryGetNUnitVersion (log, csproj, out var nunit_version, out var is_packageref)) {
				log.WriteLine ($"Failed to find NUnit version for {csproj}");
				throw new Exception ($"Failed to find NUnit version for {csproj}");
			}

			if (is_packageref) {
				if (nunit_version [0] == '2') {
					testExecutable = Path.Combine (HarnessConfiguration.RootDirectory, "..", "packages", "NUnit.Runners", nunit_version, "tools", "nunit-console.exe");
				} else {
					testExecutable = Path.Combine (HarnessConfiguration.RootDirectory, "..", "tools", $"nunit3-console-{nunit_version}");
				}
				if (!File.Exists (testExecutable))
					throw new FileNotFoundException ($"The helper script to execute the unit tests does not exist: {testExecutable}");
				workingDirectory = Path.GetDirectoryName (csproj);
			} else if (nunit_version [0] == '2') {
				testExecutable = Path.Combine (HarnessConfiguration.RootDirectory, "..", "packages", "NUnit.Runners." + nunit_version, "tools", "nunit-console.exe");
				workingDirectory = Path.Combine (Path.GetDirectoryName (testExecutable), "lib");
			} else {
				testExecutable = Path.Combine (HarnessConfiguration.RootDirectory, "..", "packages", "NUnit.ConsoleRunner." + nunit_version, "tools", "nunit3-console.exe");
				workingDirectory = Path.GetDirectoryName (testLibrary);
			}
			testExecutable = Path.GetFullPath (testExecutable);
			workingDirectory = Path.GetFullPath (workingDirectory);
			if (!File.Exists (testExecutable))
				throw new FileNotFoundException ($"The nunit executable '{testExecutable}' doesn't exist.");

			return true;
		}

		public void FindNUnitConsoleExecutable (ILog log)
		{
			if (!string.IsNullOrEmpty (TestExecutable)) {
				log.WriteLine ("Using existing executable: {0}", TestExecutable);
				return;
			}

			if (!TryGetNUnitExecutionSettings (log, TestProject.Path, TestLibrary, out var testExecutable, out var workingDirectory))
				throw new Exception ($"Unable to get NUnit execution settings for {TestProject.Path}");
			TestExecutable = testExecutable;
			WorkingDirectory = workingDirectory;
		}

		public bool IsNUnit3 {
			get {
				return Path.GetFileName (TestExecutable).Contains ("unit3-console");
			}
		}
		public override IEnumerable<ILog> AggregatedLogs {
			get {
				return base.AggregatedLogs.Union (BuildTask.Logs);
			}
		}

		public override string Mode {
			get {
				return base.Mode ?? "NUnit";
			}
			set {
				base.Mode = value;
			}
		}

		public override async Task RunTestAsync ()
		{
			using (var resource = await NotifyAndAcquireDesktopResourceAsync ()) {
				var xmlLog = Logs.CreateFile ($"log-{Timestamp}.xml", LogType.XmlLog.ToString ());
				var log = Logs.Create ($"execute-{Timestamp}.txt", LogType.ExecutionLog.ToString ());
				FindNUnitConsoleExecutable (log);

				var args = new List<string> ();
				args.Add ("-t");
				args.Add ("--");
				args.Add (Path.GetFullPath (TestExecutable));
				args.Add (Path.GetFullPath (TestLibrary));
				if (IsNUnit3) {
					args.Add ("-result=" + xmlLog + ";format=nunit2");
					args.Add ("--labels=After");
					if (InProcess)
						args.Add ("--inprocess");
				} else {
					args.Add ("-xml=" + xmlLog);
					args.Add ("-labels");
				}

				await ExecuteProcessAsync (log, Jenkins.Harness.XIBuildPath, args);

				if (ProduceHtmlReport) {
					try {
						var output = Logs.Create ($"Log-{Timestamp}.html", "HTML log");
						using (var srt = new StringReader (File.ReadAllText (Path.Combine (RootDirectory, "HtmlTransform.xslt")))) {
							using (var sri = File.OpenRead (xmlLog)) {
								using (var xrt = XmlReader.Create (srt)) {
									using (var xri = XmlReader.Create (sri)) {
										var xslt = new System.Xml.Xsl.XslCompiledTransform ();
										xslt.Load (xrt);
										using (var xwo = XmlWriter.Create (File.Create (output.FullPath), xslt.OutputSettings)) // use OutputSettings of xsl, so it can be output as HTML
										{
											xslt.Transform (xri, xwo);
										}
									}
								}
							}
						}
					} catch (Exception e) {
						log.WriteLine ("Failed to produce HTML report: {0}", e);
					}
				}
			}
		}

		public override void Reset ()
		{
			base.Reset ();
			BuildTask?.Reset ();
		}
	}
}
