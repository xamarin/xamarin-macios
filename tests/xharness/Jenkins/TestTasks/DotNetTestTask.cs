using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;

namespace Xharness.Jenkins.TestTasks {
	class DotNetTestTask : RunTestTask {
		public string Filter { get; set; } = string.Empty;
		public DotNetTestTask (Jenkins jenkins, MSBuildTask build_task, IMlaunchProcessManager processManager)
			: base (jenkins, build_task, processManager)
		{
			MSBuildTask.SetDotNetEnvironmentVariables (Environment);
		}

		public override async Task RunTestAsync ()
		{
			using (var resource = await NotifyAndAcquireDesktopResourceAsync ()) {
				var trx = Logs.Create ($"results-{Timestamp}.trx", LogType.TrxLog.ToString ());
				var html = Logs.Create ($"results-{Timestamp}.html", LogType.HtmlLog.ToString ());
				var xml = Logs.Create ($"results-{Timestamp}.xml", LogType.NUnitResult.ToString ());

				var args = new List<string> {
					"test",
					BuildTask.ProjectFile,
					"--results-directory:" + Logs.Directory,
					"--logger:console;verbosity=detailed",
					"--logger:trx;LogFileName=" + Path.GetFileName (trx.FullPath),
					"--logger:html;LogFileName=" + Path.GetFileName (html.FullPath)
				};

				if (TestProject?.IsNUnitTestProject == true)
					args.Add ($"--logger:nunit;LogFileName={Path.GetFileName (xml.FullPath)}");

				var envTestFilter = global::System.Environment.GetEnvironmentVariable ("TEST_FILTER");
				if (!string.IsNullOrEmpty (envTestFilter)) {
					if (!string.IsNullOrEmpty (Filter)) {
						Filter = $"({envTestFilter}) & ({Filter})";
					} else {
						Filter = $"{envTestFilter}";
					}
					Jenkins.MainLog.WriteLine ($"Using test filter '{envTestFilter}' for '{TestName}'. Final filter: '{Filter}'");
				}

				if (!string.IsNullOrEmpty (Filter)) {
					args.Add ("--filter");
					args.Add (Filter);
				}

				WorkingDirectory = Path.GetDirectoryName (ProjectFile);

				await ExecuteProcessAsync (Jenkins.Harness.GetDotNetExecutable (Path.GetDirectoryName (ProjectFile)), args);

				try {
					var xmlDoc = new XmlDocument ();
					xmlDoc.LoadWithoutNetworkAccess (xml.FullPath);
					var nodes = xmlDoc.SelectNodes ("//attachments/attachment")!;
					foreach (XmlNode node in nodes) {
						var filePath = node.SelectSingleNode ("filePath")?.InnerText;
						var description = node.SelectSingleNode ("description")?.InnerText;
						if (File.Exists (filePath)) {
							Logs.AddFile (filePath, description);
						} else {
							MainLog.WriteLine ($"The xml file '{xml.FullPath}' has an attachment of a file that doesn't exist: {filePath}");
						}
					}
				} catch (Exception e) {
					MainLog.WriteLine ($"Failed to look for attachments in the XML file '{xml.FullPath}': {e}");
				}
			}
		}
	}
}
