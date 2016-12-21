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

		AOTCompiler compiler;
		List<Tuple<string, string>> commandsRun;

		[SetUp]
		public void Init ()
		{
			compiler = new AOTCompiler ()
			{
				RunCommand = OnRunCommand,
				ParallelOptions = new ParallelOptions () { MaxDegreeOfParallelism = 1 }
			};

			commandsRun = new List<Tuple<string, string>> ();
		}

		int OnRunCommand (string path, string args, string [] env, StringBuilder output, bool suppressPrintOnErrors)
		{
			Assert.IsTrue (env[0] == "MONO_PATH", "MONO_PATH should be first env set");
			Assert.IsTrue (env[1] == TestRootDir, "MONO_PATH should be set to our expected value");
			commandsRun.Add (Tuple.Create <string, string>(path, args));
			return 0;
		}

		List<string> GetFiledAOTed ()
		{
			List<string> filesAOTed = new List<string> (); 

			foreach (var command in commandsRun) {
				Assert.AreEqual (command.Item1, "/Library/Frameworks/Xamarin.Mac.framework/Commands/bmac-mobile-mono", "Command should be bmac-mobile-mono");
				Assert.AreEqual (command.Item2.Split (' ')[0], "--aot=hybrid", "First arg should be --aot=hybrid");
				string fileName = command.Item2.Substring (command.Item2.IndexOf(' ') + 1).Replace ("\"", "");
				filesAOTed.Add (fileName);
			}
			return filesAOTed;
		}

		void AssertFilesAOTed (IEnumerable <string> expectedFiles)
		{
			List<string> filesAOTed = GetFiledAOTed ();

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
			"Foo Bar.exe", "libMonoPosixHelper.dylib", "mscorlib.dll", "Xamarin.Mac.dll", "System.dll" 
		};

		readonly string [] SDKFileList = { "mscorlib.dll", "Xamarin.Mac.dll", "System.dll" };

		[Test]
		public void ParsingNone_DoesNoAOT ()
		{
			compiler.Parse ("none");
			Assert.IsFalse (compiler.IsAOT, "Parsing none should not be IsAOT");
			AssertThrowErrorWithCode (() => compiler.Compile (new TestFileEnumerator (FullAppFileList)), 99);
		}

		[Test]
		public void All_AOTAllFiles ()
		{
			compiler.Parse ("all");
			Assert.IsTrue (compiler.IsAOT, "Should be IsAOT");

			compiler.Compile (new TestFileEnumerator (FullAppFileList));

			var expectedFiles = FullAppFileList.Where (x => x.EndsWith (".exe") || x.EndsWith (".dll"));
			AssertFilesAOTed (expectedFiles);
		}

		[Test]
		public void SDK_ParsingJustSDKFiles()
		{
			compiler.Parse ("sdk");
			Assert.IsTrue (compiler.IsAOT, "Should be IsAOT");

			compiler.Compile (new TestFileEnumerator (FullAppFileList));

			AssertFilesAOTed (SDKFileList);
		}

		[Test]
		public void ExplicitAssembly_JustAOTExplicitFile ()
		{
			compiler.Parse ("+System.dll");
			Assert.IsTrue (compiler.IsAOT, "Should be IsAOT");

			compiler.Compile (new TestFileEnumerator (FullAppFileList));

			AssertFilesAOTed (new string [] { "System.dll" });
		}

		[Test]
		public void SDKWithInclusionAndSubtraction ()
		{
			compiler.Parse ("sdk,+Foo.dll,-Xamarin.Mac.dll");
			Assert.IsTrue (compiler.IsAOT, "Should be IsAOT");
		
			string [] testFiles = { 
				"Foo.dll", "Foo Bar.exe", "libMonoPosixHelper.dylib", "mscorlib.dll", "Xamarin.Mac.dll", "System.dll"
			};
			compiler.Compile (new TestFileEnumerator (testFiles));

			AssertFilesAOTed (new string [] { "Foo.dll", "mscorlib.dll", "System.dll" });
		}

		[Test]
		public void SDKWithFullPath_GivesFullPathCommands ()
		{
			compiler.Parse ("sdk,-Xamarin.Mac.dll");
			Assert.IsTrue (compiler.IsAOT, "Should be IsAOT");

			compiler.Compile (new TestFileEnumerator (FullAppFileList.Select (x => TestRootDir + x)));
			AssertFilesAOTed (new string [] { TestRootDir + "mscorlib.dll", TestRootDir + "System.dll" });
		}

		[Test]
		public void ExplicitNegativeFileWithNonExistantFiles_ThrowError ()
		{
			compiler.Parse ("sdk,-NonExistant.dll");
			Assert.IsTrue (compiler.IsAOT, "Should be IsAOT");

			AssertThrowErrorWithCode (() => compiler.Compile (new TestFileEnumerator (FullAppFileList)), 3010);
		}

		[Test]
		public void ExplicitPositiveFileWithNonExistantFiles_ThrowError ()
		{
			compiler.Parse ("sdk,+NonExistant.dll");
			Assert.IsTrue (compiler.IsAOT, "Should be IsAOT");

			AssertThrowErrorWithCode (() => compiler.Compile (new TestFileEnumerator (FullAppFileList)), 3009);
		}

		[Test]
		public void ExplicitNegativeWithNoAssemblies_ShouldNoOp()
		{
			compiler.Parse ("-System.dll");
			Assert.IsTrue (compiler.IsAOT, "Should be IsAOT");

			compiler.Compile (new TestFileEnumerator (FullAppFileList));
			AssertFilesAOTed (new string [] {});
		}

		[Test]
		public void ParsingSimpleOptions_InvalidOption ()
		{
			AssertThrowErrorWithCode (() => compiler.Parse ("FooBar"), 20);
		}

		[Test]
		public void AssemblyWithSpaces_ShouldAOTWithQuotes ()
		{
			compiler.Parse ("+Foo Bar.dll");
			Assert.IsTrue (compiler.IsAOT, "Should be IsAOT");

			compiler.Compile (new TestFileEnumerator (new string [] { "Foo Bar.dll", "Xamarin.Mac.dll" }));
			AssertFilesAOTed (new string [] {"Foo Bar.dll"});
			Assert.IsTrue (commandsRun[0].Item2.EndsWith ("\"Foo Bar.dll\"", StringComparison.InvariantCulture), "Should end with quoted filename");
		}

		[Test]
		public void WhenAOTFails_ShouldReturnError ()
		{
			compiler.RunCommand = (path, args, env, output, suppressPrintOnErrors) => 42;
			compiler.Parse ("all");
			AssertThrowErrorWithCode (() => compiler.Compile (new TestFileEnumerator (FullAppFileList)), 3001);
		}
	}
}
