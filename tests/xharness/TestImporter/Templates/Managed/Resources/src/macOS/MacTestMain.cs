using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using AppKit;
using Foundation;
using BCLTests;
using Xamarin.iOS.UnitTests;
using Xamarin.iOS.UnitTests.NUnit;
using BCLTests.TestRunner.Core;
using Xamarin.iOS.UnitTests.XUnit;
using System.IO;
using NUnit.Framework.Internal.Filters;
using System.Threading.Tasks;

namespace Xamarin.Mac.Tests {
	static class MainClass {
		static Task<int> Main (string [] args)
		{
			NSApplication.Init ();
			return RunTests (args);
		}

		internal static IEnumerable<TestAssemblyInfo> GetTestAssemblies ()
		{
			// var t = Path.GetFileName (typeof (ActivatorCas).Assembly.Location);
			foreach (var name in RegisterType.TypesToRegister.Keys) {
				var a = RegisterType.TypesToRegister [name].Assembly;
				yield return new TestAssemblyInfo (a, a.Location);
			}
		}

		static async Task<int> RunTests (string [] original_args)
		{
			Console.WriteLine ("Running tests");
			var options = ApplicationOptions.Current;

			// we generate the logs in two different ways depending if the generate xml flag was
			// provided. If it was, we will write the xml file to the tcp writer if present, else
			// we will write the normal console output using the LogWriter
			var logger = new LogWriter (Console.Out);
			logger.MinimumLogLevel = MinimumLogLevel.Info;
			var testAssemblies = GetTestAssemblies ();
			var runner = RegisterType.IsXUnit ? (TestRunner) new XUnitTestRunner (logger) : new NUnitTestRunner (logger);
			var categories = IgnoreFileParser.ParseTraitsContentFile (NSBundle.MainBundle.ResourcePath, RegisterType.IsXUnit);
			runner.SkipCategories (categories);
			var skippedTests = IgnoreFileParser.ParseContentFiles (NSBundle.MainBundle.ResourcePath);
			if (skippedTests.Any ()) {
				// ensure that we skip those tests that have been passed via the ignore files
				runner.SkipTests (skippedTests);
			}
			await runner.Run (testAssemblies).ConfigureAwait (false);

			if (options.LogFile is not null) {
				using (var writer = new StreamWriter (options.LogFile)) {
					runner.WriteResultsToFile (writer, TestRunner.Jargon.NUnitV3);
				}
				logger.Info ($"Xml result can be found {options.LogFile}");
			}

			logger.Info ($"Tests run: {runner.TotalTests} Passed: {runner.PassedTests} Inconclusive: {runner.InconclusiveTests} Failed: {runner.FailedTests} Ignored: {runner.FilteredTests}");
			return runner.FailedTests != 0 ? 1 : 0;
		}
	}
}
