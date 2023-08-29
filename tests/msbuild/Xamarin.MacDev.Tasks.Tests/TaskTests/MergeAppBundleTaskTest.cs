using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Microsoft.Build.Utilities;

using NUnit.Framework;

using Xamarin.iOS.Tasks;
using Xamarin.Tests;
using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	[TestFixture]
	public class MergeAppBundleTaskTest : TestBase {
		[OneTimeSetUp]
		public void SetUp ()
		{
			var env = new Dictionary<string, string> {
				{ "MSBUILD_EXE_PATH", null }, // Comes from VSMac (when running tests from inside the IDE), and it confuses 'dotnet build', so remove it.
				{ "MSBuildSDKsPath", null }, // Comes from MSBuild, and confuses 'dotnet build'
			};

			RunMake (Path.Combine (Configuration.RootPath, "tests", "test-libraries"), environment: env);
			if (Configuration.include_dotnet)
				RunMake (Path.Combine (Configuration.RootPath, "tests", "common", "TestProjects", "ComplexAssembly"), environment: env);
		}

		static void RunMake (string directory, Dictionary<string, string> environment = null)
		{
			var arguments = new List<string> {
				"-C",
				directory,
				"V=1",
			};
			var rv = ExecutionHelper.Execute ("make",
				arguments,
				output: out var output,
				working_directory: null,
				timeout: TimeSpan.FromSeconds (120),
				environment_variables: environment);
			if (rv != 0) {
				var failure = $"'make {StringUtils.FormatArguments (StringUtils.QuoteForProcess (arguments))}' exited with exit code {rv}:";
				var indented = "\t" + string.Join ("\n\t", output.ToString ().Split ('\n'));
				Console.WriteLine (failure);
				Console.WriteLine (indented);
				// Only show the last 10 lines in the assert message, because otherwise the html reports can end up quite big.
				var shortIndented = indented.Split ('\n').Reverse ().Take (10).Reverse ();
				Assert.Fail (failure + "\n" + string.Join ("\n", shortIndented));
			}
		}

		MergeAppBundles CreateTask (string outputBundle, params string [] inputBundles)
		{
			var inputItems = new List<TaskItem> ();
			for (var i = 0; i < inputBundles.Length; i++) {
				var item = new TaskItem (inputBundles [i]);
				item.SetMetadata ("SpecificSubdirectory", $"SubDir{i + 1}");
				inputItems.Add (item);
			}
			var task = CreateTask<MergeAppBundles> ();
			task.InputAppBundles = inputItems.ToArray ();
			task.OutputAppBundle = outputBundle;
			return task;
		}

		// Create two app bundles, one with fileA, and one with fileB, in the root directory
		string [] CreateAppBundles (string fileA, string fileB, string fileName = null)
		{
			var appBundleA = Path.Combine (Cache.CreateTemporaryDirectory (), "MergeMe.app");
			var appBundleB = Path.Combine (Cache.CreateTemporaryDirectory (), "MergeMe.app");
			Directory.CreateDirectory (appBundleA);
			Directory.CreateDirectory (appBundleB);
			File.Copy (fileA, Path.Combine (appBundleA, fileName ?? Path.GetFileName (fileA)));
			File.Copy (fileB, Path.Combine (appBundleB, fileName ?? Path.GetFileName (fileB)));
			return new string [] { appBundleA, appBundleB };
		}

		string CreateAppBundle (string directory, params string [] files)
		{
			var appBundle = Path.Combine (Cache.CreateTemporaryDirectory (), "MergeMe.app");
			Directory.CreateDirectory (appBundle);
			foreach (var file in files) {
				var inputPath = Path.Combine (directory, file);
				var outputPath = Path.Combine (appBundle, file);
				Directory.CreateDirectory (Path.GetDirectoryName (outputPath));
				File.Copy (inputPath, outputPath, true);
			}
			return appBundle;
		}

		[Test]
		public void TestLipoExecutable ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.MacOSX);
			var fileA = Path.Combine (Configuration.RootPath, "tests", "test-libraries", ".libs", "macos", "libtest.arm64.dylib");
			var fileB = Path.Combine (Configuration.RootPath, "tests", "test-libraries", ".libs", "macos", "libtest.x86_64.dylib");
			var bundles = CreateAppBundles (fileA, fileB, "libtest.dylib");

			var outputBundle = Path.Combine (Cache.CreateTemporaryDirectory (), "Merged.app");
			var task = CreateTask (outputBundle, bundles);
			Assert.IsTrue (task.Execute (), "Task execution");

			// The bundle should only contain a single file.
			Assert.AreEqual (1, Directory.GetFileSystemEntries (outputBundle).Length, "Files in bundle");

			// The resulting dylib should contain 2 architectures.
			var fatLibrary = Path.Combine (outputBundle, "libtest.dylib");
			Assert.That (fatLibrary, Does.Exist, "Existence");
			var machO = MachO.Read (fatLibrary).ToArray ();
			Assert.AreEqual (2, machO.Length, "Architecture Count");
		}

		[Test]
		public void TestPEAssembly ()
		{
			Configuration.AssertDotNetAvailable ();
			var complexAssemblyPath = Path.Combine (Configuration.RootPath, "tests", "common", "TestProjects", "ComplexAssembly", "bin", "Debug", Configuration.DotNetTfm);
			var complexFiles = new string [] {
				"ComplexAssembly.dll",
				"ComplexAssembly.pdb",
				"ComplexAssembly.dll.config",
				"de/ComplexAssembly.resources.dll",
				"en-AU/ComplexAssembly.resources.dll",
				"es/ComplexAssembly.resources.dll",
			};
			var appA = CreateAppBundle (complexAssemblyPath, complexFiles);
			var appB = CreateAppBundle (complexAssemblyPath, complexFiles);
			var bundles = new string [] { appA, appB };

			var outputBundle = Path.Combine (Cache.CreateTemporaryDirectory (), "Merged.app");
			var task = CreateTask (outputBundle, bundles);
			Assert.IsTrue (task.Execute (), "Task execution");

			// The bundle should have all the files
			Assert.AreEqual (complexFiles.Length, Directory.GetFileSystemEntries (outputBundle).Length, "Files in bundle");

			// with the same structure
			foreach (var file in complexFiles)
				Assert.That (Path.Combine (outputBundle, file), Does.Exist, $"File existence");
		}

		[Test]
		public void TestDifferentOtherFiles ()
		{
			var tmpDir = Cache.CreateTemporaryDirectory ();
			var fileA = Path.Combine (tmpDir, "A.txt");
			var fileB = Path.Combine (tmpDir, "B.txt");
			File.WriteAllText (fileA, "A");
			File.WriteAllText (fileB, "B");
			var bundles = CreateAppBundles (fileA, fileB, "Something.txt");

			var outputBundle = Path.Combine (Cache.CreateTemporaryDirectory (), "Merged.app");
			var task = CreateTask (outputBundle, bundles);
			Assert.IsFalse (task.Execute (), "Task execution");
			Assert.AreEqual (3, Engine.Logger.ErrorEvents.Count, "Errors:\n\t" + string.Join ("\n\t", Engine.Logger.ErrorEvents.Select ((v) => v.Message).ToArray ()));
			Assert.AreEqual ("Unable to merge the file 'Something.txt', it's different between the input app bundles.", Engine.Logger.ErrorEvents [0].Message, "Error message");
			Assert.That (Engine.Logger.ErrorEvents [1].Message, Does.Match ("App bundle file #1: .*/MergeMe.app/Something.txt"), "Error message 2");
			Assert.That (Engine.Logger.ErrorEvents [2].Message, Does.Match ("App bundle file #2: .*/MergeMe.app/Something.txt"), "Error message 3");
		}

		[Test]
		public void TestSymlinks ()
		{
			var bundleA = Path.Combine (Cache.CreateTemporaryDirectory (), "MergeMe.app");
			var bundleB = Path.Combine (Cache.CreateTemporaryDirectory (), "MergeMe.app");
			var fileA = Path.Combine (bundleA, "A.txt");
			var fileB = Path.Combine (bundleB, "A.txt");
			Directory.CreateDirectory (Path.GetDirectoryName (fileA));
			File.WriteAllText (fileA, "A");
			Directory.CreateDirectory (Path.GetDirectoryName (fileB));
			File.WriteAllText (fileB, "A");
			var linkA = Path.Combine (bundleA, "B.txt");
			var linkB = Path.Combine (bundleB, "B.txt");
			Assert.IsTrue (PathUtils.Symlink ("A.txt", linkA), "Link A");
			Assert.IsTrue (PathUtils.Symlink ("A.txt", linkB), "Link B");


			var outputBundle = Path.Combine (Cache.CreateTemporaryDirectory (), "Merged.app");
			var task = CreateTask (outputBundle, bundleA, bundleB);
			Assert.IsTrue (task.Execute (), "Task execution");
			Assert.IsTrue (PathUtils.IsSymlink (Path.Combine (outputBundle, "B.txt")), "IsSymlink");
		}

		[Test]
		public void TestSymlinksWithDifferentTargets ()
		{
			var bundleA = Path.Combine (Cache.CreateTemporaryDirectory (), "MergeMe.app");
			var bundleB = Path.Combine (Cache.CreateTemporaryDirectory (), "MergeMe.app");
			var fileA = Path.Combine (bundleA, "A.txt");
			var fileB = Path.Combine (bundleB, "A.txt");
			var fileAC = Path.Combine (bundleA, "C.txt");
			var fileBC = Path.Combine (bundleB, "C.txt");
			Directory.CreateDirectory (Path.GetDirectoryName (fileA));
			File.WriteAllText (fileA, "A");
			File.WriteAllText (fileAC, "C");
			Directory.CreateDirectory (Path.GetDirectoryName (fileB));
			File.WriteAllText (fileB, "A");
			File.WriteAllText (fileBC, "C");
			// There's a symlink in both apps, but they have different targets.
			var linkA = Path.Combine (bundleA, "B.txt");
			var linkB = Path.Combine (bundleB, "B.txt");
			Assert.IsTrue (PathUtils.Symlink ("A.txt", linkA), "Link A");
			Assert.IsTrue (PathUtils.Symlink ("C.txt", linkB), "Link B");


			var outputBundle = Path.Combine (Cache.CreateTemporaryDirectory (), "Merged.app");
			var task = CreateTask (outputBundle, bundleA, bundleB);
			Assert.IsFalse (task.Execute (), "Task execution");
			Assert.AreEqual (3, Engine.Logger.ErrorEvents.Count, "Errors:\n\t" + string.Join ("\n\t", Engine.Logger.ErrorEvents.Select ((v) => v.Message).ToArray ()));
			Assert.AreEqual ("Can't merge the symlink 'B.txt', it has different targets.", Engine.Logger.ErrorEvents [0].Message, "Error message");
			Assert.That (Engine.Logger.ErrorEvents [1].Message, Does.Match ("App bundle file #1: .*/MergeMe.app/B.txt"), "Error message 2");
			Assert.That (Engine.Logger.ErrorEvents [2].Message, Does.Match ("App bundle file #2: .*/MergeMe.app/B.txt"), "Error message 3");
		}

		[Test]
		public void TestDirectories ()
		{
			var bundleA = Path.Combine (Cache.CreateTemporaryDirectory (), "MergeMe.app");
			var bundleB = Path.Combine (Cache.CreateTemporaryDirectory (), "MergeMe.app");
			var onlyA = "A";
			var onlyB = "B";
			var bothAB = "AB";
			var nestedOnlyA = "AA/AA";
			var nestedOnlyB = "BB/BB";
			var nestedBothAB = "ABAB/ABAB";
			var nestedSharedOnlyA = "ABS/A";
			var nestedSharedOnlyB = "ABS/B";

			Directory.CreateDirectory (Path.Combine (bundleA, onlyA));
			Directory.CreateDirectory (Path.Combine (bundleA, bothAB));
			Directory.CreateDirectory (Path.Combine (bundleA, nestedOnlyA));
			Directory.CreateDirectory (Path.Combine (bundleA, nestedBothAB));
			Directory.CreateDirectory (Path.Combine (bundleA, nestedSharedOnlyA));
			Directory.CreateDirectory (Path.Combine (bundleB, onlyB));
			Directory.CreateDirectory (Path.Combine (bundleB, nestedOnlyB));
			Directory.CreateDirectory (Path.Combine (bundleB, bothAB));
			Directory.CreateDirectory (Path.Combine (bundleB, nestedBothAB));
			Directory.CreateDirectory (Path.Combine (bundleB, nestedSharedOnlyB));

			var outputBundle = Path.Combine (Cache.CreateTemporaryDirectory (), "Merged.app");
			var task = CreateTask (outputBundle, bundleA, bundleB);
			Assert.IsTrue (task.Execute (), "Task execution");
			Assert.That (Path.Combine (outputBundle, onlyA), Does.Exist, "onlyA");
			Assert.That (Path.Combine (outputBundle, onlyB), Does.Exist, "onlyB");
			Assert.That (Path.Combine (outputBundle, bothAB), Does.Exist, "bothAB");
			Assert.That (Path.Combine (outputBundle, nestedOnlyA), Does.Exist, "nestedOnlyA");
			Assert.That (Path.Combine (outputBundle, nestedOnlyB), Does.Exist, "nestedOnlyB");
			Assert.That (Path.Combine (outputBundle, nestedBothAB), Does.Exist, "nestedBothAB");
			Assert.That (Path.Combine (outputBundle, nestedSharedOnlyA), Does.Exist, "nestedSharedOnlyA");
			Assert.That (Path.Combine (outputBundle, nestedSharedOnlyB), Does.Exist, "nestedSharedOnlyB");

			// Verify that there aren't any other directories
			Assert.AreEqual (7, Directory.GetFileSystemEntries (outputBundle).Length, "Directories in bundle");
		}

		[Test]
		public void TestSingleInput ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.MacOSX);
			var fileA = Path.Combine (Configuration.RootPath, "tests", "test-libraries", ".libs", "macos", "libtest.arm64.dylib");
			var bundle = CreateAppBundle (Path.GetDirectoryName (fileA), Path.GetFileName (fileA));
			var outputBundle = Path.Combine (Cache.CreateTemporaryDirectory (), "Merged.app");
			var task = CreateTask (outputBundle, bundle);
			Assert.IsTrue (task.Execute (), "Task execution");

			// The bundle should only contain a single file.
			Assert.AreEqual (1, Directory.GetFileSystemEntries (outputBundle).Length, "Files in bundle");

			// The resulting dylib should contain 1 architecture.
			var nonFatBinary = Path.Combine (outputBundle, "libtest.arm64.dylib");
			Assert.That (nonFatBinary, Does.Exist, "Existence");
			var machO = MachO.Read (nonFatBinary).ToArray ();
			Assert.AreEqual (1, machO.Length, "Architecture Count");

			// and the file size should be the same as the input
			Assert.That (new FileInfo (fileA).Length, Is.EqualTo (new FileInfo (nonFatBinary).Length), "File length");
		}

		[Test]
		public void TestDirectoriesAsSymlinks ()
		{
			var bundleA = Path.Combine (Cache.CreateTemporaryDirectory (), "MergeMe.app");
			var bundleB = Path.Combine (Cache.CreateTemporaryDirectory (), "MergeMe.app");
			var fileA = Path.Combine (bundleA, "A", "A.txt");
			var fileB = Path.Combine (bundleB, "A.txt");
			Directory.CreateDirectory (Path.GetDirectoryName (fileA));
			File.WriteAllText (fileA, "A");
			Directory.CreateDirectory (Path.GetDirectoryName (fileB));
			File.WriteAllText (fileB, "A");
			var linkA = Path.Combine (bundleA, "B");
			var linkB = Path.Combine (bundleB, "B");
			Assert.IsTrue (PathUtils.Symlink ("A", linkA), "Link A");
			Assert.IsTrue (PathUtils.Symlink ("A", linkB), "Link B");

			var outputBundle = Path.Combine (Cache.CreateTemporaryDirectory (), "Merged.app");
			var task = CreateTask (outputBundle, bundleA, bundleB);
			Assert.IsTrue (task.Execute (), "Task execution");
			Assert.IsTrue (PathUtils.IsSymlink (Path.Combine (outputBundle, "B")), "IsSymlink");
			Assert.IsFalse (PathUtils.IsSymlink (Path.Combine (outputBundle, "A", "A.txt")), "IsSymlink");
			Assert.IsFalse (PathUtils.IsSymlink (Path.Combine (outputBundle, "B", "A.txt")), "IsSymlink");
		}
	}
}
