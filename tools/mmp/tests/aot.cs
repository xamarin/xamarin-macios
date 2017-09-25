using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using Xamarin.Bundler;
using Xamarin.Utils;

namespace Xamarin.MMP.Tests.Unit
{
	[TestFixture]
	public class AotTests
	{
		const string TestRootDir = "/a/non/sense/dir/";

		class TestFileEnumerator : IFileEnumerator
		{
			public IEnumerable<string> Files { get; }
			public string RootDir { get; } = TestRootDir;

			public TestFileEnumerator (IEnumerable <string> files)
			{
				Files = files;
			}
		}

		List<Tuple<string, string>> commandsRun;

		[SetUp]
		public void Init ()
		{
			// Make sure this is cleared between every test
			commandsRun = new List<Tuple<string, string>> ();
		}

		void Compile (AOTOptions options, TestFileEnumerator files, AOTCompilerType compilerType = AOTCompilerType.Bundled64, RunCommandDelegate onRunDelegate = null, bool isRelease = false, bool isModern = false)
		{
			AOTCompiler compiler = new AOTCompiler (options, compilerType, isModern, isRelease)
			{
				RunCommand = onRunDelegate != null ? onRunDelegate : OnRunCommand,
				ParallelOptions = new ParallelOptions () { MaxDegreeOfParallelism = 1 },
				XamarinMacPrefix = Driver.WalkUpDirHierarchyLookingForLocalBuild (), // HACK - AOT test shouldn't need this from driver.cs 
			};
			compiler.Compile (files);
		}

		void ClearCommandsRun ()
		{
			commandsRun.Clear ();
		}

		int OnRunCommand (string path, string args, string [] env, StringBuilder output, bool suppressPrintOnErrors)
		{
			commandsRun.Add (Tuple.Create <string, string>(path, args));
			if (path != AOTCompiler.StripCommand && path != AOTCompiler.DeleteDebugSymbolCommand) {
				Assert.IsTrue (env[0] == "MONO_PATH", "MONO_PATH should be first env set");
				Assert.IsTrue (env[1] == TestRootDir, "MONO_PATH should be set to our expected value");
			}
			return 0;
		}

		string GetExpectedMonoCommand (AOTCompilerType compilerType)
		{
			switch (compilerType) {
			case AOTCompilerType.Bundled64:
				return "bmac-mobile-mono";
			case AOTCompilerType.Bundled32:
				return "bmac-mobile-mono-32";
			case AOTCompilerType.System64:
				return "mono64";
			case AOTCompilerType.System32:
				return "mono32";
			default:
				Assert.Fail ("GetMonoPath with invalid option");
				return "";
			}
		}

		List<string> GetFiledAOTed (AOTCompilerType compilerType = AOTCompilerType.Bundled64, AOTKind kind = AOTKind.Standard, bool isModern = false, bool expectStripping = false, bool expectSymbolDeletion = false)
		{
			List<string> filesAOTed = new List<string> (); 

			foreach (var command in commandsRun) {
				if (expectStripping && command.Item1 == AOTCompiler.StripCommand)
					continue;
				if (expectSymbolDeletion && command.Item1 == AOTCompiler.DeleteDebugSymbolCommand)
					continue;
				Assert.IsTrue (command.Item1.EndsWith (GetExpectedMonoCommand (compilerType)), "Unexpected command: " + command.Item1);
				string [] argParts = command.Item2.Split (' ');

				if (kind == AOTKind.Hybrid)
					Assert.AreEqual (argParts[0], "--aot=hybrid", "First arg should be --aot=hybrid");
				else
					Assert.AreEqual (argParts[0], "--aot", "First arg should be --aot");

				if (isModern)
					Assert.AreEqual (argParts[1], "--runtime=mobile", "Second arg should be --runtime=mobile");
				else
					Assert.AreNotEqual (argParts[1], "--runtime=mobile", "Second arg should not be --runtime=mobile");


				int fileNameBeginningIndex = command.Item2.IndexOf(' ') + 1;
				if (isModern)
					fileNameBeginningIndex = command.Item2.IndexOf(' ', fileNameBeginningIndex) + 1;

				string fileName = command.Item2.Substring (fileNameBeginningIndex).Replace ("\'", "");
				filesAOTed.Add (fileName);
			}
			return filesAOTed;
		}

		List<string> GetFilesStripped ()
		{
			return commandsRun.Where (x => x.Item1 == AOTCompiler.StripCommand).Select (x => x.Item2.Replace ("\'", "")).ToList ();
		}
		
		void AssertFilesStripped (IEnumerable <string> expectedFiles)
		{
			List<string> filesStripped = GetFilesStripped ();

			Func<string> getErrorDetails = () => $"\n {FormatDebugList (filesStripped)} \nvs\n {FormatDebugList (expectedFiles)}\n{AllCommandsRun}";

			Assert.AreEqual (filesStripped.Count, expectedFiles.Count (), "Different number of files stripped than expected: " + getErrorDetails ());
			Assert.IsTrue (filesStripped.All (x => expectedFiles.Contains (x)), "Different files stripped than expected: "  + getErrorDetails ());
		}
		
		List<string> GetDeletedSymbols ()
		{
			// Chop off -r prefix and quotes around filename
			return commandsRun.Where (x => x.Item1 == AOTCompiler.DeleteDebugSymbolCommand).Select (x => x.Item2.Substring(3).Replace ("\'", "")).ToList ();
		}

		string AllCommandsRun => "\nCommands Run:\n\t" + String.Join ("\n\t", commandsRun.Select (x => $"{x.Item1} {x.Item2}"));
		string FormatDebugList (IEnumerable <string> list) => String.Join (" ", list.Select (x => "\"" + x + "\""));

		void AssertSymbolsDeleted (IEnumerable <string> expectedFiles)
		{
			expectedFiles = expectedFiles.Select (x => x + ".dylib.dSYM/").ToList ();
			List<string> symbolsDeleted = GetDeletedSymbols ();

			Func<string> getErrorDetails = () => $"\n {FormatDebugList (symbolsDeleted)} \nvs\n {FormatDebugList (expectedFiles)}\n{AllCommandsRun}";

			Assert.AreEqual (symbolsDeleted.Count, expectedFiles.Count (), "Different number of symbols deleted than expected: " + getErrorDetails ());
			Assert.IsTrue (symbolsDeleted.All (x => expectedFiles.Contains (x)), "Different files deleted than expected: "  + getErrorDetails ());
		}

		void AssertFilesAOTed (IEnumerable <string> expectedFiles, AOTCompilerType compilerType = AOTCompilerType.Bundled64, AOTKind kind = AOTKind.Standard, bool isModern = false, bool expectStripping = false, bool expectSymbolDeletion = false)
		{
			List<string> filesAOTed = GetFiledAOTed (compilerType, kind, isModern: isModern, expectStripping: expectStripping, expectSymbolDeletion : expectSymbolDeletion);

			Func<string> getErrorDetails = () => $"\n {FormatDebugList (filesAOTed)} \nvs\n {FormatDebugList (expectedFiles)}\n{AllCommandsRun}";

			Assert.AreEqual (filesAOTed.Count, expectedFiles.Count (), "Different number of files AOT than expected: " + getErrorDetails ());
			Assert.IsTrue (filesAOTed.All (x => expectedFiles.Contains (x)), "Different files AOT than expected: "  + getErrorDetails ());
		}

		void AssertThrowErrorWithCode (Action action, int code)
		{
			try {
				action ();
			}
			catch (MonoMacException e) {
				Assert.AreEqual (e.Code, code, $"Got code {e.Code} but expected {code}");
				return;
			}
			catch (AggregateException e) {
				Assert.AreEqual (e.InnerExceptions.Count, 1, "Got AggregateException but more than one exception");
				MonoMacException innerException = e.InnerExceptions[0] as MonoMacException;
				Assert.IsNotNull (innerException, "Got AggregateException but inner not MonoMacException");
				Assert.AreEqual (innerException.Code, code, $"Got code {innerException.Code} but expected {code}");
				return;
			}
			Assert.Fail ($"We should have thrown MonoMacException with code: {code}");
		}

		readonly string [] FullAppFileList = { 
			"Foo Bar.exe", "libMonoPosixHelper.dylib", "mscorlib.dll", "Xamarin.Mac.dll", "System.dll", "System.Core.dll"
		};

		readonly string [] CoreXMFileList = { "mscorlib.dll", "Xamarin.Mac.dll", "System.dll" };
		readonly string [] SDKFileList = { "mscorlib.dll", "Xamarin.Mac.dll", "System.dll", "System.Core.dll" };

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
		public void Core_ParsingJustCoreFiles()
		{
			var options = new AOTOptions ("core");
			Assert.IsTrue (options.IsAOT, "Should be IsAOT");

			Compile (options, new TestFileEnumerator (FullAppFileList));

			AssertFilesAOTed (CoreXMFileList);
		}

		[Test]
		public void SDK_ParsingJustSDKFiles()
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
		public void ExplicitNegativeWithNoAssemblies_ShouldNoOp()
		{
			var options = new AOTOptions ("-System.dll");
			Assert.IsTrue (options.IsAOT, "Should be IsAOT");

			Compile (options, new TestFileEnumerator (FullAppFileList));
			AssertFilesAOTed (new string [] {});
		}

		[Test]
		public void ParsingSimpleOptions_InvalidOption ()
		{
			AssertThrowErrorWithCode (() => new AOTOptions ("FooBar"), 20);
		}

		[Test]
		public void AssemblyWithSpaces_ShouldAOTWithQuotes ()
		{
			var options = new AOTOptions ("+Foo Bar.dll");
			Assert.IsTrue (options.IsAOT, "Should be IsAOT");

			Compile (options, new TestFileEnumerator (new string [] { "Foo Bar.dll", "Xamarin.Mac.dll" }));
			AssertFilesAOTed (new string [] {"Foo Bar.dll"});
			Assert.IsTrue (commandsRun.Where (x => x.Item2.Contains ("Foo Bar.dll")).All (x => x.Item2.EndsWith ("\'Foo Bar.dll\'", StringComparison.InvariantCulture)), "Should end with quoted filename");
		}

		[Test]
		public void WhenAOTFails_ShouldReturnError ()
		{
			RunCommandDelegate runThatErrors = (path, args, env, output, suppressPrintOnErrors) => 42;
			var options = new AOTOptions ("all");

			AssertThrowErrorWithCode (() => Compile (options, new TestFileEnumerator (FullAppFileList), onRunDelegate : runThatErrors), 3001);
		}

		[Test]
		public void DifferentMonoTypes_ShouldInvokeCorrectMono ()
		{
			foreach (var compilerType in new List<AOTCompilerType> (){ AOTCompilerType.Bundled64, AOTCompilerType.Bundled32, AOTCompilerType.System32, AOTCompilerType.System64 })
			{
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
			foreach (var testCase in new string [] { "+|hybrid.dll", "core,+|hybrid.dll,-Xamarin.Mac.dll" }){
				ClearCommandsRun ();
				var options = new AOTOptions (testCase);
				Assert.IsTrue (options.IsAOT, "Should be IsAOT");
				Assert.IsFalse (options.IsHybridAOT, "Should not be IsHybridAOT");

				Compile (options, new TestFileEnumerator (new string [] { "|hybrid.dll", "Xamarin.Mac.dll" }));
				AssertFilesAOTed (new string [] {"|hybrid.dll"});
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
			AssertFilesAOTed (expectedFiles, kind : AOTKind.Hybrid);
		}

		[Test]
		public void AllReleaseHybrid_AOTStripAndDelete ()
		{
			var options = new AOTOptions ("all|hybrid");

			Compile (options, new TestFileEnumerator (FullAppFileList), isRelease : true);

			var expectedFiles = FullAppFileList.Where (x => x.EndsWith (".exe") || x.EndsWith (".dll"));
			AssertFilesAOTed (expectedFiles, kind : AOTKind.Hybrid, expectStripping : true, expectSymbolDeletion : true);
			AssertFilesStripped (expectedFiles);
			AssertSymbolsDeleted (expectedFiles);
		}

		[Test]
		public void AllReleaseNonHybrid_ShouldNotStripButDelete ()
		{
			var options = new AOTOptions ("all");

			Compile (options, new TestFileEnumerator (FullAppFileList), isRelease : true);

			var expectedFiles = FullAppFileList.Where (x => x.EndsWith (".exe") || x.EndsWith (".dll"));
			AssertFilesAOTed (expectedFiles, expectStripping : false, expectSymbolDeletion : true);
			AssertFilesStripped (new string [] {});
			AssertSymbolsDeleted (expectedFiles);
		}

		[Test]
		public void AssemblyWithSpaces_ShouldStripWithQuotes ()
		{
			var options = new AOTOptions ("all|hybrid,+Foo Bar.dll");

			var files = new string [] { "Foo Bar.dll", "Xamarin.Mac.dll" };
			Compile (options, new TestFileEnumerator (files), isRelease : true);
			AssertFilesStripped (files);
			AssertSymbolsDeleted (files);
			// We don't check end quote here, since we might have .dylib.dSYM suffix
			Assert.IsTrue (commandsRun.Where (x => x.Item2.Contains ("Foo Bar.dll")).All (x => x.Item2.Contains ("\'Foo Bar.dll")), "Should contain quoted filename");
		}

		[Test]
		public void WhenAssemblyStrippingFails_ShouldThrowError ()
		{
			RunCommandDelegate runThatErrors = (path, args, env, output, suppressPrintOnErrors) => path.Contains ("mono-cil-strip") ? 42 : 0;

			var options = new AOTOptions ("all|hybrid");

			AssertThrowErrorWithCode (() => Compile (options, new TestFileEnumerator (FullAppFileList), onRunDelegate : runThatErrors, isRelease : true), 3001);
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

			Compile (options, new TestFileEnumerator (FullAppFileList), isModern : true);

			var expectedFiles = FullAppFileList.Where (x => x.EndsWith (".exe") || x.EndsWith (".dll"));
			AssertFilesAOTed (expectedFiles, isModern : true);
		}
	}
}
