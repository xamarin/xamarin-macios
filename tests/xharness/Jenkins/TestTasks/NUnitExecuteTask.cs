using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;

namespace Xharness.Jenkins.TestTasks
{
	class NUnitExecuteTask : RunTestTask
	{
		public string TestLibrary;
		public string TestExecutable;
		public bool ProduceHtmlReport = true;
		public bool InProcess;

		public NUnitExecuteTask (BuildToolTask build_task, IProcessManager processManager)
			: base (build_task, processManager)
		{
		}

		public void FindNUnitConsoleExecutable (ILog log)
		{
			if (!string.IsNullOrEmpty (TestExecutable)) {
				log.WriteLine ("Using existing executable: {0}", TestExecutable);
				return;
			}

			var packages_conf = Path.Combine (Path.GetDirectoryName (TestProject.Path), "packages.config");
			var nunit_version = string.Empty;
			var is_packageref = false;
			const string default_nunit_version = "3.9.0";

			if (!File.Exists (packages_conf)) {
				var xml = new XmlDocument ();
				xml.LoadWithoutNetworkAccess (TestProject.Path);
				var packageref = xml.SelectSingleNode ("//*[local-name()='PackageReference' and @Include = 'NUnit.ConsoleRunner']");
				if (packageref != null) {
					is_packageref = true;
					nunit_version = packageref.Attributes ["Version"].InnerText;
					log.WriteLine ("Found PackageReference in {0} for NUnit.ConsoleRunner {1}", TestProject, nunit_version);
				} else {
					nunit_version = default_nunit_version;
					log.WriteLine ("No packages.config found for {0}: assuming nunit version is {1}", TestProject, nunit_version);
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
							nunit_version = reader.GetAttribute ("version");
							break;
						}
					}
				}
				if (nunit_version == string.Empty) {
					nunit_version = default_nunit_version;
					log.WriteLine ("Could not find the NUnit.ConsoleRunner element in {0}, using the default version ({1})", packages_conf, nunit_version);
				} else {
					log.WriteLine ("Found the NUnit.ConsoleRunner/NUnit.Runners element in {0} for {2}, version is: {1}", packages_conf, nunit_version, TestProject.Path);
				}
			}

			if (is_packageref) {
				TestExecutable = Path.Combine (Harness.RootDirectory, "..", "tools", $"nunit3-console-{nunit_version}");
				if (!File.Exists (TestExecutable))
					throw new FileNotFoundException ($"The helper script to execute the unit tests does not exist: {TestExecutable}");
				WorkingDirectory = Path.GetDirectoryName (TestProject.Path);
			} else if (nunit_version [0] == '2') {
				TestExecutable = Path.Combine (Harness.RootDirectory, "..", "packages", "NUnit.Runners." + nunit_version, "tools", "nunit-console.exe");
				WorkingDirectory = Path.Combine (Path.GetDirectoryName (TestExecutable), "lib");
			} else {
				TestExecutable = Path.Combine (Harness.RootDirectory, "..", "packages", "NUnit.ConsoleRunner." + nunit_version, "tools", "nunit3-console.exe");
				WorkingDirectory = Path.GetDirectoryName (TestLibrary);
			}
			TestExecutable = Path.GetFullPath (TestExecutable);
			WorkingDirectory = Path.GetFullPath (WorkingDirectory);
			if (!File.Exists (TestExecutable))
				throw new FileNotFoundException ($"The nunit executable '{TestExecutable}' doesn't exist.");
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

		protected override async Task RunTestAsync ()
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
					args.Add ("--labels=All");
					if (InProcess)
						args.Add ("--inprocess");
				} else {
					args.Add ("-xml=" + xmlLog);
					args.Add ("-labels");
				}

				await ExecuteProcessAsync (log, Harness.XIBuildPath, args);

				if (ProduceHtmlReport) {
					try {
						var output = Logs.Create ($"Log-{Timestamp}.html", "HTML log");
						using (var srt = new StringReader (File.ReadAllText (Path.Combine (Harness.RootDirectory, "HtmlTransform.xslt")))) {
							using (var sri = File.OpenRead (xmlLog)) {
								using (var xrt = XmlReader.Create (srt)) {
									using (var xri = XmlReader.Create (sri)) {
										var xslt = new System.Xml.Xsl.XslCompiledTransform ();
										xslt.Load (xrt);
										using (var xwo = XmlWriter.Create (output as TextWriter, xslt.OutputSettings)) // use OutputSettings of xsl, so it can be output as HTML
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
