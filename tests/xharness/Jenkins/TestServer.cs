using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;
using Xharness.Jenkins.Reports;
using Xharness.Jenkins.TestTasks;

namespace Xharness.Jenkins {

	class TestServer {

		public Task RunAsync (Jenkins jenkins, HtmlReportWriter htmlReportWriter)
		{
			var server = new HttpListener ();

			// Try and find an unused port
			int attemptsLeft = 50;
			int port = 51234; // Try this port first, to try to not vary between runs just because.
			Random r = new Random ((int) DateTime.Now.Ticks);
			while (attemptsLeft-- > 0) {
				var newPort = port != 0 ? port : r.Next (49152, 65535); // The suggested range for dynamic ports is 49152-65535 (IANA)
				server.Prefixes.Clear ();
				server.Prefixes.Add ("http://*:" + newPort + "/");
				try {
					server.Start ();
					port = newPort;
					break;
				} catch (Exception ex) {
					jenkins.MainLog.WriteLine ("Failed to listen on port {0}: {1}", newPort, ex.Message);
					port = 0;
				}
			}
			jenkins.MainLog.WriteLine ($"Created server on localhost:{port}");

			var tcs = new TaskCompletionSource<bool> ();
			var thread = new System.Threading.Thread (() => {
				while (server.IsListening) {
					var context = server.GetContext ();
					var request = context.Request;
					var response = context.Response;
					var arguments = System.Web.HttpUtility.ParseQueryString (request.Url.Query);
					try {
						var allTasks = jenkins.Tasks.SelectMany ((v) => {
							var rv = new List<ITestTask> ();
							var runsim = v as AggregatedRunSimulatorTask;
							if (runsim is not null)
								rv.AddRange (runsim.Tasks);
							rv.Add (v);
							return rv;
						});

						IEnumerable<ITestTask> find_tasks (StreamWriter writer, string ids)
						{
							IEnumerable<ITestTask> tasks;
							switch (request.Url.Query) {
							case "?all":
								tasks = jenkins.Tasks;
								break;
							case "?selected":
								tasks = allTasks.Where ((v) => !v.Ignored);
								break;
							case "?failed":
								tasks = allTasks.Where ((v) => v.Failed);
								break;
							case "?":
								writer.WriteLine ("No tasks specified");
								return Array.Empty<AppleTestTask> ();
							default:
								var id_inputs = ids.Substring (1).Split (',');
								var rv = new List<ITestTask> (id_inputs.Length);
								foreach (var id_input in id_inputs) {
									if (int.TryParse (id_input, out var id)) {
										var task = jenkins.Tasks.FirstOrDefault ((t) => t.ID == id);
										if (task is null)
											task = jenkins.Tasks.Where ((v) => v is AggregatedRunSimulatorTask).Cast<AggregatedRunSimulatorTask> ().SelectMany ((v) => v.Tasks).FirstOrDefault ((t) => t.ID == id);
										if (task is null) {
											writer.WriteLine ($"Could not find test {id}");
										} else {
											rv.Add (task);
										}
									} else {
										writer.WriteLine ($"Could not parse {arguments ["id"]}");
									}
								}
								tasks = rv;
								break;
							}
							return tasks;
						}

						string serveFile = null;
						switch (request.Url.LocalPath) {
						case "/":
							response.ContentType = System.Net.Mime.MediaTypeNames.Text.Html;
							using (var writer = new StreamWriter (response.OutputStream)) {
								htmlReportWriter.Write (jenkins.Tasks, writer);
							}
							break;
						case "/set-option":
							response.ContentType = System.Net.Mime.MediaTypeNames.Text.Plain;
							switch (request.Url.Query) {
							case "?clean":
								jenkins.CleanSuccessfulTestRuns = true;
								break;
							case "?do-not-clean":
								jenkins.CleanSuccessfulTestRuns = false;
								break;
							case "?uninstall-test-app":
								jenkins.UninstallTestApp = true;
								break;
							case "?do-not-uninstall-test-app":
								jenkins.UninstallTestApp = false;
								break;
							case "?skip-permission-tests":
								jenkins.Harness.IncludeSystemPermissionTests = false;
								break;
							case "?include-permission-tests":
								jenkins.Harness.IncludeSystemPermissionTests = true;
								break;
							case "?clear-permission-tests":
								jenkins.Harness.IncludeSystemPermissionTests = null;
								break;
							default:
								throw new NotImplementedException (request.Url.Query);
							}
							using (var writer = new StreamWriter (response.OutputStream)) {
								writer.WriteLine ("OK");
							}
							break;
						case "/select":
						case "/deselect":
							response.ContentType = System.Net.Mime.MediaTypeNames.Text.Plain;
							using (var writer = new StreamWriter (response.OutputStream)) {
								foreach (var task in allTasks) {
									bool? is_match = null;
									if (!(task.Ignored || task.NotStarted))
										continue;
									switch (request.Url.Query) {
									case "?all":
										is_match = true;
										break;
									case "?all-device":
										is_match = task is RunDeviceTask;
										break;
									case "?all-simulator":
										is_match = task is RunSimulatorTask;
										break;
									case "?all-ios":
										switch (task.Platform) {
										case TestPlatform.iOS:
										case TestPlatform.iOS_TodayExtension64:
										case TestPlatform.iOS_Unified:
										case TestPlatform.iOS_Unified32:
										case TestPlatform.iOS_Unified64:
											is_match = true;
											break;
										default:
											if (task.Platform.ToString ().StartsWith ("iOS", StringComparison.Ordinal))
												throw new NotImplementedException ();
											break;
										}
										break;
									case "?all-tvos":
										switch (task.Platform) {
										case TestPlatform.tvOS:
											is_match = true;
											break;
										default:
											if (task.Platform.ToString ().StartsWith ("tvOS", StringComparison.Ordinal))
												throw new NotImplementedException ();
											break;
										}
										break;
									case "?all-watchos":
										switch (task.Platform) {
										case TestPlatform.watchOS:
										case TestPlatform.watchOS_32:
										case TestPlatform.watchOS_64_32:
											is_match = true;
											break;
										default:
											if (task.Platform.ToString ().StartsWith ("watchOS", StringComparison.Ordinal))
												throw new NotImplementedException ();
											break;
										}
										break;
									case "?all-mac":
										switch (task.Platform) {
										case TestPlatform.Mac:
										case TestPlatform.Mac_Modern:
										case TestPlatform.Mac_Full:
										case TestPlatform.Mac_System:
											is_match = true;
											break;
										default:
											if (task.Platform.ToString ().StartsWith ("Mac", StringComparison.Ordinal))
												throw new NotImplementedException ();
											break;
										}
										break;
									default:
										writer.WriteLine ("unknown query: {0}", request.Url.Query);
										break;
									}
									if (request.Url.LocalPath == "/select") {
										if (is_match.HasValue && is_match.Value)
											task.Ignored = false;
									} else if (request.Url.LocalPath == "/deselect") {
										if (is_match.HasValue && is_match.Value)
											task.Ignored = true;
									}
								}

								writer.WriteLine ("OK");
							}
							break;
						case "/stop":
							response.ContentType = System.Net.Mime.MediaTypeNames.Text.Plain;
							using (var writer = new StreamWriter (response.OutputStream)) {
								foreach (var task in find_tasks (writer, request.Url.Query)) {
									if (!task.Waiting) {
										writer.WriteLine ($"Test '{task.TestName}' is not in a waiting state.");
									} else {
										task.Reset ();
									}
								}
								writer.WriteLine ("OK");
							}
							break;
						case "/run":
							response.ContentType = System.Net.Mime.MediaTypeNames.Text.Plain;
							using (var writer = new StreamWriter (response.OutputStream)) {
								// We want to randomize the order the tests are added, so that we don't build first the test for one device, 
								// then for another, since that would not take advantage of running tests on several devices in parallel.
								foreach (var task in find_tasks (writer, request.Url.Query).Shuffle ()) {
									if (task.InProgress || task.Waiting) {
										writer.WriteLine ($"Test '{task.TestName}' is already executing.");
									} else {
										task.Reset ();
										task.BuildOnly = false;
										task.RunAsync ();
									}
								}
								writer.WriteLine ("OK");
							}
							break;
						case "/build":
							response.ContentType = System.Net.Mime.MediaTypeNames.Text.Plain;
							using (var writer = new StreamWriter (response.OutputStream)) {
								foreach (var task in find_tasks (writer, request.Url.Query)) {
									if (task.InProgress || task.Waiting) {
										writer.WriteLine ($"Test '{task.TestName}' is already executing.");
									} else if (task is RunTestTask rtt) {
										rtt.Reset ();
										rtt.BuildAsync ().ContinueWith ((z) => {
											if (rtt.ExecutionResult == TestExecutingResult.Built)
												rtt.ExecutionResult = TestExecutingResult.BuildSucceeded;
										});
									} else {
										writer.WriteLine ($"Test '{task.TestName}' is not a test that can be only built.");
									}
								}

								writer.WriteLine ("OK");
							}
							break;
						case "/reload-devices":
							jenkins.DeviceLoader.LoadDevicesAsync ().DoNotAwait ();
							break;
						case "/reload-simulators":
							jenkins.DeviceLoader.LoadSimulatorsAsync ().DoNotAwait ();
							break;
						case "/quit":
							using (var writer = new StreamWriter (response.OutputStream)) {
								writer.WriteLine ("<!DOCTYPE html>");
								writer.WriteLine ("<html>");
								writer.WriteLine ("<body onload='close ();'>Closing web page...</body>");
								writer.WriteLine ("</html>");
							}
							server.Stop ();
							break;
						case "/favicon.ico":
							serveFile = Path.Combine (HarnessConfiguration.RootDirectory, "xharness", "favicon.ico");
							goto default;
						case "/index.html":
							var redirect_to = request.Url.AbsoluteUri.Replace ("/index.html", "/" + Path.GetFileName (jenkins.LogDirectory) + "/index.html");
							response.Redirect (redirect_to);
							break;
						default:
							var filename = Path.GetFileName (request.Url.LocalPath);
							if (filename == "index.html" && Path.GetFileName (jenkins.LogDirectory) == Path.GetFileName (Path.GetDirectoryName (request.Url.LocalPath))) {
								// We're asked for the report for the current test run, so re-generate it.
								jenkins.GenerateReport ();
							}

							if (serveFile is null)
								serveFile = Path.Combine (Path.GetDirectoryName (jenkins.LogDirectory), request.Url.LocalPath.Substring (1));
							var path = serveFile;
							if (File.Exists (path)) {
								var buffer = new byte [4096];
								using (var fs = new FileStream (path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
									int read;
									response.ContentLength64 = fs.Length;
									switch (Path.GetExtension (path).ToLowerInvariant ()) {
									case ".html":
										response.ContentType = System.Net.Mime.MediaTypeNames.Text.Html;
										break;
									case ".css":
										response.ContentType = "text/css";
										break;
									case ".js":
										response.ContentType = "text/javascript";
										break;
									case ".ico":
										response.ContentType = "image/png";
										break;
									default:
										response.ContentType = System.Net.Mime.MediaTypeNames.Text.Plain + ";charset=UTF-8";
										break;
									}
									while ((read = fs.Read (buffer, 0, buffer.Length)) > 0)
										response.OutputStream.Write (buffer, 0, read);
								}
							} else {
								Console.WriteLine ($"404: {request.Url.LocalPath}");
								response.StatusCode = 404;
								response.OutputStream.WriteByte ((byte) '?');
							}
							break;
						}
					} catch (IOException ioe) {
						Console.WriteLine (ioe.Message);
					} catch (Exception e) {
						Console.WriteLine (e);
					}
					response.Close ();
				}
				tcs.SetResult (true);
			}) {
				IsBackground = true,
			};
			thread.Start ();

			var url = $"http://localhost:{port}/" + Path.GetFileName (jenkins.LogDirectory) + "/index.html";
			Console.WriteLine ($"Launching {url} in the system's default browser.");
			Process.Start ("open", url);

			return tcs.Task;
		}
	}
}
