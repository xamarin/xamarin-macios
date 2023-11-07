using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using NUnit.Framework;

using Xamarin.iOS.Tasks;
using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	[TestFixture]
	public class CreateBindingResourceTaskTests : TestBase {
		CreateBindingResourcePackage ExecuteTask (string compress, bool symlinks, out string tmpdir)
		{
			tmpdir = Cache.CreateTemporaryDirectory ();
			var task = CreateTask<CreateBindingResourcePackage> ();
			task.Compress = compress;
			task.BindingResourcePath = Path.Combine (tmpdir, "CreateBindingResourceTaskTest");
			task.IntermediateOutputPath = Path.Combine (tmpdir, "IntermediateOutputPath");
			task.NativeReferences = CreateNativeReferences (tmpdir, symlinks);

			var currentDir = Environment.CurrentDirectory;
			try {
				Environment.CurrentDirectory = tmpdir;
				Assert.IsTrue (task.Execute (), "Execute");
			} finally {
				Environment.CurrentDirectory = currentDir;
			}

			return task;
		}

		[Test]
		[TestCase (true)]
		[TestCase (false)]
		public void Compressed (bool symlinks)
		{
			var task = ExecuteTask ("true", symlinks, out var tmpdir);

			var zipFile = task.BindingResourcePath + ".zip";
			Assert.That (zipFile, Does.Exist, "Zip existence");

			var extracted = Path.Combine (tmpdir, "Extracted");
			Extract (zipFile, extracted);
			AssertResourceDirectory (extracted, symlinks);
		}

		[Test]
		[TestCase (true)]
		[TestCase (false)]
		public void Uncompressed (bool symlinks)
		{
			var task = ExecuteTask ("false", symlinks, out var tmpdir);

			AssertResourceDirectory (task.BindingResourcePath, symlinks);
		}

		[Test]
		[TestCase (true)]
		[TestCase (false)]
		public void Auto (bool symlinks)
		{
			var task = ExecuteTask ("auto", symlinks, out var tmpdir);

			string extracted;
			if (symlinks) {
				var zipFile = task.BindingResourcePath + ".zip";
				Assert.That (zipFile, Does.Exist, "Zip existence");

				extracted = Path.Combine (tmpdir, "Extracted");
				Extract (zipFile, extracted);
			} else {
				extracted = task.BindingResourcePath;
			}
			AssertResourceDirectory (extracted, symlinks);
		}

		void Extract (string zipArchive, string targetDirectory)
		{
			var unzipArguments = new List<string> ();
			unzipArguments.Add ("-d");
			unzipArguments.Add (targetDirectory);
			unzipArguments.Add (zipArchive);
			var output = new StringBuilder ();
			var rv = Execution.RunWithStringBuildersAsync ("unzip", unzipArguments, standardOutput: output, standardError: output).Result;
			Assert.AreEqual (0, rv.ExitCode, "ExitCode\n" + output.ToString ());
		}

		void AssertResourceDirectory (string directory, bool symlinks)
		{
			var allFiles = Directory.GetFileSystemEntries (directory, "*", SearchOption.AllDirectories).OrderBy (v => v).Select (v => v.Substring (directory.Length + 1)).ToArray ();
			foreach (var file in allFiles)
				Console.WriteLine (file);
			if (symlinks) {
				Assert.AreEqual (7, allFiles.Length, "Length");
			} else {
				Assert.AreEqual (5, allFiles.Length, "Length");
			}
			Assert.AreEqual ("ABCDEFGHIJKLMAAA", File.ReadAllText (Path.Combine (directory, "A.txt")), "A.txt");
			Assert.AreEqual ("ABCDEFGHIJKLMBBB", File.ReadAllText (Path.Combine (directory, "B.txt")), "B.txt");
			Assert.AreEqual ("ABCDEFGHIJKLMCCC", File.ReadAllText (Path.Combine (directory, "C.framework/C.txt")), "C.txt");
			if (symlinks) {
				var linkToCPath = Path.Combine (directory, "C.framework/LinkToC.txt");
				Assert.AreEqual ("ABCDEFGHIJKLMCCC", File.ReadAllText (linkToCPath), "LinkToC.txt");
				Assert.IsTrue (PathUtils.IsSymlink (linkToCPath), "LinkToC.txt - IsSymlink");
				Assert.AreEqual ("C.txt", PathUtils.GetSymlinkTarget (linkToCPath), "LinkToC.txt - IsSymlink target");

				var linkToNowherePath = Path.Combine (directory, "C.framework/LinkToNowhere.txt");
				Assert.Throws<FileNotFoundException> (() => File.ReadAllText (linkToNowherePath), "LinkToNowhere.txt");
				Assert.AreEqual ("Nowhere.txt", PathUtils.GetSymlinkTarget (linkToNowherePath), "LinkToNowhere.txt - IsSymlink target");
			}

			var manifest = @"<BindingAssembly>
	<NativeReference Name=""A.txt"">
		<ForceLoad></ForceLoad>
		<Frameworks></Frameworks>
		<IsCxx></IsCxx>
		<Kind></Kind>
		<LinkerFlags></LinkerFlags>
		<NeedsGccExceptionHandling></NeedsGccExceptionHandling>
		<SmartLink></SmartLink>
		<WeakFrameworks></WeakFrameworks>
	</NativeReference>
	<NativeReference Name=""B.txt"">
		<ForceLoad></ForceLoad>
		<Frameworks></Frameworks>
		<IsCxx></IsCxx>
		<Kind></Kind>
		<LinkerFlags></LinkerFlags>
		<NeedsGccExceptionHandling></NeedsGccExceptionHandling>
		<SmartLink></SmartLink>
		<WeakFrameworks></WeakFrameworks>
	</NativeReference>
	<NativeReference Name=""C.framework"">
		<ForceLoad></ForceLoad>
		<Frameworks></Frameworks>
		<IsCxx></IsCxx>
		<Kind></Kind>
		<LinkerFlags></LinkerFlags>
		<NeedsGccExceptionHandling></NeedsGccExceptionHandling>
		<SmartLink></SmartLink>
		<WeakFrameworks></WeakFrameworks>
	</NativeReference>
</BindingAssembly>";
			Assert.AreEqual (manifest, File.ReadAllText (Path.Combine (directory, "manifest")), "Manifest");
		}

		ITaskItem [] CreateNativeReferences (string tmpdir, bool symlinks)
		{
			var rv = new List<ITaskItem> ();

			// Full path
			var fn = Path.Combine (tmpdir, "A.txt");
			File.WriteAllText (fn, "ABCDEFGHIJKLMAAA");
			rv.Add (new TaskItem (fn));

			// Relative path
			fn = Path.Combine (tmpdir, "B.txt");
			File.WriteAllText (fn, "ABCDEFGHIJKLMBBB");
			rv.Add (new TaskItem (Path.GetFileName (fn)));

			// Directory with symlink
			var dir = Path.Combine (tmpdir, "C.framework");
			Directory.CreateDirectory (dir);
			rv.Add (new TaskItem (dir));
			File.WriteAllText (Path.Combine (dir, "C.txt"), "ABCDEFGHIJKLMCCC");
			if (symlinks) {
				PathUtils.CreateSymlink (Path.Combine (dir, "LinkToC.txt"), "C.txt");
				PathUtils.CreateSymlink (Path.Combine (dir, "LinkToNowhere.txt"), "Nowhere.txt");
			}

			return rv.ToArray ();
		}

	}
}

