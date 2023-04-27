using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

using Microsoft.DotNet.XHarness.Common.Logging;
using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Hardware;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;
using Xharness.Jenkins.TestTasks;

#nullable enable
namespace Xharness.Jenkins.Reports {
	class HtmlReportWriter : IReportWriter {

		readonly Jenkins jenkins;
		readonly IResourceManager resourceManager;
		readonly IResultParser resultParser;
		readonly string? linksPrefix;
		readonly bool embededResources;

		Dictionary<ILog, Tuple<long, object>> log_data = new Dictionary<ILog, Tuple<long, object>> ();
		string? previous_test_runs;

		// convenient
		IHarness Harness => jenkins.Harness;

		public HtmlReportWriter (Jenkins jenkins, IResourceManager resourceManager, IResultParser resultParser, string? linksPrefix = null, bool embeddedResources = false)
		{
			this.jenkins = jenkins ?? throw new ArgumentNullException (nameof (jenkins));
			this.resourceManager = resourceManager ?? throw new ArgumentNullException (nameof (resourceManager));
			this.resultParser = resultParser ?? throw new ArgumentNullException (nameof (resultParser));
			this.linksPrefix = linksPrefix;
			this.embededResources = embeddedResources;
		}

		string GetLinkFullPath (string path)
			=> linksPrefix is not null ? linksPrefix + LinkEncode (path) : LinkEncode (path); // vsdrops index paths are horrible, the use a ; and we cannot use combine, ex: https://vsdrop.corp.microsoft.com/file/v1/devdiv/20200612.13/3806978;/tests/index.html

		string GetResourcePath (string resource)
		{
			var executingDir = Path.GetDirectoryName (Assembly.GetExecutingAssembly ().Location);
			return Path.Combine (executingDir, resource);
		}

		void IncludeJavascript (StreamWriter writer)
		{
			if (embededResources) {
				var jsPath = GetResourcePath ("xharness.js");
				writer.WriteLine ("<script type='text/javascript'>");
				using (var reader = new StreamReader (jsPath)) {
					string? line = null;
					while ((line = reader.ReadLine ()) is not null) {
						writer.WriteLine (line);
					}
				}
				writer.WriteLine ("</script>");
			} else {
				writer.WriteLine (@"<script type='text/javascript' src='xharness.js'></script>");
			}
		}

		void IncludeCss (StreamWriter writer)
		{
			if (embededResources) {
				var cssPath = GetResourcePath ("xharness.css");
				writer.WriteLine ("<style>");
				using (var reader = new StreamReader (cssPath)) {
					string? line = null;
					while ((line = reader.ReadLine ()) is not null) {
						writer.WriteLine (line);
					}
				}
				writer.WriteLine ("</style>");
			} else {
				writer.WriteLine ("<link rel='stylesheet' href='xharness.css'>");
			}
		}
		public void Write (IList<ITestTask> allTasks, StreamWriter writer)
		{
			var id_counter = 0;

			var failedTests = allTasks.Where ((v) => v.Failed);
			var deviceNotFound = allTasks.Where ((v) => v.DeviceNotFound);
			var unfinishedTests = allTasks.Where ((v) => !v.Finished);
			var passedTests = allTasks.Where ((v) => v.Succeeded);
			var runningTests = allTasks.Where ((v) => v.Running && !v.Waiting);
			var buildingTests = allTasks.Where ((v) => v.Building && !v.Waiting);
			var runningQueuedTests = allTasks.Where ((v) => v.Running && v.Waiting);
			var buildingQueuedTests = allTasks.Where ((v) => v.Building && v.Waiting);

			writer.WriteLine ("<!DOCTYPE html>");
			writer.WriteLine ("<html onkeypress='keyhandler(event)' lang='en'>");
			if (jenkins.IsServerMode && jenkins.Populating)
				writer.WriteLine ("<meta http-equiv=\"refresh\" content=\"1\">");
			writer.WriteLine ("<head>");
			writer.WriteLine ("<meta charset=\"utf-8\"/>");
			IncludeCss (writer);
			writer.WriteLine ("<title>Test results</title>");
			IncludeJavascript (writer);
			if (jenkins.IsServerMode) {
				writer.WriteLine ("<script type='text/javascript'>");
				writer.WriteLine ("setTimeout (autorefresh, 1000);");
				writer.WriteLine ("</script>");
			}
			writer.WriteLine ("</head>");
			writer.WriteLine ("<body onload='oninitialload ();'>");

			if (jenkins.IsServerMode) {
				writer.WriteLine ("<div id='quit' style='position:absolute; top: 20px; right: 20px;'><a href='javascript:quit()'>Quit</a><br/><a id='ajax-log-button' href='javascript:toggleAjaxLogVisibility ();'>Show log</a></div>");
				writer.WriteLine ("<div id='ajax-log' style='position:absolute; top: 200px; right: 20px; max-width: 100px; display: none;'></div>");
			}

			writer.WriteLine ("<h1>Test results</h1>");

			writer.WriteLine ($"<span id='x{id_counter++}' class='autorefreshable'>");
			foreach (var log in jenkins.Logs)
				writer.WriteLine ("<a href='{0}' type='text/plain;charset=UTF-8'>{1}</a><br />", GetLinkFullPath (log.FullPath.Substring (jenkins.LogDirectory.Length + 1)), log.Description);
			writer.WriteLine ("</span>");

			var headerColor = "black";
			if (unfinishedTests.Any ()) {
				; // default
			} else if (failedTests.Any ()) {
				headerColor = "red";
			} else if (deviceNotFound.Any ()) {
				headerColor = "orange";
			} else if (passedTests.Any ()) {
				headerColor = "green";
			} else {
				headerColor = "gray";
			}

			writer.Write ($"<h2 style='color: {headerColor}'>");
			writer.Write ($"<span id='x{id_counter++}' class='autorefreshable'>");
			if (allTasks.Count == 0) {
				writer.Write ($"Loading tests...");
			} else if (unfinishedTests.Any ()) {
				writer.Write ($"Test run in progress (");
				var list = new List<string> ();
				var grouped = allTasks.GroupBy ((v) => v.ExecutionResult).OrderBy ((v) => (int) v.Key);
				foreach (var @group in grouped)
					list.Add ($"<span style='color: {@group.GetTestColor ()}'>{@group.Key.ToString ()}: {@group.Count ()}</span>");
				writer.Write (string.Join (", ", list));
				writer.Write (")");
			} else if (failedTests.Any ()) {
				writer.Write ($"{failedTests.Count ()} tests failed, ");
				if (deviceNotFound.Any ())
					writer.Write ($"{deviceNotFound.Count ()} tests' device not found, ");
				writer.Write ($"{passedTests.Count ()} tests passed");
			} else if (deviceNotFound.Any ()) {
				writer.Write ($"{deviceNotFound.Count ()} tests' device not found, {passedTests.Count ()} tests passed");
			} else if (passedTests.Any ()) {
				writer.Write ($"All {passedTests.Count ()} tests passed");
			} else {
				writer.Write ($"No tests selected.");
			}
			writer.Write ("</span>");
			writer.WriteLine ("</h2>");
			if (allTasks.Count > 0) {
				writer.WriteLine ($"<ul id='nav'>");
				if (jenkins.IsServerMode) {
					writer.WriteLine (@"
<li>Select
<ul>
	<li class=""adminitem""><a href='javascript:sendrequest (""/select?all"");'>All tests</a></li>
	<li class=""adminitem""><a href='javascript:sendrequest (""/select?all-device"");'>All device tests</a></li>
	<li class=""adminitem""><a href='javascript:sendrequest (""/select?all-simulator"");'>All simulator tests</a></li>
	<li class=""adminitem""><a href='javascript:sendrequest (""/select?all-ios"");'>All iOS tests</a></li>
	<li class=""adminitem""><a href='javascript:sendrequest (""/select?all-tvos"");'>All tvOS tests</a></li>
	<li class=""adminitem""><a href='javascript:sendrequest (""/select?all-watchos"");'>All watchOS tests</a></li>
	<li class=""adminitem""><a href='javascript:sendrequest (""/select?all-mac"");'>All Mac tests</a></li>
</ul>
</li>
<li>Deselect
<ul>
	<li class=""adminitem""><a href='javascript:sendrequest (""/deselect?all"");'>All tests</a></li>
	<li class=""adminitem""><a href='javascript:sendrequest (""/deselect?all-device"");'>All device tests</a></li>
	<li class=""adminitem""><a href='javascript:sendrequest (""/deselect?all-simulator"");'>All simulator tests</a></li>
	<li class=""adminitem""><a href='javascript:sendrequest (""/deselect?all-ios"");'>All iOS tests</a></li>
	<li class=""adminitem""><a href='javascript:sendrequest (""/deselect?all-tvos"");'>All tvOS tests</a></li>
	<li class=""adminitem""><a href='javascript:sendrequest (""/deselect?all-watchos"");'>All watchOS tests</a></li>
	<li class=""adminitem""><a href='javascript:sendrequest (""/deselect?all-mac"");'>All Mac tests</a></li>
</ul>
</li>
<li>Execute
<ul>
	<li class=""adminitem""><a href='javascript:sendrequest (""/run?alltests"");'>Run all tests</a></li>
	<li class=""adminitem""><a href='javascript:sendrequest (""/run?selected"");'>Run all selected tests</a></li>
	<li class=""adminitem""><a href='javascript:sendrequest (""/run?failed"");'>Run all failed tests</a></li>
	<li class=""adminitem""><a href='javascript:sendrequest (""/build?all"");'>Build all tests</a></li>
	<li class=""adminitem""><a href='javascript:sendrequest (""/build?selected"");'>Build all selected tests</a></li>
	<li class=""adminitem""><a href='javascript:sendrequest (""/build?failed"");'>Build all failed tests</a></li>
</ul>
</li>");
				}
				writer.WriteLine (@"
<li>Toggle visibility
<ul>
	<li class=""adminitem""><a href='javascript:toggleAll (true);'>Expand all</a></li>
	<li class=""adminitem""><a href='javascript:toggleAll (false);'>Collapse all</a></li>
	<li class=""adminitem""><a href='javascript:toggleVisibility (""toggleable-ignored"");'>Hide/Show ignored tests</a></li>
</ul>
</li>");
				if (jenkins.IsServerMode) {
					var include_system_permission_option = string.Empty;
					var include_system_permission_icon = string.Empty;
					if (Harness.IncludeSystemPermissionTests is null) {
						include_system_permission_option = "include-permission-tests";
						include_system_permission_icon = "2753";
					} else if (Harness.IncludeSystemPermissionTests.Value) {
						include_system_permission_option = "skip-permission-tests";
						include_system_permission_icon = "2705";
					} else {
						include_system_permission_option = "clear-permission-tests";
						include_system_permission_icon = "274C";
					}
					writer.WriteLine ($@"
<li>Reload
<ul>
	<li class=""adminitem""><a href='javascript:sendrequest (""/reload-devices"");'>Devices</a></li>
	<li class=""adminitem""><a href='javascript:sendrequest (""/reload-simulators"");'>Simulators</a></li>
</ul>
</li>

<li>Options
	<ul>
		<li class=""adminitem""><span id='{id_counter++}' class='autorefreshable'><a href='javascript:sendrequest (""/set-option?{(jenkins.CleanSuccessfulTestRuns ? "do-not-clean" : "clean")}"");'>&#x{(jenkins.CleanSuccessfulTestRuns ? "2705" : "274C")} Clean successful test runs</a></span></li>
		<li class=""adminitem""><span id='{id_counter++}' class='autorefreshable'><a href='javascript:sendrequest (""/set-option?{(jenkins.UninstallTestApp ? "do-not-uninstall-test-app" : "uninstall-test-app")}"");'>&#x{(jenkins.UninstallTestApp ? "2705" : "274C")} Uninstall the app from device before and after the test run</a></span></li>
		<li class=""adminitem""><span id='{id_counter++}' class='autorefreshable'><a href='javascript:sendrequest (""/set-option?{include_system_permission_option}"");'>&#x{include_system_permission_icon} Run tests that require system permissions (might put up permission dialogs)</a></span></li>
	</ul>
</li>
");
					if (previous_test_runs is null) {
						previous_test_runs = "\t<li>Previous test runs\t\t<ul>Loading ...</ul></li>";
						ThreadPool.QueueUserWorkItem ((v) => {
							var sb = new StringBuilder ();
							var previous = Directory.GetDirectories (Path.GetDirectoryName (jenkins.LogDirectory)).
									Select ((v) => Path.Combine (v, "index.html")).
										Where (File.Exists);
							if (previous.Any ()) {
								sb.AppendLine ("\t<li>Previous test runs");
								sb.AppendLine ("\t\t<ul>");
								foreach (var prev in previous.OrderBy ((v) => v).Reverse ()) {
									var dir = Path.GetFileName (Path.GetDirectoryName (prev));
									var ts = dir;
									var description = File.ReadAllLines (prev).Where ((v) => v.StartsWith ("<h2", StringComparison.Ordinal)).FirstOrDefault ();
									if (description is not null) {
										description = description.Substring (description.IndexOf ('>') + 1); // <h2 ...>
										description = description.Substring (description.IndexOf ('>') + 1); // <span id= ...>

										var h2end = description.LastIndexOf ("</h2>", StringComparison.Ordinal);
										if (h2end > -1)
											description = description.Substring (0, h2end);
										description = description.Substring (0, description.LastIndexOf ('<'));
									} else {
										description = "<unknown state>";
									}
									sb.AppendLine ($"\t\t\t<li class=\"adminitem\"><a href='/{dir}/index.html'>{ts}: {description}</a></li>");
								}
								sb.AppendLine ("\t\t</ul>");
								sb.AppendLine ("\t</li>");
							}
							previous_test_runs = sb.ToString ();
						});
					}
					if (!string.IsNullOrEmpty (previous_test_runs))
						writer.Write (previous_test_runs);
				}
				writer.WriteLine ("</ul>");
			}

			writer.WriteLine ("<div id='test-table' style='width: 100%; display: flex;'>");
			writer.WriteLine ("<div id='test-list'>");
			var orderedTasks = allTasks.GroupBy (v => v.TestName);

			if (jenkins.IsServerMode) {
				// In server mode don't take into account anything that can change during a test run
				// when ordering, since it's confusing to have the tests reorder by themselves while
				// you're looking at the web page.
				orderedTasks = orderedTasks.OrderBy ((v) => v.Key, StringComparer.OrdinalIgnoreCase);
			} else {
				// Put failed tests at the top and ignored tests at the end.
				// Then order alphabetically.
				orderedTasks = orderedTasks.OrderBy ((v) => {
					if (v.Any ((t) => t.Failed))
						return -1;
					if (v.All ((t) => t.Ignored))
						return 1;
					return 0;
				}).
				ThenBy ((v) => v.Key, StringComparer.OrdinalIgnoreCase);
			}
			foreach (var group in orderedTasks) {
				var singleTask = group.Count () == 1;
				var groupId = group.Key.Replace (' ', '-');

				// Test header for multiple tests
				if (!singleTask) {
					var autoExpand = !jenkins.IsServerMode && group.Any ((v) => v.Failed);
					var ignoredClass = group.All ((v) => v.Ignored) ? "toggleable-ignored" : string.Empty;
					var defaultExpander = autoExpand ? "-" : "+";
					var defaultDisplay = autoExpand ? "block" : "none";
					writer.Write ($"<div class='pdiv {ignoredClass}'>");
					writer.Write ($"<span id='button_container2_{groupId}' class='expander' onclick='javascript: toggleContainerVisibility2 (\"{groupId}\");'>{defaultExpander}</span>");
					writer.Write ($"<span id='x{id_counter++}' class='p1 autorefreshable' onclick='javascript: toggleContainerVisibility2 (\"{groupId}\");'>{group.Key}{RenderTextStates (group)}</span>");
					if (jenkins.IsServerMode) {
						var groupIds = string.Join (",", group.Where ((v) => v.KnownFailure is null).Select ((v) => v.ID.ToString ()));
						writer.Write ($" <span class='runall'><a href='javascript: runtest (\"{groupIds}\");'>Run all</a> <a href='javascript: buildtest (\"{groupIds}\");'>Build all</a></span>");
					}
					writer.WriteLine ("</div>");
					writer.WriteLine ($"<div id='test_container2_{groupId}' class='togglable' style='display: {defaultDisplay}; margin-left: 20px;'>");
				}

				// Test data
				var groupedByMode = group.GroupBy ((v) => v.Mode);
				foreach (var modeGroup in groupedByMode) {
					var multipleModes = modeGroup.Count () > 1;
					if (multipleModes) {
						var modeGroupId = id_counter++.ToString ();
						var autoExpand = !jenkins.IsServerMode && modeGroup.Any ((v) => v.Failed);
						var ignoredClass = modeGroup.All ((v) => v.Ignored) ? "toggleable-ignored" : string.Empty;
						var defaultExpander = autoExpand ? "-" : "+";
						var defaultDisplay = autoExpand ? "block" : "none";
						writer.Write ($"<div class='pdiv {ignoredClass}'>");
						writer.Write ($"<span id='button_container2_{modeGroupId}' class='expander' onclick='javascript: toggleContainerVisibility2 (\"{modeGroupId}\");'>{defaultExpander}</span>");
						writer.Write ($"<span id='x{id_counter++}' class='p2 autorefreshable' onclick='javascript: toggleContainerVisibility2 (\"{modeGroupId}\");'>{modeGroup.Key}{RenderTextStates (modeGroup)}</span>");
						if (jenkins.IsServerMode) {
							var modeGroupIds = string.Join (",", modeGroup.Where ((v) => v.KnownFailure is null).Select ((v) => v.ID.ToString ()));
							writer.Write ($" <span class='runall'><a href='javascript: runtest (\"{modeGroupIds}\");'>Run all</a> <a href='javascript: buildtest (\"{modeGroupIds}\");'>Build all</a></span>");
						}
						writer.WriteLine ("</div>");

						writer.WriteLine ($"<div id='test_container2_{modeGroupId}' class='togglable' style='display: {defaultDisplay}; margin-left: 20px;'>");
					}
					foreach (var test in modeGroup.OrderBy ((v) => v.Variation, StringComparer.OrdinalIgnoreCase)) {
						var runTest = test as RunTestTask;
						string state;
						state = test.ExecutionResult.ToString ();
						var log_id = id_counter++;
						var logs = test.AggregatedLogs.ToList ();
						string title;
						if (multipleModes) {
							title = test.Variation ?? "Default";
						} else if (singleTask) {
							title = test.TestName;
						} else {
							title = test.Mode;
						}

						var autoExpand = !jenkins.IsServerMode && test.Failed;
						var ignoredClass = test.Ignored ? "toggleable-ignored" : string.Empty;
						var defaultExpander = autoExpand ? "&nbsp;" : "+";
						var defaultDisplay = autoExpand ? "block" : "none";
						var buildOnly = test.BuildOnly ? ", BuildOnly" : string.Empty;

						writer.Write ($"<div class='pdiv {ignoredClass}'>");
						writer.Write ($"<span id='button_{log_id}' class='expander' onclick='javascript: toggleLogVisibility (\"{log_id}\");'>{defaultExpander}</span>");
						// we have a very common error we want to make this easier for the person that is dealing with the results
						var knownFailure = string.Empty;
						if (test.KnownFailure is not null)
							knownFailure = $" <a href='{test.KnownFailure.IssueLink}'>{test.KnownFailure.HumanMessage}</a>";
						writer.Write ($"<span id='x{id_counter++}' class='p3 autorefreshable' onclick='javascript: toggleLogVisibility (\"{log_id}\");'>{title} (<span style='color: {test.GetTestColor ()}'>{state}{knownFailure}</span>{buildOnly}) </span>");
						if (jenkins.IsServerMode) {
							writer.Write ($" <span id='x{id_counter++}' class='autorefreshable'>");
							if (test.Waiting) {
								writer.Write ($" <a class='runall' href='javascript:stoptest ({test.ID})'>Stop</a> ");
							} else if (test.InProgress && !test.Built) {
								// Stopping is not implemented for tasks that are already executing
							} else {
								writer.Write ($" <a class='runall' href='javascript:runtest ({test.ID})'>Run</a> ");
								writer.Write ($" <a class='runall' href='javascript:buildtest ({test.ID})'>Build</a> ");
							}
							writer.Write ("</span> ");
						}
						writer.WriteLine ("</div>");
						writer.WriteLine ($"<div id='logs_{log_id}' class='autorefreshable logs togglable' data-onautorefresh='{log_id}' style='display: {defaultDisplay};'>");

						var testAssemblies = test.ReferencedNunitAndXunitTestAssemblies;
						if (testAssemblies.Any ())
							writer.WriteLine ($"Test assemblies:<br/>- {String.Join ("<br/>- ", testAssemblies)}<br />");

						if (test.KnownFailure is not null)
							writer.WriteLine ($"Known failure: <a href='{test.KnownFailure.IssueLink}'>{test.KnownFailure.HumanMessage}</a> <br />");

						if (!string.IsNullOrEmpty (test.FailureMessage)) {
							var msg = test.FailureMessage.AsHtml ();
							var prefix = test.Ignored ? "Ignored" : "Failure";
							if (test.FailureMessage.Contains ('\n')) {
								writer.WriteLine ($"{prefix}:<br /> <div style='margin-left: 20px;'>{msg}</div>");
							} else {
								writer.WriteLine ($"{prefix}: {msg} <br />");
							}
						}
						var progressMessage = test.ProgressMessage;
						if (!string.IsNullOrEmpty (progressMessage))
							writer.WriteLine (progressMessage.AsHtml () + " <br />");

						if (runTest is not null) {
							if (runTest.BuildTask.Duration.Ticks > 0) {
								writer.WriteLine ($"Project file: {runTest.BuildTask.ProjectFile} <br />");
								writer.WriteLine ($"Platform: {runTest.BuildTask.ProjectPlatform} Configuration: {runTest.BuildTask.ProjectConfiguration} <br />");
								IEnumerable<IDevice>? candidates = (runTest as RunDeviceTask)?.Candidates;
								if (candidates is null)
									candidates = (runTest as RunSimulatorTask)?.Candidates;
								if (candidates is not null) {
									writer.WriteLine ($"Candidate devices:<br />");
									try {
										foreach (var candidate in candidates)
											writer.WriteLine ($"&nbsp;&nbsp;&nbsp;&nbsp;{candidate.Name} (Version: {candidate.OSVersion})<br />");
									} catch (Exception e) {
										writer.WriteLine ($"&nbsp;&nbsp;&nbsp;&nbsp;Failed to list candidates: {e}");
									}
								}
								writer.WriteLine ($"Build duration: {runTest.BuildTask.Duration} <br />");
							}
							if (test.Duration.Ticks > 0)
								writer.WriteLine ($"Time Elapsed:  {test.TestName} - (waiting time : {test.WaitingDuration} , running time : {test.Duration}) <br />");
							var runDeviceTest = runTest as RunDeviceTask;
							if (runDeviceTest?.Device is not null) {
								if (runDeviceTest.CompanionDevice is not null) {
									writer.WriteLine ($"Device: {runDeviceTest.Device.Name} ({runDeviceTest.CompanionDevice.Name}) <br />");
								} else {
									writer.WriteLine ($"Device: {runDeviceTest.Device.Name} <br />");
								}
							}
						} else {
							if (test.Duration.Ticks > 0)
								writer.WriteLine ($"Duration: {test.Duration} <br />");
						}

						if (logs.Count () > 0) {
							foreach (var log in logs) {
								if (!(log is IFileBackedLog fileLog)) {
									continue;
								}

								log.Flush ();
								var exists = File.Exists (fileLog.FullPath);
								string log_type = System.Web.MimeMapping.GetMimeMapping (fileLog.FullPath);
								string log_target;
								switch (log_type) {
								case "text/xml":
									log_target = "_top";
									break;
								case "text/plain":
									log_type += ";charset=UTF-8";
									goto default;
								default:
									log_target = "_self";
									break;
								}
								if (!exists) {
									writer.WriteLine ("<a href='{0}' type='{2}' target='{3}'>{1}</a> (does not exist)<br />", GetLinkFullPath (fileLog.FullPath.Substring (jenkins.LogDirectory.Length + 1)), log.Description, log_type, log_target);
								} else if (log.Description == LogType.BuildLog.ToString ()) {
									var binlog = fileLog.FullPath.Replace (".txt", ".binlog");
									if (File.Exists (binlog)) {
										var textLink = string.Format ("<a href='{0}' type='{2}' target='{3}'>{1}</a>", GetLinkFullPath (fileLog.FullPath.Substring (jenkins.LogDirectory.Length + 1)), log.Description, log_type, log_target);
										var binLink = string.Format ("<a href='{0}' type='{2}' target='{3}' style='display:{4}'>{1}</a><br />", GetLinkFullPath (binlog.Substring (jenkins.LogDirectory.Length + 1)), "Binlog download", log_type, log_target, test.Building ? "none" : "inline");
										writer.Write ("{0} {1}", textLink, binLink);
									} else {
										writer.WriteLine ("<a href='{0}' type='{2}' target='{3}'>{1}</a><br />", GetLinkFullPath (fileLog.FullPath.Substring (jenkins.LogDirectory.Length + 1)), log.Description, log_type, log_target);
									}
								} else {
									writer.WriteLine ("<a href='{0}' type='{2}' target='{3}'>{1}</a><br />", GetLinkFullPath (fileLog.FullPath.Substring (jenkins.LogDirectory.Length + 1)), log.Description, log_type, log_target);
								}
								if (!exists) {
									// Don't try to parse files that don't exist
								} else if (log.Description == LogType.TestLog.ToString () || log.Description == LogType.ExecutionLog.ToString ()) {
									string summary;
									List<string> fails;
									try {
										using (var reader = fileLog.GetReader ()) {
											Tuple<long, object> data;
											if (!log_data.TryGetValue (log, out data) || data.Item1 != reader.BaseStream.Length) {
												summary = string.Empty;
												fails = new List<string> ();
												while (!reader.EndOfStream) {
													string? line = reader.ReadLine ()?.Trim ();
													if (line is null)
														continue;
													// Skip any timestamps if the file is timestamped
													if (log.Timestamp)
														if (line.Length > "HH:mm:ss.fffffff".Length)
															line = line.Substring ("HH:mm:ss.fffffff".Length).Trim ();
														else if (line.Length == "HH:mm:ss.fffffff".Length)
															line = "";
													if (line.StartsWith ("Tests run:", StringComparison.Ordinal)) {
														summary = line;
													} else if (line.StartsWith ("[FAIL]", StringComparison.Ordinal)) {
														if (fails.Count < 100) {
															fails.Add (line);
														} else if (fails.Count == 100) {
															fails.Add ("...");
														}
													}
												}
											} else {
												var data_tuple = (Tuple<string, List<string>>) data.Item2;
												summary = data_tuple.Item1;
												fails = data_tuple.Item2;
											}
										}
										if (fails.Count > 0) {
											writer.WriteLine ("<div style='padding-left: 15px;'>");
											foreach (var fail in fails)
												writer.WriteLine ("{0} <br />", fail.AsHtml ());
											writer.WriteLine ("</div>");
										}
										if (!string.IsNullOrEmpty (summary))
											writer.WriteLine ("<span style='padding-left: 15px;'>{0}</span><br />", summary);
									} catch (Exception ex) {
										writer.WriteLine ("<span style='padding-left: 15px;'>Could not parse log file: {0}: {1}</span><br />", ex.Message.AsHtml (), ex.StackTrace.AsHtml ());
									}
								} else if (log.Description == LogType.BuildLog.ToString ()) {
									HashSet<string> errors;
									try {
										using (var reader = fileLog.GetReader ()) {
											Tuple<long, object> data;
											if (!log_data.TryGetValue (log, out data) || data.Item1 != reader.BaseStream.Length) {
												errors = new HashSet<string> ();
												while (!reader.EndOfStream) {
													string? line = reader.ReadLine ()?.Trim ();
													if (line is null)
														continue;
													// Sometimes we put error messages in pull request descriptions
													// Then Jenkins create environment variables containing the pull request descriptions (and other pull request data)
													// So exclude any lines matching 'ghprbPull', to avoid reporting those environment variables as build errors.
													if (line.Contains (": error") && !line.Contains ("ghprbPull")) {
														errors.Add (line);
														if (errors.Count > 20) {
															errors.Add ("...");
															break;
														}
													}
												}
												log_data [log] = new Tuple<long, object> (reader.BaseStream.Length, errors);
											} else {
												errors = (HashSet<string>) data.Item2;
											}
										}
										if (errors.Count > 0) {
											writer.WriteLine ("<div style='padding-left: 15px;'>");
											foreach (var error in errors)
												writer.WriteLine ("{0} <br />", error.AsHtml ());
											writer.WriteLine ("</div>");
										}
									} catch (Exception ex) {
										writer.WriteLine ("<span style='padding-left: 15px;'>Could not parse log file: {0}: {1}</span><br />", ex.Message.AsHtml (), ex.StackTrace.AsHtml ());
									}
								} else if (log.Description == LogType.NUnitResult.ToString () || log.Description == LogType.XmlLog.ToString ()) {
									try {
										if (File.Exists (fileLog.FullPath) && new FileInfo (fileLog.FullPath).Length > 0) {
											if (resultParser.IsValidXml (fileLog.FullPath, out var jargon)) {
												resultParser.GenerateTestReport (writer, fileLog.FullPath, jargon);
											}
										}
									} catch (Exception ex) {
										writer.WriteLine ($"<span style='padding-left: 15px;'>Could not parse {log.Description}: {ex.Message.AsHtml ()} : {ex.StackTrace.AsHtml ()}</span><br />");
									}
								} else if (log.Description == LogType.TrxLog.ToString ()) {
									try {
										if (resultParser.IsValidXml (fileLog.FullPath, out var jargon)) {
											resultParser.GenerateTestReport (writer, fileLog.FullPath, jargon);
										}
									} catch (Exception ex) {
										writer.WriteLine ($"<span style='padding-left: 15px;'>Could not parse {log.Description}: {ex.Message.AsHtml ()}</span><br />");
									}
								}
							}
						}
						writer.WriteLine ("</div>");
					}
					if (multipleModes)
						writer.WriteLine ("</div>");
				}
				if (!singleTask)
					writer.WriteLine ("</div>");
			}
			writer.WriteLine ("</div>");
			if (jenkins.IsServerMode) {
				writer.WriteLine ("<div id='test-status' style='margin-left: 100px;' class='autorefreshable'>");
				if (failedTests.Count () == 0) {
					foreach (var group in failedTests.GroupBy ((v) => v.TestName)) {
						var enumerableGroup = group as IEnumerable<AppleTestTask>;
						if (enumerableGroup is not null) {
							writer.WriteLine ("<a href='#test_{2}'>{0}</a> ({1})<br />", group.Key, string.Join (", ", enumerableGroup.Select ((v) => string.Format ("<span style='color: {0}'>{1}</span>", v.GetTestColor (), string.IsNullOrEmpty (v.Mode) ? v.ExecutionResult.ToString () : v.Mode)).ToArray ()), group.Key.Replace (' ', '-'));
							continue;
						}

						throw new NotImplementedException ();
					}
				}

				if (buildingTests.Any ()) {
					writer.WriteLine ($"<h3>{buildingTests.Count ()} building tests:</h3>");
					foreach (var test in buildingTests) {
						var runTask = test as RunTestTask;
						var buildDuration = string.Empty;
						if (runTask is not null)
							buildDuration = runTask.BuildTask.Duration.ToString ();
						writer.WriteLine ($"<a href='#test_{test.TestName}'>{test.TestName} ({test.Mode})</a> {buildDuration}<br />");
					}
				}

				if (runningTests.Any ()) {
					writer.WriteLine ($"<h3>{runningTests.Count ()} running tests:</h3>");
					foreach (var test in runningTests) {
						writer.WriteLine ($"<a href='#test_{test.TestName}'>{test.TestName} ({test.Mode})</a> {test.Duration.ToString ()} {("\n\t" + test.ProgressMessage).AsHtml ()}<br />");
					}
				}

				if (buildingQueuedTests.Any ()) {
					writer.WriteLine ($"<h3>{buildingQueuedTests.Count ()} tests in build queue:</h3>");
					foreach (var test in buildingQueuedTests) {
						writer.WriteLine ($"<a href='#test_{test.TestName}'>{test.TestName} ({test.Mode})</a><br />");
					}
				}

				if (runningQueuedTests.Any ()) {
					writer.WriteLine ($"<h3>{runningQueuedTests.Count ()} tests in run queue:</h3>");
					foreach (var test in runningQueuedTests) {
						writer.WriteLine ($"<a href='#test_{test.TestName}'>{test.TestName} ({test.Mode})</a><br />");
					}
				}

				var resources = resourceManager.GetAll ();
				if (resources.Any ()) {
					writer.WriteLine ($"<h3>Devices/Resources:</h3>");
					foreach (var dr in resources.OrderBy ((v) => v.Description, StringComparer.OrdinalIgnoreCase)) {
						writer.WriteLine ($"{dr.Description} - {dr.Users}/{dr.MaxConcurrentUsers} users - {dr.QueuedUsers} in queue<br />");
					}
				}
			}
			writer.WriteLine ("</div>");
			writer.WriteLine ("</div>");
			writer.WriteLine ("</body>");
			writer.WriteLine ("</html>");
		}

		static string LinkEncode (string path)
		{
			return System.Web.HttpUtility.UrlEncode (path).Replace ("%2f", "/").Replace ("+", "%20");
		}

		string RenderTextStates (IEnumerable<ITestTask> tests)
		{
			// Create a collection of all non-ignored tests in the group (unless all tests were ignored).
			var allIgnored = tests.All ((v) => v.ExecutionResult == TestExecutingResult.Ignored);
			IEnumerable<ITestTask> relevantGroup;
			if (allIgnored) {
				relevantGroup = tests;
			} else {
				relevantGroup = tests.Where ((v) => v.ExecutionResult != TestExecutingResult.NotStarted);
			}
			if (!relevantGroup.Any ())
				return string.Empty;

			var results = relevantGroup
				.GroupBy ((v) => v.ExecutionResult)
				.Select ((v) => v.First ()) // GroupBy + Select = Distinct (lambda)
				.OrderBy ((v) => v.ID)
				.Select ((v) => $"<span style='color: {v.GetTestColor ()}'>{v.ExecutionResult.ToString ()}</span>")
				.ToArray ();
			return " (" + string.Join ("; ", results) + ")";
		}
	}
}
