using System.Collections;
using NUnit.Framework;
using Xharness.Execution.Mlaunch;

namespace Xharness.Tests.Execution.Tests {

	[TestFixture]
	public class MlaunchArgumentsTest {

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
						.Returns ($"--listdev {listDevFile}");

					yield return new TestCaseData (arg:
						new MlaunchArgument [] {
							new ListSimulatorsArgument (listSimFile)
						})
						.Returns ($"--listsim {listSimFile}");

					yield return new TestCaseData (arg:
						new MlaunchArgument [] {
							new XmlOutputFormatArgument ()
						})
						.Returns ($"--output-format {xmlOutputType}");

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
						.Returns ($"--download-crash-report-to \"/path/with spaces.txt\" --devname \"Test iPad\"");

					yield return new TestCaseData (arg:
						new MlaunchArgument [] {
							new ListDevicesArgument (listDevFile),
							new ListSimulatorsArgument (listSimFile)
						})
						.Returns ($"--listdev {listDevFile} --listsim {listSimFile}");

					yield return new TestCaseData (arg:
						new MlaunchArgument [] {
							new ListDevicesArgument (listDevFile),
							new XmlOutputFormatArgument (),
							new ListExtraDataArgument ()
						})
						.Returns ($"--listdev {listDevFile} --output-format {xmlOutputType} --list-extra-data");
				}
			}
		}

		[TestCaseSource (typeof (CommandLineDataTestSource), "CommandLineArgs")]
		public string AsCommandLineTest (MlaunchArgument [] args)
		{
			return new MlaunchArguments (args).AsCommandLine ();
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
