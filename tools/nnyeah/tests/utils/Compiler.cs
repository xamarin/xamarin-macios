using System;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;
using Xamarin;
using Xamarin.Utils;
using System.Collections.Generic;

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
		const string buildDirectory = "../../../../../../_build";

		public static async Task<string> CompileText (string text, string outputFile, PlatformName platformName, bool isLibrary)
		{
			var dir = Cache.CreateTemporaryDirectory ();
			var outputCSFile = Path.Combine (dir, "LibraryFile.cs");
			File.WriteAllText (outputCSFile, text);
			return await Compile (outputFile, platformName, isLibrary, dir, outputCSFile);
		}

		public static async Task<string> Compile (string outputFile, PlatformName platformName, bool isLibrary, string workingDirectory, params string[] sourceFiles)
		{
			var compilerArgs = BuildCompilerArgs (sourceFiles, outputFile, platformName, isLibrary);
			Execution execution = await Execution.RunAsync(MonoCompiler, compilerArgs, mergeOutput: true, workingDirectory: workingDirectory);
			return execution!.StandardOutput?.ToString()!;
		}

		static List<string> BuildCompilerArgs (string[] sourceFiles, string outputFile, PlatformName platformName,
			bool isLibrary)
		{
			var args = new List<string>();

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
			args.Add("/reference:" + PlatformLibPath (platformName, libName));
		}

		static string PlatformLibPath (PlatformName platformName, string libName)
		{
			return Path.Combine (XamarinPlatformLibDirectory (platformName), $"{libName}.dll");
		}

		public static string XamarinPlatformLibDirectory (PlatformName platformName) =>
			platformName switch {
				PlatformName.macOS => "/Library/Frameworks/Xamarin.Mac.framework/Versions/Current/lib/mono/Xamarin.Mac/",
				PlatformName.iOS => "/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/mono/Xamarin.iOS",
				PlatformName.tvOS => "/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/mono/Xamarin.TVOS",
				PlatformName.watchOS => "/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/mono/Xamarin.WatchOS",
				_ => throw new NotImplementedException (),
			};

		public static string XamarinLibName (PlatformName platformName) =>
			platformName switch {
				PlatformName.macOS => "Xamarin.Mac",
				PlatformName.iOS => "Xamarin.iOS",
				PlatformName.tvOS => "Xamarin.TVOS",
				PlatformName.watchOS => "Xamarin.WatchOS",
				_ => throw new NotImplementedException (),
			};

		public static string XamarinPlatformLibraryPath (PlatformName platformName) =>
			Path.Combine (XamarinPlatformLibDirectory (platformName), $"{XamarinLibName (platformName)}.dll");

		public static string MicrosoftPlatformLibDirectory (PlatformName platformName) =>
			platformName switch {
				PlatformName.macOS => Path.Combine (buildDirectory, "Microsoft.macOS.Runtime.osx-arm64/runtimes/osx-arm64/lib/net6.0"),
				PlatformName.iOS => Path.Combine (buildDirectory, "Microsoft.iOS.Runtime.ios-arm/runtimes/ios-arm/lib/net6.0"),
				PlatformName.tvOS => Path.Combine (buildDirectory, "Microsoft.tvOS.Runtime.tvossimulator-x64/runtimes/tvossimulator-x64/lib/net6.0"),
				PlatformName.watchOS => throw new NotImplementedException (),
				_ => throw new NotImplementedException (),
			};

		public static string MicrosoftLibName (PlatformName platformName) =>
			platformName switch {
				PlatformName.macOS => "Microsoft.macOS",
				PlatformName.iOS => "Microsoft.iOS",
				PlatformName.tvOS => "Xamarin.tvOS",
				PlatformName.watchOS => throw new NotImplementedException (),
				_ => throw new NotImplementedException (),
			};

		public static string MicrosoftPlatformLibraryPath (PlatformName platformName) =>
						Path.Combine (MicrosoftPlatformLibDirectory (platformName), $"{MicrosoftLibName (platformName)}.dll");

	}
}
