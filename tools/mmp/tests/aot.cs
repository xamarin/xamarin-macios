using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using Xamarin.Bundler;

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

		void Compile (AOTOptions options, TestFileEnumerator files, AOTCompilerType compilerType = AOTCompilerType.Bundled64, RunCommandDelegate onRunDelegate = null)
		{
			AOTCompiler compiler = new AOTCompiler (options, compilerType)
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
			Assert.IsTrue (env[0] == "MONO_PATH", "MONO_PATH should be first env set");
			Assert.IsTrue (env[1] == TestRootDir, "MONO_PATH should be set to our expected value");
			commandsRun.Add (Tuple.Create <string, string>(path, args));
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

		List<string> GetFiledAOTed (AOTCompilerType compilerType = AOTCompilerType.Bundled64)
		{
			List<string> filesAOTed = new List<string> (); 

			foreach (var command in commandsRun) {
				Assert.IsTrue (command.Item1.EndsWith (GetExpectedMonoCommand (compilerType)), "Unexpected command: " + command.Item1);
				Assert.AreEqual (command.Item2.Split (' ')[0], "--aot=hybrid", "First arg should be --aot=hybrid");
				string fileName = command.Item2.Substring (command.Item2.IndexOf(' ') + 1).Replace ("\"", "");
				filesAOTed.Add (fileName);
			}
			return filesAOTed;
		}

		void AssertFilesAOTed (IEnumerable <string> expectedFiles, AOTCompilerType compilerType = AOTCompilerType.Bundled64)
		{
			List<string> filesAOTed = GetFiledAOTed (compilerType);

			Func<string> getErrorDetails = () => $"\n {String.Join (" ", filesAOTed)} \nvs\n {String.Join (" ", expectedFiles)}";

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
			Assert.IsTrue (commandsRun[0].Item2.EndsWith ("\"Foo Bar.dll\"", StringComparison.InvariantCulture), "Should end with quoted filename");
		}

		[Test]
		public void WhenAOTFails_ShouldReturnError ()
		{
			RunCommandDelegate runThatErrors = (path, args, env, output, suppressPrintOnErrors) => 42;
			var options = new AOTOptions ("all");

			AssertThrowErrorWithCode (() => Compile (options, new TestFileEnumerator (FullAppFileList), AOTCompilerType.Bundled64, runThatErrors), 3001);
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
	}
}
