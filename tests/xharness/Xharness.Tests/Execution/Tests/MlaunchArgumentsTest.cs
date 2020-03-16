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

					yield return new TestCaseData (arg: new MlaunchArgument [] { new ListDevicesArgument (listDevFile) })
						.Returns ($"--listdev={listDevFile}");

					yield return new TestCaseData (arg: new MlaunchArgument [] { new ListSimulatorsArgument (listSimFile) })
						.Returns ($"--listsim={listSimFile}");

					yield return new TestCaseData (arg: new MlaunchArgument [] { new XmlOutputFormatArgument () })
						.Returns ($"--output-format={xmlOutputType}");

					yield return new TestCaseData (arg: new MlaunchArgument [] { new ListExtraDataArgument () })
						.Returns ("--list-extra-data");

					yield return new TestCaseData (arg: new MlaunchArgument [] { new ListDevicesArgument (listDevFile), new ListSimulatorsArgument (listSimFile) })
						.Returns ($"--listdev={listDevFile} --listsim={listSimFile}");

					yield return new TestCaseData (arg: new MlaunchArgument [] { new ListDevicesArgument (listDevFile), new ListExtraDataArgument () })
						.Returns ($"--listdev={listDevFile} --list-extra-data");

					yield return new TestCaseData (arg: new MlaunchArgument [] { new ListDevicesArgument (listDevFile), new XmlOutputFormatArgument (), new ListExtraDataArgument () })
						.Returns ($"--listdev={listDevFile} --output-format={xmlOutputType} --list-extra-data");
				}
			}
		}

		[TestCaseSource (typeof (CommandLineDataTestSource), "CommandLineArgs")]
		public string AsCommandLineTest (MlaunchArgument [] args)
		{
			return new MlaunchArguments (args).AsCommandLine ();
		}
	}
}
