extern alias mmp;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using Xamarin.Bundler;
using Xamarin.Utils;

using Mono.Tuner;
using MonoMac.Tuner;

namespace Xamarin.MMP.Tests.Unit {
	[TestFixture]
	public class AotTests {
		const string TestRootDir = "/a/non/sense/dir/";

		class TestFileEnumerator : IFileEnumerator {
			public IEnumerable<string> Files { get; }
			public string RootDir { get; } = TestRootDir;

			public TestFileEnumerator (IEnumerable<string> files)
			{
				Files = files;
			}
		}

		List<Tuple<string, IList<string>>> commandsRun;

		[SetUp]
		public void Init ()
		{
			// Make sure this is cleared between every test
			commandsRun = new List<Tuple<string, IList<string>>> ();
		}

		void Compile (AOTOptions options, TestFileEnumerator files, AOTCompilerType compilerType = AOTCompilerType.Bundled64, RunCommandDelegate onRunDelegate = null, bool isRelease = false, bool isModern = false)
		{
			AOTCompiler compiler = new AOTCompiler (options, GetValidAbis (compilerType), compilerType, isModern, isRelease) {
				RunCommand = onRunDelegate is not null ? onRunDelegate : OnRunCommand,
				ParallelOptions = new ParallelOptions () { MaxDegreeOfParallelism = 1 },
				XamarinMacPrefix = Xamarin.Tests.Configuration.SdkRootXM,
			};
			try {
				Driver.SdkRoot = Xamarin.Tests.Configuration.XcodeLocation;
				Driver.ValidateXcode (null, false, false);
				Profile.Current = new XamarinMacProfile ();
				compiler.Compile (files);
			} finally {
				Profile.Current = null;
			}
		}

		void ClearCommandsRun ()
		{
			commandsRun.Clear ();
		}

		int OnRunCommand (string path, IList<string> args, Dictionary<string, string> env, StringBuilder output, bool suppressPrintOnErrors)
		{
			commandsRun.Add (Tuple.Create<string, IList<string>> (path, args));
			if (path != AOTCompiler.StripCommand) {
				Assert.AreEqual (TestRootDir, env ["MONO_PATH"], "MONO_PATH should be set to our expected value");
			}
			return 0;
		}

		string GetExpectedMonoCommand (AOTCompilerType compilerType, Abi currAbi)
		{
			if (compilerType == AOTCompilerType.Bundled64) {
				if (currAbi == Abi.x86_64)
					return "mono-sgen";
				else if (currAbi == Abi.ARM64)
					return "aarch64-darwin-mono-sgen";
			} else if (compilerType == AOTCompilerType.System64 && currAbi == Abi.x86_64) {
				return "mono64";
			}
			Assert.Fail ("GetMonoPath with invalid option");
			return "";
		}

		List<string> GetFiledAOTed (AOTCompilerType compilerType = AOTCompilerType.Bundled64, AOTKind kind = AOTKind.Standard, bool isModern = false, bool expectStripping = false, bool expectSymbolDeletion = false)
		{
			List<string> filesAOTed = new List<string> ();

			foreach (var command in commandsRun) {
				if (expectStripping && command.Item1 == AOTCompiler.StripCommand)
					continue;

				var argParts = command.Item2;
				const string aotArg = "--aot=";
				const string tripleArg = "mtriple=";
				Assert.IsTrue (argParts [0].StartsWith (aotArg), $"First arg should be {aotArg}");
				var aotParts = argParts [0].Substring (aotArg.Length).Split (new char [] { ',' });
				Assert.IsTrue (aotParts [0].StartsWith (tripleArg), $"Aot first argument should be {tripleArg}triple");
				var triple = aotParts [0].Substring (tripleArg.Length);
				Assert.IsTrue (Enum.TryParse (triple, true, out Abi currAbi), "Triple does not represent a valid abi");

				Assert.IsTrue (command.Item1.EndsWith (GetExpectedMonoCommand (compilerType, currAbi)), "Unexpected command: " + command.Item1);
				if (kind == AOTKind.Hybrid)
					Assert.AreEqual (aotParts [1], "hybrid", "Aot arg should contain hybrid");
				else if (aotParts.Length > 1)
					Assert.AreNotEqual (aotParts [1], "hybrid", "Aot arg should not contain hybrid");

				if (isModern)
					Assert.AreEqual (argParts [1], "--runtime=mobile", "Second arg should be --runtime=mobile");
				else
					Assert.AreNotEqual (argParts [1], "--runtime=mobile", "Second arg should not be --runtime=mobile");


				var fileName = command.Item2 [1];
				if (isModern)
					fileName = command.Item2 [2];

				filesAOTed.Add (fileName);
			}
			return filesAOTed;
		}

		IEnumerable<string> GetFilesStripped ()
		{
			return commandsRun.Where (x => x.Item1 == AOTCompiler.StripCommand).SelectMany (x => x.Item2);
		}

		void AssertFilesStripped (IEnumerable<string> expectedFiles)
		{
			var filesStripped = GetFilesStripped ();

			Func<string> getErrorDetails = () => $"\n {FormatDebugList (filesStripped)} \nvs\n {FormatDebugList (expectedFiles)}\n{AllCommandsRun}";

			Assert.AreEqual (filesStripped.Count (), expectedFiles.Count (), "Different number of files stripped than expected: " + getErrorDetails ());
			Assert.IsTrue (filesStripped.All (x => expectedFiles.Contains (x)), "Different files stripped than expected: " + getErrorDetails ());
		}

		string AllCommandsRun => "\nCommands Run:\n\t" + String.Join ("\n\t", commandsRun.Select (x => $"{x.Item1} {x.Item2}"));
		string FormatDebugList (IEnumerable<string> list) => String.Join (" ", list.Select (x => "\"" + x + "\""));

		void AssertFilesAOTed (IEnumerable<string> expectedFiles, AOTCompilerType compilerType = AOTCompilerType.Bundled64, AOTKind kind = AOTKind.Standard, bool isModern = false, bool expectStripping = false, bool expectSymbolDeletion = false)
		{
			List<string> filesAOTed = GetFiledAOTed (compilerType, kind, isModern: isModern, expectStripping: expectStripping, expectSymbolDeletion: expectSymbolDeletion);

			Func<string> getErrorDetails = () => $"\n {FormatDebugList (filesAOTed)} \nvs\n {FormatDebugList (expectedFiles)}\n{AllCommandsRun}";

			Assert.AreEqual (filesAOTed.Count, expectedFiles.Count () * GetValidAbis (compilerType).Count (), "Different number of files AOT than expected: " + getErrorDetails ());
			Assert.IsTrue (filesAOTed.All (x => expectedFiles.Contains (x)), "Different files AOT than expected: " + getErrorDetails ());
		}

		void AssertThrowErrorWithCode (Action action, int code)
		{
			try {
				action ();
			} catch (ProductException e) {
				Assert.AreEqual (e.Code, code, $"Got code {e.Code} but expected {code}");
				return;
			} catch (AggregateException e) {
				Assert.AreEqual (e.InnerExceptions.Count, 1, "Got AggregateException but more than one exception");
				ProductException innerException = e.InnerExceptions [0] as ProductException;
				Assert.IsNotNull (innerException, "Got AggregateException but inner not ProductException");
				Assert.AreEqual (innerException.Code, code, $"Got code {innerException.Code} but expected {code}");
				return;
			}
			Assert.Fail ($"We should have thrown ProductException with code: {code}");
		}

		readonly string [] FullAppFileList = {
			"Foo Bar.exe", "libMonoPosixHelper.dylib", "mscorlib.dll", "Xamarin.Mac.dll", "System.dll", "System.Core.dll"
		};

		readonly string [] CoreXMFileList = { "mscorlib.dll", "Xamarin.Mac.dll", "System.dll" };
		readonly string [] SDKFileList = { "mscorlib.dll", "Xamarin.Mac.dll", "System.dll", "System.Core.dll" };

		IEnumerable<mmp::Xamarin.Abi> GetValidAbis (AOTCompilerType compilerType)
		{
			if (compilerType == AOTCompilerType.Bundled64) {
				yield return mmp::Xamarin.Abi.x86_64;
				yield return mmp::Xamarin.Abi.ARM64;
			} else if (compilerType == AOTCompilerType.System64) {
				yield return mmp::Xamarin.Abi.x86_64;
			} else {
				Assert.Fail ("Improper compiler type");
			}
		}

		[Test]
		public void ParsingNone_DoesNoAOT ()
		{
			var options = new AOTOptions ("none");
			Assert.IsFalse (options.IsAOT, "Parsing none should not be IsAOT");
			AssertThrowErrorWithCode (() => Compile (options, new TestFileEnumerator (FullAppFileList)), 99);
		}

		[Test]
		public void All_AOTAllFiles ()
		{
			var options = new AOTOptions ("all");
			Assert.IsTrue (options.IsAOT, "Should be IsAOT");

			Compile (options, new TestFileEnumerator (FullAppFileList));

			var expectedFiles = FullAppFileList.Where (x => x.EndsWith (".exe") || x.EndsWith (".dll"));
			AssertFilesAOTed (expectedFiles);
		}

		[Test]
		public void Core_ParsingJustCoreFiles ()
		{
			var options = new AOTOptions ("core");
			Assert.IsTrue (options.IsAOT, "Should be IsAOT");

			Compile (options, new TestFileEnumerator (FullAppFileList));

			AssertFilesAOTed (CoreXMFileList);
		}

		[Test]
		public void SDK_ParsingJustSDKFiles ()
		{
			var options = new AOTOptions ("sdk");
			Assert.IsTrue (options.IsAOT, "Should be IsAOT");

			Compile (options, new TestFileEnumerator (FullAppFileList));

			AssertFilesAOTed (SDKFileList);
		}

		[Test]
		public void ExplicitAssembly_JustAOTExplicitFile ()
		{
			var options = new AOTOptions ("+System.dll");
			Assert.IsTrue (options.IsAOT, "Should be IsAOT");

			Compile (options, new TestFileEnumerator (FullAppFileList));

			AssertFilesAOTed (new string [] { "System.dll" });
		}

		[Test]
		public void CoreWithInclusionAndSubtraction ()
		{
			var options = new AOTOptions ("core,+Foo.dll,-Xamarin.Mac.dll");
			Assert.IsTrue (options.IsAOT, "Should be IsAOT");

			string [] testFiles = {
				"Foo.dll", "Foo Bar.exe", "libMonoPosixHelper.dylib", "mscorlib.dll", "Xamarin.Mac.dll", "System.dll"
			};
			Compile (options, new TestFileEnumerator (testFiles));

			AssertFilesAOTed (new string [] { "Foo.dll", "mscorlib.dll", "System.dll" });
		}

		[Test]
		public void CoreWithFullPath_GivesFullPathCommands ()
		{
			var options = new AOTOptions ("core,-Xamarin.Mac.dll");
			Assert.IsTrue (options.IsAOT, "Should be IsAOT");

			Compile (options, new TestFileEnumerator (FullAppFileList.Select (x => TestRootDir + x)));

			AssertFilesAOTed (new string [] { TestRootDir + "mscorlib.dll", TestRootDir + "System.dll" });
		}

		[Test]
		public void ExplicitNegativeFileWithNonExistantFiles_ThrowError ()
		{
			var options = new AOTOptions ("core,-NonExistant.dll");
			Assert.IsTrue (options.IsAOT, "Should be IsAOT");

			AssertThrowErrorWithCode (() => Compile (options, new TestFileEnumerator (FullAppFileList)), 3010);
		}

		[Test]
		public void ExplicitPositiveFileWithNonExistantFiles_ThrowError ()
		{
			var options = new AOTOptions ("core,+NonExistant.dll");
			Assert.IsTrue (options.IsAOT, "Should be IsAOT");

			AssertThrowErrorWithCode (() => Compile (options, new TestFileEnumerator (FullAppFileList)), 3009);
		}

		[Test]
		public void ExplicitNegativeWithNoAssemblies_ShouldNoOp ()
		{
			var options = new AOTOptions ("-System.dll");
			Assert.IsTrue (options.IsAOT, "Should be IsAOT");

			Compile (options, new TestFileEnumerator (FullAppFileList));
			AssertFilesAOTed (new string [] { });
		}

		[Test]
		public void ParsingSimpleOptions_InvalidOption ()
		{
			AssertThrowErrorWithCode (() => new AOTOptions ("FooBar"), 20);
		}

		[Test]
		public void WhenAOTFails_ShouldReturnError ()
		{
			RunCommandDelegate runThatErrors = (path, args, env, output, suppressPrintOnErrors) => 42;
			var options = new AOTOptions ("all");

			AssertThrowErrorWithCode (() => Compile (options, new TestFileEnumerator (FullAppFileList), onRunDelegate: runThatErrors), 3001);
		}

		[Test]
		public void DifferentMonoTypes_ShouldInvokeCorrectMono ()
		{
			foreach (var compilerType in new List<AOTCompilerType> () { AOTCompilerType.Bundled64, AOTCompilerType.System64 }) {
				ClearCommandsRun ();
				var options = new AOTOptions ("sdk");
				Assert.IsTrue (options.IsAOT, "Should be IsAOT");

				Compile (options, new TestFileEnumerator (FullAppFileList), compilerType);

				AssertFilesAOTed (SDKFileList, compilerType);
			}
		}

		[Test]
		public void PipeFileName_ShouldNotHybridCompiler ()
		{
			foreach (var testCase in new string [] { "+|hybrid.dll", "core,+|hybrid.dll,-Xamarin.Mac.dll" }) {
				ClearCommandsRun ();
				var options = new AOTOptions (testCase);
				Assert.IsTrue (options.IsAOT, "Should be IsAOT");
				Assert.IsFalse (options.IsHybridAOT, "Should not be IsHybridAOT");

				Compile (options, new TestFileEnumerator (new string [] { "|hybrid.dll", "Xamarin.Mac.dll" }));
				AssertFilesAOTed (new string [] { "|hybrid.dll" });
			}
		}

		[Test]
		public void InvalidHybridOptions_ShouldThrow ()
		{
			AssertThrowErrorWithCode (() => new AOTOptions ("|"), 20);
			AssertThrowErrorWithCode (() => new AOTOptions ("|hybrid"), 20);
			AssertThrowErrorWithCode (() => new AOTOptions ("core|"), 20);
			AssertThrowErrorWithCode (() => new AOTOptions ("foo|hybrid"), 20);
			AssertThrowErrorWithCode (() => new AOTOptions ("core|foo"), 20);
			AssertThrowErrorWithCode (() => new AOTOptions ("|hybrid,+Foo.dll"), 20);
		}

		[Test]
		public void HybridOption_ShouldInvokeHybridCompiler ()
		{
			var options = new AOTOptions ("all|hybrid");
			Assert.IsTrue (options.IsAOT, "Should be IsAOT");
			Assert.IsTrue (options.IsHybridAOT, "Should be IsHybridAOT");

			Compile (options, new TestFileEnumerator (FullAppFileList));

			var expectedFiles = FullAppFileList.Where (x => x.EndsWith (".exe") || x.EndsWith (".dll"));
			AssertFilesAOTed (expectedFiles, kind: AOTKind.Hybrid);
		}

		[Test]
		public void AllReleaseHybrid_AOTStripAndDelete ()
		{
			var options = new AOTOptions ("all|hybrid");

			Compile (options, new TestFileEnumerator (FullAppFileList), isRelease: true);

			var expectedFiles = FullAppFileList.Where (x => x.EndsWith (".exe") || x.EndsWith (".dll"));
			AssertFilesAOTed (expectedFiles, kind: AOTKind.Hybrid, expectStripping: true, expectSymbolDeletion: true);
			AssertFilesStripped (expectedFiles);
		}

		[Test]
		public void AllReleaseNonHybrid_ShouldNotStripButDelete ()
		{
			var options = new AOTOptions ("all");

			Compile (options, new TestFileEnumerator (FullAppFileList), isRelease: true);

			var expectedFiles = FullAppFileList.Where (x => x.EndsWith (".exe") || x.EndsWith (".dll"));
			AssertFilesAOTed (expectedFiles, expectStripping: false, expectSymbolDeletion: true);
			AssertFilesStripped (new string [] { });
		}

		[Test]
		public void WhenAssemblyStrippingFails_ShouldThrowError ()
		{
			RunCommandDelegate runThatErrors = (path, args, env, output, suppressPrintOnErrors) => path.Contains ("mono-cil-strip") ? 42 : 0;

			var options = new AOTOptions ("all|hybrid");

			AssertThrowErrorWithCode (() => Compile (options, new TestFileEnumerator (FullAppFileList), onRunDelegate: runThatErrors, isRelease: true), 3001);
		}

		[Test]
		public void HybridOption_MustAlsoHaveAll_ThrowsIfNot ()
		{
			AssertThrowErrorWithCode (() => new AOTOptions ("core|hybrid"), 114);
			AssertThrowErrorWithCode (() => new AOTOptions ("sdk|hybrid"), 114);
			var options = new AOTOptions ("all|hybrid");
		}

		[Test]
		public void All_AOTAllFiles_Modern ()
		{
			var options = new AOTOptions ("all");
			Assert.IsTrue (options.IsAOT, "Should be IsAOT");

			Compile (options, new TestFileEnumerator (FullAppFileList), isModern: true);

			var expectedFiles = FullAppFileList.Where (x => x.EndsWith (".exe") || x.EndsWith (".dll"));
			AssertFilesAOTed (expectedFiles, isModern: true);
		}
	}
}
