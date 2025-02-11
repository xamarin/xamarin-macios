using System.CommandLine;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Macios.Transformer;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Templates;
using Xamarin.Utils;
using static System.Console;

public class Program {
	static internal readonly LoggingLevelSwitch LogLevelSwitch = new (LogEventLevel.Information);
	public static ILogger logger = Log.ForContext<Program> ();

	public static int Main (string [] args)
	{
		// Create options
		var rspOption = new Option<string []> (
			new [] { "--response-file", "--rsp" },
			"Path to the RSP file"
		);

		var destinationOption = new Option<string> (
			new [] { "--destination", "-d" },
			"Path to the destination directory"
		);

		var forceOption = new Option<bool> (
			new [] { "--force", "-f" },
			"Erase the destination directory if it exists"
		);

		var workingDirectoryOption = new Option<string> (
			new [] { "--working-directory", "-w" },
			"Absolute path to an existing working directory"
		);

		var sdkPathOption = new Option<string> (
			new [] { "--sdk", "-s" },
			"Absolute path to the sdk directory"
		);

		var verbosityOption = new Option<Verbosity> (["--verbosity", "-v"],
			getDefaultValue: () => Verbosity.Normal) {
			IsRequired = false,
			Arity = ArgumentArity.ZeroOrOne,
			Description = "Set the verbosity level"
		};

		// Create root command and add options
		var rootCmd = new RootCommand ("command to convert outdated bindings to be rgen compatible") {
			rspOption,
			destinationOption,
			forceOption,
			workingDirectoryOption,
			sdkPathOption,
			verbosityOption
		};

		// If no arguments, show help and exit
		if (args.Length == 0) {
			rootCmd.InvokeAsync (["--help"]).Wait ();
			return 0;
		}

		// Set handler for parsing and executing
		rootCmd.SetHandler (async (rspPlatformPaths, destPath, workingDirectory, sdkPath, force, verbosity) => {
			WriteLine (
				$"Microsoft.Macios.Transformer v{typeof (Program).Assembly.GetName ().Version}, (c) Microsoft Corporation. All rights reserved.\n");

			// Convert local to absolute, expand ~
			workingDirectory = ToAbsolutePath (workingDirectory);
			destPath = ToAbsolutePath (destPath);
			sdkPath = ToAbsolutePath (sdkPath);

			ValidateRsp (rspPlatformPaths, out var rspFiles);
			ValidateSdk (sdkPath);
			ValidateWorkingDirectory (workingDirectory);
			ValidateVerbosity (verbosity);
			PrepareDestination (destPath, force);

			// logging options
			Log.Logger = new LoggerConfiguration ()
				.MinimumLevel.ControlledBy (LogLevelSwitch)
				.Enrich.WithThreadName ()
				.Enrich.WithThreadId ()
				.Enrich.FromLogContext ()
				.WriteTo.Console (new ExpressionTemplate (
					"[{@t:HH:mm:ss} {@l:u3} {Substring(SourceContext, LastIndexOf(SourceContext, '.') + 1)} (Thread: {ThreadId})] {@m}\n"))
				.CreateLogger ();

			await Transformer.Execute (
				destinationDirectory: destPath,
				rspFiles: rspFiles,
				workingDirectory: workingDirectory,
				sdkDirectory: sdkPath
			);
		},
			rspOption, destinationOption, workingDirectoryOption, sdkPathOption, forceOption, verbosityOption
		);

		// Invoke command
		return rootCmd.InvokeAsync (args).Result;
	}

	static string ToAbsolutePath (string path)
	{
		var absolutePath = path.Replace ("~", Environment.GetFolderPath (Environment.SpecialFolder.UserProfile));
		if (!Path.IsPathRooted (absolutePath))
			absolutePath = Path.GetFullPath (absolutePath);
		return absolutePath;
	}

	static void ValidateRsp (string [] paths, out List<(ApplePlatform Platform, string RspPath)> rspFiles)
	{
		rspFiles = [];
		// loop over all the strings, split them by the ':' and retrieve the platform and path. Then 
		// validate the path
		foreach (var cmdPath in paths) {
			var parts = cmdPath.Split (':');
			if (parts.Length != 2)
				throw new Exception ("Invalid RSP format. Expected platform:path");
			ApplePlatform platform;
			var rspPath = ToAbsolutePath (parts [1]);
			switch (parts [0]) {
			case "ios":
				platform = ApplePlatform.iOS;
				break;
			case "tvos":
				platform = ApplePlatform.TVOS;
				break;
			case "macos":
				platform = ApplePlatform.MacOSX;
				break;
			case "maccatalys":
				platform = ApplePlatform.MacCatalyst;
				break;
			default:
				platform = ApplePlatform.None;
				break;
			}

			if (platform == ApplePlatform.None)
				throw new Exception ("Invalid platform in RSP file.");

			if (string.IsNullOrWhiteSpace (rspPath) || !File.Exists (rspPath))
				throw new FileNotFoundException ($"RSP '{rspPath}' file not found.");
			if ((File.GetAttributes (rspPath) & FileAttributes.Directory) == FileAttributes.Directory)
				throw new Exception ($"RSP {rspPath} is a directory.");
			using var stream = File.OpenRead (rspPath); // validate readability
			logger.Debug ("Adding RSP file {RspPath} for platform {Platform}", rspPath, platform);
			rspFiles.Add ((platform, rspPath));
		}
	}

	static void ValidateWorkingDirectory (string path)
	{
		if (string.IsNullOrWhiteSpace (path) || !Directory.Exists (path))
			throw new DirectoryNotFoundException ("Working directory does not exist.");
	}

	static void ValidateSdk (string path)
	{
		if (string.IsNullOrWhiteSpace (path) || !Directory.Exists (path))
			throw new DirectoryNotFoundException ("Working directory does not exist.");
	}

	static void ValidateVerbosity (Verbosity verbosity)
	{
		LogLevelSwitch.MinimumLevel = verbosity switch {
			Verbosity.Quiet => LogEventLevel.Error,
			Verbosity.Minimal => LogEventLevel.Error,
			Verbosity.Normal => LogEventLevel.Information,
			Verbosity.Detailed => LogEventLevel.Verbose,
			Verbosity.Diagnostic => LogEventLevel.Verbose,
			_ => LogEventLevel.Information,
		};
	}

	static void PrepareDestination (string path, bool force)
	{
		if (Directory.Exists (path)) {
			if (force)
				Directory.Delete (path, true);
		}

		Directory.CreateDirectory (path);
	}
}
