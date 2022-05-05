using System;
using System.Text;
using System.IO;

namespace Microsoft.MaciOS.Nnyeah.Tests {
	public class Compiler {
		const string kCompiler = "/Library/Frameworks/Mono.framework/Versions/Current/Commands/csc";

		public static string CompileText (string text, string outputFile, PlatformName platformName, bool isLibrary)
		{
			using var provider = new DisposableTempDirectory ();
			var outputCSFile = Path.Combine (provider.DirectoryPath, "LibraryFile.cs");
			using var writer = new StreamWriter (outputCSFile);
			writer.Write (text);
			writer.Close ();
			return Compile (outputFile, platformName, isLibrary, provider.DirectoryPath, outputCSFile);
		}

		public static string Compile (string outputFile, PlatformName platformName, bool isLibrary, string workingDirectory, params string[] sourceFiles)
		{
			var compilerArgs = BuildCompilerArgs (sourceFiles, outputFile, platformName, isLibrary);
			return ExecAndCollect.Run (kCompiler, compilerArgs, workingDirectory);
		}

		static string BuildCompilerArgs (string[] sourceFiles, string outputFile, PlatformName platformName,
			bool isLibrary)
		{
			var sb = new StringBuilder ();

			sb.Append ("/unsafe ").Append ("/out:").Append (outputFile);
			sb.Append (" /nostdlib+");
			AppendPlatformReference (sb, platformName, "mscorlib");
			AppendPlatformReference (sb, platformName, XamarinLibName (platformName));
			sb.Append (" /debug+ /debug:full /optimize-");
			sb.Append (" /out:").Append (outputFile);
			sb.Append (" /target:").Append (isLibrary ? "library" : "exe");

			foreach (var file in sourceFiles) {
				sb.Append (' ').Append (file);
			}

			return sb.ToString ();
		}

		static StringBuilder AppendPlatformReference (StringBuilder sb, PlatformName platformName, string libName)
		{
			return sb.Append (" /reference:").Append (PlatformLibPath (platformName, libName));
		}

		static string PlatformLibPath (PlatformName platformName, string libName)
		{
			return Path.Combine (PlatformLibDirectory (platformName), $"{libName}.dll");
		}

		static string PlatformLibDirectory (PlatformName platformName) =>
			platformName switch {
				PlatformName.macOS => "/Library/Frameworks/Xamarin.Mac.framework/Versions/Current/lib/mono/Xamarin.Mac/",
				PlatformName.iOS => "/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/mono/Xamarin.iOS",
				PlatformName.tvOS => "/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/mono/Xamarin.TVOS",
				PlatformName.watchOS => "/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/mono/Xamarin.WatchOS",
				_ => throw new NotImplementedException (),
			};

		static string XamarinLibName (PlatformName platformName) =>
			platformName switch {
				PlatformName.macOS => "Xamarin.Mac",
				PlatformName.iOS => "Xamarin.iOS",
				PlatformName.tvOS => "Xamarin.TVOS",
				PlatformName.watchOS => "Xamarin.WatchOS",
				_ => throw new NotImplementedException (),
			};
	}
}
