using System;
using System.Collections;
using NUnit.Framework;
using Xharness.Execution;

namespace Xharness.Tests.Execution.Tests {

	[TestFixture]
	public class MlaunchArgumentsTest {

		class CommandLineDataTestSource {
			public static IEnumerable CommandLineArgs {
				get {
					string listDevFile = "/my/listdev.txt";
					string listSimFile = "/my/listsim.txt";
					string xmlOutputType = "xml";
					yield return new TestCaseData (new [] { (MlaunchArgumentType.ListDev, listDevFile) }).Returns ($"--listdev={listDevFile}");
					yield return new TestCaseData (new [] { (MlaunchArgumentType.ListSim, listSimFile) }).Returns ($"--listsim={listSimFile}");
					yield return new TestCaseData (new [] { (MlaunchArgumentType.OutputFormat, xmlOutputType) }).Returns ($"--output-format={xmlOutputType}");
					yield return new TestCaseData (new [] { (MlaunchArgumentType.ListExtraData, (string) null) }).Returns ("--list-extra-data");
					yield return new TestCaseData (new [] { (MlaunchArgumentType.ListDev, listDevFile), (MlaunchArgumentType.ListSim, listSimFile) }).Returns ($"--listdev={listDevFile} --listsim={listSimFile}");
					yield return new TestCaseData (new [] { (MlaunchArgumentType.ListDev, listDevFile), (MlaunchArgumentType.ListExtraData, (string)null) }).Returns ($"--listdev={listDevFile} --list-extra-data");
					yield return new TestCaseData (new [] { (MlaunchArgumentType.ListDev, listDevFile), (MlaunchArgumentType.OutputFormat, xmlOutputType), (MlaunchArgumentType.ListExtraData, (string)null) }).Returns ($"--listdev={listDevFile} --output-format={xmlOutputType} --list-extra-data");
				}
			}
		}

		[TestCase (MlaunchArgumentType.ListDev)]
		[TestCase (MlaunchArgumentType.ListSim)]
		[TestCase (MlaunchArgumentType.OutputFormat)]
		[TestCase (MlaunchArgumentType.SdkRoot)]
		public void MissingValueThrowsTest (MlaunchArgumentType arg)
		{
			Assert.Throws<ArgumentException> (() => {
				_ = new MlaunchArguments (arg);
			});
		}

		public void ExtraArgumentThrows (MlaunchArgumentType arg)
		{
			Assert.Throws<ArgumentException> (() => {
				_ = new MlaunchArguments ((arg, "value"));
			});
		}

		[TestCaseSource (typeof (CommandLineDataTestSource), "CommandLineArgs")]
		public string AsCommandLineTest((MlaunchArgumentType type, string value) [] args)
		{
			var mlaunchArgs = new MlaunchArguments (args);
			return mlaunchArgs.AsCommandLine ();
		}
	}
}
