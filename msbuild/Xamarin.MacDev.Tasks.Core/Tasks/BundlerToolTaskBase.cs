using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.Localization.MSBuild;
using Xamarin.MacDev;
using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	// A class to express what's shared/common between mtouch and mmp.
	public abstract class BundlerToolTaskBase : XamarinToolTask {

		#region Inputs
		[Required]
		public string AppBundleDir { get; set; }

		public string ArchiveSymbols { get; set; }

		[Required]
		public bool Debug { get; set; }

		[Required]
		public bool EnableSGenConc { get; set; }

		public string ExtraArgs { get; set; }

		[Required]
		public string HttpClientHandler { get; set; }

		public string I18n { get; set; }

		public string IntermediateOutputPath { get; set; }

		public bool IsAppExtension { get; set; }

		[Required]
		public string LinkMode { get; set; }

		[Required]
		public ITaskItem MainAssembly { get; set; }

		[Required]
		public string MinimumOSVersion { get; set; }

		public ITaskItem [] NativeReferences { get; set; }

		// Note: This property is used by XVS in order to calculate the Mac-equivalent paths for the MainAssembly and possibly other properties.
		[Required]
		public string OutputPath { get; set; }

		[Required]
		public bool Profiling { get; set; }

		[Required]
		public ITaskItem [] References { get; set; }

		[Required]
		public string ResponseFilePath { get; set; }

		[Required]
		public string SdkRoot { get; set; }

		[Required]
		public string SdkVersion { get; set; }

		public int Verbosity { get; set; }

		[Required]
		public string XamarinSdkRoot { get; set; }
		#endregion

		protected override string GenerateFullPathToTool ()
		{
			if (!string.IsNullOrEmpty (ToolPath))
				return Path.Combine (ToolPath, ToolExe);

			return Path.Combine (XamarinSdkRoot, "bin", ToolExe);
		}

		// Returns command line arguments common to both mtouch and mmp
		protected CommandLineArgumentBuilder GenerateCommandLineArguments ()
		{
			var args = new CommandLineArgumentBuilder ();

			if (bool.TryParse (ArchiveSymbols?.Trim (), out var msym))
				args.AddLine ($"--msym={(msym ? "yes" : "no")}");

			if (Debug)
				args.AddLine ("--debug");

			if (EnableSGenConc)
				args.AddLine ("--sgen-conc");

			args.AddLine ("/target-framework:" + TargetFrameworkMoniker);

			args.AddLine (string.Format ("--http-message-handler={0}", HttpClientHandler));

			if (!string.IsNullOrEmpty (I18n))
				args.AddQuotedLine ($"--i18n={I18n}");

			if (!string.IsNullOrEmpty (IntermediateOutputPath)) {
				Directory.CreateDirectory (IntermediateOutputPath);

				args.AddQuotedLine ($"--cache={Path.GetFullPath (IntermediateOutputPath)}");
			}

			args.AddQuotedLine ($"--root-assembly={Path.GetFullPath (MainAssembly.ItemSpec)}");

			if (Profiling)
				args.AddLine ("--profiling");

			args.AddQuotedLine ($"--sdkroot={SdkRoot}");

			args.AddQuotedLine ($"--targetver={MinimumOSVersion}");

			var v = VerbosityUtils.Merge (ExtraArgs, (LoggerVerbosity) Verbosity);
			foreach (var arg in v)
				args.AddLine (arg);

			return args;
		}

		// Creates a response file for the given arguments, and returns the command line
		// with the response file and any arguments that can't go into the response file.
		protected string CreateResponseFile (CommandLineArgumentBuilder arguments, IList<string> nonResponseArguments)
		{
			// Generate a response file
			var responseFile = Path.GetFullPath (ResponseFilePath);

			if (File.Exists (responseFile))
				File.Delete (responseFile);

			try {
				using (var fs = File.Create (responseFile)) {
					using (var writer = new StreamWriter (fs))
						writer.Write (arguments);
				}
			} catch (Exception ex) {
				Log.LogWarning ("Failed to create response file '{0}': {1}", responseFile, ex);
			}

			// Some arguments can not safely go in the response file and are 
			// added separately. They must go _after_ the response file
			// as they may override options passed in the response file
			var actualArgs = new CommandLineArgumentBuilder ();

			actualArgs.AddQuoted ($"@{responseFile}");

			if (nonResponseArguments != null) {
				foreach (var arg in nonResponseArguments)
					actualArgs.AddQuoted (arg);
			}

			// Generate the command line
			return actualArgs.ToString ();
		}

		protected string ResolveXCFramework (string xcframework, string platformName, string variant, string architectures)
		{
			try {
				var plist = PDictionary.FromFile (Path.Combine (xcframework, "Info.plist"));
				var dir = ResolveXCFramework (plist, platformName, variant, architectures);
				if (!String.IsNullOrEmpty (dir))
					return Path.Combine (xcframework, dir);

				// either the format was incorrect or we could not find a matching framework
				// note: last part is not translated since it match the (non-translated) keys inside the `Info.plist`
				var msg = (dir == null) ? MSBStrings.E0174 : MSBStrings.E0175 + $" SupportedPlatform: '{platformName}', SupportedPlatformVariant: '{variant}', SupportedArchitectures: '{architectures}'.";
				Log.LogError (msg, xcframework);
			}
			catch (Exception) {
				Log.LogError (MSBStrings.E0174, xcframework);
			}
			return null;
		}

		internal static string ResolveXCFramework (PDictionary plist, string platformName, string variant, string architectures)
		{
			// plist structure https://github.com/spouliot/xcframework#infoplist
			var bundle_package_type = (PString) plist ["CFBundlePackageType"];
			if (bundle_package_type?.Value != "XFWK")
				return null;
			var available_libraries = plist.GetArray ("AvailableLibraries");
			if ((available_libraries == null) || (available_libraries.Count == 0))
				return null;

			var platform = platformName.ToLowerInvariant ();
			var archs = architectures.Split (new char [] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (PDictionary item in available_libraries) {
				var supported_platform = (PString) item ["SupportedPlatform"];
				if (supported_platform.Value != platform)
					continue;
				// optional key
				var supported_platform_variant = (PString) item ["SupportedPlatformVariant"];
				if (supported_platform_variant?.Value != variant)
					continue;
				var supported_architectures = (PArray) item ["SupportedArchitectures"];
				// each architecture we request must be present in the xcframework
				// but extra architectures in the xcframework are perfectly fine
				foreach (var arch in archs) {
					bool found = false;
					foreach (PString xarch in supported_architectures) {
						found = String.Compare (arch, xarch.Value, StringComparison.OrdinalIgnoreCase) == 0;
						if (found)
							break;
					}
					if (!found)
						return String.Empty;
				}
				var library_path = (PString) item ["LibraryPath"];
				var library_identifier = (PString) item ["LibraryIdentifier"];
				return Path.Combine (library_identifier, library_path);
			}
			return String.Empty;
		}
	}
}
