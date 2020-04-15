using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution.Mlaunch;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;

namespace Microsoft.DotNet.XHarness.iOS.Shared.Tests.Execution {

	[TestFixture]
	public class MlaunchArgumentsTests {

		public class CommandLineDataTestSource {
			public static IEnumerable CommandLineArgs {
				get {
					string listDevFile = "/my/listdev.txt";
					string listSimFile = "/my/listsim.txt";
					string xmlOutputType = "XML";

					yield return new TestCaseData (arg:
						new MlaunchArgument [] {
							new ListDevicesArgument (listDevFile)
						})
						.Returns ($"--listdev={listDevFile}");

					yield return new TestCaseData (arg:
						new MlaunchArgument [] {
							new ListSimulatorsArgument (listSimFile)
						})
						.Returns ($"--listsim={listSimFile}");

					yield return new TestCaseData (arg:
						new MlaunchArgument [] {
							new XmlOutputFormatArgument ()
						})
						.Returns ($"--output-format={xmlOutputType}");

					yield return new TestCaseData (arg:
						new MlaunchArgument [] {
							new ListExtraDataArgument ()
						})
						.Returns ("--list-extra-data");

					yield return new TestCaseData (arg:
						new MlaunchArgument [] {
							new DownloadCrashReportToArgument ("/path/with spaces.txt"),
							new DeviceNameArgument ("Test iPad")
						})
						.Returns ($"\"--download-crash-report-to=/path/with spaces.txt\" --devname \"Test iPad\"");

					yield return new TestCaseData (arg:
						new MlaunchArgument [] {
							new SetEnvVariableArgument ("SOME_PARAM", "true"),
							new SetEnvVariableArgument ("NUNIT_LOG_FILE", "/another space/path.txt")
						})
						.Returns ($"-setenv=SOME_PARAM=true \"-setenv=NUNIT_LOG_FILE=/another space/path.txt\"");

					yield return new TestCaseData (arg:
						new MlaunchArgument [] {
							new ListDevicesArgument (listDevFile),
							new XmlOutputFormatArgument (),
							new ListExtraDataArgument ()
						})
						.Returns ($"--listdev={listDevFile} --output-format={xmlOutputType} --list-extra-data");
				}
			}
		}

		[TestCaseSource (typeof (CommandLineDataTestSource), "CommandLineArgs")]
		public string AsCommandLineTest (MlaunchArgument [] args)
		{
			return new MlaunchArguments (args).AsCommandLine ();
		}

		[Test]
		public void MlaunchArgumentAndProcessManagerTest ()
		{
			var oldArgs = new List<string> () {
				"--download-crash-report-to=/path/with spaces.txt",
				"--sdkroot",
				"/path to xcode/spaces",
				"--devname",
				"Premek's iPhone",
			};

			var newArgs = new MlaunchArguments () {
				new DownloadCrashReportToArgument ("/path/with spaces.txt"),
				new SdkRootArgument ("/path to xcode/spaces"),
				new DeviceNameArgument ("Premek's iPhone"),
			};

			var oldWayOfPassingArgs = StringUtils.FormatArguments (oldArgs);
			var newWayOfPassingArgs = newArgs.AsCommandLine ();

			Assert.AreEqual (oldWayOfPassingArgs, newWayOfPassingArgs, "Something changed when moving to MlaunchArguments");
		}

		[Test]
		public void MlaunchArgumentEqualityTest ()
		{
			var arg1 = new DownloadCrashReportToArgument ("/path/with spaces.txt");
			var arg2 = new DownloadCrashReportToArgument ("/path/with spaces.txt");
			var arg3 = new DownloadCrashReportToArgument ("/path/with.txt");

			Assert.AreEqual (arg1, arg2, "equality is broken");
			Assert.AreNotEqual (arg1, arg3, "equality is broken");
		}

		[Test]
		public void MlaunchArgumentsEqualityTest ()
		{
			var args1 = new MlaunchArgument [] {
				new ListDevicesArgument ("foo"),
				new ListSimulatorsArgument ("bar")
			};
			var args2 = new MlaunchArgument [] {
				new ListDevicesArgument ("foo"),
				new ListSimulatorsArgument ("bar")
			};
			var args3 = new MlaunchArgument [] {
				new ListDevicesArgument ("foo"),
				new ListSimulatorsArgument ("xyz")
			};

			Assert.AreEqual (args1, args2, "equality is broken");
			Assert.AreNotEqual (args1, args3, "equality is broken");
		}
	}
}
