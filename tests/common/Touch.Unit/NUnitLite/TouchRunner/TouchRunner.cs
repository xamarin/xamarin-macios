// TouchRunner.cs: MonoTouch.Dialog-based driver to run unit tests
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2011-2013 Xamarin Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using System;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using CoreFoundation;
using Foundation;
using ObjCRuntime;
#if !__MACOS__
using UIKit;
#endif
using Constants = global::ObjCRuntime.Constants;

#if !__WATCHOS__ && !__MACOS__
using MonoTouch.Dialog;
#endif

using NUnit.Framework.Api;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;
#if NUNITLITE_NUGET
using NUnitLite;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Execution;
#else
using NUnitLite.Runner;
using NUnit.Framework.Internal.WorkItems;
#endif

#if NUNITLITE_NUGET
using SettingsDictionary = System.Collections.Generic.IDictionary<string, object>;
#else
using SettingsDictionary = System.Collections.IDictionary;
#endif

#if NET
[assembly: SupportedOSPlatform ("ios10.0")]
[assembly: SupportedOSPlatform ("tvos10.0")]
[assembly: SupportedOSPlatform ("macos10.14")]
[assembly: SupportedOSPlatform ("maccatalyst13.0")]
#endif

namespace MonoTouch.NUnit.UI {
	public abstract class BaseTouchRunner : ITestListener {
		TestSuite suite = new TestSuite ("TestSuite");
		ITestFilter filter = TestFilter.Empty;
		bool connection_failure;

		public int PassedCount { get; private set; }
		public int FailedCount { get; private set; }
		public int IgnoredCount { get; private set; }
		public int InconclusiveCount { get; private set; }
		public int TestCount {
			get {
				return suite.TestCaseCount;
			}
		}
		public TestSuite Suite { get { return suite; } }

		public bool AutoStart {
			get { return TouchOptions.Current.AutoStart; }
			set { TouchOptions.Current.AutoStart = value; }
		}

		public ITestFilter Filter {
			get { return filter; }
			set { filter = value; }
		}

		public HashSet<string> ExcludedCategories { get; set; }

		public bool TerminateAfterExecution {
			get { return TouchOptions.Current.TerminateAfterExecution && !connection_failure; }
			set { TouchOptions.Current.TerminateAfterExecution = value; }
		}

		List<Assembly> assemblies = new List<Assembly> ();
		List<string> fixtures;

		public void Add (Assembly assembly)
		{
			if (assembly == null)
				throw new ArgumentNullException ("assembly");

			assemblies.Add (assembly);
		}

		public void Add (Assembly assembly, IList<string> fixtures)
		{
			Add (assembly);
			if (fixtures != null) {
				if (this.fixtures == null) {
					this.fixtures = new List<string> (fixtures);
				} else {
					this.fixtures.AddRange (fixtures);
				}
			}
		}

		protected void FlushConsole ()
		{
			Console.Out.Flush ();
			Console.Error.Flush ();
		}

		[DllImport ("libc")]
		static extern void exit (int code);
		protected virtual void TerminateWithSuccess ()
		{
			// For WatchOS we're terminating the extension, not the watchos app itself.
			Console.WriteLine ("Exiting test run with success");
			FlushConsole ();
			exit (0);
		}

		protected virtual void TerminateWithExitCode (int exitCode)
		{
			FlushConsole ();
			if (exitCode == 0) {
				TerminateWithSuccess ();
			} else {
				Console.WriteLine ($"Exiting test run with code {exitCode}");
				exit (exitCode);
			}
		}

		protected virtual void ExecuteOnMainThread (Action action)
		{
			var obj = new NSObject ();
			obj.BeginInvokeOnMainThread (() =>
			{
				action ();
				obj.Dispose ();
			});
		}

		public void LoadSync ()
		{
			foreach (Assembly assembly in assemblies)
				Load (assembly);
			assemblies.Clear ();
		}

		public void SelectLastTestSuite ()
		{
			var lastSuite = NSUserDefaults.StandardUserDefaults.StringForKey ("CurrentTest");

			if (string.IsNullOrEmpty (lastSuite))
				return;

			var hierarchy = new List<ITest> ();
			var test = Find (suite, lastSuite, hierarchy);
			if (hierarchy.Count < 2)
				return;
			// Remove the last one, that's the main test suite
			hierarchy.RemoveAt (hierarchy.Count - 1);
			for (var i = hierarchy.Count - 1; i >= 0; i--) {
				if (hierarchy [i] is TestSuite ts) {
					Show (ts);
				} else {
					break;
				}
			}
		}

		ITest Find (ITest parent, string fullname, List<ITest> hierarchy)
		{
			if (parent.FullName == fullname) {
				hierarchy.Add (parent);
				return parent;
			}

			foreach (var test in parent.Tests) {
				var rv = Find (test, fullname, hierarchy);
				if (rv != null) {
					hierarchy.Add (parent);
					return test;
				}
			}

			return null;
		}

#if !__MACCATALYST__
		[Conditional ("IGNORED")]
#endif
		internal static void TraceLine (string message)
		{
			Console.WriteLine (message);
		}

		public void AutoRun ()
		{
			TraceLine ("AutoRun ()");
			if (!AutoStart) {
				SelectLastTestSuite ();
				return;
			}

			TraceLine ("AutoRun (): queuing test run on main thread");
			ExecuteOnMainThread (() => {
				TraceLine ("AutoRun (): running tests on main thread");
				Run ();
				TraceLine ("AutoRun (): completed test run on main thread");

				var status = FailedCount == 0 ? 0 : 1;
				// optionally end the process, e.g. click "Touch.Unit" -> log tests results, return to springboard...
				// http://stackoverflow.com/questions/1978695/uiapplication-sharedapplication-terminatewithsuccess-is-not-there
				if (TerminateAfterExecution) {
					if (WriterFinishedTask != null) {
						Task.Run (async () => {
							await WriterFinishedTask;
							TerminateWithExitCode (status);
						});
					} else {
						TerminateWithExitCode (status);
					}
				}
			});
		}

		public Task RunAsync ()
		{
			Run ();
			return Task.CompletedTask;
		}

		bool running;
		public void Run ()
		{
			if (running) {
				Console.WriteLine ("Not running because another test run is already in progress.");
				return;
			}

			running = true;
			if (!OpenWriter ("Run Everything")) {
				running = false;
				return;
			}

			try {
				Run (suite);
			} finally {
				CloseWriter ();
				running = false;
			}
		}

#region writer

		public TestResult Result { get; set; }

		public TextWriter Writer { get; set; }

		Task WriterFinishedTask { get; set; }

		static string SelectHostName (string[] names, int port)
		{
			if (names.Length == 0)
				return null;

			if (names.Length == 1)
				return names [0];

			object lock_obj = new object ();
			string result = null;
			int failures = 0;

			using (var evt = new ManualResetEvent (false)) {
				for (int i = names.Length - 1; i >= 0; i--) {
					var name = names [i];
					ThreadPool.QueueUserWorkItem ((v) =>
						{
							try {
								var client = new TcpClient (name, port);
								using (var writer = new StreamWriter (client.GetStream ())) {
									writer.WriteLine ("ping");
								}
								lock (lock_obj) {
									if (result == null)
										result = name;
								}
								evt.Set ();
							} catch (Exception e) {
								lock (lock_obj) {
									Console.WriteLine ("TCP connection failed when selecting 'hostname': {0} and 'port': {1}. {2}", name, port, e);
									failures++;
									if (failures == names.Length)
										evt.Set ();
								}
							}
						});
				}

				// Wait for 1 success or all failures
				evt.WaitOne ();
			}

			return result;
		}

		public bool OpenWriter (string message)
		{
			TouchOptions options = TouchOptions.Current;
			DateTime now = DateTime.Now;
			// let the application provide it's own TextWriter to ease automation with AutoStart property
			if (Writer == null) {
				var writers = new List<TextWriter> ();
				if (options.ShowUseNetworkLogger) {
					try {
						string hostname = null;
						WriterFinishedTask = null;
						TextWriter defaultWriter = null;
						switch (options.Transport) {
						case "FILE":
							Console.WriteLine ("[{0}] Sending '{1}' results to the file {2}", now, message, options.LogFile);
							defaultWriter = new StreamWriter (options.LogFile, true, System.Text.Encoding.UTF8)
							{
								AutoFlush = true,
							};
							break;
						case "HTTP":
							var hostnames = options.HostName.Split (',');
							hostname = hostnames [0];
							if (hostnames.Length > 1)
								Console.WriteLine ("[{0}] Found multiple host names ({1}); will only try sending to the first ({2})", now, options.HostName, hostname);
							Console.WriteLine ("[{0}] Sending '{1}' results to {2}:{3}", now, message, hostname, options.HostPort);
							var w = new HttpTextWriter ()
							{
								HostName = hostname,
								Port = options.HostPort,
							};
							w.Open ();
							defaultWriter = w;
							WriterFinishedTask = w.FinishedTask;
							break;
						default:
							Console.WriteLine ("Unknown transport '{0}': switching to default (TCP)", options.Transport);
							goto case "TCP";
						case "TCP":
							if (!options.UseTcpTunnel)
								hostname = SelectHostName (options.HostName.Split (','), options.HostPort);
							else
								hostname = "localhost";
							if (string.IsNullOrEmpty (hostname)) {
								Console.WriteLine ("Couldn't establish a TCP connection with any of the hostnames: {0}", options.HostName);
								break;
							}
							if (!options.UseTcpTunnel)
								Console.WriteLine ("[{0}] Sending '{1}' results to {2}:{3}", now, message, hostname, options.HostPort);
							else
								Console.WriteLine ("[{0}] Sending '{1}' results to {2} over a tcp tunnel", now, message, options.HostPort);
							defaultWriter = new TcpTextWriter (hostname, options.HostPort, options.UseTcpTunnel);
							break;
						}
						if (options.EnableXml) {
							OutputWriter formatter;
							switch (options.XmlVersion) {
							case XmlVersion.NUnitV3:
#if NUNITLITE_NUGET
								formatter = new NUnit3XmlOutputWriter ();
#else
								formatter = new NUnit3XmlOutputWriter (DateTime.UtcNow);
#endif
								break;
							default:
#if NUNITLITE_NUGET
								formatter = new NUnit2XmlOutputWriter ();
#else
								formatter = new NUnit2XmlOutputWriter (DateTime.UtcNow);
#endif
								break;
							}
							writers.Add (new NUnitOutputTextWriter (
								this, defaultWriter, formatter, options.XmlMode));
						} else if (defaultWriter != null) {
							writers.Add (defaultWriter);
						}
					} catch (Exception ex) {
						connection_failure = true;
						if (!ShowConnectionErrorAlert (options.HostName, options.HostPort, ex))
							return false;

						Console.WriteLine ("Network error: Cannot connect to {0}:{1}: {2}. Continuing on console.", options.HostName, options.HostPort, ex);
					}
				}
				writers.Add (Console.Out);
				Writer = new MultiplexedTextWriter (writers);
			}

			if (Writer == null)
				Writer = Console.Out;

			Writer.WriteLine ("[Runner executing:\t{0}]", message);
			Writer.WriteLine ("[MonoTouch Version:\t{0}]", Constants.Version);
			Writer.WriteLine ("[Assembly:\t{0}.dll ({1} bits)]", typeof (NSObject).Assembly.GetName ().Name, IntPtr.Size * 8);
			Writer.WriteLine ("[GC:\t{0}]", GC.MaxGeneration == 0 ? "Boehm": "sgen");
			WriteDeviceInformation (Writer);
			Writer.WriteLine ("[Device Locale:\t{0}]", NSLocale.CurrentLocale.Identifier);
			Writer.WriteLine ("[Device Date/Time:\t{0}]", now); // to match earlier C.WL output

			Writer.WriteLine ("[Bundle:\t{0}]", NSBundle.MainBundle.BundleIdentifier);
			// FIXME: add data about how the app was compiled (e.g. ARMvX, LLVM, GC and Linker options)
			PassedCount = 0;
			IgnoredCount = 0;
			FailedCount = 0;
			InconclusiveCount = 0;
			return true;
		}

		// returns true if test run should still start
		bool ShowConnectionErrorAlert (string hostname, int port, Exception ex)
		{
#if __TVOS__ || __WATCHOS__ || __MACOS__
			return true;
#else
			// Don't show any alerts if we're running automated.
			if (AutoStart)
				return true;

			// UIAlert is not available for extensions.
			if (NSBundle.MainBundle.BundlePath.EndsWith (".appex", StringComparison.Ordinal))
				return true;
			
			Console.WriteLine ("Network error: Cannot connect to {0}:{1}: {2}.", hostname, port, ex);
			UIAlertView alert = new UIAlertView ("Network Error",
				String.Format ("Cannot connect to {0}:{1}: {2}. Continue on console ?", hostname, port, ex.Message),
				(IUIAlertViewDelegate) null, "Cancel", "Continue");
			int button = -1;
			alert.Clicked += delegate (object sender, UIButtonEventArgs e)
			{
				button = (int) e.ButtonIndex;
			};
			alert.Show ();
			while (button == -1)
				NSRunLoop.Current.RunUntil (NSDate.FromTimeIntervalSinceNow (0.5));
			Console.WriteLine (button);
			Console.WriteLine ("[Host unreachable: {0}]", button == 0 ? "Execution cancelled" : "Switching to console output");
			return button != 0;
#endif
		}

		protected abstract void WriteDeviceInformation (TextWriter writer);

		public virtual void Show (TestSuite suite)
		{
		}

		public void CloseWriter ()
		{
			int total = PassedCount + InconclusiveCount + FailedCount; // ignored are *not* run
			Writer.WriteLine ("Tests run: {0} Passed: {1} Inconclusive: {2} Failed: {3} Ignored: {4}", total, PassedCount, InconclusiveCount, FailedCount, IgnoredCount);

			// In some cases, the close is not correctly implemented and we might get a InvalidOperationException, we try to close and then null the obj for it to be
			// GC.
			try {
				Writer.Close ();
			} finally {
				Writer = null;
			}
		}

#endregion

		public void TestStarted (ITest test)
		{
			if (test is TestSuite) {
				Writer.WriteLine ();
#if NUNITLITE_NUGET
				Writer.WriteLine (test.FullName);
#else
				Writer.WriteLine (test.Name);
#endif
			}
		}

		public virtual void TestFinished (ITestResult r)
		{
			TestResult result = r as TestResult;

			if (result.Test is TestSuite) {
				if (!result.IsFailure () && !result.IsSuccess () && !result.IsInconclusive () && !result.IsIgnored ())
					Writer.WriteLine ("\t[INFO] {0}", result.Message);
				var skip = result.Test.Properties ["_SKIPREASON"];
				if (skip.Count > 0)
					Writer.WriteLine ("\t[SKIPREASON] {0}", skip [0]);

#if NUNITLITE_NUGET
				string name = result.Test.FullName;
#else
				string name = result.Test.Name;
#endif
				if (!String.IsNullOrEmpty (name))
					Writer.WriteLine ("{0} : {1} ms", name, result.GetDuration ().TotalMilliseconds);
			} else {
				if (result.IsSuccess ()) {
					Writer.Write ("\t[PASS] ");
					PassedCount++;
				} else if (result.IsIgnored ()) {
					Writer.Write ("\t[IGNORED] ");
					IgnoredCount++;
				} else if (result.IsFailure ()) {
					Writer.Write ("\t[FAIL] ");
					FailedCount++;
				} else if (result.IsInconclusive ()) {
					Writer.Write ("\t[INCONCLUSIVE] ");
					InconclusiveCount++;
				} else {
					Writer.Write ("\t[INFO] ");
				}
#if NUNITLITE_NUGET
				Writer.Write (result.Test.Name);
#else
				Writer.Write (result.Test.FixtureType.Name);
				Writer.Write (".");
				Writer.Write (result.Test.Name);
#endif

				string message = result.Message;
				if (!String.IsNullOrEmpty (message)) {
					Writer.Write (" : {0}", message.Replace ("\r\n", "\\r\\n"));
				}
				Writer.WriteLine ();
#if NUNITLITE_NUGET
				if (!string.IsNullOrEmpty (result.Output))
					Writer.WriteLine (result.Output);
#endif

				string stacktrace = result.StackTrace;
				if (!String.IsNullOrEmpty (result.StackTrace)) {
					string[] lines = stacktrace.Split (new char [] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
					foreach (string line in lines)
						Writer.WriteLine ("\t\t{0}", line);
				}
			}
		}

		Dictionary<string, object> default_settings = new Dictionary<string, object> () {
#if NUNITLITE_NUGET
			{ "RunOnMainThread", true },
#endif
		};

		SettingsDictionary CreateSettings (SettingsDictionary settings)
		{
			if (fixtures == null && (settings == null || settings.Count == 0))
				return default_settings;

			var dict = new Dictionary<string, object> (default_settings);

			if (settings != null) {
				foreach (var key in settings.Keys)
					dict [key?.ToString ()] = settings [key];
			}

			if (fixtures != null)
				dict ["LOAD"] = fixtures;
			
			return dict;
		}

#if NUNITLITE_NUGET
		// we need one runner per test assembly
		DefaultTestAssemblyBuilder builder = new DefaultTestAssemblyBuilder ();
		List<NUnitTestAssemblyRunner> runners = new List<NUnitTestAssemblyRunner> ();

		public bool Load (string assemblyName, IDictionary<string, object> settings = null)
		{
			var runner = new NUnitTestAssemblyRunner (builder);
			runners.Add (runner);
			return AddSuite ((TestSuite) runner.Load (assemblyName, CreateSettings (settings)));
		}

		public bool Load (Assembly assembly, IDictionary<string, object> settings = null)
		{
			return Load (assembly.GetName ().Name, settings);
		}
#else
		NUnitLiteTestAssemblyBuilder builder = new NUnitLiteTestAssemblyBuilder ();

		public bool Load (string assemblyName, SettingsDictionary settings = null)
		{
			return AddSuite (builder.Build (assemblyName, CreateSettings (settings)));
		}

		public bool Load (Assembly assembly, SettingsDictionary settings = null)
		{
			return AddSuite (builder.Build (assembly, CreateSettings (settings)));
		}
#endif

		bool AddSuite (TestSuite ts)
		{
			if (ts == null)
				return false;
			suite.Add (ts);
			return true;
		}

		public void Run (Test test)
		{
			PassedCount = 0;
			IgnoredCount = 0;
			FailedCount = 0;
			InconclusiveCount = 0;

			Result = null;

#if NUNITLITE_NUGET
			var filter = new MatchTestFilter { MatchTest = test };
			if (this.filter != null)
				filter.AndFilters.Add ((TestFilter) this.filter);
			if (ExcludedCategories != null)
				filter.AndFilters.Add (new ExcludeCategoryFilter (ExcludedCategories));
			if (!string.IsNullOrEmpty (TouchOptions.Current.TestName))
				filter.AndFilters.Add (TestFilter.FromXml ($"<filter><test>{TouchOptions.Current.TestName.Replace ("&", "&amp").Replace ("<", "&lt;")}</test></filter>"));
			foreach (var runner in runners)
				runner.Run (this, filter);

			// The TestResult we get back from the runner is for the top-most test suite,
			// which isn't necessarily the test that we ran. So look for the TestResult
			// for the test we ran.
			ITestResult find_result (ITestResult tr)
			{
				if (tr.Test == test)
					return tr;
				foreach (var child in tr.Children) {
					var r = find_result (child);
					if (r != null)
						return r;
				}
				return null;
			}

			var tsr = new TestSuiteResult (suite);
			foreach (var runner in runners) {
				var rv = (TestResult) (find_result (runner.Result) ?? runner.Result);
				if (rv != null)
					tsr.AddResult (rv);
			}
			Result = tsr;
#else
			TestExecutionContext current = TestExecutionContext.CurrentContext;
			current.WorkDirectory = Environment.CurrentDirectory;
			current.Listener = this;
			WorkItem wi = test.CreateWorkItem (filter, new FinallyDelegate ());
			wi.Execute (current);
			Result = wi.Result;
#endif
		}

		public ITest LoadedTest {
			get {
				return suite;
			}
		}

		public void NotifySelectedTest (ITest test)
		{
			NSUserDefaults.StandardUserDefaults.SetString (test.FullName, "CurrentTest");
		}

		public void TestOutput (TestOutput testOutput)
		{
		}

#if NUNITLITE_NUGET
		public void SendMessage (TestMessage message)
		{
			Writer.WriteLine (message.ToString ());
		}
#endif
	}

#if __WATCHOS__
	public class WatchOSRunner : BaseTouchRunner {
		protected override void WriteDeviceInformation (TextWriter writer)
		{
			var device = WatchKit.WKInterfaceDevice.CurrentDevice;
			writer.WriteLine ("[{0}:\t{1} v{2}]", device.Model, device.SystemName, device.SystemVersion);
			writer.WriteLine ("[Device Name:\t{0}]", device.Name);
		}
	}
#endif
	
#if !__WATCHOS__ && !__MACOS__
	public class ConsoleRunner : BaseTouchRunner {
		protected override void WriteDeviceInformation (TextWriter writer)
		{
			UIDevice device = UIDevice.CurrentDevice;
			writer.WriteLine ("[{0}:\t{1} v{2}]", device.Model, device.SystemName, device.SystemVersion);
			writer.WriteLine ("[Device Name:\t{0}]", device.Name);
		}
	}

#if !NET
	[CLSCompliant (false)]
#endif
	public class TouchRunner : BaseTouchRunner {
		
		UIWindow window;

		public TouchRunner (UIWindow window)
		{
			if (window == null)
				throw new ArgumentNullException ("window");
			
			this.window = window;
		}
				
		public UINavigationController NavigationController {
			get { return (UINavigationController) window.RootViewController; }
		}

		protected override void TerminateWithSuccess ()
		{
			Console.WriteLine ($"Exiting test run with success");
			FlushConsole ();
			Selector selector = new Selector ("terminateWithSuccess");
			UIApplication.SharedApplication.PerformSelector (selector, UIApplication.SharedApplication, 0);						
		}

		public UIViewController GetViewController ()
		{
			var menu = new RootElement ("Test Runner");
			
			Section main = new Section ("Loading test suites...");
			menu.Add (main);
			
			Section options = new Section () {
				new StyledStringElement ("Options", Options) { Accessory = UITableViewCellAccessory.DisclosureIndicator },
				new StyledStringElement ("Credits", Credits) { Accessory = UITableViewCellAccessory.DisclosureIndicator }
			};
			menu.Add (options);

			// large unit tests applications can take more time to initialize
			// than what the iOS watchdog will allow them on devices, so loading
			// must be done async.

			// ensure that the dialog's view has been loaded so we can call Reload later
			var dialog = new DialogViewController (menu) { Autorotate = true };
			var dialogView = dialog.View;

			TraceLine ("Queuing test loading on background thread");
			ThreadPool.QueueUserWorkItem ((v) => {
				TraceLine ("Loading tests on background thread");
				LoadSync ();
				TraceLine ("Loaded tests on background thread, queuing UI update on main thead");

				ExecuteOnMainThread (() =>
				{
					TraceLine ("Updating UI on main thead");
					foreach (TestSuite ts in Suite.Tests) {
						main.Add (Setup (ts));
					}

					main.Caption = null;
					menu.Reload (main, UITableViewRowAnimation.Fade);

					options.Insert (0, new StringElement ("Run Everything", Run));
					menu.Reload (options, UITableViewRowAnimation.Fade);

					AutoRun ();
				});
			});

			return dialog;
		}

		void Options ()
		{
			NavigationController.PushViewController (TouchOptions.Current.GetViewController (), true);				
		}
		
		void Credits ()
		{
			var title = new MultilineElement ("Touch.Unit Runner\nCopyright 2011-2012 Xamarin Inc.\nAll rights reserved.");
			title.Alignment = UITextAlignment.Center;
			
			var root = new RootElement ("Credits") {
				new Section () { title },
				new Section () {
#if TVOS
					new StringElement ("About Xamarin: https://www.xamarin.com"),
					new StringElement ("About MonoTouch: https://ios.xamarin.com"),
					new StringElement ("About MonoTouch.Dialog: https://github.com/migueldeicaza/MonoTouch.Dialog"),
					new StringElement ("About NUnitLite: http://www.nunitlite.org"),
					new StringElement ("About Font Awesome: https://fortawesome.github.com/Font-Awesome")
#else
					new HtmlElement ("About Xamarin", "https://www.xamarin.com"),
					new HtmlElement ("About MonoTouch", "https://ios.xamarin.com"),
					new HtmlElement ("About MonoTouch.Dialog", "https://github.com/migueldeicaza/MonoTouch.Dialog"),
					new HtmlElement ("About NUnitLite", "http://www.nunitlite.org"),
					new HtmlElement ("About Font Awesome", "https://fortawesome.github.com/Font-Awesome")
#endif
				}
			};
				
			var dv = new DialogViewController (root, true) { Autorotate = true };
			NavigationController.PushViewController (dv, true);				
		}

		Dictionary<TestSuite, TouchViewController> suites_dvc = new Dictionary<TestSuite, TouchViewController> ();
		Dictionary<TestSuite, TestSuiteElement> suite_elements = new Dictionary<TestSuite, TestSuiteElement> ();
		Dictionary<TestMethod, TestCaseElement> case_elements = new Dictionary<TestMethod, TestCaseElement> ();
		
		public override void Show (TestSuite suite)
		{
			NavigationController.PushViewController (suites_dvc [suite], true);
		}
	
		TestSuiteElement Setup (TestSuite suite)
		{
			TestSuiteElement tse = new TestSuiteElement (suite, this);
			suite_elements.Add (suite, tse);
			
			var root = new RootElement ("Tests");
		
			Section section = new Section (suite.Name);
			foreach (ITest test in suite.Tests) {
				TestSuite ts = (test as TestSuite);
				if (ts != null) {
					section.Add (Setup (ts));
				} else {
					TestMethod tc = (test as TestMethod);
					if (tc != null) {
						section.Add (Setup (tc));
					} else {
						throw new NotImplementedException (test.GetType ().ToString ());
					}
				}
			}
		
			root.Add (section);
			
			Section options = new Section () {
				new StringElement ("Run all", delegate () {
					if (OpenWriter (suite.Name)) {
						Run (suite);
						CloseWriter ();
						suites_dvc [suite].Filter ();
					}
				})
			};
			root.Add (options);

			var tvc = new TouchViewController (root);
			tvc.ViewAppearing += (object sender, EventArgs ea) => {
				NotifySelectedTest (suite);
			};
			suites_dvc.Add (suite, tvc);
			return tse;
		}
		
		TestCaseElement Setup (TestMethod test)
		{
			TestCaseElement tce = new TestCaseElement (test, this);
			case_elements.Add (test, tce);
			return tce;
		}

		public override void TestFinished (ITestResult r)
		{
			base.TestFinished (r);

			ExecuteOnMainThread (() => {
				TestResult result = r as TestResult;
				TestSuite ts = result.Test as TestSuite;
				if (ts != null) {
					TestSuiteElement tse;
					if (suite_elements.TryGetValue (ts, out tse))
						tse.TestFinished (result);
				} else {
					TestMethod tc = result.Test as TestMethod;
					if (tc != null)
						case_elements [tc].TestFinished (result);
				}
			});
		}

		protected override void WriteDeviceInformation (TextWriter writer)
		{
			UIDevice device = UIDevice.CurrentDevice;
			writer.WriteLine ("[{0}:\t{1} v{2}]", device.Model, device.SystemName, device.SystemVersion);
			writer.WriteLine ("[Device Name:\t{0}]", device.Name);
			writer.WriteLine ("[Device UDID:\t{0}]", UniqueIdentifier);
		}

		[System.Runtime.InteropServices.DllImport ("/usr/lib/libobjc.dylib")]
		static extern IntPtr objc_msgSend (IntPtr receiver, IntPtr selector);

		// Apple blacklisted `uniqueIdentifier` (for the appstore) but it's still 
		// something useful to have inside the test logs
		static string UniqueIdentifier {
			get {
				IntPtr handle = UIDevice.CurrentDevice.Handle;
				if (UIDevice.CurrentDevice.RespondsToSelector (new Selector ("uniqueIdentifier")))
					return CFString.FromHandle (objc_msgSend (handle, Selector.GetHandle("uniqueIdentifier")));
				return "unknown";
			}
		}

		protected override void ExecuteOnMainThread (Action action)
		{
			window.BeginInvokeOnMainThread (() => action ());
		}
	}
#endif

	// A filter that matches a specific test
	class MatchTestFilter : TestFilter {
		public ITest MatchTest;
		public List<TestFilter> AndFilters = new List<TestFilter> ();

#if NUNITLITE_NUGET
		public override TNode AddToXml (TNode parentNode, bool recursive)
		{
			throw new NotImplementedException ();
		}
#endif

		public override bool Match (ITest test)
		{
			if (AndFilters != null) {
				// If any of the And filters returns false, then return false too.
				if (AndFilters.Any ((v) => !v.Match (test)))
					return false;
			}

			return IsMatch (test, MatchTest);
		}

		static bool IsMatch (ITest test, ITest match)
		{
			if (test == match)
				return true;

			if (match.HasChildren) {
				foreach (var child in match.Tests) {
					if (IsMatch (test, child))
						return true;
				}
			}

			return false;

		}
	}

	class MultiplexedTextWriter : TextWriter {
		List<TextWriter> writers = new List<TextWriter> ();

		public MultiplexedTextWriter (IEnumerable<TextWriter> writers)
		{
			this.writers.AddRange (writers);
		}

		public void Add (TextWriter writer)
		{
			writers.Add (writer);
		}

		public override Encoding Encoding {
			get {
				return Encoding.UTF8;
			}
		}

		public override void Close ()
		{
			foreach (var writer in writers)
				writer.Close ();
		}

		public override void Write (char value)
		{
			foreach (var writer in writers)
				writer.Write (value);
		}

		public override void Write (char [] buffer)
		{
			foreach (var writer in writers)
				writer.Write (buffer);
		}

		public override void WriteLine (string value)
		{
			foreach (var writer in writers)
				writer.WriteLine (value);
		}
	}
}
