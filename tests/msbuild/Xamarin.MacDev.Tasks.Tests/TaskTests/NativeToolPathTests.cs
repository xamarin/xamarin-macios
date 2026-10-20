// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;

using Microsoft.Build.Utilities;

using NUnit.Framework;

using Xamarin.Tests;
using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	[TestFixture]
	public class NativeToolPathTests : TestBase {
		static string CreateTool (string directory, string name, int exitCode = 0)
		{
			Configuration.IgnoreIfNotOnMacOS ();
			var toolDirectory = Path.Combine (directory, "custom tools");
			Directory.CreateDirectory (toolDirectory);
			var tool = Path.Combine (toolDirectory, name);
			File.WriteAllText (tool, $"#!/bin/sh\nprintf '%s\\n' \"$@\" > \"$0.arguments\"\nexit {exitCode}\n");
			if (OperatingSystem.IsMacOS ())
				File.SetUnixFileMode (tool, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);
			return tool;
		}

		[TestCase (ApplePlatform.iOS)]
		[TestCase (ApplePlatform.TVOS)]
		[TestCase (ApplePlatform.MacOSX)]
		[TestCase (ApplePlatform.MacCatalyst)]
		public void CompileNativeCodeUsesClangPath (ApplePlatform platform)
		{
			var directory = Cache.CreateTemporaryDirectory ();
			var source = Path.Combine (directory, "native source.mm");
			var output = Path.Combine (directory, "native output.o");
			File.WriteAllText (source, "");
			var info = new TaskItem (source);
			info.SetMetadata ("OutputFile", output);
			info.SetMetadata ("Arch", "arm64");

			var task = CreateTask<CompileNativeCode> ();
			task.ClangPath = CreateTool (directory, "custom-clang");
			task.CompileInfo = [info];
			task.MinimumOSVersion = "15.0";
			task.SdkRoot = Path.Combine (directory, "unused SDK");
			task.TargetFrameworkMoniker = TargetFramework.GetTargetFramework (platform).ToString ();

			ExecuteTask (task);

			var arguments = File.ReadAllLines (task.ClangPath + ".arguments");
			Assert.That (arguments [0], Is.EqualTo ("-g"), "No xcrun tool-name argument");
			Assert.That (arguments, Does.Contain (task.SdkRoot), "SDK path");
			Assert.That (arguments [^4..], Is.EqualTo (new [] { "-o", output, "-c", source }), "Compiler arguments");
			Assert.That (task.CompiledOutputFiles, Has.Length.EqualTo (1), "Compiled outputs");
			Assert.That (task.CompiledOutputFiles [0].ItemSpec, Is.EqualTo (output), "Output path");
		}

		[TestCase (0, 0)]
		[TestCase (1, 0)]
		[TestCase (0, 1)]
		public void LinkNativeCodeUsesToolPaths (int linkerExitCode, int derqExitCode)
		{
			var directory = Cache.CreateTemporaryDirectory ();
			var entitlements = Path.Combine (directory, "app entitlements.plist");
			var plist = new PDictionary {
				{ "get-task-allow", new PBoolean (true) },
			};
			plist.Save (entitlements);

			var task = CreateTask<LinkNativeCode> ();
			task.ClangPlusPlusPath = CreateTool (directory, "custom-clang++", linkerExitCode);
			task.DerqPath = CreateTool (directory, "custom-derq", derqExitCode);
			task.EntitlementsInExecutable = entitlements;
			task.MinimumOSVersion = "15.0";
			task.SdkRoot = Path.Combine (directory, "unused SDK");
			task.TargetArchitectures = "ARM64";
			task.TargetFrameworkMoniker = TargetFramework.DotNet_iOS_String;
			task.OutputFile = new TaskItem (Path.Combine (directory, "native executable"));

			ExecuteTask (task, linkerExitCode + derqExitCode);

			Assert.That (File.ReadAllLines (task.DerqPath + ".arguments"), Is.EqualTo (new [] {
				"query", "-f", "xml", "-i", entitlements, "-o", entitlements + ".der", "--raw",
			}), "DER conversion arguments");
			var arguments = File.ReadAllLines (task.ClangPlusPlusPath + ".arguments");
			Assert.That (arguments, Does.Not.Contain ("clang++"), "No xcrun tool-name argument");
			Assert.That (arguments, Does.Contain (task.SdkRoot), "SDK path");
			Assert.That (arguments, Does.Contain (entitlements), "XML entitlements");
			Assert.That (arguments, Does.Contain (entitlements + ".der"), "DER entitlements");
			Assert.That (arguments [^2..], Is.EqualTo (new [] { "-o", task.OutputFile.ItemSpec }), "Linker output");
		}

		[Test]
		public void CompileNativeCodeReportsToolFailure ()
		{
			var directory = Cache.CreateTemporaryDirectory ();
			var source = Path.Combine (directory, "source.mm");
			File.WriteAllText (source, "");
			var task = CreateTask<CompileNativeCode> ();
			task.ClangPath = CreateTool (directory, "failing-clang", 1);
			task.CompileInfo = [new TaskItem (source)];
			task.MinimumOSVersion = "15.0";
			task.SdkRoot = directory;
			task.TargetFrameworkMoniker = TargetFramework.DotNet_iOS_String;

			ExecuteTask (task, 1);
			Assert.That (task.ClangPath + ".arguments", Does.Exist, "Override was invoked");
		}

		[TestCase (0)]
		[TestCase (1)]
		public void InstallNameToolUsesToolPath (int exitCode)
		{
			var directory = Cache.CreateTemporaryDirectory ();
			var source = Path.Combine (directory, "source.dylib");
			var destination = Path.Combine (directory, "output", "library.dylib");
			File.WriteAllText (source, "native library");
			var library = new TaskItem (source);
			library.SetMetadata ("ReidentifiedPath", destination);
			library.SetMetadata ("UpdatedId", "@rpath/library.dylib");

			var task = CreateTask<InstallNameTool> ();
			task.InstallNameToolPath = CreateTool (directory, "custom-install-name-tool", exitCode);
			task.IntermediateNativeLibraryDir = Path.Combine (directory, "output");
			task.DynamicLibrary = [library];

			ExecuteTask (task, exitCode);

			Assert.That (File.ReadAllLines (task.InstallNameToolPath + ".arguments"), Is.EqualTo (new [] {
				"-id", "@rpath/library.dylib", destination + ".tmp",
			}), "Install name arguments");
			Assert.That (destination + ".tmp", Does.Not.Exist, "Temporary file cleaned up");
			if (exitCode == 0)
				Assert.That (File.ReadAllText (destination), Is.EqualTo ("native library"), "Reidentified output");
			else
				Assert.That (destination, Does.Not.Exist, "No output after failure");
		}

		[Test]
		public void InstallNameToolPathChangesInvalidateStamp ()
		{
			var directory = Cache.CreateTemporaryDirectory ();
			var source = Path.Combine (directory, "source.dylib");
			var destination = Path.Combine (directory, "output", "library.dylib");
			File.WriteAllText (source, "native library");
			var library = new TaskItem (source);
			library.SetMetadata ("ReidentifiedPath", destination);
			library.SetMetadata ("StampFile", Path.Combine (directory, "install-name-tool.cache"));
			library.SetMetadata ("UpdatedId", "@rpath/library.dylib");

			var task = CreateTask<InstallNameTool> ();
			task.InstallNameToolPath = CreateTool (directory, "first-tool");
			task.IntermediateNativeLibraryDir = Path.Combine (directory, "output");
			task.DynamicLibrary = [library];
			ExecuteTask (task);

			File.Delete (task.InstallNameToolPath + ".arguments");
			ExecuteTask (task);
			Assert.That (task.InstallNameToolPath + ".arguments", Does.Not.Exist, "Unchanged tool is skipped");

			task.InstallNameToolPath = CreateTool (directory, "second-tool");
			ExecuteTask (task);
			Assert.That (task.InstallNameToolPath + ".arguments", Does.Exist, "Changed tool is invoked");
		}
	}
}
