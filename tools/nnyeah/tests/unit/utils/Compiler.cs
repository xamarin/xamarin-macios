using System;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;
using Xamarin;
using Xamarin.Utils;
using System.Collections.Generic;
using Xamarin.Tests;

namespace Microsoft.MaciOS.Nnyeah.Tests {

	public enum PlatformName {
		None, // desktop managed executable
		macOS, // Xamarin.Mac app
		iOS,
		watchOS,
		tvOS,
	}

	public class Compiler {
		const string MonoCompiler = "/Library/Frameworks/Mono.framework/Versions/Current/Commands/csc";

		public static async Task<string> CompileText (string text, string outputFile, PlatformName platformName, bool isLibrary)
		{
			var dir = Cache.CreateTemporaryDirectory ("CompileText");
			var outputCSFile = Path.Combine (dir, "LibraryFile.cs");
			File.WriteAllText (outputCSFile, text);
			return await Compile (outputFile, platformName, isLibrary, dir, outputCSFile);
		}

		public static async Task<string> Compile (string outputFile, PlatformName platformName, bool isLibrary, string workingDirectory, params string [] sourceFiles)
		{
			var compilerArgs = BuildCompilerArgs (sourceFiles, outputFile, platformName, isLibrary);
			Execution execution = await Execution.RunAsync (MonoCompiler, compilerArgs, mergeOutput: true, workingDirectory: workingDirectory);
			return execution!.StandardOutput?.ToString ()!;
		}

		static List<string> BuildCompilerArgs (string [] sourceFiles, string outputFile, PlatformName platformName,
			bool isLibrary)
		{
			var args = new List<string> ();

			args.Add ("/unsafe");
			args.Add ("/nostdlib+");
			AppendPlatformReference (args, platformName, "mscorlib");
			AppendPlatformReference (args, platformName, XamarinLibName (platformName));
			args.Add ("/debug+");
			args.Add ("/debug:full");
			args.Add ("/optimize-");
			args.Add ("/out:" + outputFile);
			args.Add ("/target:" + (isLibrary ? "library" : "exe"));

			foreach (var file in sourceFiles) {
				args.Add (file);
			}

			return args;
		}

		static void AppendPlatformReference (List<string> args, PlatformName platformName, string libName)
		{
			args.Add ("/reference:" + Path.Combine (XamarinPlatformLibDirectory (platformName), libName + ".dll"));
		}

		public static string XamarinPlatformLibDirectory (PlatformName platformName)
		{
			return new FileInfo (XamarinPlatformLibraryPath (platformName)).DirectoryName!;
		}

		public static string XamarinLibName (PlatformName platformName) =>
			Path.GetFileNameWithoutExtension (XamarinPlatformLibraryPath (platformName)).ToString ();

		public static string XamarinPlatformLibraryPath (PlatformName platformName) =>
			platformName switch {
				PlatformName.macOS => Configuration.XamarinMacFullDll,
				PlatformName.iOS => Configuration.XamarinIOSDll,
				PlatformName.tvOS => Configuration.XamarinTVOSDll,
				PlatformName.watchOS => Configuration.XamarinWatchOSDll,
				_ => throw new NotImplementedException (platformName.ToString ()),
			};

		public static string MicrosoftPlatformLibDirectory (PlatformName platformName) =>
			Configuration.GetRefDirectory (PlatformNameToApplePlatform (platformName));

		public static string MicrosoftLibName (PlatformName platformName) =>
			Path.GetFileNameWithoutExtension (Configuration.GetBaseLibraryName (PlatformNameToApplePlatform (platformName), true));

		public static string MicrosoftPlatformLibraryPath (PlatformName platformName) =>
						Path.Combine (MicrosoftPlatformLibDirectory (platformName),
							Configuration.GetBaseLibraryName (PlatformNameToApplePlatform (platformName), true));

		public static ApplePlatform PlatformNameToApplePlatform (PlatformName platformName) =>
			platformName switch {
				PlatformName.macOS => ApplePlatform.MacOSX,
				PlatformName.iOS => ApplePlatform.iOS,
				PlatformName.tvOS => ApplePlatform.TVOS,
				PlatformName.watchOS => ApplePlatform.WatchOS,
				_ => throw new NotImplementedException (platformName.ToString ()),
			};
	}
}
