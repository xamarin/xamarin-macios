using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using NUnit.Framework;

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
				ExecuteTask (task);
			} finally {
				Environment.CurrentDirectory = currentDir;
			}

			return task;
		}

		[Test]
		public void DlsymMetadata ([Values ("true", "false")] string compress, [Values ("Default", "Required", "Disabled")] string dlsym)
		{
			var tmpdir = Cache.CreateTemporaryDirectory ();
			var library = Path.Combine (tmpdir, "libNative.a");
			File.WriteAllText (library, "");
			var reference = new TaskItem (library);
			reference.SetMetadata ("Kind", "Static");
			reference.SetMetadata ("Dlsym", dlsym);

			var package = CreateTask<CreateBindingResourcePackage> ();
			package.Compress = compress;
			package.BindingResourcePath = Path.Combine (tmpdir, "Binding.resources");
			package.IntermediateOutputPath = Path.Combine (tmpdir, "obj");
			package.NativeReferences = new ITaskItem [] { reference };
			ExecuteTask (package);

			var resources = package.BindingResourcePath;
			if (compress == "true") {
				resources = Path.Combine (tmpdir, "Extracted");
				Extract (package.BindingResourcePath + ".zip", resources);
			}
			var manifest = new XmlDocument ();
			manifest.Load (Path.Combine (resources, "manifest"));
			Assert.That (manifest.SelectSingleNode ("/BindingAssembly/NativeReference/Dlsym")?.InnerText, Is.EqualTo (dlsym), "Packaged Dlsym");

			var resolve = CreateTask<ResolveNativeReferences> ();
			resolve.Architectures = "arm64";
			resolve.FrameworksDirectory = "Frameworks";
			resolve.IntermediateOutputPath = Path.Combine (tmpdir, "resolved");
			resolve.TargetFrameworkMoniker = "net11.0-ios";
			resolve.References = new ITaskItem [] { new TaskItem (Path.Combine (tmpdir, "Binding.dll")) };
			ExecuteTask (resolve);
			Assert.That (resolve.NativeFrameworks?.Select (v => v.GetMetadata ("Dlsym")), Is.EqualTo (new [] { dlsym }), "Resolved Dlsym");
		}

		[Test]
		[TestCase (true, false)]
		[TestCase (false, true)]
		[TestCase (false, false)]
		public void Compressed (bool symlinks, bool useSystemIOCompression)
		{
			var usedSystemIOCompression = Environment.GetEnvironmentVariable ("XAMARIN_USE_SYSTEM_IO_COMPRESSION");
			try {
				Environment.SetEnvironmentVariable ("XAMARIN_USE_SYSTEM_IO_COMPRESSION", useSystemIOCompression ? "1" : null);

				var task = ExecuteTask ("true", symlinks, out var tmpdir);

				var zipFile = task.BindingResourcePath + ".zip";
				Assert.That (zipFile, Does.Exist, "Zip existence");

				var extracted = Path.Combine (tmpdir, "Extracted");
				Extract (zipFile, extracted);
				AssertResourceDirectory (extracted, symlinks);
			} finally {
				Environment.SetEnvironmentVariable ("XAMARIN_USE_SYSTEM_IO_COMPRESSION", usedSystemIOCompression);
			}
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

			var compressed = true;
			string extracted;
			if (compressed) {
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
			var rv = Execution.RunAsync ("unzip", unzipArguments).Result;
			Assert.That (rv.ExitCode, Is.EqualTo (0), "ExitCode\n" + rv.Output.MergedOutput);
		}

		void AssertResourceDirectory (string directory, bool symlinks)
		{
			var allFiles = Directory.GetFileSystemEntries (directory, "*", SearchOption.AllDirectories).OrderBy (v => v).Select (v => v.Substring (directory.Length + 1)).ToArray ();
			foreach (var file in allFiles)
				Console.WriteLine (file);
			if (symlinks) {
				Assert.That (allFiles.Length, Is.EqualTo (7), "Length");
			} else {
				Assert.That (allFiles.Length, Is.EqualTo (5), "Length");
			}
			Assert.That (File.ReadAllText (Path.Combine (directory, "A.txt")), Is.EqualTo ("ABCDEFGHIJKLMAAA"), "A.txt");
			Assert.That (File.ReadAllText (Path.Combine (directory, "B.txt")), Is.EqualTo ("ABCDEFGHIJKLMBBB"), "B.txt");
			Assert.That (File.ReadAllText (Path.Combine (directory, "C.framework/C.txt")), Is.EqualTo ("ABCDEFGHIJKLMCCC"), "C.txt");
			if (symlinks) {
				var linkToCPath = Path.Combine (directory, "C.framework/LinkToC.txt");
				Assert.That (File.ReadAllText (linkToCPath), Is.EqualTo ("ABCDEFGHIJKLMCCC"), "LinkToC.txt");
				Assert.That (PathUtils.IsSymlink (linkToCPath), Is.True, "LinkToC.txt - IsSymlink");
				Assert.That (PathUtils.GetSymlinkTarget (linkToCPath), Is.EqualTo ("C.txt"), "LinkToC.txt - IsSymlink target");

				var linkToNowherePath = Path.Combine (directory, "C.framework/LinkToNowhere.txt");
				Assert.Throws<FileNotFoundException> (() => File.ReadAllText (linkToNowherePath), "LinkToNowhere.txt");
				Assert.That (PathUtils.GetSymlinkTarget (linkToNowherePath), Is.EqualTo ("Nowhere.txt"), "LinkToNowhere.txt - IsSymlink target");
			}

			var manifest = @"<BindingAssembly>
	<NativeReference Name=""A.txt"">
		<ForceLoad></ForceLoad>
		<Frameworks></Frameworks>
		<IsCxx></IsCxx>
		<Kind></Kind>
		<LinkerFlags></LinkerFlags>
		<LinkWithSwiftSystemLibraries></LinkWithSwiftSystemLibraries>
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
		<LinkWithSwiftSystemLibraries></LinkWithSwiftSystemLibraries>
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
		<LinkWithSwiftSystemLibraries></LinkWithSwiftSystemLibraries>
		<NeedsGccExceptionHandling></NeedsGccExceptionHandling>
		<SmartLink></SmartLink>
		<WeakFrameworks></WeakFrameworks>
	</NativeReference>
</BindingAssembly>";
			Assert.That (File.ReadAllText (Path.Combine (directory, "manifest")), Is.EqualTo (manifest), "Manifest");
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
